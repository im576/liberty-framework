# End-to-end tests of tools/autopilot/Run-Scenario.ps1 and Run-Suite.ps1 against the simulated game
# (tools/tests/SimulatedGame.psm1): every way a scenario used to pass without passing must now fail.
$simModule = Join-Path $script:TestRoot 'SimulatedGame.psm1'
Import-Module $simModule -Force
Import-Module (Join-Path $script:RepoRoot 'tools/autopilot/AutopilotLogic.psm1') -Force
$runScenario = Join-Path $script:RepoRoot 'tools/autopilot/Run-Scenario.ps1'
$runSuite = Join-Path $script:RepoRoot 'tools/autopilot/Run-Suite.ps1'
$simRoot = Join-Path $script:Scratch 'autopilot-sim'
$scenarios = Join-Path $simRoot 'scenarios'
$runs = Join-Path $simRoot 'runs'
New-Item -ItemType Directory -Force -Path $scenarios, $runs | Out-Null

$world = @{
    knownCommands = @('god', 'wanted', 'clear', 'wait_engine')
    commands = @{
        'spawn 1 5 0' = @{ reply = 'spawned 1'; log = @('autopilot_spawned count=1') }
        'spawn_silent' = @{ reply = 'ok' }
        'boom' = @{ reply = 'ok'; crash = $true }
        'oops' = @{ reply = 'ok'; log = @('[ERROR] something broke') }
        'selftest' = @{ reply = 'started'; log = @('selftest_done passed=12 failed=0') }
    }
}

function Invoke-SimScenario([string] $Name, [string[]] $Lines, [hashtable] $World, [string[]] $ExistingLog) {
    Initialize-SimulatedGame (Join-Path $simRoot "state-$Name") $World $ExistingLog
    $file = Join-Path $scenarios "$Name.txt"
    [IO.File]::WriteAllLines($file, $Lines)
    $output = & $runScenario -GameDirectory $simRoot -Scenario $file -OutputDirectory $runs -AutopilotModule $simModule 6>&1 | Out-String
    $read = Read-ScenarioResult $output
    $json = if ($read.Path -and (Test-Path -LiteralPath $read.Path)) { Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json } else { $null }
    return @{ Status = $read.Status; Json = $json; Output = $output }
}

$r = Invoke-SimScenario 'good' @('# ok', 'god on', 'spawn 1 5 0', 'expect "autopilot_spawned count=1" 2', 'shot ped', 'selftest', 'expect "selftest_done passed=\d+ failed=0" 2') $world
Test-That 'sim: a clean scenario passes' ($r.Status -eq 'PASS') $r.Output
Test-That 'sim: result.json lists the screenshot' ($r.Json -and @($r.Json.screenshots) -contains 'ped.png')

$r = Invoke-SimScenario 'stale' @('spawn_silent', 'expect "autopilot_spawned count=1" 1') $world @('autopilot_spawned count=1')
Test-That 'sim: a line from before the command does not satisfy expect' ($r.Status -eq 'FAIL') $r.Output

$r = Invoke-SimScenario 'echo' @('spawn_silent', 'expect "spawn_silent" 1') $world
Test-That 'sim: the command echo does not satisfy expect' ($r.Status -eq 'FAIL') $r.Output

$r = Invoke-SimScenario 'unknown' @('god on', 'expcet something') $world
Test-That 'sim: an unknown command (typo) fails' ($r.Status -eq 'FAIL') $r.Output

$r = Invoke-SimScenario 'noshot' @('god on', 'shot view') (@{ knownCommands = @('god'); screenshotFails = $true })
Test-That 'sim: a missing screenshot fails' ($r.Status -eq 'FAIL') $r.Output

$r = Invoke-SimScenario 'crash-last' @('god on', 'boom') $world
Test-That 'sim: a crash on the last step is CRASH, not PASS' ($r.Status -eq 'CRASH') $r.Output

$r = Invoke-SimScenario 'crash-mid' @('boom', 'god on', 'expect "anything" 1') $world
Test-That 'sim: a crash mid-scenario is CRASH' ($r.Status -eq 'CRASH') $r.Output

$r = Invoke-SimScenario 'errors' @('oops') $world
Test-That 'sim: [ERROR] log lines turn a pass into NEEDS-REVIEW' ($r.Status -eq 'NEEDS-REVIEW') $r.Output

$r = Invoke-SimScenario 'empty' @('# only comments', '') $world
Test-That 'sim: a scenario that executes nothing is ERROR' ($r.Status -eq 'ERROR') $r.Output

$r = Invoke-SimScenario 'nolaunch' @('god on') (@{ running = $false; launchFails = $true })
Test-That 'sim: a failed launch is ERROR with a result file' ($r.Status -eq 'ERROR' -and $r.Json.runnerError -match 'did not reach the engine') $r.Output

$r = Invoke-SimScenario 'badexpect' @('god on', 'expect "(unclosed" 1') $world
Test-That 'sim: an unparseable expect fails' ($r.Status -eq 'FAIL') $r.Output

# The suite: statuses from result files, continues after a broken scenario, exit code 1 unless all pass.
Initialize-SimulatedGame (Join-Path $simRoot 'state-suite') $world @()
$suiteScenarios = Join-Path $simRoot 'suite'
New-Item -ItemType Directory -Force -Path $suiteScenarios | Out-Null
[IO.File]::WriteAllLines((Join-Path $suiteScenarios 'a-good.txt'), @('god on'))
[IO.File]::WriteAllLines((Join-Path $suiteScenarios 'b-empty.txt'), @('# nothing'))
[IO.File]::WriteAllLines((Join-Path $suiteScenarios 'c-good.txt'), @('selftest', 'expect "selftest_done" 2'))
$rows = & $runSuite -GameDirectory $simRoot -OutputDirectory $runs -ScenarioDirectory $suiteScenarios -AutopilotModule $simModule -Scenarios @('a-good', 'b-empty', 'missing', 'c-good') -PassThru 6>$null
$suiteExit = $LASTEXITCODE
$byName = @{}; foreach ($row in @($rows)) { $byName[$row.Scenario] = $row.Result }
Test-That 'suite: good scenarios pass' ($byName['a-good'] -eq 'PASS' -and $byName['c-good'] -eq 'PASS') ($byName | Out-String)
Test-That 'suite: an empty scenario is ERROR and the suite continues' ($byName['b-empty'] -eq 'ERROR' -and $byName.ContainsKey('c-good'))
Test-That 'suite: a missing scenario file is ERROR' ($byName['missing'] -eq 'ERROR')
Test-That 'suite: exit code 1 when anything is not PASS' ($suiteExit -eq 1) "exit $suiteExit"
$env:LIBERTY_SIM_STATE = $null

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

$r = Invoke-SimScenario 'marked' @('mark', 'spawn 1 5 0', 'spawn_silent', 'expectmarked "autopilot_spawned count=1" 1') $world
Test-That 'sim: marked sequence accepts event between commands' ($r.Status -eq 'PASS') $r.Output

$r = Invoke-SimScenario 'marked-stale' @('mark', 'spawn_silent', 'expectmarked "autopilot_spawned count=1" 1') $world @('autopilot_spawned count=1')
Test-That 'sim: marked sequence rejects event before mark' ($r.Status -eq 'FAIL') $r.Output

$r = Invoke-SimScenario 'unmarked' @('spawn 1 5 0', 'expectmarked "autopilot_spawned count=1" 1') $world
Test-That 'sim: expectmarked requires mark' ($r.Status -eq 'FAIL') $r.Output

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

Test-That 'sim: a failed launch is marked GAME-UNAVAILABLE (verify-local skips the rest of the run)' ($r.Json.runnerError -like 'GAME-UNAVAILABLE*') $r.Output

$r = Invoke-SimScenario 'noaudio' @('god on') (@{ running = $false; noAudio = $true })
Test-That 'sim: no audio output device fails at once as GAME-UNAVAILABLE' ($r.Status -eq 'ERROR' -and $r.Json.runnerError -like 'GAME-UNAVAILABLE*audio*') $r.Output

$started = Get-Date
$r = Invoke-SimScenario 'frozen' @('god on', 'expect "never_logged" 60', 'god on', 'expect "never_logged" 60') (@{ knownCommands = @('god'); frozen = $true })
$elapsed = ((Get-Date) - $started).TotalSeconds
Test-That 'sim: a frozen game ends the scenario at the first wait instead of waiting out every expect' (
    $r.Status -eq 'ERROR' -and $r.Json.runnerError -like 'GAME-FROZEN*' -and $r.Json.steps -eq 2 -and $elapsed -lt 30) "$($r.Status) $([int]$elapsed)s $($r.Output)"

Test-That 'mode line: {quick:A|B} picks A in quick, B in full' ((ConvertTo-ModeLine 'cycle-deaths {quick:5|25}' $true) -eq 'cycle-deaths 5' -and (ConvertTo-ModeLine 'cycle-deaths {quick:5|25}' $false) -eq 'cycle-deaths 25')
Test-That 'mode line: @full lines are skipped in quick runs, @quick lines in full runs' ($null -eq (ConvertTo-ModeLine '@full wait 1000' $true) -and $null -eq (ConvertTo-ModeLine '@quick god on' $false) -and (ConvertTo-ModeLine '@full wait 1000' $false) -eq 'wait 1000')
Initialize-SimulatedGame (Join-Path $simRoot 'state-quick') $world @()
$quickFile = Join-Path $scenarios 'quickmode.txt'
[IO.File]::WriteAllLines($quickFile, @('@full expect "never_logged" 1', '@quick god on', 'spawn {quick:1|9} 5 0', 'expect "autopilot_spawned count=1" 2'))
$output = & $runScenario -GameDirectory $simRoot -Scenario $quickFile -OutputDirectory $runs -AutopilotModule $simModule -Quick 6>&1 | Out-String
$read = Read-ScenarioResult $output; $json = Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json
Test-That 'quick run: short variants run, full-only lines skipped, result marked quick' ($read.Status -eq 'PASS' -and $json.mode -eq 'quick' -and $json.steps -eq 3) $output
$r = Invoke-SimScenario 'stopfirst' @('god on', 'expect "never_logged" 1', 'god on', 'god on') $world
Test-That 'without -StopOnFailure every step still runs' ($r.Json.steps -eq 4) $r.Output
Initialize-SimulatedGame (Join-Path $simRoot 'state-stop') $world @()
$stopFile = Join-Path $scenarios 'stopfirst2.txt'
[IO.File]::WriteAllLines($stopFile, @('god on', 'expect "never_logged" 1', 'god on', 'god on'))
$output = & $runScenario -GameDirectory $simRoot -Scenario $stopFile -OutputDirectory $runs -AutopilotModule $simModule -StopOnFailure 6>&1 | Out-String
$read = Read-ScenarioResult $output; $json = Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json
Test-That '-StopOnFailure ends the scenario at its first failed step' ($read.Status -eq 'FAIL' -and $json.steps -eq 2) $output

$r = Invoke-SimScenario 'badexpect' @('god on', 'expect "(unclosed" 1') $world
Test-That 'sim: an unparseable expect fails' ($r.Status -eq 'FAIL') $r.Output

$r = Invoke-SimScenario 'gpumem' @('god on', 'gpumem s1_test_day', 'gpumem s1_test_night') (@{ knownCommands = @('god'); memory = @{ gpuDedicatedMB = 1234.5; privateMB = 2000 } })
$reportDir = if ($r.Json) { Get-ChildItem -LiteralPath $runs -Directory -Filter 'gpumem-*' | Sort-Object LastWriteTime -Descending | Select-Object -First 1 } else { $null }
$measurements = if ($reportDir -and (Test-Path (Join-Path $reportDir.FullName 'measurements.json'))) { @(Get-Content -LiteralPath (Join-Path $reportDir.FullName 'measurements.json') -Raw | ConvertFrom-Json) } else { @() }
Test-That 'sim: gpumem passes and writes both labelled measurements' ($r.Status -eq 'PASS' -and $measurements.Count -eq 2 -and $measurements[0].label -eq 's1_test_day' -and $measurements[0].gpuDedicatedMB -eq 1234.5) $r.Output
$r = Invoke-SimScenario 'gpumem-nolabel' @('god on', 'gpumem') (@{ knownCommands = @('god') })
Test-That 'sim: gpumem without a label fails' ($r.Status -eq 'FAIL') $r.Output
$r = Invoke-SimScenario 'gpumem-nogpu' @('god on', 'gpumem x') (@{ knownCommands = @('god'); memory = @{ gpuDedicatedMB = $null } })
Test-That 'sim: gpumem fails when the GPU counters do not list the process (never a silent 0 MB)' ($r.Status -eq 'FAIL') $r.Output

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

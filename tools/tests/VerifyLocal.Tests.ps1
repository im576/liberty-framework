# Tests of tools/verify-local.ps1 and tools/local/VerifyLocal.psm1 without a game: the real orchestration runs against
# simulated tools (small PowerShell commands), the simulated game (SimulatedGame.psm1) and a local bare Git remote.
$verifyLocal = Join-Path $script:RepoRoot (Join-Path 'tools' 'verify-local.ps1')
Import-Module (Join-Path $script:RepoRoot (Join-Path 'tools' (Join-Path 'local' 'VerifyLocal.psm1'))) -Force
Import-Module (Join-Path $script:TestRoot 'SimulatedGame.psm1') -Force

# ---- Pure helpers
Test-That 'argument: plain value unquoted' ((ConvertTo-CommandArgument 'selftest') -eq 'selftest')
Test-That 'argument: spaces quoted' ((ConvertTo-CommandArgument 'C:\Program Files\GTA IV') -eq '"C:\Program Files\GTA IV"')
Test-That 'argument: trailing backslash doubled inside quotes' ((ConvertTo-CommandArgument 'D:\Steam Library\') -eq '"D:\Steam Library\\"')
Test-That 'argument: embedded quote escaped' ((ConvertTo-CommandArgument 'say "hi"') -eq '"say \"hi\""')
Test-That 'argument: empty string kept' ((ConvertTo-CommandArgument '') -eq '""')
$json = '{ "status": "ok", "compiled": { "textureFormat": "DXT5", "textureLevels": 6, "textureQuality": { "psnrAlphaDb": 31.5 } } }' | ConvertFrom-Json
$expect = '{ "status": "ok", "compiled.textureFormat": "DXT5", "compiled.textureLevels": 6, "compiled.textureQuality.psnrAlphaDb": "present" }' | ConvertFrom-Json
Test-That 'json expectations: all match' (@(Test-JsonExpectations $json $expect).Count -eq 0)
$expect = '{ "compiled.textureLevels": 7, "compiled.missing": "present" }' | ConvertFrom-Json
Test-That 'json expectations: wrong value and missing field reported' (@(Test-JsonExpectations $json $expect).Count -eq 2)

# ---- The real queue: ordering and selection
$queue = Read-CheckQueue (Join-Path $script:RepoRoot (Join-Path 'tests' (Join-Path 'local' 'checks.json')))
$order = @(Select-Checks $queue @() @() $false $false | ForEach-Object { $_.id })
$first = { param($predicate) for ($i = 0; $i -lt $order.Count; $i++) { $c = $queue.checks | Where-Object { $_.id -eq $order[$i] }; if (& $predicate $c) { return $i } }; return -1 }
$last = { param($predicate) for ($i = $order.Count - 1; $i -ge 0; $i--) { $c = $queue.checks | Where-Object { $_.id -eq $order[$i] }; if (& $predicate $c) { return $i } }; return -1 }
$install = [array]::IndexOf($order, 'LOOP-package-install')
Test-That 'queue order: package-install after every offline tool and probe' ((& $last { param($c) ($c.kind -eq 'pc-offline' -and $c.run.tool -notin 'package-install', 'content-report') -or $c.kind -eq 'probe' }) -lt $install)
Test-That 'queue order: scenarios and content reports after package-install' ((& $first { param($c) $c.kind -eq 'scenario' -or $c.run.tool -eq 'content-report' }) -gt $install)
Test-That 'queue order: manual checks last' ((& $first { param($c) $c.kind -eq 'manual' }) -gt (& $last { param($c) $c.kind -ne 'manual' }))
$smoke = @(Select-Checks $queue @() @() $true $false | ForEach-Object { $_.id })
Test-That 'smoke: build, package-install, SDK self-test' (($smoke -join ',') -eq 'LOOP-build,LOOP-package-install,SDK-selftest') ($smoke -join ',')
$only = @(Select-Checks $queue @('T027-raycast') @() $false $false | ForEach-Object { $_.id })
Test-That 'only a scenario: package-install is added before it' (($only -join ',') -eq 'LOOP-package-install,T027-raycast') ($only -join ',')
$threw = $false; try { Select-Checks $queue @('NO-such-check') @() $false $false | Out-Null } catch { $threw = $true }
Test-That 'only an unknown id: refused' $threw
$only = @(Select-Checks $queue @('T027-raycast-objects') @() $false $false | ForEach-Object { $_.id })
Test-That 'only a scenario that needs a probe: the probe and package-install run first' (($only -join ',') -eq 'PROBE-collision,LOOP-package-install,T027-raycast-objects') ($only -join ',')
$only = @(Select-Checks $queue @() @('scenario') $false $false | ForEach-Object { $_.id })
Test-That 'kind scenario: a needed probe is still added' ($only -contains 'PROBE-collision') ($only -join ',')

# ---- A full simulated run
$root = Join-Path $script:Scratch 'verify-local'
New-Item -ItemType Directory -Force -Path (Join-Path $root 'scenarios') | Out-Null
$remote = Join-Path $root 'remote.git'
& git init -q --bare $remote
[IO.File]::WriteAllLines((Join-Path $root (Join-Path 'scenarios' 'good.txt')), @('selftest', 'expect "selftest_done passed=\d+ failed=0" 2', 'shot view'))
[IO.File]::WriteAllLines((Join-Path $root (Join-Path 'scenarios' 'crashy.txt')), @('god on', 'boom'))
# A scenario that spawns what a probe of the same run found ({probe:<id>:<field>}; the simulated probe reports "simulated").
[IO.File]::WriteAllLines((Join-Path $root (Join-Path 'scenarios' 'probed.txt')), @('spawnprop {probe:T-probe:probe} 3 0', 'expect "autopilot_prop handle=\d+ model=simulated" 2'))
foreach ($asset in 'good_asset', 'bad_asset') {
    $folder = Join-Path $root (Join-Path 'reports' (Join-Path 'content' $asset))
    New-Item -ItemType Directory -Force -Path $folder | Out-Null
}
[IO.File]::WriteAllText((Join-Path $root 'reports/content/good_asset/report.json'), '{ "status": "ok", "compiled": { "textureFormat": "DXT1" } }')
[IO.File]::WriteAllText((Join-Path $root 'reports/content/bad_asset/report.json'), '{ "status": "error" }')

function New-TestQueue([object[]] $checks) {
    $q = @{ schemaVersion = 1; sessions = @(@{ id = 'play'; title = 'Play'; setup = 'none' }); checks = $checks }
    $path = Join-Path $root 'queue.json'
    [IO.File]::WriteAllText($path, ($q | ConvertTo-Json -Depth 8))
    return $path
}
function New-Check([string] $id, [string] $kind, $run, [hashtable] $extra) {
    $c = @{ id = $id; task = 'TEST'; title = $id; kind = $kind; run = $run; pass = 'x'; proves = 'x'; status = 'QUEUED' }
    if ($extra) { foreach ($k in $extra.Keys) { $c[$k] = $extra[$k] } }
    return $c
}
$checks = @(
    (New-Check 'T-build' 'pc-offline' @{ tool = 'build' }),
    (New-Check 'T-verify-fails' 'pc-offline' @{ tool = 'verify' }),
    (New-Check 'T-hangs' 'pc-offline' @{ tool = 'wtdcheck'; archives = @('x.img') }),
    (New-Check 'T-output-mismatch' 'pc-offline' @{ tool = 'content-selftest' }),
    (New-Check 'T-blender' 'pc-offline' @{ tool = 'blender-tests' }),
    (New-Check 'T-probe' 'probe' @{ tool = 'probe'; probe = 'drawables' }),
    (New-Check 'T-probe-noreport' 'probe' @{ tool = 'probe'; probe = 'collision' }),
    (New-Check 'LOOP-package-install' 'pc-offline' @{ tool = 'package-install' }),
    (New-Check 'T-report-good' 'pc-offline' @{ tool = 'content-report'; asset = 'good_asset'; expect = @{ status = 'ok'; 'compiled.textureFormat' = 'DXT1' } }),
    (New-Check 'T-report-bad' 'pc-offline' @{ tool = 'content-report'; asset = 'bad_asset'; expect = @{ status = 'ok' } }),
    (New-Check 'T-scenario-good' 'scenario' @{ scenario = 'good' } @{ review = @{ screenshots = @{ view = 'anything' } } }),
    (New-Check 'T-scenario-crash' 'scenario' @{ scenario = 'crashy' }),
    (New-Check 'T-scenario-after-crash' 'scenario' @{ scenario = 'good' }),
    (New-Check 'T-scenario-probed' 'scenario' @{ scenario = 'probed' } @{ needs = @('T-probe') }),
    (New-Check 'T-manual-pass' 'manual' @{ steps = @('play') } @{ minutes = 1; session = 'play' }),
    (New-Check 'T-manual-skipped' 'manual' @{ steps = @('play') } @{ minutes = 1; session = 'play' })
)
$queuePath = New-TestQueue $checks
$sim = @{
    remote = $remote
    blender = ''
    scenarioTimeoutSeconds = 120
    tools = @{
        'T-build' = @{ exit = 0; output = 'Built everything' }
        'T-verify-fails' = @{ exit = 1; output = 'RESULT passed=3 failed=1' }
        'T-hangs' = @{ exit = 0; sleepSeconds = 30; timeoutSeconds = 2 }
        'T-output-mismatch' = @{ exit = 0; output = 'selftest: ok passed=5 failed=2'; pass = 'selftest: ok passed=\d+ failed=0' }
        'T-probe-noreport' = @{ exit = 0; writesReport = $false }
    }
    manual = @{ 'T-manual-pass' = 'p' }
}
$simFile = Join-Path $root 'simulation.json'
[IO.File]::WriteAllText($simFile, ($sim | ConvertTo-Json -Depth 8))
Initialize-SimulatedGame (Join-Path $root 'game') @{
    knownCommands = @('god')
    commands = @{ 'selftest' = @{ reply = 'started'; log = @('selftest_done passed=3 failed=0') }; 'boom' = @{ reply = 'ok'; crash = $true }
        'spawnprop simulated 3 0' = @{ reply = 'spawning prop simulated'; log = @('autopilot_prop handle=77 model=simulated') } }
} @()
& $verifyLocal -Simulate -SimulationFile $simFile -QueuePath $queuePath -KeepInstall 6>&1 | Out-Null
$runFolder = Get-ChildItem -LiteralPath (Join-Path $root 'results') -Directory | Select-Object -First 1
$summary = Get-Content -LiteralPath (Join-Path $runFolder.FullName 'summary.json') -Raw | ConvertFrom-Json
$status = @{}; foreach ($c in $summary.checks) { $status[$c.id] = $c.status }
Test-That 'run: a passing tool is PASS' ($status['T-build'] -eq 'PASS') ($status | Out-String)
Test-That 'run: a failing tool is FAIL' ($status['T-verify-fails'] -eq 'FAIL')
Test-That 'run: a hung tool is killed and ERROR' ($status['T-hangs'] -eq 'ERROR')
Test-That 'run: exit 0 with output not matching the pass pattern is FAIL' ($status['T-output-mismatch'] -eq 'FAIL')
Test-That 'run: Blender not configured is NOT-RUN' ($status['T-blender'] -eq 'NOT-RUN')
Test-That 'run: a probe with a report needs review, never auto-PASS' ($status['T-probe'] -eq 'NEEDS-REVIEW')
Test-That 'run: a probe without a report is FAIL' ($status['T-probe-noreport'] -eq 'FAIL')
Test-That 'run: package-install PASS' ($status['LOOP-package-install'] -eq 'PASS')
Test-That 'run: content report with matching fields is PASS' ($status['T-report-good'] -eq 'PASS')
Test-That 'run: content report with a wrong field is FAIL' ($status['T-report-bad'] -eq 'FAIL')
Test-That 'run: a passing scenario with screenshots to judge is NEEDS-REVIEW' ($status['T-scenario-good'] -eq 'NEEDS-REVIEW')
Test-That 'run: a crashing scenario is CRASH' ($status['T-scenario-crash'] -eq 'CRASH')
Test-That 'run: the scenario after a crash relaunches and passes' ($status['T-scenario-after-crash'] -eq 'PASS')
Test-That 'run: a scenario uses the value its probe found in the same run' ($status['T-scenario-probed'] -eq 'PASS') ($status['T-scenario-probed'])
Test-That 'run: manual answer p is PASS' ($status['T-manual-pass'] -eq 'PASS')
Test-That 'run: unanswered manual check is NOT-RUN' ($status['T-manual-skipped'] -eq 'NOT-RUN')
Test-That 'run: evidence copied (scenario report and screenshot)' (Test-Path -LiteralPath (Join-Path $runFolder.FullName (Join-Path 'T-scenario-good' 'view.png')))
Test-That 'run: install kept and recorded' ($summary.run.install -like 'kept the tested build*')
$text = Get-ChildItem -LiteralPath $runFolder.FullName -Recurse -File | Where-Object { $_.Extension -in '.json', '.md', '.log' } | ForEach-Object { [IO.File]::ReadAllText($_.FullName) }
Test-That 'run: machine paths scrubbed from every text file' (-not (($text -join "`n").Contains($root))) 'the simulation root appears in the results'
Test-That 'run: summary.md written' (Test-Path -LiteralPath (Join-Path $runFolder.FullName 'summary.md'))

# Published to the orphan branch of the (bare) remote
$published = & git --git-dir $remote ls-tree -r --name-only verification-results 2>$null
Test-That 'publish: results/<run>/summary.json on verification-results' (@($published) -contains "results/$($runFolder.Name)/summary.json") ($published -join ' ')
Test-That 'publish: LATEST names the run' ((& git --git-dir $remote show verification-results:LATEST).Trim() -eq $runFolder.Name)
$history = @(& git --git-dir $remote log --format=%P verification-results)
Test-That 'publish: the branch is an orphan (no parent from the code history)' ($history.Count -eq 1 -and -not $history[0].Trim())
Test-That 'publish: no worktree left behind' (@(& git -C $script:RepoRoot worktree list).Count -eq 1)

# ---- Install failure: everything that needs the install is NOT-RUN, not PASS; a second publish appends
$sim.install = @{ fails = $true }
[IO.File]::WriteAllText($simFile, ($sim | ConvertTo-Json -Depth 8))
Start-Sleep -Seconds 1
& $verifyLocal -Simulate -SimulationFile $simFile -QueuePath $queuePath -Only @('T-scenario-good', 'T-report-good') 6>&1 | Out-Null
$second = Get-ChildItem -LiteralPath (Join-Path $root 'results') -Directory | Sort-Object Name | Select-Object -Last 1
$summary = Get-Content -LiteralPath (Join-Path $second.FullName 'summary.json') -Raw | ConvertFrom-Json
$status = @{}; foreach ($c in $summary.checks) { $status[$c.id] = $c.status }
Test-That 'install failure: install FAIL' ($status['LOOP-package-install'] -eq 'FAIL') ($status | Out-String)
Test-That 'install failure: scenario NOT-RUN' ($status['T-scenario-good'] -eq 'NOT-RUN')
Test-That 'install failure: content report NOT-RUN' ($status['T-report-good'] -eq 'NOT-RUN')
$runs = @(& git --git-dir $remote ls-tree --name-only verification-results results/)
Test-That 'publish: a second run is added next to the first' ($runs.Count -eq 2) ($runs -join ' ')

# ---- Resume: finished checks are kept, NOT-RUN ones are run again
$sim.install = @{ fails = $false }
[IO.File]::WriteAllText($simFile, ($sim | ConvertTo-Json -Depth 8))
& $verifyLocal -Simulate -SimulationFile $simFile -QueuePath $queuePath -Only @('T-scenario-good', 'T-report-good') -Resume $second.FullName -NoPush 6>&1 | Out-Null
$summary = Get-Content -LiteralPath (Join-Path $second.FullName 'summary.json') -Raw | ConvertFrom-Json
$status = @{}; foreach ($c in $summary.checks) { $status[$c.id] = $c.status }
Test-That 'resume: the finished install result is kept (FAIL)' ($status['LOOP-package-install'] -eq 'FAIL') ($status | Out-String)
Test-That 'resume: checks that did not run are tried again (still NOT-RUN without an install)' ($status['T-scenario-good'] -eq 'NOT-RUN')
$env:LIBERTY_SIM_STATE = $null

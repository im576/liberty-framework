# The engine of tools/verify-local.ps1: runs the local check queue (tests/local/checks.json) on the owner's PC, writes
# a results folder and pushes it to the orphan verification-results branch. Nothing here decides a PASS from free text:
# tools exit codes plus the check's own pass pattern, autopilot result.json files, JSON field checks, and the owner's
# answers. Every step runs as a child process with a timeout, so one hung tool cannot stop the run.
# Compatible with Windows PowerShell 5.1 and PowerShell 7. Tested in the cloud with -Simulate (tools/tests).

$ErrorActionPreference = 'Stop'
$script:FinalStatuses = @('PASS', 'FAIL', 'ERROR', 'CRASH', 'NEEDS-REVIEW')
$script:KindOrder = @{ 'pc-offline' = 0; 'probe' = 1; 'scenario' = 3; 'manual' = 4 }
# Tools that read nothing from the game folder: they run first, before the game lock is taken (tools/local/GameLock.psm1).
$script:LockFreeTools = @('build', 'content-selftest')
Import-Module (Join-Path $PSScriptRoot 'GameLock.psm1') 3>$null

# A path from parts, with the platform's separator (the same code runs on the PC and, simulated, in the cloud).
function Join-Parts([string] $Base) {
    $path = $Base
    foreach ($part in $args) { $path = [IO.Path]::Combine($path, [string]$part) }
    return $path
}

# ---------------------------------------------------------------------------------------------------------------------
# Queue

function Read-CheckQueue([string] $Path) {
    if (-not (Test-Path -LiteralPath $Path)) { throw "check queue not found: $Path" }
    $queue = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    if ($queue.schemaVersion -ne 1) { throw "unsupported check queue schemaVersion $($queue.schemaVersion)" }
    return $queue
}

# Stage of a check in the run: offline tools, probes, then package/install, the checks that read the package, the
# scenarios (need the install), and last the owner's manual checks.
function Get-CheckStage($check) {
    if (-not (Test-CheckNeedsGame $check)) { return -1 }
    if ($check.kind -eq 'pc-offline' -and $check.run.tool -eq 'package-install') { return 2 }
    if ($check.kind -eq 'pc-offline' -and $check.run.tool -eq 'content-report') { return 2.5 }
    return $script:KindOrder[[string]$check.kind]
}

# Whether a check reads or drives the game (and so runs under the game lock).
function Test-CheckNeedsGame($check) {
    return -not ($check.kind -eq 'pc-offline' -and $script:LockFreeTools -contains [string]$check.run.tool)
}

# The checks this run executes, in run order.
#   Smoke: the build, package/install and one scenario (the SDK self-test) - proves the script itself works.
#   Only / Kinds: filters. RETIRED checks never run; manual checks already PASS are skipped unless IncludePassedManual.
function Select-Checks($Queue, [string[]] $Only, [string[]] $Kinds, [bool] $Smoke, [bool] $IncludePassedManual) {
    $all = @($Queue.checks | Where-Object { $_.status -ne 'RETIRED' })
    if ($Smoke) { $all = @($all | Where-Object { @('LOOP-build', 'LOOP-package-install', 'SDK-selftest') -contains $_.id }) }
    if ($Only -and $Only.Count -gt 0) {
        $unknown = @($Only | Where-Object { $id = $_; -not ($all | Where-Object { $_.id -eq $id }) })
        if ($unknown.Count -gt 0) { throw "unknown or retired check id(s): $($unknown -join ', ')" }
        $all = @($all | Where-Object { $Only -contains $_.id })
    }
    if ($Kinds -and $Kinds.Count -gt 0) { $all = @($all | Where-Object { $Kinds -contains $_.kind }) }
    if (-not $IncludePassedManual) { $all = @($all | Where-Object { -not ($_.kind -eq 'manual' -and $_.status -eq 'PASS') }) }
    # A check's "needs" (e.g. a scenario that spawns the model a probe found) run in the same run: add them when missing.
    foreach ($check in @($all)) {
        if (-not $check.PSObject.Properties['needs']) { continue }
        foreach ($id in @($check.needs)) {
            if ($all | Where-Object { $_.id -eq $id }) { continue }
            $needed = $Queue.checks | Where-Object { $_.id -eq $id -and $_.status -ne 'RETIRED' } | Select-Object -First 1
            if (-not $needed) { throw "check $($check.id) needs $id, which is not in the queue" }
            $all = @($needed) + $all
        }
    }
    # Scenario checks and package-reading checks need the install: add package-install when they are selected.
    $needsInstall = @($all | Where-Object { $_.kind -eq 'scenario' -or ($_.kind -eq 'pc-offline' -and $_.run.tool -eq 'content-report') }).Count -gt 0
    if ($needsInstall -and -not ($all | Where-Object { $_.id -eq 'LOOP-package-install' })) {
        $install = $Queue.checks | Where-Object { $_.id -eq 'LOOP-package-install' } | Select-Object -First 1
        if ($install) { $all = @($install) + $all }
    }
    $index = 0
    $ordered = $all | ForEach-Object { [pscustomobject]@{ Check = $_; Stage = (Get-CheckStage $_); Index = $index++ } } | Sort-Object Stage, Index
    return @($ordered | ForEach-Object { $_.Check })
}

# ---------------------------------------------------------------------------------------------------------------------
# Child processes

# One Windows command-line argument, quoted per the MSVC rules when needed (spaces, quotes, empty).
function ConvertTo-CommandArgument([string] $Value) {
    if ($Value.Length -gt 0 -and $Value -notmatch '[\s"]') { return $Value }
    $escaped = [regex]::Replace($Value, '(\\*)"', { param($m) ($m.Groups[1].Value * 2) + '\"' })
    $escaped = [regex]::Replace($escaped, '(\\+)$', { param($m) $m.Groups[1].Value * 2 })
    return '"' + $escaped + '"'
}

# Runs a program with a timeout; stdout and stderr go to $LogPath. A timed-out process (and its children) is killed.
# Returns @{ ExitCode; TimedOut; Output }.
# $Priority (e.g. 'BelowNormal'): the child's priority class; its own children inherit it on Windows. Builds that run
# while another session's game test runs use it, so the game keeps the CPU.
function Invoke-ChildProcess([string] $FilePath, [string[]] $Arguments, [int] $TimeoutSeconds, [string] $LogPath, [string] $WorkingDirectory, [string] $Priority = '') {
    $argumentText = (@($Arguments) | ForEach-Object { ConvertTo-CommandArgument ([string]$_) }) -join ' '
    $out = "$LogPath.out"; $err = "$LogPath.err"
    $options = @{ FilePath = $FilePath; PassThru = $true; NoNewWindow = $true; RedirectStandardOutput = $out; RedirectStandardError = $err }
    if ($argumentText) { $options.ArgumentList = $argumentText }
    if ($WorkingDirectory) { $options.WorkingDirectory = $WorkingDirectory }
    $process = Start-Process @options
    $null = $process.Handle  # Windows PowerShell 5.1 loses ExitCode unless the handle is opened while the process runs.
    if ($Priority -and $env:OS -eq 'Windows_NT') {
        try { $process.PriorityClass = $Priority } catch { Write-Host "[verify-local] could not set priority $Priority on $($process.Id): $($_.Exception.Message)" }
    }
    $timedOut = -not $process.WaitForExit([Math]::Max(1, $TimeoutSeconds) * 1000)
    if ($timedOut) { Stop-ProcessTree $process; $process.WaitForExit(10000) | Out-Null }
    else { $process.WaitForExit() }
    $text = ''
    foreach ($file in $out, $err) { if (Test-Path -LiteralPath $file) { $text += [IO.File]::ReadAllText($file); Remove-Item -LiteralPath $file -Force } }
    if ($timedOut) { $text += "`n[verify-local] killed after $TimeoutSeconds s (timeout)" }
    [IO.File]::WriteAllText($LogPath, $text)
    $code = if ($timedOut) { -1 } else { $process.ExitCode }
    return @{ ExitCode = $code; TimedOut = $timedOut; Output = $text }
}

function Stop-ProcessTree($Process) {
    try {
        if ($env:OS -eq 'Windows_NT') { & taskkill.exe /T /F /PID $Process.Id 2>&1 | Out-Null }
        elseif ($Process.GetType().GetMethod('Kill', [Type[]]@([bool]))) { $Process.Kill($true) }
        else { $Process.Kill() }
    } catch { Write-Host "[verify-local] could not kill process $($Process.Id): $($_.Exception.Message)" }
}

# The PowerShell that runs this script (powershell.exe 5.1 or pwsh), for child scripts.
function Get-PowerShellPath { return (Get-Process -Id $PID).Path }

function Get-ScriptArguments([string] $Script) { return @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $Script) }

# ---------------------------------------------------------------------------------------------------------------------
# Evaluating results

# A nested field by dotted path ("compiled.textureQuality.psnrAlphaDb"); $null when absent.
function Get-JsonField($Object, [string] $Path) {
    $current = $Object
    foreach ($part in $Path.Split('.')) {
        if ($null -eq $current -or -not $current.PSObject.Properties[$part]) { return $null }
        $current = $current.$part
    }
    return $current
}

# Mismatches between a JSON object and { "field.path": expected }. "present" means the field exists (any value).
function Test-JsonExpectations($Object, $Expect) {
    $problems = @()
    foreach ($property in $Expect.PSObject.Properties) {
        $actual = Get-JsonField $Object $property.Name
        if ([string]$property.Value -eq 'present') { if ($null -eq $actual) { $problems += "$($property.Name) missing" }; continue }
        if ($null -eq $actual -or [string]$actual -ne [string]$property.Value) { $problems += "$($property.Name)=$actual (expected $($property.Value))" }
    }
    return $problems
}

function New-Result([string] $Status, [string] $Detail, [string[]] $Evidence) {
    return @{ Status = $Status; Detail = $Detail; Evidence = @($Evidence | Where-Object { $_ }) }
}

# A tool check: PASS when the exit code is 0 and, if the check names one, the output matches its pass pattern.
function Get-ToolStatus($Run, [string] $PassPattern) {
    if ($Run.TimedOut) { return New-Result 'ERROR' 'timed out' @() }
    if ($Run.ExitCode -ne 0) { return New-Result 'FAIL' "exit code $($Run.ExitCode)" @() }
    if ($PassPattern -and $Run.Output -notmatch $PassPattern) { return New-Result 'FAIL' "output does not match $PassPattern" @() }
    return New-Result 'PASS' 'exit code 0' @()
}

# ---------------------------------------------------------------------------------------------------------------------
# Tools (the named steps a check may use; tools/checks/checks.py lists the same names)

# The command for a named tool on the PC. Simulated runs replace this (Get-SimulatedToolCommand).
function Get-ToolCommand($Context, $Check) {
    $ps = Get-PowerShellPath
    $repo = $Context.Repo
    $tools = Join-Path $repo 'tools'
    $content = Join-Parts $tools 'content' 'bin' 'LibertyContent.exe'
    switch ([string]$Check.run.tool) {
        'build' {
            # Two scripts in one child: the core (with its unit tests) and the C# build.
            $command = "& '$(Join-Path $tools 'build-core.ps1')'; if (`$LASTEXITCODE) { exit `$LASTEXITCODE }; & '$(Join-Path $tools 'build.ps1')' -ScriptHookDotNetReference '$($Context.Shdn)'; exit `$LASTEXITCODE"
            return @{ File = $ps; Arguments = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command', $command); Timeout = 1200; Pass = '' }
        }
        'verify' { return @{ File = $ps; Arguments = (Get-ScriptArguments (Join-Path $tools 'verify.ps1')) + @('-GameDirectory', $Context.Game); Timeout = 900; Pass = 'RESULT passed=\d+ failed=0(?! notrun)' } }
        'content-selftest' {
            # The self-test, then the authoring fixtures (tests/content/fixtures) checked against their expect.json.
            $fixtures = Join-Parts $repo 'tests' 'content' 'fixtures'
            $command = "& '$(Join-Path $tools 'build-content.ps1')'; if (`$LASTEXITCODE) { exit `$LASTEXITCODE }; & '$content' selftest; if (`$LASTEXITCODE) { exit `$LASTEXITCODE }; & '$content' fixtures '$fixtures'; exit `$LASTEXITCODE"
            return @{ File = $ps; Arguments = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command', $command); Timeout = 900; Pass = 'selftest: ok passed=\d+ failed=0' }
        }
        'wtdcheck' { return @{ File = $content; Arguments = @('wtdcheck', '--game', $Context.Game) + @($Check.run.archives); Timeout = 1800; Pass = 'failed=0' } }
        'wbdcheck' {
            $out = Join-Path $Context.Results ($Check.id + '.json')
            return @{ File = $content; Arguments = @('wbdcheck', '--game', $Context.Game, '--out', $out); Timeout = 2400; Pass = 'wbdcheck wbd=\d+ ce=[1-9]\d* parsed=\d+ tableRoundTrips=\d+'; Output = $out }
        }
        'blender-tests' {
            return @{ File = $ps; Arguments = (Get-ScriptArguments (Join-Parts $tools 'blender' 'run-tests.ps1')) + @('-GameDirectory', $Context.Game, '-Blender', $Context.Blender); Timeout = 1800; Pass = 'Blender add-on tests passed' }
        }
        'probe' {
            $out = Join-Path $Context.Results ($Check.id + '.json')
            return @{ File = $content; Arguments = @('probe', [string]$Check.run.probe, '--game', $Context.Game, '--out', $out); Timeout = 2400; Pass = ''; Output = $out }
        }
        'drawable-roundtrip' {
            # Every IMG of the game unless the check names archives; the JSON report is kept as evidence.
            $out = Join-Path $Context.Results ($Check.id + '.json')
            return @{ File = $content; Arguments = @('roundtrip', '--game', $Context.Game, '--out', $out) + @($Check.run.archives | Where-Object { $_ }); Timeout = 3600; Pass = 'roundtrip: ok'; Output = $out }
        }
    }
    throw "no command for tool '$($Check.run.tool)'"
}

# Content compiler reports survive packaging in staging/phase2-reports/content/<asset>/ (tools/package-phase2.ps1).
function Get-ContentReportFolder($Context, [string] $Asset) {
    if ($Context.Simulate) { return Join-Parts $Context.SimRoot 'reports' 'content' $Asset }
    return Join-Parts $Context.Repo 'staging' 'phase2-reports' 'content' $Asset
}

# ---------------------------------------------------------------------------------------------------------------------
# Running checks

function Invoke-ToolCheck($Context, $Check) {
    if ($Check.run.tool -eq 'blender-tests' -and -not $Context.Blender) {
        return New-Result 'NOT-RUN' 'Blender is not configured (-Blender <blender.exe> once; it is remembered)' @()
    }
    $command = if ($Context.Simulate) { Get-SimulatedToolCommand $Context $Check } else { Get-ToolCommand $Context $Check }
    if ($command.File -and -not $Context.Simulate -and $command.File -like '*.exe' -and -not (Test-Path -LiteralPath $command.File)) {
        return New-Result 'NOT-RUN' "$($command.File) is missing (build it first)" @()
    }
    $log = Join-Path $Context.Results ($Check.id + '.log')
    $run = Invoke-ChildProcess $command.File $command.Arguments $command.Timeout $log $Context.Repo
    $result = Get-ToolStatus $run $command.Pass
    $result.Evidence = @((Split-Path -Leaf $log))
    if ($Check.run.tool -eq 'wbdcheck' -and $result.Status -eq 'PASS' -and -not (Test-Path -LiteralPath $command.Output)) {
        return New-Result 'FAIL' 'wbdcheck wrote no JSON report' @((Split-Path -Leaf $log))
    }
    if ($Check.kind -ne 'probe' -and $command.Output -and (Test-Path -LiteralPath $command.Output)) { $result.Evidence += (Split-Path -Leaf $command.Output) }
    if ($Check.kind -eq 'probe') {
        $out = $command.Output
        if ($result.Status -eq 'PASS' -and -not (Test-Path -LiteralPath $out)) { return New-Result 'FAIL' 'the probe wrote no report' @((Split-Path -Leaf $log)) }
        if ($result.Status -eq 'PASS') { return New-Result 'NEEDS-REVIEW' 'report written; the review session reads it' @((Split-Path -Leaf $log), (Split-Path -Leaf $out)) }
    }
    return $result
}

function Invoke-ContentReportCheck($Context, $Check) {
    $folder = Get-ContentReportFolder $Context ([string]$Check.run.asset)
    $report = Join-Path $folder 'report.json'
    if (-not (Test-Path -LiteralPath $report)) { return New-Result 'FAIL' "no report: $report" @() }
    $target = Join-Path $Context.Results $Check.id
    New-Item -ItemType Directory -Force -Path $target | Out-Null
    Get-ChildItem -LiteralPath $folder -File | Where-Object { $_.Extension -in '.json', '.png' } | ForEach-Object { Copy-Item -LiteralPath $_.FullName -Destination $target }
    try { $json = Get-Content -LiteralPath $report -Raw | ConvertFrom-Json } catch { return New-Result 'FAIL' "report.json unreadable: $($_.Exception.Message)" @($Check.id) }
    $problems = @(Test-JsonExpectations $json $Check.run.expect)
    if ($problems.Count -gt 0) { return New-Result 'FAIL' ($problems -join '; ') @($Check.id) }
    return New-Result 'PASS' 'all expected fields match' @($Check.id)
}

function Get-PackageArguments($Context, [string] $Phase) {
    # -Fast: the offline verifier and the self-tests are checks of their own (LOOP-verify, LOOP-content-selftest); repeating
    # them inside every package step held the game lock for minutes while parallel sessions waited.
    $package = (Get-ScriptArguments (Join-Parts $Context.Repo 'tools' 'package-phase2.ps1')) + @('-GameDirectory', $Context.Game, '-ScriptHookDotNetReference', $Context.Shdn, '-Fast', '-Phase', $Phase)
    if ($Context.Lvs) { $package += @('-LvsDirectory', $Context.Lvs) }
    return $package
}

# Package step 1, before the game lock: compile and generate every artifact (package-phase2.ps1 -Phase Build reads only the
# repository and the game's own archives, and snapshots what the install step needs). Runs at below-normal priority so a
# game test of another session keeps the CPU. The outcome is reported by the package-install check.
function Invoke-PackageBuild($Context, $Check) {
    if ($Context.Simulate) { $Context.PackageBuild = @{ Ok = $true; Detail = 'simulated' }; return }
    $log = Join-Path $Context.Results ($Check.id + '.log')
    Write-Host '[verify-local] building the package before taking the game (package-phase2.ps1 -Phase Build)'
    $started = Get-Date
    $run = Invoke-ChildProcess (Get-PowerShellPath) (Get-PackageArguments $Context 'Build') 2400 $log $Context.Repo 'BelowNormal'
    $seconds = [int]((Get-Date) - $started).TotalSeconds
    $Context.PackageBuild = @{ Ok = (-not $run.TimedOut -and $run.ExitCode -eq 0); Detail = "build exit $($run.ExitCode) in $seconds s"; Log = $log }
    Write-Host "               -> package build $(if ($Context.PackageBuild.Ok) { 'done' } else { 'FAILED' }) ($seconds s)"
}

function Invoke-PackageInstall($Context, $Check) {
    if ($Context.Simulate) { return Invoke-SimulatedPackageInstall $Context $Check }
    $ps = Get-PowerShellPath
    $tools = Join-Path $Context.Repo 'tools'
    $log = Join-Path $Context.Results ($Check.id + '.log')
    if (-not $Context.PackageBuild) { Invoke-PackageBuild $Context $Check }
    if (-not $Context.PackageBuild.Ok) { return New-Result 'FAIL' "package-phase2.ps1 -Phase Build failed ($($Context.PackageBuild.Detail))" @((Split-Path -Leaf $log)) }
    # Package step 2, under the game lock: stage the snapshot against the installed files (seconds), then install.
    $stageLog = Join-Path $Context.Results ($Check.id + '-stage.log')
    $run = Invoke-ChildProcess $ps (Get-PackageArguments $Context 'Stage') 600 $stageLog $Context.Repo
    if ($run.TimedOut -or $run.ExitCode -ne 0) { return New-Result 'FAIL' "package-phase2.ps1 -Phase Stage failed (exit $($run.ExitCode))" @((Split-Path -Leaf $log), (Split-Path -Leaf $stageLog)) }
    $installLog = Join-Path $Context.Results ($Check.id + '-install.log')
    $before = Get-Date
    $install = Invoke-ChildProcess $ps ((Get-ScriptArguments (Join-Path $tools 'install-phase2.ps1')) + @('-GameDirectory', $Context.Game)) 900 $installLog $Context.Repo
    $backups = Join-Parts $Context.Game 'scripts' 'LibertyFramework' 'backups'
    $backup = Get-ChildItem -LiteralPath $backups -Directory -Filter 'phase2-*' -ErrorAction SilentlyContinue | Where-Object { $_.CreationTime -ge $before.AddSeconds(-5) } | Sort-Object CreationTime -Descending | Select-Object -First 1
    if ($backup) { $Context.Backup = $backup.FullName }
    if ($install.TimedOut -or $install.ExitCode -ne 0) { return New-Result 'FAIL' "install-phase2.ps1 failed (exit $($install.ExitCode)); it rolls itself back" @((Split-Path -Leaf $log), (Split-Path -Leaf $stageLog), (Split-Path -Leaf $installLog)) }
    $Context.Installed = $true
    return New-Result 'PASS' "installed; backup $($Context.Backup); $($Context.PackageBuild.Detail)" @((Split-Path -Leaf $log), (Split-Path -Leaf $stageLog), (Split-Path -Leaf $installLog))
}

function Invoke-ScenarioCheck($Context, $Check) {
    $name = [string]$Check.run.scenario
    $scenario = Join-Path $Context.ScenarioDirectory "$name.txt"
    $runs = Join-Path $Context.Results '_runs'
    New-Item -ItemType Directory -Force -Path $runs | Out-Null
    $log = Join-Path $Context.Results ($Check.id + '.log')
    $arguments = (Get-ScriptArguments (Join-Parts $Context.Repo 'tools' 'autopilot' 'Run-Scenario.ps1')) +
        @('-GameDirectory', $Context.Game, '-Scenario', $scenario, '-OutputDirectory', $runs, '-AutopilotModule', $Context.GameModule, '-ProbeDirectory', $Context.Results) +
        @($(if ($Context.Quick) { '-Quick' }), $(if ($Context.StopOnFailure) { '-StopOnFailure' }) | Where-Object { $_ })
    $run = Invoke-ChildProcess (Get-PowerShellPath) $arguments $Context.ScenarioTimeout $log $Context.Repo
    Import-Module (Join-Parts $Context.Repo 'tools' 'autopilot' 'AutopilotLogic.psm1') -Force 3>$null
    $read = Read-ScenarioResult $run.Output
    if ($run.TimedOut) { $read = @{ Status = 'ERROR'; Detail = "scenario killed after $($Context.ScenarioTimeout) s"; Path = $read.Path } }
    $evidence = @((Split-Path -Leaf $log))
    if ($read.Path -and (Test-Path -LiteralPath $read.Path)) {
        $target = Join-Path $Context.Results $Check.id
        Copy-Item -LiteralPath (Split-Path -Parent $read.Path) -Destination $target -Recurse -Force
        $evidence += $Check.id
    }
    $status = $read.Status
    $detail = $read.Detail
    # Screenshots that must be judged: a passing run is NEEDS-REVIEW until a person or the review session looks.
    if ($status -eq 'PASS' -and $Check.PSObject.Properties['review'] -and $Check.review.screenshots) {
        $status = 'NEEDS-REVIEW'; $detail = "passed; screenshots to judge: $((@($Check.review.screenshots.PSObject.Properties) | ForEach-Object { $_.Name }) -join ', ')"
    }
    # A game that cannot start (no audio device, its own fatal error, every launch attempt failed) will not start for the
    # next scenario either: the remaining scenarios of this run are NOT-RUN instead of each retrying for minutes.
    if ($read.Path -and (Test-Path -LiteralPath $read.Path)) {
        try {
            $runnerError = [string](Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json).runnerError
            if ($runnerError -like 'GAME-UNAVAILABLE*') { $Context.LaunchBlocked = $runnerError }
        } catch { Write-Host "[verify-local] could not read $($read.Path): $($_.Exception.Message)" }
    }
    # A crashed, errored or hung game must not poison the next scenario: stop it so the next one relaunches cleanly.
    if (@('CRASH', 'ERROR') -contains $read.Status -or $run.TimedOut) { Stop-TestGame $Context }
    return New-Result $status $detail $evidence
}

# The game phase runs from the first check that reads or drives the game to the end of the run's game work (restore).
# It holds the machine-wide game lock, so everything before it (builds, the package build) overlaps other sessions' game
# tests. On entry: a game an earlier autopilot run left running is stopped; the owner's own game is never touched (the
# install and scenarios are NOT-RUN then); without an audio output device the scenarios are NOT-RUN at once.
function Enter-GamePhase($Context, $Checks) {
    $Context.GamePhase = $true
    $Context.GameSince = Get-Date
    if (-not $Context.Simulate) {
        try { $Context.Lock = Enter-GameLock "verify-local $($Context.Repo)" }
        catch { $Context.LockFailed = "the game was not free in time ($($_.Exception.Message))"; return }
        if (Get-Process GTAIV -ErrorAction SilentlyContinue) {
            if (Test-AutopilotGameRunning $Context.Game) { Write-Host '[verify-local] stopping the game an earlier test left running'; Stop-TestGame $Context }
            else { $Context.GameBlocked = 'GTA IV is running and was not started by the autopilot (the owner may be playing); close it and run again' }
        }
    }
    if (@($Checks | Where-Object { $_.kind -eq 'scenario' }).Count -gt 0) {
        try {
            Import-Module $Context.GameModule -Force 3>$null
            if (-not (Test-AudioOutput)) { $Context.LaunchBlocked = 'GAME-UNAVAILABLE: no audio output device is active; GTA IV refuses to start without one (connect speakers, a headset or the controller)' }
        } catch { Write-Host "[verify-local] audio check failed: $($_.Exception.Message)" }
    }
}

function Exit-GamePhase($Context) {
    if ($Context.Lock) { Exit-GameLock $Context.Lock; $Context.Lock = $null }
}

function Stop-TestGame($Context) {
    try {
        Import-Module $Context.GameModule -Force 3>$null
        Set-AutopilotGame $Context.Game
        Stop-Game
    } catch { Write-Host "[verify-local] Stop-Game failed: $($_.Exception.Message)" }
}

# The owner's judgement. Non-interactive runs record NOT-RUN (simulated runs answer from the simulation file).
function Invoke-ManualCheck($Context, $Check) {
    if ($Context.Simulate) {
        $answer = Get-SimProperty $Context.Sim.manual ([string]$Check.id) 's'
        $note = 'simulated answer'
    }
    elseif (-not $Context.Interactive) { return New-Result 'NOT-RUN' 'non-interactive run' @() }
    else {
        Write-Host ''
        Write-Host "---- $($Check.title)  [$($Check.id), $($Check.task), about $($Check.minutes) min]"
        $number = 1
        foreach ($step in $Check.run.steps) { Write-Host "  $number. $step"; $number++ }
        Write-Host "  Pass when: $($Check.pass)"
        if ($Check.PSObject.Properties['reference'] -and $Check.reference) { Write-Host "  Full steps: $($Check.reference)" }
        $answer = ''
        while (@('p', 'f', 's', 'q') -notcontains $answer) { $answer = (Read-Host '  Result: p = pass, f = fail, s = skip, q = skip all remaining').Trim().ToLowerInvariant() }
        $note = if ($answer -eq 'p' -or $answer -eq 'f') { Read-Host '  Note (Enter for none)' } else { '' }
    }
    switch ($answer) {
        'p' { return New-Result 'PASS' ("owner: pass" + $(if ($note) { " - $note" } else { '' })) @() }
        'f' { return New-Result 'FAIL' ("owner: fail" + $(if ($note) { " - $note" } else { '' })) @() }
        'q' { $Context.SkipManual = $true; return New-Result 'NOT-RUN' 'skipped by the owner' @() }
        default { return New-Result 'NOT-RUN' 'skipped by the owner' @() }
    }
}

function Invoke-Check($Context, $Check) {
    $started = Get-Date
    try {
        if ($Context.LockFailed -and (Test-CheckNeedsGame $Check)) { $result = New-Result 'NOT-RUN' $Context.LockFailed @() }
        elseif ($Check.kind -eq 'manual' -and $Context.SkipManual) { $result = New-Result 'NOT-RUN' 'skipped by the owner' @() }
        elseif ($Context.GameBlocked -and ($Check.kind -eq 'scenario' -or $Check.run.tool -eq 'package-install')) {
            $result = New-Result 'NOT-RUN' $Context.GameBlocked @()
        }
        elseif (($Check.kind -eq 'scenario' -or $Check.run.tool -eq 'content-report') -and -not $Context.Installed) {
            $result = New-Result 'NOT-RUN' 'LOOP-package-install did not pass in this run' @()
        }
        elseif ($Check.kind -eq 'scenario' -and $Context.MaxGameMinutes -gt 0 -and $Context.GameSince -and ((Get-Date) - $Context.GameSince).TotalMinutes -ge $Context.MaxGameMinutes) {
            $result = New-Result 'NOT-RUN' "game time cap reached ($($Context.MaxGameMinutes) min, -MaxGameMinutes): the game goes to the next session in line; run this check in a new batch" @()
        }
        elseif ($Check.kind -eq 'scenario' -and $Context.LaunchBlocked) {
            $result = New-Result 'NOT-RUN' "the game could not start earlier in this run: $($Context.LaunchBlocked)" @()
        }
        elseif ($Check.kind -eq 'scenario') { $result = Invoke-ScenarioCheck $Context $Check }
        elseif ($Check.kind -eq 'manual') { $result = Invoke-ManualCheck $Context $Check }
        elseif ($Check.run.tool -eq 'package-install') { $result = Invoke-PackageInstall $Context $Check }
        elseif ($Check.run.tool -eq 'content-report') { $result = Invoke-ContentReportCheck $Context $Check }
        else { $result = Invoke-ToolCheck $Context $Check }
    }
    catch { $result = New-Result 'ERROR' "verify-local: $($_.Exception.Message)" @() }
    return [ordered]@{
        id = [string]$Check.id; task = [string]$Check.task; kind = [string]$Check.kind; title = [string]$Check.title
        status = $result.Status; detail = $result.Detail; evidence = @($result.Evidence)
        seconds = [int]((Get-Date) - $started).TotalSeconds
    }
}

# ---------------------------------------------------------------------------------------------------------------------
# Results

function Get-StatusCounts($Checks) {
    $counts = [ordered]@{}
    foreach ($status in @('PASS', 'NEEDS-REVIEW', 'FAIL', 'CRASH', 'ERROR', 'NOT-RUN')) { $counts[$status] = @($Checks | Where-Object { $_.status -eq $status }).Count }
    return $counts
}

function Write-Summary($Context) {
    $summary = [ordered]@{
        schemaVersion = 1
        run = $Context.Run
        counts = (Get-StatusCounts $Context.Checks)
        checks = @($Context.Checks)
    }
    [IO.File]::WriteAllText((Join-Path $Context.Results 'summary.json'), ($summary | ConvertTo-Json -Depth 8), (New-Object Text.UTF8Encoding($false)))
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("# Verification run $($Context.Run.id)")
    $lines.Add('')
    $lines.Add("- Commit: $($Context.Run.commit) ($($Context.Run.branch))  Mode: $($Context.Run.mode)")
    $lines.Add("- Game: $($Context.Run.game.version)  exe SHA-256 $($Context.Run.game.exeSha256)")
    $lines.Add("- Started: $($Context.Run.startedUtc)  Finished: $($Context.Run.finishedUtc)")
    $lines.Add("- Install: $($Context.Run.install)")
    $lines.Add('- Counts: ' + ((Get-StatusCounts $Context.Checks).GetEnumerator() | ForEach-Object { "$($_.Key)=$($_.Value)" }) -join ' ')
    $lines.Add('')
    $lines.Add('| Check | Kind | Status | Detail | Evidence |')
    $lines.Add('|---|---|---|---|---|')
    foreach ($check in $Context.Checks) {
        $lines.Add("| ``$($check.id)`` | $($check.kind) | **$($check.status)** | $(([string]$check.detail).Replace('|', '\|')) | $((@($check.evidence) | ForEach-Object { $_ }) -join ', ') |")
    }
    [IO.File]::WriteAllLines((Join-Path $Context.Results 'summary.md'), $lines)
}

# Removes the owner's user name and machine paths from every text file of the results before they are pushed.
function Protect-Results([string] $Folder, [hashtable] $Replacements) {
    $pairs = @($Replacements.GetEnumerator() | Where-Object { $_.Key -and $_.Key.Length -ge 3 } | Sort-Object { $_.Key.Length } -Descending)
    foreach ($file in Get-ChildItem -LiteralPath $Folder -Recurse -File | Where-Object { @('.log', '.md', '.json', '.txt') -contains $_.Extension }) {
        $text = [IO.File]::ReadAllText($file.FullName)
        $changed = $text
        foreach ($pair in $pairs) {
            foreach ($variant in @($pair.Key, $pair.Key.Replace('\', '/'), $pair.Key.Replace('\', '\\'))) {
                $changed = [regex]::Replace($changed, [regex]::Escape($variant), $pair.Value, 'IgnoreCase')
            }
        }
        if ($changed -ne $text) { [IO.File]::WriteAllText($file.FullName, $changed) }
    }
}

# PNG screenshots become JPEGs at most $MaxWidth wide (Windows: System.Drawing). Elsewhere they are left as they are.
function Compress-Screenshots([string] $Folder, [int] $MaxWidth = 1280) {
    if ($env:OS -ne 'Windows_NT') { return }
    Add-Type -AssemblyName System.Drawing
    $codec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
    $parameters = New-Object System.Drawing.Imaging.EncoderParameters(1)
    $parameters.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter([System.Drawing.Imaging.Encoder]::Quality, [long]80)
    foreach ($png in Get-ChildItem -LiteralPath $Folder -Recurse -Filter '*.png') {
        $image = [System.Drawing.Image]::FromFile($png.FullName)
        try {
            $width = [Math]::Min($MaxWidth, $image.Width); $height = [int]($image.Height * $width / $image.Width)
            $small = New-Object System.Drawing.Bitmap($image, $width, $height)
            try { $small.Save([IO.Path]::ChangeExtension($png.FullName, '.jpg'), $codec, $parameters) } finally { $small.Dispose() }
        } finally { $image.Dispose() }
        Remove-Item -LiteralPath $png.FullName
    }
    Get-ChildItem -LiteralPath $Folder -Recurse -File | Where-Object { @('.md', '.json') -contains $_.Extension } | ForEach-Object {
        $text = [IO.File]::ReadAllText($_.FullName); [IO.File]::WriteAllText($_.FullName, ($text -replace '\.png\b', '.jpg'))
    }
}

# Pushes the results folder to the orphan branch verification-results (results/<run id>/), through a temporary
# worktree so the owner's checkout is never touched. $Remote is 'origin' or, in simulated runs, a local bare repository.
function Publish-Results([string] $Repo, [string] $Folder, [string] $Remote, [string] $Message) {
    # git reports progress on stderr; with 'Stop', Windows PowerShell 5.1 turns that into a terminating error.
    $ErrorActionPreference = 'Continue'
    $name = Split-Path -Leaf $Folder
    $worktree = Join-Path ([IO.Path]::GetTempPath()) ('lf-results-' + [Guid]::NewGuid().ToString('N').Substring(0, 8))
    $branch = 'lf-results-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
    $git = { param([string[]] $gitArgs) $output = & git @gitArgs 2>&1; if ($LASTEXITCODE -ne 0) { throw "git $($gitArgs -join ' ') failed: $output" }; $output }
    & git -C $Repo fetch $Remote 'verification-results' 2>&1 | Out-Null
    $exists = $LASTEXITCODE -eq 0
    try {
        if ($exists) {
            & $git @('-C', $Repo, 'worktree', 'add', '-f', '-b', $branch, $worktree, 'FETCH_HEAD') | Out-Null
        }
        else {
            & $git @('-C', $Repo, 'worktree', 'add', '-f', '--detach', $worktree, 'HEAD') | Out-Null
            & $git @('-C', $worktree, 'checkout', '--orphan', $branch) | Out-Null
            & $git @('-C', $worktree, 'rm', '-rf', '--quiet', '.') | Out-Null
            [IO.File]::WriteAllText((Join-Path $worktree 'README.md'), "# Verification results`n`nPushed by tools/verify-local.ps1 from the owner's PC. One folder per run under results/. Never merged into any branch. How to read them: docs/workflow/CLOUD_LOCAL_LOOP.md.`n")
        }
        $target = Join-Path (Join-Path $worktree 'results') $name
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
        Copy-Item -LiteralPath $Folder -Destination $target -Recurse -Force
        [IO.File]::WriteAllText((Join-Path $worktree 'LATEST'), $name + "`n")
        & $git @('-C', $worktree, 'add', '-A') | Out-Null
        & $git @('-C', $worktree, 'commit', '-q', '-m', $Message) | Out-Null
        & $git @('-C', $worktree, 'push', '-q', $Remote, "HEAD:refs/heads/verification-results") | Out-Null
    }
    finally {
        & git -C $Repo worktree remove --force $worktree 2>&1 | Out-Null
        & git -C $Repo branch -D $branch 2>&1 | Out-Null
    }
}

# ---------------------------------------------------------------------------------------------------------------------
# Simulation (cloud tests): tools become small PowerShell commands, the game is tools/tests/SimulatedGame.psm1.

function Get-SimProperty($Object, [string] $Name, $Default) {
    if ($null -ne $Object -and $Object.PSObject.Properties[$Name]) { return $Object.$Name }
    return $Default
}

function Get-SimulatedToolCommand($Context, $Check) {
    $tool = Get-SimProperty $Context.Sim.tools ([string]$Check.id) (Get-SimProperty $Context.Sim.tools ([string]$Check.run.tool) $null)
    $exit = [int](Get-SimProperty $tool 'exit' 0)
    $output = [string](Get-SimProperty $tool 'output' 'simulated ok')
    $sleep = [int](Get-SimProperty $tool 'sleepSeconds' 0)
    $timeout = [int](Get-SimProperty $tool 'timeoutSeconds' 60)
    $command = "Write-Output '$($output.Replace("'", "''"))'; Start-Sleep -Seconds $sleep; exit $exit"
    $result = @{ File = (Get-PowerShellPath); Arguments = @('-NoProfile', '-Command', $command); Timeout = $timeout; Pass = [string](Get-SimProperty $tool 'pass' '') }
    if ($Check.kind -eq 'probe' -or $Check.run.tool -eq 'drawable-roundtrip' -or $Check.run.tool -eq 'wbdcheck') {
        $out = Join-Path $Context.Results ($Check.id + '.json')
        if (Get-SimProperty $tool 'writesReport' $true) { $result.Arguments = @('-NoProfile', '-Command', "Set-Content -LiteralPath '$out' -Value '{""probe"":""simulated""}'; $command") }
        $result.Output = $out
    }
    return $result
}

function Invoke-SimulatedPackageInstall($Context, $Check) {
    $install = Get-SimProperty $Context.Sim 'install' $null
    if (Get-SimProperty $install 'fails' $false) { return New-Result 'FAIL' 'simulated install failure' @() }
    $Context.Backup = Join-Path $Context.SimRoot 'backup-phase2-sim'
    New-Item -ItemType Directory -Force -Path $Context.Backup | Out-Null
    $Context.Installed = $true
    return New-Result 'PASS' "installed (simulated); backup $($Context.Backup)" @()
}

# ---------------------------------------------------------------------------------------------------------------------
# The run

function Invoke-VerifyLocal([hashtable] $Options) {
    $repo = $Options.Repo
    $queue = Read-CheckQueue $Options.QueuePath
    $checks = @(Select-Checks $queue $Options.Only $Options.Kinds ([bool]$Options.Smoke) ([bool]$Options.IncludePassedManual))
    if ($checks.Count -eq 0) { throw 'no checks selected' }

    $commit = Invoke-Command { $ErrorActionPreference = 'Continue'; & git -C $repo rev-parse --short HEAD 2>$null }
    $branch = Invoke-Command { $ErrorActionPreference = 'Continue'; & git -C $repo rev-parse --abbrev-ref HEAD 2>$null }
    $runId = (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + $commit
    $results = if ($Options.Resume) { (Resolve-Path -LiteralPath $Options.Resume).Path } else { Join-Path $Options.ResultsRoot $runId }
    New-Item -ItemType Directory -Force -Path $results | Out-Null

    $context = @{
        Repo = $repo; Game = $Options.Game; Blender = $Options.Blender; Lvs = $Options.Lvs; Shdn = $Options.Shdn
        Results = $results; Simulate = [bool]$Options.Simulate; Sim = $Options.Sim; SimRoot = $Options.SimRoot
        Interactive = [bool]$Options.Interactive; GameModule = $Options.GameModule; ScenarioDirectory = $Options.ScenarioDirectory
        ScenarioTimeout = [int]$Options.ScenarioTimeout; Installed = $false; Backup = ''; SkipManual = [bool]$Options.NoManual
        Quick = [bool]$Options.Quick; StopOnFailure = [bool]$Options.StopOnFailure; MaxGameMinutes = [double]$Options.MaxGameMinutes
        Checks = New-Object System.Collections.ArrayList
    }
    $previous = @{}
    if ($Options.Resume -and (Test-Path -LiteralPath (Join-Path $results 'summary.json'))) {
        $old = Get-Content -LiteralPath (Join-Path $results 'summary.json') -Raw | ConvertFrom-Json
        foreach ($check in $old.checks) { $previous[[string]$check.id] = $check }
        if ($old.run.installBackup) { $context.Backup = [string]$old.run.installBackup }
        if ($previous.ContainsKey('LOOP-package-install') -and $previous['LOOP-package-install'].status -eq 'PASS') { $context.Installed = $true }
        $runId = Split-Path -Leaf $results
    }
    $context.Run = [ordered]@{
        id = $runId; commit = $commit; branch = $branch
        mode = ($(if ($Options.Simulate) { 'simulate' } elseif ($Options.Smoke) { 'smoke' } elseif ($Options.Quick) { 'quick' } else { 'full' }) + $(if ($Options.Quick -and $Options.Simulate) { '+quick' } else { '' }))
        startedUtc = [DateTime]::UtcNow.ToString('o'); finishedUtc = ''
        game = $Options.GameInfo; install = 'not installed'; installBackup = ''
        selected = @($checks | ForEach-Object { $_.id })
    }

    $installCheck = $checks | Where-Object { $_.run.tool -eq 'package-install' } | Select-Object -First 1
    try {
        foreach ($check in $checks) {
            if ($previous.ContainsKey([string]$check.id) -and $script:FinalStatuses -contains [string]$previous[[string]$check.id].status) {
                [void]$context.Checks.Add($previous[[string]$check.id]); continue
            }
            if (-not $context.GamePhase -and (Test-CheckNeedsGame $check)) {
                # Build the package while another session may still be using the game, then take the game.
                if ($installCheck -and -not ($previous.ContainsKey([string]$installCheck.id) -and $script:FinalStatuses -contains [string]$previous[[string]$installCheck.id].status)) {
                    Invoke-PackageBuild $context $installCheck
                }
                Enter-GamePhase $context $checks
            }
            if ($check.kind -eq 'manual' -and -not $context.ManualStarted -and $context.Interactive -and -not $context.SkipManual -and -not $context.Simulate) {
                $context.ManualStarted = $true
                if ($context.Installed) { Stop-TestGame $context }
                Write-Host ''
                Write-Host '==== Manual checks. Launch GTA IV through Steam now and load a save. Each check says what to do; answer when done.'
            }
            Write-Host ("[verify-local] {0,-34} {1}" -f $check.id, $check.title)
            $row = Invoke-Check $context $check
            [void]$context.Checks.Add($row)
            Write-Host ("               -> {0}  {1}" -f $row.status, $row.detail)
            $context.Run.install = $(if ($context.Installed) { "installed; backup $($context.Backup)" } else { 'not installed' })
            $context.Run.installBackup = $context.Backup
            Write-Summary $context
        }
        if ($context.Installed -and -not $context.Simulate) { Stop-TestGame $context }

        # Keep or restore the tested build.
        if ($context.Installed -and $context.Backup) {
            $restore = [bool]$Options.Restore -or ([bool]$Options.Smoke -and -not $Options.KeepInstall)
            if (-not $restore -and -not $Options.KeepInstall -and $context.Interactive -and -not $context.Simulate) {
                $restore = (Read-Host 'Restore your previous install? The tested build stays if you answer n [y/N]').Trim().ToLowerInvariant() -eq 'y'
            }
            if ($restore) {
                if ($context.Simulate) { $context.Run.install = "restored (simulated) from $($context.Backup)" }
                else {
                    $log = Join-Path $results 'restore.log'
                    $run = Invoke-ChildProcess (Get-PowerShellPath) ((Get-ScriptArguments (Join-Parts $repo 'tools' 'rollback-phase2.ps1')) + @('-GameDirectory', $context.Game, '-BackupDirectory', $context.Backup)) 900 $log $repo
                    $context.Run.install = $(if ($run.ExitCode -eq 0) { "restored from $($context.Backup)" } else { "RESTORE FAILED (exit $($run.ExitCode)); run tools/rollback-phase2.ps1 -BackupDirectory $($context.Backup)" })
                }
            }
            else { $context.Run.install = "kept the tested build; to undo: tools/rollback-phase2.ps1 -GameDirectory <game> -BackupDirectory $($context.Backup)" }
        }
    }
    finally {
        # Compressing, scrubbing and publishing the results need no game: other sessions get it now.
        Exit-GamePhase $context
    }
    $context.Run.finishedUtc = [DateTime]::UtcNow.ToString('o')
    Write-Summary $context

    Compress-Screenshots $results
    Protect-Results $results $Options.Replacements
    $counts = Get-StatusCounts $context.Checks
    $countText = ($counts.GetEnumerator() | Where-Object { $_.Value -gt 0 } | ForEach-Object { "$($_.Key)=$($_.Value)" }) -join ' '
    $published = 'not pushed (-NoPush)'
    if (-not $Options.NoPush) {
        try { Publish-Results $repo $results $Options.Remote "results: $runId $countText"; $published = "pushed to verification-results: results/$runId" }
        catch { $published = "PUSH FAILED: $($_.Exception.Message). The results are in $results; push later with -Resume and no other changes." }
    }
    Write-Host ''
    Write-Host "==== $runId  $countText"
    Write-Host "Results: $results"
    Write-Host "Install: $($context.Run.install)"
    Write-Host "Publish: $published"
    if (-not $Options.NoPush -and $published -like 'pushed*') { Write-Host 'Next: start a cloud session with "Process the newest verification results."' }
    return @{ Results = $results; Counts = $counts; Published = $published; Checks = $context.Checks }
}

Export-ModuleMember -Function Read-CheckQueue, Select-Checks, ConvertTo-CommandArgument, Invoke-ChildProcess, Get-JsonField, Test-JsonExpectations,
    Get-ToolStatus, Protect-Results, Publish-Results, Invoke-VerifyLocal, Get-StatusCounts

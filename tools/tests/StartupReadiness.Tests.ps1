$startupStubPath = Join-Path $script:TestRoot 'StartupReadinessStub.psm1'
$startupProbe = Import-Module $startupStubPath -Force -PassThru
$startupCaseRoot = Join-Path $script:Scratch 'startup-readiness'
New-Item -ItemType Directory -Force -Path $startupCaseRoot | Out-Null

function Invoke-StartupProbe([hashtable] $Options, [int] $Allowance = 0, [hashtable] $Arguments = @{}) {
    $path = Join-Path $startupCaseRoot ([Guid]::NewGuid().ToString('N') + '.jsonl')
    $outcome = & $startupProbe {
        param($Options, $Allowance, $Arguments, $Path)
        Initialize-StartupStub $Options
        $params = @{ TelemetryPath = $Path }
        foreach ($key in $Arguments.Keys) { $params[$key] = $Arguments[$key] }
        if ($Allowance -gt 0) { $params.DeadlineUtc = (Get-StartupStubState).Base.AddSeconds($Allowance) }
        $errorText = ''; $attempt = $null
        try { $attempt = Start-GameReady @params } catch { $errorText = $_.Exception.Message }
        [pscustomobject]@{ Attempt = $attempt; Error = $errorText; State = Get-StartupStubState }
    } $Options $Allowance $Arguments $path
    $events = @(Get-Content -LiteralPath $path | ForEach-Object { $_ | ConvertFrom-Json })
    [pscustomobject]@{ Attempt = $outcome.Attempt; Error = $outcome.Error; State = $outcome.State; Events = $events; Path = $path }
}

$ready = Invoke-StartupProbe @{}
Test-That 'startup actual loop: default ready returns attempt 1 with observed PID/UTC/boot evidence' (
    $ready.Attempt -eq 1 -and -not $ready.Error -and
    @($ready.Events | Where-Object { $_.event -eq 'process_observed' -and $_.pid -eq 101 -and $_.processStartedUtc }).Count -eq 1 -and
    @($ready.Events | Where-Object { $_.event -eq 'engine_booted' -and $_.line -match 'engine_booted' }).Count -eq 1
)
$retry = Invoke-StartupProbe @{ BootAfter = 10000; RetryBootAfter = 3 }
Test-That 'startup actual loop: standalone boot timeout still permits the existing retry and a fresh boot' (
    $retry.Attempt -eq 2 -and $retry.State.Launches -eq 2 -and
    @($retry.Events | Where-Object { $_.event -eq 'attempt_finished' -and $_.reason -eq 'boot_deadline' }).Count -eq 1
)
$lateRead = Invoke-StartupProbe @{ LogReadSeconds = 20 } 12
Test-That 'startup actual loop: blocking log read retains late boot observation but cannot claim ready after deadline' (
    $lateRead.Error -like 'STARTUP-TIMEOUT*' -and $lateRead.State.Launches -eq 1 -and
    @($lateRead.Events | Where-Object { $_.event -eq 'engine_booted' }).Count -eq 1 -and
    $null -eq $lateRead.Attempt
)
$clipped = Invoke-StartupProbe @{ BootAfter = 10000; RetryBootAfter = 10000; RetryPidDelay = 30 } 295 @{ Attempts = 6 }
Test-That 'startup actual loop: late retry shares the original deadline, never receives a fresh allowance' (
    $clipped.Error -like 'STARTUP-TIMEOUT*' -and $clipped.State.Launches -eq 2 -and
    ($clipped.State.Now - $clipped.State.Base).TotalSeconds -eq 295 -and
    @($clipped.Events | Where-Object { $_.event -eq 'attempt_finished' -and $_.reason -eq 'shared_deadline' }).Count -eq 1 -and
    @($clipped.Events | Where-Object { $_.event -eq 'process_observed' -and $_.pid -eq 102 }).Count -eq 1
)
$dialog = Invoke-StartupProbe @{ BootAfter = 10000; Dialog = 'Error building shader!' } 12
Test-That 'startup actual loop: nonfatal dialog remains observable without dismissal or invented cause' (
    $dialog.Error -like 'STARTUP-TIMEOUT*' -and $dialog.State.Stops -eq 1 -and
    @($dialog.Events | Where-Object { $_.event -eq 'dialog_observed' -and $_.title -eq 'Error building shader!' }).Count -eq 1
)
$fatal = Invoke-StartupProbe @{ BootAfter = 10000; Dialog = 'GTA IV Fatal Error' }
Test-That 'startup actual loop: fatal readiness error is retained and does not retry' (
    $fatal.Error -like 'GAME-UNAVAILABLE*Fatal Error*' -and $fatal.State.Launches -eq 1 -and
    @($fatal.Events | Where-Object { $_.event -eq 'attempt_finished' -and $_.reason -eq 'fatal_dialog' }).Count -eq 1
)
$absent = Invoke-StartupProbe @{ NeverSeen = $true; BootAfter = 10000 } 0 @{ Attempts = 2; NotSeenSeconds = 3 }
Test-That 'startup actual loop: observed process absence exhausts only the requested attempts' (
    $absent.Error -like 'GAME-UNAVAILABLE*after 2 attempts' -and $absent.State.Launches -eq 2 -and
    @($absent.Events | Where-Object { $_.reason -eq 'process_not_seen' }).Count -eq 2 -and
    $absent.Events[-1].attempt -eq 2
)
$exited = Invoke-StartupProbe @{ BootAfter = 10000; ExitAfter = 6 } 0 @{ Attempts = 1 }
Test-That 'startup actual loop: process exit is recorded as absence, never a guessed faulting module' (
    $exited.Error -like 'GAME-UNAVAILABLE*' -and
    @($exited.Events | Where-Object { $_.event -eq 'process_absent' }).Count -eq 1 -and
    @($exited.Events | Where-Object { $_.reason -eq 'process_remained_absent' }).Count -eq 1
)
$audio = Invoke-StartupProbe @{ NoAudio = $true }
Test-That 'startup actual loop: preflight readiness failure survives without a launch' (
    $audio.Error -like 'GAME-UNAVAILABLE*audio*' -and $audio.State.Launches -eq 0 -and
    $audio.Events[-1].event -eq 'startup_failed'
)
$launchError = Invoke-StartupProbe @{ LaunchError = $true }
Test-That 'startup actual loop: launch communication error is persisted and rethrown' (
    $launchError.Error -eq 'stub launch error' -and $launchError.Events[-1].reason -eq 'stub launch error'
)
Remove-Module $startupProbe

# Exercise the actual runner report path with the same virtual startup loop, not native/UI imports.
$runner = Join-Path $script:RepoRoot 'tools/autopilot/Run-Scenario.ps1'
$fixture = Join-Path $startupCaseRoot 'empty.txt'
[IO.File]::WriteAllText($fixture, '# startup-only timeout must finalize before any fixture command')
$world = Join-Path $startupCaseRoot 'world.json'
[IO.File]::WriteAllText($world, '{ "BootAfter": 10000 }')
$startupSavedEnv = $env:LIBERTY_STARTUP_STUB
try {
    $env:LIBERTY_STARTUP_STUB = $world
    $journal = Join-Path $startupCaseRoot 'parent-startup.jsonl'
    $hardDeadline = [DateTime]::UtcNow.AddSeconds(15).ToString('o')
    $output = & (Get-Process -Id $PID).Path -NoProfile -File $runner -GameDirectory $startupCaseRoot -Scenario $fixture -OutputDirectory $startupCaseRoot -AutopilotModule $startupStubPath -Quick -ScenarioDeadlineUtc $hardDeadline -StartupTelemetryPath $journal 2>&1 | Out-String
    Import-Module (Join-Path $script:RepoRoot 'tools/autopilot/AutopilotLogic.psm1') -Force
    $read = Read-ScenarioResult $output
    $result = Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json
    Test-That 'startup runner: shared deadline writes ERROR/result/startup snapshot before outer kill' (
        $read.Status -eq 'ERROR' -and $result.phase -eq 'startup' -and $result.steps -eq 0 -and
        $result.runnerError -like 'STARTUP-TIMEOUT*' -and $result.startup.Count -gt 0 -and
        $result.evidence -contains 'startup-events.jsonl' -and (Test-Path -LiteralPath $journal) -and
        (Test-Path -LiteralPath (Join-Path (Split-Path -Parent $read.Path) 'startup-events.jsonl'))
    ) $output

    foreach ($mode in @('full', 'quick')) {
        $settleSeconds = if ($mode -eq 'quick') { 2 } else { 12 }
        $initialUtc = [DateTime]::UtcNow
        # Both modes boot at virtual +53, two seconds before the shared startup deadline (+55).
        $nearDeadline = $initialUtc.AddSeconds(55 + $settleSeconds + 5)
        $trace = Join-Path $startupCaseRoot ("near-$mode-sleeps.jsonl")
        $nearWorld = Join-Path $startupCaseRoot ("near-$mode-world.json")
        [IO.File]::WriteAllText($nearWorld, (@{ InitialUtc = $initialUtc.ToString('o'); BootAfter = 51; TracePath = $trace } | ConvertTo-Json))
        $env:LIBERTY_STARTUP_STUB = $nearWorld
        $nearFixture = Join-Path $startupCaseRoot ("near-$mode.txt")
        [IO.File]::WriteAllText($nearFixture, 'wait 1')
        $nearArguments = @('-NoProfile', '-File', $runner, '-GameDirectory', $startupCaseRoot,
            '-Scenario', $nearFixture, '-OutputDirectory', $startupCaseRoot, '-AutopilotModule', $startupStubPath,
            '-ScenarioDeadlineUtc', $nearDeadline.ToString('o'), '-StartupTelemetryPath', (Join-Path $startupCaseRoot "near-$mode-startup.jsonl"))
        if ($mode -eq 'quick') { $nearArguments += '-Quick' }
        $output = & (Get-Process -Id $PID).Path @nearArguments 2>&1 | Out-String
        $read = Read-ScenarioResult $output
        $nearResult = Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json
        $sleeps = @(Get-Content -LiteralPath $trace | ForEach-Object { $_ | ConvertFrom-Json })
        $bootEvent = @($nearResult.startup | Where-Object { $_.event -eq 'engine_booted' })[0]
        Test-That "startup actual runner/helper: near-deadline $mode boot keeps full $settleSeconds s settling and serialization reserve" (
            $read.Status -eq 'PASS' -and $nearResult.phase -eq 'scenario' -and $nearResult.mode -eq $mode -and
            ($nearDeadline.AddSeconds(-($settleSeconds + 5)) - ([DateTime]$bootEvent.utc).ToUniversalTime()).TotalSeconds -eq 2 -and
            @($sleeps | Where-Object { $_.seconds -eq $settleSeconds }).Count -eq 1 -and
            ([DateTime]$sleeps[-1].utc).ToUniversalTime() -le $nearDeadline.AddSeconds(-5)
        ) $output
    }
}
finally { $env:LIBERTY_STARTUP_STUB = $startupSavedEnv }

# Parent forced-kill path: execute real Invoke-ScenarioCheck, stub only the child/cleanup dependencies.
Import-Module (Join-Path $script:RepoRoot 'tools/local/VerifyLocal.psm1') -Force
$verifyModule = Get-Module VerifyLocal
$parentCase = Join-Path $startupCaseRoot 'parent'
New-Item -ItemType Directory -Force -Path $parentCase | Out-Null
$parentOutcome = & $verifyModule {
    param($Repo, $Results, $ScenarioDirectory)
    function Invoke-ChildProcess($FilePath, $Arguments, $TimeoutSeconds, $LogPath, $WorkingDirectory) {
        $journalIndex = [Array]::IndexOf($Arguments, '-StartupTelemetryPath')
        $deadlineIndex = [Array]::IndexOf($Arguments, '-ScenarioDeadlineUtc')
        $script:CapturedTimeout = $TimeoutSeconds
        $script:CapturedDeadline = [DateTime]::Parse($Arguments[$deadlineIndex + 1]).ToUniversalTime()
        # Persisted by the child before an external kill, without stdout marker or AUTOPILOT_RESULT.
        [IO.File]::WriteAllText($Arguments[$journalIndex + 1], '{"utc":"2026-10-02T00:02:31Z","attempt":2,"event":"process_observed","pid":7920}' + "`n")
        @{ TimedOut = $true; Output = ''; ExitCode = -1 }
    }
    function Stop-TestGame($Context) { $script:StoppedTest = $true }
    $context = [pscustomobject]@{ Repo = $Repo; Results = $Results; ScenarioDirectory = $ScenarioDirectory;
        Game = 'stub'; GameModule = 'stub'; Quick = $true; StopOnFailure = $true; ScenarioTimeout = 300;
        MaxGameMinutes = 1; GameSince = (Get-Date).AddSeconds(-40) }
    $before = [DateTime]::UtcNow
    $row = Invoke-ScenarioCheck $context ([pscustomobject]@{ id = 'T-startup'; run = @{ scenario = 'empty' } })
    [pscustomobject]@{ Row = $row; Timeout = $script:CapturedTimeout; Deadline = $script:CapturedDeadline;
        Before = $before; Stopped = $script:StoppedTest }
} $script:RepoRoot $parentCase $startupCaseRoot
Test-That 'startup parent: forced kill without markers retains stable parent journal as ERROR evidence' (
    $parentOutcome.Row.Status -eq 'ERROR' -and $parentOutcome.Row.Evidence -contains 'T-startup-startup.jsonl' -and
    (Test-Path -LiteralPath (Join-Path $parentCase 'T-startup-startup.jsonl')) -and $parentOutcome.Stopped
)
Test-That 'startup parent: passes the clipped remaining game allowance without increasing the scenario cap' (
    $parentOutcome.Timeout -le 20 -and $parentOutcome.Timeout -ge 19 -and
    ($parentOutcome.Deadline - $parentOutcome.Before).TotalSeconds -le 20.5 -and
    $parentOutcome.Row.Detail -like '*remaining game time cap*'
)

# Actual external termination: only this test child is killed, never a game/Steam/another lane process.
# Override the child script target, keeping real Invoke-ScenarioCheck / Invoke-ChildProcess / tree cleanup.
$hungChild = Join-Path $startupCaseRoot 'hung-child.ps1'
[IO.File]::WriteAllText($hungChild, @'
param($GameDirectory, $Scenario, $OutputDirectory, $AutopilotModule, $ProbeDirectory,
    [switch]$Quick, [switch]$StopOnFailure, $ScenarioDeadlineUtc, $StartupTelemetryPath)
[IO.File]::AppendAllText($StartupTelemetryPath, '{"utc":"2026-10-02T00:02:31Z","attempt":2,"event":"process_observed","pid":7920,"title":"stub-private-user"}' + "`n")
Start-Sleep -Seconds 30
'@)
$killed = & $verifyModule {
    param($Repo, $Results, $ScenarioDirectory, $HungChild)
    function Get-ScriptArguments($Script) { @('-NoProfile', '-File', $HungChild) }
    function Stop-TestGame($Context) { $script:StoppedTest = $true }
    $context = [pscustomobject]@{ Repo = $Repo; Results = $Results; ScenarioDirectory = $ScenarioDirectory;
        Game = 'stub'; GameModule = 'stub'; Quick = $true; StopOnFailure = $true;
        ScenarioTimeout = 8; MaxGameMinutes = 0 }
    Invoke-ScenarioCheck $context ([pscustomobject]@{ id = 'T-hardkill'; run = @{ scenario = 'empty' } })
} $script:RepoRoot $parentCase $startupCaseRoot $hungChild
$killedJournal = Join-Path $parentCase 'T-hardkill-startup.jsonl'
Test-That 'startup parent: actual hard kill with no result/telemetry stdout marker preserves partial journal' (
    $killed.Status -eq 'ERROR' -and $killed.Detail -like '*killed after 8 s*' -and
    $killed.Evidence -contains 'T-hardkill-startup.jsonl' -and (Test-Path -LiteralPath $killedJournal) -and
    -not (Test-Path -LiteralPath (Join-Path $parentCase 'T-hardkill/result.json'))
)
# Finalize removes _runs but retains the parent's journal. Scrub observations like other result text.
Protect-Results $parentCase @{ 'stub-private-user' = '<user>' }
Test-That 'startup parent: JSONL observation text uses the existing results redaction path' (
    (Get-Content -LiteralPath $killedJournal -Raw | ConvertFrom-Json).title -eq '<user>'
)

# The existing simulated helper has no deadline/telemetry parameters; new optional runner arguments must work.
Import-Module (Join-Path $script:TestRoot 'SimulatedGame.psm1') -Force
$savedSimulationState = $env:LIBERTY_SIM_STATE
try {
    Initialize-SimulatedGame (Join-Path $startupCaseRoot 'simulation') @{ running = $false } @()
    $simFixture = Join-Path $startupCaseRoot 'simulation.txt'
    [IO.File]::WriteAllText($simFixture, 'wait 1')
    $output = & (Get-Process -Id $PID).Path -NoProfile -File $runner -GameDirectory $startupCaseRoot -Scenario $simFixture -OutputDirectory $startupCaseRoot -AutopilotModule (Join-Path $script:TestRoot 'SimulatedGame.psm1') -Quick -ScenarioDeadlineUtc ([DateTime]::UtcNow.AddSeconds(30).ToString('o')) -StartupTelemetryPath (Join-Path $startupCaseRoot 'unused-simulation.jsonl') 2>&1 | Out-String
    Import-Module (Join-Path $script:RepoRoot 'tools/autopilot/AutopilotLogic.psm1') -Force
    $read = Read-ScenarioResult $output
    Test-That 'startup runner: old simulated helper signature remains compatible with new optional arguments' (
        $read.Status -eq 'PASS' -and (Get-Content -LiteralPath $read.Path -Raw | ConvertFrom-Json).startup.Count -eq 0
    ) $output
}
finally { $env:LIBERTY_SIM_STATE = $savedSimulationState }

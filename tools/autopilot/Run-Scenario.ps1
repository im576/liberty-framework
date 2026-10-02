param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    # A scenario file (tools/autopilot/scenarios/*.txt).
    [Parameter(Mandatory = $true)][string] $Scenario,
    # Where the report folder is created (outside the repository).
    [Parameter(Mandatory = $true)][string] $OutputDirectory,
    # Keep the game running afterwards (default: leave it running for the next scenario).
    [switch] $StopGameAfter,
    # The game-side module (default Autopilot.psm1). verify-local.ps1 -Simulate passes a stub with the same functions
    # (tools/tests/SimulatedGame.psm1) so the runner's decisions are tested without the game.
    [string] $AutopilotModule = (Join-Path $PSScriptRoot 'Autopilot.psm1'),
    # Folder with this run's probe reports (<check id>.json), for {probe:<id>:<field>} values (verify-local passes its
    # results folder).
    [string] $ProbeDirectory = '',
    # Run even when the installed build comes from another worktree (installed-build.json). Only for deliberate checks of
    # someone else's build; a lane testing its own work installs it first.
    [switch] $AllowOtherBuild,
    # An expect step gives up when the engine has written nothing to its log for this long (it logs every 30 s).
    [int] $FrozenSeconds = 150,
    # Development iteration: "@quick"/"@full" lines and {quick:A|B} values pick the short variant (ConvertTo-ModeLine).
    # A quick result is marked mode=quick and is never acceptance evidence.
    [switch] $Quick,
    # Development iteration: end the scenario at its first failed step instead of running the rest.
    [switch] $StopOnFailure,
    # Absolute outer child-process deadline from verify-local, already limited by the remaining game allowance.
    # Standalone omission keeps the existing startup defaults; this never extends the parent's timeout.
    [datetime] $ScenarioDeadlineUtc = [datetime]::MaxValue,
    # Parent results path survives child kill and _runs cleanup. Omission keeps standalone evidence beside result.
    [string] $StartupTelemetryPath = ''
)

# Runs one scenario and writes <OutputDirectory>\<scenario>-<time>\report.md with every step, its reply, the
# screenshots, and the Liberty log lines of the run (errors first), plus result.json (the machine-readable outcome).
# The last output line is "AUTOPILOT_RESULT <path to result.json>"; Run-Suite.ps1 and verify-local.ps1 read the status
# from that file, never from free text. Statuses (tools/autopilot/AutopilotLogic.psm1): PASS, NEEDS-REVIEW (passed, but
# [ERROR] log lines appeared during the run), FAIL, CRASH, ERROR.
# Scenario lines:
#   # comment
#   wait <ms>                    pause on the host
#   shot <name>                  Steam F12 screenshot -> <name>.png (a missing screenshot fails the step)
#   expect <regex> [seconds]     wait for a log line written after the latest engine command (default 20 s); fails the
#                                step if absent. The command's own log line only counts when the pattern needs its reply.
#   mark                         remember the current log position before a sequence of commands
#   expectmarked <regex> [seconds] wait for a log line written after mark, including between commands in the sequence
#   key <Keys name> [hold ms]    press a key in the game window (default 80 ms)
#   gpumem <label>               record the game process's dedicated/shared GPU memory, private bytes and working set into
#                                the report and measurements.json (fails the step when the GPU counters do not list the process)
#   anything else                an engine command (see "lf help"), sent through the command channel; a refused
#                                command (unknown, error, module not running, no reply) fails the step
# Any line may contain {probe:<check id>:<field>}: a value a probe found earlier in the same run (Resolve-ScenarioLine).
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'AutopilotLogic.psm1') -Force 3>$null
$name = [IO.Path]::GetFileNameWithoutExtension($Scenario)
$report = Join-Path $OutputDirectory ($name + '-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Force -Path $report | Out-Null
$steps = New-Object System.Collections.Generic.List[string]
$failedSteps = New-Object System.Collections.Generic.List[string]
$screenshots = New-Object System.Collections.Generic.List[string]
$measurements = New-Object System.Collections.Generic.List[object]
$executed = 0
$runnerError = ''
$startUtc = [DateTime]::UtcNow
$runLog = @()
$gameAlive = $false
$phase = 'initialization'
$startupSnapshot = Join-Path $report 'startup-events.jsonl'
$startupTelemetry = if ($StartupTelemetryPath) { $StartupTelemetryPath } else { $startupSnapshot }

function Add-Failure([string] $text) { $script:failedSteps.Add($text); $script:steps.Add("FAILED: $text") }

# One game: hold the machine-wide game lock for the whole scenario (tools/local/GameLock.psm1), so a scenario from another
# session never drives the same game at the same time. Inherited when verify-local or Run-Suite already holds it.
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Import-Module (Join-Path $repoRoot 'tools\local\GameLock.psm1') -Force
$gameLock = $null
# The simulated game (-AutopilotModule stub in the offline tests) needs neither the lock nor an installed build.
$realGame = [IO.Path]::GetFullPath($AutopilotModule) -ieq [IO.Path]::GetFullPath((Join-Path $PSScriptRoot 'Autopilot.psm1'))

try {
    if ($realGame) { $gameLock = Enter-GameLock "Run-Scenario $name ($repoRoot)" }
    # The installed build must be this worktree's, or the run tests someone else's code.
    $build = if ($realGame) { Read-InstalledBuild $GameDirectory } else { $null }
    if (-not $build) { if ($realGame) { $steps.Add('build: unknown (no installed-build.json; installed before the game lock recorded builds)') } }
    else {
        $steps.Add("build: $($build.repo) $($build.branch) $($build.commit)$(if ($build.dirty) { ' +uncommitted' }) $($build.note)")
        $ours = $build.repo -and ([IO.Path]::GetFullPath($build.repo).TrimEnd('\', '/') -ieq [IO.Path]::GetFullPath($repoRoot).TrimEnd('\', '/'))
        if (-not $ours -and -not $AllowOtherBuild) {
            throw "the installed build is not from this worktree ($repoRoot) but from '$($build.repo)' $($build.note); install yours first (tools/verify-local.ps1 -Only LOOP-package-install,<checks>)"
        }
    }
    Import-Module $AutopilotModule -Force 3>$null
    Set-AutopilotGame $GameDirectory
    $lines = @(Get-Content -LiteralPath $Scenario)
    if (-not (Get-GameProcess)) {
        $phase = 'startup'
        $settleSeconds = if ($Quick) { 2 } else { 12 }
        $startupArguments = @{ Attempts = 6 }
        # Keep third-party/simulated modules with the old signature compatible. The real helper accepts both.
        $startupParameters = (Get-Command Start-GameReady).Parameters
        if ($startupParameters.ContainsKey('TelemetryPath')) {
            $startupArguments.TelemetryPath = $startupTelemetry
            Write-Host "STARTUP_EVIDENCE $startupTelemetry"
        }
        if ($ScenarioDeadlineUtc -ne [datetime]::MaxValue -and $startupParameters.ContainsKey('DeadlineUtc')) {
            # Keep the unchanged settling period plus five seconds for serialization INSIDE the allowance.
            $startupArguments.DeadlineUtc = $ScenarioDeadlineUtc.ToUniversalTime().AddSeconds(-($settleSeconds + 5))
        }
        $attempts = Start-GameReady @startupArguments
        $steps.Add("launch: engine booted on attempt $attempts")
        # A blocking helper call or an older module can return late; never shorten acceptance settling or
        # begin a sleep that uses the report reserve. The observed boot stays in startup evidence on failure.
        if ($ScenarioDeadlineUtc -ne [datetime]::MaxValue -and
            ($ScenarioDeadlineUtc.ToUniversalTime() - (Get-Date).ToUniversalTime()).TotalSeconds -lt ($settleSeconds + 5)) {
            throw "STARTUP-TIMEOUT: engine readiness observed with insufficient remaining allowance for $settleSeconds s settling and result serialization"
        }
        # Let the first frames settle (streaming, the FusionFix dialog the engine acknowledges).
        # Quick probes already await a live player in their scenario; retain the full acceptance warm-up unchanged.
        Start-Sleep -Seconds $settleSeconds
    }
    $phase = 'scenario'
    $startUtc = [DateTime]::UtcNow
    # expect only accepts log lines written after the most recent engine command was sent (a line count, not a clock).
    $mark = @(Get-SessionLog).Count
    $sequenceMark = -1

    foreach ($raw in $lines) {
        $line = $raw.Trim()
        if ($line.Length -eq 0 -or $line.StartsWith('#')) { continue }
        $line = ConvertTo-ModeLine $line ([bool]$Quick)
        if ($null -eq $line) { continue }
        if ($StopOnFailure -and $failedSteps.Count -gt 0) { $steps.Add('stopped at the first failure (-StopOnFailure)'); break }
        $executed++
        try {
            $line = Resolve-ScenarioLine $line $ProbeDirectory
            $words = $line -split '\s+'
            switch ($words[0]) {
                'wait' { Start-Sleep -Milliseconds ([int]$words[1]); $steps.Add("wait $($words[1]) ms") }
                'mark' {
                    if ($words.Count -ne 1) { Add-Failure "unparseable mark line: $line"; break }
                    $sequenceMark = @(Get-SessionLog).Count
                    $steps.Add("mark log line $sequenceMark")
                }
                'shot' {
                    $file = Save-Screenshot (Join-Path $report ($words[1] + '.png'))
                    if (-not (Test-Path -LiteralPath $file) -or (Get-Item -LiteralPath $file).Length -eq 0) { throw "screenshot $($words[1]) was not written" }
                    $screenshots.Add((Split-Path -Leaf $file))
                    $steps.Add("shot $($words[1]) -> $(Split-Path -Leaf $file)")
                }
                'gpumem' {
                    if ($words.Count -ne 2) { Add-Failure "unparseable gpumem line (gpumem <label>): $line"; break }
                    $memory = Get-GameMemory
                    $memory.label = $words[1]
                    $memory.utc = [DateTime]::UtcNow.ToString('o')
                    $measurements.Add($memory)
                    $steps.Add("gpumem $($words[1]): gpu_dedicated_mb=$($memory.gpuDedicatedMB) gpu_shared_mb=$($memory.gpuSharedMB) private_mb=$($memory.privateMB) working_set_mb=$($memory.workingSetMB) system_cpu_pct=$($memory.systemCpuPercent) game_disk_busy_pct=$($memory.gameDiskBusyPercent) game_disk_queue=$($memory.gameDiskQueue)")
                    if ($null -eq $memory.gpuDedicatedMB) { Add-Failure "gpumem $($words[1]): the GPU counters do not list the game process" }
                }
                'key' { $hold = if ($words.Count -gt 2) { [int]$words[2] } else { 80 }; Send-GameKey $words[1] $hold; $steps.Add("key $($words[1]) $hold ms") }
                { $_ -eq 'expect' -or $_ -eq 'expectmarked' } {
                    $fromSequence = $words[0] -eq 'expectmarked'
                    $expect = ConvertFrom-ExpectLine ($line -replace '^expectmarked\b', 'expect')
                    if (-not $expect) { Add-Failure "unparseable expect line: $line"; break }
                    if ($fromSequence -and $sequenceMark -lt 0) { Add-Failure "expectmarked without mark: $line"; break }
                    $from = if ($fromSequence) { $sequenceMark } else { $mark }
                    $deadline = (Get-Date).AddSeconds($expect.TimeoutSeconds)
                    $hit = $null
                    while (-not $hit -and (Get-Date) -lt $deadline) {
                        $hit = Find-ExpectedLine @(Get-SessionLog) $from $expect.Pattern
                        if (-not $hit) {
                            if (-not (Get-GameProcess)) { throw 'game exited while waiting' }
                            # The engine logs engine_status every 30 s; silence for $FrozenSeconds means a frozen game or a
                            # blocking dialog, and waiting out a long expect (or the run's timeout) cannot help.
                            if (Test-GameFrozen $FrozenSeconds) { throw "GAME-FROZEN: the engine wrote nothing to its log for $FrozenSeconds s$(if ($d = Get-GameDialog) { " (dialog: $d)" })" }
                            Start-Sleep -Milliseconds 500
                        }
                    }
                    if ($hit) { $steps.Add("$($words[0]) $($expect.Pattern): OK $hit") } else { Add-Failure "$($words[0]) $($expect.Pattern): no new log line in $($expect.TimeoutSeconds) s" }
                }
                default {
                    $mark = @(Get-SessionLog).Count
                    $reply = @(Invoke-EngineCommand @($line))
                    $steps.Add(($reply -join ' | '))
                    if (Test-CommandFailed $reply) { Add-Failure "command refused: $line => $($reply -join ' | ')" }
                }
            }
        }
        catch {
            Add-Failure "$line => EXCEPTION $($_.Exception.Message)"
            if (-not (Get-GameProcess)) { $steps.Add('game exited; scenario aborted'); break }
            # A frozen game answers nothing: every further step would only wait out its own timeout.
            if ($_.Exception.Message -like 'GAME-FROZEN*') { $runnerError = $_.Exception.Message; $steps.Add('game frozen; scenario aborted'); break }
        }
    }
}
catch { $runnerError = $_.Exception.Message; $steps.Add("RUNNER ERROR: $runnerError") }

try {
    $gameAlive = [bool](Get-GameProcess)
    $since = $startUtc.ToString('yyyy-MM-ddTHH:mm:ss')
    $runLog = @(Get-SessionLog | Where-Object { $_.Length -ge 19 -and [string]::CompareOrdinal($_.Substring(0, 19), $since) -ge 0 })
}
catch { if (-not $runnerError) { $runnerError = "could not read the game state: $($_.Exception.Message)" } }
$errors = @($runLog | Where-Object { $_ -match '\[ERROR\]' })
$startupEvents = @()
if (Test-Path -LiteralPath $startupTelemetry) {
    try { $startupEvents = @(Get-Content -LiteralPath $startupTelemetry | ForEach-Object { $_ | ConvertFrom-Json }) }
    catch {
        $message = "could not read startup telemetry: $($_.Exception.Message)"
        Write-Host $message
        if (-not $runnerError) { $runnerError = $message }
    }
    if ([IO.Path]::GetFullPath($startupTelemetry) -ne [IO.Path]::GetFullPath($startupSnapshot)) {
        try { Copy-Item -LiteralPath $startupTelemetry -Destination $startupSnapshot }
        catch { Write-Host "startup snapshot failed: $($_.Exception.Message)"; if (-not $runnerError) { $runnerError = "startup snapshot failed: $($_.Exception.Message)" } }
    }
}
$status = Get-ScenarioStatus $executed $failedSteps.Count $gameAlive $runnerError
$status = Get-ReviewStatus $status $errors.Count
$summary = "$status; steps=$executed failed=$($failedSteps.Count) logErrors=$($errors.Count) gameAlive=$gameAlive"

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("# Scenario $name")
$lines.Add('')
$lines.Add("- Result: $status")
if ($Quick) { $lines.Add('- Mode: quick (development iteration, not acceptance evidence)') }
$lines.Add("- Steps: $executed, failed: $($failedSteps.Count)")
$lines.Add("- Game alive at end: $gameAlive")
$lines.Add("- Log errors during run: $($errors.Count)")
$lines.Add("- Phase at completion: $phase")
if ($startupEvents.Count -gt 0) { $lines.Add('- Startup evidence: startup-events.jsonl (persisted during readiness polling)') }
if ($runnerError) { $lines.Add("- Runner error: $runnerError") }
$lines.Add('')
$lines.Add('## Failed steps')
foreach ($f in $failedSteps) { $lines.Add("- $f") }
$lines.Add('')
$lines.Add('## Steps')
foreach ($step in $steps) { $lines.Add("- $step") }
$lines.Add('')
$lines.Add('## Errors')
foreach ($e in $errors) { $lines.Add("    $e") }
$lines.Add('')
$lines.Add('## Log')
foreach ($l in $runLog) { $lines.Add("    $l") }
[IO.File]::WriteAllLines((Join-Path $report 'report.md'), $lines)
[IO.File]::WriteAllLines((Join-Path $report 'run.log'), [string[]]$runLog)

$result = [ordered]@{
    scenario = $name
    mode = $(if ($Quick) { 'quick' } else { 'full' })
    status = $status
    summary = $summary
    steps = $executed
    failedSteps = @($failedSteps)
    screenshots = @($screenshots)
    logErrors = $errors.Count
    gameAlive = $gameAlive
    runnerError = $runnerError
    phase = $phase
    startup = @($startupEvents)
    evidence = @($(if (Test-Path -LiteralPath $startupSnapshot) { 'startup-events.jsonl' }))
    startedUtc = $startUtc.ToString('o')
    finishedUtc = [DateTime]::UtcNow.ToString('o')
}
if ($measurements.Count -gt 0) {
    # `gpumem <label>` lines: tools/perf/Measure-Stage1.ps1 reads this next to run.log.
    [IO.File]::WriteAllText((Join-Path $report 'measurements.json'), (ConvertTo-Json -InputObject $measurements.ToArray() -Depth 4), (New-Object Text.UTF8Encoding($false)))
}
$resultPath = Join-Path $report 'result.json'
[IO.File]::WriteAllText($resultPath, ($result | ConvertTo-Json -Depth 4), (New-Object Text.UTF8Encoding($false)))
if ($StopGameAfter) { try { Stop-Game } catch { Write-Host "Stop-Game failed: $($_.Exception.Message)" } }
Exit-GameLock $gameLock
Write-Host "scenario ${name}: $summary; report $report"
Write-Output "AUTOPILOT_RESULT $resultPath"

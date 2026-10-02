# Dot-sourced by Autopilot.psm1. The readiness loop is separate so focused tests execute it with stubbed
# time/process/launch functions, without importing the native/UI/audio host or launching Steam.
function Get-StartupUtcNow { (Get-Date).ToUniversalTime() }

function Write-StartupEvent([string] $Path, [int] $Attempt, [string] $Event, [hashtable] $Observed = @{}) {
    if (-not $Path) { return }
    $record = [ordered]@{ utc = (Get-StartupUtcNow).ToString('o'); attempt = $Attempt; event = $Event }
    foreach ($key in $Observed.Keys) { $record[$key] = $Observed[$key] }
    try {
        [IO.File]::AppendAllText($Path, (($record | ConvertTo-Json -Compress -Depth 5) + "`n"), (New-Object Text.UTF8Encoding($false)))
    }
    catch {
        Write-Host "startup telemetry write failed: $($_.Exception.Message)"
        throw "STARTUP-EVIDENCE-ERROR: $($_.Exception.Message)"
    }
}

# No shared deadline/path means the original standalone retry/boot defaults. Deadline is absolute UTC:
# a relaunch never receives a fresh total allowance. Observed absence/exits do not name a crash cause.
function Start-GameReady([int] $Attempts = 5, [int] $BootTimeoutSeconds = 240, [int] $NotSeenSeconds = 90,
    [datetime] $DeadlineUtc = [datetime]::MaxValue, [string] $TelemetryPath = '') {
    if ($DeadlineUtc -ne [datetime]::MaxValue) { $DeadlineUtc = $DeadlineUtc.ToUniversalTime() }
    $attempt = 0; $lastAttempt = 0
    try {
        Write-StartupEvent $TelemetryPath 0 'preflight_started'
        if ((Get-StartupUtcNow) -ge $DeadlineUtc) { throw 'STARTUP-TIMEOUT: shared scenario deadline reached before audio preflight' }
        Assert-AudioOutput
        for ($attempt = 1; $attempt -le $Attempts; $attempt++) {
            if ((Get-StartupUtcNow) -ge $DeadlineUtc) { throw 'STARTUP-TIMEOUT: shared scenario deadline reached before another launch' }
            $lastAttempt = $attempt
            Write-StartupEvent $TelemetryPath $attempt 'attempt_started'
            Stop-Game
            if ((Get-StartupUtcNow) -ge $DeadlineUtc) { throw 'STARTUP-TIMEOUT: shared scenario deadline reached during launch cleanup' }
            $script:LaunchedUtc = Get-StartupUtcNow
            Write-StartupEvent $TelemetryPath $attempt 'launch_requested' @{ deadlineUtc = $DeadlineUtc.ToString('o') }
            Start-Process 'steam://rungameid/12210'
            $started = Get-StartupUtcNow
            $bootDeadline = $started.AddSeconds($BootTimeoutSeconds)
            $seen = $false; $lostAt = $null; $lastPid = $null; $lastDialog = $null
            $reason = 'boot_deadline'
            while ((Get-StartupUtcNow) -lt $bootDeadline) {
                $now = Get-StartupUtcNow
                $remaining = ([Math]::Min($bootDeadline.Ticks, $DeadlineUtc.Ticks) - $now.Ticks) / [TimeSpan]::TicksPerMillisecond
                if ($remaining -le 0) { break }
                Start-Sleep -Milliseconds ([int][Math]::Min(3000, [Math]::Ceiling($remaining)))
                if ((Get-StartupUtcNow) -ge $DeadlineUtc) { break }
                $process = Get-GameProcess
                if ($process) {
                    if (-not $seen) { Write-AutopilotLaunch $script:Game $process }
                    if ($lastPid -ne $process.Id) {
                        $observed = @{ pid = $process.Id }
                        if ($process.PSObject.Properties['StartTime']) {
                            try { $observed.processStartedUtc = $process.StartTime.ToUniversalTime().ToString('o') }
                            catch { $observed.startTimeError = $_.Exception.Message; Write-Host "startup: could not read process start time: $($_.Exception.Message)" }
                        }
                        Write-StartupEvent $TelemetryPath $attempt 'process_observed' $observed
                        $lastPid = $process.Id
                    }
                }
                $boot = Get-SessionLog | Where-Object { $_ -match 'engine_booted' } | Select-Object -First 1
                if ($boot) {
                    Write-StartupEvent $TelemetryPath $attempt 'engine_booted' @{ pid = $lastPid; line = [string]$boot }
                    # A blocking process/log read can outlast the allowance. Retain what was observed, but
                    # do not convert readiness observed after the deadline into a successful launch.
                    if ((Get-StartupUtcNow) -ge $DeadlineUtc) { break }
                    return $attempt
                }
                if ($process) {
                    $seen = $true; $lostAt = $null
                    $dialog = Get-GameDialog
                    if ($dialog -and $dialog -ne $lastDialog) {
                        Write-StartupEvent $TelemetryPath $attempt 'dialog_observed' @{ pid = $process.Id; title = [string]$dialog }
                        $lastDialog = $dialog
                    }
                    if (-not $dialog) { $lastDialog = $null }
                    if ($dialog -and $dialog -match '(?i)fatal') {
                        Write-StartupEvent $TelemetryPath $attempt 'attempt_finished' @{ reason = 'fatal_dialog'; seen = $seen }
                        Stop-Game
                        throw "GAME-UNAVAILABLE: GTA IV showed '$dialog' before the engine started (a sound card error means no audio output device)"
                    }
                    continue
                }
                if (-not $seen -and ((Get-StartupUtcNow) - $started).TotalSeconds -gt $NotSeenSeconds) { $reason = 'process_not_seen'; break }
                if ($seen) {
                    if (-not $lostAt) {
                        $lostAt = Get-StartupUtcNow
                        Write-StartupEvent $TelemetryPath $attempt 'process_absent' @{ lastPid = $lastPid }
                    }
                    elseif (((Get-StartupUtcNow) - $lostAt).TotalSeconds -gt 20) { $reason = 'process_remained_absent'; break }
                }
            }
            if ((Get-StartupUtcNow) -ge $DeadlineUtc) {
                Write-StartupEvent $TelemetryPath $attempt 'attempt_finished' @{ reason = 'shared_deadline'; seen = $seen }
                throw 'STARTUP-TIMEOUT: shared scenario deadline reached before engine readiness'
            }
            Write-StartupEvent $TelemetryPath $attempt 'attempt_finished' @{ reason = $reason; seen = $seen }
            $nextAction = if ($attempt -lt $Attempts) { 'relaunching' } else { 'attempts exhausted' }
            Write-Host "attempt $attempt failed (seen=$seen; reason=$reason); $nextAction"
        }
        throw "GAME-UNAVAILABLE: GTA IV did not reach the engine after $Attempts attempts"
    }
    catch {
        $startupFailure = $_
        # The for-loop counter advances past the last permitted attempt on exhaustion; that is not a launch.
        try { Write-StartupEvent $TelemetryPath $lastAttempt 'startup_failed' @{ reason = $startupFailure.Exception.Message } }
        catch { Write-Host "startup failure retained: $($startupFailure.Exception.Message); telemetry also failed: $($_.Exception.Message)" }
        throw $startupFailure
    }
}

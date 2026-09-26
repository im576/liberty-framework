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
    [string] $AutopilotModule = (Join-Path $PSScriptRoot 'Autopilot.psm1')
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
#   key <Keys name> [hold ms]    press a key in the game window (default 80 ms)
#   anything else                an engine command (see "lf help"), sent through the command channel; a refused
#                                command (unknown, error, module not running, no reply) fails the step
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'AutopilotLogic.psm1') -Force 3>$null
$name = [IO.Path]::GetFileNameWithoutExtension($Scenario)
$report = Join-Path $OutputDirectory ($name + '-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Force -Path $report | Out-Null
$steps = New-Object System.Collections.Generic.List[string]
$failedSteps = New-Object System.Collections.Generic.List[string]
$screenshots = New-Object System.Collections.Generic.List[string]
$executed = 0
$runnerError = ''
$startUtc = [DateTime]::UtcNow
$runLog = @()
$gameAlive = $false

function Add-Failure([string] $text) { $script:failedSteps.Add($text); $script:steps.Add("FAILED: $text") }

try {
    Import-Module $AutopilotModule -Force 3>$null
    Set-AutopilotGame $GameDirectory
    $lines = @(Get-Content -LiteralPath $Scenario)
    if (-not (Get-GameProcess)) {
        $attempts = Start-GameReady -Attempts 6
        $steps.Add("launch: engine booted on attempt $attempts")
        # Let the first frames settle (streaming, the FusionFix dialog the engine acknowledges).
        Start-Sleep -Seconds 12
    }
    $startUtc = [DateTime]::UtcNow
    # expect only accepts log lines written after the most recent engine command was sent (a line count, not a clock).
    $mark = @(Get-SessionLog).Count

    foreach ($raw in $lines) {
        $line = $raw.Trim()
        if ($line.Length -eq 0 -or $line.StartsWith('#')) { continue }
        $words = $line -split '\s+'
        $executed++
        try {
            switch ($words[0]) {
                'wait' { Start-Sleep -Milliseconds ([int]$words[1]); $steps.Add("wait $($words[1]) ms") }
                'shot' {
                    $file = Save-Screenshot (Join-Path $report ($words[1] + '.png'))
                    if (-not (Test-Path -LiteralPath $file) -or (Get-Item -LiteralPath $file).Length -eq 0) { throw "screenshot $($words[1]) was not written" }
                    $screenshots.Add((Split-Path -Leaf $file))
                    $steps.Add("shot $($words[1]) -> $(Split-Path -Leaf $file)")
                }
                'key' { $hold = if ($words.Count -gt 2) { [int]$words[2] } else { 80 }; Send-GameKey $words[1] $hold; $steps.Add("key $($words[1]) $hold ms") }
                'expect' {
                    $expect = ConvertFrom-ExpectLine $line
                    if (-not $expect) { Add-Failure "unparseable expect line: $line"; break }
                    $deadline = (Get-Date).AddSeconds($expect.TimeoutSeconds)
                    $hit = $null
                    while (-not $hit -and (Get-Date) -lt $deadline) {
                        $hit = Find-ExpectedLine @(Get-SessionLog) $mark $expect.Pattern
                        if (-not $hit) {
                            if (-not (Get-GameProcess)) { throw 'game exited while waiting' }
                            Start-Sleep -Milliseconds 500
                        }
                    }
                    if ($hit) { $steps.Add("expect $($expect.Pattern): OK $hit") } else { Add-Failure "expect $($expect.Pattern): no new log line in $($expect.TimeoutSeconds) s" }
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
$status = Get-ScenarioStatus $executed $failedSteps.Count $gameAlive $runnerError
$status = Get-ReviewStatus $status $errors.Count
$summary = "$status; steps=$executed failed=$($failedSteps.Count) logErrors=$($errors.Count) gameAlive=$gameAlive"

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("# Scenario $name")
$lines.Add('')
$lines.Add("- Result: $status")
$lines.Add("- Steps: $executed, failed: $($failedSteps.Count)")
$lines.Add("- Game alive at end: $gameAlive")
$lines.Add("- Log errors during run: $($errors.Count)")
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
    status = $status
    summary = $summary
    steps = $executed
    failedSteps = @($failedSteps)
    screenshots = @($screenshots)
    logErrors = $errors.Count
    gameAlive = $gameAlive
    runnerError = $runnerError
    startedUtc = $startUtc.ToString('o')
    finishedUtc = [DateTime]::UtcNow.ToString('o')
}
$resultPath = Join-Path $report 'result.json'
[IO.File]::WriteAllText($resultPath, ($result | ConvertTo-Json -Depth 4), (New-Object Text.UTF8Encoding($false)))
if ($StopGameAfter) { try { Stop-Game } catch { Write-Host "Stop-Game failed: $($_.Exception.Message)" } }
Write-Host "scenario ${name}: $summary; report $report"
Write-Output "AUTOPILOT_RESULT $resultPath"

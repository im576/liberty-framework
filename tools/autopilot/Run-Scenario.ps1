param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    # A scenario file (tools/autopilot/scenarios/*.txt).
    [Parameter(Mandatory = $true)][string] $Scenario,
    # Where the report folder is created (outside the repository).
    [Parameter(Mandatory = $true)][string] $OutputDirectory,
    # Keep the game running afterwards (default: leave it running for the next scenario).
    [switch] $StopGameAfter
)

# Runs one scenario and writes <OutputDirectory>\<scenario>-<time>\report.md with every step, its reply, the
# screenshots, and the Liberty log lines of the run (errors first).
# Scenario lines:
#   # comment
#   wait <ms>                    pause on the host
#   shot <name>                  Steam F12 screenshot -> <name>.png
#   expect <regex> [seconds]     wait for a log line from this run (default 20 s); fails the step if absent
#   key <Keys name>              press a key in the game window
#   anything else                an engine command (see "lf help"), sent through the command channel
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'Autopilot.psm1') -Force 3>$null
Set-AutopilotGame $GameDirectory
$name = [IO.Path]::GetFileNameWithoutExtension($Scenario)
$report = Join-Path $OutputDirectory ($name + '-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Force -Path $report | Out-Null
$steps = New-Object System.Collections.Generic.List[string]
$failed = 0

if (-not (Get-GameProcess)) {
    $attempts = Start-GameReady -Attempts 6
    $steps.Add("launch: engine booted on attempt $attempts")
    # Let the first frames settle (streaming, the FusionFix dialog the engine acknowledges).
    Start-Sleep -Seconds 12
}
$startUtc = [DateTime]::UtcNow
# expect only accepts log lines written after the most recent engine command.
$lastCommandUtc = $startUtc

foreach ($raw in Get-Content -LiteralPath $Scenario) {
    $line = $raw.Trim()
    if ($line.Length -eq 0 -or $line.StartsWith('#')) { continue }
    $words = $line -split '\s+'
    try {
        switch ($words[0]) {
            'wait' { Start-Sleep -Milliseconds ([int]$words[1]); $steps.Add("wait $($words[1]) ms") }
            'shot' { $file = Save-Screenshot (Join-Path $report ($words[1] + '.png')); $steps.Add("shot $($words[1]) -> $(Split-Path -Leaf $file)") }
            'key' { Send-GameKey $words[1]; $steps.Add("key $($words[1])") }
            'expect' {
                # expect <regex, optionally "quoted" when it contains spaces> [seconds]
                $parsed = [regex]::Match($line, '^expect\s+(?:"(?<q>[^"]+)"|(?<p>\S+))(?:\s+(?<t>\d+))?$')
                $words = @('expect', $(if ($parsed.Groups['q'].Success) { $parsed.Groups['q'].Value } else { $parsed.Groups['p'].Value }))
                $timeout = if ($parsed.Groups['t'].Success) { [int]$parsed.Groups['t'].Value } else { 20 }
                $deadline = (Get-Date).AddSeconds($timeout)
                $hit = $null
                while (-not $hit -and (Get-Date) -lt $deadline) {
                    $since = $lastCommandUtc.AddSeconds(-1).ToString('yyyy-MM-ddTHH:mm:ss')
                    $hit = Get-SessionLog | Where-Object { [string]::CompareOrdinal($_.Substring(0, [Math]::Min(19, $_.Length)), $since) -ge 0 -and $_ -match $words[1] } | Select-Object -First 1
                    if (-not $hit) { Start-Sleep -Milliseconds 500 }
                }
                if ($hit) { $steps.Add("expect $($words[1]): OK $hit") } else { $failed++; $steps.Add("expect $($words[1]): FAILED (no line in $timeout s)") }
            }
            default {
                $lastCommandUtc = [DateTime]::UtcNow
                $reply = Invoke-EngineCommand @($line)
                $steps.Add(($reply -join ' | '))
                if (($reply -join ' ') -match '=> (error|unknown command|module .* is not running)') { $failed++ }
            }
        }
    }
    catch { $failed++; $steps.Add("$line => EXCEPTION $($_.Exception.Message)"); if (-not (Get-GameProcess)) { $steps.Add('game exited; scenario aborted'); break } }
}

$since = $startUtc.ToString('yyyy-MM-ddTHH:mm:ss')
$runLog = Get-SessionLog | Where-Object { $_.Length -ge 19 -and [string]::CompareOrdinal($_.Substring(0, 19), $since) -ge 0 }
$errors = $runLog | Where-Object { $_ -match '\[ERROR\]' }
$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("# Scenario $name")
$lines.Add('')
$lines.Add("- Result: " + $(if ($failed -eq 0) { 'PASS' } else { "FAIL ($failed failed steps)" }))
$lines.Add("- Game alive at end: " + [bool](Get-GameProcess))
$lines.Add("- Log errors during run: $(@($errors).Count)")
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
if ($StopGameAfter) { Stop-Game }
Write-Host "scenario ${name}: $(if ($failed -eq 0) { 'PASS' } else { "FAIL ($failed)" }); report $report"

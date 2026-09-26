# Autopilot decisions that must be right for a PASS to mean anything: which log line an `expect` may accept, which
# command replies are failures, and a scenario's final status. Pure functions with no Windows dependency, so the cloud
# container tests them (tools/tests/AutopilotLogic.Tests.ps1). Run-Scenario.ps1 and Run-Suite.ps1 use them.
# Compatible with Windows PowerShell 5.1 and PowerShell 7.

# Every engine command is logged as: command source=<source> line="<command>" reply="<reply>"
$script:CommandLogLine = '\bcommand source=\S+ line="(?<line>[^"]*)"'

# `expect <regex, optionally "quoted" when it contains spaces> [seconds]` -> @{ Pattern; TimeoutSeconds } or $null.
function ConvertFrom-ExpectLine([string] $Line) {
    $parsed = [regex]::Match($Line.Trim(), '^expect\s+(?:"(?<q>[^"]+)"|(?<p>\S+))(?:\s+(?<t>\d+))?$')
    if (-not $parsed.Success) { return $null }
    $pattern = if ($parsed.Groups['q'].Success) { $parsed.Groups['q'].Value } else { $parsed.Groups['p'].Value }
    $timeout = if ($parsed.Groups['t'].Success) { [int]$parsed.Groups['t'].Value } else { 20 }
    try { [void][regex]::new($pattern) } catch { return $null }
    return @{ Pattern = $pattern; TimeoutSeconds = $timeout }
}

# The first log line an `expect` accepts, or $null.
#   $Lines: the session log; $Mark: how many of those lines existed before the scenario's latest engine command was sent.
# Only lines written after the mark count (no stale line from an earlier command or scenario can match). A command's
# own log line counts only when the pattern needs its reply: a pattern that already matches the command text as typed
# would pass without the engine doing anything.
function Find-ExpectedLine([string[]] $Lines, [int] $Mark, [string] $Pattern) {
    if ($null -eq $Lines) { return $null }
    # A log that shrank (rotated or truncated) since the mark: every line in it is newer than the mark.
    $start = if ($Mark -gt $Lines.Count) { 0 } else { $Mark }
    for ($i = $start; $i -lt $Lines.Count; $i++) {
        $candidate = $Lines[$i]
        if ($candidate -notmatch $Pattern) { continue }
        $command = [regex]::Match($candidate, $script:CommandLogLine)
        if ($command.Success -and ('line="' + $command.Groups['line'].Value + '"') -match $Pattern) { continue }
        return $candidate
    }
    return $null
}

# Reply lines from the command channel ("<command> => <reply>"). True when the engine refused or failed the command.
function Test-CommandFailed([string[]] $Reply) {
    if ($null -eq $Reply -or @($Reply).Count -eq 0) { return $true }
    $text = ($Reply -join "`n")
    return $text -match '=> (error\b|unknown command|module \S+ is not running)'
}

# The scenario's final status:
#   ERROR  the runner could not run it (no launch, unreadable scenario, nothing executed)
#   CRASH  the game exited during or at the end of the run
#   FAIL   a step failed (expect not seen, command refused, screenshot missing, exception)
#   PASS   every step passed and the game is still running
# Log errors never turn a FAIL into anything else; with a PASS they are reported for review (see Get-ReviewStatus).
function Get-ScenarioStatus([int] $ExecutedSteps, [int] $FailedSteps, [bool] $GameAlive, [string] $RunnerError) {
    if ($RunnerError) { return 'ERROR' }
    if ($ExecutedSteps -le 0) { return 'ERROR' }
    if (-not $GameAlive) { return 'CRASH' }
    if ($FailedSteps -gt 0) { return 'FAIL' }
    return 'PASS'
}

# A PASS with [ERROR] log lines during the run needs a reviewer's look before it counts.
function Get-ReviewStatus([string] $Status, [int] $LogErrors) {
    if ($Status -ne 'PASS') { return $Status }
    if ($LogErrors -gt 0) { return 'NEEDS-REVIEW' }
    return 'PASS'
}

# A scenario line may take a value a probe of the same verify-local run found: {probe:<check id>:<field>} is replaced
# by that field of <ProbeDirectory>\<check id>.json (the first element when the field is a list). The check then declares
# "needs": ["<check id>"] so the probe runs first. Missing report, field or value throws: the step fails with the reason,
# and the scenario never runs with a guessed value.
function Resolve-ScenarioLine([string] $Line, [string] $ProbeDirectory) {
    $pattern = '\{probe:(?<id>[A-Za-z0-9-]+):(?<field>[A-Za-z0-9_]+)\}'
    $match = [regex]::Match($Line, $pattern)
    while ($match.Success) {
        $id = $match.Groups['id'].Value
        $field = $match.Groups['field'].Value
        if (-not $ProbeDirectory) { throw "needs $id results from this run (verify-local.ps1 runs it first)" }
        $path = Join-Path $ProbeDirectory ($id + '.json')
        if (-not (Test-Path -LiteralPath $path)) { throw "needs $id results from this run: $path is missing" }
        $report = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
        if (-not $report.PSObject.Properties[$field]) { throw "$id report has no field '$field'" }
        $value = @(@($report.$field) | Where-Object { $null -ne $_ -and [string]$_ -ne '' }) | Select-Object -First 1
        if ($null -eq $value) { throw "$id found no value for '$field' (empty)" }
        $text = [string]$value
        if ($text -notmatch '^[A-Za-z0-9_.\-]+$') { throw "$id value '$text' for '$field' is not a plain word" }
        $Line = $Line.Substring(0, $match.Index) + $text + $Line.Substring($match.Index + $match.Length)
        $match = [regex]::Match($Line, $pattern)
    }
    return $Line
}

# Run-Scenario.ps1 ends its output with exactly one line "AUTOPILOT_RESULT <path to result.json>". The suite reads the
# status from that file, never from free text; anything missing or unreadable is ERROR.
function Read-ScenarioResult([string] $Output) {
    $marker = [regex]::Matches($Output, '(?m)^AUTOPILOT_RESULT (?<path>.+?)\s*$')
    if ($marker.Count -ne 1) { return @{ Status = 'ERROR'; Detail = "expected one AUTOPILOT_RESULT line, found $($marker.Count)"; Path = '' } }
    $path = $marker[0].Groups['path'].Value
    if (-not (Test-Path -LiteralPath $path)) { return @{ Status = 'ERROR'; Detail = "result file missing: $path"; Path = $path } }
    try { $result = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json }
    catch { return @{ Status = 'ERROR'; Detail = "result file unreadable: $($_.Exception.Message)"; Path = $path } }
    $known = @('PASS', 'FAIL', 'CRASH', 'ERROR', 'NEEDS-REVIEW')
    if ($known -notcontains [string]$result.status) { return @{ Status = 'ERROR'; Detail = "unknown status '$($result.status)'"; Path = $path } }
    return @{ Status = [string]$result.status; Detail = [string]$result.summary; Path = $path }
}

Export-ModuleMember -Function ConvertFrom-ExpectLine, Find-ExpectedLine, Test-CommandFailed, Get-ScenarioStatus, Get-ReviewStatus, Read-ScenarioResult, Resolve-ScenarioLine

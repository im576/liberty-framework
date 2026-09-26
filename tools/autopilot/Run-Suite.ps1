param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [Parameter(Mandatory = $true)][string] $OutputDirectory,
    # Scenario names (without .txt); default: every scenario in tools/autopilot/scenarios.
    [string[]] $Scenarios,
    # Passed through to Run-Scenario.ps1 (a stub game module for -Simulate tests).
    [string] $AutopilotModule = (Join-Path $PSScriptRoot 'Autopilot.psm1'),
    # Where the scenario files are (default tools/autopilot/scenarios).
    [string] $ScenarioDirectory = (Join-Path $PSScriptRoot 'scenarios'),
    # Also return the result rows as objects (for scripts).
    [switch] $PassThru
)

# Runs scenarios back to back in one game session (relaunching only if the game exits) and prints a summary table.
# Each scenario's status comes from its result.json (AUTOPILOT_RESULT line), never from matching words in its output;
# a scenario that throws or writes no result is ERROR, and the suite always continues with the next one.
# With -PassThru returns the result rows (Scenario, Result, Detail, Report). Exits 1 unless every scenario is PASS.
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'AutopilotLogic.psm1') -Force 3>$null
if (-not $Scenarios) { $Scenarios = Get-ChildItem -LiteralPath $ScenarioDirectory -Filter '*.txt' | Sort-Object Name | ForEach-Object { $_.BaseName } }
$results = @()
foreach ($name in $Scenarios) {
    $file = Join-Path $ScenarioDirectory "$name.txt"
    if (-not (Test-Path -LiteralPath $file)) {
        $results += [pscustomobject]@{ Scenario = $name; Result = 'ERROR'; Detail = "no scenario file $file"; Report = '' }
        continue
    }
    try {
        $output = & (Join-Path $PSScriptRoot 'Run-Scenario.ps1') -GameDirectory $GameDirectory -Scenario $file -OutputDirectory $OutputDirectory -AutopilotModule $AutopilotModule 6>&1 | Out-String
        $read = Read-ScenarioResult $output
    }
    catch { $read = @{ Status = 'ERROR'; Detail = "Run-Scenario threw: $($_.Exception.Message)"; Path = '' } }
    $report = if ($read.Path) { Split-Path -Parent $read.Path } else { '' }
    $results += [pscustomobject]@{ Scenario = $name; Result = $read.Status; Detail = $read.Detail; Report = $report }
}
$results | Format-Table -AutoSize | Out-String -Width 200 | Write-Host
if ($PassThru) { $results }
if (@($results | Where-Object { $_.Result -ne 'PASS' }).Count -gt 0) { exit 1 }

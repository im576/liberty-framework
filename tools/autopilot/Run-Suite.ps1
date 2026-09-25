param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [Parameter(Mandatory = $true)][string] $OutputDirectory,
    # Scenario names (without .txt); default: every scenario in tools/autopilot/scenarios.
    [string[]] $Scenarios
)

# Runs scenarios back to back in one game session (relaunching only if the game exits) and prints a summary table.
$ErrorActionPreference = 'Stop'
$folder = Join-Path $PSScriptRoot 'scenarios'
if (-not $Scenarios) { $Scenarios = Get-ChildItem -LiteralPath $folder -Filter '*.txt' | Sort-Object Name | ForEach-Object { $_.BaseName } }
$results = @()
foreach ($name in $Scenarios) {
    $output = & (Join-Path $PSScriptRoot 'Run-Scenario.ps1') -GameDirectory $GameDirectory -Scenario (Join-Path $folder "$name.txt") -OutputDirectory $OutputDirectory 6>&1 | Out-String
    $status = if ($output -match 'PASS') { 'PASS' } elseif ($output -match 'FAIL \((\d+)\)') { "FAIL ($($Matches[1]))" } else { 'ERROR' }
    $report = if ($output -match 'report (.+)$') { $Matches[1].Trim() } else { '' }
    $results += [pscustomobject]@{ Scenario = $name; Result = $status; Report = $report }
}
$results | Format-Table -AutoSize | Out-String -Width 200

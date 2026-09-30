param(
    # Report folders of a range run (every folder holding a run.log is read), e.g. results-local\<run>\T042-gunplay-range.
    [Parameter(Mandatory = $true)][string[]] $Reports,
    # Where to write the JSON summary and, optionally, the markdown table.
    [string] $Out = 'gunplay-range.json',
    [string] $Markdown
)

# T-042: delivered spread from a range run. The game side is scenario stage1-gunplay-range (`range` command); this only reads its log.
#   Measure-GunplayRange.ps1 -Reports results-local\<run>\T042-gunplay-range -Out range.json -Markdown range.md
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'GunplayRange.psm1') -Force 3>$null
$Reports = @($Reports | ForEach-Object { $_ -split ',' } | Where-Object { $_ })
$lines = New-Object System.Collections.Generic.List[string]
foreach ($path in $Reports) {
    $root = (Resolve-Path -LiteralPath $path).Path
    foreach ($log in Get-ChildItem -LiteralPath $root -Recurse -Filter run.log -File) { foreach ($line in [IO.File]::ReadAllLines($log.FullName)) { $lines.Add($line) } }
}
$shots = ConvertFrom-RangeLog $lines.ToArray()
if ($shots.Count -eq 0) { throw "no range_shot lines under: $($Reports -join ', ')" }
$summary = Get-RangeSummary $shots
$catalog = Get-Content -LiteralPath (Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) 'config\weapon-catalog.json') -Raw | ConvertFrom-Json
$labels = @{}; foreach ($entry in @($catalog.entries)) { $labels[[int]$entry.weaponId] = [string]$entry.label }
$json = [ordered]@{ schemaVersion = 1; generatedUtc = [DateTime]::UtcNow.ToString('o'); shots = $shots.Count; summary = $summary }
[IO.File]::WriteAllText([IO.Path]::GetFullPath($Out), ($json | ConvertTo-Json -Depth 6), (New-Object Text.UTF8Encoding($false)))
Write-Host "wrote $Out ($($shots.Count) shots)"
$table = Format-RangeMarkdown $summary $labels
if ($Markdown) { [IO.File]::WriteAllLines([IO.Path]::GetFullPath($Markdown), [string[]]$table, (New-Object Text.UTF8Encoding($false))); Write-Host "wrote $Markdown" } else { $table }

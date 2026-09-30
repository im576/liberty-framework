param(
    # Summarise: autopilot report folders of Stage 1 scenarios, or any folder above them (every folder holding a run.log is
    # read; results-local\<run>\_runs and results-local\<run>\<check id> both work). Writes one JSON summary per condition.
    [string[]] $Reports,
    # Where the summary JSON goes (default: stage1-summary.json in the current folder). With mixed mod-on and mod-off
    # reports, "<name>.mod-on.json" and "<name>.mod-off.json" are written next to it.
    [string] $Out = 'stage1-summary.json',
    # Compare: a mod-on and a mod-off summary JSON -> markdown table with the Stage 1 budget verdicts.
    [string] $On,
    [string] $Off,
    [string] $Markdown,
    # Label the reports 'mod-on' or 'mod-off' instead of deciding from the log (the scenarios' own `stop gunplay` lines mean
    # mod-off). Use it for reports of a run made with engine.json disabledModules set, where the scenario stops nothing.
    [ValidateSet('', 'mod-on', 'mod-off')][string] $Condition = ''
)

# T-040 measurement summary. The game side is the Stage 1 scenarios (tools/perf/New-Stage1Scenarios.ps1): `framestats`
# (p50/p95/p99 over a whole sample window), `perf` (private bytes, address space), `costs` (per-section script cost), `pools`
# and `gpumem` (the GTAIV process's dedicated GPU memory from the Windows "GPU Process Memory" counters, private bytes and
# working set). This script only reads what those scenarios recorded, so it runs on any machine.
#
#   Measure-Stage1.ps1 -Reports results-local\<run>\_runs -Out baseline\summary.json
#   Measure-Stage1.ps1 -On baseline\summary.mod-on.json -Off baseline\summary.mod-off.json -Markdown baseline\compare.md
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'Stage1Metrics.psm1') -Force 3>$null

if ($On -or $Off) {
    if (-not ($On -and $Off)) { throw 'pass both -On and -Off' }
    $onSummary = Get-Content -LiteralPath $On -Raw | ConvertFrom-Json
    $offSummary = Get-Content -LiteralPath $Off -Raw | ConvertFrom-Json
    $table = @(Format-Stage1Comparison $onSummary $offSummary)
    if ($Markdown) { [IO.File]::WriteAllLines($Markdown, [string[]]$table, (New-Object Text.UTF8Encoding($false))); Write-Host "wrote $Markdown" }
    else { $table }
    return
}

if (-not $Reports) { throw 'pass -Reports <report folders> to summarise, or -On/-Off to compare' }
$folders = New-Object System.Collections.Generic.List[string]
foreach ($path in $Reports) {
    $root = (Resolve-Path -LiteralPath $path).Path
    foreach ($log in Get-ChildItem -LiteralPath $root -Recurse -Filter run.log -File) { $folders.Add($log.DirectoryName) }
}
if ($folders.Count -eq 0) { throw "no run.log found under: $($Reports -join ', ')" }

$summaries = @()
foreach ($folder in ($folders | Sort-Object -Unique)) {
    $summary = Get-ReportSummary $folder $Condition
    if (@($summary.sections).Count -eq 0) { Write-Host "skipped $folder (no measured section: the scenario has no 'label')"; continue }
    $summaries += , $summary
}
if ($summaries.Count -eq 0) { throw 'none of the reports contains a measured section' }

# One JSON per condition: sections of the same condition are merged (the broker and city scenarios of one mod state).
$byCondition = $summaries | Group-Object { $_['condition'] }
$stem = [IO.Path]::Combine((Split-Path -Parent ([IO.Path]::GetFullPath($Out))), [IO.Path]::GetFileNameWithoutExtension($Out))
New-Item -ItemType Directory -Force -Path (Split-Path -Parent ([IO.Path]::GetFullPath($Out))) | Out-Null
foreach ($group in $byCondition) {
    $merged = [ordered]@{
        schemaVersion = 1
        condition = $group.Name
        generatedUtc = [DateTime]::UtcNow.ToString('o')
        scenarios = @($group.Group | ForEach-Object { [ordered]@{ scenario = $_['scenario']; report = $_['reportDirectory']; engineStalls = $_['engineStalls']; logErrors = $_['logErrors']; densityMinPeds = $_['densityMinPeds']; densityMinCars = $_['densityMinCars'] } })
        budgets = Get-Stage1Budgets
        sections = @($group.Group | ForEach-Object { $_['sections'] })
    }
    $path = if ($byCondition.Count -gt 1) { "$stem.$($group.Name).json" } else { [IO.Path]::GetFullPath($Out) }
    [IO.File]::WriteAllText($path, (ConvertTo-Json -InputObject $merged -Depth 8), (New-Object Text.UTF8Encoding($false)))
    Write-Host "wrote $path ($($group.Name): $(@($merged.sections).Count) sections from $($group.Count) reports)"
}

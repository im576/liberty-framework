param(
    # Only test files whose name matches (default: every tools/tests/*.Tests.ps1).
    [string] $Filter = '*',
    # Optional machine-readable per-suite timing and failure totals.
    [string] $ResultsPath
)

# Minimal test runner for the PowerShell tooling (no Pester: the cloud container cannot reach the PowerShell Gallery).
# Each tools/tests/<Name>.Tests.ps1 is dot-sourced with Test-That available and reports PASS/FAIL lines.
# Prints "RESULT passed=N failed=M" and exits 1 on any failure. Runs on PowerShell 7 (cloud) and 5.1 (PC).
$ErrorActionPreference = 'Stop'
$script:TestPassed = 0
$script:TestFailed = 0
$script:TestRoot = $PSScriptRoot
$script:RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$script:Scratch = Join-Path ([IO.Path]::GetTempPath()) ('liberty-tests-' + [Guid]::NewGuid().ToString('N').Substring(0, 8))
New-Item -ItemType Directory -Force -Path $script:Scratch | Out-Null

function Test-That([string] $Name, [bool] $Condition, [string] $Detail = '') {
    if ($Condition) { $script:TestPassed++; Write-Host "PASS $Name" }
    else { $script:TestFailed++; Write-Host ("FAIL $Name" + $(if ($Detail) { "  ($Detail)" } else { '' })) }
}

$files = @(Get-ChildItem -LiteralPath $PSScriptRoot -Filter "$Filter.Tests.ps1" | Sort-Object Name)
if ($files.Count -eq 0) { throw "No test suites match '$Filter'; refusing an empty PASS." }
$suiteResults = @()
foreach ($file in $files) {
    Write-Host "== $($file.BaseName)"
    $timer = [Diagnostics.Stopwatch]::StartNew()
    $beforePassed = $script:TestPassed; $beforeFailed = $script:TestFailed
    try { . $file.FullName }
    catch { Test-That "$($file.BaseName) ran without exception" $false "$($_.Exception.Message) at $($_.InvocationInfo.PositionMessage)" }
    $suiteResults += [ordered]@{ suite = $file.BaseName; seconds = [math]::Round($timer.Elapsed.TotalSeconds, 3)
        passed = $script:TestPassed - $beforePassed; failed = $script:TestFailed - $beforeFailed }
}
if ($ResultsPath) {
    [IO.File]::WriteAllText([IO.Path]::GetFullPath($ResultsPath), (@{ suites = $suiteResults; passed = $script:TestPassed; failed = $script:TestFailed } | ConvertTo-Json -Depth 5))
}
if ($script:TestFailed -eq 0) {
    $scratchFull = [IO.Path]::GetFullPath($script:Scratch)
    $tempFull = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    if (-not $scratchFull.StartsWith($tempFull, [StringComparison]::OrdinalIgnoreCase) -or
        (Split-Path -Leaf $scratchFull) -notlike 'liberty-tests-*') { throw "Unsafe test scratch path: $scratchFull" }
    Remove-Item -LiteralPath $scratchFull -Recurse -Force
}
else { Write-Host "Failed test fixtures preserved: $script:Scratch" }
Write-Host "RESULT passed=$script:TestPassed failed=$script:TestFailed"
if ($script:TestFailed -gt 0) { exit 1 }

param(
    # Only test files whose name matches (default: every tools/tests/*.Tests.ps1).
    [string] $Filter = '*'
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

foreach ($file in Get-ChildItem -LiteralPath $PSScriptRoot -Filter "$Filter.Tests.ps1" | Sort-Object Name) {
    Write-Host "== $($file.BaseName)"
    try { . $file.FullName }
    catch { Test-That "$($file.BaseName) ran without exception" $false "$($_.Exception.Message) at $($_.InvocationInfo.PositionMessage)" }
}
Remove-Item -LiteralPath $script:Scratch -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "RESULT passed=$script:TestPassed failed=$script:TestFailed"
if ($script:TestFailed -gt 0) { exit 1 }

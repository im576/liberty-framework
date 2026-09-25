param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[a-zA-Z0-9_-]+$')]
    [string]$Label,
    [ValidateRange(10, 600)]
    [int]$Seconds = 120,
    [string]$PresentMonPath = 'D:\GTAIV-Reborn-Tools\downloads\PresentMon-2.6.0-x64.exe',
    [string]$OutputDirectory = 'D:\GTAIV-Reborn-Tools\captures'
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $PresentMonPath -PathType Leaf)) {
    throw "PresentMon not found: $PresentMonPath"
}

$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = [Security.Principal.WindowsPrincipal]::new($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'PresentMon capture needs an elevated shell. Open PowerShell as Administrator, then rerun this command.'
}

if (-not (Get-Process -Name GTAIV -ErrorAction SilentlyContinue)) {
    throw 'GTAIV.exe is not running. Launch the game, load a save, then rerun this command.'
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$capturePath = Join-Path $OutputDirectory ('{0}-{1}.csv' -f $Label, (Get-Date -Format 'yyyyMMdd-HHmmss'))
Write-Host "Capturing GTAIV.exe for $Seconds seconds: $capturePath"

& $PresentMonPath --process_name GTAIV.exe --output_file $capturePath --timed $Seconds --terminate_after_timed --no_console_stats
if ($LASTEXITCODE -ne 0) {
    throw "PresentMon failed with exit code $LASTEXITCODE."
}
if (-not (Test-Path -LiteralPath $capturePath -PathType Leaf) -or (Get-Item -LiteralPath $capturePath).Length -eq 0) {
    throw "PresentMon exited without a nonempty CSV: $capturePath"
}
Write-Host "Capture saved: $capturePath"

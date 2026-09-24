param(
    [Parameter(Mandatory = $true)]
    [string] $GameDirectory
)

$ErrorActionPreference = 'Stop'
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
$repoRoot = Split-Path -Parent $PSScriptRoot
$exe = Join-Path $game 'GTAIV.exe'
$sourceDll = Join-Path $repoRoot 'src\LibertyFramework\bin\Release\LibertyFramework.net.dll'
$sourceConfig = Join-Path $repoRoot 'config\probe.json'
$targetDll = Join-Path $game 'scripts\LibertyFramework.net.dll'
$targetConfig = Join-Path $game 'scripts\LibertyFramework\config\probe.json'

if (-not (Test-Path -LiteralPath $exe)) { throw "GTAIV.exe not found in $game" }
if ((Get-Item -LiteralPath $exe).VersionInfo.FileVersion -ne '1.2.0.59') {
    throw 'This T-002 package is only prepared for GTAIV.exe 1.2.0.59.'
}
if (Get-Process GTAIV -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $exe }) {
    throw 'GTA IV is running. Close it before deploying T-002.'
}
if (-not (Test-Path -LiteralPath (Join-Path $game 'ScriptHookDotNet.asi'))) {
    throw 'ScriptHookDotNet.asi is missing. Install and verify T-001 first.'
}
if (-not (Test-Path -LiteralPath $sourceDll)) { throw "Build the project first: $sourceDll" }
if (-not (Test-Path -LiteralPath $sourceConfig)) { throw "Sample config is missing: $sourceConfig" }

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $targetDll) | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $targetConfig) | Out-Null
if (Test-Path -LiteralPath $targetDll) {
    Copy-Item -LiteralPath $targetDll -Destination "$targetDll.t001.bak" -Force
}
Copy-Item -LiteralPath $sourceDll -Destination $targetDll -Force
if (-not (Test-Path -LiteralPath $targetConfig)) {
    Copy-Item -LiteralPath $sourceConfig -Destination $targetConfig
    Write-Host "Installed sample config: $targetConfig"
}
else {
    Write-Host "Preserved existing config: $targetConfig"
}
Write-Host "Installed: $targetDll"

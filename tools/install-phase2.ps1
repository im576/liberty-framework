param([Parameter(Mandatory = $true)][string] $GameDirectory)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path -LiteralPath (Split-Path -Parent $PSScriptRoot)).Path
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
$stage = Join-Path $repoRoot 'staging\phase2'
$manifest = Get-Content -LiteralPath (Join-Path $stage 'manifest.json') -Raw | ConvertFrom-Json

if (Get-Process -Name GTAIV -ErrorAction SilentlyContinue) { throw 'GTA IV is running. Close it before installing.' }
if ($manifest.package -ne 'liberty-framework-phase2' -or
    (Get-Item -LiteralPath (Join-Path $game 'GTAIV.exe')).VersionInfo.FileVersion -ne $manifest.gameVersion) {
    throw 'Phase 2 package/game version mismatch.'
}
foreach ($required in @('ScriptHookDotNet.asi', 'dinput8.dll', 'plugins\GTAIV.EFLC.FusionFix.asi')) {
    if (-not (Test-Path -LiteralPath (Join-Path $game $required))) { throw "Missing dependency: $required" }
}
foreach ($file in $manifest.files) {
    $source = Join-Path $stage $file.path
    $target = Join-Path $game $file.path
    if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $file.sha256) { throw "Staged hash changed: $($file.path)" }
    if ($file.policy -eq 'keep-existing' -and (Test-Path -LiteralPath $target -PathType Leaf)) { continue }
    $currentSha = if (Test-Path -LiteralPath $target -PathType Leaf) { (Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash } else { $null }
    if ($currentSha -ne $file.baseSha256) { throw "Installed file changed after packaging: $($file.path)" }
}

$backupRoot = Join-Path $game ("scripts\LibertyFramework\backups\phase2-" + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Force -Path $backupRoot | Out-Null
$actions = @()
foreach ($file in $manifest.files) {
    $target = Join-Path $game $file.path
    $exists = Test-Path -LiteralPath $target -PathType Leaf
    if ($exists -and $file.policy -eq 'keep-existing') {
        $actions += [ordered]@{ path = $file.path; action = 'kept'; sha256 = $null }
        continue
    }
    if ($exists) {
        $backup = Join-Path $backupRoot (Join-Path 'files' $file.path)
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $backup) | Out-Null
        Copy-Item -LiteralPath $target -Destination $backup
    }
    $actions += [ordered]@{ path = $file.path; action = $(if ($exists) { 'replaced' } else { 'created' }); sha256 = $file.sha256 }
}
$rollback = [ordered]@{ package = $manifest.package; installedUtc = (Get-Date).ToUniversalTime().ToString('o'); actions = $actions }
[IO.File]::WriteAllText((Join-Path $backupRoot 'rollback.json'), ($rollback | ConvertTo-Json -Depth 5), (New-Object Text.UTF8Encoding($false)))

try {
    foreach ($action in $actions) {
        if ($action.action -eq 'kept') { continue }
        $source = Join-Path $stage $action.path
        $target = Join-Path $game $action.path
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
        Copy-Item -LiteralPath $source -Destination $target -Force
        if ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -ne $action.sha256) { throw "Installed hash mismatch: $($action.path)" }
        Write-Host ("  {0,-9} {1}" -f $action.action, $action.path)
    }
}
catch {
    Write-Host "Phase 2 install failed: $_  Rolling back."
    & (Join-Path $PSScriptRoot 'rollback-phase2.ps1') -GameDirectory $game -BackupDirectory $backupRoot
    throw
}
Write-Host "Phase 2 installed and verified. Backup: $backupRoot"

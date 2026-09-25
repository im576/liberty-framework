param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [string] $BackupDirectory
)

$ErrorActionPreference = 'Stop'
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
if (Get-Process -Name GTAIV -ErrorAction SilentlyContinue) { throw 'GTA IV is running. Close it before rollback.' }
$backups = Join-Path $game 'scripts\LibertyFramework\backups'
if (-not $BackupDirectory) {
    $BackupDirectory = Get-ChildItem -LiteralPath $backups -Directory -Filter 'phase2-*' |
        Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $BackupDirectory) { throw 'No Phase 2 backup found.' }
$backup = (Resolve-Path -LiteralPath $BackupDirectory).Path
if (-not $backup.StartsWith($backups + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Backup must be inside the game Phase 2 backup directory.' }
$rollback = Get-Content -LiteralPath (Join-Path $backup 'rollback.json') -Raw | ConvertFrom-Json
if ($rollback.package -ne 'liberty-framework-phase2') { throw 'Not a Phase 2 backup.' }
foreach ($action in $rollback.actions) {
    if ($action.action -eq 'kept') { continue }
    $target = Join-Path $game $action.path
    if ($action.action -eq 'replaced') {
        $source = Join-Path $backup (Join-Path 'files' $action.path)
        if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing backup: $($action.path)" }
        Copy-Item -LiteralPath $source -Destination $target -Force
        if ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -ne (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash) {
            throw "Rollback hash mismatch: $($action.path)"
        }
    }
    elseif ($action.action -eq 'created' -and (Test-Path -LiteralPath $target -PathType Leaf)) {
        if ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -ne $action.sha256) { throw "Created file changed since install: $($action.path)" }
        Remove-Item -LiteralPath $target -Force
    }
    Write-Host ("  restored {0}" -f $action.path)
}
Write-Host "Phase 2 rollback complete from $backup"

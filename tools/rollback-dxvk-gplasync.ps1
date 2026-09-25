param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [Parameter(Mandatory = $true)][string] $BackupDirectory
)

# Restores the files replaced by install-dxvk-gplasync.ps1 (and removes ones it added), byte for byte.
$ErrorActionPreference = 'Stop'
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
if (Get-Process -Name GTAIV -ErrorAction SilentlyContinue) { throw 'GTA IV is running. Close it before rolling back.' }
$manifest = Get-Content -LiteralPath (Join-Path $BackupDirectory 'manifest.json') -Raw | ConvertFrom-Json
foreach ($entry in @($manifest)) {
    $target = Join-Path $game $entry.Path
    if ($entry.Existed) {
        Copy-Item -LiteralPath (Join-Path $BackupDirectory $entry.Path) -Destination $target -Force
        Write-Host "  restored $($entry.Path)"
    }
    elseif (Test-Path -LiteralPath $target) {
        # Added by the installer; move it into the backup folder rather than deleting it.
        Move-Item -LiteralPath $target -Destination (Join-Path $BackupDirectory ($entry.Path + '.removed')) -Force
        Write-Host "  removed $($entry.Path)"
    }
}
Write-Host "DXVK GPLAsync rolled back from $BackupDirectory"

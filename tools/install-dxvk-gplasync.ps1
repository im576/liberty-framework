param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [Parameter(Mandatory = $true)][string] $DxvkArchivePath,
    [string] $ShaderCacheArchivePath,
    # Async shader compile threads; the mod's guidance is half the CPU's threads (4-thread Ryzen 3 2300X -> 2).
    [int] $CompilerThreads = 2
)

# T-026: installs the owner-downloaded DXVK 2.6.2 GPLAsync build (Nexus GTA IV mod 385, DXVK team + Ph42oN,
# published by ValentynL) in place of FusionFix's stock DXVK 2.6.2 vulkan.dll. The RX 570 driver has no
# graphics pipeline library, so stock DXVK compiles shaders mid-game; this build compiles them asynchronously and
# keeps a persistent cache. Violent Liberty (which needs Vulkan) stays installed. Every replaced file is backed up;
# tools/rollback-dxvk-gplasync.ps1 restores the exact bytes. Nothing third-party enters Git.
$ErrorActionPreference = 'Stop'
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
if (Get-Process -Name GTAIV -ErrorAction SilentlyContinue) { throw 'GTA IV is running. Close it before installing.' }
$exe = Join-Path $game 'GTAIV.exe'
if ((Get-Item -LiteralPath $exe).VersionInfo.FileVersion -notmatch '^1[.,]\s*2[.,]\s*0[.,]\s*59') { throw 'GTAIV.exe is not 1.2.0.59.' }

# Only the inspected archives are accepted.
$knownDxvk = '246163107B4A07EFB6C9733C39F3F491172B1D6BC7BCBB34CD9EED67B21DF301'
$knownCache = '6BE77858A893955D412682E1CB9353FAF395BB493319446120570F0443AF8618'
if ((Get-FileHash -LiteralPath $DxvkArchivePath).Hash -ne $knownDxvk) { throw 'DXVK archive hash is not the inspected one.' }
if ($ShaderCacheArchivePath -and (Get-FileHash -LiteralPath $ShaderCacheArchivePath).Hash -ne $knownCache) { throw 'Shader cache archive hash is not the inspected one.' }

Add-Type -AssemblyName System.IO.Compression.FileSystem
$work = Join-Path ([IO.Path]::GetTempPath()) ('lf-gplasync-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
try {
    [IO.Compression.ZipFile]::ExtractToDirectory($DxvkArchivePath, (Join-Path $work 'dxvk'))
    if ($ShaderCacheArchivePath) { [IO.Compression.ZipFile]::ExtractToDirectory($ShaderCacheArchivePath, (Join-Path $work 'cache')) }

    # dxvk.conf: the mod's defaults with async on, plus compiler threads sized for this CPU.
    $conf = Get-Content -LiteralPath (Join-Path $work 'dxvk\dxvk.conf')
    $conf = $conf -replace '^#?dxvk\.numAsyncThreads\s*=.*$', "dxvk.numAsyncThreads = $CompilerThreads"
    $conf = $conf -replace '^#?dxvk\.numCompilerThreads\s*=.*$', "dxvk.numCompilerThreads = $CompilerThreads"
    Set-Content -LiteralPath (Join-Path $work 'dxvk\dxvk.conf') -Value $conf -Encoding ASCII

    $files = @(
        @{ Source = (Join-Path $work 'dxvk\vulkan.dll'); Target = 'vulkan.dll' },
        @{ Source = (Join-Path $work 'dxvk\dxvk.conf'); Target = 'dxvk.conf' }
    )
    if ($ShaderCacheArchivePath) { $files += @{ Source = (Join-Path $work 'cache\GTAIV.dxvk-cache'); Target = 'GTAIV.dxvk-cache' } }

    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $backup = Join-Path $game ('scripts\LibertyFramework\backups\dxvk-gplasync-' + $stamp)
    New-Item -ItemType Directory -Path $backup | Out-Null
    $manifest = @()
    foreach ($file in $files) {
        $target = Join-Path $game $file.Target
        $existed = Test-Path -LiteralPath $target
        if ($existed) { Copy-Item -LiteralPath $target -Destination (Join-Path $backup $file.Target) }
        $manifest += [pscustomobject]@{ Path = $file.Target; Existed = $existed; Installed = (Get-FileHash -LiteralPath $file.Source).Hash }
    }
    $manifest | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $backup 'manifest.json') -Encoding UTF8
    foreach ($file in $files) {
        Copy-Item -LiteralPath $file.Source -Destination (Join-Path $game $file.Target) -Force
        if ((Get-FileHash -LiteralPath (Join-Path $game $file.Target)).Hash -ne (Get-FileHash -LiteralPath $file.Source).Hash) { throw "Hash mismatch after copying $($file.Target)" }
        Write-Host "  installed $($file.Target)"
    }
    Write-Host "DXVK GPLAsync installed. Backup: $backup"
}
finally {
    Remove-Item -LiteralPath $work -Recurse -Force -ErrorAction SilentlyContinue
}

param(
    [Parameter(Mandatory = $true)]
    [string] $GameDirectory
)

# Offline verification: builds tools/verify/OfflineVerify.exe from the game-independent sources and
# runs it against GTAIV.exe on disk. Does not start or modify the game.
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$exe = Join-Path (Resolve-Path -LiteralPath $GameDirectory).Path 'GTAIV.exe'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$output = Join-Path $repoRoot 'tools\verify\bin\OfflineVerify.exe'
$src = Join-Path $repoRoot 'src\LibertyFramework'

# Only sources without ScriptHookDotNet dependencies may be listed here.
$sources = @(
    (Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\verify') -Filter '*.cs').FullName
    (Join-Path $src 'Core\Memory\IMemory.cs')
    (Join-Path $src 'Core\Memory\MemoryReader.cs')
    (Join-Path $src 'Core\Memory\CodeScanner.cs')
    (Join-Path $src 'Core\Memory\GameAddresses.cs')
    (Join-Path $src 'Core\Config\JsonStore.cs')
    (Join-Path $src 'Core\Math3\Vec3.cs')
    (Join-Path $src 'GameApi\WeaponInfoXml.cs')
    (Join-Path $src 'DevTools\Teleport\LocationFile.cs')
    (Join-Path $src 'DevTools\Teleport\TeleportLocation.cs')
    (Get-ChildItem -LiteralPath (Join-Path $src 'Gunplay\Profiles') -Filter '*.cs').FullName
    (Get-ChildItem -LiteralPath (Join-Path $src 'Gunplay\Recoil') -Filter '*.cs').FullName
    (Get-ChildItem -LiteralPath (Join-Path $src 'Gunplay\Spread') -Filter '*.cs').FullName
)
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
& $compiler /nologo /target:exe /platform:x86 /warn:4 "/out:$output" /reference:System.Runtime.Serialization.dll /reference:System.Xml.dll $sources
if ($LASTEXITCODE -ne 0) { throw "Verifier build failed with exit code $LASTEXITCODE" }
& $output $exe $repoRoot
if ($LASTEXITCODE -ne 0) { throw "Offline verification failed (exit $LASTEXITCODE)" }

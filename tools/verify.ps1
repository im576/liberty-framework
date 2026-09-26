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
    (Join-Path $src 'Engine\ModuleReloader.cs')
    (Join-Path $src 'Engine\EngineConfig.cs')
    (Join-Path $src 'Core\Math3\Vec3.cs')
    (Join-Path $src 'GameApi\WeaponInfoXml.cs')
    (Join-Path $src 'DevTools\Teleport\LocationFile.cs')
    (Join-Path $src 'DevTools\Teleport\TeleportLocation.cs')
    (Get-ChildItem -LiteralPath (Join-Path $src 'Gunplay\Profiles') -Filter '*.cs').FullName
    (Get-ChildItem -LiteralPath (Join-Path $src 'Gunplay\Recoil') -Filter '*.cs').FullName
    (Get-ChildItem -LiteralPath (Join-Path $src 'Gunplay\Spread') -Filter '*.cs').FullName
    (Get-ChildItem -LiteralPath (Join-Path $src 'Arsenal\Contracts') -Filter '*.cs').FullName
    # T-023 vehicle extras catalog (reads the installed vehicles.img through the finishes IMG/RSC readers).
    (Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\finishes') -Filter '*.cs' | Where-Object { $_.Name -ne 'Program.cs' }).FullName
    (Join-Path $repoRoot 'tools\vehicles\VehicleExtrasScanner.cs')
    (Join-Path $src 'CombatEffects\CombatEffectsConfig.cs')
    (Join-Path $src 'GameApi\SkeletonCollapseEngine.cs')
    (Join-Path $src 'GameApi\DirectNatives.cs')
    # Any folder named Logic holds ScriptHookDotNet-free code that the verifier can test (T-020/T-021 onward).
    (Get-ChildItem -LiteralPath $src -Recurse -Directory -Filter 'Logic' | ForEach-Object { (Get-ChildItem -LiteralPath $_.FullName -Filter '*.cs').FullName })
)
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
& $compiler /nologo /target:exe /platform:x86 /warn:4 "/out:$output" /reference:System.Runtime.Serialization.dll /reference:System.Xml.dll /reference:System.Drawing.dll /reference:System.Core.dll $sources
if ($LASTEXITCODE -ne 0) { throw "Verifier build failed with exit code $LASTEXITCODE" }
& $output $exe $repoRoot
if ($LASTEXITCODE -ne 0) { throw "Offline verification failed (exit $LASTEXITCODE)" }

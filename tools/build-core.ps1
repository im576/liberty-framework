param(
    # Folder containing i686-w64-mingw32-clang++.exe (llvm-mingw). Defaults to the path tools/get-toolchains.ps1 recorded.
    [string] $ClangBin
)

# Builds native/LibertyCore -> native/LibertyCore/bin/LibertyCore.dll (32-bit, C++20, statically linked, warnings are errors).
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'toolchains.ps1')
if (-not $ClangBin) { $ClangBin = Get-LibertyToolchain 'clang' }
$compiler = Join-Path $ClangBin (Get-LibertyNativeName 'i686-w64-mingw32-clang++')
if (-not (Test-Path -LiteralPath $compiler)) { throw "clang not found: $compiler (run tools/get-toolchains.ps1)" }
$env:PATH = $ClangBin + [IO.Path]::PathSeparator + $env:PATH

$core = Join-Path $repoRoot 'native\LibertyCore'
$outputDirectory = Join-Path $core 'bin'
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$output = Join-Path $outputDirectory 'LibertyCore.dll'
$sources = Get-ChildItem -LiteralPath (Join-Path $core 'src') -Filter '*.cpp' | Sort-Object Name | ForEach-Object { $_.FullName }

& $compiler -std=c++20 -O2 -Wall -Wextra -Werror -fno-exceptions -fno-rtti -shared -static -s `
    "-I$(Join-Path $core 'include')" -o $output $sources '-Wl,--kill-at'
if ($LASTEXITCODE -ne 0) { throw "LibertyCore build failed with exit code $LASTEXITCODE" }

Write-Host "Built $output"
Write-Host ("SHA256 " + (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash)


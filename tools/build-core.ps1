param(
    # Folder containing i686-w64-mingw32-clang++.exe (llvm-mingw). Defaults to the path tools/get-toolchains.ps1 recorded.
    [string] $ClangBin
)

# Builds native/LibertyCore -> native/LibertyCore/bin/LibertyCore.dll (32-bit, C++20, statically linked, warnings are errors),
# then builds and runs the core's game-independent unit tests (native/LibertyCore/tests/*.cpp); a failing test fails the build.
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

# Unit tests: pure algorithms only (no game, no DLL), one executable per file. Each is built for the game's target;
# off Windows (the cloud container) that .exe cannot run, so the same source is also built as a 32-bit host program
# with the host clang (-m32 keeps the game's pointer size) and that one runs.
$testDirectory = Join-Path $core 'tests'
foreach ($test in Get-ChildItem -LiteralPath $testDirectory -Filter '*.cpp' | Sort-Object Name) {
    $testExe = Join-Path $outputDirectory ($test.BaseName + '.exe')
    & $compiler -std=c++20 -O2 -Wall -Wextra -Werror -fno-exceptions -fno-rtti -static "-I$(Join-Path $core 'include')" -o $testExe $test.FullName
    if ($LASTEXITCODE -ne 0) { throw "$($test.Name) failed to build (exit $LASTEXITCODE)" }
    if (-not (Test-LibertyWindows)) {
        $testExe = Join-Path $outputDirectory ($test.BaseName + '.host')
        & clang++ -m32 -std=c++20 -O2 -Wall -Wextra -Werror -fno-exceptions -fno-rtti "-I$(Join-Path $core 'include')" -o $testExe $test.FullName
        if ($LASTEXITCODE -ne 0) { throw "$($test.Name) failed to build for the host (exit $LASTEXITCODE)" }
    }
    & $testExe
    if ($LASTEXITCODE -ne 0) { throw "$($test.BaseName) failed (exit $LASTEXITCODE)" }
}

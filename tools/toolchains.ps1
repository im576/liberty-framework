# Dot-sourced by build scripts. Toolchains live outside the repository; tools/get-toolchains.ps1 downloads them and
# records their folders in tools/toolchains.local.json (git-ignored), so no machine path is baked into the scripts.
# The cloud build container (Linux, tools/cloud/setup.sh) records its own folders there too.
function Get-LibertyToolchain([string] $name) {
    $record = Join-Path $PSScriptRoot 'toolchains.local.json'
    if (-not (Test-Path -LiteralPath $record)) { throw "No toolchains recorded; run tools/get-toolchains.ps1 -Directory <folder>" }
    $paths = Get-Content -LiteralPath $record -Raw | ConvertFrom-Json
    $value = $paths.$name
    if (-not $value) { throw "Toolchain '$name' not recorded in $record" }
    return $value
}

function Test-LibertyWindows { return $env:OS -eq 'Windows_NT' }

# Runs a .NET Framework executable (Roslyn's csc.exe or a tool this repository built): directly on Windows, under Mono
# elsewhere (the cloud container has no binfmt registration for .exe files). Exit code is left in $LASTEXITCODE.
function Invoke-LibertyManaged([string] $LibertyManagedExecutable) {
    if (Test-LibertyWindows) { & $LibertyManagedExecutable @args } else { & mono $LibertyManagedExecutable @args }
}

# A native tool's file name: 'i686-w64-mingw32-clang++' is 'i686-w64-mingw32-clang++.exe' on Windows.
function Get-LibertyNativeName([string] $name) {
    if (Test-LibertyWindows) { return $name + '.exe' } else { return $name }
}

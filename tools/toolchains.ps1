# Dot-sourced by build scripts. Toolchains live outside the repository; tools/get-toolchains.ps1 downloads them and
# records their folders in tools/toolchains.local.json (git-ignored), so no machine path is baked into the scripts.
function Get-LibertyToolchain([string] $name) {
    $record = Join-Path $PSScriptRoot 'toolchains.local.json'
    if (-not (Test-Path -LiteralPath $record)) { throw "No toolchains recorded; run tools/get-toolchains.ps1 -Directory <folder>" }
    $paths = Get-Content -LiteralPath $record -Raw | ConvertFrom-Json
    $value = $paths.$name
    if (-not $value) { throw "Toolchain '$name' not recorded in $record" }
    return $value
}

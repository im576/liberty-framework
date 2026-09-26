param(
    # blender.exe (4.2 or newer). Default: $env:LIBERTY_BLENDER.
    [string] $Blender = $env:LIBERTY_BLENDER
)

# Regenerates the example assets made in Blender (tools/blender/examples/make_*.py) into content/ through the add-on.
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path -LiteralPath (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))).Path
if (-not $Blender -or -not (Test-Path -LiteralPath $Blender -PathType Leaf)) { throw 'Pass -Blender <blender.exe> or set LIBERTY_BLENDER.' }
$ErrorActionPreference = 'Continue'
$failed = 0
foreach ($script in Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'examples') -Filter 'make_*.py') {
    $log = & $Blender --background --factory-startup --python-exit-code 1 --python $script.FullName -- $repoRoot 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) { $failed++; Write-Host "FAIL $($script.Name)"; Write-Host $log } else { $log -split "`r?`n" | Where-Object { $_ -match '^EXAMPLE' } | ForEach-Object { Write-Host $_ } }
}
if ($failed -gt 0) { exit 1 }

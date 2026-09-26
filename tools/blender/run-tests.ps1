param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    # blender.exe (4.2 or newer). Default: $env:LIBERTY_BLENDER.
    [string] $Blender = $env:LIBERTY_BLENDER,
    [string] $OutputDirectory = (Join-Path ([IO.Path]::GetTempPath()) 'liberty_blender_tests')
)

# Headless tests for the Liberty Exporter add-on (tools/blender/tests/run_tests.py) plus Blender's extension validator.
# Needs LibertyContent.exe (tools/build-content.ps1) because the tests validate and build the exported assets for real.
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path -LiteralPath (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))).Path
if (-not $Blender -or -not (Test-Path -LiteralPath $Blender -PathType Leaf)) { throw 'Pass -Blender <blender.exe> or set LIBERTY_BLENDER.' }
if (-not (Test-Path -LiteralPath (Join-Path $repoRoot 'tools\content\bin\LibertyContent.exe'))) { throw 'Build LibertyContent first: tools/build-content.ps1' }

# Blender writes progress to stderr; with 'Stop' PowerShell 5.1 would turn that into terminating errors.
$ErrorActionPreference = 'Continue'
$addon = Join-Path $PSScriptRoot 'liberty_exporter'
$validate = & $Blender --background --factory-startup --command extension validate $addon 2>&1 | Out-String
Write-Host $validate.Trim()
$validated = $LASTEXITCODE -eq 0

$log = & $Blender --background --factory-startup --python-exit-code 1 --python (Join-Path $PSScriptRoot 'tests\run_tests.py') -- $repoRoot $GameDirectory $OutputDirectory 2>&1 | Out-String
$log -split "`r?`n" | Where-Object { $_ -match '^(PASS|FAIL|RESULT)|Error|Traceback|^\s+File ' } | ForEach-Object { Write-Host $_ }
$passed = $LASTEXITCODE -eq 0 -and $log -match 'RESULT passed=\d+ failed=0'
if (-not $validated) { Write-Host 'FAIL extension manifest validation' }
if (-not ($passed -and $validated)) { $log | Set-Content -LiteralPath (Join-Path $OutputDirectory 'blender.log') -Encoding utf8 -ErrorAction SilentlyContinue; exit 1 }
Write-Host 'Blender add-on tests passed'

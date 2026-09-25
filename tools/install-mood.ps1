param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [switch] $Restore
)

# M-1 Liberty Mood: generates timecyc.dat / timecycext.dat from FusionFix's pristine copies (saved on first run as
# *.fusionfix next to them) and config/mood.json, then installs them into update\pc\data. Re-running always starts
# from the pristine copies, so tuning never compounds. -Restore puts FusionFix's files back.
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
if (Get-Process -Name GTAIV -ErrorAction SilentlyContinue) { throw 'GTA IV is running. Close it first.' }
$data = Join-Path $game 'update\pc\data'
$files = @('timecyc.dat', 'timecycext.dat')
foreach ($name in $files) {
    $pristine = Join-Path $data ($name + '.fusionfix')
    if (-not (Test-Path -LiteralPath $pristine)) { Copy-Item -LiteralPath (Join-Path $data $name) -Destination $pristine; Write-Host "  saved pristine $name" }
}
if ($Restore) {
    foreach ($name in $files) { Copy-Item -LiteralPath (Join-Path $data ($name + '.fusionfix')) -Destination (Join-Path $data $name) -Force; Write-Host "  restored $name" }
    return
}
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$tool = Join-Path $repoRoot 'tools\mood\bin\MoodTimecycle.exe'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $tool) | Out-Null
& $csc /nologo /target:exe "/out:$tool" /reference:System.Web.Extensions.dll (Join-Path $repoRoot 'tools\mood\MoodTimecycle.cs')
if ($LASTEXITCODE -ne 0) { throw 'MoodTimecycle build failed' }
$work = Join-Path ([IO.Path]::GetTempPath()) ('lf-mood-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
try {
    & $tool (Join-Path $repoRoot 'config\mood.json') (Join-Path $data 'timecyc.dat.fusionfix') (Join-Path $data 'timecycext.dat.fusionfix') (Join-Path $work 'timecyc.dat') (Join-Path $work 'timecycext.dat')
    if ($LASTEXITCODE -ne 0) { throw 'MoodTimecycle failed' }
    foreach ($name in $files) {
        Copy-Item -LiteralPath (Join-Path $work $name) -Destination (Join-Path $data $name) -Force
        Write-Host "  installed $name"
    }
    Write-Host 'Liberty Mood installed. FusionFix originals: update\pc\data\*.fusionfix (restore with -Restore).'
}
finally { Remove-Item -LiteralPath $work -Recurse -Force -ErrorAction SilentlyContinue }

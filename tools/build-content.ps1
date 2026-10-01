# Builds the Liberty Content Compiler (tools/content) with the model and resource readers/writers it shares with
# tools/models and tools/finishes -> tools/content/bin/LibertyContent.exe (Roslyn, warnings are errors; CS0649 is off because the
# strap-design settings are only assigned by tools/models' own Program).
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'toolchains.ps1')
$compiler = Join-Path (Get-LibertyToolchain 'roslyn') 'csc.exe'
$output = Join-Path $repoRoot 'tools\content\bin\LibertyContent.exe'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
$sources = @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\content') -Filter '*.cs').FullName) +
    @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\models') -Filter '*.cs' | Where-Object Name -ne 'Program.cs').FullName) +
    @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\finishes') -Filter '*.cs' | Where-Object Name -ne 'Program.cs').FullName)
# Up to date: the same sources, script and compiler as the last build (tools/BuildCache.psm1).
Import-Module (Join-Path $PSScriptRoot 'BuildCache.psm1') -Force
$stamp = Join-Path (Split-Path -Parent $output) '.build-inputs'
$key = Get-InputKey $repoRoot (@($sources) + @($PSCommandPath, (Join-Path $PSScriptRoot 'toolchains.ps1'))) @("csc|$(Get-FileIdentity $compiler)")
if (Test-BuildStamp $stamp $key @($output)) { Write-Host "Built $output (up to date)"; exit 0 }
Clear-BuildStamp $stamp
Invoke-LibertyManaged $compiler /nologo /target:exe /platform:x86 /optimize+ /langversion:7.3 /warn:4 /warnaserror+ /nowarn:0649 "/out:$output" `
    /reference:System.Runtime.Serialization.dll /reference:System.Drawing.dll /reference:System.Core.dll /reference:System.Web.Extensions.dll $sources
if ($LASTEXITCODE -ne 0) { throw "LibertyContent build failed with exit code $LASTEXITCODE" }
Set-BuildStamp $stamp $key
Write-Host "Built $output from $($sources.Count) source files"

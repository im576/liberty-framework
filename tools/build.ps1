param(
    [Parameter(Mandatory = $true)]
    [string] $ScriptHookDotNetReference
)

# Builds src/LibertyFramework/bin/Release/LibertyFramework.net.dll (x86, .NET Framework 4, C# 7.3, unsafe allowed for the
# LibertyCore ABI) with Roslyn from tools/get-toolchains.ps1. Every .cs file under src/LibertyFramework is compiled.
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$reference = (Resolve-Path -LiteralPath $ScriptHookDotNetReference).Path
. (Join-Path $PSScriptRoot 'toolchains.ps1')
$compiler = Join-Path (Get-LibertyToolchain 'roslyn') 'csc.exe'
$sourceRoot = Join-Path $repoRoot 'src\LibertyFramework'
$outputDirectory = Join-Path $sourceRoot 'bin\Release'
$output = Join-Path $outputDirectory 'LibertyFramework.net.dll'

if (-not (Test-Path -LiteralPath $compiler)) {
    throw "C# compiler not found: $compiler (run tools/get-toolchains.ps1)"
}

New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$sources = Get-ChildItem -LiteralPath $sourceRoot -Recurse -Filter '*.cs' |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    Sort-Object FullName |
    ForEach-Object { $_.FullName }

& $compiler /nologo /target:library /platform:x86 /optimize+ /unsafe /langversion:7.3 /warn:4 /warnaserror+ "/out:$output" "/reference:$reference" `
    /reference:System.Runtime.Serialization.dll /reference:System.Xml.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll `
    /reference:System.Core.dll $sources
if ($LASTEXITCODE -ne 0) {
    throw "C# compiler failed with exit code $LASTEXITCODE"
}

Write-Host "Built $output from $($sources.Count) source files"
Write-Host ("SHA256 " + (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash)

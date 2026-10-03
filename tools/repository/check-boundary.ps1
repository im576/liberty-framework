param(
    [Parameter(Mandatory = $true)][string] $ModAssembly,
    [Parameter(Mandatory = $true)][string] $ScriptHookDotNetReference
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
. (Join-Path $root 'tools/toolchains.ps1')
$compiler = Join-Path (Get-LibertyToolchain 'roslyn') 'csc.exe'
$output = Join-Path $root 'results-local/offline/BoundaryChecks.exe'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
Invoke-LibertyManaged $compiler /nologo /target:exe /platform:x86 /langversion:7.3 /warn:4 /warnaserror+ "/out:$output" /reference:System.Core.dll (Join-Path $PSScriptRoot 'BoundaryChecks.cs')
if ($LASTEXITCODE -ne 0) { throw 'Boundary check compilation failed.' }
Invoke-LibertyManaged $output (Join-Path $root 'sdk/Liberty.Sdk/bin/Liberty.Sdk.dll') (Join-Path $root 'src/LibertyFramework/bin/Release/LibertyFramework.net.dll') $ModAssembly $ScriptHookDotNetReference
if ($LASTEXITCODE -ne 0) { throw 'Assembly boundary checks failed.' }

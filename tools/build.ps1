param(
    [Parameter(Mandatory = $true)]
    [string] $ScriptHookDotNetReference
)

# Builds, in dependency order, with Roslyn from tools/get-toolchains.ps1 (warnings are errors everywhere):
#   1. sdk/Liberty.Sdk            -> sdk/Liberty.Sdk/bin/Liberty.Sdk.dll (+ .xml docs). The public SDK: no ScriptHookDotNet.
#   2. src/LibertyFramework       -> src/LibertyFramework/bin/Release/LibertyFramework.net.dll (x86, .NET 4, C# 7.3, unsafe
#                                    allowed for the LibertyCore ABI). References the SDK and ScriptHookDotNet.
#   3. mods/<Name>                -> mods/<Name>/bin/<Name>.dll. SDK-only mods: they may reference Liberty.Sdk and the .NET
#                                    Framework, never ScriptHookDotNet or the engine (that is what keeps them portable).
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$reference = (Resolve-Path -LiteralPath $ScriptHookDotNetReference).Path
. (Join-Path $PSScriptRoot 'toolchains.ps1')
$compiler = Join-Path (Get-LibertyToolchain 'roslyn') 'csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    throw "C# compiler not found: $compiler (run tools/get-toolchains.ps1)"
}

function Get-Sources([string] $root) {
    Get-ChildItem -LiteralPath $root -Recurse -Filter '*.cs' |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
        Sort-Object FullName |
        ForEach-Object { $_.FullName }
}

function Invoke-Compiler([string] $name, [string[]] $arguments) {
    Invoke-LibertyManaged $compiler @arguments
    if ($LASTEXITCODE -ne 0) { throw "$name failed to compile (exit $LASTEXITCODE)" }
}

$sdkRoot = Join-Path $repoRoot 'sdk\Liberty.Sdk'
$sdkOut = Join-Path $sdkRoot 'bin'
$sdkDll = Join-Path $sdkOut 'Liberty.Sdk.dll'
$sdkSources = @(Get-Sources $sdkRoot)
$sourceRoot = Join-Path $repoRoot 'src\LibertyFramework'
$outputDirectory = Join-Path $sourceRoot 'bin\Release'
$output = Join-Path $outputDirectory 'LibertyFramework.net.dll'
# Integration workspaces contain consumer mirrors for historical fixtures. Ownership,
# not a list of gameplay names, controls what may enter the framework assembly.
$workspaceInfo = $null
if (Test-Path -LiteralPath (Join-Path $repoRoot 'workspace.json')) {
    $workspaceInfo = Get-Content -LiteralPath (Join-Path $repoRoot 'workspace.json') -Raw | ConvertFrom-Json
}
$sources = @(Get-Sources $sourceRoot | Where-Object {
    if (-not $workspaceInfo) { $true }
    else {
        $relative = $_.Substring($repoRoot.Length).TrimStart('\', '/').Replace('\', '/')
        $owner = $workspaceInfo.ownership.PSObject.Properties[$relative]
        $owner -and $owner.Value.repo -eq $workspaceInfo.repositories.framework.repo
    }
})
$modsRoot = Join-Path $repoRoot 'mods'
$mods = @(if (Test-Path -LiteralPath $modsRoot) { Get-ChildItem -LiteralPath $modsRoot -Directory | Where-Object { @(Get-Sources $_.FullName).Count -gt 0 } })

# Up to date: same sources, build script, compiler and ScriptHookDotNet reference as the last successful build, and every
# output still there (tools/BuildCache.psm1). Nothing is compiled then.
Import-Module (Join-Path $PSScriptRoot 'BuildCache.psm1') -Force
$modSources = @($mods | ForEach-Object { Get-Sources $_.FullName })
$outputs = @($sdkDll, (Join-Path $sdkOut 'Liberty.Sdk.xml'), $output, (Join-Path $outputDirectory 'Liberty.Sdk.dll')) + @($mods | ForEach-Object { Join-Path $_.FullName ('bin\' + $_.Name + '.dll') })
$stamp = Join-Path $outputDirectory '.build-inputs'
$key = Get-InputKey $repoRoot (@($sdkSources) + @($sources) + $modSources + @($PSCommandPath, (Join-Path $PSScriptRoot 'toolchains.ps1'))) @(
    "csc|$(Get-FileIdentity $compiler)", "shdn|$((Get-FileHash -LiteralPath $reference -Algorithm SHA256).Hash)", "mods|$(($mods | ForEach-Object { $_.Name }) -join ',')")
if (Test-BuildStamp $stamp $key $outputs) {
    Write-Host "Built $sdkDll (up to date)"
    Write-Host "Built $output (up to date)"
    Write-Host ("SHA256 " + (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash)
    foreach ($mod in $mods) { Write-Host "Built mod $(Join-Path $mod.FullName ('bin\' + $mod.Name + '.dll')) (up to date)" }
    exit 0
}
Clear-BuildStamp $stamp

# 1. SDK
New-Item -ItemType Directory -Force -Path $sdkOut | Out-Null
Invoke-Compiler 'Liberty.Sdk' (@('/nologo', '/target:library', '/platform:anycpu', '/optimize+', '/langversion:7.3', '/warn:4', '/warnaserror+',
    '/nowarn:1591', "/doc:$(Join-Path $sdkOut 'Liberty.Sdk.xml')", "/out:$sdkDll", '/reference:System.Core.dll') + $sdkSources)
Write-Host "Built $sdkDll from $($sdkSources.Count) source files"

# 2. Engine
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
Invoke-Compiler 'LibertyFramework.net' (@('/nologo', '/target:library', '/platform:x86', '/optimize+', '/unsafe', '/langversion:7.3', '/warn:4', '/warnaserror+',
    "/out:$output", "/reference:$reference", "/reference:$sdkDll",
    '/reference:System.Runtime.Serialization.dll', '/reference:System.Xml.dll', '/reference:System.Drawing.dll', '/reference:System.Windows.Forms.dll',
    '/reference:System.Core.dll') + $sources)
Copy-Item -LiteralPath $sdkDll -Destination (Join-Path $outputDirectory 'Liberty.Sdk.dll') -Force
Write-Host "Built $output from $($sources.Count) source files"
Write-Host ("SHA256 " + (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash)

# 3. SDK mods
foreach ($mod in $mods) {
    $modSources = @(Get-Sources $mod.FullName)
    $modOut = Join-Path $mod.FullName 'bin'
    New-Item -ItemType Directory -Force -Path $modOut | Out-Null
    $modDll = Join-Path $modOut ($mod.Name + '.dll')
    Invoke-Compiler $mod.Name (@('/nologo', '/target:library', '/platform:anycpu', '/optimize+', '/langversion:7.3', '/warn:4', '/warnaserror+',
        "/out:$modDll", "/reference:$sdkDll", '/reference:System.Core.dll', '/reference:System.Runtime.Serialization.dll') + $modSources)
    Write-Host "Built mod $modDll from $($modSources.Count) source files"
}
Set-BuildStamp $stamp $key

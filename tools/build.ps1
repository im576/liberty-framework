param(
    [Parameter(Mandatory = $true)]
    [string] $ScriptHookDotNetReference
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$reference = (Resolve-Path -LiteralPath $ScriptHookDotNetReference).Path
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$outputDirectory = Join-Path $repoRoot 'src\LibertyFramework\bin\Release'
$output = Join-Path $outputDirectory 'LibertyFramework.net.dll'

if (-not (Test-Path -LiteralPath $compiler)) {
    throw "C# compiler not found: $compiler"
}

New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$sources = @(
    (Join-Path $repoRoot 'src\LibertyFramework\RuntimeProbe.cs'),
    (Join-Path $repoRoot 'src\LibertyFramework\Core\Logging\RuntimeLog.cs'),
    (Join-Path $repoRoot 'src\LibertyFramework\Core\Config\ProbeConfig.cs'),
    (Join-Path $repoRoot 'src\LibertyFramework\Core\Config\ProbeConfigLoader.cs'),
    (Join-Path $repoRoot 'src\LibertyFramework\DevTools\DevToolsMenu.cs')
)

& $compiler /nologo /target:library /platform:x86 /optimize+ /warn:4 "/out:$output" "/reference:$reference" /reference:System.Runtime.Serialization.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll $sources
if ($LASTEXITCODE -ne 0) {
    throw "C# compiler failed with exit code $LASTEXITCODE"
}

Write-Host "Built $output"

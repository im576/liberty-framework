param(
    # Where to keep the toolchains (outside the repository; about 600 MB).
    [Parameter(Mandatory = $true)][string] $Directory
)

# Fetches the pinned build toolchains and records their paths in tools/toolchains.local.json:
#   roslyn: Microsoft.Net.Compilers.Toolset 4.11.0 (C# 7.3 for the .NET Framework 4 script DLL)
#   clang:  llvm-mingw 20260922 (i686 C++20 for native/LibertyCore)
$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Add-Type -AssemblyName System.IO.Compression.FileSystem
New-Item -ItemType Directory -Force -Path $Directory | Out-Null
$downloads = Join-Path $Directory 'downloads'
New-Item -ItemType Directory -Force -Path $downloads | Out-Null

function Get-Pinned([string] $url, [string] $file, [string] $sha256) {
    $path = Join-Path $downloads $file
    if (-not (Test-Path -LiteralPath $path)) { Invoke-WebRequest -Uri $url -OutFile $path -UseBasicParsing }
    $actual = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
    if ($sha256 -and $actual -ne $sha256) { throw "$file hash $actual does not match the pinned $sha256" }
    return $path
}

$roslynZip = Get-Pinned 'https://www.nuget.org/api/v2/package/Microsoft.Net.Compilers.Toolset/4.11.0' 'microsoft.net.compilers.toolset.4.11.0.nupkg' 'DB165C75F0D0386D078D01448F96C93F8222320633E4FE828F5D1BF8C99B7D6F'
$roslyn = Join-Path $Directory 'roslyn-4.11.0'
if (-not (Test-Path -LiteralPath $roslyn)) { [IO.Compression.ZipFile]::ExtractToDirectory($roslynZip, $roslyn) }

$clangZip = Get-Pinned 'https://github.com/mstorsjo/llvm-mingw/releases/download/20260922/llvm-mingw-20260922-ucrt-x86_64.zip' 'llvm-mingw-20260922-ucrt-x86_64.zip' $null
$clang = Join-Path $Directory 'llvm-mingw-20260922-ucrt-x86_64'
if (-not (Test-Path -LiteralPath $clang)) { [IO.Compression.ZipFile]::ExtractToDirectory($clangZip, $Directory) }

$record = [ordered]@{
    roslyn = (Join-Path $roslyn 'tasks\net472')
    clang = (Join-Path $clang 'bin')
}
[IO.File]::WriteAllText((Join-Path $PSScriptRoot 'toolchains.local.json'), ($record | ConvertTo-Json), (New-Object Text.UTF8Encoding($false)))
Write-Host "Toolchains recorded in tools/toolchains.local.json"

param(
    [Parameter(Mandatory = $true)]
    [string] $GameDirectory
)

$ErrorActionPreference = 'Stop'
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
$repoRoot = Split-Path -Parent $PSScriptRoot
$base = Join-Path $game 'common\data\WeaponInfo.xml'
$stageDirectory = Join-Path $repoRoot 'staging\t007'
$stageXml = Join-Path $stageDirectory 'WeaponInfo.xml'
$stageHash = Join-Path $stageDirectory 'base.sha256'
if (-not (Test-Path -LiteralPath $base)) { throw "Base weapon data not found: $base" }

$document = New-Object System.Xml.XmlDocument
$document.PreserveWhitespace = $true
$document.Load($base)
$pistol = $document.SelectSingleNode('/weaponinfo/weapon[@type="PISTOL"]')
if ($null -eq $pistol) { throw 'Vanilla PISTOL definition not found.' }
if ($document.SelectSingleNode('/weaponinfo/weapon[@type="LF_GOLD_PISTOL"]')) {
    throw 'Candidate name already exists in the base weapon data.'
}
$candidate = $pistol.CloneNode($true)
$candidate.Attributes['type'].Value = 'LF_GOLD_PISTOL'
$null = $document.DocumentElement.AppendChild($candidate)
New-Item -ItemType Directory -Force -Path $stageDirectory | Out-Null
$document.Save($stageXml)
$baseDigest = (Get-FileHash -LiteralPath $base -Algorithm SHA256).Hash
Set-Content -LiteralPath $stageHash -Value $baseDigest -NoNewline
Write-Host "Staged T-007 data: $stageXml"
Write-Host 'Candidate LF_GOLD_PISTOL reuses the vanilla pistol model and stats for identity testing only.'

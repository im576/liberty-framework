param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [Parameter(Mandatory = $true)][string] $ScriptHookDotNetReference,
    [string] $LvsDirectory
)

# Package only Phase 2 scripts/config. Phase 1's already-installed gold models and
# WeaponInfo.xml remain the base; the old Phase 1 asset builder expects vanilla inputs.
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path -LiteralPath (Split-Path -Parent $PSScriptRoot)).Path
$game = (Resolve-Path -LiteralPath $GameDirectory).Path
$stage = [IO.Path]::GetFullPath((Join-Path $repoRoot 'staging\phase2'))
if (-not $stage.StartsWith($repoRoot + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Invalid staging path.' }
if (Test-Path -LiteralPath $stage) { Remove-Item -LiteralPath $stage -Recurse -Force }
New-Item -ItemType Directory -Force -Path $stage | Out-Null

& (Join-Path $PSScriptRoot 'build.ps1') -ScriptHookDotNetReference $ScriptHookDotNetReference
if ($LASTEXITCODE -ne 0) { throw 'Phase 2 build failed.' }
& (Join-Path $PSScriptRoot 'verify.ps1') -GameDirectory $game | Select-String -Pattern '^(FAIL|RESULT)'
if ($LASTEXITCODE -ne 0) { throw 'Phase 2 offline verification failed.' }

function Stage-File([string] $source, [string] $relativePath, [string] $policy) {
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing Phase 2 source: $source" }
    $target = Join-Path $stage $relativePath
    $installed = Join-Path $game $relativePath
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
    if ($policy -eq 'merge-defaults' -and (Test-Path -LiteralPath $installed -PathType Leaf)) {
        $template = Get-Content -LiteralPath $source -Raw | ConvertFrom-Json
        $saved = Get-Content -LiteralPath $installed -Raw | ConvertFrom-Json
        if ($saved.schemaVersion -ne $template.schemaVersion) { throw "Cannot merge schema for $relativePath" }
        foreach ($property in $template.PSObject.Properties) {
            if (-not $saved.PSObject.Properties[$property.Name]) {
                $saved | Add-Member -NotePropertyName $property.Name -NotePropertyValue $property.Value
            }
        }
        [IO.File]::WriteAllText($target, ($saved | ConvertTo-Json -Depth 32), (New-Object Text.UTF8Encoding($false)))
    }
    elseif ([IO.Path]::GetFullPath($source) -ne [IO.Path]::GetFullPath($target)) {
        Copy-Item -LiteralPath $source -Destination $target
    }
    $baseSha = if (Test-Path -LiteralPath $installed -PathType Leaf) { (Get-FileHash -LiteralPath $installed -Algorithm SHA256).Hash } else { $null }
    $script:entries += [ordered]@{
        path = $relativePath
        sha256 = (Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash
        baseSha256 = $baseSha
        policy = $(if ($policy -eq 'merge-defaults') { 'replace' } else { $policy })
    }
}

$entries = @()
Stage-File (Join-Path $repoRoot 'src\LibertyFramework\bin\Release\LibertyFramework.net.dll') 'scripts\LibertyFramework.net.dll' 'replace'
foreach ($name in @('gunplay.json', 'combat_effects.json', 'weapon-catalog.json')) {
    Stage-File (Join-Path $repoRoot "config\$name") "scripts\LibertyFramework\config\$name" 'replace'
}
Stage-File (Join-Path $repoRoot 'config\arsenal.json') 'scripts\LibertyFramework\config\arsenal.json' 'merge-defaults'
Stage-File (Join-Path $repoRoot 'config\holsters.json') 'scripts\LibertyFramework\config\holsters.json' 'keep-existing'
Get-ChildItem -LiteralPath (Join-Path $repoRoot 'config\presets') -Filter '*.json' | Sort-Object Name | ForEach-Object {
    Stage-File $_.FullName "scripts\LibertyFramework\config\presets\$($_.Name)" 'replace'
}

if ($LvsDirectory) {
    $lvs = (Resolve-Path -LiteralPath $LvsDirectory).Path
    $extended = Join-Path $stage 'scripts\LibertyVehicleServicesCE.CS'
    & (Join-Path $PSScriptRoot 'extend-lvs-body-variants.ps1') -LvsDirectory $lvs -OutputPath $extended
    if ($LASTEXITCODE -ne 0) { throw 'LVS extension failed.' }
    # Stage-File also records the installed source hash; it is rechecked at install time.
    Stage-File $extended 'scripts\LibertyVehicleServicesCE.CS' 'replace'
    Stage-File (Join-Path $lvs 'LICENSE') 'scripts\LibertyVehicleServicesCE\LICENSE.txt' 'replace'
    Stage-File (Join-Path $lvs 'CREDITS.md') 'scripts\LibertyVehicleServicesCE\CREDITS.md' 'replace'
}

$manifest = [ordered]@{
    package = 'liberty-framework-phase2'
    builtUtc = (Get-Date).ToUniversalTime().ToString('o')
    gameVersion = '1.2.0.59'
    files = $entries
}
[IO.File]::WriteAllText((Join-Path $stage 'manifest.json'), ($manifest | ConvertTo-Json -Depth 5), (New-Object Text.UTF8Encoding($false)))
Write-Host "Phase 2 staged $($entries.Count) files in $stage"

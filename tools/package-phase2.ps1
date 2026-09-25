param(
    [Parameter(Mandatory = $true)][string] $GameDirectory,
    [Parameter(Mandatory = $true)][string] $ScriptHookDotNetReference,
    # Extracted Liberty Vehicle Services CE release (MIT, ekzestean): source for the T-023 label patch.
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
foreach ($name in @('gunplay.json', 'combat_effects.json', 'weapon-catalog.json', 'atmosphere.json')) {
    Stage-File (Join-Path $repoRoot "config\$name") "scripts\LibertyFramework\config\$name" 'replace'
}
Stage-File (Join-Path $repoRoot 'config\arsenal.json') 'scripts\LibertyFramework\config\arsenal.json' 'merge-defaults'
Stage-File (Join-Path $repoRoot 'config\holsters.json') 'scripts\LibertyFramework\config\holsters.json' 'keep-existing'
Get-ChildItem -LiteralPath (Join-Path $repoRoot 'config\presets') -Filter '*.json' | Sort-Object Name | ForEach-Object {
    Stage-File $_.FullName "scripts\LibertyFramework\config\presets\$($_.Name)" 'replace'
}


# T-023: body-part catalog of vehicle extras, generated from the player's own vehicles.img (read-only),
# plus LVS CE with its six workshop "Extra N" labels routed through that catalog.
$work = Join-Path $stage '_work'
New-Item -ItemType Directory -Force -Path $work | Out-Null
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$scanner = Join-Path $work 'VehicleExtras.exe'
$scannerSources = @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\finishes') -Filter '*.cs' | Where-Object Name -ne 'Program.cs').FullName) +
    @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\vehicles') -Filter '*.cs').FullName)
& $compiler /nologo /target:exe /platform:x86 /warn:4 /warnaserror+ "/out:$scanner" /reference:System.Runtime.Serialization.dll /reference:System.Drawing.dll /reference:System.Core.dll $scannerSources
if ($LASTEXITCODE -ne 0) { throw 'Vehicle extras scanner build failed.' }
& $scanner $game (Join-Path $work 'vehicle_extras.json')
if ($LASTEXITCODE -ne 0) { throw 'Vehicle extras scan failed.' }
Stage-File (Join-Path $work 'vehicle_extras.json') 'scripts\LibertyFramework\config\vehicle_extras.json' 'replace'

# S-2: weapon wheel icons = each installed weapon model's own HUD icon texture, extracted from the player's files.
$iconTool = Join-Path $work 'WeaponIcons.exe'
$iconSources = @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\finishes') -Filter '*.cs' | Where-Object Name -ne 'Program.cs').FullName) +
    @((Get-ChildItem -LiteralPath (Join-Path $repoRoot 'tools\ui') -Filter '*.cs').FullName)
& $compiler /nologo /target:exe /platform:x86 /warn:4 /warnaserror+ "/out:$iconTool" /reference:System.Runtime.Serialization.dll /reference:System.Drawing.dll /reference:System.Core.dll $iconSources
if ($LASTEXITCODE -ne 0) { throw 'Weapon icon extractor build failed.' }
$iconDir = Join-Path $work 'icons'
& $iconTool $game $iconDir
if ($LASTEXITCODE -ne 0) { throw 'Weapon icon extraction failed.' }
Get-ChildItem -LiteralPath $iconDir -Filter '*.png' | Sort-Object Name | ForEach-Object {
    Stage-File $_.FullName "scripts\LibertyFramework\ui\icons\$($_.Name)" 'replace'
}
if ($LvsDirectory) {
    $lvsSource = Join-Path (Resolve-Path -LiteralPath $LvsDirectory).Path 'scripts\LibertyVehicleServicesCE.CS'
    $lvsPatched = Join-Path $work 'LibertyVehicleServicesCE.CS'
    Copy-Item -LiteralPath $lvsSource -Destination $lvsPatched -Force
    & (Join-Path $repoRoot 'tools\vehicles\patch-lvs-labels.ps1') -LvsScript $lvsPatched
    # SHDN compiles .cs scripts at load; prove the patched script compiles against the same runtime first.
    & $compiler /nologo /target:library /platform:x86 "/out:$(Join-Path $work 'lvs_check.dll')" "/reference:$ScriptHookDotNetReference" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll $lvsPatched | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Patched LibertyVehicleServicesCE.CS does not compile.' }
    Stage-File $lvsPatched 'scripts\LibertyVehicleServicesCE.CS' 'replace'
}
Remove-Item -LiteralPath $work -Recurse -Force

$manifest = [ordered]@{
    package = 'liberty-framework-phase2'
    builtUtc = (Get-Date).ToUniversalTime().ToString('o')
    gameVersion = '1.2.0.59'
    files = $entries
}
[IO.File]::WriteAllText((Join-Path $stage 'manifest.json'), ($manifest | ConvertTo-Json -Depth 5), (New-Object Text.UTF8Encoding($false)))
Write-Host "Phase 2 staged $($entries.Count) files in $stage"

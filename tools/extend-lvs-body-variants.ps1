param(
    [Parameter(Mandatory = $true)][string] $LvsDirectory,
    [Parameter(Mandatory = $true)][string] $OutputPath
)

# MIT Liberty Vehicle Services CE 2026 release: present its verified, model-probed
# extra geometry as body variants. Purchase, preview rollback, owned INI capture and
# restore remain in the upstream workshop implementation.
$ErrorActionPreference = 'Stop'
$source = Join-Path (Resolve-Path -LiteralPath $LvsDirectory).Path 'scripts\LibertyVehicleServicesCE.CS'
$expectedSha256 = '5D4A3CC92A619E46A9B90CDC8D63620CB209ADC3B6E903733CD63F978F33F67C'
if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $expectedSha256) {
    throw 'LVS source differs from reviewed MIT release; review the new version before extending it.'
}
$content = [IO.File]::ReadAllText($source)
$replacements = [ordered]@{
    'subtitle = "EXTRAS";' = 'subtitle = "BODY VARIANTS";'
    'lines.Add("No available extras"); selectable.Add(false);' = 'lines.Add("No body variants on this vehicle"); selectable.Add(false);'
    'lines.Add("Extra " + extra.ToString() + ": " + state + marker + "     " + costText);' = 'lines.Add("Body variant " + extra.ToString() + ": " + state + marker + "     " + costText);'
    'text += (i == selectedIndex ? "> " : "  ") + "Extra " + extra.ToString() + ": " + state' = 'text += (i == selectedIndex ? "> " : "  ") + "Body variant " + extra.ToString() + ": " + state'
    'Notify("Extra " + extra.ToString() + " installed. Charged $" + charge.ToString() + ".");' = 'Notify("Body variant " + extra.ToString() + " installed. Charged $" + charge.ToString() + ".");'
    'Notify("Extra " + extra.ToString() + " removed free of charge.");' = 'Notify("Body variant " + extra.ToString() + " removed free of charge.");'
}
foreach ($old in $replacements.Keys) {
    $count = ([regex]::Matches($content, [regex]::Escape($old))).Count
    if ($count -ne 1) { throw "Expected one LVS anchor, got ${count}: $old" }
    $content = $content.Replace($old, $replacements[$old])
}
$rootLabel = '"Extras                         add $"'
if (([regex]::Matches($content, [regex]::Escape($rootLabel))).Count -ne 2) {
    throw 'Expected both LVS root workshop variant labels.'
}
$content = $content.Replace($rootLabel, '"Body variants                  add $"')
$target = [IO.Path]::GetFullPath($OutputPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($target)) | Out-Null
[IO.File]::WriteAllText($target, $content, (New-Object Text.UTF8Encoding($false)))
Write-Host "Extended LVS workshop body-variant labels: $target"

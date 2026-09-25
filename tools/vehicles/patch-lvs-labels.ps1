param(
    [Parameter(Mandatory = $true)]
    [string] $LvsScript
)

# T-023: applies the Liberty Framework body-part label helper to a copy of LibertyVehicleServicesCE.CS
# (MIT, ekzestean). Only the six workshop "Extra N" strings that have the serviced vehicle in scope are
# replaced; the dealer list is left as is. Refuses to patch an unexpected LVS version.
$ErrorActionPreference = 'Stop'
$text = [IO.File]::ReadAllText($LvsScript)
if ($text.Contains('LfBodyLabels')) { throw "$LvsScript is already patched" }
$old = '"Extra " + extra.ToString()'
$dealer = '"Extra " + extra.ToString() + ": " + (on ? "on" : "off")'
$total = ([regex]::Matches($text, [regex]::Escape($old))).Count
$dealerCount = ([regex]::Matches($text, [regex]::Escape($dealer))).Count
if ($total -ne 7 -or $dealerCount -ne 1) { throw "Unexpected LVS version: $total label sites, $dealerCount dealer site (expected 7 and 1)" }
$marker = '@@LF_DEALER_EXTRA@@'
$text = $text.Replace($dealer, $marker).Replace($old, 'LfBodyLabels.Extra(serviceVehicle, extra)').Replace($marker, $dealer)
$helper = [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'LfBodyLabels.cs.txt'))
# The workshop code lives in namespace AutoWorkshopCE (the file declares more than one namespace).
if (-not $text.Contains('namespace AutoWorkshopCE')) { throw 'namespace AutoWorkshopCE not found' }
$text = $text.TrimEnd() + "`r`n`r`nnamespace AutoWorkshopCE`r`n{`r`n" + $helper + "}`r`n"
[IO.File]::WriteAllText($LvsScript, $text, (New-Object Text.UTF8Encoding($false)))
Write-Host "Patched $LvsScript (6 workshop labels -> LfBodyLabels)"

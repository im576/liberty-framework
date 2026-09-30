param(
    # Scenario report folders (or results folders above them) holding the capture screenshots <point id>_day.png / _night.png.
    [Parameter(Mandatory = $true)][string[]] $OnReports,
    # Optional second set (mod-off): its screenshots are placed next to the mod-on ones, point by point.
    [string[]] $OffReports = @(),
    [Parameter(Mandatory = $true)][string] $Out,
    [int] $CellWidth = 320,
    [int] $Quality = 72
)

# One image with a row per capture point (locations.json "s1_*", in file order): day and night, mod-on and, when given,
# mod-off beside it. It is the reviewer's view of "does every point look like GTA IV, remastered" (STAGE1 Pillar 1) and of
# whether a point landed inside geometry or water (T-040), without opening 48 screenshots. Windows only (System.Drawing).
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
# powershell -File passes a comma list as one string.
$OnReports = @($OnReports | ForEach-Object { $_ -split ',' } | Where-Object { $_ })
$OffReports = @($OffReports | ForEach-Object { $_ -split ',' } | Where-Object { $_ })

function Find-Shot([string[]] $Roots, [string] $Name) {
    foreach ($root in $Roots) {
        foreach ($extension in 'png', 'jpg') {
            $hit = Get-ChildItem -LiteralPath $root -Recurse -Filter "$Name.$extension" -File -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
            if ($hit) { return $hit.FullName }
        }
    }
    return $null
}

$locations = Get-Content -LiteralPath (Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) 'config\devtools\locations.json') -Raw | ConvertFrom-Json
$points = @($locations.locations | Where-Object { $_.id -like 's1_*' })
$columns = @(@{ Set = 'on'; Time = 'day' }, @{ Set = 'off'; Time = 'day' }, @{ Set = 'on'; Time = 'night' }, @{ Set = 'off'; Time = 'night' })
if ($OffReports.Count -eq 0) { $columns = @(@{ Set = 'on'; Time = 'day' }, @{ Set = 'on'; Time = 'night' }) }
$cellHeight = [int]($CellWidth * 9 / 16)
$labelHeight = 16
$bitmap = New-Object System.Drawing.Bitmap ($CellWidth * $columns.Count), (($cellHeight + $labelHeight) * $points.Count + 20)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::FromArgb(20, 20, 20))
$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$font = New-Object System.Drawing.Font('Consolas', 9)
$brush = [System.Drawing.Brushes]::Gainsboro
$missing = 0
try {
    for ($c = 0; $c -lt $columns.Count; $c++) {
        $title = "$($columns[$c].Set -replace 'on', 'mod-on' -replace '^off$', 'mod-off') $($columns[$c].Time)"
        $graphics.DrawString($title, $font, $brush, ($c * $CellWidth + 4), 3)
    }
    for ($row = 0; $row -lt $points.Count; $row++) {
        $y = 20 + $row * ($cellHeight + $labelHeight)
        $graphics.DrawString($points[$row].id, $font, $brush, 4, $y)
        for ($c = 0; $c -lt $columns.Count; $c++) {
            $roots = if ($columns[$c].Set -eq 'on') { $OnReports } else { $OffReports }
            $path = Find-Shot $roots "$($points[$row].id)_$($columns[$c].Time)"
            if (-not $path) { $missing++; continue }
            $image = [System.Drawing.Image]::FromFile($path)
            try { $graphics.DrawImage($image, ($c * $CellWidth), ($y + $labelHeight), $CellWidth, $cellHeight) } finally { $image.Dispose() }
        }
    }
    $encoder = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
    $parameters = New-Object System.Drawing.Imaging.EncoderParameters 1
    $parameters.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter ([System.Drawing.Imaging.Encoder]::Quality), ([long]$Quality)
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent ([IO.Path]::GetFullPath($Out))) | Out-Null
    $bitmap.Save([IO.Path]::GetFullPath($Out), $encoder, $parameters)
}
finally { $graphics.Dispose(); $bitmap.Dispose() }
Write-Host "wrote $Out ($($points.Count) points x $($columns.Count) columns, $missing screenshot(s) missing)"

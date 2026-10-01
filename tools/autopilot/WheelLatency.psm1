# Validate the existing tick/draw measurement without redefining its <=1-engine-frame target.
function Test-WheelFirstDraw([string[]] $Lines) {
    $opens = 0; $draws = 0; $pending = $false; $maximum = 0
    foreach ($line in $Lines) {
        if ($line -match '\[INFO\] weapon_wheel_open frame=\d+ ') {
            if ($pending) { throw 'wheel opening has no first-draw measurement before the next opening' }
            $opens++; $pending = $true
        }
        elseif ($line -match '\[INFO\] weapon_wheel_first_draw frames=(-?\d+) ms=(-?\d+)') {
            if (-not $pending) { throw 'wheel first-draw measurement has no matching opening' }
            $frames = [int]$Matches[1]; $milliseconds = [int]$Matches[2]
            if ($frames -lt 0 -or $frames -gt 1 -or $milliseconds -lt 0) { throw "wheel first draw outside <=1 frame: frames=$frames ms=$milliseconds" }
            $maximum = [Math]::Max($maximum, $frames); $draws++; $pending = $false
        }
    }
    if ($opens -eq 0 -or $pending -or $draws -ne $opens) { throw "incomplete wheel first-draw coverage: opens=$opens draws=$draws" }
    return "wheel first-draw openings=$opens draws=$draws max_frames=$maximum target=1"
}
Export-ModuleMember -Function Test-WheelFirstDraw

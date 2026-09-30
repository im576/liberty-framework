# T-042: reads the `range_shot` lines the gunplay module writes during a range run (tools/autopilot/scenarios/stage1-gunplay-range.txt)
# and summarises delivered spread per weapon and group. Pure text processing, so it runs on any machine.
#
# range_shot line: "... range_shot weapon=7 n=1 gap_ms=-1 dev=0.123 cone=0.300 range_m=25.0 state=standing speed=0.0"
#   n = position in the chain (1 = first shot after a pause), dev = degrees between the bullet's path and the line to the aim point,
#   cone = the cone the spread model had written for that shot.

function ConvertFrom-RangeLog([string[]] $Lines) {
    $shots = New-Object System.Collections.Generic.List[object]
    foreach ($line in $Lines) {
        $m = [regex]::Match($line, 'range_shot weapon=(?<w>\d+) n=(?<n>\d+) gap_ms=(?<gap>-?\d+) dev=(?<dev>-?[\d.]+) cone=(?<cone>-?[\d.]+) range_m=(?<range>[\d.]+)')
        if (-not $m.Success) { continue }
        $inv = [Globalization.CultureInfo]::InvariantCulture
        $shots.Add([pscustomobject]@{
            weapon = [int]$m.Groups['w'].Value
            index = [int]$m.Groups['n'].Value
            gapMs = [int]$m.Groups['gap'].Value
            deviation = [double]::Parse($m.Groups['dev'].Value, $inv)
            cone = [double]::Parse($m.Groups['cone'].Value, $inv)
            rangeMeters = [double]::Parse($m.Groups['range'].Value, $inv)
        })
    }
    return , $shots.ToArray()
}

# Group of a shot: first (n = 1), burst (2-3), sustained (4+). Inside = deviation <= cone * 1.05 + 0.02 degrees.
function Get-RangeGroup([int] $Index) { if ($Index -le 1) { 'first' } elseif ($Index -le 3) { 'burst' } else { 'sustained' } }

function Get-RangeSummary([object[]] $Shots) {
    $result = New-Object System.Collections.Generic.List[object]
    foreach ($weapon in @($Shots | ForEach-Object { $_.weapon } | Sort-Object -Unique)) {
        foreach ($group in 'first', 'burst', 'sustained') {
            $rows = @($Shots | Where-Object { $_.weapon -eq $weapon -and (Get-RangeGroup $_.index) -eq $group })
            if ($rows.Count -eq 0) { $result.Add([pscustomobject]@{ weapon = $weapon; group = $group; n = 0 }); continue }
            $dev = @($rows | ForEach-Object { $_.deviation } | Sort-Object)
            $inside = @($rows | Where-Object { $_.deviation -le $_.cone * 1.05 + 0.02 }).Count
            $p95 = $dev[[Math]::Min($dev.Count - 1, [int][Math]::Ceiling($dev.Count * 0.95) - 1)]
            $result.Add([pscustomobject]@{
                weapon = $weapon; group = $group; n = $rows.Count
                meanDeviation = [Math]::Round((($dev | Measure-Object -Average).Average), 3)
                p95Deviation = [Math]::Round($p95, 3)
                maxDeviation = [Math]::Round($dev[-1], 3)
                meanCone = [Math]::Round((($rows | ForEach-Object { $_.cone } | Measure-Object -Average).Average), 3)
                insideCone = [Math]::Round($inside / $rows.Count, 2)
            })
        }
    }
    return , $result.ToArray()
}

function Format-RangeMarkdown([object[]] $Summary, [hashtable] $Labels) {
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add('| Weapon | Group | Shots | Mean dev | p95 dev | Max dev | Mean cone | Inside cone |')
    $lines.Add('|---|---|---|---|---|---|---|---|')
    foreach ($row in $Summary) {
        $name = if ($Labels -and $Labels.ContainsKey([int]$row.weapon)) { "$($Labels[[int]$row.weapon]) ($($row.weapon))" } else { "$($row.weapon)" }
        if ($row.n -eq 0) { $lines.Add("| $name | $($row.group) | 0 | - | - | - | - | - |"); continue }
        $lines.Add("| $name | $($row.group) | $($row.n) | $($row.meanDeviation) | $($row.p95Deviation) | $($row.maxDeviation) | $($row.meanCone) | $($row.insideCone) |")
    }
    return , $lines.ToArray()
}

Export-ModuleMember -Function ConvertFrom-RangeLog, Get-RangeGroup, Get-RangeSummary, Format-RangeMarkdown

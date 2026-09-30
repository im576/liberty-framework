# Parsing and summarising the Stage 1 measurement scenarios (T-040). Pure functions with no game or Windows dependency,
# so the cloud container tests them (tools/tests/Stage1Metrics.Tests.ps1). tools/perf/Measure-Stage1.ps1 is the command line.
#
# Input: an autopilot report folder (tools/autopilot/Run-Scenario.ps1): run.log (the engine log lines of the run, including
# one `command source=... line="<command>" reply="<reply>"` line per engine command), report.md and, when the scenario
# used `gpumem <label>`, measurements.json. A scenario groups its samples by `label <name>`: the first `framestats` and
# the first `costs` after a label only reset their counters; the last `framestats` is the section's frame statistics and
# every later `costs` is one window of per-section costs.
# Compatible with Windows PowerShell 5.1 and PowerShell 7.

$script:CommandLine = 'command source=\S+ line="(?<line>[^"]*)" reply="(?<reply>.*)"\s*$'

# "a=1 b=2.5 c=x%" -> @{ a = '1'; b = '2.5'; c = 'x%' } (the key=value words of an engine reply).
function ConvertFrom-KeyValueReply([string] $Reply) {
    $values = [ordered]@{}
    foreach ($m in [regex]::Matches($Reply, '(?<key>[A-Za-z_][A-Za-z0-9_]*)=(?<value>\S+)')) { $values[$m.Groups['key'].Value] = $m.Groups['value'].Value }
    return $values
}

function ConvertTo-Number($Value) {
    $number = 0.0
    if ($null -ne $Value -and [double]::TryParse(([string]$Value).TrimEnd('%'), [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$number)) { return $number }
    return $null
}

# `costs` reply -> @{ '<section>' = @{ avgMs; maxMs; count; totalMs } } ("costs_ms(avg/max/count@thread) name=avg/max/count@thread total=N ...").
function ConvertFrom-CostsReply([string] $Reply) {
    $sections = [ordered]@{}
    foreach ($m in [regex]::Matches($Reply, '(?<name>\S+)=(?<avg>[0-9.]+)/(?<max>[0-9.]+)/(?<count>\d+)@(?<thread>\d+) total=(?<total>\d+)')) {
        $sections[$m.Groups['name'].Value] = [ordered]@{
            avgMs = ConvertTo-Number $m.Groups['avg'].Value
            maxMs = ConvertTo-Number $m.Groups['max'].Value
            count = [int]$m.Groups['count'].Value
            totalMs = ConvertTo-Number $m.Groups['total'].Value
        }
    }
    return $sections
}

# Several windows of `costs` -> one table: the call-weighted mean, the largest maximum, the summed count and total.
function Merge-CostWindows([object[]] $Windows) {
    $merged = [ordered]@{}
    foreach ($window in @($Windows)) {
        foreach ($name in $window.Keys) {
            $row = $window[$name]
            if (-not $merged.Contains($name)) { $merged[$name] = [ordered]@{ avgMs = 0.0; maxMs = 0.0; count = 0; totalMs = 0.0 } }
            $target = $merged[$name]
            $target.totalMs += [double]$row.avgMs * $row.count
            $target.count += $row.count
            if ([double]$row.maxMs -gt $target.maxMs) { $target.maxMs = [double]$row.maxMs }
        }
    }
    foreach ($name in @($merged.Keys)) {
        $row = $merged[$name]
        if ($row.count -gt 0) { $row.avgMs = [math]::Round($row.totalMs / $row.count, 4) }
        $row.totalMs = [math]::Round($row.totalMs, 1)
    }
    return $merged
}

# The `command ... line="x" reply="y"` lines of a run log, in order.
function Get-CommandRecords([string[]] $LogLines) {
    $records = New-Object System.Collections.Generic.List[object]
    foreach ($line in @($LogLines)) {
        $m = [regex]::Match($line, $script:CommandLine)
        if ($m.Success) { $records.Add([pscustomobject]@{ Command = $m.Groups['line'].Value; Reply = $m.Groups['reply'].Value }) }
    }
    return $records.ToArray()
}

# One measured section per `label <name>` command: its frame statistics, cost windows, perf samples, last pools and the
# GPU measurements whose label is the section's label or starts with "<label>_".
function Get-MeasuredSections([object[]] $Records, [object[]] $Measurements) {
    $sections = New-Object System.Collections.Generic.List[object]
    $current = $null
    foreach ($record in @($Records)) {
        $words = $record.Command -split '\s+'
        if ($words[0] -eq 'label' -and $words.Count -ge 2) {
            $current = [ordered]@{ label = $words[1]; frameStatsReplies = @(); costWindows = @(); perfSamples = @(); pools = $null }
            $sections.Add($current)
            continue
        }
        if ($null -eq $current) { continue }
        switch ($words[0]) {
            'framestats' { $current.frameStatsReplies += $record.Reply }
            'costs' { $current.costWindows += , (ConvertFrom-CostsReply $record.Reply) }
            'perf' { $current.perfSamples += , (ConvertFrom-KeyValueReply $record.Reply) }
            'pools' { $current.pools = ConvertFrom-KeyValueReply $record.Reply }
        }
    }
    $results = New-Object System.Collections.Generic.List[object]
    foreach ($section in $sections) {
        $result = [ordered]@{ label = $section.label }
        # The first framestats/costs after the label reset the counters; the sample is what the later calls report.
        $stats = $null
        if ($section.frameStatsReplies.Count -ge 2) { $stats = ConvertFrom-KeyValueReply $section.frameStatsReplies[$section.frameStatsReplies.Count - 1] }
        if ($stats -and $stats.Contains('frames') -and [int]$stats['frames'] -gt 0) {
            $result.frames = [int]$stats['frames']
            foreach ($key in 'avg_ms', 'p50_ms', 'p95_ms', 'p99_ms', 'max_ms') { $result[$key] = ConvertTo-Number $stats[$key] }
            $result.over33 = [int]$stats['over33_ms']; $result.over100 = [int]$stats['over100_ms']; $result.stalls1s = [int]$stats['stalls_1s']
        }
        $windows = @($section.costWindows | Select-Object -Skip 1)
        $result.costs = if ($windows.Count -gt 0) { Merge-CostWindows $windows } else { [ordered]@{} }
        $result.perfSamples = $section.perfSamples.Count
        if ($section.perfSamples.Count -gt 0) {
            $last = $section.perfSamples[$section.perfSamples.Count - 1]
            $result.perf = [ordered]@{
                frameMs = ConvertTo-Number $last['frame_ms']; pressure = ConvertTo-Number $last['pressure']
                pressureMax = (@($section.perfSamples | ForEach-Object { ConvertTo-Number $_['pressure'] }) | Measure-Object -Maximum).Maximum
                privateMbMax = (@($section.perfSamples | ForEach-Object { ConvertTo-Number $_['private_mb'] }) | Measure-Object -Maximum).Maximum
                addressFreeMbMin = (@($section.perfSamples | ForEach-Object { ConvertTo-Number $_['address_free_mb'] }) | Measure-Object -Minimum).Minimum
                largestFreeBlockMbMin = (@($section.perfSamples | ForEach-Object { ConvertTo-Number $_['largest_free_block_mb'] }) | Measure-Object -Minimum).Minimum
                managedMbMax = (@($section.perfSamples | ForEach-Object { ConvertTo-Number $_['managed_mb'] }) | Measure-Object -Maximum).Maximum
            }
        }
        if ($section.pools) { $result.pools = $section.pools }
        $gpu = @(@($Measurements) | Where-Object { $null -ne $_ -and ($_.label -eq $section.label -or ([string]$_.label).StartsWith($section.label + '_')) })
        if ($gpu.Count -gt 0) {
            $result.gpuSamples = $gpu.Count
            $result.gpuDedicatedMbMax = ($gpu | ForEach-Object { [double]$_.gpuDedicatedMB } | Measure-Object -Maximum).Maximum
            $result.gpuDedicatedMbLast = [double]$gpu[$gpu.Count - 1].gpuDedicatedMB
            $result.privateMbMax = ($gpu | ForEach-Object { [double]$_.privateMB } | Measure-Object -Maximum).Maximum
            $result.workingSetMbMax = ($gpu | ForEach-Object { [double]$_.workingSetMB } | Measure-Object -Maximum).Maximum
            # Load on the rest of the PC while the sample ran (highest of the samples of this section). Absent = not recorded.
            foreach ($pair in @(@('systemCpuPercent', 'systemCpuPercentMax'), @('gameDiskBusyPercent', 'gameDiskBusyPercentMax'), @('gameDiskQueue', 'gameDiskQueueMax'))) {
                $values = @($gpu | Where-Object { $null -ne $_.($pair[0]) } | ForEach-Object { [double]$_.($pair[0]) })
                if ($values.Count -gt 0) { $result[$pair[1]] = ($values | Measure-Object -Maximum).Maximum }
            }
        }
        $results.Add($result)
    }
    return $results.ToArray()
}

# One report folder -> its summary object (scenario, condition, sections, engine stalls in the log).
function Get-ReportSummary([string] $ReportDirectory, [string] $Condition = '') {
    $runLog = Join-Path $ReportDirectory 'run.log'
    if (-not (Test-Path -LiteralPath $runLog)) { throw "no run.log in $ReportDirectory" }
    $lines = @(Get-Content -LiteralPath $runLog)
    $scenario = ''
    $reportFile = Join-Path $ReportDirectory 'report.md'
    if (Test-Path -LiteralPath $reportFile) {
        $head = Get-Content -LiteralPath $reportFile -TotalCount 1
        if ($head -match '^# Scenario (?<name>\S+)') { $scenario = $Matches['name'] }
    }
    $measurements = @()
    $measurementFile = Join-Path $ReportDirectory 'measurements.json'
    # Windows PowerShell 5.1 hands a JSON array on as ONE object; ForEach-Object unrolls it to the measurements.
    if (Test-Path -LiteralPath $measurementFile) { $measurements = @(Get-Content -LiteralPath $measurementFile -Raw | ConvertFrom-Json | ForEach-Object { $_ }) }
    $records = Get-CommandRecords $lines
    # Mod-off = the gameplay modules were stopped by the scenario itself (or the caller says so: engine.json disabledModules).
    if (-not $Condition) { $Condition = if (@($records | Where-Object { $_.Command -eq 'stop gunplay' }).Count -gt 0) { 'mod-off' } else { 'mod-on' } }
    # The atmosphere module's density governor lowers ped and car density when frames are slow (atmosphere.json density):
    # a sample taken while it was below 1.0 measured a thinner city. Its 30 s log lines show the lowest values.
    $densityPeds = @(); $densityCars = @()
    foreach ($line in $lines) {
        $m = [regex]::Match($line, '\bdensity frame_ms=\S+ peds=(?<p>[0-9.]+) cars=(?<c>[0-9.]+)')
        if ($m.Success) { $densityPeds += ConvertTo-Number $m.Groups['p'].Value; $densityCars += ConvertTo-Number $m.Groups['c'].Value }
    }
    return [ordered]@{
        scenario = $scenario
        condition = $Condition
        reportDirectory = (Split-Path -Leaf $ReportDirectory)
        engineStalls = @($lines | Where-Object { $_ -match 'engine_stall' }).Count
        logErrors = @($lines | Where-Object { $_ -match '\[ERROR\]' }).Count
        densityLines = $densityPeds.Count
        densityMinPeds = $(if ($densityPeds.Count -gt 0) { ($densityPeds | Measure-Object -Minimum).Minimum } else { $null })
        densityMinCars = $(if ($densityCars.Count -gt 0) { ($densityCars | Measure-Object -Minimum).Minimum } else { $null })
        sections = @(Get-MeasuredSections $records $measurements)
    }
}

# Budgets from STAGE1.md section 10 Pillar 5 (proposals until the owner confirms them; the VRAM one is confirmed).
function Get-Stage1Budgets {
    return [ordered]@{
        p95IncreasePercent = 10.0
        p99IncreasePercent = 15.0
        vramOverVanillaMbCeiling = 350.0
        vramOverVanillaMbNormal = 300.0
        stage1ScriptCostMs = 3.0
        maxStallMs = 1000.0
    }
}

function Format-Delta($On, $Off, [string] $Unit = '') {
    if ($null -eq $On -or $null -eq $Off) { return 'n/a' }
    return ('{0:+0.0;-0.0;0.0}{1}' -f ([double]$On - [double]$Off), $Unit)
}

function Format-Percent($On, $Off) {
    if ($null -eq $On -or $null -eq $Off -or [double]$Off -le 0) { return 'n/a' }
    return ('{0:+0.0;-0.0;0.0}%' -f (100.0 * ([double]$On - [double]$Off) / [double]$Off))
}

function Format-Number($Value, [string] $Format) {
    if ($null -eq $Value) { return 'n/a' }
    return ([double]$Value).ToString($Format, [Globalization.CultureInfo]::InvariantCulture)
}

function Format-Pair($On, $Off, [string] $Format) { return (Format-Number $On $Format) + ' / ' + (Format-Number $Off $Format) }

# A cost row by section name from either a summary built in memory (ordered dictionary) or one read back from JSON.
function Get-CostRow($Costs, [string] $Name) {
    if ($null -eq $Costs) { return $null }
    if ($Costs -is [Collections.IDictionary]) { if ($Costs.Contains($Name)) { return $Costs[$Name] } return $null }
    $property = $Costs.PSObject.Properties[$Name]
    if ($property) { return $property.Value }
    return $null
}

# Mod-on and mod-off summaries (in memory or read back from JSON) -> a markdown table with one row per measured label:
# the values, and OVER where a Stage 1 budget of Get-Stage1Budgets is exceeded (mod-on against mod-off).
function Format-Stage1Comparison($OnSummary, $OffSummary) {
    $budgets = Get-Stage1Budgets
    $none = [pscustomobject]@{ costs = @{} }
    $out = New-Object System.Collections.Generic.List[string]
    $out.Add('| Section | frame avg ms on / off | p50 on / off | p95 on / off (change) | p99 on / off (change) | max ms on | engine.frame avg ms on / off | GPU MB on / off (change) | private MB on / off | frames >100 ms | stalls >=1 s | PC load on / off: CPU % and game disk busy % |')
    $out.Add('|---|---|---|---|---|---|---|---|---|---|---|---|')
    foreach ($on in @($OnSummary.sections)) {
        $off = @($OffSummary.sections | Where-Object { $_.label -eq $on.label }) | Select-Object -First 1
        if ($null -eq $off) { $off = $none }
        $onCost = Get-CostRow $on.costs 'engine.frame'
        $offCost = Get-CostRow $off.costs 'engine.frame'
        $p95Change = if ($null -ne $on.p95_ms -and $null -ne $off.p95_ms -and [double]$off.p95_ms -gt 0) { 100.0 * ($on.p95_ms - $off.p95_ms) / $off.p95_ms } else { $null }
        $p99Change = if ($null -ne $on.p99_ms -and $null -ne $off.p99_ms -and [double]$off.p99_ms -gt 0) { 100.0 * ($on.p99_ms - $off.p99_ms) / $off.p99_ms } else { $null }
        $vramChange = if ($null -ne $on.gpuDedicatedMbMax -and $null -ne $off.gpuDedicatedMbMax) { $on.gpuDedicatedMbMax - $off.gpuDedicatedMbMax } else { $null }
        $p95 = (Format-Pair $on.p95_ms $off.p95_ms '0.0') + ' (' + (Format-Percent $on.p95_ms $off.p95_ms) + $(if ($null -ne $p95Change -and $p95Change -gt $budgets.p95IncreasePercent) { ' OVER' } else { '' }) + ')'
        $p99 = (Format-Pair $on.p99_ms $off.p99_ms '0.0') + ' (' + (Format-Percent $on.p99_ms $off.p99_ms) + $(if ($null -ne $p99Change -and $p99Change -gt $budgets.p99IncreasePercent) { ' OVER' } else { '' }) + ')'
        $gpu = (Format-Pair $on.gpuDedicatedMbMax $off.gpuDedicatedMbMax '0') + ' (' + (Format-Delta $on.gpuDedicatedMbMax $off.gpuDedicatedMbMax ' MB') + $(if ($null -ne $vramChange -and $vramChange -gt $budgets.vramOverVanillaMbCeiling) { ' OVER' } else { '' }) + ')'
        $load = 'CPU ' + (Format-Pair $on.systemCpuPercentMax $off.systemCpuPercentMax '0') + ', disk ' + (Format-Pair $on.gameDiskBusyPercentMax $off.gameDiskBusyPercentMax '0')
        $out.Add(('| {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} | {11} |' -f $on.label,
            (Format-Pair $on.avg_ms $off.avg_ms '0.00'), (Format-Pair $on.p50_ms $off.p50_ms '0.0'), $p95, $p99, (Format-Number $on.max_ms '0.0'),
            (Format-Pair $(if ($onCost) { $onCost.avgMs }) $(if ($offCost) { $offCost.avgMs }) '0.000'), $gpu,
            (Format-Pair $on.privateMbMax $off.privateMbMax '0'), $on.over100, $on.stalls1s, $load))
    }
    return $out.ToArray()
}
Export-ModuleMember -Function ConvertFrom-KeyValueReply, ConvertFrom-CostsReply, Merge-CostWindows, Get-CommandRecords, Get-MeasuredSections, Get-ReportSummary, Get-Stage1Budgets, Format-Stage1Comparison


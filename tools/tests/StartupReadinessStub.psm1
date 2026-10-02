# Test-only dependencies for the ACTUAL production readiness loop; no Steam, process, audio or UI calls.
. (Join-Path (Split-Path -Parent $PSScriptRoot) 'autopilot/StartupReadiness.ps1')

function Initialize-StartupStub([hashtable] $Options = @{}) {
    $script:Options = $Options
    # PS 7.5+ may deserialize ISO JSON timestamps as DateTime; stringifying one loses its UTC kind.
    $script:Now = if ($Options.ContainsKey('InitialUtc')) { ([DateTime]$Options.InitialUtc).ToUniversalTime() } else { [DateTime]::UtcNow }
    $script:Base = $script:Now
    $script:Launches = 0; $script:Stops = 0; $script:LaunchAt = $null
    $script:Owned = @(); $script:Game = 'stub'; $script:LaunchedUtc = [datetime]::MinValue
}
function Get-StubOption([string] $Name, $Default) {
    if ($script:Options.ContainsKey($Name)) { return $script:Options[$Name] }
    return $Default
}
function Get-Date([string] $Format = '') {
    if ($Format) { return $script:Now.ToString($Format) }
    $script:Now
}
function Start-Sleep([int] $Milliseconds = 0, [int] $Seconds = 0) {
    $script:Now = $script:Now.AddMilliseconds($Milliseconds + $Seconds * 1000)
    $tracePath = Get-StubOption 'TracePath' ''
    if ($tracePath) {
        [IO.File]::AppendAllText($tracePath, (([ordered]@{ utc = $script:Now.ToString('o'); seconds = $Seconds; milliseconds = $Milliseconds } | ConvertTo-Json -Compress) + "`n"))
    }
}
function Assert-AudioOutput { if (Get-StubOption 'NoAudio' $false) { throw 'GAME-UNAVAILABLE: no audio output device is active (stub)' } }
function Start-Process([string] $FilePath) {
    if ($FilePath -ne 'steam://rungameid/12210') { throw 'Unexpected launch command in startup stub' }
    if (Get-StubOption 'LaunchError' $false) { throw 'stub launch error' }
    $script:Launches++; $script:LaunchAt = $script:Now
}
function Stop-Game { $script:Stops++; $script:Now = $script:Now.AddSeconds(2); $script:LaunchAt = $null }
function Get-GameProcess {
    if (-not $script:LaunchAt) { return $null }
    if (Get-StubOption 'NeverSeen' $false) { return $null }
    $elapsed = ($script:Now - $script:LaunchAt).TotalSeconds
    if ($elapsed -ge (Get-StubOption 'ExitAfter' ([double]::PositiveInfinity))) { return $null }
    $delay = Get-StubOption 'PidDelay' 0
    if ($script:Launches -gt 1) { $delay = Get-StubOption 'RetryPidDelay' $delay }
    if ($elapsed -lt $delay) { return $null }
    [pscustomobject]@{ Id = 100 + $script:Launches; StartTime = $script:LaunchAt.AddSeconds($delay) }
}
function Get-SessionLog {
    $script:Now = $script:Now.AddSeconds((Get-StubOption 'LogReadSeconds' 0))
    if (-not $script:LaunchAt) { return @() }
    $delay = Get-StubOption 'BootAfter' 3
    if ($script:Launches -gt 1) { $delay = Get-StubOption 'RetryBootAfter' $delay }
    if (($script:Now - $script:LaunchAt).TotalSeconds -ge $delay) {
        return @($script:Now.ToString('o') + ' [INFO] engine_booted stub=True')
    }
    return @()
}
function Get-GameDialog { Get-StubOption 'Dialog' $null }
function Write-AutopilotLaunch($Game, $Process) { $script:Owned += $Process.Id }
function Set-AutopilotGame($GameDirectory) { $script:Game = $GameDirectory }
function Get-StartupStubState { [pscustomobject]@{ Now = $script:Now; Base = $script:Base; Launches = $script:Launches; Stops = $script:Stops; Owned = $script:Owned } }
function Test-GameFrozen($Seconds) { return $false }
function Invoke-EngineCommand($Lines) { return 'ok' }

$initial = @{}
if ($env:LIBERTY_STARTUP_STUB) {
    $decoded = Get-Content -LiteralPath $env:LIBERTY_STARTUP_STUB -Raw | ConvertFrom-Json
    foreach ($property in $decoded.PSObject.Properties) { $initial[$property.Name] = $property.Value }
}
Initialize-StartupStub $initial
Export-ModuleMember -Function Start-GameReady, Initialize-StartupStub, Get-StartupStubState, Set-AutopilotGame,
    Get-GameProcess, Get-SessionLog, Stop-Game, Test-GameFrozen, Invoke-EngineCommand, Get-Date, Start-Sleep

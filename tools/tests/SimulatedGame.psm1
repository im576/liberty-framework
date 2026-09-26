# A stand-in for tools/autopilot/Autopilot.psm1 with the same functions, backed by files instead of GTA IV, so the
# autopilot runner and verify-local.ps1 can be tested in the cloud container (verify-local.ps1 -Simulate and
# tools/tests/*.Tests.ps1). It never touches a game.
#
# $env:LIBERTY_SIM_STATE is a folder holding the simulated game:
#   world.json   how the "engine" behaves (all optional):
#                  running            the game is running before the scenario (default true)
#                  launchFails        Start-GameReady throws (no boot)
#                  screenshotFails    Save-Screenshot throws (no Steam screenshot)
#                  commands           { "<command line>": { "reply": "...", "log": ["..."], "crash": true } }
#                                     unknown commands reply "unknown command <word>"; known ones default to "ok"
#                  knownCommands      command words that reply "ok" without a commands entry
#   log.txt      the Liberty log (lines are prefixed with a UTC timestamp like the real log)
#   alive        present while the simulated game runs
# Import-Module -Force (Run-Suite runs each scenario that way) keeps the state because it lives on disk.

$ErrorActionPreference = 'Stop'

function Get-SimState {
    if (-not $env:LIBERTY_SIM_STATE) { throw 'LIBERTY_SIM_STATE is not set' }
    return $env:LIBERTY_SIM_STATE
}

function Get-SimWorld {
    $path = Join-Path (Get-SimState) 'world.json'
    if (-not (Test-Path -LiteralPath $path)) { return [pscustomobject]@{} }
    return Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
}

function Get-SimProperty($object, [string] $name, $default) {
    if ($null -ne $object -and $object.PSObject.Properties[$name]) { return $object.$name }
    return $default
}

function Add-SimLog([string] $text) {
    $line = [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ss.fffZ') + ' [INFO] ' + $text
    if ($text.StartsWith('[ERROR] ')) { $line = [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ss.fffZ') + ' ' + $text }
    Add-Content -LiteralPath (Join-Path (Get-SimState) 'log.txt') -Value $line
}

# Prepares a state folder: world.json from $World (a hashtable), the game running unless World.running is false, and
# optional log lines already in the log (lines from before the scenario, to test stale matches).
function Initialize-SimulatedGame([string] $State, [hashtable] $World, [string[]] $ExistingLog) {
    if (Test-Path -LiteralPath $State) { Remove-Item -LiteralPath $State -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $State | Out-Null
    [IO.File]::WriteAllText((Join-Path $State 'world.json'), ($World | ConvertTo-Json -Depth 6))
    $env:LIBERTY_SIM_STATE = $State
    New-Item -ItemType File -Force -Path (Join-Path $State 'log.txt') | Out-Null
    foreach ($line in @($ExistingLog)) { if ($line) { Add-SimLog $line } }
    $running = if ($World.ContainsKey('running')) { [bool]$World.running } else { $true }
    if ($running) { New-Item -ItemType File -Force -Path (Join-Path $State 'alive') | Out-Null }
}

function Set-AutopilotGame([string] $GameDirectory) { }

function Get-GameProcess {
    if (Test-Path -LiteralPath (Join-Path (Get-SimState) 'alive')) { return [pscustomobject]@{ Id = 4242; Name = 'GTAIV' } }
    return $null
}

function Get-SessionLog {
    $path = Join-Path (Get-SimState) 'log.txt'
    if (-not (Test-Path -LiteralPath $path)) { return @() }
    return [string[]]@(Get-Content -LiteralPath $path)
}

function Start-GameReady([int] $Attempts = 5, [int] $BootTimeoutSeconds = 240) {
    if (Get-SimProperty (Get-SimWorld) 'launchFails' $false) { throw "GTA IV did not reach the engine after $Attempts attempts" }
    New-Item -ItemType File -Force -Path (Join-Path (Get-SimState) 'alive') | Out-Null
    Add-SimLog 'engine_booted'
    return 1
}

function Stop-Game { Remove-Item -LiteralPath (Join-Path (Get-SimState) 'alive') -ErrorAction SilentlyContinue }

function Send-GameKey([string] $Key, [int] $HoldMs = 80) {
    if (-not (Get-GameProcess)) { throw 'game window not available' }
}

function Save-Screenshot([string] $Path) {
    if (-not (Get-GameProcess)) { throw 'game not running' }
    if (Get-SimProperty (Get-SimWorld) 'screenshotFails' $false) { throw "no Steam screenshot for $(Split-Path -Leaf $Path)" }
    # A tiny valid PNG (1x1) stands in for the capture.
    $png = [Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==')
    [IO.File]::WriteAllBytes($Path, $png)
    return $Path
}

function Invoke-EngineCommand([string[]] $Lines, [int] $TimeoutSeconds = 20) {
    if (-not (Get-GameProcess)) { throw 'GTA IV exited while a command was pending' }
    $world = Get-SimWorld
    $commands = Get-SimProperty $world 'commands' $null
    $known = @(Get-SimProperty $world 'knownCommands' @())
    $replies = @()
    foreach ($line in $Lines) {
        $entry = Get-SimProperty $commands $line $null
        $word = ($line -split '\s+')[0]
        if ($null -ne $entry) { $reply = [string](Get-SimProperty $entry 'reply' 'ok') }
        elseif ($known -contains $word) { $reply = 'ok' }
        else { $reply = "unknown command $word" }
        Add-SimLog ('command source=file:sim line="' + $line + '" reply="' + $reply + '"')
        if ($null -ne $entry) { foreach ($log in @(Get-SimProperty $entry 'log' @())) { Add-SimLog $log } }
        $replies += "$line => $reply"
        if ($null -ne $entry -and (Get-SimProperty $entry 'crash' $false)) { Stop-Game }
    }
    return $replies
}

function Test-GameFrozen([int] $Seconds = 75) { return $false }
function Wait-LogLine([string] $Pattern, [int] $TimeoutSeconds = 180) { return (Get-SessionLog | Where-Object { $_ -match $Pattern } | Select-Object -First 1) }

Export-ModuleMember -Function Initialize-SimulatedGame, Set-AutopilotGame, Get-GameProcess, Get-SessionLog, Start-GameReady, Stop-Game,
    Send-GameKey, Save-Screenshot, Invoke-EngineCommand, Test-GameFrozen, Wait-LogLine

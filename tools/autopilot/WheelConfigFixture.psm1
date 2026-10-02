# T-045 host fixture. Only arsenal.json's flat weaponWheel block is perturbed; original bytes and module state
# are captured before any mutation. The runner must call Restore-WheelConfigFixture in finally, even on fail-fast.
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'AutopilotLogic.psm1')

function Get-WheelRunning([string[]] $Reply) {
    $text = $Reply -join ' | '
    if ($text -notmatch '(?:^|[|>]\s*)weapon-wheel(?<off>\(off:[^)]*\))?\s+avg=') {
        throw "modules reply did not identify weapon-wheel: $text"
    }
    return -not [bool]$Matches.off
}

function Invoke-WheelFixtureCommand([scriptblock] $Command, [string] $Line) {
    $reply = @(& $Command $Line)
    if (Test-CommandFailed $reply) { throw "wheel fixture command refused: $($reply -join ' | ')" }
    return $reply
}

function New-WheelConfigFixture([string] $GameDirectory, [string] $Report, [scriptblock] $Command) {
    $path = Join-Path $GameDirectory 'scripts/LibertyFramework/config/arsenal.json'
    $bytes = [IO.File]::ReadAllBytes($path)
    $text = [Text.Encoding]::UTF8.GetString($bytes).TrimStart([char]0xFEFF)
    $parsed = $text | ConvertFrom-Json
    if (-not $parsed.weaponWheel -or $null -eq $parsed.weaponWheel.enabled) { throw 'wheel fixture needs a weaponWheel block' }
    # Require the exact flat schema this fixture edits; never apply a broad JSON rewrite to shared tuning.
    $blocks = [regex]::Matches($text, '(?s)"weaponWheel"\s*:\s*\{[^{}]*\}')
    if ($blocks.Count -ne 1) { throw 'wheel fixture needs exactly one flat weaponWheel block' }
    $running = Get-WheelRunning @(Invoke-WheelFixtureCommand $Command 'modules')
    $stamp = [IO.File]::GetLastWriteTimeUtc($path)
    $nextStamp = [DateTime]::UtcNow
    if ($nextStamp -lt $stamp) { $nextStamp = $stamp }
    [IO.File]::WriteAllBytes((Join-Path $Report 'wheel-config-original.json'), $bytes)
    return [pscustomobject]@{
        Path = $path; Original = $bytes; Text = $text; Stamp = $stamp
        NextStamp = $nextStamp; Running = $running; Enabled = [bool]$parsed.weaponWheel.enabled
        Report = $Report; Restored = $false
    }
}

function Set-WheelFixtureBytes($Fixture, [byte[]] $Bytes, [DateTime] $Stamp) {
    $temporary = $Fixture.Path + '.wheel-fixture-' + [Guid]::NewGuid().ToString('N')
    try {
        [IO.File]::WriteAllBytes($temporary, $Bytes)
        [IO.File]::SetLastWriteTimeUtc($temporary, $Stamp)
        [IO.File]::Replace($temporary, $Fixture.Path, [NullString]::Value)
    }
    finally { if (Test-Path -LiteralPath $temporary) { Remove-Item -LiteralPath $temporary -Force } }
}

function Set-WheelConfigFixture($Fixture, [ValidateSet('enabled','disabled','invalid')] [string] $Mode) {
    if ($Fixture.Restored) { throw 'wheel fixture already restored' }
    $block = [regex]::Match($Fixture.Text, '(?s)"weaponWheel"\s*:\s*\{[^{}]*\}')
    $candidate = $block.Value
    if ([regex]::Matches($candidate, '"enabled"\s*:\s*(true|false)').Count -ne 1 -or
        [regex]::Matches($candidate, '"keyboardKey"\s*:\s*"[^"\\]*"').Count -ne 1) { throw 'unexpected wheel fields' }
    $enabled = if ($Mode -eq 'enabled') { 'true' } else { 'false' }
    $candidate = [regex]::Replace($candidate, '("enabled"\s*:\s*)(true|false)', '${1}' + $enabled)
    if ($Mode -eq 'invalid') {
        # Enter is an existing navigation collision rejected by WeaponWheelConfig.Validate, not a malformed root JSON.
        $candidate = [regex]::Replace($candidate, '("keyboardKey"\s*:\s*)"[^"\\]*"', '${1}"Enter"')
    }
    $text = $Fixture.Text.Substring(0, $block.Index) + $candidate + $Fixture.Text.Substring($block.Index + $block.Length)
    $Fixture.NextStamp = $Fixture.NextStamp.AddSeconds(2)
    Set-WheelFixtureBytes $Fixture ([Text.Encoding]::UTF8.GetBytes($text)) $Fixture.NextStamp
}

function Test-WheelFixtureReload([string[]] $Lines, [ValidateSet('quiet','single')] [string] $Mode) {
    $count = @($Lines | Where-Object { $_ -match '\[INFO\] weapon_wheel_config ' }).Count
    $rejected = @($Lines | Where-Object { $_ -match '\[ERROR\] weapon_wheel_config_rejected ' }).Count
    $expected = if ($Mode -eq 'quiet') { 0 } else { 1 }
    if ($count -ne $expected -or $rejected -ne 0) { throw "wheel fixture $Mode accepted=$count expected=$expected rejected=$rejected" }
    return "wheel fixture $Mode accepted=$count rejected=$rejected"
}

function Restore-WheelConfigFixture($Fixture, [scriptblock] $Command, [scriptblock] $Alive) {
    # Bytes are restored first, so even a crashed/frozen game cannot leave invalid config behind.
    Set-WheelFixtureBytes $Fixture $Fixture.Original $Fixture.Stamp
    $current = [IO.File]::ReadAllBytes($Fixture.Path)
    if ([Convert]::ToBase64String($current) -cne [Convert]::ToBase64String($Fixture.Original)) { throw 'wheel fixture byte restoration mismatch' }
    if (-not (& $Alive)) { throw 'wheel fixture config restored; live module state cannot be restored after game exit' }
    $verb = if ($Fixture.Running) { 'restart' } else { 'stop' }
    Invoke-WheelFixtureCommand $Command "$verb weapon-wheel" | Out-Null
    $running = Get-WheelRunning @(Invoke-WheelFixtureCommand $Command 'modules')
    if ($running -ne $Fixture.Running) { throw 'wheel fixture module state restoration mismatch' }
    if ($running) {
        $status = (Invoke-WheelFixtureCommand $Command 'wheel status') -join ' | '
        $expected = 'enabled=' + $Fixture.Enabled
        if ($status -notmatch ('\b' + [regex]::Escape($expected) + '\b')) { throw 'wheel fixture enabled config restoration mismatch' }
    }
    $Fixture.Restored = $true
    [IO.File]::WriteAllText((Join-Path $Fixture.Report 'wheel-config-restored.txt'), "bytes_identical=True module_running=$running config_enabled=$($Fixture.Enabled)")
    return "wheel fixture restored original bytes module_running=$running config_enabled=$($Fixture.Enabled)"
}

Export-ModuleMember -Function New-WheelConfigFixture, Set-WheelConfigFixture, Restore-WheelConfigFixture, Test-WheelFixtureReload

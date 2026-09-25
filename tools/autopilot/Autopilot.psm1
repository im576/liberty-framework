# Liberty autopilot (host side): starts GTA IV through Steam, watches for crashes and freezes, presses keys, takes
# screenshots, and talks to the engine's command channel (scripts\LibertyFramework\autopilot\inbox|outbox).
# Import-Module tools\autopilot\Autopilot.psm1; Set-AutopilotGame 'C:\...\GTAIV'
$ErrorActionPreference = 'Stop'
$script:Game = $null
$script:LaunchedUtc = [DateTime]::MinValue

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class AutopilotNative
{
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr window, int command);
    [DllImport("user32.dll")] public static extern uint MapVirtualKey(uint code, uint mapType);
    [DllImport("user32.dll")] public static extern void keybd_event(byte vk, byte scan, uint flags, UIntPtr extra);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr window, out Rect rect);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, int x, int y, uint data, UIntPtr extra);
    public static void Click(int x, int y)
    {
        SetCursorPos(x, y);
        System.Threading.Thread.Sleep(80);
        mouse_event(0x0002, 0, 0, 0, UIntPtr.Zero);
        System.Threading.Thread.Sleep(60);
        mouse_event(0x0004, 0, 0, 0, UIntPtr.Zero);
    }
    [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left, Top, Right, Bottom; }
    // Scancode key press: DirectInput games read scancodes, not virtual-key messages.
    public static void Press(byte vk, int holdMs)
    {
        byte scan = (byte)MapVirtualKey(vk, 0);
        keybd_event(vk, scan, 0x0008, UIntPtr.Zero);
        System.Threading.Thread.Sleep(holdMs);
        keybd_event(vk, scan, 0x0008 | 0x0002, UIntPtr.Zero);
    }
}
'@

function Set-AutopilotGame([string] $GameDirectory) { $script:Game = (Resolve-Path -LiteralPath $GameDirectory).Path }

function Get-GameProcess { Get-Process GTAIV -ErrorAction SilentlyContinue | Select-Object -First 1 }

function Get-LogPath { Join-Path $script:Game 'scripts\LibertyFramework\logs\LibertyFramework.log' }

# Log lines written since the current launch (UTC timestamps at the start of each line).
function Get-SessionLog {
    $path = Get-LogPath
    if (-not (Test-Path -LiteralPath $path)) { return @() }
    $since = $script:LaunchedUtc.ToString('yyyy-MM-ddTHH:mm:ss')
    $lines = New-Object System.Collections.Generic.List[string]
    $stream = [IO.File]::Open($path, 'Open', 'Read', 'ReadWrite')
    try {
        $reader = New-Object IO.StreamReader($stream)
        while (-not $reader.EndOfStream) {
            $line = $reader.ReadLine()
            if ($line.Length -ge 19 -and [string]::CompareOrdinal($line.Substring(0, 19), $since) -ge 0) { $lines.Add($line) }
        }
    } finally { $stream.Dispose() }
    return $lines.ToArray()
}

function Start-Game {
    if (Get-GameProcess) { throw 'GTA IV is already running' }
    Stop-Game
    $script:LaunchedUtc = [DateTime]::UtcNow
    Start-Process 'steam://rungameid/12210'
    $deadline = (Get-Date).AddSeconds(120)
    while ((Get-Date) -lt $deadline) {
        $process = Get-GameProcess
        if ($process) { return $process }
        Start-Sleep -Seconds 2
    }
    throw 'GTA IV did not start within 120 s'
}

# Kills the game and the Rockstar helpers a crash leaves behind (otherwise Steam refuses with "Game already running").
function Stop-Game {
    foreach ($name in 'GTAIV', 'PlayGTAIV', 'RockstarSteamHelper', 'RockstarErrorHandler', 'Launcher', 'SocialClubHelper') {
        Get-Process $name -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue }
    }
    $deadline = (Get-Date).AddSeconds(20)
    while ((Get-GameProcess) -and (Get-Date) -lt $deadline) { Start-Sleep -Milliseconds 500 }
    Start-Sleep -Seconds 2
}

function Focus-Game {
    $process = Get-GameProcess
    if (-not $process -or $process.MainWindowHandle -eq [IntPtr]::Zero) { return $false }
    [AutopilotNative]::ShowWindow($process.MainWindowHandle, 9) | Out-Null
    [AutopilotNative]::SetForegroundWindow($process.MainWindowHandle) | Out-Null
    Start-Sleep -Milliseconds 300
    return $true
}

# Keys: names from System.Windows.Forms.Keys (Enter, Escape, Up, Down, Left, Right, Space, A..Z, F1..F12).
function Send-GameKey([string] $Key, [int] $HoldMs = 80) {
    if (-not (Focus-Game)) { throw 'game window not available' }
    $vk = [byte][int][System.Windows.Forms.Keys]$Key
    [AutopilotNative]::Press($vk, $HoldMs)
}

# The game presents through Vulkan (DXVK), which GDI capture sees as black; Steam's overlay screenshot (F12) captures
# the real frame. Falls back to a desktop capture when no Steam screenshot appears.
function Save-Screenshot([string] $Path) {
    $folders = Get-ChildItem 'C:\Program Files (x86)\Steam\userdata' -Directory -ErrorAction SilentlyContinue | ForEach-Object { Join-Path $_.FullName '760\remote\12210\screenshots' }
    $before = Get-Date
    if ((Get-GameProcess) -and (Focus-Game)) {
        Send-GameKey F12 120
        $deadline = (Get-Date).AddSeconds(8)
        while ((Get-Date) -lt $deadline) {
            Start-Sleep -Milliseconds 500
            $shot = $folders | Where-Object { Test-Path $_ } | ForEach-Object { Get-ChildItem $_ -Filter *.jpg } | Where-Object { $_.LastWriteTime -gt $before } | Sort-Object LastWriteTime -Descending | Select-Object -First 1
            if ($shot) {
                Start-Sleep -Milliseconds 300
                $image = [System.Drawing.Image]::FromFile($shot.FullName)
                try { $small = New-Object System.Drawing.Bitmap($image, [int]($image.Width / 2), [int]($image.Height / 2)); $small.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png); $small.Dispose() }
                finally { $image.Dispose() }
                return $Path
            }
        }
    }
    return Save-DesktopScreenshot $Path
}

function Save-DesktopScreenshot([string] $Path) {
    $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $process = Get-GameProcess
    if ($process -and $process.MainWindowHandle -ne [IntPtr]::Zero) {
        $rect = New-Object AutopilotNative+Rect
        if ([AutopilotNative]::GetWindowRect($process.MainWindowHandle, [ref]$rect) -and ($rect.Right - $rect.Left) -gt 100) {
            $bounds = New-Object System.Drawing.Rectangle($rect.Left, $rect.Top, ($rect.Right - $rect.Left), ($rect.Bottom - $rect.Top))
        }
    }
    $bitmap = New-Object System.Drawing.Bitmap($bounds.Width, $bounds.Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
        # Half size keeps screenshots light enough to review quickly.
        $small = New-Object System.Drawing.Bitmap($bitmap, [int]($bounds.Width / 2), [int]($bounds.Height / 2))
        $small.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
        $small.Dispose()
    } finally { $graphics.Dispose(); $bitmap.Dispose() }
    return $Path
}

# Waits for a log line matching $Pattern in this session. Returns the line, or $null on timeout. Throws if the game exits.
function Wait-LogLine([string] $Pattern, [int] $TimeoutSeconds = 180) {
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $match = Get-SessionLog | Where-Object { $_ -match $Pattern } | Select-Object -First 1
        if ($match) { return $match }
        if (-not (Get-GameProcess)) { throw "GTA IV exited while waiting for '$Pattern'" }
        Start-Sleep -Seconds 2
    }
    return $null
}

# Frozen = the process lives but the engine has written nothing for $Seconds (engine_status is logged every 30 s).
function Test-GameFrozen([int] $Seconds = 75) {
    $path = Get-LogPath
    if (-not (Get-GameProcess)) { return $false }
    $age = ((Get-Date) - (Get-Item -LiteralPath $path).LastWriteTime).TotalSeconds
    return $age -gt $Seconds
}

# Runs engine commands through the file channel and returns the reply lines.
function Invoke-EngineCommand([string[]] $Lines, [int] $TimeoutSeconds = 20) {
    $root = Join-Path $script:Game 'scripts\LibertyFramework\autopilot'
    $inbox = Join-Path $root 'inbox'
    $outbox = Join-Path $root 'outbox'
    New-Item -ItemType Directory -Force -Path $inbox, $outbox | Out-Null
    $name = 'cmd_' + [DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')
    $reply = Join-Path $outbox "$name.out"
    [IO.File]::WriteAllLines((Join-Path $inbox "$name.tmp"), $Lines)
    Move-Item -LiteralPath (Join-Path $inbox "$name.tmp") -Destination (Join-Path $inbox "$name.cmd")
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        if (Test-Path -LiteralPath $reply) { Start-Sleep -Milliseconds 100; $text = Get-Content -LiteralPath $reply; Remove-Item -LiteralPath $reply; return $text }
        if (-not (Get-GameProcess)) { throw 'GTA IV exited while a command was pending' }
        Start-Sleep -Milliseconds 250
    }
    throw "no reply to $name within $TimeoutSeconds s"
}

# Launches until the engine boots, retrying the known early startup crash (MTLX.DLL, before any mod loads).
# Returns the number of attempts used; throws after $Attempts failures.
function Start-GameReady([int] $Attempts = 5, [int] $BootTimeoutSeconds = 240) {
    for ($attempt = 1; $attempt -le $Attempts; $attempt++) {
        Stop-Game
        $script:LaunchedUtc = [DateTime]::UtcNow
        Start-Process 'steam://rungameid/12210'
        $deadline = (Get-Date).AddSeconds($BootTimeoutSeconds)
        $seen = $false
        $lostAt = $null
        while ((Get-Date) -lt $deadline) {
            Start-Sleep -Seconds 3
            if (Get-SessionLog | Where-Object { $_ -match 'engine_booted' } | Select-Object -First 1) { return $attempt }
            $process = Get-GameProcess
            if ($process) { $seen = $true; $lostAt = $null; continue }
            # PlayGTAIV/RGL start GTAIV.exe; a GTAIV that was seen and stays gone for 20 s crashed.
            if ($seen) { if (-not $lostAt) { $lostAt = Get-Date } elseif (((Get-Date) - $lostAt).TotalSeconds -gt 20) { break } }
        }
        Write-Host "attempt $attempt failed (seen=$seen); relaunching"
    }
    throw "GTA IV did not reach the engine after $Attempts attempts"
}

# Title of the game's top-level window: a modal error box (e.g. FusionFix "Error building shader!") becomes the
# main window and pauses the game.
function Get-GameDialog {
    $process = Get-GameProcess
    if (-not $process) { return $null }
    $title = $process.MainWindowTitle
    if ($title -and $title -notmatch '^(GTAIV|Grand Theft Auto IV)') { return $title }
    return $null
}

# Launches (retrying startup crashes) until mod scripts log, then watches $WatchSeconds and classifies the session:
# RUNNING (the log keeps advancing), DIALOG:<title> (a modal box blocks the game), FROZEN, or CRASHED.
function Test-Boot([int] $Attempts = 6, [int] $WatchSeconds = 60, [string] $ScriptsPattern = 'engine_booted|arsenal_ready|gunplay_started') {
    for ($attempt = 1; $attempt -le $Attempts; $attempt++) {
        Stop-Game
        $script:LaunchedUtc = [DateTime]::UtcNow
        Start-Process 'steam://rungameid/12210'
        $deadline = (Get-Date).AddSeconds(240)
        $seen = $false; $lostAt = $null; $booted = $false
        while ((Get-Date) -lt $deadline) {
            Start-Sleep -Seconds 3
            $dialog = Get-GameDialog
            if ($dialog) { return "DIALOG:$dialog (attempt $attempt, before scripts)" }
            if (Get-SessionLog | Where-Object { $_ -match $ScriptsPattern } | Select-Object -First 1) { $booted = $true; break }
            if (Get-GameProcess) { $seen = $true; $lostAt = $null; continue }
            if ($seen) { if (-not $lostAt) { $lostAt = Get-Date } elseif (((Get-Date) - $lostAt).TotalSeconds -gt 20) { break } }
        }
        if (-not $booted) { Write-Host "attempt ${attempt}: no scripts (startup crash); relaunching"; continue }
        $start = (Get-SessionLog).Count
        $watchEnd = (Get-Date).AddSeconds($WatchSeconds)
        while ((Get-Date) -lt $watchEnd) {
            Start-Sleep -Seconds 3
            if (-not (Get-GameProcess)) { return "CRASHED after scripts (attempt $attempt)" }
            $dialog = Get-GameDialog
            if ($dialog) { return "DIALOG:$dialog (attempt $attempt)" }
        }
        $grown = (Get-SessionLog).Count - $start
        if ($grown -ge 2) { return "RUNNING (attempt $attempt, $grown new log lines in $WatchSeconds s)" }
        return "FROZEN (attempt $attempt, $grown new log lines in $WatchSeconds s)"
    }
    return "NO_BOOT after $Attempts attempts"
}

# Left click at screen pixel coordinates (full resolution).
function Send-Click([int] $X, [int] $Y) { [AutopilotNative]::Click($X, $Y) }

Export-ModuleMember -Function Get-GameDialog, Test-Boot, Start-GameReady, Send-Click, Set-AutopilotGame, Get-GameProcess, Get-SessionLog, Start-Game, Stop-Game, Focus-Game, Send-GameKey,
    Save-Screenshot, Wait-LogLine, Test-GameFrozen, Invoke-EngineCommand

# Liberty autopilot (host side): starts GTA IV through Steam, watches for crashes and freezes, presses keys, takes
# screenshots, and talks to the engine's command channel (scripts\LibertyFramework\autopilot\inbox|outbox).
# Import-Module tools\autopilot\Autopilot.psm1; Set-AutopilotGame 'C:\...\GTAIV'
$ErrorActionPreference = 'Stop'
$script:Game = $null
$script:LaunchedUtc = [DateTime]::MinValue
$script:LogCache = @{}
Import-Module (Join-Path $PSScriptRoot 'AutopilotLogic.psm1') 3>$null
Import-Module (Join-Path (Split-Path -Parent $PSScriptRoot) 'local\GameLock.psm1') 3>$null
Import-Module (Join-Path (Split-Path -Parent $PSScriptRoot) 'local\AudioOutput.psm1') 3>$null

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

# A game that is already running (the previous scenario launched it; each scenario is a new PowerShell) started this
# session when its process started: earlier sessions in the same log are not this session's lines.
function Set-AutopilotGame([string] $GameDirectory) {
    $script:Game = (Resolve-Path -LiteralPath $GameDirectory).Path
    $process = Get-GameProcess
    if ($process -and $script:LaunchedUtc -eq [DateTime]::MinValue) {
        try { $script:LaunchedUtc = $process.StartTime.ToUniversalTime() }
        catch { Write-Host "autopilot: could not read the game's start time ($($_.Exception.Message)); reading the whole log" }
    }
}

function Get-GameProcess { Get-Process GTAIV -ErrorAction SilentlyContinue | Select-Object -First 1 }

function Get-LogPath { Join-Path $script:Game 'scripts\LibertyFramework\logs\LibertyFramework.log' }

# Log lines written since the current launch (UTC timestamps at the start of each line); read incrementally.
function Get-SessionLog {
    return Update-SessionLogCache $script:LogCache (Get-LogPath) $script:LaunchedUtc.ToString('yyyy-MM-ddTHH:mm:ss')
}

function Start-Game {
    if (Get-GameProcess) { throw 'GTA IV is already running' }
    Assert-AudioOutput
    Stop-Game
    $script:LaunchedUtc = [DateTime]::UtcNow
    Start-Process 'steam://rungameid/12210'
    $deadline = (Get-Date).AddSeconds(120)
    while ((Get-Date) -lt $deadline) {
        $process = Get-GameProcess
        if ($process) { Write-AutopilotLaunch $script:Game $process; return $process }
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
# Steam's install folders: LIBERTY_STEAM_DIR (override), the registry (per user, then machine), the default location.
function Get-SteamRoots {
    $roots = @($env:LIBERTY_STEAM_DIR)
    foreach ($key in @(@('HKCU:\Software\Valve\Steam', 'SteamPath'), @('HKLM:\SOFTWARE\WOW6432Node\Valve\Steam', 'InstallPath'), @('HKLM:\SOFTWARE\Valve\Steam', 'InstallPath'))) {
        try { $roots += (Get-ItemProperty -LiteralPath $key[0] -Name $key[1] -ErrorAction Stop).($key[1]) }
        catch { Write-Verbose "no Steam path at $($key[0])" }
    }
    $roots += 'C:\Program Files (x86)\Steam'
    return @($roots | Where-Object { $_ })
}

function Save-Screenshot([string] $Path) {
    $roots = Get-SteamRoots
    $folders = @(Get-SteamScreenshotFolders $roots)
    if ($folders.Count -eq 0) { throw "no Steam userdata folder found (looked in: $($roots -join '; ')); set LIBERTY_STEAM_DIR to the Steam folder" }
    $before = Get-Date
    # Right after launch the Steam overlay may not take the first F12 yet: try twice before giving up.
    for ($attempt = 0; $attempt -lt 2 -and (Get-GameProcess) -and (Focus-Game); $attempt++) {
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
    Save-DesktopScreenshot $Path | Out-Null
    # GDI capture of a Vulkan (DXVK) window is black: fail the step so a useless screenshot never passes silently.
    throw "no Steam screenshot for $(Split-Path -Leaf $Path) (desktop fallback saved; it is black under Vulkan)"
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
# Judged by the log's growth, not its timestamp: the engine keeps the log open, and Windows may update an open file's
# last-write time lazily. A rotation (the file got shorter) counts as growth.
$script:LogGrowth = @{ Length = [long]-1; Changed = [DateTime]::MinValue }
function Test-GameFrozen([int] $Seconds = 75) {
    if (-not (Get-GameProcess)) { return $false }
    $path = Get-LogPath
    # Length from an open handle (directory metadata of a file another process keeps open can lag).
    $length = [long]0
    if (Test-Path -LiteralPath $path) {
        try { $stream = [IO.File]::Open($path, 'Open', 'Read', 'ReadWrite, Delete'); try { $length = $stream.Length } finally { $stream.Dispose() } }
        catch { return $false }
    }
    $now = Get-Date
    if ($length -ne $script:LogGrowth.Length) { $script:LogGrowth.Length = $length; $script:LogGrowth.Changed = $now; return $false }
    return ($now - $script:LogGrowth.Changed).TotalSeconds -gt $Seconds
}

# Runs engine commands through the file channel and returns the reply lines.
function Invoke-EngineCommand([string[]] $Lines, [int] $TimeoutSeconds = 20) {
    # Teleports block on LOAD_SCENE (measured 0.3-5 s, 30 s once cold); give them room instead of a false no-reply.
    if (($Lines -join "`n") -match '(?im)^\s*(goto|teleport|tp)\b') { $TimeoutSeconds = [Math]::Max($TimeoutSeconds, 90) }
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

# $true when Windows has an active playback endpoint, recovering the installed Sonar Gaming virtual output if needed.
# Both verify-local and standalone scenarios call this, so disconnected headphones no longer block unattended tests.
function Test-AudioOutput {
    try {
        return Initialize-TestAudioOutput
    }
    catch {
        # No PnP module (or no permission): do not block a launch on a check that cannot run.
        Write-Host "autopilot: could not list audio devices ($($_.Exception.Message)); launching anyway"
        return $true
    }
}

# Launches until the engine boots, retrying the known early startup crash (MTLX.DLL, before any mod loads).
# Returns the number of attempts used; throws after $Attempts failures. Fails at once, without retrying, when a retry
# cannot help: no audio output device, or the game shows its own "Fatal Error" box.
function Assert-AudioOutput {
    if (-not (Test-AudioOutput)) {
        throw 'GAME-UNAVAILABLE: no audio output device is active and the installed virtual output could not be recovered; connect speakers or a headset, or enable a virtual playback device'
    }
}

function Start-GameReady([int] $Attempts = 5, [int] $BootTimeoutSeconds = 240, [int] $NotSeenSeconds = 90) {
    Assert-AudioOutput
    for ($attempt = 1; $attempt -le $Attempts; $attempt++) {
        Stop-Game
        $script:LaunchedUtc = [DateTime]::UtcNow
        Start-Process 'steam://rungameid/12210'
        $started = Get-Date
        $deadline = $started.AddSeconds($BootTimeoutSeconds)
        $seen = $false
        $lostAt = $null
        while ((Get-Date) -lt $deadline) {
            Start-Sleep -Seconds 3
            if (Get-SessionLog | Where-Object { $_ -match 'engine_booted' } | Select-Object -First 1) { return $attempt }
            $process = Get-GameProcess
            if ($process) {
                if (-not $seen) { Write-AutopilotLaunch $script:Game $process }
                $seen = $true; $lostAt = $null
                $dialog = Get-GameDialog
                if ($dialog -and $dialog -match '(?i)fatal') {
                    Stop-Game
                    throw "GAME-UNAVAILABLE: GTA IV showed '$dialog' before the engine started (a sound card error means no audio output device)"
                }
                continue
            }
            # Steam normally starts GTAIV.exe within seconds; nothing after $NotSeenSeconds means this launch went nowhere.
            if (-not $seen -and ((Get-Date) - $started).TotalSeconds -gt $NotSeenSeconds) { break }
            # PlayGTAIV/RGL start GTAIV.exe; a GTAIV that was seen and stays gone for 20 s crashed.
            if ($seen) { if (-not $lostAt) { $lostAt = Get-Date } elseif (((Get-Date) - $lostAt).TotalSeconds -gt 20) { break } }
        }
        Write-Host "attempt $attempt failed (seen=$seen); relaunching"
    }
    throw "GAME-UNAVAILABLE: GTA IV did not reach the engine after $Attempts attempts"
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
    Assert-AudioOutput
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

# The game process's memory right now, for a scenario's `gpumem <label>` line (T-040): dedicated and shared GPU memory of
# the GTAIV process (Windows "GPU Process Memory" counters, read through CIM so they do not depend on the Windows language),
# private bytes and working set. GPU figures are $null when the counters do not list the process.
function Get-GameMemory {
    $process = Get-GameProcess
    if (-not $process) { throw 'game not running' }
    $process.Refresh()
    $gpu = $null
    try { $gpu = Select-GpuProcessMemory @(Get-CimInstance Win32_PerfFormattedData_GPUPerformanceCounters_GPUProcessMemory -ErrorAction Stop) $process.Id }
    catch { Write-Host "autopilot: GPU counters unavailable ($($_.Exception.Message))" }
    # How busy the rest of the PC is right now: the game is installed on a hard disk and a build, a scan or another session
    # touching that disk during a sample shows up as frame hitches that are not the mod's. Recorded so a report can flag it.
    $cpu = $null; $diskBusy = $null; $diskQueue = $null
    try {
        $cpu = [double](Get-CimInstance Win32_PerfFormattedData_PerfOS_Processor -Filter "Name='_Total'" -ErrorAction Stop).PercentProcessorTime
        $drive = (Split-Path -Qualifier $script:Game)
        $disk = @(Get-CimInstance Win32_PerfFormattedData_PerfDisk_PhysicalDisk -ErrorAction Stop | Where-Object { $_.Name -match ('^\d+ ' + [regex]::Escape($drive) + '$') }) | Select-Object -First 1
        if ($disk) { $diskBusy = [double]$disk.PercentDiskTime; $diskQueue = [double]$disk.AvgDiskQueueLength }
    }
    catch { Write-Host "autopilot: system load counters unavailable ($($_.Exception.Message))" }
    return [ordered]@{
        processId = $process.Id
        gpuDedicatedMB = $(if ($gpu -and $gpu.Found) { [math]::Round($gpu.DedicatedBytes / 1MB, 1) } else { $null })
        gpuSharedMB = $(if ($gpu -and $gpu.Found) { [math]::Round($gpu.SharedBytes / 1MB, 1) } else { $null })
        privateMB = [math]::Round($process.PrivateMemorySize64 / 1MB, 1)
        workingSetMB = [math]::Round($process.WorkingSet64 / 1MB, 1)
        systemCpuPercent = $cpu
        gameDiskBusyPercent = $diskBusy
        gameDiskQueue = $diskQueue
    }
}

# Left click at screen pixel coordinates (full resolution).
function Send-Click([int] $X, [int] $Y) { [AutopilotNative]::Click($X, $Y) }

Export-ModuleMember -Function Get-GameDialog, Test-Boot, Start-GameReady, Send-Click, Set-AutopilotGame, Get-GameProcess, Get-SessionLog, Start-Game, Stop-Game, Focus-Game, Send-GameKey,
    Save-Screenshot, Wait-LogLine, Test-GameFrozen, Invoke-EngineCommand, Get-GameMemory, Test-AudioOutput

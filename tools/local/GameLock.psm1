# One game, one install: everything that installs into the game folder or drives the game (verify-local.ps1,
# install-phase2.ps1, rollback-phase2.ps1, autopilot\Run-Scenario.ps1, autopilot\Run-Suite.ps1) holds this machine-wide
# mutex, so parallel agent sessions and worktrees wait their turn instead of driving the same game at once.
# A holder passes the lock to its child processes through LIBERTY_GAME_LOCK_HOLDER (verify-local runs the installer and
# the scenarios as child processes); a nested call in the same process re-enters the mutex. Never set the variable by hand.
# The mutex is released when the holder ends even after a crash (the next waiter sees an abandoned mutex, which still
# grants ownership).

$script:LockName = 'Global\LibertyVerifyLocalGame'
$script:HolderVariable = 'LIBERTY_GAME_LOCK_HOLDER'

function Get-GameLockInfoPath { Join-Path ([IO.Path]::GetTempPath()) 'LibertyGameLock.txt' }

function Enter-GameLock {
    param(
        # Shown to waiters: who holds the game (script name and worktree).
        [Parameter(Mandatory = $true)][string] $Owner,
        [int] $TimeoutMinutes = 180
    )
    $mutex = New-Object System.Threading.Mutex($false, $script:LockName)
    try { $owned = $mutex.WaitOne(0) } catch [System.Threading.AbandonedMutexException] { $owned = $true }
    if (-not $owned) {
        $holder = [Environment]::GetEnvironmentVariable($script:HolderVariable)
        if ($holder -and $holder -ne "$PID" -and (Get-Process -Id ([int]$holder) -ErrorAction SilentlyContinue)) {
            # Inherited from the parent process that owns the lock (verify-local running its child steps).
            $mutex.Dispose()
            return [pscustomobject]@{ Mutex = $null; SetHolder = $false }
        }
        $info = Get-Content -LiteralPath (Get-GameLockInfoPath) -Raw -ErrorAction SilentlyContinue
        Write-Host "${Owner}: the game is in use by another run; waiting for it (up to $TimeoutMinutes min). Holder: $info"
        try { $owned = $mutex.WaitOne([TimeSpan]::FromMinutes($TimeoutMinutes)) } catch [System.Threading.AbandonedMutexException] { $owned = $true }
        if (-not $owned) { $mutex.Dispose(); throw "${Owner}: gave up waiting for the game after $TimeoutMinutes min" }
    }
    # A nested call in the same process leaves the holder note to the outer call.
    $setHolder = [Environment]::GetEnvironmentVariable($script:HolderVariable) -ne "$PID"
    if ($setHolder) {
        [Environment]::SetEnvironmentVariable($script:HolderVariable, "$PID")
        try { [IO.File]::WriteAllText((Get-GameLockInfoPath), "$Owner (pid $PID, since $([DateTime]::UtcNow.ToString('u')))") }
        catch { Write-Host "game lock: could not write the holder note: $($_.Exception.Message)" }
    }
    return [pscustomobject]@{ Mutex = $mutex; SetHolder = $setHolder }
}

function Exit-GameLock {
    param($Lock)
    if (-not $Lock -or -not $Lock.Mutex) { return }
    if ($Lock.SetHolder) {
        [Environment]::SetEnvironmentVariable($script:HolderVariable, $null)
        Remove-Item -LiteralPath (Get-GameLockInfoPath) -Force -ErrorAction SilentlyContinue
    }
    $Lock.Mutex.ReleaseMutex()
    $Lock.Mutex.Dispose()
}

# The installer records which worktree and commit it installed (scripts\LibertyFramework\installed-build.json); a scenario
# run from another worktree would test someone else's build, so Run-Scenario refuses it.
function Get-InstalledBuildPath([string] $GameDirectory) { Join-Path $GameDirectory 'scripts\LibertyFramework\installed-build.json' }

function Write-InstalledBuild {
    param([Parameter(Mandatory = $true)][string] $GameDirectory, [string] $RepoRoot = '', [string] $Note = '')
    $build = [ordered]@{ repo = $RepoRoot; branch = ''; commit = ''; dirty = $false; note = $Note; utc = [DateTime]::UtcNow.ToString('o') }
    if ($RepoRoot) {
        try {
            $build.branch = (& git -C $RepoRoot rev-parse --abbrev-ref HEAD 2>$null | Out-String).Trim()
            $build.commit = (& git -C $RepoRoot rev-parse --short HEAD 2>$null | Out-String).Trim()
            $build.dirty = [bool]((& git -C $RepoRoot status --porcelain --untracked-files=no 2>$null | Out-String).Trim())
        }
        catch { Write-Host "installed-build: could not read git state: $($_.Exception.Message)" }
    }
    $path = Get-InstalledBuildPath $GameDirectory
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $path) | Out-Null
    [IO.File]::WriteAllText($path, ($build | ConvertTo-Json), (New-Object Text.UTF8Encoding($false)))
}

function Read-InstalledBuild([string] $GameDirectory) {
    $path = Get-InstalledBuildPath $GameDirectory
    if (-not (Test-Path -LiteralPath $path)) { return $null }
    return Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
}

Export-ModuleMember -Function Enter-GameLock, Exit-GameLock, Write-InstalledBuild, Read-InstalledBuild

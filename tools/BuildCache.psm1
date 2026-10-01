# Skips build steps whose inputs did not change. Two kinds:
#   - Stamps (per worktree): a build script compares a hash of every input file with the hash it wrote next to its outputs
#     last time; equal and every output present = up to date, nothing is compiled.
#   - The shared cache (one per machine, every worktree): steps whose outputs depend only on repository files and the
#     game's own archives (vehicle extras, weapon icons, sling models, content models) store their output folder under
#     %LOCALAPPDATA%\LibertyFramework\build-cache\<step>\<input hash>; any worktree with the same inputs reuses it.
# Keys are content hashes (never timestamps of repository files), so a checkout, a rebase or another worktree can never
# reuse a stale output. LIBERTY_NO_BUILD_CACHE=1 turns both off (everything is rebuilt).
# Compatible with Windows PowerShell 5.1 and PowerShell 7 (the cloud build).

function Test-BuildCacheEnabled { return $env:LIBERTY_NO_BUILD_CACHE -ne '1' }

function Get-BytesHash([byte[]] $Bytes) {
    $sha = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($sha.ComputeHash($Bytes))).Replace('-', '') }
    finally { $sha.Dispose() }
}

# One hash over the files (each by its path relative to $Root and its content) and any extra strings (compiler, flags,
# references, game fingerprint). File order does not matter.
function Get-InputKey {
    param([string] $Root, [string[]] $Files = @(), [string[]] $Extra = @())
    $rootFull = [IO.Path]::GetFullPath($Root).TrimEnd('\', '/')
    $lines = New-Object System.Collections.Generic.List[string]
    foreach ($file in @($Files | Where-Object { $_ } | Sort-Object -Unique)) {
        $full = [IO.Path]::GetFullPath($file)
        $relative = if ($full.StartsWith($rootFull, [StringComparison]::OrdinalIgnoreCase)) { $full.Substring($rootFull.Length).TrimStart('\', '/') } else { $full }
        $lines.Add('file|' + $relative.Replace('\', '/').ToLowerInvariant() + '|' + (Get-BytesHash ([IO.File]::ReadAllBytes($full))))
    }
    foreach ($value in @($Extra)) { $lines.Add('extra|' + $value) }
    $sorted = @($lines | Sort-Object)
    return Get-BytesHash ([Text.Encoding]::UTF8.GetBytes(($sorted -join "`n")))
}

# The identity of a file outside the repository that is too big to hash every time (compiler, game archive): path, size
# and last write time.
function Get-FileIdentity([string] $Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return "$Path|missing" }
    $item = Get-Item -LiteralPath $Path
    return "$($item.Name)|$($item.Length)|$($item.LastWriteTimeUtc.Ticks)"
}

# The game files the packaging tools read: GTAIV.exe (the archive key) and every IMG/RPF archive, by size and time.
# The archives this project's install writes (LibertyModels.img, LibertyContent.img) and the install backups are left
# out: the tools never read them, and they change with every install.
function Get-GameFingerprint([string] $Game) {
    $gameFull = [IO.Path]::GetFullPath($Game).TrimEnd('\', '/')
    $own = @('libertymodels.img', 'libertycontent.img')
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add('exe|' + (Get-FileIdentity (Join-Path $gameFull 'GTAIV.exe')))
    foreach ($file in Get-ChildItem -LiteralPath $gameFull -Recurse -File -ErrorAction SilentlyContinue) {
        $extension = $file.Extension.ToLowerInvariant()
        if ($extension -ne '.img' -and $extension -ne '.rpf') { continue }
        if ($own -contains $file.Name.ToLowerInvariant()) { continue }
        $relative = $file.FullName.Substring($gameFull.Length).TrimStart('\', '/').Replace('\', '/').ToLowerInvariant()
        if ($relative -match '(^|/)backups/') { continue }
        $lines.Add("$relative|$($file.Length)|$($file.LastWriteTimeUtc.Ticks)")
    }
    $sorted = @($lines | Sort-Object)
    return Get-BytesHash ([Text.Encoding]::UTF8.GetBytes(($sorted -join "`n")))
}

# ---- Stamps (per worktree)

function Test-BuildStamp([string] $Stamp, [string] $Key, [string[]] $Outputs) {
    if (-not (Test-BuildCacheEnabled)) { return $false }
    if (-not (Test-Path -LiteralPath $Stamp -PathType Leaf)) { return $false }
    if (([IO.File]::ReadAllText($Stamp)).Trim() -ne $Key) { return $false }
    foreach ($output in $Outputs) { if (-not (Test-Path -LiteralPath $output -PathType Leaf)) { return $false } }
    return $true
}

function Set-BuildStamp([string] $Stamp, [string] $Key) {
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Stamp) | Out-Null
    [IO.File]::WriteAllText($Stamp, $Key)
}

# A stamp is removed before a build starts, so a failed or interrupted build is never taken as up to date.
function Clear-BuildStamp([string] $Stamp) { Remove-Item -LiteralPath $Stamp -Force -ErrorAction SilentlyContinue }

# ---- Shared cache (per machine)

function Get-BuildCacheRoot {
    if ($env:LIBERTY_BUILD_CACHE) { return $env:LIBERTY_BUILD_CACHE }
    if ($env:LOCALAPPDATA) { return Join-Path $env:LOCALAPPDATA 'LibertyFramework\build-cache' }
    return Join-Path ([IO.Path]::GetTempPath()) 'liberty-build-cache'
}

# Fills $OutputDirectory with the step's output: from the cache when an entry for $Key exists, otherwise by running
# $Build (which must fill $OutputDirectory and throw on failure) and then storing the result. Returns $true on a reuse.
# An entry is stored complete or not at all (written to a temporary folder, then renamed), so parallel worktrees and an
# interrupted build never leave a half entry behind.
function Invoke-CachedStep {
    param([Parameter(Mandatory = $true)][string] $Name, [Parameter(Mandatory = $true)][string] $Key,
        [Parameter(Mandatory = $true)][string] $OutputDirectory, [Parameter(Mandatory = $true)][scriptblock] $Build, [int] $Keep = 6)
    $stepRoot = Join-Path (Get-BuildCacheRoot) $Name
    $entry = Join-Path $stepRoot $Key
    # The step owns its output folder: start empty, so a reuse never mixes with an older build's files.
    if (Test-Path -LiteralPath $OutputDirectory) { Remove-Item -LiteralPath $OutputDirectory -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
    if ((Test-BuildCacheEnabled) -and (Test-Path -LiteralPath (Join-Path $entry '.complete') -PathType Leaf)) {
        Get-ChildItem -LiteralPath $entry -Force | Where-Object { $_.Name -ne '.complete' } | Copy-Item -Destination $OutputDirectory -Recurse -Force
        (Get-Item -LiteralPath $entry).LastWriteTimeUtc = [DateTime]::UtcNow
        Write-Host "cache: $Name reused ($($Key.Substring(0, [Math]::Min(12, $Key.Length))))"
        return $true
    }
    # The tools' own output goes to the log (host), not into this function's return value.
    & $Build | Out-Host
    if (-not (Test-BuildCacheEnabled)) { return $false }
    try {
        New-Item -ItemType Directory -Force -Path $stepRoot | Out-Null
        if (Test-Path -LiteralPath $entry) { Remove-Item -LiteralPath $entry -Recurse -Force }
        $temporary = "$entry.tmp-" + [Guid]::NewGuid().ToString('N').Substring(0, 8)
        New-Item -ItemType Directory -Force -Path $temporary | Out-Null
        Get-ChildItem -LiteralPath $OutputDirectory -Force | Copy-Item -Destination $temporary -Recurse -Force
        [IO.File]::WriteAllText((Join-Path $temporary '.complete'), [DateTime]::UtcNow.ToString('o'))
        try { Rename-Item -LiteralPath $temporary -NewName (Split-Path -Leaf $entry) }
        catch { Remove-Item -LiteralPath $temporary -Recurse -Force -ErrorAction SilentlyContinue; Write-Host "cache: $Name stored by another build meanwhile" }
        # Keep the newest entries of this step; old game versions or old sources are dropped.
        Get-ChildItem -LiteralPath $stepRoot -Directory | Where-Object { $_.Name -notlike '*.tmp-*' } | Sort-Object LastWriteTimeUtc -Descending |
            Select-Object -Skip $Keep | ForEach-Object { Remove-Item -LiteralPath $_.FullName -Recurse -Force -ErrorAction SilentlyContinue }
    }
    catch { Write-Host "cache: could not store $Name ($($_.Exception.Message)); the build itself succeeded" }
    return $false
}

Export-ModuleMember -Function Test-BuildCacheEnabled, Get-InputKey, Get-FileIdentity, Get-GameFingerprint, Test-BuildStamp, Set-BuildStamp, Clear-BuildStamp, Get-BuildCacheRoot, Invoke-CachedStep

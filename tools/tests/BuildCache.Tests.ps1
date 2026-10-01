# Tests of tools/BuildCache.psm1 (build stamps and the shared build cache) and tools/local/GameLock.psm1 (the game lock),
# without a game.
Import-Module (Join-Path $script:RepoRoot (Join-Path 'tools' 'BuildCache.psm1')) -Force
$cacheRoot = Join-Path $script:Scratch 'build-cache'
$work = Join-Path $script:Scratch 'build-cache-work'
New-Item -ItemType Directory -Force -Path $work | Out-Null
$env:LIBERTY_BUILD_CACHE = $cacheRoot
$env:LIBERTY_NO_BUILD_CACHE = $null

# ---- Input keys
$a = Join-Path $work 'a.cs'; $b = Join-Path $work 'b.cs'
[IO.File]::WriteAllText($a, 'class A {}'); [IO.File]::WriteAllText($b, 'class B {}')
$key = Get-InputKey $work @($a, $b) @('csc|1')
Test-That 'key: file order does not matter' ($key -eq (Get-InputKey $work @($b, $a) @('csc|1')))
Test-That 'key: a changed file changes the key' ($key -ne $(([IO.File]::WriteAllText($b, 'class B { int x; }')); Get-InputKey $work @($a, $b) @('csc|1')))
[IO.File]::WriteAllText($b, 'class B {}')
Test-That 'key: a changed extra (compiler, flags, game) changes the key' ($key -ne (Get-InputKey $work @($a, $b) @('csc|2')))
$copy = Join-Path $script:Scratch 'build-cache-copy'
New-Item -ItemType Directory -Force -Path $copy | Out-Null
Copy-Item $a, $b -Destination $copy
Test-That 'key: the same files in another worktree give the same key' ($key -eq (Get-InputKey $copy @((Join-Path $copy 'a.cs'), (Join-Path $copy 'b.cs')) @('csc|1')))
Test-That 'key: a renamed file changes the key' ($key -ne $(Rename-Item (Join-Path $copy 'b.cs') 'c.cs'; Get-InputKey $copy @((Join-Path $copy 'a.cs'), (Join-Path $copy 'c.cs')) @('csc|1')))

# ---- Stamps
$stamp = Join-Path $work 'bin/.build-inputs'
$output = Join-Path $work 'bin/out.dll'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $output) | Out-Null
Test-That 'stamp: no stamp is not up to date' (-not (Test-BuildStamp $stamp $key @($output)))
[IO.File]::WriteAllText($output, 'dll'); Set-BuildStamp $stamp $key
Test-That 'stamp: same key and outputs present is up to date' (Test-BuildStamp $stamp $key @($output))
Test-That 'stamp: another key is not up to date' (-not (Test-BuildStamp $stamp 'other' @($output)))
Remove-Item $output
Test-That 'stamp: a missing output is not up to date' (-not (Test-BuildStamp $stamp $key @($output)))
[IO.File]::WriteAllText($output, 'dll')
$env:LIBERTY_NO_BUILD_CACHE = '1'
Test-That 'stamp: LIBERTY_NO_BUILD_CACHE=1 forces a build' (-not (Test-BuildStamp $stamp $key @($output)))
$env:LIBERTY_NO_BUILD_CACHE = $null
Clear-BuildStamp $stamp
Test-That 'stamp: cleared before a build, so a failed build is never up to date' (-not (Test-BuildStamp $stamp $key @($output)))

# ---- Shared cache
$out = Join-Path $work 'step-out'
$script:builds = 0
$build = { $script:builds++; New-Item -ItemType Directory -Force -Path (Join-Path $out 'sub') | Out-Null; [IO.File]::WriteAllText((Join-Path $out 'sub/file.txt'), "build $script:builds") }
$hit1 = Invoke-CachedStep 'unit' 'k1' $out $build
$hit2 = Invoke-CachedStep 'unit' 'k1' $out $build 6>$null
Test-That 'cache: first run builds, second reuses' (-not $hit1 -and $hit2 -and $script:builds -eq 1) "hit1=$hit1 hit2=$hit2 builds=$script:builds"
Test-That 'cache: a reuse restores the output (nested folders kept)' ((Get-Content (Join-Path $out 'sub/file.txt') -Raw).Trim() -eq 'build 1')
[IO.File]::WriteAllText((Join-Path $out 'stray.txt'), 'old')
Invoke-CachedStep 'unit' 'k1' $out $build 6>$null | Out-Null
Test-That 'cache: the output folder starts empty (no stray files from older builds)' (-not (Test-Path (Join-Path $out 'stray.txt')))
Invoke-CachedStep 'unit' 'k2' $out $build | Out-Null
Test-That 'cache: another key builds again' ($script:builds -eq 2)
$threw = $false
try { Invoke-CachedStep 'unit' 'k3' $out { throw 'compile error' } | Out-Null } catch { $threw = $true }
Test-That 'cache: a failing build throws and stores nothing' ($threw -and -not (Test-Path (Join-Path $cacheRoot 'unit/k3')))
New-Item -ItemType Directory -Force -Path (Join-Path $cacheRoot 'unit/k4') | Out-Null
Invoke-CachedStep 'unit' 'k4' $out $build | Out-Null
Test-That 'cache: an entry without its completion mark is rebuilt, never reused' ($script:builds -eq 3 -and (Test-Path (Join-Path $cacheRoot 'unit/k4/.complete')))
$env:LIBERTY_NO_BUILD_CACHE = '1'
$hit = Invoke-CachedStep 'unit' 'k1' $out $build
Test-That 'cache: LIBERTY_NO_BUILD_CACHE=1 rebuilds' (-not $hit -and $script:builds -eq 4)
$env:LIBERTY_NO_BUILD_CACHE = $null
foreach ($i in 5..12) { Invoke-CachedStep 'unit' "p$i" $out $build | Out-Null }
Test-That 'cache: old entries are pruned' (@(Get-ChildItem (Join-Path $cacheRoot 'unit') -Directory).Count -le 6)

# ---- Game fingerprint: the project's own archives and the backups do not count
$game = Join-Path $script:Scratch 'fp-game'
foreach ($path in 'GTAIV.exe', 'pc/models/cdimages/weapons.img', 'update/LibertyFramework/LibertyModels.img', 'scripts/LibertyFramework/backups/x/a.img') {
    $full = Join-Path $game $path; New-Item -ItemType Directory -Force -Path (Split-Path -Parent $full) | Out-Null; [IO.File]::WriteAllText($full, 'x')
}
$fp = Get-GameFingerprint $game
[IO.File]::WriteAllText((Join-Path $game 'update/LibertyFramework/LibertyModels.img'), 'changed by an install')
[IO.File]::WriteAllText((Join-Path $game 'scripts/LibertyFramework/backups/x/a.img'), 'another backup')
Test-That 'fingerprint: an install (own archives, backups) does not change it' ($fp -eq (Get-GameFingerprint $game))
[IO.File]::WriteAllText((Join-Path $game 'pc/models/cdimages/weapons.img'), 'a modded weapons archive')
Test-That 'fingerprint: a changed game archive changes it' ($fp -ne (Get-GameFingerprint $game))
$env:LIBERTY_BUILD_CACHE = $null

# ---- Game lock (isolate both the mutex and holder note from real runs)
$lockModule = Import-Module (Join-Path $script:RepoRoot (Join-Path 'tools' (Join-Path 'local' 'GameLock.psm1'))) -Force -PassThru
$testLockNote = Join-Path $script:Scratch 'game-lock-note.txt'
& $lockModule {
    param($notePath)
    $script:LockName = 'LibertyGameLockSelfTest' + [Guid]::NewGuid().ToString('N').Substring(0, 6)
    $script:SelfTestLockNote = $notePath
    function script:Get-GameLockInfoPath { return $script:SelfTestLockNote }
} $testLockNote
$savedHolder = $env:LIBERTY_GAME_LOCK_HOLDER
$env:LIBERTY_GAME_LOCK_HOLDER = $null
$outer = Enter-GameLock 'test outer'
Test-That 'lock: self-test holder note stays in its isolated scratch directory' ((& $lockModule { Get-GameLockInfoPath }) -eq $testLockNote -and (Test-Path -LiteralPath $testLockNote))
Test-That 'lock: taken and passed on to child processes' ($outer.Mutex -and $env:LIBERTY_GAME_LOCK_HOLDER -eq "$PID")
$inner = Enter-GameLock 'test nested'
Test-That 'lock: a nested call in the same process re-enters and leaves the holder to the outer call' ($inner.Mutex -and -not $inner.SetHolder)
Exit-GameLock $inner
Test-That 'lock: still held after the nested call ends' ($env:LIBERTY_GAME_LOCK_HOLDER -eq "$PID")
Exit-GameLock $outer
Test-That 'lock: released' (-not $env:LIBERTY_GAME_LOCK_HOLDER)
$env:LIBERTY_GAME_LOCK_HOLDER = $savedHolder

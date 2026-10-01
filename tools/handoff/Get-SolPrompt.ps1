param(
    # Which agent to hand over: B, C, D, R (research) or Orchestrator.
    [Parameter(Mandatory = $true)][ValidateSet('B', 'C', 'D', 'R', 'Orchestrator')][string] $Lane,
    # Print only; do not copy to the clipboard.
    [switch] $NoClipboard
)

# Builds the ready-to-paste prompt for a Sol (GPT) agent taking over a lane: the lane's prompt
# (docs/handoffs/sol/PROMPT-*.md) followed by a LIVE STATE block read from git, the verifier results and the game lock at
# this moment. Copies it to the clipboard and saves it under %TEMP%. Read-only: changes nothing in any worktree.
# Guide: docs/handoffs/sol/README.md.
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$lanes = @{
    B = @{ Path = 'C:\Users\IM576\GTAIV-Reborn-lane-b2'; Prompt = 'PROMPT-LANE-B.md'; Live = 'Lane-B-live.md' }
    C = @{ Path = 'C:\Users\IM576\GTAIV-Reborn-lane-c-t048'; Prompt = 'PROMPT-LANE-C.md'; Live = 'Lane-C-live.md' }
    D = @{ Path = 'C:\Users\IM576\GTAIV-Reborn-lane-d'; Prompt = 'PROMPT-LANE-D.md'; Live = 'Lane-D-live.md' }
    R = @{ Path = 'C:\Users\IM576\GTAIV-Reborn-research'; Prompt = 'PROMPT-LANE-R.md'; Live = 'Lane-R-live.md' }
    Orchestrator = @{ Path = $repo; Prompt = 'PROMPT-ORCHESTRATOR.md'; Live = '' }
}
function Invoke-Git { $ErrorActionPreference = 'Continue'; & git @args 2>$null }

function Get-WorktreeState([string] $Name, [string] $Path, [string] $Live) {
    $out = New-Object System.Collections.Generic.List[string]
    $out.Add("### $Name  ($Path)")
    if (-not (Test-Path -LiteralPath $Path)) { $out.Add('worktree missing'); return $out }
    Invoke-Git -C $Path fetch -q origin | Out-Null
    $out.Add("branch: $(Invoke-Git -C $Path rev-parse --abbrev-ref HEAD)  head: $(Invoke-Git -C $Path log -1 --format='%h %ad %s' --date=format:'%Y-%m-%d %H:%M')")
    $out.Add("commits ahead of origin/main: $(Invoke-Git -C $Path rev-list --count origin/main..HEAD); behind: $(Invoke-Git -C $Path rev-list --count HEAD..origin/main)")
    $upstream = Invoke-Git -C $Path rev-parse --abbrev-ref '@{upstream}'
    if ($upstream) { $out.Add("upstream $upstream; local is behind it by $(Invoke-Git -C $Path rev-list --count "HEAD..$upstream") commits") }
    $out.Add('last commits:')
    Invoke-Git -C $Path log -8 --format='  %h %ad %s' --date=format:'%m-%d %H:%M' | ForEach-Object { $out.Add($_) }
    $dirty = @(Invoke-Git -C $Path status --porcelain)
    $out.Add("uncommitted files: $($dirty.Count)")
    $dirty | Select-Object -First 20 | ForEach-Object { $out.Add("  $_") }
    # A previous agent still at work shows as recent edits.
    $recent = Get-ChildItem -LiteralPath $Path -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '\\(\.git|bin|obj|staging|results-local)\\' } | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($recent) { $out.Add("newest source edit: $($recent.LastWriteTime.ToString('yyyy-MM-dd HH:mm')) $($recent.FullName.Substring($Path.Length))") }
    $results = Join-Path $Path 'results-local'
    if (Test-Path -LiteralPath $results) {
        foreach ($run in Get-ChildItem -LiteralPath $results -Directory | Where-Object Name -match '^\d{8}-' | Sort-Object Name -Descending | Select-Object -First 3) {
            $summary = Join-Path $run.FullName 'summary.json'
            if (-not (Test-Path -LiteralPath $summary)) { $out.Add("run $($run.Name): no summary (interrupted or still running)"); continue }
            $json = Get-Content -LiteralPath $summary -Raw | ConvertFrom-Json
            $state = if ($json.run.finishedUtc) { 'finished' } else { 'RUNNING or interrupted' }
            $out.Add("run $($run.Name) mode=$($json.run.mode) $state; install: $($json.run.install)")
            foreach ($check in $json.checks) { $out.Add("  $($check.id): $($check.status) - $($check.detail)") }
        }
    }
    if ($Live) {
        $livePath = Join-Path $Path "docs\handoffs\$Live"
        $out.Add("live handoff: $(if (Test-Path -LiteralPath $livePath) { "docs/handoffs/$Live, updated $((Get-Item -LiteralPath $livePath).LastWriteTime.ToString('yyyy-MM-dd HH:mm'))" } else { 'none yet (write it first)' })")
    }
    return $out
}

$state = New-Object System.Collections.Generic.List[string]
$state.Add('## LIVE STATE (captured ' + (Get-Date).ToString('yyyy-MM-dd HH:mm') + ' local time; verify it yourself)')
$holder = Get-Content -LiteralPath (Join-Path ([IO.Path]::GetTempPath()) 'LibertyGameLock.txt') -Raw -ErrorAction SilentlyContinue
$state.Add("game lock holder: $(if ($holder) { $holder.Trim() } else { 'none (free)' })")
$game = Get-Process GTAIV -ErrorAction SilentlyContinue | Select-Object -First 1
$state.Add("GTA IV running: $(if ($game) { "yes, pid $($game.Id) since $($game.StartTime)" } else { 'no' })")
$installed = Get-Content -LiteralPath 'C:\Games\Grand Theft Auto IV\GTAIV\scripts\LibertyFramework\installed-build.json' -Raw -ErrorAction SilentlyContinue
if ($installed) { $build = $installed | ConvertFrom-Json; $state.Add("installed build: repo=$($build.repo) commit=$($build.commit) $($build.note)") }
$state.Add('')
if ($Lane -eq 'Orchestrator') {
    foreach ($key in 'B', 'C', 'D', 'R') { (Get-WorktreeState "Lane $key" $lanes[$key].Path $lanes[$key].Live) | ForEach-Object { $state.Add($_) }; $state.Add('') }
    (Get-WorktreeState 'main' $repo '') | ForEach-Object { $state.Add($_) }
}
else { (Get-WorktreeState "Lane $Lane" $lanes[$Lane].Path $lanes[$Lane].Live) | ForEach-Object { $state.Add($_) } }

$prompt = [IO.File]::ReadAllText((Join-Path $repo "docs\handoffs\sol\$($lanes[$Lane].Prompt)"))
$text = $prompt.TrimEnd() + "`r`n`r`n" + ($state -join "`r`n") + "`r`n"
$file = Join-Path ([IO.Path]::GetTempPath()) "sol-prompt-$Lane.md"
[IO.File]::WriteAllText($file, $text, (New-Object Text.UTF8Encoding($false)))
if (-not $NoClipboard) { Set-Clipboard -Value $text; Write-Host "Copied the Lane $Lane prompt to the clipboard." }
Write-Host "Saved: $file"
Write-Host ''
$state | ForEach-Object { Write-Host $_ }

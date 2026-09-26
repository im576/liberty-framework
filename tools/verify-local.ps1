param(
    # Folder containing GTAIV.exe. Remembered in tools/verify-local.settings.json (git-ignored) after the first run.
    [string] $GameDirectory,
    # blender.exe (4.2+) for the Blender add-on tests; optional (remembered). Without it that check is NOT-RUN.
    [string] $Blender,
    # Extracted Liberty Vehicle Services CE release, passed to package-phase2.ps1 (optional, remembered).
    [string] $LvsDirectory,
    # ScriptHookDotNet compile reference. Default: <game>\ScriptHookDotNet.asi (the installed runtime).
    [string] $ScriptHookDotNetReference,
    # The branch to verify. The run refuses to start on another branch unless -AnyBranch.
    [string] $Branch = 'develop',
    [switch] $AnyBranch,
    # First run: build, package/install, the SDK self-test scenario, then restore the install. Proves the script works.
    [switch] $Smoke,
    # Only these check ids (tests/local/checks.json), or only these kinds.
    [string[]] $Only,
    [ValidateSet('pc-offline', 'probe', 'scenario', 'manual')][string[]] $Kind,
    # Skip the manual checks (they are recorded NOT-RUN), or re-ask manual checks that already passed.
    [switch] $NoManual,
    [switch] $IncludePassedManual,
    # What to do with the tested build afterwards; without either, the script asks (Smoke always restores).
    [switch] $KeepInstall,
    [switch] $Restore,
    # Do not push the results to GitHub.
    [switch] $NoPush,
    # Continue an interrupted run: its results folder. Checks that already finished are kept.
    [string] $Resume,
    # Minutes before a hung scenario is killed.
    [int] $ScenarioTimeoutMinutes = 15,
    # Cloud testing only: run the whole orchestration against a simulated game and simulated tools
    # (tools/tests/VerifyLocal.Tests.ps1). Never touches a game, never pushes to GitHub.
    [switch] $Simulate,
    [string] $SimulationFile,
    [string] $QueuePath,
    [string] $ResultsDirectory
)

# One command for the owner's PC: builds, tests, installs, runs every queued check (tests/local/checks.json), asks the
# owner about the manual ones, restores or keeps the install, and pushes the results to the verification-results branch
# for the next cloud session. Workflow: docs/workflow/CLOUD_LOCAL_LOOP.md. Plan: docs/testing/LOCAL_VERIFICATION_PLAN.md.
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path -LiteralPath (Split-Path -Parent $PSScriptRoot)).Path
# git reports progress and failures on stderr; with 'Stop', Windows PowerShell 5.1 would turn that into a terminating
# error even when it is redirected. Output is returned; stderr is dropped; $LASTEXITCODE tells the result.
function Invoke-Git { $ErrorActionPreference = 'Continue'; & git @args 2>$null }
Import-Module (Join-Path (Join-Path $PSScriptRoot 'local') 'VerifyLocal.psm1') -Force
if (-not $QueuePath) { $QueuePath = Join-Path (Join-Path $repo 'tests') (Join-Path 'local' 'checks.json') }

if ($Simulate) {
    if (-not $SimulationFile) { throw '-Simulate needs -SimulationFile (see tools/tests/VerifyLocal.Tests.ps1)' }
    $sim = Get-Content -LiteralPath $SimulationFile -Raw | ConvertFrom-Json
    $simRoot = Split-Path -Parent (Resolve-Path -LiteralPath $SimulationFile).Path
    $env:LIBERTY_SIM_STATE = Join-Path $simRoot 'game'
    $options = @{
        Repo = $repo; QueuePath = $QueuePath; Game = (Join-Path $simRoot 'game'); Blender = [string]$sim.blender; Lvs = ''; Shdn = 'simulated.asi'
        Simulate = $true; Sim = $sim; SimRoot = $simRoot; Interactive = $false; NoManual = [bool]$NoManual
        GameModule = (Join-Path $repo (Join-Path 'tools' (Join-Path 'tests' 'SimulatedGame.psm1'))); ScenarioDirectory = (Join-Path $simRoot 'scenarios')
        ScenarioTimeout = [int]$(if ($sim.PSObject.Properties['scenarioTimeoutSeconds']) { $sim.scenarioTimeoutSeconds } else { 120 })
        Only = $Only; Kinds = $Kind; Smoke = [bool]$Smoke; IncludePassedManual = [bool]$IncludePassedManual
        KeepInstall = [bool]$KeepInstall; Restore = [bool]$Restore; Resume = $Resume
        ResultsRoot = $(if ($ResultsDirectory) { $ResultsDirectory } else { Join-Path $simRoot 'results' })
        NoPush = [bool]$NoPush; Remote = [string]$sim.remote
        GameInfo = [ordered]@{ version = 'simulated'; exeSha256 = 'simulated' }
        Replacements = @{ $simRoot = '<sim>' }
    }
    Invoke-VerifyLocal $options | Out-Null
    exit 0
}

# ---- Settings (remembered between runs; git-ignored)
$settingsPath = Join-Path $PSScriptRoot 'verify-local.settings.json'
$settings = if (Test-Path -LiteralPath $settingsPath) { Get-Content -LiteralPath $settingsPath -Raw | ConvertFrom-Json } else { [pscustomobject]@{} }
function Get-Setting([string] $name, [string] $value) {
    if ($value) { return $value }
    if ($settings.PSObject.Properties[$name]) { return [string]$settings.$name }
    return ''
}
$GameDirectory = Get-Setting 'gameDirectory' $GameDirectory
$Blender = Get-Setting 'blender' $(if ($Blender) { $Blender } else { $env:LIBERTY_BLENDER })
$LvsDirectory = Get-Setting 'lvsDirectory' $LvsDirectory
if (-not $GameDirectory) { throw 'Pass -GameDirectory <folder containing GTAIV.exe> once; it is remembered afterwards.' }

# ---- Preflight: nothing is built or installed unless all of this holds.
$problems = @()
if ($env:OS -ne 'Windows_NT') { $problems += 'this runs on the Windows PC with GTA IV (the cloud uses -Simulate)' }
$exe = Join-Path $GameDirectory 'GTAIV.exe'
if (-not (Test-Path -LiteralPath $exe)) { $problems += "GTAIV.exe not found in $GameDirectory" }
if (Get-Process GTAIV -ErrorAction SilentlyContinue) { $problems += 'GTA IV is running; close it first' }
if (-not $ScriptHookDotNetReference) { $ScriptHookDotNetReference = Join-Path $GameDirectory 'ScriptHookDotNet.asi' }
if (-not (Test-Path -LiteralPath $ScriptHookDotNetReference)) { $problems += "ScriptHookDotNet reference not found: $ScriptHookDotNetReference" }
if (-not (Test-Path -LiteralPath (Join-Path $PSScriptRoot 'toolchains.local.json'))) { $problems += 'toolchains missing: run ./tools/get-toolchains.ps1 -Directory <folder outside the repository>' }
if ($Blender -and -not (Test-Path -LiteralPath $Blender)) { $problems += "Blender not found: $Blender (fix or remove it in tools/verify-local.settings.json)" }
$current = (Invoke-Git -C $repo rev-parse --abbrev-ref HEAD)
if ($current -ne $Branch -and -not $AnyBranch) { $problems += "on branch '$current', expected '$Branch' (git checkout $Branch; git pull; or pass -AnyBranch)" }
$dirty = @(Invoke-Git -C $repo status --porcelain --untracked-files=no)
if ($dirty.Count -gt 0) { $problems += "the working tree has uncommitted changes (git status); commit or stash them first" }
foreach ($drive in @((Split-Path -Qualifier $repo), (Split-Path -Qualifier (Resolve-Path -LiteralPath $GameDirectory).Path)) | Select-Object -Unique) {
    $free = (Get-PSDrive -Name $drive.TrimEnd(':') -ErrorAction SilentlyContinue).Free
    if ($free -and $free -lt 2GB) { $problems += "less than 2 GB free on $drive" }
}
if ($problems.Count -gt 0) {
    Write-Host 'verify-local cannot start:'
    $problems | ForEach-Object { Write-Host "  - $_" }
    exit 1
}
Invoke-Git -C $repo fetch -q origin $Branch | Out-Null
$behind = (Invoke-Git -C $repo rev-list --count "HEAD..origin/$Branch")
if ($behind -and [int]$behind -gt 0) { Write-Host "Note: $Branch is $behind commit(s) behind origin/$Branch. Stop with Ctrl+C and run 'git pull' to test the newest code." }

$saved = [ordered]@{ gameDirectory = $GameDirectory; blender = $Blender; lvsDirectory = $LvsDirectory }
[IO.File]::WriteAllText($settingsPath, ($saved | ConvertTo-Json), (New-Object Text.UTF8Encoding($false)))

$gameInfo = [ordered]@{
    version = (Get-Item -LiteralPath $exe).VersionInfo.FileVersion
    exeSha256 = (Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash
    fusionFix = $(if (Test-Path -LiteralPath (Join-Path $GameDirectory 'plugins\GTAIV.EFLC.FusionFix.asi')) { (Get-Item -LiteralPath (Join-Path $GameDirectory 'plugins\GTAIV.EFLC.FusionFix.asi')).VersionInfo.FileVersion } else { 'missing' })
    scriptHookDotNetSha256 = (Get-FileHash -LiteralPath $ScriptHookDotNetReference -Algorithm SHA256).Hash
}
$replacements = @{ $GameDirectory = '<game>'; $repo = '<repo>' }
if ($env:USERPROFILE) { $replacements[$env:USERPROFILE] = '<home>' }
if ($env:USERNAME) { $replacements[$env:USERNAME] = '<user>' }

$options = @{
    Repo = $repo; QueuePath = $QueuePath; Game = (Resolve-Path -LiteralPath $GameDirectory).Path; Blender = $Blender; Lvs = $LvsDirectory
    Shdn = (Resolve-Path -LiteralPath $ScriptHookDotNetReference).Path
    Simulate = $false; Interactive = ([Environment]::UserInteractive -and -not $NoManual); NoManual = [bool]$NoManual
    GameModule = (Join-Path $PSScriptRoot 'autopilot\Autopilot.psm1'); ScenarioDirectory = (Join-Path $PSScriptRoot 'autopilot\scenarios')
    ScenarioTimeout = $ScenarioTimeoutMinutes * 60
    Only = $Only; Kinds = $Kind; Smoke = [bool]$Smoke; IncludePassedManual = [bool]$IncludePassedManual
    KeepInstall = [bool]$KeepInstall; Restore = [bool]$Restore; Resume = $Resume
    ResultsRoot = $(if ($ResultsDirectory) { $ResultsDirectory } else { Join-Path $repo 'results-local' })
    NoPush = [bool]$NoPush; Remote = 'origin'; GameInfo = $gameInfo; Replacements = $replacements
}
$outcome = Invoke-VerifyLocal $options
if (($outcome.Counts['FAIL'] + $outcome.Counts['CRASH'] + $outcome.Counts['ERROR']) -gt 0) { exit 1 }
exit 0

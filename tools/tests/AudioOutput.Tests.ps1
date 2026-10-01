# Exercise recovery decisions without touching the host's audio devices (also runs in cloud PowerShell).
$audioModulePath = Join-Path $script:RepoRoot 'tools/local/AudioOutput.psm1'
function Invoke-AudioCase([bool] $InitiallyActive, [string] $Fallback, [bool] $Recovers, [bool] $Throws) {
    $module = Import-Module $audioModulePath -Force -PassThru
    & $module {
        param($InitiallyActive, $Fallback, $Recovers, $Throws)
        $script:reads = 0; $script:enables = 0; $script:sleeps = 0; $script:selected = ''
        $script:initial = $InitiallyActive; $script:fallback = $Fallback
        $script:recovers = $Recovers; $script:throws = $Throws
        function script:Get-ActiveAudioOutput {
            $script:reads++
            if ($script:initial -or ($script:recovers -and $script:enables -gt 0)) { 'active-playback' }
        }
        function script:Get-TestAudioFallback { $script:fallback }
        function script:Enable-TestAudioFallback([string] $Id) {
            $script:enables++; $script:selected = $Id
            if ($script:throws) { throw 'policy refused endpoint enable' }
        }
        function script:Start-Sleep { param($Milliseconds) $script:sleeps++ }
        $ready = Initialize-TestAudioOutput
        [pscustomobject]@{ Ready = $ready; Reads = $script:reads; Enables = $script:enables; Sleeps = $script:sleeps; Selected = $script:selected }
    } $InitiallyActive $Fallback $Recovers $Throws
}
$r = Invoke-AudioCase $true 'installed-sonar' $false $false
Test-That 'audio: a working playback output is left alone' ($r.Ready -and $r.Enables -eq 0 -and $r.Reads -eq 1)
$r = Invoke-AudioCase $false '' $false $false
Test-That 'audio: no installed fallback remains unavailable without changing devices' (-not $r.Ready -and $r.Enables -eq 0)
$r = Invoke-AudioCase $false 'machine-specific-endpoint' $true $false
Test-That 'audio: recovery enables the resolved endpoint and confirms it is active' ($r.Ready -and $r.Enables -eq 1 -and $r.Selected -eq 'machine-specific-endpoint' -and $r.Reads -eq 2)
$r = Invoke-AudioCase $false 'installed-sonar' $false $false
Test-That 'audio: an ineffective enable cannot report success' (-not $r.Ready -and $r.Enables -eq 1 -and $r.Reads -eq 11)
Test-That 'audio: waiting for PnP recovery is bounded' ($r.Sleeps -eq 10)
$r = Invoke-AudioCase $false 'installed-sonar' $true $true
Test-That 'audio: a native enable error keeps the game unavailable' (-not $r.Ready -and $r.Enables -eq 1 -and $r.Sleeps -eq 0)
# Restore the actual implementation after the mock tests; later tests must not inherit mock functions.
Import-Module $audioModulePath -Force

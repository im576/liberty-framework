# GTA IV needs a playback endpoint even for unattended tests. Reuse the owner's installed virtual driver when every
# physical output is disconnected. Never enable an arbitrary endpoint, install a driver, or change a working output.
$ErrorActionPreference = 'Stop'

function Get-ActiveAudioOutput {
    @(Get-PnpDevice -Class AudioEndpoint -ErrorAction Stop | Where-Object {
        $_.InstanceId -like 'SWD\MMDEVAPI\{0.0.0.00000000}*' -and $_.Status -eq 'OK'
    })
}

function Get-TestAudioFallback {
    $driver = @(Get-PnpDevice -Class MEDIA -ErrorAction Stop | Where-Object {
        $_.FriendlyName -eq 'SteelSeries Sonar Virtual Audio Device' -and $_.Status -eq 'OK'
    })
    if ($driver.Count -eq 0) { return $null }
    # Audio endpoint registration belongs to the native OS view, including when an x86 host launches the x86 game.
    $view = if ([Environment]::Is64BitOperatingSystem) { [Microsoft.Win32.RegistryView]::Registry64 } else { [Microsoft.Win32.RegistryView]::Registry32 }
    $base = [Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine, $view)
    $render = $null
    try {
        $render = $base.OpenSubKey('SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render')
        if (-not $render) { return $null }
        foreach ($id in $render.GetSubKeyNames()) {
            $properties = $render.OpenSubKey($id + '\Properties')
            if (-not $properties) { continue }
            try {
                if ($properties.GetValue('{a45c254e-df1c-4efd-8020-67d146a850e0},2') -eq 'SteelSeries Sonar - Gaming' -and
                    $properties.GetValue('{b3f8fa53-0004-438e-9003-51a46e139bfc},6') -eq 'SteelSeries Sonar Virtual Audio Device') {
                    # Endpoint IDs vary by PC and driver reinstall; resolve the installed endpoint, never hardcode its GUID.
                    return '{0.0.0.00000000}.' + $id
                }
            } finally { $properties.Dispose() }
        }
    } finally {
        if ($render) { $render.Dispose() }
        $base.Dispose()
    }
    return $null
}

function Enable-TestAudioFallback([string] $Id) {
    if (-not ('LibertyAudio.EndpointPolicy' -as [type])) {
        # Windows PolicyConfig ABI, independently declared from the published interface signatures:
        # https://github.com/frgnca/AudioDeviceCmdlets/blob/master/SOURCE/IPolicyConfig.cs
        # Only the last slot is called. The earlier slots preserve vtable order; no third-party implementation is used.
        Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
namespace LibertyAudio {
    [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")] class PolicyClient {}
    [ComImport, Guid("F8679F50-850A-41CF-9C72-430F290290C8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPolicy {
        [PreserveSig] int GetMixFormat();
        [PreserveSig] int GetDeviceFormat();
        [PreserveSig] int ResetDeviceFormat();
        [PreserveSig] int SetDeviceFormat();
        [PreserveSig] int GetProcessingPeriod();
        [PreserveSig] int SetProcessingPeriod();
        [PreserveSig] int GetShareMode();
        [PreserveSig] int SetShareMode();
        [PreserveSig] int GetPropertyValue();
        [PreserveSig] int SetPropertyValue();
        [PreserveSig] int SetDefaultEndpoint();
        [PreserveSig] int SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr)] string id, int enabled);
    }
    public static class EndpointPolicy {
        public static void Enable(string id) {
            var policy = (IPolicy)new PolicyClient();
            try { Marshal.ThrowExceptionForHR(policy.SetEndpointVisibility(id, 1)); }
            finally { Marshal.ReleaseComObject(policy); }
        }
    }
}
'@
    }
    [LibertyAudio.EndpointPolicy]::Enable($Id)
}

function Initialize-TestAudioOutput {
    if (@(Get-ActiveAudioOutput).Count -gt 0) { return $true }
    $fallback = Get-TestAudioFallback
    if (-not $fallback) { return $false }
    # PnP may lag behind the endpoint-policy update. Only report recovery after Windows actually exposes the output.
    try {
        Enable-TestAudioFallback $fallback
        for ($poll = 0; $poll -lt 10; $poll++) {
            if (@(Get-ActiveAudioOutput).Count -gt 0) {
                Write-Host "autopilot: recovered audio output using installed SteelSeries Sonar - Gaming ($fallback); unattended tests may be silent"
                return $true
            }
            Start-Sleep -Milliseconds 500
        }
        Write-Host 'autopilot: virtual audio output did not become active; GTA IV remains unavailable'
    }
    catch { Write-Host "autopilot: virtual audio recovery failed ($($_.Exception.Message))" }
    return $false
}

Export-ModuleMember -Function Initialize-TestAudioOutput

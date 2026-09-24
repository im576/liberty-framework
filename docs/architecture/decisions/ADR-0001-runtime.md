# ADR-0001: Proposed Complete Edition runtime

Status: **accepted for the minimal C# probe; further gameplay API behavior remains task-specific** (2026-09-24).

Target an un-downgraded GTA IV Complete Edition install with FusionFix. Prototype a C# ScriptHookDotNet script using the [Tomasak CE fork](https://github.com/Tomasak/gta4_scripthookdotnet/releases). IV-SDK .NET currently documents only [1.0.7.0/1.0.8.0](https://github.com/ClonkAndre/IV-SDK-DotNet/blob/main/Documentation/Installation.md), so it is not the baseline.

T-001 compiled an x86 .NET Framework 4.0 probe using the Windows Framework compiler and a managed reference to the downloaded `ScriptHookDotNet.asi`. Fresh game logs confirm the probe loaded, logged heartbeats, received a domain-unload event, and restarted after `ReloadScripts`; the tester reported continued gameplay. The original .NET Framework 4.8 / C# 7.3 suggestion is not a requirement for this minimal probe. The maximum supported language level and gameplay APIs have not been tested. Do not downgrade the game as an automatic fallback.

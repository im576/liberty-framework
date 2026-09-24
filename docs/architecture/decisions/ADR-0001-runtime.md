# ADR-0001: Proposed Complete Edition runtime

Status: **proposed; verify in T-000/T-001** (2026-09-24).

Target an un-downgraded GTA IV Complete Edition install with FusionFix. Prototype a C# ScriptHookDotNet script using the [Tomasak CE fork](https://github.com/Tomasak/gta4_scripthookdotnet/releases). IV-SDK .NET currently documents only [1.0.7.0/1.0.8.0](https://github.com/ClonkAndre/IV-SDK-DotNet/blob/main/Documentation/Installation.md), so it is not the baseline.

The proposed .NET Framework 4.8 / x86 / C# 7.3 toolchain is **not confirmed**. T-001 must prove compile, load, log, error handling, and reload on the tester's exact game build. If it fails, record the error and revise this ADR before implementing gameplay. Do not downgrade the game as an automatic fallback.

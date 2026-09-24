# ADR-0003: Human-readable live tuning

Status: **implemented for the harmless T-002 probe; in-game test pending** (2026-09-24).

Use readable external JSON for profiles and test locations. T-002 uses .NET Framework `DataContractJsonSerializer` for `scripts/LibertyFramework/config/probe.json`, checks its content every ten seconds, and activates a new label only after validation. It retains the last valid label on parse failure. The log is bounded by rotation at 1 MiB plus one backup. The offline valid/malformed/recovery test passed; game deployment and human playtest remain. Future gameplay profiles need their own schemas and validation rules.

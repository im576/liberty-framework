# ADR-0003: Human-readable live tuning

Status: **accepted** — T-002 probe verified in-game (live edit, malformed-input retention, recovery); T-010 applies the same pattern to `gunplay.json` (1 s polling, validation, last-valid retention, save with `.bak`) and presets. Schema: [CONFIG_SCHEMA.md](../CONFIG_SCHEMA.md). Gunplay config behaviour in game awaits the T-010 playtest. Original text:

Use readable external JSON for profiles and test locations. T-002 uses .NET Framework `DataContractJsonSerializer` for `scripts/LibertyFramework/config/probe.json`, checks its content every ten seconds, and activates a new label only after validation. It retains the last valid label on parse failure. The log is bounded by rotation at 1 MiB plus one backup. The offline valid/malformed/recovery test passed; game deployment and human playtest remain. Future gameplay profiles need their own schemas and validation rules.

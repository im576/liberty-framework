# Configuration contract

T-002 implements one harmless runtime sample at `scripts/LibertyFramework/config/probe.json`. The repository sample is `config/probe.json`:

```json
{
  "schemaVersion": 1,
  "probeLabel": "default"
}
```

`schemaVersion` is required and must equal integer `1`. `probeLabel` is required, contains 1–64 ASCII letters, digits, underscores, or hyphens, and is only written to the project log. It changes no gameplay. The script checks for edits every ten seconds; a valid edit becomes active, while malformed, missing, oversized (over 64 KiB), or invalid data is logged and the last valid value remains active. On a fresh start with no valid file, the label is `none`. The installer never overwrites an existing config. This is a probe schema, not a weapon tuning schema.

## Proposed weapon profile shape

No weapon config parser or gameplay values exist yet. This remains a proposal for later tasks.

```json
{
  "schemaVersion": 1,
  "weaponId": "confirmed-game-id",
  "aimProfile": "freeaim_controller",
  "recoilProfile": "pistol_test",
  "spreadProfile": "pistol_test",
  "finish": "GOLD_TEST"
}
```

Future files may live under `config/weapons/`, `config/aiming/`, `config/recoil/`, `config/spread/`, and `config/devtools/`. Every numerical field must state its unit, allowed range, and safe default in this document when implemented. Invalid or missing files must log a useful error and must not activate custom behavior for an unidentified weapon. Live tuning should support apply, revert, save, and reload, with atomic writes or a backup so malformed edits do not destroy a known-good profile.

T-002 uses .NET Framework's `DataContractJsonSerializer` for the probe sample. No gameplay numbers are asserted now.

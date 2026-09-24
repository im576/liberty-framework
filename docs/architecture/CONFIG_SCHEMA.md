# Proposed config contract

No config parser or gameplay values exist yet. This is a shape for later tasks, not a validated schema.

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

T-002 defines actual JSON schemas after T-001 verifies the runtime and available serializer. No gameplay numbers are asserted now.

# Configuration ownership

Current schema index after the two-repository split.

Framework owns `config/engine.json`, diagnostic `probe.json` and example-world config.
Module state/config services are described in the SDK guide and engine architecture.
Liberty+ owns gunplay, catalog, holsters, Arsenal, combat, mood, presets and presentation
configuration, plus its runtime profile at `config/runtime/engine.json`.

The runtime profile is explicitly overlaid for the combined package. Framework-only
defaults have no showcase module-disable policy. Existing in-game paths remain
compatible; separation does not silently migrate stored inventories or saved state.

The exact inherited field reference is preserved in
[the pre-split schema](../archive/pre-split/CONFIG_SCHEMA.md). Its gameplay fields now
refer to Liberty+ config. Update the owning schema/document when changing fields;
do not treat historical defaults as newly approved design decisions.

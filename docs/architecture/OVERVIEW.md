# Proposed architecture (pre-implementation)

The project has no code yet. This design is a dependency map for agents; each module is introduced only by its task card.

```text
Game/ScriptHookDotNet adapters
  -> current weapon identity -> registered gold test weapon?
      -> yes: aim, recoil, spread, reticle, debug profile
      -> no: leave vanilla behavior alone
Config loader -> validated profiles -> live tuning -> save/reload
DevTools -> test actions, teleports, debug overlay
Logger -> startup, state transitions, config errors, caught feature errors
```

The weapon registry is the boundary protecting vanilla behavior. It must use confirmed game identifiers and fail closed: an unknown weapon gets no custom gameplay changes. Configuration stores gameplay values; source code stores algorithms and validated game calls. Feature errors should be logged and disable only the affected feature.

Proposed folders when code begins: `src/Core`, `src/GameApi`, `src/Gunplay`, `src/Weapons`, `src/DevTools`; JSON in `config/`; build/deploy scripts in `tools/`. Do not create empty architecture layers before their first task.

## Logging

One project logger should record startup version/dependencies, active test weapon/profile, config load/reload, menu actions, and exceptions with context. Logs belong under the game `scripts/LibertyFramework/logs/` at runtime, never in Git. T-001 determines the actual output path and verifies it with the human tester.

## Dependencies and gates

See [ADR-0001](decisions/ADR-0001-runtime.md) for the proposed runtime, [ADR-0002](decisions/ADR-0002-weapon-slots.md) for weapon identity, and [ADR-0003](decisions/ADR-0003-config.md) for configuration. All require their named spikes before gameplay implementation relies on them.

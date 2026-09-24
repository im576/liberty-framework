# Architecture

T-001 introduced the runtime probe and logger. T-002 added a harmless JSON config loader, pending in-game test. This is the dependency map for later modules; each is introduced only by its task card.

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

Current source lives in `src/LibertyFramework/`, with `Core/Config` and `Core/Logging`. Future gameplay modules are added only by their tasks. JSON samples live in `config/`; build/deploy scripts live in `tools/`.

## Logging

The project logger records startup, heartbeats, config load/reload, and exceptions with context. It writes under the game `scripts/LibertyFramework/logs/`, never in Git, and rotates at 1 MiB with one backup. Later modules add weapon/profile and menu context as those features are implemented.

## Dependencies and gates

See [ADR-0001](decisions/ADR-0001-runtime.md) for the proposed runtime, [ADR-0002](decisions/ADR-0002-weapon-slots.md) for weapon identity, and [ADR-0003](decisions/ADR-0003-config.md) for configuration. All require their named spikes before gameplay implementation relies on them.

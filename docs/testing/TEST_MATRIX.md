# Phase 1 playtest matrix

Run this after the relevant feature exists. Until then entries are `NOT RUN`, not passing.

| Area | Cases | Status |
|---|---|---|
| Baseline | Launch, vanilla pistol aim/fire/reload, save/load | PASS (T-000, with runtime installed) |
| Input | Steam controller, K/M comparison, pause and cover bindings | PARTIAL (controller DevTools navigation PASS in T-003; K/M not compared) |
| Gold pistol | First shot, rapid fire, reload, movement, cover, vehicle | NOT RUN |
| Gold carbine | Single shot, bursts, full auto, recovery, blind fire | NOT RUN |
| Gold shotgun | Close/medium distance, kick, reload, cover | NOT RUN |
| Containment | Switch gold ↔ vanilla, missions, cutscenes, scripted weapons | NOT RUN |
| Tooling | Menu, spawn, teleport, profile reload, debug, error recovery | PARTIAL (menu + weapon switching PASS in T-003/T-007; the rest is in the T-010 checklist) |

Phase 1 cases are consolidated in [PHASE1_PLAYTEST.md](PHASE1_PLAYTEST.md); update this table from that report.

Every run records game/runtime versions, controller settings, profile ID, and a report using [the template](PLAYTEST_REPORT_TEMPLATE.md).

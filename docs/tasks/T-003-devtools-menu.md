# T-003 — Minimum controller DevTools menu

Status: **READY**. T-002's config/logging probe passed its live test. Scope: one controller-accessible open/close action, category navigation, and a read-only debug item showing runtime/profile status. No weapon actions or tuning until their APIs are verified.

Check input conflicts in aim, cover, pause, and vehicles. Record exact buttons in the final human test steps after T-000 captures the controller layout. Error paths must log and close/disable the menu without breaking the game loop. Agent sets `NEEDS-PLAYTEST` after build and documentation.

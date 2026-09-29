# Verification run 20260929-121911-6931061

- Commit: 6931061 (codex/pr7-windows-check)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T19:19:11.1731340Z  Finished: 2026-09-29T19:30:37.7088147Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260929-122603
- Counts: PASS=2 NEEDS-REVIEW=2 FAIL=0 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260929-122603 | LOOP-package-install.log, LOOP-package-install-install.log |
| `SDK-hot-reload` | scenario | **PASS** | PASS; steps=24 failed=0 logErrors=0 gameAlive=True | SDK-hot-reload.log, SDK-hot-reload |
| `T021-sling-review` | scenario | **NEEDS-REVIEW** | NEEDS-REVIEW; steps=32 failed=0 logErrors=1 gameAlive=True | T021-sling-review.log, T021-sling-review |
| `T024-trunk-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: trunk_open, trunk_wheel_next_segment, trunk_after_store, trunk_closed | T024-trunk-review.log, T024-trunk-review |

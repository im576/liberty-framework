# Verification run 20260929-142632-0d849f0

- Commit: 0d849f0 (codex/pr7-windows-check)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T21:26:32.1439960Z  Finished: 2026-09-29T21:36:27.5351087Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260929-143341
- Counts: PASS=2 NEEDS-REVIEW=0 FAIL=1 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260929-143341 | LOOP-package-install.log, LOOP-package-install-install.log |
| `SDK-hot-reload` | scenario | **PASS** | PASS; steps=24 failed=0 logErrors=0 gameAlive=True | SDK-hot-reload.log, SDK-hot-reload |
| `T021-sling-review` | scenario | **FAIL** | FAIL; steps=41 failed=1 logErrors=1 gameAlive=True | T021-sling-review.log, T021-sling-review |

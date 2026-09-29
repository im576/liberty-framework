# Verification run 20260929-144522-e02501b

- Commit: e02501b (codex/pr7-windows-check)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T21:45:22.7250242Z  Finished: 2026-09-29T22:06:39.2547222Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260929-145151
- Counts: PASS=3 NEEDS-REVIEW=1 FAIL=0 CRASH=1 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260929-145151 | LOOP-package-install.log, LOOP-package-install-install.log |
| `T031-multimat-report` | pc-offline | **PASS** | all expected fields match | T031-multimat-report |
| `T031-multimat-review` | scenario | **CRASH** | CRASH; steps=13 failed=1 logErrors=0 gameAlive=False | T031-multimat-review.log, T031-multimat-review |
| `SDK-hot-reload` | scenario | **PASS** | PASS; steps=24 failed=0 logErrors=0 gameAlive=True | SDK-hot-reload.log, SDK-hot-reload |
| `T021-sling-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: front, back, left, right, back_close | T021-sling-review.log, T021-sling-review |

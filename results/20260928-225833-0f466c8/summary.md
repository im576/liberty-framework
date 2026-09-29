# Verification run 20260928-225833-0f466c8

- Commit: 0f466c8 (codex/pr7-smoke-ray-fix)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T05:58:33.0879487Z  Finished: 2026-09-29T06:09:04.2541408Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260928-230506
- Counts: PASS=1 NEEDS-REVIEW=0 FAIL=1 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260928-230506 | LOOP-package-install.log, LOOP-package-install-install.log |
| `T027-raycast` | scenario | **FAIL** | FAIL; steps=40 failed=3 logErrors=0 gameAlive=True | T027-raycast.log, T027-raycast |

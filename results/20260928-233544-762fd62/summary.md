# Verification run 20260928-233544-762fd62

- Commit: 762fd62 (codex/pr7-windows-check)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T06:35:44.5109503Z  Finished: 2026-09-29T06:44:53.9610109Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260928-234209
- Counts: PASS=1 NEEDS-REVIEW=1 FAIL=0 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260928-234209 | LOOP-package-install.log, LOOP-package-install-install.log |
| `T026-perf-stress` | scenario | **NEEDS-REVIEW** | NEEDS-REVIEW; steps=18 failed=0 logErrors=2 gameAlive=True | T026-perf-stress.log, T026-perf-stress |

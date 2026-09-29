# Verification run 20260928-220635-84d4127

- Commit: 84d4127 (codex/pr7-smoke-ray-fix)  Mode: smoke
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T05:06:35.3168394Z  Finished: 2026-09-29T05:16:30.3299409Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260928-221240
- Counts: PASS=3 NEEDS-REVIEW=0 FAIL=0 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-build` | pc-offline | **PASS** | exit code 0 | LOOP-build.log |
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260928-221240 | LOOP-package-install.log, LOOP-package-install-install.log |
| `SDK-selftest` | scenario | **PASS** | PASS; steps=13 failed=0 logErrors=0 gameAlive=True | SDK-selftest.log, SDK-selftest |

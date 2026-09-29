# Verification run 20260929-115421-14010f6

- Commit: 14010f6 (codex/pr7-windows-check)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T18:54:21.5945143Z  Finished: 2026-09-29T19:08:39.9312731Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260929-120203
- Counts: PASS=1 NEEDS-REVIEW=1 FAIL=2 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260929-120203 | LOOP-package-install.log, LOOP-package-install-install.log |
| `SDK-hot-reload` | scenario | **FAIL** | FAIL; steps=22 failed=2 logErrors=3 gameAlive=True | SDK-hot-reload.log, SDK-hot-reload |
| `T021-sling-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: front, back, left, right, back_close | T021-sling-review.log, T021-sling-review |
| `T024-trunk-review` | scenario | **FAIL** | FAIL; steps=35 failed=5 logErrors=0 gameAlive=True | T024-trunk-review.log, T024-trunk-review |

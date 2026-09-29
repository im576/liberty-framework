# Focused Windows review at `e02501b`

The verifier's `summary.json`, scenario logs and screenshots in this run directory are the primary evidence. The engine/content builds succeeded. The authored `lf_fx_multimat` package build used the structure writer and `pc/data/maps/east/bronx_e.img/big_fence2_bxe` template; its report confirms **two geometries, two textures, readback `[]`**, and the queued `T031-multimat-report` passed.

| Check | Review |
| --- | --- |
| SDK hot reload | **PASS**, 24 steps, zero failed, zero log errors, game alive. |
| T-021 sling | Script **PASS**, 45 steps, zero failed and zero log errors. `await-alive`, `teleport_done`, alive checks and both LongGun1/LongGun2 attachment events succeeded. All five East Park screenshots show a live character; rear and side views clearly show two separate slung long guns. Visual attachment passes this capture. Blood on the pavement and character indicates nearby combat, but the alive checks remained successful. |
| T-031 multimat review | **CRASH**. The third launch attempt booted the engine. `await-alive`, teleport, alive, time/weather and `spawnprop lf_fx_multimat 2.5 0` received replies. The last relevant log line is the spawn request at 22:00:08Z. The expected `autopilot_prop` confirmation never appeared; the game exited while waiting. No multimat screenshot exists, so rendering remains unverified. |

Three fresh Windows Application Error/WER records accompanied the run. Their event times were 21:52:38Z, 21:56:18Z and 22:01:27Z; all report `StackHash_2beb`, exception `0xc0000005`, `PCH_4E_FROM_ntdll+0x0007379C`, fault module unknown. Their loaded-module lists end with `MTLX.DLL` and C++ runtimes and contain no ScriptHook or Liberty Framework module. This matches the previously documented startup-crash signature, including the September 28 record. The last WER event occurred after the multimat spawn request, but WER timing and module presence alone do not establish whether the new asset or the existing launch fault caused the game exit. There is no in-game prop log or screenshot to establish successful loading.

Restore: the verifier restored from `phase2-20260929-145151`. An independent SHA-256 comparison of all **42** rollback actions against their saved originals found **zero mismatches**. The game was closed after the run.

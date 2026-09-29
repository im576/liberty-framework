# Initial Windows verification of sessions 4b, 5 and 6

Run: `20260928-221840-84d4127`, CE 1.2.0.59, commit `84d4127` (PR #7 plus the SDK smoke-test ray fix). The complete structure-only probes, scenario logs and screenshots are on the `verification-results` branch under `results/20260928-221840-84d4127/`. The verifier restored the prior installation. The earlier smoke run `20260928-220635-84d4127` passed build, package/install and SDK self-test (`49 passed, 0 failed`).

## Queue result

Of 54 checks: 12 PASS, 10 NEEDS-REVIEW, 12 FAIL, 1 CRASH and 19 NOT-RUN. Eighteen NOT-RUN checks were manual; the Blender check lacked a configured Windows Blender path. The build, offline verifier, content self-test, SDK self-test, engine events, bullet events, exact damage and vehicle events passed. The game scenarios remained alive except the 25-ped stress run.

These results do not establish the initial engine foundation as finished. In particular, the LOD post used the v1 fallback, and the world wall spawned visibly but had no collision.

## Format findings (VERIFIED OFFLINE on the owner's game files)

- `PROBE-drawables` found 22,814 WDR entries and parsed 22,812. All parsed drawables used only LOD slot 0. The four-LOD test post therefore found no matching structure template, compiled as `template (fallback)` and retained only LOD 0. Its five screenshots do not prove LOD switching; the green one-band post is visible at 6, 18 and 38 m, the 75 m view is blocked by foliage, and the 130 m shot does not clearly show the post.
- `T031-drawable-roundtrip` found 12,688 eligible drawables, of which 5,254 rebuilt identically and 7,434 differed. The first reported differences are system offsets; the report records only the first 25 failures, so their distribution is not yet established. The structure writer cannot be made the unconditional default from this evidence.

**Focused WDR diagnostic (commit `4f56e61`):** the read-only `Template` buffer order candidate was tested on
`bronx_e.img`. Of 352 WDRs, 187 were eligible; 105 matched and 82 differed. `Template` alone accounted for 23
matches missed by both earlier orders. The full field classification found changed index-buffer data pointers in all
82 remaining files and changed vertex-buffer data pointers in 77; no vertex/index data-content mismatch was reported.
Two files also failed the drawable-box containment check. Only 66 of 187 eligible files had matching RSC page flags,
which are outside this roundtrip pass condition. Buffer placement, gaps or padding and the two bounds cases need
further research before the writer can be promoted. This targeted archive result does not replace the full-corpus
12,688-file result. Evidence: `verification-results/results/20260928-4f56e61-bronx-e/`.
- `PROBE-collision` found 732 WBN, 348 WBD and 85 WBS resources. It found zero drawable/WBN pairs with the same name in the same archive. WBN and WBD are RSC type 32; WBS is type 1. The bounds-layout probe grouped 1,165 resources into ten extension/type/root-word classes. These are measurements, not decoded collision structures.

**Focused collision-link probe (commit `a61fdfe`):** the Windows content build and 347/347 self-tests passed. Across
1,080 WBD/WBN resources in 195 opened IMG archives with WDRs, the probe found 3,572 aligned words matching hashes of
same-IMG WDR names: 3,564 in WBD and eight in WBN. `bronx_e.wbd` has twelve consecutive matching names. This
strongly suggests a WBD name-hash table but does not associate a hash with a bounds object or establish runtime
collision pairing. Five IMG archives were omitted after parse errors. Evidence:
`verification-results/results/20260929-a61fdfe-collision-links/`.

**CE WBD table check:** a read-only parser now identifies index-aligned name-hash and structured-target-pointer
arrays in root class `0x00695360`. It parsed 309/309 WBDs of that class, including 3,239 table entries with valid
targets, and re-encoded the known table fields identically in all 309. In 13 selected WBDs, all 100 name hashes match
same-IMG WDRs and their same-index target pointers are valid. Content self-test passed 354/354, including mutation
checks. This verifies the hash-to-target table structure; it does not decode the target shapes, round-trip the full
WBD, or prove GTA IV's runtime pairing. Evidence: `verification-results/results/20260929-70e7fd4-wbd-structure/`.
The queued `T032-wbdcheck` also passed through the standard Windows verifier (run
`20260929-093318-d4ebc2f`): 309/309 parsed and table-field round-trips, 3,239 entries, five unreadable IMG archives.
The run installed nothing and captured its JSON report.

**Focused WBD target prefix inventory:** `wbdtargets` sampled the first 0x200 bytes of all 3,239 indexed CE WBD
targets, bounded by the next dictionary target where available. Six first-word classes were observed. The dominant
class (`0x0069C19C`) has four consistent system-pointer fields at `+0x8C`, `+0xB0`, `+0xD0`, and `+0xE0`; their
meanings remain unknown. This supports the next read-only decode step, not an authored collision writer. Evidence:
`verification-results/results/20260929-d4ebc2f-wbd-targets/`.
- The automatic borrowed-collision search therefore found no candidate. `lf_col_crate` and `lf_world_wall` built without a borrowed WBN. Their failed raycasts do not decide whether GTA IV would pair a same-named WBN with a script-created object if one were provided. The static wall did spawn and was visible in its screenshot.
- `T028-wtdcheck` inspected 79 dictionaries; 68 rebuilt, 67 compared identical, 11 uncompressed dictionaries were skipped, and no parser exception was counted. Its nonzero exit is a real single roundtrip mismatch. The probe also recorded 99 nonzero texture-record `+0x40` words among compressed textures, while the writer emits zero, and all 79 dictionaries used several graphics pages. The native crate and alpha panel visibly rendered in game, but the texture roundtrip claim remains incomplete.

**Focused WTD rerun (commit `762fd62`):** preserving each source texture's opaque record fixed the reported mismatch.
`T028-wtdcheck` passed on the Windows game files: 79 dictionaries inspected, 68 compressed dictionaries rebuilt,
**68 byte-identical**, 11 uncompressed skipped, 0 failures. Evidence:
`verification-results/results/20260928-234601-762fd62/`. The other nonzero `+0x40` records remain opaque and are
copied from the matching source texture; their meaning has not been decoded.

## Latest focused run: 20260929-121911-6931061

- Package/install passed. SDK hot reload passed all 24 steps, zero log errors; selftest completed 49/49.
- Sling completed 32 scripted steps, but visual review is FAIL. Front/back show the player dead; left is black;
  right/close show an unarmed respawn. Both attachment-slot events occurred before death. NPC gunfire reduced
  player health after `god on` was accepted, followed by `arsenal_loss_no_safehouse`. The repeated protection
  request was a no-op for an existing owner. Reapplying on every explicit true request and checking survival
  before each capture are pending fixes; the native flag reset's cause has not been established.
- Trunk completed 35/35 steps, zero errors. The wheel navigates from rifles to sniper rifles, stores AK-47 with
  120 rounds, and closes in the final image. The partially hidden lid prevents a complete boot-closure verdict.
- Hot-reload teleport stages: move 3 ms, collision request 6 ms, `LOAD_SCENE` 3,394 ms. Later scene loads were
  296 ms (sling) and 649 ms (trunk). This isolates a blocking scene load in this run; it does not establish the
  previous 30-second stall's exact stage. Performance remains a release gate.
- `restore.log` records restoration. The resumed Windows agent independently compared all 42 saved install
  files: 42 hash matches, zero mismatches. The usage interruption did not prevent the verifier finishing.

Evidence: `verification-results/results/20260929-121911-6931061/` (published at `335f1ea`).

## Follow-up run: 20260929-142632-0d849f0

- SDK hot reload passed 24/24, with 49/49 selftest and no log errors.
- Sling failed 1/41: `alive` after setup found the player dead. Wasted/arsenal loss occurred before its first command;
  later `god on` did not resurrect the player. Respawn happened after the attempted park teleport.
- Later survival checks report health 100, and the five captured images show two slung long guns. Mac review of front
  and back-close images confirms a live player, straps and two distinct weapon props, outside the medical center.
  This establishes those attachments, not a passing scenario or movement/clipping behavior.
- The scenario now waits for a live player using a bounded SDK coroutine before teleport, expects teleport completion
  and retains its later survival assertions. The native protection-reset cause remains unknown.
- Installation restored; independent verification found zero mismatches among all 42 rollback actions.

Evidence: `verification-results/results/20260929-142632-0d849f0/` (including `review.md`).

## Runtime findings (VERIFIED IN GAME for this run)

- The raycast spike, SDK self-test, engine events, bullet events, exact damage and vehicle events passed. The main raycast scenario's fixed forward ray lost a moving pedestrian before its filtered pass-through check. A targeted rerun using a `rayto` command passed that pedestrian check, but failed three other steps when a nearby object entered the car ray and Steam screenshot capture failed. Those are separate results; a clean raycast regression is still needed.
- Hot reload restarted and reloaded, but its following self-test failed three ray checks after a nearby object blocked the test path. Sling and trunk scenarios also failed scripted steps. The visual scenarios need their screenshots reviewed independently of step success.
  The self-test now casts a short line through the spawned test ped's torso instead of starting at the player's feet;
  that targeted change needs a focused Windows `SDK-hot-reload` rerun.
- Visual review of the published screenshots: the SDK list menu is visible and readable. The sling close-up shows a grayscale death scene with the player on the ground, so it cannot establish the requested weapon placement. The trunk `after_store` and `closed` shots both still show the open radial menu; the log's missing close/choreography events reflect a visible unfinished interaction. These checks need clean reruns after their scenario behavior is corrected.
  The sling log did contain `holster_sling_attached` after the weapon grant, before the following `select` command. The
  old scenario looked only after `select`, then waited through a failed 15-second expectation while the player died.
  The runner now supports `mark` / `expectmarked`, and the sling scenario checks the event from before its weapon
  grants. This repairs the timing check; the placement screenshots and player survival still require a Windows rerun.
- The native crate texture is clearly visible on its side screenshot. The alpha panel's checker and red bar appear on both faces, but its screenshots do not put a contrasting object directly behind the checker. They establish texture rendering, not the intended 64/192 alpha blending; that visual check still needs an overlapping backdrop.
- The trunk run reported frame intervals around 400–845 ms near the missed Backspace action. Its 250 ms injected key may have fallen between input polls; the scenario now holds it for 1,200 ms. This is a test hypothesis until a focused rerun shows a close event and a closed menu.

**Focused trunk rerun (commit `a61fdfe`):** holding Backspace for 1,200 ms produced the close and choreography-complete
events. The scenario passed 34/34 steps with no log errors and the wheel is absent in the final screenshot. The
selection and store screenshots still show the same PISTOLS/Glock 17 slot and “Nothing stored in this slot”; the
250 ms Right/Left/Space presses did not visibly establish navigation or storage. The trunk lid is partly obscured.
The scenario now holds those three keys for 1,200 ms and explicitly expects `arsenal_store id=14`; that change needs
another focused in-game run. Evidence: `verification-results/results/20260929-090327-a61fdfe/`. The verifier restored
all 42 installed-file actions.
- The stress scenario logged `teleport_done` with coordinates `(0, 1417044000, 2.136292e27)` immediately before GTAIV.exe exited with an access violation. The log supports a corrupt pavement snap reaching the teleport path; it does not prove the precise source of the corrupt value or exclude other crash causes. A guard now rejects nonfinite or nonlocal snap results and falls back to the configured destination. That guard needs a focused Windows rerun.

**Focused stress rerun (commit `762fd62`):** GTA IV stayed alive and the 25-ped scenario completed all 18 steps with
no failed step. The teleport ended at `(-64.4, 674.8, 15.5)`, and all 25 spawned peds were cleared. The result is
**NEEDS-REVIEW**, because the engine logged a 5,013 ms stall in `engine.commands` during setup, before
`teleport_start`. With 25 extra peds, the reported frame p95 was 55.64 ms and `engine.frame` averaged 2.467 ms over
464 samples. The machine reported 94% physical memory load. This proves the previous crash did not recur in the
focused run, not that stability or performance has cleared the release gate. Evidence:
`verification-results/results/20260928-233544-762fd62/`. The run restored the game install; all 40 replaced files
matched their backups and both newly created files were removed.

## Required follow-up

**September 29 focused runtime run (`14010f6`):** package/install passed and GTA IV stayed alive in all three
scenarios; the verifier recorded restoration of all 42 installed-file actions. The sling scenario passed 31/31
steps with no log errors. Its images show a live player, one slung rifle and a shoulder strap. Both granted long guns
were rifles (IDs 15 and 14 share a game slot), so the scenario now gives shotgun 10 and rifle 14 and expects both
`LongGun1` and `LongGun2` attachment events before capture. Two-gun placement remains pending.

Hot reload itself succeeded and all revised ped/vehicle ray checks passed. The self-test finished 48/49: player
death cancelled its choreography. Reload released the old Autopilot's invincibility resource, and the scenario had
not acquired it on the new instance. The scenario now reapplies `god on` after proving the old resources were
released. Its initial teleport reply arrived after about 30 seconds, beyond the command channel's 20-second timeout;
new `teleport_stage` timing logs will distinguish move, collision request and scene load. This stall remains a
performance failure, not a passing teleport check.

Trunk failed all five interaction expectations. Its screenshot still shows the nearby trunk prompt and a closed
menu; the initial E press lasted 80 ms amid roughly 100 ms frames. The scenario now holds E for 1,200 ms, matching
its other interaction keys. This is a test hypothesis pending a rerun. Evidence:
`verification-results/results/20260929-115421-14010f6/`.

1. Diagnose the common WDR system-byte differences using representative original files and fix the writer or narrow its eligible set based on measured structure.
2. Research how GTA IV represents multiple LODs when every sampled WDR uses one LOD slot. Change the content strategy or synthesize a verified structure; do not claim four in-game LODs from the current fallback.
3. Map WBD/WBN to static prop collisions; decode one relevant bounds class and roundtrip original files before writing authored collision. Verify player, vehicle and raycast interaction in game.
4. Diagnose the stress setup stall, rerun the raycast and hot-reload checks in clean scenes, and inspect the other scenario log errors. The WTD mismatch is fixed; keep its roundtrip regression in the queue.
5. Review visual screenshots and run the manual collision and episode checks before release stabilization and merging into `main`.

# Collision (bounds) in GTA IV: what is known

**Status (2026-09-26): research not started in this project.** Nothing below is verified here. It sets out what to
establish before any collision writer is built (roadmap: content compiler step 3, `docs/content/README.md`).

**Update (2026-09-28):** the first Windows inventory and bounds-layout probes ran. See
[`2026-09-28-windows-verification.md`](../reports/2026-09-28-windows-verification.md). Their results below supersede
the initial status for the measured claims; no bounds class has been decoded or round-tripped yet.

Evidence labels: **VERIFIED IN GAME**, **VERIFIED OFFLINE** (from the owner's game files by a repository tool),
**PLAUSIBLE** (public community documentation, not checked here), **UNKNOWN**.

## Claims

| # | Claim | Label | Source |
|---|---|---|---|
| C1 | Static world collision ships as RAGE bounds resources in the map IMG archives: `.wbd` (a dictionary of bounds) and `.wbn` (one bounds object). | PLAUSIBLE | Community documentation of IV formats. To be checked by `PROBE-collision` |
| C2 | Fragment types (`.wft`: breakable props, vehicles) carry their own bounds inside the fragment. | PLAUSIBLE | Community documentation |
| C3 | Bounds classes include composite, geometry and primitive shapes (box, sphere, capsule). The class layouts in IV's resources are not documented here. | PLAUSIBLE / layouts UNKNOWN | Community documentation |
| C4 | How the game pairs an IDE object with its collision: a bounds resource of the same name, the map section's `.wbd`, or a fragment. | UNKNOWN | Needs the inventory, then an in-game test |
| C5 | Whether a script-created object (`CREATE_OBJECT`, `spawnprop`) gets collision from static bounds, or only from a fragment. | UNKNOWN | In-game test |
| C6 | Whether the engine's raycast (ADR-0008, the game's line test against the physics world) hits an authored object's collision. | UNKNOWN | Follows from C4/C5; `RayMask.Objects` is also untested against vanilla props |

## First Windows probe results (VERIFIED OFFLINE, CE 1.2.0.59)

`PROBE-collision` in `verification-results/results/20260928-221840-84d4127` counted 732 `.wbn`, 348 `.wbd` and
85 `.wbs` entries in opened IMG archives. WBN and WBD entries were RSC type 32; WBS entries were type 1. The
`PROBE-bounds-layout` report measured 1,165 resources in ten extension/type/root-word classes. Five archives returned
`Value was invalid.` and were omitted from that layout report; the result is an inventory of successfully opened
archives, not a proof that no other bounds resources exist.

The `propCandidates` rule found **zero** WDR/WBN pairs of the same name within one archive. Thus the automatic borrow
in this session supplied no bounds file, and the failed `collision-borrow` and `world-objects` rays cannot answer C4–C6.
Many WBN files are named for map chunks (for example `bronx_e_1.wbn`), while WBD files include map-area names (for
example `bronx_e.wbd`). This naming pattern is evidence for further investigation, not a verified lookup rule.

Next probe: inspect the WBD dictionary's entry names/hashes and its references to WBN or embedded bounds, then relate
those entries to IDE object names and runtime object collision. Choose one small, relevant bounds class and prove a
byte-for-byte round trip before enabling authored shapes. Do not substitute an arbitrary map WBN for a prop's bounds.

The measured CE `.wbd` root class (`0x00695360`, 309 files) has system pointers at root `+0x04`, `+0x10` and `+0x18` in
every sampled file; root `+0x0C` is always `1`. The pointed words at `+0x18` begin with valid system pointers, while
`+0x04` begins with zero followed by opaque words. This does **not** identify a dictionary layout yet. A focused
structure-only probe should list the first few words, pointer validity and candidate entry counts for a small CE map
WBD, then compare candidate key words with the existing Jenkins hashes of nearby WDR and IDE names. Record matching
names and offsets without publishing resource bytes. Only after the relationship is demonstrated should a reader name
those fields and attempt a round trip. The TBoGT root class has a different vtable and needs separate validation.

`PROBE-collision-links` is the first read-only check for this hypothesis. It scans 32-bit aligned system words in
WBD/WBN resources for hashes of WDR names **from the same IMG**, reporting only matching names and offsets. It does
not assume where the dictionary table starts or claim a matched word is a key. A zero-match result would narrow the
hypothesis but would not exclude references by IDE ID, cross-archive name, another hash, or an unaligned field.

**Windows result (2026-09-29, VERIFIED OFFLINE):** in 195 successfully opened IMG archives containing both drawables
and bounds, the probe scanned 1,080 WBD/WBN resources. It found 3,572 matching aligned words across 172 resources and
3,569 distinct same-IMG WDR model names. WBD accounts for 3,564 matches; WBN accounts for eight. `bronx_e.wbd`
contains twelve consecutive matching WDR hashes at system offsets `0x4FB0`–`0x4FDC`. These runs strongly support a
WBD model-name hash table, but neither a hash-to-bounds association nor the runtime pairing rule is proven. Five IMG
archives could not be opened by the probe. Full names, offsets and counts are in
`verification-results/results/20260929-a61fdfe-collision-links/`; the report contains no resource bytes.

The follow-up read-only probe traced the structure pointing to the consecutive hash run and a parallel target array.
A same-name/hash match alone was insufficient to select a collision resource for `lf_col_crate`.

**CE WBD table reader (2026-09-29, VERIFIED OFFLINE):** root vtable `0x00695360` uses a hash-array pointer at
`+0x10` and a structured-target-pointer array at `+0x18`; each has 16-bit count/capacity fields immediately after its
pointer. In 13 selected WBDs, the counts agree, all 100 hashes match WDR names in the same IMG, and all 100
same-index targets are valid system pointers. Across the available game archives, the reader parsed all 309 WBDs of
this root class (3,239 entries), and re-encoding the known root/table fields matched all 309 originals. Of those
entries, 3,039 hashes match same-IMG WDR names. The target class has six observed vtable values. This is an
index-aligned **hash-to-structured-target relationship**; calling the targets collision shapes is still an inference.
The check does not decode or round-trip the pointed-to objects or the unknown remainder of each WBD. Five IMG
archives remain unreadable by the current archive reader. Evidence:
`verification-results/results/20260929-70e7fd4-wbd-structure/`.

Next, classify the six pointed-to vtable classes and decode the smallest relevant shape/container using real-file
cross-checks. A full roundtrip of the relevant resource content and an in-game model-pairing test still precede the
authored writer.

## Plan

1. **Inventory (queued):** `PROBE-collision` (`LibertyContent probe collision`) lists which collision-like resources the
   game ships, in which archives, how many, with their RSC types, sizes and root vtable words. Structure only.
2. **Decide the target:** from the inventory, pick the resource a single static prop would use (C4). If props rely on
   fragments, that changes the plan (WFT is a deferred compiler expansion).
3. **Decode one class:** a follow-up probe that parses that resource type's root structure across all files and
   round-trips it (the method `wtdcheck` and the drawable self-test use: rebuild the game's own files byte for byte).
4. **Writer:** only after step 3 round-trips the game's files.
5. **In game:** a scenario that places an authored object with collision and proves player collision, vehicle collision
   and a raycast hit (C5, C6).

Nothing here may be implemented from memory of other tools (AGENTS.md rules 4 and 7).

## Authoring side (2026-09-26, T-030)

Artists can already tag collision in Blender: mesh, box, sphere and capsule, with an optional surface name. The
compiler imports these into its IR, `ContentCollision` ([README](../content/README.md#collision-authoring)). That set
is an authoring choice, not a claim about the game (C3 stays PLAUSIBLE / UNKNOWN).

When step 3 establishes the bounds classes:
- the collision writer maps each authored shape onto one of them;
- it lists the shapes it emits in `CompilerCapabilities.CollisionShapes`.

Until then the validator refuses collision (LCC032). Surface names are kept as authored; their game meaning is UNKNOWN.

## Session 5 (2026-09-26, T-032): tooling up to plan step 3, and one experiment

- **Plan step 1:** `PROBE-collision` (queued) is the inventory and lists `propCandidates`.
- **Input for step 3:** `PROBE-bounds-layout` (`LibertyContent probe bounds`) measures each class's layout: every root
  word classified across all files as pointer, small integer (with values), float, zero or other, the same one level
  down, and a root size bound. Decoding a class starts from that report, with labels, then a probe that round-trips the
  game's files. Nothing is decoded from memory.
- **Experiment for C4–C6:** borrowed collision. `asset.json` `collision.borrow` ships a vanilla prop's own bounds
  resource, unchanged, as `<name>.wbn` next to this project's model.
  - `lf_col_crate` and scenario `collision-borrow` test the pairing and the raycast, against `raycast-objects` on the
    same vanilla prop.
  - `lf_world_wall` (T-033) tests a placed world object and the player (`T033-world-walk`).
- **Step 4, the collision writer for authored shapes,** waits for steps 2 and 3.
  `CompilerCapabilities.CollisionShapes` stays empty and LCC032 stays until then.

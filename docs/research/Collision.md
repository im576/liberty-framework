# Collision (bounds) in GTA IV: what is known

**Status (2026-09-26): research not started in this project.** Nothing below is verified here. It sets out what to
establish before any collision writer is built (roadmap: content compiler step 3, `docs/content/README.md`).

**Update (2026-09-28):** the first Windows inventory and bounds-layout probes ran. See
[`2026-09-28-windows-verification.md`](../reports/2026-09-28-windows-verification.md). Their results below supersede
the initial status for the measured claims; no bounds class has been decoded or round-tripped yet.

Evidence labels: **VERIFIED IN GAME**, **VERIFIED OFFLINE** (from the owner's game files by a repository tool),
**PLAUSIBLE** (public community documentation, not checked here), **UNKNOWN**.

**Serialized class words are not runtime addresses:** the current probes and historical tables call the first
resource word a `vtable`. That is a grouping label for bytes in the file, not proof that the value addresses a
vtable in CE 1.2.0.59. Static inspection of the owner's executable places `0x0069C19C` in `.text`, rather than a
demonstrated vtable, and found no direct address reference in the scan. Do not disassemble it as a function start,
use it as a hook address or infer a constructor from it. Resource fixup/type dispatch must establish the mapping
to current runtime classes. An absent direct reference does not exclude indirect dispatch. The first-word class
inventory and field measurements remain valid; their runtime interpretation is UNKNOWN.
Evidence: `verification-results/results/20260929-t032-classword-pe/report.md` records the executable hash,
PE section ranges and direct-reference scan limits.

## External layout lead — HYPOTHESIS, pending CE probe

Pinned CitizenFX reference: [`phBound.h`](https://github.com/citizenfx/fivem/blob/e60d29ac2d6e894e20ba78d5fdf3c190d976fd2a/code/components/rage-formats-x/include/phBound.h)
contains explicit NY branches. Its 32-bit declarations suggest the following candidate fields, independently inferred
for a read-only measurement; these are not CE offsets approved for hooks or writers:

| Relative field | Candidate interpretation | Required owner-file evidence |
|---|---|---|
| +0x04 byte | bound kind (Geometry 4, BVH 10, Composite 12) | distribution per serialized class |
| +0x8C pointer | polygon records | count/span validity and index ranges |
| +0x90 / +0xA0 float4 | vertex quantum / offset | finite components, reconstructed bounds |
| +0xB0 pointer | quantized vertices, six bytes each | count/span validity |
| +0xC8 / +0xCC u32 | vertex / polygon counts | valid arrays across samples |
| +0xD0 pointer, +0xD8 byte | materials / count | range and per-polygon consistency |
| +0xE0 pointer | BVH | tree structure and leaf coverage |

The NY polygon candidate is 32 bytes, with signed 16-bit vertex indices beginning at +0x10; its fourth index's role
remains unknown. The four candidate pointer offsets coincide with the common class's measured fields. Coincidence
supports probing, not semantic verification.

[`NYBounds.cpp`](https://github.com/citizenfx/fivem/blob/e60d29ac2d6e894e20ba78d5fdf3c190d976fd2a/code/components/rage-formats-x/tests/NYBounds.cpp)
asserts 128-byte base bounds, 224-byte geometry and 0x58-byte BVH in its NY test configuration. That test uses a
particular WBN example, not this owner's CE corpus. The first probe must retain invalid counts and test every common
class target; report numeric metadata only. No reference code is incorporated in Liberty. A reader/writer still needs
independent field evidence, complete original-resource roundtrip and runtime pairing.

Existing `results/20260929-d4ebc2f-wbd-targets/targets.json` supplies independent raw-field correlations:

- `0x0069C19C`: +0xC8 and +0xCC classify as small integers in all 2,154 samples, while all four candidate pointers
  are system pointers. These are VERIFIED OFFLINE byte classifications; count/array semantics remain HYPOTHESIS.
- `0x0069AAF4`: those two fields are small integers in all 185 samples; +0xD8 is 1 (145 samples) or 2 (40).
- `0x0069D56C`: +0xC8 is 8 and +0xCC is 6 in all 634 samples. Some entire +0x04 words equal 3; the low byte must
  still be measured independently of surrounding flags. Its +0xD0 is a float in every sample, so do not apply the
  geometry-material pointer hypothesis to this class.

The new probe should inspect all four signed indices, including +0x16, to identify valid quads or sentinel values
rather than assuming every polygon is a triangle. Array span validity means containment in the system segment;
it does not by itself prove exclusive allocation ownership or correct decoding.


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

**Target prefix inventory (2026-09-29, VERIFIED OFFLINE):** the read-only `wbdtargets` probe grouped all 3,239 CE WBD
targets by first-word class. Class `0x0069C19C` has 2,154 entries in 298 WBDs; its target-relative offsets `+0x8C`,
`+0xB0`, `+0xD0` and `+0xE0` classify as system pointers in all samples. The other five classes show different
pointer-presence patterns. The sampled window does not establish object size, field meanings or a collision shape
type; another allocation may lie before the next dictionary target. The full class counts and structural report are
in `verification-results/results/20260929-d4ebc2f-wbd-targets/`. The next decode should follow these pointers in a
small number of originals, compare allocation relationships across files, and verify the resulting interpretation
before using it in a writer.

**Nested-pointer sample (2026-09-29, VERIFIED OFFLINE measurements):** all 2,154 targets of class `0x0069C19C`
have in-range pointer-like words at the four fields above; the first word reached through `+0xE0` is also an
in-range system-pointer-like word in every sample. Scans at strides 4–64 found one initial such word there and none
at the other three pointees. This does not establish an array or count. The scan collects candidate targets from
every aligned word in the system segment: coincidental data can match the pointer encoding. Its gaps therefore
neither establish allocation boundaries nor provide guaranteed size bounds. Evidence:
`verification-results/results/20260929-cb1b81f-wbd-nested/`. Next compare constructor/fixup code using this vtable
against the measured fields, then test any resulting layout across original resources.

## Empty encrypted archives (2026-09-29)

**VERIFIED OFFLINE header decode:** the five recurring errors are `pc/data/maps/generic/lodcull_j.img`,
`lodcull_m.img`, `pc/data/maps/interiors/test/interiors.img`, `pc/data/maps/leveldes/levelmap.img`, and
`TLAD/pc/data/maps/interiors/int_test.img`. Each is 2,048 bytes. Using the existing owner-executable key finder
and 16-round AES decryptor, Windows decoded all five headers as magic `0xA94E2A52`, version 3, entry count 0
and table size 0. The reader attempted a zero-length `TransformBlock`, which fails on .NET Framework.
The guard built at `0d849f0`, content selftest passed 360/360, and the WBD probe reopened the five originals with
zero archive errors. No resource entries were added to the bounds corpus by opening empty archives.
Header-decode evidence was captured by the Windows verification chat in turn
`01a0ee95-20ab-76b0-9a98-670c0267478f`, command `exec-4a3965b7-ff4a-4813-8e18-87dc053aaba8`.
This explains the five errors; it does not expand the measured bounds corpus or decode any shape.

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

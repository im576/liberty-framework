# Stage 1 reference mods — primary author pages

Date: **2026-09-29, America/Los_Angeles**. Separate reference deliverable; no mod downloaded, installed, or bundled.

**VERIFIED OFFLINE:** Audio/frontend evidence belongs to [T-053](WeaponAudio.md) and [T-055](Frontend.md). The explicitly requested author-page browsing is the exception to offline engine research. **PLAUSIBLE** below means an author-page assertion verified as page content, without local archive/runtime validation. **UNKNOWN** means an absent or unestablished license, technical detail, or compatibility result. An author page is not game-test evidence.

## GTA 4 Sound Overhaul V1

- **PLAUSIBLE — primary page:** [Troph1hunter, Nexus mod 14](https://www.nexusmods.com/gta4/mods/14), version 1.0, upload/update 2014-10-30. Lists revamped ENGINE_RESIDENT, EXPLOSIONS, WATER, WEAPONS and HELICOPTERS files; describes placement under `pc/audio/Sfx`.
- **PLAUSIBLE — exact rights notice:** Author explicitly reserves all rights. No reuse grant is shown. **UNKNOWN:** Separate asset provenance, source availability and any additional archive terms. Reference only; do not incorporate audio.
- **UNKNOWN:** Supported patch numbers, CE 1.2.0.59, loader requirements and exact archive entry layout. The page predates CE and does not establish current compatibility.
- **PLAUSIBLE — project implication:** Its WEAPONS target could compete with the existing weapon-bank override; the other named banks broaden the conflict surface. A whole-bank overwrite cannot be assumed composable. It is replacement prior art, not evidence for independent indoor/outdoor tails or a new-sample registration API. [Author page](https://www.nexusmods.com/gta4/mods/14).

## Robbery v1.0

- **PLAUSIBLE — primary page:** [Callum5042, Nexus mod 12](https://www.nexusmods.com/gta4/mods/12?tab=description), v1.0, 2014-07-28. Author describes a C# ScriptHookDotNet mod built with Visual Studio 2012: target a pedestrian and press M, with one wanted star if police are nearby. It installs `Robbery.net.dll` in `scripts` and requires GTAIV .Net ScriptHook.
- **UNKNOWN:** The page's author's-instructions section exposes no explicit reuse license, and source availability is unestablished. Do not copy code; public availability of a DLL is not source permission.
- **UNKNOWN:** CE 1.2.0.59, Tomasak 1.7.1.8 ABI compatibility, mission/save safety and coexistence with Liberty's inventory or police reactions. Similar runtime branding alone does not prove compatibility.
- **PLAUSIBLE — design reference:** Non-lethal intimidation and police proximity are useful behavioral reference points. Reimplement only from project-owned requirements and APIs. This is outside Stage 1's full crime-overhaul scope and is not an audio/frontend dependency. [Author page](https://www.nexusmods.com/gta4/mods/12?tab=description).

## Liberty Rush

- **PLAUSIBLE — primary author record:** [internet_rob, GTAForums release topic](https://gtaforums.com/topic/979688-liberty-rush/), first release 2022-01-15. Author describes a livelier world with restored content and additional traffic scenarios. The indexed opening post restricts compatibility to GTA IV 1.0.7.0/1.0.8.0, requires a new game, supports TLAD/TBoGT, and directs users to the bundled compatibility guide for merging.
- **VERIFIED OFFLINE:** No Liberty Rush archive or source was inspected in this deliverable. **UNKNOWN:** Latest release number, archive licensing, source licensing and exact file/dependency manifest.
- **PLAUSIBLE — applicability:** The documented version restriction excludes our CE 1.2.0.59 target. Do not describe Liberty Rush as CE-compatible or recommend downgrade for this project. Scenario variety is useful reference; independently implement within Stage 1's density/performance budgets. **UNKNOWN:** A CE port's compatibility and redistribution rights.
- **PLAUSIBLE — access limitation:** Direct open of the topic returned HTTP 403; the author opening post and its compatibility bullets were available through the search index of that same primary page. No mirror or third-party merge pack is substituted for author evidence. [Primary opening post](https://gtaforums.com/topic/979688-liberty-rush/).

## Realistic Handling and Physics

- **PLAUSIBLE — primary page:** [Shturmovik2, Nexus mod 195](https://www.nexusmods.com/gta4/mods/195), version 1.1, updated 2023-07-27. Author describes reworked car/bike handling and helicopter parameters, and states testing on 1.0.7.0 and Complete Edition. Exact CE executable version and this stack remain **UNKNOWN**.
- **PLAUSIBLE — permissions:** Reupload is permitted with creator credit; modifying files or using assets requires author permission; conversion to other games is forbidden; sold-file use and Donation Points use are forbidden. These are page permissions, not an open-source license. **UNKNOWN:** Archive-specific notices and source availability. Reference only for this work.
- **UNKNOWN:** Exact modified file manifest; page defers installation to the archive README, which was not downloaded. Do not assert a particular `handling.dat` merge or runtime hook from the title alone.
- **PLAUSIBLE — compatibility concern:** Handling changes can compete with Stage 1 vehicle data/tuning and change pursuit/mission behavior; advertised CE testing does not establish interaction with FusionFix/LVS/Liberty. Keep one explicit owner of effective handling data and separately validate driving/mission behavior before adoption. [Author page and permissions](https://www.nexusmods.com/gta4/mods/195).

## Recorded questions and policy

- **UNKNOWN:** Exact permission for redistributing any of these mods' assets/source is not obtained by this research. Troph1hunter's explicit reservation and Shturmovik2's conditional permissions are retained above; the other missing grants are not interpreted as permission.
- **UNKNOWN:** Archive-level dependencies and license provenance need primary release/readme inspection before any integration decision. This is a recorded research question, not a request to the owner or author.
- **VERIFIED OFFLINE:** No proprietary samples, mod DLLs, unlicensed source or GPL implementation was copied into these deliverables. Shared `third_party`, API files and `PROJECT_STATE` are not changed; a later approved integration must record its own provenance there.

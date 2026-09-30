# T-053 / R1 — Weapon audio, CE offline spike

Date: **2026-09-29, America/Los_Angeles**. Answer: **Phase 3**. Offline findings complete enough for handoff; format writing and playback remain unverified.

Confidence convention: **VERIFIED OFFLINE** means directly observed local files, bytes, or source. **PLAUSIBLE** means an inference or an author-page assertion without local validation. **UNKNOWN** means not established. All claims below use these labels. No game was launched, installed, patched, or used to prove playback.

## Decision and its limits

- **VERIFIED OFFLINE:** A locally installed replacement bank exists and retains the base candidate's 396-entry descriptor table. Replacement of existing samples is a concrete route worth pursuing, rather than assuming an entirely new audio engine is necessary.
- **PLAUSIBLE:** An original, fixed-length replacement writer could preserve descriptor identities and offsets while substituting one synthesized report. This would initially replace an existing sound, not add a new named event or weapon-specific routing.
- **UNKNOWN:** No original writer has been implemented or validated here. Channel/codec/flag semantics, two inconsistent size records, and the mapping from a named weapon report to its sample hash are unresolved. Thus neither "works" nor "works with limits" is justified for T-053's injection requirement. Phase 3 is a readiness disposition, not proof that bank replacement is impossible or necessarily requires engine hooks.
- **UNKNOWN:** Independent reports, actions, reloads, tails, ballistic cracks, and indoor/outdoor selection for new weapons require decoded event routing or deeper engine control. Existing bank replacement does not establish these capabilities.

## Reproducible evidence

**VERIFIED OFFLINE:** The independently written [audio probe](../../tools/research/audio_t053_probe.py) opens local files read-only, inventories SFX archives/config, scans aligned empirical bank headers, checks descriptor/metadata relationships, and compares payloads in memory. It exports metadata only: [audio evidence](../../tools/research/audio_t053_evidence.json). It contains no imported community code, decryption keys, asset export, or bank writer.

```powershell
Set-Location 'C:\Users\IM576\GTAIV-Reborn-research'
python tools/research/audio_t053_probe.py --game 'C:\Games\Grand Theft Auto IV\GTAIV' --sources 'D:\GTAIV-Reborn-Tools\sources\FusionFix' > tools/research/audio_t053_evidence.json
```

**VERIFIED OFFLINE:** Executable version is `1.2.0.59`, 17,425,752 bytes, SHA-256 `08759a5516f9837920ea504436236bbab89d0826a8e4d04ff106345177b5345d`. The frontend evidence independently records this executable hash. Evidence is specific to this installation; installed overrides make it a modded input, not a vanilla reference installation.

| Local input, under `C:\Games\Grand Theft Auto IV\GTAIV` | Observed evidence |
|---|---|
| `pc\audio\sfx\resident.rpf` | **VERIFIED OFFLINE:** 85,196,800 bytes; SHA-256 `6222151567be76db72f05a2043e8c8c34ef312752b0c61a629f369375b8a438b` |
| `update\pc\audio\sfx\resident.rpf\RESIDENT\WEAPONS` | **VERIFIED OFFLINE:** 17,605,310 bytes; SHA-256 `0ee1da46f55687f6caf1547d56b50fd7a64cfcfb1c703ddfbf31979648d9e3f2` |
| Base-bank candidate extent `[0x1D02000, 0x2992800)` in `resident.rpf` | **VERIFIED OFFLINE:** 13,174,784 bytes; SHA-256 `b114df7f265e19eb679f65e7e4461182d2ae7abc6dd674bfa37b49bfa5afe1d5`; candidate identity discussed below |
| `pc\audio\config\sounds.dat15` | **VERIFIED OFFLINE:** 1,883,018 bytes; SHA-256 `d18e558b0e7ffb38d51e911d529d65fffb0a1f23752971f1d61e98f53c64ea81` |

## Archive, configuration, and bank observations

- **VERIFIED OFFLINE:** `resident.rpf` begins `RPF3`; its first five little-endian words are `0x33465052, 2048, 21, 0, 0xFFFFFFFF`. The bytes at TOC offset `0x800` do not expose readable names. The probe does not decrypt the TOC or claim a complete archive directory.
- **PLAUSIBLE:** The TOC is encrypted, consistent with the local FusionFix RPF reader. The aligned-header scan finds 18 banks fitting the uniform 32-byte metadata model; this is a filtered candidate list, not a full inventory of all resident banks.
- **VERIFIED OFFLINE:** `rpf.xml` registers `resident`, type `rpf`, alias `resident`. `driverSettings.xml` names `audio:/sfx/`, `audio:/config/rpf.xml`, and `audio:/config/waveslots.xml`. `engineSettings.xml` names logical metadata paths `sounds.dat`, `curves.dat`, `categories.dat`, `effects.dat`, `game.dat`, `speech.dat`; the on-disk versioned files include `sounds.dat15`, `game.dat16`, `curves.dat12`, `effects.dat11` and `categories.dat15`. The probe records all config file hashes and the SFX file inventory.

| `waveslots.xml` slot | Static bank | Header size / slot size |
|---|---|---|
| WEAPONS | RESIDENT\WEAPONS | **VERIFIED OFFLINE:** BANK, 20,480 / 13,174,784 bytes |
| FRONTEND_MENU | RESIDENT\FRONTEND_MENU | **VERIFIED OFFLINE:** BANK, 4,096 / 1,525,760 bytes |
| FRONTEND_GAME | RESIDENT\FRONTEND_GAME | **VERIFIED OFFLINE:** BANK, 4,096 / 4,689,920 bytes |

- **VERIFIED OFFLINE:** One aligned bank candidate at file offset `0x1D02000` exactly matches the override's first 6,364 bytes (header plus descriptors). The next uniform candidate starts at `0x2992800`; their distance exactly matches the WEAPONS XML slot size.
- **PLAUSIBLE:** That candidate is the base WEAPONS bank. Matching all descriptor identities, the header, and the declared slot extent supports the association; the encrypted RPF directory name was not independently decoded.
- **VERIFIED OFFLINE:** Base candidate and override both have 396 distinct descriptor identifiers. The same descriptor table refers to 396 distinct metadata offsets, a permutation of `0, 32, ..., 12640`; all descriptor lengths are 32. The override has 39 changed payloads, 367 unchanged payload sizes, and zero changes to metadata word +8.
- **VERIFIED OFFLINE:** The installed override is 4,430,526 bytes larger than the XML WEAPONS slot allocation. No `LoadRPF` line occurs in the inspected installed FusionFix INI.
- **UNKNOWN:** Effective runtime allocation, loader options read elsewhere, whether the override is actually consumed, and consequences of the size excess. Do not raise a slot budget or infer a crash from static size differences alone.

## Empirical layout, not a complete writer specification

**VERIFIED OFFLINE:** These reads fit both compared banks. Offsets below are bank-relative; they describe observed byte relationships, not approved engine structures.

| Offset / range | Observation |
|---|---|
| +0x00, u64 | **VERIFIED OFFLINE:** 28, the start of descriptors |
| +0x08, u64 | **VERIFIED OFFLINE:** 19,036, equal to `28 + 48 × 396` |
| +0x10 / +0x14, u32 | **VERIFIED OFFLINE:** 396 / 0; second field's general meaning **UNKNOWN** |
| +0x18, u32 | **VERIFIED OFFLINE:** 20,480; payload base used by all in-bounds ranges |
| +28, 396 records of 16 bytes | **VERIFIED OFFLINE:** u64 metadata-relative offset; u32 identifier; u32 length 32 |
| +6,364, 396 records of 32 bytes | **VERIFIED OFFLINE:** u64 payload-relative offset, then u32 identifier, byte-length-like u32, sample-count-like u32, another u32, u16 rate-like value, two bytes, final u32 |
| Metadata record +8 | **VERIFIED OFFLINE:** Equal to the descriptor identifier for every one of 396 records. It is not a demonstrated checksum; CRC32 of every payload differs. |
| Metadata record +28 | **VERIFIED OFFLINE:** 1 in all 396 records; codec/channel meaning **UNKNOWN** |

- **VERIFIED OFFLINE:** Every modeled payload range fits the compared input. Base candidate payload end is 13,173,944, leaving 840 bytes to the next candidate. Override payload end exactly equals its 17,605,310-byte file size.
- **VERIFIED OFFLINE:** Base candidate: 347 rate-like values of 32,000, 33 of 24,000, 16 of 44,100; every byte length equals twice the sample-like count. Override: 338 / 33 / 25 at those rates; only 394 of 396 satisfy that relationship.
- **PLAUSIBLE:** Most records hold little-endian 16-bit mono PCM at those rates. Sample bytes look consistent with signed PCM. No decoder, channel-layout proof, or playable original-tone bank validates this interpretation.
- **VERIFIED OFFLINE:** Override records 161 (`241b77b0`) and 162 (`4a01c37c`) each advertise 276,550 payload bytes and sample-like count 18,553 at rate-like 44,100; `2 × 18,553 = 37,106`, not 276,550. The evidence preserves their complete raw metadata fields.
- **UNKNOWN:** Whether those exceptions are stale importer counts, an encoding variant, multichannel data, or fields with different semantics. An original writer must define a supported subset and reject these cases until understood.

## Event routing and what is still missing

- **VERIFIED OFFLINE:** `sounds.dat15` contains byte strings `RESIDENT\WEAPONS`, `AUD_EVENT_HANDGUN_RELOAD_INSERT_CLIP`, `AUD_EVENT_AK47_RELOAD_COCK_PULL`, `AUD_EVENT_DEAGLE_RELOAD_INSERT_CLIP`, and `DISTANT_GUNSHOTS_GUN_PISTOL`; exact file offsets are in the audio evidence.
- **PLAUSIBLE:** Separate reload-action and distant-shot event routing exists. String presence establishes names, not graph types, sample selection, firing synchronisation, attenuation, tail mixing, or indoor/outdoor logic.
- **UNKNOWN:** The precise report hash for a chosen catalog weapon, any sound shared with other weapons, event graph/variation records, loop semantics, and new-ID registration. No native was added or guessed, and no memory/API file was changed.

## Tools, source provenance, and compatibility

- **VERIFIED OFFLINE:** Local FusionFix source commit is `cbfebd2e672c15277e33f5554281a8d49fcabd03`; local `LICENSE` is GPL-3.0. Source paths are `D:\GTAIV-Reborn-Tools\sources\FusionFix\source\rpfloader.ixx:1197–1250` and `LICENSE`. Hashes are in the audio evidence. Source inspection establishes an RPF folder-merging implementation gated by `[FILELOADER] LoadRPF` and the loader API. No implementation or cryptographic material was copied.
- **UNKNOWN:** Source checkout equivalence to the installed ASI, whose observed size is 16,022,048 bytes. Therefore source behavior is not a verified installed-loader result.
- **VERIFIED OFFLINE:** Existing repository `tools/install-weapon-pack.ps1` targets the exact WEAPONS override path. The local pack description at `D:\GTAIV-Reborn-Tools\research\rwo\Realistic Weapon Overhaul Mod\Mod description.txt` credits sounds to lenol03 and SparktimusPrime and describes OpenIV import into `resident.rpf\RESIDENT`. This is pre-existing reference material, not permission to redistribute its audio.
- **UNKNOWN:** OpenIV/SparkIV/other bank-editor license terms and original writer support were not independently established from local tool sources. No editor was downloaded or installed. Their licensing is not the blocker for an independently authored writer.
- **VERIFIED OFFLINE:** The original probe uses only Python's standard library. Local game banks are proprietary research inputs; no bank, WAV, or commercial asset is included in this deliverable. The four requested mod author-page records are separate in [ReferenceMods.md](ReferenceMods.md).
- **PLAUSIBLE:** A second replacement targeting this same WEAPONS entry will compete with the current weapon pack. Combining packs requires deliberate sample-level composition and preserved identifiers; folder existence alone proves no ordering or compatibility.

## Phase 3 handoff and recorded questions

**UNKNOWN — A1:** Establish a named, existing single weapon report → sample identifier mapping, including variants and shared users. **UNKNOWN — A2:** Establish supported PCM encoding, channel count, loop/flag semantics, and resolve the size exceptions. **UNKNOWN — A3:** Verify an original writer with unchanged-record preservation, a no-op round trip, independently decoded original-tone duration/rate/channel checks, and correct descriptor/offset/size regeneration. **UNKNOWN — A4:** Establish loader/config precedence and allocation limits on the installed baseline. **UNKNOWN — A5:** Decode event metadata before promising per-weapon tails/cracks/environment selection or new audio IDs.

**PLAUSIBLE — future procedure, not queued or run:** Once A1–A4 pass, main may prepare one quiet original test tone in a scratch bank and an isolated override with rollback. Record original/output hashes, replaced hash, all changed ranges, and the exact supported format. Queue a single-weapon close/far/outdoor/indoor/reload/disable-restore check before any game run. Success must include actual playback evidence, unaffected sounds, no duplicate reports, load logs, and restoration of the owner's existing override. No game check is queued in this scoped helper change because an original writer is not yet viable.

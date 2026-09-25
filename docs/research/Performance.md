# Performance research: where the frame time goes and how to win it back

2026-09-25, Claude. Evidence comes from the owner's logs, WER reports, the installed files, GTAIV.exe disassembly, ScriptHookDotNet.asi strings and external references (listed at the end).

## 1. Measured facts

| Fact | Evidence |
|---|---|
| Frame p50 is 15–24 ms in normal play after the script cost passes (it was 60–92 ms) | `performance` lines, 2026-09-25 09:13–09:15 |
| Each Liberty Framework script runs on its own thread (gunplay 11, combat 9, arsenal 7, holsters 8, devtools 10) | `performance_scripts` `@thread` |
| One SHDN native call costs 110–180 µs | `native_cost` |
| Our remaining per-frame cost is about 10 ms: gunplay 7–8 (setup ≈3.5 = ~20 natives, weapon ≈3, HUD ≈0.6), arsenal ≈1.5, holsters ≈1 | `performance_scripts` |
| `VirtualQuery` memory checks cost 1–5 ms each under load; fixed for pools/image data | camera phase went from 5 ms to 0.01 ms |
| The RX 570 driver has no Vulkan graphics pipeline library, so DXVK compiles shaders mid-game (hitches) | `GTAIV_d3d9.log`: "Graphics pipeline libraries not supported" |
| **GTA IV is installed on the HDD** (D: = WD10EZEX 7200 rpm); the SSD (C:, ADATA SU630) has 38.6 GB free and the game is 23.6 GB | `Get-Partition`, `Get-Volume` |
| **RAM is a single 8 GB DDR4-3000 stick (single channel)** | `Win32_PhysicalMemory`: 1 module, `P0 CHANNEL A` |
| Windows power plan: Balanced | `powercfg` |
| Implicit Vulkan layers injected into the 32-bit game: EOS overlay, Steam overlay, Steam Fossilize | `HKLM\SOFTWARE\WOW6432Node\Khronos\Vulkan\ImplicitLayers` |
| Startup crashes are a Rockstar `MTLX.DLL` issue since 09-20, independent of renderer and mods | WER archive (see T-026) |

## 2. How SHDN costs us frame time (reverse-engineered)

ScriptHookDotNet.asi contains `NativeThread`, `GtaThread`/`scrThread`, `SyncBackNativeCall`, `ScheduleForRemoteProcessingThread`, `bJustWaitingForCommand`, `BlockWait` and `EventWaitHandle`, and no fiber APIs. Reading those names together (my interpretation):

- Every SHDN script runs on its own managed thread.
- Natives cannot run there, so each `Function.Call` is queued to the game's script thread (`GtaThread`, on the main thread), which executes it and signals back.
- While a script ticks, the game's thread waits for its commands.

This explains 110–180 µs per native (two thread handoffs), and why our tick time shows up in frame time.

The natives themselves are trivial. Disassembly of the CE handlers:

- `GET_CHAR_HEALTH` (0xB9EE50) → worker 0xBA6A40: `pedPool.GetAt(handle)`, then ped vfunc `+0xFC`, then float→int.
- `IS_CHAR_DEAD` (0xB9F730) → 0xBA8320: `GetAt`, then one check function.
- `DOES_CHAR_EXIST` (0xB9E9A0) → 0xBA5DA0: `GetAt`.
- `HAS_CHAR_BEEN_DAMAGED_BY_CHAR` (0xB9F680) → 0xBA8070.

The handler ABI is `handler(ctx)`, where `ctx+0` points to the return value and `ctx+8` points to the arguments (also seen in `STOP_PTFX`). So a hot getter is about 1 µs of engine work behind 150 µs of transport.

## 3. What to do, ranked by gain ÷ risk

### A. Owner-side changes (no code, biggest wins)

1. **Move GTA IV to the SSD.** In Steam: Settings → Storage, add a library on C:, then GTA IV → Properties → Installed Files → Move install folder. GTA IV streams the city continuously from disk. On a 7200 rpm HDD that causes texture and LOD pop-in, hitches while driving, and slow loads (likely part of the "buildings flicker"). The game fits: 23.6 GB needed, 38.6 GB free. Afterwards our installers take the new `-GameDirectory`.
2. **Add a second 8 GB DDR4 stick (dual channel).** Ryzen CPUs lose up to 20–25 % in CPU-bound games on one channel, and GTA IV is CPU-bound. Match 3000 MHz and put it in the second channel slot (usually A2/B2 per the board manual). This also takes the system from 8 to 16 GB; Windows, Steam, the Rockstar launcher and the game with DXVK are tight in 8.
3. **Graphics settings that cost CPU, not GPU, in GTA IV:**
   - **Night Shadows:** off (up to 30 %).
   - **Detail distance:** about 30–50.
   - **View distance:** about 30–50.
   - **Vehicle density:** about 50.
   - **Shadows:** High, not Very High (the RX 570 DXVK report is 40–45 → 80 FPS).
   - **FusionFix `ExtraDynamicShadows`:** 1 or 0 instead of 2.

   Change one at a time and compare the `performance` lines.
4. **Windows power plan:** High performance, or AMD Ryzen Balanced. Balanced lets the 2300X park and down-clock cores between frames.
5. **Steam:**
   - Disable Shader Pre-caching (the Fossilize layer), as recommended for DXVK.
   - Disable the in-game overlay for GTA IV; it is also next to the startup crash.
   - If Epic Games isn't used while playing, set the user environment variable `EOS_OVERLAY_DISABLE_VULKAN_WIN32=1`. This stops the EOS overlay layer from loading into GTA IV.

### B. DXVK and renderer (keeps Vulkan and Violent Liberty)

6. **dxvk-gplasync 2.6.2** (Nexus GTA IV mod 385). The same 2.6.2 base Violent Liberty validates, plus asynchronous shader compilation and a persistent cache. It targets exactly the missing-GPL stutter on this card.
   - Needs the owner's download.
   - Test Violent Liberty's renderer path afterwards; it may pick its "unknown DXVK" fallback.
   - Keep the stock `vulkan.dll` backup.
7. **`dxvk.conf` in the game folder,** tested one line at a time: `d3d9.maxFrameLatency = 1`, `d3d9.numBackBuffers = 3`; with gplasync also `dxvk.enableAsync = true` and `dxvk.gplAsyncCache = true`.

### C. Framework architecture (our code; required before building "a lot more")

8. **Fast direct natives.**
   - Call whitelisted read-only native handlers directly from our thread (build the ctx, call the handler address from `CodeScanner.FindNative`), or call the worker/vfunc they wrap (health = ped vfunc +0xFC).
   - Validate each one at startup against the SHDN result; fall back per native.
   - Expected: 150 µs → about 1 µs per call. Our ~10 ms per frame would drop under 1 ms.
   - Safe only because the game thread is parked while our tick runs, so this must be proven first (item 11).
9. **One script, one world snapshot.**
   - Replace the 5 SHDN scripts with one `LibertyCore` script: one thread, one tick.
   - Each tick, build a shared snapshot from memory (player ped, weapon, clip, camera, and nearby peds from the ped pool with position, health and dead state).
   - Gunplay, gore, Arsenal and holsters become modules that read the snapshot.
   - Today each script re-asks the same questions (who is the player, which weapon) with its own natives.
10. **Frame budget scheduler.** Each module declares a per-frame budget in µs. Periodic work (inventory scans, holster checks, config polling) is time-sliced across frames; nothing heavy runs on the same frame. This scales as features are added.
11. **Measure whether we stall the game.** Read the engine's frame counter at tick start and end. If it doesn't advance during our tick, the game thread is parked (confirms item 8's safety). If it does, our ticks run in parallel and cost frame time only through the handoffs.
12. **Event hooks instead of polling.**
   - Use the proven `SkeletonCollapseEngine` call-site technique on the engine's ped damage/impact path, as Violent Liberty does ("blood impact", "ped update" hook labels).
   - A native stub writes (ped, bone, weapon, **exact impact position**, damage) into a ring buffer; C# drains it once per tick.
   - Removes the damage scan entirely, and gives gore the exact bullet position (wound placement, correct cut side).
13. **Adaptive density governor.** GTA IV's CPU load scales with peds and cars. When frame p95 exceeds a target, lower `SET_PED_DENSITY_MULTIPLIER` / `SET_CAR_DENSITY_MULTIPLIER` (per-frame natives) smoothly, and raise them when there is headroom. Players rarely notice a 20 % lower density; they notice stutter.
14. **Native C++ core (later).** If C# overhead still matters after items 8–10, move the hooks, snapshot and scheduler into our own ASI. Keep gameplay logic in C#.

## 4. Suggested order

- **Now, owner:** items A1 (SSD) and A3–A5 (settings), then one measured session.
- **Next build:**
  - item 11 (proof), then items 8 and 9 (fast natives + single script with snapshot), with CostMeter before/after;
  - item 13 (density governor) behind a config switch.
- **Then:** item 6 (gplasync, owner download), item 12 (damage event hook, which also upgrades gore), item 10 as features grow.

## References

- [Gillian's GTA IV Modding Guide: Optimization](https://gillian-guide.github.io/optimization/) (DXVK, dxvk.conf, async fork, Steam pre-caching)
- [Steam discussion: poor performance with latest FusionFix](https://steamcommunity.com/app/12210/discussions/0/601915789280503901/) (night shadows, detail/view distance)
- [DXVK issue #3267](https://github.com/doitsujin/dxvk/issues/3267) (RX 570 shadows Very High vs High)
- [DXVK issue #4000](https://github.com/doitsujin/dxvk/issues/4000) (GTA IV DXVK stutter)
- [DXVK GPLAsync 2.6.2 for GTA IV, Nexus 385](https://www.nexusmods.com/gta4/mods/385)
- [TechSpot: single stick vs dual channel](https://www.techspot.com/article/3066-single-stick-vs-dual-channel-ram/), [UltrabookReview: Ryzen single vs dual channel](https://www.ultrabookreview.com/36308-single-dual-channel-ram-amd/)
- [FusionFix](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix)

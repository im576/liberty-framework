using System;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.Verify
{
    // Expected values were read independently from a Capstone disassembly of the same
    // GTAIV.exe 1.2.0.59 (preferred base 0x400000). The runtime resolver must reproduce them.
    internal static class AddressChecks
    {
        internal static void Run(string exePath, Checker check)
        {
            ImageFileMemory memory = new ImageFileMemory(exePath);
            CodeScanner scanner = new CodeScanner(memory);
            check.True("native table populated (>2500 natives)", scanner.NativeCount > 2500, "count=" + scanner.NativeCount);
            GameAddresses addresses = GameAddresses.Resolve(scanner);
            foreach (string line in addresses.Report) { Console.WriteLine("    resolver: " + line); }

            check.Equal("PREF_AUTO_AIM", 0x1160C68u, addresses.PrefAutoAim);
            check.Equal("PREF_RETICULE", 0x1160C90u, addresses.PrefReticule);
            check.Equal("CPlayerInfo array", 0x11A8808u, addresses.PlayerInfoArray);
            check.Equal("player info count", 32, addresses.PlayerInfoCount);
            check.Equal("player ped offset", 0x598, addresses.PlayerPedOffset);
            check.Equal("ped targeting flags offset", 0x264, addresses.PedTargetFlagsOffset);
            check.Equal("lock-on disabled mask", 0x800000u, addresses.LockOnDisabledMask);
            check.Equal("camera pool global", 0x12FB1A0u, addresses.CamPoolGlobal);
            check.Equal("CCam::FindChild", 0xA7C740u, addresses.FindChildCamFunction);
            check.Equal("aim camera type", 9, addresses.AimCamType);
            check.Equal("aim camera pitch offset", 0x218, addresses.AimCamPitchOffset);
            check.Equal("aim camera heading offset", 0x21C, addresses.AimCamHeadingOffset);
            check.Equal("vehicle camera type", 2, addresses.VehicleCamType);
            check.Equal("vehicle camera pitch offset", 0x190, addresses.VehicleCamPitchOffset);
            check.Equal("vehicle camera heading offset", 0x194, addresses.VehicleCamHeadingOffset);
            check.Equal("weapon info array (vanilla, FusionFix relocates at runtime)", 0x15F8BC0u, addresses.WeaponInfoArray);
            check.Equal("weapon info count (vanilla)", 60, addresses.WeaponInfoCount);
            check.Equal("weapon info stride", 0x110, addresses.WeaponInfoStride);
            check.Equal("CWeaponInfo accuracy offset", 0x34, addresses.AccuracyOffset);
            check.Equal("CWeaponInfo flags offset", 0x20, addresses.AccuracyFlagsOffset);
            check.Equal("alternate accuracy flag", 0x8u, addresses.AccuracyAlternateFlag);
            check.Equal("alternate accuracy offset", 0x38, addresses.AccuracyAlternateOffset);

            // FusionFix ExtendedLimits NOPs the getter's bound check (limits.ixx); the resolver must still work.
            ImageFileMemory patched = new ImageFileMemory(exePath);
            patched.Patch(0xAF65D7, new byte[] { 0x90, 0x90 });
            GameAddresses patchedAddresses = GameAddresses.Resolve(new CodeScanner(patched));
            check.Equal("weapon info array with FusionFix NOP", 0x15F8BC0u, patchedAddresses.WeaponInfoArray);
            check.Equal("weapon info count with FusionFix NOP (unbounded)", 0, patchedAddresses.WeaponInfoCount);
            check.True("weapon info resolved with FusionFix NOP", patchedAddresses.WeaponInfoResolved, "");

            check.Equal("aim settle timer offset", 0xEA0, addresses.AimSettleTimerOffset);
            check.Equal("aim settle snapshot offset", 0xEA4, addresses.AimSettleSnapshotOffset);
            check.Equal("aim settle window global (500 ms)", 0x1046A78u, addresses.AimSettleWindowGlobal);
            check.Near("aim settle window value", 500.0, memory.ReadSingle(addresses.AimSettleWindowGlobal), 1e-6);

            check.Equal("bullet count global", 0x15F8BB0u, addresses.BulletCountGlobal);
            check.Equal("bullet array global", 0x15F8BB8u, addresses.BulletArrayGlobal);
            check.Equal("bullet stride", 0x30, addresses.BulletStride);
            check.Equal("bullet owner offset", 0x20, addresses.BulletOwnerOffset);
            check.Equal("bullet maximum", 50, addresses.BulletMaximum);
            check.Equal("HUD component array", 0x118E7F8u, addresses.HudComponentArray);
            check.Equal("HUD reticle components", 4, addresses.ReticleComponents.Count);
            if (addresses.ReticleComponents.Count == 4)
            {
                check.Equal("crosshair index global", 0x118EE30u, addresses.ReticleComponents[0].IndexGlobal);
                check.Equal("crosshair alpha global", 0x118EE44u, addresses.ReticleComponents[0].AlphaGlobal);
                check.Equal("crosshair size global", 0x118EE3Cu, addresses.ReticleComponents[0].SizeGlobal);
                check.Equal("health target index global", 0x118EE4Cu, addresses.ReticleComponents[1].IndexGlobal);
                check.Equal("health target size global", 0x118EE58u, addresses.ReticleComponents[1].SizeGlobal);
                check.Equal("armour target index global", 0x118EE68u, addresses.ReticleComponents[2].IndexGlobal);
                check.Equal("dot index global", 0x118EE84u, addresses.ReticleComponents[3].IndexGlobal);
                check.Equal("dot size global", 0x118EE90u, addresses.ReticleComponents[3].SizeGlobal);
            }

            // T-015: aim-camera settings table (independently read from the Capstone disassembly of 0xA26F9D/0xA2774B/0xA25230).
            check.Equal("aim camera settings table", 0x103C118u, addresses.AimCamSettingsTable);
            check.Equal("aim camera settings records", 15, addresses.AimCamSettingsCount);
            check.Equal("aim camera lateral offset field", 0x10, addresses.AimCamLateralOffset);
            check.Near("on-foot aim lateral offset is the right shoulder (0.475 m)", 0.475, memory.ReadSingle(0x103C118u + 0x10), 1e-6);
            check.Near("cover aim lateral offset (0.2 m)", 0.2, memory.ReadSingle(0x103C118u + 3 * 40 + 0x10), 1e-6);

            // T-022: ped skeleton access (Capstone: EXPLODE_CHAR_HEAD worker 0xBA5F20, GET_PED_BONE_POSITION worker 0xBA75B0,
            // CPed::CopyBoneMatrix 0x9E70B0, CPed::BoneMatrix 0x9E74E0 "shl eax,6; add eax,[ecx+14h]", identity scratch 0x1632C20).
            check.Equal("ped pool global", 0x18B6F1Cu, addresses.PedPoolGlobal);
            check.Equal("CPed::CopyBoneMatrix", 0x9E70B0u, addresses.BoneMatrixCopyFunction);
            check.Equal("CPed::BoneMatrix", 0x9E74E0u, addresses.BoneMatrixPointerFunction);
            check.Equal("bone matrix identity scratch", 0x1632C20u, addresses.BoneScratchMatrix);
            check.True("ped skeleton resolved", addresses.PedSkeletonResolved, "");
            // ADR-0005 hook targets (Capstone: fragInst skeleton sync 0x5F7D70, called from 0x61183C; fragInst pose 0x5F6FB0,
            // referenced by four vtables). Both thiscall, no arguments, single plain ret.
            check.Equal("fragInst skeleton sync", 0x5F7D70u, addresses.FragSkeletonSyncFunction);
            check.Equal("fragInst pose", 0x5F6FB0u, addresses.FragPoseFunction);
            byte[] sync = memory.Read(0x5F7D70u, 11);
            byte[] pose = memory.Read(0x5F6FB0u, 12);
            check.True("sync entry is the 11 stolen bytes the hook expects", sync[0] == 0x81 && sync[1] == 0xEC && sync[6] == 0xA1, BitConverter.ToString(sync));
            // crSkeleton::Update (Capstone: 10 E8 callers of 0x466BE0; body encrypted on disk). In-place call sites:
            // 0x5F6E35, 0x5F6F84, 0x60BA64, 0x60BA79, 0x698769, 0x698830, 0x879D0E, 0x87AF87 (0x876F3B / 0xC1AB3C write elsewhere).
            // T-026 (Capstone: GET_FRAME_COUNT 0xB8B060 -> 0xB8B120 "mov eax,[1173604h]; ret"; GET_CHAR_HEALTH 0xB9EE50).
            check.Equal("engine frame counter global", 0x1173604u, addresses.FrameCounterGlobal);
            check.Equal("GET_CHAR_HEALTH handler", 0xB9EE50u, addresses.GetCharHealthHandler);
            // ADR-0006 core v2: DOES_VEHICLE_EXIST / DOES_OBJECT_EXIST handler -> worker "mov ecx,[pool]; push ebx; ..." (Capstone).
            check.Equal("vehicle pool global", 0x12E22A4u, addresses.VehiclePoolGlobal);
            check.Equal("object pool global", 0x1632C60u, addresses.ObjectPoolGlobal);
            // ADR-0007: damage-response routine (only real writer of [ped+0xA78]) and the component-to-bone helper (Capstone).
            check.Equal("ped damage-response routine", 0xCA3820u, addresses.DamageResponseFunction);
            // Raycast: the general line test (Capstone: GET_GROUND_Z_FOR_3D_COORD -> A523F0 -> A54510 -> 738880 on [world]).
            check.Equal("line test function", 0xA536B0u, addresses.LineTestFunction);
            check.Equal("physics world global", 0x12B9C78u, addresses.PhysicsWorldGlobal);
            check.Equal("component-to-bone helper", 0xA76700u, addresses.ComponentToBoneFunction);
            // T-026 step 2: every direct native is registered; handlers match the Capstone thread-safety scan (all clean).
            System.Collections.Generic.Dictionary<string, uint> scanned = new System.Collections.Generic.Dictionary<string, uint> {
                { "IS_PLAYER_PLAYING", 0xBB2530 }, { "GET_PLAYER_ID", 0xBB1F30 }, { "IS_CHAR_DUCKING", 0xB9F750 }, { "IS_PED_IN_COVER", 0xBA0150 },
                { "IS_CHAR_IN_ANY_CAR", 0xB9F970 }, { "IS_CHAR_IN_AIR", 0xB9F840 }, { "GET_CHAR_SPEED", 0xB9F030 }, { "IS_PAUSE_MENU_ACTIVE", 0xB8CD30 },
                { "IS_SCREEN_FADED_OUT", 0xB86F10 }, { "IS_PLAYER_CONTROL_ON", 0xBB23F0 }, { "GET_CAM_FOV", 0xB86B30 }, { "GET_CHAR_HEALTH", 0xB9EE50 },
                { "IS_CHAR_DEAD", 0xB9F730 }, { "DOES_CHAR_EXIST", 0xB9E9A0 }, { "GET_CURRENT_CHAR_WEAPON", 0xBD0E40 }, { "GET_AMMO_IN_CLIP", 0xBD0DF0 },
                { "GET_MAX_AMMO_IN_CLIP", 0xBD0F30 }, { "HAS_CHAR_BEEN_DAMAGED_BY_CHAR", 0xB9F680 } };
            foreach (System.Collections.Generic.KeyValuePair<string, uint> native in LibertyFramework.GameApi.DirectNatives.Hashes)
            {
                uint handler = scanner.FindNative(native.Value);
                uint expected;
                check.True("direct native handler matches the thread-safety scan: " + native.Key, scanned.TryGetValue(native.Key, out expected) && handler == expected,
                    "handler=0x" + handler.ToString("X8"));
            }
            check.Equal("crSkeleton::Update", 0x466BE0u, addresses.SkeletonUpdateFunction);
            check.Equal("in-place skeleton update call sites", 8, addresses.SkeletonUpdateCallSites.Count);
            check.True("update call sites are the eight Capstone-verified ones", addresses.SkeletonUpdateCallSites.Contains(0x5F6E35u) &&
                addresses.SkeletonUpdateCallSites.Contains(0x60BA79u) && addresses.SkeletonUpdateCallSites.Contains(0x87AF87u) &&
                !addresses.SkeletonUpdateCallSites.Contains(0x876F3Bu) && !addresses.SkeletonUpdateCallSites.Contains(0xC1AB3Cu), "");
            check.True("pose entry is the 12 stolen bytes the hook expects", pose[0] == 0x55 && pose[3] == 0x83 && pose[6] == 0x81 && pose[11] == 0x00, BitConverter.ToString(pose));
        }
    }
}

using System;
using System.Collections.Generic;

namespace LibertyFramework.Core.Memory
{
    // Resolves every engine location Liberty Framework touches on GTAIV.exe 1.2.0.59.
    // Each resolver anchors on a script-native hash or a unique instruction shape, checks
    // the surrounding bytes, and fails closed: a failed group only disables its feature.
    // Derivations are documented in docs/game-api/MEMORY.md; tools/verify runs this same
    // code against GTAIV.exe on disk and compares the results with the offline disassembly.
    internal sealed class GameAddresses
    {
        // Script native hashes (Jenkins one-at-a-time of the native name), as registered by the game.
        internal const uint HashDisablePlayerLockon = 0x711214F3;
        internal const uint HashIsAutoAimingOn = 0x366B0444;
        internal const uint HashIsHudReticuleComplex = 0x4DDB5D59;
        internal const uint HashGetRootCam = 0x75E005F1;
        internal const uint HashIsBulletInArea = 0x58493B8E;

        internal readonly List<string> Report = new List<string>();

        // Menu preferences (int32 each).
        internal uint PrefAutoAim;
        internal uint PrefReticule;

        // Player lock-on flag written by DISABLE_PLAYER_LOCKON.
        internal uint PlayerInfoArray;
        internal int PlayerInfoCount;
        internal int PlayerPedOffset;
        internal int PedTargetFlagsOffset;
        internal uint LockOnDisabledMask;

        // Camera pool + CCam::FindChild(type, index) used by SET_GAME_CAM_PITCH/HEADING.
        internal uint CamPoolGlobal;
        internal uint FindChildCamFunction;
        internal int AimCamType;
        internal int AimCamPitchOffset;
        internal int AimCamHeadingOffset;

        // CWeaponInfo table (patched by FusionFix ExtendedLimits at runtime; read from code).
        internal uint WeaponInfoArray;
        internal int WeaponInfoCount;
        internal int WeaponInfoStride;
        internal int AccuracyOffset;
        internal int AccuracyFlagsOffset;
        internal uint AccuracyAlternateFlag;
        internal int AccuracyAlternateOffset;

        // Per-frame bullet trace list used by IS_BULLET_IN_AREA.
        internal uint BulletCountGlobal;
        internal uint BulletArrayGlobal;
        internal int BulletStride;
        internal int BulletOwnerOffset;
        internal int BulletMaximum;

        // hud.dat reticle components.
        internal uint HudComponentArray;
        internal readonly List<HudComponentGlobals> ReticleComponents = new List<HudComponentGlobals>();

        internal bool PrefsResolved { get { return PrefAutoAim != 0 && PrefReticule != 0; } }
        internal bool LockOnResolved { get { return PlayerInfoArray != 0 && PlayerPedOffset > 0 && PedTargetFlagsOffset > 0 && LockOnDisabledMask != 0; } }
        internal bool AimCameraResolved { get { return CamPoolGlobal != 0 && FindChildCamFunction != 0 && AimCamPitchOffset > 0 && AimCamHeadingOffset > 0; } }
        internal bool WeaponInfoResolved { get { return WeaponInfoArray != 0 && WeaponInfoStride > 0 && AccuracyOffset > 0; } }
        internal bool BulletsResolved { get { return BulletCountGlobal != 0 && BulletArrayGlobal != 0 && BulletStride > 0; } }
        internal bool HudResolved { get { return HudComponentArray != 0 && ReticleComponents.Count == 4; } }

        internal sealed class HudComponentGlobals
        {
            internal string Name;
            internal uint IndexGlobal;
            internal uint PositionGlobal;
            internal uint SizeGlobal;
            internal uint AlphaGlobal;
            internal uint ColourGlobal;
        }

        internal static GameAddresses Resolve(CodeScanner scanner)
        {
            GameAddresses result = new GameAddresses();
            result.Run("prefs", scanner, result.ResolvePrefs);
            result.Run("lockon", scanner, result.ResolveLockOn);
            result.Run("aim_camera", scanner, result.ResolveAimCamera);
            result.Run("weapon_info", scanner, result.ResolveWeaponInfo);
            result.Run("bullets", scanner, result.ResolveBullets);
            result.Run("hud_reticle", scanner, result.ResolveHud);
            return result;
        }

        private void Run(string name, CodeScanner scanner, Action<CodeScanner> resolver)
        {
            try
            {
                resolver(scanner);
            }
            catch (Exception error)
            {
                Report.Add(name + " FAILED: " + error.Message);
            }
        }

        private static uint RequireNative(CodeScanner scanner, uint hash, string name)
        {
            uint function = scanner.FindNative(hash);
            if (function == 0) { throw new InvalidOperationException("native " + name + " not registered"); }
            return function;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) { throw new InvalidOperationException(message); }
        }

        // IS_AUTO_AIMING_ON / IS_HUD_RETICULE_COMPLEX: call helper; helper is "cmp dword [pref], 0; setne al; ret".
        private void ResolvePrefs(CodeScanner scanner)
        {
            PrefAutoAim = ResolvePrefFromNative(scanner, HashIsAutoAimingOn, "IS_AUTO_AIMING_ON");
            PrefReticule = ResolvePrefFromNative(scanner, HashIsHudReticuleComplex, "IS_HUD_RETICULE_COMPLEX");
            Require(PrefReticule - PrefAutoAim == 40, "PREF_AUTO_AIM (8) and PREF_RETICULE (18) are not 10 slots apart");
            Report.Add("prefs ok auto_aim=0x" + PrefAutoAim.ToString("X8") + " reticule=0x" + PrefReticule.ToString("X8"));
        }

        private static uint ResolvePrefFromNative(CodeScanner scanner, uint hash, string name)
        {
            IMemory memory = scanner.Memory;
            uint native = RequireNative(scanner, hash, name);
            Require(scanner.ShapeAt(native, "E8 ?? ?? ?? ?? 0F B6 C8"), name + " shape");
            uint helper = memory.RelativeTarget(native);
            Require(scanner.ShapeAt(helper, "83 3D ?? ?? ?? ?? 00 0F 95 C0 C3"), name + " helper shape");
            return memory.ReadUInt32(helper + 2);
        }

        // DISABLE_PLAYER_LOCKON -> helper(player, flag): playerInfo = table[player]; ped = [info+X];
        // ped[Y] bit 23 = lock-on disabled. The same bit is tested by the player targeting code.
        private void ResolveLockOn(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            uint native = RequireNative(scanner, HashDisablePlayerLockon, "DISABLE_PLAYER_LOCKON");
            Require(scanner.ShapeAt(native, "8B 44 24 04 8B 40 08 83 78 04 00 0F 95 44 24 04 FF 74 24 04 FF 30 E8"), "native shape");
            uint helper = memory.RelativeTarget(native + 22);
            Require(scanner.ShapeAt(helper, "FF 74 24 04 E8 ?? ?? ?? ?? 8B 88 ?? ?? ?? ?? 0F B6 44 24 0C C1 E0 ?? 33 81 ?? ?? ?? ?? 83 C4 04 25 ?? ?? ?? ?? 31 81"),
                "helper shape");
            uint getPlayerInfo = memory.RelativeTarget(helper + 4);
            Require(scanner.ShapeAt(getPlayerInfo, "8B 44 24 04 83 F8 ?? 77 ?? 8B 04 85 ?? ?? ?? ?? C3"), "player info getter shape");
            PlayerInfoCount = memory.ReadByte(getPlayerInfo + 6) + 1;
            PlayerInfoArray = memory.ReadUInt32(getPlayerInfo + 12);
            PlayerPedOffset = memory.ReadInt32(helper + 11);
            int shift = memory.ReadByte(helper + 22);
            PedTargetFlagsOffset = memory.ReadInt32(helper + 25);
            LockOnDisabledMask = memory.ReadUInt32(helper + 33);
            Require(LockOnDisabledMask == (1u << shift), "mask/shift mismatch");
            Report.Add("lockon ok player_info=0x" + PlayerInfoArray.ToString("X8") + " ped_offset=0x" + PlayerPedOffset.ToString("X") +
                " flags_offset=0x" + PedTargetFlagsOffset.ToString("X") + " mask=0x" + LockOnDisabledMask.ToString("X"));
        }

        // GET_ROOT_CAM -> "push [root]; mov ecx,[camPool]; call GetIndex". SET_GAME_CAM_PITCH's worker walks the
        // game camera tree with FindChild(type 9) and writes the aim camera pitch/heading fields.
        private void ResolveAimCamera(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            uint native = RequireNative(scanner, HashGetRootCam, "GET_ROOT_CAM");
            Require(scanner.ShapeAt(native, "8B 44 24 04 8B 40 08 FF 30 E8"), "GET_ROOT_CAM shape");
            uint wrapper = memory.RelativeTarget(native + 9);
            Require(scanner.ShapeAt(wrapper, "FF 74 24 04 B9 ?? ?? ?? ?? E8"), "root wrapper shape");
            uint getter = memory.RelativeTarget(wrapper + 9);
            Require(scanner.ShapeAt(getter, "FF 35 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? E8"), "root getter shape");
            CamPoolGlobal = memory.ReadUInt32(getter + 8);

            List<uint> sites = scanner.FindPattern("6A 00 6A 09 8B CF E8 ?? ?? ?? ?? 8B F0 85 F6", true);
            Require(sites.Count == 1, "aim camera tree walk site count=" + sites.Count);
            uint site = sites[0];
            AimCamType = memory.ReadByte(site + 3);
            FindChildCamFunction = memory.RelativeTarget(site + 6);
            Require(scanner.ShapeAt(FindChildCamFunction, "56 8B F1 57 8B 06 FF 50 28 8B 7C 24 0C 3B C7 75 16"), "FindChild shape");

            // Collect "movss [esi+disp32], xmm0/xmm1" stores until the next child lookup (type 2).
            List<int> stores = new List<int>();
            byte[] body = memory.Read(site, 0x160);
            for (int offset = 11; offset + 8 <= body.Length; offset++)
            {
                if (body[offset] == 0x6A && body[offset + 1] == 0x00 && body[offset + 2] == 0x6A && body[offset + 3] == 0x02) { break; }
                if (body[offset] == 0xF3 && body[offset + 1] == 0x0F && body[offset + 2] == 0x11 &&
                    (body[offset + 3] == 0x86 || body[offset + 3] == 0x8E))
                {
                    stores.Add(BitConverter.ToInt32(body, offset + 4));
                }
            }
            Require(stores.Count == 3, "aim camera store count=" + stores.Count);
            Require(stores[0] == stores[1] && stores[2] == stores[0] + 4, "aim camera store layout");
            AimCamPitchOffset = stores[0];
            AimCamHeadingOffset = stores[2];
            Report.Add("aim_camera ok cam_pool=0x" + CamPoolGlobal.ToString("X8") + " find_child=0x" + FindChildCamFunction.ToString("X8") +
                " type=" + AimCamType + " pitch=0x" + AimCamPitchOffset.ToString("X") + " heading=0x" + AimCamHeadingOffset.ToString("X"));
        }

        // CWeaponInfo::Get(type): "cmp eax, count; jge; imul eax, eax, stride; add eax, array; ret".
        // FusionFix ExtendedLimits relocates the array and NOPs the jge (limits.ixx), so both forms are accepted.
        // WeaponInfoCount is 0 when the bound check was removed; callers then validate entries against WeaponInfo.xml.
        private void ResolveWeaponInfo(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            List<uint> sites = new List<uint>();
            foreach (uint candidate in scanner.FindPattern("8B 44 24 04 83 F8 ?? ?? ?? 69 C0 10 01 00 00 05 ?? ?? ?? ?? C3", true))
            {
                if (scanner.ShapeAt(candidate + 7, "7D") || scanner.ShapeAt(candidate + 7, "90 90")) { sites.Add(candidate); }
            }
            Require(sites.Count == 1, "weapon info getter count=" + sites.Count);
            uint getter = sites[0];
            bool bounded = scanner.ShapeAt(getter + 7, "7D");
            WeaponInfoCount = bounded ? memory.ReadByte(getter + 6) : 0;
            WeaponInfoStride = memory.ReadInt32(getter + 11);
            WeaponInfoArray = memory.ReadUInt32(getter + 16);
            Require(WeaponInfoStride == 0x110, "unexpected CWeaponInfo stride 0x" + WeaponInfoStride.ToString("X"));

            // CWeaponInfo::GetAccuracy(ped): reads the XML <aiming accuracy>, or an alternate value when a flag bit is set.
            List<uint> accuracy = scanner.FindPattern("83 EC 08 56 8B 74 24 10 57 8B F9 F3 0F 10 47 ?? F3 0F 11 44 24 0C F3 0F 11 44 24 08", true);
            Require(accuracy.Count == 1, "accuracy getter count=" + accuracy.Count);
            uint getAccuracy = accuracy[0];
            Require(scanner.ShapeAt(getAccuracy + 0x31, "8B 47 ?? C1 E8 ?? A8 01"), "accuracy flag shape");
            Require(scanner.ShapeAt(getAccuracy + 0x5D, "F3 0F 10 47 ??"), "alternate accuracy shape");
            AccuracyOffset = memory.ReadByte(getAccuracy + 15);
            AccuracyFlagsOffset = memory.ReadByte(getAccuracy + 0x33);
            AccuracyAlternateFlag = 1u << memory.ReadByte(getAccuracy + 0x36);
            AccuracyAlternateOffset = memory.ReadByte(getAccuracy + 0x61);
            Report.Add("weapon_info ok array=0x" + WeaponInfoArray.ToString("X8") + " count=" + WeaponInfoCount + " stride=0x" + WeaponInfoStride.ToString("X") +
                " accuracy=0x" + AccuracyOffset.ToString("X") + " flags=0x" + AccuracyFlagsOffset.ToString("X") + " alt_flag=0x" + AccuracyAlternateFlag.ToString("X") +
                " alt_accuracy=0x" + AccuracyAlternateOffset.ToString("X"));
        }

        // IS_BULLET_IN_AREA -> helper -> list scan: "mov esi,[count]; ...; mov eax,[array]; ...; add eax,18; ...cmp [eax+8],edx".
        private void ResolveBullets(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            uint native = RequireNative(scanner, HashIsBulletInArea, "IS_BULLET_IN_AREA");
            Require(scanner.ShapeAt(native + 0x42, "E8"), "IS_BULLET_IN_AREA call shape");
            uint helper = memory.RelativeTarget(native + 0x42);
            Require(scanner.ShapeAt(helper + 0x46, "E8"), "bullet helper call shape");
            uint scan = memory.RelativeTarget(helper + 0x46);
            Require(scanner.ShapeAt(scan, "56 8B 35 ?? ?? ?? ?? 33 C9 57 85 F6 0F 8E ?? ?? ?? ?? A1 ?? ?? ?? ?? 8B 54 24 14 8B 7C 24 0C F3 0F 10 35 ?? ?? ?? ?? 83 C0 ??"),
                "bullet scan shape");
            BulletCountGlobal = memory.ReadUInt32(scan + 3);
            BulletArrayGlobal = memory.ReadUInt32(scan + 19);
            int ownerBias = memory.ReadByte(scan + 41);
            Require(scanner.ShapeAt(scan + 0x10D, "41 83 C0 ?? 3B CE 0F 8C"), "bullet loop stride shape");
            BulletStride = memory.ReadByte(scan + 0x110);
            Require(scanner.ShapeAt(scan + 0x34, "39 50 ??"), "bullet owner compare shape");
            BulletOwnerOffset = ownerBias + memory.ReadByte(scan + 0x36);
            // AddBullet refuses to append once count reaches this value ("cmp eax, 32h").
            List<uint> adders = scanner.FindPattern("A1 ?? ?? ?? ?? 83 F8 ?? 7D ?? 8B 4C 24 08 8D 14 40", true);
            Require(adders.Count == 1 && memory.ReadUInt32(adders[0] + 1) == BulletCountGlobal, "bullet add shape");
            BulletMaximum = memory.ReadByte(adders[0] + 7);
            Report.Add("bullets ok count=0x" + BulletCountGlobal.ToString("X8") + " array=0x" + BulletArrayGlobal.ToString("X8") +
                " stride=0x" + BulletStride.ToString("X") + " owner=0x" + BulletOwnerOffset.ToString("X") + " max=" + BulletMaximum);
        }

        // hud.dat registration: push alpha; push colour; push 0; push &size; push &pos; push type; push name; call register.
        private void ResolveHud(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            string[] names = { "HUD_WEAPON_CROSSHAIR", "HUD_WEAPON_HEALTH_TARGET", "HUD_WEAPON_ARMOUR_TARGET", "HUD_WEAPON_DOT" };
            foreach (string name in names)
            {
                uint text = scanner.FindAsciiString(name);
                Require(text != 0, name + " string missing");
                byte[] push = new byte[5];
                push[0] = 0x68;
                BitConverter.GetBytes(text).CopyTo(push, 1);
                List<uint> sites = scanner.FindPattern(ToPattern(push), true);
                Require(sites.Count == 1, name + " push count=" + sites.Count);
                uint site = sites[0];
                Require(scanner.ShapeAt(site - 26, "FF 35 ?? ?? ?? ?? FF 35 ?? ?? ?? ?? 6A ?? 68 ?? ?? ?? ?? 68 ?? ?? ?? ?? 6A ??"), name + " registration shape");
                Require(scanner.ShapeAt(site + 5, "E8"), name + " register call");
                HudComponentGlobals component = new HudComponentGlobals();
                component.Name = name;
                component.AlphaGlobal = memory.ReadUInt32(site - 24);
                component.ColourGlobal = memory.ReadUInt32(site - 18);
                component.SizeGlobal = memory.ReadUInt32(site - 11);
                component.PositionGlobal = memory.ReadUInt32(site - 6);
                byte[] after = memory.Read(site + 10, 24);
                uint arrayAddress = 0;
                for (int offset = 0; offset + 5 <= after.Length; offset++)
                {
                    if (after[offset] == 0xA3 && component.IndexGlobal == 0) { component.IndexGlobal = BitConverter.ToUInt32(after, offset + 1); }
                    if (after[offset] == 0x8B && (after[offset + 1] == 0x0C || after[offset + 1] == 0x04) && after[offset + 2] == 0x85 && arrayAddress == 0)
                    {
                        arrayAddress = BitConverter.ToUInt32(after, offset + 3);
                    }
                }
                Require(component.IndexGlobal != 0 && arrayAddress != 0, name + " index/array not found");
                Require(HudComponentArray == 0 || HudComponentArray == arrayAddress, name + " array mismatch");
                Require(component.SizeGlobal == component.PositionGlobal + 8 && component.AlphaGlobal == component.SizeGlobal + 8,
                    name + " globals layout");
                HudComponentArray = arrayAddress;
                ReticleComponents.Add(component);
                Report.Add("hud ok " + name + " index=0x" + component.IndexGlobal.ToString("X8") + " size=0x" + component.SizeGlobal.ToString("X8") +
                    " alpha=0x" + component.AlphaGlobal.ToString("X8"));
            }
        }

        private static string ToPattern(byte[] bytes)
        {
            string[] parts = new string[bytes.Length];
            for (int index = 0; index < bytes.Length; index++) { parts[index] = bytes[index].ToString("X2"); }
            return string.Join(" ", parts);
        }
    }
}

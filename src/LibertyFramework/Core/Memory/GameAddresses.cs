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
        // CE script native hashes as registered by the game (from FusionFix's CE native list; not derivable from names).
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
        // Vehicle follow camera (type 2) written by the same worker; used for drive-by kick when validated.
        internal int VehicleCamType;
        internal int VehicleCamPitchOffset;
        internal int VehicleCamHeadingOffset;

        // CWeaponInfo table (patched by FusionFix ExtendedLimits at runtime; read from code).
        internal uint WeaponInfoArray;
        internal int WeaponInfoCount;
        internal int WeaponInfoStride;
        internal int AccuracyOffset;
        internal int AccuracyFlagsOffset;
        internal uint AccuracyAlternateFlag;
        internal int AccuracyAlternateOffset;

        // Player aim-settle timer: CPed accumulates aiming time (ms) and DoAccuracy scales every player bullet
        // offset by 1 - min(snapshot, window) / window, so a settled aim becomes perfectly accurate.
        internal int AimSettleTimerOffset;
        internal int AimSettleSnapshotOffset;
        internal uint AimSettleWindowGlobal;

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
        internal bool AimSettleResolved { get { return AimSettleTimerOffset > 0 && AimSettleSnapshotOffset == AimSettleTimerOffset + 4 && AimSettleWindowGlobal != 0; } }
        internal bool BulletsResolved { get { return BulletCountGlobal != 0 && BulletArrayGlobal != 0 && BulletStride > 0; } }
        internal bool HudResolved { get { return HudComponentArray != 0 && ReticleComponents.Count == 4; } }

        // Aim-camera settings table (T-015): records of AimCamSettingsStride bytes chosen per camera state by
        // CCamAimWeapon's update; the float at AimCamLateralOffset is multiplied by the camera right vector
        // to place the camera beside the shoulder (0.475 m on foot, 0.2 m in cover on 1.2.0.59).
        internal uint AimCamSettingsTable;
        internal int AimCamSettingsStride;
        internal int AimCamSettingsCount;
        internal int AimCamLateralOffset;
        internal bool AimCamSettingsResolved { get { return AimCamSettingsTable != 0 && AimCamSettingsStride == 40 && AimCamSettingsCount > 0 && AimCamLateralOffset > 0; } }

        // Ped skeleton access (T-022 dismemberment), from the GET_PED_BONE_POSITION implementation:
        // CPed::CopyBoneMatrix(this, Matrix34* out, int boneTag) and CPed::BoneMatrix(this, int index) which
        // returns &skeleton->objectMatrices[index] (64-byte stride) or a shared identity scratch when the ped
        // has no skeleton. The ped pool is the rage pool EXPLODE_CHAR_HEAD reads handles from.
        internal uint PedPoolGlobal;
        internal uint BoneMatrixCopyFunction;
        internal uint BoneMatrixPointerFunction;
        internal uint BoneScratchMatrix;
        internal int BoneMatrixStride;
        internal bool PedSkeletonResolved { get { return PedPoolGlobal != 0 && BoneMatrixCopyFunction != 0 && BoneMatrixPointerFunction != 0 && BoneScratchMatrix != 0 && BoneMatrixStride == 64; } }

        // ADR-0005 (T-022): the two fragInst methods that rebuild a ragdolled ped's skeleton matrices every frame
        // (thiscall, no arguments, plain ret). Dismemberment re-applies its bone collapse right after them.
        internal uint FragSkeletonSyncFunction;
        internal uint FragPoseFunction;
        internal const int FragSkeletonSyncStolenBytes = 11;
        internal const int FragPoseStolenBytes = 12;
        internal bool SkeletonHooksResolved { get { return FragSkeletonSyncFunction != 0 && FragPoseFunction != 0; } }

        // crSkeleton::Update(parentMatrix, globalMatrices) (0x466BE0 on 1.2.0.59; its body is encrypted on disk): the
        // single routine that turns local bone transforms into the global matrices that get skinned. The call sites
        // that update a skeleton in place ("push [r+14h]; push [r+8]; call", ecx = the skeleton) are hooked so the
        // dismemberment collapse lands after every pose update (ADR-0005).
        internal uint SkeletonUpdateFunction;
        internal readonly List<uint> SkeletonUpdateCallSites = new List<uint>();
        internal bool SkeletonUpdateResolved { get { return SkeletonUpdateFunction != 0 && SkeletonUpdateCallSites.Count >= 4; } }

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
            result.Run("aim_settle", scanner, result.ResolveAimSettle);
            result.Run("bullets", scanner, result.ResolveBullets);
            result.Run("hud_reticle", scanner, result.ResolveHud);
            result.Run("aim_camera_settings", scanner, result.ResolveAimCameraSettings);
            result.Run("ped_skeleton", scanner, result.ResolvePedSkeleton);
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

            // The same worker then does FindChild(type 2) and writes the vehicle camera's pitch/heading.
            int vehicleSite = -1;
            for (int offset = 11; offset + 7 <= body.Length; offset++)
            {
                if (body[offset] == 0x6A && body[offset + 1] == 0x00 && body[offset + 2] == 0x6A && body[offset + 4] == 0x8B && body[offset + 5] == 0xCF && body[offset + 6] == 0xE8)
                {
                    vehicleSite = offset;
                    break;
                }
            }
            if (vehicleSite > 0)
            {
                byte[] tail = memory.Read(site + (uint)vehicleSite, 0x180);
                List<int> vehicleStores = new List<int>();
                for (int offset = 11; offset + 8 <= tail.Length; offset++)
                {
                    if (tail[offset] == 0x5E && tail[offset + 1] == 0x5F && tail[offset + 2] == 0xC3) { break; }
                    if (tail[offset] == 0xF3 && tail[offset + 1] == 0x0F && tail[offset + 2] == 0x11 && (tail[offset + 3] == 0x86 || tail[offset + 3] == 0x8E))
                    {
                        vehicleStores.Add(BitConverter.ToInt32(tail, offset + 4));
                    }
                }
                if (vehicleStores.Count == 3 && vehicleStores[0] == vehicleStores[1] && vehicleStores[2] == vehicleStores[0] + 4 &&
                    memory.RelativeTarget(site + (uint)vehicleSite + 6) == FindChildCamFunction)
                {
                    VehicleCamType = tail[3];
                    VehicleCamPitchOffset = vehicleStores[0];
                    VehicleCamHeadingOffset = vehicleStores[2];
                }
            }
            AimCamHeadingOffset = stores[2];
            Report.Add("aim_camera ok cam_pool=0x" + CamPoolGlobal.ToString("X8") + " find_child=0x" + FindChildCamFunction.ToString("X8") +
                " type=" + AimCamType + " pitch=0x" + AimCamPitchOffset.ToString("X") + " heading=0x" + AimCamHeadingOffset.ToString("X") +
                " vehicle_type=" + VehicleCamType + " vehicle_pitch=0x" + VehicleCamPitchOffset.ToString("X") + " vehicle_heading=0x" + VehicleCamHeadingOffset.ToString("X"));
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

        // Reader in the accuracy sampler: "movd xmm0,[esi+snapshot]; movss xmm2,[window]; cvtdq2ps; comiss".
        // Writer in the ped aim update: "add [edi+timer], frameMs; ... cap 10000" and "sub [edi+timer], ...".
        private void ResolveAimSettle(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            List<uint> readers = scanner.FindPattern("66 0F 6E 86 ?? ?? ?? ?? F3 0F 10 15 ?? ?? ?? ?? 0F 5B C0 0F 2F C2", true);
            Require(readers.Count == 1, "aim settle reader count=" + readers.Count);
            AimSettleSnapshotOffset = memory.ReadInt32(readers[0] + 4);
            AimSettleWindowGlobal = memory.ReadUInt32(readers[0] + 12);
            List<uint> adders = scanner.FindPattern("01 87 ?? ?? ?? ?? 8B 87 ?? ?? ?? ?? B9 10 27 00 00", true);
            List<uint> subtractors = scanner.FindPattern("29 87 ?? ?? ?? ?? D9 6C 24 0E 79 31", true);
            Require(adders.Count == 1 && subtractors.Count == 1, "aim settle writer counts=" + adders.Count + "/" + subtractors.Count);
            AimSettleTimerOffset = memory.ReadInt32(adders[0] + 2);
            Require(memory.ReadInt32(subtractors[0] + 2) == AimSettleTimerOffset, "aim settle writers disagree");
            Require(AimSettleSnapshotOffset == AimSettleTimerOffset + 4, "aim settle snapshot is not timer+4");
            Report.Add("aim_settle ok timer=0x" + AimSettleTimerOffset.ToString("X") + " snapshot=0x" + AimSettleSnapshotOffset.ToString("X") +
                " window=0x" + AimSettleWindowGlobal.ToString("X8"));
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

        // CCamAimWeapon update picks a settings record: "add ecx,eax; lea eax,[ecx+ecx*4]; lea eax,[eax*8+table]" (stride 40),
        // later "mov eax,[esp+10h]; movss xmm1,[edi+blend]; mulss xmm1,[eax+lateral]" scales the camera right vector.
        // The pitch clamp helper "movss xmm1,[ecx+pitch]; movss xmm0,[eax+min]" proves the same table drives the aim camera.
        private void ResolveAimCameraSettings(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            List<uint> selects = scanner.FindPattern("03 C8 8D 04 89 8D 04 C5 ?? ?? ?? ?? EB", true);
            Require(selects.Count >= 1, "aim settings select count=" + selects.Count);
            uint table = memory.ReadUInt32(selects[0] + 8);
            foreach (uint site in selects) { Require(memory.ReadUInt32(site + 8) == table, "aim settings selects disagree"); }
            List<uint> lateral = scanner.FindPattern("8B 44 24 10 F3 0F 10 8F ?? ?? ?? ?? F3 0F 59 48 ??", true);
            Require(lateral.Count == 1, "aim lateral use count=" + lateral.Count);
            List<uint> clamp = scanner.FindPattern("8B 44 24 04 F3 0F 10 89 ?? ?? ?? ?? F3 0F 10 40 ?? 0F 2F C8", true);
            Require(clamp.Count == 1 && AimCamPitchOffset > 0 && memory.ReadInt32(clamp[0] + 8) == AimCamPitchOffset,
                "aim settings pitch clamp does not use the resolved aim-camera pitch field");
            int count = 0;
            for (int index = 0; index < 32; index++)
            {
                byte[] record = memory.Read(table + (uint)(index * 40), 40);
                bool empty = true;
                foreach (byte value in record) { if (value != 0) { empty = false; break; } }
                if (empty) { break; }
                count++;
            }
            Require(count > 0, "aim settings table empty");
            AimCamSettingsTable = table;
            AimCamSettingsStride = 40;
            AimCamSettingsCount = count;
            AimCamLateralOffset = memory.ReadByte(lateral[0] + 16);
            Report.Add("aim_camera_settings ok table=0x" + table.ToString("X8") + " records=" + count + " lateral=+0x" + AimCamLateralOffset.ToString("X"));
        }

        internal const uint HashExplodeCharHead = 0x4A802E89;
        internal const uint HashGetPedBonePosition = 0x43475BB3;

        private void ResolvePedSkeleton(CodeScanner scanner)
        {
            IMemory memory = scanner.Memory;
            // EXPLODE_CHAR_HEAD -> worker: "mov ecx,[pedPool]; sub esp,50h; push esi; push [esp+58h]; call pool.GetAt".
            uint explode = RequireNative(scanner, HashExplodeCharHead, "EXPLODE_CHAR_HEAD");
            Require(scanner.ShapeAt(explode, "8B 44 24 04 8B 40 08 FF 30 E8"), "EXPLODE_CHAR_HEAD shape");
            uint explodeWorker = memory.RelativeTarget(explode + 9);
            Require(scanner.ShapeAt(explodeWorker, "8B 0D ?? ?? ?? ?? 83 EC ?? 56 FF 74 24 ?? E8"), "explode worker shape");
            uint pool = memory.ReadUInt32(explodeWorker + 2);
            // GET_PED_BONE_POSITION -> worker; worker+58h: "push [ebp+0Ch]; lea ecx,[esp+..]; push ecx; mov ecx,eax; call CopyBoneMatrix".
            uint bonePosition = RequireNative(scanner, HashGetPedBonePosition, "GET_PED_BONE_POSITION");
            Require(scanner.ShapeAt(bonePosition, "FF 74 24 04 68 ?? ?? ?? ?? E8"), "GET_PED_BONE_POSITION shape");
            uint boneWorker = memory.ReadUInt32(bonePosition + 5);
            Require(scanner.ShapeAt(boneWorker, "8B 0D") && memory.ReadUInt32(boneWorker + 2) == pool ||
                scanner.ShapeAt(boneWorker + 9, "8B 0D") && memory.ReadUInt32(boneWorker + 11) == pool, "bone worker does not use the ped pool");
            Require(scanner.ShapeAt(boneWorker + 0x58, "FF 75 0C 8D 4C 24 ?? 51 8B C8 E8"), "bone worker copy call shape");
            uint copy = memory.RelativeTarget(boneWorker + 0x58 + 10);
            Require(scanner.ShapeAt(copy, "57 8B F9 8B 07 FF 90 A0 00 00 00"), "CopyBoneMatrix shape");
            Require(scanner.ShapeAt(copy + 0x62, "FF 70 04 E8 ?? ?? ?? ?? 83 C4 08 8B CF 50 E8"), "CopyBoneMatrix index/pointer calls");
            uint pointer = memory.RelativeTarget(copy + 0x62 + 14);
            Require(scanner.ShapeAt(pointer, "56 8B F1 8B 06 FF 90 A0 00 00 00"), "BoneMatrix shape");
            Require(scanner.ShapeAt(pointer + 0x35, "C7 05 ?? ?? ?? ?? 00 00 80 3F"), "BoneMatrix scratch identity");
            // Search only inside BoneMatrix: the live process decrypts extra code, so global pattern counts differ from disk.
            uint tail = 0;
            for (uint offset = 0x40; offset < 0x100 && tail == 0; offset++)
            {
                if (scanner.ShapeAt(pointer + offset, "8B 44 24 08 C1 E0 06 03 41 ?? 5E C2 04 00")) { tail = pointer + offset; }
            }
            Require(tail != 0, "BoneMatrix stride tail");
            List<uint> tails = new List<uint> { tail };
            PedPoolGlobal = pool;
            BoneMatrixCopyFunction = copy;
            BoneMatrixPointerFunction = pointer;
            BoneScratchMatrix = memory.ReadUInt32(pointer + 0x37);
            BoneMatrixStride = 64;
            Report.Add("ped_skeleton ok ped_pool=0x" + pool.ToString("X8") + " copy=0x" + copy.ToString("X8") + " pointer=0x" + pointer.ToString("X8") +
                " scratch=0x" + BoneScratchMatrix.ToString("X8") + " matrices=+0x" + memory.ReadByte(tails[0] + 9).ToString("X"));

            // fragInst skeleton sync (0x5F7D70 on 1.2.0.59): "sub esp,164h; mov eax,[cookie]; xor eax,esp; ...; mov edi,ecx;
            // ...; mov ecx,[edi+5Ch]" and fragInst pose (0x5F6FB0): "push ebp; mov ebp,esp; and esp,-16; sub esp,114h; push ebx;
            // mov eax,0FFFFh; ...; cmp [ecx+8],ax". Long patterns stay unique even with the live process's decrypted code.
            List<uint> sync = scanner.FindPattern("81 EC 64 01 00 00 A1 ?? ?? ?? ?? 33 C4 89 84 24 ?? ?? ?? ?? 56 57 8B F9 89 7C 24 1C 8B 4F 5C", true);
            List<uint> pose = scanner.FindPattern("55 8B EC 83 E4 F0 81 EC 14 01 00 00 53 B8 FF FF 00 00 56 57 89 4C 24 54 66 39 41 08", true);
            if (sync.Count == 1 && pose.Count == 1)
            {
                FragSkeletonSyncFunction = sync[0];
                FragPoseFunction = pose[0];
                Report.Add("skeleton_hooks ok sync=0x" + sync[0].ToString("X8") + " pose=0x" + pose[0].ToString("X8"));
            }
            else
            {
                Report.Add("skeleton_hooks unavailable sync_count=" + sync.Count + " pose_count=" + pose.Count);
            }

            // Anchor (0x60BA5E): the fragInst updates its skeleton, then the cache entry's copy skeleton ([esi+168h]):
            // "push [ecx+14h]; push [ecx+8]; call U; mov ecx,[esi+168h]; test ecx,ecx; jz; push [ecx+14h]; push [ecx+8]; call U".
            List<uint> anchor = scanner.FindPattern("FF 71 14 FF 71 08 E8 ?? ?? ?? ?? 8B 8E 68 01 00 00 85 C9 74 0B FF 71 14 FF 71 08 E8", true);
            if (anchor.Count != 1) { Report.Add("skeleton_update unavailable anchor_count=" + anchor.Count); return; }
            uint call = anchor[0] + 6;
            uint update = call + 5 + (uint)memory.ReadInt32(call + 1);
            if (!scanner.InExecutable(update)) { Report.Add("skeleton_update unavailable target=0x" + update.ToString("X8")); return; }
            foreach (uint site in scanner.FindCallsTo(update))
            {
                // In-place updates only: both "push dword [reg+14h]" and "push dword [reg+8]" within the 8 bytes before the call.
                byte[] before = memory.Read(site - 8, 8);
                bool globals = false, parent = false;
                for (int index = 0; index + 2 < before.Length; index++)
                {
                    if (before[index] != 0xFF || (before[index + 1] & 0xF8) != 0x70) { continue; }
                    if (before[index + 2] == 0x14) { globals = true; }
                    if (before[index + 2] == 0x08) { parent = true; }
                }
                if (globals && parent) { SkeletonUpdateCallSites.Add(site); }
            }
            if (SkeletonUpdateCallSites.Count >= 4) { SkeletonUpdateFunction = update; }
            Report.Add("skeleton_update " + (SkeletonUpdateResolved ? "ok" : "unavailable") + " function=0x" + update.ToString("X8") +
                " sites=" + SkeletonUpdateCallSites.Count);
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

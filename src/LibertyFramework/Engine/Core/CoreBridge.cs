using System;
using System.IO;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.Engine.Core
{
    // Loads LibertyCore.dll (scripts\LibertyFramework\bin), hands it the verified addresses and native handlers, and
    // exposes each frame's snapshot. Every failure leaves Available false; the engine then runs on its SHDN fallback.
    internal sealed unsafe class CoreBridge
    {
        private const string Dll = "LibertyCore.dll";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern uint lc_abi_version();
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern uint lc_native_hash(int id);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_init(ref LcAddressBook book, uint[] handlers, byte[] error, int errorSize);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern void lc_set_native_verified(int id, int verified);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_raycast_install(uint lineTest);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_raycast(ref LcRay ray, ref LcRayHit hit);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_call_native(int id, int argc, int[] args, int[] outs);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern IntPtr lc_frame(ref LcFrameInput input);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern void lc_shutdown();
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)] private static extern int lc_install_crash_handler(string directory);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private static extern void lc_register_phase(int index, string name);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern void lc_set_phase(int index);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_get_phase();
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern void lc_faults(out LcFault fault, uint number);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_damage_hook_install(uint function, uint componentToBone);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_hooks_report(byte[] buffer, int size);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private static extern int lc_write_dump(string reason);

        internal bool Available { get; private set; }
        // The DLL is loaded (crash capture and phases work even when the snapshot is off).
        internal bool Loaded { get; private set; }
        private readonly bool[] verified = new bool[CoreAbi.NativeCount];
        private LcFrameInput input;

        internal static string DllPath { get { return Path.Combine(LibertyPaths.Root, Path.Combine("bin", Dll)); } }
        internal static string CrashDirectory { get { return Path.Combine(LibertyPaths.Root, "crashes"); } }

        internal static int ExpectedSnapshotBytes
        {
            get
            {
                return sizeof(LcSnapshotHead) + CoreAbi.MaxPeds * sizeof(LcPed) + 4 + CoreAbi.MaxVehicles * sizeof(LcVehicle) + 4 + CoreAbi.MaxBullets * sizeof(LcBullet) + 4 + CoreAbi.MaxDamages * sizeof(LcDamage) + 8 +
                    CoreAbi.MaxEvents * sizeof(LcEvent);
            }
        }

        // Loads the DLL and installs crash capture; independent of address resolution so crashes during startup are caught.
        internal bool Load()
        {
            if (Loaded) { return true; }
            try
            {
                if (!File.Exists(DllPath)) { RuntimeLog.Error("engine_core_missing path=" + DllPath); return false; }
                // Loading by full path first makes the DllImport name bind to this copy.
                if (LoadLibrary(DllPath) == IntPtr.Zero) { RuntimeLog.Error("engine_core_load_failed error=" + Marshal.GetLastWin32Error()); return false; }
                uint abi = lc_abi_version();
                if (abi != CoreAbi.Version) { RuntimeLog.Error("engine_core_abi_mismatch core=" + abi + " engine=" + CoreAbi.Version); return false; }
                Loaded = true;
                int dumps = lc_install_crash_handler(CrashDirectory);
                RuntimeLog.Info("engine_crash_capture dir=" + CrashDirectory + " minidumps=" + (dumps != 0));
                return true;
            }
            catch (Exception error)
            {
                RuntimeLog.Error("engine_core_unavailable error=" + error.Message);
                return false;
            }
        }

        internal bool Initialize(CodeScanner scanner, GameAddresses addresses)
        {
            try
            {
                if (!Load()) { return false; }
                uint[] handlers = new uint[CoreAbi.NativeCount];
                int missing = 0;
                for (int id = 0; id < CoreAbi.NativeCount; id++)
                {
                    handlers[id] = scanner.FindNative(lc_native_hash(id));
                    if (handlers[id] == 0) { missing++; RuntimeLog.Error("engine_core_native_missing id=" + id + " hash=0x" + lc_native_hash(id).ToString("X8")); }
                }
                LcAddressBook book = new LcAddressBook();
                book.Size = (uint)sizeof(LcAddressBook);
                book.PedPoolGlobal = addresses.PedPoolGlobal;
                book.FrameCounter = addresses.FrameCounterGlobal;
                book.VehiclePoolGlobal = addresses.VehiclePoolGlobal;
                book.ObjectPoolGlobal = addresses.ObjectPoolGlobal;
                if (addresses.BulletsResolved)
                {
                    book.BulletCountGlobal = addresses.BulletCountGlobal;
                    book.BulletArrayGlobal = addresses.BulletArrayGlobal;
                    book.BulletStride = addresses.BulletStride;
                    book.BulletOwnerOffset = addresses.BulletOwnerOffset;
                    book.BulletMaximum = addresses.BulletMaximum;
                }
                byte[] error = new byte[256];
                if (lc_init(ref book, handlers, error, error.Length) == 0)
                {
                    RuntimeLog.Error("engine_core_init_failed error=" + System.Text.Encoding.ASCII.GetString(error).TrimEnd('\0'));
                    return false;
                }
                input.Size = (uint)sizeof(LcFrameInput);
                Available = true;
                RuntimeLog.Info("engine_core_loaded abi=" + CoreAbi.Version + " natives=" + (CoreAbi.NativeCount - missing) + "/" + CoreAbi.NativeCount +
                    " ped_pool=0x" + book.PedPoolGlobal.ToString("X8") + " vehicle_pool=0x" + book.VehiclePoolGlobal.ToString("X8") +
                    " object_pool=0x" + book.ObjectPoolGlobal.ToString("X8") + " frame_counter=0x" + book.FrameCounter.ToString("X8"));
                return true;
            }
            catch (Exception error)
            {
                RuntimeLog.Error("engine_core_unavailable error=" + error.Message);
                Available = false;
                return false;
            }
        }

        // Direct call for verification; returns int.MinValue when the core refused the call.
        internal int Call(int id, int[] args, int[] outs) { return lc_call_native(id, args == null ? 0 : args.Length, args ?? new int[0], outs ?? new int[4]); }

        internal void SetVerified(int id, bool ok)
        {
            verified[id] = ok;
            lc_set_native_verified(id, ok ? 1 : 0);
        }

        internal bool IsVerified(int id) { return verified[id]; }

        // ADR-0007: the exact-damage observer. Only after the core itself is up (the drain runs in lc_frame).
        internal bool InstallDamageHook(GameAddresses addresses)
        {
            if (!Available || addresses == null || !addresses.DamageResolved) { return false; }
            bool ok = lc_damage_hook_install(addresses.DamageResponseFunction, addresses.ComponentToBoneFunction) != 0;
            RuntimeLog.Info("engine_damage_hook installed=" + ok + " response=0x" + addresses.DamageResponseFunction.ToString("X8"));
            return ok;
        }

        internal bool RaycastReady { get; private set; }

        internal bool InstallRaycast(GameAddresses addresses)
        {
            if (!Available || addresses == null || !addresses.LineTestResolved) { return false; }
            RaycastReady = lc_raycast_install(addresses.LineTestFunction) != 0;
            RuntimeLog.Info("engine_raycast installed=" + RaycastReady + " line_test=0x" + addresses.LineTestFunction.ToString("X8"));
            return RaycastReady;
        }

        // 1 hit, 0 no hit, -1 unavailable or contained fault. Engine tick only.
        internal int Raycast(ref LcRay ray, ref LcRayHit hit)
        {
            if (!RaycastReady) { return -1; }
            if (hit.Raw == null) { hit.Raw = new uint[24]; }
            return lc_raycast(ref ray, ref hit);
        }
        internal string HooksReport()
        {
            if (!Loaded) { return "core not loaded"; }
            byte[] buffer = new byte[2048];
            int count = lc_hooks_report(buffer, buffer.Length);
            string text = System.Text.Encoding.ASCII.GetString(buffer).TrimEnd('\0').Trim().Replace("\n", " | ");
            return count == 0 ? "no hooks" : text;
        }

        internal void RegisterPhase(int index, string name) { if (Loaded) { lc_register_phase(index, name); } }

        // Marks what the engine is running, so a crash report names the module (no allocation).
        internal void SetPhase(int index) { if (Loaded) { lc_set_phase(index); } }

        internal int Phase { get { return Loaded ? lc_get_phase() : -1; } }

        // Watchdog: a minidump of the stalled process (scripts\LibertyFramework\crashes\stall-*.dmp).
        internal bool WriteStallDump(string reason) { return Loaded && lc_write_dump(reason) != 0; }

        // Runs the core frame. Returns null when the core is unavailable or rejected the input. A fault that escaped the core's
        // own containment (a corrupted-state exception) switches the core off for the session instead of reaching the game.
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions, System.Security.SecurityCritical]
        internal LcSnapshotHead* Frame(int playerPed, float pedRadius, float vehicleRadius, uint enabled)
        {
            if (!Available) { return null; }
            input.PlayerPed = playerPed;
            input.PedRadius = pedRadius;
            input.VehicleRadius = vehicleRadius;
            input.Enabled = enabled;
            IntPtr snapshot;
            try { snapshot = lc_frame(ref input); }
            catch (AccessViolationException error)
            {
                RuntimeLog.Error("engine_core_fault_escaped error=" + error.Message + "; core disabled for this session");
                Available = false;
                return null;
            }
            if (snapshot == IntPtr.Zero) { return null; }
            LcSnapshotHead* head = (LcSnapshotHead*)snapshot.ToPointer();
            if (head->Size != ExpectedSnapshotBytes)
            {
                RuntimeLog.Error("engine_core_snapshot_size core=" + head->Size + " engine=" + ExpectedSnapshotBytes);
                Available = false;
                return null;
            }
            return head;
        }

        private uint faultsSeen;

        // Faults the core contained since the last call, one log line each (the faulting native is already off in the core).
        internal void ReportFaults()
        {
            if (!Loaded) { return; }
            LcFault latest;
            lc_faults(out latest, 0);
            if (latest.Count == faultsSeen) { return; }
            uint first = Math.Max(faultsSeen + 1, latest.Count > 16 ? latest.Count - 15 : 1);
            for (uint number = first; number <= latest.Count; number++)
            {
                LcFault fault;
                lc_faults(out fault, number);
                string what = fault.Native >= 0 && fault.Native < CoreAbi.NativeNames.Length ? CoreAbi.NativeNames[fault.Native] + (fault.Argument != 0 ? " handle=" + fault.Argument : "") : "raw game-memory read";
                RuntimeLog.Error("engine_core_fault number=" + number + " in=" + what + " code=0x" + fault.Code.ToString("X8") +
                    " address=0x" + fault.Address.ToString("X8") + " data=0x" + fault.DataAddress.ToString("X8") + "; that native is off for the session");
            }
            faultsSeen = latest.Count;
        }

        internal static LcPed* Peds(LcSnapshotHead* head) { return (LcPed*)((byte*)head + sizeof(LcSnapshotHead)); }

        private static byte* AfterPeds(LcSnapshotHead* head) { return (byte*)Peds(head) + CoreAbi.MaxPeds * sizeof(LcPed); }

        internal static int VehicleCount(LcSnapshotHead* head) { return *(int*)AfterPeds(head); }

        internal static LcVehicle* Vehicles(LcSnapshotHead* head) { return (LcVehicle*)(AfterPeds(head) + 4); }

        private static byte* AfterVehicles(LcSnapshotHead* head) { return AfterPeds(head) + 4 + CoreAbi.MaxVehicles * sizeof(LcVehicle); }

        internal static int BulletCount(LcSnapshotHead* head) { return *(int*)AfterVehicles(head); }

        internal static LcBullet* Bullets(LcSnapshotHead* head) { return (LcBullet*)(AfterVehicles(head) + 4); }

        private static byte* AfterBullets(LcSnapshotHead* head) { return AfterVehicles(head) + 4 + CoreAbi.MaxBullets * sizeof(LcBullet); }

        internal static int DamageCount(LcSnapshotHead* head) { return *(int*)AfterBullets(head); }

        internal static LcDamage* Damages(LcSnapshotHead* head) { return (LcDamage*)(AfterBullets(head) + 4); }

        private static byte* AfterDamages(LcSnapshotHead* head) { return AfterBullets(head) + 4 + CoreAbi.MaxDamages * sizeof(LcDamage); }

        internal static int EventCount(LcSnapshotHead* head) { return *(int*)AfterDamages(head); }

        internal static int EventsDropped(LcSnapshotHead* head) { return *(int*)(AfterDamages(head) + 4); }

        internal static LcEvent* Events(LcSnapshotHead* head) { return (LcEvent*)(AfterDamages(head) + 8); }

        internal void Shutdown()
        {
            if (!Available) { return; }
            try { lc_shutdown(); } catch (Exception error) { RuntimeLog.Error("engine_core_shutdown_failed error=" + error.Message); }
            Available = false;
        }
    }
}

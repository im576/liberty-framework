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
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern int lc_call_native(int id, int argc, int[] args, int[] outs);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern IntPtr lc_frame(ref LcFrameInput input);
        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)] private static extern void lc_shutdown();

        internal bool Available { get; private set; }
        private readonly bool[] verified = new bool[CoreAbi.NativeCount];
        private LcFrameInput input;

        internal static string DllPath { get { return Path.Combine(LibertyPaths.Root, Path.Combine("bin", Dll)); } }

        internal bool Initialize(CodeScanner scanner, GameAddresses addresses)
        {
            try
            {
                if (!File.Exists(DllPath)) { RuntimeLog.Error("engine_core_missing path=" + DllPath); return false; }
                // Loading by full path first makes the DllImport name bind to this copy.
                if (LoadLibrary(DllPath) == IntPtr.Zero) { RuntimeLog.Error("engine_core_load_failed error=" + Marshal.GetLastWin32Error()); return false; }
                uint abi = lc_abi_version();
                if (abi != CoreAbi.Version) { RuntimeLog.Error("engine_core_abi_mismatch core=" + abi + " engine=" + CoreAbi.Version); return false; }
                int expected = sizeof(LcSnapshotHead) + CoreAbi.MaxPeds * sizeof(LcPed) + 8 + CoreAbi.MaxEvents * sizeof(LcEvent);
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
                byte[] error = new byte[256];
                if (lc_init(ref book, handlers, error, error.Length) == 0)
                {
                    RuntimeLog.Error("engine_core_init_failed error=" + System.Text.Encoding.ASCII.GetString(error).TrimEnd('\0'));
                    return false;
                }
                input.Size = (uint)sizeof(LcFrameInput);
                Available = true;
                SnapshotBytes = expected;
                RuntimeLog.Info("engine_core_loaded abi=" + abi + " natives=" + (CoreAbi.NativeCount - missing) + "/" + CoreAbi.NativeCount +
                    " ped_pool=0x" + book.PedPoolGlobal.ToString("X8") + " frame_counter=0x" + book.FrameCounter.ToString("X8"));
                return true;
            }
            catch (Exception error)
            {
                RuntimeLog.Error("engine_core_unavailable error=" + error.Message);
                Available = false;
                return false;
            }
        }

        internal int SnapshotBytes { get; private set; }

        // Direct call for verification; returns int.MinValue when the core refused the call.
        internal int Call(int id, int[] args, int[] outs) { return lc_call_native(id, args == null ? 0 : args.Length, args ?? new int[0], outs ?? new int[4]); }

        internal void SetVerified(int id, bool ok)
        {
            verified[id] = ok;
            lc_set_native_verified(id, ok ? 1 : 0);
        }

        internal bool IsVerified(int id) { return verified[id]; }

        // Runs the core frame. Returns null when the core is unavailable or rejected the input.
        internal LcSnapshotHead* Frame(int playerPed, float pedRadius, uint enabled)
        {
            if (!Available) { return null; }
            input.PlayerPed = playerPed;
            input.PedRadius = pedRadius;
            input.Enabled = enabled;
            IntPtr snapshot = lc_frame(ref input);
            if (snapshot == IntPtr.Zero) { return null; }
            LcSnapshotHead* head = (LcSnapshotHead*)snapshot.ToPointer();
            if (head->Size != SnapshotBytes)
            {
                RuntimeLog.Error("engine_core_snapshot_size core=" + head->Size + " engine=" + SnapshotBytes);
                Available = false;
                return null;
            }
            return head;
        }

        internal static LcPed* Peds(LcSnapshotHead* head) { return (LcPed*)((byte*)head + sizeof(LcSnapshotHead)); }

        internal static int EventCount(LcSnapshotHead* head) { return *(int*)((byte*)Peds(head) + CoreAbi.MaxPeds * sizeof(LcPed)); }

        internal static int EventsDropped(LcSnapshotHead* head) { return *(int*)((byte*)Peds(head) + CoreAbi.MaxPeds * sizeof(LcPed) + 4); }

        internal static LcEvent* Events(LcSnapshotHead* head) { return (LcEvent*)((byte*)Peds(head) + CoreAbi.MaxPeds * sizeof(LcPed) + 8); }

        internal void Shutdown()
        {
            if (!Available) { return; }
            try { lc_shutdown(); } catch (Exception error) { RuntimeLog.Error("engine_core_shutdown_failed error=" + error.Message); }
            Available = false;
        }
    }
}

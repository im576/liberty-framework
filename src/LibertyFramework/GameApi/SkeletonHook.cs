using System;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.GameApi
{
    // ADR-0005: after-call hook on a thiscall, no-argument engine function (the fragInst skeleton rebuilds).
    // The function's first instructions are moved to a trampoline; its entry jumps to a stub that calls the
    // original, then (if the enable flag is set) calls a managed callback with 'this'. The callback runs on the
    // engine's own thread right after the skeleton was rebuilt, so bone edits made there are what gets drawn.
    // Installed only from a script tick (the game's update thread is parked), only over the exact expected
    // bytes, and removed (original bytes restored, flag cleared) on unload/exit/error.
    internal sealed class SkeletonHook : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void AfterCallback(IntPtr instance);

        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint type, uint protect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protect, out uint old);
        [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();

        private const uint MemCommitReserve = 0x3000;
        private const uint PageExecuteReadWrite = 0x40;
        private readonly string name;
        private readonly IntPtr target;
        private readonly byte[] original;
        private readonly IntPtr block;
        private readonly AfterCallback callback; // kept alive for the lifetime of the hook
        private bool installed;

        // expected: byte pattern for the stolen bytes (null entries = wildcard, e.g. a relocated absolute address).
        internal SkeletonHook(string name, uint address, byte?[] expected, AfterCallback callback)
        {
            this.name = name;
            this.callback = callback;
            target = new IntPtr((int)address);
            int length = expected.Length;
            original = new byte[length];
            Marshal.Copy(target, original, 0, length);
            for (int i = 0; i < length; i++)
            {
                if (expected[i].HasValue && original[i] != expected[i].Value)
                {
                    throw new InvalidOperationException(name + " entry bytes differ at +" + i + " (already hooked by another mod?)");
                }
            }
            block = VirtualAlloc(IntPtr.Zero, new UIntPtr(256), MemCommitReserve, PageExecuteReadWrite);
            if (block == IntPtr.Zero) { throw new InvalidOperationException(name + " VirtualAlloc failed " + Marshal.GetLastWin32Error()); }
            int blockAddress = block.ToInt32();
            int flag = blockAddress + 0;          // dword enable flag
            int slot = blockAddress + 4;          // dword callback pointer
            int stub = blockAddress + 16;
            int tramp = blockAddress + 64;
            Marshal.WriteInt32(new IntPtr(flag), 0);
            Marshal.WriteInt32(new IntPtr(slot), Marshal.GetFunctionPointerForDelegate(callback).ToInt32());
            byte[] code = new byte[]
            {
                0x51,                               // push ecx          (save this)
                0xE8, 0, 0, 0, 0,                   // call tramp        (original function)
                0x59,                               // pop ecx
                0x60,                               // pushad
                0x83, 0x3D, 0, 0, 0, 0, 0x00,       // cmp dword [flag], 0
                0x74, 0x07,                         // je skip
                0x51,                               // push ecx          (callback argument)
                0xFF, 0x15, 0, 0, 0, 0,             // call [slot]       (stdcall, cleans its argument)
                0x61,                               // skip: popad
                0xC3                                // ret
            };
            BitConverter.GetBytes(tramp - (stub + 6)).CopyTo(code, 2);
            BitConverter.GetBytes(flag).CopyTo(code, 10);
            BitConverter.GetBytes(slot).CopyTo(code, 20);
            Marshal.Copy(code, 0, new IntPtr(stub), code.Length);
            Marshal.Copy(original, 0, new IntPtr(tramp), length);
            byte[] back = new byte[5];
            back[0] = 0xE9;
            BitConverter.GetBytes((target.ToInt32() + length) - (tramp + length + 5)).CopyTo(back, 1);
            Marshal.Copy(back, 0, new IntPtr(tramp + length), 5);
            FlushInstructionCache(GetCurrentProcess(), block, new UIntPtr(256));
            stubAddress = stub;
        }

        private readonly int stubAddress;

        internal bool Installed { get { return installed; } }

        // Gate for the managed callback (memory flag the stub tests): off while nothing needs it, so the engine pays nothing.
        internal void SetActive(bool value) { if (installed) { Marshal.WriteInt32(block, value ? 1 : 0); } }

        internal void Install()
        {
            if (installed) { return; }
            byte[] patch = new byte[original.Length];
            for (int i = 0; i < patch.Length; i++) { patch[i] = 0x90; }
            patch[0] = 0xE9;
            BitConverter.GetBytes(stubAddress - (target.ToInt32() + 5)).CopyTo(patch, 1);
            Write(patch);
            installed = true;
            RuntimeLog.Info("skeleton_hook_installed " + name + " at=0x" + target.ToInt32().ToString("X8"));
        }

        // Memory-only: safe at DomainUnload/ProcessExit (no natives). Clears the flag first so no managed call follows.
        internal void Remove()
        {
            if (!installed) { return; }
            Marshal.WriteInt32(block, 0);
            Write(original);
            installed = false;
            RuntimeLog.Info("skeleton_hook_removed " + name);
        }

        private void Write(byte[] bytes)
        {
            uint old;
            if (!VirtualProtect(target, new UIntPtr((uint)bytes.Length), PageExecuteReadWrite, out old))
            {
                throw new InvalidOperationException(name + " VirtualProtect failed " + Marshal.GetLastWin32Error());
            }
            Marshal.Copy(bytes, 0, target, bytes.Length);
            uint ignored;
            VirtualProtect(target, new UIntPtr((uint)bytes.Length), old, out ignored);
            FlushInstructionCache(GetCurrentProcess(), target, new UIntPtr((uint)bytes.Length));
        }

        // The stub and trampoline stay allocated: a thread could still be returning through them.
        public void Dispose() { Remove(); }
    }
}

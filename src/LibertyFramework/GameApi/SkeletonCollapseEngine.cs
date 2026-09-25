using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.GameApi
{
    // ADR-0005 (T-022): dismemberment collapse applied inside the engine, right after a skeleton's pose is computed.
    //
    // Playtest 2 showed a collapse written from the script tick only holds on frames where the engine skips the ped's
    // pose update (the head "spawned and despawned"), and the earlier fragInst-only hooks never matched a ped. Now every
    // in-place call of crSkeleton::Update (the one routine that builds the global matrices that get skinned) is
    // redirected to a stub: call the original, then a native routine that finds the skeleton's matrix array in a
    // published table and collapses the listed bones. The routine is plain x86 (no managed code runs on engine
    // threads), matches on matrix pointer AND bone count, and only runs while the table has entries.
    //
    // Collapsed bones get a tiny uniform scale (not zero) around the cut joint: invisible, but still a finite,
    // invertible transform for any physics that reads it (zero axes are the suspected cause of corpses vanishing).
    internal sealed class SkeletonCollapseEngine : IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint type, uint protect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protect, out uint old);
        [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();

        internal const int MaximumIndices = 160;
        internal const int MaximumEntries = 40;
        private const int BlockSize = 0x10000;
        private const int FlagOffset = 0x00, TableOffset = 0x04, HitsOffset = 0x08, CallsOffset = 0x0C, ScaleOffset = 0x10;
        private const int ApplyOffset = 0x100, StubsOffset = 0x400, StubSize = 0x40, TrampolinesOffset = 0xC00;
        private const int TableAOffset = 0x1000, TableBOffset = 0x8000;
        private const int EntrySize = 0x20 + MaximumIndices * 4;
        private const int EntriesOffset = 0x10;

        internal sealed class Entry
        {
            internal uint Matrices;
            internal int BoneCount;
            internal int CutIndex;
            internal int[] Indices;
        }

        private sealed class Patch
        {
            internal string Name;
            internal IntPtr Address;
            internal byte[] Original;
            internal byte[] Replacement;
            internal bool Applied;
            internal bool RelativeOnly; // call site: only the rel32 changes, written with a single 4-byte store
        }

        private readonly IntPtr block;
        private readonly int blockAddress;
        private readonly List<Patch> patches = new List<Patch>();
        private int nextStub, nextTrampoline;
        private bool useTableB;

        internal SkeletonCollapseEngine(float collapseScale)
        {
            block = VirtualAlloc(IntPtr.Zero, new UIntPtr(BlockSize), 0x3000, 0x40);
            if (block == IntPtr.Zero) { throw new InvalidOperationException("collapse engine VirtualAlloc failed " + Marshal.GetLastWin32Error()); }
            blockAddress = block.ToInt32();
            Marshal.WriteInt32(block, FlagOffset, 0);
            Marshal.WriteInt32(block, TableOffset, blockAddress + TableAOffset);
            Marshal.WriteInt32(block, TableAOffset, 0);
            Marshal.WriteInt32(block, TableBOffset, 0);
            Marshal.WriteInt32(block, ScaleOffset, BitConverter.ToInt32(BitConverter.GetBytes(collapseScale), 0));
            byte[] apply = BuildApply(blockAddress + ApplyOffset);
            Marshal.Copy(apply, 0, new IntPtr(blockAddress + ApplyOffset), apply.Length);
        }

        internal int Hits { get { return Marshal.ReadInt32(block, HitsOffset); } }
        internal int Calls { get { return Marshal.ReadInt32(block, CallsOffset); } }
        internal int PatchCount { get { int count = 0; foreach (Patch patch in patches) { if (patch.Applied) { count++; } } return count; } }

        // ecx = crSkeleton*. Clobbers every register (callers wrap it in pushad/popad).
        private byte[] BuildApply(int origin)
        {
            X86 a = new X86(origin);
            a.Bytes(0x85, 0xC9); a.Jz("ret");                              // test ecx,ecx
            a.Bytes(0xFF, 0x05); a.Int(blockAddress + CallsOffset);        // inc dword [calls]
            a.Bytes(0x8B, 0x79, 0x14);                                     // mov edi,[ecx+14h]   global matrices
            a.Bytes(0x85, 0xFF); a.Jz("ret");
            a.Bytes(0x8B, 0x41, 0x04);                                     // mov eax,[ecx+4]     skeleton data
            a.Bytes(0x85, 0xC0); a.Jz("ret");
            a.Bytes(0x0F, 0xB7, 0x58, 0x14);                               // movzx ebx,word [eax+14h]  bone count
            a.Bytes(0x8B, 0x35); a.Int(blockAddress + TableOffset);        // mov esi,[table]
            a.Bytes(0x85, 0xF6); a.Jz("ret");
            a.Bytes(0x8B, 0x16);                                           // mov edx,[esi]       entry count
            a.Bytes(0x83, 0xC6, EntriesOffset);                            // add esi,10h
            a.Label("loop");
            a.Bytes(0x85, 0xD2); a.Jz("ret");                              // test edx,edx
            a.Bytes(0x39, 0x3E); a.Jnz("next");                            // cmp [esi],edi
            a.Bytes(0x39, 0x5E, 0x04); a.Jnz("next");                      // cmp [esi+4],ebx
            a.Bytes(0x52, 0x53);                                           // push edx; push ebx
            a.Bytes(0xFF, 0x46, 0x10);                                     // inc dword [esi+10h] entry hits
            a.Bytes(0xFF, 0x05); a.Int(blockAddress + HitsOffset);         // inc dword [hits]
            a.Bytes(0x8B, 0x46, 0x08);                                     // mov eax,[esi+8]     cut index
            a.Bytes(0xC1, 0xE0, 0x06);                                     // shl eax,6
            a.Bytes(0x8D, 0x1C, 0x07);                                     // lea ebx,[edi+eax]   cut matrix
            a.Bytes(0x8B, 0x4E, 0x0C);                                     // mov ecx,[esi+0Ch]   index count
            a.Bytes(0x8D, 0x56, 0x20);                                     // lea edx,[esi+20h]   indices
            a.Label("bone");
            a.Bytes(0x85, 0xC9); a.Jz("bones_done");                       // test ecx,ecx
            a.Bytes(0x8B, 0x02);                                           // mov eax,[edx]
            a.Bytes(0xC1, 0xE0, 0x06);                                     // shl eax,6
            a.Bytes(0x01, 0xF8);                                           // add eax,edi
            a.Bytes(0x8B, 0x2D); a.Int(blockAddress + ScaleOffset);        // mov ebp,[scale]
            a.Bytes(0x89, 0x28);                                           // mov [eax],ebp       x axis = (s,0,0)
            a.Bytes(0x89, 0x68, 0x14);                                     // mov [eax+14h],ebp   y axis = (0,s,0)
            a.Bytes(0x89, 0x68, 0x28);                                     // mov [eax+28h],ebp   z axis = (0,0,s)
            a.Bytes(0x31, 0xED);                                           // xor ebp,ebp
            a.Bytes(0x89, 0x68, 0x04); a.Bytes(0x89, 0x68, 0x08);
            a.Bytes(0x89, 0x68, 0x10); a.Bytes(0x89, 0x68, 0x18);
            a.Bytes(0x89, 0x68, 0x20); a.Bytes(0x89, 0x68, 0x24);
            a.Bytes(0x8B, 0x6B, 0x30); a.Bytes(0x89, 0x68, 0x30);          // origin = cut joint
            a.Bytes(0x8B, 0x6B, 0x34); a.Bytes(0x89, 0x68, 0x34);
            a.Bytes(0x8B, 0x6B, 0x38); a.Bytes(0x89, 0x68, 0x38);
            a.Bytes(0x83, 0xC2, 0x04);                                     // add edx,4
            a.Bytes(0x49);                                                 // dec ecx
            a.Jmp("bone");
            a.Label("bones_done");
            a.Bytes(0x5B, 0x5A);                                           // pop ebx; pop edx
            a.Label("next");
            a.Bytes(0x81, 0xC6); a.Int(EntrySize);                         // add esi,EntrySize
            a.Bytes(0x4A);                                                 // dec edx
            a.Jmp("loop");
            a.Label("ret");
            a.Bytes(0xC3);
            byte[] code = a.Finish();
            if (code.Length > StubsOffset - ApplyOffset) { throw new InvalidOperationException("collapse routine too large"); }
            return code;
        }

        // Redirect "call crSkeleton::Update" at 'site' (thiscall, 2 arguments, callee pops 8).
        internal void HookCallSite(string name, uint site, uint function)
        {
            IntPtr address = new IntPtr((int)site);
            byte[] original = new byte[5];
            Marshal.Copy(address, original, 0, 5);
            if (original[0] != 0xE8 || site + 5 + (uint)BitConverter.ToInt32(original, 1) != function)
            {
                throw new InvalidOperationException(name + " call site 0x" + site.ToString("X8") + " is not a call to the skeleton update (hooked by another mod?)");
            }
            int stub = NextStub();
            X86 a = new X86(stub);
            a.Bytes(0x51);                                                 // push ecx
            a.Bytes(0xFF, 0x74, 0x24, 0x0C);                               // push [esp+0Ch]  (argument 2)
            a.Bytes(0xFF, 0x74, 0x24, 0x0C);                               // push [esp+0Ch]  (argument 1)
            a.Call((int)function);                                         // original update
            a.Bytes(0x59);                                                 // pop ecx
            a.Bytes(0x83, 0x3D); a.Int(blockAddress + FlagOffset); a.Bytes(0x00);
            a.Jz("done");
            a.Bytes(0x60);                                                 // pushad
            a.Call(blockAddress + ApplyOffset);
            a.Bytes(0x61);                                                 // popad
            a.Label("done");
            a.Bytes(0xC2, 0x08, 0x00);                                     // ret 8
            WriteStub(stub, a.Finish());
            byte[] replacement = new byte[5];
            replacement[0] = 0xE8;
            BitConverter.GetBytes(stub - ((int)site + 5)).CopyTo(replacement, 1);
            AddPatch(name, address, original, replacement);
            patches[patches.Count - 1].RelativeOnly = true;
        }

        // After-call hook at a fragInst method's entry (thiscall, no arguments, plain ret): the collapse is applied to
        // the skeleton returned by fragInst vfunc +0xE0 (what the engine itself calls inside these methods).
        internal void HookFragFunction(string name, uint function, byte?[] expected)
        {
            IntPtr address = new IntPtr((int)function);
            int length = expected.Length;
            byte[] original = new byte[length];
            Marshal.Copy(address, original, 0, length);
            for (int i = 0; i < length; i++)
            {
                if (expected[i].HasValue && original[i] != expected[i].Value) { throw new InvalidOperationException(name + " entry bytes differ at +" + i); }
            }
            int trampoline = blockAddress + TrampolinesOffset + nextTrampoline * 0x40;
            if (++nextTrampoline > 8) { throw new InvalidOperationException("too many trampolines"); }
            byte[] tramp = new byte[length + 5];
            Array.Copy(original, tramp, length);
            tramp[length] = 0xE9;
            BitConverter.GetBytes(((int)function + length) - (trampoline + length + 5)).CopyTo(tramp, length + 1);
            WriteStub(trampoline, tramp);
            int stub = NextStub();
            X86 a = new X86(stub);
            a.Bytes(0x51);                                                 // push ecx
            a.Call(trampoline);                                            // original method
            a.Bytes(0x59);                                                 // pop ecx
            a.Bytes(0x83, 0x3D); a.Int(blockAddress + FlagOffset); a.Bytes(0x00);
            a.Jz("done");
            a.Bytes(0x60);                                                 // pushad
            a.Bytes(0x8B, 0x01);                                           // mov eax,[ecx]
            a.Bytes(0xFF, 0x90, 0xE0, 0x00, 0x00, 0x00);                   // call [eax+0E0h]  -> skeleton
            a.Bytes(0x8B, 0xC8);                                           // mov ecx,eax
            a.Call(blockAddress + ApplyOffset);
            a.Bytes(0x61);                                                 // popad
            a.Label("done");
            a.Bytes(0xC3);
            WriteStub(stub, a.Finish());
            byte[] replacement = new byte[length];
            for (int i = 0; i < length; i++) { replacement[i] = 0x90; }
            replacement[0] = 0xE9;
            BitConverter.GetBytes(stub - ((int)function + 5)).CopyTo(replacement, 1);
            AddPatch(name, address, original, replacement);
        }

        private int NextStub()
        {
            int stub = blockAddress + StubsOffset + nextStub * StubSize;
            if (++nextStub > (TrampolinesOffset - StubsOffset) / StubSize) { throw new InvalidOperationException("too many stubs"); }
            return stub;
        }

        private void WriteStub(int address, byte[] code)
        {
            if (code.Length > StubSize) { throw new InvalidOperationException("stub too large"); }
            Marshal.Copy(code, 0, new IntPtr(address), code.Length);
            FlushInstructionCache(GetCurrentProcess(), new IntPtr(address), new UIntPtr((uint)code.Length));
        }

        private void AddPatch(string name, IntPtr address, byte[] original, byte[] replacement)
        {
            Patch patch = new Patch();
            patch.Name = name; patch.Address = address; patch.Original = original; patch.Replacement = replacement;
            patches.Add(patch);
        }

        // Writes every prepared patch (from a script tick; the engine's update thread is parked while scripts run).
        internal void Install()
        {
            foreach (Patch patch in patches)
            {
                if (patch.Applied) { continue; }
                Write(patch, patch.Replacement);
                patch.Applied = true;
            }
            RuntimeLog.Info("skeleton_collapse_engine_installed patches=" + PatchCount);
        }

        // Engine collapse hits recorded for a matrix array in the live table (0 when absent). Grows every frame the
        // engine re-applied our collapse after a pose update - proof the entry is live on the ped's current skeleton.
        internal int HitsFor(uint matrices)
        {
            int table = Marshal.ReadInt32(block, TableOffset);
            int count = Marshal.ReadInt32(new IntPtr(table));
            for (int i = 0; i < count && i < MaximumEntries; i++)
            {
                int at = table + EntriesOffset + i * EntrySize;
                if ((uint)Marshal.ReadInt32(new IntPtr(at)) == matrices) { return Marshal.ReadInt32(new IntPtr(at + 0x10)); }
            }
            return 0;
        }

        // Replaces the table the native routine reads (double-buffered: the engine may still be reading the old one).
        internal void Publish(IList<Entry> entries)
        {
            int table = blockAddress + (useTableB ? TableBOffset : TableAOffset);
            useTableB = !useTableB;
            int count = 0;
            foreach (Entry entry in entries)
            {
                if (count >= MaximumEntries || entry.Matrices == 0 || entry.Indices == null || entry.Indices.Length == 0 ||
                    entry.Indices.Length > MaximumIndices || entry.BoneCount <= 0) { continue; }
                bool valid = entry.CutIndex >= 0 && entry.CutIndex < entry.BoneCount;
                foreach (int index in entry.Indices) { if (index <= 0 || index >= entry.BoneCount) { valid = false; } }
                if (!valid) { continue; }
                int at = table + EntriesOffset + count * EntrySize;
                Marshal.WriteInt32(new IntPtr(at), (int)entry.Matrices);
                Marshal.WriteInt32(new IntPtr(at + 4), entry.BoneCount);
                Marshal.WriteInt32(new IntPtr(at + 8), entry.CutIndex);
                Marshal.WriteInt32(new IntPtr(at + 0xC), entry.Indices.Length);
                Marshal.WriteInt32(new IntPtr(at + 0x10), 0);
                Marshal.Copy(entry.Indices, 0, new IntPtr(at + 0x20), entry.Indices.Length);
                count++;
            }
            Marshal.WriteInt32(new IntPtr(table), count);
            Marshal.WriteInt32(block, TableOffset, table);
            Marshal.WriteInt32(block, FlagOffset, count > 0 ? 1 : 0);
        }

        // Memory only: safe at DomainUnload/ProcessExit. The block stays allocated (a thread may be inside a stub).
        internal void Remove()
        {
            Marshal.WriteInt32(block, FlagOffset, 0);
            foreach (Patch patch in patches)
            {
                if (!patch.Applied) { continue; }
                try { Write(patch, patch.Original); patch.Applied = false; }
                catch (Exception error) { RuntimeLog.Error("skeleton_collapse_restore_failed " + patch.Name + " error=" + error.Message); }
            }
            RuntimeLog.Info("skeleton_collapse_engine_removed");
        }

        public void Dispose() { Remove(); }

        // A call site only swaps its rel32, with one 4-byte store, so a thread executing it sees the old or new target.
        private static void Write(Patch patch, byte[] bytes)
        {
            IntPtr address = patch.Address;
            uint old;
            if (!VirtualProtect(address, new UIntPtr((uint)bytes.Length), 0x40, out old)) { throw new InvalidOperationException("VirtualProtect failed " + Marshal.GetLastWin32Error()); }
            if (patch.RelativeOnly) { Marshal.WriteInt32(address, 1, BitConverter.ToInt32(bytes, 1)); }
            else { Marshal.Copy(bytes, 0, address, bytes.Length); }
            uint ignored;
            VirtualProtect(address, new UIntPtr((uint)bytes.Length), old, out ignored);
            FlushInstructionCache(GetCurrentProcess(), address, new UIntPtr((uint)bytes.Length));
        }

        // Tiny x86 emitter: raw bytes, absolute call targets, and near (rel32) jumps to labels.
        private sealed class X86
        {
            private readonly int origin;
            private readonly List<byte> code = new List<byte>();
            private readonly Dictionary<string, int> labels = new Dictionary<string, int>();
            private readonly List<KeyValuePair<int, string>> fixups = new List<KeyValuePair<int, string>>();

            internal X86(int origin) { this.origin = origin; }
            internal void Bytes(int value, params int[] more) { code.Add((byte)value); foreach (int b in more) { code.Add((byte)b); } }
            internal void Int(int value) { code.AddRange(BitConverter.GetBytes(value)); }
            internal void Label(string name) { labels[name] = code.Count; }
            internal void Call(int target) { code.Add(0xE8); Int(target - (origin + code.Count + 4)); }
            internal void Jz(string label) { code.Add(0x0F); code.Add(0x84); Fixup(label); }
            internal void Jnz(string label) { code.Add(0x0F); code.Add(0x85); Fixup(label); }
            internal void Jmp(string label) { code.Add(0xE9); Fixup(label); }
            private void Fixup(string label) { fixups.Add(new KeyValuePair<int, string>(code.Count, label)); Int(0); }

            internal byte[] Finish()
            {
                byte[] result = code.ToArray();
                foreach (KeyValuePair<int, string> fixup in fixups)
                {
                    int target = labels[fixup.Value];
                    BitConverter.GetBytes(target - (fixup.Key + 4)).CopyTo(result, fixup.Key);
                }
                return result;
            }
        }
    }
}

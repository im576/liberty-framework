using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibertyFramework.GameApi;

namespace LibertyFramework.Verify
{
    // ADR-0005: runs the real SkeletonCollapseEngine machine code in this (x86) process against a fake skeleton,
    // a fake crSkeleton::Update call site, and a fake fragInst method, then checks the collapse, the pass-through of
    // arguments and return values, the table gating, and the byte-exact restore.
    internal static class CollapseEngineChecks
    {
        [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint type, uint protect);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int NoArgs();

        private const int Bones = 12;
        private const float Scale = 0.01f;

        internal static void Run(Checker check)
        {
            IntPtr memory = VirtualAlloc(IntPtr.Zero, new UIntPtr(0x4000), 0x3000, 0x40);
            int m = memory.ToInt32();
            int skeleton = m + 0x100, data = m + 0x140, matrices = m + 0x400, record = m + 0x180;
            int update = m + 0x1000, caller = m + 0x1100, getter = m + 0x1200, fragMethod = m + 0x1300, fragCaller = m + 0x1400;
            int frag = m + 0x1500, vtable = m + 0x1600;
            const int A1 = 0x11110000, A2 = 0x22220000;

            // Fake crSkeleton: +4 -> data (bone count word at +14h), +14h -> global matrices.
            Marshal.WriteInt32(new IntPtr(skeleton + 4), data);
            Marshal.WriteInt32(new IntPtr(skeleton + 0x14), matrices);
            Marshal.WriteInt16(new IntPtr(data + 0x14), (short)Bones);
            // Update(a1, a2): records ecx and both arguments, returns 12345678h, ret 8.
            Emit(update, 0x89, 0x0D); EmitInt(update + 2, record);                      // mov [record],ecx
            Emit(update + 6, 0x8B, 0x44, 0x24, 0x04, 0xA3); EmitInt(update + 11, record + 4);  // mov eax,[esp+4]; mov [record+4],eax
            Emit(update + 15, 0x8B, 0x44, 0x24, 0x08, 0xA3); EmitInt(update + 20, record + 8); // mov eax,[esp+8]; mov [record+8],eax
            Emit(update + 24, 0xB8); EmitInt(update + 25, 0x12345678);                  // mov eax,12345678h
            Emit(update + 29, 0xC2, 0x08, 0x00);                                        // ret 8
            // Caller: mov ecx,skeleton; push A2; push A1; call update; ret
            Emit(caller, 0xB9); EmitInt(caller + 1, skeleton);
            Emit(caller + 5, 0x68); EmitInt(caller + 6, A2);
            Emit(caller + 10, 0x68); EmitInt(caller + 11, A1);
            int site = caller + 15;
            Emit(site, 0xE8); EmitInt(site + 1, update - (site + 5));
            Emit(site + 5, 0xC3);
            byte[] siteBefore = Read(site, 5);

            SkeletonCollapseEngine engine = new SkeletonCollapseEngine(Scale);
            engine.HookCallSite("test_site", (uint)site, (uint)update);
            engine.Install();
            check.True("collapse engine patched the call site", Read(site, 1)[0] == 0xE8 && !Same(Read(site, 5), siteBefore), BitConverter.ToString(Read(site, 5)));

            NoArgs callUpdate = (NoArgs)Marshal.GetDelegateForFunctionPointer(new IntPtr(caller), typeof(NoArgs));
            FillIdentity(matrices);
            int result = callUpdate();
            check.True("hooked update passes this, both arguments and the return value through", result == 0x12345678 &&
                Marshal.ReadInt32(new IntPtr(record)) == skeleton && Marshal.ReadInt32(new IntPtr(record + 4)) == A1 &&
                Marshal.ReadInt32(new IntPtr(record + 8)) == A2, "result=0x" + result.ToString("X"));
            check.True("empty table: nothing collapsed, routine not entered", IsIdentity(matrices, 5) && engine.Calls == 0, "calls=" + engine.Calls);

            SkeletonCollapseEngine.Entry entry = new SkeletonCollapseEngine.Entry();
            entry.Matrices = (uint)matrices; entry.BoneCount = Bones; entry.CutIndex = 4; entry.Indices = new[] { 4, 5, 6 };
            engine.Publish(new List<SkeletonCollapseEngine.Entry> { entry });
            FillIdentity(matrices);
            callUpdate();
            check.True("listed bones collapse to the cut joint at the configured scale", IsCollapsed(matrices, 4, 4) && IsCollapsed(matrices, 5, 4) &&
                IsCollapsed(matrices, 6, 4) && IsIdentity(matrices, 3) && IsIdentity(matrices, 7) && engine.Hits == 1, "hits=" + engine.Hits);

            SkeletonCollapseEngine.Entry wrongCount = new SkeletonCollapseEngine.Entry();
            wrongCount.Matrices = (uint)matrices; wrongCount.BoneCount = Bones + 1; wrongCount.CutIndex = 4; wrongCount.Indices = new[] { 4, 5 };
            engine.Publish(new List<SkeletonCollapseEngine.Entry> { wrongCount });
            FillIdentity(matrices);
            callUpdate();
            check.True("a skeleton with a different bone count is never written", IsIdentity(matrices, 4) && IsIdentity(matrices, 5), "");

            // fragInst method: 12 NOPs (the stolen bytes) then "mov eax,55h; ret"; vfunc +E0h returns the skeleton.
            for (int i = 0; i < 12; i++) { Emit(fragMethod + i, 0x90); }
            Emit(fragMethod + 12, 0xB8); EmitInt(fragMethod + 13, 0x55); Emit(fragMethod + 17, 0xC3);
            Emit(getter, 0xB8); EmitInt(getter + 1, skeleton); Emit(getter + 5, 0xC3);
            Marshal.WriteInt32(new IntPtr(frag), vtable);
            Marshal.WriteInt32(new IntPtr(vtable + 0xE0), getter);
            Emit(fragCaller, 0xB9); EmitInt(fragCaller + 1, frag);
            Emit(fragCaller + 5, 0xE8); EmitInt(fragCaller + 6, fragMethod - (fragCaller + 10));
            Emit(fragCaller + 10, 0xC3);
            byte?[] nops = new byte?[12];
            for (int i = 0; i < 12; i++) { nops[i] = 0x90; }
            SkeletonCollapseEngine fragEngine = new SkeletonCollapseEngine(Scale);
            fragEngine.HookFragFunction("test_frag", (uint)fragMethod, nops);
            fragEngine.Install();
            entry.Indices = new[] { 8, 9 }; entry.CutIndex = 8;
            fragEngine.Publish(new List<SkeletonCollapseEngine.Entry> { entry });
            FillIdentity(matrices);
            int fragResult = ((NoArgs)Marshal.GetDelegateForFunctionPointer(new IntPtr(fragCaller), typeof(NoArgs)))();
            check.True("fragInst hook runs the method, then collapses the skeleton from vfunc E0h", fragResult == 0x55 &&
                IsCollapsed(matrices, 8, 8) && IsCollapsed(matrices, 9, 8) && IsIdentity(matrices, 7), "result=0x" + fragResult.ToString("X"));

            engine.Remove();
            fragEngine.Remove();
            bool fragRestored = true;
            for (int i = 0; i < 12; i++) { if (Read(fragMethod + i, 1)[0] != 0x90) { fragRestored = false; } }
            check.True("remove restores the original bytes exactly", Same(Read(site, 5), siteBefore) && fragRestored, BitConverter.ToString(Read(site, 5)));
            FillIdentity(matrices);
            check.True("after remove the update runs unhooked", callUpdate() == 0x12345678 && IsIdentity(matrices, 4), "");
        }

        private static void Emit(int address, params int[] bytes) { for (int i = 0; i < bytes.Length; i++) Marshal.WriteByte(new IntPtr(address + i), (byte)bytes[i]); }
        private static void EmitInt(int address, int value) { Marshal.WriteInt32(new IntPtr(address), value); }
        private static byte[] Read(int address, int length) { byte[] bytes = new byte[length]; Marshal.Copy(new IntPtr(address), bytes, 0, length); return bytes; }
        private static bool Same(byte[] a, byte[] b) { if (a.Length != b.Length) return false; for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }

        // Identity axes with origin (i, 2i, 3i) per bone.
        private static void FillIdentity(int matrices)
        {
            for (int bone = 0; bone < Bones; bone++)
            {
                float[] m = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, bone, bone * 2, bone * 3, 1 };
                Marshal.Copy(m, 0, new IntPtr(matrices + bone * 64), 16);
            }
        }

        private static float[] Matrix(int matrices, int bone) { float[] m = new float[16]; Marshal.Copy(new IntPtr(matrices + bone * 64), m, 0, 16); return m; }

        private static bool IsIdentity(int matrices, int bone)
        {
            float[] m = Matrix(matrices, bone);
            return m[0] == 1 && m[5] == 1 && m[10] == 1 && m[1] == 0 && m[4] == 0 && m[12] == bone && m[13] == bone * 2 && m[14] == bone * 3;
        }

        private static bool IsCollapsed(int matrices, int bone, int cut)
        {
            float[] m = Matrix(matrices, bone);
            return m[0] == Scale && m[5] == Scale && m[10] == Scale && m[1] == 0 && m[2] == 0 && m[4] == 0 && m[6] == 0 && m[8] == 0 && m[9] == 0 &&
                m[12] == cut && m[13] == cut * 2 && m[14] == cut * 3 && m[15] == 1;
        }
    }
}

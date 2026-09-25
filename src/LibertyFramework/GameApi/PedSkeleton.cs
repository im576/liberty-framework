using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // T-022: read and collapse a ped's skinning matrices (skeleton->objectMatrices, 64 bytes each).
    // Collapsing a bone = zero its three axis rows and move its origin to a cut point, so every vertex
    // skinned to it shrinks into that joint: a severed limb. Located through the engine's own
    // CPed::CopyBoneMatrix / CPed::BoneMatrix (see GameAddresses.ResolvePedSkeleton, MEMORY.md).
    // Nothing is code-patched; writes only touch the ped's own matrix array and never the identity scratch.
    internal sealed class PedSkeleton : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate byte CopyBoneMatrixFunction(IntPtr ped, IntPtr matrix, int boneTag);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr BoneMatrixFunction(IntPtr ped, int index);

        internal const int MaximumBones = 256;
        private const int MatrixBytes = 64;
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;
        private readonly CopyBoneMatrixFunction copy;
        private readonly BoneMatrixFunction pointer;
        private readonly IntPtr scratch = Marshal.AllocHGlobal(MatrixBytes);
        // modelHash -> boneTag -> index (same model = same skeleton layout).
        private readonly Dictionary<int, Dictionary<int, int>> indices = new Dictionary<int, Dictionary<int, int>>();

        internal PedSkeleton(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
            copy = (CopyBoneMatrixFunction)Marshal.GetDelegateForFunctionPointer(new IntPtr((int)addresses.BoneMatrixCopyFunction), typeof(CopyBoneMatrixFunction));
            pointer = (BoneMatrixFunction)Marshal.GetDelegateForFunctionPointer(new IntPtr((int)addresses.BoneMatrixPointerFunction), typeof(BoneMatrixFunction));
        }

        public void Dispose() { Marshal.FreeHGlobal(scratch); }

        // rage pool: objects +0, flags +4, size +8, item size +12; handle = index << 8 | generation.
        internal uint PedFromHandle(int handle)
        {
            if (handle <= 0) { return 0; }
            uint pool = memory.TryReadPointer(addresses.PedPoolGlobal);
            if (pool == 0 || !memory.IsReadable(pool, 16)) { return 0; }
            uint objects = memory.ReadUInt32(pool);
            uint flags = memory.ReadUInt32(pool + 4);
            int size = memory.ReadInt32(pool + 8);
            int itemSize = memory.ReadInt32(pool + 12);
            int index = handle >> 8;
            if (index < 0 || index >= size || itemSize <= 0 || !memory.IsReadable(flags + (uint)index, 1)) { return 0; }
            byte flag = memory.ReadByte(flags + (uint)index);
            if ((flag & 0x80) != 0 || flag != (handle & 0xFF)) { return 0; }
            uint ped = objects + (uint)(index * itemSize);
            return memory.IsReadable(ped, 0x400) ? ped : 0;
        }

        // Address of objectMatrices[0] or 0 (no skeleton / not an array).
        internal uint MatrixBase(uint ped)
        {
            uint first = (uint)pointer(new IntPtr((int)ped), 0).ToInt32();
            if (first == 0 || first == addresses.BoneScratchMatrix) { return 0; }
            uint second = (uint)pointer(new IntPtr((int)ped), 1).ToInt32();
            if (second != first + MatrixBytes || !memory.IsWritable(first, MatrixBytes * 2)) { return 0; }
            return first;
        }

        // Bone index for a tag: copy the tag's matrix through the engine, then find the identical matrix in the array.
        internal int IndexOf(uint ped, int modelHash, int boneTag)
        {
            Dictionary<int, int> map;
            if (!indices.TryGetValue(modelHash, out map)) { map = new Dictionary<int, int>(); indices[modelHash] = map; }
            int cached;
            if (map.TryGetValue(boneTag, out cached)) { return cached; }
            int found = -1;
            uint baseAddress = MatrixBase(ped);
            if (baseAddress != 0)
            {
                copy(new IntPtr((int)ped), scratch, boneTag);
                byte[] wanted = new byte[MatrixBytes];
                Marshal.Copy(scratch, wanted, 0, MatrixBytes);
                int matches = 0;
                for (int index = 0; index < MaximumBones; index++)
                {
                    uint address = baseAddress + (uint)(index * MatrixBytes);
                    if (!memory.IsReadable(address, MatrixBytes)) { break; }
                    if (SameMatrix(memory.Read(address, MatrixBytes), wanted)) { found = index; matches++; }
                }
                // Ambiguous or root-aliased (unknown tags resolve to index 0) results are rejected.
                if (matches != 1 || (found == 0 && boneTag != 0)) { found = -1; }
            }
            map[boneTag] = found;
            return found;
        }

        private static bool SameMatrix(byte[] a, byte[] b)
        {
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 12; column++)
                {
                    if (a[row * 16 + column] != b[row * 16 + column]) { return false; }
                }
            }
            return true;
        }

        internal float[] Origin(uint ped, int index)
        {
            uint baseAddress = MatrixBase(ped);
            if (baseAddress == 0 || index < 0) { return null; }
            uint address = baseAddress + (uint)(index * MatrixBytes) + 48;
            return new float[] { memory.ReadSingle(address), memory.ReadSingle(address + 4), memory.ReadSingle(address + 8) };
        }

        // Zero-axis matrices at 'cut' for every index. Returns how many were still exactly our previous write
        // (evidence that the engine did not rebuild them since the last tick).
        internal int Collapse(uint ped, IList<int> bones, float[] cut, float[] previousCut)
        {
            uint baseAddress = MatrixBase(ped);
            if (baseAddress == 0) { return -1; }
            int persisted = 0;
            foreach (int index in bones)
            {
                if (index <= 0 || index >= MaximumBones) { continue; }
                uint address = baseAddress + (uint)(index * MatrixBytes);
                if (previousCut != null && IsCollapsedAt(address, previousCut)) { persisted++; }
                WriteRows(address, cut);
            }
            return persisted;
        }

        private bool IsCollapsedAt(uint address, float[] point)
        {
            byte[] current = memory.Read(address, MatrixBytes);
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    if (BitConverter.ToSingle(current, row * 16 + column * 4) != 0f) { return false; }
                }
            }
            return BitConverter.ToSingle(current, 48) == point[0] && BitConverter.ToSingle(current, 52) == point[1] &&
                BitConverter.ToSingle(current, 56) == point[2];
        }

        private void WriteRows(uint address, float[] cut)
        {
            for (int row = 0; row < 3; row++)
            {
                memory.WriteSingle(address + (uint)(row * 16), 0f);
                memory.WriteSingle(address + (uint)(row * 16 + 4), 0f);
                memory.WriteSingle(address + (uint)(row * 16 + 8), 0f);
            }
            memory.WriteSingle(address + 48, cut[0]);
            memory.WriteSingle(address + 52, cut[1]);
            memory.WriteSingle(address + 56, cut[2]);
        }

        // Count of consecutive plausible bone matrices (unit-length axes, origin near the root): the skeleton size.
        internal int BoneCount(uint ped)
        {
            uint baseAddress = MatrixBase(ped);
            if (baseAddress == 0) { return 0; }
            float[] root = Origin(ped, 0);
            int count = 0;
            for (int index = 0; index < MaximumBones; index++)
            {
                uint address = baseAddress + (uint)(index * MatrixBytes);
                if (!memory.IsReadable(address, MatrixBytes)) { break; }
                byte[] m = memory.Read(address, MatrixBytes);
                bool unit = true;
                for (int row = 0; row < 3 && unit; row++)
                {
                    float x = BitConverter.ToSingle(m, row * 16), y = BitConverter.ToSingle(m, row * 16 + 4), z = BitConverter.ToSingle(m, row * 16 + 8);
                    double length = Math.Sqrt(x * x + y * y + z * z);
                    unit = length > 0.9 && length < 1.1;
                }
                float tx = BitConverter.ToSingle(m, 48) - root[0], ty = BitConverter.ToSingle(m, 52) - root[1], tz = BitConverter.ToSingle(m, 56) - root[2];
                if (!unit || Math.Sqrt(tx * tx + ty * ty + tz * tz) > 3.0) { break; }
                count++;
            }
            if (count < 10) { RuntimeLog.Error("ped_skeleton_bone_count_implausible count=" + count); return 0; }
            return count;
        }
    }
}

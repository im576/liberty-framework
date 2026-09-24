using System;
using System.Collections.Generic;
using LibertyFramework.Core.Math3;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // Reads the engine's per-frame bullet trace list (the data IS_BULLET_IN_AREA tests).
    // Each entry: start xyz at +0x00, end xyz (after accuracy offset / impact) at +0x10, owner ped.
    internal sealed class BulletLog
    {
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;
        private readonly HashSet<string> previousFrame = new HashSet<string>();

        internal int LastTotal { get; private set; }
        internal uint LastForeignOwner { get; private set; }
        internal int LastOwnedCount { get; private set; }

        internal struct Trace
        {
            internal Vec3 Start;
            internal Vec3 End;
        }

        internal BulletLog(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
        }

        // Returns traces owned by 'ownerPed' that were not already returned on the previous call.
        internal List<Trace> ReadNew(uint ownerPed)
        {
            List<Trace> result = new List<Trace>();
            HashSet<string> current = new HashSet<string>();
            int count = memory.ReadInt32(addresses.BulletCountGlobal);
            LastTotal = count;
            LastForeignOwner = 0;
            LastOwnedCount = 0;
            uint array = memory.TryReadPointer(addresses.BulletArrayGlobal);
            if (count > 0 && count <= addresses.BulletMaximum && array != 0 && memory.IsReadable(array, count * addresses.BulletStride))
            {
                byte[] data = memory.Read(array, count * addresses.BulletStride);
                for (int index = 0; index < count; index++)
                {
                    int offset = index * addresses.BulletStride;
                    uint owner = BitConverter.ToUInt32(data, offset + addresses.BulletOwnerOffset);
                    if (owner != ownerPed) { if (LastForeignOwner == 0) { LastForeignOwner = owner; } continue; }
                    LastOwnedCount++;
                    string key = BitConverter.ToString(data, offset, 0x1C);
                    current.Add(key);
                    if (previousFrame.Contains(key)) { continue; }
                    Trace trace = new Trace();
                    trace.Start = new Vec3(BitConverter.ToSingle(data, offset), BitConverter.ToSingle(data, offset + 4), BitConverter.ToSingle(data, offset + 8));
                    trace.End = new Vec3(BitConverter.ToSingle(data, offset + 0x10), BitConverter.ToSingle(data, offset + 0x14), BitConverter.ToSingle(data, offset + 0x18));
                    result.Add(trace);
                }
            }
            previousFrame.Clear();
            foreach (string key in current) { previousFrame.Add(key); }
            return result;
        }
    }
}

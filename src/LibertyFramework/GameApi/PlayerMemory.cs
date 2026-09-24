using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // CPlayerInfo table -> player CPed, resolved from the DISABLE_PLAYER_LOCKON implementation.
    internal sealed class PlayerMemory
    {
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;

        internal PlayerMemory(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
        }

        internal uint PedPointer(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= addresses.PlayerInfoCount) { return 0; }
            uint info = memory.TryReadPointer(addresses.PlayerInfoArray + (uint)(playerIndex * 4));
            if (info == 0) { return 0; }
            uint ped = memory.TryReadPointer(info + (uint)addresses.PlayerPedOffset);
            return ped != 0 && memory.IsReadable(ped + (uint)addresses.PedTargetFlagsOffset, 4) ? ped : 0;
        }

        // Current value of the flag DISABLE_PLAYER_LOCKON writes; null when unreadable.
        internal bool? LockOnDisabled(int playerIndex)
        {
            uint ped = PedPointer(playerIndex);
            if (ped == 0) { return null; }
            return (memory.ReadUInt32(ped + (uint)addresses.PedTargetFlagsOffset) & addresses.LockOnDisabledMask) != 0;
        }
    }
}

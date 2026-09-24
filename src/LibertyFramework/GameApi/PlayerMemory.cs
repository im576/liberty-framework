using System;
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

        // Vanilla "aim settle": after ~0.5 s of steady aiming the game scales player bullet offsets to zero.
        // For test weapons LF owns the spread cone, so the timer is held at zero every frame; the game then
        // applies at most one frame of settle (~3% at 60 fps), which the shot-audit calibration absorbs.
        internal void ClearAimSettle(uint ped)
        {
            if (ped == 0 || !addresses.AimSettleResolved) { return; }
            uint timer = ped + (uint)addresses.AimSettleTimerOffset;
            if (!memory.IsWritable(timer, 8)) { return; }
            memory.WriteUInt32(timer, 0);
            memory.WriteUInt32(timer + 4, 0);
        }

        // Fraction of the configured spread the game will actually apply right now (1 = none removed).
        internal double AimSettleFactor(uint ped)
        {
            if (ped == 0 || !addresses.AimSettleResolved || !memory.IsReadable(ped + (uint)addresses.AimSettleSnapshotOffset, 4)) { return 1.0; }
            double window = memory.ReadSingle(addresses.AimSettleWindowGlobal);
            if (window <= 0) { return 1.0; }
            int snapshot = memory.ReadInt32(ped + (uint)addresses.AimSettleSnapshotOffset);
            return 1.0 - Math.Min(Math.Max(snapshot, 0), window) / window;
        }
    }
}
using System;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // T-015: the aim camera's per-state settings table (see GameAddresses.ResolveAimCameraSettings and
    // docs/game-api/MEMORY.md). Shoulder swap scales every record's lateral offset by a side factor
    // (+1 = the game's right shoulder, -1 = left). Only data is written; originals are captured once,
    // validated, and restored on disable, unload and process exit (memory-only, no natives).
    internal sealed class AimCameraSettings
    {
        private const float MaximumLateralMeters = 1.5f;
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;
        private readonly float[] originals;
        private double appliedFactor = 1.0;

        internal AimCameraSettings(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
            originals = new float[addresses.AimCamSettingsCount];
            bool anyShoulder = false;
            for (int index = 0; index < originals.Length; index++)
            {
                float value = memory.ReadSingle(Address(index));
                if (float.IsNaN(value) || Math.Abs(value) > MaximumLateralMeters)
                {
                    throw new InvalidOperationException("aim settings record " + index + " lateral " + value + " out of range");
                }
                if (value > 0.1f) { anyShoulder = true; }
                originals[index] = value;
            }
            if (!anyShoulder) { throw new InvalidOperationException("aim settings have no right-shoulder offset; table already altered?"); }
            RuntimeLog.Info("shoulder_settings_validated records=" + originals.Length + " first_lateral=" + originals[0].ToString("0.000"));
        }

        internal double AppliedFactor { get { return appliedFactor; } }

        private uint Address(int index)
        {
            return addresses.AimCamSettingsTable + (uint)(index * addresses.AimCamSettingsStride + addresses.AimCamLateralOffset);
        }

        internal void Apply(double sideFactor)
        {
            sideFactor = Math.Max(-1.0, Math.Min(1.0, sideFactor));
            if (Math.Abs(sideFactor - appliedFactor) < 0.0005) { return; }
            for (int index = 0; index < originals.Length; index++)
            {
                memory.WriteSingle(Address(index), (float)(originals[index] * sideFactor));
            }
            appliedFactor = sideFactor;
        }

        internal void Restore()
        {
            for (int index = 0; index < originals.Length; index++) { memory.WriteSingle(Address(index), originals[index]); }
            appliedFactor = 1.0;
        }
    }
}

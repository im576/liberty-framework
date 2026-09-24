using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // Pause-menu preferences stored in the game's pref array (the values IS_AUTO_AIMING_ON and
    // IS_HUD_RETICULE_COMPLEX return). Changing them is the same as changing the menu option.
    internal sealed class GamePrefs
    {
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;

        internal GamePrefs(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
        }

        internal int AutoAim
        {
            get { return memory.ReadInt32(addresses.PrefAutoAim); }
            set { memory.WriteUInt32(addresses.PrefAutoAim, (uint)value); }
        }

        internal int Reticule
        {
            get { return memory.ReadInt32(addresses.PrefReticule); }
        }
    }
}

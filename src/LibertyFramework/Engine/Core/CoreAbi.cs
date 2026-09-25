using System.Runtime.InteropServices;

namespace LibertyFramework.Engine.Core
{
    // Field-for-field mirror of native/LibertyCore/include/liberty_core.h (ABI version 1). CoreBridge checks the sizes
    // against the native lc_snapshot.size before reading anything.
    internal static class CoreAbi
    {
        internal const uint Version = 1;
        internal const int MaxPeds = 128;
        internal const int MaxEvents = 256;

        internal const uint ValidWorld = 0x1, ValidPlayer = 0x2, ValidPeds = 0x4;
        internal const uint PedDead = 0x1, PedInVehicle = 0x2, PedPlayer = 0x4, PedNew = 0x8;
        internal const uint PlayerPlaying = 0x1, PlayerControl = 0x2, PlayerDead = 0x4, PlayerInVehicle = 0x8;
        internal const uint WorldPaused = 0x1, WorldFadedOut = 0x2;

        // lc_native_id order.
        internal const int DoesCharExist = 0, IsCharDead = 1, GetCharHealth = 2, GetCharArmour = 3, GetCharCoordinates = 4,
            GetCharHeading = 5, GetCharModel = 6, IsCharInAnyCar = 7, GetCarCharIsUsing = 8, GetCurrentCharWeapon = 9,
            GetAmmoInClip = 10, GetCharLastDamageBone = 11, HasCharBeenDamagedByChar = 12, GetPlayerId = 13,
            IsPlayerPlaying = 14, IsPlayerControlOn = 15, IsPauseMenuActive = 16, IsScreenFadedOut = 17, GetGameTimer = 18,
            GetHoursOfDay = 19, GetMinutesOfDay = 20, GetCurrentWeather = 21, NativeCount = 22;

        internal const int EvPedAppeared = 1, EvPedRemoved = 2, EvPedDamaged = 3, EvPedDied = 4, EvPlayerShot = 5,
            EvPlayerReloaded = 6, EvPlayerWeapon = 7, EvPlayerEnteredVehicle = 8, EvPlayerExitedVehicle = 9, EvPlayerDied = 10,
            EvPlayerDamaged = 11, EvWeatherChanged = 12, EvPauseChanged = 13, EvFadeChanged = 14;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcAddressBook
    {
        internal uint Size;
        internal uint PedPoolGlobal;
        internal uint FrameCounter;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcFrameInput
    {
        internal uint Size;
        internal int PlayerPed;
        internal float PedRadius;
        internal uint Enabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcWorld
    {
        internal uint GameTimerMs;
        internal int Hours;
        internal int Minutes;
        internal int Weather;
        internal uint Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcPlayer
    {
        internal int Index;
        internal int Ped;
        internal float X, Y, Z;
        internal float Heading;
        internal int Health;
        internal int Armour;
        internal int Weapon;
        internal int AmmoInClip;
        internal int Vehicle;
        internal uint Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcPed
    {
        internal int Handle;
        internal uint Model;
        internal float X, Y, Z;
        internal float Heading;
        internal int Health;
        internal int Armour;
        internal int Vehicle;
        internal uint Flags;
        internal float Distance;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcEvent
    {
        internal int Type;
        internal int A, B, C, D, E;
    }

    // lc_snapshot up to and including ped_count; peds, event_count, events_dropped and events follow.
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcSnapshotHead
    {
        internal uint Size;
        internal uint AbiVersion;
        internal uint Frame;
        internal uint EngineFrame;
        internal uint ParkedViolations;
        internal uint Valid;
        internal float CoreMicroseconds;
        internal LcWorld World;
        internal LcPlayer Player;
        internal int PedCount;
    }
}

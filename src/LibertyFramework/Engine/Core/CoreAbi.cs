using System.Runtime.InteropServices;

namespace LibertyFramework.Engine.Core
{
    // Field-for-field mirror of native/LibertyCore/include/liberty_core.h (ABI version 3). CoreBridge checks the sizes
    // against the native lc_snapshot.size before reading anything.
    internal static class CoreAbi
    {
        internal const uint Version = 3;
        internal const int MaxPeds = 128;
        internal const int MaxVehicles = 64;
        internal const int MaxEvents = 256;
        internal const int MaxBullets = 32;
        internal const int MaxDamages = 32;

        internal const uint ValidWorld = 0x1, ValidPlayer = 0x2, ValidPeds = 0x4, ValidVehicles = 0x8, ValidBullets = 0x10, ValidDamage = 0x20;
        internal const uint DamageKilled = 0x1;
        internal const uint PedDead = 0x1, PedInVehicle = 0x2, PedPlayer = 0x4, PedNew = 0x8;
        internal const uint VehicleNew = 0x1, VehiclePlayer = 0x2;
        internal const uint PlayerPlaying = 0x1, PlayerControl = 0x2, PlayerDead = 0x4, PlayerInVehicle = 0x8, PlayerReloading = 0x10;
        internal const uint WorldPaused = 0x1, WorldFadedOut = 0x2;

        // lc_native_id order.
        internal const int DoesCharExist = 0, IsCharDead = 1, GetCharHealth = 2, GetCharArmour = 3, GetCharCoordinates = 4,
            GetCharHeading = 5, GetCharModel = 6, IsCharInAnyCar = 7, GetCarCharIsUsing = 8, GetCurrentCharWeapon = 9,
            GetAmmoInClip = 10, GetCharLastDamageBone = 11, HasCharBeenDamagedByChar = 12, GetPlayerId = 13,
            IsPlayerPlaying = 14, IsPlayerControlOn = 15, IsPauseMenuActive = 16, IsScreenFadedOut = 17, GetGameTimer = 18,
            GetHoursOfDay = 19, GetMinutesOfDay = 20, GetCurrentWeather = 21,
            DoesVehicleExist = 22, GetCarCoordinates = 23, GetCarHeading = 24, GetCarSpeed = 25, GetCarHealth = 26,
            GetEngineHealth = 27, GetCarModel = 28, GetDriverOfCar = 29, NativeCount = 30;

        internal static readonly string[] NativeNames =
        {
            "DOES_CHAR_EXIST", "IS_CHAR_DEAD", "GET_CHAR_HEALTH", "GET_CHAR_ARMOUR", "GET_CHAR_COORDINATES", "GET_CHAR_HEADING", "GET_CHAR_MODEL",
            "IS_CHAR_IN_ANY_CAR", "GET_CAR_CHAR_IS_USING", "GET_CURRENT_CHAR_WEAPON", "GET_AMMO_IN_CLIP", "GET_CHAR_LAST_DAMAGE_BONE",
            "HAS_CHAR_BEEN_DAMAGED_BY_CHAR", "GET_PLAYER_ID", "IS_PLAYER_PLAYING", "IS_PLAYER_CONTROL_ON", "IS_PAUSE_MENU_ACTIVE",
            "IS_SCREEN_FADED_OUT", "GET_GAME_TIMER", "GET_HOURS_OF_DAY", "GET_MINUTES_OF_DAY", "GET_CURRENT_WEATHER", "DOES_VEHICLE_EXIST",
            "GET_CAR_COORDINATES", "GET_CAR_HEADING", "GET_CAR_SPEED", "GET_CAR_HEALTH", "GET_ENGINE_HEALTH", "GET_CAR_MODEL", "GET_DRIVER_OF_CAR",
        };

        internal const int EvPedAppeared = 1, EvPedRemoved = 2, EvPedDamaged = 3, EvPedDied = 4, EvPlayerShot = 5,
            EvPlayerReloaded = 6, EvPlayerWeapon = 7, EvPlayerEnteredVehicle = 8, EvPlayerExitedVehicle = 9, EvPlayerDied = 10,
            EvPlayerDamaged = 11, EvWeatherChanged = 12, EvPauseChanged = 13, EvFadeChanged = 14,
            EvVehicleAppeared = 15, EvVehicleRemoved = 16, EvVehicleDamaged = 17, EvVehicleDestroyed = 18, EvPlayerReloadStarted = 19;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcAddressBook
    {
        internal uint Size;
        internal uint PedPoolGlobal;
        internal uint FrameCounter;
        internal uint VehiclePoolGlobal;
        internal uint ObjectPoolGlobal;
        internal uint BulletCountGlobal;
        internal uint BulletArrayGlobal;
        internal int BulletStride;
        internal int BulletOwnerOffset;
        internal int BulletMaximum;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcFrameInput
    {
        internal uint Size;
        internal int PlayerPed;
        internal float PedRadius;
        internal uint Enabled;
        internal float VehicleRadius;
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
    internal struct LcPools
    {
        internal int PedsUsed, PedsSize;
        internal int VehiclesUsed, VehiclesSize;
        internal int ObjectsUsed, ObjectsSize;
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
    internal struct LcVehicle
    {
        internal int Handle;
        internal uint Model;
        internal float X, Y, Z;
        internal float Heading;
        internal float Speed;
        internal int Health;
        internal float EngineHealth;
        internal int Driver;
        internal uint Flags;
        internal float Distance;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcBullet
    {
        internal int Shooter;
        internal int Weapon;
        internal float FromX, FromY, FromZ;
        internal float ToX, ToY, ToZ;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcDamage
    {
        internal int Victim;
        internal int Attacker;
        internal int AttackerKind;
        internal int Weapon;
        internal int Component;
        internal int Bone;
        internal float Amount;
        internal float HealthLost;
        internal float ArmourLost;
        internal uint Flags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcEvent
    {
        internal int Type;
        internal int A, B, C, D, E;
    }

    // lc_snapshot up to and including ped_count; peds, vehicle_count, vehicles, bullet_count, bullets, damage_count, damages, event_count, events_dropped and events follow.
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
        internal LcPools Pools;
        internal int PedCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LcFault
    {
        internal uint Count;
        internal int Native;
        internal int Argument;
        internal uint Address;
        internal uint DataAddress;
        internal uint Code;
    }
}

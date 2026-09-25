using System;
using GTA;
using GTA.Native;
using LibertyFramework.Core.Math3;

namespace LibertyFramework.GameApi
{
    // Every raw native used by Liberty Framework goes through here; each is listed in
    // docs/game-api/NATIVES.md with its CE 1.2.0.59 implementation address and evidence.
    internal static class Natives
    {
        // T-026: cached player index for direct calls that take a Player (single-player: stable per session).
        private static int cachedPlayerIndex = -1;

        internal static int PlayerIndex()
        {
            int index = DirectNatives.Ready(DirectNatives.GetPlayerId) ? DirectNatives.Call(DirectNatives.GetPlayerId) : Function.Call<int>("GET_PLAYER_ID");
            cachedPlayerIndex = index;
            return index;
        }

        private static int H(Ped ped) { return ped.GetHashCode(); }
        private static bool Direct(string name) { return DirectNatives.Ready(name); }

        // Direct ped reads only for a handle the engine's own pool lookup confirms (some workers call a ped vfunc
        // without a null check). An invalid handle falls through to SHDN, which throws a catchable exception instead.
        private static bool DirectPed(string name, Ped ped)
        {
            return ped != null && DirectNatives.Ready(name) && DirectNatives.Ready(DirectNatives.DoesCharExist) &&
                DirectNatives.Call(DirectNatives.DoesCharExist, H(ped)) != 0;
        }
        private static bool DirectForPlayer(string name) { return cachedPlayerIndex >= 0 && DirectNatives.Ready(name); }

        internal static void DisablePlayerLockOn(Player player, bool disabled)
        {
            Function.Call("DISABLE_PLAYER_LOCKON", player, disabled);
        }

        internal static int GameCamHandle()
        {
            Pointer handle = typeof(int);
            Function.Call("GET_GAME_CAM", handle);
            return (int)handle;
        }

        internal static bool CamExists(int camera)
        {
            return camera != 0 && Function.Call<bool>("DOES_CAM_EXIST", camera);
        }

        internal static Vec3 CamRotation(int camera)
        {
            Pointer x = typeof(float);
            Pointer y = typeof(float);
            Pointer z = typeof(float);
            Function.Call("GET_CAM_ROT", camera, x, y, z);
            return new Vec3((float)x, (float)y, (float)z);
        }

        internal static Vec3 CamPosition(int camera)
        {
            Pointer x = typeof(float);
            Pointer y = typeof(float);
            Pointer z = typeof(float);
            Function.Call("GET_CAM_POS", camera, x, y, z);
            return new Vec3((float)x, (float)y, (float)z);
        }

        internal static float CamFov(int camera)
        {
            Pointer fov = typeof(float);
            if (Direct(DirectNatives.GetCamFov)) { DirectNatives.Call(DirectNatives.GetCamFov, camera, DirectNatives.Out(0)); return DirectNatives.OutFloat(0); }
            Function.Call("GET_CAM_FOV", camera, fov);
            return (float)fov;
        }

        internal static float CharSpeed(Ped ped)
        {
            Pointer speed = typeof(float);
            if (DirectPed(DirectNatives.GetCharSpeed, ped)) { DirectNatives.Call(DirectNatives.GetCharSpeed, H(ped), DirectNatives.Out(0)); return DirectNatives.OutFloat(0); }
            Function.Call("GET_CHAR_SPEED", ped, speed);
            return (float)speed;
        }

        internal static bool IsDucking(Ped ped)
        {
            return DirectPed(DirectNatives.IsCharDucking, ped) ? DirectNatives.Call(DirectNatives.IsCharDucking, H(ped)) != 0 : Function.Call<bool>("IS_CHAR_DUCKING", ped);
        }

        internal static bool IsInCover(Ped ped)
        {
            return DirectPed(DirectNatives.IsPedInCover, ped) ? DirectNatives.Call(DirectNatives.IsPedInCover, H(ped)) != 0 : Function.Call<bool>("IS_PED_IN_COVER", ped);
        }

        internal static bool IsInAnyCar(Ped ped)
        {
            return DirectPed(DirectNatives.IsCharInAnyCar, ped) ? DirectNatives.Call(DirectNatives.IsCharInAnyCar, H(ped)) != 0 : Function.Call<bool>("IS_CHAR_IN_ANY_CAR", ped);
        }

        internal static bool IsInAir(Ped ped)
        {
            return DirectPed(DirectNatives.IsCharInAir, ped) ? DirectNatives.Call(DirectNatives.IsCharInAir, H(ped)) != 0 : Function.Call<bool>("IS_CHAR_IN_AIR", ped);
        }

        internal static bool IsPauseMenuActive()
        {
            return Direct(DirectNatives.IsPauseMenuActive) ? DirectNatives.Call(DirectNatives.IsPauseMenuActive) != 0 : Function.Call<bool>("IS_PAUSE_MENU_ACTIVE");
        }

        internal static bool IsScreenFadedOut()
        {
            return Direct(DirectNatives.IsScreenFadedOut) ? DirectNatives.Call(DirectNatives.IsScreenFadedOut) != 0 : Function.Call<bool>("IS_SCREEN_FADED_OUT");
        }

        internal static bool IsPlayerPlaying(Player player)
        {
            return DirectForPlayer(DirectNatives.IsPlayerPlaying) ? DirectNatives.Call(DirectNatives.IsPlayerPlaying, cachedPlayerIndex) != 0 : Function.Call<bool>("IS_PLAYER_PLAYING", player);
        }

        internal static bool IsPlayerControlOn(Player player)
        {
            return DirectForPlayer(DirectNatives.IsPlayerControlOn) ? DirectNatives.Call(DirectNatives.IsPlayerControlOn, cachedPlayerIndex) != 0 : Function.Call<bool>("IS_PLAYER_CONTROL_ON", player);
        }

        internal static float GroundZ(float x, float y, float z)
        {
            Pointer ground = typeof(float);
            Function.Call("GET_GROUND_Z_FOR_3D_COORD", x, y, z, ground);
            return (float)ground;
        }

        internal static void RequestCollisionAt(float x, float y, float z)
        {
            Function.Call("REQUEST_COLLISION_AT_POSN", x, y, z);
        }

        internal static void LoadScene(float x, float y, float z)
        {
            Function.Call("LOAD_SCENE", x, y, z);
        }

        internal static void SetCharCoordinates(Ped ped, float x, float y, float z)
        {
            Function.Call("SET_CHAR_COORDINATES", ped, x, y, z);
        }

        internal static void SetCharHeading(Ped ped, float heading)
        {
            Function.Call("SET_CHAR_HEADING", ped, heading);
        }

        internal static int GameViewportId()
        {
            Pointer viewport = typeof(int);
            Function.Call("GET_GAME_VIEWPORT_ID", viewport);
            return (int)viewport;
        }

        // Signature from the CE handler: (x, y, z, viewportId, float* screenX, float* screenY) -> on screen.
        // Screen coordinates are normalised 0..1.
        internal static bool ViewportPositionOfCoord(int viewport, Vec3 world, out float screenX, out float screenY)
        {
            Pointer x = typeof(float);
            Pointer y = typeof(float);
            bool visible = Function.Call<bool>("GET_VIEWPORT_POSITION_OF_COORD", (float)world.X, (float)world.Y, (float)world.Z, viewport, x, y);
            screenX = (float)x;
            screenY = (float)y;
            return visible;
        }
        // T-026 ped reads used every sample by gunplay, holsters and combat effects.
        // Raw native health (SHDN's Ped.Health reads 100 lower for peds; callers keep SHDN's scale via PedHealth).
        internal static int PedHealth(Ped ped)
        {
            if (!DirectPed(DirectNatives.GetCharHealth, ped)) { return ped.Health; }
            DirectNatives.Call(DirectNatives.GetCharHealth, H(ped), DirectNatives.Out(0));
            return DirectNatives.OutInt(0) - 100;
        }

        internal static bool PedExists(Ped ped)
        {
            if (ped == null) { return false; }
            return Direct(DirectNatives.DoesCharExist) ? DirectNatives.Call(DirectNatives.DoesCharExist, H(ped)) != 0 : ped.Exists();
        }

        internal static bool PedDead(Ped ped)
        {
            return DirectPed(DirectNatives.IsCharDead, ped) ? DirectNatives.Call(DirectNatives.IsCharDead, H(ped)) != 0 : ped.isDead;
        }

        internal static bool DamagedBy(Ped ped, Ped attacker)
        {
            return DirectPed(DirectNatives.HasCharBeenDamagedByChar, ped) && DirectPed(DirectNatives.HasCharBeenDamagedByChar, attacker) ?
                DirectNatives.Call(DirectNatives.HasCharBeenDamagedByChar, H(ped), H(attacker), 0) != 0 : ped.HasBeenDamagedBy(attacker);
        }

        internal static int CurrentWeapon(Ped ped)
        {
            if (!DirectPed(DirectNatives.GetCurrentCharWeapon, ped)) { return (int)ped.Weapons.CurrentType; }
            DirectNatives.Call(DirectNatives.GetCurrentCharWeapon, H(ped), DirectNatives.Out(0));
            return DirectNatives.OutInt(0);
        }

        // -1 = unavailable (no direct path); callers then use SHDN's Weapon object.
        internal static int AmmoInClip(Ped ped, int weapon)
        {
            if (!DirectPed(DirectNatives.GetAmmoInClip, ped)) { return -1; }
            DirectNatives.Call(DirectNatives.GetAmmoInClip, H(ped), weapon, DirectNatives.Out(0));
            return DirectNatives.OutInt(0);
        }

        internal static int MaxAmmoInClip(Ped ped, int weapon)
        {
            if (!DirectPed(DirectNatives.GetMaxAmmoInClip, ped)) { return -1; }
            DirectNatives.Call(DirectNatives.GetMaxAmmoInClip, H(ped), weapon, DirectNatives.Out(0));
            return DirectNatives.OutInt(0);
        }

        // Verifies every direct native against SHDN on the player (same tick, game parked). Unverified ones keep SHDN.
        internal static void VerifyDirect(Player player, Ped ped, int gameCamera)
        {
            int index = Function.Call<int>("GET_PLAYER_ID");
            int handle = H(ped);
            DirectNatives.Verify(DirectNatives.GetPlayerId, () => DirectNatives.Call(DirectNatives.GetPlayerId), () => index);
            cachedPlayerIndex = index;
            DirectNatives.Verify(DirectNatives.IsPlayerPlaying, () => DirectNatives.Call(DirectNatives.IsPlayerPlaying, index), () => Function.Call<bool>("IS_PLAYER_PLAYING", player) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsPlayerControlOn, () => DirectNatives.Call(DirectNatives.IsPlayerControlOn, index), () => Function.Call<bool>("IS_PLAYER_CONTROL_ON", player) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsCharDucking, () => DirectNatives.Call(DirectNatives.IsCharDucking, handle), () => Function.Call<bool>("IS_CHAR_DUCKING", ped) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsPedInCover, () => DirectNatives.Call(DirectNatives.IsPedInCover, handle), () => Function.Call<bool>("IS_PED_IN_COVER", ped) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsCharInAnyCar, () => DirectNatives.Call(DirectNatives.IsCharInAnyCar, handle), () => Function.Call<bool>("IS_CHAR_IN_ANY_CAR", ped) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsCharInAir, () => DirectNatives.Call(DirectNatives.IsCharInAir, handle), () => Function.Call<bool>("IS_CHAR_IN_AIR", ped) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsPauseMenuActive, () => DirectNatives.Call(DirectNatives.IsPauseMenuActive), () => Function.Call<bool>("IS_PAUSE_MENU_ACTIVE") ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsScreenFadedOut, () => DirectNatives.Call(DirectNatives.IsScreenFadedOut), () => Function.Call<bool>("IS_SCREEN_FADED_OUT") ? 1 : 0);
            DirectNatives.Verify(DirectNatives.IsCharDead, () => DirectNatives.Call(DirectNatives.IsCharDead, handle), () => Function.Call<bool>("IS_CHAR_DEAD", ped) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.DoesCharExist, () => DirectNatives.Call(DirectNatives.DoesCharExist, handle), () => Function.Call<bool>("DOES_CHAR_EXIST", ped) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.HasCharBeenDamagedByChar, () => DirectNatives.Call(DirectNatives.HasCharBeenDamagedByChar, handle, handle, 0),
                () => Function.Call<bool>("HAS_CHAR_BEEN_DAMAGED_BY_CHAR", ped, ped, false) ? 1 : 0);
            DirectNatives.Verify(DirectNatives.GetCharHealth, () => { DirectNatives.Call(DirectNatives.GetCharHealth, handle, DirectNatives.Out(0)); return DirectNatives.OutInt(0); },
                () => { Pointer value = typeof(int); Function.Call("GET_CHAR_HEALTH", ped, value); return (int)value; });
            DirectNatives.Verify(DirectNatives.GetCharSpeed, () => { DirectNatives.Call(DirectNatives.GetCharSpeed, handle, DirectNatives.Out(0)); return DirectNatives.OutInt(0); },
                () => { Pointer value = typeof(float); Function.Call("GET_CHAR_SPEED", ped, value); return BitConverter.ToInt32(BitConverter.GetBytes((float)value), 0); });
            int weapon = 0;
            DirectNatives.Verify(DirectNatives.GetCurrentCharWeapon, () => { DirectNatives.Call(DirectNatives.GetCurrentCharWeapon, handle, DirectNatives.Out(0)); return DirectNatives.OutInt(0); },
                () => { Pointer value = typeof(int); Function.Call("GET_CURRENT_CHAR_WEAPON", ped, value); weapon = (int)value; return weapon; });
            DirectNatives.Verify(DirectNatives.GetAmmoInClip, () => { DirectNatives.Call(DirectNatives.GetAmmoInClip, handle, weapon, DirectNatives.Out(0)); return DirectNatives.OutInt(0); },
                () => { Pointer value = typeof(int); Function.Call("GET_AMMO_IN_CLIP", ped, weapon, value); return (int)value; });
            DirectNatives.Verify(DirectNatives.GetMaxAmmoInClip, () => { DirectNatives.Call(DirectNatives.GetMaxAmmoInClip, handle, weapon, DirectNatives.Out(0)); return DirectNatives.OutInt(0); },
                () => { Pointer value = typeof(int); Function.Call("GET_MAX_AMMO_IN_CLIP", ped, weapon, value); return (int)value; });
            if (gameCamera != 0)
            {
                DirectNatives.Verify(DirectNatives.GetCamFov, () => { DirectNatives.Call(DirectNatives.GetCamFov, gameCamera, DirectNatives.Out(0)); return DirectNatives.OutInt(0); },
                    () => { Pointer value = typeof(float); Function.Call("GET_CAM_FOV", gameCamera, value); return BitConverter.ToInt32(BitConverter.GetBytes((float)value), 0); });
            }
        }

        internal static void ClearWantedLevel(Player player)        {
            Function.Call("CLEAR_WANTED_LEVEL", player);
        }
    }
}

using GTA;
using GTA.Native;
using LibertyFramework.Core.Math3;

namespace LibertyFramework.GameApi
{
    // Every raw native used by Liberty Framework goes through here; each is listed in
    // docs/game-api/NATIVES.md with its CE 1.2.0.59 implementation address and evidence.
    internal static class Natives
    {
        internal static int PlayerIndex()
        {
            return Function.Call<int>("GET_PLAYER_ID");
        }

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
            Function.Call("GET_CAM_FOV", camera, fov);
            return (float)fov;
        }

        internal static float CharSpeed(Ped ped)
        {
            Pointer speed = typeof(float);
            Function.Call("GET_CHAR_SPEED", ped, speed);
            return (float)speed;
        }

        internal static bool IsDucking(Ped ped)
        {
            return Function.Call<bool>("IS_CHAR_DUCKING", ped);
        }

        internal static bool IsInCover(Ped ped)
        {
            return Function.Call<bool>("IS_PED_IN_COVER", ped);
        }

        internal static bool IsInAnyCar(Ped ped)
        {
            return Function.Call<bool>("IS_CHAR_IN_ANY_CAR", ped);
        }

        internal static bool IsInAir(Ped ped)
        {
            return Function.Call<bool>("IS_CHAR_IN_AIR", ped);
        }

        internal static bool IsPauseMenuActive()
        {
            return Function.Call<bool>("IS_PAUSE_MENU_ACTIVE");
        }

        internal static bool IsScreenFadedOut()
        {
            return Function.Call<bool>("IS_SCREEN_FADED_OUT");
        }

        internal static bool IsPlayerPlaying(Player player)
        {
            return Function.Call<bool>("IS_PLAYER_PLAYING", player);
        }

        internal static bool IsPlayerControlOn(Player player)
        {
            return Function.Call<bool>("IS_PLAYER_CONTROL_ON", player);
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
        internal static void ClearWantedLevel(Player player)
        {
            Function.Call("CLEAR_WANTED_LEVEL", player);
        }
    }
}

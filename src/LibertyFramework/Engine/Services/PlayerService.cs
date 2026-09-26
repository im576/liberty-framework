using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IPlayer. Reads come from the frame snapshot where it has them. Control locks and invincibility are owned:
    // control returns when the last locking module releases or stops.
    public sealed class PlayerService : IPlayer
    {
        private readonly LibertyEngine engine;
        // Lock holders: a module (LockControl) or a module's input capture (distinct key).
        private readonly HashSet<object> lockers = new HashSet<object>();
        private readonly HashSet<LibertyModule> invincible = new HashSet<LibertyModule>();

        internal PlayerService(LibertyEngine engine) { this.engine = engine; }

        public PedRef Ped
        {
            get
            {
                if (engine.World.HasPlayer) { return engine.World.Player.Ped; }
                GTA.Player player = GTA.Game.LocalPlayer;
                return player != null ? Handles.Ref(player.Character) : PedRef.None;
            }
        }

        public int Index { get { return engine.World.HasPlayer ? engine.World.Player.Index : Function.Call<int>("GET_PLAYER_ID"); } }

        public int Money
        {
            get { return NativeCall.OutInt("STORE_SCORE", Index); }
            set { Function.Call("ADD_SCORE", Index, value - Money); }
        }

        public int WantedLevel
        {
            get { return NativeCall.OutInt("STORE_WANTED_LEVEL", Index); }
            set
            {
                if (value <= 0) { Function.Call("CLEAR_WANTED_LEVEL", Index); return; }
                Function.Call("ALTER_WANTED_LEVEL", Index, value > 6 ? 6 : value);
                Function.Call("APPLY_WANTED_LEVEL_CHANGE_NOW", Index);
            }
        }

        public bool IsPlaying { get { return engine.World.HasPlayer ? engine.World.Player.IsPlaying : Function.Call<bool>("IS_PLAYER_PLAYING", Index); } }

        public bool HasControl { get { return engine.World.HasPlayer ? engine.World.Player.HasControl : Function.Call<bool>("IS_PLAYER_CONTROL_ON", Index); } }

        public void LockControl(LibertyModule owner)
        {
            engine.RequireCapability(owner, Capabilities.PlayerControl);
            if (!lockers.Add(owner)) { return; }
            if (lockers.Count == 1) { Function.Call("SET_PLAYER_CONTROL", Index, false); }
            engine.Ledger.Add(owner, "control", 0, () => Unlock(owner));
        }

        public void ReleaseControl(LibertyModule owner) { engine.Ledger.Release(owner, "control", 0); }

        // Input capture and open menus lock control under their own key (same counting; the caller cleans up).
        internal void LockControlInternal(object key)
        {
            if (lockers.Add(key) && lockers.Count == 1) { Function.Call("SET_PLAYER_CONTROL", Index, false); }
        }

        internal void UnlockControlInternal(object key) { Unlock(key); }

        private void Unlock(object owner)
        {
            if (lockers.Remove(owner) && lockers.Count == 0) { Function.Call("SET_PLAYER_CONTROL", Index, true); }
        }

        // Streams collision and the scene first so the player does not fall through the map.
        public void Teleport(Vec3 position, float heading)
        {
            Function.Call("REQUEST_COLLISION_AT_POSN", position.X, position.Y, position.Z);
            Function.Call("LOAD_SCENE", position.X, position.Y, position.Z);
            int ped = Ped.Handle;
            Function.Call("SET_CHAR_COORDINATES", ped, position.X, position.Y, position.Z);
            Function.Call("SET_CHAR_HEADING", ped, heading);
        }

        public void SetInvincible(LibertyModule owner, bool on)
        {
            if (on)
            {
                if (!invincible.Add(owner)) { return; }
                if (invincible.Count == 1) { Function.Call("SET_PLAYER_INVINCIBLE", Index, true); }
                engine.Ledger.Add(owner, "invincible", 0, () => { if (invincible.Remove(owner) && invincible.Count == 0) { Function.Call("SET_PLAYER_INVINCIBLE", Index, false); } });
            }
            else { engine.Ledger.Release(owner, "invincible", 0); }
        }
    }
}

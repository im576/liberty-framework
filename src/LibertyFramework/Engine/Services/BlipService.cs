using System;
using System.Collections.Generic;
using GTA;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IBlips over ScriptHookDotNet's Blip class. Owned; removed when the module stops.
    public sealed class BlipService : IBlips
    {
        private readonly LibertyEngine engine;
        private readonly Dictionary<int, Blip> blips = new Dictionary<int, Blip>();

        internal BlipService(LibertyEngine engine) { this.engine = engine; }

        public BlipRef AddForPosition(LibertyModule owner, Vec3 position) { return Own(owner, Blip.AddBlip(Handles.V(position))); }

        public BlipRef AddForPed(LibertyModule owner, PedRef ped)
        {
            Ped target = Handles.Ped(ped);
            return target != null ? Own(owner, Blip.AddBlip(target)) : BlipRef.None;
        }

        public BlipRef AddForVehicle(LibertyModule owner, VehicleRef vehicle)
        {
            Vehicle target = Handles.Vehicle(vehicle);
            return target != null ? Own(owner, Blip.AddBlip(target)) : BlipRef.None;
        }

        private BlipRef Own(LibertyModule owner, Blip blip)
        {
            if (blip == null) { return BlipRef.None; }
            int handle = blip.GetHashCode();
            blips[handle] = blip;
            engine.Ledger.Add(owner, "blip", handle, () => RemoveNow(handle));
            return new BlipRef(handle);
        }

        private Blip Get(BlipRef blip)
        {
            Blip value;
            return blips.TryGetValue(blip.Handle, out value) ? value : null;
        }

        public void SetSprite(BlipRef blip, int sprite) { Blip b = Get(blip); if (b != null) { b.Icon = (BlipIcon)sprite; } }

        public void SetColour(BlipRef blip, int colour) { Blip b = Get(blip); if (b != null) { b.Color = (BlipColor)colour; } }

        public void SetName(BlipRef blip, string name) { Blip b = Get(blip); if (b != null) { b.Name = name; } }

        public void SetRoute(BlipRef blip, bool on) { Blip b = Get(blip); if (b != null) { b.RouteActive = on; } }

        public void Remove(BlipRef blip) { if (engine.Ledger.Has("blip", blip.Handle)) { engine.Ledger.Release("blip", blip.Handle); } }

        private void RemoveNow(int handle)
        {
            Blip blip;
            if (!blips.TryGetValue(handle, out blip)) { return; }
            blips.Remove(handle);
            try { blip.Delete(); }
            catch (Exception error) { RuntimeLog.Info("blip_already_gone handle=" + handle + " " + error.Message); }
        }
    }
}

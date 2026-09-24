using System.Collections.Generic;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // Hides vanilla hud.dat reticle components by shrinking them and zeroing their alpha.
    // The HUD code copies these hud.dat globals into the live components every frame, so the
    // change is order-independent and restoring the saved values brings the reticle back.
    internal sealed class HudReticle
    {
        // Rendered size is scaled from this; a tiny non-zero value avoids any divide-by-zero.
        private const float HiddenSize = 0.00001f;
        private readonly LiveMemory memory;
        private readonly Dictionary<string, SavedComponent> saved = new Dictionary<string, SavedComponent>();
        private readonly Dictionary<string, GameAddresses.HudComponentGlobals> components = new Dictionary<string, GameAddresses.HudComponentGlobals>();

        private struct SavedComponent
        {
            internal uint Alpha;
            internal float Width;
            internal float Height;
        }

        internal const string Crosshair = "HUD_WEAPON_CROSSHAIR";
        internal const string HealthTarget = "HUD_WEAPON_HEALTH_TARGET";
        internal const string ArmourTarget = "HUD_WEAPON_ARMOUR_TARGET";
        internal const string Dot = "HUD_WEAPON_DOT";

        internal HudReticle(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            foreach (GameAddresses.HudComponentGlobals component in addresses.ReticleComponents)
            {
                components[component.Name] = component;
            }
        }

        internal bool IsHidden(string name)
        {
            return saved.ContainsKey(name);
        }

        // Called every frame while hidden; re-applies in case the HUD reloaded hud.dat.
        internal void Hide(string name)
        {
            GameAddresses.HudComponentGlobals component;
            if (!components.TryGetValue(name, out component)) { return; }
            if (!saved.ContainsKey(name))
            {
                SavedComponent original = new SavedComponent();
                original.Alpha = memory.ReadUInt32(component.AlphaGlobal);
                original.Width = memory.ReadSingle(component.SizeGlobal);
                original.Height = memory.ReadSingle(component.SizeGlobal + 4);
                if (original.Width <= HiddenSize && original.Height <= HiddenSize) { return; }
                saved.Add(name, original);
                RuntimeLog.Info("hud_hide " + name + " alpha=" + original.Alpha + " size=" + original.Width + "x" + original.Height);
            }
            memory.WriteUInt32(component.AlphaGlobal, 0);
            memory.WriteSingle(component.SizeGlobal, HiddenSize);
            memory.WriteSingle(component.SizeGlobal + 4, HiddenSize);
        }

        internal void Restore(string name)
        {
            SavedComponent original;
            if (!saved.TryGetValue(name, out original)) { return; }
            GameAddresses.HudComponentGlobals component = components[name];
            memory.WriteUInt32(component.AlphaGlobal, original.Alpha);
            memory.WriteSingle(component.SizeGlobal, original.Width);
            memory.WriteSingle(component.SizeGlobal + 4, original.Height);
            saved.Remove(name);
            RuntimeLog.Info("hud_restore " + name + " alpha=" + original.Alpha + " size=" + original.Width + "x" + original.Height);
        }

        internal void RestoreAll()
        {
            foreach (string name in new List<string>(saved.Keys)) { Restore(name); }
        }
    }
}

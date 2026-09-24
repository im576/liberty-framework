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
                if (original.Width <= HiddenSize && original.Height <= HiddenSize)
                {
                    // Already hidden (e.g. an earlier script instance was killed without restoring):
                    // fall back to the [HD] values in common/data/hud.dat so a later restore is still correct.
                    if (!TryReadHudDat(name, ref original)) { return; }
                    RuntimeLog.Info("hud_hide " + name + " was already hidden; restore values taken from hud.dat");
                }
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

        private static bool TryReadHudDat(string name, ref SavedComponent original)
        {
            try
            {
                string path = System.IO.Path.Combine(LibertyFramework.Core.Config.LibertyPaths.GameDirectory, @"common\data\hud.dat");
                foreach (string raw in System.IO.File.ReadAllLines(path))
                {
                    string[] parts = raw.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 5 || parts[0] != name) { continue; }
                    string[] size = parts[2].Split(',');
                    original.Width = float.Parse(size[0], System.Globalization.CultureInfo.InvariantCulture);
                    original.Height = float.Parse(size[1], System.Globalization.CultureInfo.InvariantCulture);
                    original.Alpha = uint.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                    return true;
                }
            }
            catch (System.Exception error)
            {
                RuntimeLog.Error("hud_dat_read_failed " + name + " error=" + error.Message);
            }
            return false;
        }

        internal void RestoreAll()
        {
            foreach (string name in new List<string>(saved.Keys)) { Restore(name); }
        }
    }
}

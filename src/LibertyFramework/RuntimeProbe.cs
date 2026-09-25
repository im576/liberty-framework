using System;
using GTA;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework
{
    // T-001 only verifies that a managed script can load and keep ticking on CE.
    [LibertyFramework.Engine.Module("probe", Order = 100)]
    public sealed class RuntimeProbe : LibertyFramework.Engine.Module
    {
        internal static string ActiveProbeLabel = "none";
        private bool disabled;
        private readonly ProbeConfigLoader configLoader;

        public RuntimeProbe()
        {
            Interval = 10000;
            Tick += OnTick;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            configLoader = new ProbeConfigLoader();
            RuntimeLog.Info("T-001 runtime probe started; assembly=" + GetType().Assembly.GetName().Version);
            configLoader.Poll();
            ActiveProbeLabel = configLoader.ActiveLabel;
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled)
            {
                return;
            }

            try
            {
                configLoader.Poll();
                ActiveProbeLabel = configLoader.ActiveLabel;
                RuntimeLog.Info("T-001 heartbeat probe_label=" + configLoader.ActiveLabel);
            }
            catch (Exception error)
            {
                disabled = true;
                Game.Console.Print("[LibertyFramework] T-001 heartbeat disabled after error: " + error);
            }
        }

        private void OnDomainUnload(object sender, EventArgs args)
        {
            try
            {
                RuntimeLog.Info("T-001 script domain unloading");
            }
            catch (Exception error)
            {
                Game.Console.Print("[LibertyFramework] T-001 unload logging failed: " + error);
            }
        }
    }
}

using System;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework
{
    // T-001 only verifies that a managed script can load and keep ticking on CE.
    public sealed class RuntimeProbe : Script
    {
        private bool disabled;

        public RuntimeProbe()
        {
            Interval = 10000;
            Tick += OnTick;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            RuntimeLog.Info("T-001 runtime probe started; assembly=" + GetType().Assembly.GetName().Version);
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled)
            {
                return;
            }

            try
            {
                RuntimeLog.Info("T-001 heartbeat");
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

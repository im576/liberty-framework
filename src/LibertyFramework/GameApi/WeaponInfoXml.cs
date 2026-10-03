using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace LibertyFramework.GameApi
{
    // Reads <aiming accuracy> per weapon type from the WeaponInfo.xml the game loaded
    // (FusionFix's update folder copy takes precedence over common/data).
    internal static class WeaponInfoXml
    {
        internal static string ActivePath(string gameDirectory)
        {
            string update = Path.Combine(gameDirectory, Path.Combine("update", Path.Combine("common", Path.Combine("data", "WeaponInfo.xml"))));
            if (File.Exists(update)) { return update; }
            return Path.Combine(gameDirectory, Path.Combine("common", Path.Combine("data", "WeaponInfo.xml")));
        }

        internal static Dictionary<string, float> ReadAccuracies(string path)
        {
            Dictionary<string, float> result = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;
            document.Load(path);
            foreach (XmlNode weapon in document.SelectNodes("/weaponinfo/weapon"))
            {
                XmlAttribute type = weapon.Attributes["type"];
                XmlNode aiming = weapon.SelectSingleNode("data/aiming");
                if (type == null || aiming == null || aiming.Attributes["accuracy"] == null) { continue; }
                float accuracy;
                if (float.TryParse(aiming.Attributes["accuracy"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out accuracy))
                {
                    result[type.Value] = accuracy;
                }
            }
            return result;
        }

        // The identity fields of every weapon entry (T-041): <data timebetweenshots clipsize ammomax> and <damage base>.
        internal static Dictionary<string, LibertyFramework.GameApi.WeaponStats> ReadStats(string path)
        {
            Dictionary<string, LibertyFramework.GameApi.WeaponStats> result =
                new Dictionary<string, LibertyFramework.GameApi.WeaponStats>(StringComparer.OrdinalIgnoreCase);
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;
            document.Load(path);
            foreach (XmlNode weapon in document.SelectNodes("/weaponinfo/weapon"))
            {
                XmlAttribute type = weapon.Attributes["type"];
                XmlNode data = weapon.SelectSingleNode("data");
                if (type == null || data == null) { continue; }
                LibertyFramework.GameApi.WeaponStats stats = new LibertyFramework.GameApi.WeaponStats();
                stats.TimeBetweenShotsMilliseconds = IntAttribute(data, "timebetweenshots");
                stats.ClipSize = IntAttribute(data, "clipsize");
                stats.AmmoMax = IntAttribute(data, "ammomax");
                stats.DamageBase = IntAttribute(data.SelectSingleNode("damage"), "base");
                result[type.Value] = stats;
            }
            return result;
        }

        private static int? IntAttribute(XmlNode node, string name)
        {
            if (node == null || node.Attributes[name] == null) { return null; }
            int value;
            return int.TryParse(node.Attributes[name].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? (int?)value : null;
        }

        internal static string ModelFor(string path, string weaponType)
        {
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;
            document.Load(path);
            XmlNode assets = document.SelectSingleNode("/weaponinfo/weapon[@type='" + weaponType + "']/assets");
            return assets == null || assets.Attributes["model"] == null ? null : assets.Attributes["model"].Value;
        }
    }
}

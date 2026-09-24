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

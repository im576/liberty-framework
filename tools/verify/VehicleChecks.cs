using System;
using System.Collections.Generic;
using System.IO;
using LibertyFramework.Finishes;
using LibertyFramework.Vehicles;

namespace LibertyFramework.Verify
{
    // T-023: the extras scanner must name known parts correctly from the installed vehicles.img.
    internal static class VehicleChecks
    {
        internal static void Run(string exePath, Checker check)
        {
            string game = Path.GetDirectoryName(Path.GetFullPath(exePath));
            byte[] key = ImgArchive.FindKey(exePath);
            ImgArchive img = ImgArchive.Open(Path.Combine(game, @"pc\models\cdimages\vehicles.img"), key);
            Expect(check, img, "sultanrs", "\"extra\": 1, \"parent\": \"bonnet\"", "Hood scoop");
            Expect(check, img, "police", "\"extra\": 1, \"parent\": \"chassis\"", "Roof item");
            Expect(check, img, "infernus", "\"extra\": 1, \"parent\": \"boot\"", "Trunk spoiler");
            check.True("model hash is Jenkins of lower-case name (sultanrs)", VehicleExtrasScanner.ModelHash("SultanRS") == VehicleExtrasScanner.ModelHash("sultanrs"), "");
        }

        private static void Expect(Checker check, ImgArchive img, string model, string part, string label)
        {
            List<string> extras = VehicleExtrasScanner.ScanExtras(RscResource.Parse(img.Extract(model + ".wft")).Body);
            string match = extras.Find(e => e.Contains(part));
            check.True("vehicle extras: " + model + " " + label, match != null && match.Contains(label), match ?? string.Join(" | ", extras.ToArray()));
        }
    }
}
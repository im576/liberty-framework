using System;
using System.Collections.Generic;
using System.IO;
using LibertyFramework.Finishes;

namespace LibertyFramework.UiTools
{
    // S-2: extracts each weapon model's own "icon" texture (the game's weapon HUD icon, from the installed weapon pack)
    // to <out>\<weaponId>.png for the Liberty weapon wheel. Runs at install time on the owner's files; no game art is
    // committed. usage: WeaponIcons <game dir> <out dir>
    internal static class WeaponIcons
    {
        // weapon id -> model (texture dictionary) name; ids 1-20 vanilla, 58-60 Liberty Framework finishes.
        private static readonly Dictionary<int, string> Models = new Dictionary<int, string>
        {
            { 1, "w_bat" }, { 2, "w_cue" }, { 3, "w_knife" }, { 4, "w_grenade" }, { 5, "w_molotov" },
            { 7, "w_glock" }, { 9, "w_eagle" }, { 10, "w_shotgun" }, { 11, "w_pumpshot" }, { 12, "w_uzi" }, { 13, "w_mp5" },
            { 14, "w_ak47" }, { 15, "w_m4" }, { 16, "w_psg1" }, { 17, "w_rifle" }, { 18, "rpg" },
            { 58, "lf_gold_pistol" }, { 59, "lf_gold_carbine" }, { 60, "lf_gold_shotgun" },
        };

        private static int Main(string[] args)
        {
            if (args.Length != 2) { Console.WriteLine("usage: WeaponIcons <game dir> <out dir>"); return 2; }
            string game = args[0];
            Directory.CreateDirectory(args[1]);
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            List<ImgArchive> archives = new List<ImgArchive>();
            foreach (string path in new[] { @"pc\models\cdimages\weapons.img", @"update\LibertyFramework\LibertyFramework.img" })
            {
                string full = Path.Combine(game, path);
                if (File.Exists(full)) { archives.Add(ImgArchive.Open(full, key)); }
            }
            int written = 0;
            foreach (KeyValuePair<int, string> pair in Models)
            {
                byte[] raw = null;
                foreach (ImgArchive archive in archives)
                {
                    foreach (ImgArchive.Entry entry in archive.Entries)
                    {
                        if (string.Equals(entry.Name, pair.Value + ".wtd", StringComparison.OrdinalIgnoreCase)) { raw = archive.Extract(entry.Name); break; }
                    }
                    if (raw != null) { break; }
                }
                if (raw == null) { Console.WriteLine("  no texture dictionary for " + pair.Value); continue; }
                RscResource resource = RscResource.Parse(raw);
                TextureDictionary dictionary;
                try { dictionary = TextureDictionary.Parse(resource, false); }
                catch (Exception error) { Console.WriteLine("  unreadable " + pair.Value + ": " + error.Message); continue; }
                TextureDictionary.Texture icon = null;
                foreach (TextureDictionary.Texture texture in dictionary.Textures)
                {
                    if (texture.Name.IndexOf("icon", StringComparison.OrdinalIgnoreCase) >= 0) { icon = texture; break; }
                }
                if (icon == null) { Console.WriteLine("  no icon in " + pair.Value); continue; }
                IconDecoder.Save(resource.Body, icon, Path.Combine(args[1], pair.Key + ".png"));
                written++;
            }
            Console.WriteLine("weapon icons written=" + written + "/" + Models.Count);
            return written > 0 ? 0 : 1;
        }
    }
}

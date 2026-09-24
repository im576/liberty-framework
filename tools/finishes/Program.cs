using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LibertyFramework.Finishes
{
    // Builds weapon finish variants from the user's own game files:
    //   <staging>/update/LibertyFramework/LibertyFramework.img  (variant .wdr + recoloured .wtd)
    //   <staging>/update/common/data/lf_finishes.ide            (weap + amat registration)
    //   <staging>/previews/*.png                                (before/after of each recoloured texture)
    // Nothing from the game is committed to the repository; the output lives in ignored staging/.
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("usage: FinishBuilder <game directory> <repo root> <staging directory>");
                return 2;
            }
            try
            {
                Build(args[0], args[1], args[2]);
                return 0;
            }
            catch (Exception error)
            {
                Console.WriteLine("FAILED " + error);
                return 1;
            }
        }

        private static void Build(string game, string repo, string staging)
        {
            FinishCatalogFile catalog = LoadCatalog(Path.Combine(repo, Path.Combine("assets", Path.Combine("finishes", "finishes.json"))));
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            Console.WriteLine("IMG key located in GTAIV.exe");
            string previews = Path.Combine(staging, "previews");
            Directory.CreateDirectory(previews);
            List<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            StringBuilder weap = new StringBuilder();
            StringBuilder amat = new StringBuilder();

            foreach (ModelVariant variant in catalog.Variants)
            {
                FinishDefinition finish = catalog.Finishes.FirstOrDefault(f => f.Id == variant.Finish);
                if (finish == null) { throw new InvalidDataException("variant " + variant.VariantModel + " uses unknown finish " + variant.Finish); }
                ImgArchive source = ImgArchive.Open(Path.Combine(game, variant.SourceImg.Replace('/', '\\')), key);
                byte[] drawable = source.Extract(variant.BaseModel + ".wdr");
                byte[] textures = source.Extract(variant.BaseModel + ".wtd");
                RscResource.Parse(drawable);

                RscResource dictionaryResource = RscResource.Parse(textures);
                TextureDictionary dictionary = TextureDictionary.Parse(dictionaryResource);
                foreach (TextureDictionary.Texture texture in dictionary.Textures)
                {
                    string role;
                    if (!variant.TextureRoles.TryGetValue(texture.Name, out role))
                    {
                        Console.WriteLine("  keep " + texture.Name + " (no role)");
                        continue;
                    }
                    ColourRamp ramp = role == "diffuse" ? finish.Diffuse : role == "specular" ? finish.Specular : role == "icon" ? finish.Icon : null;
                    if (ramp == null) { throw new InvalidDataException("unknown texture role " + role); }
                    string stem = variant.VariantModel + "_" + texture.Name;
                    DxtPreview.Save(dictionaryResource.Body, texture, Path.Combine(previews, stem + "_before.png"));
                    int blocks = DxtRecolor.Apply(dictionaryResource.Body, texture, ramp);
                    DxtPreview.Save(dictionaryResource.Body, texture, Path.Combine(previews, stem + "_after.png"));
                    Console.WriteLine("  recoloured " + texture.Name + " " + texture.Format + " " + texture.Width + "x" + texture.Height +
                        " levels=" + texture.Levels + " role=" + role + " blocks=" + blocks);
                }
                byte[] recoloured = dictionaryResource.Serialize();
                RscResource check = RscResource.Parse(recoloured);
                if (!check.Body.SequenceEqual(dictionaryResource.Body) || check.Flags != dictionaryResource.Flags)
                {
                    throw new InvalidDataException("round-trip check failed for " + variant.VariantModel + ".wtd");
                }
                TextureDictionary.Parse(check);
                files.Add(new KeyValuePair<string, byte[]>(variant.VariantModel + ".wdr", drawable));
                files.Add(new KeyValuePair<string, byte[]>(variant.VariantModel + ".wtd", recoloured));
                weap.AppendLine(variant.VariantModel + ", " + variant.VariantModel + ", " + variant.AnimGroup + ", 1, " + variant.DrawDistance + ", 0");
                amat.AppendLine(variant.VariantModel + ", 0, " + variant.AudioMaterial);
                Console.WriteLine("variant " + variant.VariantModel + " <- " + variant.BaseModel + " finish=" + finish.Id);
            }

            string imgPath = Path.Combine(staging, Path.Combine("update", Path.Combine("LibertyFramework", "LibertyFramework.img")));
            Directory.CreateDirectory(Path.GetDirectoryName(imgPath));
            ImgArchive.Write(imgPath, files);
            ImgArchive written = ImgArchive.Open(imgPath, key);
            foreach (KeyValuePair<string, byte[]> file in files)
            {
                if (!written.Extract(file.Key).SequenceEqual(file.Value)) { throw new InvalidDataException("IMG read-back mismatch for " + file.Key); }
            }
            Console.WriteLine("wrote " + imgPath + " entries=" + files.Count + " (read-back verified)");

            string idePath = Path.Combine(staging, Path.Combine("update", Path.Combine("common", Path.Combine("data", "lf_finishes.ide"))));
            Directory.CreateDirectory(Path.GetDirectoryName(idePath));
            string ide = "# Liberty Framework weapon finish variants (generated by tools/finishes)\r\nweap\r\n" + weap + "end\r\namat\r\n" + amat + "end\r\n";
            File.WriteAllText(idePath, ide.Replace("\r\n", "\n").Replace("\n", "\r\n"), new UTF8Encoding(false));
            Console.WriteLine("wrote " + idePath);
        }

        private static FinishCatalogFile LoadCatalog(string path)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
            settings.UseSimpleDictionaryFormat = true;
            using (FileStream stream = File.OpenRead(path))
            {
                FinishCatalogFile catalog = (FinishCatalogFile)new DataContractJsonSerializer(typeof(FinishCatalogFile), settings).ReadObject(stream);
                if (catalog.SchemaVersion != 1) { throw new InvalidDataException("finishes.json schemaVersion must be 1"); }
                return catalog;
            }
        }
    }
}

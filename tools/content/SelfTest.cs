using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // `selftest`: offline tests of the compiler's parts that need no game files. DXT1/DXT3/DXT5 codecs, the texture name
    // hash, the mip chain, the from-scratch texture dictionary writer (read back with the same parser that reads the game's
    // dictionaries, its own output re-captured and rebuilt as `wtdcheck` does with game files, single and multi-page
    // placement), the native read-back check, the validator's LOD/material rules, manifests, and the sample asset's glTF
    // import. `--out <dir>` also writes source/decoded PNGs of the test textures and the test dictionaries. Exit 0 = all pass.
    internal static class SelfTest
    {
        // Quality floors for the synthetic test images (smooth gradients): reference DXT encoders score 35+ dB on them.
        private const double MinimumGradientPsnrDb = 30;
        private const int MaxRampAlphaError = 20;

        private sealed class Runner
        {
            internal int Passed;
            internal readonly List<string> Failures = new List<string>();
            internal string Group;

            internal void Check(bool condition, string name, string detail)
            {
                if (condition) { Passed++; return; }
                Failures.Add(Group + ": " + name + (string.IsNullOrEmpty(detail) ? "" : " (" + detail + ")"));
            }

            internal void Check(bool condition, string name) { Check(condition, name, null); }

            internal void Throws<T>(Action action, string name) where T : Exception
            {
                try { action(); Failures.Add(Group + ": " + name + " (no exception)"); }
                catch (T) { Passed++; }
                catch (Exception other) { Failures.Add(Group + ": " + name + " (threw " + other.GetType().Name + ": " + other.Message + ")"); }
            }
        }

        internal static int Run(string[] args)
        {
            string output = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--out" && i + 1 < args.Length) { output = args[++i]; }
                else { throw new ArgumentException("selftest: unknown argument " + args[i]); }
            }
            if (output != null) { Directory.CreateDirectory(output); }
            Runner runner = new Runner();
            List<KeyValuePair<string, Action<Runner, string>>> groups = new List<KeyValuePair<string, Action<Runner, string>>>
            {
                Group("name hash", NameHash),
                Group("dxt1", Dxt1),
                Group("dxt5", Dxt5),
                Group("dxt decoder", Decoder),
                Group("mip chain", MipChain),
                Group("wtd writer", Writer),
                Group("wtd multi-page", MultiPage),
                Group("wtd errors", WriterErrors),
                Group("native texture", NativePipeline),
                Group("native source", NativeSource),
                Group("validator", Validator),
                Group("manifest", Manifest),
                Group("sample asset", Sample),
            };
            foreach (KeyValuePair<string, Action<Runner, string>> group in groups)
            {
                runner.Group = group.Key;
                int before = runner.Passed + runner.Failures.Count;
                try { group.Value(runner, output); }
                catch (Exception error) { runner.Failures.Add(group.Key + ": threw " + error.GetType().Name + ": " + error.Message); }
                Console.WriteLine("  " + group.Key + ": " + (runner.Passed + runner.Failures.Count - before) + " checks");
            }
            foreach (string failure in runner.Failures) { Console.WriteLine("  FAIL " + failure); }
            Console.WriteLine("selftest: " + (runner.Failures.Count == 0 ? "ok" : "FAILED") + " passed=" + runner.Passed + " failed=" + runner.Failures.Count);
            return runner.Failures.Count == 0 ? 0 : 1;
        }

        private static KeyValuePair<string, Action<Runner, string>> Group(string name, Action<Runner, string> body)
        {
            return new KeyValuePair<string, Action<Runner, string>>(name, body);
        }

        // --- test images ---

        // Smooth RGB gradient with a diagonal alpha ramp (alpha 255 when opaque).
        internal static RgbaImage Gradient(int width, int height, bool withAlpha)
        {
            RgbaImage image = new RgbaImage(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int at = (y * width + x) * 4;
                    image.Pixels[at] = (byte)(x * 255 / Math.Max(1, width - 1));
                    image.Pixels[at + 1] = (byte)(y * 255 / Math.Max(1, height - 1));
                    image.Pixels[at + 2] = (byte)(255 - (x + y) * 255 / Math.Max(1, width + height - 2));
                    image.Pixels[at + 3] = withAlpha ? (byte)((x + y) * 255 / Math.Max(1, width + height - 2)) : (byte)255;
                }
            }
            return image;
        }

        private static RgbaImage Decode(byte[] data, string format, int width, int height)
        {
            return new RgbaImage(width, height, DxtDecoder.Decode(data, 0, format, width, height));
        }

        private static void SavePng(string output, string name, RgbaImage image)
        {
            if (output == null) { return; }
            using (Bitmap bitmap = image.ToBitmap()) { bitmap.Save(Path.Combine(output, name + ".png"), ImageFormat.Png); }
        }

        // --- groups ---

        private static void NameHash(Runner t, string output)
        {
            // Published Jenkins one-at-a-time vector: "a" -> 0xCA2E9442.
            t.Check(TextureNameHash.Compute("a") == 0xCA2E9442u, "one-at-a-time vector 'a'", "0x" + TextureNameHash.Compute("a").ToString("X8"));
            t.Check(TextureNameHash.Compute("") == 0u, "empty name hashes to 0");
            t.Check(TextureNameHash.Compute("AMB_Nailgun") == TextureNameHash.Compute("amb_nailgun"), "case-insensitive");
            t.Check(TextureNameHash.Compute("amb_nailgun") != TextureNameHash.Compute("amb_nailgun2"), "different names differ");
        }

        private static void Dxt1(Runner t, string output)
        {
            RgbaImage source = Gradient(64, 64, false);
            byte[] encoded = Dxt1Encoder.Encode(source.ToRgb(), 64, 64);
            t.Check(encoded.Length == DxtDecoder.LevelBytes("DXT1", 64, 64) && encoded.Length == 16 * 16 * 8, "64x64 size", encoded.Length.ToString());
            RgbaImage decoded = Decode(encoded, "DXT1", 64, 64);
            double psnr = ImageMetrics.PsnrDb(source, decoded, false);
            t.Check(psnr >= MinimumGradientPsnrDb, "gradient PSNR >= " + MinimumGradientPsnrDb + " dB", psnr.ToString("0.0"));
            t.Check(!decoded.HasTranslucency(), "encoder writes opaque (4-colour) blocks only");
            for (int block = 0; block < encoded.Length; block += 8)
            {
                if (BitConverter.ToUInt16(encoded, block) <= BitConverter.ToUInt16(encoded, block + 2)) { t.Check(false, "c0 > c1 in every block", "block " + block / 8); break; }
            }
            // Colours exact in RGB565 survive unchanged, including the uniform-block endpoint fix-up (black and white).
            foreach (byte[] colour in new[] { new byte[] { 255, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 255, 255, 255 }, new byte[] { 0, 255, 0 } })
            {
                RgbaImage solid = RgbaImage.Solid(8, 8, colour[0], colour[1], colour[2], 255);
                RgbaImage back = Decode(Dxt1Encoder.Encode(solid.ToRgb(), 8, 8), "DXT1", 8, 8);
                t.Check(ImageMetrics.MaxError(solid, back, false) == 0, "solid " + string.Join(",", colour.Select(c => c.ToString()).ToArray()) + " exact", ImageMetrics.MaxError(solid, back, false).ToString());
            }
            // Sizes below one block still take a whole block.
            t.Check(Dxt1Encoder.Encode(new byte[2 * 2 * 3], 2, 2).Length == 8 && DxtDecoder.LevelBytes("DXT1", 1, 1) == 8, "2x2 and 1x1 are one block");
            SavePng(output, "dxt1_source", source);
            SavePng(output, "dxt1_decoded", decoded);
        }

        private static void Dxt5(Runner t, string output)
        {
            RgbaImage source = Gradient(64, 64, true);
            byte[] encoded = Dxt5Encoder.Encode(source.Pixels, 64, 64);
            t.Check(encoded.Length == 16 * 16 * 16, "64x64 size", encoded.Length.ToString());
            RgbaImage decoded = Decode(encoded, "DXT5", 64, 64);
            double rgb = ImageMetrics.PsnrDb(source, decoded, false), alpha = ImageMetrics.PsnrDb(source, decoded, true);
            t.Check(rgb >= MinimumGradientPsnrDb, "gradient colour PSNR >= " + MinimumGradientPsnrDb + " dB", rgb.ToString("0.0"));
            t.Check(alpha >= MinimumGradientPsnrDb, "gradient alpha PSNR >= " + MinimumGradientPsnrDb + " dB", alpha.ToString("0.0"));
            SavePng(output, "dxt5_source", source);
            SavePng(output, "dxt5_decoded", decoded);

            // Alpha blocks: uniform (any value), binary 0/255, a full ramp (8-value mode), and 0/255 plus mid values
            // (6-value mode keeps 0 and 255 exact).
            byte[] block = new byte[16];
            foreach (int value in new[] { 0, 1, 128, 254, 255 })
            {
                Dxt5Encoder.EncodeAlphaBlock(Enumerable.Repeat(value, 16).ToArray(), block, 0);
                t.Check(AlphaError(block, Enumerable.Repeat(value, 16).ToArray()) == 0, "uniform alpha " + value + " exact");
            }
            int[] binary = Enumerable.Range(0, 16).Select(i => i % 3 == 0 ? 0 : 255).ToArray();
            Dxt5Encoder.EncodeAlphaBlock(binary, block, 0);
            t.Check(AlphaError(block, binary) == 0, "binary alpha exact");
            int[] ramp = Enumerable.Range(0, 16).Select(i => i * 17).ToArray();
            Dxt5Encoder.EncodeAlphaBlock(ramp, block, 0);
            t.Check(block[0] > block[1], "ramp uses 8-value mode (a0 > a1)", block[0] + " " + block[1]);
            t.Check(AlphaError(block, ramp) <= MaxRampAlphaError, "ramp max error <= " + MaxRampAlphaError, AlphaError(block, ramp).ToString());
            int[] mixed = { 0, 255, 100, 110, 120, 0, 255, 105, 115, 0, 255, 100, 120, 0, 255, 110 };
            Dxt5Encoder.EncodeAlphaBlock(mixed, block, 0);
            t.Check(block[0] <= block[1], "0/255 plus mid values use 6-value mode (a0 <= a1)", block[0] + " " + block[1]);
            int[] decodedMixed = AlphaValues(block);
            t.Check(Enumerable.Range(0, 16).All(i => (mixed[i] != 0 && mixed[i] != 255) || decodedMixed[i] == mixed[i]), "6-value mode keeps 0 and 255 exact");
            t.Check(AlphaError(block, mixed) <= 4, "6-value mode mid values within 4", AlphaError(block, mixed).ToString());
            // Colour of a DXT5 block always decodes in 4-colour mode, whatever the endpoint order.
            RgbaImage solid = RgbaImage.Solid(4, 4, 0, 0, 0, 77);
            RgbaImage back = Decode(Dxt5Encoder.Encode(solid.Pixels, 4, 4), "DXT5", 4, 4);
            t.Check(ImageMetrics.MaxError(solid, back, false) == 0 && ImageMetrics.MaxError(solid, back, true) == 0, "black with alpha 77 exact");
        }

        private static int[] AlphaValues(byte[] block)
        {
            int[] palette = Dxt5Encoder.Palette(block[0], block[1]);
            ulong bits = 0;
            for (int i = 0; i < 6; i++) { bits |= (ulong)block[2 + i] << (8 * i); }
            return Enumerable.Range(0, 16).Select(i => palette[(int)((bits >> (3 * i)) & 7)]).ToArray();
        }

        private static int AlphaError(byte[] block, int[] expected)
        {
            int[] values = AlphaValues(block);
            return Enumerable.Range(0, 16).Max(i => Math.Abs(values[i] - expected[i]));
        }

        private static void Decoder(Runner t, string output)
        {
            // Hand-made DXT1 block in 3-colour mode (c0 <= c1): index 2 is the midpoint, index 3 transparent black.
            byte[] dxt1 = new byte[8];
            PutU16(dxt1, 0, 0x001F); // blue
            PutU16(dxt1, 2, 0xF800); // red
            uint indices = 0;
            for (int i = 0; i < 16; i++) { indices |= (uint)(i % 4) << (2 * i); }
            Buffer.BlockCopy(BitConverter.GetBytes(indices), 0, dxt1, 4, 4);
            byte[] rgba = DxtDecoder.Decode(dxt1, 0, "DXT1", 4, 4);
            t.Check(rgba[0] == 0 && rgba[2] == 255 && rgba[3] == 255, "index 0 = c0 (blue)");
            t.Check(rgba[4] == 255 && rgba[6] == 0 && rgba[7] == 255, "index 1 = c1 (red)");
            t.Check(rgba[8] == 127 && rgba[10] == 127 && rgba[11] == 255, "index 2 = midpoint in 3-colour mode", rgba[8] + "," + rgba[10]);
            t.Check(rgba[12] == 0 && rgba[13] == 0 && rgba[14] == 0 && rgba[15] == 0, "index 3 = transparent black in 3-colour mode");
            // DXT3: explicit 4-bit alpha, nibble n -> n * 17.
            byte[] dxt3 = new byte[16];
            for (int i = 0; i < 8; i++) { dxt3[i] = (byte)((i * 2) | ((i * 2 + 1) << 4)); }
            PutU16(dxt3, 8, 0xFFFF); PutU16(dxt3, 10, 0x0000);
            byte[] rgba3 = DxtDecoder.Decode(dxt3, 0, "DXT3", 4, 4);
            t.Check(Enumerable.Range(0, 16).All(i => rgba3[i * 4 + 3] == i * 17), "DXT3 alpha nibbles");
            t.Check(rgba3[0] == 255 && rgba3[1] == 255 && rgba3[2] == 255, "DXT3 colour index 0 = c0");
            // Offsets and bounds.
            byte[] padded = new byte[3 + 8];
            Buffer.BlockCopy(dxt1, 0, padded, 3, 8);
            t.Check(DxtDecoder.Decode(padded, 3, "DXT1", 4, 4).SequenceEqual(rgba), "decodes at an offset");
            t.Throws<ArgumentException>(() => DxtDecoder.Decode(dxt1, 1, "DXT1", 4, 4), "rejects data running past the buffer");
            t.Throws<ArgumentException>(() => DxtDecoder.BlockBytes("ATI2"), "rejects non-DXT formats");
        }

        private static void MipChain(Runner t, string output)
        {
            t.Check(TextureEncoder.FullChainLevels(256, 256) == 7, "256x256 has 7 levels (as amb_nailgun.wtd)");
            t.Check(TextureEncoder.FullChainLevels(128, 128) == 6 && TextureEncoder.FullChainLevels(64, 64) == 5, "128 -> 6, 64 -> 5 levels");
            t.Check(TextureEncoder.FullChainLevels(4, 4) == 1 && TextureEncoder.FullChainLevels(2048, 4) == 1, "smaller side 4 -> 1 level");
            t.Check(TextureEncoder.FullChainLevels(512, 64) == 5 && TextureEncoder.FullChainLevels(2048, 2048) == 10, "512x64 -> 5, 2048 -> 10 levels");
            NativeTexture texture = TextureEncoder.Encode("chain", Gradient(128, 32, false), "DXT1", 0);
            t.Check(texture.Levels.Count == 4, "128x32 full chain = 4 levels", texture.Levels.Count.ToString());
            int[] expected = { 128 * 32 / 2, 64 * 16 / 2, 32 * 8 / 2, 16 * 4 / 2 };
            t.Check(texture.Levels.Select(l => l.Length).SequenceEqual(expected), "level sizes", string.Join(",", texture.Levels.Select(l => l.Length.ToString()).ToArray()));
            t.Check(TextureEncoder.Encode("one", Gradient(16, 16, false), "DXT5", 1).Levels.Count == 1, "levels = 1 writes the top level only");
            t.Throws<ArgumentException>(() => TextureEncoder.Encode("bad", Gradient(12, 16, false), "DXT1", 0), "rejects a non-power-of-two size");
            t.Throws<ArgumentException>(() => TextureEncoder.Encode("bad", Gradient(2, 2, false), "DXT1", 0), "rejects sizes below 4");
            t.Throws<ArgumentException>(() => TextureEncoder.Encode("bad", Gradient(16, 16, false), "DXT1", 4), "rejects more levels than the chain has");
            t.Throws<ArgumentException>(() => TextureEncoder.Encode("bad", Gradient(16, 16, false), "DXT3", 0), "rejects formats it does not encode");

            RgbaImage checker = new RgbaImage(2, 2, new byte[] { 0, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 0, 0, 0, 0 });
            RgbaImage half = checker.Half();
            t.Check(half.Width == 1 && half.Height == 1 && half.Pixels.All(p => p == 128), "2x2 box filter rounds to 128", string.Join(",", half.Pixels.Select(p => p.ToString()).ToArray()));
            t.Check(new RgbaImage(4, 1).Half().Width == 2 && new RgbaImage(4, 1).Half().Height == 1, "halving clamps at 1 pixel");
            t.Check(Gradient(8, 8, true).HasTranslucency() && !Gradient(8, 8, false).HasTranslucency(), "translucency detection");

            RgbaImage gradient = Gradient(64, 32, true);
            t.Check(gradient.Resize(64, 32).Pixels.SequenceEqual(gradient.Pixels), "resize to the same size is an exact copy");
            t.Check(!Gradient(100, 30, false).Resize(128, 32).HasTranslucency() && !Gradient(100, 30, false).Resize(16, 64).HasTranslucency(), "resizing keeps an opaque image opaque");
            RgbaImage flat = RgbaImage.Solid(7, 5, 10, 200, 90, 130).Resize(32, 8);
            t.Check(Enumerable.Range(0, 32 * 8).All(i => flat.Pixels[i * 4] == 10 && flat.Pixels[i * 4 + 1] == 200 && flat.Pixels[i * 4 + 2] == 90 && flat.Pixels[i * 4 + 3] == 130), "a flat colour stays flat");
            RgbaImage stripes = new RgbaImage(8, 2);
            for (int i = 0; i < 16; i++) { byte v = (byte)((i % 8) % 2 == 0 ? 0 : 255); stripes.Pixels[i * 4] = v; stripes.Pixels[i * 4 + 3] = 255; }
            RgbaImage averaged = stripes.Resize(4, 1);
            t.Check(Math.Abs(averaged.Pixels[4] - 128) <= 1 && Math.Abs(averaged.Pixels[8] - 128) <= 1 && Math.Abs(Enumerable.Range(0, 4).Average(i => averaged.Pixels[i * 4]) - 127.5) <= 1,
                "halving 1-pixel stripes: interior samples average them, the mean is kept", string.Join(",", Enumerable.Range(0, 4).Select(i => averaged.Pixels[i * 4].ToString()).ToArray()));
            RgbaImage wide = Gradient(64, 4, false).Resize(256, 4);
            t.Check(Enumerable.Range(1, 255).All(x => wide.Pixels[x * 4] >= wide.Pixels[(x - 1) * 4]), "enlarging a ramp stays monotonic");

            t.Check(PropCompiler.NativeSide(256) == 256 && PropCompiler.NativeSide(300) == 256 && PropCompiler.NativeSide(400) == 512, "native size rounds on a log scale");
            t.Check(PropCompiler.NativeSide(1) == TextureEncoder.MinSizePixels && PropCompiler.NativeSide(10000) == TextureEncoder.MaxSizePixels, "native size clamps to 4-2048");
        }

        // Textures with deterministic level bytes of the right sizes (the writer does not look inside them).
        private static NativeTexture Synthetic(string name, string format, int width, int height, int levels, int seed)
        {
            NativeTexture texture = new NativeTexture { Name = name, Format = format, Width = width, Height = height };
            for (int level = 0; level < levels; level++)
            {
                byte[] bytes = new byte[DxtDecoder.LevelBytes(format, Math.Max(1, width >> level), Math.Max(1, height >> level))];
                for (int i = 0; i < bytes.Length; i++) { bytes[i] = (byte)(seed * 31 + level * 7 + i * 13 + (i >> 8)); }
                texture.Levels.Add(bytes);
            }
            return texture;
        }

        // Every texture parses back with its name, size, format, level count and exact level bytes.
        private static void CheckReadsBack(Runner t, string label, byte[] file, IList<NativeTexture> textures)
        {
            RscResource resource = RscResource.Parse(file);
            TextureDictionary parsed = TextureDictionary.Parse(resource);
            t.Check(parsed.Textures.Count == textures.Count, label + ": texture count", parsed.Textures.Count.ToString());
            foreach (NativeTexture texture in textures)
            {
                TextureDictionary.Texture back = parsed.Textures.FirstOrDefault(p => p.Name == texture.Name);
                if (back == null) { t.Check(false, label + ": " + texture.Name + " present"); continue; }
                t.Check(back.Width == texture.Width && back.Height == texture.Height && back.Format == texture.Format && back.Levels == texture.Levels.Count,
                    label + ": " + texture.Name + " header", back.Width + "x" + back.Height + " " + back.Format + " " + back.Levels);
                int at = back.DataOffset;
                bool same = true;
                for (int level = 0; level < texture.Levels.Count && same; level++)
                {
                    byte[] bytes = texture.Levels[level];
                    if (back.LevelBytes(level) != bytes.Length || at + bytes.Length > resource.Body.Length) { same = false; break; }
                    for (int k = 0; k < bytes.Length; k++) { if (resource.Body[at + k] != bytes[k]) { same = false; break; } }
                    at += bytes.Length;
                }
                t.Check(same, label + ": " + texture.Name + " level bytes");
            }
        }

        private static void Writer(Runner t, string output)
        {
            TextureDictionaryPrototype builtin = TextureDictionaryPrototype.Builtin();

            // One 256x256 DXT1 texture with the full chain: the shape of amb_nailgun.wtd.
            NativeTexture single = TextureEncoder.Encode("lf_test_crate", Gradient(256, 256, false), "DXT1", 0);
            TextureDictionaryWriter.Output written = TextureDictionaryWriter.Write(new List<NativeTexture> { single }, builtin);
            byte[] file = written.Resource.Serialize();
            CheckReadsBack(t, "single", file, new[] { single });
            RscResource resource = RscResource.Parse(file);
            byte[] body = resource.Body;
            t.Check(resource.Type == TextureDictionaryPrototype.TextureDictionaryResourceType, "resource type 8");
            t.Check((resource.Flags & TextureDictionaryPrototype.UnknownFlagMask) == builtin.FlagBits, "flag bits 30-31 from the prototype");
            t.Check(resource.SystemSize % TextureDictionaryWriter.SystemSizeAlignmentBytes == 0, "system size is a multiple of 4 KB", resource.SystemSize.ToString());
            t.Check(written.PageCount == 1 && resource.GraphicsSize == written.PageBytes && written.PageBytes >= single.TotalBytes, "graphics segment is one page",
                written.PageCount + " x " + written.PageBytes);
            t.Check(BitConverter.ToUInt32(body, 0x00) == 0x00695384u && BitConverter.ToUInt32(body, 0x0C) == 1, "opaque header bytes from the prototype");
            t.Check(BitConverter.ToUInt32(body, 0x04) == 0x50000020u, "block map pointer +0x20");
            t.Check(Enumerable.Range(TextureDictionaryPrototype.BlockMapOffset + 4, TextureDictionaryPrototype.BlockMapBytes - 4).All(i => body[i] == 0xCD) &&
                BitConverter.ToUInt32(body, TextureDictionaryPrototype.BlockMapOffset) == 0, "block map is a zero word and 0xCD fill");
            t.Check(BitConverter.ToUInt16(body, 0x14) == 1 && BitConverter.ToUInt16(body, 0x16) == 1 && BitConverter.ToUInt16(body, 0x1C) == 1 && BitConverter.ToUInt16(body, 0x1E) == 1,
                "hash and texture collection count/capacity");
            int record = (int)(BitConverter.ToUInt32(body, (int)(BitConverter.ToUInt32(body, 0x18) & 0x0FFFFFFF)) & 0x0FFFFFFF);
            t.Check(record == TextureDictionaryWriter.FirstRecordOffset, "first record at +0x230", "0x" + record.ToString("X"));
            t.Check(BitConverter.ToUInt32(body, record) == 0x006B1D94u && BitConverter.ToUInt16(body, record + 0x24) == 128, "record vtable slot and 256-wide DXT1 stride 128");
            t.Check(BitConverter.ToUInt32(body, (int)(BitConverter.ToUInt32(body, 0x10) & 0x0FFFFFFF)) == TextureNameHash.Compute("lf_test_crate"), "hash array holds the name hash");
            int name = (int)(BitConverter.ToUInt32(body, record + 0x14) & 0x0FFFFFFF);
            t.Check(Encoding.ASCII.GetString(body, name, "pack:/lf_test_crate.dds".Length + 1) == "pack:/lf_test_crate.dds\0", "stored name pack:/<name>.dds");
            t.Check(TextureDictionaryWriter.RowStrideBytes("DXT5", 256) == 256 && TextureDictionaryWriter.RowStrideBytes("DXT1", 4) == 2, "row stride rule (DXT5 256 -> 256, DXT1 4 -> 2)");
            t.Check(TextureDictionaryWriter.Write(new List<NativeTexture> { single }, builtin).Resource.Serialize().SequenceEqual(file), "deterministic output");
            if (output != null) { File.WriteAllBytes(Path.Combine(output, "selftest_single.wtd"), file); }

            // Several textures, mixed formats, sizes and name case: hash order, alignment, no overlap.
            List<NativeTexture> mixed = new List<NativeTexture>
            {
                Synthetic("Zeta_Diffuse", "DXT1", 512, 128, TextureEncoder.FullChainLevels(512, 128), 1),
                Synthetic("alpha_mask", "DXT5", 64, 64, 5, 2),
                Synthetic("detail", "DXT3", 32, 32, 1, 3),
                Synthetic("tiny", "DXT1", 4, 4, 1, 4),
                TextureEncoder.Encode("gradient_alpha", Gradient(128, 128, true), "DXT5", 0),
            };
            TextureDictionaryWriter.Output many = TextureDictionaryWriter.Write(mixed, builtin);
            byte[] manyFile = many.Resource.Serialize();
            CheckReadsBack(t, "mixed", manyFile, mixed);
            RscResource manyResource = RscResource.Parse(manyFile);
            byte[] manyBody = manyResource.Body;
            int hashes = (int)(BitConverter.ToUInt32(manyBody, 0x10) & 0x0FFFFFFF);
            List<uint> stored = Enumerable.Range(0, mixed.Count).Select(i => BitConverter.ToUInt32(manyBody, hashes + i * 4)).ToList();
            t.Check(stored.Zip(stored.Skip(1), (a, b) => a < b).All(x => x), "hash array ascending");
            TextureDictionary parsed = TextureDictionary.Parse(manyResource);
            t.Check(parsed.Textures.Select(p => TextureNameHash.Compute(p.Name)).SequenceEqual(stored), "texture pointer array in hash order");
            t.Check(many.Textures.All(p => p.DataOffset % TextureDictionaryWriter.TextureDataAlignmentBytes == 0), "texture data 256-byte aligned");
            List<Tuple<int, int>> ranges = many.Textures.Select(p => Tuple.Create(p.DataOffset, p.DataOffset + p.Texture.TotalBytes)).OrderBy(r => r.Item1).ToList();
            t.Check(ranges.Zip(ranges.Skip(1), (a, b) => a.Item2 <= b.Item1).All(x => x) && ranges.Last().Item2 <= manyResource.GraphicsSize, "texture data ranges do not overlap");
            t.Check(many.PageCount == 1, "small dictionary is one page");
            t.Check(many.Textures.All(p => p.RecordOffset % 16 == 0 && p.NameOffset % 16 == 0), "records and names 16-byte aligned");
            if (output != null) { File.WriteAllBytes(Path.Combine(output, "selftest_mixed.wtd"), manyFile); }

            // What `wtdcheck` does with a game dictionary, applied to the writer's own output: capture the prototype from the
            // file, compare it with the builtin one, rebuild from the file's textures and compare everything but placement.
            foreach (KeyValuePair<string, byte[]> sample in new[] { new KeyValuePair<string, byte[]>("single", file), new KeyValuePair<string, byte[]>("mixed", manyFile) })
            {
                RscResource original = RscResource.Parse(sample.Value);
                TextureDictionaryPrototype captured = TextureDictionaryPrototype.FromResource(original, sample.Key);
                List<string> versusBuiltin = captured.OpaqueDifferences(builtin);
                t.Check(versusBuiltin.Count == 0, sample.Key + ": captured prototype equals builtin", string.Join(", ", versusBuiltin.ToArray()));
                List<NativeTexture> sources = new List<NativeTexture>();
                foreach (TextureDictionary.Texture texture in TextureDictionary.Parse(original).Textures)
                {
                    NativeTexture native = new NativeTexture { Name = texture.Name, Format = texture.Format, Width = texture.Width, Height = texture.Height };
                    int at = texture.DataOffset;
                    for (int level = 0; level < Math.Max(1, texture.Levels); level++) { byte[] bytes = new byte[texture.LevelBytes(level)]; Buffer.BlockCopy(original.Body, at, bytes, 0, bytes.Length); native.Levels.Add(bytes); at += bytes.Length; }
                    sources.Add(native);
                }
                RscResource rebuilt = RscResource.Parse(TextureDictionaryWriter.Write(sources, captured).Resource.Serialize());
                List<string> differences = TextureDictionaryCheck.Compare(original, rebuilt);
                t.Check(differences.Count == 0, sample.Key + ": rebuild compares identical", string.Join("; ", differences.Take(4).ToArray()));
                t.Check(rebuilt.Body.SequenceEqual(original.Body) && rebuilt.Flags == original.Flags, sample.Key + ": rebuild is byte-identical");
            }
            // Compare really detects a change (a pixel byte and an opaque record byte).
            RscResource altered = RscResource.Parse(file);
            TextureDictionary.Texture only = TextureDictionary.Parse(altered).Textures[0];
            altered.Body[only.DataOffset + 5] ^= 0xFF;
            altered.Body[record + 0x30] ^= 0x01;
            List<string> found = TextureDictionaryCheck.Compare(RscResource.Parse(file), altered);
            t.Check(found.Any(d => d.Contains("pixel data")) && found.Any(d => d.Contains("record+0x30")), "compare reports pixel and record differences", string.Join("; ", found.ToArray()));
        }

        private static void MultiPage(Runner t, string output)
        {
            // Three 2048x2048 DXT5 full chains (about 5.6 MB each) exceed one 8 MB page: 8 MB pages, no texture straddles one.
            List<NativeTexture> large = Enumerable.Range(0, 3).Select(i => Synthetic("large_" + i, "DXT5", 2048, 2048, TextureEncoder.FullChainLevels(2048, 2048), 10 + i)).ToList();
            TextureDictionaryWriter.Output written = TextureDictionaryWriter.Write(large, TextureDictionaryPrototype.Builtin());
            t.Check(written.PageBytes == TextureDictionaryWriter.MaxPageBytes, "8 MB pages", written.PageBytes.ToString());
            t.Check(written.PageCount == 3, "three pages", written.PageCount.ToString());
            t.Check(written.Textures.All(p => p.DataOffset / written.PageBytes == (p.DataOffset + p.Texture.TotalBytes - 1) / written.PageBytes), "no texture straddles a page");
            CheckReadsBack(t, "multi-page", written.Resource.Serialize(), large);

            // Exactly one page's worth still fits one page; one byte more would not be written as a single page.
            NativeTexture fits = Synthetic("fits", "DXT5", 2048, 2048, 1, 20); // 4 MB
            TextureDictionaryWriter.Output one = TextureDictionaryWriter.Write(new List<NativeTexture> { fits, Synthetic("fits2", "DXT5", 2048, 2048, 1, 21) }, TextureDictionaryPrototype.Builtin());
            t.Check(one.PageCount == 1 && one.PageBytes == TextureDictionaryWriter.MaxPageBytes, "8 MB of data is one 8 MB page", one.PageCount + " x " + one.PageBytes);
        }

        private static void WriterErrors(Runner t, string output)
        {
            TextureDictionaryPrototype builtin = TextureDictionaryPrototype.Builtin();
            Func<NativeTexture> ok = () => Synthetic("ok", "DXT1", 16, 16, 3, 0);
            t.Throws<ArgumentException>(() => TextureDictionaryWriter.Write(new List<NativeTexture>(), builtin), "rejects an empty dictionary");
            t.Throws<ArgumentNullException>(() => TextureDictionaryWriter.Write(new List<NativeTexture> { ok() }, null), "rejects a missing prototype");
            t.Throws<ArgumentException>(() => TextureDictionaryWriter.Write(new List<NativeTexture> { Synthetic("Same", "DXT1", 8, 8, 1, 0), Synthetic("same", "DXT1", 8, 8, 1, 1) }, builtin),
                "rejects names that differ only in case (same hash)");
            foreach (string bad in new[] { "", "a/b", "a b", "c:x", new string('x', TextureDictionaryWriter.MaxNameLength + 1) })
            {
                NativeTexture texture = ok(); texture.Name = bad;
                t.Throws<ArgumentException>(() => TextureDictionaryWriter.Write(new List<NativeTexture> { texture }, builtin), "rejects name '" + (bad.Length > 10 ? bad.Substring(0, 10) + "..." : bad) + "'");
            }
            NativeTexture shortLevel = ok(); shortLevel.Levels[1] = new byte[3];
            t.Throws<ArgumentException>(() => TextureDictionaryWriter.Write(new List<NativeTexture> { shortLevel }, builtin), "rejects a level of the wrong size");
            NativeTexture format = ok(); format.Format = "ARGB";
            t.Throws<ArgumentException>(() => TextureDictionaryWriter.Write(new List<NativeTexture> { format }, builtin), "rejects an unknown format");
            NativeTexture noLevels = ok(); noLevels.Levels.Clear();
            t.Throws<ArgumentException>(() => TextureDictionaryWriter.Write(new List<NativeTexture> { noLevels }, builtin), "rejects a texture without levels");
            // Captured prototypes: a resource whose block map pointer is not +0x20 is refused.
            RscResource resource = RscResource.Parse(TextureDictionaryWriter.Write(new List<NativeTexture> { ok() }, builtin).Resource.Serialize());
            resource.Body[0x04] = 0x40;
            t.Throws<InvalidDataException>(() => TextureDictionaryPrototype.FromResource(resource, "moved block map"), "prototype capture refuses a moved block map");
        }

        private static void NativePipeline(Runner t, string output)
        {
            // What PropCompiler's native mode does after picking the source pixels: encode, write, then Readback's check.
            foreach (bool alpha in new[] { false, true })
            {
                string format = alpha ? "DXT5" : "DXT1";
                RgbaImage source = Gradient(128, 64, alpha);
                NativeTexture texture = TextureEncoder.Encode("lf_native_" + format.ToLowerInvariant(), source, format, 0);
                byte[] file = TextureDictionaryWriter.Write(new List<NativeTexture> { texture }, TextureDictionaryPrototype.Builtin()).Resource.Serialize();
                TextureQuality quality;
                List<string> problems = Readback.VerifyNativeTexture(file, texture, source, out quality);
                t.Check(problems.Count == 0, format + ": read-back passes", string.Join("; ", problems.ToArray()));
                t.Check(quality != null && quality.PsnrRgbDb >= MinimumGradientPsnrDb, format + ": colour PSNR", quality == null ? "none" : quality.PsnrRgbDb.ToString("0.0"));
                t.Check(quality != null && quality.HasAlpha == alpha && (!alpha || quality.PsnrAlphaDb >= MinimumGradientPsnrDb), format + ": alpha measured for DXT5 only",
                    quality == null ? "none" : quality.HasAlpha + " " + quality.PsnrAlphaDb.ToString("0.0"));
                if (output != null) { File.WriteAllBytes(Path.Combine(output, "selftest_native_" + format.ToLowerInvariant() + ".wtd"), file); }

                // The check catches a corrupted level, a wrong format and wrong source pixels.
                RscResource corrupt = RscResource.Parse(file);
                TextureDictionary.Texture back = TextureDictionary.Parse(corrupt).Textures[0];
                corrupt.Body[back.DataOffset + back.LevelBytes(0) + 1] ^= 0x55;
                t.Check(Readback.VerifyNativeTexture(corrupt.Serialize(), texture, source, out quality).Any(p => p.Contains("mip level 1")), format + ": corrupted level 1 is reported");
                NativeTexture renamed = new NativeTexture { Name = texture.Name, Format = alpha ? "DXT1" : "DXT5", Width = texture.Width, Height = texture.Height };
                t.Check(Readback.VerifyNativeTexture(file, renamed, source, out quality).Any(p => p.Contains("format")), format + ": format mismatch is reported");
                RgbaImage swapped = new RgbaImage(source.Width, source.Height, (byte[])source.Pixels.Clone());
                for (int i = 0; i < swapped.Pixels.Length; i += 4) { byte r = swapped.Pixels[i]; swapped.Pixels[i] = swapped.Pixels[i + 2]; swapped.Pixels[i + 2] = r; }
                t.Check(Readback.VerifyNativeTexture(file, texture, swapped, out quality).Any(p => p.Contains("PSNR")), format + ": swapped red/blue source fails the PSNR floor",
                    quality == null ? "none" : quality.PsnrRgbDb.ToString("0.0"));
            }
        }

        // A one-mesh asset whose material has the given alpha mode and image (null: no texture).
        private static ContentAsset TexturedAsset(string alphaMode, Bitmap image, float[] baseColour)
        {
            ContentAsset asset = new ContentAsset { Name = "selftest", SourcePath = "selftest.gltf" };
            ContentMaterial material = new ContentMaterial { Name = "m", AlphaMode = alphaMode, BaseColour = baseColour };
            if (image != null) { material.Image = 0; asset.Images.Add(image); asset.ImageNames.Add("image"); }
            asset.Materials.Add(material);
            asset.Meshes.Add(Box("mesh", 0, 0, 1));
            return asset;
        }

        private static Bitmap MakeBitmap(int width, int height, Func<int, int, Color> pixel)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < height; y++) { for (int x = 0; x < width; x++) { bitmap.SetPixel(x, y, pixel(x, y)); } }
            return bitmap;
        }

        // PropCompiler's choice of native pixels and format (the part of native mode that needs no template).
        private static void NativeSource(Runner t, string output)
        {
            float[] white = { 1, 1, 1, 1 };
            using (Bitmap translucent = MakeBitmap(64, 64, (x, y) => Color.FromArgb(x < 32 ? 255 : 100, 200, 40, 10)))
            using (Bitmap opaque = MakeBitmap(100, 30, (x, y) => Color.FromArgb(255, x * 2, y * 8, 50)))
            {
                List<string> notes = new List<string>();
                string format;
                RgbaImage pixels = PropCompiler.NativeSourcePixels(TexturedAsset("BLEND", translucent, white), notes, out format);
                t.Check(format == "DXT5" && pixels.Width == 64 && pixels.Height == 64, "BLEND with translucent pixels -> DXT5 at source size", format + " " + pixels.Width);
                t.Check(pixels.Pixels[0] == 200 && pixels.Pixels[1] == 40 && pixels.Pixels[2] == 10 && pixels.Pixels[3] == 255, "RGBA order kept", string.Join(",", pixels.Pixels.Take(4).Select(p => p.ToString()).ToArray()));
                t.Check(pixels.Pixels[(10 * 64 + 50) * 4 + 3] == 100, "alpha kept", pixels.Pixels[(10 * 64 + 50) * 4 + 3].ToString());
                PropCompiler.NativeSourcePixels(TexturedAsset("OPAQUE", translucent, white), notes, out format);
                t.Check(format == "DXT1", "OPAQUE -> DXT1 even with translucent pixels");
                notes.Clear();
                pixels = PropCompiler.NativeSourcePixels(TexturedAsset("MASK", opaque, new float[] { 1, 0.5f, 1, 1 }), notes, out format);
                t.Check(format == "DXT1" && notes.Any(n => n.Contains("every pixel is opaque")), "MASK with only opaque pixels -> DXT1 with a note", string.Join("; ", notes.ToArray()));
                t.Check(pixels.Width == 128 && pixels.Height == 32 && notes.Any(n => n.Contains("100x30 -> 128x32")), "100x30 resampled to 128x32 with a note", pixels.Width + "x" + pixels.Height);
                t.Check(pixels.Pixels[(16 * 128 + 64) * 4 + 1] <= 128, "base colour factor multiplies the texture", pixels.Pixels[(16 * 128 + 64) * 4 + 1].ToString());
            }
            List<string> solidNotes = new List<string>();
            string solidFormat;
            RgbaImage solid = PropCompiler.NativeSourcePixels(TexturedAsset("BLEND", null, new float[] { 1, 0, 0, 0.5f }), solidNotes, out solidFormat);
            t.Check(solid.Width == PropCompiler.SolidColourTextureSizePixels && solidFormat == "DXT5" && solid.Pixels[0] == 255 && solid.Pixels[3] == 128,
                "no texture: base colour square, translucent base colour -> DXT5", solidFormat + " " + string.Join(",", solid.Pixels.Take(4).Select(p => p.ToString()).ToArray()));
        }

        // --- validator ---

        private static ContentMesh Box(string name, int lod, int material, float size)
        {
            ContentMesh mesh = new ContentMesh { Name = name, Lod = lod, Material = material, HadNormals = true, HadUvs = true };
            float[][] corners = { new[] { 0f, 0f, 0f }, new[] { size, 0f, 0f }, new[] { size, size, 0f }, new[] { 0f, size, 0f }, new[] { 0f, 0f, size }, new[] { size, 0f, size }, new[] { size, size, size }, new[] { 0f, size, size } };
            foreach (float[] c in corners)
            {
                float length = (float)Math.Sqrt(3);
                mesh.Vertices.Add(new ContentVertex { X = c[0] - size / 2, Y = c[1] - size / 2, Z = c[2], NX = 1 / length, NY = 1 / length, NZ = 1 / length, Colour = 0xFFFFFFFF });
            }
            int[] faces = { 0, 2, 1, 0, 3, 2, 4, 5, 6, 4, 6, 7, 0, 1, 5, 0, 5, 4, 1, 2, 6, 1, 6, 5, 2, 3, 7, 2, 7, 6, 3, 0, 4, 3, 4, 7 };
            mesh.Indices.AddRange(faces);
            return mesh;
        }

        private static ContentAsset Asset(params ContentMesh[] meshes)
        {
            ContentAsset asset = new ContentAsset { Name = "selftest", SourcePath = "selftest.gltf" };
            asset.Materials.Add(new ContentMaterial { Name = "first" });
            asset.Materials.Add(new ContentMaterial { Name = "second" });
            asset.Meshes.AddRange(meshes);
            return asset;
        }

        private static List<string> Codes(ContentAsset asset, CompilerCapabilities capabilities, string severity)
        {
            return AssetValidator.Validate(asset, capabilities).Where(i => severity == null || i.Severity == severity).Select(i => i.Code).ToList();
        }

        private static void Validator(Runner t, string output)
        {
            CompilerCapabilities v1 = CompilerCapabilities.Current;
            t.Check(v1.MaxMaterialsPerLod == 1 && v1.CompiledLodLevels == 1, "current capabilities are v1 (1 material, LOD 0)");
            t.Check(Codes(Asset(Box("crate", 0, 0, 1)), v1, "error").Count == 0, "one box, one material: no errors", string.Join(",", Codes(Asset(Box("crate", 0, 0, 1)), v1, "error").ToArray()));

            ContentAsset twoMaterials = Asset(Box("a", 0, 0, 1), Box("b", 0, 1, 0.5f));
            t.Check(Codes(twoMaterials, v1, "error").Contains("LCC016"), "two materials in LOD 0: LCC016 for v1");
            t.Check(!Codes(twoMaterials, new CompilerCapabilities("test", 2, 1), null).Contains("LCC016"), "LCC016 follows the capabilities (2 materials allowed)");
            t.Check(!Codes(Asset(Box("a", 0, 0, 1), Box("a2", 0, 0, 0.5f)), v1, null).Contains("LCC016"), "two meshes of one material are one group");

            ContentAsset lods = Asset(Box("lod0", 0, 0, 1), Box("lod1", 1, 0, 1));
            List<string> lodCodes = Codes(lods, v1, null);
            t.Check(lodCodes.Contains("LCC025") && !lodCodes.Contains("LCC018"), "LOD 1 validated but not compiled: LCC025 (info)");
            t.Check(lodCodes.Contains("LCC017"), "LOD 1 not lighter than LOD 0: LCC017");
            t.Check(!Codes(lods, new CompilerCapabilities("test", 1, 2), null).Contains("LCC025"), "LCC025 follows the capabilities (2 LODs compiled)");
            t.Check(Codes(Asset(Box("lod1", 1, 0, 1)), v1, "error").Contains("LCC018"), "no LOD 0: LCC018");
            t.Check(Codes(Asset(Box("lod0", 0, 0, 1), Box("lod4", 4, 0, 1)), v1, "error").Contains("LCC024"), "LOD 4: LCC024");
            t.Check(Codes(Asset(Box("lod0", 0, 0, 1), Box("neg", -1, 0, 1)), v1, "error").Contains("LCC024"), "negative LOD: LCC024");

            ContentMesh huge = new ContentMesh { Name = "huge", Lod = 0, Material = 0, HadNormals = true, HadUvs = true };
            for (int i = 0; i <= CompilerCapabilities.MaxVerticesPerGeometry; i++) { huge.Vertices.Add(new ContentVertex { X = i * 1e-4f, Y = (i % 7) * 0.01f, Z = (i % 3) * 0.01f, NZ = 1 }); }
            huge.Indices.AddRange(new[] { 0, 1, 2 });
            t.Check(Codes(Asset(huge), v1, "error").Contains("LCC004"), "65,536 vertices in one material group: LCC004");

            ContentAsset empty = new ContentAsset { Name = "empty", SourcePath = "empty.gltf" };
            t.Check(Codes(empty, v1, "error").SequenceEqual(new[] { "LCC001" }), "no meshes: LCC001 only");
            ContentAsset alpha = Asset(Box("glass", 0, 0, 1));
            alpha.Materials[0].AlphaMode = "BLEND";
            t.Check(Codes(alpha, v1, "warning").Contains("LCC020") && Codes(alpha, v1, "error").Count == 0, "alpha material: LCC020 warning, not an error");

            List<ContentLod> built = Asset(Box("b", 0, 1, 1), Box("a", 0, 0, 1), Box("c", 2, 0, 1), Box("d", 0, -1, 1)).BuildLods();
            t.Check(built.Select(l => l.Level).SequenceEqual(new[] { 0, 2 }), "BuildLods orders levels");
            t.Check(built[0].Groups.Select(g => g.Material).SequenceEqual(new[] { -1, 0, 1 }), "BuildLods orders material groups (-1 first)");
            t.Check(built[0].TriangleCount == 36 && built[0].VertexCount == 24, "LOD totals", built[0].TriangleCount + " " + built[0].VertexCount);
        }

        private static AssetManifest ParseManifest(string json)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json))) { return AssetManifest.Parse(stream, "test.json"); }
        }

        private static void Manifest(Runner t, string output)
        {
            const string head = "{\"schemaVersion\":1,\"name\":\"lf_test\",\"type\":\"prop\",\"source\":\"lf_test.gltf\",\"template\":{\"archive\":\"pc/models/cdimages/weapons.img\",\"model\":\"amb_nailgun\"},\"textureDictionary\":\"lf_test\",\"drawDistanceMeters\":100";
            t.Check(ParseManifest(head + "}").TextureMode == AssetManifest.TextureModeTemplate, "textureMode defaults to template");
            t.Check(ParseManifest(head + ",\"textureMode\":\"native\"}").TextureMode == AssetManifest.TextureModeNative, "textureMode native");
            t.Throws<InvalidDataException>(() => ParseManifest(head + ",\"textureMode\":\"dds\"}"), "unknown textureMode is refused");
            t.Throws<InvalidDataException>(() => ParseManifest(head.Replace("\"schemaVersion\":1", "\"schemaVersion\":2") + "}"), "schemaVersion 2 is refused");
            t.Throws<InvalidDataException>(() => ParseManifest(head.Replace("\"name\":\"lf_test\"", "\"name\":\"" + new string('n', 24) + "\"") + "}"), "names over 23 characters are refused");
            t.Throws<InvalidDataException>(() => ParseManifest(head.Replace("\"drawDistanceMeters\":100", "\"drawDistanceMeters\":0") + "}"), "draw distance 0 is refused");
        }

        private static void Sample(Runner t, string output)
        {
            string folder = Path.Combine(Path.GetTempPath(), "lcc_selftest_" + Guid.NewGuid().ToString("N"));
            try
            {
                SampleAsset.Write(folder, "lf_selftest_crate");
                ContentAsset asset = GltfImporter.Import(Path.Combine(folder, "lf_selftest_crate.gltf"));
                List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, CompilerCapabilities.Current);
                t.Check(!AssetValidator.HasErrors(issues), "sample crate validates", string.Join("; ", issues.Where(i => i.Severity == "error").Select(i => i.ToString()).ToArray()));
                t.Check(asset.Meshes.Count > 0 && asset.Meshes.All(m => m.Lod == 0 && m.HadNormals && m.HadUvs), "sample crate imports with normals and UVs");
                ContentMaterial material = asset.Materials.FirstOrDefault();
                t.Check(material != null && material.Image >= 0 && asset.Images[material.Image] != null, "sample crate texture decodes");
                if (material != null && material.Image >= 0 && asset.Images[material.Image] != null)
                {
                    Bitmap image = asset.Images[material.Image];
                    int width = PropCompiler.NativeSide(image.Width), height = PropCompiler.NativeSide(image.Height);
                    RgbaImage pixels = RgbaImage.FromBitmap(image, width, height, material.BaseColour);
                    NativeTexture texture = TextureEncoder.Encode("lf_selftest_crate", pixels, "DXT1", 0);
                    byte[] file = TextureDictionaryWriter.Write(new List<NativeTexture> { texture }, TextureDictionaryPrototype.Builtin()).Resource.Serialize();
                    TextureQuality quality;
                    List<string> problems = Readback.VerifyNativeTexture(file, texture, pixels, out quality);
                    t.Check(problems.Count == 0, "sample crate texture through the native writer reads back", string.Join("; ", problems.ToArray()));
                    if (output != null)
                    {
                        SavePng(output, "sample_crate_source", pixels);
                        RscResource resource = RscResource.Parse(file);
                        TextureDictionary.Texture back = TextureDictionary.Parse(resource).Textures[0];
                        SavePng(output, "sample_crate_decoded", new RgbaImage(back.Width, back.Height, DxtDecoder.Decode(resource.Body, back.DataOffset, back.Format, back.Width, back.Height)));
                        File.WriteAllBytes(Path.Combine(output, "selftest_sample_crate.wtd"), file);
                    }
                }
            }
            finally
            {
                try { if (Directory.Exists(folder)) { Directory.Delete(folder, true); } }
                catch (IOException error) { Console.WriteLine("  (could not remove " + folder + ": " + error.Message + ")"); }
            }
        }

        private static void PutU16(byte[] b, int at, int value) { b[at] = (byte)value; b[at + 1] = (byte)(value >> 8); }
    }
}

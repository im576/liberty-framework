using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace LibertyFramework.Content
{
    // Writes a small, original test asset as glTF (what Blender's exporter produces: Y-up, metres, a PNG base colour):
    // a 0.6 m wooden supply crate with a Liberty stencil, sitting on Z=0 in GTA space. Lets the whole pipeline run
    // end to end without Blender.
    internal static class SampleAsset
    {
        internal static void Write(string directory, string name)
        {
            Directory.CreateDirectory(directory);
            List<float> positions = new List<float>(), normals = new List<float>(), uvs = new List<float>();
            List<ushort> indices = new List<ushort>();
            float h = 0.3f;
            // Faces in glTF space (Y up). Each: normal, then four corners counter-clockwise seen from outside.
            float[][] faces =
            {
                new float[] { 0, 1, 0,  -h, 2*h,  h,   h, 2*h,  h,   h, 2*h, -h,  -h, 2*h, -h },
                new float[] { 0, -1, 0, -h, 0, -h,   h, 0, -h,   h, 0,  h,  -h, 0,  h },
                new float[] { 0, 0, 1,  -h, 0,  h,   h, 0,  h,   h, 2*h,  h,  -h, 2*h,  h },
                new float[] { 0, 0, -1,  h, 0, -h,  -h, 0, -h,  -h, 2*h, -h,   h, 2*h, -h },
                new float[] { 1, 0, 0,   h, 0,  h,   h, 0, -h,   h, 2*h, -h,   h, 2*h,  h },
                new float[] { -1, 0, 0, -h, 0, -h,  -h, 0,  h,  -h, 2*h,  h,  -h, 2*h, -h },
            };
            float[] cornerU = { 0, 1, 1, 0 }, cornerV = { 1, 1, 0, 0 };
            foreach (float[] f in faces)
            {
                int start = positions.Count / 3;
                for (int c = 0; c < 4; c++)
                {
                    positions.Add(f[3 + c * 3]); positions.Add(f[4 + c * 3]); positions.Add(f[5 + c * 3]);
                    normals.Add(f[0]); normals.Add(f[1]); normals.Add(f[2]);
                    uvs.Add(cornerU[c]); uvs.Add(cornerV[c]);
                }
                indices.AddRange(new[] { (ushort)start, (ushort)(start + 1), (ushort)(start + 2), (ushort)start, (ushort)(start + 2), (ushort)(start + 3) });
            }
            byte[] bin;
            int posOffset, normOffset, uvOffset, indexOffset;
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                posOffset = 0; foreach (float v in positions) { writer.Write(v); }
                normOffset = (int)stream.Position; foreach (float v in normals) { writer.Write(v); }
                uvOffset = (int)stream.Position; foreach (float v in uvs) { writer.Write(v); }
                indexOffset = (int)stream.Position; foreach (ushort v in indices) { writer.Write(v); }
                while (stream.Position % 4 != 0) { writer.Write((byte)0); }
                bin = stream.ToArray();
            }
            File.WriteAllBytes(Path.Combine(directory, name + ".bin"), bin);
            SaveTexture(Path.Combine(directory, name + "_basecolor.png"));

            int vertexCount = positions.Count / 3;
            StringBuilder json = new StringBuilder();
            json.Append("{\n  \"asset\": { \"version\": \"2.0\", \"generator\": \"Liberty Content Compiler sample\" },\n");
            json.Append("  \"scene\": 0,\n  \"scenes\": [ { \"name\": \"Scene\", \"nodes\": [0], \"extras\": { \"liberty_asset\": \"" + name + "\" } } ],\n");
            json.Append("  \"nodes\": [ { \"name\": \"" + name + "_lod0\", \"mesh\": 0 } ],\n");
            json.Append("  \"meshes\": [ { \"name\": \"" + name + "\", \"primitives\": [ { \"attributes\": { \"POSITION\": 0, \"NORMAL\": 1, \"TEXCOORD_0\": 2 }, \"indices\": 3, \"material\": 0 } ] } ],\n");
            json.Append("  \"materials\": [ { \"name\": \"crate_wood\", \"pbrMetallicRoughness\": { \"baseColorTexture\": { \"index\": 0 }, \"metallicFactor\": 0, \"roughnessFactor\": 0.9 }, \"extras\": { \"liberty_shader\": \"gta_default\" } } ],\n");
            json.Append("  \"textures\": [ { \"source\": 0 } ],\n  \"images\": [ { \"uri\": \"" + name + "_basecolor.png\", \"name\": \"" + name + "_basecolor\" } ],\n");
            json.Append("  \"accessors\": [\n");
            json.Append("    { \"bufferView\": 0, \"componentType\": 5126, \"count\": " + vertexCount + ", \"type\": \"VEC3\", \"min\": [" + F(-h) + ", 0, " + F(-h) + "], \"max\": [" + F(h) + ", " + F(2 * h) + ", " + F(h) + "] },\n");
            json.Append("    { \"bufferView\": 1, \"componentType\": 5126, \"count\": " + vertexCount + ", \"type\": \"VEC3\" },\n");
            json.Append("    { \"bufferView\": 2, \"componentType\": 5126, \"count\": " + vertexCount + ", \"type\": \"VEC2\" },\n");
            json.Append("    { \"bufferView\": 3, \"componentType\": 5123, \"count\": " + indices.Count + ", \"type\": \"SCALAR\" }\n  ],\n");
            json.Append("  \"bufferViews\": [\n");
            json.Append("    { \"buffer\": 0, \"byteOffset\": " + posOffset + ", \"byteLength\": " + (normOffset - posOffset) + " },\n");
            json.Append("    { \"buffer\": 0, \"byteOffset\": " + normOffset + ", \"byteLength\": " + (uvOffset - normOffset) + " },\n");
            json.Append("    { \"buffer\": 0, \"byteOffset\": " + uvOffset + ", \"byteLength\": " + (indexOffset - uvOffset) + " },\n");
            json.Append("    { \"buffer\": 0, \"byteOffset\": " + indexOffset + ", \"byteLength\": " + (indices.Count * 2) + " }\n  ],\n");
            json.Append("  \"buffers\": [ { \"uri\": \"" + name + ".bin\", \"byteLength\": " + bin.Length + " } ]\n}\n");
            File.WriteAllText(Path.Combine(directory, name + ".gltf"), json.ToString());
        }

        // Planks with grain, a darker frame and a stencil: procedural, original artwork.
        private static void SaveTexture(string path)
        {
            const int size = 256;
            using (Bitmap bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Random random = new Random(7);
                for (int plank = 0; plank < 5; plank++)
                {
                    int top = plank * size / 5;
                    int shade = 120 + random.Next(-12, 12);
                    using (Brush wood = new SolidBrush(Color.FromArgb(255, shade + 40, shade + 12, shade - 40))) { g.FillRectangle(wood, 0, top, size, size / 5); }
                    using (Pen grain = new Pen(Color.FromArgb(60, 70, 40, 15), 1))
                    {
                        for (int line = 0; line < 9; line++) { int y = top + 3 + random.Next(size / 5 - 6); g.DrawLine(grain, 0, y, size, y + random.Next(-3, 4)); }
                    }
                    using (Pen seam = new Pen(Color.FromArgb(255, 60, 38, 18), 2)) { g.DrawLine(seam, 0, top, size, top); }
                }
                using (Pen frame = new Pen(Color.FromArgb(255, 72, 46, 22), 22)) { g.DrawRectangle(frame, 11, 11, size - 22, size - 22); }
                using (Font font = new Font(FontFamily.GenericSansSerif, 30, FontStyle.Bold, GraphicsUnit.Pixel))
                using (Brush ink = new SolidBrush(Color.FromArgb(200, 30, 30, 32)))
                using (StringFormat centre = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString("LIBERTY", font, ink, new RectangleF(0, size / 2f - 40, size, 40), centre);
                    g.DrawString("SUPPLY", font, ink, new RectangleF(0, size / 2f, size, 40), centre);
                }
                bitmap.Save(path, ImageFormat.Png);
            }
        }

        private static string F(float v) { return v.ToString("0.######", CultureInfo.InvariantCulture); }
    }
}

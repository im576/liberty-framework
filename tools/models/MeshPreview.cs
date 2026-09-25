using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LibertyFramework.Models
{
    // Flat-shaded orthographic previews (side X-Z, top X-Y, front Y-Z) so geometry can be checked without a 3D tool.
    internal static class MeshPreview
    {
        internal static void Save(IList<Mesh> meshes, string path, string title) { Save(meshes, path, title, null); }

        // tints: optional per-mesh colour (null = neutral grey-blue).
        internal static void Save(IList<Mesh> meshes, string path, string title, IList<Color> tints)
        {
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (Mesh mesh in meshes)
            {
                float a, b, c, d, e, f;
                mesh.Bounds(out a, out b, out c, out d, out e, out f);
                minX = Math.Min(minX, a); minY = Math.Min(minY, b); minZ = Math.Min(minZ, c);
                maxX = Math.Max(maxX, d); maxY = Math.Max(maxY, e); maxZ = Math.Max(maxZ, f);
            }
            float span = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
            if (span <= 0) { span = 1; }
            const int Cell = 420;
            using (Bitmap bitmap = new Bitmap(Cell * 3, Cell + 40, PixelFormat.Format24bppRgb))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("Segoe UI", 11f))
            {
                g.Clear(Color.FromArgb(28, 30, 34));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawString(title + string.Format("   size {0:0.000} x {1:0.000} x {2:0.000} m", maxX - minX, maxY - minY, maxZ - minZ), font, Brushes.White, 8, 8);
                string[] names = { "X-Z (seen from -Y)", "X-Y (seen from +Z)", "Y-Z (seen from +X)" };
                for (int view = 0; view < 3; view++)
                {
                    g.DrawString(names[view], font, Brushes.Gray, view * Cell + 8, Cell + 16);
                    List<KeyValuePair<float, PointF[]>> faces = new List<KeyValuePair<float, PointF[]>>();
                    List<float> shades = new List<float>();
                    List<Color> colours = new List<Color>();
                    int meshIndex = -1;
                    foreach (Mesh mesh in meshes)
                    {
                        meshIndex++;
                        Color tint = tints != null && meshIndex < tints.Count ? tints[meshIndex] : Color.FromArgb(200, 200, 215);
                        for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
                        {
                            Mesh.Vertex[] v = { mesh.Vertices[mesh.Indices[i]], mesh.Vertices[mesh.Indices[i + 1]], mesh.Vertices[mesh.Indices[i + 2]] };
                            PointF[] points = new PointF[3];
                            float depth = 0;
                            for (int k = 0; k < 3; k++)
                            {
                                float h, u, d;
                                Project(view, v[k], minX, minY, minZ, out h, out u, out d);
                                points[k] = new PointF(view * Cell + 20 + h / span * (Cell - 40), 30 + (Cell - 40) - u / span * (Cell - 40));
                                depth += d;
                            }
                            faces.Add(new KeyValuePair<float, PointF[]>(depth, points));
                            float nx = (v[0].NX + v[1].NX + v[2].NX) / 3, ny = (v[0].NY + v[1].NY + v[2].NY) / 3, nz = (v[0].NZ + v[1].NZ + v[2].NZ) / 3;
                            shades.Add(Math.Max(0.15f, Math.Abs(view == 0 ? ny : view == 1 ? nz : nx)));
                            colours.Add(tint);
                        }
                    }
                    int[] order = new int[faces.Count];
                    for (int i = 0; i < order.Length; i++) { order[i] = i; }
                    Array.Sort(order, (x, y) => faces[x].Key.CompareTo(faces[y].Key));
                    foreach (int i in order)
                    {
                        float light = 0.3f + 0.7f * shades[i];
                        Color c = colours[i];
                        using (Brush brush = new SolidBrush(Color.FromArgb((int)(c.R * light), (int)(c.G * light), (int)(c.B * light)))) { g.FillPolygon(brush, faces[i].Value); }
                    }
                }
                bitmap.Save(path, ImageFormat.Png);
            }
        }

        // h = horizontal, u = up, d = depth (larger = closer to the viewer, drawn last).
        private static void Project(int view, Mesh.Vertex v, float minX, float minY, float minZ, out float h, out float u, out float d)
        {
            switch (view)
            {
                case 0: h = v.X - minX; u = v.Z - minZ; d = -(v.Y - minY); break;
                case 1: h = v.X - minX; u = v.Y - minY; d = v.Z - minZ; break;
                default: h = v.Y - minY; u = v.Z - minZ; d = v.X - minX; break;
            }
        }
    }
}

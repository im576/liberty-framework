using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LibertyFramework.Models
{
    // Wavefront OBJ bridge so meshes can be viewed and authored in Blender. The game is Z-up, like Blender, so
    // coordinates pass through unchanged. OBJ's V axis points up; Direct3D's points down, so V is flipped both ways.
    internal static class ObjFile
    {
        internal static void Write(string path, IList<KeyValuePair<string, Mesh>> meshes)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("# Liberty Framework model export");
            int baseIndex = 1;
            foreach (KeyValuePair<string, Mesh> pair in meshes)
            {
                text.AppendLine("o " + pair.Key);
                foreach (Mesh.Vertex v in pair.Value.Vertices) { text.AppendLine("v " + F(v.X) + " " + F(v.Y) + " " + F(v.Z)); }
                foreach (Mesh.Vertex v in pair.Value.Vertices) { text.AppendLine("vt " + F(v.U) + " " + F(1 - v.V)); }
                foreach (Mesh.Vertex v in pair.Value.Vertices) { text.AppendLine("vn " + F(v.NX) + " " + F(v.NY) + " " + F(v.NZ)); }
                for (int i = 0; i < pair.Value.Indices.Count; i += 3)
                {
                    int a = pair.Value.Indices[i] + baseIndex, b = pair.Value.Indices[i + 1] + baseIndex, c = pair.Value.Indices[i + 2] + baseIndex;
                    text.AppendLine("f " + a + "/" + a + "/" + a + " " + b + "/" + b + "/" + b + " " + c + "/" + c + "/" + c);
                }
                baseIndex += pair.Value.Vertices.Count;
            }
            File.WriteAllText(path, text.ToString());
        }

        // Reads every object in the file into one mesh. Faces with more than three corners are fanned.
        // Each distinct position/uv/normal corner becomes one vertex.
        internal static Mesh Read(string path)
        {
            List<float[]> positions = new List<float[]>(), uvs = new List<float[]>(), normals = new List<float[]>();
            Dictionary<string, int> corners = new Dictionary<string, int>();
            Mesh mesh = new Mesh();
            foreach (string raw in File.ReadAllLines(path))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') { continue; }
                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case "v": positions.Add(new[] { P(parts[1]), P(parts[2]), P(parts[3]) }); break;
                    case "vt": uvs.Add(new[] { P(parts[1]), parts.Length > 2 ? P(parts[2]) : 0f }); break;
                    case "vn": normals.Add(new[] { P(parts[1]), P(parts[2]), P(parts[3]) }); break;
                    case "f":
                        List<int> face = new List<int>();
                        for (int i = 1; i < parts.Length; i++) { face.Add(Corner(parts[i], positions, uvs, normals, corners, mesh)); }
                        for (int i = 1; i + 1 < face.Count; i++) { mesh.Indices.Add(face[0]); mesh.Indices.Add(face[i]); mesh.Indices.Add(face[i + 1]); }
                        break;
                }
            }
            if (normals.Count == 0) { MeshNormals.Recompute(mesh); }
            mesh.Validate();
            return mesh;
        }

        private static int Corner(string token, List<float[]> positions, List<float[]> uvs, List<float[]> normals, Dictionary<string, int> corners, Mesh mesh)
        {
            int existing;
            if (corners.TryGetValue(token, out existing)) { return existing; }
            string[] ids = token.Split('/');
            Mesh.Vertex vertex = new Mesh.Vertex();
            float[] p = positions[Resolve(ids[0], positions.Count)];
            vertex.X = p[0]; vertex.Y = p[1]; vertex.Z = p[2];
            if (ids.Length > 1 && ids[1].Length > 0) { float[] t = uvs[Resolve(ids[1], uvs.Count)]; vertex.U = t[0]; vertex.V = 1 - t[1]; }
            if (ids.Length > 2 && ids[2].Length > 0) { float[] n = normals[Resolve(ids[2], normals.Count)]; vertex.NX = n[0]; vertex.NY = n[1]; vertex.NZ = n[2]; }
            vertex.Colour = 0xFFFFFFFF;
            mesh.Vertices.Add(vertex);
            corners[token] = mesh.Vertices.Count - 1;
            return mesh.Vertices.Count - 1;
        }

        private static int Resolve(string id, int count)
        {
            int value = int.Parse(id, CultureInfo.InvariantCulture);
            return value < 0 ? count + value : value - 1;
        }

        private static float P(string text) { return float.Parse(text, CultureInfo.InvariantCulture); }
        private static string F(float value) { return value.ToString("0.######", CultureInfo.InvariantCulture); }
    }
}

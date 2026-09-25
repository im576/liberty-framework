using System;

namespace LibertyFramework.Models
{
    // Smooth per-vertex normals from area-weighted face normals.
    internal static class MeshNormals
    {
        internal static void Recompute(Mesh mesh)
        {
            double[] sums = new double[mesh.Vertices.Count * 3];
            for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
            {
                int a = mesh.Indices[i], b = mesh.Indices[i + 1], c = mesh.Indices[i + 2];
                Mesh.Vertex va = mesh.Vertices[a], vb = mesh.Vertices[b], vc = mesh.Vertices[c];
                double ux = vb.X - va.X, uy = vb.Y - va.Y, uz = vb.Z - va.Z;
                double wx = vc.X - va.X, wy = vc.Y - va.Y, wz = vc.Z - va.Z;
                double nx = uy * wz - uz * wy, ny = uz * wx - ux * wz, nz = ux * wy - uy * wx;
                foreach (int index in new[] { a, b, c }) { sums[index * 3] += nx; sums[index * 3 + 1] += ny; sums[index * 3 + 2] += nz; }
            }
            for (int v = 0; v < mesh.Vertices.Count; v++)
            {
                double x = sums[v * 3], y = sums[v * 3 + 1], z = sums[v * 3 + 2];
                double length = Math.Sqrt(x * x + y * y + z * z);
                Mesh.Vertex vertex = mesh.Vertices[v];
                if (length > 1e-12) { vertex.NX = (float)(x / length); vertex.NY = (float)(y / length); vertex.NZ = (float)(z / length); }
                else { vertex.NX = 0; vertex.NY = 0; vertex.NZ = 1; }
                mesh.Vertices[v] = vertex;
            }
        }
    }
}

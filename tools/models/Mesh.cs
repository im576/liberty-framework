using System;
using System.Collections.Generic;

namespace LibertyFramework.Models
{
    // Triangle-list mesh in game units (metres, Z up). Colour is D3DCOLOR (0xAARRGGBB).
    internal sealed class Mesh
    {
        internal struct Vertex
        {
            internal float X, Y, Z, NX, NY, NZ, U, V;
            internal uint Colour;
        }

        internal readonly List<Vertex> Vertices = new List<Vertex>();
        internal readonly List<int> Indices = new List<int>();

        internal void Bounds(out float minX, out float minY, out float minZ, out float maxX, out float maxY, out float maxZ)
        {
            if (Vertices.Count == 0) { throw new InvalidOperationException("empty mesh"); }
            minX = minY = minZ = float.MaxValue; maxX = maxY = maxZ = float.MinValue;
            foreach (Vertex v in Vertices)
            {
                minX = Math.Min(minX, v.X); minY = Math.Min(minY, v.Y); minZ = Math.Min(minZ, v.Z);
                maxX = Math.Max(maxX, v.X); maxY = Math.Max(maxY, v.Y); maxZ = Math.Max(maxZ, v.Z);
            }
        }

        // Largest distance from the given point to any vertex.
        internal float RadiusAround(float cx, float cy, float cz)
        {
            double best = 0;
            foreach (Vertex v in Vertices)
            {
                double dx = v.X - cx, dy = v.Y - cy, dz = v.Z - cz;
                best = Math.Max(best, dx * dx + dy * dy + dz * dz);
            }
            return (float)Math.Sqrt(best);
        }

        internal void Validate()
        {
            if (Vertices.Count == 0 || Indices.Count == 0 || Indices.Count % 3 != 0) { throw new InvalidOperationException("mesh must be a non-empty triangle list"); }
            if (Vertices.Count > 65535) { throw new InvalidOperationException("mesh has " + Vertices.Count + " vertices; 16-bit indices allow 65535"); }
            foreach (int index in Indices) { if (index < 0 || index >= Vertices.Count) { throw new InvalidOperationException("index out of range: " + index); } }
        }
    }
}

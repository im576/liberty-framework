using System;
using System.Collections.Generic;
using System.Linq;

namespace LibertyFramework.Models
{
    // W-5 sling strap fitted to the player's body. The strap runs in the plane that contains the line from a point
    // over one shoulder to a point on the opposite hip, and the body's front-back axis. The designer slices every
    // supplied body mesh (all of Niko's upper-body outfits) with that plane, takes the convex hull of the cut (a taut
    // strap bridges hollows), pushes it out by a clearance, smooths it, and sweeps a flat band along it. The result
    // is expressed in the attach bone's bind-pose space, so the game attaches it with zero offset and zero rotation.
    internal static class StrapDesigner
    {
        internal sealed class Spec
        {
            internal Vec3 ShoulderPoint;   // model space
            internal Vec3 HipPoint;        // model space
            internal double ClearanceMeters;
            internal double HipMarginMeters;
            internal double WidthMeters;
            internal double ThicknessMeters;
            internal double TextureRepeatMeters;
            internal int Segments;
        }

        internal static Mesh Design(IList<Mesh> body, Skeleton skeleton, int attachBone, Spec spec, out List<Vec3> modelPath)
        {
            Vec3 forward = new Vec3(0, 1, 0);
            Vec3 along = (spec.HipPoint - spec.ShoulderPoint);
            along = (along - forward * along.Dot(forward)).Normalized();
            Vec3 normal = along.Cross(forward).Normalized();
            Vec3 origin = (spec.ShoulderPoint + spec.HipPoint) * 0.5;

            // Past the hip anchor the plane runs down into the leg; the strap turns there, so the cut stops at the hip.
            double hipLimit = (spec.HipPoint - origin).Dot(along) + spec.HipMarginMeters;
            List<double[]> cut = new List<double[]>();
            foreach (Mesh mesh in body)
            {
                for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
                {
                    Vec3[] p = new Vec3[3];
                    double[] d = new double[3];
                    for (int k = 0; k < 3; k++)
                    {
                        Mesh.Vertex v = mesh.Vertices[mesh.Indices[i + k]];
                        p[k] = new Vec3(v.X, v.Y, v.Z);
                        d[k] = (p[k] - origin).Dot(normal);
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        int n = (k + 1) % 3;
                        if ((d[k] < 0) == (d[n] < 0)) { continue; }
                        double t = d[k] / (d[k] - d[n]);
                        Vec3 hit = p[k] + (p[n] - p[k]) * t - origin;
                        if (hit.Dot(along) > hipLimit) { continue; }
                        cut.Add(new[] { hit.Dot(along), hit.Dot(forward) });
                    }
                }
            }
            if (cut.Count < 16) { throw new InvalidOperationException("strap plane misses the body (" + cut.Count + " points)"); }

            List<double[]> loop = Smooth(Offset(Hull(cut), spec.ClearanceMeters), 3);
            loop = Resample(loop, spec.Segments);
            modelPath = loop.Select(q => origin + along * q[0] + forward * q[1]).ToList();

            Skeleton.Bone bone = skeleton.Bones[attachBone];
            Quat toBone = bone.Rotation.Conjugate();
            Func<Vec3, Vec3> point = v => toBone.Rotate(v - bone.Position);
            Func<Vec3, Vec3> direction = v => toBone.Rotate(v);
            return Sweep(modelPath, normal, spec, point, direction);
        }

        // Closed flat band: outer face, inner face and two edges; flat-shaded (separate vertices per face).
        private static Mesh Sweep(List<Vec3> path, Vec3 widthAxis, Spec spec, Func<Vec3, Vec3> point, Func<Vec3, Vec3> direction)
        {
            Mesh mesh = new Mesh();
            int count = path.Count;
            Vec3 centre = Vec3.Zero;
            foreach (Vec3 p in path) { centre = centre + p; }
            centre = centre * (1.0 / count);
            double[] v = new double[count + 1];
            for (int i = 1; i <= count; i++) { v[i] = v[i - 1] + (path[i % count] - path[i - 1]).Length / spec.TextureRepeatMeters; }
            Vec3 half = widthAxis * (spec.WidthMeters / 2);
            // face: 0 outer, 1 inner, 2 edge (+width), 3 edge (-width)
            for (int face = 0; face < 4; face++)
            {
                int start = mesh.Vertices.Count;
                for (int i = 0; i <= count; i++)
                {
                    Vec3 p = path[i % count];
                    Vec3 tangent = (path[(i + 1) % count] - path[(i + count - 1) % count]).Normalized();
                    Vec3 outward = widthAxis.Cross(tangent).Normalized();
                    if (outward.Dot(p - centre) < 0) { outward = -outward; }
                    Vec3 inner = p - outward * spec.ThicknessMeters;
                    Vec3 a, b, n;
                    switch (face)
                    {
                        case 0: a = p - half; b = p + half; n = outward; break;
                        case 1: a = inner + half; b = inner - half; n = -outward; break;
                        case 2: a = p + half; b = inner + half; n = widthAxis; break;
                        default: a = inner - half; b = p - half; n = -widthAxis; break;
                    }
                    bool edge = face >= 2;
                    mesh.Vertices.Add(Vertex(point(a), direction(n), edge ? 0.02 : 0.0, v[i]));
                    mesh.Vertices.Add(Vertex(point(b), direction(n), edge ? 0.06 : 1.0, v[i]));
                }
                for (int i = 0; i < count; i++)
                {
                    int a0 = start + i * 2, b0 = a0 + 1, a1 = a0 + 2, b1 = a0 + 3;
                    mesh.Indices.AddRange(new[] { a0, a1, b0, b0, a1, b1 });
                }
            }
            FixWinding(mesh);
            return mesh;
        }

        // Makes each triangle's winding agree with its vertex normals (front faces counter-clockwise).
        private static void FixWinding(Mesh mesh)
        {
            for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
            {
                Mesh.Vertex a = mesh.Vertices[mesh.Indices[i]], b = mesh.Vertices[mesh.Indices[i + 1]], c = mesh.Vertices[mesh.Indices[i + 2]];
                Vec3 faceNormal = new Vec3(b.X - a.X, b.Y - a.Y, b.Z - a.Z).Cross(new Vec3(c.X - a.X, c.Y - a.Y, c.Z - a.Z));
                if (faceNormal.Dot(new Vec3(a.NX + b.NX + c.NX, a.NY + b.NY + c.NY, a.NZ + b.NZ + c.NZ)) < 0)
                {
                    int swap = mesh.Indices[i + 1]; mesh.Indices[i + 1] = mesh.Indices[i + 2]; mesh.Indices[i + 2] = swap;
                }
            }
        }

        private static Mesh.Vertex Vertex(Vec3 p, Vec3 n, double u, double v)
        {
            Mesh.Vertex vertex = new Mesh.Vertex();
            vertex.X = (float)p.X; vertex.Y = (float)p.Y; vertex.Z = (float)p.Z;
            Vec3 unit = n.Normalized();
            vertex.NX = (float)unit.X; vertex.NY = (float)unit.Y; vertex.NZ = (float)unit.Z;
            vertex.U = (float)u; vertex.V = (float)v;
            vertex.Colour = 0xFFFFFFFF;
            return vertex;
        }

        // Andrew's monotone chain; counter-clockwise.
        internal static List<double[]> Hull(List<double[]> points)
        {
            List<double[]> sorted = points.OrderBy(p => p[0]).ThenBy(p => p[1]).ToList();
            List<double[]> hull = new List<double[]>();
            for (int pass = 0; pass < 2; pass++)
            {
                int start = hull.Count;
                foreach (double[] p in sorted)
                {
                    while (hull.Count >= start + 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0) { hull.RemoveAt(hull.Count - 1); }
                    hull.Add(p);
                }
                hull.RemoveAt(hull.Count - 1);
                sorted.Reverse();
            }
            return hull;
        }

        private static double Cross(double[] o, double[] a, double[] b) { return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0]); }

        // Pushes each vertex of a counter-clockwise convex polygon outward along its averaged edge normals.
        private static List<double[]> Offset(List<double[]> polygon, double distance)
        {
            List<double[]> result = new List<double[]>();
            int n = polygon.Count;
            for (int i = 0; i < n; i++)
            {
                double[] prev = polygon[(i + n - 1) % n], p = polygon[i], next = polygon[(i + 1) % n];
                double[] n1 = EdgeNormal(prev, p), n2 = EdgeNormal(p, next);
                double nx = n1[0] + n2[0], ny = n1[1] + n2[1], l = Math.Sqrt(nx * nx + ny * ny);
                if (l < 1e-9) { nx = n1[0]; ny = n1[1]; l = 1; }
                result.Add(new[] { p[0] + nx / l * distance, p[1] + ny / l * distance });
            }
            return result;
        }

        private static double[] EdgeNormal(double[] a, double[] b)
        {
            double dx = b[0] - a[0], dy = b[1] - a[1], l = Math.Sqrt(dx * dx + dy * dy);
            return l < 1e-12 ? new double[] { 0, 0 } : new[] { dy / l, -dx / l };
        }

        // Chaikin corner cutting on a closed polygon.
        private static List<double[]> Smooth(List<double[]> polygon, int iterations)
        {
            for (int k = 0; k < iterations; k++)
            {
                List<double[]> next = new List<double[]>();
                for (int i = 0; i < polygon.Count; i++)
                {
                    double[] a = polygon[i], b = polygon[(i + 1) % polygon.Count];
                    next.Add(new[] { 0.75 * a[0] + 0.25 * b[0], 0.75 * a[1] + 0.25 * b[1] });
                    next.Add(new[] { 0.25 * a[0] + 0.75 * b[0], 0.25 * a[1] + 0.75 * b[1] });
                }
                polygon = next;
            }
            return polygon;
        }

        private static List<double[]> Resample(List<double[]> polygon, int segments)
        {
            int n = polygon.Count;
            double[] cumulative = new double[n + 1];
            for (int i = 1; i <= n; i++)
            {
                double[] a = polygon[i - 1], b = polygon[i % n];
                cumulative[i] = cumulative[i - 1] + Math.Sqrt((b[0] - a[0]) * (b[0] - a[0]) + (b[1] - a[1]) * (b[1] - a[1]));
            }
            List<double[]> result = new List<double[]>();
            int edge = 0;
            for (int s = 0; s < segments; s++)
            {
                double target = cumulative[n] * s / segments;
                while (cumulative[edge + 1] < target) { edge++; }
                double[] a = polygon[edge], b = polygon[(edge + 1) % n];
                double span = cumulative[edge + 1] - cumulative[edge];
                double t = span < 1e-12 ? 0 : (target - cumulative[edge]) / span;
                result.Add(new[] { a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t });
            }
            return result;
        }
    }
}

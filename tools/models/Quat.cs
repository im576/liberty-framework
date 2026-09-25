using System;

namespace LibertyFramework.Models
{
    // Rotation quaternion, components stored x, y, z, w as in RAGE's bone records.
    internal struct Quat
    {
        internal readonly double X, Y, Z, W;
        internal Quat(double x, double y, double z, double w) { X = x; Y = y; Z = z; W = w; }
        internal static readonly Quat Identity = new Quat(0, 0, 0, 1);

        public static Quat operator *(Quat a, Quat b)
        {
            return new Quat(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        internal Quat Conjugate() { return new Quat(-X, -Y, -Z, W); }

        internal Quat Normalized()
        {
            double l = Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            return l < 1e-12 ? Identity : new Quat(X / l, Y / l, Z / l, W / l);
        }

        internal Vec3 Rotate(Vec3 v)
        {
            Quat r = this * new Quat(v.X, v.Y, v.Z, 0) * Conjugate();
            return new Vec3(r.X, r.Y, r.Z);
        }
    }
}

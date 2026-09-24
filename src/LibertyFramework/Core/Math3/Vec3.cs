using System;

namespace LibertyFramework.Core.Math3
{
    // Minimal double-precision vector so gunplay math has no ScriptHookDotNet dependency.
    internal struct Vec3
    {
        internal readonly double X;
        internal readonly double Y;
        internal readonly double Z;

        internal Vec3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }

        internal Vec3 Normalized()
        {
            double length = Length;
            return length > 1e-9 ? new Vec3(X / length, Y / length, Z / length) : new Vec3(0, 0, 0);
        }

        public static Vec3 operator +(Vec3 a, Vec3 b) { return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Vec3 operator -(Vec3 a, Vec3 b) { return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static Vec3 operator *(Vec3 a, double s) { return new Vec3(a.X * s, a.Y * s, a.Z * s); }

        internal static double Dot(Vec3 a, Vec3 b) { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }

        // GTA IV camera rotation (degrees): X = pitch (up positive), Z = heading (0 = north/+Y, counter-clockwise).
        internal static Vec3 FromPitchHeadingDegrees(double pitchDegrees, double headingDegrees)
        {
            double pitch = pitchDegrees * Math.PI / 180.0;
            double heading = headingDegrees * Math.PI / 180.0;
            return new Vec3(-Math.Sin(heading) * Math.Cos(pitch), Math.Cos(heading) * Math.Cos(pitch), Math.Sin(pitch));
        }

        public override string ToString()
        {
            return X.ToString("0.00") + "," + Y.ToString("0.00") + "," + Z.ToString("0.00");
        }
    }
}

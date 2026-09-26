using System;
using System.Globalization;

namespace Liberty.Sdk
{
    // World-space vector in metres (GTA IV: X east, Y north, Z up).
    public struct Vec3 : IEquatable<Vec3>
    {
        public float X, Y, Z;

        public Vec3(float x, float y, float z) { X = x; Y = y; Z = z; }

        public static readonly Vec3 Zero = new Vec3(0, 0, 0);
        public static readonly Vec3 Up = new Vec3(0, 0, 1);

        public static Vec3 operator +(Vec3 a, Vec3 b) { return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Vec3 operator -(Vec3 a, Vec3 b) { return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static Vec3 operator -(Vec3 a) { return new Vec3(-a.X, -a.Y, -a.Z); }
        public static Vec3 operator *(Vec3 a, float s) { return new Vec3(a.X * s, a.Y * s, a.Z * s); }
        public static Vec3 operator *(float s, Vec3 a) { return new Vec3(a.X * s, a.Y * s, a.Z * s); }
        public static Vec3 operator /(Vec3 a, float s) { return new Vec3(a.X / s, a.Y / s, a.Z / s); }
        public static bool operator ==(Vec3 a, Vec3 b) { return a.Equals(b); }
        public static bool operator !=(Vec3 a, Vec3 b) { return !a.Equals(b); }

        public float Length { get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); } }
        public float LengthSquared { get { return X * X + Y * Y + Z * Z; } }
        public Vec3 Normalized { get { float l = Length; return l > 1e-6f ? this / l : Zero; } }
        public float Dot(Vec3 b) { return X * b.X + Y * b.Y + Z * b.Z; }
        public Vec3 Cross(Vec3 b) { return new Vec3(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X); }
        public float DistanceTo(Vec3 b) { return (this - b).Length; }
        public float DistanceTo2D(Vec3 b) { float dx = X - b.X, dy = Y - b.Y; return (float)Math.Sqrt(dx * dx + dy * dy); }

        // Unit direction for a GTA heading in degrees (0 = north/+Y, counter-clockwise).
        public static Vec3 FromHeading(float degrees)
        {
            double r = degrees * Math.PI / 180.0;
            return new Vec3((float)-Math.Sin(r), (float)Math.Cos(r), 0);
        }

        // GTA heading in degrees (0-360) of this direction on the ground plane.
        public float ToHeading()
        {
            double h = Math.Atan2(-X, Y) * 180.0 / Math.PI;
            return (float)(h < 0 ? h + 360 : h);
        }

        public bool Equals(Vec3 other) { return X == other.X && Y == other.Y && Z == other.Z; }
        public override bool Equals(object obj) { return obj is Vec3 && Equals((Vec3)obj); }
        public override int GetHashCode() { return X.GetHashCode() ^ (Y.GetHashCode() * 397) ^ (Z.GetHashCode() * 17); }
        public override string ToString() { return string.Format(CultureInfo.InvariantCulture, "({0:0.###}, {1:0.###}, {2:0.###})", X, Y, Z); }
    }
}
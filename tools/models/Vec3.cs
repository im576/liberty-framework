using System;

namespace LibertyFramework.Models
{
    internal struct Vec3
    {
        internal readonly double X, Y, Z;
        internal Vec3(double x, double y, double z) { X = x; Y = y; Z = z; }
        internal static readonly Vec3 Zero = new Vec3(0, 0, 0);
        public static Vec3 operator +(Vec3 a, Vec3 b) { return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Vec3 operator -(Vec3 a, Vec3 b) { return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static Vec3 operator -(Vec3 a) { return new Vec3(-a.X, -a.Y, -a.Z); }
        public static Vec3 operator *(Vec3 a, double s) { return new Vec3(a.X * s, a.Y * s, a.Z * s); }
        internal double Dot(Vec3 b) { return X * b.X + Y * b.Y + Z * b.Z; }
        internal Vec3 Cross(Vec3 b) { return new Vec3(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X); }
        internal double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }
        internal Vec3 Normalized() { double l = Length; return l < 1e-12 ? new Vec3(0, 0, 1) : this * (1 / l); }
        public override string ToString() { return string.Format("({0:0.000},{1:0.000},{2:0.000})", X, Y, Z); }
    }
}

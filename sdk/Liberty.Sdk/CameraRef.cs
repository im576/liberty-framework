using System;

namespace Liberty.Sdk
{
    // A scripted camera owned by a module. Value type; compare with == or IsNone.
    public struct CameraRef : IEquatable<CameraRef>
    {
        public readonly int Handle;
        public CameraRef(int handle) { Handle = handle; }
        public static readonly CameraRef None = new CameraRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(CameraRef a, CameraRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(CameraRef a, CameraRef b) { return a.Handle != b.Handle; }
        public bool Equals(CameraRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is CameraRef && Equals((CameraRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "CameraRef(" + Handle + ")"; }
    }
}
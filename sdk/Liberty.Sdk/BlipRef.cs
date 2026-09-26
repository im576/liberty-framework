using System;

namespace Liberty.Sdk
{
    // A radar blip owned by a module. Value type; compare with == or IsNone.
    public struct BlipRef : IEquatable<BlipRef>
    {
        public readonly int Handle;
        public BlipRef(int handle) { Handle = handle; }
        public static readonly BlipRef None = new BlipRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(BlipRef a, BlipRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(BlipRef a, BlipRef b) { return a.Handle != b.Handle; }
        public bool Equals(BlipRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is BlipRef && Equals((BlipRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "BlipRef(" + Handle + ")"; }
    }
}
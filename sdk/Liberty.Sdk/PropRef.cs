using System;

namespace Liberty.Sdk
{
    // An object/prop in the world. None when absent. Value type; compare with == or IsNone.
    public struct PropRef : IEquatable<PropRef>
    {
        public readonly int Handle;
        public PropRef(int handle) { Handle = handle; }
        public static readonly PropRef None = new PropRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(PropRef a, PropRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(PropRef a, PropRef b) { return a.Handle != b.Handle; }
        public bool Equals(PropRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is PropRef && Equals((PropRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "PropRef(" + Handle + ")"; }
    }
}
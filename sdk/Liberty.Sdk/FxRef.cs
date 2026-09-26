using System;

namespace Liberty.Sdk
{
    // A running particle effect owned by a module. Value type; compare with == or IsNone.
    public struct FxRef : IEquatable<FxRef>
    {
        public readonly int Handle;
        public FxRef(int handle) { Handle = handle; }
        public static readonly FxRef None = new FxRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(FxRef a, FxRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(FxRef a, FxRef b) { return a.Handle != b.Handle; }
        public bool Equals(FxRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is FxRef && Equals((FxRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "FxRef(" + Handle + ")"; }
    }
}
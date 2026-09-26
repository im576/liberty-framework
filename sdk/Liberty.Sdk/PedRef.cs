using System;

namespace Liberty.Sdk
{
    // A ped (character) in the world. None when absent. Value type; compare with == or IsNone.
    public struct PedRef : IEquatable<PedRef>
    {
        public readonly int Handle;
        public PedRef(int handle) { Handle = handle; }
        public static readonly PedRef None = new PedRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(PedRef a, PedRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(PedRef a, PedRef b) { return a.Handle != b.Handle; }
        public bool Equals(PedRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is PedRef && Equals((PedRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "PedRef(" + Handle + ")"; }
    }
}
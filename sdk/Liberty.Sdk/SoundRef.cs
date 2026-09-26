using System;

namespace Liberty.Sdk
{
    // A playing sound owned by a module. Value type; compare with == or IsNone.
    public struct SoundRef : IEquatable<SoundRef>
    {
        public readonly int Handle;
        public SoundRef(int handle) { Handle = handle; }
        public static readonly SoundRef None = new SoundRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(SoundRef a, SoundRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(SoundRef a, SoundRef b) { return a.Handle != b.Handle; }
        public bool Equals(SoundRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is SoundRef && Equals((SoundRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "SoundRef(" + Handle + ")"; }
    }
}
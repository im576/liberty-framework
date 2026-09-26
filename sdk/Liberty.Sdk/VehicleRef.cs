using System;

namespace Liberty.Sdk
{
    // A vehicle in the world. None when absent. Value type; compare with == or IsNone.
    public struct VehicleRef : IEquatable<VehicleRef>
    {
        public readonly int Handle;
        public VehicleRef(int handle) { Handle = handle; }
        public static readonly VehicleRef None = new VehicleRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(VehicleRef a, VehicleRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(VehicleRef a, VehicleRef b) { return a.Handle != b.Handle; }
        public bool Equals(VehicleRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is VehicleRef && Equals((VehicleRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "VehicleRef(" + Handle + ")"; }
    }
}
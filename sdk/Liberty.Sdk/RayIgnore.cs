using System;

namespace Liberty.Sdk
{
    // Up to four entities a ray never stops at (itself, its vehicle, ...). Value type: no allocation.
    //   RayIgnore.Of(ped).And(vehicle)
    public struct RayIgnore
    {
        public const int Capacity = 4;
        public static readonly RayIgnore Nothing = new RayIgnore();

        private int count;
        private RayEntityKind kind0, kind1, kind2, kind3;
        private int handle0, handle1, handle2, handle3;

        public int Count { get { return count; } }

        public static RayIgnore Of(PedRef ped) { return Nothing.And(ped); }
        public static RayIgnore Of(VehicleRef vehicle) { return Nothing.And(vehicle); }
        public static RayIgnore Of(PropRef prop) { return Nothing.And(prop); }

        // None is skipped, so And(Peds.GetVehicle(ped)) is safe on foot. More than Capacity entities throws.
        public RayIgnore And(PedRef ped) { return ped.IsNone ? this : With(RayEntityKind.Ped, ped.Handle); }
        public RayIgnore And(VehicleRef vehicle) { return vehicle.IsNone ? this : With(RayEntityKind.Vehicle, vehicle.Handle); }
        public RayIgnore And(PropRef prop) { return prop.IsNone ? this : With(RayEntityKind.Object, prop.Handle); }

        public RayEntityKind KindAt(int index) { Check(index); return index == 0 ? kind0 : index == 1 ? kind1 : index == 2 ? kind2 : kind3; }
        public int HandleAt(int index) { Check(index); return index == 0 ? handle0 : index == 1 ? handle1 : index == 2 ? handle2 : handle3; }

        public bool Contains(RayEntityKind kind, int handle)
        {
            for (int i = 0; i < count; i++) { if (KindAt(i) == kind && HandleAt(i) == handle) { return true; } }
            return false;
        }

        private RayIgnore With(RayEntityKind kind, int handle)
        {
            if (Contains(kind, handle)) { return this; }
            if (count >= Capacity) { throw new ArgumentException("a ray ignores at most " + Capacity + " entities"); }
            RayIgnore next = this;
            switch (count)
            {
                case 0: next.kind0 = kind; next.handle0 = handle; break;
                case 1: next.kind1 = kind; next.handle1 = handle; break;
                case 2: next.kind2 = kind; next.handle2 = handle; break;
                default: next.kind3 = kind; next.handle3 = handle; break;
            }
            next.count = count + 1;
            return next;
        }

        private void Check(int index)
        {
            if (index < 0 || index >= count) { throw new ArgumentOutOfRangeException("index"); }
        }
    }
}

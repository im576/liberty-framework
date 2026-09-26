using System;

namespace Liberty.Sdk
{
    // Objects/props. Created props belong to the creating module and are deleted when it stops.
    public interface IProps
    {
        void Spawn(LibertyModule owner, ModelRef model, Vec3 position, Action<PropRef> onReady);
        PropRef TryCreate(LibertyModule owner, ModelRef model, Vec3 position);
        void Delete(PropRef prop);
        bool Exists(PropRef prop);
        Vec3 GetPosition(PropRef prop);
        void SetPosition(PropRef prop, Vec3 position);
        // Heading in degrees (0 = north, counter-clockwise).
        float GetHeading(PropRef prop);
        // Rotation in degrees (pitch, roll, yaw).
        void SetRotation(PropRef prop, Vec3 degrees);
        void SetCollision(PropRef prop, bool on);
        void SetFrozen(PropRef prop, bool frozen);
        // Offset in the bone's space (metres), rotation in degrees.
        void AttachToPed(PropRef prop, PedRef ped, Bone bone, Vec3 offset, Vec3 rotation);
        void AttachToVehicle(PropRef prop, VehicleRef vehicle, Vec3 offset, Vec3 rotation);
        void Detach(PropRef prop);
        // World position of a point in the prop's local space.
        Vec3 GetOffsetPosition(PropRef prop, Vec3 local);
    }
}
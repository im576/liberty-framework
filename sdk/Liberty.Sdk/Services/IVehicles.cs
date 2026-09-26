using System;

namespace Liberty.Sdk
{
    // Vehicles. Created vehicles belong to the creating module and are deleted when it stops unless Release'd.
    public interface IVehicles
    {
        void Spawn(LibertyModule owner, ModelRef model, Vec3 position, float heading, Action<VehicleRef> onReady);
        void Delete(VehicleRef vehicle);
        void Release(LibertyModule owner, VehicleRef vehicle);

        bool Exists(VehicleRef vehicle);
        Vec3 GetPosition(VehicleRef vehicle);
        void SetPosition(VehicleRef vehicle, Vec3 position);
        float GetHeading(VehicleRef vehicle);
        void SetHeading(VehicleRef vehicle, float heading);
        float GetSpeed(VehicleRef vehicle);
        int GetHealth(VehicleRef vehicle);
        void SetHealth(VehicleRef vehicle, int health);
        float GetEngineHealth(VehicleRef vehicle);
        void SetEngineHealth(VehicleRef vehicle, float health);
        ModelRef GetModel(VehicleRef vehicle);
        PedRef GetDriver(VehicleRef vehicle);
        // World position of a point given in the vehicle's local space (X right, Y forward, Z up).
        Vec3 GetOffsetPosition(VehicleRef vehicle, Vec3 local);
        void OpenDoor(VehicleRef vehicle, VehicleDoor door);
        void CloseDoor(VehicleRef vehicle, VehicleDoor door);
        bool IsExtraOn(VehicleRef vehicle, int extra);
        void SetExtra(VehicleRef vehicle, int extra, bool on);
        void SetColours(VehicleRef vehicle, int primary, int secondary);
        void Repair(VehicleRef vehicle);
        VehicleRef GetClosest(Vec3 position, float radius);
    }
}
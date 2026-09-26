namespace Liberty.Sdk
{
    // AI tasks. Each call replaces the ped's current task unless noted.
    public interface ITasks
    {
        void Clear(PedRef ped);
        void ClearImmediately(PedRef ped);
        void StandStill(PedRef ped, int milliseconds);
        void GoTo(PedRef ped, Vec3 target, bool run);
        void Wander(PedRef ped);
        void TurnTo(PedRef ped, Vec3 target);
        void LookAt(PedRef ped, Vec3 target, int milliseconds);
        void Flee(PedRef ped, PedRef from, float distance);
        void Attack(PedRef ped, PedRef target);
        void AimAt(PedRef ped, Vec3 target, int milliseconds);
        void ShootAt(PedRef ped, Vec3 target, int milliseconds);
        void HandsUp(PedRef ped, int milliseconds);
        void Cower(PedRef ped);
        void EnterVehicle(PedRef ped, VehicleRef vehicle, int seat);
        void LeaveVehicle(PedRef ped);
        void DriveTo(PedRef driver, VehicleRef vehicle, Vec3 target, float speed);
    }
}
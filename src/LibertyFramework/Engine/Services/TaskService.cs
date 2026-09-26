using GTA;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK ITasks. Natives whose argument meaning is documented (Sanny Builder's GTA IV library, cited in NATIVES.md) are
    // called directly; tasks whose extra arguments are unknown go through ScriptHookDotNet's task wrappers.
    public sealed class TaskService : ITasks
    {
        private const int MoveWalk = 2, MoveRun = 3;

        public void Clear(PedRef ped) { Function.Call("CLEAR_CHAR_TASKS", ped.Handle); }

        public void ClearImmediately(PedRef ped) { Function.Call("CLEAR_CHAR_TASKS_IMMEDIATELY", ped.Handle); }

        public void StandStill(PedRef ped, int milliseconds) { Function.Call("TASK_STAND_STILL", ped.Handle, milliseconds); }

        // TASK_GO_STRAIGHT_TO_COORD(ped, x, y, z, moveState 2 walk / 3 run, time ms).
        public void GoTo(PedRef ped, Vec3 target, bool run)
        {
            Function.Call("TASK_GO_STRAIGHT_TO_COORD", ped.Handle, target.X, target.Y, target.Z, run ? MoveRun : MoveWalk, 45000);
        }

        public void Wander(PedRef ped) { Function.Call("TASK_WANDER_STANDARD", ped.Handle); }

        public void TurnTo(PedRef ped, Vec3 target) { Function.Call("TASK_TURN_CHAR_TO_FACE_COORD", ped.Handle, target.X, target.Y, target.Z); }

        public void LookAt(PedRef ped, Vec3 target, int milliseconds) { Function.Call("TASK_LOOK_AT_COORD", ped.Handle, target.X, target.Y, target.Z, milliseconds, 0); }

        // TASK_SMART_FLEE_CHAR(ped, from, radius, time); -1 = until safe.
        public void Flee(PedRef ped, PedRef from, float distance) { Function.Call("TASK_SMART_FLEE_CHAR", ped.Handle, from.Handle, distance, -1); }

        public void Attack(PedRef ped, PedRef target) { Function.Call("TASK_COMBAT", ped.Handle, target.Handle); }

        public void AimAt(PedRef ped, Vec3 target, int milliseconds) { Function.Call("TASK_AIM_GUN_AT_COORD", ped.Handle, target.X, target.Y, target.Z, milliseconds); }

        // TASK_SHOOT_AT_COORD's last two arguments are undocumented; SHDN's Ped.ShootAt makes the call.
        public void ShootAt(PedRef ped, Vec3 target, int milliseconds)
        {
            Ped wrapper = Handles.Ped(ped);
            if (wrapper != null) { wrapper.ShootAt(Handles.V(target)); }
        }

        public void HandsUp(PedRef ped, int milliseconds) { Function.Call("TASK_HANDS_UP", ped.Handle, milliseconds); }

        public void Cower(PedRef ped) { Function.Call("TASK_COWER", ped.Handle); }

        // seat -1 = driver, 0.. = passenger seats. 10000 ms to reach the door.
        public void EnterVehicle(PedRef ped, VehicleRef vehicle, int seat)
        {
            if (seat < 0) { Function.Call("TASK_ENTER_CAR_AS_DRIVER", ped.Handle, vehicle.Handle, 10000); }
            else { Function.Call("TASK_ENTER_CAR_AS_PASSENGER", ped.Handle, vehicle.Handle, 10000, seat); }
        }

        public void LeaveVehicle(PedRef ped) { Function.Call("TASK_LEAVE_ANY_CAR", ped.Handle); }

        // TASK_CAR_DRIVE_TO_COORD's model and style arguments are wrapped by SHDN's DriveTo.
        public void DriveTo(PedRef driver, VehicleRef vehicle, Vec3 target, float speed)
        {
            Ped wrapper = Handles.Ped(driver);
            Vehicle car = Handles.Vehicle(vehicle);
            if (wrapper != null && car != null) { wrapper.Task.DriveTo(car, Handles.V(target), speed, false); }
        }
    }
}

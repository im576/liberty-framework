// docs/sdk/README.md "Raycast and line of sight". Compiled against the current SDK by tools/verify.ps1.
using Liberty.Sdk;

[Module("example-raycast", Description = "Raycast example")]
public sealed class RaycastExample : LibertyModule
{
    internal bool Look(Vec3 from, Vec3 to, Vec3 eye, Vec3 target, PedRef guard)
    {
        PedRef me = Liberty.Player.Ped;
        RayIgnore ignore = RayIgnore.Of(me).And(Liberty.Peds.GetVehicle(me));   // None is skipped
        RayHit hit = Liberty.Query.Raycast(from, to, RayMask.World | RayMask.Vehicles, ignore);
        if (hit.IsHit && hit.Kind == RayEntityKind.Vehicle) { VehicleRef car = hit.Vehicle; /* hit.Position, hit.Normal, hit.Distance */ }

        bool clear = Liberty.Query.HasLineOfSight(eye, target, RayMask.World | RayMask.Vehicles | RayMask.Objects, ignore);
        bool sees = Liberty.Query.HasLineOfSight(guard, Liberty.Player.Ped);    // head to head/chest/pelvis
        return clear && sees;
    }
}

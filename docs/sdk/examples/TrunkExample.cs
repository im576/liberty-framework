// docs/sdk/README.md "Choreography". Compiled against the current SDK by tools/verify.ps1.
using Liberty.Sdk;

[Module("example-trunk", Description = "Choreography example")]
public sealed class TrunkExample : LibertyModule
{
    private bool done, busy;

    private void Next() { done = true; }

    internal void SetBusy(bool value) { busy = value; }

    internal void Open(PedRef ped, VehicleRef car, Vec3 trunkPosition)
    {
        Liberty.Animation.Choreography(this, "trunk")
            .TurnTo(ped, trunkPosition, 450)
            .At(520, () => Liberty.Vehicles.OpenDoor(car, VehicleDoor.Trunk))   // 520 ms into the next step
            .Play(ped, new AnimClip("amb@car_stash", "open_boot"), AnimOptions.Default, 700, 2600)
            .LoopUntil(() => done, body => body.WaitUntil(() => done || busy, 3000).Do(Next))
            .OnComplete(() => Liberty.Tasks.Clear(ped))
            .OnCancel(() => Liberty.Vehicles.CloseDoor(car, VehicleDoor.Trunk))   // also runs if your module stops
            .Begin();
    }
}

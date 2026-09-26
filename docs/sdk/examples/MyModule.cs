// docs/sdk/README.md "A first mod". Compiled against the current SDK by tools/verify.ps1 (SDK examples section).
using Liberty.Sdk;
using Liberty.Sdk.Events;

[Module("my-mod", Version = "1.0.0", Description = "What it does")]
public sealed class MyModule : LibertyModule
{
    protected override void OnStart()
    {
        Liberty.Events.Subscribe<PedDied>(this, e => Liberty.Log.Info(this, "ped died, by player: " + e.ByPlayer));
        Liberty.Commands.Register(this, "hello", "hello - say hi", args => "hi from my-mod");
    }

    protected override void OnUpdate() { /* every frame, or every Interval ms */ }
}

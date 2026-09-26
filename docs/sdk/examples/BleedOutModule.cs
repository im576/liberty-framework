// docs/architecture/ENGINE.md "Writing a module". Compiled against the current SDK by tools/verify.ps1.
using System.Collections;
using Liberty.Sdk;
using Liberty.Sdk.Events;

[Module("bleedout", Order = 60, Description = "Badly wounded peds cower before they die")]
public sealed class BleedOutModule : LibertyModule
{
    protected override void OnStart()
    {
        Interval = 100;                                                  // OnUpdate at most every 100 ms
        Liberty.Events.Subscribe<PedDamaged>(this, OnDamaged);           // react instead of polling
        Liberty.Commands.Register(this, "bleed", "bleed - status", args => "ok");
    }

    private void OnDamaged(PedDamaged e)
    {
        if (e.HealthAfter < 20 && e.ByPlayer) { Liberty.Scheduler.Start(this, "cower", Cower(e.Ped)); }
    }

    private IEnumerator Cower(PedRef ped)
    {
        yield return Wait.Milliseconds(800);                             // straight-line sequences
        if (!Liberty.Peds.Exists(ped)) { yield break; }
        Liberty.Tasks.Cower(ped);
        yield return Wait.Until(() => !Liberty.Peds.Exists(ped) || Liberty.Peds.IsDead(ped), 10000);
    }

    protected override void OnUpdate() { /* periodic work: read Liberty.World instead of calling the game */ }
    protected override void OnStop() { /* entities, menus, locks and patches it owns are released for it */ }
}

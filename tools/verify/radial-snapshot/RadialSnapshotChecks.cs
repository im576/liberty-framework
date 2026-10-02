using System;
using System.Collections.Generic;
using System.Reflection;
using Liberty.Sdk;
using LibertyFramework.Engine;
using LibertyFramework.Engine.Services;
using LibertyFramework.Engine.Ui;
using LibertyFramework.Arsenal.Ui;
using LibertyFramework.Arsenal.Contracts;
using LibertyFramework.DevTools.Menu;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Verify.RadialSnapshot
{
    internal static class RadialSnapshotChecks
    {
        private sealed class Owner : LibertyModule { }
        private sealed class Container : StorageWheel.IHost
        {
            public string Title { get { return "TRUNK"; } }
            public IList<WeaponRecord> Carried { get; private set; }
            public IList<WeaponRecord> Stored { get; private set; }
            public int Capacity { get { return 4; } }
            public bool GunsmithAvailable { get { return false; } }
            internal int Closes, Actions;
            internal bool ThrowOnName;
            internal Container()
            {
                Carried = new List<WeaponRecord> { new WeaponRecord { WeaponId = 12, Ammo = 9, Owned = true } };
                Stored = new List<WeaponRecord> { new WeaponRecord { WeaponId = 13, Ammo = 5 } };
            }
            public string Name(int id) { if (ThrowOnName) { throw new InvalidOperationException("name fixture"); } return "weapon " + id; }
            public int Segment(WeaponRecord record) { return 2; }
            public WeaponRecord DisplacedBy(WeaponRecord record) { return Carried[0]; }
            public string Store(WeaponRecord record) { Actions++; return null; }
            public string Take(WeaponRecord record) { Actions++; return null; }
            public List<MenuItem> GunsmithItems() { return new List<MenuItem>(); }
            public void WheelClosed() { Closes++; }
        }

        private static int passed, failed;
        private static void Check(bool value, string name)
        { Console.WriteLine((value ? "PASS " : "FAIL ") + name); if (value) { passed++; } else { failed++; } }
        private static IMenu[] Published(UiService ui)
        { return (IMenu[])typeof(UiService).GetField("drawMenus", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ui); }
        private static IMenu Open(UiService ui, LibertyModule owner, RadialMenu radial, int selected)
        {
            MethodInfo method = typeof(UiService).GetMethod("OpenRadial", BindingFlags.Instance | BindingFlags.NonPublic,
                null, new[] { typeof(LibertyModule), typeof(RadialMenu), typeof(int) }, null);
            // Allows the identical regression to execute against the preserved pre-fix source.
            if (method == null) { IMenu old = ui.OpenRadial(owner, radial); old.Selected = selected; return old; }
            return (IMenu)method.Invoke(ui, new object[] { owner, radial, selected });
        }
        private static RadialMenu Radial()
        {
            RadialMenu radial = new RadialMenu { Title = "WHEEL" };
            for (int i = 0; i < 5; i++) { radial.Segments.Add(new RadialSegment { Label = () => "slot" }); }
            radial.CenterLines = s => new[] { "selected " + s };
            return radial;
        }
        private static void Draw(LibertyEngine engine)
        {
            engine.OnTick = false;
            try { engine.Ui.Draw(new GTA.GraphicsEventArgs(), c => { }); }
            finally { engine.OnTick = true; }
        }

        private static int Main()
        {
            LibertyEngine engine = new LibertyEngine();
            Owner owner = new Owner { Running = true }, other = new Owner { Running = true };
            LibertyModule caller = new Owner { Running = true };
            engine.CurrentModule = caller;
            int evaluations = 0, accepts = 0, cycles = 0, extras = 0, closes = 0;
            bool ownerTick = true, unpublished = true, cleanupRegistered = true;
            RadialMenu radial = Radial();
            Action observe = () =>
            {
                evaluations++;
                ownerTick &= engine.OnTick && engine.CurrentModule == owner;
                unpublished &= Published(engine.Ui).Length == 0;
                cleanupRegistered &= engine.Ledger.CountOf(owner) == 1 && engine.Input.Captured.Count == 1 && engine.Input.Locked.Count == 1;
            };
            radial.Segments[0].Label = () => { observe(); return "slot0"; };
            radial.Segments[0].Icon = () => { observe(); return TextureRef.None; };
            radial.Segments[0].Badge = () => { observe(); return "badge"; };
            radial.CenterLines = s => { observe(); return new[] { "selected " + s }; };
            radial.OnAccept = s => { accepts++; ownerTick &= engine.OnTick && engine.CurrentModule == owner; return null; };
            radial.OnCycle = (s, d) => cycles++;
            radial.OnX = s => { extras++; return null; };
            radial.OnY = s => { extras++; return null; };
            radial.OnClosed = () => closes++;
            engine.Input.Pending.UnionWith(new[] { PadButton.A, PadButton.DPadRight, PadButton.RightShoulder, PadButton.X, PadButton.Y });
            IMenu view = Open(engine.Ui, owner, radial, 2);
            Check(evaluations == 4 && ownerTick && engine.CurrentModule == caller, "snapshot delegates run under owner tick and restore caller context");
            Check(unpublished && cleanupRegistered && evaluations > 0, "cleanup registered before callbacks; new radial unpublished during preparation");
            Check(view.Selected == 2 && Published(engine.Ui).Length == 1, "initial selected slot published");
            Check(engine.Input.Reads == 0 && accepts == 0 && cycles == 0 && extras == 0 && view.IsOpen, "opening consumes no input or action callbacks");
            int beforeDraw = evaluations;
            Draw(engine);
            Check(engine.Ui.Canvas.Texts.Contains("selected 2") && engine.Ui.Canvas.Sprites.Contains(102), "first published draw has initial centre and highlight without Update");
            Check(evaluations == beforeDraw && engine.Input.Reads == 0, "draw uses snapshot only; no owner callbacks or input reads");
            Check(((RadialMenuView)view).FirstDrawFrame == 100 && engine.Frame == 100, "first draw records unchanged actual engine frame");
            engine.Ui.Update();
            Check(accepts == 1 && cycles == 1 && extras == 2 && view.Selected == 3 && ownerTick, "opening input remains available to ordinary owner-tick Update");
            view.Close(); view.Close();
            Check(closes == 1 && engine.Ledger.Count == 0 && engine.Input.Captured.Count == 0 && engine.Input.Locked.Count == 0 && Published(engine.Ui).Length == 0, "close is idempotent and releases publication capture lock ledger");

            engine.Input.Pending.Clear();
            IMenu zero = engine.Ui.OpenRadial(owner, Radial());
            Draw(engine);
            Check(engine.Ui.Canvas.Texts.Contains("selected 0") && zero.Selected == 0, "unchanged public API draws default slot immediately");
            zero.Close();
            IMenu clamped = Open(engine.Ui, owner, Radial(), 99);
            Draw(engine);
            Check(clamped.Selected == 4 && engine.Ui.Canvas.Texts.Contains("selected 4"), "initial slot clamps before snapshot");
            clamped.Close();
            IMenu lower = Open(engine.Ui, owner, Radial(), -3);
            Draw(engine);
            Check(lower.Selected == 0 && engine.Ui.Canvas.Texts.Contains("selected 0"), "negative initial slot clamps before snapshot");
            lower.Close();
            engine.Input.Pending.Add(PadButton.B);
            int readsBeforeBack = engine.Input.Reads;
            bool closedOnOwnerTick = false;
            RadialMenu cancel = Radial();
            cancel.OnClosed = () => closedOnOwnerTick = engine.OnTick && engine.CurrentModule == owner;
            IMenu pendingCancel = engine.Ui.OpenRadial(owner, cancel);
            Check(pendingCancel.IsOpen && engine.Input.Reads == readsBeforeBack, "opening does not consume pending cancel input");
            engine.Ui.Update();
            Check(!pendingCancel.IsOpen && closedOnOwnerTick && engine.Ledger.Count == 0 && engine.Input.Captured.Count == 0, "ordinary owner-tick cancel releases capture and ledger");
            engine.Input.Pending.Clear();
            RadialMenu empty = new RadialMenu { OnClosed = () => closes++ };
            IMenu noSegments = engine.Ui.OpenRadial(owner, empty);
            Check(!noSegments.IsOpen && Published(engine.Ui).Length == 0 && engine.Ledger.Count == 0 && engine.Input.Captured.Count == 0, "empty radial closes before publication and releases capture ledger");
            noSegments.Close(); // Baseline cleanup lets subsequent regressions run independently.

            IMenu survivor = engine.Ui.OpenRadial(other, Radial());
            RadialMenu broken = Radial();
            bool closeOwner = false;
            broken.CenterLines = s => { throw new InvalidOperationException("centre fixture"); };
            broken.OnClosed = () => { closeOwner = engine.OnTick && engine.CurrentModule == owner; };
            IMenu failedView = engine.Ui.OpenRadial(owner, broken);
            Check(!owner.Running && !failedView.IsOpen && closeOwner && engine.CurrentModule == caller, "initial snapshot failure isolated under owner and closes menu");
            Check(engine.Ledger.CountOf(owner) == 0 && !engine.Input.Captured.Contains(failedView) && !engine.Input.Locked.Contains(failedView) && Published(engine.Ui).Length == 1 && survivor.IsOpen && other.Running, "failed owner cleanup preserves other owner menu");
            failedView.Close(); survivor.Close(); owner.Running = true;
            int errors = RuntimeLog.Errors.Count;
            RadialMenu tolerant = Radial();
            tolerant.Segments[0].Label = () => { throw new InvalidOperationException("segment fixture"); };
            IMenu tolerated = engine.Ui.OpenRadial(owner, tolerant);
            engine.Ui.Update(); Draw(engine);
            Check(tolerated.IsOpen && owner.Running && RuntimeLog.Errors.Count == errors + 1 && engine.Ui.Canvas.Texts.Contains("selected 0"), "segment failure remains tolerated and logged once");
            engine.Ledger.ReleaseAll(owner);
            Check(!tolerated.IsOpen && engine.Input.Captured.Count == 0 && engine.Input.Locked.Count == 0 && Published(engine.Ui).Length == 0, "owner ledger release closes initialized radial");

            int releaseErrors = RuntimeLog.Errors.Count;
            RadialMenu closeThrows = Radial();
            closeThrows.OnClosed = () => { throw new InvalidOperationException("close fixture"); };
            IMenu releaseView = engine.Ui.OpenRadial(owner, closeThrows);
            engine.Ledger.ReleaseAll(owner);
            Check(!releaseView.IsOpen && engine.Ledger.Count == 0 && engine.Input.Captured.Count == 0 && engine.Input.Locked.Count == 0 && Published(engine.Ui).Length == 0 && RuntimeLog.Errors.Count == releaseErrors + 1, "throwing close callback logged after real ledger and capture cleanup");

            Container container = new Container();
            StorageWheel storage = new StorageWheel(owner);
            engine.Input.Pending.Add(PadButton.A);
            storage.Open(container); Draw(engine);
            Check(storage.IsOpen && storage.StatusLine().Contains("selected=2") && engine.Ui.Canvas.Texts.Contains("weapon 12") && engine.Ui.Canvas.Sprites.Contains(102), "actual StorageWheel first carried slot selected before first snapshot");
            Check(engine.Ui.Canvas.Texts.Contains("A  Swap in weapon 13") && container.Actions == 0 && engine.Input.Locked.Count == 0, "actual storage centre/panel prepared without take or extra control lock");
            storage.Close();
            Check(container.Closes == 0 && !storage.IsOpen && engine.Ledger.Count == 0 && engine.Input.Captured.Count == 0, "external storage close retains host callback semantics and cleanup");
            storage.Open(container); engine.Ledger.ReleaseAll(owner);
            Check(container.Closes == 1 && !storage.IsOpen && engine.Input.Captured.Count == 0, "owner-stop storage close notifies host once");
            Container brokenContainer = new Container { ThrowOnName = true };
            owner.Running = true;
            storage.Open(brokenContainer);
            Check(!owner.Running && !storage.IsOpen && brokenContainer.Closes == 1 && engine.Ledger.Count == 0 && engine.Input.Captured.Count == 0 && Published(engine.Ui).Length == 0, "actual storage initial centre failure cleans host panel menu ledger");
            Console.WriteLine("Radial snapshot focused checks: " + passed + " passed / " + failed + " failed");
            return failed == 0 ? 0 : 1;
        }
    }
}

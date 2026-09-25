using System;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Arsenal.Ui
{
    // S-3: Niko physically uses the trunk, with GTA IV's own animations:
    //   turn to the trunk -> amb@car_stash "open_boot" (the lid opens part-way through) -> amb@car_stash "idle" held while
    //   the wheel is open -> "boot_withdraw" for every take/store -> car_boot "close_boot" (the lid shuts part-way).
    // Durations are not read from the clips; each step ends when its clip stops playing or after a timeout, so a clip
    // that fails to load never traps the player.
    internal sealed class TrunkChoreography
    {
        private enum Step { Idle, Turning, Opening, Browsing, Handling, Closing }

        private static readonly AnimationSet Stash = new AnimationSet("amb@car_stash");
        private static readonly AnimationSet Boot = new AnimationSet("car_boot");
        private const float BlendSpeed = 4.0f;

        private Step step = Step.Idle;
        private Ped ped;
        private Vehicle vehicle;
        private int stepStarted;
        private bool doorMoved;

        internal bool Active { get { return step != Step.Idle; } }
        internal bool WheelReady { get { return step == Step.Browsing || step == Step.Handling; } }
        internal bool Closing { get { return step == Step.Closing; } }

        internal void Begin(Ped player, Vehicle trunk)
        {
            ped = player; vehicle = trunk;
            ped.Task.ClearAll();
            ped.Task.TurnTo(trunk.Position);
            Enter(Step.Turning);
            RuntimeLog.Info("trunk_anim begin");
        }

        // A take or store just happened: reach into the trunk.
        internal void Handle()
        {
            if (step != Step.Browsing && step != Step.Handling) { return; }
            Play(Stash, "boot_withdraw");
            Enter(Step.Handling);
        }

        internal void Close()
        {
            if (step == Step.Idle || step == Step.Closing) { return; }
            Play(Boot, "close_boot");
            Enter(Step.Closing);
        }

        // Returns true while the sequence still runs.
        internal bool Update()
        {
            if (step == Step.Idle) { return false; }
            if (ped == null || !ped.Exists() || ped.isDead || vehicle == null || !vehicle.Exists()) { Abort(); return false; }
            int elapsed = Environment.TickCount - stepStarted;
            switch (step)
            {
                case Step.Turning:
                    if (elapsed >= 450) { Play(Stash, "open_boot"); Enter(Step.Opening); }
                    break;
                case Step.Opening:
                    if (!doorMoved && elapsed >= 520) { vehicle.Door(VehicleDoor.Trunk).Open(); doorMoved = true; }
                    if ((elapsed >= 700 && !Playing(Stash, "open_boot")) || elapsed >= 2600) { Play(Stash, "idle"); Enter(Step.Browsing); }
                    break;
                case Step.Browsing:
                    if (!Playing(Stash, "idle") && elapsed >= 300) { Play(Stash, "idle"); stepStarted = Environment.TickCount; }
                    break;
                case Step.Handling:
                    if ((elapsed >= 500 && !Playing(Stash, "boot_withdraw")) || elapsed >= 2400) { Play(Stash, "idle"); Enter(Step.Browsing); }
                    break;
                case Step.Closing:
                    if (!doorMoved && elapsed >= 450) { vehicle.Door(VehicleDoor.Trunk).Close(); doorMoved = true; }
                    if ((elapsed >= 700 && !Playing(Boot, "close_boot")) || elapsed >= 2400) { Finish(); return false; }
                    break;
            }
            return true;
        }

        // Error/reload path: stop animating and shut the lid now.
        internal void Abort()
        {
            if (step == Step.Idle) { return; }
            try
            {
                if (vehicle != null && vehicle.Exists()) { vehicle.Door(VehicleDoor.Trunk).Close(); }
                if (ped != null && ped.Exists()) { ped.Task.ClearAll(); }
            }
            catch (Exception error) { RuntimeLog.Error("trunk_anim_abort_failed error=" + error.Message); }
            step = Step.Idle;
        }

        private void Finish()
        {
            if (vehicle != null && vehicle.Exists() && !doorMoved) { vehicle.Door(VehicleDoor.Trunk).Close(); }
            if (ped != null && ped.Exists()) { ped.Task.ClearAll(); }
            step = Step.Idle;
            RuntimeLog.Info("trunk_anim end");
        }

        private void Enter(Step next)
        {
            step = next;
            stepStarted = Environment.TickCount;
            if (next == Step.Opening || next == Step.Closing) { doorMoved = false; }
        }

        private void Play(AnimationSet set, string name)
        {
            try { ped.Task.PlayAnimation(set, name, BlendSpeed); }
            catch (Exception error) { RuntimeLog.Error("trunk_anim_play_failed " + set.Name + "/" + name + " error=" + error.Message); }
        }

        private bool Playing(AnimationSet set, string name)
        {
            try { return ped.Animation.isPlaying(set, name); }
            catch (Exception) { return false; }
        }
    }
}

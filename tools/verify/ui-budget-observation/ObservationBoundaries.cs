using System;
using Liberty.Sdk;
using LibertyFramework.Engine;

// No game queries: actual PerfService, CostMeter, RuntimeLog, ResourceLedger and TrunkSequence are compiled.
// These engine/SHDN boundaries throw if an observational path tries to call the game.
namespace GTA
{
    internal static class Game
    {
        internal static class Console { internal static void Print(string text) { throw new InvalidOperationException(text); } }
    }
}
namespace LibertyFramework.Core.Config
{
    internal static class LibertyPaths { internal static string Root { get; set; } }
}
namespace LibertyFramework.Engine.Performance { }
namespace LibertyFramework.Engine
{
    internal sealed class LibertyEngine
    {
        internal static LibertyEngine Current;
        internal int Frame { get; set; }
        internal ResourceLedger Ledger { get; private set; } = new ResourceLedger();
        internal TestGovernor Governor { get; private set; } = new TestGovernor();
        internal TestWatchdog Watchdog { get; private set; } = new TestWatchdog();
        internal TestAnimation Animation { get; private set; } = new TestAnimation();
        internal TestTasks Tasks { get; private set; } = new TestTasks();
        internal TestVehicles Vehicles { get; private set; } = new TestVehicles();
        internal TestPeds Peds { get; private set; } = new TestPeds();
        internal void RequireOwner(LibertyModule owner) { if (owner == null) { throw new ArgumentNullException("owner"); } }
    }
    internal sealed class TestGovernor { internal float Pressure { get { return 0; } } }
    internal sealed class TestWatchdog { internal TestMemory Memory { get; private set; } = new TestMemory(); }
    internal sealed class TestMemory
    {
        internal long PrivateBytes { get { return 0; } }
        internal long AddressSpaceFreeBytes { get { return 0; } }
        internal long LargestFreeBlockBytes { get { return 0; } }
    }
    internal sealed class TestAnimation
    {
        internal IChoreographyBuilder Choreography(LibertyModule owner, string name) { throw new InvalidOperationException("game choreography"); }
        internal bool Play(PedRef ped, AnimClip clip, AnimOptions options) { throw new InvalidOperationException("game play"); }
        internal bool IsPlaying(PedRef ped, AnimClip clip) { throw new InvalidOperationException("game query"); }
    }
    internal sealed class TestTasks
    { internal void Clear(PedRef ped) { throw new InvalidOperationException("game clear"); } }
    internal sealed class TestVehicles
    {
        internal bool Exists(VehicleRef vehicle) { throw new InvalidOperationException("game exists"); }
        internal void OpenDoor(VehicleRef vehicle, VehicleDoor door) { throw new InvalidOperationException("game open"); }
        internal void CloseDoor(VehicleRef vehicle, VehicleDoor door) { throw new InvalidOperationException("game close"); }
    }
    internal sealed class TestPeds
    { internal bool Exists(PedRef ped) { throw new InvalidOperationException("game exists"); } }
}

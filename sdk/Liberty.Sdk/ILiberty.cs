namespace Liberty.Sdk
{
    // The engine's public services (SDK 1.0). Everything a gameplay mod needs goes through here: no ScriptHookDotNet,
    // raw natives or memory addresses.
    public interface ILiberty
    {
        string EngineVersion { get; }
        Episode Episode { get; }
        IWorldState World { get; }
        IWorldQuery Query { get; }
        IEventBus Events { get; }
        IScheduler Scheduler { get; }
        IPlayer Player { get; }
        IPeds Peds { get; }
        IVehicles Vehicles { get; }
        IProps Props { get; }
        IWeapons Weapons { get; }
        ITasks Tasks { get; }
        ICameras Cameras { get; }
        IAnimation Animation { get; }
        IFx Fx { get; }
        IAudio Audio { get; }
        IStreaming Streaming { get; }
        IInput Input { get; }
        IUi Ui { get; }
        IWorldControl WorldControl { get; }
        IBlips Blips { get; }
        IState State { get; }
        IConfig Config { get; }
        ICommands Commands { get; }
        ILog Log { get; }
        IPerf Perf { get; }
        IModules Modules { get; }
        // Privileged (capability-checked) low-level access.
        IMemory Memory { get; }
        INatives Natives { get; }
    }
}
namespace Liberty.Sdk
{
    // Per-module save data (DataContract types), stored as state\<module>\<name>.json. Load returns null when absent
    // or unreadable (logged); Save is atomic with one backup.
    public interface IState
    {
        T Load<T>(LibertyModule owner, string name) where T : class;
        void Save<T>(LibertyModule owner, string name, T value) where T : class;
    }
}
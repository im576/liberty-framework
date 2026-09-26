namespace Liberty.Sdk
{
    public interface ICoroutine
    {
        string Name { get; }
        bool Finished { get; }
        // True when the last Wait.Until ended by timeout instead of its condition.
        bool LastTimedOut { get; }
        void Cancel();
    }
}
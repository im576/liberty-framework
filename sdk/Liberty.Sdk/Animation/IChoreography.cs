namespace Liberty.Sdk
{
    public interface IChoreography
    {
        string Name { get; }
        bool IsRunning { get; }
        bool Completed { get; }
        int StepIndex { get; }
        // Stops at the current step and runs the OnCancel cleanup.
        void Cancel();
    }
}
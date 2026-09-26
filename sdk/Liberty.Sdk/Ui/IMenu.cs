namespace Liberty.Sdk
{
    // An open menu. Closing (by B, Close() or the owner stopping) releases input.
    public interface IMenu
    {
        bool IsOpen { get; }
        int Selected { get; set; }
        // Shows a status line for a moment.
        void Message(string text);
        void Close();
    }
}
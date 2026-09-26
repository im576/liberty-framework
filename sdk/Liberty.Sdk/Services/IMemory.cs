namespace Liberty.Sdk
{
    // Game memory for privileged modules (Capabilities.MemoryPatch). Addresses come from signatures, never constants.
    // Every write is recorded and the original bytes are restored when the module stops or the scripts reload.
    public interface IMemory
    {
        // First match of an IDA-style pattern ("8B 0D ?? ?? ?? ?? E8") in GTAIV.exe code, or 0.
        uint FindPattern(string pattern);
        // Handler address of a script native by CE hash, or 0.
        uint FindNative(uint hash);
        bool IsReadable(uint address, int length);
        byte[] Read(uint address, int length);
        int ReadInt32(uint address);
        float ReadSingle(uint address);
        bool Patch(LibertyModule owner, uint address, byte[] bytes);
        void RestoreAll(LibertyModule owner);
        string GameVersion { get; }
    }
}
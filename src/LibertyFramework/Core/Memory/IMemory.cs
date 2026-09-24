namespace LibertyFramework.Core.Memory
{
    // Read-only view of a 32-bit GTAIV.exe address space. The live game and the offline
    // verifier (which maps GTAIV.exe from disk at its preferred base) share this contract,
    // so every address resolver can be checked without running the game.
    internal interface IMemory
    {
        uint ModuleBase { get; }
        bool IsReadable(uint address, int length);
        byte[] Read(uint address, int length);
    }
}

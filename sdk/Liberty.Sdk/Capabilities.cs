namespace Liberty.Sdk
{
    // Privileges a module declares in ModuleAttribute.Capabilities. Normal gameplay mods need none of them.
    public static class Capabilities
    {
        // Use engine internals (raw natives, memory, ScriptHookDotNet) directly. Engine-level modules only.
        public const string EngineInternal = "engine.internal";
        // Write game memory through IMemory (tracked and restored when the module stops).
        public const string MemoryPatch = "memory.patch";
        // Take player control away (cutscenes, menus); restored automatically.
        public const string PlayerControl = "player.control";
        // Capture controller/keyboard input for UI focus.
        public const string InputCapture = "input.capture";
        // Run developer/test commands that change the world (teleport, spawn, time).
        public const string Developer = "developer";
    }
}
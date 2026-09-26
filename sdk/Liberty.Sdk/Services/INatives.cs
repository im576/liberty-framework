namespace Liberty.Sdk
{
    // Raw script-native calls for cases no service covers yet (Capabilities.EngineInternal). Arguments: int, float,
    // bool, string, handles (PedRef/VehicleRef/PropRef) and NativeOut slots for out-parameters.
    public interface INatives
    {
        int CallInt(LibertyModule owner, string name, params object[] args);
        float CallFloat(LibertyModule owner, string name, params object[] args);
        bool CallBool(LibertyModule owner, string name, params object[] args);
        void Call(LibertyModule owner, string name, params object[] args);
    }
}
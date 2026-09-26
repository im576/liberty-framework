namespace Liberty.Sdk
{
    // An out-parameter slot for INatives: pass NativeOut.Int() / NativeOut.Float(), read Value after the call.
    public sealed class NativeOut
    {
        public bool IsFloat { get; private set; }
        public object Value { get; set; }
        public static NativeOut Int() { return new NativeOut(); }
        public static NativeOut Float() { NativeOut o = new NativeOut(); o.IsFloat = true; return o; }
    }
}
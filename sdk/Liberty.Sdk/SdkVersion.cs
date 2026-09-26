namespace Liberty.Sdk
{
    // Liberty SDK version. Mods declare the SDK version they were built for (ModuleAttribute.SdkVersion); the engine
    // loads a mod when the major versions match and the mod's minor version is not newer than the engine's.
    // 1.1: IWorldQuery raycast and line of sight (RayMask, RayHit, RayIgnore, RayStatus, RayEntityKind).
    public static class SdkVersion
    {
        public const int Major = 1;
        public const int Minor = 1;
        public const int Patch = 0;
        public const string Text = "1.1.0";

        public static bool IsCompatible(string required)
        {
            if (string.IsNullOrEmpty(required)) { return true; }
            string[] parts = required.Split('.');
            int major, minor = 0;
            if (!int.TryParse(parts[0], out major)) { return false; }
            if (parts.Length > 1 && !int.TryParse(parts[1], out minor)) { return false; }
            return major == Major && minor <= Minor;
        }
    }
}
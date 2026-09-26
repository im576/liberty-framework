using System.Globalization;

namespace Liberty.Autopilot
{
    // Command argument parsing (invariant culture: scenario files use '.' decimals everywhere).
    internal static class Args
    {
        internal static bool On(string[] args) { return args.Length == 0 || args[0] == "on" || args[0] == "1" || args[0] == "true"; }
        internal static int Int(string[] args, int index) { return int.Parse(args[index], CultureInfo.InvariantCulture); }
        internal static int Int(string[] args, int index, int fallback) { return args.Length > index ? Int(args, index) : fallback; }
        internal static float Float(string[] args, int index) { return float.Parse(args[index], CultureInfo.InvariantCulture); }
        internal static float Float(string[] args, int index, float fallback) { return args.Length > index ? Float(args, index) : fallback; }
        internal static string F(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
    }
}

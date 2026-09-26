using System;
using System.Globalization;

namespace Liberty.Autopilot
{
    // Command argument parsing (invariant culture: scenario files use '.' decimals everywhere).
    internal static class Args
    {
        internal static bool On(string[] args) { return args.Length == 0 || args[0] == "on" || args[0] == "1" || args[0] == "true"; }
        // Missing or malformed arguments throw ArgumentException: the engine replies with the message and the command's
        // usage, and the module keeps running (a typo in the console must not stop the autopilot).
        internal static int Int(string[] args, int index)
        {
            int value;
            if (!int.TryParse(Word(args, index), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) { throw Bad(args, index, "a whole number"); }
            return value;
        }
        internal static int Int(string[] args, int index, int fallback) { return args.Length > index ? Int(args, index) : fallback; }
        internal static float Float(string[] args, int index)
        {
            float value;
            if (!float.TryParse(Word(args, index), NumberStyles.Float, CultureInfo.InvariantCulture, out value)) { throw Bad(args, index, "a number"); }
            return value;
        }
        internal static float Float(string[] args, int index, float fallback) { return args.Length > index ? Float(args, index) : fallback; }
        internal static string Word(string[] args, int index)
        {
            if (index >= args.Length) { throw new ArgumentException("argument " + (index + 1) + " is missing"); }
            return args[index];
        }

        private static ArgumentException Bad(string[] args, int index, string expected)
        {
            return new ArgumentException("argument " + (index + 1) + " '" + args[index] + "' is not " + expected);
        }

        internal static string F(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
    }
}

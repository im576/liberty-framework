using System;

namespace LibertyFramework.Verify
{
    internal sealed class Checker
    {
        internal int Passed;
        internal int Failed;

        internal void True(string name, bool condition, string detail)
        {
            if (condition) { Passed++; Console.WriteLine("PASS " + name + (detail.Length > 0 ? " (" + detail + ")" : "")); }
            else { Failed++; Console.WriteLine("FAIL " + name + (detail.Length > 0 ? " (" + detail + ")" : "")); }
        }

        internal void Equal(string name, uint expected, uint actual)
        {
            True(name, expected == actual, "expected 0x" + expected.ToString("X") + " actual 0x" + actual.ToString("X"));
        }

        internal void Equal(string name, int expected, int actual)
        {
            True(name, expected == actual, "expected " + expected + " actual " + actual);
        }

        internal void Near(string name, double expected, double actual, double tolerance)
        {
            True(name, Math.Abs(expected - actual) <= tolerance,
                "expected " + expected.ToString("0.####") + " actual " + actual.ToString("0.####") + " tol " + tolerance);
        }
    }
}

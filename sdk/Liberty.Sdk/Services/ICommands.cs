using System;

namespace Liberty.Sdk
{
    // Commands reachable from the in-game console ("lf <command>") and the autopilot's file channel.
    public interface ICommands
    {
        void Register(LibertyModule owner, string name, string usage, Func<string[], string> handler);
        string Execute(string line, string source);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // Named commands any module can register. Reached from the ScriptHookDotNet console ("lf <command> ...") and from
    // the file channel scripts\LibertyFramework\autopilot\inbox\*.cmd (one command per line), which the autopilot uses:
    // replies go to autopilot\outbox\<name>.out and every command is logged. Commands run on the engine thread.
    public sealed class CommandRegistry : ICommands
    {
        private sealed class Entry
        {
            internal LibertyModule Owner;
            internal string Usage;
            internal Func<string[], string> Handler;
        }

        private readonly Dictionary<string, Entry> commands = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        private int lastPollMs;
        // The engine's current-module context around each handler (capability checks, implicit ownership).
        internal Action<LibertyModule> Enter = m => { };
        internal Func<LibertyModule> Current = () => null;
        private const int PollIntervalMs = 250;

        public static string Inbox { get { return Path.Combine(LibertyPaths.Root, Path.Combine("autopilot", "inbox")); } }
        public static string Outbox { get { return Path.Combine(LibertyPaths.Root, Path.Combine("autopilot", "outbox")); } }

        // owner null = engine command. Handler gets the words after the command name and returns the reply.
        public void Register(LibertyModule owner, string name, string usage, Func<string[], string> handler)
        {
            Entry entry = new Entry();
            entry.Owner = owner; entry.Usage = usage; entry.Handler = handler;
            Entry previous;
            if (commands.TryGetValue(name, out previous) && previous.Owner != owner)
            {
                // Last registration wins (unchanged behaviour); logged because the earlier owner silently loses the command.
                RuntimeLog.Error("command_replaced name=" + name + " was=" + (previous.Owner != null ? previous.Owner.Id : "engine") +
                    " now=" + (owner != null ? owner.Id : "engine"));
            }
            commands[name] = entry;
        }

        internal void RemoveOwner(LibertyModule owner)
        {
            foreach (string name in commands.Where(pair => pair.Value.Owner == owner).Select(pair => pair.Key).ToList()) { commands.Remove(name); }
        }

        public IEnumerable<string> Names { get { return commands.Keys.OrderBy(k => k); } }

        public string Execute(string line, string source)
        {
            string[] words = (line ?? "").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) { return "empty command"; }
            if (words[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                return string.Join("; ", commands.OrderBy(p => p.Key).Select(p => p.Key + ": " + p.Value.Usage).ToArray());
            }
            Entry entry;
            string reply;
            if (!commands.TryGetValue(words[0], out entry)) { reply = "unknown command " + words[0]; }
            else if (entry.Owner != null && !entry.Owner.Running) { reply = "module " + entry.Owner.Id + " is not running"; }
            else
            {
                LibertyModule previous = Current();
                Enter(entry.Owner);
                try { reply = entry.Handler(words.Skip(1).ToArray()) ?? "ok"; }
                catch (Exception error)
                {
                    reply = "error " + error.Message;
                    if (entry.Owner != null) { LibertyEngine.Current.Fail(entry.Owner, error); }
                    else { RuntimeLog.Error("command_failed name=" + words[0] + " error=" + error); }
                }
                finally { Enter(previous); }
            }
            RuntimeLog.Info("command source=" + source + " line=\"" + line + "\" reply=\"" + reply + "\"");
            return reply;
        }

        internal void PumpFileChannel()
        {
            int now = Environment.TickCount;
            if (unchecked(now - lastPollMs) < PollIntervalMs) { return; }
            lastPollMs = now;
            if (!Directory.Exists(Inbox)) { return; }
            string[] files;
            try { files = Directory.GetFiles(Inbox, "*.cmd"); }
            catch (Exception error) { RuntimeLog.Error("command_inbox_failed error=" + error.Message); return; }
            if (files.Length == 0) { return; }
            Array.Sort(files, StringComparer.Ordinal);
            string file = files[0];
            try
            {
                string[] lines = File.ReadAllLines(file);
                File.Delete(file);
                StringBuilder replies = new StringBuilder();
                foreach (string line in lines)
                {
                    if (line.Trim().Length == 0 || line.TrimStart().StartsWith("#")) { continue; }
                    replies.AppendLine(line + " => " + Execute(line, "file:" + Path.GetFileName(file)));
                }
                Directory.CreateDirectory(Outbox);
                File.WriteAllText(Path.Combine(Outbox, Path.GetFileNameWithoutExtension(file) + ".out"), replies.ToString());
            }
            catch (IOException error)
            {
                // The autopilot may still be writing it: leave it for the next poll (250 ms).
                RuntimeLog.Info("command_file_busy file=" + Path.GetFileName(file) + " error=" + error.Message);
            }
            catch (Exception error) { RuntimeLog.Error("command_file_failed file=" + Path.GetFileName(file) + " error=" + error.Message); }
        }
    }
}
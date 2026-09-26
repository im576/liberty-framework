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
        private readonly Action<LibertyModule, Exception> onFailed;
        // The engine's current-module context around each handler (capability checks, implicit ownership).
        internal Action<LibertyModule> Enter = m => { };
        internal Func<LibertyModule> Current = () => null;
        private const int PollIntervalMs = 250;

        internal CommandRegistry(Action<LibertyModule, Exception> onFailed) { this.onFailed = onFailed; }

        public static string Inbox { get { return Path.Combine(LibertyPaths.Root, Path.Combine("autopilot", "inbox")); } }
        public static string Outbox { get { return Path.Combine(LibertyPaths.Root, Path.Combine("autopilot", "outbox")); } }

        // The module's handler gets the words after the command name and returns the reply. A name is held by one owner at
        // a time: a second module (or a module reusing an engine command's name) is refused and logged, so no module can
        // silently take over another's command. A module's names are freed when it stops (reload and restart re-register).
        public void Register(LibertyModule owner, string name, string usage, Func<string[], string> handler)
        {
            if (owner == null) { throw new ArgumentNullException("owner", "pass the calling module (this) as owner"); }
            Add(owner, name, usage, handler);
        }

        // Engine commands (no owning module).
        internal void RegisterEngine(string name, string usage, Func<string[], string> handler) { Add(null, name, usage, handler); }

        private void Add(LibertyModule owner, string name, string usage, Func<string[], string> handler)
        {
            if (string.IsNullOrEmpty(name) || name.IndexOfAny(new[] { ' ', '\t' }) >= 0) { throw new ArgumentException("command name must be one word", "name"); }
            if (handler == null) { throw new ArgumentNullException("handler"); }
            Entry previous;
            if (commands.TryGetValue(name, out previous) && previous.Owner != owner && (previous.Owner == null || previous.Owner.Running))
            {
                RuntimeLog.Error("command_refused name=" + name + " held_by=" + OwnerName(previous.Owner) + " requested_by=" + OwnerName(owner));
                return;
            }
            Entry entry = new Entry();
            entry.Owner = owner; entry.Usage = usage; entry.Handler = handler;
            commands[name] = entry;
        }

        private static string OwnerName(LibertyModule owner) { return owner != null ? owner.Id : "engine"; }

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
                catch (Exception error) when (error is ArgumentException || error is FormatException)
                {
                    // Bad input from whoever typed the command (a missing or malformed argument): the reply says so and the
                    // module keeps running.
                    reply = "error " + error.Message + " (usage: " + entry.Usage + ")";
                    RuntimeLog.Error("command_rejected name=" + words[0] + " owner=" + OwnerName(entry.Owner) + " error=" + error.Message);
                }
                catch (Exception error)
                {
                    reply = "error " + error.Message;
                    if (entry.Owner != null) { onFailed(entry.Owner, error); }
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
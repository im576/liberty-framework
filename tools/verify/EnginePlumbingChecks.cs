using System;
using System.Collections;
using System.Collections.Generic;
using Liberty.Sdk;
using LibertyFramework.Engine;
using LibertyFramework.Engine.Events;
using LibertyFramework.Engine.Scheduling;
using LibertyFramework.Engine.Services;

namespace LibertyFramework.Verify
{
    // Engine plumbing without the game (engine audit, docs/reports/2026-09-26-engine-audit.md): a module's error stops
    // that module alone and never the engine frame, no event is skipped when a handler's module stops mid-publish,
    // unowned resources are refused, commands cannot be taken over, and manifests cannot be rewritten by readers.
    internal static class EnginePlumbingChecks
    {
        private sealed class TestModule : LibertyModule { }

        private struct Ping { internal int Value; }

        internal static void Run(Checker check)
        {
            SchedulerChecks(check);
            EventBusChecks(check);
            LedgerChecks(check);
            CommandChecks(check);
            ManifestChecks(check);
        }

        private static TestModule Module(string id, params string[] capabilities)
        {
            ModuleAttribute attribute = new ModuleAttribute(id);
            attribute.Capabilities = capabilities;
            TestModule module = new TestModule();
            module.Manifest = ModuleManifest.From(attribute, "verify");
            module.Running = true;
            return module;
        }

        // What LibertyEngine.Fail does, reduced to the parts these classes see: the module stops.
        private static Action<LibertyModule, Exception> Failer(List<string> failed)
        {
            return (module, error) => { failed.Add(module.Id); module.Running = false; };
        }

        private static IEnumerator Counting(int[] counter)
        {
            while (true) { counter[0]++; yield return Wait.NextFrame(); }
        }

        private static IEnumerator WaitingOn(Func<bool> condition)
        {
            yield return Wait.Until(condition, 60000);
        }

        private static void SchedulerChecks(Checker check)
        {
            List<string> failed = new List<string>();
            Scheduler scheduler = new Scheduler(Failer(failed));
            TestModule bad = Module("bad"), good = Module("good");
            LibertyModule current = null, seenInCondition = null;
            int[] ticks = { 0 };
            scheduler.Start(bad, "throws-in-condition", WaitingOn(() => { seenInCondition = current; throw new InvalidOperationException("condition"); }));
            scheduler.Start(good, "counter", Counting(ticks));
            bool escaped = false;
            for (int frame = 0; frame < 3; frame++)
            {
                try { scheduler.Run(m => current = m); }
                catch (Exception) { escaped = true; }
            }
            check.True("scheduler: a throwing Wait.Until condition does not escape Run", !escaped, "");
            check.True("scheduler: the condition's module is failed, once", failed.Count == 1 && failed[0] == "bad", string.Join(",", failed.ToArray()));
            check.True("scheduler: the condition runs as its module (capability context)", seenInCondition == bad, seenInCondition == null ? "null" : seenInCondition.Id);
            check.Equal("scheduler: the other module's coroutine keeps running every frame", 3, ticks[0]);
            check.True("scheduler: context is cleared after Run", current == null, "");

            bool refused = false;
            try { scheduler.Start(null, "unowned", Counting(new int[1])); }
            catch (ArgumentNullException) { refused = true; }
            check.True("scheduler: a coroutine without an owner is refused at Start", refused, "");
            bool frameOk = true;
            try { scheduler.Run(m => { }); } catch (Exception) { frameOk = false; }
            check.True("scheduler: Run still works after the refused Start", frameOk && ticks[0] == 4, "ticks=" + ticks[0]);
        }

        private static void EventBusChecks(Checker check)
        {
            List<string> failed = new List<string>();
            EventBus bus = null;
            // Like the engine: the failing module stops and its handlers are removed while the publish is still running.
            bus = new EventBus((module, error) => { failed.Add(module.Id); module.Running = false; bus.RemoveOwner(module); });
            TestModule a = Module("a"), b = Module("b"), c = Module("c");
            List<string> seen = new List<string>();
            bus.Subscribe<Ping>(a, p => seen.Add("a" + p.Value));
            bus.Subscribe<Ping>(b, p => { seen.Add("b" + p.Value); throw new InvalidOperationException("b"); });
            bus.Subscribe<Ping>(c, p => seen.Add("c" + p.Value));
            bus.Publish(new Ping { Value = 1 });
            check.True("events: a handler whose module fails mid-publish does not skip the next module", string.Join(",", seen.ToArray()) == "a1,b1,c1", string.Join(",", seen.ToArray()));
            check.True("events: the throwing handler's module is failed", failed.Count == 1 && failed[0] == "b", string.Join(",", failed.ToArray()));
            seen.Clear();
            bus.Publish(new Ping { Value = 2 });
            check.True("events: the failed module gets no further events", string.Join(",", seen.ToArray()) == "a2,c2", string.Join(",", seen.ToArray()));

            // Unsubscribe during a publish, and subscribe during a publish.
            EventBus bus2 = new EventBus(Failer(failed));
            TestModule x = Module("x"), y = Module("y"), z = Module("z");
            List<string> order = new List<string>();
            Action<Ping> yHandler = p => order.Add("y" + p.Value);
            bus2.Subscribe<Ping>(x, p =>
            {
                order.Add("x" + p.Value);
                if (p.Value == 1) { bus2.Unsubscribe<Ping>(y, yHandler); bus2.Subscribe<Ping>(x, q => order.Add("late" + q.Value)); }
            });
            bus2.Subscribe<Ping>(y, yHandler);
            bus2.Subscribe<Ping>(z, p => order.Add("z" + p.Value));
            bus2.Publish(new Ping { Value = 1 });
            check.True("events: unsubscribing during a publish skips only that handler", string.Join(",", order.ToArray()) == "x1,z1", string.Join(",", order.ToArray()));
            order.Clear();
            bus2.Publish(new Ping { Value = 2 });
            check.True("events: a handler added during a publish starts with the next event", string.Join(",", order.ToArray()) == "x2,z2,late2", string.Join(",", order.ToArray()));

            // A handler that publishes the same event type (nested publish) sees consistent lists.
            EventBus bus3 = new EventBus(Failer(failed));
            TestModule n = Module("n");
            int depth = 0, calls = 0;
            bus3.Subscribe<Ping>(n, p => { calls++; if (depth++ == 0) { bus3.Publish(new Ping { Value = 9 }); } });
            bus3.Publish(new Ping { Value = 1 });
            check.Equal("events: nested publish of the same type delivers both events", 2, calls);
        }

        private static void LedgerChecks(Checker check)
        {
            ResourceLedger ledger = new ResourceLedger();
            bool refused = false;
            try { ledger.Add(null, "control", 0, () => { }); }
            catch (ArgumentNullException) { refused = true; }
            check.True("ledger: an entry without an owner is refused (it could never be released)", refused && ledger.Count == 0, "count=" + ledger.Count);

            TestModule m = Module("m");
            List<int> released = new List<int>();
            ledger.Add(m, "fx", 1, () => released.Add(1));
            ledger.Add(m, "fx", 2, () => { released.Add(2); throw new InvalidOperationException("release"); });
            ledger.Add(m, "fx", 3, () => released.Add(3));
            int count = ledger.ReleaseAll(m);
            check.True("ledger: a module's resources are released newest first, past a throwing release", count == 3 && string.Join(",", released.ConvertAll(i => i.ToString()).ToArray()) == "3,2,1",
                "count=" + count + " order=" + string.Join(",", released.ConvertAll(i => i.ToString()).ToArray()));
        }

        private static void CommandChecks(Checker check)
        {
            List<string> failed = new List<string>();
            CommandRegistry commands = new CommandRegistry(Failer(failed));
            TestModule a = Module("a"), b = Module("b");
            commands.RegisterEngine("engine", "engine status", args => "engine-reply");
            commands.Register(a, "hello", "hello <n>", args => "a:" + int.Parse(args[0]));
            commands.Register(b, "hello", "hello (b)", args => "b");
            commands.Register(b, "engine", "stolen", args => "b");
            check.True("commands: a second module cannot take over a running module's command", commands.Execute("hello 5", "verify") == "a:5", commands.Execute("hello 5", "verify"));
            check.True("commands: a module cannot take over an engine command", commands.Execute("engine", "verify") == "engine-reply", commands.Execute("engine", "verify"));

            string reply = commands.Execute("hello x", "verify");
            check.True("commands: bad input (FormatException) is an error reply with the usage", reply.StartsWith("error ") && reply.Contains("usage: hello <n>"), reply);
            check.True("commands: bad input does not stop the module", failed.Count == 0 && a.Running, string.Join(",", failed.ToArray()));
            reply = commands.Execute("hello", "verify");
            check.True("commands: a missing argument (IndexOutOfRange) is a real bug and stops the module", failed.Count == 1 && failed[0] == "a", reply + " failed=" + string.Join(",", failed.ToArray()));

            commands.Register(b, "hello", "hello (b)", args => "b");
            check.True("commands: a stopped module's name can be taken", commands.Execute("hello", "verify") == "b", commands.Execute("hello", "verify"));

            bool refused = false;
            try { commands.Register(null, "anon", "", args => ""); }
            catch (ArgumentNullException) { refused = true; }
            check.True("commands: SDK registration needs an owner", refused, "");
        }

        private static void ManifestChecks(Checker check)
        {
            TestModule m = Module("m", Capabilities.Developer);
            string[] copy = m.Manifest.Capabilities;
            copy[0] = Capabilities.EngineInternal;
            check.True("manifest: a reader cannot rewrite a module's capabilities in place", !m.Manifest.Has(Capabilities.EngineInternal) && m.Manifest.Has(Capabilities.Developer), "");
            string[] requires = m.Manifest.Requires;
            check.True("manifest: missing arrays read as empty", requires != null && requires.Length == 0, "");
        }
    }
}

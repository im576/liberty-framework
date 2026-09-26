using System;
using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK INatives: raw native calls by name through ScriptHookDotNet, for engine-level modules only
    // (Capabilities.EngineInternal). Gameplay mods should ask for a service instead; unknown natives are the most
    // common source of crashes. Every call is counted per module (the "natives" console command lists them).
    public sealed class NativeService : INatives
    {
        private readonly LibertyEngine engine;
        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();

        internal NativeService(LibertyEngine engine) { this.engine = engine; }

        public int CallInt(LibertyModule owner, string name, params object[] args)
        {
            Parameter[] parameters;
            NativeOut[] outs;
            Pointer[] pointers;
            Prepare(owner, name, args, out parameters, out outs, out pointers);
            int result = Function.Call<int>(name, parameters);
            Finish(outs, pointers);
            return result;
        }

        public float CallFloat(LibertyModule owner, string name, params object[] args)
        {
            Parameter[] parameters;
            NativeOut[] outs;
            Pointer[] pointers;
            Prepare(owner, name, args, out parameters, out outs, out pointers);
            float result = Function.Call<float>(name, parameters);
            Finish(outs, pointers);
            return result;
        }

        public bool CallBool(LibertyModule owner, string name, params object[] args)
        {
            Parameter[] parameters;
            NativeOut[] outs;
            Pointer[] pointers;
            Prepare(owner, name, args, out parameters, out outs, out pointers);
            bool result = Function.Call<bool>(name, parameters);
            Finish(outs, pointers);
            return result;
        }

        public void Call(LibertyModule owner, string name, params object[] args)
        {
            Parameter[] parameters;
            NativeOut[] outs;
            Pointer[] pointers;
            Prepare(owner, name, args, out parameters, out outs, out pointers);
            Function.Call(name, parameters);
            Finish(outs, pointers);
        }

        private void Prepare(LibertyModule owner, string name, object[] args, out Parameter[] parameters, out NativeOut[] outs, out Pointer[] pointers)
        {
            engine.RequireCapability(owner, Capabilities.EngineInternal);
            string key = owner.Id + ":" + name;
            int n;
            counts.TryGetValue(key, out n);
            counts[key] = n + 1;
            args = args ?? new object[0];
            parameters = new Parameter[args.Length];
            outs = new NativeOut[args.Length];
            pointers = new Pointer[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                object a = args[i];
                NativeOut slot = a as NativeOut;
                if (slot != null)
                {
                    outs[i] = slot;
                    pointers[i] = slot.IsFloat ? (Pointer)typeof(float) : (Pointer)typeof(int);
                    parameters[i] = pointers[i];
                }
                else if (a is int) { parameters[i] = (int)a; }
                else if (a is float) { parameters[i] = (float)a; }
                else if (a is double) { parameters[i] = (float)(double)a; }
                else if (a is bool) { parameters[i] = (bool)a; }
                else if (a is string) { parameters[i] = (string)a; }
                else if (a is uint) { parameters[i] = unchecked((int)(uint)a); }
                else if (a is PedRef) { parameters[i] = ((PedRef)a).Handle; }
                else if (a is VehicleRef) { parameters[i] = ((VehicleRef)a).Handle; }
                else if (a is PropRef) { parameters[i] = ((PropRef)a).Handle; }
                else if (a is ModelRef) { parameters[i] = ((ModelRef)a).Hash; }
                else { throw new ArgumentException("native " + name + " argument " + i + " has unsupported type " + (a == null ? "null" : a.GetType().Name)); }
            }
        }

        private static void Finish(NativeOut[] outs, Pointer[] pointers)
        {
            for (int i = 0; i < outs.Length; i++)
            {
                if (outs[i] == null) { continue; }
                outs[i].Value = outs[i].IsFloat ? (object)(float)pointers[i] : (object)(int)pointers[i];
            }
        }

        internal string Report()
        {
            List<string> rows = new List<string>();
            foreach (KeyValuePair<string, int> pair in counts) { rows.Add(pair.Key + "=" + pair.Value); }
            rows.Sort(StringComparer.Ordinal);
            return rows.Count == 0 ? "no raw native calls" : string.Join(" ", rows.ToArray());
        }
    }
}

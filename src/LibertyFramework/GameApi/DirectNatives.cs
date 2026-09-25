using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // T-026 step 2: read-only script natives called through their own CE handlers on the script's thread, instead
    // of SHDN's cross-thread transport (32-180 us per call in the owner's logs; direct 0.16-0.2 us).
    //
    // Why it is safe:
    // - EngineThreadProbe measured the engine frame counter never advancing during an SHDN tick (5,000+ ticks):
    //   the game thread is parked while our scripts run, so these reads are serialized with the game.
    // - Every handler below was scanned (with its callees, 3 levels) for the current-script-thread global
    //   0x1BB54DC / getters 0x86D060, 0x94B860 and for fs: access; all are clean. Natives whose callees could not be
    //   analysed (encrypted on disk: GET_GAME_CAM, GET_PLAYER_CHAR) are not listed.
    // - Each native is verified at startup against SHDN's result on the player (Verify) and used only if it matched.
    //
    // CE native ABI: handler(ctx) cdecl; ctx+0 = pointer to the return slot, ctx+4 = argument count, ctx+8 = pointer
    // to the argument array (4 bytes each; out-parameters are pointers).
    internal static class DirectNatives
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Handler(IntPtr context);

        internal const string IsPlayerPlaying = "IS_PLAYER_PLAYING";
        internal const string GetPlayerId = "GET_PLAYER_ID";
        internal const string IsCharDucking = "IS_CHAR_DUCKING";
        internal const string IsPedInCover = "IS_PED_IN_COVER";
        internal const string IsCharInAnyCar = "IS_CHAR_IN_ANY_CAR";
        internal const string IsCharInAir = "IS_CHAR_IN_AIR";
        internal const string GetCharSpeed = "GET_CHAR_SPEED";
        internal const string IsPauseMenuActive = "IS_PAUSE_MENU_ACTIVE";
        internal const string IsScreenFadedOut = "IS_SCREEN_FADED_OUT";
        internal const string IsPlayerControlOn = "IS_PLAYER_CONTROL_ON";
        internal const string GetCamFov = "GET_CAM_FOV";
        internal const string GetCharHealth = "GET_CHAR_HEALTH";
        internal const string IsCharDead = "IS_CHAR_DEAD";
        internal const string DoesCharExist = "DOES_CHAR_EXIST";
        internal const string GetCurrentCharWeapon = "GET_CURRENT_CHAR_WEAPON";
        internal const string GetAmmoInClip = "GET_AMMO_IN_CLIP";
        internal const string GetMaxAmmoInClip = "GET_MAX_AMMO_IN_CLIP";
        internal const string HasCharBeenDamagedByChar = "HAS_CHAR_BEEN_DAMAGED_BY_CHAR";

        // CE hashes (FusionFix natives.ixx; registration checked by tools/verify).
        internal static readonly Dictionary<string, uint> Hashes = new Dictionary<string, uint>
        {
            { IsPlayerPlaying, 0x08274BA4 }, { GetPlayerId, 0x62E319C6 }, { IsCharDucking, 0x495D6021 },
            { IsPedInCover, 0x5C825D83 }, { IsCharInAnyCar, 0x71184DA3 }, { IsCharInAir, 0x23C15141 },
            { GetCharSpeed, 0x3E156AFC }, { IsPauseMenuActive, 0x6C4568A7 }, { IsScreenFadedOut, 0x59EE3A11 },
            { IsPlayerControlOn, 0x30CD2F1F }, { GetCamFov, 0x7BF4652D }, { GetCharHealth, 0x4B6C2256 },
            { IsCharDead, 0x6A6B4F18 }, { DoesCharExist, 0x46531797 }, { GetCurrentCharWeapon, 0x5AB8289F },
            { GetAmmoInClip, 0x612C748F }, { GetMaxAmmoInClip, 0x01794A3C }, { HasCharBeenDamagedByChar, 0x1DD624A0 },
        };

        private sealed class Entry
        {
            internal Handler Function;
            internal bool Verified;
        }

        private static readonly Dictionary<string, Entry> Entries = new Dictionary<string, Entry>();
        private static readonly object Gate = new object();
        [ThreadStatic] private static IntPtr block; // ctx 0..11, args 16..79, return slot 96, out slots 112..175

        // Maps each listed native to its handler. Handlers stay unused until Verify accepts them.
        internal static void Initialize(CodeScanner scanner)
        {
            lock (Gate)
            {
                foreach (KeyValuePair<string, uint> pair in Hashes)
                {
                    uint handler = scanner.FindNative(pair.Value);
                    if (handler == 0) { RuntimeLog.Error("direct_native_missing " + pair.Key); continue; }
                    Entry entry = new Entry();
                    entry.Function = (Handler)Marshal.GetDelegateForFunctionPointer(new IntPtr((int)handler), typeof(Handler));
                    Entries[pair.Key] = entry;
                }
            }
        }

        internal static bool Ready(string name)
        {
            lock (Gate) { Entry entry; return Entries.TryGetValue(name, out entry) && entry.Verified; }
        }

        // Accepts a native for direct use when its direct result equals SHDN's (evaluated back to back in one tick).
        internal static bool Verify(string name, Func<long> direct, Func<long> viaShdn)
        {
            Entry entry;
            lock (Gate) { if (!Entries.TryGetValue(name, out entry)) { return false; } }
            long a, b;
            try { a = direct(); b = viaShdn(); }
            catch (Exception error) { RuntimeLog.Error("direct_native_verify_failed " + name + " error=" + error.Message); return false; }
            bool ok = a == b;
            lock (Gate) { entry.Verified = ok; }
            if (!ok) { RuntimeLog.Error("direct_native_rejected " + name + " direct=" + a + " shdn=" + b); }
            return ok;
        }

        private static IntPtr Block
        {
            get
            {
                if (block == IntPtr.Zero) { block = Marshal.AllocHGlobal(192); }
                return block;
            }
        }

        // Address of out-slot 0..15 (for pointer arguments), and its value after a call.
        internal static int Out(int slot) { return Block.ToInt32() + 112 + slot * 4; }
        internal static int OutInt(int slot) { return Marshal.ReadInt32(new IntPtr(Out(slot))); }
        internal static float OutFloat(int slot) { return BitConverter.ToSingle(BitConverter.GetBytes(OutInt(slot)), 0); }

        // Calls the native with up to 4 int-sized arguments; returns the return slot as an int (bools are 0/1).
        internal static int Call(string name, int argumentCount, int a0, int a1, int a2, int a3)
        {
            Entry entry;
            lock (Gate) { entry = Entries[name]; }
            IntPtr ctx = Block;
            int baseAddress = ctx.ToInt32();
            Marshal.WriteInt32(ctx, 0, baseAddress + 96);
            Marshal.WriteInt32(ctx, 4, argumentCount);
            Marshal.WriteInt32(ctx, 8, baseAddress + 16);
            Marshal.WriteInt32(ctx, 16, a0);
            Marshal.WriteInt32(ctx, 20, a1);
            Marshal.WriteInt32(ctx, 24, a2);
            Marshal.WriteInt32(ctx, 28, a3);
            Marshal.WriteInt32(ctx, 96, 0);
            entry.Function(ctx);
            return Marshal.ReadInt32(ctx, 96);
        }

        internal static int Call(string name) { return Call(name, 0, 0, 0, 0, 0); }
        internal static int Call(string name, int a0) { return Call(name, 1, a0, 0, 0, 0); }
        internal static int Call(string name, int a0, int a1) { return Call(name, 2, a0, a1, 0, 0); }
        internal static int Call(string name, int a0, int a1, int a2) { return Call(name, 3, a0, a1, a2, 0); }

        internal static string Summary()
        {
            int verified = 0, total = 0;
            lock (Gate) { foreach (Entry entry in Entries.Values) { total++; if (entry.Verified) { verified++; } } }
            return "direct_natives verified=" + verified + "/" + total;
        }
    }
}

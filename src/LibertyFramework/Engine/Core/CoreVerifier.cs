using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Core
{
    // Accepts each core native only when its direct result equals ScriptHookDotNet's for the same call, back to back
    // in one tick (the game is parked). Floats are compared bit for bit. Rejected natives stay off in the core.
    internal static class CoreVerifier
    {
        private enum Out { None, Int, Float }

        private sealed class Check
        {
            internal int Id;
            internal string Name;
            internal Func<int[]> Args;
            internal Out[] Outs;
            internal bool UseReturn;
        }

        internal static int VerifyAll(CoreBridge core, Player player, Ped ped)
        {
            int pedHandle = ped.GetHashCode();
            int index = Function.Call<int>("GET_PLAYER_ID");
            Pointer weaponOut = typeof(int);
            Function.Call("GET_CURRENT_CHAR_WEAPON", ped, weaponOut);
            int weapon = (int)weaponOut;
            List<Check> checks = new List<Check>
            {
                New(CoreAbi.DoesCharExist, "DOES_CHAR_EXIST", () => new[] { pedHandle }, true),
                New(CoreAbi.IsCharDead, "IS_CHAR_DEAD", () => new[] { pedHandle }, true),
                New(CoreAbi.GetCharHealth, "GET_CHAR_HEALTH", () => new[] { pedHandle }, false, Out.Int),
                New(CoreAbi.GetCharArmour, "GET_CHAR_ARMOUR", () => new[] { pedHandle }, false, Out.Int),
                New(CoreAbi.GetCharCoordinates, "GET_CHAR_COORDINATES", () => new[] { pedHandle }, false, Out.Float, Out.Float, Out.Float),
                New(CoreAbi.GetCharHeading, "GET_CHAR_HEADING", () => new[] { pedHandle }, false, Out.Float),
                New(CoreAbi.GetCharModel, "GET_CHAR_MODEL", () => new[] { pedHandle }, false, Out.Int),
                New(CoreAbi.IsCharInAnyCar, "IS_CHAR_IN_ANY_CAR", () => new[] { pedHandle }, true),
                New(CoreAbi.GetCurrentCharWeapon, "GET_CURRENT_CHAR_WEAPON", () => new[] { pedHandle }, false, Out.Int),
                New(CoreAbi.GetAmmoInClip, "GET_AMMO_IN_CLIP", () => new[] { pedHandle, weapon }, false, Out.Int),
                New(CoreAbi.GetCharLastDamageBone, "GET_CHAR_LAST_DAMAGE_BONE", () => new[] { pedHandle }, false, Out.Int),
                New(CoreAbi.HasCharBeenDamagedByChar, "HAS_CHAR_BEEN_DAMAGED_BY_CHAR", () => new[] { pedHandle, pedHandle, 0 }, true),
                New(CoreAbi.GetPlayerId, "GET_PLAYER_ID", () => new int[0], true),
                New(CoreAbi.IsPlayerPlaying, "IS_PLAYER_PLAYING", () => new[] { index }, true),
                New(CoreAbi.IsPlayerControlOn, "IS_PLAYER_CONTROL_ON", () => new[] { index }, true),
                New(CoreAbi.IsPauseMenuActive, "IS_PAUSE_MENU_ACTIVE", () => new int[0], true),
                New(CoreAbi.IsScreenFadedOut, "IS_SCREEN_FADED_OUT", () => new int[0], true),
                New(CoreAbi.GetHoursOfDay, "GET_HOURS_OF_DAY", () => new int[0], true),
                New(CoreAbi.GetMinutesOfDay, "GET_MINUTES_OF_DAY", () => new int[0], true),
                New(CoreAbi.GetCurrentWeather, "GET_CURRENT_WEATHER", () => new int[0], false, Out.Int),
            };
            int accepted = 0;
            List<string> rejected = new List<string>();
            foreach (Check check in checks)
            {
                bool ok = false;
                try { ok = Compare(core, check); }
                catch (Exception error) { RuntimeLog.Error("engine_verify_failed " + check.Name + " error=" + error.Message); }
                core.SetVerified(check.Id, ok);
                if (ok) { accepted++; } else { rejected.Add(check.Name); }
            }
            // GET_CAR_CHAR_IS_USING needs a vehicle to compare; it follows IS_CHAR_IN_ANY_CAR and is checked when the
            // player is in one (VerifyVehicle). GET_GAME_TIMER moves between two calls; checked within 50 ms.
            VerifyGameTimer(core);
            if (core.IsVerified(CoreAbi.GetGameTimer)) { accepted++; } else { rejected.Add("GET_GAME_TIMER"); }
            RuntimeLog.Info("engine_verify accepted=" + accepted + "/" + (checks.Count + 1) + (rejected.Count > 0 ? " rejected=" + string.Join(",", rejected.ToArray()) : ""));
            return accepted;
        }

        internal static void VerifyVehicle(CoreBridge core, Ped ped)
        {
            if (core.IsVerified(CoreAbi.GetCarCharIsUsing)) { return; }
            Check check = New(CoreAbi.GetCarCharIsUsing, "GET_CAR_CHAR_IS_USING", () => new[] { ped.GetHashCode() }, false, Out.Int);
            bool ok = false;
            try { ok = Compare(core, check); } catch (Exception error) { RuntimeLog.Error("engine_verify_failed " + check.Name + " error=" + error.Message); }
            core.SetVerified(check.Id, ok);
            RuntimeLog.Info("engine_verify GET_CAR_CHAR_IS_USING ok=" + ok);
        }

        private static void VerifyGameTimer(CoreBridge core)
        {
            int[] outs = new int[4];
            if (core.Call(CoreAbi.GetGameTimer, new int[0], outs) == int.MinValue) { core.SetVerified(CoreAbi.GetGameTimer, false); return; }
            Pointer value = typeof(int);
            Function.Call("GET_GAME_TIMER", value);
            long difference = Math.Abs((long)(uint)(int)value - (long)(uint)outs[0]);
            bool ok = difference <= 50;
            core.SetVerified(CoreAbi.GetGameTimer, ok);
            if (!ok) { RuntimeLog.Error("engine_verify GET_GAME_TIMER direct=" + (uint)outs[0] + " shdn=" + (uint)(int)value); }
        }

        private static Check New(int id, string name, Func<int[]> args, bool useReturn, params Out[] outs)
        {
            Check check = new Check();
            check.Id = id; check.Name = name; check.Args = args; check.UseReturn = useReturn; check.Outs = outs;
            return check;
        }

        private static bool Compare(CoreBridge core, Check check)
        {
            int[] args = check.Args();
            int[] outs = new int[4];
            int direct = core.Call(check.Id, args, outs);
            if (direct == int.MinValue && !check.UseReturn) { return false; }
            List<object> parameters = new List<object>();
            foreach (int a in args) { parameters.Add(a); }
            Pointer[] pointers = new Pointer[check.Outs.Length];
            for (int i = 0; i < check.Outs.Length; i++)
            {
                pointers[i] = check.Outs[i] == Out.Float ? (Pointer)typeof(float) : (Pointer)typeof(int);
                parameters.Add(pointers[i]);
            }
            Parameter[] converted = new Parameter[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                object value = parameters[i];
                converted[i] = value is Pointer ? (Parameter)(Pointer)value : (Parameter)(int)value;
            }
            if (check.UseReturn)
            {
                int viaShdn = Function.Call<int>(check.Name, converted);
                bool same = IsBoolean(check.Name) ? (direct != 0) == (viaShdn != 0) : direct == viaShdn;
                if (!same) { RuntimeLog.Error("engine_verify_mismatch " + check.Name + " direct=" + direct + " shdn=" + viaShdn); }
                return same;
            }
            Function.Call(check.Name, converted);
            for (int i = 0; i < check.Outs.Length; i++)
            {
                int shdn = check.Outs[i] == Out.Float ? BitConverter.ToInt32(BitConverter.GetBytes((float)pointers[i]), 0) : (int)pointers[i];
                if (shdn != outs[i])
                {
                    RuntimeLog.Error("engine_verify_mismatch " + check.Name + " out" + i + " direct=" + outs[i] + " shdn=" + shdn);
                    return false;
                }
            }
            return true;
        }

        // Boolean natives: only truthiness is compared (handlers may leave upper bytes of the slot unspecified).
        private static bool IsBoolean(string name) { return name.StartsWith("IS_") || name.StartsWith("DOES_") || name.StartsWith("HAS_"); }
    }
}

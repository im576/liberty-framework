using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IWeapons: plain weapon natives on a ped handle (NATIVES.md). The gunplay and arsenal modules layer their
    // own rules on top; this service changes nothing by itself.
    public sealed class WeaponService : IWeapons
    {
        // GET_CHAR_WEAPON_IN_SLOT covers the ten inventory slots (unarmed .. special).
        private const int SlotCount = 10;

        public void Give(PedRef ped, int weapon, int ammo) { Function.Call("GIVE_WEAPON_TO_CHAR", ped.Handle, weapon, ammo, false); }

        public void Remove(PedRef ped, int weapon) { Function.Call("REMOVE_WEAPON_FROM_CHAR", ped.Handle, weapon); }

        public void RemoveAll(PedRef ped) { Function.Call("REMOVE_ALL_CHAR_WEAPONS", ped.Handle); }

        public void Select(PedRef ped, int weapon) { Function.Call("SET_CURRENT_CHAR_WEAPON", ped.Handle, weapon, true); }

        public int Current(PedRef ped) { return NativeCall.OutInt("GET_CURRENT_CHAR_WEAPON", ped.Handle); }

        public bool Has(PedRef ped, int weapon) { return Function.Call<bool>("HAS_CHAR_GOT_WEAPON", ped.Handle, weapon); }

        public int GetAmmo(PedRef ped, int weapon)
        {
            Pointer ammo = typeof(int);
            Function.Call("GET_AMMO_IN_CHAR_WEAPON", ped.Handle, weapon, ammo);
            return (int)ammo;
        }

        public void SetAmmo(PedRef ped, int weapon, int ammo) { Function.Call("SET_CHAR_AMMO", ped.Handle, weapon, ammo); }

        public int GetAmmoInClip(PedRef ped, int weapon)
        {
            Pointer ammo = typeof(int);
            Function.Call("GET_AMMO_IN_CLIP", ped.Handle, weapon, ammo);
            return (int)ammo;
        }

        public IList<WeaponSlotState> Inventory(PedRef ped)
        {
            List<WeaponSlotState> result = new List<WeaponSlotState>();
            for (int slot = 0; slot < SlotCount; slot++)
            {
                Pointer weapon = typeof(int), ammo = typeof(int), unused = typeof(int);
                Function.Call("GET_CHAR_WEAPON_IN_SLOT", ped.Handle, slot, weapon, ammo, unused);
                if ((int)weapon <= 0) { continue; }
                WeaponSlotState state = new WeaponSlotState();
                state.Weapon = (int)weapon; state.Ammo = (int)ammo; state.Slot = slot;
                result.Add(state);
            }
            return result;
        }

        public ModelRef ModelOf(int weapon)
        {
            Pointer model = typeof(int);
            Function.Call("GET_WEAPONTYPE_MODEL", weapon, model);
            return ModelRef.FromHash((int)model);
        }

        public int SlotOf(int weapon)
        {
            Pointer slot = typeof(int);
            Function.Call("GET_WEAPONTYPE_SLOT", weapon, slot);
            return (int)slot;
        }
    }
}

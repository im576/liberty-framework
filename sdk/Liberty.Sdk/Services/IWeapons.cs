using System.Collections.Generic;

namespace Liberty.Sdk
{
    // Weapons on peds (ids: 1 bat .. 20 minigun in GTA IV, 21+ episodic, 58-60 Liberty test weapons).
    public interface IWeapons
    {
        void Give(PedRef ped, int weapon, int ammo);
        void Remove(PedRef ped, int weapon);
        void RemoveAll(PedRef ped);
        void Select(PedRef ped, int weapon);
        int Current(PedRef ped);
        bool Has(PedRef ped, int weapon);
        int GetAmmo(PedRef ped, int weapon);
        void SetAmmo(PedRef ped, int weapon, int ammo);
        int GetAmmoInClip(PedRef ped, int weapon);
        IList<WeaponSlotState> Inventory(PedRef ped);
        // The weapon's world model as the game has it loaded (GET_WEAPONTYPE_MODEL); None when unknown.
        ModelRef ModelOf(int weapon);
        // Inventory slot of a weapon type (0 unarmed, 1 melee, 2 handgun, ...), or -1.
        int SlotOf(int weapon);
    }
}
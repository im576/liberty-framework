using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GTA;
using LibertyFramework.Arsenal.Contracts;
using LibertyFramework.Arsenal.Holsters.Logic;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.DevTools;
using LibertyFramework.DevTools.Menu;
using LibertyFramework.GameApi;

namespace LibertyFramework.Arsenal.Holsters
{
    public sealed class HolsterController : Script
    {
        private readonly Dictionary<BodySlot, GTA.Object> props = new Dictionary<BodySlot, GTA.Object>();
        private readonly Dictionary<BodySlot, int> shownIds = new Dictionary<BodySlot, int>();
        private readonly Dictionary<BodySlot, int> shownModels = new Dictionary<BodySlot, int>();
        private readonly Dictionary<string, string> modelNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly string journalPath = Path.Combine(LibertyPaths.StateDirectory, "holsters_props.json");
        private bool recovered;
        private bool removing;
        private int removingRevision;
        private HolsterConfig config;
        private bool disabled;
        private BodySlot selectedSlot = BodySlot.SidearmPrimary;
        private string configHash;

        public HolsterController()
        {
            Interval = 0;
            LoadConfig();
            ArsenalRegistry.WeaponsRemoving += OnWeaponsRemoving;
            DevToolsPages.Register("Holsters", MenuItems);
            Tick += OnTick;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
        }

        private void LoadConfig()
        {
            try
            {
                byte[] bytes = JsonStore.ReadBytes(LibertyPaths.HolstersConfig);
                string hash = JsonStore.Hash(bytes);
                if (hash == configHash) { return; }
                configHash = hash;
                HolsterConfig candidate = JsonStore.Parse<HolsterConfig>(bytes);
                candidate.Validate();
                foreach (HolsterPlacement placement in candidate.Placements)
                {
                    Bone bone;
                    if (!Enum.TryParse<Bone>(placement.Bone, out bone) || !Enum.IsDefined(typeof(Bone), bone))
                    { throw new InvalidDataException("holsters unknown ScriptHookDotNet bone " + placement.Bone); }
                }
                if (props.Count > 0) { Clear(); }
                config = candidate;
                modelNames.Clear();
                RuntimeLog.Info("holsters_config_loaded weapons=" + config.Weapons.Count);
            }
            catch (Exception error) { RuntimeLog.Error("holsters_config_rejected error=" + error); }
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled) { return; }
            try
            {
                if (!recovered) { RecoverPreviousProps(); recovered = true; }
                LoadConfig();
                if (config == null || !config.Enabled) { Clear(); return; }
                Player player = Player;
                Ped ped = player == null ? null : player.Character;
                if (ped == null) { Clear(); return; }
                bool inVehicle = ped.isInVehicle();
                bool onBike = inVehicle && ped.CurrentVehicle != null && ped.CurrentVehicle.Model.isBike;
                bool visible = HolsterRules.Visible(false, !ped.isDead,
                    Natives.IsPlayerPlaying(player) && Natives.IsPlayerControlOn(player),
                    Natives.IsScreenFadedOut(), inVehicle, onBike, config.ShowOnBikes);
                if (!visible) { Clear(); return; }
                ICarriedWeaponsSource source = ArsenalRegistry.CarriedWeapons;
                if (removing && source != null && source.Revision <= removingRevision) { Clear(); return; }
                removing = false;
                int held = (int)ped.Weapons.CurrentType;
                IList<CarriedWeapon> carried = source != null ? source.Carried : ReadInventory(ped, held);
                if (carried == null) { Clear(); return; }
                HashSet<BodySlot> wanted = new HashSet<BodySlot>();
                foreach (CarriedWeapon weapon in carried)
                {
                    if (weapon == null || weapon.Slot == BodySlot.None || weapon.InHand || weapon.WeaponId == held) { continue; }
                    if (!wanted.Add(weapon.Slot)) { continue; }
                    Show(ped, weapon);
                }
                foreach (BodySlot slot in new List<BodySlot>(props.Keys)) { if (!wanted.Contains(slot)) { Remove(slot); } }
            }
            catch (Exception error)
            {
                RuntimeLog.Error("feature_disabled holsters error=" + error);
                try { Clear(); } catch (Exception restoreError) { RuntimeLog.Error("holsters_cleanup_failed error=" + restoreError); }
                disabled = true;
            }
        }

        private IList<CarriedWeapon> ReadInventory(Ped ped, int held)
        {
            List<CarriedWeapon> result = new List<CarriedWeapon>();
            bool firstLong = true;
            foreach (HolsterWeapon weapon in config.Weapons)
            {
                GTA.value.Weapon instance = ped.Weapons.FromType((Weapon)weapon.WeaponId);
                if (instance == null || !instance.isPresent) { continue; }
                BodySlot slot = HolsterRules.SlotFor(weapon.Category, firstLong);
                if (slot == BodySlot.LongGun1) { firstLong = false; }
                result.Add(new CarriedWeapon(weapon.WeaponId, weapon.Category, slot, weapon.WeaponId == held));
            }
            if (Game.CurrentEpisode != GameEpisode.GTAIV)
            {
                for (int weaponId = 21; weaponId <= 41; weaponId++)
                {
                    GTA.value.Weapon instance = ped.Weapons.FromType((Weapon)weaponId);
                    if (instance == null || !instance.isPresent) { continue; }
                    WeaponCategory category = CategoryFor(instance.Slot);
                    BodySlot slot = HolsterRules.SlotFor(category, firstLong);
                    if (slot == BodySlot.LongGun1) { firstLong = false; }
                    result.Add(new CarriedWeapon(weaponId, category, slot, weaponId == held));
                }
            }
            return result;
        }

        private static WeaponCategory CategoryFor(WeaponSlot slot)
        {
            switch (slot)
            {
                case WeaponSlot.Melee: return WeaponCategory.Melee;
                case WeaponSlot.Handgun: return WeaponCategory.Handgun;
                case WeaponSlot.Shotgun: return WeaponCategory.Shotgun;
                case WeaponSlot.SMG: return WeaponCategory.SMG;
                case WeaponSlot.Rifle: return WeaponCategory.Rifle;
                case WeaponSlot.Sniper: return WeaponCategory.Sniper;
                case WeaponSlot.Heavy: return WeaponCategory.Heavy;
                case WeaponSlot.Thrown: return WeaponCategory.Thrown;
                default: return WeaponCategory.Other;
            }
        }

        private void Show(Ped ped, CarriedWeapon weapon)
        {
            HolsterWeapon entry = config.FindWeapon(weapon.WeaponId);
            if (entry == null && (weapon.WeaponId < 21 || weapon.WeaponId > 41 || Game.CurrentEpisode == GameEpisode.GTAIV))
            { Remove(weapon.Slot); return; }
            string weaponType = entry != null ? entry.WeaponInfoType : "EPISODIC_" + (weapon.WeaponId - 20);
            string xmlPath = entry != null ? WeaponInfoXml.ActivePath(LibertyPaths.GameDirectory) :
                Path.Combine(LibertyPaths.GameDirectory, Path.Combine(Game.CurrentEpisode == GameEpisode.TLAD ? "TLAD" : "TBoGT",
                    Path.Combine("common", Path.Combine("data", "WeaponInfo.xml"))));
            string modelKey = xmlPath + "|" + weaponType;
            string modelName;
            if (!modelNames.TryGetValue(modelKey, out modelName))
            {
                modelName = WeaponInfoXml.ModelFor(xmlPath, weaponType);
                modelNames[modelKey] = modelName;
                if (string.IsNullOrEmpty(modelName))
                { RuntimeLog.Error("holster_no_weaponinfo_model type=" + weaponType + " id=" + weapon.WeaponId); }
            }
            if (string.IsNullOrEmpty(modelName)) { Remove(weapon.Slot); return; }
            int shown;
            if (shownIds.TryGetValue(weapon.Slot, out shown) && shown == weapon.WeaponId && props.ContainsKey(weapon.Slot)) { return; }
            Remove(weapon.Slot);
            Model model = new Model(modelName);
            if (!model.isValid) { RuntimeLog.Error("holster_model_invalid " + modelName); return; }
            HolsterNatives.RequestModel(model);
            if (!HolsterNatives.HasModelLoaded(model)) { return; }
            HolsterPlacement placement = config.FindPlacement(weapon.Slot, weapon.Category, modelName);
            if (placement == null) { return; }
            GTA.Object prop = World.CreateObject(model, ped.Position);
            if (prop == null) { return; }
            try
            {
                prop.Collision = false;
                prop.AttachToPed(ped, (Bone)Enum.Parse(typeof(Bone), placement.Bone),
                    new Vector3(placement.Position[0], placement.Position[1], placement.Position[2]),
                    new Vector3(placement.Rotation[0], placement.Rotation[1], placement.Rotation[2]));
                props[weapon.Slot] = prop;
                shownIds[weapon.Slot] = weapon.WeaponId;
                shownModels[weapon.Slot] = model.Hash;
                SaveJournal();
            }
            catch (Exception error) { prop.Delete(); RuntimeLog.Error("holster_attach_failed error=" + error); throw; }
        }

        private void Remove(BodySlot slot)
        {
            GTA.Object prop;
            if (props.TryGetValue(slot, out prop))
            {
                props.Remove(slot);
                shownIds.Remove(slot);
                shownModels.Remove(slot);
                if (prop != null && prop.Exists()) { prop.Delete(); }
                SaveJournal();
            }
        }

        private void Clear()
        {
            foreach (BodySlot slot in new List<BodySlot>(props.Keys)) { Remove(slot); }
        }

        private void OnWeaponsRemoving(string reason)
        {
            try
            {
                ICarriedWeaponsSource source = ArsenalRegistry.CarriedWeapons;
                removing = true;
                removingRevision = source != null ? source.Revision : -1;
                Clear();
                RuntimeLog.Info("holsters_removed reason=" + reason);
            }
            catch (Exception error) { RuntimeLog.Error("holsters_remove_failed error=" + error); disabled = true; }
        }

        private void OnDomainUnload(object sender, EventArgs args)
        {
            // SHDN natives are unavailable during DomainUnload. A same-process ReloadScripts
            // consumes this journal on its first tick and deletes any surviving matching props.
            ArsenalRegistry.WeaponsRemoving -= OnWeaponsRemoving;
        }

        private void SaveJournal()
        {
            if (props.Count == 0)
            {
                if (File.Exists(journalPath)) { File.Delete(journalPath); }
                return;
            }
            using (Process process = Process.GetCurrentProcess())
            {
                HolsterPropJournal journal = new HolsterPropJournal();
                journal.ProcessId = process.Id;
                journal.ProcessStartUtcTicks = process.StartTime.ToUniversalTime().Ticks;
                journal.Props = new List<HolsterPropRecord>();
                foreach (KeyValuePair<BodySlot, GTA.Object> pair in props)
                {
                    HolsterPropRecord record = new HolsterPropRecord();
                    record.Handle = pair.Value.GetHashCode(); // SHDN HandleObject.GetHashCode returns the native handle.
                    record.ModelHash = shownModels[pair.Key];
                    journal.Props.Add(record);
                }
                JsonStore.Save(journalPath, journal);
            }
        }

        private void RecoverPreviousProps()
        {
            if (!File.Exists(journalPath)) { return; }
            HolsterPropJournal journal = JsonStore.Load<HolsterPropJournal>(journalPath);
            using (Process process = Process.GetCurrentProcess())
            {
                if (journal.ProcessId == process.Id && journal.ProcessStartUtcTicks == process.StartTime.ToUniversalTime().Ticks)
                {
                    if (journal.Props == null) { throw new InvalidDataException("holster journal missing props"); }
                    foreach (HolsterPropRecord prop in journal.Props)
                    {
                        if (prop != null && HolsterNatives.ObjectExists(prop.Handle) && HolsterNatives.ObjectModel(prop.Handle) == prop.ModelHash)
                        {
                            HolsterNatives.DeleteObject(prop.Handle);
                            RuntimeLog.Info("holster_orphan_removed handle=" + prop.Handle);
                        }
                    }
                }
            }
            File.Delete(journalPath);
        }

        private List<MenuItem> MenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();
            if (config == null) { items.Add(MenuItem.Info(() => "holsters.json unavailable; see log")); return items; }
            items.Add(MenuItem.Info(() => "Selected " + selectedSlot));
            items.Add(MenuItem.Action("Next slot", () => { selectedSlot = selectedSlot == BodySlot.Melee ? BodySlot.SidearmPrimary : selectedSlot + 1; return selectedSlot.ToString(); }));
            items.Add(NudgeItem("Position X", false, 0));
            items.Add(NudgeItem("Position Y", false, 1));
            items.Add(NudgeItem("Position Z", false, 2));
            items.Add(NudgeItem("Rotation X", true, 0));
            items.Add(NudgeItem("Rotation Y", true, 1));
            items.Add(NudgeItem("Rotation Z", true, 2));
            items.Add(MenuItem.Action("Save offsets", () => { JsonStore.Save(LibertyPaths.HolstersConfig, config); return "Saved holsters.json (.bak)"; }));
            return items;
        }

        private MenuItem NudgeItem(string label, bool rotation, int axis)
        {
            MenuItem item = new MenuItem();
            item.Label = () =>
            {
                HolsterPlacement placement = config.FindPlacement(selectedSlot, WeaponCategory.Other, null);
                float[] vector = rotation ? placement.Rotation : placement.Position;
                return label + " " + vector[axis].ToString("0.000");
            };
            item.Adjust = direction =>
            {
                HolsterPlacement placement = config.FindPlacement(selectedSlot, WeaponCategory.Other, null);
                float[] vector = rotation ? placement.Rotation : placement.Position;
                vector[axis] += direction * (rotation ? config.NudgeRotationDegrees : config.NudgePositionMeters);
                Remove(selectedSlot);
                return label + " " + vector[axis].ToString("0.000");
            };
            return item;
        }
    }
}

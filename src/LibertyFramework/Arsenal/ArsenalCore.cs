using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Keys = System.Windows.Forms.Keys;
using GTA;
using GTA.Native;
using LibertyFramework.Arsenal.Contracts;
using LibertyFramework.Arsenal.Logic;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Input;
using LibertyFramework.Core.Logging;
using LibertyFramework.DevTools;
using LibertyFramework.DevTools.Menu;
using LibertyFramework.Weapons.Logic;

namespace LibertyFramework.Arsenal
{
    // All game API access stays on Script ticks, including DevTools actions.
    public sealed class ArsenalCore : Script, ICarriedWeaponsSource
    {
        internal static bool StorageOpen { get; private set; }
        private ArsenalConfig config;
        private WeaponCatalog weaponCatalog;
        private ArsenalState state;
        private string episode;
        private string statePath;
        private readonly List<WeaponRecord> carried = new List<WeaponRecord>();
        private readonly List<WeaponRecord> pendingReplacements = new List<WeaponRecord>();
        private readonly List<CarriedWeapon> presentation = new List<CarriedWeapon>();
        private readonly Dictionary<int, long> lastUsed = new Dictionary<int, long>();
        private readonly Dictionary<string, StorageBin> temporaryTrunks = new Dictionary<string, StorageBin>();
        private readonly Dictionary<string, Vehicle> temporaryVehicles = new Dictionary<string, Vehicle>();
        private readonly Dictionary<int, string> identifiedVehicles = new Dictionary<int, string>();
        private List<LvsOwnedVehicleEntry> lvsOwned = new List<LvsOwnedVehicleEntry>();
        private readonly string lvsPath = Path.Combine(LibertyPaths.GameDirectory, "scripts\\LibertyVehicleServicesCE.owned.ini");
        private DateTime lvsLastReadUtc = DateTime.MinValue;
        private bool disabled;
        private bool deadHandled;
        private int previousMoney = -1;
        private long moneyDecreaseAt = -1;
        private int revision;
        private Vehicle lastVehicle;
        private Vehicle openedTrunk;
        private List<SafehouseRule> discoveredSafehouses = new List<SafehouseRule>();
        private DateTime lastDiscoveryUtc = DateTime.MinValue;
        private bool discoveryDisabled;
        private bool overflowDeferredLogged;
        private readonly ControllerInput storageInput = new ControllerInput();
        private readonly GTA.Font storageFont = new GTA.Font(17.0F, FontScaling.Pixel);
        private readonly GTA.Font storageTitleFont = new GTA.Font(20.0F, FontScaling.Pixel, true, false);
        private StorageBin activeStorage;
        private Vehicle nearbyTrunk;
        private SafehouseRule nearbySafehouse;
        private List<MenuItem> storageItems = new List<MenuItem>();
        private int storageSelection;
        private string storageMessage = "";
        private bool storageControlLocked;
        private bool previousStorageKey, previousUp, previousDown, previousSelect, previousBack;
        private int lastStorageScanTicks;
        private int lastSafehouseObserveTicks;
        private int lastTemporaryPruneTicks;
        private DateTime lastSnapshotUtc = DateTime.MinValue;

        public ArsenalCore()
        {
            Interval = 30;
            storageFont.Color = Color.White;
            storageTitleFont.Color = Color.FromArgb(255, 235, 200, 90);
            Tick += OnTick;
            PerFrameDrawing += OnStorageDraw;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            DevToolsPages.Register("ARSENAL", BuildPage);
            RuntimeLog.Info("arsenal_started");
        }

        int ICarriedWeaponsSource.Revision { get { return revision; } }
        IList<CarriedWeapon> ICarriedWeaponsSource.Carried { get { return new List<CarriedWeapon>(presentation); } }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled) { if (storageControlLocked) { CloseStorageSafely(); } return; }
            try
            {
                if (config == null) { Initialize(); }
                if (config == null || Player == null || Player.Character == null) { return; }
                Ped ped = Player.Character;
                int money = Player.Money;
                if (previousMoney >= 0 && money < previousMoney) { moneyDecreaseAt = Environment.TickCount; }
                previousMoney = money;
                RefreshLvs();
                ObserveVehicle(ped);
                DiscoverSafehouses();
                int nowTicks = Environment.TickCount;
                if (lastSafehouseObserveTicks == 0 || unchecked(nowTicks - lastSafehouseObserveTicks) >= 250)
                {
                    lastSafehouseObserveTicks = nowTicks;
                    ObserveSafehouse(ped);
                }
                if (lastTemporaryPruneTicks == 0 || unchecked(nowTicks - lastTemporaryPruneTicks) >= 1000)
                {
                    lastTemporaryPruneTicks = nowTicks;
                    PruneTemporaryTrunks();
                }
                if (openedTrunk != null && activeStorage == null && !DevToolsMenu.IsOpen) { CloseTrunk(); }

                bool arrested = Function.Call<bool>("IS_PLAYER_BEING_ARRESTED");
                bool dead = Function.Call<bool>("IS_PLAYER_DEAD", Player.ID);
                if (arrested || dead)
                {
                    CloseStorage();
                    if (!deadHandled) { HandleLoss(arrested); deadHandled = true; }
                    return;
                }
                deadHandled = false;
                bool mission = Function.Call<bool>("GET_MISSION_FLAG");
                bool gated = !ArsenalPolicy.MayMoveWeapons(mission, (Function.Call<bool>("HAS_CUTSCENE_LOADED") && !Function.Call<bool>("HAS_CUTSCENE_FINISHED")) ||
                    Function.Call<bool>("IS_SCREEN_FADING") || Function.Call<bool>("IS_SCREEN_FADED_OUT"));
                Reconcile(ped, mission, gated);
                UpdateStorageInteraction(ped, gated);
            }
            catch (Exception error)
            {
                Disable(error);
            }
        }

        private void Initialize()
        {
            config = JsonStore.Load<ArsenalConfig>(LibertyPaths.ArsenalConfig);
            ArsenalConfigValidator.Validate(config);
            try
            {
                weaponCatalog = JsonStore.Load<WeaponCatalog>(LibertyPaths.WeaponCatalog);
                weaponCatalog.Validate();
            }
            catch (Exception error)
            {
                weaponCatalog = null;
                RuntimeLog.Error("arsenal_catalog_unavailable legacy_weapons_active error=" + error);
            }
            int index = Function.Call<int>("GET_CURRENT_EPISODE");
            episode = index == 0 ? "iv" : index == 1 ? "tlad" : index == 2 ? "tbogt" : "episode_" + index;
            statePath = LibertyPaths.ArsenalState(episode);
            state = ArsenalStateStore.LoadOrEmpty(statePath,
                error => RuntimeLog.Error("arsenal_state_corrupt starting_empty path=" + statePath + " error=" + error));
            WeaponIdentity.Normalize(state);
            ArsenalRegistry.CarriedWeapons = this;
            RuntimeLog.Info("arsenal_ready episode=" + episode + " state=" + statePath);
        }

        private static WeaponCategory Category(WeaponSlot slot)
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

        private static List<WeaponRecord> ReadInventory(Ped ped)
        {
            List<WeaponRecord> result = new List<WeaponRecord>();
            foreach (WeaponSlot slot in new WeaponSlot[] { WeaponSlot.Melee, WeaponSlot.Handgun, WeaponSlot.Shotgun,
                WeaponSlot.SMG, WeaponSlot.Rifle, WeaponSlot.Sniper, WeaponSlot.Heavy, WeaponSlot.Thrown })
            {
                GTA.value.Weapon weapon = ped.Weapons.inSlot(slot);
                if (weapon == null || !weapon.isPresent || weapon.Type == Weapon.Unarmed || weapon.Type == Weapon.None) { continue; }
                WeaponRecord record = new WeaponRecord(); record.WeaponId = (int)weapon.Type;
                record.Category = Category(slot); record.Ammo = weapon.Ammo;
                result.Add(record);
            }
            return result;
        }

        private void Reconcile(Ped ped, bool mission, bool gated)
        {
            List<WeaponRecord> observed = ReadInventory(ped);
            int current = (int)ped.Weapons.CurrentType;
            if (current > 0) { lastUsed[current] = Environment.TickCount; }
            foreach (WeaponRecord prior in carried)
            {
                if (Find(observed, prior.WeaponId) != null) { continue; }
                bool replaced = false;
                foreach (WeaponRecord gained in observed) { if (gained.Category == prior.Category && Find(carried, gained.WeaponId) == null) { replaced = true; break; } }
                if (prior.Owned && replaced && !gated)
                {
                    StorageBin destination = OverflowDestination();
                    if (destination != null) { destination.Weapons.Add(prior.Clone()); Persist(); RuntimeLog.Info("arsenal_replaced_owned id=" + prior.WeaponId + " to=" + destination.Id); }
                }
                else if (prior.Owned && replaced) { pendingReplacements.Add(prior.Clone()); RuntimeLog.Info("arsenal_replacement_deferred id=" + prior.WeaponId); }
                state.OwnedCarried.Remove(prior.WeaponId);
            }
            foreach (WeaponRecord record in observed)
            {
                WeaponRecord prior = Find(carried, record.WeaponId);
                if (prior == null) { prior = Find(state.CarriedRecords, record.WeaponId); }
                if (prior != null)
                {
                    record.Owned = prior.Owned; record.Finish = prior.Finish; record.AcquiredUtc = prior.AcquiredUtc;
                    record.InstanceId = prior.InstanceId; record.CatalogId = prior.CatalogId;
                    record.Attachments = prior.Attachments == null ? null : new List<string>(prior.Attachments);
                    record.Progression = prior.Progression;
                }
                else
                {
                    record.Owned = state.OwnedCarried.Contains(record.WeaponId) ||
                        ArsenalPolicy.IsOwnedGain(false, mission, Environment.TickCount, moneyDecreaseAt, config.PurchaseWindowMilliseconds);
                    record.AcquiredUtc = DateTime.UtcNow.ToString("o");
                    if (record.Owned && !state.OwnedCarried.Contains(record.WeaponId)) { state.OwnedCarried.Add(record.WeaponId); Persist(); }
                    RuntimeLog.Info("arsenal_gain id=" + record.WeaponId + " owned=" + record.Owned + " mission=" + mission);
                }
                WeaponIdentity.Ensure(record);
                WeaponCatalogEntry entry = weaponCatalog == null ? null : weaponCatalog.Find(record.WeaponId);
                if (entry != null && string.IsNullOrEmpty(record.CatalogId))
                {
                    record.CatalogId = entry.Id;
                    if (string.IsNullOrEmpty(record.Finish)) { record.Finish = entry.Finishes[0]; }
                }
            }
            SetCarried(observed, current);
            if (CarriedIdentityChanged()) { Persist(); }
            if ((DateTime.UtcNow - lastSnapshotUtc).TotalSeconds >= 5)
            {
                if (CarriedSnapshotChanged()) { Persist(); }
                lastSnapshotUtc = DateTime.UtcNow;
            }
            if (gated) { return; }
            if (pendingReplacements.Count > 0)
            {
                StorageBin replacementDestination = OverflowDestination();
                if (replacementDestination != null)
                {
                    foreach (WeaponRecord pending in pendingReplacements)
                    {
                        replacementDestination.Weapons.Add(pending);
                        RuntimeLog.Info("arsenal_replaced_owned id=" + pending.WeaponId + " to=" + replacementDestination.Id);
                    }
                    pendingReplacements.Clear(); Persist();
                }
            }
            int overflow;
            while ((overflow = ArsenalPolicy.OverflowIndex(config, carried, lastUsed)) >= 0)
            {
                StorageBin destination = OverflowDestination();
                if (destination == null)
                {
                    // Logged once per episode of waiting: this loop runs every tick until a car or safehouse is known.
                    if (!overflowDeferredLogged) { overflowDeferredLogged = true; RuntimeLog.Info("arsenal_overflow_deferred no_vehicle_or_safehouse"); }
                    break;
                }
                overflowDeferredLogged = false;
                WeaponRecord moved = carried[overflow];
                ArsenalRegistry.RaiseWeaponsRemoving("overflow");
                ped.Weapons.FromType((Weapon)moved.WeaponId).Remove();
                moved.Owned = true;
                destination.Weapons.Add(moved.Clone());
                state.OwnedCarried.Remove(moved.WeaponId);
                RuntimeLog.Info("arsenal_overflow id=" + moved.WeaponId + " to=" + destination.Id);
                carried.RemoveAt(overflow);
                Persist();
                RefreshPresentation((int)ped.Weapons.CurrentType);
            }
        }

        private static WeaponRecord Find(IList<WeaponRecord> records, int id)
        {
            foreach (WeaponRecord record in records) { if (record.WeaponId == id) { return record; } }
            return null;
        }

        private void SetCarried(List<WeaponRecord> observed, int current)
        {
            bool changed = observed.Count != carried.Count;
            foreach (WeaponRecord record in observed) { if (Find(carried, record.WeaponId) == null) { changed = true; } }
            carried.Clear(); carried.AddRange(observed);
            if (changed) { RefreshPresentation(current); }
            else
            {
                foreach (CarriedWeapon item in presentation)
                {
                    if (item.InHand != (item.WeaponId == current)) { RefreshPresentation(current); break; }
                }
            }
        }

        private void RefreshPresentation(int current)
        {
            presentation.Clear();
            bool longOneUsed = false, longTwoUsed = false, sideOneUsed = false, sideTwoUsed = false;
            foreach (WeaponRecord record in carried)
            {
                string group = ArsenalPolicy.Group(config, record.Category);
                BodySlot slot = ArsenalPolicy.Slot(config, record.Category);
                if (group == "longGun")
                {
                    if (slot == BodySlot.LongGun1 && longOneUsed) { slot = BodySlot.LongGun2; }
                    else if (slot == BodySlot.LongGun2 && longTwoUsed) { slot = BodySlot.LongGun1; }
                    if (slot == BodySlot.LongGun1) { longOneUsed = true; } else { longTwoUsed = true; }
                }
                if (group == "sidearm")
                {
                    if (slot == BodySlot.SidearmPrimary && sideOneUsed) { slot = BodySlot.SidearmSecondary; }
                    else if (slot == BodySlot.SidearmSecondary && sideTwoUsed) { slot = BodySlot.SidearmPrimary; }
                    if (slot == BodySlot.SidearmPrimary) { sideOneUsed = true; } else { sideTwoUsed = true; }
                }
                presentation.Add(new CarriedWeapon(record.WeaponId, record.Category, slot, record.WeaponId == current));
            }
            revision++;
        }

        private void HandleLoss(bool busted)
        {
            ArsenalRegistry.RaiseWeaponsRemoving(busted ? "busted" : "wasted");
            string safehouseId = state.LastSafehouseId;
            if (!busted && string.IsNullOrEmpty(safehouseId))
            {
                // No safehouse visited yet this episode: the nearest known one keeps owned weapons from vanishing.
                SafehouseRule nearest = NearestSafehouse(Player.Character.Position);
                if (nearest != null) { safehouseId = nearest.Id; RuntimeLog.Info("arsenal_loss_nearest_safehouse id=" + nearest.Id); }
            }
            StorageBin destination = !busted && !string.IsNullOrEmpty(safehouseId) ?
                ArsenalPolicy.FindOrAdd(state.SafehouseStashes, safehouseId) : null;
            if (!busted && destination == null) { RuntimeLog.Error("arsenal_loss_no_safehouse owned weapons cannot be stored"); }
            foreach (WeaponRecord record in carried)
            {
                RuntimeLog.Info("arsenal_loss reason=" + (busted ? "busted" : "wasted") + " id=" + record.WeaponId + " owned=" + record.Owned);
                try
                {
                    GTA.value.Weapon weapon = Player.Character.Weapons.FromType((Weapon)record.WeaponId);
                    if (weapon.isPresent) { weapon.Remove(); }
                }
                catch (Exception error) { RuntimeLog.Error("arsenal_loss_remove_failed id=" + record.WeaponId + " error=" + error); }
            }
            ArsenalPolicy.ResolveLoss(carried, busted, destination);
            pendingReplacements.Clear();
            state.OwnedCarried.Clear();
            state.CarriedRecords.Clear();
            presentation.Clear(); revision++;
            Persist();
        }

        private List<SafehouseRule> AllSafehouses()
        {
            List<SafehouseRule> all = new List<SafehouseRule>(config.Safehouses);
            all.AddRange(discoveredSafehouses);
            return all;
        }

        private SafehouseRule NearestSafehouse(Vector3 position)
        {
            SafehouseRule best = null;
            double bestDistance = double.MaxValue;
            foreach (SafehouseRule house in AllSafehouses())
            {
                if (!string.Equals(house.Episode, episode, StringComparison.OrdinalIgnoreCase)) { continue; }
                double distance = Distance(position, house.X, house.Y, house.Z);
                if (distance < bestDistance) { bestDistance = distance; best = house; }
            }
            return best;
        }

        // The game marks every unlocked safehouse on the radar with its own sprite. Reading those blips gives
        // real, story-aware safehouse positions without shipping coordinates. Runs every 10 s on the tick.
        private void DiscoverSafehouses()
        {
            if (discoveryDisabled || config.SafehouseBlipSprite <= 0 || (DateTime.UtcNow - lastDiscoveryUtc).TotalSeconds < 10) { return; }
            lastDiscoveryUtc = DateTime.UtcNow;
            try
            {
                List<SafehouseRule> found = new List<SafehouseRule>();
                int blip = Function.Call<int>("GET_FIRST_BLIP_INFO_ID", config.SafehouseBlipSprite);
                for (int guard = 0; blip != 0 && guard < 32; guard++)
                {
                    if (Function.Call<bool>("DOES_BLIP_EXIST", blip))
                    {
                        Pointer coords = typeof(Vector3);
                        Function.Call("GET_BLIP_COORDS", blip, coords);
                        Vector3 position = (Vector3)coords;
                        if (Math.Abs(position.X) > 1 || Math.Abs(position.Y) > 1)
                        {
                            SafehouseRule house = new SafehouseRule();
                            house.Id = "blip_" + episode + "_" + ((int)Math.Round(position.X)) + "_" + ((int)Math.Round(position.Y));
                            house.Name = "Safehouse (map)";
                            house.Episode = episode;
                            house.X = position.X; house.Y = position.Y; house.Z = position.Z;
                            house.Radius = config.DiscoveredSafehouseRadiusMeters;
                            house.Verified = true;
                            found.Add(house);
                        }
                    }
                    blip = Function.Call<int>("GET_NEXT_BLIP_INFO_ID", config.SafehouseBlipSprite);
                }
                if (found.Count != discoveredSafehouses.Count) { RuntimeLog.Info("arsenal_safehouses_discovered count=" + found.Count); }
                discoveredSafehouses = found;
            }
            catch (Exception error)
            {
                discoveryDisabled = true;
                RuntimeLog.Error("arsenal_safehouse_discovery_disabled use Mark safehouse here error=" + error.Message);
            }
        }

        private void ObserveSafehouse(Ped ped)
        {
            foreach (SafehouseRule house in AllSafehouses())
            {
                if (!string.Equals(house.Episode, episode, StringComparison.OrdinalIgnoreCase)) { continue; }
                if (Distance(ped.Position, house.X, house.Y, house.Z) <= house.Radius && state.LastSafehouseId != house.Id)
                {
                    state.LastSafehouseId = house.Id; Persist(); RuntimeLog.Info("arsenal_safehouse_enter id=" + house.Id);
                }
            }
        }

        private void ObserveVehicle(Ped ped)
        {
            Vehicle current = ped.CurrentVehicle;
            if (current != null && current.Exists()) { lastVehicle = current; }
            if (current == null && lastVehicle != null && lastVehicle.Exists())
            {
                string key = VehicleKey(lastVehicle);
                if (key != null && key.StartsWith("lvs:") && state.LastVehicleKey != key) { state.LastVehicleKey = key; Persist(); }
                else if (key != null && temporaryTrunks.ContainsKey(key))
                {
                    string marker = "fallback:" + lastVehicle.Model.Hash + ":" + lastVehicle.Position.X.ToString("0.0") + ":" + lastVehicle.Position.Y.ToString("0.0");
                    if (state.LastVehicleKey == marker) { return; }
                    state.LastVehicleKey = marker;
                    state.FallbackModelHash = lastVehicle.Model.Hash;
                    state.FallbackX = lastVehicle.Position.X; state.FallbackY = lastVehicle.Position.Y; state.FallbackZ = lastVehicle.Position.Z;
                    StorageBin stored = ArsenalPolicy.FindOrAdd(state.VehicleTrunks, state.LastVehicleKey);
                    stored.Weapons.Clear(); foreach (WeaponRecord weapon in temporaryTrunks[key].Weapons) { stored.Weapons.Add(weapon.Clone()); }
                    Persist();
                }
            }
        }

        private void RefreshLvs()
        {
            if ((DateTime.UtcNow - lvsLastReadUtc).TotalSeconds < 5) { return; }
            lvsLastReadUtc = DateTime.UtcNow;
            if (!File.Exists(lvsPath)) { lvsOwned.Clear(); return; }
            try { lvsOwned = LvsOwnedVehicleReader.Parse(File.ReadAllText(lvsPath)); }
            catch (Exception error) { lvsOwned.Clear(); RuntimeLog.Error("arsenal_lvs_read_failed fallback_enabled error=" + error); }
        }

        private string VehicleKey(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.Exists()) { return null; }
            string id = LvsOwnedVehicleReader.Match(lvsOwned, episode, vehicle.Model.Hash, vehicle.Position.X,
                vehicle.Position.Y, vehicle.Position.Z, config.OwnedVehicleMatchMeters);
            if (id != null) { identifiedVehicles[vehicle.GetHashCode()] = id; return "lvs:" + id; }
            if (identifiedVehicles.TryGetValue(vehicle.GetHashCode(), out id)) { return "lvs:" + id; }
            if (vehicle.Model.Hash == state.FallbackModelHash &&
                Distance(vehicle.Position, state.FallbackX, state.FallbackY, state.FallbackZ) <= config.FallbackVehicleMatchMeters)
                { return state.LastVehicleKey; }
            return "temporary:" + vehicle.GetHashCode().ToString("X8");
        }

        private StorageBin Trunk(Vehicle vehicle)
        {
            string key = VehicleKey(vehicle);
            if (key == null) { return null; }
            if (!key.StartsWith("temporary:")) { return ArsenalPolicy.FindOrAdd(state.VehicleTrunks, key); }
            StorageBin bin;
            if (!temporaryTrunks.TryGetValue(key, out bin)) { bin = new StorageBin(); bin.Id = key; temporaryTrunks.Add(key, bin); temporaryVehicles.Add(key, vehicle); }
            return bin;
        }

        private StorageBin OverflowDestination()
        {
            if (lastVehicle != null && lastVehicle.Exists() && lastVehicle.Health > 0 && !Function.Call<bool>("IS_CAR_IN_WATER", lastVehicle)) { return Trunk(lastVehicle); }
            if (!string.IsNullOrEmpty(state.LastVehicleKey) && !state.LastVehicleKey.StartsWith("temporary:"))
                { return ArsenalPolicy.FindOrAdd(state.VehicleTrunks, state.LastVehicleKey); }
            if (!string.IsNullOrEmpty(state.LastSafehouseId)) { return ArsenalPolicy.FindOrAdd(state.SafehouseStashes, state.LastSafehouseId); }
            return null;
        }

        private void PruneTemporaryTrunks()
        {
            List<string> gone = new List<string>();
            foreach (KeyValuePair<string, Vehicle> pair in temporaryVehicles)
            {
                if (!pair.Value.Exists() || pair.Value.Health <= 0 || pair.Value.isOnFire || Function.Call<bool>("IS_CAR_IN_WATER", pair.Value)) { gone.Add(pair.Key); }
            }
            foreach (string key in gone) { RuntimeLog.Info("arsenal_temporary_trunk_lost id=" + key); temporaryTrunks.Remove(key); temporaryVehicles.Remove(key); }
        }

        private static double Distance(Vector3 position, float x, float y, float z)
        {
            double dx = position.X - x, dy = position.Y - y, dz = position.Z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        private void SnapshotCarried()
        {
            state.CarriedRecords.Clear();
            foreach (WeaponRecord record in carried) { state.CarriedRecords.Add(record.Clone()); }
        }

        private bool CarriedSnapshotChanged()
        {
            if (CarriedIdentityChanged()) { return true; }
            foreach (WeaponRecord record in carried)
            {
                WeaponRecord saved = Find(state.CarriedRecords, record.WeaponId);
                if (saved == null || saved.Ammo != record.Ammo || saved.Finish != record.Finish ||
                    saved.CatalogId != record.CatalogId || saved.Progression != record.Progression)
                    { return true; }
                if ((saved.Attachments == null ? 0 : saved.Attachments.Count) !=
                    (record.Attachments == null ? 0 : record.Attachments.Count)) { return true; }
                if (record.Attachments != null)
                {
                    for (int i = 0; i < record.Attachments.Count; i++)
                    {
                        if (saved.Attachments[i] != record.Attachments[i]) { return true; }
                    }
                }
            }
            return false;
        }

        private bool CarriedIdentityChanged()
        {
            if (state.CarriedRecords.Count != carried.Count) { return true; }
            foreach (WeaponRecord record in carried)
            {
                WeaponRecord saved = Find(state.CarriedRecords, record.WeaponId);
                if (saved == null || saved.InstanceId != record.InstanceId || saved.Owned != record.Owned)
                    { return true; }
            }
            return false;
        }

        private void Persist() { SnapshotCarried(); JsonStore.Save(statePath, state); }

        private void UpdateStorageInteraction(Ped ped, bool gated)
        {
            storageInput.Poll();
            bool openKey = Game.isKeyPressed(Keys.E) || storageInput.IsDown(ControllerInput.XButton);
            bool up = Game.isKeyPressed(Keys.Up) || storageInput.IsDown(ControllerInput.DPadUp);
            bool down = Game.isKeyPressed(Keys.Down) || storageInput.IsDown(ControllerInput.DPadDown);
            bool select = Game.isKeyPressed(Keys.Enter) || storageInput.IsDown(ControllerInput.AButton);
            bool back = Game.isKeyPressed(Keys.Back) || storageInput.IsDown(ControllerInput.BButton);

            if (activeStorage != null)
            {
                if (gated || DevToolsMenu.IsOpen) { CloseStorage(); }
                else
                {
                    if (up && !previousUp) { storageSelection = (storageSelection + storageItems.Count - 1) % storageItems.Count; }
                    if (down && !previousDown) { storageSelection = (storageSelection + 1) % storageItems.Count; }
                    if (select && !previousSelect && storageItems[storageSelection].Activate != null)
                    {
                        storageMessage = RunAction(storageItems[storageSelection].Activate);
                        RebuildStorageItems();
                    }
                    if (back && !previousBack) { CloseStorage(); }
                }
            }
            else if (!gated && !DevToolsMenu.IsOpen && Player.CanControlCharacter &&
                !Function.Call<bool>("IS_CHAR_IN_ANY_CAR", ped))
            {
                int now = Environment.TickCount;
                if (lastStorageScanTicks == 0 || unchecked(now - lastStorageScanTicks) >= 250)
                {
                    lastStorageScanTicks = now;
                    FindNearbyStorage(ped);
                }
                if (openKey && !previousStorageKey && (nearbyTrunk != null || nearbySafehouse != null))
                {
                    if (nearbyTrunk != null && !nearbyTrunk.Exists()) { nearbyTrunk = null; }
                    if (nearbyTrunk == null && nearbySafehouse == null) { previousStorageKey = openKey; return; }
                    activeStorage = nearbyTrunk != null ? Trunk(nearbyTrunk) :
                        ArsenalPolicy.FindOrAdd(state.SafehouseStashes, nearbySafehouse.Id);
                    if (activeStorage == null) { nearbyTrunk = null; previousStorageKey = openKey; return; }
                    StorageOpen = true;
                    if (nearbyTrunk != null)
                    {
                        CloseTrunk();
                        nearbyTrunk.Door(VehicleDoor.Trunk).Open();
                        openedTrunk = nearbyTrunk;
                    }
                    storageSelection = 0;
                    storageMessage = "";
                    RebuildStorageItems();
                    Player.CanControlCharacter = false;
                    storageControlLocked = true;
                    RuntimeLog.Info("arsenal_storage_open id=" + activeStorage.Id);
                }
            }
            else { nearbyTrunk = null; nearbySafehouse = null; }

            previousStorageKey = openKey;
            previousUp = up;
            previousDown = down;
            previousSelect = select;
            previousBack = back;
        }

        private void FindNearbyStorage(Ped ped)
        {
            nearbyTrunk = null;
            nearbySafehouse = null;
            Vehicle vehicle = World.GetClosestVehicle(ped.Position, config.TrunkDistanceMeters + config.TrunkRearOffsetMeters);
            if (vehicle != null && vehicle.Exists() && vehicle.Health > 0 && !Function.Call<bool>("IS_CAR_IN_WATER", vehicle))
            {
                Vector3 rear = vehicle.GetOffsetPosition(new Vector3(0, -config.TrunkRearOffsetMeters, 0));
                if (Distance(ped.Position, rear.X, rear.Y, rear.Z) <= config.TrunkDistanceMeters) { nearbyTrunk = vehicle; }
            }
            if (nearbyTrunk != null) { return; }
            foreach (SafehouseRule house in AllSafehouses())
            {
                if (house.Episode == episode && Distance(ped.Position, house.X, house.Y, house.Z) <=
                    Math.Min(house.Radius, config.TrunkDistanceMeters)) { nearbySafehouse = house; break; }
            }
        }

        private void RebuildStorageItems()
        {
            storageItems.Clear();
            StorageBin bin = activeStorage;
            if (bin == null) { return; }
            foreach (WeaponRecord record in carried)
            {
                WeaponRecord choice = record;
                storageItems.Add(MenuItem.Action("Store " + Describe(choice), () => Store(choice, bin)));
            }
            foreach (WeaponRecord record in new List<WeaponRecord>(bin.Weapons))
            {
                WeaponRecord choice = record;
                storageItems.Add(MenuItem.Action("Take " + Describe(choice), () => Take(choice, bin)));
            }
            if (storageItems.Count == 0) { storageItems.Add(MenuItem.Info(() => "No weapons to transfer")); }
            storageSelection = Math.Min(storageSelection, storageItems.Count - 1);
        }

        private void CloseStorage()
        {
            if (activeStorage == null && !storageControlLocked) { return; }
            string id = activeStorage != null ? activeStorage.Id : "unknown";
            activeStorage = null;
            StorageOpen = false;
            storageItems.Clear();
            try { CloseTrunk(); }
            finally
            {
                if (storageControlLocked && Player != null) { Player.CanControlCharacter = true; }
                storageControlLocked = false;
            }
            RuntimeLog.Info("arsenal_storage_closed id=" + id);
        }

        private void OnStorageDraw(object sender, GraphicsEventArgs args)
        {
            if (disabled || (activeStorage == null && nearbyTrunk == null && nearbySafehouse == null) || DevToolsMenu.IsOpen) { return; }
            try
            {
                GTA.Graphics graphics = args.Graphics;
                graphics.Scaling = FontScaling.Pixel;
                if (activeStorage == null)
                {
                    string label = nearbyTrunk != null ? "Square / X or E  Open trunk" : "Square / X or E  Open safehouse storage";
                    graphics.DrawRectangle(new RectangleF(36, 580, 420, 38), Color.FromArgb(190, 8, 12, 18));
                    graphics.DrawText(label, new RectangleF(48, 587, 400, 27), TextAlignment.Left, storageFont);
                    return;
                }
                int rows = Math.Min(10, storageItems.Count);
                float height = 95 + rows * 28 + 50;
                graphics.DrawRectangle(new RectangleF(36, 80, 590, height), Color.FromArgb(205, 8, 12, 18));
                string title = openedTrunk != null ? "TRUNK" : "SAFEHOUSE STORAGE";
                graphics.DrawText(title, new RectangleF(52, 92, 550, 29), TextAlignment.Left, storageTitleFont);
                int scroll = Math.Max(0, storageSelection - rows + 1);
                for (int row = 0; row < rows; row++)
                {
                    int index = scroll + row;
                    if (index >= storageItems.Count) { break; }
                    float y = 130 + row * 28;
                    if (index == storageSelection) { graphics.DrawRectangle(new RectangleF(46, y - 2, 570, 27), Color.FromArgb(120, 190, 145, 35)); }
                    graphics.DrawText(storageItems[index].Label(), new RectangleF(58, y, 540, 26), TextAlignment.Left, storageFont);
                }
                graphics.DrawText(storageMessage.Length > 0 ? storageMessage : "D-pad choose   A transfer   B close",
                    new RectangleF(52, 142 + rows * 28, 550, 29), TextAlignment.Left, storageFont);
            }
            catch (Exception error)
            {
                RuntimeLog.Error("arsenal_storage_draw_failed error=" + error);
                disabled = true;
            }
        }

        private List<MenuItem> BuildPage()
        {
            if (disabled) { return new List<MenuItem> { MenuItem.Info(() => "Arsenal disabled; see log") }; }
            try { return BuildPageCore(); }
            catch (Exception error)
            {
                Disable(error);
                return new List<MenuItem> { MenuItem.Info(() => "Arsenal disabled; see log") };
            }
        }

        private List<MenuItem> BuildPageCore()
        {
            List<MenuItem> items = new List<MenuItem>();
            if (config == null || state == null || Player == null || Player.Character == null)
                { items.Add(MenuItem.Info(() => "Arsenal unavailable; see log")); return items; }
            items.Add(MenuItem.Action("Mark safehouse here", () => RunAction(MarkSafehouse)));
            if (!StorageAllowed())
                { items.Add(MenuItem.Info(() => "Storage locked during mission or fade")); return items; }
            Ped ped = Player.Character;
            Vehicle vehicle = World.GetClosestVehicle(ped.Position, config.TrunkDistanceMeters + 3);
            StorageBin bin = null;
            bool trunk = false;
            if (vehicle != null && vehicle.Exists() && vehicle.Health > 0 &&
                !Function.Call<bool>("IS_CAR_IN_WATER", vehicle) &&
                Distance(ped.Position, vehicle.GetOffsetPosition(new Vector3(0, -config.TrunkRearOffsetMeters, 0)).X,
                    vehicle.GetOffsetPosition(new Vector3(0, -config.TrunkRearOffsetMeters, 0)).Y,
                    vehicle.GetOffsetPosition(new Vector3(0, -config.TrunkRearOffsetMeters, 0)).Z) <= config.TrunkDistanceMeters)
            {
                bin = Trunk(vehicle); trunk = true;
                if (openedTrunk != vehicle) { CloseTrunk(); vehicle.Door(VehicleDoor.Trunk).Open(); openedTrunk = vehicle; }
            }
            if (bin == null)
            {
                foreach (SafehouseRule house in AllSafehouses())
                {
                    if (house.Episode == episode && Distance(ped.Position, house.X, house.Y, house.Z) <= house.Radius)
                        { bin = ArsenalPolicy.FindOrAdd(state.SafehouseStashes, house.Id); break; }
                }
            }
            if (bin == null) { items.Add(MenuItem.Info(() => "Stand at a trunk rear or safehouse stash")); return items; }
            StorageBin selected = bin;
            items.Add(MenuItem.Info(() => (trunk ? "TRUNK " : "SAFEHOUSE ") + selected.Id));
            if (!trunk && weaponCatalog != null)
            {
                WeaponRecord pistol = Find(carried, 7) ?? Find(carried, 58);
                if (pistol != null)
                {
                    WeaponRecord choice = pistol;
                    items.Add(MenuItem.Info(() => "GUNSMITH: service pistol finish"));
                    if (choice.WeaponId == 7 && (choice.Progression > 0 || config.GunsmithGoldFinishPrice > 0))
                    {
                        string label = choice.Progression > 0 ? "Equip gold finish" :
                            "Buy gold finish ($" + config.GunsmithGoldFinishPrice + ")";
                        items.Add(MenuItem.Confirmed(label, () => RunAction(() => ChangePistolFinish(choice, true))));
                    }
                    else
                    {
                        items.Add(MenuItem.Action("Equip factory finish", () => RunAction(() => ChangePistolFinish(choice, false))));
                    }
                }
            }
            foreach (WeaponRecord record in carried)
            {
                WeaponRecord choice = record;
                items.Add(MenuItem.Action("Store " + Describe(choice), () => RunAction(() => Store(choice, selected))));
            }
            foreach (WeaponRecord record in new List<WeaponRecord>(bin.Weapons))
            {
                WeaponRecord choice = record;
                items.Add(MenuItem.Action("Take " + Describe(choice), () => RunAction(() => Take(choice, selected))));
            }
            return items;
        }

        private static string Describe(WeaponRecord record)
        {
            LibertyFramework.Gunplay.GunplayController gunplay = LibertyFramework.Gunplay.GunplayController.Instance;
            string name = LibertyFramework.Weapons.TestWeaponActions.LabelFor(gunplay != null ? gunplay.Config : null, record.WeaponId);
            return name + "  (" + record.Ammo + " rounds" + (record.Owned ? ", owned" : "") + ")";
        }

        private string Store(WeaponRecord record, StorageBin bin)
        {
            if (Player == null || Player.Character == null || !StorageAllowed()) { return "Storage unavailable"; }
            GTA.value.Weapon weapon = Player.Character.Weapons.FromType((Weapon)record.WeaponId);
            if (!weapon.isPresent) { return "Weapon no longer carried"; }
            ArsenalRegistry.RaiseWeaponsRemoving("store");
            WeaponRecord stored = record.Clone(); stored.Owned = true; stored.Ammo = weapon.Ammo;
            weapon.Remove(); bin.Weapons.Add(stored);
            carried.Remove(record); state.OwnedCarried.Remove(record.WeaponId); Persist();
            RefreshPresentation((int)Player.Character.Weapons.CurrentType);
            RuntimeLog.Info("arsenal_store id=" + record.WeaponId + " to=" + bin.Id);
            return "Stored " + record.WeaponId;
        }

        private string ChangePistolFinish(WeaponRecord record, bool gold)
        {
            if (Player == null || Player.Character == null || !StorageAllowed()) { return "Gunsmith unavailable"; }
            if (string.IsNullOrEmpty(state.LastSafehouseId) ||
                Find(carried, record.WeaponId) != record || (record.WeaponId != 7 && record.WeaponId != 58))
                { return "Service pistol not available"; }
            bool purchase = gold && record.Progression == 0;
            if (purchase && config.GunsmithGoldFinishPrice <= 0) { return "Gold finish price not configured"; }
            if (purchase && Player.Money < config.GunsmithGoldFinishPrice) { return "Not enough money"; }
            int sourceId = record.WeaponId;
            int targetId = gold ? 58 : 7;
            GTA.value.Weapon source = Player.Character.Weapons.FromType((Weapon)sourceId);
            if (!source.isPresent) { return "Pistol no longer carried"; }
            int ammo = source.Ammo;
            ArsenalRegistry.RaiseWeaponsRemoving("gunsmith_finish_switch");
            Player.Character.Weapons.Select((Weapon)targetId);
            GTA.value.Weapon target = Player.Character.Weapons.FromType((Weapon)targetId);
            if (!target.isPresent) { return "Finish switch failed; no charge"; }
            target.Ammo = ammo;
            if (purchase)
            {
                Player.Money = Player.Money - config.GunsmithGoldFinishPrice;
                previousMoney = Player.Money; moneyDecreaseAt = -1;
                record.Progression = 1;
            }
            record.WeaponId = targetId;
            record.CatalogId = gold ? "gold-test-pistol" : "service-pistol";
            record.Finish = gold ? "gold-test" : "factory";
            record.Owned = true;
            state.OwnedCarried.Remove(sourceId);
            if (!state.OwnedCarried.Contains(targetId)) { state.OwnedCarried.Add(targetId); }
            Persist(); RefreshPresentation((int)Player.Character.Weapons.CurrentType);
            RuntimeLog.Info("arsenal_gunsmith_finish instance=" + record.InstanceId + " from=" + sourceId +
                " to=" + targetId + " paid=" + purchase + " ammo=" + ammo);
            return gold ? "Gold finish equipped" : "Factory finish equipped";
        }

        private string Take(WeaponRecord record, StorageBin bin)
        {
            if (Player == null || Player.Character == null || !StorageAllowed()) { return "Storage unavailable"; }
            if (!WeaponIdentity.CanTake(carried, record)) { return "Already carrying this weapon type; store it first"; }
            WeaponRecord displaced = null;
            foreach (WeaponRecord carriedRecord in carried)
            {
                if (carriedRecord.Category == record.Category && carriedRecord.WeaponId != record.WeaponId)
                    { displaced = carriedRecord; break; }
            }
            WeaponRecord savedDisplaced = null;
            if (displaced != null && displaced.Owned)
            {
                GTA.value.Weapon priorWeapon = Player.Character.Weapons.FromType((Weapon)displaced.WeaponId);
                savedDisplaced = displaced.Clone(); savedDisplaced.Ammo = priorWeapon.Ammo;
            }
            Player.Character.Weapons.Select((Weapon)record.WeaponId);
            Player.Character.Weapons.FromType((Weapon)record.WeaponId).Ammo = record.Ammo;
            bin.Weapons.Remove(record);
            if (savedDisplaced != null)
            {
                bin.Weapons.Add(savedDisplaced);
                RuntimeLog.Info("arsenal_take_displaced id=" + savedDisplaced.WeaponId + " instance=" + savedDisplaced.InstanceId + " to=" + bin.Id);
            }
            if (displaced != null) { carried.Remove(displaced); state.OwnedCarried.Remove(displaced.WeaponId); }
            WeaponRecord restored = record.Clone(); restored.Owned = true;
            WeaponRecord stale = Find(carried, restored.WeaponId);
            if (stale != null) { carried.Remove(stale); }
            carried.Add(restored);
            if (!state.OwnedCarried.Contains(record.WeaponId)) { state.OwnedCarried.Add(record.WeaponId); }
            Persist(); RefreshPresentation((int)Player.Character.Weapons.CurrentType);
            RuntimeLog.Info("arsenal_take id=" + record.WeaponId + " instance=" + record.InstanceId + " from=" + bin.Id);
            return "Taken " + record.WeaponId;
        }

        private string MarkSafehouse()
        {
            if (Player == null || Player.Character == null) { return "Player unavailable"; }
            Vector3 position = Player.Character.Position;
            SafehouseRule house = new SafehouseRule();
            house.Id = "marked_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"); house.Name = "Marked safehouse";
            house.Episode = episode; house.X = position.X; house.Y = position.Y; house.Z = position.Z;
            house.Radius = config.TrunkDistanceMeters; house.Verified = true;
            config.Safehouses.Add(house); ArsenalConfigValidator.Validate(config);
            JsonStore.Save(LibertyPaths.ArsenalConfig, config);
            state.LastSafehouseId = house.Id; Persist();
            RuntimeLog.Info("arsenal_safehouse_marked id=" + house.Id + " x=" + house.X + " y=" + house.Y + " z=" + house.Z);
            return "Marked " + house.Id;
        }

        private static bool StorageAllowed()
        {
            return !Function.Call<bool>("GET_MISSION_FLAG") &&
                !(Function.Call<bool>("HAS_CUTSCENE_LOADED") && !Function.Call<bool>("HAS_CUTSCENE_FINISHED")) &&
                !Function.Call<bool>("IS_SCREEN_FADING") && !Function.Call<bool>("IS_SCREEN_FADED_OUT");
        }

        private string RunAction(Func<string> action)
        {
            if (disabled) { return "Arsenal disabled; see log"; }
            try { return action(); }
            catch (Exception error) { Disable(error); return "Arsenal disabled; see log"; }
        }

        private void Disable(Exception error)
        {
            disabled = true;
            RuntimeLog.Error("arsenal_disabled error=" + error);
            CloseStorageSafely();
            CloseTrunkSafely();
            if (ArsenalRegistry.CarriedWeapons == this) { ArsenalRegistry.CarriedWeapons = null; }
        }

        private void CloseTrunk()
        {
            if (openedTrunk != null && openedTrunk.Exists()) { openedTrunk.Door(VehicleDoor.Trunk).Close(); }
            openedTrunk = null;
        }

        private void CloseTrunkSafely()
        {
            try { CloseTrunk(); } catch (Exception error) { RuntimeLog.Error("arsenal_trunk_restore_failed error=" + error); }
        }

        private void CloseStorageSafely()
        {
            try { CloseStorage(); } catch (Exception error) { RuntimeLog.Error("arsenal_storage_restore_failed error=" + error); }
        }

        private void OnDomainUnload(object sender, EventArgs args)
        {
            StorageOpen = false;
            // Restore player control as the DevTools script does on reload; door natives stay on ticks.
            if (storageControlLocked && Player != null)
            {
                try { Player.CanControlCharacter = true; storageControlLocked = false; }
                catch (Exception error) { RuntimeLog.Error("arsenal_storage_unload_restore_failed error=" + error); }
            }
            if (ArsenalRegistry.CarriedWeapons == this) { ArsenalRegistry.CarriedWeapons = null; }
        }
    }
}

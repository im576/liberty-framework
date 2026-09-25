# T-025 — Physical weapon records and catalog prototype

Status: **NEEDS-PLAYTEST**

## Scope

Arsenal records now carry a stable `instanceId`, catalog ID, finish, attachment IDs, and progression counter through carried state, trunks, safehouses, and wasted recovery. The older `ownedCarried` ID list remains readable. Legacy storage records gain instance IDs on load. Catalog entries register a non-gold vanilla service-sidearm replacement pair (pistol 7, combat pistol 9) and the existing custom carbine 59 as an add-on example. No new model or mechanical attachment effect is asserted. The catalog is a data contract for later gunsmith visuals and effects.

The existing CE custom weapon IDs 58–60 and original gold assets are reused. No new native or memory address is called.

A safehouse **GUNSMITH** row now sells the existing gold pistol finish for the configured price. It switches vanilla pistol ID 7 to custom ID 58 with the same ammo and physical instance ID, and records progression level 1. The owner can switch back to factory and re-equip gold for free. The change is visually observable; attachment slots remain metadata only and have no effect yet. If the catalog is absent or corrupt, Arsenal logs `arsenal_catalog_unavailable` and continues its legacy storage operation; a missing/zero gunsmith price disables purchase.

## Offline evidence

`tools/build.ps1` builds 95 C# sources with zero errors and warnings. `tools/verify.ps1` reports `RESULT passed=250 failed=0`, including JSON round-trip, transfer identity, same-type take rejection, loss, finish-unlock, duplicate repair, and catalog checks. GTA IV remains unrun by this agent.

## Human test steps

Precondition: install `LibertyFramework.net.dll` and `config/weapon-catalog.json` with the integrated Phase 2 package while GTA IV is closed. Retain a backup of `scripts/LibertyFramework/state/arsenal_iv.json`.

1. Load IV free roam. Hold **L3+R3 for 0.7 s** or press **F10**, choose **WEAPONS** with D-pad and **A**, give the gold test carbine (ID 59), and close with **B**. Expect normal carbine aim/fire. Check `state/arsenal_iv.json`: its `carriedRecords` entry for ID 59 has a nonempty `instanceId`, `catalogId` of `gold-test-carbine`, and `finish` of `gold-test` after at most five seconds.
2. At a previously marked safehouse or the rear of an owned vehicle, open **ARSENAL**. Select **Store** for that carbine with **A**. Check the JSON: the same `instanceId` is now in that safehouse stash or `lvs:<owned id>` vehicle trunk, absent from `carriedRecords`. Select **Take** with **A**. Expect the carbine and ammo back; verify the same ID is in `carriedRecords` and absent from storage.
3. Close the game normally, relaunch, and repeat step 2. Expect the same instance ID and ammo. Also verify the owned car's trunk association survives relaunch. Test once in TLAD/TBOGT if available; each episode must use its own Arsenal state file.
4. With two owned same-category weapons, take one from storage while carrying the other. Expect the displaced owned record in the same stash or trunk with its original instance ID. The taken weapon should retain its own instance ID. Confirm both IDs are distinct and the log has `arsenal_take_displaced`.
5. With a purchased owned weapon carried, get **wasted**. Return to the last safehouse stash; its record should retain the same instance ID. Repeat from a save and get **busted**; expect carried weapons to be gone. Restore the backup state after destructive test scenarios.
6. Carry vanilla pistol ID 7 at a safehouse. Open **ARSENAL**, select **Buy gold finish ($500)** and confirm with **A** twice. Expect money to fall by exactly $500, the visible pistol to become gold ID 58, ammo unchanged, and the same `instanceId` in state with `progression: 1`. Choose **Equip factory finish** with **A**; expect the original model ID 7 and no charge. Choose **Equip gold finish** again; expect gold ID 58 and no charge. Store/take that pistol and reload to confirm the unlock and identity persist. If the player's existing `arsenal.json` lacks `gunsmithGoldFinishPrice`, the purchase row should be absent while storage still works.

## Integration

Package `config/weapon-catalog.json` with the DLL and merge `gunsmithGoldFinishPrice` from the template `config/arsenal.json` into the installed config while preserving its marked safehouses. The branch does not change the integrated installer. Arsenal can run without the catalog, but gunsmith choices require it. No game files were installed by this stream.

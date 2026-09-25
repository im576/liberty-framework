# T-023 — LVS CE body variants

Status: **BLOCKED**

## Scope and findings

The existing MIT Liberty Vehicle Services CE workshop already probes vehicle extras, previews and charges for them, and saves their states in `scripts/LibertyVehicleServicesCE.owned.ini` for owned-car restore. These extras can include body geometry, but may also be taxi signs, roof lights, or other equipment. A generic relabel as "body variants" was rejected before integration because it would misrepresent those parts.

The installed GTA IV 1.2.0.59 assets contain `sultanrs.wft` in `pc/models/cdimages/vehicles.img` and a `sultanrs` definition in `common/data/vehicles.ide`. An offline RSC05 asset probe did not reveal the extra-slot mapping. Secondary reports mention a Sultan RS hood-intake variation, but do not establish its native extra index or behavior on this installed build. No third-party body assets were copied, no LVS derivative is shipped, and no game files were modified.

## Blocked

Need an in-game probe that logs the available extra indices on an owned Sultan RS and records a screenshot before/after each preview. Once a slot is confirmed as a body part, constrain any workshop label or variant grouping to that model and slot. The current generic **Extras** workshop remains functional and retains LVS ownership/persistence.

## Human test steps for the spike

1. In IV free roam, drive or register a **Sultan RS** and take it to an LVS workshop. Use the workshop's on-screen controls to open **Extras**.
2. For each available slot, move **Left/Right** to preview on/off, photograph the car from the same camera angle, and record the slot number plus the visibly changed part. Press the on-screen **Back** key to cancel; verify the car returns to its prior state and no money is charged.
3. If a hood intake, spoiler, bumper, or other actual body panel is identified, buy that slot with the on-screen **Select** key, close and relaunch GTA IV, and revisit the same owned car. Verify the part is restored and its `extras=` entry is present under the matching `[owned.<id>]` record in `scripts/LibertyVehicleServicesCE.owned.ini`.
4. Damage and repair nearby panels, open the trunk, and check collision and vehicle entry. Report the model, slot, screenshot pair, INI excerpt, and any log errors so the model-scoped extension can be implemented.

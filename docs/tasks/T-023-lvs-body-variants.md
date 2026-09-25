# T-023 — LVS CE body variant workshop labels

Status: **NEEDS-PLAYTEST**

## Scope

`tools/extend-lvs-body-variants.ps1` transforms the reviewed MIT Liberty Vehicle Services CE source into an extended script for staging. It calls the existing workshop's extra-geometry probe and relabels available geometry as body variants in its native and fallback menus. Preview, purchase/refund, owned-vehicle INI capture, and restore remain LVS's existing implementation. The script refuses upstream source versions other than the reviewed SHA-256. It introduces no external model or texture and no new native.

The exact body shape behind each GTA IV extra slot and which car exposes it need live visual verification. No claim is made that every vehicle has variants or that arbitrary extras form exclusive sets.

## Human test steps

Precondition: orchestrator stages the patched `LibertyVehicleServicesCE.CS` from the reviewed MIT source, preserves its LICENSE/CREDITS and existing INI, then installs only while GTA IV is closed. Back up an owned vehicle before testing.

1. Drive a registered car to an LVS CE workshop. Use the on-screen workshop key to enter the menu, choose **Body variants** (the former **Extras** item), and select it. If this model reports no variants, try a different GTA IV car and record its model. For a car with variants, each row should say `Body variant <slot>` and preview should visibly change the vehicle body geometry when moving left or right.
2. Press the workshop **Select/Buy** key shown on screen for one variant. Expect the displayed price to be charged exactly once and the variant to remain after exiting the workshop. Cancel a different preview with the on-screen **Back** key; expect no charge and the prior geometry restored.
3. With the purchased car registered in LVS, close and relaunch GTA IV. Return to the owned car or request delivery. Expect the purchased variant restored from `scripts/LibertyVehicleServicesCE.owned.ini` `extras=` on its `[owned.<id>]` record.
4. Damage a panel near the variant, repair at the workshop, and test trunk access at the rear. Expect normal collision, damage, and trunk behavior. Repeat a load in TLAD or TBOGT if available and report which model/slot was tested.

## Integration

Run `tools/extend-lvs-body-variants.ps1 -LvsDirectory <reviewed source root> -OutputPath <staging>/scripts/LibertyVehicleServicesCE.CS` during packaging. It generated an output against the reviewed local source without touching the game. This is an MIT derivative; ship the upstream LICENSE and CREDITS beside the generated script.

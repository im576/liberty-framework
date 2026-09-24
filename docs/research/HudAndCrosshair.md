# HUD and crosshair

The gold-weapon reticle should be minimal, neutral, and communicate spread. It should not show target identity or health. This is a product requirement, not a claim that CE exposes a per-weapon HUD hook.

Possible implementation paths to test in T-008, in order of least invasive change: game targeting options, supported native/ScriptHookDotNet HUD calls, and only then a documented hook. A global setting would need precise save/restore around gold-weapon selection and error handling; it may still fail during cover or missions. No native or memory address is validated yet.

T-008 should record screenshots or video for vanilla and gold weapons while aiming at empty space, a pedestrian, an enemy, and a vehicle, with controller and keyboard/mouse. If vanilla elements cannot be hidden safely per weapon, keep the limitation explicit rather than drawing a custom reticle over them.

The proposed spread reticle needs a mapping from calculated accuracy to on-screen radius. No numerical tuning is specified before the first human visual test.

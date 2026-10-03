---
name: liberty-presentation
description: Design or validate Liberty mod HUD, reticles, menus and atmosphere against gameplay state, actual captures, input ownership and restoration behavior.
---

# Coordinate presentation

Read the owning mod's current visual brief and relevant approved art requests.
Keep its palette, layout, weapon character and weather tuning in that mod. Generic
canvas, input and visibility mechanisms belong to Liberty Framework.

For HUD/reticles/wheel, identify the real gameplay state being represented and its
update timing. Reticle spread/recovery must come from the gunplay state; distinct
weapon designs may present that state differently. Define input focus and layering
with other menus. Preserve essential mission/help/radar information and prove the
affected vanilla elements hide and restore on disable, death, reload and cutscenes.

For atmosphere, separate color/lighting, cloud geometry/textures, rain particles,
wet materials/reflections and ambient effects. Timecycle tuning alone does not
prove cloud-shape or wet-surface control. Identify each path's evidence and expected
cost before promising it. Use the research skill for unknown game consumers/hooks.

Compare actual baseline/candidate captures under matched location, time, weather,
camera and plugin configuration. Inspect moving sequences and transitions as well
as stills. Check readability in bright/dark scenes, natural color, effect clipping,
draw ordering and controller interaction where relevant. Concept art communicates
intent; a capture path or generated reference is not runtime visual proof.

Measure affected frame times with unchanged population and comparable scene load.
Do not improve apparent performance by hiding required gameplay or reducing scene
density without the user's direction. Separate cold asset preparation from steady
state. Keep visual sign-off distinct from correctness and performance evidence.

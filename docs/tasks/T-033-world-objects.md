# T-033 — Static world objects: build, register, place

Status: **NEEDS-PLAYTEST** (queued: `T033-world-wall-report`, `T033-world-objects`, `T033-world-walk`)

Depends on: T-030 (`type: object`), T-032 (borrowed collision). Session 6 of [NEXT_SESSIONS](../workflow/NEXT_SESSIONS.md).

## The path

Blender → export (`type: object`) → LibertyContent builds the drawable (either writer), its texture dictionary and the
borrowed collision → `package` puts them in `LibertyContent.img` and registers the model in `lf_content.ide` →
`mods/Liberty.World` places it in the map from `config/world/objects.json` → scenario `world-objects`.

- **Registration.** World objects use the same IDE class (`weap`) as every LibertyContent model: the class the proven
  script-spawn path uses. A dedicated `objs` entry and map placement files (IPL/WPL) are not written because their
  formats are not established in this repository (AGENTS.md rule 4). A map editor is out of scope.
- **Placement** (`mods/Liberty.World`, `WorldObjectsModule`). An SDK mod:
  - reads `config\world\objects.json`: placements with name, model, position, heading and snapToGround; stream-in and
    stream-out radii; check interval; collision on or off;
  - hot-reloads it;
  - spawns each object frozen, with collision, snapped to the ground, when the player is within `streamInMeters`, and
    deletes it beyond `streamOutMeters` (hysteresis);
  - `world` lists the objects and their state, and `world ray <name>` raycasts from the player to one (the collision
    test);
  - uses only SDK services (props, streaming, world query, world control), so no new native.
- **Collision.** Borrowed from a vanilla prop (T-032) until the collision writer exists. `lf_world_wall`'s authored
  shape is not written (its outline is the borrowed prop's, not the wall's).
- **Capabilities.** Both writers now build `type: object`. LCC033 remains only for a compiler that excludes it, and
  LCC034 warns about an object without authored or borrowed collision.

## Built

- `mods/Liberty.World`: `WorldObjectsModule.cs`, and `Logic/` holding `WorldObjectsConfig` and `WorldStreaming`.
  - `WorldObjectsConfig` is DataContract, with defaults kept by `[OnDeserializing]`, because
    `DataContractJsonSerializer` runs no initialisers. The verifier caught that before the fix.
  - `WorldStreaming` holds the decision and the horizontal distance.
- `config/world/objects.json`: `test_wall`, 8 m north of the east_park teleport. Staged by `package-phase2.ps1`, and
  documented in [CONFIG_SCHEMA.md](../architecture/CONFIG_SCHEMA.md#world-objects-configworldobjectsjson).
- Verifier section "World objects: config and streaming": the shipped config validates, every placed model is a
  `content/` asset, bad configs are refused, absent members keep their defaults, and hysteresis holds.
  `verify.ps1` now compiles `mods/*/Logic` too.
- `content/objects/lf_world_wall` (`tools/blender/examples/make_lf_world_wall.py`): a 2.1 m stone wall with LOD 0 and
  LOD 1, one material (v1 build), `type: object`, borrowed collision.
- Scenario `world-objects`; manual check `T033-world-walk`.

## Offline evidence

- C# build: the mod builds, warnings as errors.
- Verifier: 242 passed / 7 NOT RUN.
- LibertyContent: `lf_world_wall` validates (LCC039, LCC025).
- **NEEDS LOCAL VERIFY:** everything in game.

## Open questions

1. Does the object stream in, stand on the ground, and show its texture? `T033-world-objects`.
2. Does its borrowed collision stop the engine's ray (`T033-world-objects`) and the player (`T033-world-walk`)?
3. Heading: `SetRotation(0, 0, heading)` is assumed to match `GetHeading`'s convention (0 = north, counter-clockwise).
   The test wall's long side is its model X axis and it stands at heading 90. If the rotation applies the heading, it
   runs north–south, end-on to the player, who faces north after the teleport. Across the view means it did not.

## Human test steps

1. Run `./tools/verify-local.ps1 -Only T033-world-wall-report,T033-world-objects,T033-world-walk`, or everything queued.
2. **`T033-world-objects`:** PASS, or FAIL at `world ray` if the borrowed shape does not reach the ray. The screenshot
   `wall_ahead` shows a low grey wall ahead of the player, standing on the ground, seen end-on: it runs away from the
   player, north–south. Across the view means the heading was not applied.
3. **`T033-world-walk`** (manual, 2 minutes). Teleport to East Park, walk into the wall and along it. Answer p if it
   blocks you somewhere, f if you pass through everywhere.

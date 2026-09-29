# Scenario hot-reload

- Result: PASS
- Steps: 24, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- launch: engine booted on attempt 1
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- spawnprop lf_test_crate 2.5 0 => spawning prop lf_test_crate
- expect autopilot_prop handle=\d+ model=lf_test_crate: OK 2026-09-29T19:28:57.311Z [INFO] [autopilot] autopilot_prop handle=16644 model=lf_test_crate at=(-64.428, 677.333, 13.568)
- restart devtools => restarted devtools
- expect engine_module_restarted devtools: OK 2026-09-29T19:28:57.503Z [INFO] engine_module_restarted devtools
- reload autopilot => reloaded Liberty.Autopilot.dll: autopilot
- expect engine_module_stopped autopilot released=[1-9]: OK 2026-09-29T19:28:57.999Z [INFO] engine_module_stopped autopilot released=2 reason=reloading
- expect engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1: OK 2026-09-29T19:28:58.006Z [INFO] engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1 leaked_kb=54
- owned autopilot => autopilot: nothing
- expect line=.owned autopilot. reply=.autopilot: nothing: OK 2026-09-29T19:28:58.243Z [INFO] command source=file:cmd_20260929192858200.cmd line="owned autopilot" reply="autopilot: nothing"
- god on => invincible True
- wanted 0 => wanted 0
- modules => gunplay avg=1.184 max=176.37 int=0 | combat avg=0.109 max=147.60 int=0 | arsenal avg=0.555 max=105.65 int=30 | holsters avg=0.067 max=69.98 int=50 | atmosphere avg=0.225 max=8.13 int=0 | devtools avg=0.022 max=0.04 int=30 | probe avg=1.428 max=1.44 int=10000 | weapon-probe avg=0.000 max=0.01 int=0 | world avg=0.239 max=2.79 int=500 | autopilot avg=0.000 max=0.00 int=0
- selftest => selftest started
- expect selftest_done passed=\d+ failed=0: OK 2026-09-29T19:29:12.873Z [INFO] [autopilot] selftest_done passed=49 failed=0
- reload devtools => devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)
- expect line=.reload devtools. reply=.devtools is built into the engine assembly: OK 2026-09-29T19:29:13.140Z [INFO] command source=file:cmd_20260929192913051.cmd line="reload devtools" reply="devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)"
- hotreload on => hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=54 limit_mb=32
- expect reply=.hot reload on: OK 2026-09-29T19:29:13.672Z [INFO] command source=file:cmd_20260929192913447.cmd line="hotreload on" reply="hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=54 limit_mb=32"
- hotreload off => hot reload off: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=54 limit_mb=32
- clear => cleared 0

## Errors

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T19:28:46.889Z [INFO] command source=file:cmd_20260929192846694.cmd line="god on" reply="invincible True"
    2026-09-29T19:28:47.136Z [INFO] command source=file:cmd_20260929192847109.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T19:28:47.644Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T19:28:47.648Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=3
    2026-09-29T19:28:47.649Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T19:28:47.656Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=6
    2026-09-29T19:28:47.657Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T19:28:51.052Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=3394
    2026-09-29T19:28:51.053Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T19:28:51.054Z [INFO] command source=file:cmd_20260929192847511.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T19:28:53.349Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T19:28:55.048Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:28:55.708Z [INFO] command source=file:cmd_20260929192855469.cmd line="spawnprop lf_test_crate 2.5 0" reply="spawning prop lf_test_crate"
    2026-09-29T19:28:56.860Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=16132 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T19:28:57.311Z [INFO] [autopilot] autopilot_prop handle=16644 model=lf_test_crate at=(-64.428, 677.333, 13.568)
    2026-09-29T19:28:57.497Z [INFO] engine_module_stopped devtools released=0 reason=restarting
    2026-09-29T19:28:57.500Z [INFO] devtools_started
    2026-09-29T19:28:57.502Z [INFO] engine_module_started devtools
    2026-09-29T19:28:57.503Z [INFO] engine_module_restarted devtools
    2026-09-29T19:28:57.503Z [INFO] command source=file:cmd_20260929192857431.cmd line="restart devtools" reply="restarted devtools"
    2026-09-29T19:28:57.999Z [INFO] engine_module_stopped autopilot released=2 reason=reloading
    2026-09-29T19:28:58.004Z [INFO] [autopilot] autopilot_ready sdk=1.1.0 engine=1.1.0 episode=GTAIV
    2026-09-29T19:28:58.005Z [INFO] engine_module_started autopilot
    2026-09-29T19:28:58.006Z [INFO] engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1 leaked_kb=54
    2026-09-29T19:28:58.006Z [INFO] command source=file:cmd_20260929192857816.cmd line="reload autopilot" reply="reloaded Liberty.Autopilot.dll: autopilot"
    2026-09-29T19:28:58.243Z [INFO] command source=file:cmd_20260929192858200.cmd line="owned autopilot" reply="autopilot: nothing"
    2026-09-29T19:28:58.799Z [INFO] command source=file:cmd_20260929192858584.cmd line="god on" reply="invincible True"
    2026-09-29T19:28:59.046Z [INFO] command source=file:cmd_20260929192858975.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T19:28:59.564Z [INFO] command source=file:cmd_20260929192859354.cmd line="modules" reply="gunplay avg=1.184 max=176.37 int=0 | combat avg=0.109 max=147.60 int=0 | arsenal avg=0.555 max=105.65 int=30 | holsters avg=0.067 max=69.98 int=50 | atmosphere avg=0.225 max=8.13 int=0 | devtools avg=0.022 max=0.04 int=30 | probe avg=1.428 max=1.44 int=10000 | weapon-probe avg=0.000 max=0.01 int=0 | world avg=0.239 max=2.79 int=500 | autopilot avg=0.000 max=0.00 int=0"
    2026-09-29T19:28:59.826Z [INFO] command source=file:cmd_20260929192859734.cmd line="selftest" reply="selftest started"
    2026-09-29T19:28:59.879Z [INFO] [autopilot] selftest_begin engine=1.1.0 sdk=1.1.0
    2026-09-29T19:28:59.880Z [INFO] [autopilot] selftest world ok from_core=True peds=13 vehicles=21
    2026-09-29T19:28:59.881Z [INFO] [autopilot] selftest player ok ped=2 index=0
    2026-09-29T19:28:59.883Z [INFO] [autopilot] selftest modules ok loaded=10
    2026-09-29T19:28:59.884Z [INFO] [autopilot] selftest perf ok frame_ms=41.9 free_mb=1557 pressure=0.40
    2026-09-29T19:28:59.885Z [INFO] [autopilot] selftest input info pad=False
    2026-09-29T19:28:59.885Z [INFO] [autopilot] selftest episode info GTAIV
    2026-09-29T19:28:59.887Z [INFO] [autopilot] selftest query-ground ok ground=13.56 water=False 0.00
    2026-09-29T19:28:59.888Z [INFO] [autopilot] selftest raycast-available ok
    2026-09-29T19:28:59.892Z [INFO] [autopilot] selftest raycast-ground ok Hit World at (-64.428, 674.833, 13.562) distance 1.51 normal=(-0.054, -0.002, 0.999) ground_z=13.56
    2026-09-29T19:28:59.893Z [INFO] [autopilot] selftest raycast-clear ok Clear
    2026-09-29T19:28:59.894Z [INFO] [autopilot] selftest raycast-invalid ok from == to is refused
    2026-09-29T19:28:59.895Z [INFO] [autopilot] selftest capability-memory ok IMemory refused without memory.patch
    2026-09-29T19:28:59.898Z [INFO] [autopilot] selftest capability-natives ok INatives refused without engine.internal
    2026-09-29T19:28:59.900Z [INFO] [autopilot] selftest config ok <game>\scripts\LibertyFramework\config\autopilot\selftest.json
    2026-09-29T19:28:59.911Z [INFO] [autopilot] selftest state ok
    2026-09-29T19:28:59.913Z [INFO] [autopilot] selftest weapon-model ok model=0xF44C839D slot=2
    2026-09-29T19:28:59.916Z [INFO] [autopilot] selftest weapons ok inventory=6
    2026-09-29T19:28:59.918Z [INFO] [autopilot] selftest prop-create ok handle=35075
    2026-09-29T19:28:59.919Z [INFO] [autopilot] selftest prop-position ok at=(-64.428, 676.333, 15.567)
    2026-09-29T19:28:59.921Z [INFO] weapon_changed from=12 to=7 profile=vanilla
    2026-09-29T19:29:00.063Z [INFO] [autopilot] selftest attach-rotation ok heading_delta=90.0 (90 requested; the game itself takes radians)
    2026-09-29T19:29:00.064Z [INFO] [autopilot] selftest attach-rotation-units info degrees heading_delta=90.0 (90 requested)
    2026-09-29T19:29:00.066Z [INFO] [autopilot] selftest prop-delete ok
    2026-09-29T19:29:00.102Z [INFO] [autopilot] selftest ped-spawn ok handle=8450
    2026-09-29T19:29:00.103Z [INFO] [autopilot] selftest ped-health ok health=150
    2026-09-29T19:29:00.342Z [INFO] [autopilot] selftest ped-bone ok head=(-64.415, 678.118, 15.971) origin=(-64.428, 678.333, 15.319)
    2026-09-29T19:29:00.344Z [INFO] [autopilot] selftest ped-weapon ok
    2026-09-29T19:29:00.481Z [INFO] [autopilot] selftest ped-snapshot ok in_snapshot distance=3.5
    2026-09-29T19:29:00.491Z [INFO] [autopilot] selftest query-radius ok peds_within_10m=1
    2026-09-29T19:29:00.492Z [INFO] [autopilot] selftest query-cone ok found=8450
    2026-09-29T19:29:00.495Z [INFO] [autopilot] selftest query-onscreen ok on_screen=True
    2026-09-29T19:29:00.496Z [INFO] [autopilot] selftest raycast-ped ok Hit Ped 8450 at (-64.422, 678.07, 15.404) distance 1.34
    2026-09-29T19:29:00.496Z [INFO] [autopilot] selftest raycast-pass-through ok Clear passed=1 tests=2
    2026-09-29T19:29:00.497Z [INFO] [autopilot] selftest raycast-ignore ok Clear
    2026-09-29T19:29:00.501Z [INFO] [autopilot] selftest line-of-sight-ped ok
    2026-09-29T19:29:01.014Z [INFO] [autopilot] selftest ped-delete ok
    2026-09-29T19:29:01.150Z [INFO] [autopilot] selftest vehicle-spawn ok handle=8706
    2026-09-29T19:29:01.151Z [INFO] [autopilot] selftest vehicle-engine ok engine=1000 body=1000
    2026-09-29T19:29:01.153Z [INFO] [autopilot] selftest vehicle-offset ok nose=(-70.424, 676.834, 14.237)
    2026-09-29T19:29:01.327Z [INFO] [autopilot] selftest raycast-vehicle ok Hit Vehicle 8706 at (-71.485, 674.824, 14.138) distance 1.49 normal=(-0.935, -0.002, 0.354)
    2026-09-29T19:29:01.328Z [INFO] [autopilot] selftest line-of-sight-vehicle ok blocked=True world_only=Clear passed=1
    2026-09-29T19:29:01.974Z [INFO] [autopilot] selftest vehicle-snapshot ok listed=True appeared_event=True
    2026-09-29T19:29:04.415Z [INFO] density frame_ms=40.9 peds=0.64 cars=0.68
    2026-09-29T19:29:04.752Z [INFO] [autopilot] selftest vehicle-driver ok ped=6147 snapshot_driver=6147 get_driver=6147
    2026-09-29T19:29:04.756Z [INFO] [autopilot] selftest vehicle-delete ok
    2026-09-29T19:29:04.805Z [INFO] [autopilot] selftest vehicle-removed-event ok
    2026-09-29T19:29:04.808Z [INFO] [autopilot] selftest fx-burst info blood_gun_entry=True
    2026-09-29T19:29:04.810Z [INFO] [autopilot] selftest audio-id ok sound=2
    2026-09-29T19:29:04.815Z [INFO] [autopilot] selftest blip ok blip=2228243
    2026-09-29T19:29:05.060Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:29:05.060Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1402 core=on peds=5 vehicles=21 modules=10/10 coroutines=1 resources=3 raycast=on episode=GTAIV frame_ms=38.61 p95_ms=61.40 pressure=0.23 private_mb=1862 working_set_mb=1825 address_free_mb=1554 largest_free_block_mb=1532 managed_mb=14 physical_load=94% core_us=55.5
    2026-09-29T19:29:05.140Z [INFO] [autopilot] selftest camera-create ok
    2026-09-29T19:29:05.143Z [INFO] [autopilot] selftest game-camera ok fov=45.0 position=(-66.244, 678.261, 17.222)
    2026-09-29T19:29:05.436Z [INFO] performance samples=1405 frame_p50_ms=9 frame_p95_ms=47 frame_p99_ms=81 frames_over_33ms=201 frames_over_50ms=51 gunplay_avg_ms=1.118 gunplay_max_ms=2.550 phase_samples=1406 phase_setup_avg_ms=0.814 phase_setup_max_ms=142.245 phase_camera_avg_ms=0.026 phase_camera_max_ms=15.415 phase_bullets_avg_ms=0.007 phase_bullets_max_ms=5.387 phase_weapon_avg_ms=0.020 phase_weapon_max_ms=3.484 phase_hud_avg_ms=0.373 phase_hud_max_ms=4.184
    2026-09-29T19:29:05.453Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=5.022/3424.3/1405@7 total=7055 module.gunplay=1.245/176.4/1405@7 total=1750 tick.gunplay=1.244/176.1/1405@7 total=1748 gp.freeaim=0.648/45.3/1406@7 total=912 module.arsenal=0.798/105.6/600@7 total=479 tick.arsenal=0.798/105.6/600@7 total=479 module.combat=0.146/147.6/1405@7 total=205 module.atmosphere=0.145/8.1/1405@7 total=204 tick.combat=0.145/147.5/1405@7 total=204 tick.atmosphere=0.144/7.5/1405@7 total=202 module.holsters=0.575/70.0/315@7 total=181 tick.holsters=0.574/70.0/315@7 total=181 ar.storage=0.294/2.5/600@7 total=176 engine.scheduler=0.118/54.5/1406@7 total=166 ho.show=0.498/69.9/314@7 total=156 ar.reconcile=0.244/81.1/600@7 total=146 engine.world=0.092/28.5/1406@7 total=129 gp.player=0.070/85.6/1406@7 total=98 ar.safehouse_flags=0.142/2.8/600@7 total=85 gp.index_pad=0.052/0.6/1406@7 total=73 combat.sample=0.608/1.8/55@7 total=33 gp.shoulder=0.016/1.6/1406@7 total=23 module.devtools=0.030/3.8/600@7 total=18 tick.devtools=0.030/3.8/600@7 total=18 cam.find_active=0.012/13.3/1406@7 total=16 gp.weapon_id=0.011/1.9/1406@7 total=16 cam.handle=0.011/1.9/1406@7 total=15 gp.cycle=0.008/1.4/1406@7 total=11 gp.state=0.006/0.9/1406@7 total=8 module.world=0.126/2.8/48@7 total=6 gp.shots=0.004/4.0/1406@7 total=6 module.probe=1.320/1.4/4@7 total=5 ar.discover=0.005/2.4/600@7 total=3 ar.vehicle=0.005/1.4/600@7 total=3 gp.spread=0.002/0.8/1406@7 total=3 cam.aim_key=0.002/0.0/1406@7 total=3 combat.dismember=0.002/2.0/1405@7 total=3 ar.lvs=0.003/0.5/600@7 total=2 engine.raycast=0.124/0.8/10@7 total=1 gp.feel=0.001/0.7/1406@7 total=1 combat.blood=0.001/0.6/1405@7 total=1 combat.pending=0.000/0.4/1405@7 total=1 ho.carried=0.002/0.1/314@7 total=1 gp.recoil=0.000/0.3/1406@7 total=0 cam.fov=0.004/0.2/89@7 total=0 module.autopilot=0.000/0.0/1405@7 total=0 module.weapon-probe=0.000/0.0/1405@7 total=0
    2026-09-29T19:29:05.454Z [INFO] engine_thread_probe ticks=1404 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1402 ticks_after_skipped_frames=1
    2026-09-29T19:29:05.456Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=11.63 shdn_us=56.5
    2026-09-29T19:29:05.966Z [INFO] camera_already_gone handle=3330 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:29:07.591Z [INFO] [autopilot] selftest ui-list ok
    2026-09-29T19:29:07.593Z [INFO] [autopilot] selftest ui-list-close ok
    2026-09-29T19:29:10.626Z [INFO] [autopilot] selftest ui-radial ok
    2026-09-29T19:29:10.636Z [INFO] [autopilot] choreography_begin selftest steps=2
    2026-09-29T19:29:10.685Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T19:29:10.704Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T19:29:12.835Z [INFO] [autopilot] choreography_complete selftest
    2026-09-29T19:29:12.870Z [INFO] [autopilot] selftest choreography ok step=1
    2026-09-29T19:29:12.873Z [INFO] [autopilot] selftest_done passed=49 failed=0
    2026-09-29T19:29:13.140Z [INFO] command source=file:cmd_20260929192913051.cmd line="reload devtools" reply="devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)"
    2026-09-29T19:29:13.672Z [INFO] command source=file:cmd_20260929192913447.cmd line="hotreload on" reply="hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=54 limit_mb=32"
    2026-09-29T19:29:13.921Z [INFO] command source=file:cmd_20260929192913841.cmd line="hotreload off" reply="hot reload off: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=54 limit_mb=32"
    2026-09-29T19:29:14.465Z [INFO] command source=file:cmd_20260929192914238.cmd line="clear" reply="cleared 0"

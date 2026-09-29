# Scenario hot-reload

- Result: PASS
- Steps: 24, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- launch: engine booted on attempt 2
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- spawnprop lf_test_crate 2.5 0 => spawning prop lf_test_crate
- expect autopilot_prop handle=\d+ model=lf_test_crate: OK 2026-09-29T22:05:42.133Z [INFO] [autopilot] autopilot_prop handle=17156 model=lf_test_crate at=(-64.423, 677.334, 13.568)
- restart devtools => restarted devtools
- expect engine_module_restarted devtools: OK 2026-09-29T22:05:42.323Z [INFO] engine_module_restarted devtools
- reload autopilot => reloaded Liberty.Autopilot.dll: autopilot
- expect engine_module_stopped autopilot released=[1-9]: OK 2026-09-29T22:05:42.825Z [INFO] engine_module_stopped autopilot released=2 reason=reloading
- expect engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1: OK 2026-09-29T22:05:42.833Z [INFO] engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1 leaked_kb=56
- owned autopilot => autopilot: nothing
- expect line=.owned autopilot. reply=.autopilot: nothing: OK 2026-09-29T22:05:43.085Z [INFO] command source=file:cmd_20260929220542981.cmd line="owned autopilot" reply="autopilot: nothing"
- god on => invincible True
- wanted 0 => wanted 0
- modules => gunplay avg=0.815 max=176.85 int=0 | combat avg=0.098 max=149.76 int=0 | arsenal avg=0.478 max=110.56 int=30 | holsters avg=0.057 max=69.56 int=50 | atmosphere avg=0.165 max=8.22 int=0 | devtools avg=0.021 max=0.03 int=30 | probe avg=1.385 max=1.40 int=10000 | weapon-probe avg=0.000 max=0.00 int=0 | world avg=0.261 max=2.76 int=500 | autopilot avg=0.000 max=0.00 int=0
- selftest => selftest started
- expect selftest_done passed=\d+ failed=0: OK 2026-09-29T22:05:56.451Z [INFO] [autopilot] selftest_done passed=49 failed=0
- reload devtools => devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)
- expect line=.reload devtools. reply=.devtools is built into the engine assembly: OK 2026-09-29T22:05:56.875Z [INFO] command source=file:cmd_20260929220556820.cmd line="reload devtools" reply="devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)"
- hotreload on => hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=56 limit_mb=32
- expect reply=.hot reload on: OK 2026-09-29T22:05:57.367Z [INFO] command source=file:cmd_20260929220557212.cmd line="hotreload on" reply="hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=56 limit_mb=32"
- hotreload off => hot reload off: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=56 limit_mb=32
- clear => cleared 0

## Errors

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T22:05:32.627Z [INFO] command source=file:cmd_20260929220532364.cmd line="god on" reply="invincible True"
    2026-09-29T22:05:32.872Z [INFO] command source=file:cmd_20260929220532771.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T22:05:33.380Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T22:05:33.386Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=6
    2026-09-29T22:05:33.387Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T22:05:33.398Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=10
    2026-09-29T22:05:33.399Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T22:05:36.492Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=3092
    2026-09-29T22:05:36.493Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T22:05:36.494Z [INFO] command source=file:cmd_20260929220533154.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T22:05:38.663Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T22:05:39.786Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=1028 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T22:05:40.058Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T22:05:40.898Z [INFO] command source=file:cmd_20260929220540751.cmd line="spawnprop lf_test_crate 2.5 0" reply="spawning prop lf_test_crate"
    2026-09-29T22:05:42.133Z [INFO] [autopilot] autopilot_prop handle=17156 model=lf_test_crate at=(-64.423, 677.334, 13.568)
    2026-09-29T22:05:42.317Z [INFO] engine_module_stopped devtools released=0 reason=restarting
    2026-09-29T22:05:42.320Z [INFO] devtools_started
    2026-09-29T22:05:42.321Z [INFO] engine_module_started devtools
    2026-09-29T22:05:42.323Z [INFO] engine_module_restarted devtools
    2026-09-29T22:05:42.323Z [INFO] command source=file:cmd_20260929220542205.cmd line="restart devtools" reply="restarted devtools"
    2026-09-29T22:05:42.825Z [INFO] engine_module_stopped autopilot released=2 reason=reloading
    2026-09-29T22:05:42.831Z [INFO] [autopilot] autopilot_ready sdk=1.1.0 engine=1.1.0 episode=GTAIV
    2026-09-29T22:05:42.832Z [INFO] engine_module_started autopilot
    2026-09-29T22:05:42.833Z [INFO] engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1 leaked_kb=56
    2026-09-29T22:05:42.833Z [INFO] command source=file:cmd_20260929220542600.cmd line="reload autopilot" reply="reloaded Liberty.Autopilot.dll: autopilot"
    2026-09-29T22:05:43.085Z [INFO] command source=file:cmd_20260929220542981.cmd line="owned autopilot" reply="autopilot: nothing"
    2026-09-29T22:05:43.627Z [INFO] command source=file:cmd_20260929220543374.cmd line="god on" reply="invincible True"
    2026-09-29T22:05:43.897Z [INFO] command source=file:cmd_20260929220543753.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T22:05:44.165Z [INFO] command source=file:cmd_20260929220544146.cmd line="modules" reply="gunplay avg=0.815 max=176.85 int=0 | combat avg=0.098 max=149.76 int=0 | arsenal avg=0.478 max=110.56 int=30 | holsters avg=0.057 max=69.56 int=50 | atmosphere avg=0.165 max=8.22 int=0 | devtools avg=0.021 max=0.03 int=30 | probe avg=1.385 max=1.40 int=10000 | weapon-probe avg=0.000 max=0.00 int=0 | world avg=0.261 max=2.76 int=500 | autopilot avg=0.000 max=0.00 int=0"
    2026-09-29T22:05:44.696Z [INFO] command source=file:cmd_20260929220544530.cmd line="selftest" reply="selftest started"
    2026-09-29T22:05:44.740Z [INFO] [autopilot] selftest_begin engine=1.1.0 sdk=1.1.0
    2026-09-29T22:05:44.742Z [INFO] [autopilot] selftest world ok from_core=True peds=10 vehicles=18
    2026-09-29T22:05:44.743Z [INFO] [autopilot] selftest player ok ped=2 index=0
    2026-09-29T22:05:44.745Z [INFO] [autopilot] selftest modules ok loaded=10
    2026-09-29T22:05:44.746Z [INFO] [autopilot] selftest perf ok frame_ms=36.3 free_mb=1524 pressure=0.18
    2026-09-29T22:05:44.746Z [INFO] [autopilot] selftest input info pad=False
    2026-09-29T22:05:44.747Z [INFO] [autopilot] selftest episode info GTAIV
    2026-09-29T22:05:44.749Z [INFO] [autopilot] selftest query-ground ok ground=13.56 water=False 0.00
    2026-09-29T22:05:44.750Z [INFO] [autopilot] selftest raycast-available ok
    2026-09-29T22:05:44.754Z [INFO] [autopilot] selftest raycast-ground ok Hit World at (-64.423, 674.834, 13.562) distance 1.51 normal=(-0.054, -0.002, 0.999) ground_z=13.56
    2026-09-29T22:05:44.755Z [INFO] [autopilot] selftest raycast-clear ok Clear
    2026-09-29T22:05:44.756Z [INFO] [autopilot] selftest raycast-invalid ok from == to is refused
    2026-09-29T22:05:44.757Z [INFO] [autopilot] selftest capability-memory ok IMemory refused without memory.patch
    2026-09-29T22:05:44.760Z [INFO] [autopilot] selftest capability-natives ok INatives refused without engine.internal
    2026-09-29T22:05:44.762Z [INFO] [autopilot] selftest config ok <game>\scripts\LibertyFramework\config\autopilot\selftest.json
    2026-09-29T22:05:44.778Z [INFO] [autopilot] selftest state ok
    2026-09-29T22:05:44.780Z [INFO] [autopilot] selftest weapon-model ok model=0xF44C839D slot=2
    2026-09-29T22:05:44.784Z [INFO] [autopilot] selftest weapons ok inventory=6
    2026-09-29T22:05:44.786Z [INFO] [autopilot] selftest prop-create ok handle=16645
    2026-09-29T22:05:44.787Z [INFO] [autopilot] selftest prop-position ok at=(-64.423, 676.334, 15.568)
    2026-09-29T22:05:44.789Z [INFO] weapon_changed from=12 to=7 profile=vanilla
    2026-09-29T22:05:44.955Z [INFO] [autopilot] selftest attach-rotation ok heading_delta=90.0 (90 requested; the game itself takes radians)
    2026-09-29T22:05:44.956Z [INFO] [autopilot] selftest attach-rotation-units info degrees heading_delta=90.0 (90 requested)
    2026-09-29T22:05:44.959Z [INFO] [autopilot] selftest prop-delete ok
    2026-09-29T22:05:44.987Z [INFO] [autopilot] selftest ped-spawn ok handle=1027
    2026-09-29T22:05:44.989Z [INFO] [autopilot] selftest ped-health ok health=150
    2026-09-29T22:05:45.153Z [INFO] [autopilot] selftest ped-bone ok head=(-64.45, 678.335, 16.094) origin=(-64.423, 678.334, 15.451)
    2026-09-29T22:05:45.154Z [INFO] [autopilot] selftest ped-weapon ok
    2026-09-29T22:05:45.240Z [INFO] [autopilot] selftest ped-snapshot ok in_snapshot distance=3.6
    2026-09-29T22:05:45.248Z [INFO] [autopilot] selftest query-radius ok peds_within_10m=1
    2026-09-29T22:05:45.250Z [INFO] [autopilot] selftest query-cone ok found=1027
    2026-09-29T22:05:45.252Z [INFO] [autopilot] selftest query-onscreen ok on_screen=True
    2026-09-29T22:05:45.253Z [INFO] [autopilot] selftest raycast-ped ok Hit Ped 1027 at (-64.436, 678.084, 15.605) distance 1.24
    2026-09-29T22:05:45.254Z [INFO] [autopilot] selftest raycast-pass-through ok Clear passed=1 tests=2
    2026-09-29T22:05:45.255Z [INFO] [autopilot] selftest raycast-ignore ok Clear
    2026-09-29T22:05:45.257Z [INFO] [autopilot] selftest line-of-sight-ped ok
    2026-09-29T22:05:45.757Z [INFO] [autopilot] selftest ped-delete ok
    2026-09-29T22:05:45.878Z [INFO] [autopilot] selftest vehicle-spawn ok handle=7938
    2026-09-29T22:05:45.880Z [INFO] [autopilot] selftest vehicle-engine ok engine=1000 body=1000
    2026-09-29T22:05:45.882Z [INFO] [autopilot] selftest vehicle-offset ok nose=(-70.42, 676.834, 14.239)
    2026-09-29T22:05:46.046Z [INFO] [autopilot] selftest raycast-vehicle ok Hit Vehicle 7938 at (-71.475, 674.825, 14.153) distance 1.49 normal=(-0.933, -0.002, 0.36)
    2026-09-29T22:05:46.047Z [INFO] [autopilot] selftest line-of-sight-vehicle ok blocked=True world_only=Clear passed=1
    2026-09-29T22:05:46.685Z [INFO] [autopilot] selftest vehicle-snapshot ok listed=True appeared_event=True
    2026-09-29T22:05:49.276Z [INFO] [autopilot] selftest vehicle-driver ok ped=1796 snapshot_driver=1796 get_driver=1796
    2026-09-29T22:05:49.279Z [INFO] [autopilot] selftest vehicle-delete ok
    2026-09-29T22:05:49.372Z [INFO] [autopilot] selftest vehicle-removed-event ok
    2026-09-29T22:05:49.373Z [INFO] [autopilot] selftest fx-burst info blood_gun_entry=True
    2026-09-29T22:05:49.375Z [INFO] [autopilot] selftest audio-id ok sound=2
    2026-09-29T22:05:49.378Z [INFO] [autopilot] selftest blip ok blip=1966096
    2026-09-29T22:05:49.541Z [INFO] density frame_ms=44.0 peds=0.61 cars=0.65
    2026-09-29T22:05:49.704Z [INFO] [autopilot] selftest camera-create ok
    2026-09-29T22:05:49.705Z [INFO] [autopilot] selftest game-camera ok fov=45.0 position=(-66.243, 678.259, 17.222)
    2026-09-29T22:05:50.098Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T22:05:50.099Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1470 core=on peds=7 vehicles=17 modules=10/10 coroutines=1 resources=3 raycast=on episode=GTAIV frame_ms=59.49 p95_ms=51.32 pressure=0.41 private_mb=1879 working_set_mb=1823 address_free_mb=1537 largest_free_block_mb=1489 managed_mb=15 physical_load=94% core_us=61.8
    2026-09-29T22:05:50.316Z [INFO] performance samples=1470 frame_p50_ms=9 frame_p95_ms=43 frame_p99_ms=96 frames_over_33ms=171 frames_over_50ms=40 gunplay_avg_ms=0.752 gunplay_max_ms=3.280 phase_samples=1471 phase_setup_avg_ms=0.598 phase_setup_max_ms=139.776 phase_camera_avg_ms=0.026 phase_camera_max_ms=18.283 phase_bullets_avg_ms=0.007 phase_bullets_max_ms=6.364 phase_weapon_avg_ms=0.020 phase_weapon_max_ms=3.313 phase_hud_avg_ms=0.217 phase_hud_max_ms=3.909
    2026-09-29T22:05:50.333Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.268/3128.5/1470@1 total=6273 module.gunplay=0.874/176.8/1470@1 total=1284 tick.gunplay=0.872/176.6/1470@1 total=1282 gp.freeaim=0.423/43.0/1471@1 total=622 module.arsenal=0.744/110.6/622@1 total=462 tick.arsenal=0.743/110.5/622@1 total=462 module.combat=0.142/149.8/1470@1 total=209 tick.combat=0.141/149.7/1470@1 total=208 module.atmosphere=0.134/8.2/1470@1 total=197 tick.atmosphere=0.133/7.6/1470@1 total=195 module.holsters=0.567/69.6/323@1 total=183 tick.holsters=0.566/69.6/323@1 total=183 ho.show=0.493/69.5/322@1 total=159 ar.storage=0.254/2.7/622@1 total=158 engine.scheduler=0.106/61.8/1471@1 total=157 engine.world=0.093/33.8/1471@1 total=137 ar.reconcile=0.221/80.7/622@1 total=137 gp.player=0.067/85.8/1471@1 total=98 gp.index_pad=0.066/0.6/1471@1 total=98 ar.safehouse_flags=0.137/3.5/622@1 total=85 combat.sample=0.629/2.0/55@1 total=35 gp.shoulder=0.016/1.4/1471@1 total=23 module.devtools=0.030/3.8/622@1 total=19 tick.devtools=0.030/3.8/622@1 total=18 cam.find_active=0.012/15.3/1471@1 total=18 cam.handle=0.011/2.7/1471@1 total=16 gp.weapon_id=0.011/1.8/1471@1 total=16 gp.cycle=0.008/1.4/1471@1 total=12 gp.state=0.005/0.9/1471@1 total=8 module.world=0.134/2.8/48@1 total=6 gp.shots=0.004/3.9/1471@1 total=6 module.probe=1.224/1.4/4@1 total=5 ar.discover=0.007/3.2/622@1 total=4 gp.spread=0.002/0.8/1471@1 total=3 cam.aim_key=0.002/0.0/1471@1 total=3 ar.vehicle=0.005/1.4/622@1 total=3 combat.dismember=0.002/1.9/1470@1 total=3 ar.lvs=0.004/0.7/622@1 total=2 engine.raycast=0.139/0.9/10@1 total=1 gp.feel=0.001/0.8/1471@1 total=1 combat.blood=0.001/0.6/1470@1 total=1 ho.carried=0.002/0.1/322@1 total=1 combat.pending=0.000/0.3/1470@1 total=1 gp.recoil=0.000/0.3/1471@1 total=0 module.autopilot=0.000/0.0/1470@1 total=0 cam.fov=0.004/0.2/90@1 total=0 module.weapon-probe=0.000/0.0/1470@1 total=0
    2026-09-29T22:05:50.334Z [INFO] engine_thread_probe ticks=1469 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1467 ticks_after_skipped_frames=1
    2026-09-29T22:05:50.337Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=11.37 shdn_us=55.8
    2026-09-29T22:05:50.528Z [INFO] camera_already_gone handle=3330 Invalid call to an object that doesn't exist anymore!
    2026-09-29T22:05:52.105Z [INFO] [autopilot] selftest ui-list ok
    2026-09-29T22:05:52.106Z [INFO] [autopilot] selftest ui-list-close ok
    2026-09-29T22:05:54.184Z [INFO] [autopilot] selftest ui-radial ok
    2026-09-29T22:05:54.192Z [INFO] [autopilot] choreography_begin selftest steps=2
    2026-09-29T22:05:54.233Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T22:05:54.255Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T22:05:56.415Z [INFO] [autopilot] choreography_complete selftest
    2026-09-29T22:05:56.450Z [INFO] [autopilot] selftest choreography ok step=1
    2026-09-29T22:05:56.451Z [INFO] [autopilot] selftest_done passed=49 failed=0
    2026-09-29T22:05:56.875Z [INFO] command source=file:cmd_20260929220556820.cmd line="reload devtools" reply="devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)"
    2026-09-29T22:05:57.367Z [INFO] command source=file:cmd_20260929220557212.cmd line="hotreload on" reply="hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=56 limit_mb=32"
    2026-09-29T22:05:57.653Z [INFO] command source=file:cmd_20260929220557612.cmd line="hotreload off" reply="hot reload off: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=56 limit_mb=32"
    2026-09-29T22:05:58.184Z [INFO] command source=file:cmd_20260929220557996.cmd line="clear" reply="cleared 0"

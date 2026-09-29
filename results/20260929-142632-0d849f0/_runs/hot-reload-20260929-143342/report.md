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
- expect autopilot_prop handle=\d+ model=lf_test_crate: OK 2026-09-29T21:35:19.065Z [INFO] [autopilot] autopilot_prop handle=33027 model=lf_test_crate at=(-64.425, 677.333, 13.568)
- restart devtools => restarted devtools
- expect engine_module_restarted devtools: OK 2026-09-29T21:35:19.325Z [INFO] engine_module_restarted devtools
- reload autopilot => reloaded Liberty.Autopilot.dll: autopilot
- expect engine_module_stopped autopilot released=[1-9]: OK 2026-09-29T21:35:19.841Z [INFO] engine_module_stopped autopilot released=2 reason=reloading
- expect engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1: OK 2026-09-29T21:35:19.847Z [INFO] engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1 leaked_kb=55
- owned autopilot => autopilot: nothing
- expect line=.owned autopilot. reply=.autopilot: nothing: OK 2026-09-29T21:35:20.098Z [INFO] command source=file:cmd_20260929213520052.cmd line="owned autopilot" reply="autopilot: nothing"
- god on => invincible True
- wanted 0 => wanted 0
- modules => gunplay avg=1.067 max=177.27 int=0 | combat avg=0.130 max=139.71 int=0 | arsenal avg=0.579 max=124.19 int=30 | holsters avg=0.063 max=76.20 int=50 | atmosphere avg=0.185 max=8.71 int=0 | devtools avg=0.023 max=0.04 int=30 | probe avg=1.525 max=1.55 int=10000 | weapon-probe avg=0.000 max=0.00 int=0 | world avg=0.246 max=3.03 int=500 | autopilot avg=0.000 max=0.00 int=0
- selftest => selftest started
- expect selftest_done passed=\d+ failed=0: OK 2026-09-29T21:35:33.584Z [INFO] [autopilot] selftest_done passed=49 failed=0
- reload devtools => devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)
- expect line=.reload devtools. reply=.devtools is built into the engine assembly: OK 2026-09-29T21:35:33.908Z [INFO] command source=file:cmd_20260929213533849.cmd line="reload devtools" reply="devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)"
- hotreload on => hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=55 limit_mb=32
- expect reply=.hot reload on: OK 2026-09-29T21:35:34.417Z [INFO] command source=file:cmd_20260929213534237.cmd line="hotreload on" reply="hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=55 limit_mb=32"
- hotreload off => hot reload off: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=55 limit_mb=32
- clear => cleared 0

## Errors

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T21:35:08.499Z [INFO] command source=file:cmd_20260929213508318.cmd line="god on" reply="invincible True"
    2026-09-29T21:35:08.760Z [INFO] command source=file:cmd_20260929213508747.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T21:35:09.262Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T21:35:09.265Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=1
    2026-09-29T21:35:09.265Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T21:35:09.272Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=6
    2026-09-29T21:35:09.273Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T21:35:12.439Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=3165
    2026-09-29T21:35:12.440Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T21:35:12.441Z [INFO] command source=file:cmd_20260929213509137.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T21:35:15.011Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T21:35:16.647Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:35:16.997Z [INFO] command source=file:cmd_20260929213516734.cmd line="spawnprop lf_test_crate 2.5 0" reply="spawning prop lf_test_crate"
    2026-09-29T21:35:18.465Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=17924 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T21:35:19.065Z [INFO] [autopilot] autopilot_prop handle=33027 model=lf_test_crate at=(-64.425, 677.333, 13.568)
    2026-09-29T21:35:19.319Z [INFO] engine_module_stopped devtools released=0 reason=restarting
    2026-09-29T21:35:19.322Z [INFO] devtools_started
    2026-09-29T21:35:19.324Z [INFO] engine_module_started devtools
    2026-09-29T21:35:19.325Z [INFO] engine_module_restarted devtools
    2026-09-29T21:35:19.326Z [INFO] command source=file:cmd_20260929213519245.cmd line="restart devtools" reply="restarted devtools"
    2026-09-29T21:35:19.841Z [INFO] engine_module_stopped autopilot released=2 reason=reloading
    2026-09-29T21:35:19.846Z [INFO] [autopilot] autopilot_ready sdk=1.1.0 engine=1.1.0 episode=GTAIV
    2026-09-29T21:35:19.846Z [INFO] engine_module_started autopilot
    2026-09-29T21:35:19.847Z [INFO] engine_module_reloaded assembly=Liberty.Autopilot.dll modules=autopilot reloads=1 leaked_kb=55
    2026-09-29T21:35:19.848Z [INFO] command source=file:cmd_20260929213519652.cmd line="reload autopilot" reply="reloaded Liberty.Autopilot.dll: autopilot"
    2026-09-29T21:35:20.098Z [INFO] command source=file:cmd_20260929213520052.cmd line="owned autopilot" reply="autopilot: nothing"
    2026-09-29T21:35:20.624Z [INFO] command source=file:cmd_20260929213520443.cmd line="god on" reply="invincible True"
    2026-09-29T21:35:20.888Z [INFO] command source=file:cmd_20260929213520836.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T21:35:21.390Z [INFO] command source=file:cmd_20260929213521219.cmd line="modules" reply="gunplay avg=1.067 max=177.27 int=0 | combat avg=0.130 max=139.71 int=0 | arsenal avg=0.579 max=124.19 int=30 | holsters avg=0.063 max=76.20 int=50 | atmosphere avg=0.185 max=8.71 int=0 | devtools avg=0.023 max=0.04 int=30 | probe avg=1.525 max=1.55 int=10000 | weapon-probe avg=0.000 max=0.00 int=0 | world avg=0.246 max=3.03 int=500 | autopilot avg=0.000 max=0.00 int=0"
    2026-09-29T21:35:21.645Z [INFO] command source=file:cmd_20260929213521603.cmd line="selftest" reply="selftest started"
    2026-09-29T21:35:21.700Z [INFO] [autopilot] selftest_begin engine=1.1.0 sdk=1.1.0
    2026-09-29T21:35:21.701Z [INFO] [autopilot] selftest world ok from_core=True peds=10 vehicles=17
    2026-09-29T21:35:21.703Z [INFO] [autopilot] selftest player ok ped=2 index=0
    2026-09-29T21:35:21.705Z [INFO] [autopilot] selftest modules ok loaded=10
    2026-09-29T21:35:21.706Z [INFO] [autopilot] selftest perf ok frame_ms=37.1 free_mb=1551 pressure=0.33
    2026-09-29T21:35:21.707Z [INFO] [autopilot] selftest input info pad=False
    2026-09-29T21:35:21.707Z [INFO] [autopilot] selftest episode info GTAIV
    2026-09-29T21:35:21.709Z [INFO] [autopilot] selftest query-ground ok ground=13.56 water=False 0.00
    2026-09-29T21:35:21.710Z [INFO] [autopilot] selftest raycast-available ok
    2026-09-29T21:35:21.715Z [INFO] [autopilot] selftest raycast-ground ok Hit World at (-64.425, 674.833, 13.562) distance 1.51 normal=(-0.054, -0.002, 0.999) ground_z=13.56
    2026-09-29T21:35:21.716Z [INFO] [autopilot] selftest raycast-clear ok Clear
    2026-09-29T21:35:21.717Z [INFO] [autopilot] selftest raycast-invalid ok from == to is refused
    2026-09-29T21:35:21.718Z [INFO] [autopilot] selftest capability-memory ok IMemory refused without memory.patch
    2026-09-29T21:35:21.721Z [INFO] [autopilot] selftest capability-natives ok INatives refused without engine.internal
    2026-09-29T21:35:21.730Z [INFO] [autopilot] selftest config ok <game>\scripts\LibertyFramework\config\autopilot\selftest.json
    2026-09-29T21:35:21.741Z [INFO] [autopilot] selftest state ok
    2026-09-29T21:35:21.743Z [INFO] [autopilot] selftest weapon-model ok model=0xF44C839D slot=2
    2026-09-29T21:35:21.746Z [INFO] [autopilot] selftest weapons ok inventory=6
    2026-09-29T21:35:21.748Z [INFO] [autopilot] selftest prop-create ok handle=34819
    2026-09-29T21:35:21.749Z [INFO] [autopilot] selftest prop-position ok at=(-64.425, 676.333, 15.568)
    2026-09-29T21:35:21.753Z [INFO] weapon_changed from=12 to=7 profile=vanilla
    2026-09-29T21:35:21.896Z [INFO] [autopilot] selftest attach-rotation ok heading_delta=90.0 (90 requested; the game itself takes radians)
    2026-09-29T21:35:21.897Z [INFO] [autopilot] selftest attach-rotation-units info degrees heading_delta=90.0 (90 requested)
    2026-09-29T21:35:21.899Z [INFO] [autopilot] selftest prop-delete ok
    2026-09-29T21:35:21.928Z [INFO] [autopilot] selftest ped-spawn ok handle=1797
    2026-09-29T21:35:21.929Z [INFO] [autopilot] selftest ped-health ok health=150
    2026-09-29T21:35:22.101Z [INFO] [autopilot] selftest ped-bone ok head=(-64.459, 678.327, 16.085) origin=(-64.425, 678.333, 15.439)
    2026-09-29T21:35:22.102Z [INFO] [autopilot] selftest ped-weapon ok
    2026-09-29T21:35:22.192Z [INFO] [autopilot] selftest ped-snapshot ok in_snapshot distance=3.6
    2026-09-29T21:35:22.201Z [INFO] [autopilot] selftest query-radius ok peds_within_10m=1
    2026-09-29T21:35:22.202Z [INFO] [autopilot] selftest query-cone ok found=1797
    2026-09-29T21:35:22.212Z [INFO] [autopilot] selftest query-onscreen ok on_screen=True
    2026-09-29T21:35:22.213Z [INFO] [autopilot] selftest raycast-ped ok Hit Ped 1797 at (-64.441, 678.084, 15.59) distance 1.24
    2026-09-29T21:35:22.214Z [INFO] [autopilot] selftest raycast-pass-through ok Clear passed=1 tests=2
    2026-09-29T21:35:22.214Z [INFO] [autopilot] selftest raycast-ignore ok Clear
    2026-09-29T21:35:22.224Z [INFO] [autopilot] selftest line-of-sight-ped ok
    2026-09-29T21:35:22.751Z [INFO] [autopilot] selftest ped-delete ok
    2026-09-29T21:35:22.869Z [INFO] [autopilot] selftest vehicle-spawn ok handle=7938
    2026-09-29T21:35:22.871Z [INFO] [autopilot] selftest vehicle-engine ok engine=1000 body=1000
    2026-09-29T21:35:22.873Z [INFO] [autopilot] selftest vehicle-offset ok nose=(-70.423, 676.834, 14.241)
    2026-09-29T21:35:23.113Z [INFO] [autopilot] selftest raycast-vehicle ok Hit Vehicle 7938 at (-71.496, 674.82, 14.128) distance 1.49 normal=(-0.942, -0.004, 0.336)
    2026-09-29T21:35:23.115Z [INFO] [autopilot] selftest line-of-sight-vehicle ok blocked=True world_only=Clear passed=1
    2026-09-29T21:35:23.792Z [INFO] [autopilot] selftest vehicle-snapshot ok listed=True appeared_event=True
    2026-09-29T21:35:25.984Z [INFO] density frame_ms=41.3 peds=0.63 cars=0.67
    2026-09-29T21:35:26.365Z [INFO] [autopilot] selftest vehicle-driver ok ped=1798 snapshot_driver=1798 get_driver=1798
    2026-09-29T21:35:26.367Z [INFO] [autopilot] selftest vehicle-delete ok
    2026-09-29T21:35:26.397Z [INFO] [autopilot] selftest vehicle-removed-event ok
    2026-09-29T21:35:26.398Z [INFO] [autopilot] selftest fx-burst info blood_gun_entry=True
    2026-09-29T21:35:26.400Z [INFO] [autopilot] selftest audio-id ok sound=2
    2026-09-29T21:35:26.402Z [INFO] [autopilot] selftest blip ok blip=2031634
    2026-09-29T21:35:26.592Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1384 core=on peds=7 vehicles=15 modules=10/10 coroutines=1 resources=3 raycast=on episode=GTAIV frame_ms=33.63 p95_ms=57.10 pressure=0.15 private_mb=1896 working_set_mb=1252 address_free_mb=1542 largest_free_block_mb=1511 managed_mb=15 physical_load=95% core_us=49.3
    2026-09-29T21:35:26.658Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:35:26.727Z [INFO] [autopilot] selftest camera-create ok
    2026-09-29T21:35:26.730Z [INFO] [autopilot] selftest game-camera ok fov=45.0 position=(-66.249, 678.257, 17.222)
    2026-09-29T21:35:26.781Z [INFO] performance samples=1388 frame_p50_ms=9 frame_p95_ms=41 frame_p99_ms=81 frames_over_33ms=178 frames_over_50ms=39 gunplay_avg_ms=1.027 gunplay_max_ms=2.945 phase_samples=1389 phase_setup_avg_ms=0.720 phase_setup_max_ms=152.874 phase_camera_avg_ms=0.019 phase_camera_max_ms=5.790 phase_bullets_avg_ms=0.008 phase_bullets_max_ms=6.183 phase_weapon_avg_ms=0.020 phase_weapon_max_ms=3.486 phase_hud_avg_ms=0.383 phase_hud_max_ms=3.989
    2026-09-29T21:35:26.797Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.875/3202.8/1388@7 total=6767 module.gunplay=1.156/177.3/1388@7 total=1604 tick.gunplay=1.154/177.1/1388@7 total=1602 gp.freeaim=0.536/45.8/1389@7 total=744 module.arsenal=0.825/124.2/605@7 total=499 tick.arsenal=0.825/124.0/605@7 total=499 module.atmosphere=0.154/8.7/1388@7 total=214 tick.atmosphere=0.153/8.0/1388@7 total=212 module.combat=0.144/139.7/1388@7 total=200 tick.combat=0.143/139.6/1388@7 total=199 module.holsters=0.574/76.2/317@7 total=182 tick.holsters=0.573/76.2/317@7 total=182 engine.scheduler=0.129/67.6/1389@7 total=179 ar.storage=0.293/5.3/605@7 total=177 ho.show=0.507/76.1/316@7 total=160 ar.reconcile=0.253/84.5/605@7 total=153 engine.world=0.094/29.0/1389@7 total=130 gp.player=0.077/94.4/1389@7 total=107 gp.index_pad=0.063/1.6/1389@7 total=88 ar.safehouse_flags=0.142/2.6/605@7 total=86 combat.sample=0.665/2.1/55@7 total=37 module.devtools=0.038/7.7/605@7 total=23 gp.shoulder=0.016/1.5/1389@7 total=23 tick.devtools=0.037/7.7/605@7 total=22 gp.weapon_id=0.011/1.9/1389@7 total=16 cam.handle=0.010/1.3/1389@7 total=14 gp.cycle=0.008/2.0/1389@7 total=12 cam.find_active=0.006/4.9/1389@7 total=8 gp.state=0.006/0.9/1389@7 total=8 gp.shots=0.004/4.1/1389@7 total=6 module.world=0.124/3.0/47@7 total=6 module.probe=1.326/1.6/4@7 total=5 ar.discover=0.007/3.0/605@7 total=4 combat.dismember=0.003/2.8/1388@7 total=4 gp.spread=0.002/0.9/1389@7 total=3 ar.vehicle=0.005/1.4/605@7 total=3 cam.aim_key=0.002/0.0/1389@7 total=3 ar.lvs=0.003/0.6/605@7 total=2 engine.raycast=0.153/1.2/10@7 total=2 combat.blood=0.001/0.8/1388@7 total=1 gp.feel=0.001/0.7/1389@7 total=1 combat.pending=0.001/0.4/1388@7 total=1 ho.carried=0.002/0.1/316@7 total=1 gp.recoil=0.000/0.3/1389@7 total=0 cam.fov=0.004/0.3/88@7 total=0 module.autopilot=0.000/0.0/1388@7 total=0 module.weapon-probe=0.000/0.0/1388@7 total=0
    2026-09-29T21:35:26.798Z [INFO] engine_thread_probe ticks=1387 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1385 ticks_after_skipped_frames=1
    2026-09-29T21:35:26.800Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=11.34 shdn_us=33.0
    2026-09-29T21:35:27.585Z [INFO] camera_already_gone handle=3330 Invalid call to an object that doesn't exist anymore!
    2026-09-29T21:35:28.912Z [INFO] [autopilot] selftest ui-list ok
    2026-09-29T21:35:28.914Z [INFO] [autopilot] selftest ui-list-close ok
    2026-09-29T21:35:30.801Z [INFO] [autopilot] selftest ui-radial ok
    2026-09-29T21:35:30.812Z [INFO] [autopilot] choreography_begin selftest steps=2
    2026-09-29T21:35:30.855Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T21:35:30.876Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T21:35:33.542Z [INFO] [autopilot] choreography_complete selftest
    2026-09-29T21:35:33.583Z [INFO] [autopilot] selftest choreography ok step=1
    2026-09-29T21:35:33.584Z [INFO] [autopilot] selftest_done passed=49 failed=0
    2026-09-29T21:35:33.908Z [INFO] command source=file:cmd_20260929213533849.cmd line="reload devtools" reply="devtools is built into the engine assembly; use restart (or SHDN ReloadScripts for new engine code)"
    2026-09-29T21:35:34.417Z [INFO] command source=file:cmd_20260929213534237.cmd line="hotreload on" reply="hot reload on: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=55 limit_mb=32"
    2026-09-29T21:35:34.691Z [INFO] command source=file:cmd_20260929213534618.cmd line="hotreload off" reply="hot reload off: <game>\scripts\LibertyFramework\mods reloads=1 leaked_kb=55 limit_mb=32"
    2026-09-29T21:35:35.220Z [INFO] command source=file:cmd_20260929213534995.cmd line="clear" reply="cleared 0"

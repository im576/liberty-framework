# Scenario trunk-review

- Result: PASS
- Steps: 35, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 5000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- give 14 120 => gave 14
- spawncar admiral 8 => spawning admiral
- expect autopilot_spawncar handle=: OK 2026-09-29T19:29:54.643Z [INFO] [autopilot] autopilot_spawncar handle=6148 model=admiral
- wait 1500 ms
- at-trunk 3.0 => at trunk of 6148 (-61.464, 682.841, 14.658)
- wait 2000 ms
- key E 1200 ms
- wait 4000 ms
- expect choreography_begin trunk: OK 2026-09-29T19:29:59.512Z [INFO] [arsenal] choreography_begin trunk steps=7
- expect arsenal_storage_open: OK 2026-09-29T19:29:59.513Z [INFO] arsenal_storage_open id=temporary:00001804
- hud off => hud off
- shot trunk_open -> trunk_open.jpg
- key Right 1200 ms
- wait 500 ms
- shot trunk_wheel_next_segment -> trunk_wheel_next_segment.jpg
- key Left 1200 ms
- wait 500 ms
- key Space 1200 ms
- expect arsenal_store id=14 to=: OK 2026-09-29T19:30:17.317Z [INFO] arsenal_store id=14 to=temporary:00001804
- wait 2500 ms
- shot trunk_after_store -> trunk_after_store.jpg
- key Back 1200 ms
- wait 3500 ms
- expect choreography_complete trunk: OK 2026-09-29T19:30:25.897Z [INFO] [arsenal] choreography_complete trunk
- expect arsenal_storage_closed: OK 2026-09-29T19:30:25.903Z [INFO] arsenal_storage_closed id=temporary:00001804
- shot trunk_closed -> trunk_closed.jpg
- hud on => hud on
- clear => cleared 1

## Errors

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T19:29:45.289Z [INFO] [autopilot] event PedAppeared handle=5895
    2026-09-29T19:29:45.478Z [INFO] [autopilot] event PedAppeared handle=6151
    2026-09-29T19:29:46.249Z [INFO] command source=file:cmd_20260929192946197.cmd line="events on" reply="event log on"
    2026-09-29T19:29:46.548Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:29:46.754Z [INFO] command source=file:cmd_20260929192946623.cmd line="god on" reply="invincible True"
    2026-09-29T19:29:46.802Z [INFO] [autopilot] event PedAppeared handle=6403
    2026-09-29T19:29:46.840Z [INFO] [autopilot] event PedAppeared handle=6659
    2026-09-29T19:29:47.255Z [INFO] command source=file:cmd_20260929192947006.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T19:29:47.411Z [INFO] [autopilot] event VehicleAppeared handle=4611
    2026-09-29T19:29:47.474Z [INFO] [autopilot] event VehicleRemoved handle=4611
    2026-09-29T19:29:47.500Z [INFO] [autopilot] event VehicleAppeared handle=5124
    2026-09-29T19:29:47.501Z [INFO] [autopilot] event VehicleAppeared handle=5636
    2026-09-29T19:29:47.506Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T19:29:47.508Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=0
    2026-09-29T19:29:47.509Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T19:29:47.517Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=7
    2026-09-29T19:29:47.518Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T19:29:48.169Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=649
    2026-09-29T19:29:48.169Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T19:29:48.170Z [INFO] command source=file:cmd_20260929192947382.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T19:29:48.192Z [INFO] [autopilot] event PedRemoved handle=6151
    2026-09-29T19:29:48.193Z [INFO] [autopilot] event PedRemoved handle=2567
    2026-09-29T19:29:48.194Z [INFO] [autopilot] event PedRemoved handle=6659
    2026-09-29T19:29:48.194Z [INFO] [autopilot] event PedRemoved handle=5123
    2026-09-29T19:29:48.195Z [INFO] [autopilot] event PedRemoved handle=4611
    2026-09-29T19:29:48.196Z [INFO] [autopilot] event PedRemoved handle=4099
    2026-09-29T19:29:48.196Z [INFO] [autopilot] event PedRemoved handle=3587
    2026-09-29T19:29:48.197Z [INFO] [autopilot] event PedRemoved handle=6403
    2026-09-29T19:29:48.198Z [INFO] [autopilot] event PedRemoved handle=5379
    2026-09-29T19:29:48.199Z [INFO] [autopilot] event PedRemoved handle=3843
    2026-09-29T19:29:48.199Z [INFO] [autopilot] event PedRemoved handle=3331
    2026-09-29T19:29:48.200Z [INFO] [autopilot] event PedRemoved handle=5895
    2026-09-29T19:29:48.200Z [INFO] [autopilot] event PedRemoved handle=2823
    2026-09-29T19:29:48.201Z [INFO] [autopilot] event PedRemoved handle=3076
    2026-09-29T19:29:48.202Z [INFO] [autopilot] event PedRemoved handle=2052
    2026-09-29T19:29:48.202Z [INFO] [autopilot] event PedRemoved handle=4356
    2026-09-29T19:29:48.203Z [INFO] [autopilot] event PedRemoved handle=1796
    2026-09-29T19:29:48.204Z [INFO] [autopilot] event PedRemoved handle=5637
    2026-09-29T19:29:48.204Z [INFO] [autopilot] event PedRemoved handle=1541
    2026-09-29T19:29:48.205Z [INFO] [autopilot] event VehicleRemoved handle=2053
    2026-09-29T19:29:48.206Z [INFO] [autopilot] event VehicleRemoved handle=773
    2026-09-29T19:29:48.206Z [INFO] [autopilot] event VehicleRemoved handle=7171
    2026-09-29T19:29:48.207Z [INFO] [autopilot] event VehicleRemoved handle=6403
    2026-09-29T19:29:48.208Z [INFO] [autopilot] event VehicleRemoved handle=3843
    2026-09-29T19:29:48.208Z [INFO] [autopilot] event VehicleRemoved handle=3587
    2026-09-29T19:29:48.209Z [INFO] [autopilot] event VehicleRemoved handle=3331
    2026-09-29T19:29:48.210Z [INFO] [autopilot] event VehicleRemoved handle=3075
    2026-09-29T19:29:48.210Z [INFO] [autopilot] event VehicleRemoved handle=2819
    2026-09-29T19:29:48.211Z [INFO] [autopilot] event VehicleRemoved handle=2563
    2026-09-29T19:29:48.212Z [INFO] [autopilot] event VehicleRemoved handle=5636
    2026-09-29T19:29:48.212Z [INFO] [autopilot] event VehicleRemoved handle=5124
    2026-09-29T19:29:48.213Z [INFO] [autopilot] event VehicleRemoved handle=5380
    2026-09-29T19:29:48.214Z [INFO] [autopilot] event VehicleRemoved handle=2308
    2026-09-29T19:29:48.215Z [INFO] [autopilot] event VehicleRemoved handle=1796
    2026-09-29T19:29:48.215Z [INFO] [autopilot] event VehicleRemoved handle=1540
    2026-09-29T19:29:48.216Z [INFO] [autopilot] event VehicleRemoved handle=1284
    2026-09-29T19:29:48.217Z [INFO] [autopilot] event VehicleRemoved handle=1028
    2026-09-29T19:29:48.237Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=33541 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T19:29:48.306Z [INFO] [autopilot] event VehicleAppeared handle=8707
    2026-09-29T19:29:48.306Z [INFO] [autopilot] event VehicleAppeared handle=8962
    2026-09-29T19:29:48.307Z [INFO] [autopilot] event VehicleAppeared handle=9218
    2026-09-29T19:29:48.308Z [INFO] [autopilot] event VehicleAppeared handle=9474
    2026-09-29T19:29:48.308Z [INFO] [autopilot] event VehicleAppeared handle=9730
    2026-09-29T19:29:48.657Z [INFO] [autopilot] event PedAppeared handle=5124
    2026-09-29T19:29:48.658Z [INFO] [autopilot] event PedAppeared handle=5380
    2026-09-29T19:29:48.658Z [INFO] [autopilot] event PedAppeared handle=5638
    2026-09-29T19:29:48.659Z [INFO] [autopilot] event PedAppeared handle=5896
    2026-09-29T19:29:48.733Z [INFO] [autopilot] event PedAppeared handle=6152
    2026-09-29T19:29:48.753Z [INFO] [autopilot] event PedAppeared handle=6404
    2026-09-29T19:29:48.754Z [INFO] [autopilot] event PedAppeared handle=6660
    2026-09-29T19:29:48.776Z [INFO] [autopilot] event PedAppeared handle=6915
    2026-09-29T19:29:48.846Z [INFO] [autopilot] event PedAppeared handle=7173
    2026-09-29T19:29:49.026Z [INFO] [autopilot] event PedAppeared handle=7429
    2026-09-29T19:29:49.096Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T19:29:49.110Z [INFO] [autopilot] event PedAppeared handle=7685
    2026-09-29T19:29:49.110Z [INFO] [autopilot] event VehicleRemoved handle=8707
    2026-09-29T19:29:49.682Z [INFO] [autopilot] event PedRemoved handle=7173
    2026-09-29T19:29:49.683Z [INFO] [autopilot] event PedRemoved handle=6404
    2026-09-29T19:29:49.712Z [INFO] [autopilot] event PedRemoved handle=6915
    2026-09-29T19:29:49.713Z [INFO] [autopilot] event PedRemoved handle=6660
    2026-09-29T19:29:49.776Z [INFO] [autopilot] event PedRemoved handle=5896
    2026-09-29T19:29:49.777Z [INFO] [autopilot] event PedRemoved handle=5638
    2026-09-29T19:29:49.778Z [INFO] [autopilot] event PedRemoved handle=5380
    2026-09-29T19:29:49.778Z [INFO] [autopilot] event PedRemoved handle=5124
    2026-09-29T19:29:50.146Z [INFO] [autopilot] event PedAppeared handle=5381
    2026-09-29T19:29:50.224Z [INFO] [autopilot] event PedAppeared handle=5639
    2026-09-29T19:29:50.261Z [INFO] [autopilot] event PedAppeared handle=5897
    2026-09-29T19:29:51.481Z [INFO] [autopilot] event PedAppeared handle=6405
    2026-09-29T19:29:52.388Z [INFO] [autopilot] event PedRemoved handle=5897
    2026-09-29T19:29:52.495Z [INFO] [autopilot] event PedRemoved handle=5639
    2026-09-29T19:29:52.516Z [INFO] [autopilot] event PedAppeared handle=5898
    2026-09-29T19:29:52.803Z [INFO] [autopilot] event PedAppeared handle=8707
    2026-09-29T19:29:52.968Z [INFO] [autopilot] event PedAppeared handle=8963
    2026-09-29T19:29:53.115Z [INFO] [autopilot] event PedRemoved handle=5381
    2026-09-29T19:29:53.478Z [INFO] [autopilot] event PedAppeared handle=5640
    2026-09-29T19:29:53.527Z [INFO] command source=file:cmd_20260929192953324.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T19:29:53.784Z [INFO] command source=file:cmd_20260929192953718.cmd line="weather 1" reply="weather 1"
    2026-09-29T19:29:53.888Z [INFO] [autopilot] event PedAppeared handle=9219
    2026-09-29T19:29:54.275Z [INFO] command source=file:cmd_20260929192954109.cmd line="give 14 120" reply="gave 14"
    2026-09-29T19:29:54.300Z [INFO] [autopilot] event PlayerWeaponChanged 0->14
    2026-09-29T19:29:54.301Z [INFO] weapon_changed from=0 to=14 profile=vanilla
    2026-09-29T19:29:54.321Z [INFO] arsenal_gain id=14 owned=False mission=False
    2026-09-29T19:29:54.527Z [INFO] command source=file:cmd_20260929192954501.cmd line="spawncar admiral 8" reply="spawning admiral"
    2026-09-29T19:29:54.643Z [INFO] [autopilot] autopilot_spawncar handle=6148 model=admiral
    2026-09-29T19:29:54.673Z [INFO] [autopilot] event VehicleAppeared handle=6148
    2026-09-29T19:29:55.004Z [INFO] [autopilot] event PedRemoved handle=8963
    2026-09-29T19:29:55.460Z [INFO] [autopilot] event PedRemoved handle=5898
    2026-09-29T19:29:55.618Z [INFO] [autopilot] event PedRemoved handle=8707
    2026-09-29T19:29:55.902Z [INFO] [autopilot] event PedAppeared handle=8708
    2026-09-29T19:29:56.005Z [INFO] [autopilot] event PedAppeared handle=8964
    2026-09-29T19:29:56.177Z [INFO] [autopilot] event PedRemoved handle=5640
    2026-09-29T19:29:56.338Z [INFO] [autopilot] event VehicleAppeared handle=10754
    2026-09-29T19:29:56.830Z [INFO] command source=file:cmd_20260929192956441.cmd line="at-trunk 3.0" reply="at trunk of 6148 (-61.464, 682.841, 14.658)"
    2026-09-29T19:29:56.849Z [INFO] [autopilot] event PedAppeared handle=10243
    2026-09-29T19:29:56.850Z [INFO] [autopilot] event PedRemoved handle=8708
    2026-09-29T19:29:56.859Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:29:57.166Z [INFO] [autopilot] event PedAppeared handle=10499
    2026-09-29T19:29:57.166Z [INFO] [autopilot] event VehicleAppeared handle=1541
    2026-09-29T19:29:57.167Z [INFO] [autopilot] event VehicleAppeared handle=1797
    2026-09-29T19:29:57.168Z [INFO] [autopilot] event VehicleAppeared handle=2054
    2026-09-29T19:29:57.437Z [INFO] [autopilot] event PedRemoved handle=9219
    2026-09-29T19:29:57.955Z [INFO] [autopilot] event PedAppeared handle=4100
    2026-09-29T19:29:57.956Z [INFO] [autopilot] event PedAppeared handle=4357
    2026-09-29T19:29:58.493Z [INFO] [autopilot] event PedRemoved handle=10243
    2026-09-29T19:29:58.589Z [INFO] [autopilot] event PedAppeared handle=10244
    2026-09-29T19:29:59.512Z [INFO] [arsenal] choreography_begin trunk steps=7
    2026-09-29T19:29:59.513Z [INFO] arsenal_storage_open id=temporary:00001804
    2026-09-29T19:29:59.656Z [INFO] [autopilot] event PedRemoved handle=8964
    2026-09-29T19:29:59.728Z [INFO] [autopilot] event PedRemoved handle=10244
    2026-09-29T19:30:00.454Z [INFO] [autopilot] event PedRemoved handle=10499
    2026-09-29T19:30:00.546Z [INFO] [autopilot] event PedAppeared handle=2053
    2026-09-29T19:30:01.357Z [INFO] [autopilot] event PedAppeared handle=2310
    2026-09-29T19:30:04.598Z [INFO] density frame_ms=731.0 peds=0.55 cars=0.60
    2026-09-29T19:30:05.484Z [INFO] command source=file:cmd_20260929193004692.cmd line="hud off" reply="hud off"
    2026-09-29T19:30:07.104Z [INFO] performance samples=1052 frame_p50_ms=18 frame_p95_ms=58 frame_p99_ms=160 frames_over_33ms=90 frames_over_50ms=58 gunplay_avg_ms=1.183 gunplay_max_ms=5.263 phase_samples=1052 phase_setup_avg_ms=0.764 phase_setup_max_ms=4.873 phase_camera_avg_ms=0.022 phase_camera_max_ms=1.508 phase_bullets_avg_ms=0.002 phase_bullets_max_ms=0.015 phase_weapon_avg_ms=0.020 phase_weapon_max_ms=0.605 phase_hud_avg_ms=0.375 phase_hud_max_ms=0.677
    2026-09-29T19:30:07.105Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=3.238/670.4/1052@7 total=3406 module.gunplay=1.192/8.2/1052@7 total=1254 tick.gunplay=1.191/8.2/1052@7 total=1252 gp.freeaim=0.628/4.7/1052@7 total=661 module.arsenal=0.517/15.1/609@7 total=315 tick.arsenal=0.516/15.1/609@7 total=314 module.atmosphere=0.210/2.5/1052@7 total=221 tick.atmosphere=0.208/2.5/1052@7 total=219 engine.world=0.196/26.2/1052@7 total=206 ar.storage=0.287/5.0/609@7 total=175 ar.safehouse_flags=0.170/3.2/609@7 total=103 gp.index_pad=0.075/0.3/1052@7 total=79 engine.scheduler=0.050/2.6/1052@7 total=52 module.combat=0.044/1.6/1052@7 total=47 tick.combat=0.044/1.6/1052@7 total=46 combat.sample=0.902/1.5/24@7 total=22 module.holsters=0.053/1.1/343@7 total=18 tick.holsters=0.052/1.1/343@7 total=18 cam.handle=0.016/1.5/1052@7 total=17 gp.shoulder=0.016/0.6/1052@7 total=17 module.devtools=0.028/3.2/609@7 total=17 tick.devtools=0.028/3.2/609@7 total=17 ar.reconcile=0.027/14.8/609@7 total=16 gp.weapon_id=0.012/0.8/1052@7 total=13 gp.player=0.012/0.1/1052@7 total=12 gp.cycle=0.008/0.1/1052@7 total=8 gp.state=0.007/0.1/1052@7 total=7 module.world=0.103/1.3/53@7 total=5 ar.discover=0.007/3.2/609@7 total=4 module.probe=1.303/1.6/3@7 total=4 gp.shots=0.003/0.0/1052@7 total=3 cam.aim_key=0.003/0.0/1052@7 total=3 cam.find_active=0.002/0.0/1052@7 total=2 gp.spread=0.002/0.1/1052@7 total=2 ar.vehicle=0.003/0.1/609@7 total=2 ar.lvs=0.003/0.3/609@7 total=2 ho.carried=0.003/0.4/234@7 total=1 combat.dismember=0.001/0.0/1052@7 total=1 ho.show=0.002/0.0/234@7 total=0 module.autopilot=0.000/0.0/1052@7 total=0 combat.pending=0.000/0.1/1052@7 total=0 combat.blood=0.000/0.0/1052@7 total=0 gp.feel=0.000/0.0/1052@7 total=0 module.weapon-probe=0.000/0.0/1052@7 total=0 gp.recoil=0.000/0.0/1052@7 total=0 cam.fov=0.001/0.0/96@7 total=0
    2026-09-29T19:30:07.106Z [INFO] engine_thread_probe ticks=1052 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1049 ticks_after_skipped_frames=3
    2026-09-29T19:30:07.108Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.18 shdn_us=53.3
    2026-09-29T19:30:07.112Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:30:07.115Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=2974 core=on peds=9 vehicles=9 modules=10/10 coroutines=1 resources=7 raycast=on episode=GTAIV frame_ms=237.91 p95_ms=90.07 pressure=0.79 private_mb=2201 working_set_mb=1971 address_free_mb=1238 largest_free_block_mb=1213 managed_mb=14 physical_load=95% core_us=50.1
    2026-09-29T19:30:09.623Z [INFO] ui_input menu=radial right
    2026-09-29T19:30:12.125Z [INFO] [autopilot] event PedRemoved handle=2053
    2026-09-29T19:30:14.806Z [INFO] ui_input menu=radial left
    2026-09-29T19:30:17.301Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:30:17.302Z [INFO] ui_input menu=radial x
    2026-09-29T19:30:17.305Z [INFO] holsters_removed reason=store
    2026-09-29T19:30:17.317Z [INFO] arsenal_store id=14 to=temporary:00001804
    2026-09-29T19:30:18.408Z [INFO] [autopilot] event PlayerWeaponChanged 14->0
    2026-09-29T19:30:18.413Z [INFO] weapon_changed from=14 to=0 profile=vanilla
    2026-09-29T19:30:24.517Z [INFO] ui_input menu=radial back
    2026-09-29T19:30:24.689Z [INFO] [autopilot] event PedAppeared handle=2569
    2026-09-29T19:30:25.056Z [INFO] [autopilot] event PedRemoved handle=2310
    2026-09-29T19:30:25.843Z [INFO] [autopilot] event PedAppeared handle=2825
    2026-09-29T19:30:25.897Z [INFO] [arsenal] choreography_complete trunk
    2026-09-29T19:30:25.903Z [INFO] arsenal_storage_closed id=temporary:00001804
    2026-09-29T19:30:27.290Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:30:27.538Z [INFO] [autopilot] event PedAppeared handle=5641
    2026-09-29T19:30:27.882Z [INFO] [autopilot] event VehicleAppeared handle=6660
    2026-09-29T19:30:29.251Z [INFO] [autopilot] event VehicleAppeared handle=10242
    2026-09-29T19:30:29.436Z [INFO] [autopilot] event PedAppeared handle=9475
    2026-09-29T19:30:29.437Z [INFO] [autopilot] event PedAppeared handle=9731
    2026-09-29T19:30:30.110Z [INFO] command source=file:cmd_20260929193030075.cmd line="hud on" reply="hud on"
    2026-09-29T19:30:30.642Z [INFO] command source=file:cmd_20260929193030463.cmd line="clear" reply="cleared 1"
    2026-09-29T19:30:30.676Z [INFO] [autopilot] event VehicleRemoved handle=6148

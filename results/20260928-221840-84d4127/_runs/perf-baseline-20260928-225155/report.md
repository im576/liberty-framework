# Scenario perf-baseline

- Result: NEEDS-REVIEW
- Steps: 10, failed: 0
- Game alive at end: True
- Log errors during run: 1

## Failed steps

## Steps
- events off => event log off
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- time 13 0 => time 13:00
- give 14 120 => gave 14
- wait 35000 ms
- expect performance_scripts: OK 2026-09-29T05:52:21.507Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.435/310.3/1188@7 total=2893 module.gunplay=1.293/4.6/1188@7 total=1536 tick.gunplay=1.292/4.6/1188@7 total=1535 gp.freeaim=0.655/2.4/1188@7 total=778 module.arsenal=0.600/14.5/761@7 total=457 tick.arsenal=0.600/14.5/761@7 total=456 ar.storage=0.390/2.7/761@7 total=296 module.atmosphere=0.178/2.5/1188@7 total=211 tick.atmosphere=0.176/2.5/1188@7 total=210 gp.index_pad=0.138/0.3/1188@7 total=163 engine.world=0.116/5.7/1188@7 total=137 ar.safehouse_flags=0.163/2.7/761@7 total=124 module.combat=0.064/2.4/1188@7 total=76 tick.combat=0.063/2.4/1188@7 total=75 combat.sample=1.052/2.4/46@7 total=48 cam.handle=0.022/1.6/1188@7 total=26 module.devtools=0.028/4.3/761@7 total=21 tick.devtools=0.027/4.3/761@7 total=21 module.holsters=0.051/0.7/402@7 total=20 tick.holsters=0.050/0.7/402@7 total=20 gp.shoulder=0.014/0.2/1188@7 total=17 gp.weapon_id=0.013/0.9/1188@7 total=15 ar.reconcile=0.019/13.3/761@7 total=15 gp.player=0.010/0.1/1188@7 total=12 gp.cycle=0.008/0.1/1188@7 total=10 gp.state=0.006/0.1/1188@7 total=8 module.world=0.123/0.4/57@7 total=7 engine.scheduler=0.005/1.7/1188@7 total=6 module.probe=1.319/1.5/3@7 total=4 cam.aim_key=0.003/0.1/1188@7 total=3 cam.find_active=0.003/0.1/1188@7 total=3 gp.shots=0.002/0.0/1188@7 total=3 gp.spread=0.002/0.0/1188@7 total=2 ar.vehicle=0.003/0.0/761@7 total=2 ar.lvs=0.003/0.4/761@7 total=2 ar.discover=0.002/0.6/761@7 total=2 ho.show=0.002/0.2/402@7 total=1 ho.carried=0.002/0.0/402@7 total=1 combat.dismember=0.001/0.0/1188@7 total=1 module.autopilot=0.000/0.0/1188@7 total=1 combat.blood=0.000/0.0/1188@7 total=0 module.weapon-probe=0.000/0.0/1188@7 total=0 gp.feel=0.000/0.0/1188@7 total=0 combat.pending=0.000/0.0/1188@7 total=0 gp.recoil=0.000/0.0/1188@7 total=0 cam.fov=0.001/0.0/107@7 total=0
- status => engine 1.1.0 sdk 1.1.0 frame=24053 core=on peds=19 vehicles=14 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV

## Errors
    2026-09-29T05:52:34.722Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:51:55.400Z [INFO] [autopilot] event PedAppeared handle=11544
    2026-09-29T05:51:56.036Z [INFO] command source=file:cmd_20260929055155873.cmd line="events off" reply="event log off"
    2026-09-29T05:51:56.547Z [INFO] command source=file:cmd_20260929055156301.cmd line="god on" reply="invincible True"
    2026-09-29T05:51:56.794Z [INFO] command source=file:cmd_20260929055156683.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:51:57.617Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:51:57.617Z [INFO] command source=file:cmd_20260929055157067.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:51:58.535Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:52:00.522Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:52:02.048Z [INFO] command source=file:cmd_20260929055201993.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:52:02.566Z [INFO] command source=file:cmd_20260929055202373.cmd line="give 14 120" reply="gave 14"
    2026-09-29T05:52:02.597Z [INFO] weapon_changed from=0 to=14 profile=vanilla
    2026-09-29T05:52:02.599Z [INFO] arsenal_gain id=14 owned=False mission=False
    2026-09-29T05:52:10.536Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:52:20.538Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:52:21.506Z [INFO] performance samples=1188 frame_p50_ms=19 frame_p95_ms=35 frame_p99_ms=65 frames_over_33ms=90 frames_over_50ms=18 gunplay_avg_ms=1.288 gunplay_max_ms=3.239 phase_samples=1188 phase_setup_avg_ms=0.852 phase_setup_max_ms=2.638 phase_camera_avg_ms=0.028 phase_camera_max_ms=1.567 phase_bullets_avg_ms=0.006 phase_bullets_max_ms=0.156 phase_weapon_avg_ms=0.018 phase_weapon_max_ms=0.163 phase_hud_avg_ms=0.384 phase_hud_max_ms=0.627
    2026-09-29T05:52:21.507Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.435/310.3/1188@7 total=2893 module.gunplay=1.293/4.6/1188@7 total=1536 tick.gunplay=1.292/4.6/1188@7 total=1535 gp.freeaim=0.655/2.4/1188@7 total=778 module.arsenal=0.600/14.5/761@7 total=457 tick.arsenal=0.600/14.5/761@7 total=456 ar.storage=0.390/2.7/761@7 total=296 module.atmosphere=0.178/2.5/1188@7 total=211 tick.atmosphere=0.176/2.5/1188@7 total=210 gp.index_pad=0.138/0.3/1188@7 total=163 engine.world=0.116/5.7/1188@7 total=137 ar.safehouse_flags=0.163/2.7/761@7 total=124 module.combat=0.064/2.4/1188@7 total=76 tick.combat=0.063/2.4/1188@7 total=75 combat.sample=1.052/2.4/46@7 total=48 cam.handle=0.022/1.6/1188@7 total=26 module.devtools=0.028/4.3/761@7 total=21 tick.devtools=0.027/4.3/761@7 total=21 module.holsters=0.051/0.7/402@7 total=20 tick.holsters=0.050/0.7/402@7 total=20 gp.shoulder=0.014/0.2/1188@7 total=17 gp.weapon_id=0.013/0.9/1188@7 total=15 ar.reconcile=0.019/13.3/761@7 total=15 gp.player=0.010/0.1/1188@7 total=12 gp.cycle=0.008/0.1/1188@7 total=10 gp.state=0.006/0.1/1188@7 total=8 module.world=0.123/0.4/57@7 total=7 engine.scheduler=0.005/1.7/1188@7 total=6 module.probe=1.319/1.5/3@7 total=4 cam.aim_key=0.003/0.1/1188@7 total=3 cam.find_active=0.003/0.1/1188@7 total=3 gp.shots=0.002/0.0/1188@7 total=3 gp.spread=0.002/0.0/1188@7 total=2 ar.vehicle=0.003/0.0/761@7 total=2 ar.lvs=0.003/0.4/761@7 total=2 ar.discover=0.002/0.6/761@7 total=2 ho.show=0.002/0.2/402@7 total=1 ho.carried=0.002/0.0/402@7 total=1 combat.dismember=0.001/0.0/1188@7 total=1 module.autopilot=0.000/0.0/1188@7 total=1 combat.blood=0.000/0.0/1188@7 total=0 module.weapon-probe=0.000/0.0/1188@7 total=0 gp.feel=0.000/0.0/1188@7 total=0 combat.pending=0.000/0.0/1188@7 total=0 gp.recoil=0.000/0.0/1188@7 total=0 cam.fov=0.001/0.0/107@7 total=0
    2026-09-29T05:52:21.507Z [INFO] engine_thread_probe ticks=1188 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1187 ticks_after_skipped_frames=1
    2026-09-29T05:52:21.514Z [INFO] direct_native get_char_health direct=180 shdn=80 match=False direct_us=0.21 shdn_us=291.5
    2026-09-29T05:52:21.515Z [INFO] density frame_ms=24.4 peds=1.00 cars=1.00
    2026-09-29T05:52:21.515Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=23457 core=on peds=14 vehicles=14 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV frame_ms=22.12 p95_ms=33.60 pressure=0.00 private_mb=2324 working_set_mb=1881 address_free_mb=1083 largest_free_block_mb=1025 managed_mb=14 physical_load=85% core_us=65.2
    2026-09-29T05:52:30.588Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:52:34.721Z [INFO] holsters_removed reason=wasted
    2026-09-29T05:52:34.722Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored
    2026-09-29T05:52:34.722Z [INFO] arsenal_loss reason=wasted id=14 owned=False
    2026-09-29T05:52:37.916Z [INFO] command source=autopilot line="engine" reply="engine 1.1.0 sdk 1.1.0 frame=24053 core=on peds=19 vehicles=14 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV"
    2026-09-29T05:52:37.916Z [INFO] command source=file:cmd_20260929055237797.cmd line="status" reply="engine 1.1.0 sdk 1.1.0 frame=24053 core=on peds=19 vehicles=14 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV"

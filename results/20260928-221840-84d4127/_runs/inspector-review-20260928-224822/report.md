# Scenario inspector-review

- Result: PASS
- Steps: 13, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- inspector on => inspector on
- expect reply=.inspector on: OK 2026-09-29T05:48:29.319Z [INFO] command source=file:cmd_20260929054829247.cmd line="inspector on" reply="inspector on"
- wait 2000 ms
- shot inspector -> inspector.jpg
- restart devtools => restarted devtools
- wait 1500 ms
- shot inspector_after_restart -> inspector_after_restart.jpg
- inspector off => inspector off
- clear => cleared 0

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:48:23.337Z [INFO] command source=file:cmd_20260929054823267.cmd line="god on" reply="invincible True"
    2026-09-29T05:48:23.862Z [INFO] command source=file:cmd_20260929054823690.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:48:24.105Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:48:24.882Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:48:24.883Z [INFO] command source=file:cmd_20260929054824080.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:48:25.007Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=34837 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T05:48:25.816Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:48:29.319Z [INFO] command source=file:cmd_20260929054829247.cmd line="inspector on" reply="inspector on"
    2026-09-29T05:48:36.973Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:48:41.521Z [INFO] engine_module_stopped devtools released=0 reason=restarting
    2026-09-29T05:48:41.522Z [INFO] devtools_started
    2026-09-29T05:48:41.523Z [INFO] engine_module_started devtools
    2026-09-29T05:48:41.523Z [INFO] engine_module_restarted devtools
    2026-09-29T05:48:41.524Z [INFO] command source=file:cmd_20260929054839492.cmd line="restart devtools" reply="restarted devtools"
    2026-09-29T05:48:48.467Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:48:50.157Z [INFO] performance samples=352 frame_p50_ms=25 frame_p95_ms=45 frame_p99_ms=200 frames_over_33ms=85 frames_over_50ms=13 gunplay_avg_ms=1.236 gunplay_max_ms=3.739 phase_samples=352 phase_setup_avg_ms=0.805 phase_setup_max_ms=3.134 phase_camera_avg_ms=0.030 phase_camera_max_ms=1.496 phase_bullets_avg_ms=0.002 phase_bullets_max_ms=0.005 phase_weapon_avg_ms=0.019 phase_weapon_max_ms=0.126 phase_hud_avg_ms=0.380 phase_hud_max_ms=0.705
    2026-09-29T05:48:50.158Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.581/756.2/352@7 total=1612 module.gunplay=1.247/4.8/352@7 total=439 tick.gunplay=1.245/4.8/352@7 total=438 gp.freeaim=0.608/2.2/352@7 total=214 module.arsenal=0.793/4.1/239@7 total=190 tick.arsenal=0.793/4.1/239@7 total=189 ar.storage=0.561/4.0/239@7 total=134 module.atmosphere=0.239/2.1/352@7 total=84 tick.atmosphere=0.238/2.1/352@7 total=84 ar.safehouse_flags=0.196/1.7/239@7 total=47 gp.index_pad=0.129/0.2/352@7 total=45 engine.world=0.124/0.8/352@7 total=44 module.combat=0.030/0.5/352@7 total=10 tick.combat=0.029/0.5/352@7 total=10 module.devtools=0.042/4.2/239@7 total=10 tick.devtools=0.041/4.2/239@7 total=10 module.holsters=0.065/0.6/136@7 total=9 tick.holsters=0.065/0.6/136@7 total=9 cam.handle=0.024/1.5/352@7 total=9 gp.shoulder=0.015/0.1/352@7 total=5 gp.weapon_id=0.012/0.0/352@7 total=4 gp.player=0.011/0.1/352@7 total=4 module.probe=1.191/1.4/3@7 total=4 gp.cycle=0.009/0.0/352@7 total=3 engine.scheduler=0.007/2.3/352@7 total=3 gp.state=0.006/0.0/352@7 total=2 module.world=0.094/0.4/23@7 total=2 ar.discover=0.007/0.8/239@7 total=2 gp.shots=0.004/0.1/352@7 total=1 ar.lvs=0.004/0.3/239@7 total=1 cam.find_active=0.003/0.0/352@7 total=1 cam.aim_key=0.003/0.0/352@7 total=1 ar.vehicle=0.003/0.0/239@7 total=1 gp.spread=0.002/0.0/352@7 total=1 ar.reconcile=0.002/0.1/239@7 total=1 ho.carried=0.002/0.0/136@7 total=0 combat.dismember=0.001/0.0/352@7 total=0 ho.show=0.002/0.0/136@7 total=0 module.autopilot=0.000/0.0/352@7 total=0 combat.blood=0.000/0.0/352@7 total=0 module.weapon-probe=0.000/0.0/352@7 total=0 gp.feel=0.000/0.0/352@7 total=0 combat.pending=0.000/0.0/352@7 total=0 cam.fov=0.001/0.0/40@7 total=0 gp.recoil=0.000/0.0/352@7 total=0
    2026-09-29T05:48:50.159Z [INFO] engine_thread_probe ticks=352 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=351 ticks_after_skipped_frames=1
    2026-09-29T05:48:50.160Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.25 shdn_us=27.8
    2026-09-29T05:48:50.165Z [INFO] density frame_ms=58.5 peds=0.55 cars=0.60
    2026-09-29T05:48:50.168Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=17773 core=on peds=5 vehicles=2 modules=10/10 coroutines=0 resources=2 raycast=on episode=GTAIV frame_ms=22.61 p95_ms=43.83 pressure=0.14 private_mb=2285 working_set_mb=1821 address_free_mb=1099 largest_free_block_mb=1041 managed_mb=15 physical_load=84% core_us=50.2
    2026-09-29T05:48:52.904Z [INFO] command source=file:cmd_20260929054850784.cmd line="inspector off" reply="inspector off"
    2026-09-29T05:48:53.443Z [INFO] command source=file:cmd_20260929054853257.cmd line="clear" reply="cleared 0"

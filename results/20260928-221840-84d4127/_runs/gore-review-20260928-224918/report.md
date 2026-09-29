# Scenario gore-review

- Result: PASS
- Steps: 39, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto gun_test_range => Teleporting to Gun Test Range (Broker)
- wait 4000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- spawn 1 2.5 => spawning 1 at 2.5 m
- wait 3000 ms
- gore arm => gore arm requested on the nearest NPC
- expect gore_test sever part=: OK 2026-09-29T05:49:31.796Z [INFO] gore_test sever part=left_arm_elbow
- wait 2500 ms
- hud off => hud off
- cam ped 0 20 2.8 1.0 => camera at 20 deg, 2.8 m
- wait 1500 ms
- shot arm_cut -> arm_cut.jpg
- clear => cleared 1
- cam off => camera off
- spawn 1 2.5 => spawning 1 at 2.5 m
- wait 3000 ms
- gore leg => gore leg requested on the nearest NPC
- expect gore_test sever part=: OK 2026-09-29T05:49:42.807Z [INFO] gore_test sever part=right_leg_hip
- wait 2500 ms
- cam ped 0 20 2.8 1.0 => camera at 20 deg, 2.8 m
- wait 1500 ms
- shot leg_cut -> leg_cut.jpg
- clear => cleared 1
- cam off => camera off
- spawn 1 2.5 => spawning 1 at 2.5 m
- wait 3000 ms
- gore head => gore head requested on the nearest NPC
- expect gore_test sever part=: OK 2026-09-29T05:49:53.541Z [INFO] gore_test sever part=head
- wait 2500 ms
- cam ped 0 20 2.8 1.0 => camera at 20 deg, 2.8 m
- wait 1500 ms
- shot head_cut -> head_cut.jpg
- cam off => camera off
- hud on => hud on
- clear => cleared 1

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:49:18.696Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:19.491Z [INFO] command source=file:cmd_20260929054919158.cmd line="events on" reply="event log on"
    2026-09-29T05:49:19.524Z [INFO] [autopilot] event BulletFired shooter=11024 weapon=15 by_player=False from=(-82.172, 637.644, 15.195) to=(-30.871, 748.799, 15.352)
    2026-09-29T05:49:19.678Z [INFO] [autopilot] event BulletFired shooter=11024 weapon=15 by_player=False from=(-82.177, 637.631, 15.191) to=(264.973, 1358.822, -8.71)
    2026-09-29T05:49:19.872Z [INFO] [autopilot] event BulletFired shooter=11024 weapon=15 by_player=False from=(-82.171, 637.634, 15.202) to=(-77.783, 646.518, 15.092)
    2026-09-29T05:49:19.873Z [INFO] [autopilot] event PedAppeared handle=14602
    2026-09-29T05:49:19.985Z [INFO] [autopilot] event BulletFired shooter=11024 weapon=15 by_player=False from=(-82.187, 637.636, 15.198) to=(255.138, 1363.559, -3.696)
    2026-09-29T05:49:20.017Z [INFO] [autopilot] event BulletFired shooter=11536 weapon=15 by_player=False from=(-13.232, 704.566, 24.122) to=(-67.077, 674.437, 14.246)
    2026-09-29T05:49:20.023Z [INFO] command source=file:cmd_20260929054919854.cmd line="god on" reply="invincible True"
    2026-09-29T05:49:20.265Z [INFO] performance samples=586 frame_p50_ms=26 frame_p95_ms=97 frame_p99_ms=200 frames_over_33ms=142 frames_over_50ms=43 gunplay_avg_ms=1.298 gunplay_max_ms=5.333 phase_samples=586 phase_setup_avg_ms=0.839 phase_setup_max_ms=3.842 phase_camera_avg_ms=0.038 phase_camera_max_ms=3.688 phase_bullets_avg_ms=0.011 phase_bullets_max_ms=0.173 phase_weapon_avg_ms=0.020 phase_weapon_max_ms=0.120 phase_hud_avg_ms=0.390 phase_hud_max_ms=3.349
    2026-09-29T05:49:20.266Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.921/280.3/586@7 total=1712 module.gunplay=1.310/6.2/586@7 total=767 tick.gunplay=1.308/6.2/586@7 total=766 gp.freeaim=0.631/2.5/586@7 total=370 module.arsenal=0.671/3.0/416@7 total=279 tick.arsenal=0.671/3.0/416@7 total=279 ar.storage=0.407/2.5/416@7 total=169 module.atmosphere=0.223/3.8/586@7 total=131 tick.atmosphere=0.222/3.8/586@7 total=130 ar.safehouse_flags=0.230/2.8/416@7 total=96 gp.index_pad=0.133/0.3/586@7 total=78 engine.world=0.117/1.7/586@7 total=68 module.combat=0.033/0.6/586@7 total=19 tick.combat=0.032/0.6/586@7 total=19 cam.handle=0.031/3.7/586@7 total=18 module.holsters=0.063/0.6/242@7 total=15 tick.holsters=0.062/0.6/242@7 total=15 module.devtools=0.034/4.1/416@7 total=14 tick.devtools=0.033/4.1/416@7 total=14 gp.shoulder=0.016/0.1/586@7 total=10 module.world=0.179/1.1/48@7 total=9 gp.weapon_id=0.013/0.1/586@7 total=8 gp.player=0.011/0.1/586@7 total=7 gp.cycle=0.009/0.1/586@7 total=5 module.probe=1.460/1.6/3@7 total=4 gp.state=0.007/0.1/586@7 total=4 gp.shots=0.004/0.0/586@7 total=2 cam.aim_key=0.003/0.2/586@7 total=2 ar.lvs=0.004/0.3/416@7 total=2 ar.discover=0.004/0.7/416@7 total=2 cam.find_active=0.003/0.0/586@7 total=2 ar.vehicle=0.003/0.1/416@7 total=1 gp.spread=0.002/0.0/586@7 total=1 ar.reconcile=0.003/0.0/416@7 total=1 combat.dismember=0.001/0.0/586@7 total=0 ho.carried=0.002/0.0/214@7 total=0 engine.scheduler=0.001/0.0/586@7 total=0 ho.show=0.002/0.0/214@7 total=0 module.autopilot=0.000/0.0/586@7 total=0 combat.blood=0.000/0.0/586@7 total=0 gp.feel=0.000/0.0/586@7 total=0 module.weapon-probe=0.000/0.0/586@7 total=0 cam.fov=0.001/0.0/82@7 total=0 combat.pending=0.000/0.0/586@7 total=0 gp.recoil=0.000/0.0/586@7 total=0
    2026-09-29T05:49:20.267Z [INFO] engine_thread_probe ticks=586 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=585 ticks_after_skipped_frames=1
    2026-09-29T05:49:20.268Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.21 shdn_us=46.0
    2026-09-29T05:49:20.278Z [INFO] density frame_ms=141.8 peds=0.55 cars=0.60
    2026-09-29T05:49:20.280Z [INFO] command source=file:cmd_20260929054920247.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:49:20.281Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=18359 core=on peds=13 vehicles=7 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV frame_ms=37.30 p95_ms=422.56 pressure=0.52 private_mb=2296 working_set_mb=1836 address_free_mb=1100 largest_free_block_mb=1041 managed_mb=15 physical_load=84% core_us=58.3
    2026-09-29T05:49:20.306Z [INFO] [autopilot] event PedDamaged handle=10512 100->-100 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=280.2 health_lost=200.0 armour_lost=100.0 attacker=10512 vehicle=7689 killed=True hit=False
    2026-09-29T05:49:20.307Z [INFO] [autopilot] event PedDied handle=10512 bone=0xFFFFFFFF by_player=False exact=True type=Vehicle killer=10512 weapon=55
    2026-09-29T05:49:20.308Z [INFO] [autopilot] event PedDamaged handle=11274 100->-100 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=280.2 health_lost=200.0 armour_lost=100.0 attacker=10512 vehicle=7689 killed=True hit=False
    2026-09-29T05:49:20.309Z [INFO] [autopilot] event PedDied handle=11274 bone=0xFFFFFFFF by_player=False exact=True type=Vehicle killer=10512 weapon=55
    2026-09-29T05:49:20.310Z [INFO] [autopilot] event PedDamaged handle=11536 100->-100 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=280.2 health_lost=200.0 armour_lost=100.0 attacker=10512 vehicle=7689 killed=True hit=False
    2026-09-29T05:49:20.310Z [INFO] [autopilot] event PedDied handle=11536 bone=0xFFFFFFFF by_player=False exact=True type=Vehicle killer=10512 weapon=55
    2026-09-29T05:49:20.311Z [INFO] [autopilot] event VehicleDamaged handle=7689 999->822 engine 1000->1000
    2026-09-29T05:49:20.346Z [INFO] [autopilot] event PedAppeared handle=8984
    2026-09-29T05:49:20.347Z [INFO] [autopilot] event PedAppeared handle=9240
    2026-09-29T05:49:20.348Z [INFO] [autopilot] event PedAppeared handle=9491
    2026-09-29T05:49:20.348Z [INFO] [autopilot] event PedAppeared handle=9753
    2026-09-29T05:49:20.349Z [INFO] [autopilot] event PedAppeared handle=10013
    2026-09-29T05:49:20.350Z [INFO] [autopilot] event PedAppeared handle=10267
    2026-09-29T05:49:20.351Z [INFO] [autopilot] event PedAppeared handle=11798
    2026-09-29T05:49:20.352Z [INFO] [autopilot] event PedAppeared handle=12814
    2026-09-29T05:49:20.352Z [INFO] [autopilot] event PedAppeared handle=14856
    2026-09-29T05:49:20.353Z [INFO] [autopilot] event PedAppeared handle=15109
    2026-09-29T05:49:20.354Z [INFO] [autopilot] event VehicleAppeared handle=4619
    2026-09-29T05:49:20.428Z [INFO] [autopilot] event VehicleAppeared handle=5900
    2026-09-29T05:49:20.430Z [INFO] [autopilot] event VehicleAppeared handle=6155
    2026-09-29T05:49:20.468Z [INFO] [autopilot] event VehicleAppeared handle=6412
    2026-09-29T05:49:20.663Z [INFO] [autopilot] event PedRemoved handle=14602
    2026-09-29T05:49:20.709Z [INFO] [autopilot] event PedRemoved handle=15109
    2026-09-29T05:49:20.710Z [INFO] [autopilot] event PedRemoved handle=14856
    2026-09-29T05:49:20.710Z [INFO] [autopilot] event PedRemoved handle=12814
    2026-09-29T05:49:20.711Z [INFO] [autopilot] event PedRemoved handle=11798
    2026-09-29T05:49:20.712Z [INFO] [autopilot] event PedRemoved handle=10013
    2026-09-29T05:49:20.713Z [INFO] [autopilot] event VehicleDamaged handle=7689 822->792 engine 1000->1000
    2026-09-29T05:49:20.713Z [INFO] [autopilot] event VehicleRemoved handle=4106
    2026-09-29T05:49:20.741Z [INFO] [autopilot] event PedDamaged handle=10512 -100->-100 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=105.4 health_lost=0.0 armour_lost=0.0 attacker=10512 vehicle=7689 killed=False hit=False
    2026-09-29T05:49:20.742Z [INFO] [autopilot] event PedDamaged handle=11274 -100->-100 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=105.4 health_lost=0.0 armour_lost=0.0 attacker=10512 vehicle=7689 killed=False hit=False
    2026-09-29T05:49:20.742Z [INFO] [autopilot] event PedDamaged handle=11536 -100->-100 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=105.4 health_lost=0.0 armour_lost=0.0 attacker=10512 vehicle=7689 killed=False hit=False
    2026-09-29T05:49:20.743Z [INFO] [autopilot] event PedRemoved handle=9491
    2026-09-29T05:49:20.744Z [INFO] [autopilot] event VehicleDamaged handle=7689 792->732 engine 1000->1000
    2026-09-29T05:49:23.081Z [INFO] teleport_start id=gun_test_range target=1041,-568.3,20 snap=pavement
    2026-09-29T05:49:23.082Z [INFO] command source=file:cmd_20260929054920635.cmd line="goto gun_test_range" reply="Teleporting to Gun Test Range (Broker)"
    2026-09-29T05:49:23.105Z [INFO] [autopilot] event PedRemoved handle=10267
    2026-09-29T05:49:23.106Z [INFO] [autopilot] event PedRemoved handle=9753
    2026-09-29T05:49:23.107Z [INFO] [autopilot] event PedRemoved handle=9240
    2026-09-29T05:49:23.107Z [INFO] [autopilot] event PedRemoved handle=8984
    2026-09-29T05:49:23.108Z [INFO] [autopilot] event PedRemoved handle=13325
    2026-09-29T05:49:23.109Z [INFO] [autopilot] event PedRemoved handle=13068
    2026-09-29T05:49:23.109Z [INFO] [autopilot] event PedRemoved handle=8475
    2026-09-29T05:49:23.110Z [INFO] [autopilot] event PedRemoved handle=7954
    2026-09-29T05:49:23.111Z [INFO] [autopilot] event PedRemoved handle=7695
    2026-09-29T05:49:23.111Z [INFO] [autopilot] event PedRemoved handle=11274
    2026-09-29T05:49:23.112Z [INFO] [autopilot] event PedRemoved handle=8213
    2026-09-29T05:49:23.112Z [INFO] [autopilot] event PedRemoved handle=10773
    2026-09-29T05:49:23.113Z [INFO] [autopilot] event PedRemoved handle=11536
    2026-09-29T05:49:23.114Z [INFO] [autopilot] event PedRemoved handle=10512
    2026-09-29T05:49:23.114Z [INFO] [autopilot] event PedRemoved handle=11024
    2026-09-29T05:49:23.115Z [INFO] [autopilot] event VehicleRemoved handle=15112
    2026-09-29T05:49:23.115Z [INFO] [autopilot] event VehicleRemoved handle=6412
    2026-09-29T05:49:23.116Z [INFO] [autopilot] event VehicleRemoved handle=5900
    2026-09-29T05:49:23.117Z [INFO] [autopilot] event VehicleRemoved handle=3084
    2026-09-29T05:49:23.117Z [INFO] [autopilot] event VehicleRemoved handle=7689
    2026-09-29T05:49:23.118Z [INFO] [autopilot] event VehicleRemoved handle=15879
    2026-09-29T05:49:23.119Z [INFO] [autopilot] event VehicleRemoved handle=6155
    2026-09-29T05:49:23.119Z [INFO] [autopilot] event VehicleRemoved handle=4619
    2026-09-29T05:49:23.120Z [INFO] [autopilot] event VehicleRemoved handle=7179
    2026-09-29T05:49:23.121Z [INFO] [autopilot] event VehicleRemoved handle=13578
    2026-09-29T05:49:23.138Z [INFO] [world] world_object removed name=test_wall reason=out of range
    2026-09-29T05:49:23.839Z [INFO] [autopilot] event VehicleAppeared handle=6667
    2026-09-29T05:49:24.012Z [INFO] teleport_done id=gun_test_range final=1045.1,-563.0,25.6
    2026-09-29T05:49:24.658Z [INFO] [autopilot] event PedAppeared handle=9241
    2026-09-29T05:49:24.888Z [INFO] [autopilot] event PedAppeared handle=9492
    2026-09-29T05:49:25.508Z [INFO] [autopilot] event PedAppeared handle=6924
    2026-09-29T05:49:25.571Z [INFO] [autopilot] event PedAppeared handle=7181
    2026-09-29T05:49:25.587Z [INFO] [autopilot] event PedAppeared handle=7442
    2026-09-29T05:49:27.432Z [INFO] command source=file:cmd_20260929054927398.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:49:27.937Z [INFO] command source=file:cmd_20260929054927781.cmd line="weather 1" reply="weather 1"
    2026-09-29T05:49:28.183Z [INFO] command source=file:cmd_20260929054928172.cmd line="spawn 1 2.5" reply="spawning 1 at 2.5 m"
    2026-09-29T05:49:28.248Z [INFO] [autopilot] event PedAppeared handle=7696
    2026-09-29T05:49:28.250Z [INFO] [autopilot] autopilot_spawned count=1
    2026-09-29T05:49:28.702Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:31.775Z [INFO] command source=file:cmd_20260929054931569.cmd line="gore arm" reply="gore arm requested on the nearest NPC"
    2026-09-29T05:49:31.796Z [INFO] gore_test sever part=left_arm_elbow
    2026-09-29T05:49:31.836Z [INFO] [autopilot] event PedDamaged handle=7696 -100->-100 bone=0xFFFFFFFF by_player=False weapon=7 exact=True type=Bullet amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T05:49:31.837Z [INFO] [autopilot] event PedDied handle=7696 bone=0xFFFFFFFF by_player=False exact=True type=Bullet killer=0 weapon=7
    2026-09-29T05:49:32.309Z [INFO] dismember part=left_arm_elbow bones=17 skeleton_bones=80 engine=True copy=False
    2026-09-29T05:49:32.312Z [INFO] ptfx effect=blood_bang_chunks bone=0x4C1 scale=4.2 refused
    2026-09-29T05:49:32.313Z [INFO] ptfx effect=blood_bang_chunks bone=0x4C1 scale=4.2 loop_failed
    2026-09-29T05:49:32.314Z [INFO] ptfx effect=blood_artery_mist bone=0x4C1 scale=4.2 refused
    2026-09-29T05:49:32.315Z [INFO] ptfx effect=blood_artery_mist bone=0x4C1 scale=4.2 loop_failed
    2026-09-29T05:49:32.316Z [INFO] ptfx effect=blood_gun_chunks bone=0x4C1 scale=4.2 refused
    2026-09-29T05:49:32.317Z [INFO] ptfx effect=blood_gun_chunks bone=0x4C1 scale=4.2 loop_failed
    2026-09-29T05:49:32.318Z [INFO] ptfx effect=blood_artery bone=0x4C1 scale=3.0 refused
    2026-09-29T05:49:32.319Z [INFO] ptfx effect=blood_artery bone=0x4C1 scale=3.0 loop_failed
    2026-09-29T05:49:32.321Z [INFO] ptfx effect=blood_gun_entry bone=0x4C1 scale=3.0 leak 9000ms
    2026-09-29T05:49:32.322Z [INFO] ptfx effect=blood_drips bone=0x4C1 scale=3.3 refused
    2026-09-29T05:49:32.323Z [INFO] ptfx effect=blood_drips bone=0x4C1 scale=3.3 loop_failed
    2026-09-29T05:49:32.324Z [INFO] ptfx effect=blood_gun_entry bone=0x4C1 scale=3.3 leak 25000ms
    2026-09-29T05:49:32.325Z [INFO] combat_sever part=left_arm_elbow collapsed=True
    2026-09-29T05:49:32.423Z [INFO] dismember_limb_thrown part=left_arm_elbow collapsed=62
    2026-09-29T05:49:32.468Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0xFFFFFFFF by_player=False weapon=7 exact=True type=Bullet amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T05:49:32.469Z [INFO] [autopilot] event PedDied handle=12557 bone=0xFFFFFFFF by_player=False exact=True type=Bullet killer=0 weapon=7
    2026-09-29T05:49:32.470Z [INFO] [autopilot] event PedAppeared handle=12557
    2026-09-29T05:49:32.619Z [INFO] dismember_limb_visible part=left_arm_elbow_limb confirm_ticks=3
    2026-09-29T05:49:33.096Z [INFO] [autopilot] event PedDamaged handle=7696 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.203Z [INFO] [autopilot] event PedDamaged handle=7696 -100->-100 bone=0x36A0 by_player=False weapon=54 exact=True type=Fall amount=0.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.203Z [INFO] [autopilot] event PedDamaged handle=7696 -100->-100 bone=0x36A0 by_player=False weapon=54 exact=True type=Fall amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.237Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0xFFFFFFFF by_player=False weapon=7 exact=True type=Bullet amount=10.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.389Z [INFO] [autopilot] event PedDamaged handle=7696 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.465Z [INFO] [autopilot] event PedAppeared handle=12815
    2026-09-29T05:49:33.465Z [INFO] [autopilot] event PedAppeared handle=13069
    2026-09-29T05:49:33.466Z [INFO] [autopilot] event PedAppeared handle=13326
    2026-09-29T05:49:33.467Z [INFO] [autopilot] event PedAppeared handle=13579
    2026-09-29T05:49:33.554Z [INFO] dismember_evidence part=left_arm_elbow clone=False ticks=30 engine_hits=20 engine_calls=1411 skeleton=True copy=False
    2026-09-29T05:49:33.618Z [INFO] dismember_evidence part=left_arm_elbow_limb clone=True ticks=30 engine_hits=20 engine_calls=1557 skeleton=True copy=False
    2026-09-29T05:49:33.662Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x1A7 by_player=False weapon=54 exact=True type=Fall amount=1.9 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.695Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x1A7 by_player=False weapon=54 exact=True type=Fall amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.696Z [INFO] [autopilot] event VehicleAppeared handle=6413
    2026-09-29T05:49:33.728Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=1.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.772Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x36A0 by_player=False weapon=54 exact=True type=Fall amount=0.9 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.835Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x4C8 by_player=False weapon=54 exact=True type=Fall amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:33.836Z [INFO] [autopilot] event PedAppeared handle=13833
    2026-09-29T05:49:33.836Z [INFO] [autopilot] event PedAppeared handle=14090
    2026-09-29T05:49:33.904Z [INFO] [autopilot] event PedAppeared handle=14351
    2026-09-29T05:49:33.905Z [INFO] [autopilot] event PedAppeared handle=14603
    2026-09-29T05:49:33.937Z [INFO] [autopilot] event PedAppeared handle=14857
    2026-09-29T05:49:33.937Z [INFO] [autopilot] event PedAppeared handle=15110
    2026-09-29T05:49:33.938Z [INFO] [autopilot] event PedAppeared handle=15368
    2026-09-29T05:49:33.981Z [INFO] [autopilot] event PedAppeared handle=15620
    2026-09-29T05:49:33.981Z [INFO] [autopilot] event PedAppeared handle=15877
    2026-09-29T05:49:33.982Z [INFO] [autopilot] event PedAppeared handle=16134
    2026-09-29T05:49:33.983Z [INFO] [autopilot] event PedAppeared handle=16387
    2026-09-29T05:49:33.983Z [INFO] [autopilot] event PedRemoved handle=13579
    2026-09-29T05:49:34.014Z [INFO] [autopilot] event PedAppeared handle=16644
    2026-09-29T05:49:34.015Z [INFO] [autopilot] event PedAppeared handle=16899
    2026-09-29T05:49:34.016Z [INFO] [autopilot] event VehicleAppeared handle=7946
    2026-09-29T05:49:34.045Z [INFO] [autopilot] event PedAppeared handle=17155
    2026-09-29T05:49:34.046Z [INFO] [autopilot] event PedAppeared handle=17411
    2026-09-29T05:49:34.117Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x1A7 by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:34.118Z [INFO] [autopilot] event PedAppeared handle=17667
    2026-09-29T05:49:34.119Z [INFO] [autopilot] event PedAppeared handle=17923
    2026-09-29T05:49:34.121Z [INFO] dismember_limb_landed part=left_arm_elbow_limb height=0.42
    2026-09-29T05:49:34.151Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x1A7 by_player=False weapon=54 exact=True type=Fall amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:34.151Z [INFO] [autopilot] event PedAppeared handle=18179
    2026-09-29T05:49:34.294Z [INFO] [autopilot] event PedAppeared handle=18435
    2026-09-29T05:49:34.395Z [INFO] [autopilot] event PedDamaged handle=12557 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:34.612Z [INFO] [autopilot] event PedRemoved handle=16387
    2026-09-29T05:49:34.695Z [INFO] command source=file:cmd_20260929054934484.cmd line="hud off" reply="hud off"
    2026-09-29T05:49:34.944Z [INFO] command source=file:cmd_20260929054934868.cmd line="cam ped 0 20 2.8 1.0" reply="camera at 20 deg, 2.8 m"
    2026-09-29T05:49:34.992Z [INFO] [autopilot] event PedAppeared handle=16388
    2026-09-29T05:49:34.993Z [INFO] [autopilot] event PedRemoved handle=16134
    2026-09-29T05:49:36.167Z [INFO] [autopilot] event PedAppeared handle=16135
    2026-09-29T05:49:36.167Z [INFO] [autopilot] event PedRemoved handle=15877
    2026-09-29T05:49:37.196Z [INFO] [autopilot] event PedAppeared handle=18691
    2026-09-29T05:49:37.196Z [INFO] [autopilot] event PedRemoved handle=16135
    2026-09-29T05:49:37.254Z [INFO] [autopilot] event PedAppeared handle=18947
    2026-09-29T05:49:38.172Z [INFO] [autopilot] event PedAppeared handle=19203
    2026-09-29T05:49:38.173Z [INFO] [autopilot] event PedRemoved handle=18691
    2026-09-29T05:49:38.299Z [INFO] [autopilot] event VehicleAppeared handle=3597
    2026-09-29T05:49:38.411Z [INFO] [autopilot] event VehicleAppeared handle=3852
    2026-09-29T05:49:38.513Z [INFO] [autopilot] event VehicleAppeared handle=4108
    2026-09-29T05:49:38.654Z [INFO] command source=file:cmd_20260929054938595.cmd line="clear" reply="cleared 1"
    2026-09-29T05:49:38.670Z [INFO] [autopilot] event PedRemoved handle=7696
    2026-09-29T05:49:38.691Z [INFO] [autopilot] event PedRemoved handle=19203
    2026-09-29T05:49:38.695Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:38.819Z [INFO] [autopilot] event VehicleAppeared handle=4365
    2026-09-29T05:49:38.913Z [INFO] [autopilot] event PedAppeared handle=7956
    2026-09-29T05:49:38.914Z [INFO] [autopilot] event PedAppeared handle=8215
    2026-09-29T05:49:39.068Z [INFO] [autopilot] event PedAppeared handle=8477
    2026-09-29T05:49:39.132Z [INFO] [autopilot] event PedAppeared handle=8731
    2026-09-29T05:49:39.133Z [INFO] [autopilot] event VehicleAppeared handle=4877
    2026-09-29T05:49:39.169Z [INFO] [autopilot] event VehicleAppeared handle=4620
    2026-09-29T05:49:39.180Z [INFO] camera_already_gone handle=8707 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:49:39.181Z [INFO] command source=file:cmd_20260929054938976.cmd line="cam off" reply="camera off"
    2026-09-29T05:49:39.228Z [INFO] [autopilot] event PedAppeared handle=8986
    2026-09-29T05:49:39.229Z [INFO] [autopilot] event PedAppeared handle=9755
    2026-09-29T05:49:39.229Z [INFO] [autopilot] event PedRemoved handle=16388
    2026-09-29T05:49:39.230Z [INFO] [autopilot] event PedRemoved handle=18947
    2026-09-29T05:49:39.431Z [INFO] command source=file:cmd_20260929054939358.cmd line="spawn 1 2.5" reply="spawning 1 at 2.5 m"
    2026-09-29T05:49:39.471Z [INFO] [autopilot] event PedAppeared handle=10015
    2026-09-29T05:49:39.473Z [INFO] [autopilot] event PedRemoved handle=13326
    2026-09-29T05:49:39.474Z [INFO] [autopilot] autopilot_spawned count=1
    2026-09-29T05:49:39.656Z [INFO] [autopilot] event VehicleAppeared handle=8972
    2026-09-29T05:49:40.730Z [INFO] [autopilot] event PedRemoved handle=14603
    2026-09-29T05:49:40.777Z [INFO] [autopilot] event PedRemoved handle=8477
    2026-09-29T05:49:41.147Z [INFO] [autopilot] event PedAppeared handle=10269
    2026-09-29T05:49:41.169Z [INFO] [autopilot] event PedAppeared handle=10514
    2026-09-29T05:49:41.686Z [INFO] [autopilot] event PedAppeared handle=10775
    2026-09-29T05:49:41.687Z [INFO] [autopilot] event PedRemoved handle=10269
    2026-09-29T05:49:42.787Z [INFO] command source=file:cmd_20260929054942745.cmd line="gore leg" reply="gore leg requested on the nearest NPC"
    2026-09-29T05:49:42.807Z [INFO] gore_test sever part=right_leg_hip
    2026-09-29T05:49:42.837Z [INFO] [autopilot] event PedDamaged handle=10015 -100->-100 bone=0xFFFFFFFF by_player=False weapon=7 exact=True type=Bullet amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T05:49:42.838Z [INFO] [autopilot] event PedDied handle=10015 bone=0xFFFFFFFF by_player=False exact=True type=Bullet killer=0 weapon=7
    2026-09-29T05:49:43.015Z [INFO] [autopilot] event PedRemoved handle=10775
    2026-09-29T05:49:43.297Z [INFO] dismember part=right_leg_hip bones=5 skeleton_bones=80 engine=True copy=False
    2026-09-29T05:49:43.300Z [INFO] combat_sever part=right_leg_hip collapsed=True
    2026-09-29T05:49:43.334Z [INFO] [autopilot] event VehicleAppeared handle=7690
    2026-09-29T05:49:43.401Z [INFO] dismember_limb_thrown part=right_leg_hip collapsed=74
    2026-09-29T05:49:43.436Z [INFO] [autopilot] event PedDamaged handle=11026 -100->-100 bone=0xFFFFFFFF by_player=False weapon=7 exact=True type=Bullet amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T05:49:43.437Z [INFO] [autopilot] event PedDied handle=11026 bone=0xFFFFFFFF by_player=False exact=True type=Bullet killer=0 weapon=7
    2026-09-29T05:49:43.437Z [INFO] [autopilot] event PedAppeared handle=11026
    2026-09-29T05:49:43.439Z [INFO] [autopilot] event PedRemoved handle=15110
    2026-09-29T05:49:43.488Z [INFO] [autopilot] event PedAppeared handle=11276
    2026-09-29T05:49:43.489Z [INFO] [autopilot] event VehicleAppeared handle=7438
    2026-09-29T05:49:43.590Z [INFO] dismember_limb_visible part=right_leg_hip_limb confirm_ticks=3
    2026-09-29T05:49:43.926Z [INFO] [autopilot] event PedDamaged handle=10015 -100->-100 bone=0x36A0 by_player=False weapon=54 exact=True type=Fall amount=1.6 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:43.965Z [INFO] [autopilot] event PedDamaged handle=10015 -100->-100 bone=0x36A1 by_player=False weapon=54 exact=True type=Fall amount=1.3 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:43.966Z [INFO] [autopilot] event PedDamaged handle=10015 -100->-100 bone=0x36A1 by_player=False weapon=54 exact=True type=Fall amount=0.5 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:44.011Z [INFO] [autopilot] event PedDamaged handle=10015 -100->-100 bone=0x36A1 by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:44.093Z [INFO] [autopilot] event VehicleRemoved handle=7690
    2026-09-29T05:49:44.440Z [INFO] [autopilot] event VehicleAppeared handle=5388
    2026-09-29T05:49:44.476Z [INFO] dismember_evidence part=right_leg_hip clone=False ticks=30 engine_hits=57 engine_calls=34235 skeleton=True copy=False
    2026-09-29T05:49:44.552Z [INFO] dismember_evidence part=right_leg_hip_limb clone=True ticks=30 engine_hits=59 engine_calls=34418 skeleton=True copy=False
    2026-09-29T05:49:45.091Z [INFO] dismember_limb_floating_removed part=right_leg_hip_limb height=0.94
    2026-09-29T05:49:45.132Z [INFO] [autopilot] event PedAppeared handle=11538
    2026-09-29T05:49:45.133Z [INFO] [autopilot] event PedRemoved handle=11026
    2026-09-29T05:49:45.483Z [INFO] [autopilot] event VehicleAppeared handle=9227
    2026-09-29T05:49:45.570Z [INFO] [autopilot] event VehicleAppeared handle=5134
    2026-09-29T05:49:45.697Z [INFO] command source=file:cmd_20260929054945645.cmd line="cam ped 0 20 2.8 1.0" reply="camera at 20 deg, 2.8 m"
    2026-09-29T05:49:45.743Z [INFO] [autopilot] event PedAppeared handle=11800
    2026-09-29T05:49:45.743Z [INFO] [autopilot] event PedRemoved handle=11276
    2026-09-29T05:49:46.400Z [INFO] [autopilot] event VehicleAppeared handle=8459
    2026-09-29T05:49:46.448Z [INFO] [autopilot] event VehicleAppeared handle=6156
    2026-09-29T05:49:46.859Z [INFO] [autopilot] event VehicleAppeared handle=9483
    2026-09-29T05:49:46.860Z [INFO] [autopilot] event VehicleRemoved handle=5134
    2026-09-29T05:49:46.988Z [INFO] [autopilot] event PedAppeared handle=12042
    2026-09-29T05:49:47.417Z [INFO] [autopilot] event VehicleRemoved handle=7946
    2026-09-29T05:49:47.438Z [INFO] [autopilot] event VehicleAppeared handle=8716
    2026-09-29T05:49:47.907Z [INFO] [autopilot] event VehicleAppeared handle=9739
    2026-09-29T05:49:48.433Z [INFO] [autopilot] event VehicleAppeared handle=6925
    2026-09-29T05:49:48.714Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:49.082Z [INFO] [autopilot] event VehicleRemoved handle=3852
    2026-09-29T05:49:49.101Z [INFO] [autopilot] event VehicleRemoved handle=4877
    2026-09-29T05:49:49.434Z [INFO] command source=file:cmd_20260929054949218.cmd line="clear" reply="cleared 1"
    2026-09-29T05:49:49.462Z [INFO] [autopilot] event PedAppeared handle=10270
    2026-09-29T05:49:49.463Z [INFO] [autopilot] event PedRemoved handle=10015
    2026-09-29T05:49:49.702Z [INFO] camera_already_gone handle=8963 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:49:49.702Z [INFO] command source=file:cmd_20260929054949614.cmd line="cam off" reply="camera off"
    2026-09-29T05:49:49.724Z [INFO] [autopilot] event PedAppeared handle=10776
    2026-09-29T05:49:49.725Z [INFO] [autopilot] event PedRemoved handle=11800
    2026-09-29T05:49:49.726Z [INFO] [autopilot] event PedRemoved handle=12042
    2026-09-29T05:49:50.216Z [INFO] command source=file:cmd_20260929054949993.cmd line="spawn 1 2.5" reply="spawning 1 at 2.5 m"
    2026-09-29T05:49:50.256Z [INFO] [autopilot] event PedAppeared handle=11027
    2026-09-29T05:49:50.257Z [INFO] [autopilot] event PedRemoved handle=11538
    2026-09-29T05:49:50.259Z [INFO] [autopilot] autopilot_spawned count=1
    2026-09-29T05:49:50.261Z [INFO] performance samples=972 frame_p50_ms=26 frame_p95_ms=47 frame_p99_ms=76 frames_over_33ms=203 frames_over_50ms=31 gunplay_avg_ms=1.264 gunplay_max_ms=6.919 phase_samples=972 phase_setup_avg_ms=0.844 phase_setup_max_ms=6.243 phase_camera_avg_ms=0.021 phase_camera_max_ms=0.506 phase_bullets_avg_ms=0.002 phase_bullets_max_ms=0.014 phase_weapon_avg_ms=0.018 phase_weapon_max_ms=0.250 phase_hud_avg_ms=0.379 phase_hud_max_ms=0.675
    2026-09-29T05:49:50.262Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=5.148/2314.1/972@7 total=5003 module.gunplay=1.272/6.9/972@7 total=1236 tick.gunplay=1.270/6.9/972@7 total=1235 gp.freeaim=0.655/5.6/972@7 total=637 module.combat=0.529/153.8/972@7 total=515 tick.combat=0.529/153.8/972@7 total=514 module.arsenal=0.549/8.2/661@7 total=363 tick.arsenal=0.548/8.2/661@7 total=362 combat.pending=0.296/153.8/972@7 total=288 engine.world=0.255/16.1/972@7 total=248 ar.storage=0.336/3.6/661@7 total=222 module.atmosphere=0.157/3.8/972@7 total=153 tick.atmosphere=0.156/3.8/972@7 total=152 combat.dismember=0.135/27.6/972@7 total=131 gp.index_pad=0.128/0.4/972@7 total=124 ar.safehouse_flags=0.181/7.6/661@7 total=119 combat.blood=0.068/3.2/972@7 total=66 module.holsters=0.052/0.7/365@7 total=19 tick.holsters=0.051/0.7/365@7 total=19 module.devtools=0.027/3.4/661@7 total=18 tick.devtools=0.027/3.4/661@7 total=18 gp.shoulder=0.015/0.2/972@7 total=15 cam.handle=0.015/0.5/972@7 total=14 gp.weapon_id=0.012/0.1/972@7 total=12 gp.player=0.012/0.1/972@7 total=11 engine.scheduler=0.010/2.7/972@7 total=10 gp.cycle=0.008/0.1/972@7 total=8 gp.state=0.006/0.0/972@7 total=6 module.probe=1.320/1.3/3@7 total=4 gp.shots=0.004/0.0/972@7 total=4 cam.aim_key=0.003/0.1/972@7 total=3 cam.find_active=0.002/0.0/972@7 total=2 ar.lvs=0.003/0.4/661@7 total=2 ar.vehicle=0.003/0.0/661@7 total=2 gp.spread=0.002/0.0/972@7 total=2 ar.discover=0.003/0.7/661@7 total=2 module.world=0.021/0.9/53@7 total=1 ar.reconcile=0.002/0.0/661@7 total=1 ho.carried=0.002/0.0/365@7 total=1 ho.show=0.002/0.0/365@7 total=1 module.autopilot=0.000/0.0/972@7 total=0 module.weapon-probe=0.000/0.0/972@7 total=0 gp.feel=0.000/0.0/972@7 total=0 gp.recoil=0.000/0.0/972@7 total=0 cam.fov=0.001/0.0/99@7 total=0
    2026-09-29T05:49:50.263Z [INFO] engine_thread_probe ticks=972 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=971 ticks_after_skipped_frames=1
    2026-09-29T05:49:50.265Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.20 shdn_us=34.6
    2026-09-29T05:49:50.266Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=19331 core=on peds=32 vehicles=16 modules=10/10 coroutines=0 resources=4 raycast=on episode=GTAIV frame_ms=23.71 p95_ms=47.96 pressure=0.14 private_mb=2314 working_set_mb=1860 address_free_mb=1082 largest_free_block_mb=1025 managed_mb=15 physical_load=84% core_us=99.3
    2026-09-29T05:49:50.308Z [INFO] density frame_ms=38.2 peds=0.63 cars=0.68
    2026-09-29T05:49:50.850Z [INFO] [autopilot] event PedRemoved handle=10270
    2026-09-29T05:49:51.316Z [INFO] [autopilot] event PedAppeared handle=11277
    2026-09-29T05:49:51.408Z [INFO] [autopilot] event PedRemoved handle=13069
    2026-09-29T05:49:51.409Z [INFO] [autopilot] event VehicleAppeared handle=12040
    2026-09-29T05:49:51.689Z [INFO] [autopilot] event PedRemoved handle=10776
    2026-09-29T05:49:51.799Z [INFO] [autopilot] event VehicleAppeared handle=12553
    2026-09-29T05:49:51.824Z [INFO] [autopilot] event PedAppeared handle=11539
    2026-09-29T05:49:51.825Z [INFO] [autopilot] event PedRemoved handle=17411
    2026-09-29T05:49:51.967Z [INFO] [autopilot] event PedRemoved handle=9755
    2026-09-29T05:49:52.254Z [INFO] [autopilot] event PedAppeared handle=10016
    2026-09-29T05:49:52.255Z [INFO] [autopilot] event PedRemoved handle=11277
    2026-09-29T05:49:52.438Z [INFO] [autopilot] event PedAppeared handle=10271
    2026-09-29T05:49:52.791Z [INFO] [autopilot] event VehicleAppeared handle=10250
    2026-09-29T05:49:52.919Z [INFO] [autopilot] event PedAppeared handle=10777
    2026-09-29T05:49:53.028Z [INFO] [autopilot] event PedAppeared handle=11278
    2026-09-29T05:49:53.029Z [INFO] [autopilot] event PedRemoved handle=17923
    2026-09-29T05:49:53.218Z [INFO] [autopilot] event PedAppeared handle=11801
    2026-09-29T05:49:53.505Z [INFO] command source=file:cmd_20260929054953374.cmd line="gore head" reply="gore head requested on the nearest NPC"
    2026-09-29T05:49:53.541Z [INFO] gore_test sever part=head
    2026-09-29T05:49:53.569Z [INFO] [autopilot] event PedDamaged handle=11027 -100->-100 bone=0xFFFFFFFF by_player=False weapon=7 exact=True type=Bullet amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T05:49:53.569Z [INFO] [autopilot] event PedDied handle=11027 bone=0xFFFFFFFF by_player=False exact=True type=Bullet killer=0 weapon=7
    2026-09-29T05:49:54.014Z [INFO] dismember part=head bones=23 skeleton_bones=80 engine=True copy=False
    2026-09-29T05:49:54.016Z [INFO] combat_sever part=head collapsed=True
    2026-09-29T05:49:54.141Z [INFO] [autopilot] event PedAppeared handle=12043
    2026-09-29T05:49:54.141Z [INFO] [autopilot] event PedRemoved handle=10271
    2026-09-29T05:49:54.280Z [INFO] [autopilot] event PedDamaged handle=11027 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.4 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:54.411Z [INFO] [autopilot] event VehicleRemoved handle=4620
    2026-09-29T05:49:54.455Z [INFO] [autopilot] event PedDamaged handle=11027 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.3 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:54.536Z [INFO] [autopilot] event PedDamaged handle=11027 -100->-100 bone=0x4B5 by_player=False weapon=54 exact=True type=Fall amount=0.3 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:49:55.052Z [INFO] [autopilot] event PedAppeared handle=12300
    2026-09-29T05:49:55.052Z [INFO] [autopilot] event PedAppeared handle=13070
    2026-09-29T05:49:55.236Z [INFO] dismember_evidence part=head clone=False ticks=30 engine_hits=30 engine_calls=66603 skeleton=True copy=False
    2026-09-29T05:49:56.373Z [INFO] command source=file:cmd_20260929054956264.cmd line="cam ped 0 20 2.8 1.0" reply="camera at 20 deg, 2.8 m"
    2026-09-29T05:49:56.408Z [INFO] [autopilot] event PedAppeared handle=13327
    2026-09-29T05:49:56.409Z [INFO] [autopilot] event PedRemoved handle=12043
    2026-09-29T05:49:56.410Z [INFO] [autopilot] event PedRemoved handle=17155
    2026-09-29T05:49:56.410Z [INFO] [autopilot] event PedRemoved handle=15620
    2026-09-29T05:49:56.605Z [INFO] [autopilot] event PedAppeared handle=13580
    2026-09-29T05:49:57.742Z [INFO] [autopilot] event PedAppeared handle=14604
    2026-09-29T05:49:58.284Z [INFO] [autopilot] event VehicleRemoved handle=4108
    2026-09-29T05:49:58.285Z [INFO] [autopilot] event VehicleRemoved handle=3597
    2026-09-29T05:49:58.562Z [INFO] [autopilot] event VehicleAppeared handle=11785
    2026-09-29T05:49:58.712Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:58.735Z [INFO] [autopilot] event VehicleAppeared handle=8206
    2026-09-29T05:49:58.949Z [INFO] [autopilot] event VehicleAppeared handle=4109
    2026-09-29T05:49:58.981Z [INFO] [autopilot] event PedAppeared handle=15111
    2026-09-29T05:49:59.517Z [INFO] [autopilot] event VehicleAppeared handle=7181
    2026-09-29T05:50:00.022Z [INFO] camera_already_gone handle=9218 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:50:00.022Z [INFO] command source=file:cmd_20260929054959848.cmd line="cam off" reply="camera off"
    2026-09-29T05:50:00.056Z [INFO] [autopilot] event PedAppeared handle=13581
    2026-09-29T05:50:00.057Z [INFO] [autopilot] event PedAppeared handle=14605
    2026-09-29T05:50:00.058Z [INFO] [autopilot] event PedAppeared handle=15621
    2026-09-29T05:50:00.059Z [INFO] [autopilot] event PedRemoved handle=13327
    2026-09-29T05:50:00.059Z [INFO] [autopilot] event PedRemoved handle=14604
    2026-09-29T05:50:00.060Z [INFO] [autopilot] event PedRemoved handle=13580
    2026-09-29T05:50:00.155Z [INFO] [autopilot] event VehicleRemoved handle=6667
    2026-09-29T05:50:00.273Z [INFO] command source=file:cmd_20260929055000227.cmd line="hud on" reply="hud on"
    2026-09-29T05:50:00.322Z [INFO] [autopilot] event VehicleRemoved handle=11785
    2026-09-29T05:50:00.344Z [INFO] [autopilot] event VehicleAppeared handle=11020
    2026-09-29T05:50:00.777Z [INFO] command source=file:cmd_20260929055000603.cmd line="clear" reply="cleared 1"
    2026-09-29T05:50:00.798Z [INFO] [autopilot] event PedAppeared handle=12044
    2026-09-29T05:50:00.799Z [INFO] [autopilot] event PedRemoved handle=11027

# Scenario sling-review

- Result: NEEDS-REVIEW
- Steps: 32, failed: 0
- Game alive at end: True
- Log errors during run: 1

## Failed steps

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- mark log line 322
- give 10 200 => gave 10
- give 14 200 => gave 14
- give 7 100 => gave 7
- select 7 => selected 7
- expectmarked holster_sling_attached slot=LongGun1: OK 2026-09-29T19:29:22.771Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
- expectmarked holster_sling_attached slot=LongGun2: OK 2026-09-29T19:29:22.803Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
- hud off => hud off
- cam 0 2.2 0.4 => camera at 0 deg, 2.2 m
- wait 1500 ms
- shot front -> front.jpg
- cam 180 2.2 0.4 => camera at 180 deg, 2.2 m
- wait 1500 ms
- shot back -> back.jpg
- cam 90 2.0 0.4 => camera at 90 deg, 2 m
- wait 1500 ms
- shot left -> left.jpg
- cam 270 2.0 0.4 => camera at 270 deg, 2 m
- wait 1500 ms
- shot right -> right.jpg
- cam 150 1.4 0.6 => camera at 150 deg, 1.4 m
- wait 1500 ms
- shot back_close -> back_close.jpg
- cam off => camera off
- hud on => hud on

## Errors
    2026-09-29T19:29:25.501Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T19:29:15.059Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:29:16.032Z [INFO] command source=file:cmd_20260929192915953.cmd line="events on" reply="event log on"
    2026-09-29T19:29:16.080Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.992, 637.298, 15.196) to=(327.219, 1322.256, -1.153)
    2026-09-29T19:29:16.210Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-81.78, 639.744, 15.188) to=(265.313, 1361.165, 5.136)
    2026-09-29T19:29:16.211Z [INFO] [autopilot] event PedAppeared handle=2565
    2026-09-29T19:29:16.258Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.982, 637.298, 15.197) to=(-67.549, 669.686, 14.522)
    2026-09-29T19:29:16.373Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-81.811, 639.724, 15.197) to=(282.166, 1352.897, 16.634)
    2026-09-29T19:29:16.530Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-81.805, 639.708, 15.186) to=(-74.5, 654.806, 14.812)
    2026-09-29T19:29:16.531Z [INFO] [autopilot] event PedRemoved handle=2565
    2026-09-29T19:29:16.567Z [INFO] command source=file:cmd_20260929192916354.cmd line="god on" reply="invincible True"
    2026-09-29T19:29:16.808Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.841, 658.742, 15.065) to=(-10.666, 1457.505, 15.024)
    2026-09-29T19:29:16.812Z [INFO] command source=file:cmd_20260929192916737.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T19:29:16.876Z [INFO] [autopilot] event PedAppeared handle=2822
    2026-09-29T19:29:16.877Z [INFO] [autopilot] event PedAppeared handle=5635
    2026-09-29T19:29:16.878Z [INFO] [autopilot] event PedAppeared handle=5892
    2026-09-29T19:29:16.878Z [INFO] [autopilot] event PedAppeared handle=6148
    2026-09-29T19:29:16.879Z [INFO] [autopilot] event PedAppeared handle=7171
    2026-09-29T19:29:16.879Z [INFO] [autopilot] event PedAppeared handle=7427
    2026-09-29T19:29:16.998Z [INFO] [autopilot] event VehicleAppeared handle=516
    2026-09-29T19:29:17.033Z [INFO] [autopilot] event PedAppeared handle=7683
    2026-09-29T19:29:17.034Z [INFO] [autopilot] event VehicleAppeared handle=772
    2026-09-29T19:29:17.251Z [INFO] [autopilot] event PedAppeared handle=7940
    2026-09-29T19:29:17.252Z [INFO] [autopilot] event PedAppeared handle=8195
    2026-09-29T19:29:17.288Z [INFO] [autopilot] event PedAppeared handle=8451
    2026-09-29T19:29:17.289Z [INFO] [autopilot] event PedAppeared handle=8706
    2026-09-29T19:29:17.290Z [INFO] [autopilot] event PedAppeared handle=8962
    2026-09-29T19:29:17.290Z [INFO] [autopilot] event PedAppeared handle=9218
    2026-09-29T19:29:17.320Z [INFO] [autopilot] event PedAppeared handle=9474
    2026-09-29T19:29:17.325Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T19:29:17.330Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=4
    2026-09-29T19:29:17.331Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T19:29:17.332Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=1
    2026-09-29T19:29:17.333Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T19:29:17.630Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=296
    2026-09-29T19:29:17.631Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T19:29:17.632Z [INFO] command source=file:cmd_20260929192917131.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T19:29:17.652Z [INFO] [autopilot] event PedAppeared handle=9730
    2026-09-29T19:29:17.653Z [INFO] [autopilot] event PedAppeared handle=9986
    2026-09-29T19:29:17.654Z [INFO] [autopilot] event PedAppeared handle=10242
    2026-09-29T19:29:17.655Z [INFO] [autopilot] event PedAppeared handle=10498
    2026-09-29T19:29:17.655Z [INFO] [autopilot] event PedAppeared handle=10754
    2026-09-29T19:29:17.656Z [INFO] [autopilot] event PedAppeared handle=11010
    2026-09-29T19:29:17.657Z [INFO] [autopilot] event VehicleAppeared handle=5890
    2026-09-29T19:29:17.657Z [INFO] [autopilot] event VehicleAppeared handle=6146
    2026-09-29T19:29:17.658Z [INFO] [autopilot] event VehicleRemoved handle=5122
    2026-09-29T19:29:17.659Z [INFO] [autopilot] event VehicleRemoved handle=4866
    2026-09-29T19:29:17.659Z [INFO] [autopilot] event VehicleRemoved handle=4610
    2026-09-29T19:29:17.737Z [INFO] [autopilot] event PedAppeared handle=11266
    2026-09-29T19:29:17.738Z [INFO] [autopilot] event VehicleAppeared handle=2307
    2026-09-29T19:29:17.791Z [INFO] [autopilot] event PedRemoved handle=6148
    2026-09-29T19:29:17.792Z [INFO] [autopilot] event PedRemoved handle=5892
    2026-09-29T19:29:17.793Z [INFO] [autopilot] event PedRemoved handle=2822
    2026-09-29T19:29:17.794Z [INFO] [autopilot] event PedRemoved handle=7427
    2026-09-29T19:29:17.794Z [INFO] [autopilot] event PedRemoved handle=7683
    2026-09-29T19:29:17.795Z [INFO] [autopilot] event PedRemoved handle=7171
    2026-09-29T19:29:17.796Z [INFO] [autopilot] event PedRemoved handle=5635
    2026-09-29T19:29:17.899Z [INFO] [autopilot] event PedRemoved handle=7940
    2026-09-29T19:29:17.899Z [INFO] [autopilot] event PedRemoved handle=8451
    2026-09-29T19:29:17.900Z [INFO] [autopilot] event PedRemoved handle=8195
    2026-09-29T19:29:17.901Z [INFO] [autopilot] event PedRemoved handle=9218
    2026-09-29T19:29:17.994Z [INFO] [autopilot] event PedRemoved handle=9986
    2026-09-29T19:29:17.995Z [INFO] [autopilot] event PedRemoved handle=10754
    2026-09-29T19:29:18.054Z [INFO] [autopilot] event PedRemoved handle=11010
    2026-09-29T19:29:18.055Z [INFO] [autopilot] event PedRemoved handle=8962
    2026-09-29T19:29:18.055Z [INFO] [autopilot] event PedRemoved handle=10242
    2026-09-29T19:29:18.056Z [INFO] [autopilot] event PedRemoved handle=8706
    2026-09-29T19:29:18.098Z [INFO] [autopilot] event PedRemoved handle=9474
    2026-09-29T19:29:18.144Z [INFO] [autopilot] event PedRemoved handle=11266
    2026-09-29T19:29:18.144Z [INFO] [autopilot] event PedRemoved handle=9730
    2026-09-29T19:29:18.194Z [INFO] [autopilot] event PedRemoved handle=10498
    2026-09-29T19:29:18.562Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T19:29:18.595Z [INFO] [autopilot] event PedAppeared handle=5636
    2026-09-29T19:29:18.596Z [INFO] [autopilot] event PedAppeared handle=5893
    2026-09-29T19:29:18.596Z [INFO] [autopilot] event VehicleAppeared handle=4610
    2026-09-29T19:29:18.597Z [INFO] [autopilot] event VehicleAppeared handle=4866
    2026-09-29T19:29:18.598Z [INFO] [autopilot] event VehicleAppeared handle=5122
    2026-09-29T19:29:18.598Z [INFO] [autopilot] event VehicleRemoved handle=6146
    2026-09-29T19:29:18.599Z [INFO] [autopilot] event VehicleRemoved handle=5890
    2026-09-29T19:29:18.758Z [INFO] [autopilot] event PedAppeared handle=6149
    2026-09-29T19:29:18.956Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.984, 637.298, 15.202) to=(-34.754, 721.169, 17.165)
    2026-09-29T19:29:19.006Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-81.83, 639.695, 15.145) to=(298.394, 1344.557, 32.258)
    2026-09-29T19:29:19.007Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.206, 654.051, 15.11) to=(-22.634, 1454.42, 22.698)
    2026-09-29T19:29:19.131Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.981, 637.287, 15.177) to=(325.574, 1323.114, 47.538)
    2026-09-29T19:29:19.170Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-81.86, 639.999, 15.202) to=(301.513, 1343.288, 26.211)
    2026-09-29T19:29:19.171Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.34, 654.146, 15.107) to=(-24.466, 1454.004, 46.061)
    2026-09-29T19:29:19.306Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.45, 654.185, 15.112) to=(-55.776, 753.216, 13.754)
    2026-09-29T19:29:19.307Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.862, 637.402, 15.126) to=(310.025, 1332.844, 4.887)
    2026-09-29T19:29:19.346Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-82.01, 640.327, 15.205) to=(-80.641, 643.233, 15.142)
    2026-09-29T19:29:19.417Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.589, 637.689, 15.142) to=(328.27, 1322.35, 1.437)
    2026-09-29T19:29:19.506Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-82.237, 640.697, 15.081) to=(280.161, 1354.344, -3.154)
    2026-09-29T19:29:19.592Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-86.25, 638.102, 15.102) to=(-76.045, 655.017, 14.255)
    2026-09-29T19:29:19.674Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-82.446, 641.207, 15.156) to=(302.125, 1343.776, 20.877)
    2026-09-29T19:29:19.763Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-85.903, 638.566, 15.141) to=(325.038, 1325.319, -10.729)
    2026-09-29T19:29:19.858Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-82.435, 641.587, 15.128) to=(-75.429, 655.017, 14.619)
    2026-09-29T19:29:19.859Z [INFO] [autopilot] event PedAppeared handle=7428
    2026-09-29T19:29:19.859Z [INFO] [autopilot] event PedAppeared handle=7684
    2026-09-29T19:29:20.445Z [INFO] [autopilot] event PedAppeared handle=7941
    2026-09-29T19:29:20.573Z [INFO] [autopilot] event PedRemoved handle=5636
    2026-09-29T19:29:20.721Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-83.824, 641.122, 15.122) to=(-75.651, 655.008, 14.742)
    2026-09-29T19:29:20.901Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.87, 658.79, 15.079) to=(-64.624, 674.85, 14.875)
    2026-09-29T19:29:20.901Z [INFO] [autopilot] event PedDamaged handle=2 60->45 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.624, 674.85, 14.875) dir=(0.077, 0.997, -0.013)
    2026-09-29T19:29:20.902Z [INFO] [autopilot] event PlayerDamaged 60->45
    2026-09-29T19:29:21.069Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.001, 658.808, 15.122) to=(5.187, 1455.806, -13.368)
    2026-09-29T19:29:21.243Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.998, 658.809, 15.11) to=(8.985, 1455.551, -9.066)
    2026-09-29T19:29:21.385Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.997, 658.816, 15.123) to=(-64.664, 674.848, 14.938)
    2026-09-29T19:29:21.386Z [INFO] [autopilot] event PedDamaged handle=2 45->30 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.664, 674.848, 14.938) dir=(0.083, 0.996, -0.011)
    2026-09-29T19:29:21.387Z [INFO] [autopilot] event PlayerDamaged 45->30
    2026-09-29T19:29:21.570Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.014, 658.82, 15.116) to=(12.129, 1455.349, -4.139)
    2026-09-29T19:29:21.754Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.018, 658.829, 15.12) to=(-47.513, 898.56, 13.756)
    2026-09-29T19:29:21.755Z [INFO] [autopilot] event VehicleAppeared handle=2052
    2026-09-29T19:29:21.805Z [INFO] [autopilot] event PedRemoved handle=5893
    2026-09-29T19:29:21.858Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-79.538, 647.919, 15.111) to=(306.434, 1349.208, -0.296)
    2026-09-29T19:29:21.858Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-81.405, 644.066, 15.13) to=(321.249, 1336.106, -3.47)
    2026-09-29T19:29:21.869Z [INFO] command source=file:cmd_20260929192921802.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T19:29:22.025Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-79.345, 648.504, 15.139) to=(332.082, 1335.619, 15.383)
    2026-09-29T19:29:22.026Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-81.096, 644.49, 15.124) to=(-75.141, 654.794, 14.856)
    2026-09-29T19:29:22.154Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-80.679, 644.942, 15.121) to=(298.575, 1349.646, -21.547)
    2026-09-29T19:29:22.201Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-79.101, 649.111, 15.119) to=(-67.556, 668.569, 14.891)
    2026-09-29T19:29:22.308Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-80.342, 645.313, 15.121) to=(-75.146, 654.79, 14.886)
    2026-09-29T19:29:22.371Z [INFO] command source=file:cmd_20260929192922221.cmd line="weather 1" reply="weather 1"
    2026-09-29T19:29:22.478Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-80.003, 645.733, 15.141) to=(303.786, 1348.204, -1.851)
    2026-09-29T19:29:22.647Z [INFO] command source=file:cmd_20260929192922613.cmd line="give 10 200" reply="gave 10"
    2026-09-29T19:29:22.683Z [INFO] [autopilot] event PlayerWeaponChanged 7->10
    2026-09-29T19:29:22.685Z [INFO] weapon_changed from=7 to=10 profile=vanilla
    2026-09-29T19:29:22.688Z [INFO] arsenal_gain id=10 owned=False mission=False
    2026-09-29T19:29:22.729Z [INFO] holsters_removed reason=overflow
    2026-09-29T19:29:22.731Z [INFO] arsenal_overflow id=16 to=fallback:-67282078:1413.7:188.8
    2026-09-29T19:29:22.771Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T19:29:22.803Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T19:29:22.819Z [INFO] [autopilot] event PlayerWeaponChanged 10->7
    2026-09-29T19:29:22.821Z [INFO] weapon_changed from=10 to=7 profile=vanilla
    2026-09-29T19:29:23.182Z [INFO] command source=file:cmd_20260929192922993.cmd line="give 14 200" reply="gave 14"
    2026-09-29T19:29:23.202Z [INFO] [autopilot] event PlayerWeaponChanged 7->14
    2026-09-29T19:29:23.204Z [INFO] weapon_changed from=7 to=14 profile=vanilla
    2026-09-29T19:29:23.321Z [INFO] arsenal_gain id=14 owned=False mission=False
    2026-09-29T19:29:23.353Z [INFO] holsters_removed reason=overflow
    2026-09-29T19:29:23.353Z [INFO] arsenal_overflow id=18 to=fallback:-67282078:1413.7:188.8
    2026-09-29T19:29:23.400Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T19:29:23.420Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T19:29:23.433Z [INFO] [autopilot] event PlayerWeaponChanged 14->12
    2026-09-29T19:29:23.435Z [INFO] weapon_changed from=14 to=12 profile=vanilla
    2026-09-29T19:29:23.441Z [INFO] command source=file:cmd_20260929192923384.cmd line="give 7 100" reply="gave 7"
    2026-09-29T19:29:23.473Z [INFO] [autopilot] event PlayerWeaponChanged 12->7
    2026-09-29T19:29:23.475Z [INFO] weapon_changed from=12 to=7 profile=vanilla
    2026-09-29T19:29:23.945Z [INFO] command source=file:cmd_20260929192923776.cmd line="select 7" reply="selected 7"
    2026-09-29T19:29:24.212Z [INFO] command source=file:cmd_20260929192924202.cmd line="hud off" reply="hud off"
    2026-09-29T19:29:24.513Z [INFO] [autopilot] event PedAppeared handle=5894
    2026-09-29T19:29:24.513Z [INFO] [autopilot] event VehicleRemoved handle=5378
    2026-09-29T19:29:24.702Z [INFO] [autopilot] event VehicleAppeared handle=5379
    2026-09-29T19:29:24.736Z [INFO] command source=file:cmd_20260929192924594.cmd line="cam 0 2.2 0.4" reply="camera at 0 deg, 2.2 m"
    2026-09-29T19:29:25.070Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:29:25.163Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.001, 658.813, 15.12) to=(-64.365, 674.698, 15.072)
    2026-09-29T19:29:25.164Z [INFO] [autopilot] event PedDamaged handle=2 30->10 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.365, 674.698, 15.072) dir=(0.102, 0.995, -0.003)
    2026-09-29T19:29:25.165Z [INFO] [autopilot] event PlayerDamaged 30->10
    2026-09-29T19:29:25.326Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.022, 658.818, 15.111) to=(-64.546, 674.786, 14.64)
    2026-09-29T19:29:25.327Z [INFO] [autopilot] event PedDamaged handle=2 11->-100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=110.9 armour_lost=0.0 attacker=2051 vehicle=0 killed=True hit=True at=(-64.546, 674.786, 14.64) dir=(0.092, 0.995, -0.029)
    2026-09-29T19:29:25.329Z [INFO] [autopilot] event PedDied handle=2 bone=0x4B3 by_player=False exact=True type=Bullet killer=2051 weapon=15
    2026-09-29T19:29:25.330Z [INFO] [autopilot] event PlayerWeaponChanged 7->0
    2026-09-29T19:29:25.331Z [INFO] [autopilot] event PlayerDied
    2026-09-29T19:29:25.332Z [INFO] [autopilot] event PlayerDamaged 10->-100
    2026-09-29T19:29:25.500Z [INFO] holsters_removed reason=wasted
    2026-09-29T19:29:25.501Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored
    2026-09-29T19:29:25.502Z [INFO] arsenal_loss reason=wasted id=3 owned=False
    2026-09-29T19:29:25.503Z [INFO] arsenal_loss reason=wasted id=7 owned=False
    2026-09-29T19:29:25.503Z [INFO] arsenal_loss reason=wasted id=10 owned=False
    2026-09-29T19:29:25.504Z [INFO] arsenal_loss reason=wasted id=12 owned=False
    2026-09-29T19:29:25.505Z [INFO] arsenal_loss reason=wasted id=14 owned=False
    2026-09-29T19:29:25.505Z [INFO] arsenal_loss reason=wasted id=5 owned=False
    2026-09-29T19:29:26.261Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.025, 655.042, 14.83) to=(-67.549, 670.493, 14.683)
    2026-09-29T19:29:26.262Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.761, 646.381, 15.186) to=(-67.647, 667.566, 14.306)
    2026-09-29T19:29:26.331Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4B5 by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T19:29:26.476Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.057, 655.016, 14.775) to=(331.207, 1343.52, -5.692)
    2026-09-29T19:29:26.477Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.727, 646.344, 15.18) to=(247.493, 1377.665, -6.657)
    2026-09-29T19:29:26.536Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.991, 658.86, 15.098) to=(-63.867, 675.615, 14.202)
    2026-09-29T19:29:26.537Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A8 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-63.867, 675.615, 14.202) dir=(0.126, 0.991, -0.053)
    2026-09-29T19:29:26.606Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-76.998, 655.052, 14.825) to=(-67.549, 670.459, 14.278)
    2026-09-29T19:29:26.660Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.722, 646.354, 15.189) to=(-67.549, 669.096, 14.434)
    2026-09-29T19:29:26.731Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.027, 658.846, 15.105) to=(26.619, 1451.805, -45.327)
    2026-09-29T19:29:26.787Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-76.902, 655.094, 15.054) to=(335.237, 1339.984, -30.205)
    2026-09-29T19:29:26.788Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.729, 646.363, 15.177) to=(258.947, 1371.392, -31.698)
    2026-09-29T19:29:26.856Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.017, 658.85, 15.091) to=(-63.774, 685.171, 13.666)
    2026-09-29T19:29:26.964Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-76.858, 655.117, 15.148) to=(-67.549, 671.735, 13.981)
    2026-09-29T19:29:26.965Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.733, 646.364, 15.187) to=(260.134, 1371.198, -24.241)
    2026-09-29T19:29:27.021Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.016, 658.848, 15.104) to=(-63.603, 681.181, 13.698)
    2026-09-29T19:29:27.234Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.024, 658.839, 15.094) to=(6.553, 1455.415, -24.814)
    2026-09-29T19:29:28.345Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.001, 658.873, 15.084) to=(6.483, 1452.565, -62.893)
    2026-09-29T19:29:28.482Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.698, 646.37, 15.169) to=(-77.61, 646.643, 15.161)
    2026-09-29T19:29:28.540Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.026, 658.85, 15.101) to=(-64.175, 676.516, 13.895)
    2026-09-29T19:29:28.541Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.175, 676.516, 13.895) dir=(0.104, 0.992, -0.068)
    2026-09-29T19:29:28.604Z [INFO] camera_already_gone handle=3586 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:29:28.605Z [INFO] command source=file:cmd_20260929192928398.cmd line="cam 180 2.2 0.4" reply="camera at 180 deg, 2.2 m"
    2026-09-29T19:29:28.671Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-76.992, 654.995, 15.168) to=(-67.549, 670.641, 14.025)
    2026-09-29T19:29:28.672Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.732, 646.362, 15.188) to=(-67.549, 668.16, 14.41)
    2026-09-29T19:29:29.789Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.02, 658.843, 15.093) to=(3.509, 1453.323, -56.574)
    2026-09-29T19:29:30.209Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.023, 654.989, 15.182) to=(325.051, 1346.622, -18.027)
    2026-09-29T19:29:30.210Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.018, 658.851, 15.104) to=(-64.163, 680.726, 13.592)
    2026-09-29T19:29:30.210Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.731, 646.35, 15.193) to=(-67.549, 668.943, 14.554)
    2026-09-29T19:29:30.298Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.728, 646.362, 15.203) to=(-67.549, 669.665, 14.69)
    2026-09-29T19:29:30.358Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.024, 654.977, 15.172) to=(-67.558, 671.561, 13.923)
    2026-09-29T19:29:30.359Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.03, 658.843, 15.099) to=(-63.531, 682.319, 13.694)
    2026-09-29T19:29:30.360Z [INFO] [autopilot] event PedRemoved handle=6149
    2026-09-29T19:29:30.445Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.74, 646.36, 15.197) to=(-73.728, 654.983, 14.723)
    2026-09-29T19:29:30.485Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.02, 654.973, 15.18) to=(-67.549, 670.745, 14.329)
    2026-09-29T19:29:30.529Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.023, 658.843, 15.101) to=(-64.346, 675.461, 13.568)
    2026-09-29T19:29:30.641Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.034, 654.976, 15.175) to=(-67.549, 671.466, 14.341)
    2026-09-29T19:29:30.642Z [INFO] [autopilot] event PedAppeared handle=6150
    2026-09-29T19:29:31.244Z [INFO] [autopilot] event PedRemoved handle=5894
    2026-09-29T19:29:31.595Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.997, 658.861, 15.083) to=(-63.846, 681.512, 13.62)
    2026-09-29T19:29:31.677Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-76.993, 654.999, 15.166) to=(334.596, 1339.477, -41.038)
    2026-09-29T19:29:31.709Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.702, 646.356, 15.188) to=(-67.549, 668.588, 14.214)
    2026-09-29T19:29:31.739Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.025, 658.844, 15.098) to=(26.398, 1452.326, -39.404)
    2026-09-29T19:29:31.840Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.024, 654.987, 15.182) to=(-67.549, 670.348, 14.167)
    2026-09-29T19:29:31.881Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.734, 646.341, 15.2) to=(259.52, 1371.771, -17.816)
    2026-09-29T19:29:31.911Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.022, 658.843, 15.091) to=(-0.188, 1455.405, -33.852)
    2026-09-29T19:29:32.040Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.021, 654.98, 15.174) to=(325.628, 1345.726, -26.25)
    2026-09-29T19:29:32.041Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.732, 646.336, 15.193) to=(-73.798, 654.977, 14.713)
    2026-09-29T19:29:32.073Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.024, 658.856, 15.102) to=(-64.275, 676.42, 13.588)
    2026-09-29T19:29:32.074Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.275, 676.42, 13.588) dir=(0.099, 0.991, -0.085)
    2026-09-29T19:29:32.075Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4B3 by_player=False weapon=54 exact=True type=Fall amount=0.3 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T19:29:32.077Z [INFO] camera_already_gone handle=3842 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:29:32.079Z [INFO] command source=file:cmd_20260929192931983.cmd line="cam 90 2.0 0.4" reply="camera at 90 deg, 2 m"
    2026-09-29T19:29:32.343Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.729, 646.349, 15.2) to=(238.408, 1381.798, -5.331)
    2026-09-29T19:29:32.463Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.016, 654.982, 15.184) to=(-67.549, 670.673, 13.96)
    2026-09-29T19:29:32.463Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.024, 658.855, 15.094) to=(-64.28, 675.873, 13.811)
    2026-09-29T19:29:32.464Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A9 by_player=False weapon=15 exact=True type=Bullet amount=9.9 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.28, 675.873, 13.811) dir=(0.102, 0.992, -0.075)
    2026-09-29T19:29:32.650Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.027, 654.986, 15.175) to=(330.231, 1342.937, -27.258)
    2026-09-29T19:29:32.651Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.739, 646.343, 15.197) to=(-74.078, 654.977, 14.702)
    2026-09-29T19:29:32.779Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.732, 646.348, 15.2) to=(230.608, 1384.952, -11.177)
    2026-09-29T19:29:33.247Z [INFO] [autopilot] event VehicleRemoved handle=6914
    2026-09-29T19:29:33.870Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-76.997, 654.994, 15.164) to=(-67.549, 670.844, 14.246)
    2026-09-29T19:29:33.871Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.001, 658.869, 15.084) to=(-64.271, 676.302, 13.932)
    2026-09-29T19:29:33.871Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.271, 676.302, 13.932) dir=(0.099, 0.993, -0.066)
    2026-09-29T19:29:33.872Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x36A0 by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T19:29:33.958Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.698, 646.364, 15.191) to=(-73.951, 655.142, 14.793)
    2026-09-29T19:29:34.021Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.028, 654.987, 15.18) to=(324.899, 1345.244, -39.652)
    2026-09-29T19:29:34.022Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.031, 658.852, 15.1) to=(-64.536, 676.734, 13.668)
    2026-09-29T19:29:34.023Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.536, 676.734, 13.668) dir=(0.083, 0.993, -0.08)
    2026-09-29T19:29:34.069Z [INFO] [autopilot] event PedRemoved handle=7684
    2026-09-29T19:29:34.104Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.73, 646.351, 15.2) to=(227.753, 1385.924, -16.663)
    2026-09-29T19:29:34.184Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.027, 654.985, 15.173) to=(-67.578, 671.022, 13.778)
    2026-09-29T19:29:34.185Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.026, 658.852, 15.091) to=(-64.193, 676.345, 13.822)
    2026-09-29T19:29:34.185Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.193, 676.345, 13.822) dir=(0.104, 0.992, -0.072)
    2026-09-29T19:29:34.306Z [INFO] [autopilot] event BulletFired shooter=1283 weapon=15 by_player=False from=(-77.731, 646.335, 15.19) to=(-73.614, 655.009, 14.743)
    2026-09-29T19:29:34.342Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-77.021, 654.994, 15.188) to=(326.179, 1345.468, -25.326)
    2026-09-29T19:29:34.343Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.025, 658.86, 15.101) to=(21.323, 1453.465, -32.133)
    2026-09-29T19:29:34.432Z [INFO] density frame_ms=67.9 peds=0.55 cars=0.60
    2026-09-29T19:29:36.508Z [INFO] [autopilot] event BulletFired shooter=0 weapon=-1 by_player=False from=(-77.723, 646.34, 15.2) to=(-73.836, 654.978, 14.719)
    2026-09-29T19:29:36.509Z [INFO] [autopilot] event PedRemoved handle=6150
    2026-09-29T19:29:36.510Z [INFO] [autopilot] event PedRemoved handle=7428
    2026-09-29T19:29:36.511Z [INFO] [autopilot] event PedRemoved handle=7941
    2026-09-29T19:29:36.511Z [INFO] [autopilot] event PedRemoved handle=1283
    2026-09-29T19:29:36.512Z [INFO] [autopilot] event PedRemoved handle=2307
    2026-09-29T19:29:36.512Z [INFO] [autopilot] event PedRemoved handle=1795
    2026-09-29T19:29:36.513Z [INFO] [autopilot] event PedRemoved handle=2051
    2026-09-29T19:29:36.514Z [INFO] [autopilot] event VehicleAppeared handle=773
    2026-09-29T19:29:36.515Z [INFO] [autopilot] event VehicleAppeared handle=1028
    2026-09-29T19:29:36.515Z [INFO] [autopilot] event VehicleAppeared handle=1284
    2026-09-29T19:29:36.516Z [INFO] [autopilot] event VehicleAppeared handle=1540
    2026-09-29T19:29:36.517Z [INFO] [autopilot] event VehicleAppeared handle=1796
    2026-09-29T19:29:36.517Z [INFO] [autopilot] event VehicleAppeared handle=2053
    2026-09-29T19:29:36.518Z [INFO] [autopilot] event VehicleAppeared handle=2308
    2026-09-29T19:29:36.519Z [INFO] [autopilot] event VehicleAppeared handle=2563
    2026-09-29T19:29:36.519Z [INFO] [autopilot] event VehicleAppeared handle=2819
    2026-09-29T19:29:36.520Z [INFO] [autopilot] event VehicleAppeared handle=3075
    2026-09-29T19:29:36.521Z [INFO] [autopilot] event VehicleAppeared handle=3331
    2026-09-29T19:29:36.521Z [INFO] [autopilot] event VehicleAppeared handle=3587
    2026-09-29T19:29:36.522Z [INFO] [autopilot] event VehicleAppeared handle=3843
    2026-09-29T19:29:36.522Z [INFO] [autopilot] event VehicleRemoved handle=5379
    2026-09-29T19:29:36.523Z [INFO] [autopilot] event VehicleRemoved handle=2307
    2026-09-29T19:29:36.524Z [INFO] [autopilot] event VehicleRemoved handle=2052
    2026-09-29T19:29:36.524Z [INFO] [autopilot] event VehicleRemoved handle=772
    2026-09-29T19:29:36.525Z [INFO] [autopilot] event VehicleRemoved handle=516
    2026-09-29T19:29:36.526Z [INFO] [autopilot] event VehicleRemoved handle=5122
    2026-09-29T19:29:36.526Z [INFO] [autopilot] event VehicleRemoved handle=4866
    2026-09-29T19:29:36.527Z [INFO] [autopilot] event VehicleRemoved handle=4610
    2026-09-29T19:29:36.527Z [INFO] [autopilot] event VehicleRemoved handle=7938
    2026-09-29T19:29:36.528Z [INFO] [autopilot] event VehicleRemoved handle=7426
    2026-09-29T19:29:36.529Z [INFO] [autopilot] event VehicleRemoved handle=7170
    2026-09-29T19:29:36.529Z [INFO] [autopilot] event VehicleRemoved handle=6658
    2026-09-29T19:29:36.530Z [INFO] [autopilot] event VehicleRemoved handle=6402
    2026-09-29T19:29:36.531Z [INFO] [autopilot] event VehicleRemoved handle=5634
    2026-09-29T19:29:36.531Z [INFO] [autopilot] event VehicleRemoved handle=4354
    2026-09-29T19:29:36.532Z [INFO] [autopilot] event VehicleRemoved handle=4098
    2026-09-29T19:29:36.533Z [INFO] [autopilot] event VehicleRemoved handle=3842
    2026-09-29T19:29:36.533Z [INFO] [autopilot] event VehicleRemoved handle=3586
    2026-09-29T19:29:36.534Z [INFO] [autopilot] event VehicleRemoved handle=3330
    2026-09-29T19:29:36.535Z [INFO] [autopilot] event VehicleRemoved handle=3074
    2026-09-29T19:29:36.535Z [INFO] [autopilot] event VehicleRemoved handle=2818
    2026-09-29T19:29:36.536Z [INFO] [autopilot] event VehicleRemoved handle=2562
    2026-09-29T19:29:36.538Z [INFO] weapon_changed from=7 to=0 profile=vanilla
    2026-09-29T19:29:36.540Z [INFO] performance samples=516 frame_p50_ms=40 frame_p95_ms=88 frame_p99_ms=200 frames_over_33ms=387 frames_over_50ms=131 gunplay_avg_ms=0.977 gunplay_max_ms=9.813 phase_samples=374 phase_setup_avg_ms=0.853 phase_setup_max_ms=3.205 phase_camera_avg_ms=0.073 phase_camera_max_ms=6.867 phase_bullets_avg_ms=0.010 phase_bullets_max_ms=0.089 phase_weapon_avg_ms=0.018 phase_weapon_max_ms=0.196 phase_hud_avg_ms=0.376 phase_hud_max_ms=0.710
    2026-09-29T19:29:36.540Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.388/313.2/516@7 total=2264 module.gunplay=1.019/23.5/516@7 total=526 tick.gunplay=1.018/23.5/516@7 total=525 module.arsenal=0.756/59.1/495@7 total=374 tick.arsenal=0.756/59.1/495@7 total=374 module.holsters=0.990/57.7/283@7 total=280 tick.holsters=0.990/57.7/283@7 total=280 gp.freeaim=0.718/3.1/374@7 total=268 engine.world=0.466/28.5/516@7 total=240 ho.show=0.974/57.7/191@7 total=186 ar.storage=0.389/3.7/358@7 total=139 engine.scheduler=0.242/81.6/516@7 total=125 ar.reconcile=0.290/57.6/358@7 total=104 module.atmosphere=0.199/2.8/516@7 total=103 tick.atmosphere=0.198/2.8/516@7 total=102 ar.safehouse_flags=0.174/1.9/358@7 total=62 module.combat=0.111/9.0/516@7 total=57 tick.combat=0.110/9.0/516@7 total=57 combat.sample=1.073/8.9/40@7 total=43 cam.handle=0.067/6.8/374@7 total=25 gp.index_pad=0.053/0.2/374@7 total=20 module.devtools=0.031/3.6/495@7 total=15 tick.devtools=0.030/3.6/495@7 total=15 gp.weapon_id=0.025/0.9/374@7 total=9 module.world=0.134/0.4/48@7 total=6 gp.shoulder=0.015/0.2/374@7 total=6 gp.player=0.012/0.1/374@7 total=4 gp.cycle=0.007/0.1/374@7 total=3 module.probe=1.232/1.3/2@7 total=2 gp.state=0.006/0.0/374@7 total=2 ar.discover=0.004/0.7/495@7 total=2 ar.lvs=0.003/0.3/495@7 total=2 ar.vehicle=0.003/0.1/495@7 total=2 cam.aim_key=0.003/0.0/374@7 total=1 cam.find_active=0.002/0.0/374@7 total=1 gp.spread=0.002/0.0/374@7 total=1 gp.shots=0.002/0.0/374@7 total=1 ho.carried=0.002/0.0/191@7 total=0 combat.dismember=0.001/0.0/516@7 total=0 module.autopilot=0.000/0.0/516@7 total=0 combat.blood=0.000/0.0/516@7 total=0 combat.pending=0.000/0.0/516@7 total=0 module.weapon-probe=0.000/0.0/516@7 total=0 cam.fov=0.001/0.0/59@7 total=0 gp.feel=0.000/0.0/374@7 total=0 gp.recoil=0.000/0.0/374@7 total=0
    2026-09-29T19:29:36.541Z [INFO] engine_thread_probe ticks=516 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=514 ticks_after_skipped_frames=2
    2026-09-29T19:29:36.544Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=2.06 shdn_us=125.7
    2026-09-29T19:29:36.548Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:29:36.549Z [INFO] [world] world_object removed name=test_wall reason=out of range
    2026-09-29T19:29:36.551Z [INFO] camera_already_gone handle=4098 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:29:36.552Z [INFO] command source=file:cmd_20260929192935568.cmd line="cam 270 2.0 0.4" reply="camera at 270 deg, 2 m"
    2026-09-29T19:29:36.554Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1922 core=on peds=1 vehicles=13 modules=10/10 coroutines=0 resources=4 raycast=on episode=GTAIV frame_ms=44.80 p95_ms=85.76 pressure=0.84 private_mb=2072 working_set_mb=1925 address_free_mb=1354 largest_free_block_mb=1423 managed_mb=14 physical_load=96% core_us=36.3
    2026-09-29T19:29:37.969Z [INFO] [autopilot] event PedAppeared handle=1541
    2026-09-29T19:29:38.749Z [INFO] [autopilot] event VehicleAppeared handle=5380
    2026-09-29T19:29:38.855Z [INFO] [autopilot] event VehicleAppeared handle=6403
    2026-09-29T19:29:38.981Z [INFO] [autopilot] event PedAppeared handle=1796
    2026-09-29T19:29:39.024Z [INFO] [autopilot] event PedAppeared handle=2052
    2026-09-29T19:29:39.082Z [INFO] [autopilot] event PedAppeared handle=2308
    2026-09-29T19:29:39.083Z [INFO] [autopilot] event PedAppeared handle=2566
    2026-09-29T19:29:39.125Z [INFO] [autopilot] event PedAppeared handle=2823
    2026-09-29T19:29:39.126Z [INFO] [autopilot] event PedAppeared handle=3076
    2026-09-29T19:29:39.127Z [INFO] [autopilot] event PedAppeared handle=3331
    2026-09-29T19:29:39.127Z [INFO] [autopilot] event PedAppeared handle=3587
    2026-09-29T19:29:39.128Z [INFO] [autopilot] event PedAppeared handle=3843
    2026-09-29T19:29:39.129Z [INFO] [autopilot] event PedAppeared handle=4099
    2026-09-29T19:29:39.130Z [INFO] [autopilot] event PedAppeared handle=4355
    2026-09-29T19:29:39.229Z [INFO] [autopilot] event PedAppeared handle=4611
    2026-09-29T19:29:39.388Z [INFO] [autopilot] event PedAppeared handle=4867
    2026-09-29T19:29:39.951Z [INFO] [autopilot] event PedAppeared handle=5123
    2026-09-29T19:29:40.157Z [INFO] camera_already_gone handle=4610 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:29:40.159Z [INFO] command source=file:cmd_20260929192940001.cmd line="cam 150 1.4 0.6" reply="camera at 150 deg, 1.4 m"
    2026-09-29T19:29:42.872Z [INFO] [autopilot] event PedRemoved handle=2566
    2026-09-29T19:29:43.040Z [INFO] [autopilot] event PedRemoved handle=4355
    2026-09-29T19:29:43.243Z [INFO] [autopilot] event PedRemoved handle=2308
    2026-09-29T19:29:43.327Z [INFO] [autopilot] event PedAppeared handle=2567
    2026-09-29T19:29:43.375Z [INFO] [autopilot] event PedAppeared handle=4356
    2026-09-29T19:29:43.634Z [INFO] [autopilot] event VehicleAppeared handle=7171
    2026-09-29T19:29:43.737Z [INFO] [autopilot] event PedAppeared handle=5379
    2026-09-29T19:29:43.886Z [INFO] camera_already_gone handle=5378 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:29:43.887Z [INFO] command source=file:cmd_20260929192943640.cmd line="cam off" reply="camera off"
    2026-09-29T19:29:44.146Z [INFO] command source=file:cmd_20260929192944036.cmd line="hud on" reply="hud on"

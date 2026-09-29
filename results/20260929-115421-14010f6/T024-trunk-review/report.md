# Scenario trunk-review

- Result: FAIL
- Steps: 35, failed: 5
- Game alive at end: True
- Log errors during run: 0

## Failed steps
- expect choreography_begin trunk: no new log line in 8 s
- expect arsenal_storage_open: no new log line in 8 s
- expect arsenal_store id=14 to=: no new log line in 10 s
- expect choreography_complete trunk: no new log line in 10 s
- expect arsenal_storage_closed: no new log line in 10 s

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
- expect autopilot_spawncar handle=: OK 2026-09-29T19:07:14.093Z [INFO] [autopilot] autopilot_spawncar handle=7429 model=admiral
- wait 1500 ms
- at-trunk 3.0 => at trunk of 7429 (-61.456, 682.84, 14.658)
- wait 2000 ms
- key E 80 ms
- wait 4000 ms
- FAILED: expect choreography_begin trunk: no new log line in 8 s
- FAILED: expect arsenal_storage_open: no new log line in 8 s
- hud off => hud off
- shot trunk_open -> trunk_open.jpg
- key Right 1200 ms
- wait 500 ms
- shot trunk_wheel_next_segment -> trunk_wheel_next_segment.jpg
- key Left 1200 ms
- wait 500 ms
- key Space 1200 ms
- FAILED: expect arsenal_store id=14 to=: no new log line in 10 s
- wait 2500 ms
- shot trunk_after_store -> trunk_after_store.jpg
- key Back 1200 ms
- wait 3500 ms
- FAILED: expect choreography_complete trunk: no new log line in 10 s
- FAILED: expect arsenal_storage_closed: no new log line in 10 s
- shot trunk_closed -> trunk_closed.jpg
- hud on => hud on
- clear => cleared 1

## Errors

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T19:07:05.818Z [INFO] command source=file:cmd_20260929190705770.cmd line="events on" reply="event log on"
    2026-09-29T19:07:06.279Z [INFO] [autopilot] event PedRemoved handle=9225
    2026-09-29T19:07:06.320Z [INFO] [autopilot] event PedRemoved handle=7692
    2026-09-29T19:07:06.326Z [INFO] command source=file:cmd_20260929190706189.cmd line="god on" reply="invincible True"
    2026-09-29T19:07:06.511Z [INFO] [autopilot] event PedRemoved handle=6411
    2026-09-29T19:07:06.714Z [INFO] [autopilot] event PedRemoved handle=9988
    2026-09-29T19:07:06.766Z [INFO] [autopilot] event PedAppeared handle=6412
    2026-09-29T19:07:06.822Z [INFO] [autopilot] event PedAppeared handle=7693
    2026-09-29T19:07:06.829Z [INFO] command source=file:cmd_20260929190706594.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T19:07:06.887Z [INFO] [autopilot] event PedAppeared handle=9226
    2026-09-29T19:07:06.888Z [INFO] [autopilot] event PedAppeared handle=9989
    2026-09-29T19:07:06.889Z [INFO] [autopilot] event PedAppeared handle=10244
    2026-09-29T19:07:06.889Z [INFO] [autopilot] event PedAppeared handle=10501
    2026-09-29T19:07:06.890Z [INFO] [autopilot] event VehicleAppeared handle=2053
    2026-09-29T19:07:06.910Z [INFO] [autopilot] event VehicleAppeared handle=2309
    2026-09-29T19:07:06.933Z [INFO] [autopilot] event VehicleAppeared handle=2564
    2026-09-29T19:07:06.955Z [INFO] [autopilot] event VehicleAppeared handle=2820
    2026-09-29T19:07:06.994Z [INFO] [autopilot] event PedAppeared handle=12036
    2026-09-29T19:07:07.341Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T19:07:07.342Z [INFO] command source=file:cmd_20260929190706982.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T19:07:07.364Z [INFO] [autopilot] event PedAppeared handle=12293
    2026-09-29T19:07:07.365Z [INFO] [autopilot] event VehicleAppeared handle=3076
    2026-09-29T19:07:07.366Z [INFO] [autopilot] event VehicleAppeared handle=3332
    2026-09-29T19:07:07.366Z [INFO] [autopilot] event VehicleAppeared handle=3589
    2026-09-29T19:07:07.367Z [INFO] [autopilot] event VehicleAppeared handle=4101
    2026-09-29T19:07:07.368Z [INFO] [autopilot] event VehicleAppeared handle=5124
    2026-09-29T19:07:07.389Z [INFO] [autopilot] event PedAppeared handle=12547
    2026-09-29T19:07:07.390Z [INFO] [autopilot] event PedRemoved handle=7435
    2026-09-29T19:07:07.432Z [INFO] [autopilot] event PedRemoved handle=10501
    2026-09-29T19:07:07.451Z [INFO] [autopilot] event PedRemoved handle=9226
    2026-09-29T19:07:07.473Z [INFO] [autopilot] event VehicleAppeared handle=5381
    2026-09-29T19:07:07.526Z [INFO] [autopilot] event PedAppeared handle=9227
    2026-09-29T19:07:07.527Z [INFO] [autopilot] event PedAppeared handle=9990
    2026-09-29T19:07:07.528Z [INFO] [autopilot] event PedRemoved handle=11781
    2026-09-29T19:07:07.528Z [INFO] [autopilot] event PedRemoved handle=9989
    2026-09-29T19:07:07.551Z [INFO] [autopilot] event PedAppeared handle=10245
    2026-09-29T19:07:07.552Z [INFO] [autopilot] event PedRemoved handle=10244
    2026-09-29T19:07:07.597Z [INFO] [autopilot] event PedAppeared handle=10502
    2026-09-29T19:07:07.598Z [INFO] [autopilot] event PedAppeared handle=10757
    2026-09-29T19:07:07.702Z [INFO] [autopilot] event VehicleAppeared handle=5893
    2026-09-29T19:07:07.748Z [INFO] [autopilot] event VehicleDamaged handle=2309 1000->957 engine 1000->936
    2026-09-29T19:07:07.749Z [INFO] [autopilot] event VehicleDamaged handle=2820 1000->975 engine 1000->1000
    2026-09-29T19:07:07.777Z [INFO] [autopilot] event PedRemoved handle=6412
    2026-09-29T19:07:07.777Z [INFO] [autopilot] event PedRemoved handle=3336
    2026-09-29T19:07:07.778Z [INFO] [autopilot] event PedRemoved handle=5125
    2026-09-29T19:07:07.779Z [INFO] [autopilot] event PedRemoved handle=8453
    2026-09-29T19:07:07.797Z [INFO] [autopilot] event PedAppeared handle=5126
    2026-09-29T19:07:07.798Z [INFO] [autopilot] event PedAppeared handle=5383
    2026-09-29T19:07:07.798Z [INFO] [autopilot] event PedAppeared handle=5640
    2026-09-29T19:07:07.848Z [INFO] [autopilot] event PedAppeared handle=8454
    2026-09-29T19:07:07.848Z [INFO] [autopilot] event PedAppeared handle=8968
    2026-09-29T19:07:07.849Z [INFO] [autopilot] event PedAppeared handle=11012
    2026-09-29T19:07:07.876Z [INFO] [autopilot] event PedAppeared handle=11270
    2026-09-29T19:07:07.877Z [INFO] [autopilot] event PedAppeared handle=11782
    2026-09-29T19:07:07.878Z [INFO] [autopilot] event PedAppeared handle=12803
    2026-09-29T19:07:07.879Z [INFO] [autopilot] event PedAppeared handle=13059
    2026-09-29T19:07:07.880Z [INFO] [autopilot] event PedRemoved handle=9990
    2026-09-29T19:07:07.915Z [INFO] [autopilot] event PedAppeared handle=9991
    2026-09-29T19:07:07.916Z [INFO] [autopilot] event PedAppeared handle=13314
    2026-09-29T19:07:07.917Z [INFO] [autopilot] event PedAppeared handle=13570
    2026-09-29T19:07:07.918Z [INFO] [autopilot] event PedAppeared handle=13826
    2026-09-29T19:07:07.919Z [INFO] [autopilot] event PedRemoved handle=7693
    2026-09-29T19:07:07.954Z [INFO] [autopilot] event PedRemoved handle=9227
    2026-09-29T19:07:07.983Z [INFO] [autopilot] event PedRemoved handle=10502
    2026-09-29T19:07:08.047Z [INFO] [autopilot] event PedRemoved handle=10245
    2026-09-29T19:07:08.071Z [INFO] [autopilot] event PedRemoved handle=10757
    2026-09-29T19:07:08.260Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T19:07:08.280Z [INFO] [autopilot] event PedAppeared handle=3078
    2026-09-29T19:07:08.281Z [INFO] [autopilot] event PedAppeared handle=3337
    2026-09-29T19:07:08.281Z [INFO] [autopilot] event PedRemoved handle=9733
    2026-09-29T19:07:08.282Z [INFO] [autopilot] event PedRemoved handle=11525
    2026-09-29T19:07:08.283Z [INFO] [autopilot] event PedRemoved handle=9478
    2026-09-29T19:07:08.284Z [INFO] [autopilot] event VehicleRemoved handle=5124
    2026-09-29T19:07:08.304Z [INFO] [autopilot] event PedAppeared handle=7694
    2026-09-29T19:07:08.305Z [INFO] [autopilot] event VehicleAppeared handle=7174
    2026-09-29T19:07:08.356Z [INFO] [autopilot] event PedAppeared handle=9228
    2026-09-29T19:07:08.428Z [INFO] [autopilot] event PedAppeared handle=9479
    2026-09-29T19:07:08.567Z [INFO] [autopilot] event PedRemoved handle=11012
    2026-09-29T19:07:08.598Z [INFO] [autopilot] event PedRemoved handle=8968
    2026-09-29T19:07:08.599Z [INFO] [autopilot] event PedRemoved handle=13059
    2026-09-29T19:07:08.635Z [INFO] [autopilot] event PedRemoved handle=12803
    2026-09-29T19:07:08.671Z [INFO] [autopilot] event PedAppeared handle=8969
    2026-09-29T19:07:08.672Z [INFO] [autopilot] event PedRemoved handle=3078
    2026-09-29T19:07:08.707Z [INFO] [autopilot] event PedAppeared handle=9229
    2026-09-29T19:07:08.708Z [INFO] [autopilot] event PedRemoved handle=9228
    2026-09-29T19:07:08.709Z [INFO] [autopilot] event PedRemoved handle=7694
    2026-09-29T19:07:08.710Z [INFO] [autopilot] event PedRemoved handle=12547
    2026-09-29T19:07:08.747Z [INFO] [autopilot] event PedRemoved handle=9479
    2026-09-29T19:07:08.747Z [INFO] [autopilot] event PedRemoved handle=12293
    2026-09-29T19:07:08.784Z [INFO] [autopilot] event PedAppeared handle=5384
    2026-09-29T19:07:08.785Z [INFO] [autopilot] event PedAppeared handle=7695
    2026-09-29T19:07:08.786Z [INFO] [autopilot] event PedAppeared handle=8455
    2026-09-29T19:07:08.787Z [INFO] [autopilot] event PedAppeared handle=9480
    2026-09-29T19:07:08.788Z [INFO] [autopilot] event PedAppeared handle=9734
    2026-09-29T19:07:08.791Z [INFO] [autopilot] event PedRemoved handle=11782
    2026-09-29T19:07:08.792Z [INFO] [autopilot] event PedRemoved handle=11270
    2026-09-29T19:07:08.793Z [INFO] [autopilot] event PedRemoved handle=5126
    2026-09-29T19:07:08.793Z [INFO] [autopilot] event PedRemoved handle=5383
    2026-09-29T19:07:08.794Z [INFO] [autopilot] event PedRemoved handle=8454
    2026-09-29T19:07:08.824Z [INFO] [autopilot] event PedAppeared handle=9992
    2026-09-29T19:07:08.825Z [INFO] [autopilot] event PedAppeared handle=10246
    2026-09-29T19:07:08.825Z [INFO] [autopilot] event PedRemoved handle=9991
    2026-09-29T19:07:08.894Z [INFO] [autopilot] event PedRemoved handle=3337
    2026-09-29T19:07:09.348Z [INFO] [autopilot] event VehicleDamaged handle=3332 999->966 engine 1000->950
    2026-09-29T19:07:09.398Z [INFO] [autopilot] event PedAppeared handle=5127
    2026-09-29T19:07:09.399Z [INFO] [autopilot] event PedAppeared handle=10503
    2026-09-29T19:07:09.448Z [INFO] [autopilot] event PedAppeared handle=10758
    2026-09-29T19:07:10.076Z [INFO] [autopilot] event PedRemoved handle=10503
    2026-09-29T19:07:10.105Z [INFO] [autopilot] event PedRemoved handle=10758
    2026-09-29T19:07:10.628Z [INFO] [autopilot] event PedAppeared handle=10759
    2026-09-29T19:07:10.629Z [INFO] [autopilot] event PedAppeared handle=11013
    2026-09-29T19:07:10.671Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.894, 635.072, 15.194) to=(270.366, 1352.365, 3.555)
    2026-09-29T19:07:10.710Z [INFO] [autopilot] event BulletFired shooter=6916 weapon=15 by_player=False from=(-61.583, 717.132, 105.38) to=(-67.148, 674.842, 14.975)
    2026-09-29T19:07:10.730Z [INFO] [autopilot] event PedAppeared handle=1798
    2026-09-29T19:07:10.731Z [INFO] [autopilot] event PedAppeared handle=2055
    2026-09-29T19:07:10.732Z [INFO] [autopilot] event PedAppeared handle=2311
    2026-09-29T19:07:10.788Z [INFO] [autopilot] event PedAppeared handle=11271
    2026-09-29T19:07:10.810Z [INFO] [autopilot] event PedAppeared handle=11526
    2026-09-29T19:07:10.835Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.897, 635.073, 15.185) to=(248.992, 1362.513, 18.434)
    2026-09-29T19:07:10.890Z [INFO] [autopilot] event BulletFired shooter=6916 weapon=15 by_player=False from=(-61.802, 716.918, 105.378) to=(-65.037, 673.218, 13.558)
    2026-09-29T19:07:10.968Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.898, 635.067, 15.197) to=(-81.662, 637.622, 15.161)
    2026-09-29T19:07:11.148Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.881, 635.064, 15.191) to=(-64.417, 674.811, 15.299)
    2026-09-29T19:07:11.149Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=5127 vehicle=0 killed=False hit=True at=(-64.417, 674.811, 15.299) dir=(0.421, 0.907, 0.002)
    2026-09-29T19:07:11.311Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.88, 635.055, 15.192) to=(243.704, 1364.863, 9.712)
    2026-09-29T19:07:11.409Z [INFO] [autopilot] event PedRemoved handle=10246
    2026-09-29T19:07:11.528Z [INFO] [autopilot] event PedRemoved handle=9734
    2026-09-29T19:07:11.594Z [INFO] [autopilot] event PedRemoved handle=8455
    2026-09-29T19:07:11.644Z [INFO] [autopilot] event PedRemoved handle=11526
    2026-09-29T19:07:11.925Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:07:12.015Z [INFO] [autopilot] event BulletFired shooter=2311 weapon=15 by_player=False from=(-32.584, 664.586, 100.132) to=(-64.178, 674.654, 13.575)
    2026-09-29T19:07:12.221Z [INFO] [autopilot] event BulletFired shooter=2311 weapon=15 by_player=False from=(-33.061, 664.987, 94.328) to=(-64.025, 674.905, 13.584)
    2026-09-29T19:07:12.324Z [INFO] [autopilot] event PedAppeared handle=9735
    2026-09-29T19:07:12.359Z [INFO] [autopilot] event PedAppeared handle=10247
    2026-09-29T19:07:12.412Z [INFO] [autopilot] event BulletFired shooter=2311 weapon=15 by_player=False from=(-33.524, 665.382, 88.25) to=(-67.077, 675.574, 14.519)
    2026-09-29T19:07:12.585Z [INFO] [autopilot] event BulletFired shooter=6916 weapon=15 by_player=False from=(-62.592, 720.995, 103.388) to=(-64.635, 672.13, 13.557)
    2026-09-29T19:07:12.616Z [INFO] [autopilot] event BulletFired shooter=2311 weapon=15 by_player=False from=(-33.991, 665.782, 81.689) to=(-65.339, 674.418, 13.558)
    2026-09-29T19:07:12.651Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-45.52, 663.499, 15.02) to=(-726.633, 1084.383, 6.034)
    2026-09-29T19:07:12.696Z [INFO] command source=file:cmd_20260929190712656.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T19:07:12.786Z [INFO] [autopilot] event PedAppeared handle=10504
    2026-09-29T19:07:12.787Z [INFO] [autopilot] event PedRemoved handle=9480
    2026-09-29T19:07:12.806Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.263, 661.174, 14.964) to=(-45.493, 663.043, 15.051)
    2026-09-29T19:07:12.826Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-45.522, 663.484, 15.079) to=(-64.284, 674.869, 14.724)
    2026-09-29T19:07:12.827Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-64.284, 674.869, 14.724) dir=(-0.855, 0.519, -0.016)
    2026-09-29T19:07:12.980Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.262, 661.158, 15.041) to=(-45.534, 663.02, 15.03)
    2026-09-29T19:07:12.981Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-45.515, 663.486, 15.19) to=(-736.754, 1067.691, 16.342)
    2026-09-29T19:07:13.073Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.633, 636.936, 15.201) to=(306.048, 1334.787, 4.312)
    2026-09-29T19:07:13.094Z [INFO] [autopilot] event VehicleDamaged handle=3332 966->938 engine 950->950
    2026-09-29T19:07:13.134Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.254, 661.163, 15.147) to=(-740.935, 1051.856, -7.331)
    2026-09-29T19:07:13.135Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-45.521, 663.482, 15.166) to=(-67.077, 675.645, 14.688)
    2026-09-29T19:07:13.162Z [INFO] [autopilot] event PedAppeared handle=11527
    2026-09-29T19:07:13.186Z [INFO] [autopilot] event VehicleDamaged handle=3332 938->921 engine 950->950
    2026-09-29T19:07:13.209Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.643, 636.928, 15.193) to=(-67.549, 668.407, 14.662)
    2026-09-29T19:07:13.215Z [INFO] command source=file:cmd_20260929190713046.cmd line="weather 1" reply="weather 1"
    2026-09-29T19:07:13.317Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.266, 661.168, 15.077) to=(-67.077, 675.019, 14.623)
    2026-09-29T19:07:13.318Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-45.523, 663.505, 15.068) to=(-732.925, 1073.898, 10.934)
    2026-09-29T19:07:13.393Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.639, 636.944, 15.201) to=(-67.549, 668.338, 14.536)
    2026-09-29T19:07:13.394Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-46.836, 657.741, 15.143) to=(-49.204, 659.828, 15.117)
    2026-09-29T19:07:13.395Z [INFO] [autopilot] event PedAppeared handle=11783
    2026-09-29T19:07:13.396Z [INFO] [autopilot] event PedRemoved handle=10504
    2026-09-29T19:07:13.396Z [INFO] [autopilot] event VehicleDamaged handle=2053 1000->988 engine 1000->1000
    2026-09-29T19:07:13.462Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.263, 661.179, 15.012) to=(-734.825, 1062.787, 9.745)
    2026-09-29T19:07:13.463Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-45.512, 663.509, 15.054) to=(-67.077, 676.698, 14.713)
    2026-09-29T19:07:13.468Z [INFO] command source=file:cmd_20260929190713438.cmd line="give 14 120" reply="gave 14"
    2026-09-29T19:07:13.492Z [INFO] [autopilot] event PlayerWeaponChanged 7->14
    2026-09-29T19:07:13.495Z [INFO] weapon_changed from=7 to=14 profile=vanilla
    2026-09-29T19:07:13.546Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.646, 636.952, 15.197) to=(-67.549, 668.438, 14.691)
    2026-09-29T19:07:13.546Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.915, 635.042, 15.181) to=(240.506, 1366.269, 13.23)
    2026-09-29T19:07:13.587Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-46.59, 657.576, 15.183) to=(-49.232, 660.126, 15.074)
    2026-09-29T19:07:13.588Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=False
    2026-09-29T19:07:13.589Z [INFO] [autopilot] event VehicleDamaged handle=2053 988->976 engine 1000->1000
    2026-09-29T19:07:13.656Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.634, 636.956, 15.2) to=(310.737, 1331.975, 3.322)
    2026-09-29T19:07:13.684Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.888, 635.066, 15.195) to=(-81.781, 637.558, 15.161)
    2026-09-29T19:07:13.685Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-46.385, 657.492, 15.176) to=(-633.591, 1201.761, 13.035)
    2026-09-29T19:07:13.822Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-45.21, 657.47, 15.15) to=(-644.576, 1188.485, 22.938)
    2026-09-29T19:07:13.876Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.893, 635.068, 15.186) to=(246.397, 1363.411, -7.353)
    2026-09-29T19:07:13.877Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-46.118, 657.398, 15.18) to=(-49.504, 660.561, 15.04)
    2026-09-29T19:07:13.878Z [INFO] [autopilot] event VehicleDamaged handle=2053 976->958 engine 1000->1000
    2026-09-29T19:07:13.965Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-45.04, 657.645, 15.174) to=(-45.288, 657.863, 15.193)
    2026-09-29T19:07:13.966Z [INFO] [autopilot] event VehicleDamaged handle=3844 1000->994 engine 1000->1000
    2026-09-29T19:07:13.992Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-45.926, 657.301, 15.144) to=(-50.546, 661.487, 15.007)
    2026-09-29T19:07:13.993Z [INFO] [autopilot] event PedAppeared handle=8456
    2026-09-29T19:07:13.993Z [INFO] [autopilot] event PedRemoved handle=11783
    2026-09-29T19:07:13.994Z [INFO] [autopilot] event VehicleDamaged handle=2053 958->952 engine 1000->1000
    2026-09-29T19:07:13.999Z [INFO] command source=file:cmd_20260929190713831.cmd line="spawncar admiral 8" reply="spawning admiral"
    2026-09-29T19:07:14.022Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.896, 635.063, 15.196) to=(-77.76, 646.376, 14.944)
    2026-09-29T19:07:14.093Z [INFO] [autopilot] autopilot_spawncar handle=7429 model=admiral
    2026-09-29T19:07:14.120Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-44.807, 657.887, 15.169) to=(-45.01, 658.099, 15.195)
    2026-09-29T19:07:14.121Z [INFO] [autopilot] event VehicleDamaged handle=3844 994->988 engine 1000->1000
    2026-09-29T19:07:14.122Z [INFO] [autopilot] event VehicleAppeared handle=7429
    2026-09-29T19:07:14.187Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.883, 635.068, 15.191) to=(253.158, 1360.626, 23.552)
    2026-09-29T19:07:14.364Z [INFO] [autopilot] event PedRemoved handle=8456
    2026-09-29T19:07:14.419Z [INFO] [autopilot] event PedDamaged handle=1798 100->-100 bone=0xFFFFFFFF by_player=False weapon=51 exact=True type=Explosion amount=14998.5 health_lost=200.0 armour_lost=100.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T19:07:14.420Z [INFO] [autopilot] event PedDied handle=1798 bone=0xFFFFFFFF by_player=False exact=True type=Explosion killer=0 weapon=51
    2026-09-29T19:07:14.420Z [INFO] [autopilot] event PedDamaged handle=2055 100->-100 bone=0xFFFFFFFF by_player=False weapon=51 exact=True type=Explosion amount=14998.5 health_lost=200.0 armour_lost=100.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T19:07:14.421Z [INFO] [autopilot] event PedDied handle=2055 bone=0xFFFFFFFF by_player=False exact=True type=Explosion killer=0 weapon=51
    2026-09-29T19:07:14.422Z [INFO] [autopilot] event PedDamaged handle=2311 100->-100 bone=0xFFFFFFFF by_player=False weapon=51 exact=True type=Explosion amount=14998.5 health_lost=200.0 armour_lost=100.0 attacker=0 vehicle=0 killed=True hit=False
    2026-09-29T19:07:14.422Z [INFO] [autopilot] event PedDied handle=2311 bone=0xFFFFFFFF by_player=False exact=True type=Explosion killer=0 weapon=51
    2026-09-29T19:07:14.423Z [INFO] [autopilot] event VehicleDamaged handle=3332 921->0 engine 950->529
    2026-09-29T19:07:14.450Z [INFO] [autopilot] event PedAppeared handle=8713
    2026-09-29T19:07:14.477Z [INFO] [autopilot] event PedDamaged handle=2055 -100->-100 bone=0xFFFFFFFF by_player=False weapon=51 exact=True type=Explosion amount=433.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T19:07:14.478Z [INFO] [autopilot] event PedDamaged handle=2311 -100->-100 bone=0xFFFFFFFF by_player=False weapon=51 exact=True type=Explosion amount=433.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T19:07:14.540Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.6, 636.926, 15.185) to=(-67.552, 670.288, 14.881)
    2026-09-29T19:07:14.636Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 529->517
    2026-09-29T19:07:14.665Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 517->504
    2026-09-29T19:07:14.697Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-86.634, 636.924, 15.199) to=(317.184, 1328.088, 1.376)
    2026-09-29T19:07:14.832Z [INFO] [autopilot] event PedAppeared handle=9481
    2026-09-29T19:07:14.833Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 504->432
    2026-09-29T19:07:15.058Z [INFO] [autopilot] event PedAppeared handle=10505
    2026-09-29T19:07:15.059Z [INFO] [autopilot] event PedRemoved handle=8713
    2026-09-29T19:07:15.089Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 432->425
    2026-09-29T19:07:15.210Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 425->392
    2026-09-29T19:07:15.211Z [INFO] [autopilot] event VehicleDamaged handle=3844 988->961 engine 1000->960
    2026-09-29T19:07:15.486Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.913, 635.044, 15.185) to=(248.829, 1362.453, 15.191)
    2026-09-29T19:07:15.563Z [INFO] [autopilot] event PedRemoved handle=12036
    2026-09-29T19:07:15.563Z [INFO] [autopilot] event VehicleRemoved handle=2309
    2026-09-29T19:07:15.564Z [INFO] [autopilot] event VehicleRemoved handle=2564
    2026-09-29T19:07:15.610Z [INFO] [autopilot] event VehicleRemoved handle=4101
    2026-09-29T19:07:15.611Z [INFO] [autopilot] event VehicleRemoved handle=3589
    2026-09-29T19:07:15.664Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.886, 635.068, 15.198) to=(-81.772, 637.584, 15.161)
    2026-09-29T19:07:15.665Z [INFO] [autopilot] event PedRemoved handle=9481
    2026-09-29T19:07:15.665Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 392->375
    2026-09-29T19:07:15.695Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 375->354
    2026-09-29T19:07:15.696Z [INFO] [autopilot] event VehicleRemoved handle=5381
    2026-09-29T19:07:15.799Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-82.892, 635.069, 15.187) to=(250.483, 1361.625, 1.446)
    2026-09-29T19:07:15.834Z [INFO] [autopilot] event PedRemoved handle=10505
    2026-09-29T19:07:16.169Z [INFO] command source=file:cmd_20260929190715753.cmd line="at-trunk 3.0" reply="at trunk of 7429 (-61.456, 682.84, 14.658)"
    2026-09-29T19:07:16.195Z [INFO] [autopilot] event PedRemoved handle=9992
    2026-09-29T19:07:16.196Z [INFO] [autopilot] event VehicleAppeared handle=2310
    2026-09-29T19:07:16.197Z [INFO] [autopilot] event VehicleAppeared handle=2565
    2026-09-29T19:07:16.197Z [INFO] [autopilot] event VehicleAppeared handle=3590
    2026-09-29T19:07:16.198Z [INFO] [autopilot] event VehicleAppeared handle=6916
    2026-09-29T19:07:16.199Z [INFO] [autopilot] event VehicleAppeared handle=8453
    2026-09-29T19:07:16.320Z [INFO] [autopilot] event PedRemoved handle=7941
    2026-09-29T19:07:17.130Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 354->330
    2026-09-29T19:07:18.415Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-99.568, 672.83, 13.871) to=(-36.623, 689.105, 17.297)
    2026-09-29T19:07:18.518Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 330->306
    2026-09-29T19:07:18.609Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 306->275
    2026-09-29T19:07:18.910Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-98.763, 673.032, 13.925) to=(-81.467, 677.754, 14.547)
    2026-09-29T19:07:19.053Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 275->183
    2026-09-29T19:07:19.453Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 183->168
    2026-09-29T19:07:19.993Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-99.73, 673.273, 13.851) to=(-36.399, 688.005, 17.249)
    2026-09-29T19:07:19.994Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 168->156
    2026-09-29T19:07:20.035Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 156->140
    2026-09-29T19:07:20.072Z [INFO] [autopilot] event PedRemoved handle=7174
    2026-09-29T19:07:20.073Z [INFO] [autopilot] event PedRemoved handle=6916
    2026-09-29T19:07:20.074Z [INFO] [autopilot] event PedRemoved handle=6663
    2026-09-29T19:07:20.148Z [INFO] [autopilot] event PedAppeared handle=4875
    2026-09-29T19:07:20.219Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.989, 660.968, 15.038) to=(-61.394, 682.725, 15.189)
    2026-09-29T19:07:20.220Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.394, 682.725, 15.189) dir=(-0.397, 0.918, 0.006)
    2026-09-29T19:07:20.259Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 140->43
    2026-09-29T19:07:20.298Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.353, 673.486, 13.798) to=(-36.695, 686.945, 16.481)
    2026-09-29T19:07:20.368Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.996, 660.931, 15.024) to=(-364.596, 1397.732, 36.012)
    2026-09-29T19:07:20.633Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.98, 660.934, 15.042) to=(-365.462, 1397.357, 36.156)
    2026-09-29T19:07:20.634Z [INFO] [autopilot] event PedRemoved handle=11527
    2026-09-29T19:07:20.996Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 43->19
    2026-09-29T19:07:21.179Z [INFO] [autopilot] event PedAppeared handle=5899
    2026-09-29T19:07:21.952Z [INFO] performance samples=1134 frame_p50_ms=21 frame_p95_ms=54 frame_p99_ms=106 frames_over_33ms=180 frames_over_50ms=60 gunplay_avg_ms=1.228 gunplay_max_ms=6.503 phase_samples=1134 phase_setup_avg_ms=0.804 phase_setup_max_ms=6.031 phase_camera_avg_ms=0.021 phase_camera_max_ms=0.683 phase_bullets_avg_ms=0.005 phase_bullets_max_ms=0.218 phase_weapon_avg_ms=0.018 phase_weapon_max_ms=0.115 phase_hud_avg_ms=0.379 phase_hud_max_ms=1.306
    2026-09-29T19:07:21.953Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.973/336.9/1134@7 total=3371 module.gunplay=1.236/6.5/1134@7 total=1401 tick.gunplay=1.234/6.5/1134@7 total=1400 gp.freeaim=0.662/5.9/1134@7 total=751 module.arsenal=0.602/5.0/720@7 total=433 tick.arsenal=0.601/5.0/720@7 total=433 engine.world=0.336/10.8/1134@7 total=382 ar.storage=0.396/4.9/720@7 total=285 module.atmosphere=0.168/3.4/1134@7 total=190 tick.atmosphere=0.166/3.4/1134@7 total=189 module.combat=0.118/3.6/1134@7 total=134 tick.combat=0.117/3.6/1134@7 total=133 ar.safehouse_flags=0.175/2.6/720@7 total=126 combat.sample=1.513/3.6/71@7 total=107 gp.index_pad=0.083/0.3/1134@7 total=94 module.holsters=0.140/31.0/404@7 total=56 tick.holsters=0.139/31.0/404@7 total=56 ho.show=0.088/31.0/404@7 total=35 module.devtools=0.028/3.5/720@7 total=20 tick.devtools=0.028/3.5/720@7 total=20 cam.handle=0.015/0.7/1134@7 total=17 gp.shoulder=0.015/0.1/1134@7 total=17 gp.weapon_id=0.013/0.9/1134@7 total=15 gp.player=0.013/0.1/1134@7 total=14 gp.cycle=0.008/0.1/1134@7 total=9 module.world=0.142/0.8/58@7 total=8 gp.state=0.006/0.1/1134@7 total=7 module.probe=1.524/1.8/3@7 total=5 engine.scheduler=0.003/2.6/1134@7 total=4 cam.aim_key=0.003/0.0/1134@7 total=3 cam.find_active=0.002/0.0/1134@7 total=3 ar.reconcile=0.003/0.2/720@7 total=2 ar.vehicle=0.003/0.0/720@7 total=2 gp.spread=0.002/0.0/1134@7 total=2 gp.shots=0.002/0.0/1134@7 total=2 ar.lvs=0.003/0.3/720@7 total=2 ar.discover=0.002/0.5/720@7 total=2 ho.carried=0.002/0.0/404@7 total=1 combat.dismember=0.001/0.0/1134@7 total=1 module.autopilot=0.000/0.1/1134@7 total=0 combat.blood=0.000/0.0/1134@7 total=0 gp.feel=0.000/0.0/1134@7 total=0 module.weapon-probe=0.000/0.0/1134@7 total=0 combat.pending=0.000/0.0/1134@7 total=0 gp.recoil=0.000/0.0/1134@7 total=0 cam.fov=0.001/0.0/112@7 total=0
    2026-09-29T19:07:21.954Z [INFO] engine_thread_probe ticks=1134 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1132 ticks_after_skipped_frames=2
    2026-09-29T19:07:21.955Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.19 shdn_us=27.8
    2026-09-29T19:07:21.958Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:07:21.959Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=8716 core=on peds=24 vehicles=16 modules=10/10 coroutines=0 resources=4 raycast=on episode=GTAIV frame_ms=73.74 p95_ms=99.10 pressure=0.98 private_mb=2217 working_set_mb=1459 address_free_mb=1217 largest_free_block_mb=1183 managed_mb=14 physical_load=95% core_us=75.0
    2026-09-29T19:07:22.062Z [INFO] density frame_ms=85.4 peds=0.55 cars=0.60
    2026-09-29T19:07:22.241Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-46.111, 657.169, 15.042) to=(-451.201, 1347.86, 16.585)
    2026-09-29T19:07:22.340Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.462, 675.125, 13.906) to=(-37.51, 687.381, 14.601)
    2026-09-29T19:07:22.341Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 19->0
    2026-09-29T19:07:22.438Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-46.118, 657.174, 15.052) to=(-451.1, 1347.721, 31.805)
    2026-09-29T19:07:22.438Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.281, 660.763, 15.114) to=(-61.392, 682.711, 15.231)
    2026-09-29T19:07:22.439Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.392, 682.711, 15.231) dir=(-0.347, 0.938, 0.005)
    2026-09-29T19:07:22.440Z [INFO] [autopilot] event PedRemoved handle=10759
    2026-09-29T19:07:22.441Z [INFO] [autopilot] event PedRemoved handle=11013
    2026-09-29T19:07:22.724Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-46.117, 657.162, 15.046) to=(-46.982, 658.591, 15.06)
    2026-09-29T19:07:22.725Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.577, 660.657, 15.118) to=(-91.431, 764.025, 18.787)
    2026-09-29T19:07:22.725Z [INFO] [autopilot] event VehicleDamaged handle=3844 961->955 engine 960->960
    2026-09-29T19:07:22.726Z [INFO] [autopilot] event VehicleRemoved handle=11780
    2026-09-29T19:07:22.820Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.444, 675.994, 13.792) to=(-37.056, 685.183, 17.181)
    2026-09-29T19:07:22.926Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.769, 660.636, 15.105) to=(-91.517, 764.283, 19.11)
    2026-09-29T19:07:23.222Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.102, 660.595, 15.129) to=(-313.204, 1418.29, 4.967)
    2026-09-29T19:07:23.323Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.481, 676.713, 13.886) to=(-37.077, 685.699, 17.188)
    2026-09-29T19:07:23.479Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.328, 660.551, 15.115) to=(-87.714, 764.282, 19.044)
    2026-09-29T19:07:24.071Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine 0->-4
    2026-09-29T19:07:24.073Z [INFO] [autopilot] event VehicleDestroyed handle=3332
    2026-09-29T19:07:24.113Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -4->-7
    2026-09-29T19:07:24.226Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -7->-10
    2026-09-29T19:07:24.262Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -10->-14
    2026-09-29T19:07:24.298Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -14->-17
    2026-09-29T19:07:24.336Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -17->-19
    2026-09-29T19:07:24.372Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -19->-22
    2026-09-29T19:07:24.408Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -22->-26
    2026-09-29T19:07:24.442Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -26->-29
    2026-09-29T19:07:24.478Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -29->-30
    2026-09-29T19:07:24.512Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-55.624, 660.316, 15.076) to=(-249.269, 1437.195, 16.124)
    2026-09-29T19:07:24.513Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -30->-32
    2026-09-29T19:07:24.548Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -32->-34
    2026-09-29T19:07:24.581Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -34->-35
    2026-09-29T19:07:24.614Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -35->-38
    2026-09-29T19:07:24.648Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -38->-40
    2026-09-29T19:07:24.681Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-55.885, 660.207, 15.082) to=(-61.692, 682.672, 15.022)
    2026-09-29T19:07:24.682Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.692, 682.672, 15.022) dir=(-0.25, 0.968, -0.003)
    2026-09-29T19:07:24.682Z [INFO] [autopilot] event PedRemoved handle=9229
    2026-09-29T19:07:24.683Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -40->-43
    2026-09-29T19:07:24.716Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.655, 663.288, 15.152) to=(-658.914, 1167.015, 20.079)
    2026-09-29T19:07:24.717Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -43->-46
    2026-09-29T19:07:24.756Z [INFO] [autopilot] event PedRemoved handle=4875
    2026-09-29T19:07:24.757Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -46->-49
    2026-09-29T19:07:24.799Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -49->-53
    2026-09-29T19:07:24.840Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.097, 660.154, 15.067) to=(-229.924, 1441.664, 21.983)
    2026-09-29T19:07:24.841Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -53->-57
    2026-09-29T19:07:24.879Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.563, 663.107, 15.202) to=(-667.245, 1156.192, 25.142)
    2026-09-29T19:07:24.880Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -57->-60
    2026-09-29T19:07:24.917Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -60->-62
    2026-09-29T19:07:24.953Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.563, 678.849, 13.897) to=(-36.917, 686.219, 16.254)
    2026-09-29T19:07:24.954Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -62->-65
    2026-09-29T19:07:24.989Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -65->-67
    2026-09-29T19:07:25.017Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.425, 660.112, 15.084) to=(-71.284, 724.902, 15.706)
    2026-09-29T19:07:25.018Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -67->-69
    2026-09-29T19:07:25.049Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.543, 663.102, 15.195) to=(-663.359, 1161.17, 12.218)
    2026-09-29T19:07:25.050Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -69->-71
    2026-09-29T19:07:25.078Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -71->-73
    2026-09-29T19:07:25.110Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -73->-74
    2026-09-29T19:07:25.139Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -74->-77
    2026-09-29T19:07:25.168Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -77->-79
    2026-09-29T19:07:25.198Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.547, 663.107, 15.208) to=(-162.996, 759.96, 16.956)
    2026-09-29T19:07:25.199Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.761, 660.059, 15.073) to=(-226.825, 1442.454, 18.325)
    2026-09-29T19:07:25.200Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -79->-81
    2026-09-29T19:07:25.229Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -81->-84
    2026-09-29T19:07:25.259Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -84->-85
    2026-09-29T19:07:25.304Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.542, 678.948, 13.929) to=(-36.73, 684.495, 16.723)
    2026-09-29T19:07:25.305Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -85->-89
    2026-09-29T19:07:25.340Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -89->-92
    2026-09-29T19:07:25.371Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.555, 663.092, 15.202) to=(-659.127, 1166.477, 16.139)
    2026-09-29T19:07:25.372Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.999, 659.988, 15.058) to=(-205.357, 1446.903, 6.03)
    2026-09-29T19:07:25.373Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -92->-93
    2026-09-29T19:07:25.402Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -93->-94
    2026-09-29T19:07:25.451Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -94->-97
    2026-09-29T19:07:25.562Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -97->-100
    2026-09-29T19:07:25.654Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -100->-104
    2026-09-29T19:07:25.751Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.167, 644.499, 15.163) to=(300.765, 1348.081, 21.036)
    2026-09-29T19:07:25.752Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -104->-108
    2026-09-29T19:07:25.864Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.529, 678.973, 13.929) to=(-67.552, 681.403, 14.879)
    2026-09-29T19:07:25.865Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -108->-112
    2026-09-29T19:07:25.971Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -112->-114
    2026-09-29T19:07:26.078Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.021, 644.777, 15.192) to=(-31.421, 734.587, 16.563)
    2026-09-29T19:07:26.079Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -114->-119
    2026-09-29T19:07:26.180Z [INFO] [autopilot] event PedRemoved handle=7695
    2026-09-29T19:07:26.181Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -119->-123
    2026-09-29T19:07:26.273Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.002, 644.817, 15.202) to=(300.982, 1348.343, 30.023)
    2026-09-29T19:07:26.274Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.689, 659.721, 15.037) to=(-594.794, 1236.518, 44.538)
    2026-09-29T19:07:26.275Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -123->-126
    2026-09-29T19:07:26.370Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -126->-131
    2026-09-29T19:07:26.461Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -131->-137
    2026-09-29T19:07:26.557Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.012, 644.82, 15.197) to=(-31.37, 743.11, 15.961)
    2026-09-29T19:07:26.558Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.423, 659.39, 15.05) to=(-114.359, 737.56, 15.914)
    2026-09-29T19:07:26.558Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -137->-142
    2026-09-29T19:07:26.649Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -142->-146
    2026-09-29T19:07:26.741Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.011, 644.828, 15.199) to=(-67.437, 670.165, 15.003)
    2026-09-29T19:07:26.742Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.257, 659.203, 14.994) to=(-575.996, 1252.898, 41.747)
    2026-09-29T19:07:26.743Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -146->-149
    2026-09-29T19:07:26.832Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -149->-155
    2026-09-29T19:07:26.833Z [INFO] [autopilot] event VehicleRemoved handle=5893
    2026-09-29T19:07:26.927Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -155->-157
    2026-09-29T19:07:27.022Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.201, 658.977, 15) to=(-66.99, 689.789, 15.098)
    2026-09-29T19:07:27.023Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -157->-160
    2026-09-29T19:07:27.115Z [INFO] [autopilot] event PedRemoved handle=5899
    2026-09-29T19:07:27.115Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -160->-166
    2026-09-29T19:07:27.209Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.234, 658.825, 15.014) to=(-584.133, 1245.095, 40.319)
    2026-09-29T19:07:27.210Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-58.898, 659.633, 14.988) to=(-141.387, 1456.061, 15.283)
    2026-09-29T19:07:27.211Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -166->-170
    2026-09-29T19:07:27.296Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -170->-175
    2026-09-29T19:07:27.382Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -175->-181
    2026-09-29T19:07:27.471Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.541, 678.943, 13.923) to=(-36.899, 686.322, 16.441)
    2026-09-29T19:07:27.472Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-59.255, 659.568, 14.996) to=(-116.494, 1458.354, 16.225)
    2026-09-29T19:07:27.473Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -181->-185
    2026-09-29T19:07:27.559Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -185->-188
    2026-09-29T19:07:27.645Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-59.432, 659.53, 14.962) to=(-132.606, 1456.465, 39.522)
    2026-09-29T19:07:27.646Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -188->-193
    2026-09-29T19:07:27.737Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -193->-196
    2026-09-29T19:07:27.840Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -196->-201
    2026-09-29T19:07:27.934Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-59.719, 659.442, 14.967) to=(-126.115, 1457.138, 31.582)
    2026-09-29T19:07:27.935Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -201->-208
    2026-09-29T19:07:28.034Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -208->-211
    2026-09-29T19:07:28.130Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -211->-217
    2026-09-29T19:07:28.231Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-60.03, 659.403, 14.956) to=(-61.557, 682.67, 15.053)
    2026-09-29T19:07:28.232Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.557, 682.67, 15.053) dir=(-0.065, 0.998, 0.004)
    2026-09-29T19:07:28.232Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -217->-221
    2026-09-29T19:07:28.414Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -221->-226
    2026-09-29T19:07:28.430Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-60.247, 659.383, 14.966) to=(-61.542, 682.675, 15.086)
    2026-09-29T19:07:28.431Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.542, 682.675, 15.086) dir=(-0.055, 0.998, 0.005)
    2026-09-29T19:07:28.432Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -226->-229
    2026-09-29T19:07:28.527Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-37.036, 668.005, 14.968) to=(-61.309, 682.936, 14.796)
    2026-09-29T19:07:28.528Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.309, 682.936, 14.796) dir=(-0.852, 0.524, -0.006)
    2026-09-29T19:07:28.529Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -229->-234
    2026-09-29T19:07:28.628Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -234->-239
    2026-09-29T19:07:28.721Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -239->-245
    2026-09-29T19:07:28.814Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -245->-251
    2026-09-29T19:07:28.905Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -251->-254
    2026-09-29T19:07:28.995Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.546, 678.946, 13.927) to=(-36.795, 685.612, 15.273)
    2026-09-29T19:07:28.996Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -254->-257
    2026-09-29T19:07:29.058Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -257->-260
    2026-09-29T19:07:29.088Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-80.978, 644.836, 15.186) to=(-67.544, 672.121, 14.931)
    2026-09-29T19:07:29.089Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -260->-262
    2026-09-29T19:07:29.128Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -262->-263
    2026-09-29T19:07:29.162Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -263->-266
    2026-09-29T19:07:29.194Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -266->-269
    2026-09-29T19:07:29.227Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -269->-271
    2026-09-29T19:07:29.255Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.009, 644.819, 15.201) to=(-30.871, 747, 16.248)
    2026-09-29T19:07:29.256Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -271->-273
    2026-09-29T19:07:29.285Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-101.549, 678.946, 13.928) to=(-36.698, 684.179, 16.466)
    2026-09-29T19:07:29.286Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -273->-274
    2026-09-29T19:07:29.316Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -274->-277
    2026-09-29T19:07:29.345Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-61.37, 659.202, 15.093) to=(-37.256, 1459.772, 23.241)
    2026-09-29T19:07:29.346Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -277->-280
    2026-09-29T19:07:29.375Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -280->-281
    2026-09-29T19:07:29.405Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.006, 644.815, 15.192) to=(270.118, 1364.524, 12.646)
    2026-09-29T19:07:29.406Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -281->-282
    2026-09-29T19:07:29.438Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -282->-285
    2026-09-29T19:07:29.466Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -285->-287
    2026-09-29T19:07:29.496Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -287->-288
    2026-09-29T19:07:29.526Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-61.631, 659.124, 15.097) to=(-66.617, 1459.719, 25.685)
    2026-09-29T19:07:29.527Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -288->-290
    2026-09-29T19:07:29.557Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.002, 644.818, 15.205) to=(-67.564, 670.343, 14.912)
    2026-09-29T19:07:29.558Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -290->-292
    2026-09-29T19:07:29.593Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -292->-294
    2026-09-29T19:07:29.622Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -294->-295
    2026-09-29T19:07:29.653Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -295->-297
    2026-09-29T19:07:29.683Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-61.82, 659.054, 15.125) to=(-43.059, 1459.218, 39.347)
    2026-09-29T19:07:29.684Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -297->-299
    2026-09-29T19:07:29.718Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -299->-300
    2026-09-29T19:07:29.747Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.009, 644.823, 15.196) to=(279.423, 1359.751, 8.906)
    2026-09-29T19:07:29.748Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -300->-302
    2026-09-29T19:07:29.779Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -302->-304
    2026-09-29T19:07:29.809Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -304->-306
    2026-09-29T19:07:29.838Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-62.017, 659.012, 15.132) to=(-60.97, 1459.838, 17.437)
    2026-09-29T19:07:29.839Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -306->-308
    2026-09-29T19:07:29.871Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -308->-310
    2026-09-29T19:07:29.905Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-37.128, 667.758, 15.103) to=(-713.133, 1096.407, 37.226)
    2026-09-29T19:07:29.906Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -310->-313
    2026-09-29T19:07:29.940Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -313->-315
    2026-09-29T19:07:29.971Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -315->-317
    2026-09-29T19:07:30.007Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-62.25, 658.978, 15.104) to=(-61.346, 682.694, 15.557)
    2026-09-29T19:07:30.008Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.346, 682.694, 15.557) dir=(0.038, 0.999, 0.019)
    2026-09-29T19:07:30.009Z [INFO] [autopilot] event VehicleDamaged handle=3076 1000->983 engine 1000->975
    2026-09-29T19:07:30.010Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -317->-321
    2026-09-29T19:07:30.045Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -321->-323
    2026-09-29T19:07:30.078Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.919, 667.535, 15.113) to=(-722.347, 1081.183, 28.841)
    2026-09-29T19:07:30.079Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -323->-327
    2026-09-29T19:07:30.120Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -327->-329
    2026-09-29T19:07:30.166Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -329->-332
    2026-09-29T19:07:30.200Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -332->-334
    2026-09-29T19:07:30.241Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.725, 667.348, 15.09) to=(-61.301, 682.784, 15.572)
    2026-09-29T19:07:30.242Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.301, 682.784, 15.572) dir=(-0.847, 0.532, 0.017)
    2026-09-29T19:07:30.243Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -334->-337
    2026-09-29T19:07:30.321Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -337->-341
    2026-09-29T19:07:30.416Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -341->-345
    2026-09-29T19:07:30.513Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.65, 667.158, 15.093) to=(-719.105, 1086.003, 9.265)
    2026-09-29T19:07:30.514Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -345->-350
    2026-09-29T19:07:30.611Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -350->-356
    2026-09-29T19:07:30.703Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -356->-362
    2026-09-29T19:07:30.798Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.502, 666.865, 15.113) to=(-706.077, 1106.059, 2.722)
    2026-09-29T19:07:30.799Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -362->-367
    2026-09-29T19:07:30.890Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -367->-373
    2026-09-29T19:07:30.980Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.355, 666.723, 15.112) to=(-718.658, 1085.837, 16.868)
    2026-09-29T19:07:30.981Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -373->-378
    2026-09-29T19:07:31.076Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -378->-383
    2026-09-29T19:07:31.170Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -383->-386
    2026-09-29T19:07:31.266Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.375, 678.11, 13.882) to=(-35.74, 685.975, 14.764)
    2026-09-29T19:07:31.267Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -386->-392
    2026-09-29T19:07:31.358Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -392->-398
    2026-09-29T19:07:31.445Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -398->-403
    2026-09-29T19:07:31.531Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -403->-408
    2026-09-29T19:07:31.620Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-80.977, 644.829, 15.188) to=(282.773, 1358.032, 11.239)
    2026-09-29T19:07:31.620Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -408->-414
    2026-09-29T19:07:31.712Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-99.502, 678.124, 13.999) to=(-67.549, 681.666, 14.794)
    2026-09-29T19:07:31.713Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.074, 680.625, 14.978) to=(-836.179, 782.348, 24.794)
    2026-09-29T19:07:31.713Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -414->-419
    2026-09-29T19:07:31.797Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -419->-426
    2026-09-29T19:07:31.892Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.011, 644.819, 15.203) to=(-31.37, 742.602, 16.474)
    2026-09-29T19:07:31.893Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.344, 680.648, 15) to=(-836.623, 777.77, 42.112)
    2026-09-29T19:07:31.894Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -426->-429
    2026-09-29T19:07:31.976Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -429->-433
    2026-09-29T19:07:31.981Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:07:32.074Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.001, 644.82, 15.189) to=(-32.262, 741.13, 16.737)
    2026-09-29T19:07:32.075Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.117, 665.958, 15.047) to=(-696.905, 1117.891, 27.312)
    2026-09-29T19:07:32.076Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -433->-438
    2026-09-29T19:07:32.168Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-99.651, 678.897, 13.891) to=(-37.019, 684.265, 16.976)
    2026-09-29T19:07:32.169Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.759, 658.037, 14.7) to=(-578.04, 1250.78, 25.06)
    2026-09-29T19:07:32.170Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.419, 680.679, 15.054) to=(-836.76, 781.627, 15.628)
    2026-09-29T19:07:32.171Z [INFO] [autopilot] event PedAppeared handle=6156
    2026-09-29T19:07:32.171Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -438->-444
    2026-09-29T19:07:32.274Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -444->-446
    2026-09-29T19:07:32.368Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.003, 644.817, 15.204) to=(-62.889, 678.246, 15.634)
    2026-09-29T19:07:32.369Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.103, 665.751, 15.079) to=(-692.032, 1124.943, 20.719)
    2026-09-29T19:07:32.370Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.431, 680.67, 15.011) to=(-836.246, 782.589, 38.078)
    2026-09-29T19:07:32.371Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -446->-451
    2026-09-29T19:07:32.471Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.756, 658.019, 14.755) to=(-568.384, 1258.885, 36.965)
    2026-09-29T19:07:32.472Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -451->-454
    2026-09-29T19:07:32.569Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.012, 644.822, 15.195) to=(292.632, 1352.87, 22.038)
    2026-09-29T19:07:32.569Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.089, 665.687, 15.088) to=(-62.843, 683.016, 14.976)
    2026-09-29T19:07:32.570Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -454->-459
    2026-09-29T19:07:32.571Z [INFO] [autopilot] event VehicleDamaged handle=7429 1000->988 engine 1000->1000
    2026-09-29T19:07:32.679Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.764, 658.023, 14.765) to=(-128.793, 764.083, 16.971)
    2026-09-29T19:07:32.680Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-42.433, 680.673, 15.007) to=(-61.34, 682.725, 15.479)
    2026-09-29T19:07:32.681Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.34, 682.725, 15.479) dir=(-0.994, 0.108, 0.025)
    2026-09-29T19:07:32.682Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -459->-465
    2026-09-29T19:07:32.795Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -465->-468
    2026-09-29T19:07:32.891Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-81.012, 644.818, 15.203) to=(-31.57, 741.653, 17.698)
    2026-09-29T19:07:32.892Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.763, 658.022, 14.764) to=(-129.382, 764.083, 16.984)
    2026-09-29T19:07:32.893Z [INFO] [autopilot] event PedRemoved handle=6156
    2026-09-29T19:07:32.893Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -468->-471
    2026-09-29T19:07:32.993Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -471->-473
    2026-09-29T19:07:33.090Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -473->-476
    2026-09-29T19:07:33.186Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.75, 658.027, 14.751) to=(-574.003, 1254.179, 28.76)
    2026-09-29T19:07:33.187Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -476->-479
    2026-09-29T19:07:33.283Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -479->-482
    2026-09-29T19:07:33.381Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.733, 658.036, 14.735) to=(-128.598, 763.814, 18.245)
    2026-09-29T19:07:33.382Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -482->-484
    2026-09-29T19:07:33.474Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-65.501, 658.72, 14.934) to=(56.761, 1450.205, 11.121)
    2026-09-29T19:07:33.475Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -484->-489
    2026-09-29T19:07:33.567Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -489->-495
    2026-09-29T19:07:33.654Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -495->-499
    2026-09-29T19:07:33.747Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-81.5, 646.133, 15.165) to=(320.493, 1338.509, 8.6)
    2026-09-29T19:07:33.748Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-65.817, 658.759, 14.981) to=(-61.421, 682.865, 14.625)
    2026-09-29T19:07:33.749Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.421, 682.865, 14.625) dir=(0.179, 0.984, -0.015)
    2026-09-29T19:07:33.750Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -499->-503
    2026-09-29T19:07:33.837Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -503->-508
    2026-09-29T19:07:33.869Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-81.251, 646.459, 15.12) to=(-61.574, 682.519, 15.307)
    2026-09-29T19:07:33.870Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-41.971, 680.209, 15.09) to=(-114.001, 689.044, 16.642)
    2026-09-29T19:07:33.870Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-65.942, 658.763, 14.964) to=(76.464, 1446.502, 27.18)
    2026-09-29T19:07:33.871Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=5127 vehicle=0 killed=False hit=True at=(-61.574, 682.519, 15.307) dir=(0.479, 0.878, 0.005)
    2026-09-29T19:07:33.872Z [INFO] [autopilot] event PedAppeared handle=7696
    2026-09-29T19:07:33.872Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -508->-510
    2026-09-29T19:07:33.912Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -510->-512
    2026-09-29T19:07:33.943Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -512->-514
    2026-09-29T19:07:33.980Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -514->-516
    2026-09-29T19:07:34.012Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-80.805, 646.997, 15.148) to=(313.487, 1343.635, 35.862)
    2026-09-29T19:07:34.013Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -516->-517
    2026-09-29T19:07:34.046Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -517->-520
    2026-09-29T19:07:34.080Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -520->-522
    2026-09-29T19:07:34.109Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -522->-524
    2026-09-29T19:07:34.142Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -524->-526
    2026-09-29T19:07:34.174Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.025, 680.102, 13.957) to=(-35.067, 684.455, 15.134)
    2026-09-29T19:07:34.175Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -526->-528
    2026-09-29T19:07:34.210Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -528->-531
    2026-09-29T19:07:34.246Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.69, 658.072, 15.133) to=(-577.795, 1250.918, 24.549)
    2026-09-29T19:07:34.247Z [INFO] [autopilot] event PedRemoved handle=7696
    2026-09-29T19:07:34.248Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -531->-533
    2026-09-29T19:07:34.290Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-79.027, 646.78, 15.132) to=(252.12, 1375.948, 28.04)
    2026-09-29T19:07:34.291Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -533->-537
    2026-09-29T19:07:34.335Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -537->-542
    2026-09-29T19:07:34.374Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -542->-544
    2026-09-29T19:07:34.415Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -544->-547
    2026-09-29T19:07:34.446Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -547->-549
    2026-09-29T19:07:34.486Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -549->-551
    2026-09-29T19:07:34.524Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -551->-554
    2026-09-29T19:07:34.561Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -554->-556
    2026-09-29T19:07:34.597Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -556->-559
    2026-09-29T19:07:34.635Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -559->-560
    2026-09-29T19:07:34.680Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -560->-562
    2026-09-29T19:07:34.712Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -562->-565
    2026-09-29T19:07:34.753Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -565->-566
    2026-09-29T19:07:34.793Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-65.351, 658.663, 14.978) to=(-23.957, 896.322, 24.2)
    2026-09-29T19:07:34.794Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -566->-569
    2026-09-29T19:07:34.832Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -569->-571
    2026-09-29T19:07:34.833Z [INFO] [autopilot] event VehicleRemoved handle=2053
    2026-09-29T19:07:34.868Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -571->-574
    2026-09-29T19:07:34.904Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -574->-577
    2026-09-29T19:07:34.938Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-65.107, 658.582, 14.981) to=(-61.43, 682.646, 15.598)
    2026-09-29T19:07:34.939Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.43, 682.646, 15.598) dir=(0.151, 0.988, 0.025)
    2026-09-29T19:07:34.939Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -577->-579
    2026-09-29T19:07:34.973Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -579->-582
    2026-09-29T19:07:35.006Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -582->-584
    2026-09-29T19:07:35.040Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -584->-585
    2026-09-29T19:07:35.073Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -585->-586
    2026-09-29T19:07:35.107Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-40.986, 680.636, 15.1) to=(-206.916, 700.773, 15.243)
    2026-09-29T19:07:35.108Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-64.842, 658.539, 14.968) to=(61.805, 1448.891, 38.534)
    2026-09-29T19:07:35.108Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -586->-590
    2026-09-29T19:07:35.143Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -590->-593
    2026-09-29T19:07:35.189Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -593->-597
    2026-09-29T19:07:35.280Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -597->-602
    2026-09-29T19:07:35.381Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-40.797, 680.749, 15.086) to=(-838.342, 749.33, 32.301)
    2026-09-29T19:07:35.382Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-64.597, 658.529, 14.983) to=(-61.596, 682.739, 14.949)
    2026-09-29T19:07:35.383Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.596, 682.739, 14.949) dir=(0.123, 0.992, -0.001)
    2026-09-29T19:07:35.384Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -602->-605
    2026-09-29T19:07:35.474Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -605->-608
    2026-09-29T19:07:35.564Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-64.402, 658.521, 14.983) to=(28.011, 1453.817, 18.104)
    2026-09-29T19:07:35.565Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -608->-614
    2026-09-29T19:07:35.654Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.584, 647.603, 15.182) to=(261.665, 1372.867, 11.938)
    2026-09-29T19:07:35.655Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-40.572, 680.93, 15.08) to=(-837.557, 757.702, 15.214)
    2026-09-29T19:07:35.656Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -614->-620
    2026-09-29T19:07:35.745Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -620->-625
    2026-09-29T19:07:35.835Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-40.439, 681.05, 15.089) to=(-61.356, 682.82, 15.785)
    2026-09-29T19:07:35.836Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.356, 682.82, 15.785) dir=(-0.996, 0.084, 0.033)
    2026-09-29T19:07:35.836Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -625->-627
    2026-09-29T19:07:35.922Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.489, 647.547, 15.18) to=(-67.556, 669.245, 14.89)
    2026-09-29T19:07:35.923Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -627->-633
    2026-09-29T19:07:36.012Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -633->-637
    2026-09-29T19:07:36.101Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.428, 647.549, 15.174) to=(261.179, 1372.991, 25.849)
    2026-09-29T19:07:36.102Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-40.189, 681.249, 15.074) to=(-61.554, 682.619, 15.202)
    2026-09-29T19:07:36.103Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.554, 682.619, 15.202) dir=(-0.998, 0.064, 0.006)
    2026-09-29T19:07:36.103Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -637->-643
    2026-09-29T19:07:36.192Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -643->-645
    2026-09-29T19:07:36.282Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -645->-648
    2026-09-29T19:07:36.378Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.424, 647.59, 15.203) to=(249.604, 1378.286, 28.625)
    2026-09-29T19:07:36.379Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -648->-653
    2026-09-29T19:07:36.472Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -653->-655
    2026-09-29T19:07:36.568Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.421, 647.605, 15.191) to=(-62.735, 678.271, 15.495)
    2026-09-29T19:07:36.569Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -655->-657
    2026-09-29T19:07:36.661Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -657->-661
    2026-09-29T19:07:36.763Z [INFO] [autopilot] event PedRemoved handle=5384
    2026-09-29T19:07:36.764Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -661->-667
    2026-09-29T19:07:36.888Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.118, 680.041, 14.041) to=(-35.273, 685.325, 15.598)
    2026-09-29T19:07:36.889Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -667->-673
    2026-09-29T19:07:36.992Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -673->-678
    2026-09-29T19:07:37.087Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -678->-683
    2026-09-29T19:07:37.181Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -683->-689
    2026-09-29T19:07:37.276Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.521, 663.515, 15.124) to=(-61.362, 683.142, 15.295)
    2026-09-29T19:07:37.277Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C9 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.362, 683.142, 15.295) dir=(-0.711, 0.703, 0.006)
    2026-09-29T19:07:37.277Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -689->-696
    2026-09-29T19:07:37.375Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-62.42, 658.276, 15.136) to=(-16.979, 1457.847, -0.612)
    2026-09-29T19:07:37.376Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -696->-702
    2026-09-29T19:07:37.472Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -702->-707
    2026-09-29T19:07:37.569Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.66, 664.079, 15.107) to=(-623.885, 1213.765, 4.929)
    2026-09-29T19:07:37.570Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -707->-710
    2026-09-29T19:07:37.668Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-62.142, 658.241, 15.155) to=(-49.951, 1458.724, 28.796)
    2026-09-29T19:07:37.668Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -710->-714
    2026-09-29T19:07:37.757Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.704, 664.376, 15.131) to=(-619.214, 1219.139, 1.131)
    2026-09-29T19:07:37.757Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-38.928, 682.297, 14.995) to=(-61.467, 682.568, 15.411)
    2026-09-29T19:07:37.758Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.467, 682.568, 15.411) dir=(-1, 0.012, 0.018)
    2026-09-29T19:07:37.759Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -714->-718
    2026-09-29T19:07:38.039Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-61.944, 658.243, 15.141) to=(-31.222, 1458.416, 10.088)
    2026-09-29T19:07:38.040Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -718->-722
    2026-09-29T19:07:38.131Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -722->-724
    2026-09-29T19:07:38.220Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.719, 664.791, 15.127) to=(-109.164, 728.254, 17.467)
    2026-09-29T19:07:38.221Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-38.659, 682.48, 14.997) to=(-839.153, 692.09, 23.094)
    2026-09-29T19:07:38.222Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -724->-730
    2026-09-29T19:07:38.307Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-61.623, 658.191, 15.117) to=(-46.448, 1458.81, 2.403)
    2026-09-29T19:07:38.308Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -730->-737
    2026-09-29T19:07:38.401Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.685, 665.066, 15.124) to=(-61.348, 682.779, 15.629)
    2026-09-29T19:07:38.402Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-38.515, 682.606, 14.951) to=(-838.675, 714.281, 32.895)
    2026-09-29T19:07:38.403Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.348, 682.779, 15.629) dir=(-0.743, 0.669, 0.019)
    2026-09-29T19:07:38.403Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -737->-743
    2026-09-29T19:07:38.493Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-61.427, 658.153, 15.06) to=(-53.301, 1458.365, 46.677)
    2026-09-29T19:07:38.494Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -743->-748
    2026-09-29T19:07:38.592Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -748->-754
    2026-09-29T19:07:38.684Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.652, 665.501, 15.112) to=(-658.979, 1175.587, 27.975)
    2026-09-29T19:07:38.685Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-38.278, 682.768, 14.972) to=(-61.483, 683.09, 15.351)
    2026-09-29T19:07:38.686Z [INFO] [autopilot] event PedAppeared handle=5900
    2026-09-29T19:07:38.686Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -754->-757
    2026-09-29T19:07:38.800Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -757->-761
    2026-09-29T19:07:38.900Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.058, 680.081, 13.971) to=(-37.019, 685.111, 17.407)
    2026-09-29T19:07:38.901Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-38.129, 682.874, 14.96) to=(-384.849, 686.887, 30.313)
    2026-09-29T19:07:38.902Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -761->-765
    2026-09-29T19:07:38.957Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -765->-770
    2026-09-29T19:07:38.995Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -770->-773
    2026-09-29T19:07:39.041Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-37.89, 683.067, 14.964) to=(-838.256, 683.198, 37.731)
    2026-09-29T19:07:39.042Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -773->-776
    2026-09-29T19:07:39.080Z [INFO] [autopilot] event PedAppeared handle=6157
    2026-09-29T19:07:39.081Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -776->-780
    2026-09-29T19:07:39.124Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.35, 665.435, 15.233) to=(-61.354, 682.812, 14.956)
    2026-09-29T19:07:39.125Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.354, 682.812, 14.956) dir=(-0.821, 0.571, -0.009)
    2026-09-29T19:07:39.125Z [INFO] [autopilot] event PedAppeared handle=6415
    2026-09-29T19:07:39.126Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -780->-781
    2026-09-29T19:07:39.162Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.059, 680.082, 13.969) to=(-37.019, 683.266, 17.171)
    2026-09-29T19:07:39.163Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -781->-785
    2026-09-29T19:07:39.202Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.384, 647.578, 15.187) to=(-32.711, 748.901, 15.786)
    2026-09-29T19:07:39.203Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -785->-789
    2026-09-29T19:07:39.240Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -789->-792
    2026-09-29T19:07:39.276Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -792->-795
    2026-09-29T19:07:39.314Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.169, 665.54, 15.288) to=(-697.398, 1116.947, -0.423)
    2026-09-29T19:07:39.315Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -795->-798
    2026-09-29T19:07:39.352Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.414, 647.563, 15.189) to=(251.672, 1377.303, 32.302)
    2026-09-29T19:07:39.353Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -798->-801
    2026-09-29T19:07:39.390Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -801->-803
    2026-09-29T19:07:39.427Z [INFO] [autopilot] event PedRemoved handle=6157
    2026-09-29T19:07:39.428Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -803->-805
    2026-09-29T19:07:39.433Z [INFO] command source=file:cmd_20260929190739406.cmd line="hud off" reply="hud off"
    2026-09-29T19:07:39.468Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.161, 665.545, 15.281) to=(-699.978, 1112.98, 37.447)
    2026-09-29T19:07:39.469Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -805->-807
    2026-09-29T19:07:39.503Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -807->-810
    2026-09-29T19:07:39.537Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.329, 647.649, 15.163) to=(-67.531, 668.841, 14.939)
    2026-09-29T19:07:39.538Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -810->-811
    2026-09-29T19:07:39.572Z [INFO] [autopilot] event PedRemoved handle=6415
    2026-09-29T19:07:39.573Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -811->-815
    2026-09-29T19:07:39.607Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -815->-818
    2026-09-29T19:07:39.641Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.166, 665.551, 15.291) to=(-699.898, 1113.213, 17.103)
    2026-09-29T19:07:39.642Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -818->-819
    2026-09-29T19:07:39.676Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-77.195, 647.73, 15.179) to=(234.356, 1385.483, 6.674)
    2026-09-29T19:07:39.677Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -819->-821
    2026-09-29T19:07:39.713Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -821->-824
    2026-09-29T19:07:39.747Z [INFO] [autopilot] event PedRemoved handle=5900
    2026-09-29T19:07:39.748Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -824->-826
    2026-09-29T19:07:39.784Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -826->-828
    2026-09-29T19:07:39.825Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.178, 665.539, 15.286) to=(-702.411, 1109.599, 8.219)
    2026-09-29T19:07:39.826Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -828->-831
    2026-09-29T19:07:39.868Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-76.961, 647.76, 15.162) to=(-61.886, 682.75, 15.11)
    2026-09-29T19:07:39.868Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-59.581, 658.03, 14.976) to=(-105.764, 1457.611, 6.549)
    2026-09-29T19:07:39.869Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=11271 vehicle=0 killed=False hit=True at=(-61.886, 682.75, 15.11) dir=(0.396, 0.918, -0.001)
    2026-09-29T19:07:39.870Z [INFO] [autopilot] event PedAppeared handle=6158
    2026-09-29T19:07:39.871Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -831->-835
    2026-09-29T19:07:39.903Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -835->-837
    2026-09-29T19:07:39.936Z [INFO] [autopilot] event PedRemoved handle=9735
    2026-09-29T19:07:39.937Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -837->-840
    2026-09-29T19:07:39.970Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.184, 665.539, 15.29) to=(-61.472, 683.112, 15.346)
    2026-09-29T19:07:39.971Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -840->-842
    2026-09-29T19:07:40.004Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -842->-845
    2026-09-29T19:07:40.037Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-59.306, 657.959, 14.992) to=(-61.639, 682.623, 15.204)
    2026-09-29T19:07:40.038Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.639, 682.623, 15.204) dir=(-0.094, 0.996, 0.009)
    2026-09-29T19:07:40.039Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -845->-848
    2026-09-29T19:07:40.074Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -848->-849
    2026-09-29T19:07:40.111Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -849->-852
    2026-09-29T19:07:40.147Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -852->-854
    2026-09-29T19:07:40.185Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -854->-855
    2026-09-29T19:07:40.221Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-59.054, 657.919, 14.981) to=(-122.026, 1455.753, 44.419)
    2026-09-29T19:07:40.222Z [INFO] [autopilot] event PedRemoved handle=6158
    2026-09-29T19:07:40.223Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -855->-856
    2026-09-29T19:07:40.263Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -856->-860
    2026-09-29T19:07:40.299Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -860->-863
    2026-09-29T19:07:40.377Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -863->-865
    2026-09-29T19:07:40.472Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-36.315, 684.277, 15.099) to=(-835.856, 642.334, 15.1)
    2026-09-29T19:07:40.472Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-58.856, 657.915, 14.996) to=(-132.08, 1454.965, 35.515)
    2026-09-29T19:07:40.473Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -865->-870
    2026-09-29T19:07:40.569Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -870->-874
    2026-09-29T19:07:40.668Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -874->-879
    2026-09-29T19:07:40.821Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-36.057, 684.418, 15.081) to=(-62.656, 682.432, 14.861)
    2026-09-29T19:07:40.822Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-58.608, 657.901, 15.016) to=(-134.772, 1454.797, 31.763)
    2026-09-29T19:07:40.823Z [INFO] [autopilot] event PedAppeared handle=6416
    2026-09-29T19:07:40.823Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -879->-881
    2026-09-29T19:07:40.824Z [INFO] [autopilot] event VehicleDamaged handle=7429 988->976 engine 1000->1000
    2026-09-29T19:07:40.958Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -881->-886
    2026-09-29T19:07:41.114Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-35.89, 684.527, 15.096) to=(-385.331, 653.804, 20.556)
    2026-09-29T19:07:41.114Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -886->-891
    2026-09-29T19:07:41.253Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -891->-894
    2026-09-29T19:07:41.363Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-35.743, 684.637, 15.113) to=(-832.753, 605.247, 11.121)
    2026-09-29T19:07:41.364Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -894->-898
    2026-09-29T19:07:41.499Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -898->-901
    2026-09-29T19:07:41.612Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-35.58, 684.773, 15.101) to=(-834.511, 634.822, 35.269)
    2026-09-29T19:07:41.613Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -901->-906
    2026-09-29T19:07:41.710Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -906->-910
    2026-09-29T19:07:41.815Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.057, 680.082, 13.964) to=(-35.237, 685.316, 17.158)
    2026-09-29T19:07:41.816Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -910->-916
    2026-09-29T19:07:41.973Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.839, 648.004, 15.185) to=(234.578, 1386.013, 7.637)
    2026-09-29T19:07:41.974Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -916->-919
    2026-09-29T19:07:41.979Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:07:42.088Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -919->-922
    2026-09-29T19:07:42.181Z [INFO] [autopilot] event PedAppeared handle=7438
    2026-09-29T19:07:42.182Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -922->-924
    2026-09-29T19:07:42.270Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.859, 647.999, 15.2) to=(247.445, 1380.637, 11.44)
    2026-09-29T19:07:42.271Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -924->-926
    2026-09-29T19:07:42.364Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.061, 680.084, 13.972) to=(-61.471, 682.927, 15.844)
    2026-09-29T19:07:42.364Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=7 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=10247 vehicle=0 killed=False hit=True at=(-61.471, 682.927, 15.844) dir=(0.996, 0.073, 0.048)
    2026-09-29T19:07:42.365Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -926->-931
    2026-09-29T19:07:42.456Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.855, 647.994, 15.193) to=(-67.656, 667.317, 15.149)
    2026-09-29T19:07:42.457Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.212, 665.612, 14.895) to=(-706.777, 1102.699, 40.209)
    2026-09-29T19:07:42.457Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -931->-934
    2026-09-29T19:07:42.547Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -934->-940
    2026-09-29T19:07:42.641Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.851, 647.993, 15.203) to=(-31.5, 749.419, 16.575)
    2026-09-29T19:07:42.642Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -940->-946
    2026-09-29T19:07:42.739Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.17, 665.615, 14.896) to=(-61.36, 682.861, 14.623)
    2026-09-29T19:07:42.740Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.36, 682.861, 14.623) dir=(-0.825, 0.565, -0.009)
    2026-09-29T19:07:42.740Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -946->-952
    2026-09-29T19:07:42.830Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.85, 647.98, 15.189) to=(237.796, 1384.566, 23.659)
    2026-09-29T19:07:42.831Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.945, 657.746, 15.033) to=(-212.707, 1443.308, 10.354)
    2026-09-29T19:07:42.832Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -952->-956
    2026-09-29T19:07:42.928Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.051, 647.503, 15.164) to=(205.6, 1398.239, 20.418)
    2026-09-29T19:07:42.928Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.171, 665.685, 14.953) to=(-703.951, 1107.04, 33.184)
    2026-09-29T19:07:42.929Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -956->-960
    2026-09-29T19:07:43.026Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -960->-964
    2026-09-29T19:07:43.125Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.717, 657.698, 15.05) to=(-82.673, 809.186, 16.614)
    2026-09-29T19:07:43.126Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -964->-968
    2026-09-29T19:07:43.228Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.058, 647.526, 15.171) to=(-67.564, 663.647, 14.911)
    2026-09-29T19:07:43.229Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.208, 665.801, 15.192) to=(-345.483, 878.565, 14.305)
    2026-09-29T19:07:43.230Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-34.307, 685.701, 15.086) to=(-36.777, 685.381, 15.209)
    2026-09-29T19:07:43.231Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -968->-974
    2026-09-29T19:07:43.326Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.555, 657.696, 15.056) to=(-71.19, 725.002, 16.87)
    2026-09-29T19:07:43.327Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -974->-979
    2026-09-29T19:07:43.432Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.098, 647.541, 15.161) to=(-67.562, 663.946, 14.905)
    2026-09-29T19:07:43.433Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-36.224, 665.831, 15.261) to=(-61.887, 682.367, 14.771)
    2026-09-29T19:07:43.434Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -979->-985
    2026-09-29T19:07:43.435Z [INFO] [autopilot] event VehicleDamaged handle=7429 976->970 engine 1000->1000
    2026-09-29T19:07:43.533Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-34.068, 685.832, 15.104) to=(-36.777, 685.481, 15.224)
    2026-09-29T19:07:43.534Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -985->-989
    2026-09-29T19:07:43.632Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.298, 657.668, 15.081) to=(-222.236, 1440.708, 35.49)
    2026-09-29T19:07:43.633Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -989->-991
    2026-09-29T19:07:43.738Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.21, 647.521, 15.174) to=(-62.925, 678.257, 15.623)
    2026-09-29T19:07:43.739Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-33.925, 685.966, 15.079) to=(-101.044, 679.455, 15.676)
    2026-09-29T19:07:43.740Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -991->-996
    2026-09-29T19:07:43.837Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-56.109, 657.631, 15.063) to=(-61.713, 688.067, 15.481)
    2026-09-29T19:07:43.838Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -996->-1000
    2026-09-29T19:07:43.934Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.302, 647.538, 15.162) to=(-31.57, 762.398, 15.052)
    2026-09-29T19:07:43.935Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1000->-1004
    2026-09-29T19:07:44.031Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-33.713, 686.112, 15.07) to=(-828.727, 591.725, 17.535)
    2026-09-29T19:07:44.032Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1004->-1010
    2026-09-29T19:07:44.128Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-55.859, 657.589, 15.064) to=(-211.01, 1443.286, 11.312)
    2026-09-29T19:07:44.129Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1010->-1015
    2026-09-29T19:07:44.224Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-33.58, 686.169, 15.077) to=(-334.5, 643.601, 21.138)
    2026-09-29T19:07:44.225Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1015->-1018
    2026-09-29T19:07:44.309Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1018->-1023
    2026-09-29T19:07:44.340Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-33.47, 686.242, 15.098) to=(-63.063, 683.15, 15.076)
    2026-09-29T19:07:44.341Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1023->-1025
    2026-09-29T19:07:44.341Z [INFO] [autopilot] event VehicleDamaged handle=7429 970->964 engine 1000->1000
    2026-09-29T19:07:44.374Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1025->-1027
    2026-09-29T19:07:44.414Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1027->-1029
    2026-09-29T19:07:44.462Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1029->-1032
    2026-09-29T19:07:44.496Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.058, 680.083, 13.964) to=(-35.132, 684.633, 15.541)
    2026-09-29T19:07:44.497Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1032->-1036
    2026-09-29T19:07:44.530Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.825, 647.976, 15.186) to=(-63.074, 678.533, 15.666)
    2026-09-29T19:07:44.530Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1036->-1038
    2026-09-29T19:07:44.562Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1038->-1039
    2026-09-29T19:07:44.611Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1039->-1041
    2026-09-29T19:07:44.666Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1041->-1046
    2026-09-29T19:07:44.716Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1046->-1048
    2026-09-29T19:07:44.889Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1048->-1055
    2026-09-29T19:07:44.919Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1055->-1057
    2026-09-29T19:07:44.957Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.06, 680.083, 13.965) to=(-37.019, 684.7, 17.395)
    2026-09-29T19:07:44.958Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.767, 684.548, 14.998) to=(-391.701, 647.123, 28.089)
    2026-09-29T19:07:44.959Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1057->-1060
    2026-09-29T19:07:45.020Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1060->-1065
    2026-09-29T19:07:45.060Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1065->-1067
    2026-09-29T19:07:45.096Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1067->-1069
    2026-09-29T19:07:45.136Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.782, 684.55, 15.03) to=(-839.097, 614.5, 33.579)
    2026-09-29T19:07:45.136Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1069->-1072
    2026-09-29T19:07:45.181Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1072->-1076
    2026-09-29T19:07:45.217Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1076->-1079
    2026-09-29T19:07:45.255Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1079->-1082
    2026-09-29T19:07:45.298Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.792, 684.55, 15.008) to=(-385.626, 648.307, 19.394)
    2026-09-29T19:07:45.299Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1082->-1085
    2026-09-29T19:07:45.364Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1085->-1090
    2026-09-29T19:07:45.429Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.459, 657.424, 15.1) to=(-273.298, 1427.492, 24.572)
    2026-09-29T19:07:45.429Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1090->-1096
    2026-09-29T19:07:45.490Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.79, 684.537, 14.984) to=(-61.365, 682.888, 15.74)
    2026-09-29T19:07:45.491Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.365, 682.888, 15.74) dir=(-0.996, -0.084, 0.038)
    2026-09-29T19:07:45.492Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1096->-1099
    2026-09-29T19:07:45.560Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1099->-1104
    2026-09-29T19:07:45.598Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.217, 657.343, 15.107) to=(-275.649, 1426.814, 7.387)
    2026-09-29T19:07:45.599Z [INFO] [autopilot] event PedRemoved handle=6416
    2026-09-29T19:07:45.600Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1104->-1106
    2026-09-29T19:07:45.650Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.535, 686.998, 15.046) to=(-334.939, 640.576, 18.18)
    2026-09-29T19:07:45.651Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1106->-1110
    2026-09-29T19:07:45.685Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1110->-1112
    2026-09-29T19:07:45.725Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.018, 657.298, 15.09) to=(-67.077, 698.617, 14.798)
    2026-09-29T19:07:45.725Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1112->-1115
    2026-09-29T19:07:45.766Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1115->-1117
    2026-09-29T19:07:45.799Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1117->-1120
    2026-09-29T19:07:45.842Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1120->-1123
    2026-09-29T19:07:45.876Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1123->-1126
    2026-09-29T19:07:45.910Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.806, 657.264, 15.105) to=(-267.309, 1428.906, 28.853)
    2026-09-29T19:07:45.911Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1126->-1129
    2026-09-29T19:07:45.950Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1129->-1131
    2026-09-29T19:07:45.982Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1131->-1134
    2026-09-29T19:07:46.031Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1134->-1139
    2026-09-29T19:07:46.126Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.677, 657.23, 15.119) to=(-269.212, 1428.401, 21.155)
    2026-09-29T19:07:46.127Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1139->-1145
    2026-09-29T19:07:46.221Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.057, 680.082, 13.962) to=(-35.069, 684.037, 14.965)
    2026-09-29T19:07:46.222Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1145->-1147
    2026-09-29T19:07:46.317Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.275, 648.425, 15.17) to=(233.443, 1387.036, 29.818)
    2026-09-29T19:07:46.318Z [INFO] [autopilot] event PedAppeared handle=6664
    2026-09-29T19:07:46.318Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1147->-1151
    2026-09-29T19:07:46.427Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1151->-1155
    2026-09-29T19:07:46.518Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1155->-1160
    2026-09-29T19:07:46.611Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.321, 648.368, 15.199) to=(-67.646, 666.921, 15.023)
    2026-09-29T19:07:46.612Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.789, 647.555, 14.776) to=(203.444, 1398.671, 23.205)
    2026-09-29T19:07:46.613Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1160->-1162
    2026-09-29T19:07:46.701Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.061, 680.084, 13.971) to=(-35.239, 685.806, 16.127)
    2026-09-29T19:07:46.702Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-37.863, 671.754, 15.071) to=(-753.791, 1030.132, 39.361)
    2026-09-29T19:07:46.702Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1162->-1166
    2026-09-29T19:07:46.792Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.32, 648.378, 15.194) to=(-37.116, 745.557, 17.272)
    2026-09-29T19:07:46.792Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1166->-1170
    2026-09-29T19:07:46.886Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.838, 647.593, 14.795) to=(194.281, 1401.668, 37.281)
    2026-09-29T19:07:46.887Z [INFO] [autopilot] event PedRemoved handle=6664
    2026-09-29T19:07:46.888Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1170->-1175
    2026-09-29T19:07:46.982Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.327, 648.396, 15.202) to=(232.86, 1387.258, 31.553)
    2026-09-29T19:07:46.984Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-38.136, 672.194, 14.995) to=(-765.64, 1006.638, 13.105)
    2026-09-29T19:07:46.984Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.375, 687.07, 14.961) to=(-281.381, 654.963, 17.676)
    2026-09-29T19:07:46.985Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1175->-1180
    2026-09-29T19:07:47.071Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.838, 647.609, 14.796) to=(196.573, 1401.229, 16.438)
    2026-09-29T19:07:47.072Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1180->-1184
    2026-09-29T19:07:47.171Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-38.319, 672.459, 15.005) to=(-62.809, 683.033, 14.956)
    2026-09-29T19:07:47.171Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1184->-1187
    2026-09-29T19:07:47.172Z [INFO] [autopilot] event VehicleDamaged handle=7429 964->958 engine 1000->1000
    2026-09-29T19:07:47.266Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.339, 648.405, 15.195) to=(-31.505, 761.705, 15.519)
    2026-09-29T19:07:47.267Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.354, 687.045, 14.962) to=(-821.823, 553.501, 26.754)
    2026-09-29T19:07:47.268Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.77, 657.055, 15.121) to=(-67.077, 702.385, 14.835)
    2026-09-29T19:07:47.269Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1187->-1192
    2026-09-29T19:07:47.365Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.834, 647.599, 14.795) to=(-73.826, 647.598, 14.783)
    2026-09-29T19:07:47.366Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1192->-1197
    2026-09-29T19:07:47.465Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.327, 648.413, 15.199) to=(231.224, 1388.057, 6.162)
    2026-09-29T19:07:47.465Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-38.518, 672.869, 14.989) to=(-777.885, 980.382, 14.343)
    2026-09-29T19:07:47.466Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.377, 687.04, 14.927) to=(-823.191, 563.451, 34.153)
    2026-09-29T19:07:47.467Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1197->-1202
    2026-09-29T19:07:47.566Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.817, 647.598, 14.788) to=(184.564, 1405.23, 29.917)
    2026-09-29T19:07:47.567Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.574, 656.974, 15.138) to=(-293.535, 1420.594, 27.68)
    2026-09-29T19:07:47.568Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1202->-1207
    2026-09-29T19:07:47.673Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1207->-1210
    2026-09-29T19:07:47.771Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.363, 687.02, 15.031) to=(-307, 643.888, 20.381)
    2026-09-29T19:07:47.772Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.401, 656.934, 15.108) to=(-67.383, 701.01, 15.189)
    2026-09-29T19:07:47.773Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1210->-1215
    2026-09-29T19:07:47.876Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1215->-1218
    2026-09-29T19:07:47.966Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.353, 687.015, 15.106) to=(-391.304, 641.913, 24.065)
    2026-09-29T19:07:47.967Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1218->-1222
    2026-09-29T19:07:48.062Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.06, 680.083, 13.972) to=(-35.055, 683.475, 15.787)
    2026-09-29T19:07:48.063Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1222->-1229
    2026-09-29T19:07:48.156Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.045, 683.873, 14.97) to=(-839.693, 648.274, 59.126)
    2026-09-29T19:07:48.157Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1229->-1233
    2026-09-29T19:07:48.251Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1233->-1237
    2026-09-29T19:07:48.342Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1237->-1240
    2026-09-29T19:07:48.435Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.041, 683.834, 14.947) to=(-67.179, 683.712, 14.995)
    2026-09-29T19:07:48.436Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1240->-1243
    2026-09-29T19:07:48.528Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.062, 680.085, 13.973) to=(-67.549, 681.419, 14.33)
    2026-09-29T19:07:48.529Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1243->-1247
    2026-09-29T19:07:48.620Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.047, 683.824, 14.964) to=(-383.442, 675.295, 22.037)
    2026-09-29T19:07:48.621Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1247->-1252
    2026-09-29T19:07:48.707Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1252->-1258
    2026-09-29T19:07:48.798Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1258->-1263
    2026-09-29T19:07:48.886Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.29, 648.39, 15.186) to=(-67.582, 666.872, 15.2)
    2026-09-29T19:07:48.887Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.031, 683.824, 15.117) to=(-841.965, 684.83, 47.761)
    2026-09-29T19:07:48.888Z [INFO] [autopilot] event PedAppeared handle=8714
    2026-09-29T19:07:48.888Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1263->-1265
    2026-09-29T19:07:48.976Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1265->-1270
    2026-09-29T19:07:49.069Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.025, 683.817, 15.116) to=(-838.686, 637.385, 67.246)
    2026-09-29T19:07:49.070Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1270->-1273
    2026-09-29T19:07:49.166Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1273->-1278
    2026-09-29T19:07:49.258Z [INFO] [autopilot] event PedRemoved handle=8714
    2026-09-29T19:07:49.259Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1278->-1283
    2026-09-29T19:07:49.353Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.038, 683.832, 15.008) to=(-102.173, 680.217, 18.771)
    2026-09-29T19:07:49.354Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.519, 656.602, 15.061) to=(-89.31, 764.141, 20.641)
    2026-09-29T19:07:49.354Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1283->-1286
    2026-09-29T19:07:49.447Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1286->-1292
    2026-09-29T19:07:49.544Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1292->-1296
    2026-09-29T19:07:49.598Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.509, 656.559, 15.053) to=(-329.764, 1406.985, 36.632)
    2026-09-29T19:07:49.599Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1296->-1300
    2026-09-29T19:07:49.638Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1300->-1302
    2026-09-29T19:07:49.676Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1302->-1306
    2026-09-29T19:07:49.716Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.403, 687.054, 14.975) to=(-827.983, 594.248, 17.168)
    2026-09-29T19:07:49.717Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.477, 656.54, 15.078) to=(-301.537, 1417.549, 48.455)
    2026-09-29T19:07:49.718Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1306->-1308
    2026-09-29T19:07:49.758Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.056, 680.081, 13.96) to=(-67.549, 681.696, 14.79)
    2026-09-29T19:07:49.759Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1308->-1312
    2026-09-29T19:07:49.806Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1312->-1315
    2026-09-29T19:07:49.845Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1315->-1318
    2026-09-29T19:07:49.883Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.374, 687.033, 14.99) to=(-62.737, 683.42, 14.878)
    2026-09-29T19:07:49.884Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.473, 656.542, 15.072) to=(-61.817, 682.705, 14.743)
    2026-09-29T19:07:49.885Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1318->-1321
    2026-09-29T19:07:49.885Z [INFO] [autopilot] event VehicleDamaged handle=7429 958->940 engine 1000->1000
    2026-09-29T19:07:49.925Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1321->-1325
    2026-09-29T19:07:49.957Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1325->-1328
    2026-09-29T19:07:49.992Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.702, 647.064, 15.168) to=(-36.527, 766.782, 20.496)
    2026-09-29T19:07:49.993Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1328->-1330
    2026-09-29T19:07:50.031Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.372, 687.042, 14.986) to=(-80.511, 678.251, 16.161)
    2026-09-29T19:07:50.032Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.486, 656.546, 15.036) to=(-313.633, 1412.713, 51.909)
    2026-09-29T19:07:50.032Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1330->-1332
    2026-09-29T19:07:50.070Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-100.06, 680.083, 13.964) to=(-67.549, 682.679, 14.073)
    2026-09-29T19:07:50.071Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1332->-1334
    2026-09-29T19:07:50.107Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1334->-1338
    2026-09-29T19:07:50.143Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-75.301, 648.395, 15.189) to=(245.145, 1382.484, 20.704)
    2026-09-29T19:07:50.144Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.483, 656.541, 15.046) to=(-61.727, 682.401, 14.458)
    2026-09-29T19:07:50.145Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1338->-1341
    2026-09-29T19:07:50.145Z [INFO] [autopilot] event VehicleDamaged handle=7429 940->934 engine 1000->1000
    2026-09-29T19:07:50.180Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1341->-1343
    2026-09-29T19:07:50.219Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.376, 687.05, 14.994) to=(-391.701, 642.975, 40.242)
    2026-09-29T19:07:50.219Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1343->-1346
    2026-09-29T19:07:50.258Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1346->-1348
    2026-09-29T19:07:50.297Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1348->-1350
    2026-09-29T19:07:50.333Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1350->-1353
    2026-09-29T19:07:50.372Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.374, 687.039, 14.994) to=(-80.612, 678.551, 15.587)
    2026-09-29T19:07:50.373Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1353->-1356
    2026-09-29T19:07:50.410Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1356->-1357
    2026-09-29T19:07:50.446Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1357->-1361
    2026-09-29T19:07:50.488Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1361->-1364
    2026-09-29T19:07:50.524Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1364->-1366
    2026-09-29T19:07:50.558Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1366->-1369
    2026-09-29T19:07:50.592Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1369->-1371
    2026-09-29T19:07:50.625Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1371->-1374
    2026-09-29T19:07:50.661Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1374->-1377
    2026-09-29T19:07:50.693Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1377->-1378
    2026-09-29T19:07:50.725Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1378->-1380
    2026-09-29T19:07:50.757Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1380->-1382
    2026-09-29T19:07:50.801Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1382->-1386
    2026-09-29T19:07:50.831Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1386->-1388
    2026-09-29T19:07:50.861Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1388->-1390
    2026-09-29T19:07:50.891Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1390->-1392
    2026-09-29T19:07:50.923Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1392->-1394
    2026-09-29T19:07:51.001Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1394->-1399
    2026-09-29T19:07:51.092Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.67, 682.75, 15.101) to=(-61.504, 683.079, 15.264)
    2026-09-29T19:07:51.093Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C9 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.504, 683.079, 15.264) dir=(-1, 0.016, 0.008)
    2026-09-29T19:07:51.094Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1399->-1403
    2026-09-29T19:07:51.184Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1403->-1408
    2026-09-29T19:07:51.276Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.616, 682.545, 15.073) to=(-840.76, 707.746, 33.928)
    2026-09-29T19:07:51.277Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1408->-1412
    2026-09-29T19:07:51.366Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1412->-1416
    2026-09-29T19:07:51.459Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1416->-1421
    2026-09-29T19:07:51.566Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.54, 682.189, 15.077) to=(-61.905, 682.36, 14.797)
    2026-09-29T19:07:51.567Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1421->-1426
    2026-09-29T19:07:51.567Z [INFO] [autopilot] event VehicleDamaged handle=7429 934->928 engine 1000->1000
    2026-09-29T19:07:51.663Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-74.218, 649.03, 15.132) to=(-71.733, 655.242, 14.973)
    2026-09-29T19:07:51.664Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1426->-1431
    2026-09-29T19:07:51.764Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.507, 681.928, 15.079) to=(-61.315, 682.932, 14.907)
    2026-09-29T19:07:51.765Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.315, 682.932, 14.907) dir=(-0.999, 0.048, -0.008)
    2026-09-29T19:07:51.766Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1431->-1437
    2026-09-29T19:07:51.877Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1437->-1443
    2026-09-29T19:07:51.975Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-73.704, 649.165, 15.127) to=(189.519, 1405.318, 19.522)
    2026-09-29T19:07:51.976Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1443->-1445
    2026-09-29T19:07:51.980Z [INFO] performance samples=441 frame_p50_ms=69 frame_p95_ms=108 frame_p99_ms=157 frames_over_33ms=377 frames_over_50ms=231 gunplay_avg_ms=1.845 gunplay_max_ms=13.609 phase_samples=441 phase_setup_avg_ms=1.340 phase_setup_max_ms=13.118 phase_camera_avg_ms=0.061 phase_camera_max_ms=3.500 phase_bullets_avg_ms=0.038 phase_bullets_max_ms=0.162 phase_weapon_avg_ms=0.023 phase_weapon_max_ms=0.155 phase_hud_avg_ms=0.384 phase_hud_max_ms=0.718
    2026-09-29T19:07:51.980Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.866/16.8/441@7 total=2146 module.gunplay=1.853/13.6/441@7 total=817 tick.gunplay=1.851/13.6/441@7 total=816 engine.world=1.504/4.4/441@7 total=663 gp.freeaim=1.152/13.0/441@7 total=508 module.arsenal=0.856/9.4/438@7 total=375 tick.arsenal=0.855/9.4/438@7 total=374 ar.storage=0.528/9.3/438@7 total=231 ar.safehouse_flags=0.291/3.4/438@7 total=127 module.combat=0.262/8.4/441@7 total=115 tick.combat=0.261/8.4/441@7 total=115 combat.sample=1.429/8.4/68@7 total=97 module.atmosphere=0.153/1.7/441@7 total=67 tick.atmosphere=0.152/1.7/441@7 total=67 gp.index_pad=0.095/0.3/441@7 total=42 cam.handle=0.053/3.5/441@7 total=24 module.holsters=0.065/0.9/330@7 total=21 tick.holsters=0.064/0.8/330@7 total=21 gp.player=0.025/0.1/441@7 total=11 module.devtools=0.023/0.1/438@7 total=10 tick.devtools=0.023/0.1/438@7 total=10 gp.shoulder=0.020/0.2/441@7 total=9 module.world=0.148/0.9/56@7 total=8 gp.weapon_id=0.016/0.1/441@7 total=7 gp.cycle=0.009/0.1/441@7 total=4 module.probe=1.212/1.3/3@7 total=4 ho.show=0.010/0.0/330@7 total=3 gp.state=0.008/0.1/441@7 total=3 ar.reconcile=0.005/0.0/438@7 total=2 ar.discover=0.004/0.8/438@7 total=2 cam.aim_key=0.004/0.0/441@7 total=2 ar.lvs=0.004/0.3/438@7 total=2 ar.vehicle=0.003/0.0/438@7 total=1 cam.find_active=0.003/0.0/441@7 total=1 gp.shots=0.002/0.1/441@7 total=1 gp.spread=0.002/0.0/441@7 total=1 ho.carried=0.002/0.0/330@7 total=1 combat.dismember=0.001/0.2/441@7 total=0 engine.scheduler=0.001/0.0/441@7 total=0 module.autopilot=0.000/0.0/441@7 total=0 cam.fov=0.001/0.0/106@7 total=0 combat.blood=0.000/0.0/441@7 total=0 gp.recoil=0.000/0.0/441@7 total=0 gp.feel=0.000/0.0/441@7 total=0 combat.pending=0.000/0.0/441@7 total=0 module.weapon-probe=0.000/0.0/441@7 total=0
    2026-09-29T19:07:51.981Z [INFO] engine_thread_probe ticks=441 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=441 ticks_after_skipped_frames=0
    2026-09-29T19:07:51.982Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.14 shdn_us=38.6
    2026-09-29T19:07:51.984Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:07:51.985Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=9157 core=on peds=17 vehicles=13 modules=10/10 coroutines=0 resources=5 raycast=on episode=GTAIV frame_ms=66.42 p95_ms=109.73 pressure=0.91 private_mb=2218 working_set_mb=1464 address_free_mb=1224 largest_free_block_mb=1183 managed_mb=15 physical_load=94% core_us=64.8
    2026-09-29T19:07:52.084Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.463, 681.566, 15.089) to=(-838.719, 739.533, 42.925)
    2026-09-29T19:07:52.085Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1445->-1448
    2026-09-29T19:07:52.091Z [INFO] density frame_ms=80.0 peds=0.55 cars=0.60
    2026-09-29T19:07:52.194Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-73.294, 649.202, 15.159) to=(-35.398, 763.309, 17.336)
    2026-09-29T19:07:52.195Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1448->-1451
    2026-09-29T19:07:52.289Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1451->-1455
    2026-09-29T19:07:52.381Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.951, 649.19, 15.164) to=(169.389, 1412.444, 14.544)
    2026-09-29T19:07:52.382Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1455->-1460
    2026-09-29T19:07:52.477Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1460->-1464
    2026-09-29T19:07:52.566Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.493, 656.593, 15.062) to=(-336.341, 1404.462, 38.246)
    2026-09-29T19:07:52.567Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1464->-1470
    2026-09-29T19:07:52.664Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.644, 649.085, 15.165) to=(-67.25, 666.895, 15.156)
    2026-09-29T19:07:52.665Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.428, 687.067, 14.964) to=(-277.153, 654.189, 16.614)
    2026-09-29T19:07:52.666Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1470->-1475
    2026-09-29T19:07:52.759Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1475->-1482
    2026-09-29T19:07:52.850Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.432, 649.018, 15.147) to=(170.067, 1411.885, 36.317)
    2026-09-29T19:07:52.851Z [INFO] [autopilot] event PedAppeared handle=9230
    2026-09-29T19:07:52.852Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1482->-1487
    2026-09-29T19:07:52.948Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.398, 687.044, 14.978) to=(-824.032, 569.079, 35.792)
    2026-09-29T19:07:52.949Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1487->-1492
    2026-09-29T19:07:53.037Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1492->-1497
    2026-09-29T19:07:53.124Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.405, 687.055, 14.965) to=(-291.623, 643.291, 16.87)
    2026-09-29T19:07:53.124Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1497->-1500
    2026-09-29T19:07:53.212Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1500->-1503
    2026-09-29T19:07:53.303Z [INFO] [autopilot] event PedRemoved handle=9230
    2026-09-29T19:07:53.303Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1503->-1505
    2026-09-29T19:07:53.394Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.191, 680.009, 15.044) to=(-835.228, 775.12, 16.319)
    2026-09-29T19:07:53.395Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.397, 687.055, 14.982) to=(-823.715, 567.507, 42.108)
    2026-09-29T19:07:53.396Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1505->-1510
    2026-09-29T19:07:53.481Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.499, 647.608, 14.778) to=(114.986, 1426.378, 31.424)
    2026-09-29T19:07:53.482Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1510->-1516
    2026-09-29T19:07:53.574Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.396, 687.045, 14.971) to=(-61.337, 682.941, 15.211)
    2026-09-29T19:07:53.575Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.337, 682.941, 15.211) dir=(-0.99, -0.14, 0.008)
    2026-09-29T19:07:53.575Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1516->-1520
    2026-09-29T19:07:53.669Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.498, 647.615, 14.816) to=(126.362, 1423.573, 24.013)
    2026-09-29T19:07:53.670Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1520->-1522
    2026-09-29T19:07:53.764Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1522->-1526
    2026-09-29T19:07:53.865Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1526->-1531
    2026-09-29T19:07:53.962Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.5, 647.614, 14.814) to=(121.533, 1424.809, 24.584)
    2026-09-29T19:07:53.963Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1531->-1537
    2026-09-29T19:07:54.061Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.322, 649.031, 15.185) to=(-36.698, 764.291, 18.209)
    2026-09-29T19:07:54.062Z [INFO] [autopilot] event PedAppeared handle=9482
    2026-09-29T19:07:54.063Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1537->-1543
    2026-09-29T19:07:54.163Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.495, 647.62, 14.795) to=(-70.49, 647.591, 14.778)
    2026-09-29T19:07:54.164Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1543->-1547
    2026-09-29T19:07:54.257Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1547->-1550
    2026-09-29T19:07:54.351Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.344, 649.015, 15.201) to=(189.526, 1405.771, 6.902)
    2026-09-29T19:07:54.352Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1550->-1556
    2026-09-29T19:07:54.445Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.479, 647.612, 14.773) to=(121.483, 1424.553, 40.087)
    2026-09-29T19:07:54.446Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1556->-1562
    2026-09-29T19:07:54.517Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.337, 649.003, 15.192) to=(-37.001, 763.256, 15.91)
    2026-09-29T19:07:54.518Z [INFO] [autopilot] event PedRemoved handle=9482
    2026-09-29T19:07:54.519Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1562->-1568
    2026-09-29T19:07:54.553Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-70.466, 647.622, 14.782) to=(124.86, 1423.649, 42.778)
    2026-09-29T19:07:54.554Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.495, 656.63, 15.029) to=(-339.13, 1403.678, 26.533)
    2026-09-29T19:07:54.555Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1568->-1571
    2026-09-29T19:07:54.587Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-96.542, 691.153, 13.515) to=(-33.114, 677.127, 17.925)
    2026-09-29T19:07:54.588Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1571->-1573
    2026-09-29T19:07:54.622Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1573->-1575
    2026-09-29T19:07:54.658Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.33, 648.984, 15.201) to=(158.626, 1415.606, 32.885)
    2026-09-29T19:07:54.659Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.914, 678.349, 15.048) to=(-62.578, 682.468, 14.819)
    2026-09-29T19:07:54.660Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1575->-1577
    2026-09-29T19:07:54.661Z [INFO] [autopilot] event VehicleDamaged handle=7429 928->916 engine 1000->1000
    2026-09-29T19:07:54.694Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1577->-1579
    2026-09-29T19:07:54.725Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.512, 656.598, 15.045) to=(-88.922, 764.082, 17.2)
    2026-09-29T19:07:54.726Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1579->-1580
    2026-09-29T19:07:54.763Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1580->-1582
    2026-09-29T19:07:54.798Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.405, 687.06, 14.973) to=(-61.332, 682.947, 15.627)
    2026-09-29T19:07:54.799Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.332, 682.947, 15.627) dir=(-0.99, -0.141, 0.022)
    2026-09-29T19:07:54.800Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1582->-1585
    2026-09-29T19:07:54.835Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.336, 648.98, 15.195) to=(174.088, 1410.611, 32.728)
    2026-09-29T19:07:54.836Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.839, 678.043, 15.05) to=(-61.714, 682.691, 14.97)
    2026-09-29T19:07:54.836Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.714, 682.691, 14.97) dir=(-0.978, 0.208, -0.004)
    2026-09-29T19:07:54.837Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1585->-1586
    2026-09-29T19:07:54.868Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-95.866, 691.522, 13.62) to=(-67.549, 684.457, 14.488)
    2026-09-29T19:07:54.869Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1586->-1587
    2026-09-29T19:07:54.904Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.506, 656.598, 15.034) to=(-347.15, 1400.792, 12.883)
    2026-09-29T19:07:54.905Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1587->-1590
    2026-09-29T19:07:54.941Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1590->-1593
    2026-09-29T19:07:54.974Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.334, 648.976, 15.2) to=(-32.761, 766.193, 15.903)
    2026-09-29T19:07:54.975Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.377, 687.042, 14.989) to=(-61.34, 682.788, 15.028)
    2026-09-29T19:07:54.976Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.34, 682.788, 15.028) dir=(-0.989, -0.145, 0.001)
    2026-09-29T19:07:54.976Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1593->-1595
    2026-09-29T19:07:55.010Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.758, 677.779, 15.031) to=(-819.879, 855.46, 52.639)
    2026-09-29T19:07:55.011Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1595->-1597
    2026-09-29T19:07:55.044Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1597->-1599
    2026-09-29T19:07:55.073Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1599->-1601
    2026-09-29T19:07:55.104Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1601->-1604
    2026-09-29T19:07:55.133Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.375, 687.048, 14.98) to=(-825.944, 580.5, 16.632)
    2026-09-29T19:07:55.134Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1604->-1607
    2026-09-29T19:07:55.167Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.722, 677.518, 15.043) to=(-61.307, 682.956, 14.74)
    2026-09-29T19:07:55.168Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.307, 682.956, 14.74) dir=(-0.97, 0.244, -0.014)
    2026-09-29T19:07:55.168Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1607->-1609
    2026-09-29T19:07:55.199Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1609->-1612
    2026-09-29T19:07:55.233Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-96.005, 692.214, 13.582) to=(-33.07, 675.924, 17.203)
    2026-09-29T19:07:55.234Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1612->-1614
    2026-09-29T19:07:55.273Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1614->-1616
    2026-09-29T19:07:55.304Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.688, 677.301, 15.045) to=(-813.133, 883.403, 35.079)
    2026-09-29T19:07:55.304Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.379, 687.051, 14.99) to=(-335.458, 640.23, 20.534)
    2026-09-29T19:07:55.305Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1616->-1618
    2026-09-29T19:07:55.334Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1618->-1621
    2026-09-29T19:07:55.366Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1621->-1624
    2026-09-29T19:07:55.397Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1624->-1626
    2026-09-29T19:07:55.427Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.367, 687.039, 14.958) to=(-335.185, 644.164, 26.84)
    2026-09-29T19:07:55.428Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1626->-1629
    2026-09-29T19:07:55.458Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1629->-1632
    2026-09-29T19:07:55.490Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.624, 677.008, 15.039) to=(-817.042, 867.936, 34.657)
    2026-09-29T19:07:55.491Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1632->-1633
    2026-09-29T19:07:55.522Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1633->-1635
    2026-09-29T19:07:55.556Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1635->-1636
    2026-09-29T19:07:55.586Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1636->-1639
    2026-09-29T19:07:55.618Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1639->-1642
    2026-09-29T19:07:55.650Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1642->-1645
    2026-09-29T19:07:55.686Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1645->-1648
    2026-09-29T19:07:55.721Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1648->-1649
    2026-09-29T19:07:55.759Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1649->-1651
    2026-09-29T19:07:55.794Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.492, 656.625, 15.03) to=(-93.64, 764.529, 15.817)
    2026-09-29T19:07:55.795Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1651->-1654
    2026-09-29T19:07:55.837Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1654->-1656
    2026-09-29T19:07:55.881Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1656->-1659
    2026-09-29T19:07:55.916Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1659->-1661
    2026-09-29T19:07:55.959Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1661->-1664
    2026-09-29T19:07:55.995Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1664->-1666
    2026-09-29T19:07:56.032Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1666->-1668
    2026-09-29T19:07:56.071Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1668->-1671
    2026-09-29T19:07:56.110Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1671->-1675
    2026-09-29T19:07:56.146Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1675->-1677
    2026-09-29T19:07:56.185Z [INFO] [autopilot] event PedAppeared handle=9483
    2026-09-29T19:07:56.186Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1677->-1678
    2026-09-29T19:07:56.226Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1678->-1682
    2026-09-29T19:07:56.263Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1682->-1684
    2026-09-29T19:07:56.301Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1684->-1687
    2026-09-29T19:07:56.336Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.381, 687.06, 14.958) to=(-391.689, 642.295, 24.767)
    2026-09-29T19:07:56.337Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1687->-1689
    2026-09-29T19:07:56.374Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1689->-1692
    2026-09-29T19:07:56.408Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.306, 648.992, 15.188) to=(-63.097, 678.657, 15.702)
    2026-09-29T19:07:56.409Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1692->-1696
    2026-09-29T19:07:56.444Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1696->-1697
    2026-09-29T19:07:56.476Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.287, 675.559, 15.002) to=(-795.02, 939.642, 33.32)
    2026-09-29T19:07:56.477Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1697->-1699
    2026-09-29T19:07:56.513Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.367, 687.032, 14.94) to=(-238.532, 655.815, 16.315)
    2026-09-29T19:07:56.514Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1699->-1700
    2026-09-29T19:07:56.546Z [INFO] [autopilot] event PedRemoved handle=7438
    2026-09-29T19:07:56.547Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1700->-1703
    2026-09-29T19:07:56.582Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.338, 648.977, 15.201) to=(185.5, 1406.977, 21.254)
    2026-09-29T19:07:56.583Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1703->-1705
    2026-09-29T19:07:56.617Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1705->-1708
    2026-09-29T19:07:56.651Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.22, 675.259, 15.027) to=(-789.881, 954.584, 17.651)
    2026-09-29T19:07:56.652Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1708->-1709
    2026-09-29T19:07:56.687Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.36, 687.018, 14.997) to=(-825.044, 575.107, 23.282)
    2026-09-29T19:07:56.688Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1709->-1711
    2026-09-29T19:07:56.736Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1711->-1712
    2026-09-29T19:07:56.769Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.334, 648.974, 15.192) to=(-63.143, 678.574, 15.041)
    2026-09-29T19:07:56.770Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1712->-1717
    2026-09-29T19:07:56.806Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.166, 675.051, 15.022) to=(-61.232, 683.085, 15.468)
    2026-09-29T19:07:56.807Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.36, 687.02, 15.093) to=(-825.562, 578.949, 27.589)
    2026-09-29T19:07:56.808Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.232, 683.085, 15.468) dir=(-0.939, 0.342, 0.019)
    2026-09-29T19:07:56.808Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1717->-1719
    2026-09-29T19:07:56.841Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.444, 704.479, 15.136) to=(-371.871, -29.455, 5.421)
    2026-09-29T19:07:56.841Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1719->-1720
    2026-09-29T19:07:56.876Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1720->-1723
    2026-09-29T19:07:56.909Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1723->-1724
    2026-09-29T19:07:56.943Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.329, 648.977, 15.202) to=(189.189, 1405.863, 7.115)
    2026-09-29T19:07:56.944Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1724->-1726
    2026-09-29T19:07:56.984Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.098, 674.816, 15.005) to=(-61.518, 682.595, 15.222)
    2026-09-29T19:07:56.984Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.358, 687.018, 15.098) to=(-61.31, 682.858, 14.986)
    2026-09-29T19:07:56.985Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.518, 682.595, 15.222) dir=(-0.945, 0.328, 0.009)
    2026-09-29T19:07:56.986Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.31, 682.858, 14.986) dir=(-0.99, -0.142, -0.004)
    2026-09-29T19:07:56.987Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1726->-1730
    2026-09-29T19:07:57.021Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.407, 704.743, 15.148) to=(-62.645, 678.738, 14.753)
    2026-09-29T19:07:57.022Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1730->-1731
    2026-09-29T19:07:57.058Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.492, 656.602, 15.035) to=(-320.794, 1410.643, 22.402)
    2026-09-29T19:07:57.059Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1731->-1733
    2026-09-29T19:07:57.094Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.343, 648.978, 15.197) to=(168.697, 1412.456, 11.292)
    2026-09-29T19:07:57.095Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1733->-1735
    2026-09-29T19:07:57.131Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.07, 674.59, 14.969) to=(-787.211, 958.763, 41.533)
    2026-09-29T19:07:57.132Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1735->-1737
    2026-09-29T19:07:57.171Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.376, 704.965, 15.162) to=(-62.758, 678.799, 15.652)
    2026-09-29T19:07:57.172Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1737->-1740
    2026-09-29T19:07:57.203Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1740->-1743
    2026-09-29T19:07:57.241Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.502, 656.573, 15.052) to=(-93.262, 764.085, 17.628)
    2026-09-29T19:07:57.242Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1743->-1744
    2026-09-29T19:07:57.268Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1744->-1746
    2026-09-29T19:07:57.301Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1746->-1748
    2026-09-29T19:07:57.332Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.409, 705.238, 15.151) to=(-62.764, 678.806, 15.567)
    2026-09-29T19:07:57.333Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1748->-1750
    2026-09-29T19:07:57.367Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1750->-1753
    2026-09-29T19:07:57.404Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.495, 656.571, 15.046) to=(-339.088, 1403.671, 23.925)
    2026-09-29T19:07:57.405Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1753->-1755
    2026-09-29T19:07:57.431Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1755->-1756
    2026-09-29T19:07:57.461Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1756->-1759
    2026-09-29T19:07:57.492Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.451, 705.47, 15.126) to=(-366.352, -30.414, 37.472)
    2026-09-29T19:07:57.493Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1759->-1762
    2026-09-29T19:07:57.526Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1762->-1764
    2026-09-29T19:07:57.557Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.491, 656.578, 15.056) to=(-61.578, 682.581, 15.209)
    2026-09-29T19:07:57.558Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.578, 682.581, 15.209) dir=(-0.362, 0.932, 0.005)
    2026-09-29T19:07:57.559Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1764->-1766
    2026-09-29T19:07:57.589Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1766->-1767
    2026-09-29T19:07:57.621Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1767->-1768
    2026-09-29T19:07:57.652Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.479, 705.704, 15.139) to=(-96.858, 606.85, 15.449)
    2026-09-29T19:07:57.653Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1768->-1770
    2026-09-29T19:07:57.687Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.504, 656.565, 15.054) to=(-343.929, 1401.761, 30.965)
    2026-09-29T19:07:57.688Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1770->-1772
    2026-09-29T19:07:57.719Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1772->-1775
    2026-09-29T19:07:57.753Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1775->-1776
    2026-09-29T19:07:57.786Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1776->-1777
    2026-09-29T19:07:57.824Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1777->-1779
    2026-09-29T19:07:57.858Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.498, 656.567, 15.045) to=(-61.209, 683.121, 15.387)
    2026-09-29T19:07:57.859Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.209, 683.121, 15.387) dir=(-0.343, 0.939, 0.012)
    2026-09-29T19:07:57.860Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1779->-1780
    2026-09-29T19:07:57.896Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1780->-1781
    2026-09-29T19:07:57.933Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1781->-1785
    2026-09-29T19:07:57.968Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1785->-1787
    2026-09-29T19:07:58.007Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1787->-1788
    2026-09-29T19:07:58.042Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1788->-1790
    2026-09-29T19:07:58.077Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1790->-1793
    2026-09-29T19:07:58.113Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.319, 649.003, 15.19) to=(167.469, 1412.901, 16.041)
    2026-09-29T19:07:58.114Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1793->-1795
    2026-09-29T19:07:58.155Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1795->-1798
    2026-09-29T19:07:58.190Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1798->-1800
    2026-09-29T19:07:58.229Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1800->-1803
    2026-09-29T19:07:58.270Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1803->-1806
    2026-09-29T19:07:58.313Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1806->-1808
    2026-09-29T19:07:58.353Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1808->-1811
    2026-09-29T19:07:58.392Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1811->-1814
    2026-09-29T19:07:58.433Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.602, 672.805, 14.979) to=(-777.222, 980.959, 45.621)
    2026-09-29T19:07:58.434Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1814->-1817
    2026-09-29T19:07:58.472Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.404, 687.056, 14.977) to=(-824.852, 574.56, 34.901)
    2026-09-29T19:07:58.473Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1817->-1821
    2026-09-29T19:07:58.511Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1821->-1823
    2026-09-29T19:07:58.546Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1823->-1826
    2026-09-29T19:07:58.583Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.531, 672.544, 14.987) to=(-762.541, 1014.54, 21.999)
    2026-09-29T19:07:58.584Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1826->-1828
    2026-09-29T19:07:58.616Z [INFO] [autopilot] event PedRemoved handle=9483
    2026-09-29T19:07:58.617Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1828->-1831
    2026-09-29T19:07:58.653Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.376, 687.041, 14.988) to=(-336.052, 645.363, 22.011)
    2026-09-29T19:07:58.654Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1831->-1835
    2026-09-29T19:07:58.689Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1835->-1836
    2026-09-29T19:07:58.724Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.468, 672.335, 14.987) to=(-61.312, 682.75, 15.504)
    2026-09-29T19:07:58.725Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.312, 682.75, 15.504) dir=(-0.91, 0.415, 0.021)
    2026-09-29T19:07:58.725Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1836->-1839
    2026-09-29T19:07:58.762Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1839->-1841
    2026-09-29T19:07:58.796Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.374, 687.048, 14.982) to=(-823.576, 565.892, 32.98)
    2026-09-29T19:07:58.797Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1841->-1845
    2026-09-29T19:07:58.831Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1845->-1848
    2026-09-29T19:07:58.868Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.399, 672.151, 14.987) to=(-766.075, 1005.947, 20.727)
    2026-09-29T19:07:58.869Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1848->-1851
    2026-09-29T19:07:58.902Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1851->-1853
    2026-09-29T19:07:58.934Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1853->-1856
    2026-09-29T19:07:58.968Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.389, 687.05, 14.995) to=(-822.355, 556.377, 20.299)
    2026-09-29T19:07:58.969Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1856->-1858
    2026-09-29T19:07:59.003Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1858->-1859
    2026-09-29T19:07:59.037Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.298, 671.901, 14.969) to=(-61.36, 682.777, 14.981)
    2026-09-29T19:07:59.038Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.36, 682.777, 14.981) dir=(-0.904, 0.427, 0)
    2026-09-29T19:07:59.039Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1859->-1862
    2026-09-29T19:07:59.072Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1862->-1864
    2026-09-29T19:07:59.105Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.821, 707.834, 15.141) to=(-93.117, 605.55, 16.953)
    2026-09-29T19:07:59.106Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1864->-1865
    2026-09-29T19:07:59.140Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.399, 687.036, 14.974) to=(-62.982, 682.273, 15.024)
    2026-09-29T19:07:59.141Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1865->-1869
    2026-09-29T19:07:59.142Z [INFO] [autopilot] event VehicleDamaged handle=7429 916->904 engine 1000->1000
    2026-09-29T19:07:59.176Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.511, 656.582, 15.061) to=(-327.406, 1408.264, 10.367)
    2026-09-29T19:07:59.177Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1869->-1870
    2026-09-29T19:07:59.211Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1870->-1873
    2026-09-29T19:07:59.245Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1873->-1875
    2026-09-29T19:07:59.278Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.848, 708.117, 15.155) to=(-324.933, -44.583, 9.252)
    2026-09-29T19:07:59.279Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1875->-1876
    2026-09-29T19:07:59.311Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.405, 687.037, 14.973) to=(-825.604, 580.274, 41.424)
    2026-09-29T19:07:59.312Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1876->-1877
    2026-09-29T19:07:59.346Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.518, 656.542, 15.184) to=(-61.468, 682.705, 15.137)
    2026-09-29T19:07:59.347Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.468, 682.705, 15.137) dir=(-0.355, 0.935, -0.002)
    2026-09-29T19:07:59.347Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1877->-1879
    2026-09-29T19:07:59.378Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.309, 648.993, 15.187) to=(177.86, 1409.415, 32.089)
    2026-09-29T19:07:59.379Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1879->-1882
    2026-09-29T19:07:59.410Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1882->-1885
    2026-09-29T19:07:59.439Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.888, 708.366, 15.121) to=(-89.801, 609.549, 16.658)
    2026-09-29T19:07:59.440Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1885->-1887
    2026-09-29T19:07:59.471Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1887->-1889
    2026-09-29T19:07:59.500Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.51, 656.551, 15.135) to=(-61.281, 682.809, 15.554)
    2026-09-29T19:07:59.501Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.281, 682.809, 15.554) dir=(-0.349, 0.937, 0.015)
    2026-09-29T19:07:59.502Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1889->-1890
    2026-09-29T19:07:59.533Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.34, 648.977, 15.202) to=(163.976, 1413.969, 13.021)
    2026-09-29T19:07:59.534Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1890->-1891
    2026-09-29T19:07:59.566Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1891->-1893
    2026-09-29T19:07:59.598Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.911, 708.564, 15.135) to=(-61.472, 683.009, 15.007)
    2026-09-29T19:07:59.599Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.472, 683.009, 15.007) dir=(-0.35, -0.937, -0.005)
    2026-09-29T19:07:59.600Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1893->-1894
    2026-09-29T19:07:59.629Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1894->-1897
    2026-09-29T19:07:59.661Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1897->-1898
    2026-09-29T19:07:59.693Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.5, 656.572, 15.047) to=(-343.115, 1402.294, 12.064)
    2026-09-29T19:07:59.693Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1898->-1900
    2026-09-29T19:07:59.729Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.337, 648.974, 15.193) to=(-31.782, 782.263, 16.08)
    2026-09-29T19:07:59.729Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1900->-1902
    2026-09-29T19:07:59.764Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.912, 708.756, 15.148) to=(-67.29, 663.522, 15.07)
    2026-09-29T19:07:59.765Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1902->-1903
    2026-09-29T19:07:59.797Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1903->-1904
    2026-09-29T19:07:59.831Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1904->-1907
    2026-09-29T19:07:59.863Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-72.335, 648.978, 15.207) to=(177.979, 1409.364, 31.91)
    2026-09-29T19:07:59.864Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.508, 656.561, 15.063) to=(-338.625, 1404.035, 7.497)
    2026-09-29T19:07:59.865Z [INFO] [autopilot] event PedAppeared handle=7698
    2026-09-29T19:07:59.865Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1907->-1908
    2026-09-29T19:07:59.897Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1908->-1911
    2026-09-29T19:07:59.928Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-51.959, 708.966, 15.144) to=(-62.802, 678.777, 15.964)
    2026-09-29T19:07:59.929Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1911->-1914
    2026-09-29T19:07:59.962Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1914->-1917
    2026-09-29T19:07:59.996Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1917->-1920
    2026-09-29T19:08:00.039Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1920->-1922
    2026-09-29T19:08:00.075Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1922->-1924
    2026-09-29T19:08:00.115Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1924->-1926
    2026-09-29T19:08:00.151Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1926->-1929
    2026-09-29T19:08:00.188Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1929->-1931
    2026-09-29T19:08:00.224Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1931->-1933
    2026-09-29T19:08:00.261Z [INFO] [autopilot] event PedRemoved handle=7698
    2026-09-29T19:08:00.262Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1933->-1936
    2026-09-29T19:08:00.300Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.686, 670.19, 14.944) to=(-163.729, 736.197, 15.862)
    2026-09-29T19:08:00.301Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1936->-1938
    2026-09-29T19:08:00.343Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1938->-1940
    2026-09-29T19:08:00.382Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1940->-1943
    2026-09-29T19:08:00.419Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1943->-1945
    2026-09-29T19:08:00.460Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.618, 669.96, 15.009) to=(-62.711, 682.867, 14.919)
    2026-09-29T19:08:00.461Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.43, 687.062, 14.963) to=(-826.523, 584.952, 26.456)
    2026-09-29T19:08:00.462Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1945->-1947
    2026-09-29T19:08:00.463Z [INFO] [autopilot] event VehicleDamaged handle=7429 904->898 engine 1000->1000
    2026-09-29T19:08:00.503Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1947->-1950
    2026-09-29T19:08:00.542Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1950->-1951
    2026-09-29T19:08:00.581Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1951->-1952
    2026-09-29T19:08:00.615Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.555, 669.747, 15.087) to=(-61.528, 682.595, 15.204)
    2026-09-29T19:08:00.616Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.528, 682.595, 15.204) dir=(-0.881, 0.472, 0.004)
    2026-09-29T19:08:00.616Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1952->-1955
    2026-09-29T19:08:00.656Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1955->-1959
    2026-09-29T19:08:00.691Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1959->-1960
    2026-09-29T19:08:00.725Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1960->-1963
    2026-09-29T19:08:00.759Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1963->-1966
    2026-09-29T19:08:00.798Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.459, 669.494, 15.117) to=(-743.815, 1046.421, 25.275)
    2026-09-29T19:08:00.799Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1966->-1969
    2026-09-29T19:08:00.834Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1969->-1971
    2026-09-29T19:08:00.867Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1971->-1972
    2026-09-29T19:08:00.901Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1972->-1975
    2026-09-29T19:08:00.939Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.37, 669.301, 15.105) to=(-740.163, 1052.328, 46.334)
    2026-09-29T19:08:00.939Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1975->-1979
    2026-09-29T19:08:00.974Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1979->-1982
    2026-09-29T19:08:01.007Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.179, 710.151, 15.09) to=(-297.843, -51.747, 31.057)
    2026-09-29T19:08:01.008Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1982->-1984
    2026-09-29T19:08:01.043Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1984->-1987
    2026-09-29T19:08:01.079Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.282, 669.119, 15.099) to=(-163.027, 737.103, 17.636)
    2026-09-29T19:08:01.080Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1987->-1990
    2026-09-29T19:08:01.116Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1990->-1993
    2026-09-29T19:08:01.149Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-70.772, 649.848, 15.132) to=(145.914, 1420.456, 35.401)
    2026-09-29T19:08:01.150Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1993->-1996
    2026-09-29T19:08:01.185Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.158, 710.31, 15.108) to=(-88.63, 609.195, 17.158)
    2026-09-29T19:08:01.186Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1996->-1997
    2026-09-29T19:08:01.219Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1997->-1998
    2026-09-29T19:08:01.252Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1998->-1999
    2026-09-29T19:08:01.284Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -1999->-2001
    2026-09-29T19:08:01.320Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-70.339, 650.465, 15.116) to=(-67.395, 661.471, 15.031)
    2026-09-29T19:08:01.321Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2001->-2003
    2026-09-29T19:08:01.353Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2003->-2007
    2026-09-29T19:08:01.386Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2007->-2010
    2026-09-29T19:08:01.419Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2010->-2012
    2026-09-29T19:08:01.452Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.411, 687.067, 14.965) to=(-826.115, 582.252, 25.995)
    2026-09-29T19:08:01.453Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2012->-2015
    2026-09-29T19:08:01.486Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2015->-2016
    2026-09-29T19:08:01.515Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.493, 656.616, 15.041) to=(-336.868, 1404.627, 18.17)
    2026-09-29T19:08:01.516Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2016->-2018
    2026-09-29T19:08:01.546Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2018->-2021
    2026-09-29T19:08:01.575Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2021->-2023
    2026-09-29T19:08:01.605Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2023->-2024
    2026-09-29T19:08:01.635Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2024->-2027
    2026-09-29T19:08:01.665Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2027->-2029
    2026-09-29T19:08:01.694Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2029->-2032
    2026-09-29T19:08:01.724Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2032->-2034
    2026-09-29T19:08:01.752Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2034->-2035
    2026-09-29T19:08:01.783Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2035->-2037
    2026-09-29T19:08:01.812Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2037->-2040
    2026-09-29T19:08:01.843Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2040->-2043
    2026-09-29T19:08:01.880Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2043->-2046
    2026-09-29T19:08:01.909Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2046->-2047
    2026-09-29T19:08:01.940Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2047->-2048
    2026-09-29T19:08:01.971Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2048->-2051
    2026-09-29T19:08:02.001Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2051->-2053
    2026-09-29T19:08:02.004Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:08:02.032Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2053->-2056
    2026-09-29T19:08:02.061Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2056->-2057
    2026-09-29T19:08:02.092Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2057->-2059
    2026-09-29T19:08:02.124Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2059->-2062
    2026-09-29T19:08:02.158Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2062->-2064
    2026-09-29T19:08:02.191Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2064->-2066
    2026-09-29T19:08:02.226Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2066->-2069
    2026-09-29T19:08:02.259Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2069->-2070
    2026-09-29T19:08:02.296Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.179, 710.551, 15.008) to=(-312.136, -46.394, 36.628)
    2026-09-29T19:08:02.297Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2070->-2073
    2026-09-29T19:08:02.332Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.742, 667.771, 15.079) to=(-345.53, 849.589, 15.345)
    2026-09-29T19:08:02.333Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2073->-2076
    2026-09-29T19:08:02.370Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2076->-2080
    2026-09-29T19:08:02.405Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2080->-2081
    2026-09-29T19:08:02.441Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.14, 710.55, 14.998) to=(-308.294, -48.004, 15.792)
    2026-09-29T19:08:02.442Z [INFO] [autopilot] event PedAppeared handle=11528
    2026-09-29T19:08:02.442Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2081->-2083
    2026-09-29T19:08:02.480Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2083->-2085
    2026-09-29T19:08:02.517Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.628, 667.583, 15.074) to=(-725.061, 1076.258, 26.038)
    2026-09-29T19:08:02.518Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2085->-2087
    2026-09-29T19:08:02.554Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2087->-2089
    2026-09-29T19:08:02.591Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.132, 710.538, 14.994) to=(-61.61, 683.061, 15.175)
    2026-09-29T19:08:02.593Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4D0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.61, 683.061, 15.175) dir=(-0.326, -0.945, 0.006)
    2026-09-29T19:08:02.593Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2089->-2091
    2026-09-29T19:08:02.652Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2091->-2095
    2026-09-29T19:08:02.710Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.533, 667.458, 15.055) to=(-61.708, 682.463, 14.516)
    2026-09-29T19:08:02.711Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2095->-2100
    2026-09-29T19:08:02.712Z [INFO] [autopilot] event VehicleDamaged handle=7429 898->892 engine 1000->1000
    2026-09-29T19:08:02.778Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.122, 710.533, 15.09) to=(-317.881, -44.686, 26.069)
    2026-09-29T19:08:02.779Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.403, 687.063, 14.976) to=(-333.546, 643.172, 26.237)
    2026-09-29T19:08:02.779Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2100->-2104
    2026-09-29T19:08:02.829Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.51, 667.363, 15.058) to=(-720.171, 1083.919, 22.456)
    2026-09-29T19:08:02.830Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2104->-2108
    2026-09-29T19:08:02.898Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2108->-2112
    2026-09-29T19:08:02.951Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.112, 710.549, 15.169) to=(-305.609, -48.882, 11.033)
    2026-09-29T19:08:02.952Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.37, 687.043, 14.966) to=(-822.052, 554.038, 17.188)
    2026-09-29T19:08:02.953Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2112->-2114
    2026-09-29T19:08:02.994Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.466, 667.171, 15.082) to=(-61.321, 682.806, 15.044)
    2026-09-29T19:08:02.994Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.321, 682.806, 15.044) dir=(-0.846, 0.532, -0.001)
    2026-09-29T19:08:02.995Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2114->-2117
    2026-09-29T19:08:03.057Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2117->-2122
    2026-09-29T19:08:03.128Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.363, 687.057, 14.963) to=(-333.901, 642.06, 24.205)
    2026-09-29T19:08:03.129Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2122->-2126
    2026-09-29T19:08:03.166Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.486, 656.625, 15.033) to=(-93.482, 764.529, 16.209)
    2026-09-29T19:08:03.167Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2126->-2130
    2026-09-29T19:08:03.168Z [INFO] [autopilot] event VehicleAppeared handle=5895
    2026-09-29T19:08:03.209Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2130->-2132
    2026-09-29T19:08:03.251Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2132->-2134
    2026-09-29T19:08:03.290Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2134->-2138
    2026-09-29T19:08:03.326Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.501, 656.593, 15.046) to=(-316.241, 1412.258, 30.702)
    2026-09-29T19:08:03.327Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2138->-2142
    2026-09-29T19:08:03.366Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2142->-2145
    2026-09-29T19:08:03.401Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2145->-2148
    2026-09-29T19:08:03.433Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2148->-2150
    2026-09-29T19:08:03.465Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.492, 656.59, 15.038) to=(-61.555, 682.508, 15.272)
    2026-09-29T19:08:03.466Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.555, 682.508, 15.272) dir=(-0.362, 0.932, 0.008)
    2026-09-29T19:08:03.467Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2150->-2152
    2026-09-29T19:08:03.503Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2152->-2154
    2026-09-29T19:08:03.537Z [INFO] [autopilot] event PedAppeared handle=11015
    2026-09-29T19:08:03.538Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2154->-2156
    2026-09-29T19:08:03.583Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2156->-2159
    2026-09-29T19:08:03.626Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.495, 656.595, 15.051) to=(-95.273, 764.082, 15.794)
    2026-09-29T19:08:03.627Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2159->-2162
    2026-09-29T19:08:03.657Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2162->-2163
    2026-09-29T19:08:03.688Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2163->-2165
    2026-09-29T19:08:03.720Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2165->-2167
    2026-09-29T19:08:03.750Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2167->-2169
    2026-09-29T19:08:03.782Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.504, 656.585, 15.043) to=(-94.552, 764.082, 17.531)
    2026-09-29T19:08:03.783Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2169->-2171
    2026-09-29T19:08:03.813Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2171->-2173
    2026-09-29T19:08:03.846Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2173->-2175
    2026-09-29T19:08:03.878Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2175->-2177
    2026-09-29T19:08:03.909Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2177->-2178
    2026-09-29T19:08:03.939Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2178->-2180
    2026-09-29T19:08:03.970Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2180->-2182
    2026-09-29T19:08:03.971Z [INFO] [autopilot] event VehicleAppeared handle=6662
    2026-09-29T19:08:04.002Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2182->-2185
    2026-09-29T19:08:04.032Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2185->-2188
    2026-09-29T19:08:04.065Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2188->-2190
    2026-09-29T19:08:04.096Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2190->-2192
    2026-09-29T19:08:04.128Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2192->-2194
    2026-09-29T19:08:04.158Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2194->-2196
    2026-09-29T19:08:04.190Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.171, 710.519, 15.024) to=(-62.963, 678.822, 14.914)
    2026-09-29T19:08:04.191Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2196->-2198
    2026-09-29T19:08:04.222Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.383, 687.037, 15.036) to=(-391.087, 641.697, 19.394)
    2026-09-29T19:08:04.223Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2198->-2199
    2026-09-29T19:08:04.261Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.242, 666.793, 14.964) to=(-721.837, 1080.403, 28.248)
    2026-09-29T19:08:04.262Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2199->-2203
    2026-09-29T19:08:04.298Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2203->-2205
    2026-09-29T19:08:04.332Z [INFO] [autopilot] event PedAppeared handle=9736
    2026-09-29T19:08:04.333Z [INFO] [autopilot] event PedAppeared handle=9993
    2026-09-29T19:08:04.334Z [INFO] [autopilot] event PedAppeared handle=10506
    2026-09-29T19:08:04.335Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2205->-2206
    2026-09-29T19:08:04.369Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.145, 710.533, 15.034) to=(-320.264, -43.92, 24.566)
    2026-09-29T19:08:04.370Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2206->-2208
    2026-09-29T19:08:04.405Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2208->-2210
    2026-09-29T19:08:04.441Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2210->-2211
    2026-09-29T19:08:04.476Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2211->-2214
    2026-09-29T19:08:04.513Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.149, 710.539, 15.032) to=(-61.662, 682.745, 14.839)
    2026-09-29T19:08:04.514Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.662, 682.745, 14.839) dir=(-0.324, -0.946, -0.007)
    2026-09-29T19:08:04.515Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2214->-2217
    2026-09-29T19:08:04.554Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2217->-2219
    2026-09-29T19:08:04.591Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2219->-2221
    2026-09-29T19:08:04.628Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2221->-2223
    2026-09-29T19:08:04.668Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.491, 656.604, 15.038) to=(-90.778, 764.082, 16.813)
    2026-09-29T19:08:04.669Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2223->-2225
    2026-09-29T19:08:04.711Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.159, 710.538, 15.038) to=(-61.302, 682.932, 15.03)
    2026-09-29T19:08:04.712Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.302, 682.932, 15.03) dir=(-0.314, -0.949, 0)
    2026-09-29T19:08:04.713Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2225->-2228
    2026-09-29T19:08:04.754Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2228->-2230
    2026-09-29T19:08:04.796Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2230->-2232
    2026-09-29T19:08:04.836Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2232->-2235
    2026-09-29T19:08:04.875Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2235->-2237
    2026-09-29T19:08:04.915Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2237->-2240
    2026-09-29T19:08:04.950Z [INFO] [autopilot] event PedRemoved handle=11528
    2026-09-29T19:08:04.951Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2240->-2242
    2026-09-29T19:08:04.988Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2242->-2244
    2026-09-29T19:08:05.024Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2244->-2247
    2026-09-29T19:08:05.063Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2247->-2249
    2026-09-29T19:08:05.097Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2249->-2252
    2026-09-29T19:08:05.132Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2252->-2253
    2026-09-29T19:08:05.171Z [INFO] [autopilot] event PedAppeared handle=7176
    2026-09-29T19:08:05.172Z [INFO] [autopilot] event PedAppeared handle=8715
    2026-09-29T19:08:05.172Z [INFO] [autopilot] event PedAppeared handle=9231
    2026-09-29T19:08:05.173Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2253->-2254
    2026-09-29T19:08:05.209Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2254->-2256
    2026-09-29T19:08:05.242Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2256->-2260
    2026-09-29T19:08:05.279Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2260->-2262
    2026-09-29T19:08:05.316Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2262->-2266
    2026-09-29T19:08:05.350Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2266->-2267
    2026-09-29T19:08:05.384Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.095, 668.237, 15.114) to=(-61.622, 682.675, 14.999)
    2026-09-29T19:08:05.385Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.622, 682.675, 14.999) dir=(-0.862, 0.507, -0.004)
    2026-09-29T19:08:05.386Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2267->-2269
    2026-09-29T19:08:05.421Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2269->-2272
    2026-09-29T19:08:05.454Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2272->-2275
    2026-09-29T19:08:05.488Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2275->-2277
    2026-09-29T19:08:05.522Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.398, 687.053, 14.993) to=(-824.313, 571.383, 40.38)
    2026-09-29T19:08:05.523Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2277->-2280
    2026-09-29T19:08:05.559Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.275, 668.586, 15.112) to=(-61.685, 682.608, 14.507)
    2026-09-29T19:08:05.560Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2280->-2283
    2026-09-29T19:08:05.560Z [INFO] [autopilot] event VehicleDamaged handle=7429 892->886 engine 1000->1000
    2026-09-29T19:08:05.593Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2283->-2286
    2026-09-29T19:08:05.628Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2286->-2288
    2026-09-29T19:08:05.662Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-137.05, 729.942, 46.594) to=(-54.572, 680.486, 13.708)
    2026-09-29T19:08:05.663Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2288->-2291
    2026-09-29T19:08:05.696Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.426, 668.91, 15.101) to=(-737.52, 1057.484, 19.266)
    2026-09-29T19:08:05.697Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.375, 687.034, 14.988) to=(-278.689, 652.584, 22.566)
    2026-09-29T19:08:05.698Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2291->-2294
    2026-09-29T19:08:05.735Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2294->-2295
    2026-09-29T19:08:05.770Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2295->-2298
    2026-09-29T19:08:05.801Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.462, 656.568, 15.064) to=(-349.861, 1399.5, 28.47)
    2026-09-29T19:08:05.802Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2298->-2300
    2026-09-29T19:08:05.839Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-133.553, 729.004, 47.542) to=(-57.977, 682.278, 13.656)
    2026-09-29T19:08:05.839Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.191, 710.494, 15.011) to=(-61.529, 683.09, 15.33)
    2026-09-29T19:08:05.840Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4D0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.529, 683.09, 15.33) dir=(-0.323, -0.946, 0.011)
    2026-09-29T19:08:05.841Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2300->-2303
    2026-09-29T19:08:05.872Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.632, 669.273, 15.116) to=(-731.59, 1068.151, 36.731)
    2026-09-29T19:08:05.873Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.375, 687.039, 14.978) to=(-822.713, 559.622, 28.991)
    2026-09-29T19:08:05.874Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2303->-2306
    2026-09-29T19:08:05.904Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2306->-2308
    2026-09-29T19:08:05.936Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.195, 654.116, 15.089) to=(71.099, 1442.722, 31.57)
    2026-09-29T19:08:05.937Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2308->-2310
    2026-09-29T19:08:05.969Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2310->-2312
    2026-09-29T19:08:06.000Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.16, 710.509, 15.022) to=(-62.574, 678.555, 15.151)
    2026-09-29T19:08:06.000Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2312->-2315
    2026-09-29T19:08:06.033Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-129.678, 727.816, 48.802) to=(-55.551, 677.086, 13.703)
    2026-09-29T19:08:06.034Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.822, 669.596, 15.045) to=(-735.868, 1061.753, 13.043)
    2026-09-29T19:08:06.034Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2315->-2317
    2026-09-29T19:08:06.066Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2317->-2318
    2026-09-29T19:08:06.098Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.117, 654.016, 15.136) to=(64.241, 1443.994, 7.889)
    2026-09-29T19:08:06.099Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2318->-2320
    2026-09-29T19:08:06.132Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.163, 710.514, 15.016) to=(-61.329, 682.863, 15.793)
    2026-09-29T19:08:06.133Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.329, 682.863, 15.793) dir=(-0.315, -0.949, 0.027)
    2026-09-29T19:08:06.134Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2320->-2323
    2026-09-29T19:08:06.166Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2323->-2326
    2026-09-29T19:08:06.202Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.961, 669.948, 14.994) to=(-731.322, 1069.939, 43.077)
    2026-09-29T19:08:06.202Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2326->-2327
    2026-09-29T19:08:06.236Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2327->-2328
    2026-09-29T19:08:06.291Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.115, 654.013, 15.126) to=(-24.657, 905.326, 23.436)
    2026-09-29T19:08:06.292Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2328->-2332
    2026-09-29T19:08:06.327Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.169, 710.512, 15.026) to=(-67.251, 663.109, 15.043)
    2026-09-29T19:08:06.328Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2332->-2334
    2026-09-29T19:08:06.362Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2334->-2336
    2026-09-29T19:08:06.397Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2336->-2338
    2026-09-29T19:08:06.431Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.111, 654.02, 15.137) to=(74.727, 1442.203, 18.814)
    2026-09-29T19:08:06.432Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2338->-2340
    2026-09-29T19:08:06.473Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.159, 710.511, 15.022) to=(-295.152, -52.361, 20.889)
    2026-09-29T19:08:06.474Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2340->-2341
    2026-09-29T19:08:06.513Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-45.204, 756.929, 72.941) to=(-61.278, 678.54, 13.531)
    2026-09-29T19:08:06.514Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2341->-2344
    2026-09-29T19:08:06.554Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2344->-2345
    2026-09-29T19:08:06.592Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.133, 654.021, 15.134) to=(-25.495, 917.332, 18.074)
    2026-09-29T19:08:06.593Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2345->-2348
    2026-09-29T19:08:06.633Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.162, 710.517, 15.027) to=(-297.176, -51.614, 25.893)
    2026-09-29T19:08:06.633Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2348->-2352
    2026-09-29T19:08:06.674Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-45.64, 752.629, 74.572) to=(-61.718, 678.406, 13.658)
    2026-09-29T19:08:06.675Z [INFO] [autopilot] event PedRemoved handle=11015
    2026-09-29T19:08:06.675Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2352->-2354
    2026-09-29T19:08:06.712Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2354->-2356
    2026-09-29T19:08:06.755Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2356->-2360
    2026-09-29T19:08:06.800Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2360->-2364
    2026-09-29T19:08:06.846Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.507, 656.572, 15.176) to=(-320.061, 1410.886, 21.369)
    2026-09-29T19:08:06.847Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2364->-2366
    2026-09-29T19:08:06.894Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-46.328, 747.029, 76.811) to=(-63.369, 678.51, 13.99)
    2026-09-29T19:08:06.894Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.434, 687.054, 14.959) to=(-105.537, 677.449, 17.083)
    2026-09-29T19:08:06.895Z [INFO] [autopilot] event PedAppeared handle=11529
    2026-09-29T19:08:06.896Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2366->-2368
    2026-09-29T19:08:06.940Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2368->-2370
    2026-09-29T19:08:06.984Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-112.419, 720.622, 56.134) to=(-59.072, 680.164, 13.619)
    2026-09-29T19:08:06.985Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2370->-2372
    2026-09-29T19:08:07.031Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.514, 656.557, 15.111) to=(-90.997, 763.813, 18.436)
    2026-09-29T19:08:07.032Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2372->-2377
    2026-09-29T19:08:07.074Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2377->-2380
    2026-09-29T19:08:07.114Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-47.232, 740.953, 79.406) to=(-63.97, 683.337, 15.094)
    2026-09-29T19:08:07.115Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2380->-2382
    2026-09-29T19:08:07.116Z [INFO] [autopilot] event VehicleDamaged handle=7429 886->880 engine 1000->1000
    2026-09-29T19:08:07.154Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-109.046, 718.862, 57.848) to=(-60.435, 682.791, 13.532)
    2026-09-29T19:08:07.155Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.502, 656.566, 15.038) to=(-350.833, 1399.268, 21.099)
    2026-09-29T19:08:07.156Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2382->-2385
    2026-09-29T19:08:07.200Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.143, 641.016, 15.2) to=(134.223, 1414.383, 2.609)
    2026-09-29T19:08:07.200Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2385->-2387
    2026-09-29T19:08:07.239Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2387->-2388
    2026-09-29T19:08:07.276Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-47.991, 736.496, 81.429) to=(-60.506, 683.653, 13.528)
    2026-09-29T19:08:07.277Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2388->-2390
    2026-09-29T19:08:07.322Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.497, 656.567, 15.068) to=(-78.865, 730.221, 17.355)
    2026-09-29T19:08:07.322Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2390->-2394
    2026-09-29T19:08:07.360Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-105.436, 716.861, 59.825) to=(-59.424, 678.701, 13.646)
    2026-09-29T19:08:07.361Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.14, 641.014, 15.192) to=(150.859, 1409.684, 10.81)
    2026-09-29T19:08:07.361Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2394->-2396
    2026-09-29T19:08:07.398Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2396->-2397
    2026-09-29T19:08:07.437Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2397->-2399
    2026-09-29T19:08:07.473Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.14, 641.022, 15.206) to=(-51.998, 710.725, 14.77)
    2026-09-29T19:08:07.474Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2399->-2401
    2026-09-29T19:08:07.512Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.742, 672.581, 14.975) to=(-61.314, 682.924, 14.834)
    2026-09-29T19:08:07.513Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.314, 682.924, 14.834) dir=(-0.909, 0.417, -0.006)
    2026-09-29T19:08:07.513Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2401->-2405
    2026-09-29T19:08:07.550Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2405->-2408
    2026-09-29T19:08:07.586Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2408->-2411
    2026-09-29T19:08:07.624Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.158, 641.023, 15.199) to=(-37.042, 763.256, 14.451)
    2026-09-29T19:08:07.625Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2411->-2413
    2026-09-29T19:08:07.668Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.186, 710.519, 15.014) to=(-307.096, -48.459, 15.038)
    2026-09-29T19:08:07.668Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2413->-2415
    2026-09-29T19:08:07.712Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.648, 672.897, 14.996) to=(-768.469, 1001.958, 26.31)
    2026-09-29T19:08:07.713Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2415->-2419
    2026-09-29T19:08:07.754Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2419->-2421
    2026-09-29T19:08:07.795Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.153, 641.036, 15.202) to=(139.848, 1412.742, 23.082)
    2026-09-29T19:08:07.796Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2421->-2424
    2026-09-29T19:08:07.838Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.76, 673.13, 15.004) to=(-67.068, 685.636, 14.896)
    2026-09-29T19:08:07.839Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.15, 710.539, 15.035) to=(-61.535, 682.928, 14.96)
    2026-09-29T19:08:07.840Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.535, 682.928, 14.96) dir=(-0.322, -0.947, -0.003)
    2026-09-29T19:08:07.840Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2424->-2426
    2026-09-29T19:08:07.880Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2426->-2430
    2026-09-29T19:08:07.919Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2430->-2432
    2026-09-29T19:08:07.956Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2432->-2435
    2026-09-29T19:08:07.991Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2435->-2437
    2026-09-29T19:08:08.023Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.913, 673.54, 14.985) to=(-783.132, 968.387, 30.515)
    2026-09-29T19:08:08.024Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.152, 710.539, 15.022) to=(-61.735, 682.91, 15.077)
    2026-09-29T19:08:08.025Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.735, 682.91, 15.077) dir=(-0.328, -0.945, 0.002)
    2026-09-29T19:08:08.025Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2437->-2439
    2026-09-29T19:08:08.057Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2439->-2440
    2026-09-29T19:08:08.091Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2440->-2442
    2026-09-29T19:08:08.126Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2442->-2444
    2026-09-29T19:08:08.159Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-51.172, 712.875, 93.48) to=(-61.19, 683.044, 13.507)
    2026-09-29T19:08:08.160Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.156, 710.535, 15.039) to=(-84.784, 609.47, 16.557)
    2026-09-29T19:08:08.160Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2444->-2446
    2026-09-29T19:08:08.193Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.061, 673.883, 14.924) to=(-771.955, 997.061, 27.504)
    2026-09-29T19:08:08.194Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2446->-2447
    2026-09-29T19:08:08.227Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2447->-2450
    2026-09-29T19:08:08.261Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-88.418, 707.985, 70.193) to=(-61.929, 683.736, 14.629)
    2026-09-29T19:08:08.262Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2450->-2451
    2026-09-29T19:08:08.263Z [INFO] [autopilot] event VehicleDamaged handle=7429 880->874 engine 1000->1000
    2026-09-29T19:08:08.298Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2451->-2454
    2026-09-29T19:08:08.331Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-52.095, 709.004, 95.644) to=(-61.254, 681.041, 13.513)
    2026-09-29T19:08:08.332Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.148, 710.534, 15.033) to=(-61.53, 682.889, 15.514)
    2026-09-29T19:08:08.333Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.53, 682.889, 15.514) dir=(-0.321, -0.947, 0.016)
    2026-09-29T19:08:08.333Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2454->-2457
    2026-09-29T19:08:08.370Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2457->-2460
    2026-09-29T19:08:08.405Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2460->-2463
    2026-09-29T19:08:08.443Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-85.875, 706.199, 72.087) to=(-59.4, 679.99, 13.604)
    2026-09-29T19:08:08.444Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.492, 656.601, 15.045) to=(-338.969, 1403.979, 14.86)
    2026-09-29T19:08:08.445Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2463->-2465
    2026-09-29T19:08:08.482Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.421, 687.073, 14.966) to=(-825.61, 579.463, 33.345)
    2026-09-29T19:08:08.483Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2465->-2466
    2026-09-29T19:08:08.516Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2466->-2468
    2026-09-29T19:08:08.553Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-53.297, 704.092, 98.204) to=(-60.331, 681.763, 13.539)
    2026-09-29T19:08:08.554Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2468->-2472
    2026-09-29T19:08:08.593Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2472->-2475
    2026-09-29T19:08:08.634Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2475->-2477
    2026-09-29T19:08:08.680Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.393, 687.047, 14.979) to=(-334.779, 643.864, 25.797)
    2026-09-29T19:08:08.681Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2477->-2480
    2026-09-29T19:08:08.722Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2480->-2481
    2026-09-29T19:08:08.758Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2481->-2484
    2026-09-29T19:08:08.806Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2484->-2487
    2026-09-29T19:08:08.843Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2487->-2490
    2026-09-29T19:08:08.884Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.183, 654.116, 15.096) to=(-28.406, 854.341, 16.026)
    2026-09-29T19:08:08.885Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2490->-2493
    2026-09-29T19:08:08.925Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2493->-2496
    2026-09-29T19:08:08.968Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2496->-2498
    2026-09-29T19:08:09.012Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2498->-2500
    2026-09-29T19:08:09.052Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.107, 654.034, 15.127) to=(-27.262, 864.656, 16.762)
    2026-09-29T19:08:09.053Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2500->-2502
    2026-09-29T19:08:09.097Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.476, 675.551, 15.02) to=(-93.425, 693.722, 16.703)
    2026-09-29T19:08:09.098Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2502->-2504
    2026-09-29T19:08:09.142Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2504->-2507
    2026-09-29T19:08:09.189Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2507->-2510
    2026-09-29T19:08:09.232Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.11, 654.036, 15.128) to=(-61.409, 682.85, 14.884)
    2026-09-29T19:08:09.233Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=11271 vehicle=0 killed=False hit=True at=(-61.409, 682.85, 14.884) dir=(0.161, 0.987, -0.008)
    2026-09-29T19:08:09.233Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2510->-2511
    2026-09-29T19:08:09.271Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.575, 675.876, 15.035) to=(-794.867, 942.105, 30.932)
    2026-09-29T19:08:09.272Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2511->-2515
    2026-09-29T19:08:09.310Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2515->-2517
    2026-09-29T19:08:09.348Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.117, 654.046, 15.142) to=(-62.672, 678.348, 15.527)
    2026-09-29T19:08:09.349Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2517->-2520
    2026-09-29T19:08:09.392Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2520->-2523
    2026-09-29T19:08:09.428Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.69, 676.206, 15.025) to=(-61.423, 682.772, 15.734)
    2026-09-29T19:08:09.429Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.423, 682.772, 15.734) dir=(-0.957, 0.289, 0.031)
    2026-09-29T19:08:09.429Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2523->-2525
    2026-09-29T19:08:09.469Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2525->-2527
    2026-09-29T19:08:09.509Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.138, 654.054, 15.131) to=(-21.522, 945.597, 24.272)
    2026-09-29T19:08:09.509Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2527->-2530
    2026-09-29T19:08:09.544Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2530->-2532
    2026-09-29T19:08:09.580Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.811, 676.496, 15.056) to=(-808.957, 899.242, 7.649)
    2026-09-29T19:08:09.581Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2532->-2533
    2026-09-29T19:08:09.618Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2533->-2535
    2026-09-29T19:08:09.655Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.175, 710.552, 15.011) to=(-88.095, 609.195, 17.591)
    2026-09-29T19:08:09.656Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2535->-2537
    2026-09-29T19:08:09.701Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2537->-2538
    2026-09-29T19:08:09.734Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2538->-2540
    2026-09-29T19:08:09.768Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.939, 676.841, 15.061) to=(-808.133, 901.746, 36.621)
    2026-09-29T19:08:09.769Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2540->-2543
    2026-09-29T19:08:09.805Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2543->-2545
    2026-09-29T19:08:09.840Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.405, 687.062, 14.975) to=(-824.364, 573.403, 47.02)
    2026-09-29T19:08:09.841Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2545->-2548
    2026-09-29T19:08:09.876Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.492, 656.628, 15.031) to=(-93.607, 763.813, 18.418)
    2026-09-29T19:08:09.876Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2548->-2552
    2026-09-29T19:08:09.911Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2552->-2553
    2026-09-29T19:08:09.944Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2553->-2556
    2026-09-29T19:08:09.977Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2556->-2558
    2026-09-29T19:08:10.014Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-68.694, 693.064, 83.736) to=(-59.565, 682.517, 13.578)
    2026-09-29T19:08:10.015Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2558->-2560
    2026-09-29T19:08:10.055Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.511, 656.598, 15.043) to=(-61.815, 682.7, 15.026)
    2026-09-29T19:08:10.056Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.815, 682.7, 15.026) dir=(-0.367, 0.93, -0.001)
    2026-09-29T19:08:10.056Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2560->-2563
    2026-09-29T19:08:10.086Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2563->-2565
    2026-09-29T19:08:10.118Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2565->-2569
    2026-09-29T19:08:10.148Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2569->-2571
    2026-09-29T19:08:10.179Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-67.477, 691.981, 84.529) to=(-61.657, 681.473, 13.658)
    2026-09-29T19:08:10.180Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2571->-2574
    2026-09-29T19:08:10.211Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2574->-2576
    2026-09-29T19:08:10.247Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2576->-2579
    2026-09-29T19:08:10.279Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2579->-2580
    2026-09-29T19:08:10.312Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2580->-2583
    2026-09-29T19:08:10.343Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2583->-2584
    2026-09-29T19:08:10.375Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2584->-2585
    2026-09-29T19:08:10.406Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-66.107, 690.751, 85.422) to=(-61.889, 680.941, 13.687)
    2026-09-29T19:08:10.407Z [INFO] [autopilot] event PedRemoved handle=11529
    2026-09-29T19:08:10.408Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2585->-2588
    2026-09-29T19:08:10.443Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2588->-2589
    2026-09-29T19:08:10.475Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2589->-2590
    2026-09-29T19:08:10.507Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2590->-2593
    2026-09-29T19:08:10.541Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2593->-2594
    2026-09-29T19:08:10.571Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2594->-2596
    2026-09-29T19:08:10.604Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2596->-2599
    2026-09-29T19:08:10.636Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2599->-2601
    2026-09-29T19:08:10.667Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2601->-2603
    2026-09-29T19:08:10.700Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2603->-2605
    2026-09-29T19:08:10.738Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.175, 710.526, 15.016) to=(-62.611, 678.631, 15.847)
    2026-09-29T19:08:10.739Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2605->-2607
    2026-09-29T19:08:10.779Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2607->-2609
    2026-09-29T19:08:10.813Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2609->-2612
    2026-09-29T19:08:10.848Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2612->-2615
    2026-09-29T19:08:10.885Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2615->-2617
    2026-09-29T19:08:10.926Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2617->-2620
    2026-09-29T19:08:10.961Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2620->-2622
    2026-09-29T19:08:11.001Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2622->-2625
    2026-09-29T19:08:11.038Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2625->-2629
    2026-09-29T19:08:11.077Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2629->-2632
    2026-09-29T19:08:11.116Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2632->-2636
    2026-09-29T19:08:11.156Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2636->-2637
    2026-09-29T19:08:11.198Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2637->-2640
    2026-09-29T19:08:11.239Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.375, 687.074, 14.963) to=(-821.963, 553.846, 20.508)
    2026-09-29T19:08:11.240Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2640->-2642
    2026-09-29T19:08:11.282Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-67.154, 663.561, 106.216) to=(-62.532, 683.28, 14.788)
    2026-09-29T19:08:11.282Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2642->-2645
    2026-09-29T19:08:11.283Z [INFO] [autopilot] event VehicleDamaged handle=7429 874->868 engine 1000->1000
    2026-09-29T19:08:11.324Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2645->-2648
    2026-09-29T19:08:11.363Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.498, 656.581, 15.047) to=(-321.037, 1410.457, 28.115)
    2026-09-29T19:08:11.364Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2648->-2652
    2026-09-29T19:08:11.405Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.723, 680.049, 15.081) to=(-832.009, 803.619, 14.121)
    2026-09-29T19:08:11.406Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.361, 687.04, 14.948) to=(-825.392, 577.823, 26.996)
    2026-09-29T19:08:11.406Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2652->-2656
    2026-09-29T19:08:11.442Z [INFO] [autopilot] event BulletFired shooter=9231 weapon=15 by_player=False from=(-67.58, 662.066, 106.111) to=(-58.593, 684.792, 13.625)
    2026-09-29T19:08:11.443Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2656->-2659
    2026-09-29T19:08:11.484Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.944, 640.951, 15.133) to=(125.595, 1416.605, 16.08)
    2026-09-29T19:08:11.485Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2659->-2661
    2026-09-29T19:08:11.523Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.489, 656.578, 15.038) to=(-61.363, 682.743, 15.241)
    2026-09-29T19:08:11.524Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.363, 682.743, 15.241) dir=(-0.353, 0.936, 0.007)
    2026-09-29T19:08:11.524Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2661->-2664
    2026-09-29T19:08:11.559Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-61.278, 688.041, 88.387) to=(-62.272, 681.217, 13.727)
    2026-09-29T19:08:11.560Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.769, 680.313, 15.108) to=(-836.81, 763.935, 39.801)
    2026-09-29T19:08:11.560Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.367, 687.025, 14.948) to=(-306.409, 644.528, 16.756)
    2026-09-29T19:08:11.561Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2664->-2667
    2026-09-29T19:08:11.596Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2667->-2670
    2026-09-29T19:08:11.632Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.263, 654.169, 15.081) to=(-25.157, 915.848, 21.552)
    2026-09-29T19:08:11.632Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2670->-2672
    2026-09-29T19:08:11.669Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.071, 640.983, 15.201) to=(-63.051, 678.46, 15.711)
    2026-09-29T19:08:11.670Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.492, 656.578, 15.055) to=(-341.694, 1402.819, 12.607)
    2026-09-29T19:08:11.670Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2672->-2674
    2026-09-29T19:08:11.707Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.805, 680.622, 15.098) to=(-835.067, 782.731, 20.407)
    2026-09-29T19:08:11.708Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.36, 687.019, 15.064) to=(-824.999, 574.71, 21.871)
    2026-09-29T19:08:11.708Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2674->-2676
    2026-09-29T19:08:11.746Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-60.637, 688.091, 88.814) to=(-59.908, 682.332, 13.56)
    2026-09-29T19:08:11.747Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2676->-2678
    2026-09-29T19:08:11.785Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2678->-2680
    2026-09-29T19:08:11.823Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.129, 654.039, 15.133) to=(59.849, 1444.696, 12.567)
    2026-09-29T19:08:11.824Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2680->-2682
    2026-09-29T19:08:11.862Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.837, 680.933, 15.102) to=(-61.348, 682.775, 15.02)
    2026-09-29T19:08:11.863Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.347, 687.012, 15.12) to=(-825.631, 578.846, 16.277)
    2026-09-29T19:08:11.863Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.504, 656.574, 15.046) to=(-94.25, 764.529, 15.972)
    2026-09-29T19:08:11.864Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.348, 682.775, 15.02) dir=(-0.996, 0.089, -0.004)
    2026-09-29T19:08:11.864Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2682->-2685
    2026-09-29T19:08:11.897Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2685->-2686
    2026-09-29T19:08:11.936Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2686->-2689
    2026-09-29T19:08:11.970Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.126, 654.033, 15.127) to=(-24.653, 896.67, 21.738)
    2026-09-29T19:08:11.972Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.503, 656.572, 15.051) to=(-61.466, 682.581, 15.447)
    2026-09-29T19:08:11.973Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.466, 682.581, 15.447) dir=(-0.358, 0.934, 0.014)
    2026-09-29T19:08:11.973Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2689->-2691
    2026-09-29T19:08:12.010Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.882, 681.239, 15.092) to=(-839.926, 727.736, 43.551)
    2026-09-29T19:08:12.011Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2691->-2694
    2026-09-29T19:08:12.014Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:08:12.050Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2694->-2697
    2026-09-29T19:08:12.086Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2697->-2699
    2026-09-29T19:08:12.124Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.125, 654.04, 15.141) to=(-61.47, 682.898, 14.727)
    2026-09-29T19:08:12.125Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=11271 vehicle=0 killed=False hit=True at=(-61.47, 682.898, 14.727) dir=(0.159, 0.987, -0.014)
    2026-09-29T19:08:12.125Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2699->-2702
    2026-09-29T19:08:12.163Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.497, 656.571, 15.045) to=(-61.446, 682.755, 15.729)
    2026-09-29T19:08:12.164Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.446, 682.755, 15.729) dir=(-0.355, 0.935, 0.024)
    2026-09-29T19:08:12.165Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2702->-2705
    2026-09-29T19:08:12.202Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2705->-2707
    2026-09-29T19:08:12.236Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2707->-2710
    2026-09-29T19:08:12.269Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.142, 654.039, 15.135) to=(79.21, 1441.384, 28.138)
    2026-09-29T19:08:12.269Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2710->-2714
    2026-09-29T19:08:12.303Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2714->-2716
    2026-09-29T19:08:12.335Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2716->-2717
    2026-09-29T19:08:12.367Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2717->-2719
    2026-09-29T19:08:12.400Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2719->-2722
    2026-09-29T19:08:12.433Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2722->-2723
    2026-09-29T19:08:12.466Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2723->-2726
    2026-09-29T19:08:12.498Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.051, 641.043, 15.188) to=(141.754, 1412.275, 14.651)
    2026-09-29T19:08:12.499Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2726->-2728
    2026-09-29T19:08:12.535Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2728->-2731
    2026-09-29T19:08:12.568Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2731->-2732
    2026-09-29T19:08:12.600Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2732->-2735
    2026-09-29T19:08:12.632Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2735->-2737
    2026-09-29T19:08:12.667Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.077, 641.02, 15.202) to=(-71.314, 647.415, 15.128)
    2026-09-29T19:08:12.668Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2737->-2739
    2026-09-29T19:08:12.699Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2739->-2742
    2026-09-29T19:08:12.733Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2742->-2744
    2026-09-29T19:08:12.769Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2744->-2747
    2026-09-29T19:08:12.804Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2747->-2749
    2026-09-29T19:08:12.840Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.069, 641, 15.193) to=(145.867, 1411.057, 19.946)
    2026-09-29T19:08:12.841Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2749->-2752
    2026-09-29T19:08:12.884Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-56.897, 689.27, 91.195) to=(-60.597, 682.077, 13.525)
    2026-09-29T19:08:12.885Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2752->-2754
    2026-09-29T19:08:12.935Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.189, 710.497, 15.014) to=(-84.041, 609.47, 17.394)
    2026-09-29T19:08:12.936Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2754->-2757
    2026-09-29T19:08:12.974Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2757->-2760
    2026-09-29T19:08:13.010Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.059, 640.986, 15.189) to=(-71.408, 647.39, 15.096)
    2026-09-29T19:08:13.011Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2760->-2764
    2026-09-29T19:08:13.051Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-56.425, 689.416, 91.435) to=(-60.645, 682.591, 13.522)
    2026-09-29T19:08:13.052Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2764->-2767
    2026-09-29T19:08:13.091Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.076, 683.475, 15.06) to=(-841.068, 652.883, 26.541)
    2026-09-29T19:08:13.092Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.157, 710.51, 15.027) to=(-84.886, 609.47, 17.635)
    2026-09-29T19:08:13.093Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2767->-2769
    2026-09-29T19:08:13.133Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.401, 687.055, 14.979) to=(-334.604, 641.381, 24.54)
    2026-09-29T19:08:13.134Z [INFO] [autopilot] event PedAppeared handle=11784
    2026-09-29T19:08:13.135Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2769->-2771
    2026-09-29T19:08:13.176Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.994, 641.034, 15.154) to=(-72.796, 641.794, 15.161)
    2026-09-29T19:08:13.177Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2771->-2775
    2026-09-29T19:08:13.217Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2775->-2777
    2026-09-29T19:08:13.257Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-55.931, 689.55, 91.661) to=(-62.848, 681.891, 14.627)
    2026-09-29T19:08:13.258Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.045, 683.684, 15.024) to=(-840.437, 637.963, 18.728)
    2026-09-29T19:08:13.259Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2777->-2779
    2026-09-29T19:08:13.260Z [INFO] [autopilot] event VehicleDamaged handle=7429 868->862 engine 1000->1000
    2026-09-29T19:08:13.303Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.375, 687.034, 14.987) to=(-61.84, 682.786, 14.797)
    2026-09-29T19:08:13.304Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2779->-2783
    2026-09-29T19:08:13.305Z [INFO] [autopilot] event VehicleDamaged handle=7429 862->856 engine 1000->1000
    2026-09-29T19:08:13.350Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-66.674, 648.646, 104.771) to=(-61.014, 681.994, 13.508)
    2026-09-29T19:08:13.351Z [INFO] [autopilot] event PedAppeared handle=12037
    2026-09-29T19:08:13.352Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2783->-2787
    2026-09-29T19:08:13.393Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2787->-2790
    2026-09-29T19:08:13.433Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.043, 683.772, 14.977) to=(-383.442, 671.493, 24.812)
    2026-09-29T19:08:13.434Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2790->-2794
    2026-09-29T19:08:13.480Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-55.494, 689.656, 91.839) to=(-61.07, 681.606, 13.509)
    2026-09-29T19:08:13.481Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.374, 687.041, 14.983) to=(-824.536, 570.687, 13.931)
    2026-09-29T19:08:13.482Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2794->-2796
    2026-09-29T19:08:13.522Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-66.786, 648.033, 104.636) to=(-62.069, 681.952, 14.676)
    2026-09-29T19:08:13.523Z [INFO] [autopilot] event PedRemoved handle=11784
    2026-09-29T19:08:13.524Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2796->-2800
    2026-09-29T19:08:13.525Z [INFO] [autopilot] event VehicleDamaged handle=7429 856->850 engine 1000->1000
    2026-09-29T19:08:13.568Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2800->-2803
    2026-09-29T19:08:13.607Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.03, 683.807, 15.027) to=(-61.322, 682.898, 15.182)
    2026-09-29T19:08:13.608Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.322, 682.898, 15.182) dir=(-0.999, -0.045, 0.008)
    2026-09-29T19:08:13.609Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2803->-2804
    2026-09-29T19:08:13.647Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-55.23, 689.715, 91.934) to=(-61.178, 683.805, 13.506)
    2026-09-29T19:08:13.648Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.379, 687.05, 14.989) to=(-822.878, 560.259, 23.6)
    2026-09-29T19:08:13.648Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2804->-2807
    2026-09-29T19:08:13.685Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2807->-2811
    2026-09-29T19:08:13.721Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-67.004, 647.412, 104.636) to=(-60.423, 685.306, 13.53)
    2026-09-29T19:08:13.722Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2811->-2814
    2026-09-29T19:08:13.758Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.022, 683.81, 15.001) to=(-841.499, 664.868, 30.288)
    2026-09-29T19:08:13.759Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.372, 687.036, 14.988) to=(-391.701, 643.689, 17.347)
    2026-09-29T19:08:13.760Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2814->-2816
    2026-09-29T19:08:13.801Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2816->-2818
    2026-09-29T19:08:13.837Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2818->-2821
    2026-09-29T19:08:13.877Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2821->-2822
    2026-09-29T19:08:13.912Z [INFO] [autopilot] event BulletFired shooter=8715 weapon=15 by_player=False from=(-67.261, 646.949, 104.837) to=(-62.528, 680.801, 13.718)
    2026-09-29T19:08:13.913Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2822->-2823
    2026-09-29T19:08:13.948Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2823->-2826
    2026-09-29T19:08:13.985Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.502, 656.592, 15.001) to=(-335.422, 1405.183, 16.738)
    2026-09-29T19:08:13.986Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2826->-2828
    2026-09-29T19:08:14.022Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2828->-2831
    2026-09-29T19:08:14.058Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2831->-2832
    2026-09-29T19:08:14.093Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2832->-2835
    2026-09-29T19:08:14.129Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.193, 710.5, 15.011) to=(-309.988, -47.038, 42.283)
    2026-09-29T19:08:14.130Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2835->-2837
    2026-09-29T19:08:14.165Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2837->-2839
    2026-09-29T19:08:14.203Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2839->-2842
    2026-09-29T19:08:14.235Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2842->-2845
    2026-09-29T19:08:14.269Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.162, 710.521, 15.029) to=(-63.036, 678.684, 15.777)
    2026-09-29T19:08:14.270Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2845->-2847
    2026-09-29T19:08:14.305Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2847->-2849
    2026-09-29T19:08:14.341Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2849->-2851
    2026-09-29T19:08:14.377Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2851->-2854
    2026-09-29T19:08:14.410Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2854->-2856
    2026-09-29T19:08:14.440Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.202, 654.12, 15.094) to=(-24.407, 876.734, 18.758)
    2026-09-29T19:08:14.441Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.168, 710.528, 15.019) to=(-301.839, -50.241, 11.266)
    2026-09-29T19:08:14.442Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2856->-2857
    2026-09-29T19:08:14.472Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2857->-2859
    2026-09-29T19:08:14.503Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2859->-2860
    2026-09-29T19:08:14.535Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-55.418, 690.128, 92.419) to=(-60.973, 681.452, 13.514)
    2026-09-29T19:08:14.536Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2860->-2861
    2026-09-29T19:08:14.567Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2861->-2863
    2026-09-29T19:08:14.598Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.123, 654.03, 15.133) to=(-23.789, 945.557, 18.688)
    2026-09-29T19:08:14.599Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2863->-2864
    2026-09-29T19:08:14.630Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.171, 710.525, 15.029) to=(-61.321, 682.961, 14.946)
    2026-09-29T19:08:14.631Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.321, 682.961, 14.946) dir=(-0.315, -0.949, -0.003)
    2026-09-29T19:08:14.632Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2864->-2865
    2026-09-29T19:08:14.662Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2865->-2867
    2026-09-29T19:08:14.693Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2867->-2868
    2026-09-29T19:08:14.726Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-55.69, 690.41, 92.615) to=(-62.62, 680.953, 13.738)
    2026-09-29T19:08:14.727Z [INFO] [autopilot] event PedRemoved handle=8969
    2026-09-29T19:08:14.727Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2868->-2870
    2026-09-29T19:08:14.757Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.12, 654.025, 15.125) to=(-61.309, 682.75, 15.514)
    2026-09-29T19:08:14.758Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=11271 vehicle=0 killed=False hit=True at=(-61.309, 682.75, 15.514) dir=(0.165, 0.986, 0.013)
    2026-09-29T19:08:14.759Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2870->-2871
    2026-09-29T19:08:14.792Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.155, 710.526, 15.025) to=(-306.924, -48.267, 33.257)
    2026-09-29T19:08:14.793Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2871->-2875
    2026-09-29T19:08:14.828Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2875->-2876
    2026-09-29T19:08:14.859Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2876->-2878
    2026-09-29T19:08:14.896Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2878->-2880
    2026-09-29T19:08:14.928Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.118, 654.033, 15.14) to=(-52.414, 732.668, 13.734)
    2026-09-29T19:08:14.929Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.071, 683.813, 15.034) to=(-840.914, 647.429, 16.57)
    2026-09-29T19:08:14.929Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2880->-2882
    2026-09-29T19:08:14.961Z [INFO] [autopilot] event PedAppeared handle=10761
    2026-09-29T19:08:14.962Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2882->-2884
    2026-09-29T19:08:14.996Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2884->-2885
    2026-09-29T19:08:15.028Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2885->-2887
    2026-09-29T19:08:15.063Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2887->-2889
    2026-09-29T19:08:15.099Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.141, 654.037, 15.134) to=(56.869, 1445.19, 11.93)
    2026-09-29T19:08:15.100Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.43, 687.061, 14.965) to=(-61.283, 682.801, 15.529)
    2026-09-29T19:08:15.101Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.283, 682.801, 15.529) dir=(-0.989, -0.146, 0.019)
    2026-09-29T19:08:15.101Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2889->-2891
    2026-09-29T19:08:15.138Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2891->-2895
    2026-09-29T19:08:15.176Z [INFO] [autopilot] event PedAppeared handle=11016
    2026-09-29T19:08:15.177Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2895->-2897
    2026-09-29T19:08:15.214Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2897->-2899
    2026-09-29T19:08:15.252Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2899->-2902
    2026-09-29T19:08:15.291Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.404, 687.04, 14.976) to=(-824.969, 575.352, 35.019)
    2026-09-29T19:08:15.292Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2902->-2904
    2026-09-29T19:08:15.328Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.499, 656.591, 15.054) to=(-342.01, 1402.622, 21.319)
    2026-09-29T19:08:15.329Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2904->-2907
    2026-09-29T19:08:15.371Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2907->-2910
    2026-09-29T19:08:15.414Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2910->-2912
    2026-09-29T19:08:15.453Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.4, 687.047, 14.968) to=(-825.304, 576.898, 26.612)
    2026-09-29T19:08:15.453Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2912->-2914
    2026-09-29T19:08:15.495Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.508, 656.564, 15.072) to=(-61.258, 682.851, 15.547)
    2026-09-29T19:08:15.496Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.258, 682.851, 15.547) dir=(-0.348, 0.937, 0.017)
    2026-09-29T19:08:15.497Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2914->-2918
    2026-09-29T19:08:15.538Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2918->-2919
    2026-09-29T19:08:15.579Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2919->-2922
    2026-09-29T19:08:15.621Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.404, 687.051, 14.979) to=(-283.127, 655.879, 19.145)
    2026-09-29T19:08:15.622Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2922->-2924
    2026-09-29T19:08:15.666Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.502, 656.568, 15.044) to=(-90.824, 764.082, 16.952)
    2026-09-29T19:08:15.667Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2924->-2925
    2026-09-29T19:08:15.702Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-56.286, 690.704, 93.321) to=(-61.563, 683.309, 13.657)
    2026-09-29T19:08:15.703Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2925->-2927
    2026-09-29T19:08:15.743Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2927->-2931
    2026-09-29T19:08:15.779Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.394, 687.04, 14.975) to=(-334.906, 640.911, 22.256)
    2026-09-29T19:08:15.780Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2931->-2934
    2026-09-29T19:08:15.822Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.502, 656.574, 15.06) to=(-67.077, 697.319, 14.681)
    2026-09-29T19:08:15.823Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2934->-2936
    2026-09-29T19:08:15.865Z [INFO] [autopilot] event BulletFired shooter=10506 weapon=15 by_player=False from=(-56.275, 691.164, 93.282) to=(-63.437, 681.672, 13.705)
    2026-09-29T19:08:15.866Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.18, 710.519, 15.022) to=(-84.301, 609.47, 16.03)
    2026-09-29T19:08:15.867Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2936->-2940
    2026-09-29T19:08:15.904Z [INFO] [autopilot] event PedRemoved handle=10761
    2026-09-29T19:08:15.905Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2940->-2943
    2026-09-29T19:08:15.942Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2943->-2945
    2026-09-29T19:08:15.976Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.079, 683.819, 14.989) to=(-841.236, 656.572, 26.279)
    2026-09-29T19:08:15.977Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2945->-2947
    2026-09-29T19:08:16.014Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2947->-2949
    2026-09-29T19:08:16.050Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.153, 710.537, 15.036) to=(-62.637, 678.659, 15.786)
    2026-09-29T19:08:16.051Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2949->-2951
    2026-09-29T19:08:16.085Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2951->-2955
    2026-09-29T19:08:16.119Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2955->-2956
    2026-09-29T19:08:16.154Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.057, 683.797, 14.987) to=(-61.521, 683.043, 15.26)
    2026-09-29T19:08:16.155Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4D0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.521, 683.043, 15.26) dir=(-0.999, -0.037, 0.013)
    2026-09-29T19:08:16.156Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2956->-2957
    2026-09-29T19:08:16.194Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.154, 710.543, 15.029) to=(-287.536, -54.747, 29.507)
    2026-09-29T19:08:16.195Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2957->-2961
    2026-09-29T19:08:16.231Z [INFO] [autopilot] event PedAppeared handle=11530
    2026-09-29T19:08:16.232Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2961->-2963
    2026-09-29T19:08:16.267Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2963->-2964
    2026-09-29T19:08:16.305Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.054, 683.802, 14.986) to=(-839.838, 635.879, 43.414)
    2026-09-29T19:08:16.306Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2964->-2966
    2026-09-29T19:08:16.347Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.156, 710.545, 15.011) to=(-63.051, 678.67, 15.865)
    2026-09-29T19:08:16.348Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2966->-2968
    2026-09-29T19:08:16.394Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2968->-2971
    2026-09-29T19:08:16.428Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2971->-2972
    2026-09-29T19:08:16.464Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.058, 683.807, 14.998) to=(-840.449, 641.227, 25.379)
    2026-09-29T19:08:16.465Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2972->-2975
    2026-09-29T19:08:16.505Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.144, 710.56, 15.03) to=(-305.361, -49.029, 10.315)
    2026-09-29T19:08:16.506Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2975->-2979
    2026-09-29T19:08:16.551Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2979->-2983
    2026-09-29T19:08:16.582Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2983->-2986
    2026-09-29T19:08:16.621Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2986->-2989
    2026-09-29T19:08:16.656Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.058, 683.802, 14.99) to=(-383.172, 672.195, 25.417)
    2026-09-29T19:08:16.657Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.146, 710.576, 15.071) to=(-87.137, 609.261, 16.437)
    2026-09-29T19:08:16.658Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2989->-2992
    2026-09-29T19:08:16.689Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2992->-2994
    2026-09-29T19:08:16.780Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2994->-2998
    2026-09-29T19:08:16.839Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.405, 687.061, 14.97) to=(-278.89, 653.07, 20.811)
    2026-09-29T19:08:16.840Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -2998->-3003
    2026-09-29T19:08:16.879Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3003->-3004
    2026-09-29T19:08:16.911Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3004->-3007
    2026-09-29T19:08:16.941Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3007->-3009
    2026-09-29T19:08:16.972Z [INFO] [autopilot] event PedRemoved handle=12037
    2026-09-29T19:08:16.973Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3009->-3012
    2026-09-29T19:08:17.006Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.378, 687.04, 14.981) to=(-824.214, 571.166, 43.696)
    2026-09-29T19:08:17.007Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.497, 656.628, 15.031) to=(-61.36, 682.765, 15.43)
    2026-09-29T19:08:17.007Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.36, 682.765, 15.43) dir=(-0.353, 0.935, 0.014)
    2026-09-29T19:08:17.008Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3012->-3015
    2026-09-29T19:08:17.040Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3015->-3016
    2026-09-29T19:08:17.072Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3016->-3017
    2026-09-29T19:08:17.105Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3017->-3018
    2026-09-29T19:08:17.140Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.376, 687.045, 14.977) to=(-61.342, 682.876, 14.937)
    2026-09-29T19:08:17.140Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.342, 682.876, 14.937) dir=(-0.99, -0.142, -0.001)
    2026-09-29T19:08:17.141Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3018->-3020
    2026-09-29T19:08:17.174Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3020->-3022
    2026-09-29T19:08:17.224Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3022->-3024
    2026-09-29T19:08:17.262Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3024->-3025
    2026-09-29T19:08:17.304Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.381, 687.048, 14.991) to=(-823.879, 566.159, 13.166)
    2026-09-29T19:08:17.305Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3025->-3029
    2026-09-29T19:08:17.352Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3029->-3034
    2026-09-29T19:08:17.393Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3034->-3036
    2026-09-29T19:08:17.520Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3036->-3041
    2026-09-29T19:08:17.559Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3041->-3042
    2026-09-29T19:08:17.593Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3042->-3046
    2026-09-29T19:08:17.634Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3046->-3048
    2026-09-29T19:08:17.678Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3048->-3051
    2026-09-29T19:08:17.723Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-53.236, 697.473, 91.918) to=(-63.167, 683.217, 15.078)
    2026-09-29T19:08:17.724Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3051->-3056
    2026-09-29T19:08:17.725Z [INFO] [autopilot] event VehicleDamaged handle=7429 850->844 engine 1000->1000
    2026-09-29T19:08:17.767Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3056->-3060
    2026-09-29T19:08:17.806Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3060->-3061
    2026-09-29T19:08:17.844Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3061->-3063
    2026-09-29T19:08:17.883Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-52.898, 698.046, 91.815) to=(-61.11, 682.932, 13.507)
    2026-09-29T19:08:17.884Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3063->-3067
    2026-09-29T19:08:17.919Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3067->-3068
    2026-09-29T19:08:17.955Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3068->-3069
    2026-09-29T19:08:17.989Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.557, 654.279, 15.126) to=(84.144, 1440.587, 18.951)
    2026-09-29T19:08:17.990Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3069->-3071
    2026-09-29T19:08:18.029Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3071->-3072
    2026-09-29T19:08:18.065Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3072->-3073
    2026-09-29T19:08:18.102Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-52.554, 698.879, 91.712) to=(-61.756, 681.193, 13.668)
    2026-09-29T19:08:18.103Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.108, 683.828, 14.968) to=(-61.361, 682.982, 15.339)
    2026-09-29T19:08:18.103Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.167, 710.521, 15.042) to=(-87.584, 609.195, 17.847)
    2026-09-29T19:08:18.104Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.361, 682.982, 15.339) dir=(-0.999, -0.042, 0.018)
    2026-09-29T19:08:18.105Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3073->-3076
    2026-09-29T19:08:18.135Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3076->-3079
    2026-09-29T19:08:18.172Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.411, 654.315, 15.111) to=(-25.156, 910.877, 21.507)
    2026-09-29T19:08:18.173Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3079->-3080
    2026-09-29T19:08:18.208Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3080->-3083
    2026-09-29T19:08:18.243Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.144, 710.539, 15.028) to=(-88.124, 609.195, 17.084)
    2026-09-29T19:08:18.244Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3083->-3085
    2026-09-29T19:08:18.280Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-52.344, 699.607, 91.655) to=(-63.181, 683.078, 15.084)
    2026-09-29T19:08:18.281Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.084, 683.805, 14.979) to=(-839.666, 626.156, 28.49)
    2026-09-29T19:08:18.281Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.487, 656.624, 15.031) to=(-61.423, 682.624, 15.53)
    2026-09-29T19:08:18.282Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.423, 682.624, 15.53) dir=(-0.357, 0.934, 0.018)
    2026-09-29T19:08:18.283Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3085->-3088
    2026-09-29T19:08:18.283Z [INFO] [autopilot] event VehicleDamaged handle=7429 844->838 engine 1000->1000
    2026-09-29T19:08:18.327Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3088->-3093
    2026-09-29T19:08:18.362Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.143, 654.276, 15.085) to=(-24.655, 903.24, 22.158)
    2026-09-29T19:08:18.363Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3093->-3096
    2026-09-29T19:08:18.400Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.08, 683.811, 14.972) to=(-841.045, 647.686, 13.183)
    2026-09-29T19:08:18.401Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3096->-3098
    2026-09-29T19:08:18.436Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.499, 656.59, 15.048) to=(-332.204, 1406.189, 32.736)
    2026-09-29T19:08:18.436Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3098->-3102
    2026-09-29T19:08:18.472Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.008, 654.245, 15.038) to=(81.465, 1441.179, 37.583)
    2026-09-29T19:08:18.473Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3102->-3105
    2026-09-29T19:08:18.512Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3105->-3107
    2026-09-29T19:08:18.547Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3107->-3108
    2026-09-29T19:08:18.588Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.493, 656.589, 15.038) to=(-90.546, 763.813, 18.237)
    2026-09-29T19:08:18.589Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3108->-3110
    2026-09-29T19:08:18.630Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3110->-3114
    2026-09-29T19:08:18.672Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-65.495, 654.237, 15.046) to=(48.468, 1446.749, 11.769)
    2026-09-29T19:08:18.673Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.374, 687.075, 14.962) to=(-821.918, 556.738, 45.269)
    2026-09-29T19:08:18.674Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3114->-3116
    2026-09-29T19:08:18.705Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3116->-3118
    2026-09-29T19:08:18.736Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.495, 656.591, 15.051) to=(-329.134, 1407.546, 16.586)
    2026-09-29T19:08:18.737Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3118->-3120
    2026-09-29T19:08:18.769Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3120->-3121
    2026-09-29T19:08:18.801Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3121->-3124
    2026-09-29T19:08:18.832Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3124->-3127
    2026-09-29T19:08:18.864Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3127->-3129
    2026-09-29T19:08:18.895Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.503, 656.575, 15.044) to=(-93.42, 763.829, 17.973)
    2026-09-29T19:08:18.895Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3129->-3130
    2026-09-29T19:08:18.927Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3130->-3133
    2026-09-29T19:08:18.958Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3133->-3135
    2026-09-29T19:08:18.990Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3135->-3136
    2026-09-29T19:08:19.020Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3136->-3138
    2026-09-29T19:08:19.054Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3138->-3139
    2026-09-29T19:08:19.083Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.495, 656.578, 15.046) to=(-316.093, 1412.329, 27.673)
    2026-09-29T19:08:19.084Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3139->-3140
    2026-09-29T19:08:19.118Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.17, 710.519, 15.028) to=(-296.773, -51.783, 23.037)
    2026-09-29T19:08:19.119Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3140->-3143
    2026-09-29T19:08:19.151Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3143->-3145
    2026-09-29T19:08:19.185Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3145->-3146
    2026-09-29T19:08:19.217Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3146->-3149
    2026-09-29T19:08:19.249Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3149->-3150
    2026-09-29T19:08:19.282Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3150->-3151
    2026-09-29T19:08:19.316Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3151->-3155
    2026-09-29T19:08:19.353Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3155->-3157
    2026-09-29T19:08:19.387Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-62.754, 714.936, 15.228) to=(-58.715, 649.959, 14.58)
    2026-09-29T19:08:19.388Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.121, 683.841, 14.958) to=(-840.643, 656.311, 48.794)
    2026-09-29T19:08:19.388Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3157->-3159
    2026-09-29T19:08:19.443Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3159->-3162
    2026-09-29T19:08:19.484Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.94, 640.961, 15.125) to=(135.545, 1413.843, 28.557)
    2026-09-29T19:08:19.485Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3162->-3165
    2026-09-29T19:08:19.531Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3165->-3167
    2026-09-29T19:08:19.569Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.079, 683.801, 15.044) to=(-383.442, 675.431, 19.74)
    2026-09-29T19:08:19.570Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3167->-3168
    2026-09-29T19:08:19.609Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3168->-3170
    2026-09-29T19:08:19.647Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.062, 640.989, 15.201) to=(-51.882, 710.622, 14.925)
    2026-09-29T19:08:19.648Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3170->-3173
    2026-09-29T19:08:19.685Z [INFO] [autopilot] event PedRemoved handle=11530
    2026-09-29T19:08:19.686Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3173->-3177
    2026-09-29T19:08:19.727Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-41.02, 683.686, 15.087) to=(-382.659, 658.51, 17.044)
    2026-09-29T19:08:19.728Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3177->-3180
    2026-09-29T19:08:19.769Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3180->-3183
    2026-09-29T19:08:19.812Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.06, 640.998, 15.195) to=(-71.237, 647.658, 15.161)
    2026-09-29T19:08:19.813Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3183->-3186
    2026-09-29T19:08:19.852Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3186->-3188
    2026-09-29T19:08:19.891Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.947, 683.434, 15.098) to=(-840.633, 644.122, 23.991)
    2026-09-29T19:08:19.892Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3188->-3190
    2026-09-29T19:08:19.931Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3190->-3192
    2026-09-29T19:08:19.970Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.068, 641.01, 15.206) to=(-36.996, 763.256, 16.106)
    2026-09-29T19:08:19.971Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3192->-3194
    2026-09-29T19:08:20.008Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3194->-3197
    2026-09-29T19:08:20.045Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.88, 683.116, 15.093) to=(-61.345, 682.935, 15.188)
    2026-09-29T19:08:20.046Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.345, 682.935, 15.188) dir=(-1, -0.009, 0.005)
    2026-09-29T19:08:20.047Z [INFO] [autopilot] event PedAppeared handle=11785
    2026-09-29T19:08:20.047Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3197->-3198
    2026-09-29T19:08:20.085Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.392, 687.051, 15.001) to=(-61.225, 683.106, 15.435)
    2026-09-29T19:08:20.086Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.225, 683.106, 15.435) dir=(-0.991, -0.136, 0.015)
    2026-09-29T19:08:20.087Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3198->-3202
    2026-09-29T19:08:20.124Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.088, 641.021, 15.198) to=(136.846, 1413.647, 5.816)
    2026-09-29T19:08:20.125Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3202->-3204
    2026-09-29T19:08:20.163Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3204->-3207
    2026-09-29T19:08:20.197Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3207->-3210
    2026-09-29T19:08:20.232Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3210->-3211
    2026-09-29T19:08:20.272Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.362, 687.025, 14.994) to=(-391.701, 644.264, 17.891)
    2026-09-29T19:08:20.273Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3211->-3215
    2026-09-29T19:08:20.318Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3215->-3216
    2026-09-29T19:08:20.369Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-49.389, 707.927, 91.43) to=(-62.682, 682.218, 14.851)
    2026-09-29T19:08:20.369Z [INFO] [autopilot] event PedRemoved handle=11785
    2026-09-29T19:08:20.370Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3216->-3221
    2026-09-29T19:08:20.371Z [INFO] [autopilot] event VehicleDamaged handle=7429 838->832 engine 1000->1000
    2026-09-29T19:08:20.406Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3221->-3223
    2026-09-29T19:08:20.441Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.366, 687.034, 14.995) to=(-825.977, 582.443, 34.742)
    2026-09-29T19:08:20.442Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3223->-3225
    2026-09-29T19:08:20.478Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3225->-3228
    2026-09-29T19:08:20.514Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-62.308, 714.813, 15.212) to=(-58.449, 649.827, 15.69)
    2026-09-29T19:08:20.515Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-49.122, 708.447, 91.735) to=(-60.036, 683.075, 13.553)
    2026-09-29T19:08:20.516Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3228->-3229
    2026-09-29T19:08:20.587Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3229->-3232
    2026-09-29T19:08:20.623Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.191, 710.496, 15.013) to=(-294.619, -52.427, 30.614)
    2026-09-29T19:08:20.624Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.377, 687.038, 15.006) to=(-823.4, 564.238, 28.804)
    2026-09-29T19:08:20.625Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3232->-3235
    2026-09-29T19:08:20.658Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3235->-3239
    2026-09-29T19:08:20.697Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3239->-3242
    2026-09-29T19:08:20.735Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-48.876, 709.061, 92.334) to=(-60.648, 680.731, 13.542)
    2026-09-29T19:08:20.736Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3242->-3245
    2026-09-29T19:08:20.788Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.158, 710.51, 15.027) to=(-308.198, -47.942, 26.669)
    2026-09-29T19:08:20.789Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3245->-3247
    2026-09-29T19:08:20.822Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.38, 687.032, 14.979) to=(-823.613, 564.703, 18.311)
    2026-09-29T19:08:20.823Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3247->-3249
    2026-09-29T19:08:20.859Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3249->-3250
    2026-09-29T19:08:20.890Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3250->-3251
    2026-09-29T19:08:20.920Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-48.747, 709.605, 93.121) to=(-61.537, 683.453, 13.657)
    2026-09-29T19:08:20.921Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.47, 656.573, 15.032) to=(-92.938, 763.813, 18.209)
    2026-09-29T19:08:20.922Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3251->-3252
    2026-09-29T19:08:20.952Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.163, 710.514, 15.014) to=(-62.585, 678.659, 14.843)
    2026-09-29T19:08:20.953Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3252->-3253
    2026-09-29T19:08:20.984Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3253->-3255
    2026-09-29T19:08:21.016Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3255->-3256
    2026-09-29T19:08:21.049Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3256->-3258
    2026-09-29T19:08:21.080Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3258->-3259
    2026-09-29T19:08:21.111Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-48.706, 710.083, 94.015) to=(-63.321, 680.181, 13.711)
    2026-09-29T19:08:21.112Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.167, 710.51, 15.027) to=(-310.613, -47.287, 14.902)
    2026-09-29T19:08:21.113Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3259->-3262
    2026-09-29T19:08:21.144Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3262->-3263
    2026-09-29T19:08:21.177Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3263->-3265
    2026-09-29T19:08:21.208Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.517, 681.066, 15.065) to=(-840.605, 718.752, 28.751)
    2026-09-29T19:08:21.209Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3265->-3268
    2026-09-29T19:08:21.242Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3268->-3269
    2026-09-29T19:08:21.273Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.156, 710.51, 15.022) to=(-88.168, 609.195, 16.733)
    2026-09-29T19:08:21.274Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3269->-3270
    2026-09-29T19:08:21.307Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3270->-3273
    2026-09-29T19:08:21.340Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3273->-3275
    2026-09-29T19:08:21.371Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3275->-3277
    2026-09-29T19:08:21.406Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3277->-3279
    2026-09-29T19:08:21.439Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3279->-3282
    2026-09-29T19:08:21.471Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3282->-3285
    2026-09-29T19:08:21.505Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3285->-3287
    2026-09-29T19:08:21.540Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3287->-3291
    2026-09-29T19:08:21.575Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3291->-3293
    2026-09-29T19:08:21.612Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3293->-3295
    2026-09-29T19:08:21.649Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3295->-3298
    2026-09-29T19:08:21.685Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3298->-3301
    2026-09-29T19:08:21.724Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3301->-3302
    2026-09-29T19:08:21.757Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-62.322, 714.828, 15.204) to=(-60.2, 649.768, 16.364)
    2026-09-29T19:08:21.758Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3302->-3306
    2026-09-29T19:08:21.801Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.433, 687.055, 14.959) to=(-822.72, 560.486, 37.182)
    2026-09-29T19:08:21.802Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3306->-3309
    2026-09-29T19:08:21.846Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3309->-3312
    2026-09-29T19:08:21.888Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3312->-3315
    2026-09-29T19:08:21.924Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3315->-3317
    2026-09-29T19:08:21.963Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.406, 687.036, 14.974) to=(-67.225, 681.173, 15.026)
    2026-09-29T19:08:21.964Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3317->-3319
    2026-09-29T19:08:22.003Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3319->-3321
    2026-09-29T19:08:22.005Z [INFO] performance samples=772 frame_p50_ms=36 frame_p95_ms=55 frame_p99_ms=98 frames_over_33ms=571 frames_over_50ms=44 gunplay_avg_ms=1.772 gunplay_max_ms=12.520 phase_samples=772 phase_setup_avg_ms=1.291 phase_setup_max_ms=12.129 phase_camera_avg_ms=0.037 phase_camera_max_ms=3.547 phase_bullets_avg_ms=0.036 phase_bullets_max_ms=0.216 phase_weapon_avg_ms=0.023 phase_weapon_max_ms=0.206 phase_hud_avg_ms=0.385 phase_hud_max_ms=0.706
    2026-09-29T19:08:22.006Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.443/18.4/772@7 total=3430 module.gunplay=1.782/12.5/772@7 total=1376 tick.gunplay=1.781/12.5/772@7 total=1375 engine.world=1.437/4.5/772@7 total=1110 gp.freeaim=1.110/12.0/772@7 total=857 module.arsenal=0.719/13.5/764@7 total=550 tick.arsenal=0.719/13.5/764@7 total=549 ar.storage=0.485/13.5/764@7 total=371 ar.safehouse_flags=0.203/9.6/764@7 total=155 module.atmosphere=0.169/6.5/772@7 total=131 tick.atmosphere=0.168/6.5/772@7 total=130 module.combat=0.167/7.1/772@7 total=129 tick.combat=0.166/7.1/772@7 total=128 combat.sample=1.485/7.1/71@7 total=105 gp.index_pad=0.097/0.3/772@7 total=75 module.holsters=0.061/0.6/405@7 total=25 tick.holsters=0.061/0.6/405@7 total=25 cam.handle=0.030/3.5/772@7 total=23 gp.player=0.027/0.2/772@7 total=20 module.devtools=0.023/0.3/764@7 total=18 tick.devtools=0.023/0.3/764@7 total=17 gp.shoulder=0.019/0.2/772@7 total=15 gp.weapon_id=0.017/0.1/772@7 total=13 module.world=0.144/0.6/58@7 total=8 gp.cycle=0.009/0.1/772@7 total=7 gp.state=0.008/0.2/772@7 total=6 ho.show=0.011/0.1/405@7 total=4 module.probe=1.190/1.3/3@7 total=4 cam.aim_key=0.004/0.1/772@7 total=3 ar.vehicle=0.003/0.0/764@7 total=2 ar.reconcile=0.003/0.1/764@7 total=2 cam.find_active=0.003/0.0/772@7 total=2 ar.discover=0.003/0.7/764@7 total=2 ar.lvs=0.002/0.3/764@7 total=2 gp.spread=0.002/0.2/772@7 total=2 gp.shots=0.002/0.0/772@7 total=2 ho.carried=0.002/0.0/405@7 total=1 engine.scheduler=0.001/0.0/772@7 total=1 combat.dismember=0.001/0.0/772@7 total=0 module.autopilot=0.000/0.0/772@7 total=0 combat.blood=0.000/0.0/772@7 total=0 gp.feel=0.000/0.0/772@7 total=0 cam.fov=0.001/0.0/111@7 total=0 combat.pending=0.000/0.0/772@7 total=0 gp.recoil=0.000/0.0/772@7 total=0 module.weapon-probe=0.000/0.0/772@7 total=0
    2026-09-29T19:08:22.007Z [INFO] engine_thread_probe ticks=772 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=772 ticks_after_skipped_frames=0
    2026-09-29T19:08:22.009Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.19 shdn_us=59.8
    2026-09-29T19:08:22.013Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=9929 core=on peds=22 vehicles=15 modules=10/10 coroutines=0 resources=5 raycast=on episode=GTAIV frame_ms=36.85 p95_ms=47.02 pressure=0.18 private_mb=2222 working_set_mb=1470 address_free_mb=1225 largest_free_block_mb=1183 managed_mb=14 physical_load=93% core_us=75.5
    2026-09-29T19:08:22.048Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3321->-3325
    2026-09-29T19:08:22.054Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:08:22.090Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3325->-3327
    2026-09-29T19:08:22.093Z [INFO] density frame_ms=39.0 peds=0.68 cars=0.72
    2026-09-29T19:08:22.127Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.403, 687.041, 14.968) to=(-825.388, 576.649, 18.075)
    2026-09-29T19:08:22.128Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3327->-3330
    2026-09-29T19:08:22.167Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3330->-3334
    2026-09-29T19:08:22.204Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3334->-3336
    2026-09-29T19:08:22.240Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3336->-3337
    2026-09-29T19:08:22.275Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3337->-3340
    2026-09-29T19:08:22.334Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-48.613, 712.871, 100.265) to=(-60.308, 682.807, 13.539)
    2026-09-29T19:08:22.335Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.409, 687.046, 14.976) to=(-822.186, 555.24, 22.439)
    2026-09-29T19:08:22.336Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3340->-3342
    2026-09-29T19:08:22.336Z [INFO] [autopilot] event VehicleRemoved handle=3076
    2026-09-29T19:08:22.352Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.174, 679.142, 15.062) to=(-829.712, 812.124, 15.368)
    2026-09-29T19:08:22.353Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3342->-3345
    2026-09-29T19:08:22.391Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3345->-3348
    2026-09-29T19:08:22.427Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3348->-3351
    2026-09-29T19:08:22.487Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.191, 710.522, 15.018) to=(-62.741, 678.74, 16.323)
    2026-09-29T19:08:22.488Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.406, 687.034, 14.97) to=(-307.366, 643.242, 18.365)
    2026-09-29T19:08:22.489Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3351->-3354
    2026-09-29T19:08:22.504Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-48.495, 713.377, 101.097) to=(-59.917, 680.221, 13.575)
    2026-09-29T19:08:22.505Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3354->-3355
    2026-09-29T19:08:22.542Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.081, 678.828, 15.059) to=(-113.722, 692.561, 18.455)
    2026-09-29T19:08:22.543Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.495, 656.588, 15.068) to=(-90.65, 764.082, 15.383)
    2026-09-29T19:08:22.544Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3355->-3357
    2026-09-29T19:08:22.578Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3357->-3359
    2026-09-29T19:08:22.612Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.157, 710.536, 15.03) to=(-67.036, 667.778, 15.151)
    2026-09-29T19:08:22.613Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3359->-3361
    2026-09-29T19:08:22.650Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3361->-3365
    2026-09-29T19:08:22.683Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-40.02, 678.585, 15.045) to=(-829.538, 812.069, 30.148)
    2026-09-29T19:08:22.684Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.512, 656.551, 15.067) to=(-346.779, 1400.689, 30.415)
    2026-09-29T19:08:22.685Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3365->-3366
    2026-09-29T19:08:22.722Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3366->-3369
    2026-09-29T19:08:22.755Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.274, 641.134, 14.767) to=(142.791, 1411.929, 26.062)
    2026-09-29T19:08:22.756Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3369->-3373
    2026-09-29T19:08:22.791Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.154, 710.542, 15.024) to=(-307.093, -48.411, 16.134)
    2026-09-29T19:08:22.792Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3373->-3374
    2026-09-29T19:08:22.830Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-62.417, 713.636, 15.147) to=(-61.27, 648.541, 15.484)
    2026-09-29T19:08:22.831Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3374->-3378
    2026-09-29T19:08:22.873Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.981, 678.296, 15.062) to=(-61.357, 682.756, 15.078)
    2026-09-29T19:08:22.874Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.507, 656.559, 15.06) to=(-61.73, 682.713, 14.953)
    2026-09-29T19:08:22.874Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.357, 682.756, 15.078) dir=(-0.979, 0.204, 0.001)
    2026-09-29T19:08:22.875Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.73, 682.713, 14.953) dir=(-0.364, 0.931, -0.004)
    2026-09-29T19:08:22.876Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3378->-3381
    2026-09-29T19:08:22.921Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3381->-3385
    2026-09-29T19:08:22.959Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.278, 641.128, 14.779) to=(-73.162, 641.534, 14.778)
    2026-09-29T19:08:22.960Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.158, 710.537, 15.032) to=(-63.107, 678.646, 15.805)
    2026-09-29T19:08:22.960Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3385->-3387
    2026-09-29T19:08:22.994Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3387->-3390
    2026-09-29T19:08:23.030Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.932, 678.005, 15.061) to=(-819.219, 860.097, 44.938)
    2026-09-29T19:08:23.031Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.5, 656.571, 15.071) to=(-324.956, 1409.005, 25.085)
    2026-09-29T19:08:23.031Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3390->-3391
    2026-09-29T19:08:23.059Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3391->-3393
    2026-09-29T19:08:23.090Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.146, 710.534, 15.029) to=(-83.922, 609.47, 16.405)
    2026-09-29T19:08:23.091Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3393->-3394
    2026-09-29T19:08:23.122Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3394->-3397
    2026-09-29T19:08:23.156Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-62.488, 712.496, 15.12) to=(-61.45, 647.387, 14.668)
    2026-09-29T19:08:23.157Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3397->-3400
    2026-09-29T19:08:23.189Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.866, 677.771, 15.051) to=(-824.812, 836.209, 31.683)
    2026-09-29T19:08:23.190Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.511, 656.569, 15.048) to=(-328.825, 1407.554, 23.706)
    2026-09-29T19:08:23.191Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3400->-3401
    2026-09-29T19:08:23.222Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3401->-3404
    2026-09-29T19:08:23.256Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.15, 710.536, 15.037) to=(-61.793, 682.867, 15.013)
    2026-09-29T19:08:23.257Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.793, 682.867, 15.013) dir=(-0.329, -0.944, -0.001)
    2026-09-29T19:08:23.257Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3404->-3406
    2026-09-29T19:08:23.289Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3406->-3410
    2026-09-29T19:08:23.323Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3410->-3411
    2026-09-29T19:08:23.357Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3411->-3412
    2026-09-29T19:08:23.390Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3412->-3415
    2026-09-29T19:08:23.421Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3415->-3417
    2026-09-29T19:08:23.452Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3417->-3419
    2026-09-29T19:08:23.485Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3419->-3422
    2026-09-29T19:08:23.515Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3422->-3423
    2026-09-29T19:08:23.549Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3423->-3425
    2026-09-29T19:08:23.582Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3425->-3426
    2026-09-29T19:08:23.615Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-65.854, 658.752, 15.083) to=(-62.809, 678.244, 15.213)
    2026-09-29T19:08:23.616Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3426->-3427
    2026-09-29T19:08:23.652Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3427->-3429
    2026-09-29T19:08:23.689Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3429->-3430
    2026-09-29T19:08:23.723Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3430->-3433
    2026-09-29T19:08:23.758Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3433->-3435
    2026-09-29T19:08:23.794Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-65.997, 658.801, 15.128) to=(83.304, 1445.266, 25.868)
    2026-09-29T19:08:23.794Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3435->-3437
    2026-09-29T19:08:23.833Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3437->-3438
    2026-09-29T19:08:23.869Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3438->-3440
    2026-09-29T19:08:23.912Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3440->-3443
    2026-09-29T19:08:23.948Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.003, 658.8, 15.122) to=(-24.849, 870.785, 20.241)
    2026-09-29T19:08:23.949Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3443->-3444
    2026-09-29T19:08:23.992Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.412, 687.067, 14.965) to=(-826.106, 582.438, 28.499)
    2026-09-29T19:08:23.993Z [INFO] [autopilot] event PedAppeared handle=12038
    2026-09-29T19:08:23.994Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3444->-3447
    2026-09-29T19:08:24.033Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3447->-3450
    2026-09-29T19:08:24.071Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3450->-3451
    2026-09-29T19:08:24.110Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3451->-3453
    2026-09-29T19:08:24.145Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.381, 687.048, 14.981) to=(-823.251, 563.514, 31.808)
    2026-09-29T19:08:24.146Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3453->-3455
    2026-09-29T19:08:24.187Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3455->-3458
    2026-09-29T19:08:24.227Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3458->-3460
    2026-09-29T19:08:24.267Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3460->-3461
    2026-09-29T19:08:24.304Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.374, 687.048, 14.976) to=(-61.379, 682.789, 15.761)
    2026-09-29T19:08:24.305Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.379, 682.789, 15.761) dir=(-0.989, -0.145, 0.027)
    2026-09-29T19:08:24.306Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3461->-3464
    2026-09-29T19:08:24.344Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.497, 676.023, 15.032) to=(-802.285, 918.609, 32.759)
    2026-09-29T19:08:24.345Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3464->-3466
    2026-09-29T19:08:24.380Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.496, 656.629, 15.03) to=(-342.192, 1402.609, 22.935)
    2026-09-29T19:08:24.381Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3466->-3469
    2026-09-29T19:08:24.420Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3469->-3472
    2026-09-29T19:08:24.457Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-47.546, 717.846, 106.667) to=(-64.044, 679.35, 13.598)
    2026-09-29T19:08:24.458Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.381, 687.045, 14.987) to=(-822.91, 560.932, 28.952)
    2026-09-29T19:08:24.459Z [INFO] [autopilot] event PedRemoved handle=12038
    2026-09-29T19:08:24.460Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3472->-3473
    2026-09-29T19:08:24.495Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.731, 641.507, 15.126) to=(-72.633, 641.832, 15.153)
    2026-09-29T19:08:24.496Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3473->-3476
    2026-09-29T19:08:24.533Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.401, 675.728, 15.028) to=(-796.251, 938.114, 3.779)
    2026-09-29T19:08:24.533Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.177, 710.551, 15.004) to=(-61.377, 682.885, 15.723)
    2026-09-29T19:08:24.534Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.377, 682.885, 15.723) dir=(-0.315, -0.949, 0.025)
    2026-09-29T19:08:24.535Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3476->-3478
    2026-09-29T19:08:24.570Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.512, 656.598, 15.044) to=(-61.243, 683.045, 15.412)
    2026-09-29T19:08:24.571Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.243, 683.045, 15.412) dir=(-0.345, 0.938, 0.013)
    2026-09-29T19:08:24.572Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3478->-3480
    2026-09-29T19:08:24.609Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.372, 687.033, 14.981) to=(-825.178, 575.704, 20.587)
    2026-09-29T19:08:24.610Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3480->-3482
    2026-09-29T19:08:24.646Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-47.516, 718.168, 106.984) to=(-61.478, 683.107, 13.657)
    2026-09-29T19:08:24.647Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3482->-3485
    2026-09-29T19:08:24.684Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.481, 641.536, 15.117) to=(-62.83, 678.244, 15.363)
    2026-09-29T19:08:24.685Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.33, 675.504, 15.011) to=(-800.009, 924.221, 38.812)
    2026-09-29T19:08:24.686Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.146, 710.567, 15.026) to=(-61.313, 682.943, 14.828)
    2026-09-29T19:08:24.687Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4100 vehicle=0 killed=False hit=True at=(-61.313, 682.943, 14.828) dir=(-0.315, -0.949, -0.007)
    2026-09-29T19:08:24.688Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3485->-3487
    2026-09-29T19:08:24.725Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3487->-3489
    2026-09-29T19:08:24.761Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-51.506, 656.598, 15.034) to=(-338.532, 1403.943, 21.476)
    2026-09-29T19:08:24.762Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3489->-3491
    2026-09-29T19:08:24.802Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3491->-3494
    2026-09-29T19:08:24.838Z [INFO] [autopilot] event BulletFired shooter=9993 weapon=15 by_player=False from=(-47.482, 718.503, 107.261) to=(-60.632, 681.275, 13.527)
    2026-09-29T19:08:24.839Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.128, 641.894, 15.136) to=(-67.321, 659.668, 15.081)
    2026-09-29T19:08:24.840Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3494->-3495
    2026-09-29T19:08:24.876Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.277, 675.217, 15.022) to=(-794.832, 939.485, 31.717)
    2026-09-29T19:08:24.876Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3495->-3499
    2026-09-29T19:08:24.915Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3499->-3502
    2026-09-29T19:08:24.950Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3502->-3505
    2026-09-29T19:08:24.987Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-71.689, 642.33, 15.132) to=(-31.782, 797.052, 17.987)
    2026-09-29T19:08:24.988Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3505->-3507
    2026-09-29T19:08:25.024Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.224, 674.98, 15.071) to=(-61.619, 682.693, 15.14)
    2026-09-29T19:08:25.025Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.619, 682.693, 15.14) dir=(-0.945, 0.326, 0.003)
    2026-09-29T19:08:25.026Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3507->-3510
    2026-09-29T19:08:25.060Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3510->-3512
    2026-09-29T19:08:25.097Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3512->-3514
    2026-09-29T19:08:25.131Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3514->-3517
    2026-09-29T19:08:25.163Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-71.404, 642.987, 15.163) to=(128.193, 1418.258, 23.688)
    2026-09-29T19:08:25.164Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-39.169, 674.793, 15.05) to=(-787.013, 960.893, 22.878)
    2026-09-29T19:08:25.165Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3517->-3518
    2026-09-29T19:08:25.197Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3518->-3520
    2026-09-29T19:08:25.230Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3520->-3523
    2026-09-29T19:08:25.263Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3523->-3526
    2026-09-29T19:08:25.295Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3526->-3527
    2026-09-29T19:08:25.329Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3527->-3529
    2026-09-29T19:08:25.361Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3529->-3531
    2026-09-29T19:08:25.392Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3531->-3532
    2026-09-29T19:08:25.424Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3532->-3534
    2026-09-29T19:08:25.457Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3534->-3536
    2026-09-29T19:08:25.490Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3536->-3538
    2026-09-29T19:08:25.523Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.385, 687.073, 14.964) to=(-61.546, 682.993, 15.236)
    2026-09-29T19:08:25.523Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4D0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.546, 682.993, 15.236) dir=(-0.99, -0.139, 0.009)
    2026-09-29T19:08:25.524Z [INFO] [autopilot] event PedAppeared handle=12294
    2026-09-29T19:08:25.525Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3538->-3539
    2026-09-29T19:08:25.557Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3539->-3542
    2026-09-29T19:08:25.590Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3542->-3543
    2026-09-29T19:08:25.623Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3543->-3545
    2026-09-29T19:08:25.656Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.208, 657.792, 15.133) to=(-61.71, 682.634, 14.914)
    2026-09-29T19:08:25.657Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.71, 682.634, 14.914) dir=(-0.357, 0.934, -0.008)
    2026-09-29T19:08:25.657Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3545->-3548
    2026-09-29T19:08:25.690Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3548->-3550
    2026-09-29T19:08:25.723Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3550->-3551
    2026-09-29T19:08:25.756Z [INFO] [autopilot] event VehicleDamaged handle=3332 0->0 engine -3551->-3553
    2026-09-29T19:08:25.793Z [INFO] [autopilot] event PedRemoved handle=2055
    2026-09-29T19:08:25.794Z [INFO] [autopilot] event PedRemoved handle=2311
    2026-09-29T19:08:25.795Z [INFO] [autopilot] event PedRemoved handle=1798
    2026-09-29T19:08:25.795Z [INFO] [autopilot] event VehicleRemoved handle=3332
    2026-09-29T19:08:25.837Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.42, 658.032, 15.152) to=(-334.385, 1407.489, 4.697)
    2026-09-29T19:08:25.976Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.586, 658.313, 15.126) to=(-342.562, 1404.695, 28.641)
    2026-09-29T19:08:26.011Z [INFO] [autopilot] event PedRemoved handle=12294
    2026-09-29T19:08:26.156Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.768, 658.614, 15.141) to=(-88.032, 764.082, 17.038)
    2026-09-29T19:08:26.337Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.778, 673.172, 15.037) to=(-769.679, 999.835, 30.045)
    2026-09-29T19:08:26.337Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.015, 658.948, 15.126) to=(-86.777, 764.082, 17.404)
    2026-09-29T19:08:26.481Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.209, 659.144, 15.141) to=(-306.428, 1418.873, -2.358)
    2026-09-29T19:08:26.513Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.675, 672.899, 15.012) to=(-774.52, 987.58, 41.023)
    2026-09-29T19:08:26.675Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.582, 672.67, 14.983) to=(-67.077, 685.734, 14.8)
    2026-09-29T19:08:26.676Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.175, 710.52, 15.021) to=(-306.127, -48.552, 32.743)
    2026-09-29T19:08:26.838Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.527, 672.442, 14.985) to=(-776.573, 983.264, 22.283)
    2026-09-29T19:08:26.839Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.144, 710.534, 15.039) to=(-62.567, 678.593, 15.638)
    2026-09-29T19:08:26.839Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.379, 687.039, 15.109) to=(-825.126, 575.651, 25.133)
    2026-09-29T19:08:26.840Z [INFO] [autopilot] event PedAppeared handle=2056
    2026-09-29T19:08:26.999Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-38.459, 672.185, 14.991) to=(-769.044, 999.441, 28.035)
    2026-09-29T19:08:27.000Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.151, 710.54, 15.03) to=(-84.678, 609.47, 15.711)
    2026-09-29T19:08:27.033Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.369, 687.028, 15.02) to=(-61.515, 683.008, 15.255)
    2026-09-29T19:08:27.034Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4D0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.515, 683.008, 15.255) dir=(-0.991, -0.137, 0.008)
    2026-09-29T19:08:27.162Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.159, 710.539, 15.04) to=(-84.672, 609.47, 16.79)
    2026-09-29T19:08:27.163Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.371, 687.041, 14.968) to=(-822.717, 560.228, 34.626)
    2026-09-29T19:08:27.197Z [INFO] [autopilot] event PedRemoved handle=2056
    2026-09-29T19:08:27.323Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.146, 710.534, 15.041) to=(-308.013, -48.073, 18.332)
    2026-09-29T19:08:27.324Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.371, 687.043, 15.006) to=(-826.187, 583.531, 31.718)
    2026-09-29T19:08:27.499Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.158, 710.52, 15.03) to=(-84.263, 609.649, 15.342)
    2026-09-29T19:08:27.756Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.221, 662.018, 15.124) to=(-61.267, 682.827, 15.524)
    2026-09-29T19:08:27.757Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.267, 682.827, 15.524) dir=(-0.321, 0.947, 0.018)
    2026-09-29T19:08:27.910Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.326, 662.31, 15.156) to=(-319.297, 1417.795, 16.998)
    2026-09-29T19:08:28.073Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.4, 662.694, 15.137) to=(-318.742, 1418.489, 9.202)
    2026-09-29T19:08:28.241Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.451, 663.084, 15.149) to=(-306.833, 1422.828, 34.533)
    2026-09-29T19:08:28.276Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.831, 670.467, 14.95) to=(-738.776, 1057.288, 32.909)
    2026-09-29T19:08:28.387Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.53, 663.438, 15.145) to=(-88.348, 764.082, 17.189)
    2026-09-29T19:08:28.425Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.747, 670.227, 14.964) to=(-748.809, 1038.701, 4.329)
    2026-09-29T19:08:28.534Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.404, 687.06, 14.976) to=(-335.135, 641.375, 28.26)
    2026-09-29T19:08:28.604Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.624, 669.996, 14.966) to=(-61.881, 682.668, 14.991)
    2026-09-29T19:08:28.644Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.192, 710.497, 15.013) to=(-87.476, 609.195, 17.228)
    2026-09-29T19:08:28.712Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-65.836, 658.727, 15.062) to=(-62.644, 678.382, 15.255)
    2026-09-29T19:08:28.748Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.579, 669.814, 15.063) to=(-748.924, 1037.204, 32.965)
    2026-09-29T19:08:28.816Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.162, 710.512, 15.025) to=(-67.386, 667.798, 15.246)
    2026-09-29T19:08:28.882Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-65.998, 658.803, 15.129) to=(85.519, 1444.769, 33.85)
    2026-09-29T19:08:28.918Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-37.504, 669.561, 15.108) to=(-61.231, 683.157, 15.316)
    2026-09-29T19:08:28.919Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=4358 vehicle=0 killed=False hit=True at=(-61.231, 683.157, 15.316) dir=(-0.868, 0.497, 0.008)
    2026-09-29T19:08:28.954Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.166, 710.519, 15.017) to=(-84.431, 609.47, 17.387)
    2026-09-29T19:08:29.054Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.01, 658.806, 15.122) to=(-29.238, 849.151, 15.168)
    2026-09-29T19:08:29.122Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.172, 710.518, 15.027) to=(-294.134, -52.563, 30.801)
    2026-09-29T19:08:29.219Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.007, 658.813, 15.133) to=(75.747, 1446.851, 12.328)
    2026-09-29T19:08:29.283Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.26, 641.147, 14.769) to=(129.563, 1415.68, 28.006)
    2026-09-29T19:08:29.284Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.16, 710.52, 15.023) to=(-62.654, 678.728, 14.99)
    2026-09-29T19:08:29.384Z [INFO] [autopilot] event BulletFired shooter=11271 weapon=15 by_player=False from=(-66.022, 658.821, 15.128) to=(-27.179, 849.151, 16.641)
    2026-09-29T19:08:29.453Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.262, 641.143, 14.787) to=(-73.074, 641.743, 14.8)
    2026-09-29T19:08:29.454Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.164, 710.524, 15.028) to=(-84.849, 609.47, 18.068)
    2026-09-29T19:08:29.605Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-73.255, 641.156, 14.78) to=(141.29, 1412.611, 10.557)
    2026-09-29T19:08:29.756Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.302, 665.926, 15.126) to=(-361.21, 1405.488, 10.782)
    2026-09-29T19:08:29.848Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.433, 687.058, 14.96) to=(-823.115, 563.171, 40.928)
    2026-09-29T19:08:29.849Z [INFO] [autopilot] event PedAppeared handle=2312
    2026-09-29T19:08:29.939Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.217, 666.165, 15.133) to=(-61.339, 682.773, 15.214)
    2026-09-29T19:08:29.940Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.339, 682.773, 15.214) dir=(-0.394, 0.919, 0.004)
    2026-09-29T19:08:30.002Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.404, 687.037, 14.976) to=(-822.654, 558.113, 19.361)
    2026-09-29T19:08:30.101Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.869, 668.188, 15.066) to=(-719.238, 1086.957, 21.649)
    2026-09-29T19:08:30.102Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.122, 666.353, 15.135) to=(-61.345, 682.791, 15.098)
    2026-09-29T19:08:30.103Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.345, 682.791, 15.098) dir=(-0.402, 0.916, -0.002)
    2026-09-29T19:08:30.191Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.403, 687.042, 14.967) to=(-175.714, 663.612, 15.651)
    2026-09-29T19:08:30.276Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.806, 667.999, 15.072) to=(-61.459, 683.135, 15.408)
    2026-09-29T19:08:30.277Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-54.003, 666.554, 15.148) to=(-61.353, 682.863, 14.8)
    2026-09-29T19:08:30.278Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.353, 682.863, 14.8) dir=(-0.411, 0.912, -0.019)
    2026-09-29T19:08:30.362Z [INFO] [autopilot] event PedRemoved handle=2312
    2026-09-29T19:08:30.437Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.406, 687.049, 14.978) to=(-307.304, 643.32, 17.58)
    2026-09-29T19:08:30.495Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.758, 667.836, 15.086) to=(-721.975, 1081.467, 39.084)
    2026-09-29T19:08:30.496Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-53.878, 666.724, 15.133) to=(-61.294, 682.857, 15.608)
    2026-09-29T19:08:30.497Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3588 vehicle=0 killed=False hit=True at=(-61.294, 682.857, 15.608) dir=(-0.418, 0.908, 0.027)
    2026-09-29T19:08:30.498Z [INFO] [autopilot] event PedAppeared handle=2567
    2026-09-29T19:08:30.657Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.737, 667.708, 15.098) to=(-724.404, 1077.489, 33.159)
    2026-09-29T19:08:30.658Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.18, 710.52, 15.019) to=(-62.629, 678.698, 15.027)
    2026-09-29T19:08:30.792Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.638, 667.532, 15.078) to=(-711.334, 1098.471, 29.163)
    2026-09-29T19:08:30.793Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.15, 710.536, 15.038) to=(-84.777, 609.649, 15.312)
    2026-09-29T19:08:30.835Z [INFO] [autopilot] event PedRemoved handle=2567
    2026-09-29T19:08:30.951Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.156, 710.541, 15.028) to=(-297.983, -51.317, 25.994)
    2026-09-29T19:08:31.151Z [INFO] [autopilot] event BulletFired shooter=4100 weapon=15 by_player=False from=(-52.16, 710.538, 15.04) to=(-305.374, -48.957, 18.008)
    2026-09-29T19:08:31.221Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-72.274, 641.682, 15.147) to=(-67.549, 660.785, 14.77)
    2026-09-29T19:08:31.300Z [INFO] command source=file:cmd_20260929190831112.cmd line="hud on" reply="hud on"
    2026-09-29T19:08:31.334Z [INFO] [autopilot] event BulletFired shooter=3847 weapon=15 by_player=False from=(-32.415, 687.063, 14.966) to=(-61.256, 682.965, 15.468)
    2026-09-29T19:08:31.335Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3847 vehicle=0 killed=False hit=True at=(-61.256, 682.965, 15.468) dir=(-0.99, -0.141, 0.017)
    2026-09-29T19:08:31.336Z [INFO] [autopilot] event PedAppeared handle=2823
    2026-09-29T19:08:31.408Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-71.764, 642.212, 15.106) to=(-68.314, 655.061, 14.782)
    2026-09-29T19:08:31.548Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-71.449, 642.67, 15.145) to=(122.607, 1419.446, 4.877)
    2026-09-29T19:08:31.553Z [INFO] command source=file:cmd_20260929190831498.cmd line="clear" reply="cleared 1"
    2026-09-29T19:08:31.587Z [INFO] [autopilot] event VehicleRemoved handle=7429
    2026-09-29T19:08:31.618Z [INFO] [autopilot] event PedRemoved handle=2823
    2026-09-29T19:08:31.649Z [INFO] [autopilot] event BulletFired shooter=10247 weapon=7 by_player=False from=(-67.956, 697.506, 15.071) to=(-61.474, 682.924, 15.602)
    2026-09-29T19:08:31.650Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.733, 667.997, 15.125) to=(-67.277, 693.501, 15.061)
    2026-09-29T19:08:31.650Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=7 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=10247 vehicle=0 killed=False hit=True at=(-61.474, 682.924, 15.602) dir=(0.406, -0.913, 0.033)
    2026-09-29T19:08:31.711Z [INFO] [autopilot] event BulletFired shooter=5127 weapon=15 by_player=False from=(-71.409, 643.419, 15.15) to=(-62.824, 678.237, 15.122)
    2026-09-29T19:08:31.796Z [INFO] [autopilot] event BulletFired shooter=3588 weapon=15 by_player=False from=(-52.509, 668.064, 15.142) to=(-473.934, 1348.865, -2.812)
    2026-09-29T19:08:31.847Z [INFO] [autopilot] event BulletFired shooter=4358 weapon=15 by_player=False from=(-36.26, 666.8, 14.951) to=(-706.845, 1103.762, 39.755)

# Scenario lod-review

- Result: PASS
- Steps: 28, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 5000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- spawnprop lf_lod_post 4 0 => spawning prop lf_lod_post
- expect autopilot_prop handle=\d+ model=lf_lod_post: OK 2026-09-29T05:42:59.528Z [INFO] [autopilot] autopilot_prop handle=12806 model=lf_lod_post at=(-64.419, 678.834, 13.572)
- wait 1500 ms
- hud off => hud off
- cam prop 0 6 1.5 => camera at 0 deg, 6 m
- wait 2500 ms
- shot lod_6m -> lod_6m.jpg
- cam prop 0 18 2.5 => camera at 0 deg, 18 m
- wait 2500 ms
- shot lod_18m -> lod_18m.jpg
- cam prop 0 38 4 => camera at 0 deg, 38 m
- wait 2500 ms
- shot lod_38m -> lod_38m.jpg
- cam prop 0 75 6 => camera at 0 deg, 75 m
- wait 2500 ms
- shot lod_75m -> lod_75m.jpg
- cam prop 0 130 9 => camera at 0 deg, 130 m
- wait 2500 ms
- shot lod_130m -> lod_130m.jpg
- cam off => camera off
- hud on => hud on
- clear => cleared 1

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:42:51.744Z [INFO] command source=file:cmd_20260929054251605.cmd line="god on" reply="invincible True"
    2026-09-29T05:42:52.248Z [INFO] command source=file:cmd_20260929054252020.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:42:53.182Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:42:53.183Z [INFO] command source=file:cmd_20260929054252409.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:42:53.207Z [INFO] [autopilot] event PedRemoved handle=9736
    2026-09-29T05:42:53.208Z [INFO] [autopilot] event PedRemoved handle=9225
    2026-09-29T05:42:53.209Z [INFO] [autopilot] event PedRemoved handle=8713
    2026-09-29T05:42:53.210Z [INFO] [autopilot] event PedRemoved handle=7942
    2026-09-29T05:42:53.211Z [INFO] [autopilot] event PedRemoved handle=9478
    2026-09-29T05:42:53.211Z [INFO] [autopilot] event PedRemoved handle=8456
    2026-09-29T05:42:53.212Z [INFO] [autopilot] event PedRemoved handle=8198
    2026-09-29T05:42:53.213Z [INFO] [autopilot] event PedRemoved handle=6150
    2026-09-29T05:42:53.213Z [INFO] [autopilot] event PedRemoved handle=8967
    2026-09-29T05:42:53.214Z [INFO] [autopilot] event PedRemoved handle=5895
    2026-09-29T05:42:53.215Z [INFO] [autopilot] event PedRemoved handle=5642
    2026-09-29T05:42:53.216Z [INFO] [autopilot] event PedRemoved handle=6917
    2026-09-29T05:42:53.216Z [INFO] [autopilot] event PedRemoved handle=5381
    2026-09-29T05:42:53.217Z [INFO] [autopilot] event PedRemoved handle=4357
    2026-09-29T05:42:53.218Z [INFO] [autopilot] event PedRemoved handle=7173
    2026-09-29T05:42:53.218Z [INFO] [autopilot] event PedRemoved handle=5125
    2026-09-29T05:42:53.219Z [INFO] [autopilot] event PedRemoved handle=4101
    2026-09-29T05:42:53.220Z [INFO] [autopilot] event PedRemoved handle=7428
    2026-09-29T05:42:53.221Z [INFO] [autopilot] event PedRemoved handle=4868
    2026-09-29T05:42:53.221Z [INFO] [autopilot] event PedRemoved handle=3844
    2026-09-29T05:42:53.222Z [INFO] [autopilot] event PedRemoved handle=3331
    2026-09-29T05:42:53.223Z [INFO] [autopilot] event PedRemoved handle=4611
    2026-09-29T05:42:53.223Z [INFO] [autopilot] event PedRemoved handle=3075
    2026-09-29T05:42:53.224Z [INFO] [autopilot] event PedRemoved handle=2825
    2026-09-29T05:42:53.225Z [INFO] [autopilot] event PedRemoved handle=7684
    2026-09-29T05:42:53.226Z [INFO] [autopilot] event PedRemoved handle=3588
    2026-09-29T05:42:53.226Z [INFO] [autopilot] event PedRemoved handle=2564
    2026-09-29T05:42:53.227Z [INFO] [autopilot] event VehicleAppeared handle=7940
    2026-09-29T05:42:53.228Z [INFO] [autopilot] event VehicleAppeared handle=8197
    2026-09-29T05:42:53.228Z [INFO] [autopilot] event VehicleAppeared handle=8452
    2026-09-29T05:42:53.229Z [INFO] [autopilot] event VehicleAppeared handle=8708
    2026-09-29T05:42:53.230Z [INFO] [autopilot] event VehicleAppeared handle=8965
    2026-09-29T05:42:53.231Z [INFO] [autopilot] event VehicleAppeared handle=9221
    2026-09-29T05:42:53.231Z [INFO] [autopilot] event VehicleAppeared handle=9733
    2026-09-29T05:42:53.232Z [INFO] [autopilot] event VehicleAppeared handle=10246
    2026-09-29T05:42:53.233Z [INFO] [autopilot] event VehicleRemoved handle=1286
    2026-09-29T05:42:53.233Z [INFO] [autopilot] event VehicleRemoved handle=6917
    2026-09-29T05:42:53.234Z [INFO] [autopilot] event VehicleRemoved handle=5125
    2026-09-29T05:42:53.235Z [INFO] [autopilot] event VehicleRemoved handle=2053
    2026-09-29T05:42:53.235Z [INFO] [autopilot] event VehicleRemoved handle=1797
    2026-09-29T05:42:53.236Z [INFO] [autopilot] event VehicleRemoved handle=1029
    2026-09-29T05:42:53.237Z [INFO] [autopilot] event VehicleRemoved handle=7684
    2026-09-29T05:42:53.238Z [INFO] [autopilot] event VehicleRemoved handle=2564
    2026-09-29T05:42:53.238Z [INFO] [autopilot] event VehicleRemoved handle=772
    2026-09-29T05:42:53.239Z [INFO] [autopilot] event VehicleRemoved handle=7171
    2026-09-29T05:42:53.240Z [INFO] [autopilot] event VehicleRemoved handle=6403
    2026-09-29T05:42:53.241Z [INFO] [autopilot] event VehicleRemoved handle=6147
    2026-09-29T05:42:53.241Z [INFO] [autopilot] event VehicleRemoved handle=5635
    2026-09-29T05:42:53.242Z [INFO] [autopilot] event VehicleRemoved handle=5379
    2026-09-29T05:42:53.243Z [INFO] [autopilot] event VehicleRemoved handle=4867
    2026-09-29T05:42:53.244Z [INFO] [autopilot] event VehicleRemoved handle=4611
    2026-09-29T05:42:53.244Z [INFO] [autopilot] event VehicleRemoved handle=4355
    2026-09-29T05:42:53.245Z [INFO] [autopilot] event VehicleRemoved handle=4099
    2026-09-29T05:42:53.246Z [INFO] [autopilot] event VehicleRemoved handle=3843
    2026-09-29T05:42:53.246Z [INFO] [autopilot] event VehicleRemoved handle=3587
    2026-09-29T05:42:53.247Z [INFO] [autopilot] event VehicleRemoved handle=3075
    2026-09-29T05:42:53.248Z [INFO] [autopilot] event VehicleRemoved handle=2819
    2026-09-29T05:42:53.248Z [INFO] [autopilot] event VehicleRemoved handle=2307
    2026-09-29T05:42:53.326Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=33029 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T05:42:53.458Z [INFO] [autopilot] event VehicleAppeared handle=10757
    2026-09-29T05:42:54.030Z [INFO] [autopilot] event PedAppeared handle=5896
    2026-09-29T05:42:54.030Z [INFO] [autopilot] event PedAppeared handle=6151
    2026-09-29T05:42:54.086Z [INFO] [autopilot] event PedAppeared handle=6410
    2026-09-29T05:42:54.091Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:42:54.112Z [INFO] [autopilot] event PedAppeared handle=6665
    2026-09-29T05:42:54.113Z [INFO] [autopilot] event VehicleRemoved handle=9221
    2026-09-29T05:42:54.113Z [INFO] [autopilot] event VehicleRemoved handle=8965
    2026-09-29T05:42:54.152Z [INFO] [autopilot] event PedAppeared handle=7685
    2026-09-29T05:42:54.222Z [INFO] [autopilot] event PedAppeared handle=7943
    2026-09-29T05:42:54.425Z [INFO] [autopilot] event PedAppeared handle=8199
    2026-09-29T05:42:54.490Z [INFO] [autopilot] event PedAppeared handle=8457
    2026-09-29T05:42:54.529Z [INFO] [autopilot] event PedAppeared handle=8714
    2026-09-29T05:42:54.575Z [INFO] [autopilot] event PedAppeared handle=9737
    2026-09-29T05:42:54.576Z [INFO] [autopilot] event PedAppeared handle=9989
    2026-09-29T05:42:54.630Z [INFO] [autopilot] event PedAppeared handle=10245
    2026-09-29T05:42:56.793Z [INFO] [autopilot] event PedRemoved handle=10245
    2026-09-29T05:42:57.034Z [INFO] [autopilot] event PedRemoved handle=9989
    2026-09-29T05:42:57.065Z [INFO] [autopilot] event PedRemoved handle=8457
    2026-09-29T05:42:57.121Z [INFO] [autopilot] event PedRemoved handle=8199
    2026-09-29T05:42:57.231Z [INFO] [autopilot] event PedAppeared handle=8458
    2026-09-29T05:42:57.232Z [INFO] [autopilot] event PedRemoved handle=8714
    2026-09-29T05:42:57.461Z [INFO] [autopilot] event PedAppeared handle=8715
    2026-09-29T05:42:57.462Z [INFO] [autopilot] event PedRemoved handle=9737
    2026-09-29T05:42:57.546Z [INFO] [autopilot] event PedAppeared handle=9738
    2026-09-29T05:42:57.692Z [INFO] [autopilot] event PedAppeared handle=9990
    2026-09-29T05:42:57.731Z [INFO] [autopilot] event PedAppeared handle=10246
    2026-09-29T05:42:57.949Z [INFO] [autopilot] event PedAppeared handle=10501
    2026-09-29T05:42:58.444Z [INFO] command source=file:cmd_20260929054258340.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:42:58.959Z [INFO] command source=file:cmd_20260929054258731.cmd line="weather 1" reply="weather 1"
    2026-09-29T05:42:59.241Z [INFO] [autopilot] event PedAppeared handle=10758
    2026-09-29T05:42:59.250Z [INFO] command source=file:cmd_20260929054259119.cmd line="spawnprop lf_lod_post 4 0" reply="spawning prop lf_lod_post"
    2026-09-29T05:42:59.458Z [INFO] weather_block from=SUNNY to=CLOUDY hours=5 at=13h
    2026-09-29T05:42:59.528Z [INFO] [autopilot] autopilot_prop handle=12806 model=lf_lod_post at=(-64.419, 678.834, 13.572)
    2026-09-29T05:42:59.704Z [INFO] [autopilot] event PedRemoved handle=8458
    2026-09-29T05:43:00.152Z [INFO] [autopilot] event PedDamaged handle=8968 200->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=291.6 health_lost=200.0 armour_lost=100.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.153Z [INFO] [autopilot] event PedDamaged handle=9226 200->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=291.6 health_lost=200.0 armour_lost=100.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.153Z [INFO] [autopilot] event PedDamaged handle=9479 200->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=291.6 health_lost=200.0 armour_lost=100.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.157Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:43:00.179Z [INFO] [autopilot] event PedAppeared handle=11012
    2026-09-29T05:43:00.212Z [INFO] [autopilot] event PedDamaged handle=8968 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=90.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=True hit=False
    2026-09-29T05:43:00.213Z [INFO] [autopilot] event PedDied handle=8968 bone=0xFFFFFFFF by_player=False exact=True type=Vehicle killer=0 weapon=55
    2026-09-29T05:43:00.214Z [INFO] [autopilot] event PedDamaged handle=9226 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=90.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=True hit=False
    2026-09-29T05:43:00.214Z [INFO] [autopilot] event PedDied handle=9226 bone=0xFFFFFFFF by_player=False exact=True type=Vehicle killer=0 weapon=55
    2026-09-29T05:43:00.215Z [INFO] [autopilot] event PedDamaged handle=9479 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=90.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=True hit=False
    2026-09-29T05:43:00.216Z [INFO] [autopilot] event PedDied handle=9479 bone=0xFFFFFFFF by_player=False exact=True type=Vehicle killer=0 weapon=55
    2026-09-29T05:43:00.233Z [INFO] [autopilot] event PedDamaged handle=8968 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=125.6 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.234Z [INFO] [autopilot] event PedDamaged handle=9226 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=125.6 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.235Z [INFO] [autopilot] event PedDamaged handle=9479 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=125.6 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.277Z [INFO] [autopilot] event PedDamaged handle=8968 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=85.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.277Z [INFO] [autopilot] event PedDamaged handle=9226 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=85.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.278Z [INFO] [autopilot] event PedDamaged handle=9479 0->0 bone=0xFFFFFFFF by_player=False weapon=55 exact=True type=Vehicle amount=85.7 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=11780 killed=False hit=False
    2026-09-29T05:43:00.481Z [INFO] [autopilot] event PedRemoved handle=9990
    2026-09-29T05:43:00.604Z [INFO] [autopilot] event PedRemoved handle=10246
    2026-09-29T05:43:00.656Z [INFO] [autopilot] event PedRemoved handle=8715
    2026-09-29T05:43:00.750Z [INFO] [autopilot] event PedRemoved handle=9738
    2026-09-29T05:43:01.059Z [INFO] [autopilot] event PedAppeared handle=9739
    2026-09-29T05:43:01.060Z [INFO] [autopilot] event PedAppeared handle=9991
    2026-09-29T05:43:01.118Z [INFO] [autopilot] event PedAppeared handle=10247
    2026-09-29T05:43:01.169Z [INFO] [autopilot] event PedAppeared handle=11268
    2026-09-29T05:43:01.341Z [INFO] [autopilot] event PedRemoved handle=10758
    2026-09-29T05:43:01.813Z [INFO] command source=file:cmd_20260929054301586.cmd line="hud off" reply="hud off"
    2026-09-29T05:43:01.850Z [INFO] [autopilot] event PedAppeared handle=11524
    2026-09-29T05:43:02.049Z [INFO] command source=file:cmd_20260929054301982.cmd line="cam prop 0 6 1.5" reply="camera at 0 deg, 6 m"
    2026-09-29T05:43:02.081Z [INFO] [autopilot] event PedAppeared handle=10759
    2026-09-29T05:43:02.082Z [INFO] [autopilot] event PedRemoved handle=10501
    2026-09-29T05:43:02.418Z [INFO] [autopilot] event PedAppeared handle=11779
    2026-09-29T05:43:03.260Z [INFO] [autopilot] event PedRemoved handle=11012
    2026-09-29T05:43:03.433Z [INFO] [autopilot] event PedRemoved handle=9739
    2026-09-29T05:43:03.571Z [INFO] [autopilot] event PedRemoved handle=11268
    2026-09-29T05:43:03.712Z [INFO] [autopilot] event PedAppeared handle=5127
    2026-09-29T05:43:03.826Z [INFO] [autopilot] event PedAppeared handle=8200
    2026-09-29T05:43:03.929Z [INFO] [autopilot] event PedRemoved handle=10247
    2026-09-29T05:43:04.060Z [INFO] [autopilot] event PedAppeared handle=8459
    2026-09-29T05:43:04.384Z [INFO] [autopilot] event PedAppeared handle=8716
    2026-09-29T05:43:04.662Z [INFO] [autopilot] event VehicleAppeared handle=11524
    2026-09-29T05:43:04.788Z [INFO] [autopilot] event PedAppeared handle=8969
    2026-09-29T05:43:05.737Z [INFO] [autopilot] event PedAppeared handle=9227
    2026-09-29T05:43:06.152Z [INFO] [autopilot] event PedAppeared handle=6918
    2026-09-29T05:43:06.153Z [INFO] [autopilot] event PedAppeared handle=7174
    2026-09-29T05:43:06.154Z [INFO] [autopilot] event PedAppeared handle=7429
    2026-09-29T05:43:06.390Z [INFO] [autopilot] event PedRemoved handle=5127
    2026-09-29T05:43:06.580Z [INFO] [autopilot] event PedRemoved handle=8200
    2026-09-29T05:43:06.775Z [INFO] camera_already_gone handle=5378 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:06.776Z [INFO] command source=file:cmd_20260929054306676.cmd line="cam prop 0 18 2.5" reply="camera at 0 deg, 18 m"
    2026-09-29T05:43:06.819Z [INFO] [autopilot] event PedRemoved handle=8969
    2026-09-29T05:43:06.820Z [INFO] [autopilot] event PedRemoved handle=8716
    2026-09-29T05:43:06.820Z [INFO] [autopilot] event PedRemoved handle=11779
    2026-09-29T05:43:06.821Z [INFO] [autopilot] event PedRemoved handle=9991
    2026-09-29T05:43:06.822Z [INFO] [autopilot] event PedRemoved handle=7943
    2026-09-29T05:43:06.822Z [INFO] [autopilot] event PedRemoved handle=7685
    2026-09-29T05:43:06.823Z [INFO] [autopilot] event PedRemoved handle=6151
    2026-09-29T05:43:06.930Z [INFO] [autopilot] event PedRemoved handle=8459
    2026-09-29T05:43:08.121Z [INFO] [autopilot] event PedAppeared handle=6152
    2026-09-29T05:43:10.162Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:43:10.471Z [INFO] [autopilot] event PedAppeared handle=7686
    2026-09-29T05:43:10.537Z [INFO] [autopilot] event PedAppeared handle=7944
    2026-09-29T05:43:10.800Z [INFO] [autopilot] event PedRemoved handle=7429
    2026-09-29T05:43:10.801Z [INFO] [autopilot] event PedRemoved handle=7174
    2026-09-29T05:43:10.801Z [INFO] [autopilot] event PedRemoved handle=6918
    2026-09-29T05:43:11.034Z [INFO] [autopilot] event PedRemoved handle=6152
    2026-09-29T05:43:11.315Z [INFO] [autopilot] event VehicleDamaged handle=11524 1000->999 engine 1000->1000
    2026-09-29T05:43:11.321Z [INFO] camera_already_gone handle=5634 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:11.322Z [INFO] command source=file:cmd_20260929054311289.cmd line="cam prop 0 38 4" reply="camera at 0 deg, 38 m"
    2026-09-29T05:43:11.355Z [INFO] [autopilot] event PedRemoved handle=9227
    2026-09-29T05:43:11.524Z [INFO] [autopilot] event VehicleDamaged handle=11524 999->952 engine 1000->930
    2026-09-29T05:43:12.747Z [INFO] [autopilot] event PedRemoved handle=7686
    2026-09-29T05:43:12.819Z [INFO] [autopilot] event PedRemoved handle=7944
    2026-09-29T05:43:13.323Z [INFO] [autopilot] event PedAppeared handle=3077
    2026-09-29T05:43:13.726Z [INFO] [autopilot] event PedAppeared handle=6918
    2026-09-29T05:43:13.727Z [INFO] [autopilot] event PedAppeared handle=7174
    2026-09-29T05:43:13.727Z [INFO] [autopilot] event PedAppeared handle=7429
    2026-09-29T05:43:15.869Z [INFO] [autopilot] event VehicleAppeared handle=11013
    2026-09-29T05:43:16.043Z [INFO] camera_already_gone handle=5890 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:16.044Z [INFO] command source=file:cmd_20260929054315903.cmd line="cam prop 0 75 6" reply="camera at 0 deg, 75 m"
    2026-09-29T05:43:16.070Z [INFO] [autopilot] event PedRemoved handle=3077
    2026-09-29T05:43:16.071Z [INFO] [autopilot] event PedRemoved handle=10759
    2026-09-29T05:43:17.420Z [INFO] [autopilot] event PedAppeared handle=5382
    2026-09-29T05:43:17.421Z [INFO] [autopilot] event PedAppeared handle=5643
    2026-09-29T05:43:18.514Z [INFO] density frame_ms=30.3 peds=0.88 cars=0.89
    2026-09-29T05:43:19.005Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=6500 core=on peds=10 vehicles=9 modules=10/10 coroutines=0 resources=6 raycast=on episode=GTAIV frame_ms=30.43 p95_ms=43.59 pressure=0.01 private_mb=2227 working_set_mb=1997 address_free_mb=1203 largest_free_block_mb=1162 managed_mb=14 physical_load=93% core_us=54.0
    2026-09-29T05:43:19.176Z [INFO] performance samples=1014 frame_p50_ms=26 frame_p95_ms=46 frame_p99_ms=87 frames_over_33ms=285 frames_over_50ms=35 gunplay_avg_ms=1.250 gunplay_max_ms=4.231 phase_samples=1014 phase_setup_avg_ms=0.816 phase_setup_max_ms=3.824 phase_camera_avg_ms=0.023 phase_camera_max_ms=0.916 phase_bullets_avg_ms=0.002 phase_bullets_max_ms=0.052 phase_weapon_avg_ms=0.019 phase_weapon_max_ms=0.768 phase_hud_avg_ms=0.390 phase_hud_max_ms=3.539
    2026-09-29T05:43:19.177Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=3.033/690.3/1014@7 total=3076 module.gunplay=1.256/5.2/1014@7 total=1274 tick.gunplay=1.255/5.1/1014@7 total=1272 gp.freeaim=0.646/3.6/1014@7 total=655 module.arsenal=0.642/4.1/753@7 total=484 tick.arsenal=0.642/4.1/753@7 total=483 ar.storage=0.438/3.5/753@7 total=330 engine.world=0.230/42.3/1014@7 total=233 module.atmosphere=0.190/11.0/1014@7 total=193 tick.atmosphere=0.189/11.0/1014@7 total=191 ar.safehouse_flags=0.174/2.5/753@7 total=131 gp.index_pad=0.107/0.6/1014@7 total=108 module.combat=0.025/0.6/1014@7 total=26 tick.combat=0.025/0.6/1014@7 total=25 module.devtools=0.029/3.8/753@7 total=22 tick.devtools=0.028/3.8/753@7 total=21 module.holsters=0.051/0.6/408@7 total=21 tick.holsters=0.050/0.6/408@7 total=21 cam.handle=0.017/0.9/1014@7 total=17 gp.shoulder=0.016/0.8/1014@7 total=16 gp.weapon_id=0.012/0.1/1014@7 total=12 gp.player=0.011/0.1/1014@7 total=11 gp.cycle=0.008/0.1/1014@7 total=8 module.world=0.127/0.4/57@7 total=7 gp.state=0.006/0.1/1014@7 total=6 engine.scheduler=0.006/2.0/1014@7 total=6 module.probe=1.415/1.6/3@7 total=4 gp.shots=0.004/0.0/1014@7 total=4 cam.aim_key=0.003/0.0/1014@7 total=3 cam.find_active=0.003/0.1/1014@7 total=3 ar.vehicle=0.003/0.1/753@7 total=2 ar.lvs=0.003/0.3/753@7 total=2 gp.spread=0.002/0.1/1014@7 total=2 ar.discover=0.002/0.5/753@7 total=2 ar.reconcile=0.002/0.0/753@7 total=1 ho.carried=0.002/0.0/408@7 total=1 combat.dismember=0.001/0.0/1014@7 total=1 ho.show=0.002/0.0/408@7 total=1 module.autopilot=0.000/0.0/1014@7 total=0 combat.blood=0.000/0.0/1014@7 total=0 module.weapon-probe=0.000/0.0/1014@7 total=0 gp.feel=0.000/0.0/1014@7 total=0 combat.pending=0.000/0.0/1014@7 total=0 gp.recoil=0.000/0.0/1014@7 total=0 cam.fov=0.001/0.0/110@7 total=0
    2026-09-29T05:43:19.177Z [INFO] engine_thread_probe ticks=1014 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1013 ticks_after_skipped_frames=1
    2026-09-29T05:43:19.179Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.20 shdn_us=53.3
    2026-09-29T05:43:20.188Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:43:20.762Z [INFO] camera_already_gone handle=6146 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:20.764Z [INFO] command source=file:cmd_20260929054320527.cmd line="cam prop 0 130 9" reply="camera at 0 deg, 130 m"
    2026-09-29T05:43:25.168Z [INFO] camera_already_gone handle=6402 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:25.169Z [INFO] command source=file:cmd_20260929054325110.cmd line="cam off" reply="camera off"
    2026-09-29T05:43:25.212Z [INFO] [autopilot] event PedAppeared handle=4870
    2026-09-29T05:43:25.212Z [INFO] [autopilot] event PedAppeared handle=5128
    2026-09-29T05:43:25.685Z [INFO] command source=file:cmd_20260929054325498.cmd line="hud on" reply="hud on"
    2026-09-29T05:43:25.934Z [INFO] command source=file:cmd_20260929054325875.cmd line="clear" reply="cleared 1"

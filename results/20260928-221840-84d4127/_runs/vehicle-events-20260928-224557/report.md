# Scenario vehicle-events

- Result: PASS
- Steps: 13, failed: 0
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
- spawncar admiral 8 => spawning admiral
- expect autopilot_spawncar handle=: OK 2026-09-29T05:46:07.766Z [INFO] [autopilot] autopilot_spawncar handle=5897 model=admiral
- expect event VehicleAppeared: OK 2026-09-29T05:46:07.800Z [INFO] [autopilot] event VehicleAppeared handle=5897
- vehicles => vehicles=10 5897:8.009m/1000/1000/0.06mps 16134:108.771m/1000/1000/0mps 3848:122.475m/1000/1000/0mps 10504:127.351m/1000/1000/0mps 6153:131.651m/1000/1000/0mps 11528:132.49m/1000/1000/0mps 13063:136.739m/1000/1000/0mps 13320:142.777m/1000/1000/0mps 15878:147.058m/1000/1000/0mps 13575:149.319m/1000/1000/0mps
- pools => peds=25/120 vehicles=34/140 objects=121/1300
- clear => cleared 1
- expect event VehicleRemoved: OK 2026-09-29T05:46:08.984Z [INFO] [autopilot] event VehicleRemoved handle=5897

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:45:58.679Z [INFO] command source=file:cmd_20260929054558557.cmd line="events on" reply="event log on"
    2026-09-29T05:45:59.191Z [INFO] command source=file:cmd_20260929054558989.cmd line="god on" reply="invincible True"
    2026-09-29T05:45:59.398Z [INFO] [autopilot] event PedRemoved handle=14599
    2026-09-29T05:45:59.443Z [INFO] command source=file:cmd_20260929054559384.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:45:59.725Z [INFO] [autopilot] event PedRemoved handle=7691
    2026-09-29T05:45:59.763Z [INFO] [autopilot] event PedAppeared handle=8470
    2026-09-29T05:45:59.801Z [INFO] [autopilot] event VehicleAppeared handle=8970
    2026-09-29T05:45:59.802Z [INFO] [autopilot] event VehicleAppeared handle=13062
    2026-09-29T05:45:59.832Z [INFO] [autopilot] event PedAppeared handle=8977
    2026-09-29T05:46:02.117Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:46:02.118Z [INFO] command source=file:cmd_20260929054559772.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:46:02.142Z [INFO] [autopilot] event PedRemoved handle=8977
    2026-09-29T05:46:02.143Z [INFO] [autopilot] event PedRemoved handle=8470
    2026-09-29T05:46:02.143Z [INFO] [autopilot] event PedRemoved handle=13317
    2026-09-29T05:46:02.144Z [INFO] [autopilot] event PedRemoved handle=11788
    2026-09-29T05:46:02.145Z [INFO] [autopilot] event PedRemoved handle=14347
    2026-09-29T05:46:02.146Z [INFO] [autopilot] event PedRemoved handle=6157
    2026-09-29T05:46:02.146Z [INFO] [autopilot] event PedRemoved handle=12804
    2026-09-29T05:46:02.147Z [INFO] [autopilot] event PedRemoved handle=15876
    2026-09-29T05:46:02.148Z [INFO] [autopilot] event PedRemoved handle=13828
    2026-09-29T05:46:02.148Z [INFO] [autopilot] event PedRemoved handle=15619
    2026-09-29T05:46:02.149Z [INFO] [autopilot] event PedRemoved handle=15107
    2026-09-29T05:46:02.150Z [INFO] [autopilot] event PedRemoved handle=13059
    2026-09-29T05:46:02.150Z [INFO] [autopilot] event PedRemoved handle=16133
    2026-09-29T05:46:02.151Z [INFO] [autopilot] event PedRemoved handle=14085
    2026-09-29T05:46:02.152Z [INFO] [autopilot] event PedRemoved handle=12037
    2026-09-29T05:46:02.153Z [INFO] [autopilot] event PedRemoved handle=12549
    2026-09-29T05:46:02.153Z [INFO] [autopilot] event PedRemoved handle=9235
    2026-09-29T05:46:02.154Z [INFO] [autopilot] event PedRemoved handle=8723
    2026-09-29T05:46:02.155Z [INFO] [autopilot] event PedRemoved handle=9745
    2026-09-29T05:46:02.156Z [INFO] [autopilot] event PedRemoved handle=8209
    2026-09-29T05:46:02.156Z [INFO] [autopilot] event PedRemoved handle=15367
    2026-09-29T05:46:02.157Z [INFO] [autopilot] event PedRemoved handle=11271
    2026-09-29T05:46:02.158Z [INFO] [autopilot] event PedRemoved handle=13572
    2026-09-29T05:46:02.158Z [INFO] [autopilot] event PedRemoved handle=7177
    2026-09-29T05:46:02.159Z [INFO] [autopilot] event PedRemoved handle=7950
    2026-09-29T05:46:02.160Z [INFO] [autopilot] event PedRemoved handle=6414
    2026-09-29T05:46:02.161Z [INFO] [autopilot] event PedRemoved handle=14854
    2026-09-29T05:46:02.161Z [INFO] [autopilot] event PedRemoved handle=12294
    2026-09-29T05:46:02.162Z [INFO] [autopilot] event PedRemoved handle=4614
    2026-09-29T05:46:02.163Z [INFO] [autopilot] event PedRemoved handle=4360
    2026-09-29T05:46:02.163Z [INFO] [autopilot] event PedRemoved handle=4104
    2026-09-29T05:46:02.164Z [INFO] [autopilot] event PedRemoved handle=16386
    2026-09-29T05:46:02.165Z [INFO] [autopilot] event VehicleRemoved handle=8970
    2026-09-29T05:46:02.165Z [INFO] [autopilot] event VehicleRemoved handle=12293
    2026-09-29T05:46:02.166Z [INFO] [autopilot] event VehicleRemoved handle=12037
    2026-09-29T05:46:02.167Z [INFO] [autopilot] event VehicleRemoved handle=7944
    2026-09-29T05:46:02.167Z [INFO] [autopilot] event VehicleRemoved handle=6664
    2026-09-29T05:46:02.168Z [INFO] [autopilot] event VehicleRemoved handle=4360
    2026-09-29T05:46:02.169Z [INFO] [autopilot] event VehicleRemoved handle=9480
    2026-09-29T05:46:02.170Z [INFO] [autopilot] event VehicleRemoved handle=7176
    2026-09-29T05:46:02.170Z [INFO] [autopilot] event VehicleRemoved handle=5896
    2026-09-29T05:46:02.171Z [INFO] [autopilot] event VehicleRemoved handle=2824
    2026-09-29T05:46:02.171Z [INFO] [autopilot] event VehicleRemoved handle=6409
    2026-09-29T05:46:02.172Z [INFO] [autopilot] event VehicleRemoved handle=3081
    2026-09-29T05:46:02.173Z [INFO] [autopilot] event VehicleRemoved handle=8713
    2026-09-29T05:46:02.173Z [INFO] [autopilot] event VehicleRemoved handle=5129
    2026-09-29T05:46:02.174Z [INFO] [autopilot] event VehicleRemoved handle=9225
    2026-09-29T05:46:02.175Z [INFO] [autopilot] event VehicleRemoved handle=8457
    2026-09-29T05:46:02.176Z [INFO] [autopilot] event VehicleRemoved handle=6921
    2026-09-29T05:46:02.177Z [INFO] [autopilot] event VehicleRemoved handle=3337
    2026-09-29T05:46:02.177Z [INFO] [autopilot] event VehicleRemoved handle=13062
    2026-09-29T05:46:02.178Z [INFO] [autopilot] event VehicleRemoved handle=13830
    2026-09-29T05:46:02.179Z [INFO] [autopilot] event VehicleRemoved handle=14598
    2026-09-29T05:46:02.180Z [INFO] [autopilot] event VehicleRemoved handle=15110
    2026-09-29T05:46:02.180Z [INFO] [autopilot] event VehicleRemoved handle=11782
    2026-09-29T05:46:02.181Z [INFO] [autopilot] event VehicleRemoved handle=12550
    2026-09-29T05:46:02.182Z [INFO] [autopilot] event VehicleRemoved handle=14854
    2026-09-29T05:46:02.182Z [INFO] [autopilot] event VehicleRemoved handle=12806
    2026-09-29T05:46:02.183Z [INFO] [autopilot] event VehicleRemoved handle=4102
    2026-09-29T05:46:02.184Z [INFO] [autopilot] event VehicleRemoved handle=3847
    2026-09-29T05:46:02.184Z [INFO] [autopilot] event VehicleRemoved handle=5383
    2026-09-29T05:46:02.185Z [INFO] [autopilot] event VehicleRemoved handle=4615
    2026-09-29T05:46:02.186Z [INFO] [autopilot] event VehicleRemoved handle=10759
    2026-09-29T05:46:02.187Z [INFO] [autopilot] event VehicleRemoved handle=11015
    2026-09-29T05:46:02.187Z [INFO] [autopilot] event VehicleRemoved handle=7687
    2026-09-29T05:46:02.188Z [INFO] [autopilot] event VehicleRemoved handle=2311
    2026-09-29T05:46:02.189Z [INFO] [autopilot] event VehicleRemoved handle=10247
    2026-09-29T05:46:03.150Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:46:03.337Z [INFO] [autopilot] event VehicleAppeared handle=3848
    2026-09-29T05:46:03.338Z [INFO] [autopilot] event VehicleAppeared handle=6153
    2026-09-29T05:46:03.339Z [INFO] [autopilot] event VehicleAppeared handle=10504
    2026-09-29T05:46:03.339Z [INFO] [autopilot] event VehicleAppeared handle=11528
    2026-09-29T05:46:03.340Z [INFO] [autopilot] event VehicleAppeared handle=13063
    2026-09-29T05:46:03.341Z [INFO] [autopilot] event VehicleAppeared handle=13320
    2026-09-29T05:46:03.341Z [INFO] [autopilot] event VehicleAppeared handle=13575
    2026-09-29T05:46:03.342Z [INFO] [autopilot] event VehicleAppeared handle=15878
    2026-09-29T05:46:03.343Z [INFO] [autopilot] event VehicleAppeared handle=16134
    2026-09-29T05:46:03.662Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:46:03.861Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=2315 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T05:46:04.432Z [INFO] [autopilot] event PedAppeared handle=5901
    2026-09-29T05:46:04.487Z [INFO] [autopilot] event PedAppeared handle=6158
    2026-09-29T05:46:04.829Z [INFO] [autopilot] event PedAppeared handle=8978
    2026-09-29T05:46:05.005Z [INFO] [autopilot] event PedAppeared handle=9236
    2026-09-29T05:46:05.031Z [INFO] [autopilot] event PedAppeared handle=9488
    2026-09-29T05:46:05.096Z [INFO] [autopilot] event PedAppeared handle=9746
    2026-09-29T05:46:05.197Z [INFO] [autopilot] event PedAppeared handle=10005
    2026-09-29T05:46:06.916Z [INFO] [autopilot] event BulletFired shooter=6158 weapon=7 by_player=False from=(-87.19, 637.726, 15.103) to=(-53.813, 693.64, 14.64)
    2026-09-29T05:46:06.967Z [INFO] [autopilot] event PedAppeared handle=10260
    2026-09-29T05:46:07.242Z [INFO] [autopilot] event BulletFired shooter=6158 weapon=7 by_player=False from=(-86.884, 637.971, 15.154) to=(-67.549, 670.295, 14.685)
    2026-09-29T05:46:07.329Z [INFO] [autopilot] event BulletFired shooter=5901 weapon=7 by_player=False from=(-81.957, 638.56, 15.159) to=(-53.5, 697.126, 15.717)
    2026-09-29T05:46:07.416Z [INFO] command source=file:cmd_20260929054607262.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:46:07.674Z [INFO] [autopilot] event BulletFired shooter=5901 weapon=7 by_player=False from=(-81.751, 638.88, 15.161) to=(-67.549, 668.997, 14.761)
    2026-09-29T05:46:07.680Z [INFO] command source=file:cmd_20260929054607657.cmd line="spawncar admiral 8" reply="spawning admiral"
    2026-09-29T05:46:07.766Z [INFO] [autopilot] autopilot_spawncar handle=5897 model=admiral
    2026-09-29T05:46:07.800Z [INFO] [autopilot] event VehicleAppeared handle=5897
    2026-09-29T05:46:07.862Z [INFO] [autopilot] event PedRemoved handle=10005
    2026-09-29T05:46:07.898Z [INFO] [autopilot] event PedRemoved handle=9746
    2026-09-29T05:46:07.997Z [INFO] [autopilot] event BulletFired shooter=5901 weapon=7 by_player=False from=(-81.765, 638.906, 15.166) to=(-52.608, 697.106, 14.497)
    2026-09-29T05:46:07.998Z [INFO] [autopilot] event PedRemoved handle=9236
    2026-09-29T05:46:08.210Z [INFO] command source=file:cmd_20260929054608072.cmd line="vehicles" reply="vehicles=10 5897:8.009m/1000/1000/0.06mps 16134:108.771m/1000/1000/0mps 3848:122.475m/1000/1000/0mps 10504:127.351m/1000/1000/0mps 6153:131.651m/1000/1000/0mps 11528:132.49m/1000/1000/0mps 13063:136.739m/1000/1000/0mps 13320:142.777m/1000/1000/0mps 15878:147.058m/1000/1000/0mps 13575:149.319m/1000/1000/0mps"
    2026-09-29T05:46:08.312Z [INFO] [autopilot] event PedAppeared handle=9747
    2026-09-29T05:46:08.313Z [INFO] [autopilot] event PedAppeared handle=10006
    2026-09-29T05:46:08.463Z [INFO] [autopilot] event PedAppeared handle=10508
    2026-09-29T05:46:08.469Z [INFO] command source=file:cmd_20260929054608459.cmd line="pools" reply="peds=25/120 vehicles=34/140 objects=121/1300"
    2026-09-29T05:46:08.741Z [INFO] [autopilot] event BulletFired shooter=6158 weapon=7 by_player=False from=(-86.882, 637.977, 15.152) to=(-52.305, 693.137, 14.591)
    2026-09-29T05:46:08.872Z [INFO] [autopilot] event PedAppeared handle=10768
    2026-09-29T05:46:08.955Z [INFO] command source=file:cmd_20260929054608838.cmd line="clear" reply="cleared 1"
    2026-09-29T05:46:08.984Z [INFO] [autopilot] event VehicleRemoved handle=5897
    2026-09-29T05:46:09.070Z [INFO] [autopilot] event BulletFired shooter=6158 weapon=7 by_player=False from=(-86.882, 637.989, 15.151) to=(-67.549, 669.42, 14.485)
    2026-09-29T05:46:09.219Z [INFO] [autopilot] event BulletFired shooter=5901 weapon=7 by_player=False from=(-81.748, 638.889, 15.151) to=(-52.788, 697.21, 16.013)

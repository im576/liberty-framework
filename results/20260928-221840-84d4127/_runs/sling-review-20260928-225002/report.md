# Scenario sling-review

- Result: FAIL
- Steps: 31, failed: 1
- Game alive at end: True
- Log errors during run: 1

## Failed steps
- expect holster_sling_attached: no new log line in 15 s

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- give 15 200 => gave 15
- give 14 200 => gave 14
- give 7 100 => gave 7
- select 7 => selected 7
- wait 3000 ms
- FAILED: expect holster_sling_attached: no new log line in 15 s
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
    2026-09-29T05:50:45.783Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:50:02.469Z [INFO] [autopilot] event PedAppeared handle=12558
    2026-09-29T05:50:02.470Z [INFO] [autopilot] event PedRemoved handle=12557
    2026-09-29T05:50:02.585Z [INFO] [autopilot] event VehicleAppeared handle=13067
    2026-09-29T05:50:02.893Z [INFO] [autopilot] event PedAppeared handle=13328
    2026-09-29T05:50:03.422Z [INFO] command source=file:cmd_20260929055003183.cmd line="events on" reply="event log on"
    2026-09-29T05:50:03.443Z [INFO] [autopilot] event PedAppeared handle=15622
    2026-09-29T05:50:03.444Z [INFO] [autopilot] event PedRemoved handle=15621
    2026-09-29T05:50:03.481Z [INFO] [autopilot] event VehicleAppeared handle=9995
    2026-09-29T05:50:03.665Z [INFO] command source=file:cmd_20260929055003638.cmd line="god on" reply="invincible True"
    2026-09-29T05:50:03.934Z [INFO] [autopilot] event PedAppeared handle=15878
    2026-09-29T05:50:03.935Z [INFO] [autopilot] event PedRemoved handle=15622
    2026-09-29T05:50:04.059Z [INFO] [autopilot] event VehicleAppeared handle=5645
    2026-09-29T05:50:04.165Z [INFO] command source=file:cmd_20260929055004031.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:50:04.197Z [INFO] [autopilot] event VehicleRemoved handle=8972
    2026-09-29T05:50:04.242Z [INFO] [autopilot] event PedAppeared handle=15623
    2026-09-29T05:50:04.243Z [INFO] [autopilot] event PedRemoved handle=12558
    2026-09-29T05:50:04.317Z [INFO] [autopilot] event VehicleRemoved handle=6156
    2026-09-29T05:50:06.526Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:50:06.527Z [INFO] command source=file:cmd_20260929055004411.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:50:06.554Z [INFO] [autopilot] event PedRemoved handle=15878
    2026-09-29T05:50:06.555Z [INFO] [autopilot] event PedRemoved handle=13328
    2026-09-29T05:50:06.556Z [INFO] [autopilot] event PedRemoved handle=15623
    2026-09-29T05:50:06.557Z [INFO] [autopilot] event PedRemoved handle=15111
    2026-09-29T05:50:06.558Z [INFO] [autopilot] event PedRemoved handle=13070
    2026-09-29T05:50:06.558Z [INFO] [autopilot] event PedRemoved handle=12300
    2026-09-29T05:50:06.559Z [INFO] [autopilot] event PedRemoved handle=11278
    2026-09-29T05:50:06.560Z [INFO] [autopilot] event PedRemoved handle=10016
    2026-09-29T05:50:06.561Z [INFO] [autopilot] event PedRemoved handle=11539
    2026-09-29T05:50:06.561Z [INFO] [autopilot] event PedRemoved handle=8986
    2026-09-29T05:50:06.562Z [INFO] [autopilot] event PedRemoved handle=8731
    2026-09-29T05:50:06.562Z [INFO] [autopilot] event PedRemoved handle=8215
    2026-09-29T05:50:06.563Z [INFO] [autopilot] event PedRemoved handle=18179
    2026-09-29T05:50:06.564Z [INFO] [autopilot] event PedRemoved handle=17667
    2026-09-29T05:50:06.564Z [INFO] [autopilot] event PedRemoved handle=18435
    2026-09-29T05:50:06.565Z [INFO] [autopilot] event PedRemoved handle=16899
    2026-09-29T05:50:06.566Z [INFO] [autopilot] event PedRemoved handle=16644
    2026-09-29T05:50:06.566Z [INFO] [autopilot] event PedRemoved handle=15368
    2026-09-29T05:50:06.567Z [INFO] [autopilot] event PedRemoved handle=14090
    2026-09-29T05:50:06.568Z [INFO] [autopilot] event PedRemoved handle=14857
    2026-09-29T05:50:06.569Z [INFO] [autopilot] event PedRemoved handle=13833
    2026-09-29T05:50:06.569Z [INFO] [autopilot] event PedRemoved handle=14351
    2026-09-29T05:50:06.570Z [INFO] [autopilot] event PedRemoved handle=12815
    2026-09-29T05:50:06.570Z [INFO] [autopilot] event PedRemoved handle=14605
    2026-09-29T05:50:06.571Z [INFO] [autopilot] event PedRemoved handle=13581
    2026-09-29T05:50:06.572Z [INFO] [autopilot] event PedRemoved handle=10514
    2026-09-29T05:50:06.572Z [INFO] [autopilot] event PedRemoved handle=7442
    2026-09-29T05:50:06.573Z [INFO] [autopilot] event PedRemoved handle=7181
    2026-09-29T05:50:06.574Z [INFO] [autopilot] event PedRemoved handle=12044
    2026-09-29T05:50:06.575Z [INFO] [autopilot] event PedRemoved handle=6924
    2026-09-29T05:50:06.576Z [INFO] [autopilot] event PedRemoved handle=7956
    2026-09-29T05:50:06.577Z [INFO] [autopilot] event PedRemoved handle=9492
    2026-09-29T05:50:06.577Z [INFO] [autopilot] event PedRemoved handle=11801
    2026-09-29T05:50:06.578Z [INFO] [autopilot] event PedRemoved handle=10777
    2026-09-29T05:50:06.579Z [INFO] [autopilot] event PedRemoved handle=9241
    2026-09-29T05:50:06.579Z [INFO] [autopilot] event VehicleRemoved handle=10250
    2026-09-29T05:50:06.580Z [INFO] [autopilot] event VehicleRemoved handle=12553
    2026-09-29T05:50:06.581Z [INFO] [autopilot] event VehicleRemoved handle=12040
    2026-09-29T05:50:06.581Z [INFO] [autopilot] event VehicleRemoved handle=8206
    2026-09-29T05:50:06.582Z [INFO] [autopilot] event VehicleRemoved handle=7438
    2026-09-29T05:50:06.583Z [INFO] [autopilot] event VehicleRemoved handle=11020
    2026-09-29T05:50:06.583Z [INFO] [autopilot] event VehicleRemoved handle=8716
    2026-09-29T05:50:06.584Z [INFO] [autopilot] event VehicleRemoved handle=5388
    2026-09-29T05:50:06.584Z [INFO] [autopilot] event VehicleRemoved handle=5645
    2026-09-29T05:50:06.585Z [INFO] [autopilot] event VehicleRemoved handle=7181
    2026-09-29T05:50:06.586Z [INFO] [autopilot] event VehicleRemoved handle=4109
    2026-09-29T05:50:06.586Z [INFO] [autopilot] event VehicleRemoved handle=6925
    2026-09-29T05:50:06.587Z [INFO] [autopilot] event VehicleRemoved handle=4365
    2026-09-29T05:50:06.587Z [INFO] [autopilot] event VehicleRemoved handle=6413
    2026-09-29T05:50:06.588Z [INFO] [autopilot] event VehicleRemoved handle=9995
    2026-09-29T05:50:06.589Z [INFO] [autopilot] event VehicleRemoved handle=13067
    2026-09-29T05:50:06.589Z [INFO] [autopilot] event VehicleRemoved handle=9739
    2026-09-29T05:50:06.590Z [INFO] [autopilot] event VehicleRemoved handle=9483
    2026-09-29T05:50:06.590Z [INFO] [autopilot] event VehicleRemoved handle=8459
    2026-09-29T05:50:06.591Z [INFO] [autopilot] event VehicleRemoved handle=9227
    2026-09-29T05:50:07.070Z [INFO] [autopilot] event VehicleAppeared handle=9228
    2026-09-29T05:50:07.071Z [INFO] [autopilot] event VehicleAppeared handle=9484
    2026-09-29T05:50:07.072Z [INFO] [autopilot] event VehicleAppeared handle=9740
    2026-09-29T05:50:07.072Z [INFO] [autopilot] event VehicleAppeared handle=13323
    2026-09-29T05:50:07.073Z [INFO] [autopilot] event VehicleAppeared handle=14344
    2026-09-29T05:50:07.074Z [INFO] [autopilot] event VehicleAppeared handle=14600
    2026-09-29T05:50:07.074Z [INFO] [autopilot] event VehicleAppeared handle=14856
    2026-09-29T05:50:07.075Z [INFO] [autopilot] event VehicleAppeared handle=15113
    2026-09-29T05:50:07.076Z [INFO] [autopilot] event VehicleAppeared handle=15367
    2026-09-29T05:50:07.076Z [INFO] [autopilot] event VehicleAppeared handle=16390
    2026-09-29T05:50:07.077Z [INFO] [autopilot] event VehicleAppeared handle=16645
    2026-09-29T05:50:07.079Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=37395 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T05:50:07.440Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:50:07.461Z [INFO] [autopilot] event VehicleAppeared handle=15623
    2026-09-29T05:50:07.461Z [INFO] [autopilot] event VehicleAppeared handle=15880
    2026-09-29T05:50:07.462Z [INFO] [autopilot] event VehicleAppeared handle=16136
    2026-09-29T05:50:07.463Z [INFO] [autopilot] event VehicleRemoved handle=9228
    2026-09-29T05:50:08.466Z [INFO] [autopilot] event PedAppeared handle=12045
    2026-09-29T05:50:08.491Z [INFO] [autopilot] event PedAppeared handle=12301
    2026-09-29T05:50:08.618Z [INFO] [autopilot] event PedAppeared handle=12559
    2026-09-29T05:50:08.663Z [INFO] [autopilot] event PedAppeared handle=12816
    2026-09-29T05:50:08.693Z [INFO] [autopilot] event PedAppeared handle=13071
    2026-09-29T05:50:08.709Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:50:08.942Z [INFO] [autopilot] event PedAppeared handle=13329
    2026-09-29T05:50:09.139Z [INFO] [autopilot] event PedRemoved handle=12301
    2026-09-29T05:50:09.801Z [INFO] [autopilot] event PedAppeared handle=13582
    2026-09-29T05:50:09.821Z [INFO] [autopilot] event PedAppeared handle=13834
    2026-09-29T05:50:10.433Z [INFO] [autopilot] event PedRemoved handle=13582
    2026-09-29T05:50:11.161Z [INFO] [autopilot] event PedAppeared handle=14091
    2026-09-29T05:50:11.166Z [INFO] command source=file:cmd_20260929055010923.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:50:11.339Z [INFO] [autopilot] event PedRemoved handle=12816
    2026-09-29T05:50:11.376Z [INFO] [autopilot] event PedRemoved handle=13329
    2026-09-29T05:50:11.430Z [INFO] command source=file:cmd_20260929055011307.cmd line="weather 1" reply="weather 1"
    2026-09-29T05:50:11.682Z [INFO] [autopilot] event PedRemoved handle=12559
    2026-09-29T05:50:11.874Z [INFO] [autopilot] event PedAppeared handle=12817
    2026-09-29T05:50:11.961Z [INFO] [autopilot] event PedRemoved handle=14091
    2026-09-29T05:50:11.966Z [INFO] command source=file:cmd_20260929055011702.cmd line="give 15 200" reply="gave 15"
    2026-09-29T05:50:11.994Z [INFO] [autopilot] event PlayerWeaponChanged 0->15
    2026-09-29T05:50:11.996Z [INFO] weapon_changed from=0 to=15 profile=vanilla
    2026-09-29T05:50:11.998Z [INFO] arsenal_gain id=15 owned=False mission=False
    2026-09-29T05:50:12.064Z [INFO] [autopilot] event PedAppeared handle=13330
    2026-09-29T05:50:12.160Z [INFO] [autopilot] event PedAppeared handle=13583
    2026-09-29T05:50:12.196Z [INFO] [autopilot] event PedAppeared handle=14092
    2026-09-29T05:50:12.475Z [INFO] command source=file:cmd_20260929055012354.cmd line="give 14 200" reply="gave 14"
    2026-09-29T05:50:12.508Z [INFO] [autopilot] event PlayerWeaponChanged 15->14
    2026-09-29T05:50:12.509Z [INFO] [autopilot] event PedAppeared handle=14352
    2026-09-29T05:50:12.511Z [INFO] weapon_changed from=15 to=14 profile=vanilla
    2026-09-29T05:50:12.512Z [INFO] arsenal_gain id=14 owned=False mission=False
    2026-09-29T05:50:12.571Z [INFO] [autopilot] event ReloadFinished weapon=14 clip_after=30
    2026-09-29T05:50:12.734Z [INFO] [autopilot] event PedAppeared handle=14606
    2026-09-29T05:50:12.997Z [INFO] command source=file:cmd_20260929055012751.cmd line="give 7 100" reply="gave 7"
    2026-09-29T05:50:13.019Z [INFO] [autopilot] event PlayerWeaponChanged 14->7
    2026-09-29T05:50:13.021Z [INFO] weapon_changed from=14 to=7 profile=vanilla
    2026-09-29T05:50:13.022Z [INFO] arsenal_gain id=7 owned=False mission=False
    2026-09-29T05:50:13.121Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T05:50:13.242Z [INFO] command source=file:cmd_20260929055013144.cmd line="select 7" reply="selected 7"
    2026-09-29T05:50:13.410Z [INFO] [autopilot] event PedRemoved handle=14606
    2026-09-29T05:50:14.140Z [INFO] [autopilot] event PedAppeared handle=16389
    2026-09-29T05:50:14.553Z [INFO] [autopilot] event PedRemoved handle=14092
    2026-09-29T05:50:14.554Z [INFO] [autopilot] event PedRemoved handle=13330
    2026-09-29T05:50:14.618Z [INFO] [autopilot] event PedRemoved handle=13583
    2026-09-29T05:50:14.842Z [INFO] [autopilot] event PedRemoved handle=12817
    2026-09-29T05:50:14.899Z [INFO] [autopilot] event PedRemoved handle=16389
    2026-09-29T05:50:15.001Z [INFO] [autopilot] event PedAppeared handle=13331
    2026-09-29T05:50:15.018Z [INFO] [autopilot] event PedAppeared handle=13584
    2026-09-29T05:50:15.062Z [INFO] [autopilot] event PedAppeared handle=14093
    2026-09-29T05:50:15.105Z [INFO] [autopilot] event PedAppeared handle=14607
    2026-09-29T05:50:15.207Z [INFO] [autopilot] event PedAppeared handle=16390
    2026-09-29T05:50:15.600Z [INFO] [autopilot] event PedAppeared handle=16645
    2026-09-29T05:50:15.675Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.052, 637.658, 15.146) to=(-31.37, 742.661, 15.609)
    2026-09-29T05:50:15.836Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.044, 637.995, 15.181) to=(-77.874, 646.518, 15.033)
    2026-09-29T05:50:16.008Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.01, 638.165, 15.189) to=(-62.945, 678.274, 15.722)
    2026-09-29T05:50:16.148Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.997, 638.18, 15.202) to=(263.893, 1360.236, 12.965)
    2026-09-29T05:50:16.233Z [INFO] [autopilot] event PedAppeared handle=16900
    2026-09-29T05:50:16.234Z [INFO] [autopilot] event PedRemoved handle=16645
    2026-09-29T05:50:16.317Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.009, 638.193, 15.197) to=(-67.549, 669.379, 14.629)
    2026-09-29T05:50:16.681Z [INFO] [autopilot] event PedRemoved handle=16900
    2026-09-29T05:50:16.903Z [INFO] [autopilot] event PedAppeared handle=17156
    2026-09-29T05:50:17.309Z [INFO] [autopilot] event PedRemoved handle=13584
    2026-09-29T05:50:17.401Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.975, 638.198, 15.185) to=(-77.794, 646.439, 15.049)
    2026-09-29T05:50:17.402Z [INFO] [autopilot] event VehicleAppeared handle=6668
    2026-09-29T05:50:17.483Z [INFO] [autopilot] event PedRemoved handle=13331
    2026-09-29T05:50:17.547Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.003, 638.186, 15.198) to=(274.498, 1354.988, 4.975)
    2026-09-29T05:50:17.601Z [INFO] [autopilot] event PedRemoved handle=17156
    2026-09-29T05:50:17.602Z [INFO] [autopilot] event PedRemoved handle=16390
    2026-09-29T05:50:17.648Z [INFO] [autopilot] event PedRemoved handle=13834
    2026-09-29T05:50:17.724Z [INFO] [autopilot] event PedRemoved handle=14093
    2026-09-29T05:50:17.777Z [INFO] [autopilot] event PedRemoved handle=14607
    2026-09-29T05:50:18.063Z [INFO] [autopilot] event PedAppeared handle=11280
    2026-09-29T05:50:18.090Z [INFO] [autopilot] event VehicleAppeared handle=10764
    2026-09-29T05:50:18.110Z [INFO] [autopilot] event PedAppeared handle=11541
    2026-09-29T05:50:18.160Z [INFO] [autopilot] event PedAppeared handle=11803
    2026-09-29T05:50:18.211Z [INFO] [autopilot] event PedAppeared handle=12302
    2026-09-29T05:50:18.346Z [INFO] [autopilot] event PedAppeared handle=12560
    2026-09-29T05:50:18.572Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.974, 638.197, 15.186) to=(-77.922, 646.539, 15.001)
    2026-09-29T05:50:18.714Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:50:18.730Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.006, 638.187, 15.2) to=(-74.446, 654.794, 14.855)
    2026-09-29T05:50:18.880Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.004, 638.182, 15.192) to=(266.06, 1359.134, 10.605)
    2026-09-29T05:50:19.449Z [INFO] [autopilot] event PedRemoved handle=14352
    2026-09-29T05:50:19.466Z [INFO] [autopilot] event PedAppeared handle=8216
    2026-09-29T05:50:19.467Z [INFO] [autopilot] event PedAppeared handle=8478
    2026-09-29T05:50:19.467Z [INFO] [autopilot] event PedAppeared handle=8732
    2026-09-29T05:50:19.839Z [INFO] [autopilot] event PedAppeared handle=12818
    2026-09-29T05:50:19.885Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.975, 638.198, 15.187) to=(272.524, 1355.848, -10.3)
    2026-09-29T05:50:19.910Z [INFO] [autopilot] event PedAppeared handle=7443
    2026-09-29T05:50:19.911Z [INFO] [autopilot] event PedAppeared handle=7697
    2026-09-29T05:50:19.912Z [INFO] [autopilot] event PedAppeared handle=7957
    2026-09-29T05:50:20.144Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-85.946, 637.485, 15.204) to=(-75.217, 654.96, 14.974)
    2026-09-29T05:50:20.281Z [INFO] performance samples=1105 frame_p50_ms=21 frame_p95_ms=44 frame_p99_ms=70 frames_over_33ms=170 frames_over_50ms=26 gunplay_avg_ms=1.299 gunplay_max_ms=10.223 phase_samples=1105 phase_setup_avg_ms=0.870 phase_setup_max_ms=9.662 phase_camera_avg_ms=0.027 phase_camera_max_ms=3.799 phase_bullets_avg_ms=0.003 phase_bullets_max_ms=0.158 phase_weapon_avg_ms=0.018 phase_weapon_max_ms=0.216 phase_hud_avg_ms=0.380 phase_hud_max_ms=0.662
    2026-09-29T05:50:20.282Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.475/2102.0/1105@7 total=4945 module.gunplay=1.305/10.2/1105@7 total=1442 tick.gunplay=1.304/10.2/1105@7 total=1441 gp.freeaim=0.674/9.5/1105@7 total=745 module.arsenal=0.681/18.1/707@7 total=481 tick.arsenal=0.680/18.1/707@7 total=481 ar.storage=0.434/6.0/707@7 total=307 engine.world=0.275/37.8/1105@7 total=304 module.combat=0.215/137.9/1105@7 total=238 tick.combat=0.215/137.9/1105@7 total=237 module.atmosphere=0.167/2.0/1105@7 total=184 tick.atmosphere=0.166/2.0/1105@7 total=183 gp.index_pad=0.133/0.4/1105@7 total=147 combat.pending=0.125/137.8/1105@7 total=138 ar.safehouse_flags=0.156/1.8/707@7 total=110 ar.reconcile=0.062/16.9/707@7 total=44 module.holsters=0.099/12.6/389@7 total=39 tick.holsters=0.099/12.6/389@7 total=38 combat.blood=0.025/1.5/1105@7 total=28 combat.sample=1.349/3.3/20@7 total=27 cam.handle=0.021/3.8/1105@7 total=23 module.devtools=0.028/3.8/707@7 total=20 ho.show=0.050/12.6/389@7 total=20 tick.devtools=0.027/3.8/707@7 total=19 combat.dismember=0.017/10.4/1105@7 total=19 gp.shoulder=0.015/0.2/1105@7 total=17 gp.weapon_id=0.014/0.9/1105@7 total=16 gp.player=0.012/0.3/1105@7 total=13 gp.cycle=0.008/0.1/1105@7 total=9 gp.state=0.006/0.1/1105@7 total=7 module.world=0.075/0.5/56@7 total=4 module.probe=1.355/1.4/3@7 total=4 gp.shots=0.003/0.1/1105@7 total=3 cam.aim_key=0.003/0.1/1105@7 total=3 cam.find_active=0.003/0.0/1105@7 total=3 engine.scheduler=0.002/1.9/1105@7 total=3 ar.vehicle=0.003/0.0/707@7 total=2 gp.spread=0.002/0.0/1105@7 total=2 ar.lvs=0.003/0.3/707@7 total=2 ar.discover=0.003/0.6/707@7 total=2 ho.carried=0.002/0.0/389@7 total=1 module.autopilot=0.000/0.0/1105@7 total=0 module.weapon-probe=0.000/0.0/1105@7 total=0 gp.feel=0.000/0.0/1105@7 total=0 gp.recoil=0.000/0.0/1105@7 total=0 cam.fov=0.001/0.0/104@7 total=0
    2026-09-29T05:50:20.283Z [INFO] engine_thread_probe ticks=1105 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1104 ticks_after_skipped_frames=1
    2026-09-29T05:50:20.284Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.20 shdn_us=27.7
    2026-09-29T05:50:20.285Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=20436 core=on peds=15 vehicles=15 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV frame_ms=21.38 p95_ms=31.35 pressure=0.00 private_mb=2313 working_set_mb=1862 address_free_mb=1083 largest_free_block_mb=1025 managed_mb=15 physical_load=83% core_us=68.2
    2026-09-29T05:50:20.304Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-85.928, 637.563, 15.195) to=(314.507, 1330.684, -2.555)
    2026-09-29T05:50:20.305Z [INFO] [autopilot] event PedRemoved handle=12818
    2026-09-29T05:50:20.309Z [INFO] density frame_ms=22.4 peds=1.00 cars=1.00
    2026-09-29T05:50:20.452Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-85.923, 637.568, 15.202) to=(-75.102, 655.337, 15.116)
    2026-09-29T05:50:20.591Z [INFO] [autopilot] event PedRemoved handle=11803
    2026-09-29T05:50:20.612Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-85.941, 637.59, 15.199) to=(328.999, 1322.243, 5.027)
    2026-09-29T05:50:20.760Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.979, 638.2, 15.187) to=(-67.549, 669.028, 14.612)
    2026-09-29T05:50:20.791Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-85.943, 637.603, 15.2) to=(316.932, 1329.471, 14.671)
    2026-09-29T05:50:20.846Z [INFO] [autopilot] event PedRemoved handle=12302
    2026-09-29T05:50:20.917Z [INFO] [autopilot] event PedAppeared handle=12303
    2026-09-29T05:50:20.953Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-85.932, 637.606, 15.191) to=(317.01, 1329.502, 21.636)
    2026-09-29T05:50:21.145Z [INFO] [autopilot] event PedRemoved handle=11541
    2026-09-29T05:50:21.995Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.976, 638.196, 15.189) to=(255.242, 1364.13, -8.578)
    2026-09-29T05:50:22.140Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.011, 638.186, 15.203) to=(-78.004, 646.603, 14.962)
    2026-09-29T05:50:22.314Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.007, 638.182, 15.193) to=(253.638, 1365.175, 19.281)
    2026-09-29T05:50:22.444Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-84.292, 639.959, 15.111) to=(-75.984, 654.977, 14.699)
    2026-09-29T05:50:23.129Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.979, 638.199, 15.187) to=(255.721, 1364.085, 8.127)
    2026-09-29T05:50:23.274Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.009, 638.187, 15.2) to=(-62.851, 678.239, 15.375)
    2026-09-29T05:50:23.473Z [INFO] [autopilot] event PedRemoved handle=12303
    2026-09-29T05:50:23.488Z [INFO] [autopilot] event PedRemoved handle=8732
    2026-09-29T05:50:23.489Z [INFO] [autopilot] event PedRemoved handle=8478
    2026-09-29T05:50:23.489Z [INFO] [autopilot] event PedRemoved handle=8216
    2026-09-29T05:50:23.506Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.869, 640.697, 15.189) to=(321.876, 1330.941, 16.364)
    2026-09-29T05:50:23.806Z [INFO] [autopilot] event PedAppeared handle=11804
    2026-09-29T05:50:23.953Z [INFO] [autopilot] event PedAppeared handle=12304
    2026-09-29T05:50:24.275Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.979, 638.198, 15.189) to=(262.816, 1360.526, -6.933)
    2026-09-29T05:50:24.450Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-41.51, 694.808, 109.055) to=(-63.189, 676.119, 13.722)
    2026-09-29T05:50:24.511Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.866, 640.669, 15.185) to=(-62.901, 678.24, 15.222)
    2026-09-29T05:50:24.592Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-43.253, 695.612, 109.087) to=(-66.442, 677.277, 13.596)
    2026-09-29T05:50:24.662Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.901, 640.667, 15.202) to=(324.78, 1329.102, 0.819)
    2026-09-29T05:50:24.803Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-45.136, 696.474, 109.166) to=(-62.964, 676.161, 13.721)
    2026-09-29T05:50:24.827Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.899, 640.664, 15.192) to=(-67.549, 670.58, 14.635)
    2026-09-29T05:50:24.966Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.895, 640.661, 15.204) to=(322.844, 1330.165, -2.574)
    2026-09-29T05:50:25.148Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.904, 640.66, 15.196) to=(315.756, 1334.379, 13.101)
    2026-09-29T05:50:25.312Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.902, 640.657, 15.199) to=(-67.526, 668.829, 14.943)
    2026-09-29T05:50:25.591Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.979, 638.194, 15.192) to=(276.904, 1354.053, 18.192)
    2026-09-29T05:50:25.745Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.01, 638.187, 15.201) to=(-67.647, 667.001, 15.104)
    2026-09-29T05:50:25.898Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.009, 638.182, 15.194) to=(-67.604, 667.481, 15.281)
    2026-09-29T05:50:26.058Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.003, 638.185, 15.205) to=(263.242, 1360.376, 0.722)
    2026-09-29T05:50:26.110Z [INFO] [autopilot] event PedRemoved handle=11804
    2026-09-29T05:50:26.234Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.012, 638.191, 15.198) to=(-67.67, 667.416, 15.217)
    2026-09-29T05:50:26.235Z [INFO] [autopilot] event PedRemoved handle=12304
    2026-09-29T05:50:26.531Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-58.413, 702.965, 108.52) to=(-63.804, 676.343, 13.667)
    2026-09-29T05:50:26.655Z [INFO] [autopilot] event VehicleAppeared handle=11274
    2026-09-29T05:50:26.698Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.869, 640.663, 15.188) to=(300.403, 1343.13, 17.589)
    2026-09-29T05:50:26.699Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-59.478, 703.647, 107.914) to=(-66.457, 673.071, 13.597)
    2026-09-29T05:50:26.727Z [INFO] [autopilot] event PedAppeared handle=12305
    2026-09-29T05:50:26.854Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.903, 640.657, 15.2) to=(-75.186, 655.335, 14.871)
    2026-09-29T05:50:27.247Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.978, 638.199, 15.188) to=(266.329, 1359.059, 17.923)
    2026-09-29T05:50:27.422Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.023, 638.198, 15.137) to=(-67.406, 667.212, 15.393)
    2026-09-29T05:50:27.546Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-63.832, 706.477, 104.172) to=(-62.25, 673.528, 13.718)
    2026-09-29T05:50:27.582Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.071, 638.553, 15.163) to=(257.323, 1363.702, 9.674)
    2026-09-29T05:50:27.720Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-64.71, 706.989, 103.222) to=(-64.968, 672.802, 13.558)
    2026-09-29T05:50:27.744Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.907, 639.216, 15.16) to=(-74.471, 654.806, 14.845)
    2026-09-29T05:50:27.897Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.812, 639.94, 15.135) to=(276.8, 1355.471, -12.484)
    2026-09-29T05:50:27.936Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-65.717, 707.547, 102.1) to=(-61.997, 672.785, 13.69)
    2026-09-29T05:50:28.080Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-81.85, 640.566, 15.164) to=(-31.57, 744.161, 16.472)
    2026-09-29T05:50:28.126Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-66.572, 708.005, 101.18) to=(-63.863, 671.948, 13.564)
    2026-09-29T05:50:28.145Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.87, 640.663, 15.188) to=(319.768, 1331.916, -4.772)
    2026-09-29T05:50:28.302Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.904, 640.657, 15.2) to=(305.569, 1340.274, 21.217)
    2026-09-29T05:50:28.432Z [INFO] [autopilot] event PedAppeared handle=9243
    2026-09-29T05:50:28.495Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.902, 640.653, 15.191) to=(-67.549, 670.408, 14.713)
    2026-09-29T05:50:28.709Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:50:29.117Z [INFO] [autopilot] event PedRemoved handle=12305
    2026-09-29T05:50:29.165Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-71.313, 710.768, 97.061) to=(-66.118, 673.938, 13.579)
    2026-09-29T05:50:29.333Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-72.143, 711.294, 96.505) to=(-64.852, 673.139, 13.558)
    2026-09-29T05:50:29.553Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-73.067, 711.866, 95.939) to=(-65.128, 675.2, 13.558)
    2026-09-29T05:50:29.624Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.094, 642.014, 15.189) to=(-75.04, 654.788, 15.018)
    2026-09-29T05:50:29.738Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-73.944, 712.381, 95.451) to=(-63.388, 673.802, 13.698)
    2026-09-29T05:50:29.781Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.127, 642.012, 15.201) to=(-75.186, 655.116, 15.071)
    2026-09-29T05:50:29.947Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.124, 641.995, 15.192) to=(-63.058, 678.382, 15.525)
    2026-09-29T05:50:30.100Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.119, 641.983, 15.203) to=(-35.401, 725.407, 16.046)
    2026-09-29T05:50:30.261Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.126, 641.979, 15.195) to=(-75.118, 654.776, 15.006)
    2026-09-29T05:50:30.438Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.123, 641.976, 15.199) to=(289.719, 1350.801, -5.993)
    2026-09-29T05:50:30.643Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.872, 640.663, 15.189) to=(-75.869, 654.977, 14.715)
    2026-09-29T05:50:30.811Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.907, 640.658, 15.202) to=(-75.584, 654.984, 14.723)
    2026-09-29T05:50:30.976Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.906, 640.653, 15.194) to=(316.566, 1333.992, 20.318)
    2026-09-29T05:50:31.178Z [INFO] [autopilot] event PedRemoved handle=9243
    2026-09-29T05:50:31.604Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-80.912, 714.958, 92.734) to=(-63.822, 671.049, 13.555)
    2026-09-29T05:50:31.656Z [INFO] [autopilot] event PedAppeared handle=9494
    2026-09-29T05:50:31.814Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-81.508, 715.124, 92.55) to=(-66.271, 674.504, 13.587)
    2026-09-29T05:50:32.015Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.097, 641.99, 15.186) to=(-31.57, 733.99, 15.655)
    2026-09-29T05:50:32.088Z [INFO] [autopilot] event PedRemoved handle=13071
    2026-09-29T05:50:32.094Z [INFO] command source=file:cmd_20260929055032032.cmd line="hud off" reply="hud off"
    2026-09-29T05:50:32.164Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.127, 641.978, 15.201) to=(-75.095, 654.806, 14.713)
    2026-09-29T05:50:32.165Z [INFO] [autopilot] event PedRemoved handle=11280
    2026-09-29T05:50:32.357Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.125, 641.974, 15.192) to=(-75.097, 655.116, 15.136)
    2026-09-29T05:50:32.514Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.118, 641.976, 15.201) to=(-75.098, 655.277, 15.121)
    2026-09-29T05:50:32.599Z [INFO] command source=file:cmd_20260929055032422.cmd line="cam 0 2.2 0.4" reply="camera at 0 deg, 2.2 m"
    2026-09-29T05:50:33.415Z [INFO] [autopilot] event PedAppeared handle=10017
    2026-09-29T05:50:33.416Z [INFO] [autopilot] event PedAppeared handle=10272
    2026-09-29T05:50:33.416Z [INFO] [autopilot] event PedAppeared handle=10515
    2026-09-29T05:50:33.417Z [INFO] [autopilot] event PedAppeared handle=10778
    2026-09-29T05:50:33.627Z [INFO] [autopilot] event VehicleAppeared handle=4366
    2026-09-29T05:50:33.728Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.874, 640.67, 15.185) to=(-31.57, 738.111, 14.937)
    2026-09-29T05:50:33.729Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-84.324, 715.726, 91.782) to=(-66.548, 675.522, 13.602)
    2026-09-29T05:50:33.961Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.905, 640.662, 15.196) to=(-67.562, 668.919, 14.906)
    2026-09-29T05:50:33.962Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-84.752, 715.742, 91.688) to=(-63.101, 673.472, 13.739)
    2026-09-29T05:50:34.074Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.903, 640.657, 15.188) to=(-67.549, 670.224, 14.717)
    2026-09-29T05:50:34.108Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-85.15, 715.728, 91.606) to=(-63.186, 675.296, 13.731)
    2026-09-29T05:50:34.502Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.095, 641.992, 15.184) to=(-75.204, 655.026, 14.942)
    2026-09-29T05:50:34.580Z [INFO] [autopilot] event PedAppeared handle=8216
    2026-09-29T05:50:34.581Z [INFO] [autopilot] event PedAppeared handle=8478
    2026-09-29T05:50:34.581Z [INFO] [autopilot] event PedAppeared handle=8732
    2026-09-29T05:50:34.655Z [INFO] [autopilot] event PedRemoved handle=9494
    2026-09-29T05:50:34.693Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.128, 641.982, 15.198) to=(295.336, 1348.081, 18.156)
    2026-09-29T05:50:34.869Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.126, 641.978, 15.189) to=(-67.67, 667.765, 15.194)
    2026-09-29T05:50:35.044Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.121, 641.981, 15.201) to=(303.811, 1343.28, -1.685)
    2026-09-29T05:50:35.207Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.135, 641.982, 15.194) to=(-74.968, 654.79, 15.06)
    2026-09-29T05:50:35.441Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.128, 641.982, 15.197) to=(-31.57, 734.568, 15.326)
    2026-09-29T05:50:35.923Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.871, 640.666, 15.186) to=(-75.215, 655.227, 14.953)
    2026-09-29T05:50:36.091Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.905, 640.661, 15.196) to=(-67.322, 669.089, 15.08)
    2026-09-29T05:50:36.155Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-88.224, 714.687, 91.086) to=(-64.602, 675.947, 13.555)
    2026-09-29T05:50:36.248Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.903, 640.656, 15.189) to=(-67.457, 668.99, 14.989)
    2026-09-29T05:50:36.325Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-88.426, 714.538, 91.056) to=(-64.078, 672.698, 13.574)
    2026-09-29T05:50:36.398Z [INFO] camera_already_gone handle=9474 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:50:36.401Z [INFO] command source=file:cmd_20260929055036155.cmd line="cam 180 2.2 0.4" reply="camera at 180 deg, 2.2 m"
    2026-09-29T05:50:36.422Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.9, 640.66, 15.203) to=(302.661, 1341.728, 4.149)
    2026-09-29T05:50:36.609Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-88.642, 714.354, 91.025) to=(-62.619, 672.98, 13.739)
    2026-09-29T05:50:36.632Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.91, 640.66, 15.198) to=(-64.472, 674.792, 15.15)
    2026-09-29T05:50:36.633Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.096, 641.985, 15.19) to=(298.551, 1346.226, 1.466)
    2026-09-29T05:50:36.634Z [INFO] [autopilot] event PedDamaged handle=2 100->80 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=12560 vehicle=0 killed=False hit=True at=(-64.472, 674.792, 15.15) dir=(0.495, 0.869, -0.001)
    2026-09-29T05:50:36.634Z [INFO] [autopilot] event PlayerDamaged 100->80
    2026-09-29T05:50:36.716Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-88.817, 714.182, 91.001) to=(-65.544, 673.49, 13.558)
    2026-09-29T05:50:36.736Z [INFO] [autopilot] event PedAppeared handle=9757
    2026-09-29T05:50:36.755Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.908, 640.657, 15.203) to=(-75.186, 655.119, 14.905)
    2026-09-29T05:50:36.809Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.13, 641.978, 15.202) to=(-62.995, 678.265, 15.137)
    2026-09-29T05:50:36.810Z [INFO] [autopilot] event PedAppeared handle=11029
    2026-09-29T05:50:36.923Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-88.993, 713.977, 90.978) to=(-64.201, 673.051, 13.57)
    2026-09-29T05:50:36.961Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.126, 641.973, 15.192) to=(288.112, 1351.796, 5.019)
    2026-09-29T05:50:37.147Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.12, 641.976, 15.201) to=(-62.897, 678.24, 15.296)
    2026-09-29T05:50:37.301Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.13, 641.981, 15.196) to=(305.85, 1342.167, -2.222)
    2026-09-29T05:50:37.983Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.88, 640.678, 15.189) to=(-67.549, 669.85, 14.577)
    2026-09-29T05:50:38.147Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.906, 640.658, 15.2) to=(328.926, 1326.927, 17.169)
    2026-09-29T05:50:38.299Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.905, 640.653, 15.194) to=(-31.37, 730.847, 15.356)
    2026-09-29T05:50:38.459Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.899, 640.655, 15.203) to=(306.711, 1339.526, 13.169)
    2026-09-29T05:50:38.625Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.914, 640.659, 15.2) to=(-64.499, 674.847, 15.117)
    2026-09-29T05:50:38.626Z [INFO] [autopilot] event PedDamaged handle=2 80->60 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=12560 vehicle=0 killed=False hit=True at=(-64.499, 674.847, 15.117) dir=(0.494, 0.87, -0.002)
    2026-09-29T05:50:38.626Z [INFO] [autopilot] event PlayerDamaged 80->60
    2026-09-29T05:50:38.736Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.908, 711.91, 90.86) to=(-66.265, 675.331, 13.587)
    2026-09-29T05:50:38.741Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:50:38.933Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.951, 711.705, 90.851) to=(-64.45, 673.016, 13.557)
    2026-09-29T05:50:39.282Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-90.001, 711.42, 90.841) to=(-63.331, 675.924, 13.728)
    2026-09-29T05:50:39.376Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.095, 641.985, 15.188) to=(305.199, 1342.699, 11.244)
    2026-09-29T05:50:39.531Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.13, 641.978, 15.201) to=(-75.104, 654.778, 15.008)
    2026-09-29T05:50:39.673Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.129, 641.974, 15.194) to=(-75.041, 654.806, 14.713)
    2026-09-29T05:50:39.701Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.872, 640.666, 15.186) to=(-67.549, 670.312, 14.828)
    2026-09-29T05:50:39.740Z [INFO] [autopilot] event PedRemoved handle=11029
    2026-09-29T05:50:39.772Z [INFO] [autopilot] event PedRemoved handle=9757
    2026-09-29T05:50:39.837Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.123, 641.976, 15.205) to=(-62.855, 678.239, 15.405)
    2026-09-29T05:50:39.876Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.907, 640.657, 15.202) to=(314.299, 1335.022, -2.046)
    2026-09-29T05:50:39.877Z [INFO] [autopilot] event PedAppeared handle=15369
    2026-09-29T05:50:39.878Z [INFO] [autopilot] event PedAppeared handle=15624
    2026-09-29T05:50:39.878Z [INFO] [autopilot] event PedAppeared handle=15879
    2026-09-29T05:50:39.879Z [INFO] [autopilot] event PedAppeared handle=16136
    2026-09-29T05:50:39.961Z [INFO] camera_already_gone handle=9730 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:50:39.963Z [INFO] command source=file:cmd_20260929055039801.cmd line="cam 90 2.0 0.4" reply="camera at 90 deg, 2 m"
    2026-09-29T05:50:39.990Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.134, 641.98, 15.199) to=(-67.656, 667.673, 15.15)
    2026-09-29T05:50:40.075Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.904, 640.653, 15.193) to=(-67.549, 670.259, 14.522)
    2026-09-29T05:50:40.154Z [INFO] [autopilot] event PedAppeared handle=11030
    2026-09-29T05:50:40.182Z [INFO] [autopilot] event PedAppeared handle=11281
    2026-09-29T05:50:40.213Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.899, 640.655, 15.204) to=(-67.549, 670.536, 14.663)
    2026-09-29T05:50:40.477Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-90.081, 710.002, 90.813) to=(-62.444, 674.381, 13.739)
    2026-09-29T05:50:40.641Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-90.074, 709.762, 90.811) to=(-65.39, 673.78, 13.558)
    2026-09-29T05:50:40.819Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-90.063, 709.515, 90.809) to=(-66.64, 674.334, 13.607)
    2026-09-29T05:50:41.602Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.875, 640.669, 15.187) to=(304.932, 1340.585, 23.846)
    2026-09-29T05:50:41.763Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.908, 640.657, 15.203) to=(-67.544, 668.869, 14.931)
    2026-09-29T05:50:41.893Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.905, 640.653, 15.194) to=(321.253, 1331.1, 0.797)
    2026-09-29T05:50:42.033Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.901, 640.656, 15.206) to=(308.384, 1338.63, 17.68)
    2026-09-29T05:50:42.060Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.097, 641.989, 15.187) to=(-31.57, 733.833, 15.108)
    2026-09-29T05:50:42.190Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.907, 640.661, 15.198) to=(-31.57, 733.138, 15.937)
    2026-09-29T05:50:42.214Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.129, 641.978, 15.2) to=(-75.097, 655.434, 15.132)
    2026-09-29T05:50:42.401Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.126, 641.973, 15.191) to=(-62.978, 678.264, 15.243)
    2026-09-29T05:50:42.402Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.879, 707.695, 90.793) to=(-64.671, 674.929, 13.549)
    2026-09-29T05:50:42.553Z [INFO] [autopilot] event PedRemoved handle=11281
    2026-09-29T05:50:42.609Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.85, 707.512, 90.791) to=(-65.879, 674.59, 13.566)
    2026-09-29T05:50:42.866Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.813, 707.289, 90.791) to=(-62.27, 675.011, 13.721)
    2026-09-29T05:50:42.937Z [INFO] [autopilot] event PedRemoved handle=11030
    2026-09-29T05:50:43.068Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.775, 707.076, 90.79) to=(-63.321, 673.276, 13.709)
    2026-09-29T05:50:43.279Z [INFO] [autopilot] event PedAppeared handle=11282
    2026-09-29T05:50:43.485Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.098, 641.987, 15.189) to=(-67.522, 670.156, 14.946)
    2026-09-29T05:50:43.555Z [INFO] camera_already_gone handle=9986 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:50:43.556Z [INFO] command source=file:cmd_20260929055043433.cmd line="cam 270 2.0 0.4" reply="camera at 270 deg, 2 m"
    2026-09-29T05:50:43.959Z [INFO] [autopilot] event PedRemoved handle=11282
    2026-09-29T05:50:44.074Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-83.191, 642.311, 15.122) to=(316.71, 1335.991, 25.603)
    2026-09-29T05:50:44.074Z [INFO] [autopilot] event BulletFired shooter=10515 weapon=13 by_player=False from=(-50.224, 692.324, 15.135) to=(-51.37, 690.934, 15.036)
    2026-09-29T05:50:44.075Z [INFO] [autopilot] event PedDamaged handle=2 60->49 bone=0x1A2 by_player=False weapon=13 exact=True type=Bullet amount=11.1 health_lost=11.1 armour_lost=0.0 attacker=10515 vehicle=0 killed=False hit=False
    2026-09-29T05:50:44.076Z [INFO] [autopilot] event PlayerDamaged 60->49
    2026-09-29T05:50:44.079Z [INFO] [autopilot] event VehicleDamaged handle=11274 1000->995 engine 1000->1000
    2026-09-29T05:50:44.225Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.911, 643.051, 15.124) to=(-67.549, 669.148, 14.673)
    2026-09-29T05:50:44.336Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-50.296, 691.201, 15.225) to=(-95.478, 640.666, 14.899)
    2026-09-29T05:50:44.505Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-50.259, 691.22, 15.237) to=(-559.969, 74.044, -8.644)
    2026-09-29T05:50:44.561Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.097, 641.994, 15.181) to=(288.199, 1351.903, 15.25)
    2026-09-29T05:50:44.656Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-50.259, 691.236, 15.227) to=(-67.077, 672.348, 14.68)
    2026-09-29T05:50:44.712Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.13, 641.982, 15.199) to=(-75.171, 655.153, 14.832)
    2026-09-29T05:50:44.830Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-50.265, 691.24, 15.238) to=(-64.259, 674.85, 14.429)
    2026-09-29T05:50:44.831Z [INFO] [autopilot] event PedDamaged handle=2 49->34 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=10017 vehicle=0 killed=False hit=True at=(-64.259, 674.85, 14.429) dir=(-0.649, -0.76, -0.038)
    2026-09-29T05:50:44.831Z [INFO] [autopilot] event PlayerDamaged 49->34
    2026-09-29T05:50:44.958Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.466, 705.556, 90.787) to=(-65.243, 674.436, 13.558)
    2026-09-29T05:50:44.982Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-50.252, 691.232, 15.232) to=(-67.077, 672.675, 14.795)
    2026-09-29T05:50:45.009Z [INFO] [autopilot] event BulletFired shooter=10515 weapon=13 by_player=False from=(-50.222, 692.356, 15.133) to=(-178.897, 538.564, 10.568)
    2026-09-29T05:50:45.139Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-50.259, 691.228, 15.238) to=(-64.525, 674.971, 14.647)
    2026-09-29T05:50:45.140Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.436, 705.415, 90.786) to=(-63.211, 673.014, 13.727)
    2026-09-29T05:50:45.141Z [INFO] [autopilot] event PedDamaged handle=2 34->14 bone=0x4B2 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=10017 vehicle=0 killed=False hit=True at=(-64.525, 674.971, 14.647) dir=(-0.659, -0.751, -0.027)
    2026-09-29T05:50:45.142Z [INFO] [autopilot] event PlayerDamaged 34->14
    2026-09-29T05:50:45.337Z [INFO] [autopilot] event BulletFired shooter=7697 weapon=15 by_player=False from=(-89.405, 705.264, 90.785) to=(-64.271, 672.06, 13.566)
    2026-09-29T05:50:45.668Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.26, 644.07, 15.183) to=(324.52, 1333.629, 10.671)
    2026-09-29T05:50:45.701Z [INFO] [autopilot] event BulletFired shooter=12045 weapon=15 by_player=False from=(-82.097, 641.991, 15.184) to=(304.916, 1342.895, 20.645)
    2026-09-29T05:50:45.769Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.957, 694.354, 15.161) to=(-64.181, 674.719, 14.501)
    2026-09-29T05:50:45.770Z [INFO] [autopilot] event PedDamaged handle=2 15->-100 bone=0x4C9 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=114.6 armour_lost=0.0 attacker=10778 vehicle=0 killed=True hit=True at=(-64.181, 674.719, 14.501) dir=(-0.496, -0.868, -0.029)
    2026-09-29T05:50:45.771Z [INFO] [autopilot] event PedDied handle=2 bone=0x4C9 by_player=False exact=True type=Bullet killer=10778 weapon=15
    2026-09-29T05:50:45.772Z [INFO] [autopilot] event PlayerWeaponChanged 7->0
    2026-09-29T05:50:45.773Z [INFO] [autopilot] event PlayerDied
    2026-09-29T05:50:45.773Z [INFO] [autopilot] event PlayerDamaged 14->-100
    2026-09-29T05:50:45.782Z [INFO] holsters_removed reason=wasted
    2026-09-29T05:50:45.783Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored
    2026-09-29T05:50:45.783Z [INFO] arsenal_loss reason=wasted id=7 owned=False
    2026-09-29T05:50:45.784Z [INFO] arsenal_loss reason=wasted id=14 owned=False
    2026-09-29T05:50:46.290Z [INFO] [autopilot] event VehicleAppeared handle=4110
    2026-09-29T05:50:46.366Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:50:46.367Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:50:46.425Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:50:46.486Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4C8 by_player=False weapon=54 exact=True type=Fall amount=1.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:50:46.593Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.254, 644.054, 15.171) to=(-75.993, 655.025, 14.755)
    2026-09-29T05:50:46.758Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:50:46.786Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.281, 644.047, 15.19) to=(328.427, 1330.026, -27.001)
    2026-09-29T05:50:46.928Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.28, 644.041, 15.183) to=(317.296, 1336.838, -23.851)
    2026-09-29T05:50:46.929Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.991, 694.814, 15.235) to=(-67.048, 669.607, 13.884)
    2026-09-29T05:50:46.979Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x36A1 by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:50:47.031Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.977, 691.25, 15.211) to=(-562.019, 78.646, -44.145)
    2026-09-29T05:50:47.059Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.954, 694.818, 15.245) to=(-434.48, -6.255, -48.069)
    2026-09-29T05:50:47.107Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.276, 644.045, 15.191) to=(-75.497, 654.981, 14.721)
    2026-09-29T05:50:47.112Z [INFO] camera_already_gone handle=10242 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:50:47.114Z [INFO] command source=file:cmd_20260929055047076.cmd line="cam 150 1.4 0.6" reply="camera at 150 deg, 1.4 m"
    2026-09-29T05:50:47.208Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.946, 691.25, 15.223) to=(-66.958, 670.577, 13.707)
    2026-09-29T05:50:47.233Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.285, 644.047, 15.185) to=(328.309, 1331.083, -6.491)
    2026-09-29T05:50:47.234Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.961, 694.823, 15.235) to=(-62.688, 676.148, 13.721)
    2026-09-29T05:50:47.392Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.28, 644.041, 15.187) to=(-75.872, 655.066, 14.786)
    2026-09-29T05:50:47.393Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.968, 694.825, 15.243) to=(-63.136, 675.916, 13.729)
    2026-09-29T05:50:47.394Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.947, 691.26, 15.213) to=(-66.081, 670.73, 13.577)
    2026-09-29T05:50:47.558Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.955, 694.828, 15.235) to=(-450.701, 2.041, -40.985)
    2026-09-29T05:50:47.559Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.95, 691.267, 15.219) to=(-62.355, 676.265, 13.697)
    2026-09-29T05:50:47.626Z [INFO] [autopilot] event BulletFired shooter=10272 weapon=10 by_player=False from=(-52.881, 691.357, 15.147) to=(-66.767, 670.019, 13.674)
    2026-09-29T05:50:47.717Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.956, 694.833, 15.239) to=(-62.065, 677.649, 13.719)
    2026-09-29T05:50:47.718Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.94, 691.262, 15.214) to=(-569.529, 83.383, -26.42)
    2026-09-29T05:50:48.532Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.249, 644.057, 15.172) to=(325.737, 1332.377, -12.129)
    2026-09-29T05:50:48.533Z [INFO] [autopilot] event VehicleRemoved handle=9484
    2026-09-29T05:50:48.688Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.28, 644.047, 15.186) to=(-67.549, 669.4, 14.502)
    2026-09-29T05:50:48.718Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.99, 694.808, 15.227) to=(-443.086, -1.63, -48.328)
    2026-09-29T05:50:48.719Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.98, 691.248, 15.206) to=(-61.759, 676.916, 13.669)
    2026-09-29T05:50:48.751Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:50:48.873Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.278, 644.038, 15.178) to=(-75.509, 655.017, 14.441)
    2026-09-29T05:50:48.874Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.958, 694.822, 15.244) to=(-63.675, 674.282, 13.757)
    2026-09-29T05:50:48.875Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.947, 691.253, 15.221) to=(-551.929, 68.468, -22.201)
    2026-09-29T05:50:48.875Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A4 by_player=False weapon=15 exact=True type=Bullet amount=9.9 health_lost=0.0 armour_lost=0.0 attacker=10778 vehicle=0 killed=False hit=True at=(-63.675, 674.282, 13.757) dir=(-0.462, -0.885, -0.064)
    2026-09-29T05:50:49.041Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.273, 644.04, 15.192) to=(-75.493, 655.017, 14.505)
    2026-09-29T05:50:49.042Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.962, 694.828, 15.235) to=(-67.048, 668.695, 13.726)
    2026-09-29T05:50:49.043Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.949, 691.261, 15.214) to=(-66.937, 670.368, 13.703)
    2026-09-29T05:50:49.179Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.287, 644.041, 15.185) to=(-67.549, 668.446, 14.348)
    2026-09-29T05:50:49.180Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.967, 694.825, 15.247) to=(-66.676, 669.933, 13.641)
    2026-09-29T05:50:49.181Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.954, 691.258, 15.227) to=(-565.265, 81.134, -41.49)
    2026-09-29T05:50:49.340Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.282, 644.039, 15.19) to=(-67.549, 668.996, 14.617)
    2026-09-29T05:50:49.341Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.953, 694.824, 15.24) to=(-67.077, 669.497, 14.081)
    2026-09-29T05:50:49.342Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.942, 691.255, 15.219) to=(-63.83, 674.15, 13.714)
    2026-09-29T05:50:49.342Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A4 by_player=False weapon=15 exact=True type=Bullet amount=9.9 health_lost=0.0 armour_lost=0.0 attacker=10017 vehicle=0 killed=False hit=True at=(-63.83, 674.15, 13.714) dir=(-0.629, -0.775, -0.068)
    2026-09-29T05:50:49.506Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.957, 694.828, 15.243) to=(-436.84, -4.489, -54.59)
    2026-09-29T05:50:49.507Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.944, 691.259, 15.221) to=(-555.344, 73.597, -49.082)
    2026-09-29T05:50:49.586Z [INFO] [autopilot] event VehicleRemoved handle=13323
    2026-09-29T05:50:49.739Z [INFO] [autopilot] event PedRemoved handle=8732
    2026-09-29T05:50:49.740Z [INFO] [autopilot] event PedRemoved handle=8478
    2026-09-29T05:50:49.741Z [INFO] [autopilot] event PedRemoved handle=8216
    2026-09-29T05:50:49.844Z [INFO] [autopilot] event BulletFired shooter=10272 weapon=10 by_player=False from=(-52.889, 691.341, 15.145) to=(-107.179, 607.034, 8.13)
    2026-09-29T05:50:50.302Z [INFO] performance samples=1142 frame_p50_ms=22 frame_p95_ms=43 frame_p99_ms=80 frames_over_33ms=175 frames_over_50ms=39 gunplay_avg_ms=1.192 gunplay_max_ms=8.014 phase_samples=1005 phase_setup_avg_ms=0.902 phase_setup_max_ms=7.424 phase_camera_avg_ms=0.023 phase_camera_max_ms=0.862 phase_bullets_avg_ms=0.020 phase_bullets_max_ms=0.326 phase_weapon_avg_ms=0.019 phase_weapon_max_ms=0.130 phase_hud_avg_ms=0.387 phase_hud_max_ms=0.635
    2026-09-29T05:50:50.303Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.354/30.0/1142@7 total=2688 module.gunplay=1.199/8.0/1142@7 total=1369 tick.gunplay=1.197/8.0/1142@7 total=1367 gp.freeaim=0.700/7.2/1005@7 total=703 module.arsenal=0.610/23.5/747@7 total=455 tick.arsenal=0.609/23.5/747@7 total=455 engine.world=0.303/7.7/1142@7 total=346 ar.storage=0.421/8.1/635@7 total=267 module.atmosphere=0.213/5.6/1142@7 total=243 tick.atmosphere=0.212/5.6/1142@7 total=242 gp.index_pad=0.139/0.7/1005@7 total=140 ar.safehouse_flags=0.189/9.8/635@7 total=120 module.combat=0.088/3.6/1142@7 total=100 tick.combat=0.087/3.6/1142@7 total=99 combat.sample=1.210/3.6/60@7 total=73 module.holsters=0.060/0.7/409@7 total=25 tick.holsters=0.059/0.7/409@7 total=24 module.devtools=0.024/0.1/747@7 total=18 tick.devtools=0.023/0.1/747@7 total=17 cam.handle=0.016/0.9/1005@7 total=16 gp.shoulder=0.016/0.1/1005@7 total=16 gp.player=0.013/0.2/1005@7 total=13 gp.weapon_id=0.013/0.2/1005@7 total=13 module.world=0.176/1.4/57@7 total=10 gp.cycle=0.009/0.1/1005@7 total=9 gp.state=0.007/0.0/1005@7 total=7 module.probe=1.448/1.7/3@7 total=4 ho.show=0.012/0.0/347@7 total=4 cam.aim_key=0.003/0.0/1005@7 total=3 cam.find_active=0.003/0.1/1005@7 total=3 ar.vehicle=0.003/0.0/747@7 total=2 ar.lvs=0.003/0.4/747@7 total=2 gp.spread=0.002/0.0/1005@7 total=2 ar.reconcile=0.003/0.0/635@7 total=2 ar.discover=0.002/0.6/747@7 total=2 gp.shots=0.002/0.0/1005@7 total=2 combat.dismember=0.001/0.0/1142@7 total=1 engine.scheduler=0.001/0.0/1142@7 total=1 ho.carried=0.002/0.0/347@7 total=1 module.autopilot=0.000/0.0/1142@7 total=0 combat.blood=0.000/0.0/1142@7 total=0 module.weapon-probe=0.000/0.0/1142@7 total=0 gp.feel=0.000/0.0/1005@7 total=0 combat.pending=0.000/0.0/1142@7 total=0 gp.recoil=0.000/0.0/1005@7 total=0 cam.fov=0.001/0.0/94@7 total=0
    2026-09-29T05:50:50.304Z [INFO] engine_thread_probe ticks=1142 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1142 ticks_after_skipped_frames=0
    2026-09-29T05:50:50.306Z [INFO] direct_native get_char_health direct=0 shdn=-100 match=False direct_us=0.28 shdn_us=64.6
    2026-09-29T05:50:50.308Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=21578 core=on peds=14 vehicles=16 modules=10/10 coroutines=0 resources=5 raycast=on episode=GTAIV frame_ms=36.16 p95_ms=48.28 pressure=0.07 private_mb=2285 working_set_mb=1838 address_free_mb=1074 largest_free_block_mb=1025 managed_mb=14 physical_load=85% core_us=66.7
    2026-09-29T05:50:50.343Z [INFO] density frame_ms=38.1 peds=0.75 cars=0.77
    2026-09-29T05:50:50.523Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.986, 694.815, 15.225) to=(-439.344, -3.508, -50.114)
    2026-09-29T05:50:50.682Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.956, 694.823, 15.241) to=(-427.988, -10.813, -33.924)
    2026-09-29T05:50:50.775Z [INFO] camera_already_gone handle=10498 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:50:50.775Z [INFO] command source=file:cmd_20260929055050660.cmd line="cam off" reply="camera off"
    2026-09-29T05:50:50.836Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.253, 644.037, 15.189) to=(330.383, 1328.785, -27.815)
    2026-09-29T05:50:50.837Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.958, 694.826, 15.234) to=(-62.509, 677.26, 13.723)
    2026-09-29T05:50:50.973Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.965, 694.82, 15.247) to=(-441.624, -2.956, -40.11)
    2026-09-29T05:50:50.974Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.98, 691.251, 15.212) to=(-552.681, 70.756, -42.009)
    2026-09-29T05:50:51.003Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.286, 644.034, 15.206) to=(-75.638, 655.017, 14.462)
    2026-09-29T05:50:51.129Z [INFO] [autopilot] event BulletFired shooter=10778 weapon=15 by_player=False from=(-52.954, 694.817, 15.241) to=(-435.746, -7.691, -18.827)
    2026-09-29T05:50:51.130Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.947, 691.249, 15.225) to=(-556.961, 73.426, -31.735)
    2026-09-29T05:50:51.159Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.285, 644.029, 15.197) to=(-67.549, 669.727, 14.314)
    2026-09-29T05:50:51.280Z [INFO] command source=file:cmd_20260929055051050.cmd line="hud on" reply="hud on"
    2026-09-29T05:50:51.302Z [INFO] [autopilot] event BulletFired shooter=12560 weapon=15 by_player=False from=(-82.281, 644.032, 15.205) to=(-75.186, 655.467, 14.888)
    2026-09-29T05:50:51.303Z [INFO] [autopilot] event BulletFired shooter=10017 weapon=15 by_player=False from=(-49.945, 691.254, 15.213) to=(-67.048, 670.501, 13.742)

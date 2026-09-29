# Scenario bullet-events

- Result: PASS
- Steps: 13, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto gun_test_range => Teleporting to Gun Test Range (Broker)
- wait 5000 ms
- time 12 0 => time 12:00
- spawn 2 8 7 => spawning 2 at 8 m
- wait 3000 ms
- expect autopilot_spawned count=2: OK 2026-09-29T05:44:30.399Z [INFO] [autopilot] autopilot_spawned count=2
- fight 0 1 => fight 5132 vs 5385
- expect event BulletFired shooter=[1-9]\d* weapon=\d+: OK 2026-09-29T05:44:37.156Z [INFO] [autopilot] event BulletFired shooter=5132 weapon=7 by_player=False from=(1045.179, -560.803, 24.818) to=(1037.167, -534.065, 35.577)
- wait 3000 ms
- clear => cleared 2

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:44:22.182Z [INFO] [autopilot] event VehicleRemoved handle=11269
    2026-09-29T05:44:22.873Z [INFO] [autopilot] event VehicleAppeared handle=11781
    2026-09-29T05:44:22.983Z [INFO] [autopilot] event PedAppeared handle=13058
    2026-09-29T05:44:22.984Z [INFO] [autopilot] event PedAppeared handle=13314
    2026-09-29T05:44:22.985Z [INFO] [autopilot] event PedAppeared handle=13570
    2026-09-29T05:44:22.985Z [INFO] [autopilot] event PedRemoved handle=11270
    2026-09-29T05:44:23.041Z [INFO] command source=file:cmd_20260929054422829.cmd line="events on" reply="event log on"
    2026-09-29T05:44:23.068Z [INFO] [autopilot] event VehicleAppeared handle=9991
    2026-09-29T05:44:23.100Z [INFO] [autopilot] event PedAppeared handle=13826
    2026-09-29T05:44:23.251Z [INFO] [autopilot] event VehicleAppeared handle=6662
    2026-09-29T05:44:23.285Z [INFO] command source=file:cmd_20260929054423253.cmd line="god on" reply="invincible True"
    2026-09-29T05:44:23.378Z [INFO] [autopilot] event PedAppeared handle=14082
    2026-09-29T05:44:23.447Z [INFO] [autopilot] event VehicleAppeared handle=5127
    2026-09-29T05:44:23.678Z [INFO] [autopilot] event VehicleAppeared handle=3591
    2026-09-29T05:44:23.801Z [INFO] command source=file:cmd_20260929054423659.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:44:23.926Z [INFO] [autopilot] event VehicleAppeared handle=12036
    2026-09-29T05:44:24.046Z [INFO] [autopilot] event PedAppeared handle=14338
    2026-09-29T05:44:24.047Z [INFO] [autopilot] event PedRemoved handle=13570
    2026-09-29T05:44:24.566Z [INFO] teleport_start id=gun_test_range target=1041,-568.3,20 snap=pavement
    2026-09-29T05:44:24.567Z [INFO] command source=file:cmd_20260929054424078.cmd line="goto gun_test_range" reply="Teleporting to Gun Test Range (Broker)"
    2026-09-29T05:44:24.583Z [INFO] [autopilot] event PedAppeared handle=9996
    2026-09-29T05:44:24.584Z [INFO] [autopilot] event PedRemoved handle=9744
    2026-09-29T05:44:24.585Z [INFO] [autopilot] event VehicleAppeared handle=10502
    2026-09-29T05:44:24.585Z [INFO] [autopilot] event VehicleRemoved handle=5127
    2026-09-29T05:44:24.722Z [INFO] [autopilot] event PedRemoved handle=12547
    2026-09-29T05:44:24.723Z [INFO] [autopilot] event PedRemoved handle=10763
    2026-09-29T05:44:24.723Z [INFO] [autopilot] event PedRemoved handle=11016
    2026-09-29T05:44:24.790Z [INFO] [autopilot] event PedAppeared handle=10253
    2026-09-29T05:44:24.791Z [INFO] [autopilot] event PedRemoved handle=14338
    2026-09-29T05:44:24.791Z [INFO] [autopilot] event VehicleAppeared handle=5127
    2026-09-29T05:44:25.143Z [INFO] [autopilot] event PedAppeared handle=10764
    2026-09-29T05:44:25.232Z [INFO] [autopilot] event PedAppeared handle=11017
    2026-09-29T05:44:25.277Z [INFO] [autopilot] event PedAppeared handle=11271
    2026-09-29T05:44:25.489Z [INFO] [autopilot] event VehicleAppeared handle=10758
    2026-09-29T05:44:25.499Z [INFO] teleport_done id=gun_test_range final=1045.1,-563.0,25.6
    2026-09-29T05:44:25.535Z [INFO] [autopilot] event PedAppeared handle=10254
    2026-09-29T05:44:25.536Z [INFO] [autopilot] event PedAppeared handle=11527
    2026-09-29T05:44:25.537Z [INFO] [autopilot] event PedAppeared handle=12548
    2026-09-29T05:44:25.537Z [INFO] [autopilot] event PedRemoved handle=11526
    2026-09-29T05:44:25.538Z [INFO] [autopilot] event PedRemoved handle=9996
    2026-09-29T05:44:25.539Z [INFO] [autopilot] event PedRemoved handle=10253
    2026-09-29T05:44:25.540Z [INFO] [autopilot] event VehicleAppeared handle=12292
    2026-09-29T05:44:25.540Z [INFO] [autopilot] event VehicleRemoved handle=10758
    2026-09-29T05:44:25.681Z [INFO] [autopilot] event VehicleAppeared handle=10758
    2026-09-29T05:44:26.266Z [INFO] [autopilot] event PedAppeared handle=13571
    2026-09-29T05:44:26.267Z [INFO] [autopilot] event PedRemoved handle=12548
    2026-09-29T05:44:26.759Z [INFO] [autopilot] event VehicleAppeared handle=2822
    2026-09-29T05:44:27.195Z [INFO] [autopilot] event PedAppeared handle=14339
    2026-09-29T05:44:27.195Z [INFO] [autopilot] event PedRemoved handle=13571
    2026-09-29T05:44:27.633Z [INFO] [autopilot] event VehicleAppeared handle=8199
    2026-09-29T05:44:28.313Z [INFO] [autopilot] event VehicleRemoved handle=2822
    2026-09-29T05:44:28.587Z [INFO] [autopilot] event PedRemoved handle=14082
    2026-09-29T05:44:28.678Z [INFO] [autopilot] event PedRemoved handle=13058
    2026-09-29T05:44:28.679Z [INFO] [autopilot] event PedRemoved handle=13314
    2026-09-29T05:44:28.865Z [INFO] [autopilot] event PedRemoved handle=14339
    2026-09-29T05:44:29.228Z [INFO] [autopilot] event PedAppeared handle=13315
    2026-09-29T05:44:29.419Z [INFO] [autopilot] event VehicleAppeared handle=9735
    2026-09-29T05:44:29.783Z [INFO] command source=file:cmd_20260929054429749.cmd line="time 12 0" reply="time 12:00"
    2026-09-29T05:44:30.000Z [INFO] [autopilot] event PedRemoved handle=4872
    2026-09-29T05:44:30.199Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:44:30.236Z [INFO] [autopilot] event VehicleAppeared handle=3846
    2026-09-29T05:44:30.256Z [INFO] [autopilot] event PedRemoved handle=5131
    2026-09-29T05:44:30.298Z [INFO] command source=file:cmd_20260929054430144.cmd line="spawn 2 8 7" reply="spawning 2 at 8 m"
    2026-09-29T05:44:30.343Z [INFO] [autopilot] event PedAppeared handle=5132
    2026-09-29T05:44:30.343Z [INFO] [autopilot] event PedRemoved handle=11780
    2026-09-29T05:44:30.388Z [INFO] [autopilot] event PedAppeared handle=5385
    2026-09-29T05:44:30.389Z [INFO] [autopilot] event PedAppeared handle=5646
    2026-09-29T05:44:30.390Z [INFO] [autopilot] event PedRemoved handle=5645
    2026-09-29T05:44:30.390Z [INFO] [autopilot] event VehicleRemoved handle=3336
    2026-09-29T05:44:30.399Z [INFO] [autopilot] autopilot_spawned count=2
    2026-09-29T05:44:30.583Z [INFO] [autopilot] event VehicleAppeared handle=5638
    2026-09-29T05:44:30.771Z [INFO] [autopilot] event PedAppeared handle=6156
    2026-09-29T05:44:30.772Z [INFO] [autopilot] event PedRemoved handle=7432
    2026-09-29T05:44:30.957Z [INFO] [autopilot] event VehicleRemoved handle=5638
    2026-09-29T05:44:31.000Z [INFO] [autopilot] event VehicleAppeared handle=14596
    2026-09-29T05:44:31.097Z [INFO] [autopilot] event VehicleAppeared handle=4870
    2026-09-29T05:44:31.251Z [INFO] [autopilot] event VehicleRemoved handle=11525
    2026-09-29T05:44:31.357Z [INFO] [autopilot] event PedAppeared handle=7433
    2026-09-29T05:44:32.115Z [INFO] [autopilot] event PedAppeared handle=7948
    2026-09-29T05:44:32.116Z [INFO] [autopilot] event PedRemoved handle=7947
    2026-09-29T05:44:32.257Z [INFO] [autopilot] event PedAppeared handle=8209
    2026-09-29T05:44:32.258Z [INFO] [autopilot] event PedAppeared handle=8723
    2026-09-29T05:44:32.258Z [INFO] [autopilot] event PedAppeared handle=9745
    2026-09-29T05:44:32.530Z [INFO] [autopilot] event PedRemoved handle=7433
    2026-09-29T05:44:32.738Z [INFO] [autopilot] event VehicleAppeared handle=7433
    2026-09-29T05:44:32.924Z [INFO] [autopilot] event PedAppeared handle=9997
    2026-09-29T05:44:33.151Z [INFO] [autopilot] event VehicleRemoved handle=4870
    2026-09-29T05:44:33.520Z [INFO] [autopilot] event PedAppeared handle=11781
    2026-09-29T05:44:33.521Z [INFO] [autopilot] event PedRemoved handle=9997
    2026-09-29T05:44:33.631Z [INFO] command source=file:cmd_20260929054433586.cmd line="fight 0 1" reply="fight 5132 vs 5385"
    2026-09-29T05:44:34.768Z [INFO] [autopilot] event PedAppeared handle=12549
    2026-09-29T05:44:34.769Z [INFO] [autopilot] event PedAppeared handle=13059
    2026-09-29T05:44:34.770Z [INFO] [autopilot] event PedAppeared handle=13572
    2026-09-29T05:44:35.008Z [INFO] [autopilot] event VehicleAppeared handle=13828
    2026-09-29T05:44:35.320Z [INFO] [autopilot] event PedAppeared handle=14083
    2026-09-29T05:44:35.321Z [INFO] [autopilot] event PedAppeared handle=14340
    2026-09-29T05:44:35.596Z [INFO] [autopilot] event PedAppeared handle=14594
    2026-09-29T05:44:35.597Z [INFO] [autopilot] event PedRemoved handle=11781
    2026-09-29T05:44:35.598Z [INFO] [autopilot] event VehicleAppeared handle=8711
    2026-09-29T05:44:36.467Z [INFO] [autopilot] event VehicleRemoved handle=8711
    2026-09-29T05:44:37.064Z [INFO] [autopilot] event PedAppeared handle=9998
    2026-09-29T05:44:37.064Z [INFO] [autopilot] event PedRemoved handle=9485
    2026-09-29T05:44:37.156Z [INFO] [autopilot] event BulletFired shooter=5132 weapon=7 by_player=False from=(1045.179, -560.803, 24.818) to=(1037.167, -534.065, 35.577)
    2026-09-29T05:44:37.440Z [INFO] [autopilot] event VehicleAppeared handle=12805
    2026-09-29T05:44:37.528Z [INFO] [autopilot] event BulletFired shooter=5132 weapon=7 by_player=False from=(1044.601, -560.389, 24.96) to=(1044.409, -560.457, 24.934)
    2026-09-29T05:44:37.658Z [INFO] [autopilot] event PedAppeared handle=11782
    2026-09-29T05:44:37.659Z [INFO] [autopilot] event PedRemoved handle=9998
    2026-09-29T05:44:37.681Z [INFO] [autopilot] event VehicleAppeared handle=14340
    2026-09-29T05:44:37.860Z [INFO] [autopilot] event BulletFired shooter=5132 weapon=7 by_player=False from=(1044.549, -560.104, 24.995) to=(1044.329, -560.121, 24.981)
    2026-09-29T05:44:37.861Z [INFO] [autopilot] event PedAppeared handle=14850
    2026-09-29T05:44:38.133Z [INFO] [autopilot] event VehicleRemoved handle=5381
    2026-09-29T05:44:38.194Z [INFO] [autopilot] event BulletFired shooter=5132 weapon=7 by_player=False from=(1044.551, -560.123, 24.767) to=(1044.487, -560.183, 24.69)
    2026-09-29T05:44:38.240Z [INFO] [autopilot] event PedRemoved handle=14850
    2026-09-29T05:44:38.429Z [INFO] [autopilot] event VehicleAppeared handle=6407
    2026-09-29T05:44:38.520Z [INFO] [autopilot] event BulletFired shooter=5132 weapon=7 by_player=False from=(1044.559, -560.146, 24.733) to=(1044.563, -560.199, 24.688)
    2026-09-29T05:44:38.623Z [INFO] [autopilot] event PedAppeared handle=14851
    2026-09-29T05:44:38.623Z [INFO] [autopilot] event PedRemoved handle=14594
    2026-09-29T05:44:38.624Z [INFO] [autopilot] event VehicleRemoved handle=14340
    2026-09-29T05:44:39.195Z [INFO] [autopilot] event VehicleRemoved handle=13060
    2026-09-29T05:44:39.270Z [INFO] [autopilot] event PedAppeared handle=15106
    2026-09-29T05:44:39.271Z [INFO] [autopilot] event PedRemoved handle=14851
    2026-09-29T05:44:39.798Z [INFO] [autopilot] event PedAppeared handle=15362
    2026-09-29T05:44:39.799Z [INFO] [autopilot] event PedRemoved handle=15106
    2026-09-29T05:44:39.844Z [INFO] [autopilot] event PedRemoved handle=11782
    2026-09-29T05:44:40.006Z [INFO] [autopilot] event BulletFired shooter=5385 weapon=7 by_player=False from=(1044.839, -559.72, 25.138) to=(1050.72, -532.865, 24.751)
    2026-09-29T05:44:40.207Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:44:40.283Z [INFO] [autopilot] event PedAppeared handle=14595
    2026-09-29T05:44:40.604Z [INFO] command source=file:cmd_20260929054440586.cmd line="clear" reply="cleared 2"
    2026-09-29T05:44:40.645Z [INFO] [autopilot] event PedAppeared handle=5386
    2026-09-29T05:44:40.646Z [INFO] [autopilot] event PedAppeared handle=7434
    2026-09-29T05:44:40.646Z [INFO] [autopilot] event PedRemoved handle=5132
    2026-09-29T05:44:40.647Z [INFO] [autopilot] event PedRemoved handle=5385

# Scenario asset-review

- Result: NEEDS-REVIEW
- Steps: 33, failed: 0
- Game alive at end: True
- Log errors during run: 1

## Failed steps

## Steps
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 5000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- spawnprop lf_test_crate 2.5 0 => spawning prop lf_test_crate
- expect autopilot_prop handle=\d+ model=lf_test_crate: OK 2026-09-29T05:43:36.286Z [INFO] [autopilot] autopilot_prop handle=12808 model=lf_test_crate at=(-64.419, 677.334, 13.568)
- wait 1500 ms
- hud off => hud off
- cam prop 0 2.2 0.6 => camera at 0 deg, 2.2 m
- wait 1500 ms
- shot crate_front -> crate_front.jpg
- cam prop 90 2.2 0.9 => camera at 90 deg, 2.2 m
- wait 1500 ms
- shot crate_side -> crate_side.jpg
- cam prop 200 1.6 1.4 => camera at 200 deg, 1.6 m
- wait 1500 ms
- shot crate_top -> crate_top.jpg
- cam off => camera off
- clear => cleared 1
- spawnprop lf_blender_barrel 2.5 0 => spawning prop lf_blender_barrel
- expect autopilot_prop handle=\d+ model=lf_blender_barrel: OK 2026-09-29T05:43:50.293Z [INFO] [autopilot] autopilot_prop handle=11793 model=lf_blender_barrel at=(-68.048, 675.773, 13.731)
- wait 1500 ms
- cam prop 20 2.4 0.8 => camera at 20 deg, 2.4 m
- wait 1500 ms
- shot barrel_front -> barrel_front.jpg
- cam prop 150 1.8 1.6 => camera at 150 deg, 1.8 m
- wait 1500 ms
- shot barrel_top -> barrel_top.jpg
- cam off => camera off
- hud on => hud on
- clear => cleared 1

## Errors
    2026-09-29T05:43:47.888Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:43:28.065Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-30.851, 695.155, 14.57) to=(-87.262, 662.719, 14.285)
    2026-09-29T05:43:28.392Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.213, 695.682, 14.669) to=(-31.374, 695.584, 14.739)
    2026-09-29T05:43:28.393Z [INFO] [autopilot] event VehicleDamaged handle=11013 1000->994 engine 1000->1000
    2026-09-29T05:43:28.542Z [INFO] command source=file:cmd_20260929054328500.cmd line="god on" reply="invincible True"
    2026-09-29T05:43:29.062Z [INFO] command source=file:cmd_20260929054328928.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:43:29.123Z [INFO] [autopilot] event PedAppeared handle=5129
    2026-09-29T05:43:29.124Z [INFO] [autopilot] event PedAppeared handle=5897
    2026-09-29T05:43:29.125Z [INFO] [autopilot] event PedAppeared handle=6411
    2026-09-29T05:43:29.125Z [INFO] [autopilot] event PedAppeared handle=6666
    2026-09-29T05:43:29.126Z [INFO] [autopilot] event PedAppeared handle=8202
    2026-09-29T05:43:29.127Z [INFO] [autopilot] event PedAppeared handle=8461
    2026-09-29T05:43:29.127Z [INFO] [autopilot] event PedAppeared handle=8971
    2026-09-29T05:43:29.128Z [INFO] [autopilot] event PedAppeared handle=9229
    2026-09-29T05:43:29.129Z [INFO] [autopilot] event PedAppeared handle=9481
    2026-09-29T05:43:29.129Z [INFO] [autopilot] event PedAppeared handle=9740
    2026-09-29T05:43:29.130Z [INFO] [autopilot] event PedAppeared handle=9992
    2026-09-29T05:43:29.131Z [INFO] [autopilot] event PedAppeared handle=10248
    2026-09-29T05:43:29.132Z [INFO] [autopilot] event PedAppeared handle=10502
    2026-09-29T05:43:29.132Z [INFO] [autopilot] event PedRemoved handle=6665
    2026-09-29T05:43:29.133Z [INFO] [autopilot] event PedRemoved handle=6410
    2026-09-29T05:43:29.134Z [INFO] [autopilot] event PedRemoved handle=5896
    2026-09-29T05:43:29.210Z [INFO] [autopilot] event VehicleAppeared handle=2308
    2026-09-29T05:43:29.230Z [INFO] [autopilot] event VehicleAppeared handle=2820
    2026-09-29T05:43:29.515Z [INFO] [autopilot] event PedRemoved handle=10248
    2026-09-29T05:43:29.516Z [INFO] [autopilot] event PedRemoved handle=9740
    2026-09-29T05:43:29.517Z [INFO] [autopilot] event PedRemoved handle=6666
    2026-09-29T05:43:29.518Z [INFO] [autopilot] event PedRemoved handle=8971
    2026-09-29T05:43:29.565Z [INFO] [autopilot] event PedRemoved handle=5129
    2026-09-29T05:43:29.566Z [INFO] [autopilot] event PedRemoved handle=9992
    2026-09-29T05:43:29.864Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:43:29.865Z [INFO] command source=file:cmd_20260929054329318.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:43:29.885Z [INFO] [autopilot] event PedRemoved handle=9229
    2026-09-29T05:43:29.886Z [INFO] [autopilot] event PedRemoved handle=8461
    2026-09-29T05:43:29.886Z [INFO] [autopilot] event PedRemoved handle=8202
    2026-09-29T05:43:29.887Z [INFO] [autopilot] event PedRemoved handle=6411
    2026-09-29T05:43:29.888Z [INFO] [autopilot] event PedRemoved handle=9481
    2026-09-29T05:43:29.889Z [INFO] [autopilot] event PedRemoved handle=10502
    2026-09-29T05:43:29.890Z [INFO] [autopilot] event VehicleAppeared handle=8965
    2026-09-29T05:43:29.890Z [INFO] [autopilot] event VehicleAppeared handle=9221
    2026-09-29T05:43:29.957Z [INFO] [autopilot] event PedRemoved handle=5897
    2026-09-29T05:43:30.179Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:43:30.471Z [INFO] [autopilot] event PedAppeared handle=6918
    2026-09-29T05:43:30.472Z [INFO] [autopilot] event PedAppeared handle=7174
    2026-09-29T05:43:30.473Z [INFO] [autopilot] event PedAppeared handle=7429
    2026-09-29T05:43:30.771Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:43:30.791Z [INFO] [autopilot] event VehicleAppeared handle=3844
    2026-09-29T05:43:30.792Z [INFO] [autopilot] event VehicleRemoved handle=9221
    2026-09-29T05:43:30.792Z [INFO] [autopilot] event VehicleRemoved handle=8965
    2026-09-29T05:43:31.133Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.507, 695.253, 15.088) to=(-86.193, 660.048, 18.306)
    2026-09-29T05:43:33.471Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-30.675, 698.703, 15.141) to=(-67.265, 671.77, 15.053)
    2026-09-29T05:43:33.788Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.061, 695.537, 15.064) to=(-67.077, 673.66, 14.084)
    2026-09-29T05:43:33.951Z [INFO] [autopilot] event PedAppeared handle=5898
    2026-09-29T05:43:34.109Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.036, 695.557, 15.105) to=(-67.07, 672.617, 14.893)
    2026-09-29T05:43:34.457Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.044, 695.535, 15.103) to=(-86.316, 661.158, 13.857)
    2026-09-29T05:43:34.856Z [INFO] [autopilot] event VehicleAppeared handle=3076
    2026-09-29T05:43:35.481Z [INFO] command source=file:cmd_20260929054335260.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:43:35.575Z [INFO] [autopilot] event PedRemoved handle=7429
    2026-09-29T05:43:35.576Z [INFO] [autopilot] event PedRemoved handle=7174
    2026-09-29T05:43:35.577Z [INFO] [autopilot] event PedRemoved handle=6918
    2026-09-29T05:43:35.742Z [INFO] command source=file:cmd_20260929054335644.cmd line="weather 1" reply="weather 1"
    2026-09-29T05:43:35.898Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-30.718, 698.701, 15.097) to=(-113.842, 642.195, 12.214)
    2026-09-29T05:43:36.257Z [INFO] command source=file:cmd_20260929054336040.cmd line="spawnprop lf_test_crate 2.5 0" reply="spawning prop lf_test_crate"
    2026-09-29T05:43:36.286Z [INFO] [autopilot] autopilot_prop handle=12808 model=lf_test_crate at=(-64.419, 677.334, 13.568)
    2026-09-29T05:43:36.499Z [INFO] [autopilot] event PedAppeared handle=6412
    2026-09-29T05:43:36.625Z [INFO] [autopilot] event PedAppeared handle=6667
    2026-09-29T05:43:36.804Z [INFO] [autopilot] event PedRemoved handle=5898
    2026-09-29T05:43:36.877Z [INFO] [autopilot] event PedAppeared handle=8203
    2026-09-29T05:43:37.051Z [INFO] [autopilot] event PedAppeared handle=8462
    2026-09-29T05:43:37.170Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-31.164, 699.905, 15.089) to=(-67.048, 673.429, 13.846)
    2026-09-29T05:43:37.227Z [INFO] [autopilot] event VehicleAppeared handle=1798
    2026-09-29T05:43:37.403Z [INFO] [autopilot] event PedAppeared handle=8972
    2026-09-29T05:43:37.943Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.071, 695.561, 15.069) to=(-64.3, 674.862, 15.211)
    2026-09-29T05:43:37.943Z [INFO] [autopilot] event PedDamaged handle=2 100->80 bone=0x4B5 by_player=False weapon=7 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=5382 vehicle=0 killed=False hit=True at=(-64.3, 674.862, 15.211) dir=(-0.849, -0.529, 0.004)
    2026-09-29T05:43:37.944Z [INFO] [autopilot] event PlayerDamaged 100->80
    2026-09-29T05:43:38.064Z [INFO] command source=file:cmd_20260929054337973.cmd line="hud off" reply="hud off"
    2026-09-29T05:43:38.232Z [INFO] [autopilot] event VehicleRemoved handle=3076
    2026-09-29T05:43:38.233Z [INFO] [autopilot] event VehicleRemoved handle=2820
    2026-09-29T05:43:38.280Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.044, 695.594, 15.103) to=(-67.077, 673.229, 14.151)
    2026-09-29T05:43:38.565Z [INFO] command source=file:cmd_20260929054338354.cmd line="cam prop 0 2.2 0.6" reply="camera at 0 deg, 2.2 m"
    2026-09-29T05:43:38.644Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.046, 695.575, 15.107) to=(-86.341, 661.199, 16.015)
    2026-09-29T05:43:38.706Z [INFO] [autopilot] event PedAppeared handle=9230
    2026-09-29T05:43:38.736Z [INFO] [autopilot] event PedAppeared handle=9482
    2026-09-29T05:43:38.770Z [INFO] [autopilot] event PedRemoved handle=6667
    2026-09-29T05:43:39.032Z [INFO] [autopilot] event PedRemoved handle=6412
    2026-09-29T05:43:39.143Z [INFO] [autopilot] event PedAppeared handle=6668
    2026-09-29T05:43:39.237Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.244, 698.403, 15.131) to=(-112.998, 634.702, 14.315)
    2026-09-29T05:43:39.291Z [INFO] [autopilot] event VehicleRemoved handle=11524
    2026-09-29T05:43:39.509Z [INFO] [autopilot] event PedRemoved handle=8972
    2026-09-29T05:43:39.530Z [INFO] [autopilot] event VehicleAppeared handle=1542
    2026-09-29T05:43:39.587Z [INFO] [autopilot] event PedAppeared handle=8973
    2026-09-29T05:43:39.852Z [INFO] [autopilot] event PedRemoved handle=8462
    2026-09-29T05:43:39.903Z [INFO] [autopilot] event PedRemoved handle=8203
    2026-09-29T05:43:39.941Z [INFO] [autopilot] event PedAppeared handle=8463
    2026-09-29T05:43:40.192Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:43:40.394Z [INFO] [autopilot] event PedAppeared handle=9741
    2026-09-29T05:43:40.421Z [INFO] [autopilot] event PedAppeared handle=9993
    2026-09-29T05:43:40.446Z [INFO] [autopilot] event VehicleAppeared handle=6404
    2026-09-29T05:43:40.559Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-31.396, 694.932, 14.857) to=(-64.67, 674.927, 14.694)
    2026-09-29T05:43:40.560Z [INFO] [autopilot] event PedDamaged handle=2 80->65 bone=0x4C2 by_player=False weapon=7 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=5382 vehicle=0 killed=False hit=True at=(-64.67, 674.927, 14.694) dir=(-0.857, -0.515, -0.004)
    2026-09-29T05:43:40.560Z [INFO] [autopilot] event PlayerDamaged 80->65
    2026-09-29T05:43:40.561Z [INFO] [autopilot] event PedAppeared handle=10249
    2026-09-29T05:43:40.676Z [INFO] [autopilot] event PedAppeared handle=10503
    2026-09-29T05:43:40.836Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-32.029, 694.205, 14.884) to=(-67.077, 672.466, 14.442)
    2026-09-29T05:43:41.000Z [INFO] [autopilot] event PedAppeared handle=10760
    2026-09-29T05:43:41.080Z [INFO] [autopilot] event PedAppeared handle=3333
    2026-09-29T05:43:41.081Z [INFO] [autopilot] event PedAppeared handle=3590
    2026-09-29T05:43:41.259Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.224, 698.44, 15.133) to=(-64.352, 674.916, 14.995)
    2026-09-29T05:43:41.260Z [INFO] [autopilot] event PedDamaged handle=2 52->19 bone=0x36A1 by_player=False weapon=10 exact=True type=Bullet amount=33.0 health_lost=33.0 armour_lost=0.0 attacker=5643 vehicle=0 killed=False hit=True at=(-64.352, 674.916, 14.995) dir=(-0.778, -0.628, -0.004)
    2026-09-29T05:43:41.261Z [INFO] [autopilot] event PedDamaged handle=2 31->19 bone=0x1A8 by_player=False weapon=10 exact=True type=Bullet amount=12.4 health_lost=12.4 armour_lost=0.0 attacker=5643 vehicle=0 killed=False hit=True at=(-64.352, 674.916, 14.995) dir=(-0.778, -0.628, -0.004)
    2026-09-29T05:43:41.262Z [INFO] [autopilot] event PlayerDamaged 65->19
    2026-09-29T05:43:42.012Z [INFO] [autopilot] event PedRemoved handle=8973
    2026-09-29T05:43:42.218Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-36.377, 691.376, 15.095) to=(-93.159, 659.491, 14.969)
    2026-09-29T05:43:42.225Z [INFO] camera_already_gone handle=6658 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:42.226Z [INFO] command source=file:cmd_20260929054342083.cmd line="cam prop 90 2.2 0.9" reply="camera at 90 deg, 2.2 m"
    2026-09-29T05:43:42.543Z [INFO] [autopilot] event PedAppeared handle=8204
    2026-09-29T05:43:42.544Z [INFO] [autopilot] event PedRemoved handle=6668
    2026-09-29T05:43:42.625Z [INFO] [autopilot] event PedAppeared handle=6153
    2026-09-29T05:43:42.626Z [INFO] [autopilot] event PedAppeared handle=7687
    2026-09-29T05:43:42.627Z [INFO] [autopilot] event PedAppeared handle=7945
    2026-09-29T05:43:42.628Z [INFO] [autopilot] event PedAppeared handle=8718
    2026-09-29T05:43:42.716Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-37.414, 690.779, 14.953) to=(-92.894, 656.686, 16.425)
    2026-09-29T05:43:42.789Z [INFO] [autopilot] event VehicleRemoved handle=10246
    2026-09-29T05:43:42.929Z [INFO] [autopilot] event PedRemoved handle=9741
    2026-09-29T05:43:43.093Z [INFO] [autopilot] event PedRemoved handle=8463
    2026-09-29T05:43:43.309Z [INFO] [autopilot] event PedAppeared handle=8464
    2026-09-29T05:43:43.331Z [INFO] [autopilot] event PedRemoved handle=9993
    2026-09-29T05:43:43.372Z [INFO] [autopilot] event PedAppeared handle=8974
    2026-09-29T05:43:43.446Z [INFO] [autopilot] event VehicleRemoved handle=3844
    2026-09-29T05:43:43.730Z [INFO] [autopilot] event PedRemoved handle=10249
    2026-09-29T05:43:43.965Z [INFO] [autopilot] event PedRemoved handle=9230
    2026-09-29T05:43:44.176Z [INFO] [autopilot] event PedAppeared handle=9231
    2026-09-29T05:43:44.290Z [INFO] [autopilot] event PedRemoved handle=9482
    2026-09-29T05:43:44.322Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-40.356, 689.028, 15.094) to=(-96.702, 656.427, 14.092)
    2026-09-29T05:43:44.678Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-40.359, 689.032, 15.097) to=(-96.287, 655.696, 15.339)
    2026-09-29T05:43:44.973Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.231, 698.429, 15.131) to=(-113.409, 635.233, 15.523)
    2026-09-29T05:43:44.974Z [INFO] [autopilot] event PedRemoved handle=9231
    2026-09-29T05:43:45.539Z [INFO] [autopilot] event PedRemoved handle=10503
    2026-09-29T05:43:45.599Z [INFO] [autopilot] event PedRemoved handle=8464
    2026-09-29T05:43:45.866Z [INFO] camera_already_gone handle=6914 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:45.867Z [INFO] command source=file:cmd_20260929054345716.cmd line="cam prop 200 1.6 1.4" reply="camera at 200 deg, 1.6 m"
    2026-09-29T05:43:45.888Z [INFO] [autopilot] event PedRemoved handle=8204
    2026-09-29T05:43:45.934Z [INFO] [autopilot] event VehicleRemoved handle=7940
    2026-09-29T05:43:45.966Z [INFO] [autopilot] event PedAppeared handle=8465
    2026-09-29T05:43:46.292Z [INFO] [autopilot] event PedAppeared handle=9232
    2026-09-29T05:43:46.346Z [INFO] [autopilot] event PedAppeared handle=9483
    2026-09-29T05:43:46.385Z [INFO] [autopilot] event PedAppeared handle=9742
    2026-09-29T05:43:46.427Z [INFO] [autopilot] event PedRemoved handle=8974
    2026-09-29T05:43:46.470Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.23, 698.429, 15.131) to=(-67.156, 672.192, 14.98)
    2026-09-29T05:43:46.672Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-40.345, 689.054, 15.098) to=(-96.798, 656.633, 14.16)
    2026-09-29T05:43:46.976Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-40.189, 689.131, 15.043) to=(-67.072, 672.575, 13.93)
    2026-09-29T05:43:47.883Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.227, 698.433, 15.131) to=(-64.244, 674.74, 14.412)
    2026-09-29T05:43:47.884Z [INFO] [autopilot] event PedDamaged handle=2 20->-100 bone=0x1A7 by_player=False weapon=10 exact=True type=Bullet amount=24.8 health_lost=120.0 armour_lost=0.0 attacker=5643 vehicle=0 killed=True hit=True at=(-64.244, 674.74, 14.412) dir=(-0.774, -0.632, -0.019)
    2026-09-29T05:43:47.885Z [INFO] [autopilot] event PedDied handle=2 bone=0x1A7 by_player=False exact=True type=Bullet killer=5643 weapon=10
    2026-09-29T05:43:47.885Z [INFO] [autopilot] event PlayerDied
    2026-09-29T05:43:47.886Z [INFO] [autopilot] event PlayerDamaged 19->-100
    2026-09-29T05:43:47.888Z [INFO] holsters_removed reason=wasted
    2026-09-29T05:43:47.888Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored
    2026-09-29T05:43:48.335Z [INFO] [autopilot] event PedRemoved handle=9232
    2026-09-29T05:43:48.369Z [INFO] [autopilot] event PedRemoved handle=9742
    2026-09-29T05:43:48.490Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=0.5 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:43:48.538Z [INFO] density frame_ms=30.4 peds=0.89 cars=0.90
    2026-09-29T05:43:48.647Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4C0 by_player=False weapon=54 exact=True type=Fall amount=0.4 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:43:48.947Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.188, 690.507, 14.989) to=(-95.309, 659.29, 13.572)
    2026-09-29T05:43:49.006Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:43:49.010Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=7628 core=on peds=12 vehicles=10 modules=10/10 coroutines=0 resources=6 raycast=on episode=GTAIV frame_ms=30.75 p95_ms=40.26 pressure=0.04 private_mb=2211 working_set_mb=1982 address_free_mb=1181 largest_free_block_mb=1146 managed_mb=14 physical_load=92% core_us=57.1
    2026-09-29T05:43:49.176Z [INFO] performance samples=1129 frame_p50_ms=24 frame_p95_ms=43 frame_p99_ms=65 frames_over_33ms=227 frames_over_50ms=29 gunplay_avg_ms=1.225 gunplay_max_ms=4.702 phase_samples=1082 phase_setup_avg_ms=0.838 phase_setup_max_ms=4.270 phase_camera_avg_ms=0.023 phase_camera_max_ms=1.576 phase_bullets_avg_ms=0.005 phase_bullets_max_ms=0.224 phase_weapon_avg_ms=0.019 phase_weapon_max_ms=0.182 phase_hud_avg_ms=0.394 phase_hud_max_ms=1.132
    2026-09-29T05:43:49.177Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.531/301.3/1129@7 total=2857 module.gunplay=1.233/5.5/1129@7 total=1392 tick.gunplay=1.231/5.5/1129@7 total=1390 gp.freeaim=0.662/4.1/1082@7 total=716 module.arsenal=0.618/13.2/793@7 total=490 tick.arsenal=0.618/13.2/793@7 total=490 ar.storage=0.425/9.2/759@7 total=322 engine.world=0.207/11.8/1129@7 total=234 module.atmosphere=0.192/2.8/1129@7 total=216 tick.atmosphere=0.191/2.8/1129@7 total=215 ar.safehouse_flags=0.167/2.5/759@7 total=126 gp.index_pad=0.114/0.4/1082@7 total=124 module.combat=0.024/0.5/1129@7 total=27 tick.combat=0.023/0.5/1129@7 total=26 module.devtools=0.028/3.5/793@7 total=22 tick.devtools=0.027/3.5/793@7 total=22 module.holsters=0.051/0.6/421@7 total=21 tick.holsters=0.050/0.6/421@7 total=21 cam.handle=0.017/1.6/1082@7 total=18 gp.shoulder=0.015/0.2/1082@7 total=16 gp.weapon_id=0.012/0.0/1082@7 total=13 gp.player=0.011/0.0/1082@7 total=12 module.world=0.197/1.0/58@7 total=11 gp.cycle=0.008/0.1/1082@7 total=9 gp.state=0.006/0.1/1082@7 total=7 gp.shots=0.004/0.1/1082@7 total=4 module.probe=1.211/1.3/3@7 total=4 cam.aim_key=0.003/0.0/1082@7 total=3 cam.find_active=0.002/0.0/1082@7 total=3 ar.vehicle=0.003/0.0/793@7 total=2 engine.scheduler=0.002/1.6/1129@7 total=2 gp.spread=0.002/0.1/1082@7 total=2 ar.lvs=0.002/0.3/793@7 total=2 ar.discover=0.002/0.9/793@7 total=2 ar.reconcile=0.002/0.1/759@7 total=1 ho.carried=0.002/0.0/403@7 total=1 combat.dismember=0.001/0.0/1129@7 total=1 ho.show=0.002/0.0/403@7 total=1 combat.blood=0.000/0.1/1129@7 total=0 module.autopilot=0.000/0.0/1129@7 total=0 module.weapon-probe=0.000/0.0/1129@7 total=0 gp.feel=0.000/0.0/1082@7 total=0 combat.pending=0.000/0.0/1129@7 total=0 gp.recoil=0.000/0.0/1082@7 total=0 cam.fov=0.001/0.0/108@7 total=0
    2026-09-29T05:43:49.178Z [INFO] engine_thread_probe ticks=1129 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1128 ticks_after_skipped_frames=1
    2026-09-29T05:43:49.190Z [INFO] direct_native get_char_health direct=0 shdn=-100 match=False direct_us=0.19 shdn_us=556.5
    2026-09-29T05:43:49.210Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.239, 698.428, 15.128) to=(-61.879, 678.14, 13.703)
    2026-09-29T05:43:49.319Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.183, 690.506, 14.987) to=(-66.314, 675.468, 13.881)
    2026-09-29T05:43:49.320Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4C9 by_player=False weapon=7 exact=True type=Bullet amount=14.9 health_lost=0.0 armour_lost=0.0 attacker=5382 vehicle=0 killed=False hit=True at=(-66.314, 675.468, 13.881) dir=(-0.881, -0.471, -0.035)
    2026-09-29T05:43:49.417Z [INFO] camera_already_gone handle=7170 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:49.418Z [INFO] command source=file:cmd_20260929054349318.cmd line="cam off" reply="camera off"
    2026-09-29T05:43:49.454Z [INFO] [autopilot] event PedRemoved handle=9483
    2026-09-29T05:43:49.913Z [INFO] command source=file:cmd_20260929054349712.cmd line="clear" reply="cleared 1"
    2026-09-29T05:43:50.167Z [INFO] command source=file:cmd_20260929054350093.cmd line="spawnprop lf_blender_barrel 2.5 0" reply="spawning prop lf_blender_barrel"
    2026-09-29T05:43:50.189Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:43:50.293Z [INFO] [autopilot] autopilot_prop handle=11793 model=lf_blender_barrel at=(-68.048, 675.773, 13.731)
    2026-09-29T05:43:50.663Z [INFO] [autopilot] event VehicleRemoved handle=2308
    2026-09-29T05:43:51.288Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.242, 698.435, 15.128) to=(-66.129, 675.121, 13.859)
    2026-09-29T05:43:51.289Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x36A1 by_player=False weapon=10 exact=True type=Bullet amount=33.0 health_lost=0.0 armour_lost=0.0 attacker=5643 vehicle=0 killed=False hit=True at=(-66.129, 675.121, 13.859) dir=(-0.798, -0.602, -0.033)
    2026-09-29T05:43:51.542Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.185, 690.518, 15.004) to=(-66.169, 675.219, 13.744)
    2026-09-29T05:43:51.543Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4C1 by_player=False weapon=7 exact=True type=Bullet amount=14.9 health_lost=0.0 armour_lost=0.0 attacker=5382 vehicle=0 killed=False hit=True at=(-66.169, 675.219, 13.744) dir=(-0.877, -0.479, -0.039)
    2026-09-29T05:43:51.717Z [INFO] [autopilot] event PedRemoved handle=8465
    2026-09-29T05:43:51.852Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.178, 690.503, 14.995) to=(-61.764, 677.493, 13.67)
    2026-09-29T05:43:52.173Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.179, 690.517, 14.984) to=(-94.96, 658.878, 11.206)
    2026-09-29T05:43:52.242Z [INFO] [autopilot] event PedAppeared handle=7175
    2026-09-29T05:43:52.245Z [INFO] command source=file:cmd_20260929054351998.cmd line="cam prop 20 2.4 0.8" reply="camera at 20 deg, 2.4 m"
    2026-09-29T05:43:52.266Z [INFO] [autopilot] event PedAppeared handle=7430
    2026-09-29T05:43:52.725Z [INFO] [autopilot] event PedAppeared handle=8205
    2026-09-29T05:43:52.909Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.242, 698.427, 15.126) to=(-62.422, 677.682, 13.713)
    2026-09-29T05:43:52.910Z [INFO] [autopilot] event PedAppeared handle=8466
    2026-09-29T05:43:53.266Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.154, 690.52, 14.973) to=(-94.61, 658.195, 12.411)
    2026-09-29T05:43:54.817Z [INFO] [autopilot] event BulletFired shooter=5382 weapon=7 by_player=False from=(-38.149, 690.526, 14.983) to=(-66.981, 674.895, 13.711)
    2026-09-29T05:43:55.044Z [INFO] [autopilot] event BulletFired shooter=5643 weapon=10 by_player=False from=(-35.244, 698.428, 15.126) to=(-115.816, 638.48, 10.743)
    2026-09-29T05:43:55.496Z [INFO] [autopilot] event PedRemoved handle=8466
    2026-09-29T05:43:55.497Z [INFO] [autopilot] event PedRemoved handle=8205
    2026-09-29T05:43:55.497Z [INFO] [autopilot] event PedRemoved handle=7175
    2026-09-29T05:43:55.498Z [INFO] [autopilot] event PedRemoved handle=7687
    2026-09-29T05:43:55.499Z [INFO] [autopilot] event PedRemoved handle=3590
    2026-09-29T05:43:55.500Z [INFO] [autopilot] event PedRemoved handle=3333
    2026-09-29T05:43:55.500Z [INFO] [autopilot] event PedRemoved handle=10760
    2026-09-29T05:43:55.501Z [INFO] [autopilot] event PedRemoved handle=6153
    2026-09-29T05:43:55.502Z [INFO] [autopilot] event PedRemoved handle=7945
    2026-09-29T05:43:55.502Z [INFO] [autopilot] event PedRemoved handle=8718
    2026-09-29T05:43:55.503Z [INFO] [autopilot] event PedRemoved handle=5643
    2026-09-29T05:43:55.504Z [INFO] [autopilot] event PedRemoved handle=7430
    2026-09-29T05:43:55.505Z [INFO] [autopilot] event PedRemoved handle=5382
    2026-09-29T05:43:55.505Z [INFO] [autopilot] event VehicleAppeared handle=1543
    2026-09-29T05:43:55.506Z [INFO] [autopilot] event VehicleAppeared handle=1799
    2026-09-29T05:43:55.507Z [INFO] [autopilot] event VehicleAppeared handle=2055
    2026-09-29T05:43:55.507Z [INFO] [autopilot] event VehicleAppeared handle=2309
    2026-09-29T05:43:55.508Z [INFO] [autopilot] event VehicleAppeared handle=2566
    2026-09-29T05:43:55.509Z [INFO] [autopilot] event VehicleAppeared handle=2821
    2026-09-29T05:43:55.509Z [INFO] [autopilot] event VehicleAppeared handle=3077
    2026-09-29T05:43:55.510Z [INFO] [autopilot] event VehicleAppeared handle=3335
    2026-09-29T05:43:55.511Z [INFO] [autopilot] event VehicleAppeared handle=3589
    2026-09-29T05:43:55.511Z [INFO] [autopilot] event VehicleAppeared handle=3845
    2026-09-29T05:43:55.512Z [INFO] [autopilot] event VehicleAppeared handle=4100
    2026-09-29T05:43:55.513Z [INFO] [autopilot] event VehicleAppeared handle=4356
    2026-09-29T05:43:55.514Z [INFO] [autopilot] event VehicleAppeared handle=4612
    2026-09-29T05:43:55.514Z [INFO] [autopilot] event VehicleAppeared handle=5126
    2026-09-29T05:43:55.515Z [INFO] [autopilot] event VehicleAppeared handle=5380
    2026-09-29T05:43:55.516Z [INFO] [autopilot] event VehicleAppeared handle=5636
    2026-09-29T05:43:55.517Z [INFO] [autopilot] event VehicleAppeared handle=5893
    2026-09-29T05:43:55.517Z [INFO] [autopilot] event VehicleAppeared handle=6661
    2026-09-29T05:43:55.518Z [INFO] [autopilot] event VehicleRemoved handle=1542
    2026-09-29T05:43:55.519Z [INFO] [autopilot] event VehicleRemoved handle=1798
    2026-09-29T05:43:55.520Z [INFO] [autopilot] event VehicleRemoved handle=11013
    2026-09-29T05:43:55.520Z [INFO] [autopilot] event VehicleRemoved handle=10757
    2026-09-29T05:43:55.521Z [INFO] [autopilot] event VehicleRemoved handle=9733
    2026-09-29T05:43:55.522Z [INFO] [autopilot] event VehicleRemoved handle=8197
    2026-09-29T05:43:55.523Z [INFO] [autopilot] event VehicleRemoved handle=6404
    2026-09-29T05:43:55.524Z [INFO] [autopilot] event VehicleRemoved handle=8708
    2026-09-29T05:43:55.524Z [INFO] [autopilot] event VehicleRemoved handle=8452
    2026-09-29T05:43:55.530Z [INFO] [world] world_object removed name=test_wall reason=out of range
    2026-09-29T05:43:55.778Z [INFO] camera_already_gone handle=7426 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:55.779Z [INFO] command source=file:cmd_20260929054355565.cmd line="cam prop 150 1.8 1.6" reply="camera at 150 deg, 1.8 m"
    2026-09-29T05:43:55.943Z [INFO] [autopilot] event VehicleAppeared handle=6918
    2026-09-29T05:43:55.944Z [INFO] [autopilot] event VehicleAppeared handle=7173
    2026-09-29T05:43:55.961Z [INFO] [autopilot] event PedAppeared handle=3846
    2026-09-29T05:43:55.980Z [INFO] [autopilot] event VehicleAppeared handle=7430
    2026-09-29T05:43:56.065Z [INFO] [autopilot] event PedAppeared handle=4103
    2026-09-29T05:43:56.066Z [INFO] [autopilot] event VehicleAppeared handle=7685
    2026-09-29T05:43:56.099Z [INFO] [autopilot] event PedAppeared handle=4359
    2026-09-29T05:43:56.110Z [INFO] [autopilot] event PedAppeared handle=4613
    2026-09-29T05:43:56.128Z [INFO] [autopilot] event PedAppeared handle=4871
    2026-09-29T05:43:56.152Z [INFO] [autopilot] event PedAppeared handle=5130
    2026-09-29T05:43:56.153Z [INFO] [autopilot] event PedAppeared handle=5383
    2026-09-29T05:43:56.153Z [INFO] [autopilot] event VehicleAppeared handle=7941
    2026-09-29T05:43:56.165Z [INFO] [autopilot] event PedAppeared handle=5644
    2026-09-29T05:43:56.204Z [INFO] [autopilot] event PedAppeared handle=5899
    2026-09-29T05:43:56.205Z [INFO] [autopilot] event VehicleAppeared handle=8198
    2026-09-29T05:43:56.245Z [INFO] [autopilot] event VehicleAppeared handle=8453
    2026-09-29T05:43:56.281Z [INFO] [autopilot] event PedAppeared handle=6154
    2026-09-29T05:43:56.293Z [INFO] [autopilot] event PedAppeared handle=6413
    2026-09-29T05:43:56.306Z [INFO] [autopilot] event VehicleAppeared handle=8709
    2026-09-29T05:43:56.332Z [INFO] [autopilot] event VehicleAppeared handle=8966
    2026-09-29T05:43:56.475Z [INFO] [autopilot] event VehicleAppeared handle=9222
    2026-09-29T05:43:56.643Z [INFO] [autopilot] event VehicleAppeared handle=9478
    2026-09-29T05:43:56.971Z [INFO] [autopilot] event PedAppeared handle=6669
    2026-09-29T05:43:56.972Z [INFO] [autopilot] event PedAppeared handle=6919
    2026-09-29T05:43:56.988Z [INFO] [autopilot] event PedAppeared handle=7176
    2026-09-29T05:43:57.005Z [INFO] [autopilot] event VehicleAppeared handle=9734
    2026-09-29T05:43:57.021Z [INFO] [autopilot] event PedAppeared handle=7431
    2026-09-29T05:43:57.022Z [INFO] [autopilot] event PedAppeared handle=7688
    2026-09-29T05:43:57.023Z [INFO] [autopilot] event PedAppeared handle=7946
    2026-09-29T05:43:57.040Z [INFO] [autopilot] event PedAppeared handle=8206
    2026-09-29T05:43:57.058Z [INFO] [autopilot] event PedAppeared handle=8467
    2026-09-29T05:43:57.059Z [INFO] [autopilot] event PedAppeared handle=8719
    2026-09-29T05:43:57.099Z [INFO] [autopilot] event PedAppeared handle=8975
    2026-09-29T05:43:57.126Z [INFO] [autopilot] event PedAppeared handle=9233
    2026-09-29T05:43:57.218Z [INFO] [autopilot] event VehicleAppeared handle=9990
    2026-09-29T05:43:57.336Z [INFO] [autopilot] event PedAppeared handle=9484
    2026-09-29T05:43:59.325Z [INFO] camera_already_gone handle=4355 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:43:59.325Z [INFO] command source=file:cmd_20260929054359183.cmd line="cam off" reply="camera off"
    2026-09-29T05:43:59.598Z [INFO] command source=file:cmd_20260929054359569.cmd line="hud on" reply="hud on"
    2026-09-29T05:44:00.098Z [INFO] [autopilot] event PedAppeared handle=9743
    2026-09-29T05:44:00.098Z [INFO] [autopilot] event PedAppeared handle=9994
    2026-09-29T05:44:00.149Z [INFO] command source=file:cmd_20260929054359948.cmd line="clear" reply="cleared 1"
    2026-09-29T05:44:00.191Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:44:00.277Z [INFO] [autopilot] event PedAppeared handle=10250

# Scenario sling-review

- Result: FAIL
- Steps: 41, failed: 1
- Game alive at end: True
- Log errors during run: 1

## Failed steps
- command refused: alive => alive => error player is missing or dead

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- god on => invincible True
- alive => error player is missing or dead
- FAILED: command refused: alive => alive => error player is missing or dead
- owned autopilot => autopilot: invincible=1
- expect line=.owned autopilot. reply=.*invincible=1\b: OK 2026-09-29T21:35:43.622Z [INFO] command source=file:cmd_20260929213543398.cmd line="owned autopilot" reply="autopilot: invincible=1"
- time 13 0 => time 13:00
- weather 1 => weather 1
- mark log line 302
- give 10 200 => gave 10
- give 14 200 => gave 14
- give 7 100 => gave 7
- select 7 => selected 7
- expectmarked holster_sling_attached slot=LongGun1: OK 2026-09-29T21:35:57.667Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
- expectmarked holster_sling_attached slot=LongGun2: OK 2026-09-29T21:35:57.687Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
- hud off => hud off
- cam 0 2.2 0.4 => camera at 0 deg, 2.2 m
- wait 1500 ms
- alive => player alive health=100
- shot front -> front.jpg
- cam 180 2.2 0.4 => camera at 180 deg, 2.2 m
- wait 1500 ms
- alive => player alive health=100
- shot back -> back.jpg
- cam 90 2.0 0.4 => camera at 90 deg, 2 m
- wait 1500 ms
- alive => player alive health=100
- shot left -> left.jpg
- cam 270 2.0 0.4 => camera at 270 deg, 2 m
- wait 1500 ms
- alive => player alive health=100
- shot right -> right.jpg
- cam 150 1.4 0.6 => camera at 150 deg, 1.4 m
- wait 1500 ms
- alive => player alive health=100
- shot back_close -> back_close.jpg
- cam off => camera off
- hud on => hud on

## Errors
    2026-09-29T21:35:36.258Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T21:35:36.257Z [INFO] holsters_removed reason=wasted
    2026-09-29T21:35:36.258Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored
    2026-09-29T21:35:36.259Z [INFO] arsenal_loss reason=wasted id=3 owned=False
    2026-09-29T21:35:36.260Z [INFO] arsenal_loss reason=wasted id=7 owned=False
    2026-09-29T21:35:36.261Z [INFO] arsenal_loss reason=wasted id=12 owned=False
    2026-09-29T21:35:36.261Z [INFO] arsenal_loss reason=wasted id=16 owned=False
    2026-09-29T21:35:36.262Z [INFO] arsenal_loss reason=wasted id=18 owned=False
    2026-09-29T21:35:36.263Z [INFO] arsenal_loss reason=wasted id=5 owned=False
    2026-09-29T21:35:36.732Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:35:36.820Z [INFO] command source=file:cmd_20260929213536766.cmd line="events on" reply="event log on"
    2026-09-29T21:35:37.298Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.542, 641.655, 15.182) to=(-67.549, 670.29, 14.804)
    2026-09-29T21:35:37.299Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.5 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T21:35:37.300Z [INFO] [autopilot] event PedAppeared handle=6148
    2026-09-29T21:35:37.370Z [INFO] command source=file:cmd_20260929213537171.cmd line="god on" reply="invincible True"
    2026-09-29T21:35:37.501Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.986, 658.856, 15.097) to=(-62.877, 684.819, 13.695)
    2026-09-29T21:35:37.502Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.571, 641.65, 15.195) to=(-75.171, 655.524, 14.793)
    2026-09-29T21:35:37.577Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.821, 640.217, 15.185) to=(-67.549, 669.66, 14.546)
    2026-09-29T21:35:37.578Z [INFO] [autopilot] event PedRemoved handle=6148
    2026-09-29T21:35:37.579Z [INFO] [autopilot] event VehicleAppeared handle=1283
    2026-09-29T21:35:37.666Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.565, 641.652, 15.181) to=(300.234, 1344.635, -5.4)
    2026-09-29T21:35:37.670Z [INFO] command source=file:cmd_20260929213537559.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T21:35:37.741Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.017, 658.837, 15.106) to=(31.208, 1451.63, -40.761)
    2026-09-29T21:35:37.823Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.854, 640.214, 15.191) to=(311.471, 1335.502, -6.169)
    2026-09-29T21:35:37.824Z [INFO] [autopilot] event PedAppeared handle=6403
    2026-09-29T21:35:37.825Z [INFO] [autopilot] event PedAppeared handle=6659
    2026-09-29T21:35:37.825Z [INFO] [autopilot] event PedAppeared handle=8194
    2026-09-29T21:35:37.826Z [INFO] [autopilot] event PedAppeared handle=8450
    2026-09-29T21:35:37.827Z [INFO] [autopilot] event PedAppeared handle=8706
    2026-09-29T21:35:37.828Z [INFO] [autopilot] event PedAppeared handle=8962
    2026-09-29T21:35:37.906Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.564, 641.651, 15.193) to=(-75.186, 655.245, 14.859)
    2026-09-29T21:35:37.999Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.842, 640.213, 15.179) to=(-76.418, 655.017, 14.4)
    2026-09-29T21:35:38.000Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.007, 658.835, 15.094) to=(-64.541, 676.573, 13.56)
    2026-09-29T21:35:38.004Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T21:35:38.012Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=6
    2026-09-29T21:35:38.012Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T21:35:38.013Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=0
    2026-09-29T21:35:38.014Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T21:35:38.348Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=334
    2026-09-29T21:35:38.349Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T21:35:38.350Z [INFO] command source=file:cmd_20260929213537941.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T21:35:38.367Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.57, 641.652, 15.183) to=(-75.803, 654.983, 14.675)
    2026-09-29T21:35:38.368Z [INFO] [autopilot] event PedAppeared handle=9218
    2026-09-29T21:35:38.547Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.002, 658.836, 15.105) to=(-64.056, 676.654, 14.619)
    2026-09-29T21:35:38.548Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.056, 676.654, 14.619) dir=(0.109, 0.994, -0.027)
    2026-09-29T21:35:38.624Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.848, 640.195, 15.2) to=(-55.861, 692.714, 15.277)
    2026-09-29T21:35:38.625Z [INFO] [autopilot] event PedAppeared handle=9474
    2026-09-29T21:35:38.627Z [INFO] [autopilot] event VehicleDamaged handle=7682 1000->994 engine 1000->1000
    2026-09-29T21:35:38.712Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.014, 658.811, 15.114) to=(12.828, 1455.378, -1.614)
    2026-09-29T21:35:38.713Z [INFO] [autopilot] event PedAppeared handle=9730
    2026-09-29T21:35:38.713Z [INFO] [autopilot] event PedAppeared handle=9986
    2026-09-29T21:35:38.714Z [INFO] [autopilot] event PedAppeared handle=10242
    2026-09-29T21:35:38.715Z [INFO] [autopilot] event PedAppeared handle=10498
    2026-09-29T21:35:38.791Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.858, 640.194, 15.188) to=(294.967, 1345.172, 10.481)
    2026-09-29T21:35:38.792Z [INFO] [autopilot] event PedAppeared handle=10754
    2026-09-29T21:35:38.792Z [INFO] [autopilot] event PedAppeared handle=11010
    2026-09-29T21:35:38.864Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A7 by_player=False weapon=54 exact=True type=Fall amount=0.3 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T21:35:38.952Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.3 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T21:35:39.085Z [INFO] [autopilot] event PedAppeared handle=3843
    2026-09-29T21:35:39.157Z [INFO] [autopilot] event PedAppeared handle=4099
    2026-09-29T21:35:39.157Z [INFO] [autopilot] event PedAppeared handle=4355
    2026-09-29T21:35:39.230Z [INFO] [autopilot] event PedAppeared handle=6149
    2026-09-29T21:35:39.315Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A7 by_player=False weapon=54 exact=True type=Fall amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T21:35:39.320Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T21:35:39.400Z [INFO] [autopilot] event PedAppeared handle=2308
    2026-09-29T21:35:39.400Z [INFO] [autopilot] event PedAppeared handle=7682
    2026-09-29T21:35:39.401Z [INFO] [autopilot] event PedAppeared handle=7938
    2026-09-29T21:35:40.186Z [INFO] [autopilot] event PedRemoved handle=8962
    2026-09-29T21:35:40.411Z [INFO] [autopilot] event PedRemoved handle=8706
    2026-09-29T21:35:40.613Z [INFO] [autopilot] event PedAppeared handle=8707
    2026-09-29T21:35:40.953Z [INFO] [autopilot] event PedAppeared handle=8963
    2026-09-29T21:35:41.019Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.531, 641.652, 15.163) to=(253.757, 1367.879, -13.29)
    2026-09-29T21:35:41.097Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.983, 658.853, 15.079) to=(-64.494, 685.727, 13.574)
    2026-09-29T21:35:41.228Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.58, 641.646, 15.187) to=(-76.365, 654.998, 14.661)
    2026-09-29T21:35:41.287Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.023, 658.831, 15.101) to=(-18.872, 1455.315, -52.333)
    2026-09-29T21:35:41.352Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-82.575, 641.643, 15.178) to=(264.052, 1362.752, -13.664)
    2026-09-29T21:35:41.418Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.013, 658.838, 15.087) to=(-64.778, 677.144, 13.548)
    2026-09-29T21:35:41.419Z [INFO] [autopilot] event PedRemoved handle=8450
    2026-09-29T21:35:41.420Z [INFO] [autopilot] event PedRemoved handle=8194
    2026-09-29T21:35:41.987Z [INFO] [autopilot] event PedAppeared handle=8451
    2026-09-29T21:35:41.988Z [INFO] [autopilot] event PedAppeared handle=11266
    2026-09-29T21:35:42.201Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.815, 640.226, 15.163) to=(-77.12, 655.017, 14.586)
    2026-09-29T21:35:42.616Z [INFO] [autopilot] event PedRemoved handle=9218
    2026-09-29T21:35:42.687Z [INFO] [autopilot] event VehicleAppeared handle=7939
    2026-09-29T21:35:42.759Z [INFO] command source=file:cmd_20260929213542607.cmd line="god on" reply="invincible True"
    2026-09-29T21:35:42.917Z [INFO] [autopilot] event VehicleAppeared handle=6659
    2026-09-29T21:35:43.082Z [INFO] command source=file:cmd_20260929213543007.cmd line="alive" reply="error player is missing or dead"
    2026-09-29T21:35:43.622Z [INFO] command source=file:cmd_20260929213543398.cmd line="owned autopilot" reply="autopilot: invincible=1"
    2026-09-29T21:35:43.697Z [INFO] [autopilot] event PedAppeared handle=8195
    2026-09-29T21:35:43.698Z [INFO] [autopilot] event PedRemoved handle=2308
    2026-09-29T21:35:43.699Z [INFO] [autopilot] event PedRemoved handle=7938
    2026-09-29T21:35:43.699Z [INFO] [autopilot] event PedRemoved handle=6659
    2026-09-29T21:35:43.700Z [INFO] [autopilot] event PedRemoved handle=7682
    2026-09-29T21:35:43.848Z [INFO] [autopilot] event PedRemoved handle=6403
    2026-09-29T21:35:43.931Z [INFO] command source=file:cmd_20260929213543811.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T21:35:44.139Z [INFO] [autopilot] event PedRemoved handle=4355
    2026-09-29T21:35:44.140Z [INFO] [autopilot] event PedRemoved handle=3843
    2026-09-29T21:35:44.208Z [INFO] [autopilot] event VehicleAppeared handle=8706
    2026-09-29T21:35:44.213Z [INFO] command source=file:cmd_20260929213544188.cmd line="weather 1" reply="weather 1"
    2026-09-29T21:35:44.386Z [INFO] [autopilot] event PedAppeared handle=4356
    2026-09-29T21:35:47.572Z [INFO] [autopilot] event PedRemoved handle=4356
    2026-09-29T21:35:47.573Z [INFO] [autopilot] event PedRemoved handle=6149
    2026-09-29T21:35:47.574Z [INFO] [autopilot] event PedRemoved handle=5636
    2026-09-29T21:35:47.574Z [INFO] [autopilot] event PedRemoved handle=8451
    2026-09-29T21:35:47.575Z [INFO] [autopilot] event PedRemoved handle=8963
    2026-09-29T21:35:47.576Z [INFO] [autopilot] event PedRemoved handle=5891
    2026-09-29T21:35:47.576Z [INFO] [autopilot] event PedRemoved handle=11010
    2026-09-29T21:35:47.577Z [INFO] [autopilot] event PedRemoved handle=10498
    2026-09-29T21:35:47.577Z [INFO] [autopilot] event PedRemoved handle=9986
    2026-09-29T21:35:47.578Z [INFO] [autopilot] event PedRemoved handle=9474
    2026-09-29T21:35:47.579Z [INFO] [autopilot] event PedRemoved handle=7426
    2026-09-29T21:35:47.579Z [INFO] [autopilot] event PedRemoved handle=6914
    2026-09-29T21:35:47.581Z [INFO] [autopilot] event PedRemoved handle=3330
    2026-09-29T21:35:47.582Z [INFO] [autopilot] event PedRemoved handle=2818
    2026-09-29T21:35:47.582Z [INFO] [autopilot] event PedRemoved handle=8195
    2026-09-29T21:35:47.583Z [INFO] [autopilot] event PedRemoved handle=8707
    2026-09-29T21:35:47.583Z [INFO] [autopilot] event PedRemoved handle=4099
    2026-09-29T21:35:47.584Z [INFO] [autopilot] event PedRemoved handle=3075
    2026-09-29T21:35:47.585Z [INFO] [autopilot] event PedRemoved handle=2563
    2026-09-29T21:35:47.585Z [INFO] [autopilot] event PedRemoved handle=2051
    2026-09-29T21:35:47.586Z [INFO] [autopilot] event PedRemoved handle=11266
    2026-09-29T21:35:47.586Z [INFO] [autopilot] event PedRemoved handle=10754
    2026-09-29T21:35:47.587Z [INFO] [autopilot] event PedRemoved handle=10242
    2026-09-29T21:35:47.588Z [INFO] [autopilot] event PedRemoved handle=9730
    2026-09-29T21:35:47.588Z [INFO] [autopilot] event PedRemoved handle=7170
    2026-09-29T21:35:47.589Z [INFO] [autopilot] event VehicleAppeared handle=516
    2026-09-29T21:35:47.590Z [INFO] [autopilot] event VehicleAppeared handle=772
    2026-09-29T21:35:47.590Z [INFO] [autopilot] event VehicleAppeared handle=1028
    2026-09-29T21:35:47.591Z [INFO] [autopilot] event VehicleAppeared handle=1284
    2026-09-29T21:35:47.592Z [INFO] [autopilot] event VehicleAppeared handle=1540
    2026-09-29T21:35:47.592Z [INFO] [autopilot] event VehicleAppeared handle=1796
    2026-09-29T21:35:47.593Z [INFO] [autopilot] event VehicleAppeared handle=2053
    2026-09-29T21:35:47.593Z [INFO] [autopilot] event VehicleAppeared handle=2307
    2026-09-29T21:35:47.594Z [INFO] [autopilot] event VehicleAppeared handle=2563
    2026-09-29T21:35:47.596Z [INFO] [autopilot] event VehicleAppeared handle=2819
    2026-09-29T21:35:47.596Z [INFO] [autopilot] event VehicleAppeared handle=3075
    2026-09-29T21:35:47.597Z [INFO] [autopilot] event VehicleAppeared handle=3331
    2026-09-29T21:35:47.598Z [INFO] [autopilot] event VehicleAppeared handle=3587
    2026-09-29T21:35:47.598Z [INFO] [autopilot] event VehicleRemoved handle=6659
    2026-09-29T21:35:47.599Z [INFO] [autopilot] event VehicleRemoved handle=7939
    2026-09-29T21:35:47.600Z [INFO] [autopilot] event VehicleRemoved handle=1283
    2026-09-29T21:35:47.600Z [INFO] [autopilot] event VehicleRemoved handle=1795
    2026-09-29T21:35:47.601Z [INFO] [autopilot] event VehicleRemoved handle=1539
    2026-09-29T21:35:47.601Z [INFO] [autopilot] event VehicleRemoved handle=259
    2026-09-29T21:35:47.602Z [INFO] [autopilot] event VehicleRemoved handle=771
    2026-09-29T21:35:47.603Z [INFO] [autopilot] event VehicleRemoved handle=8706
    2026-09-29T21:35:47.603Z [INFO] [autopilot] event VehicleRemoved handle=7682
    2026-09-29T21:35:47.604Z [INFO] [autopilot] event VehicleRemoved handle=7170
    2026-09-29T21:35:47.605Z [INFO] [autopilot] event VehicleRemoved handle=6402
    2026-09-29T21:35:47.605Z [INFO] [autopilot] event VehicleRemoved handle=5890
    2026-09-29T21:35:47.606Z [INFO] [autopilot] event VehicleRemoved handle=5634
    2026-09-29T21:35:47.607Z [INFO] [autopilot] event VehicleRemoved handle=5378
    2026-09-29T21:35:47.607Z [INFO] [autopilot] event VehicleRemoved handle=5122
    2026-09-29T21:35:47.609Z [INFO] [autopilot] event VehicleRemoved handle=4354
    2026-09-29T21:35:47.609Z [INFO] [autopilot] event VehicleRemoved handle=3842
    2026-09-29T21:35:47.610Z [INFO] [autopilot] event VehicleRemoved handle=2818
    2026-09-29T21:35:47.611Z [INFO] [autopilot] event VehicleRemoved handle=2562
    2026-09-29T21:35:47.611Z [INFO] [autopilot] event VehicleRemoved handle=2306
    2026-09-29T21:35:47.616Z [INFO] weapon_changed from=7 to=0 profile=vanilla
    2026-09-29T21:35:47.640Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:35:47.647Z [INFO] [world] world_object removed name=test_wall reason=out of range
    2026-09-29T21:35:47.655Z [INFO] command source=file:cmd_20260929213544583.cmd line="give 10 200" reply="gave 10"
    2026-09-29T21:35:47.883Z [INFO] [autopilot] event PlayerWeaponChanged 0->10
    2026-09-29T21:35:47.888Z [INFO] weapon_changed from=0 to=10 profile=vanilla
    2026-09-29T21:35:47.902Z [INFO] arsenal_gain id=10 owned=True mission=False
    2026-09-29T21:35:48.300Z [INFO] command source=file:cmd_20260929213547925.cmd line="give 14 200" reply="gave 14"
    2026-09-29T21:35:48.809Z [INFO] [autopilot] event PlayerWeaponChanged 10->14
    2026-09-29T21:35:48.812Z [INFO] weapon_changed from=10 to=14 profile=vanilla
    2026-09-29T21:35:48.829Z [INFO] arsenal_gain id=14 owned=True mission=False
    2026-09-29T21:35:48.844Z [INFO] command source=file:cmd_20260929213548579.cmd line="give 7 100" reply="gave 7"
    2026-09-29T21:35:49.112Z [INFO] [autopilot] event PlayerWeaponChanged 14->7
    2026-09-29T21:35:49.113Z [INFO] weapon_changed from=14 to=7 profile=vanilla
    2026-09-29T21:35:49.128Z [INFO] arsenal_gain id=7 owned=True mission=False
    2026-09-29T21:35:49.309Z [INFO] [autopilot] event PedAppeared handle=2309
    2026-09-29T21:35:49.342Z [INFO] [autopilot] event PedAppeared handle=2564
    2026-09-29T21:35:49.384Z [INFO] command source=file:cmd_20260929213549230.cmd line="select 7" reply="selected 7"
    2026-09-29T21:35:50.034Z [INFO] [autopilot] event VehicleAppeared handle=4355
    2026-09-29T21:35:50.046Z [INFO] [autopilot] event VehicleAppeared handle=4611
    2026-09-29T21:35:50.083Z [INFO] [autopilot] event VehicleAppeared handle=4867
    2026-09-29T21:35:50.098Z [INFO] [autopilot] event VehicleAppeared handle=5123
    2026-09-29T21:35:50.130Z [INFO] [autopilot] event VehicleAppeared handle=5379
    2026-09-29T21:35:50.146Z [INFO] [autopilot] event VehicleAppeared handle=5635
    2026-09-29T21:35:50.165Z [INFO] [autopilot] event VehicleAppeared handle=5891
    2026-09-29T21:35:50.369Z [INFO] [autopilot] event PedAppeared handle=2819
    2026-09-29T21:35:50.408Z [INFO] [autopilot] event PedAppeared handle=3076
    2026-09-29T21:35:50.409Z [INFO] [autopilot] event PedAppeared handle=3331
    2026-09-29T21:35:50.410Z [INFO] [autopilot] event PedAppeared handle=3587
    2026-09-29T21:35:50.410Z [INFO] [autopilot] event PedAppeared handle=3844
    2026-09-29T21:35:50.433Z [INFO] [autopilot] event PedAppeared handle=4100
    2026-09-29T21:35:50.461Z [INFO] [autopilot] event PedAppeared handle=4357
    2026-09-29T21:35:50.462Z [INFO] [autopilot] event PedAppeared handle=4611
    2026-09-29T21:35:50.521Z [INFO] [autopilot] event PedAppeared handle=4867
    2026-09-29T21:35:50.543Z [INFO] [autopilot] event PedAppeared handle=5123
    2026-09-29T21:35:50.573Z [INFO] [autopilot] event PedAppeared handle=5379
    2026-09-29T21:35:50.677Z [INFO] [autopilot] event PedAppeared handle=5637
    2026-09-29T21:35:50.815Z [INFO] [autopilot] event VehicleAppeared handle=6148
    2026-09-29T21:35:50.828Z [INFO] [autopilot] event PedAppeared handle=5892
    2026-09-29T21:35:50.955Z [INFO] [autopilot] event VehicleAppeared handle=6403
    2026-09-29T21:35:50.999Z [INFO] [autopilot] event VehicleAppeared handle=6660
    2026-09-29T21:35:51.072Z [INFO] [autopilot] event VehicleAppeared handle=6915
    2026-09-29T21:35:54.997Z [INFO] [autopilot] event PedAppeared handle=6150
    2026-09-29T21:35:54.998Z [INFO] [autopilot] event PedAppeared handle=6405
    2026-09-29T21:35:55.279Z [INFO] [autopilot] event PedAppeared handle=6661
    2026-09-29T21:35:55.988Z [INFO] density frame_ms=168.1 peds=0.55 cars=0.60
    2026-09-29T21:35:56.602Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1849 core=on peds=19 vehicles=24 modules=10/10 coroutines=0 resources=2 raycast=on episode=GTAIV frame_ms=33.40 p95_ms=92.20 pressure=0.77 private_mb=2135 working_set_mb=950 address_free_mb=1306 largest_free_block_mb=1271 managed_mb=14 physical_load=94% core_us=71.6
    2026-09-29T21:35:56.776Z [INFO] performance samples=469 frame_p50_ms=36 frame_p95_ms=97 frame_p99_ms=200 frames_over_33ms=261 frames_over_50ms=167 gunplay_avg_ms=0.943 gunplay_max_ms=20.037 phase_samples=360 phase_setup_avg_ms=0.701 phase_setup_max_ms=5.557 phase_camera_avg_ms=0.028 phase_camera_max_ms=0.662 phase_bullets_avg_ms=0.005 phase_bullets_max_ms=0.272 phase_weapon_avg_ms=0.094 phase_weapon_max_ms=14.205 phase_hud_avg_ms=0.385 phase_hud_max_ms=0.577
    2026-09-29T21:35:56.777Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.125/352.3/469@7 total=1935 module.gunplay=0.992/23.2/469@7 total=465 tick.gunplay=0.991/23.2/469@7 total=465 module.arsenal=0.640/27.5/380@7 total=243 tick.arsenal=0.640/27.5/380@7 total=243 gp.freeaim=0.558/3.9/360@7 total=201 engine.scheduler=0.404/151.4/469@7 total=189 engine.world=0.385/41.8/469@7 total=181 module.holsters=0.529/53.6/267@7 total=141 tick.holsters=0.528/53.6/267@7 total=141 module.atmosphere=0.254/5.9/469@7 total=119 tick.atmosphere=0.253/5.9/469@7 total=118 ar.reconcile=0.283/26.5/273@7 total=77 ho.show=0.859/53.2/65@7 total=56 ar.safehouse_flags=0.193/4.4/273@7 total=53 ar.storage=0.150/1.7/273@7 total=41 module.combat=0.085/3.5/469@7 total=40 tick.combat=0.084/3.5/469@7 total=40 gp.shoulder=0.091/14.2/360@7 total=33 combat.sample=0.798/3.0/33@7 total=26 gp.index_pad=0.064/1.0/360@7 total=23 module.world=0.252/7.1/44@7 total=11 module.devtools=0.026/1.3/380@7 total=10 tick.devtools=0.026/1.3/380@7 total=10 cam.handle=0.023/0.6/360@7 total=8 gp.weapon_id=0.022/1.7/360@7 total=8 gp.player=0.011/0.1/360@7 total=4 module.probe=1.334/1.6/2@7 total=3 gp.cycle=0.007/0.0/360@7 total=3 gp.state=0.006/0.4/360@7 total=2 ar.lvs=0.004/0.2/380@7 total=1 ar.discover=0.004/0.5/380@7 total=1 ar.vehicle=0.003/0.0/380@7 total=1 cam.aim_key=0.003/0.0/360@7 total=1 cam.find_active=0.002/0.0/360@7 total=1 gp.spread=0.002/0.0/360@7 total=1 gp.shots=0.002/0.1/360@7 total=1 combat.dismember=0.001/0.0/469@7 total=0 combat.blood=0.000/0.0/469@7 total=0 module.autopilot=0.000/0.0/469@7 total=0 combat.pending=0.000/0.0/469@7 total=0 ho.carried=0.002/0.0/65@7 total=0 module.weapon-probe=0.000/0.0/469@7 total=0 gp.feel=0.000/0.0/360@7 total=0 cam.fov=0.001/0.0/50@7 total=0 gp.recoil=0.000/0.0/360@7 total=0
    2026-09-29T21:35:56.778Z [INFO] engine_thread_probe ticks=469 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=466 ticks_after_skipped_frames=3
    2026-09-29T21:35:56.780Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.48 shdn_us=33.3
    2026-09-29T21:35:57.099Z [INFO] [autopilot] event PedAppeared handle=6915
    2026-09-29T21:35:57.100Z [INFO] [autopilot] event PedAppeared handle=7171
    2026-09-29T21:35:57.626Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:35:57.667Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T21:35:57.687Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T21:35:58.367Z [INFO] command source=file:cmd_20260929213557987.cmd line="hud off" reply="hud off"
    2026-09-29T21:35:58.381Z [INFO] [autopilot] event PedAppeared handle=7427
    2026-09-29T21:35:58.957Z [INFO] command source=file:cmd_20260929213558631.cmd line="cam 0 2.2 0.4" reply="camera at 0 deg, 2.2 m"
    2026-09-29T21:35:59.188Z [INFO] [autopilot] event VehicleRemoved handle=516
    2026-09-29T21:35:59.768Z [INFO] [autopilot] event PedAppeared handle=7683
    2026-09-29T21:36:00.071Z [INFO] [autopilot] event VehicleRemoved handle=4611
    2026-09-29T21:36:00.372Z [INFO] [autopilot] event VehicleAppeared handle=3843
    2026-09-29T21:36:00.971Z [INFO] command source=file:cmd_20260929213600808.cmd line="alive" reply="player alive health=100"
    2026-09-29T21:36:01.440Z [INFO] [autopilot] event VehicleAppeared handle=4099
    2026-09-29T21:36:02.108Z [INFO] [autopilot] event PedRemoved handle=3331
    2026-09-29T21:36:02.201Z [INFO] [autopilot] event VehicleAppeared handle=7171
    2026-09-29T21:36:02.626Z [INFO] [autopilot] event PedAppeared handle=7939
    2026-09-29T21:36:02.751Z [INFO] [autopilot] event PedRemoved handle=6661
    2026-09-29T21:36:02.752Z [INFO] [autopilot] event PedRemoved handle=2819
    2026-09-29T21:36:03.026Z [INFO] [autopilot] event PedAppeared handle=3332
    2026-09-29T21:36:03.099Z [INFO] [autopilot] event PedAppeared handle=6662
    2026-09-29T21:36:03.557Z [INFO] camera_already_gone handle=3843 Invalid call to an object that doesn't exist anymore!
    2026-09-29T21:36:03.560Z [INFO] command source=file:cmd_20260929213603467.cmd line="cam 180 2.2 0.4" reply="camera at 180 deg, 2.2 m"
    2026-09-29T21:36:05.081Z [INFO] [autopilot] event PedRemoved handle=7171
    2026-09-29T21:36:05.082Z [INFO] [autopilot] event PedRemoved handle=6915
    2026-09-29T21:36:05.453Z [INFO] [autopilot] event VehicleAppeared handle=4612
    2026-09-29T21:36:05.517Z [INFO] [autopilot] event PedAppeared handle=6916
    2026-09-29T21:36:05.632Z [INFO] command source=file:cmd_20260929213605367.cmd line="alive" reply="player alive health=100"
    2026-09-29T21:36:06.230Z [INFO] [autopilot] event PedAppeared handle=7172
    2026-09-29T21:36:06.517Z [INFO] [autopilot] event PedAppeared handle=8196
    2026-09-29T21:36:06.629Z [INFO] [autopilot] event VehicleAppeared handle=8195
    2026-09-29T21:36:06.853Z [INFO] [autopilot] event PedRemoved handle=7939
    2026-09-29T21:36:07.069Z [INFO] [autopilot] event PedAppeared handle=8452
    2026-09-29T21:36:07.070Z [INFO] [autopilot] event PedAppeared handle=8708
    2026-09-29T21:36:07.457Z [INFO] [autopilot] event PedAppeared handle=8964
    2026-09-29T21:36:07.503Z [INFO] [autopilot] event PedRemoved handle=3844
    2026-09-29T21:36:07.641Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:36:07.936Z [INFO] [autopilot] event PedAppeared handle=7940
    2026-09-29T21:36:08.068Z [INFO] camera_already_gone handle=4099 Invalid call to an object that doesn't exist anymore!
    2026-09-29T21:36:08.070Z [INFO] command source=file:cmd_20260929213607822.cmd line="cam 90 2.0 0.4" reply="camera at 90 deg, 2 m"
    2026-09-29T21:36:08.659Z [INFO] [autopilot] event VehicleRemoved handle=7171
    2026-09-29T21:36:08.723Z [INFO] [autopilot] event PedRemoved handle=6916
    2026-09-29T21:36:09.254Z [INFO] [autopilot] event PedAppeared handle=9219
    2026-09-29T21:36:09.480Z [INFO] [autopilot] event VehicleRemoved handle=4612
    2026-09-29T21:36:09.791Z [INFO] [autopilot] event PedAppeared handle=9475
    2026-09-29T21:36:09.792Z [INFO] [autopilot] event PedAppeared handle=9731
    2026-09-29T21:36:09.999Z [INFO] command source=file:cmd_20260929213609738.cmd line="alive" reply="player alive health=100"
    2026-09-29T21:36:10.211Z [INFO] [autopilot] event VehicleAppeared handle=7683
    2026-09-29T21:36:10.483Z [INFO] [autopilot] event VehicleRemoved handle=4355
    2026-09-29T21:36:10.755Z [INFO] [autopilot] event PedRemoved handle=4611
    2026-09-29T21:36:10.967Z [INFO] [autopilot] event PedRemoved handle=8964
    2026-09-29T21:36:11.030Z [INFO] [autopilot] event PedRemoved handle=8708
    2026-09-29T21:36:11.525Z [INFO] [autopilot] event PedAppeared handle=6917
    2026-09-29T21:36:12.087Z [INFO] camera_already_gone handle=4354 Invalid call to an object that doesn't exist anymore!
    2026-09-29T21:36:12.089Z [INFO] command source=file:cmd_20260929213611837.cmd line="cam 270 2.0 0.4" reply="camera at 270 deg, 2 m"
    2026-09-29T21:36:12.293Z [INFO] [autopilot] event VehicleRemoved handle=3843
    2026-09-29T21:36:13.021Z [INFO] [autopilot] event VehicleAppeared handle=7940
    2026-09-29T21:36:13.320Z [INFO] [autopilot] event VehicleRemoved handle=4099
    2026-09-29T21:36:13.886Z [INFO] command source=file:cmd_20260929213613744.cmd line="alive" reply="player alive health=100"
    2026-09-29T21:36:14.227Z [INFO] [autopilot] event PedRemoved handle=8196
    2026-09-29T21:36:14.530Z [INFO] [autopilot] event VehicleRemoved handle=7683
    2026-09-29T21:36:14.684Z [INFO] [autopilot] event PedAppeared handle=8197
    2026-09-29T21:36:15.567Z [INFO] [autopilot] event VehicleRemoved handle=7940
    2026-09-29T21:36:15.590Z [INFO] [autopilot] event PedAppeared handle=8709
    2026-09-29T21:36:15.620Z [INFO] [autopilot] event VehicleAppeared handle=9218
    2026-09-29T21:36:15.991Z [INFO] camera_already_gone handle=4610 Invalid call to an object that doesn't exist anymore!
    2026-09-29T21:36:15.992Z [INFO] command source=file:cmd_20260929213615859.cmd line="cam 150 1.4 0.6" reply="camera at 150 deg, 1.4 m"
    2026-09-29T21:36:16.634Z [INFO] [autopilot] event PedAppeared handle=8965
    2026-09-29T21:36:17.422Z [INFO] [autopilot] event PedAppeared handle=9987
    2026-09-29T21:36:17.644Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T21:36:17.703Z [INFO] [autopilot] event PedRemoved handle=9731
    2026-09-29T21:36:17.704Z [INFO] [autopilot] event PedRemoved handle=9475
    2026-09-29T21:36:17.903Z [INFO] [autopilot] event PedRemoved handle=3332
    2026-09-29T21:36:17.966Z [INFO] [autopilot] event PedAppeared handle=3845
    2026-09-29T21:36:18.006Z [INFO] command source=file:cmd_20260929213617761.cmd line="alive" reply="player alive health=100"
    2026-09-29T21:36:18.155Z [INFO] [autopilot] event VehicleAppeared handle=8451
    2026-09-29T21:36:18.448Z [INFO] [autopilot] event PedAppeared handle=4612
    2026-09-29T21:36:18.769Z [INFO] [autopilot] event VehicleRemoved handle=5379
    2026-09-29T21:36:18.799Z [INFO] [autopilot] event VehicleAppeared handle=7684
    2026-09-29T21:36:19.485Z [INFO] [autopilot] event PedRemoved handle=8197
    2026-09-29T21:36:20.098Z [INFO] camera_already_gone handle=4866 Invalid call to an object that doesn't exist anymore!
    2026-09-29T21:36:20.098Z [INFO] command source=file:cmd_20260929213619891.cmd line="cam off" reply="camera off"
    2026-09-29T21:36:20.403Z [INFO] command source=file:cmd_20260929213620272.cmd line="hud on" reply="hud on"
    2026-09-29T21:36:20.536Z [INFO] [autopilot] event PedAppeared handle=8198

# Scenario sling-review

- Result: PASS
- Steps: 45, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- events on => event log on
- await-alive 60000 => waiting for live player
- expect autopilot_player_ready: OK 2026-09-29T22:06:00.394Z [INFO] [autopilot] autopilot_player_ready
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- expect teleport_done id=east_park: OK 2026-09-29T22:06:02.602Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
- wait 4000 ms
- god on => invincible True
- alive => player alive health=60
- pos => -64.421 674.834 14.568 heading 0 core_peds=11 core_vehicles=19
- owned autopilot => autopilot: invincible=1
- expect line=.owned autopilot. reply=.*invincible=1\b: OK 2026-09-29T22:06:08.207Z [INFO] command source=file:cmd_20260929220608186.cmd line="owned autopilot" reply="autopilot: invincible=1"
- time 13 0 => time 13:00
- weather 1 => weather 1
- mark log line 306
- give 10 200 => gave 10
- give 14 200 => gave 14
- give 7 100 => gave 7
- select 7 => selected 7
- expectmarked holster_sling_attached slot=LongGun1: OK 2026-09-29T22:06:09.615Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
- expectmarked holster_sling_attached slot=LongGun2: OK 2026-09-29T22:06:09.649Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
- hud off => hud off
- cam 0 2.2 0.4 => camera at 0 deg, 2.2 m
- wait 1500 ms
- alive => player alive health=60
- shot front -> front.jpg
- cam 180 2.2 0.4 => camera at 180 deg, 2.2 m
- wait 1500 ms
- alive => player alive health=60
- shot back -> back.jpg
- cam 90 2.0 0.4 => camera at 90 deg, 2 m
- wait 1500 ms
- alive => player alive health=60
- shot left -> left.jpg
- cam 270 2.0 0.4 => camera at 270 deg, 2 m
- wait 1500 ms
- alive => player alive health=60
- shot right -> right.jpg
- cam 150 1.4 0.6 => camera at 150 deg, 1.4 m
- wait 1500 ms
- alive => player alive health=60
- shot back_close -> back_close.jpg
- cam off => camera off
- hud on => hud on

## Errors

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T22:05:59.774Z [INFO] command source=file:cmd_20260929220559706.cmd line="events on" reply="event log on"
    2026-09-29T22:06:00.110Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T22:06:00.154Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.906, 658.84, 15.087) to=(-64.489, 674.682, 14.539)
    2026-09-29T22:06:00.155Z [INFO] [autopilot] event PedDamaged handle=2 100->80 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.489, 674.682, 14.539) dir=(0.089, 0.995, -0.034)
    2026-09-29T22:06:00.156Z [INFO] [autopilot] event PlayerDamaged 100->80
    2026-09-29T22:06:00.314Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.022, 658.851, 15.12) to=(-32.039, 965.88, 13.969)
    2026-09-29T22:06:00.315Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.164, 639.167, 15.15) to=(-81.469, 640.504, 15.119)
    2026-09-29T22:06:00.319Z [INFO] command source=file:cmd_20260929220600110.cmd line="await-alive 60000" reply="waiting for live player"
    2026-09-29T22:06:00.394Z [INFO] [autopilot] autopilot_player_ready
    2026-09-29T22:06:00.498Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.846, 640.292, 15.184) to=(315.517, 1333.625, 10.768)
    2026-09-29T22:06:00.499Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.027, 658.862, 15.113) to=(-64.335, 674.767, 14.613)
    2026-09-29T22:06:00.500Z [INFO] [autopilot] event PedDamaged handle=2 80->60 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.335, 674.767, 14.613) dir=(0.106, 0.994, -0.031)
    2026-09-29T22:06:00.501Z [INFO] [autopilot] event PlayerDamaged 80->60
    2026-09-29T22:06:00.593Z [INFO] command source=file:cmd_20260929220600541.cmd line="god on" reply="invincible True"
    2026-09-29T22:06:00.664Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.879, 640.282, 15.199) to=(-67.549, 669.978, 14.721)
    2026-09-29T22:06:00.664Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.037, 658.874, 15.126) to=(-64.286, 674.795, 14.488)
    2026-09-29T22:06:00.665Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.286, 674.795, 14.488) dir=(0.109, 0.993, -0.04)
    2026-09-29T22:06:00.916Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.876, 640.278, 15.189) to=(318.786, 1331.721, 16.403)
    2026-09-29T22:06:00.917Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.047, 658.889, 15.113) to=(16.423, 1454.776, -14.298)
    2026-09-29T22:06:01.010Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.871, 640.289, 15.198) to=(334.333, 1322.307, -1.103)
    2026-09-29T22:06:01.100Z [INFO] command source=file:cmd_20260929220600943.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T22:06:01.194Z [INFO] [autopilot] event PedAppeared handle=1797
    2026-09-29T22:06:01.194Z [INFO] [autopilot] event PedAppeared handle=2053
    2026-09-29T22:06:01.195Z [INFO] [autopilot] event PedAppeared handle=3332
    2026-09-29T22:06:01.196Z [INFO] [autopilot] event PedAppeared handle=5892
    2026-09-29T22:06:01.196Z [INFO] [autopilot] event PedAppeared handle=6148
    2026-09-29T22:06:01.197Z [INFO] [autopilot] event PedAppeared handle=6403
    2026-09-29T22:06:01.198Z [INFO] [autopilot] event PedAppeared handle=6659
    2026-09-29T22:06:01.199Z [INFO] [autopilot] event VehicleAppeared handle=1796
    2026-09-29T22:06:01.239Z [INFO] [autopilot] event VehicleAppeared handle=2052
    2026-09-29T22:06:01.366Z [INFO] teleport_stage id=east_park stage=move begin
    2026-09-29T22:06:01.369Z [INFO] teleport_stage id=east_park stage=move elapsed_ms=2
    2026-09-29T22:06:01.370Z [INFO] teleport_stage id=east_park stage=request_collision begin
    2026-09-29T22:06:01.371Z [INFO] teleport_stage id=east_park stage=request_collision elapsed_ms=0
    2026-09-29T22:06:01.372Z [INFO] teleport_stage id=east_park stage=load_scene begin
    2026-09-29T22:06:01.670Z [INFO] teleport_stage id=east_park stage=load_scene elapsed_ms=298
    2026-09-29T22:06:01.671Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T22:06:01.672Z [INFO] command source=file:cmd_20260929220601330.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T22:06:01.693Z [INFO] [autopilot] event VehicleAppeared handle=5378
    2026-09-29T22:06:01.694Z [INFO] [autopilot] event VehicleAppeared handle=5634
    2026-09-29T22:06:01.695Z [INFO] [autopilot] event VehicleRemoved handle=4610
    2026-09-29T22:06:01.695Z [INFO] [autopilot] event VehicleRemoved handle=3842
    2026-09-29T22:06:01.696Z [INFO] [autopilot] event VehicleRemoved handle=3586
    2026-09-29T22:06:01.810Z [INFO] [autopilot] event VehicleAppeared handle=7939
    2026-09-29T22:06:01.946Z [INFO] [autopilot] event PedRemoved handle=6659
    2026-09-29T22:06:01.947Z [INFO] [autopilot] event PedRemoved handle=5892
    2026-09-29T22:06:01.948Z [INFO] [autopilot] event PedRemoved handle=3332
    2026-09-29T22:06:01.948Z [INFO] [autopilot] event PedRemoved handle=2053
    2026-09-29T22:06:01.949Z [INFO] [autopilot] event PedRemoved handle=1797
    2026-09-29T22:06:01.950Z [INFO] [autopilot] event PedRemoved handle=6148
    2026-09-29T22:06:01.951Z [INFO] [autopilot] event PedRemoved handle=6403
    2026-09-29T22:06:02.498Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.321, 654.05, 15.06) to=(-63.872, 666.938, 13.639)
    2026-09-29T22:06:02.602Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T22:06:02.636Z [INFO] [autopilot] event VehicleAppeared handle=3586
    2026-09-29T22:06:02.636Z [INFO] [autopilot] event VehicleAppeared handle=3842
    2026-09-29T22:06:02.637Z [INFO] [autopilot] event VehicleAppeared handle=4610
    2026-09-29T22:06:02.638Z [INFO] [autopilot] event VehicleRemoved handle=5634
    2026-09-29T22:06:02.638Z [INFO] [autopilot] event VehicleRemoved handle=5378
    2026-09-29T22:06:02.688Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.432, 654.138, 15.067) to=(-5.124, 1450.431, 73.127)
    2026-09-29T22:06:03.090Z [INFO] [autopilot] event PedAppeared handle=2054
    2026-09-29T22:06:03.091Z [INFO] [autopilot] event PedAppeared handle=3333
    2026-09-29T22:06:03.189Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.878, 640.289, 15.177) to=(-67.549, 669.918, 14.852)
    2026-09-29T22:06:03.190Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-81.967, 640.583, 15.167) to=(-31.57, 742.449, 17.939)
    2026-09-29T22:06:03.320Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.799, 640.441, 15.133) to=(338.234, 1320.001, -12.731)
    2026-09-29T22:06:03.321Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.008, 640.7, 15.147) to=(296.867, 1345.84, -9.285)
    2026-09-29T22:06:03.511Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.491, 640.896, 15.137) to=(-67.549, 669.487, 14.519)
    2026-09-29T22:06:03.511Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.201, 640.929, 15.223) to=(-74.993, 654.759, 14.973)
    2026-09-29T22:06:03.630Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-84.271, 641.307, 15.104) to=(310.075, 1338.483, 9.241)
    2026-09-29T22:06:03.631Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.468, 641.189, 15.108) to=(-35.374, 725.407, 15.774)
    2026-09-29T22:06:03.670Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.074, 658.803, 15.085) to=(-64.248, 674.753, 13.981)
    2026-09-29T22:06:03.670Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x1A7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.248, 674.753, 13.981) dir=(0.113, 0.991, -0.069)
    2026-09-29T22:06:03.805Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-83.966, 641.906, 15.146) to=(-75.636, 654.977, 14.694)
    2026-09-29T22:06:03.806Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.352, 641.753, 15.114) to=(294.851, 1347.389, -20.55)
    2026-09-29T22:06:03.970Z [INFO] [autopilot] event PedAppeared handle=5893
    2026-09-29T22:06:03.971Z [INFO] [autopilot] event PedAppeared handle=6149
    2026-09-29T22:06:04.316Z [INFO] [autopilot] event PedAppeared handle=6404
    2026-09-29T22:06:04.786Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.745, 644.913, 15.104) to=(-67.549, 670.717, 14.769)
    2026-09-29T22:06:04.951Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-81.906, 645.499, 15.105) to=(-67.549, 670.363, 14.709)
    2026-09-29T22:06:05.136Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-81.641, 646.087, 15.148) to=(-52, 694.157, 15.304)
    2026-09-29T22:06:05.137Z [INFO] [autopilot] event VehicleDamaged handle=2052 1000->988 engine 1000->1000
    2026-09-29T22:06:05.259Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-81.4, 646.537, 15.108) to=(324.065, 1336.647, -14.088)
    2026-09-29T22:06:05.442Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-81.075, 647.033, 15.148) to=(-67.549, 669.008, 14.59)
    2026-09-29T22:06:05.578Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-80.857, 647.447, 15.11) to=(-55.962, 689.492, 15.176)
    2026-09-29T22:06:05.579Z [INFO] [autopilot] event VehicleDamaged handle=7939 1000->994 engine 1000->1000
    2026-09-29T22:06:05.683Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-79.455, 647.661, 15.13) to=(314.454, 1344.334, -15.985)
    2026-09-29T22:06:05.773Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-80.524, 648.105, 15.147) to=(324.505, 1338.904, 22.7)
    2026-09-29T22:06:05.819Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.859, 658.781, 15.081) to=(-2.236, 1456.506, -7.794)
    2026-09-29T22:06:05.819Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-79.266, 648.137, 15.117) to=(-75.208, 655.394, 14.945)
    2026-09-29T22:06:06.010Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.998, 658.809, 15.121) to=(9.116, 1455.565, -7.186)
    2026-09-29T22:06:06.011Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-78.97, 648.669, 15.141) to=(320.709, 1342.267, -4.49)
    2026-09-29T22:06:06.153Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.994, 658.809, 15.11) to=(-64.317, 674.83, 15.167)
    2026-09-29T22:06:06.153Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-78.769, 649.096, 15.118) to=(-75.186, 655.298, 15.022)
    2026-09-29T22:06:06.154Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.317, 674.83, 15.167) dir=(0.104, 0.995, 0.004)
    2026-09-29T22:06:06.311Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.996, 658.822, 15.12) to=(-64.372, 674.677, 14.756)
    2026-09-29T22:06:06.312Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.372, 674.677, 14.756) dir=(0.102, 0.995, -0.023)
    2026-09-29T22:06:06.497Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.009, 658.823, 15.115) to=(17.406, 1454.82, -5.238)
    2026-09-29T22:06:06.636Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.014, 658.832, 15.118) to=(-64.515, 674.751, 14.657)
    2026-09-29T22:06:06.637Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.515, 674.751, 14.657) dir=(0.094, 0.995, -0.029)
    2026-09-29T22:06:06.727Z [INFO] [autopilot] event PedRemoved handle=3076
    2026-09-29T22:06:07.106Z [INFO] command source=file:cmd_20260929220607040.cmd line="god on" reply="invincible True"
    2026-09-29T22:06:07.667Z [INFO] command source=file:cmd_20260929220607423.cmd line="alive" reply="player alive health=60"
    2026-09-29T22:06:07.939Z [INFO] command source=file:cmd_20260929220607801.cmd line="pos" reply="-64.421 674.834 14.568 heading 0 core_peds=11 core_vehicles=19"
    2026-09-29T22:06:08.207Z [INFO] command source=file:cmd_20260929220608186.cmd line="owned autopilot" reply="autopilot: invincible=1"
    2026-09-29T22:06:08.644Z [INFO] [autopilot] event VehicleRemoved handle=5122
    2026-09-29T22:06:08.709Z [INFO] command source=file:cmd_20260929220608576.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T22:06:08.883Z [INFO] [autopilot] event VehicleAppeared handle=7427
    2026-09-29T22:06:08.941Z [INFO] [autopilot] event PedRemoved handle=1283
    2026-09-29T22:06:08.963Z [INFO] [autopilot] event PedRemoved handle=3333
    2026-09-29T22:06:09.225Z [INFO] command source=file:cmd_20260929220608964.cmd line="weather 1" reply="weather 1"
    2026-09-29T22:06:09.486Z [INFO] command source=file:cmd_20260929220609365.cmd line="give 10 200" reply="gave 10"
    2026-09-29T22:06:09.508Z [INFO] [autopilot] event PlayerWeaponChanged 7->10
    2026-09-29T22:06:09.509Z [INFO] weapon_changed from=7 to=10 profile=vanilla
    2026-09-29T22:06:09.511Z [INFO] arsenal_gain id=10 owned=False mission=False
    2026-09-29T22:06:09.558Z [INFO] holsters_removed reason=overflow
    2026-09-29T22:06:09.559Z [INFO] arsenal_overflow id=16 to=fallback:-67282078:1413.7:188.8
    2026-09-29T22:06:09.584Z [INFO] [autopilot] event PlayerWeaponChanged 10->7
    2026-09-29T22:06:09.585Z [INFO] weapon_changed from=10 to=7 profile=vanilla
    2026-09-29T22:06:09.615Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T22:06:09.649Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T22:06:09.665Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.846, 658.742, 15.036) to=(-58.5, 754.34, 13.705)
    2026-09-29T22:06:09.792Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.17, 654.748, 15.148) to=(-67.506, 670.421, 14.956)
    2026-09-29T22:06:09.826Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.035, 658.832, 15.12) to=(6.817, 1455.643, -15.456)
    2026-09-29T22:06:09.938Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.272, 654.809, 15.193) to=(326.089, 1347.008, 11.028)
    2026-09-29T22:06:09.939Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.258, 654.172, 15.094) to=(-32.657, 973.183, 13.803)
    2026-09-29T22:06:09.967Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.038, 658.831, 15.112) to=(-64.161, 674.636, 14.797)
    2026-09-29T22:06:09.969Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.161, 674.636, 14.797) dir=(0.118, 0.993, -0.02)
    2026-09-29T22:06:09.973Z [INFO] command source=file:cmd_20260929220609755.cmd line="give 14 200" reply="gave 14"
    2026-09-29T22:06:09.999Z [INFO] [autopilot] event PlayerWeaponChanged 7->14
    2026-09-29T22:06:10.001Z [INFO] weapon_changed from=7 to=14 profile=vanilla
    2026-09-29T22:06:10.003Z [INFO] arsenal_gain id=14 owned=False mission=False
    2026-09-29T22:06:10.053Z [INFO] holsters_removed reason=overflow
    2026-09-29T22:06:10.054Z [INFO] arsenal_overflow id=18 to=fallback:-67282078:1413.7:188.8
    2026-09-29T22:06:10.103Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T22:06:10.124Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T22:06:10.164Z [INFO] [autopilot] event PlayerWeaponChanged 14->12
    2026-09-29T22:06:10.166Z [INFO] weapon_changed from=14 to=12 profile=vanilla
    2026-09-29T22:06:10.170Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T22:06:10.204Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.185, 654.128, 15.107) to=(-64.722, 674.881, 14.693)
    2026-09-29T22:06:10.205Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2307 vehicle=0 killed=False hit=True at=(-64.722, 674.881, 14.693) dir=(0.07, 0.997, -0.02)
    2026-09-29T22:06:10.235Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.036, 658.838, 15.123) to=(-64.55, 674.783, 15.079)
    2026-09-29T22:06:10.236Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.258, 654.819, 15.188) to=(-67.549, 669.152, 14.778)
    2026-09-29T22:06:10.236Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.55, 674.783, 15.079) dir=(0.093, 0.996, -0.003)
    2026-09-29T22:06:10.240Z [INFO] command source=file:cmd_20260929220610148.cmd line="give 7 100" reply="gave 7"
    2026-09-29T22:06:10.272Z [INFO] [autopilot] event PlayerWeaponChanged 12->7
    2026-09-29T22:06:10.275Z [INFO] weapon_changed from=12 to=7 profile=vanilla
    2026-09-29T22:06:10.363Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.055, 658.836, 15.119) to=(20.599, 1454.679, 3.886)
    2026-09-29T22:06:10.364Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.238, 654.837, 15.2) to=(319.968, 1350.581, 7.125)
    2026-09-29T22:06:10.364Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.175, 654.091, 15.119) to=(10.954, 1450.888, -0.307)
    2026-09-29T22:06:10.522Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.245, 654.856, 15.19) to=(-67.44, 669.325, 15)
    2026-09-29T22:06:10.523Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.172, 654.098, 15.131) to=(-56.004, 756.243, 13.752)
    2026-09-29T22:06:10.691Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.192, 654.101, 15.127) to=(12.458, 1450.975, 13.685)
    2026-09-29T22:06:10.753Z [INFO] command source=file:cmd_20260929220610539.cmd line="select 7" reply="selected 7"
    2026-09-29T22:06:10.822Z [INFO] [autopilot] event VehicleRemoved handle=6914
    2026-09-29T22:06:11.001Z [INFO] command source=file:cmd_20260929220610931.cmd line="hud off" reply="hud off"
    2026-09-29T22:06:11.052Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.167, 654.162, 15.195) to=(347.647, 1333.446, -1.109)
    2026-09-29T22:06:11.233Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.166, 654.161, 15.186) to=(-58.816, 682.506, 15.186)
    2026-09-29T22:06:11.234Z [INFO] [autopilot] event VehicleDamaged handle=7682 1000->988 engine 1000->1000
    2026-09-29T22:06:11.388Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.163, 654.171, 15.195) to=(-67.5, 669.535, 14.96)
    2026-09-29T22:06:11.528Z [INFO] command source=file:cmd_20260929220611307.cmd line="cam 0 2.2 0.4" reply="camera at 0 deg, 2.2 m"
    2026-09-29T22:06:12.405Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.137, 654.18, 15.179) to=(-67.549, 669.126, 14.714)
    2026-09-29T22:06:12.406Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.842, 658.773, 15.048) to=(-6.032, 1457.263, 18.709)
    2026-09-29T22:06:12.556Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.001, 658.838, 15.121) to=(-59.249, 749.731, 13.648)
    2026-09-29T22:06:12.720Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.013, 658.848, 15.108) to=(7.289, 1455.699, -12.033)
    2026-09-29T22:06:12.871Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.015, 658.848, 15.124) to=(-58.289, 723.709, 13.652)
    2026-09-29T22:06:13.031Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.029, 658.847, 15.116) to=(-64.371, 674.692, 14.859)
    2026-09-29T22:06:13.032Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.371, 674.692, 14.859) dir=(0.104, 0.994, -0.016)
    2026-09-29T22:06:13.231Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.03, 658.856, 15.119) to=(10.777, 1455.891, 14.467)
    2026-09-29T22:06:13.236Z [INFO] command source=file:cmd_20260929220613199.cmd line="alive" reply="player alive health=60"
    2026-09-29T22:06:13.395Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.139, 654.184, 15.18) to=(320.606, 1349.582, 11.029)
    2026-09-29T22:06:13.544Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.169, 654.178, 15.193) to=(-67.412, 668.837, 15.019)
    2026-09-29T22:06:13.721Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.165, 654.174, 15.184) to=(320.729, 1349.659, 20.308)
    2026-09-29T22:06:13.722Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.19, 654.707, 15.143) to=(-67.549, 670.696, 14.624)
    2026-09-29T22:06:13.885Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.165, 654.174, 15.198) to=(-64.467, 674.787, 15.144)
    2026-09-29T22:06:13.886Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.299, 654.801, 15.194) to=(335.84, 1341.181, 9.053)
    2026-09-29T22:06:13.887Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3586 vehicle=0 killed=False hit=True at=(-64.467, 674.787, 15.144) dir=(0.494, 0.87, -0.002)
    2026-09-29T22:06:14.074Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.181, 654.175, 15.192) to=(-67.446, 670.098, 14.996)
    2026-09-29T22:06:14.075Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.305, 654.799, 15.187) to=(320.848, 1349.861, -0.125)
    2026-09-29T22:06:14.255Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.3, 654.803, 15.198) to=(330.475, 1344.001, -8.42)
    2026-09-29T22:06:14.452Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.316, 654.809, 15.193) to=(-67.318, 670.261, 15.083)
    2026-09-29T22:06:14.638Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.31, 654.819, 15.193) to=(-67.549, 669.637, 14.732)
    2026-09-29T22:06:15.028Z [INFO] [autopilot] event VehicleAppeared handle=771
    2026-09-29T22:06:15.363Z [INFO] [autopilot] event PedRemoved handle=6149
    2026-09-29T22:06:15.473Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.14, 654.186, 15.18) to=(-67.549, 669.609, 14.741)
    2026-09-29T22:06:15.474Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.032, 658.86, 15.123) to=(10.898, 1455.292, -14.234)
    2026-09-29T22:06:15.652Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.175, 654.175, 15.197) to=(-62.752, 678.264, 15.402)
    2026-09-29T22:06:15.653Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.037, 658.862, 15.112) to=(-61.754, 708.311, 13.563)
    2026-09-29T22:06:15.659Z [INFO] camera_already_gone handle=3586 Invalid call to an object that doesn't exist anymore!
    2026-09-29T22:06:15.660Z [INFO] command source=file:cmd_20260929220615476.cmd line="cam 180 2.2 0.4" reply="camera at 180 deg, 2.2 m"
    2026-09-29T22:06:16.846Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.163, 654.177, 15.183) to=(-31.37, 730.922, 15.418)
    2026-09-29T22:06:16.847Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.035, 658.869, 15.125) to=(-56.016, 740.411, 13.737)
    2026-09-29T22:06:17.097Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.047, 658.872, 15.117) to=(-64.435, 674.702, 14.696)
    2026-09-29T22:06:17.098Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.435, 674.702, 14.696) dir=(0.101, 0.995, -0.026)
    2026-09-29T22:06:17.271Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.05, 658.879, 15.119) to=(15.135, 1455.254, 3.189)
    2026-09-29T22:06:17.387Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.048, 658.885, 15.112) to=(-64.461, 674.7, 15.036)
    2026-09-29T22:06:17.387Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.461, 674.7, 15.036) dir=(0.1, 0.995, -0.005)
    2026-09-29T22:06:17.429Z [INFO] command source=file:cmd_20260929220617370.cmd line="alive" reply="player alive health=60"
    2026-09-29T22:06:17.729Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.143, 654.187, 15.18) to=(-62.85, 678.232, 15.168)
    2026-09-29T22:06:17.729Z [INFO] [autopilot] event PedAppeared handle=4866
    2026-09-29T22:06:17.730Z [INFO] [autopilot] event PedAppeared handle=5122
    2026-09-29T22:06:17.731Z [INFO] [autopilot] event PedAppeared handle=5378
    2026-09-29T22:06:17.731Z [INFO] [autopilot] event PedAppeared handle=5634
    2026-09-29T22:06:17.923Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.173, 654.175, 15.194) to=(319.149, 1350.11, -3.371)
    2026-09-29T22:06:18.329Z [INFO] [autopilot] event PedRemoved handle=5893
    2026-09-29T22:06:18.980Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.144, 654.185, 15.182) to=(-67.549, 669.257, 14.62)
    2026-09-29T22:06:19.049Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.183, 654.687, 15.126) to=(-67.435, 669.974, 15.004)
    2026-09-29T22:06:19.173Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.307, 654.816, 15.198) to=(-67.549, 669.256, 14.669)
    2026-09-29T22:06:19.336Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.309, 654.82, 15.186) to=(-67.371, 670.014, 15.047)
    2026-09-29T22:06:19.527Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.849, 658.777, 15.057) to=(-12.803, 1457.293, -6.618)
    2026-09-29T22:06:19.527Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.304, 654.825, 15.196) to=(-67.414, 669.553, 15.018)
    2026-09-29T22:06:19.559Z [INFO] density frame_ms=161.2 peds=0.55 cars=0.60
    2026-09-29T22:06:19.696Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.011, 658.833, 15.121) to=(-64.332, 674.736, 14.582)
    2026-09-29T22:06:19.697Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.314, 654.836, 15.19) to=(-31.57, 728.763, 14.703)
    2026-09-29T22:06:19.698Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.332, 674.736, 14.582) dir=(0.105, 0.994, -0.034)
    2026-09-29T22:06:19.702Z [INFO] camera_already_gone handle=3842 Invalid call to an object that doesn't exist anymore!
    2026-09-29T22:06:19.708Z [INFO] command source=file:cmd_20260929220619469.cmd line="cam 90 2.0 0.4" reply="camera at 90 deg, 2 m"
    2026-09-29T22:06:19.989Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.017, 658.836, 15.111) to=(-64.512, 674.797, 14.577)
    2026-09-29T22:06:19.990Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.317, 654.84, 15.196) to=(-67.549, 669.775, 14.793)
    2026-09-29T22:06:19.991Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.512, 674.797, 14.577) dir=(0.094, 0.995, -0.033)
    2026-09-29T22:06:20.181Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.018, 658.848, 15.12) to=(17.19, 1455.256, 14.126)
    2026-09-29T22:06:20.185Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T22:06:20.185Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=2057 core=on peds=11 vehicles=19 modules=10/10 coroutines=0 resources=5 raycast=on episode=GTAIV frame_ms=59.13 p95_ms=75.45 pressure=0.88 private_mb=1962 working_set_mb=1864 address_free_mb=1459 largest_free_block_mb=1426 managed_mb=15 physical_load=95% core_us=63.7
    2026-09-29T22:06:20.270Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.14, 654.182, 15.182) to=(-67.549, 669.836, 14.614)
    2026-09-29T22:06:20.326Z [INFO] performance samples=589 frame_p50_ms=40 frame_p95_ms=74 frame_p99_ms=200 frames_over_33ms=423 frames_over_50ms=129 gunplay_avg_ms=0.964 gunplay_max_ms=10.940 phase_samples=589 phase_setup_avg_ms=0.689 phase_setup_max_ms=10.631 phase_camera_avg_ms=0.026 phase_camera_max_ms=0.616 phase_bullets_avg_ms=0.014 phase_bullets_max_ms=0.146 phase_weapon_avg_ms=0.017 phase_weapon_max_ms=0.177 phase_hud_avg_ms=0.218 phase_hud_max_ms=0.420
    2026-09-29T22:06:20.327Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=3.979/311.7/589@1 total=2344 module.gunplay=1.007/25.4/589@1 total=593 tick.gunplay=1.006/25.4/589@1 total=593 module.arsenal=0.818/65.9/567@1 total=464 tick.arsenal=0.817/65.9/567@1 total=463 gp.freeaim=0.553/10.5/589@1 total=326 module.holsters=0.823/62.1/321@1 total=264 tick.holsters=0.822/62.1/321@1 total=264 ar.storage=0.369/5.9/567@1 total=209 ho.show=0.630/62.1/316@1 total=199 engine.world=0.329/5.8/589@1 total=194 ar.reconcile=0.246/64.6/567@1 total=140 engine.scheduler=0.202/76.2/589@1 total=119 ar.safehouse_flags=0.174/2.1/567@1 total=99 module.atmosphere=0.164/5.1/589@1 total=97 tick.atmosphere=0.164/5.1/589@1 total=96 module.combat=0.116/2.0/589@1 total=69 tick.combat=0.116/2.0/589@1 total=68 combat.sample=0.799/1.9/66@1 total=53 gp.index_pad=0.067/0.2/589@1 total=39 module.devtools=0.028/3.2/567@1 total=16 tick.devtools=0.027/3.2/567@1 total=15 cam.handle=0.021/0.6/589@1 total=12 gp.weapon_id=0.016/0.9/589@1 total=10 gp.shoulder=0.014/0.2/589@1 total=9 module.world=0.139/0.4/52@1 total=7 gp.player=0.012/0.1/589@1 total=7 gp.cycle=0.007/0.0/589@1 total=4 module.probe=1.185/1.2/3@1 total=4 gp.state=0.005/0.1/589@1 total=3 ar.lvs=0.003/0.3/567@1 total=2 ar.vehicle=0.003/0.0/567@1 total=2 cam.aim_key=0.003/0.1/589@1 total=2 cam.find_active=0.002/0.1/589@1 total=1 ar.discover=0.003/0.5/567@1 total=1 gp.spread=0.002/0.0/589@1 total=1 gp.shots=0.002/0.0/589@1 total=1 ho.carried=0.002/0.0/316@1 total=1 combat.dismember=0.001/0.3/589@1 total=1 module.autopilot=0.000/0.0/589@1 total=0 combat.blood=0.000/0.0/589@1 total=0 combat.pending=0.000/0.0/589@1 total=0 gp.feel=0.000/0.0/589@1 total=0 cam.fov=0.001/0.0/97@1 total=0 module.weapon-probe=0.000/0.0/589@1 total=0 gp.recoil=0.000/0.0/589@1 total=0
    2026-09-29T22:06:20.327Z [INFO] engine_thread_probe ticks=589 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=587 ticks_after_skipped_frames=2
    2026-09-29T22:06:20.329Z [INFO] direct_native get_char_health direct=160 shdn=60 match=False direct_us=0.19 shdn_us=55.5
    2026-09-29T22:06:20.493Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.172, 654.18, 15.193) to=(-67.562, 669.699, 14.908)
    2026-09-29T22:06:20.641Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.174, 654.171, 15.188) to=(-67.554, 669.644, 14.884)
    2026-09-29T22:06:21.147Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.324, 659.347, 15.007) to=(-7.967, 1458.044, 14.585)
    2026-09-29T22:06:21.328Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.189, 659.99, 15.009) to=(-62.785, 755.781, 15.713)
    2026-09-29T22:06:21.482Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.166, 660.461, 15.029) to=(-16.689, 1459.865, 23.709)
    2026-09-29T22:06:21.607Z [INFO] command source=file:cmd_20260929220621369.cmd line="alive" reply="player alive health=60"
    2026-09-29T22:06:21.633Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.107, 661.072, 15.021) to=(-64.336, 674.693, 14.736)
    2026-09-29T22:06:21.634Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.336, 674.693, 14.736) dir=(0.056, 0.998, -0.021)
    2026-09-29T22:06:21.802Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.142, 654.185, 15.181) to=(303.322, 1359.232, 3.193)
    2026-09-29T22:06:21.803Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.122, 661.649, 15.03) to=(-63.015, 756.005, 14.782)
    2026-09-29T22:06:21.803Z [INFO] [autopilot] event PedAppeared handle=1541
    2026-09-29T22:06:21.938Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.176, 654.175, 15.197) to=(310.771, 1354.818, -5.773)
    2026-09-29T22:06:22.107Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.173, 654.171, 15.188) to=(-67.419, 670.185, 15.015)
    2026-09-29T22:06:22.289Z [INFO] [autopilot] event PedRemoved handle=1541
    2026-09-29T22:06:22.803Z [INFO] [autopilot] event PedRemoved handle=2054
    2026-09-29T22:06:23.047Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.144, 654.187, 15.181) to=(318.244, 1350.71, -0.518)
    2026-09-29T22:06:23.048Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.199, 654.759, 15.154) to=(-67.549, 669.72, 14.761)
    2026-09-29T22:06:23.208Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.311, 654.835, 15.198) to=(343.176, 1336.667, -0.119)
    2026-09-29T22:06:23.376Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.309, 654.835, 15.186) to=(338.124, 1339.593, -3.937)
    2026-09-29T22:06:23.526Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.305, 654.843, 15.199) to=(338.083, 1339.771, 3.501)
    2026-09-29T22:06:23.673Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.322, 654.849, 15.193) to=(-67.561, 669.331, 14.905)
    2026-09-29T22:06:23.738Z [INFO] camera_already_gone handle=4098 Invalid call to an object that doesn't exist anymore!
    2026-09-29T22:06:23.740Z [INFO] command source=file:cmd_20260929220623512.cmd line="cam 270 2.0 0.4" reply="camera at 270 deg, 2 m"
    2026-09-29T22:06:23.771Z [INFO] [autopilot] event PedAppeared handle=1798
    2026-09-29T22:06:24.855Z [INFO] [autopilot] event PedRemoved handle=1798
    2026-09-29T22:06:25.612Z [INFO] command source=file:cmd_20260929220625412.cmd line="alive" reply="player alive health=60"
    2026-09-29T22:06:25.800Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.143, 654.185, 15.18) to=(-67.55, 669.957, 14.875)
    2026-09-29T22:06:26.028Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.175, 654.176, 15.195) to=(319.239, 1349.847, -14.29)
    2026-09-29T22:06:26.117Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.451, 655.024, 14.789) to=(349.99, 1332.733, 3.694)
    2026-09-29T22:06:26.624Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.173, 654.171, 15.187) to=(303.017, 1359.34, 0.904)
    2026-09-29T22:06:26.720Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.466, 655.015, 14.794) to=(338.43, 1339.679, 26.059)
    2026-09-29T22:06:26.834Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.169, 654.18, 15.197) to=(-67.549, 669.177, 14.766)
    2026-09-29T22:06:27.960Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.42, 655.036, 14.858) to=(342.994, 1336.974, 9.777)
    2026-09-29T22:06:28.642Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.177, 654.179, 15.192) to=(315.251, 1352.561, 8.68)
    2026-09-29T22:06:28.643Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.82, 666.121, 15.056) to=(-59.322, 723.159, 13.597)
    2026-09-29T22:06:28.652Z [INFO] camera_already_gone handle=4354 Invalid call to an object that doesn't exist anymore!
    2026-09-29T22:06:28.653Z [INFO] command source=file:cmd_20260929220628498.cmd line="cam 150 1.4 0.6" reply="camera at 150 deg, 1.4 m"
    2026-09-29T22:06:28.777Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.328, 655.079, 15.026) to=(-67.473, 669.774, 14.979)
    2026-09-29T22:06:29.322Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-65.826, 658.745, 15.056) to=(-54.862, 855.522, 13.758)
    2026-09-29T22:06:29.423Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.986, 666.212, 15.117) to=(-64.531, 674.748, 14.897)
    2026-09-29T22:06:29.424Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.531, 674.748, 14.897) dir=(0.168, 0.985, -0.025)
    2026-09-29T22:06:29.464Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.274, 655.104, 15.163) to=(333.825, 1342.8, 16.759)
    2026-09-29T22:06:29.523Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.986, 666.213, 15.106) to=(-64.491, 674.707, 15.014)
    2026-09-29T22:06:29.523Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.002, 658.821, 15.122) to=(-64.462, 674.711, 14.677)
    2026-09-29T22:06:29.524Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.491, 674.707, 15.014) dir=(0.173, 0.985, -0.011)
    2026-09-29T22:06:29.525Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2307 vehicle=0 killed=False hit=True at=(-64.462, 674.711, 14.677) dir=(0.096, 0.995, -0.028)
    2026-09-29T22:06:29.526Z [INFO] [autopilot] event VehicleAppeared handle=515
    2026-09-29T22:06:30.157Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.984, 666.22, 15.119) to=(-64.577, 674.797, 14.765)
    2026-09-29T22:06:30.157Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.002, 658.822, 15.11) to=(17.209, 1454.674, -14.464)
    2026-09-29T22:06:30.158Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.577, 674.797, 14.765) dir=(0.162, 0.986, -0.041)
    2026-09-29T22:06:30.223Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T22:06:30.271Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.994, 666.222, 15.108) to=(85.826, 1451.311, -27.897)
    2026-09-29T22:06:30.272Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.003, 658.835, 15.119) to=(-42.174, 931.277, 13.75)
    2026-09-29T22:06:30.414Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.016, 658.828, 15.117) to=(24.608, 1454.372, 7.3)
    2026-09-29T22:06:30.599Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-66.017, 658.839, 15.118) to=(-64.301, 674.721, 14.656)
    2026-09-29T22:06:30.599Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2307 vehicle=0 killed=False hit=True at=(-64.301, 674.721, 14.656) dir=(0.107, 0.994, -0.029)
    2026-09-29T22:06:30.747Z [INFO] command source=file:cmd_20260929220630459.cmd line="alive" reply="player alive health=60"
    2026-09-29T22:06:31.032Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.143, 654.184, 15.183) to=(317.029, 1351.366, -2.06)
    2026-09-29T22:06:31.033Z [INFO] [autopilot] event PedAppeared handle=6150
    2026-09-29T22:06:31.252Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.178, 654.175, 15.198) to=(-67.549, 669.509, 14.675)
    2026-09-29T22:06:31.420Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.174, 654.171, 15.188) to=(-67.549, 669.079, 14.788)
    2026-09-29T22:06:31.472Z [INFO] [autopilot] event PedRemoved handle=6150
    2026-09-29T22:06:31.549Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.413, 654.974, 15.183) to=(331.442, 1344.014, 28.718)
    2026-09-29T22:06:31.733Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.446, 654.969, 15.195) to=(350.498, 1332.255, 2.549)
    2026-09-29T22:06:31.928Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.444, 654.965, 15.185) to=(-67.526, 669.006, 14.943)
    2026-09-29T22:06:32.087Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.439, 654.969, 15.197) to=(-67.549, 669.664, 14.611)
    2026-09-29T22:06:32.263Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.448, 654.974, 15.187) to=(-67.549, 670.157, 14.842)
    2026-09-29T22:06:32.508Z [INFO] [autopilot] event BulletFired shooter=5378 weapon=15 by_player=False from=(-45.169, 694.777, 15.176) to=(-589.9, 107.899, 9.853)
    2026-09-29T22:06:32.509Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-44.935, 693.735, 15.265) to=(-625.647, 142.266, 20.667)
    2026-09-29T22:06:32.510Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.142, 654.184, 15.181) to=(313.155, 1353.671, -1.306)
    2026-09-29T22:06:32.674Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-44.8, 693.319, 15.141) to=(-49.248, 688.862, 15.072)
    2026-09-29T22:06:32.675Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.176, 654.175, 15.197) to=(-56.017, 688.654, 15.012)
    2026-09-29T22:06:32.676Z [INFO] [autopilot] event VehicleDamaged handle=2052 988->982 engine 1000->1000
    2026-09-29T22:06:32.676Z [INFO] [autopilot] event VehicleDamaged handle=7682 988->982 engine 1000->1000
    2026-09-29T22:06:32.677Z [INFO] [autopilot] event VehicleDamaged handle=7939 994->988 engine 1000->1000
    2026-09-29T22:06:32.709Z [INFO] [autopilot] event BulletFired shooter=5378 weapon=15 by_player=False from=(-45.008, 694.359, 15.293) to=(-49.331, 689.773, 15.064)
    2026-09-29T22:06:32.710Z [INFO] [autopilot] event VehicleDamaged handle=2052 982->970 engine 1000->1000
    2026-09-29T22:06:32.711Z [INFO] [autopilot] event VehicleDamaged handle=7682 982->976 engine 1000->1000
    2026-09-29T22:06:32.823Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-44.766, 692.835, 15.144) to=(-634.496, 151.244, 18.82)
    2026-09-29T22:06:32.870Z [INFO] [autopilot] event BulletFired shooter=5378 weapon=15 by_player=False from=(-44.932, 694.018, 15.152) to=(-130.894, 609.517, 17.11)
    2026-09-29T22:06:32.871Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.174, 654.171, 15.187) to=(317.553, 1350.991, -4.753)
    2026-09-29T22:06:32.995Z [INFO] [autopilot] event BulletFired shooter=5378 weapon=15 by_player=False from=(-44.889, 693.542, 15.141) to=(-49.646, 688.692, 14.997)
    2026-09-29T22:06:32.996Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-44.741, 692.421, 15.277) to=(-58.831, 679.673, 14.345)
    2026-09-29T22:06:32.997Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.169, 654.174, 15.2) to=(-64.373, 674.764, 15.195)
    2026-09-29T22:06:32.998Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3586 vehicle=0 killed=False hit=True at=(-64.373, 674.764, 15.195) dir=(0.497, 0.868, 0)
    2026-09-29T22:06:32.999Z [INFO] [autopilot] event VehicleDamaged handle=2052 970->964 engine 1000->1000
    2026-09-29T22:06:32.999Z [INFO] [autopilot] event VehicleDamaged handle=7682 976->964 engine 1000->988
    2026-09-29T22:06:33.084Z [INFO] camera_already_gone handle=4610 Invalid call to an object that doesn't exist anymore!
    2026-09-29T22:06:33.085Z [INFO] command source=file:cmd_20260929220632848.cmd line="cam off" reply="camera off"
    2026-09-29T22:06:33.178Z [INFO] [autopilot] event BulletFired shooter=5378 weapon=15 by_player=False from=(-44.817, 693.136, 15.254) to=(-57.794, 680.435, 14.992)
    2026-09-29T22:06:33.179Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-44.673, 692.064, 15.175) to=(-77.849, 662.085, 15.805)
    2026-09-29T22:06:33.180Z [INFO] [autopilot] event BulletFired shooter=3586 weapon=15 by_player=False from=(-76.185, 654.176, 15.194) to=(-67.562, 669.98, 14.906)
    2026-09-29T22:06:33.180Z [INFO] [autopilot] event VehicleDamaged handle=7682 964->958 engine 988->988
    2026-09-29T22:06:33.260Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.862, 666.143, 15.086) to=(31.356, 1460.153, -17.214)
    2026-09-29T22:06:33.302Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.416, 654.981, 15.181) to=(336.141, 1341.23, 16.836)
    2026-09-29T22:06:33.342Z [INFO] [autopilot] event BulletFired shooter=5378 weapon=15 by_player=False from=(-44.745, 692.756, 15.209) to=(-58.859, 679.598, 14.672)
    2026-09-29T22:06:33.343Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-44.677, 691.572, 15.161) to=(-58.847, 679.542, 14.587)
    2026-09-29T22:06:33.344Z [INFO] [autopilot] event VehicleDamaged handle=7682 958->946 engine 988->976
    2026-09-29T22:06:33.348Z [INFO] command source=file:cmd_20260929220633236.cmd line="hud on" reply="hud on"
    2026-09-29T22:06:33.440Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.005, 666.204, 15.122) to=(68.153, 1454.603, -22.406)
    2026-09-29T22:06:33.485Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.447, 654.971, 15.194) to=(-67.549, 669.324, 14.706)
    2026-09-29T22:06:33.605Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.014, 666.212, 15.106) to=(-64.596, 674.81, 14.902)
    2026-09-29T22:06:33.606Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-76.444, 654.966, 15.185) to=(351.109, 1332.056, 11.112)
    2026-09-29T22:06:33.606Z [INFO] [autopilot] event BulletFired shooter=2307 weapon=15 by_player=False from=(-65.838, 658.74, 15.053) to=(-21.122, 1458.149, 12.882)
    2026-09-29T22:06:33.607Z [INFO] [autopilot] event PedDamaged handle=2 60->60 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.596, 674.81, 14.902) dir=(0.163, 0.986, -0.023)

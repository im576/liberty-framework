# Scenario sdk-selftest

- Result: FAIL
- Steps: 13, failed: 1
- Game alive at end: True
- Log errors during run: 1

## Failed steps
- expect selftest_done passed=\d+ failed=0: no new log line in 90 s

## Steps
- launch: engine booted on attempt 1
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 5000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- modules => gunplay avg=1.067 max=180.96 int=0 | combat avg=0.077 max=145.99 int=0 | arsenal avg=0.761 max=108.99 int=30 | holsters avg=0.710 max=71.05 int=50 | atmosphere avg=0.164 max=7.78 int=0 | devtools avg=0.037 max=4.97 int=30 | probe avg=1.373 max=1.40 int=10000 | weapon-probe avg=0.000 max=0.01 int=0 | world avg=0.264 max=2.84 int=500 | autopilot avg=0.000 max=0.02 int=0
- selftest => selftest started
- FAILED: expect selftest_done passed=\d+ failed=0: no new log line in 90 s
- perf => frame_ms=19.85 p95_ms=22.33 pressure=0.00 private_mb=2138 working_set_mb=1343 address_free_mb=1260 largest_free_block_mb=1204 managed_mb=14 physical_load=96% core_us=102.3
- pools => peds=30/120 vehicles=39/140 objects=230/1300
- owned autopilot => autopilot: invincible=1 weather=1

## Errors
    2026-09-29T05:03:45.318Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:03:00.044Z [INFO] weapon_changed from=3 to=7 profile=vanilla
    2026-09-29T05:03:00.243Z [INFO] weapon_changed from=7 to=12 profile=vanilla
    2026-09-29T05:03:00.329Z [INFO] weapon_changed from=12 to=16 profile=vanilla
    2026-09-29T05:03:00.501Z [INFO] weapon_changed from=16 to=5 profile=vanilla
    2026-09-29T05:03:00.535Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T05:03:00.642Z [INFO] weapon_changed from=5 to=0 profile=vanilla
    2026-09-29T05:03:00.768Z [INFO] weapon_changed from=0 to=5 profile=vanilla
    2026-09-29T05:03:00.943Z [INFO] command source=file:cmd_20260929050300765.cmd line="events on" reply="event log on"
    2026-09-29T05:03:00.994Z [INFO] [autopilot] event PlayerWeaponChanged 5->16
    2026-09-29T05:03:00.996Z [INFO] weapon_changed from=5 to=16 profile=vanilla
    2026-09-29T05:03:01.175Z [INFO] [autopilot] event PlayerWeaponChanged 16->12
    2026-09-29T05:03:01.177Z [INFO] weapon_changed from=16 to=12 profile=vanilla
    2026-09-29T05:03:01.202Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T05:03:01.215Z [INFO] command source=file:cmd_20260929050301170.cmd line="god on" reply="invincible True"
    2026-09-29T05:03:01.277Z [INFO] [autopilot] event PlayerWeaponChanged 12->7
    2026-09-29T05:03:01.278Z [INFO] weapon_changed from=12 to=7 profile=vanilla
    2026-09-29T05:03:01.372Z [INFO] [autopilot] event PlayerWeaponChanged 7->12
    2026-09-29T05:03:01.374Z [INFO] weapon_changed from=7 to=12 profile=vanilla
    2026-09-29T05:03:01.398Z [INFO] [autopilot] event VehicleRemoved handle=2306
    2026-09-29T05:03:01.706Z [INFO] command source=file:cmd_20260929050301567.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:03:01.795Z [INFO] [autopilot] event PlayerWeaponChanged 12->16
    2026-09-29T05:03:01.797Z [INFO] weapon_changed from=12 to=16 profile=vanilla
    2026-09-29T05:03:05.182Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:03:05.182Z [INFO] command source=file:cmd_20260929050301960.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:03:05.227Z [INFO] [autopilot] event ReloadFinished weapon=16 clip_after=10
    2026-09-29T05:03:05.229Z [INFO] [autopilot] event PedRemoved handle=2306
    2026-09-29T05:03:05.230Z [INFO] [autopilot] event PedRemoved handle=1794
    2026-09-29T05:03:05.231Z [INFO] [autopilot] event PedRemoved handle=258
    2026-09-29T05:03:05.232Z [INFO] [autopilot] event PedRemoved handle=2050
    2026-09-29T05:03:05.233Z [INFO] [autopilot] event PedRemoved handle=514
    2026-09-29T05:03:05.234Z [INFO] [autopilot] event VehicleRemoved handle=1795
    2026-09-29T05:03:05.235Z [INFO] [autopilot] event VehicleRemoved handle=1539
    2026-09-29T05:03:05.235Z [INFO] [autopilot] event VehicleRemoved handle=771
    2026-09-29T05:03:05.236Z [INFO] [autopilot] event VehicleRemoved handle=515
    2026-09-29T05:03:05.237Z [INFO] [autopilot] event VehicleRemoved handle=2562
    2026-09-29T05:03:05.237Z [INFO] [autopilot] event VehicleRemoved handle=1282
    2026-09-29T05:03:05.238Z [INFO] [autopilot] event VehicleRemoved handle=1026
    2026-09-29T05:03:05.239Z [INFO] [autopilot] event VehicleRemoved handle=2
    2026-09-29T05:03:07.409Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:03:07.933Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:03:08.151Z [INFO] [autopilot] event VehicleAppeared handle=2307
    2026-09-29T05:03:08.152Z [INFO] [autopilot] event VehicleAppeared handle=2563
    2026-09-29T05:03:08.153Z [INFO] [autopilot] event VehicleAppeared handle=2818
    2026-09-29T05:03:08.154Z [INFO] [autopilot] event VehicleAppeared handle=3074
    2026-09-29T05:03:08.154Z [INFO] [autopilot] event VehicleAppeared handle=3330
    2026-09-29T05:03:08.155Z [INFO] [autopilot] event VehicleAppeared handle=3586
    2026-09-29T05:03:08.156Z [INFO] [autopilot] event VehicleAppeared handle=3842
    2026-09-29T05:03:08.157Z [INFO] [autopilot] event VehicleAppeared handle=4354
    2026-09-29T05:03:08.157Z [INFO] [autopilot] event VehicleAppeared handle=4610
    2026-09-29T05:03:08.158Z [INFO] [autopilot] event VehicleAppeared handle=5122
    2026-09-29T05:03:08.159Z [INFO] [autopilot] event VehicleAppeared handle=5890
    2026-09-29T05:03:08.159Z [INFO] [autopilot] event VehicleAppeared handle=6146
    2026-09-29T05:03:08.160Z [INFO] [autopilot] event VehicleAppeared handle=6402
    2026-09-29T05:03:08.161Z [INFO] [autopilot] event VehicleAppeared handle=6658
    2026-09-29T05:03:08.161Z [INFO] [autopilot] event VehicleAppeared handle=6914
    2026-09-29T05:03:08.369Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=1028 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T05:03:08.753Z [INFO] [autopilot] event PedAppeared handle=515
    2026-09-29T05:03:08.809Z [INFO] [autopilot] event PedAppeared handle=1795
    2026-09-29T05:03:09.021Z [INFO] [autopilot] event PedAppeared handle=2051
    2026-09-29T05:03:09.022Z [INFO] [autopilot] event PedAppeared handle=2307
    2026-09-29T05:03:09.199Z [INFO] [autopilot] event PedAppeared handle=2562
    2026-09-29T05:03:09.257Z [INFO] [autopilot] event PedAppeared handle=2818
    2026-09-29T05:03:09.560Z [INFO] [autopilot] event PedRemoved handle=515
    2026-09-29T05:03:10.492Z [INFO] [autopilot] event PedAppeared handle=3074
    2026-09-29T05:03:10.802Z [INFO] command source=file:cmd_20260929050310557.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:03:11.059Z [INFO] command source=file:cmd_20260929050310936.cmd line="weather 1" reply="weather 1"
    2026-09-29T05:03:11.278Z [INFO] [autopilot] event PedRemoved handle=2307
    2026-09-29T05:03:11.279Z [INFO] [autopilot] event PedRemoved handle=3074
    2026-09-29T05:03:11.340Z [INFO] command source=file:cmd_20260929050311312.cmd line="modules" reply="gunplay avg=1.067 max=180.96 int=0 | combat avg=0.077 max=145.99 int=0 | arsenal avg=0.761 max=108.99 int=30 | holsters avg=0.710 max=71.05 int=50 | atmosphere avg=0.164 max=7.78 int=0 | devtools avg=0.037 max=4.97 int=30 | probe avg=1.373 max=1.40 int=10000 | weapon-probe avg=0.000 max=0.01 int=0 | world avg=0.264 max=2.84 int=500 | autopilot avg=0.000 max=0.02 int=0"
    2026-09-29T05:03:11.677Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.581, 649.126, 15.199) to=(-67.515, 660.321, 14.95)
    2026-09-29T05:03:11.844Z [INFO] [autopilot] event PedRemoved handle=2051
    2026-09-29T05:03:11.892Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.584, 649.12, 15.189) to=(-67.479, 658.986, 14.975)
    2026-09-29T05:03:11.896Z [INFO] command source=file:cmd_20260929050311696.cmd line="selftest" reply="selftest started"
    2026-09-29T05:03:11.969Z [INFO] [autopilot] selftest_begin engine=1.1.0 sdk=1.1.0
    2026-09-29T05:03:11.971Z [INFO] [autopilot] selftest world ok from_core=True peds=4 vehicles=15
    2026-09-29T05:03:11.973Z [INFO] [autopilot] selftest player ok ped=2 index=0
    2026-09-29T05:03:11.975Z [INFO] [autopilot] selftest modules ok loaded=10
    2026-09-29T05:03:11.977Z [INFO] [autopilot] selftest perf ok frame_ms=57.8 free_mb=1574 pressure=0.87
    2026-09-29T05:03:11.978Z [INFO] [autopilot] selftest input info pad=False
    2026-09-29T05:03:11.978Z [INFO] [autopilot] selftest episode info GTAIV
    2026-09-29T05:03:11.980Z [INFO] [autopilot] selftest query-ground ok ground=13.56 water=False 0.00
    2026-09-29T05:03:11.981Z [INFO] [autopilot] selftest raycast-available ok
    2026-09-29T05:03:11.985Z [INFO] [autopilot] selftest raycast-ground ok Hit World at (-64.422, 674.834, 13.562) distance 1.51 normal=(-0.054, -0.002, 0.999) ground_z=13.56
    2026-09-29T05:03:11.986Z [INFO] [autopilot] selftest raycast-clear ok Clear
    2026-09-29T05:03:11.988Z [INFO] [autopilot] selftest raycast-invalid ok from == to is refused
    2026-09-29T05:03:11.990Z [INFO] [autopilot] selftest capability-memory ok IMemory refused without memory.patch
    2026-09-29T05:03:11.993Z [INFO] [autopilot] selftest capability-natives ok INatives refused without engine.internal
    2026-09-29T05:03:12.002Z [INFO] [autopilot] selftest config ok <game>\scripts\LibertyFramework\config\autopilot\selftest.json
    2026-09-29T05:03:12.017Z [INFO] [autopilot] selftest state ok
    2026-09-29T05:03:12.019Z [INFO] [autopilot] selftest weapon-model ok model=0xF44C839D slot=2
    2026-09-29T05:03:12.023Z [INFO] [autopilot] selftest weapons ok inventory=6
    2026-09-29T05:03:12.025Z [INFO] [autopilot] selftest prop-create ok handle=17668
    2026-09-29T05:03:12.027Z [INFO] [autopilot] selftest prop-position ok at=(-64.422, 676.334, 15.568)
    2026-09-29T05:03:12.029Z [INFO] weapon_changed from=16 to=7 profile=vanilla
    2026-09-29T05:03:12.113Z [INFO] [autopilot] event PlayerWeaponChanged 16->7
    2026-09-29T05:03:12.138Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T05:03:12.176Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.588, 649.137, 15.199) to=(-64.353, 674.751, 15.19)
    2026-09-29T05:03:12.179Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.353, 674.751, 15.19) dir=(0.2, 0.98, 0)
    2026-09-29T05:03:12.181Z [INFO] [autopilot] event VehicleAppeared handle=516
    2026-09-29T05:03:12.239Z [INFO] [autopilot] event PedAppeared handle=2308
    2026-09-29T05:03:12.300Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.589, 649.147, 15.186) to=(-39.39, 795.222, 14.441)
    2026-09-29T05:03:12.302Z [INFO] [autopilot] event VehicleDamaged handle=2307 1000->994 engine 1000->1000
    2026-09-29T05:03:12.304Z [INFO] [autopilot] selftest attach-rotation ok heading_delta=90.0 (90 requested; the game itself takes radians)
    2026-09-29T05:03:12.305Z [INFO] [autopilot] selftest attach-rotation-units info degrees heading_delta=90.0 (90 requested)
    2026-09-29T05:03:12.307Z [INFO] [autopilot] selftest prop-delete ok
    2026-09-29T05:03:12.375Z [INFO] [autopilot] event PedAppeared handle=3075
    2026-09-29T05:03:12.377Z [INFO] [autopilot] selftest ped-spawn ok handle=3075
    2026-09-29T05:03:12.378Z [INFO] [autopilot] selftest ped-health ok health=150
    2026-09-29T05:03:12.527Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.586, 649.144, 15.197) to=(83.585, 1434.95, 8.691)
    2026-09-29T05:03:12.655Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.573, 649.137, 15.183) to=(74.127, 1436.751, 2.468)
    2026-09-29T05:03:12.656Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-69.775, 654.338, 15.192) to=(-31.884, 794.377, 16.938)
    2026-09-29T05:03:12.722Z [INFO] [autopilot] selftest ped-bone ok head=(-64.351, 678.167, 15.858) origin=(-64.422, 678.334, 15.13)
    2026-09-29T05:03:12.725Z [INFO] [autopilot] selftest ped-weapon ok
    2026-09-29T05:03:12.861Z [INFO] [autopilot] selftest ped-snapshot ok in_snapshot distance=3.5
    2026-09-29T05:03:12.870Z [INFO] [autopilot] selftest query-radius ok peds_within_10m=1
    2026-09-29T05:03:12.872Z [INFO] [autopilot] selftest query-cone ok found=3075
    2026-09-29T05:03:12.875Z [INFO] [autopilot] selftest query-onscreen ok on_screen=True
    2026-09-29T05:03:12.876Z [INFO] [autopilot] selftest raycast-ped ok Hit Ped 3075 at (-64.422, 678.084, 14.591) distance 3.25
    2026-09-29T05:03:12.876Z [INFO] [autopilot] selftest raycast-pass-through ok Clear passed=1 tests=2
    2026-09-29T05:03:12.877Z [INFO] [autopilot] selftest raycast-ignore ok Clear
    2026-09-29T05:03:12.880Z [INFO] [autopilot] selftest line-of-sight-ped ok
    2026-09-29T05:03:12.972Z [INFO] [autopilot] event PedRemoved handle=2308
    2026-09-29T05:03:13.129Z [INFO] [autopilot] event PedAppeared handle=6658
    2026-09-29T05:03:13.168Z [INFO] [autopilot] event PedAppeared handle=6914
    2026-09-29T05:03:13.211Z [INFO] [autopilot] event PedAppeared handle=7170
    2026-09-29T05:03:13.264Z [INFO] [autopilot] event PedAppeared handle=7426
    2026-09-29T05:03:13.314Z [INFO] [autopilot] event ReloadFinished weapon=7 clip_after=17
    2026-09-29T05:03:13.402Z [INFO] [autopilot] selftest ped-delete ok
    2026-09-29T05:03:13.455Z [INFO] [autopilot] event PedRemoved handle=3075
    2026-09-29T05:03:13.536Z [INFO] [autopilot] event PedAppeared handle=1283
    2026-09-29T05:03:13.537Z [INFO] [autopilot] event VehicleAppeared handle=1796
    2026-09-29T05:03:13.541Z [INFO] [autopilot] selftest vehicle-spawn ok handle=1796
    2026-09-29T05:03:13.542Z [INFO] [autopilot] selftest vehicle-engine ok engine=1000 body=1000
    2026-09-29T05:03:13.544Z [INFO] [autopilot] selftest vehicle-offset ok nose=(-70.42, 676.834, 14.24)
    2026-09-29T05:03:13.832Z [INFO] [autopilot] event PedAppeared handle=1539
    2026-09-29T05:03:13.910Z [INFO] [autopilot] selftest raycast-vehicle ok Hit Vehicle 1796 at (-71.487, 674.822, 14.135) distance 1.99 normal=(-0.939, -0.003, 0.344)
    2026-09-29T05:03:13.912Z [INFO] [autopilot] selftest line-of-sight-vehicle FAIL blocked=True world_only=Hit World at (-67.549, 674.818, 14.459) distance 5.94 passed=1
    2026-09-29T05:03:14.283Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.505, 635.686, 15.199) to=(-67.57, 666.892, 15.251)
    2026-09-29T05:03:14.427Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.496, 635.689, 15.186) to=(262.653, 1358.093, 15.11)
    2026-09-29T05:03:14.519Z [INFO] [autopilot] event PedRemoved handle=1539
    2026-09-29T05:03:14.569Z [INFO] [autopilot] selftest vehicle-snapshot ok listed=True appeared_event=True
    2026-09-29T05:03:14.621Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.571, 649.112, 15.186) to=(92.396, 1433.062, 3.017)
    2026-09-29T05:03:14.622Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.499, 635.692, 15.2) to=(-77.472, 646.119, 15.044)
    2026-09-29T05:03:14.623Z [INFO] [autopilot] event PedAppeared handle=1027
    2026-09-29T05:03:14.790Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-69.57, 649.114, 15.197) to=(-64.579, 674.789, 15.038)
    2026-09-29T05:03:14.791Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.511, 635.704, 15.193) to=(-67.392, 667.299, 15.404)
    2026-09-29T05:03:14.791Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.579, 674.789, 15.038) dir=(0.191, 0.982, -0.006)
    2026-09-29T05:03:15.317Z [INFO] [autopilot] event PedAppeared handle=1540
    2026-09-29T05:03:15.661Z [INFO] [autopilot] event PedAppeared handle=8194
    2026-09-29T05:03:15.662Z [INFO] [autopilot] event PedAppeared handle=8450
    2026-09-29T05:03:15.840Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.471, 635.707, 15.182) to=(242.826, 1367.328, 22.856)
    2026-09-29T05:03:15.940Z [INFO] [autopilot] event PedRemoved handle=7426
    2026-09-29T05:03:15.941Z [INFO] [autopilot] event PedRemoved handle=6914
    2026-09-29T05:03:16.035Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-68.75, 649.646, 15.107) to=(-67.962, 655.189, 15.109)
    2026-09-29T05:03:16.036Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.504, 635.703, 15.198) to=(-77.605, 646.251, 15.058)
    2026-09-29T05:03:16.037Z [INFO] [autopilot] event PedRemoved handle=1540
    2026-09-29T05:03:16.186Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-68.389, 649.861, 15.127) to=(48.344, 1441.513, -21.072)
    2026-09-29T05:03:16.245Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.503, 635.704, 15.188) to=(247.116, 1365.194, -1.2)
    2026-09-29T05:03:16.345Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-67.962, 650.12, 15.135) to=(39.155, 1443.633, 18.517)
    2026-09-29T05:03:16.399Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.498, 635.702, 15.199) to=(257.124, 1360.568, 0.526)
    2026-09-29T05:03:16.400Z [INFO] [autopilot] event PedRemoved handle=6658
    2026-09-29T05:03:16.531Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-67.519, 650.179, 15.161) to=(-67.065, 654.489, 15.108)
    2026-09-29T05:03:16.532Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.503, 635.701, 15.187) to=(-62.905, 678.247, 15.478)
    2026-09-29T05:03:16.673Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-67.071, 650.046, 15.107) to=(25.548, 1445.351, 14.987)
    2026-09-29T05:03:16.732Z [INFO] [autopilot] event PedAppeared handle=6659
    2026-09-29T05:03:16.736Z [INFO] density frame_ms=63.2 peds=0.55 cars=0.60
    2026-09-29T05:03:16.935Z [INFO] [autopilot] event PedAppeared handle=6915
    2026-09-29T05:03:16.936Z [INFO] [autopilot] event VehicleAppeared handle=259
    2026-09-29T05:03:17.170Z [INFO] [autopilot] selftest vehicle-driver ok ped=1027 snapshot_driver=1027 get_driver=1027
    2026-09-29T05:03:17.172Z [INFO] [autopilot] selftest vehicle-delete ok
    2026-09-29T05:03:17.220Z [INFO] [autopilot] event PedRemoved handle=1027
    2026-09-29T05:03:17.221Z [INFO] [autopilot] event VehicleRemoved handle=1796
    2026-09-29T05:03:17.222Z [INFO] [autopilot] selftest vehicle-removed-event ok
    2026-09-29T05:03:17.224Z [INFO] [autopilot] selftest fx-burst info blood_gun_entry=True
    2026-09-29T05:03:17.225Z [INFO] [autopilot] selftest audio-id ok sound=2
    2026-09-29T05:03:17.227Z [INFO] [autopilot] selftest blip ok blip=1638411
    2026-09-29T05:03:17.317Z [INFO] [autopilot] event VehicleRemoved handle=6658
    2026-09-29T05:03:17.367Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.478, 635.711, 15.182) to=(-32.644, 748.901, 15.096)
    2026-09-29T05:03:17.461Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1161 core=on peds=10 vehicles=16 modules=10/10 coroutines=1 resources=4 raycast=on episode=GTAIV frame_ms=49.76 p95_ms=77.16 pressure=0.76 private_mb=1907 working_set_mb=1252 address_free_mb=1502 largest_free_block_mb=1451 managed_mb=14 physical_load=94% core_us=61.5
    2026-09-29T05:03:17.501Z [INFO] [autopilot] event PedRemoved handle=7170
    2026-09-29T05:03:17.545Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.508, 635.696, 15.199) to=(-67.378, 669.582, 15.042)
    2026-09-29T05:03:17.548Z [INFO] [autopilot] selftest camera-create ok
    2026-09-29T05:03:17.549Z [INFO] [autopilot] selftest game-camera ok fov=45.1 position=(-62.711, 678.844, 15.06)
    2026-09-29T05:03:17.720Z [INFO] performance samples=1164 frame_p50_ms=11 frame_p95_ms=60 frame_p99_ms=130 frames_over_33ms=190 frames_over_50ms=106 gunplay_avg_ms=1.195 gunplay_max_ms=4.874 phase_samples=1165 phase_setup_avg_ms=0.813 phase_setup_max_ms=158.300 phase_camera_avg_ms=0.097 phase_camera_max_ms=6.003 phase_bullets_avg_ms=0.007 phase_bullets_max_ms=5.196 phase_weapon_avg_ms=0.026 phase_weapon_max_ms=3.118 phase_hud_avg_ms=0.402 phase_hud_max_ms=3.959
    2026-09-29T05:03:17.737Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=6.107/3221.3/1164@7 total=7109 module.gunplay=1.351/181.0/1164@7 total=1573 tick.gunplay=1.350/180.8/1164@7 total=1571 gp.freeaim=0.609/53.7/1165@7 total=709 module.arsenal=0.949/109.0/539@7 total=511 tick.arsenal=0.948/108.9/539@7 total=511 module.holsters=1.560/71.0/301@7 total=469 tick.holsters=1.559/71.0/301@7 total=469 ho.show=1.459/71.0/300@7 total=438 module.combat=0.184/146.0/1164@7 total=214 tick.combat=0.183/145.9/1164@7 total=213 module.atmosphere=0.178/7.8/1164@7 total=208 engine.world=0.177/26.4/1165@7 total=207 tick.atmosphere=0.177/7.2/1164@7 total=206 ar.storage=0.345/4.5/539@7 total=186 engine.scheduler=0.144/74.0/1165@7 total=168 ar.reconcile=0.292/98.9/539@7 total=157 gp.player=0.090/94.1/1165@7 total=105 ar.safehouse_flags=0.166/2.3/539@7 total=90 cam.projection=0.377/2.4/222@7 total=84 gp.index_pad=0.052/0.5/1165@7 total=61 combat.sample=0.851/5.4/52@7 total=44 gp.weapon_id=0.024/1.6/1165@7 total=28 gp.shoulder=0.017/1.3/1165@7 total=20 module.devtools=0.036/5.0/539@7 total=19 tick.devtools=0.035/5.0/539@7 total=19 cam.handle=0.015/3.2/1165@7 total=17 gp.cycle=0.009/1.4/1165@7 total=10 cam.find_active=0.007/5.2/1165@7 total=8 gp.state=0.007/0.8/1165@7 total=8 module.world=0.141/2.8/48@7 total=7 gp.shots=0.005/3.8/1165@7 total=6 gp.recoil=0.004/2.5/1165@7 total=5 module.probe=1.231/1.4/3@7 total=4 ar.discover=0.006/2.0/539@7 total=3 gp.spread=0.002/0.8/1165@7 total=3 ar.vehicle=0.005/1.2/539@7 total=3 cam.aim_key=0.002/0.0/1165@7 total=3 combat.dismember=0.002/1.9/1164@7 total=3 ar.lvs=0.004/0.5/539@7 total=2 engine.raycast=0.132/0.9/10@7 total=1 combat.blood=0.001/0.7/1164@7 total=1 gp.feel=0.001/0.7/1165@7 total=1 ho.carried=0.002/0.1/300@7 total=1 combat.pending=0.001/0.3/1164@7 total=1 cam.fov=0.002/0.2/300@7 total=1 module.autopilot=0.000/0.0/1164@7 total=0 module.weapon-probe=0.000/0.0/1164@7 total=0
    2026-09-29T05:03:17.738Z [INFO] engine_thread_probe ticks=1163 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1161 ticks_after_skipped_frames=1
    2026-09-29T05:03:17.740Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=18.64 shdn_us=42.4
    2026-09-29T05:03:17.967Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:03:18.123Z [INFO] [autopilot] event PedAppeared handle=1541
    2026-09-29T05:03:18.246Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.168, 654.957, 15.08) to=(-47.904, 1455.315, 0.785)
    2026-09-29T05:03:18.365Z [INFO] camera_already_gone handle=3844 Invalid call to an object that doesn't exist anymore!
    2026-09-29T05:03:19.020Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.147, 655.068, 15.087) to=(-64.317, 674.666, 15.025)
    2026-09-29T05:03:19.021Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C7 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=1795 vehicle=0 killed=False hit=True at=(-64.317, 674.666, 15.025) dir=(0.042, 0.999, -0.003)
    2026-09-29T05:03:19.997Z [INFO] [autopilot] event PedRemoved handle=1541
    2026-09-29T05:03:19.998Z [INFO] [autopilot] selftest ui-list ok
    2026-09-29T05:03:20.000Z [INFO] [autopilot] selftest ui-list-close ok
    2026-09-29T05:03:22.117Z [INFO] [autopilot] selftest ui-radial ok
    2026-09-29T05:03:22.128Z [INFO] [autopilot] choreography_begin selftest steps=2
    2026-09-29T05:03:22.170Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T05:03:22.192Z [INFO] holster_sling_attached slot=LongGun1 model=lf_sling_a bone=Spine2
    2026-09-29T05:03:22.428Z [INFO] [autopilot] event PedRemoved handle=1283
    2026-09-29T05:03:22.961Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.495, 635.571, 15.184) to=(261.215, 1358.62, 6.56)
    2026-09-29T05:03:23.160Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.499, 635.572, 15.198) to=(-77.644, 646.245, 14.978)
    2026-09-29T05:03:23.861Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.281, 654.894, 15.085) to=(-54.709, 1455.538, 17.459)
    2026-09-29T05:03:24.290Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.473, 635.584, 15.185) to=(-63.07, 678.396, 15.439)
    2026-09-29T05:03:24.382Z [INFO] [autopilot] choreography_complete selftest
    2026-09-29T05:03:24.428Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.503, 635.58, 15.195) to=(239.478, 1368.491, -3.935)
    2026-09-29T05:03:24.429Z [INFO] [autopilot] selftest choreography ok step=1
    2026-09-29T05:03:24.430Z [INFO] [autopilot] selftest_done passed=48 failed=1
    2026-09-29T05:03:24.610Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.503, 635.581, 15.188) to=(-81.651, 637.4, 15.161)
    2026-09-29T05:03:24.752Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.499, 635.588, 15.2) to=(-31.57, 750.462, 14.788)
    2026-09-29T05:03:24.806Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.971, 654.19, 15.054) to=(7.418, 1451.528, 7.607)
    2026-09-29T05:03:24.907Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.515, 635.587, 15.195) to=(-31.447, 752.332, 16.117)
    2026-09-29T05:03:25.010Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.29, 654.111, 15.103) to=(-45.224, 837.192, 13.762)
    2026-09-29T05:03:25.161Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.021, 646.257, 15.113) to=(-18.371, 1445.554, 23.051)
    2026-09-29T05:03:25.162Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.438, 654.13, 15.099) to=(27.301, 1449.455, 17.154)
    2026-09-29T05:03:25.306Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.021, 646.265, 15.125) to=(-11.249, 1444.884, -1.03)
    2026-09-29T05:03:25.307Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.558, 654.168, 15.115) to=(19.16, 1450.133, 7.971)
    2026-09-29T05:03:25.307Z [INFO] [autopilot] event PedRemoved handle=6915
    2026-09-29T05:03:25.499Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.029, 646.262, 15.118) to=(-58.021, 804.491, 13.678)
    2026-09-29T05:03:25.535Z [INFO] [autopilot] event VehicleAppeared handle=1540
    2026-09-29T05:03:25.882Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.689, 639.726, 15.153) to=(306.485, 1338.874, 12.153)
    2026-09-29T05:03:26.013Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.501, 640.096, 15.186) to=(-74.775, 654.879, 15.137)
    2026-09-29T05:03:26.182Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.387, 640.292, 15.2) to=(327.586, 1327.472, 9.157)
    2026-09-29T05:03:26.353Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.402, 640.3, 15.195) to=(-75.194, 654.948, 15.012)
    2026-09-29T05:03:26.526Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.403, 640.312, 15.199) to=(311.235, 1336.657, -6.885)
    2026-09-29T05:03:26.592Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-65.998, 646.281, 15.109) to=(-64.537, 674.807, 15.106)
    2026-09-29T05:03:26.592Z [INFO] [autopilot] event PedDamaged handle=2 100->80 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.537, 674.807, 15.106) dir=(0.051, 0.999, 0)
    2026-09-29T05:03:26.594Z [INFO] [autopilot] event PlayerDamaged 100->80
    2026-09-29T05:03:26.595Z [INFO] [autopilot] event VehicleAppeared handle=7682
    2026-09-29T05:03:26.640Z [INFO] [autopilot] event PedAppeared handle=5890
    2026-09-29T05:03:26.640Z [INFO] [autopilot] event PedAppeared handle=6146
    2026-09-29T05:03:26.641Z [INFO] [autopilot] event PedAppeared handle=6402
    2026-09-29T05:03:26.691Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.403, 640.33, 15.187) to=(-31.57, 734.36, 15.631)
    2026-09-29T05:03:26.930Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.476, 635.588, 15.184) to=(259.863, 1359.308, 8.494)
    2026-09-29T05:03:27.129Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.508, 635.576, 15.198) to=(-67.483, 667.17, 15.35)
    2026-09-29T05:03:27.325Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.506, 635.572, 15.189) to=(-77.488, 646.135, 15.034)
    2026-09-29T05:03:27.475Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.501, 635.575, 15.201) to=(-63.054, 678.313, 15.354)
    2026-09-29T05:03:27.634Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.518, 635.575, 15.196) to=(-77.813, 646.458, 15.041)
    2026-09-29T05:03:27.779Z [INFO] [autopilot] event PedAppeared handle=3076
    2026-09-29T05:03:27.779Z [INFO] [autopilot] event PedAppeared handle=7682
    2026-09-29T05:03:27.780Z [INFO] [autopilot] event PedAppeared handle=7938
    2026-09-29T05:03:27.872Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66, 646.28, 15.11) to=(-64.45, 674.671, 14.532)
    2026-09-29T05:03:27.873Z [INFO] [autopilot] event PedDamaged handle=2 80->60 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=19.8 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.45, 674.671, 14.532) dir=(0.055, 0.998, -0.02)
    2026-09-29T05:03:27.873Z [INFO] [autopilot] event PlayerDamaged 80->60
    2026-09-29T05:03:27.974Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:03:28.068Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-136.553, 718.196, 46.888) to=(-57.547, 673.425, 13.665)
    2026-09-29T05:03:28.068Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.027, 646.26, 15.121) to=(-60.657, 722.204, 13.525)
    2026-09-29T05:03:28.209Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.023, 646.256, 15.114) to=(-11.575, 1444.964, 4.785)
    2026-09-29T05:03:28.254Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-133.388, 715.563, 47.926) to=(-67.514, 676.647, 14.951)
    2026-09-29T05:03:28.348Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.368, 640.319, 15.183) to=(-31.57, 735.024, 16.263)
    2026-09-29T05:03:28.394Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.02, 646.261, 15.124) to=(-32.139, 1446.287, 17.385)
    2026-09-29T05:03:28.441Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.239, 654.134, 15.095) to=(-9.288, 1452.754, 6.802)
    2026-09-29T05:03:28.489Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.396, 640.31, 15.195) to=(305.659, 1339.733, -11.954)
    2026-09-29T05:03:28.585Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.03, 646.262, 15.117) to=(-11.616, 1444.919, 0.822)
    2026-09-29T05:03:28.586Z [INFO] [autopilot] event PedAppeared handle=1542
    2026-09-29T05:03:28.634Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.159, 654.048, 15.128) to=(-64.513, 674.589, 14.971)
    2026-09-29T05:03:28.634Z [INFO] [autopilot] event PedDamaged handle=2 60->45 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=1795 vehicle=0 killed=False hit=True at=(-64.513, 674.589, 14.971) dir=(0.08, 0.997, -0.008)
    2026-09-29T05:03:28.635Z [INFO] [autopilot] event PlayerDamaged 60->45
    2026-09-29T05:03:28.684Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.404, 640.372, 15.161) to=(-31.57, 741.011, 16.109)
    2026-09-29T05:03:28.779Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.159, 654.046, 15.119) to=(-50.044, 814.193, 13.762)
    2026-09-29T05:03:28.833Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.346, 640.547, 15.183) to=(-75.217, 654.783, 14.974)
    2026-09-29T05:03:28.982Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-83.247, 640.839, 15.152) to=(293.559, 1347.28, 6.731)
    2026-09-29T05:03:28.983Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.155, 654.052, 15.13) to=(-64.216, 674.932, 14.9)
    2026-09-29T05:03:28.983Z [INFO] [autopilot] event PedDamaged handle=2 45->30 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=1795 vehicle=0 killed=False hit=True at=(-64.216, 674.932, 14.9) dir=(0.092, 0.996, -0.011)
    2026-09-29T05:03:28.984Z [INFO] [autopilot] event PlayerDamaged 45->30
    2026-09-29T05:03:29.055Z [INFO] [autopilot] event PedRemoved handle=1542
    2026-09-29T05:03:29.112Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.477, 635.585, 15.186) to=(-31.705, 744.761, 16.1)
    2026-09-29T05:03:29.113Z [INFO] [autopilot] event PedAppeared handle=6916
    2026-09-29T05:03:29.169Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.171, 654.057, 15.125) to=(-2.285, 1452.129, 10.678)
    2026-09-29T05:03:29.278Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-139.062, 710.611, 58.066) to=(-60.166, 674.369, 13.548)
    2026-09-29T05:03:29.279Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.512, 635.575, 15.201) to=(256.615, 1360.818, 14.482)
    2026-09-29T05:03:29.390Z [INFO] [autopilot] event PedRemoved handle=6916
    2026-09-29T05:03:29.447Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-136.342, 708.912, 58.74) to=(-63.702, 675.544, 13.674)
    2026-09-29T05:03:29.555Z [INFO] [autopilot] event PedAppeared handle=7171
    2026-09-29T05:03:29.659Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-132.842, 706.798, 59.855) to=(-61.994, 674.947, 14.139)
    2026-09-29T05:03:29.772Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.004, 646.281, 15.11) to=(-9.068, 1444.982, 16.785)
    2026-09-29T05:03:29.826Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-130.159, 705.244, 60.884) to=(-60.348, 672.6, 13.538)
    2026-09-29T05:03:29.881Z [INFO] [autopilot] event VehicleAppeared handle=1027
    2026-09-29T05:03:29.931Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.029, 646.26, 15.123) to=(-62.018, 730.195, 13.828)
    2026-09-29T05:03:29.982Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-103.574, 694.604, 63.321) to=(-65, 674.93, 13.558)
    2026-09-29T05:03:30.034Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-126.788, 703.384, 62.342) to=(-60.78, 673.516, 13.516)
    2026-09-29T05:03:30.134Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-100.859, 693.009, 65.102) to=(-65.689, 674.725, 13.558)
    2026-09-29T05:03:30.135Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.024, 646.256, 15.112) to=(-19.675, 1445.401, 2.527)
    2026-09-29T05:03:30.189Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.948, 644.214, 15.186) to=(315.71, 1338.982, -0.122)
    2026-09-29T05:03:30.290Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.021, 646.261, 15.124) to=(-16.973, 1445.403, 15.589)
    2026-09-29T05:03:30.339Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.479, 635.588, 15.183) to=(265.636, 1356.632, 9.421)
    2026-09-29T05:03:30.388Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.973, 644.209, 15.198) to=(313.542, 1340.485, 26.499)
    2026-09-29T05:03:30.435Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.036, 646.256, 15.12) to=(-58.514, 748.723, 13.685)
    2026-09-29T05:03:30.530Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.979, 644.215, 15.19) to=(-75.186, 655.317, 15.044)
    2026-09-29T05:03:30.674Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.973, 644.234, 15.197) to=(300.914, 1347.567, 10.7)
    2026-09-29T05:03:30.816Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.974, 644.238, 15.191) to=(-31.57, 732.91, 16.328)
    2026-09-29T05:03:31.003Z [INFO] [autopilot] event PedRemoved handle=7171
    2026-09-29T05:03:31.674Z [INFO] [autopilot] event PedAppeared handle=4354
    2026-09-29T05:03:31.674Z [INFO] [autopilot] event PedAppeared handle=4610
    2026-09-29T05:03:31.675Z [INFO] [autopilot] event PedAppeared handle=4866
    2026-09-29T05:03:31.676Z [INFO] [autopilot] event PedAppeared handle=5122
    2026-09-29T05:03:31.990Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.936, 644.216, 15.183) to=(323.587, 1334.316, -6.647)
    2026-09-29T05:03:32.039Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.325, 646.779, 15.062) to=(0.142, 1444.694, 13.593)
    2026-09-29T05:03:32.127Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.966, 644.213, 15.195) to=(-67.539, 670.043, 14.934)
    2026-09-29T05:03:32.219Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-72.171, 674.007, 83.514) to=(-63.38, 674.088, 13.7)
    2026-09-29T05:03:32.312Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.968, 644.195, 15.188) to=(-31.37, 730.15, 15.463)
    2026-09-29T05:03:32.413Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-70.579, 672.575, 84.505) to=(-63.38, 675.997, 13.725)
    2026-09-29T05:03:32.414Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.261, 654.165, 15.087) to=(21.52, 1450.12, 12.153)
    2026-09-29T05:03:32.464Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.963, 644.195, 15.2) to=(-67.549, 668.79, 14.787)
    2026-09-29T05:03:32.604Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-69.041, 671.145, 85.45) to=(-63.488, 674.848, 13.693)
    2026-09-29T05:03:32.605Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.145, 654.055, 15.127) to=(-62.258, 696.083, 13.68)
    2026-09-29T05:03:32.745Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.143, 654.056, 15.117) to=(-58.914, 728.946, 13.644)
    2026-09-29T05:03:32.792Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-67.763, 670.008, 86.177) to=(-62.972, 675.334, 13.739)
    2026-09-29T05:03:32.938Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.142, 654.06, 15.13) to=(-50.383, 865.738, 13.756)
    2026-09-29T05:03:32.989Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-82.081, 680.311, 91.35) to=(-65.727, 673.847, 13.558)
    2026-09-29T05:03:32.990Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-66.57, 669.053, 86.886) to=(-63.189, 674.991, 13.73)
    2026-09-29T05:03:33.101Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.157, 654.062, 15.122) to=(-58.281, 730.166, 13.665)
    2026-09-29T05:03:33.156Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-80.584, 679.273, 92.395) to=(-66.033, 674.98, 13.574)
    2026-09-29T05:03:33.262Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.158, 654.073, 15.126) to=(-3.442, 1452.247, 11.057)
    2026-09-29T05:03:33.331Z [INFO] [autopilot] event VehicleAppeared handle=772
    2026-09-29T05:03:33.388Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-78.657, 677.911, 93.716) to=(-63.136, 674.15, 13.739)
    2026-09-29T05:03:33.445Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.947, 644.216, 15.186) to=(323.04, 1334.832, 9.407)
    2026-09-29T05:03:33.558Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-77.174, 676.857, 94.704) to=(-63.827, 676.067, 13.663)
    2026-09-29T05:03:33.619Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.972, 644.198, 15.197) to=(-67.557, 669.599, 14.892)
    2026-09-29T05:03:33.681Z [INFO] [autopilot] event BulletFired shooter=6402 weapon=15 by_player=False from=(-64.531, 668.384, 88.773) to=(-65.596, 676.707, 13.558)
    2026-09-29T05:03:33.793Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-75.411, 675.609, 95.829) to=(-63.131, 675.171, 13.739)
    2026-09-29T05:03:33.793Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.969, 644.192, 15.188) to=(306.564, 1344.235, 8.394)
    2026-09-29T05:03:33.854Z [INFO] [autopilot] event BulletFired shooter=6402 weapon=15 by_player=False from=(-63.783, 667.745, 89.268) to=(-64.791, 675.924, 13.558)
    2026-09-29T05:03:33.964Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.965, 644.195, 15.2) to=(-67.549, 669.427, 14.728)
    2026-09-29T05:03:34.025Z [INFO] [autopilot] event BulletFired shooter=6402 weapon=15 by_player=False from=(-63.089, 667.111, 89.643) to=(-64.708, 676.787, 13.551)
    2026-09-29T05:03:34.084Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.968, 644.201, 15.186) to=(321.891, 1335.15, -14.576)
    2026-09-29T05:03:34.256Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.971, 644.198, 15.196) to=(320.086, 1336.59, 17.047)
    2026-09-29T05:03:34.257Z [INFO] [autopilot] event BulletFired shooter=6402 weapon=15 by_player=False from=(-62.201, 666.263, 90.025) to=(-62.616, 674.862, 13.739)
    2026-09-29T05:03:34.469Z [INFO] [autopilot] event BulletFired shooter=6402 weapon=15 by_player=False from=(-61.553, 665.635, 90.26) to=(-64.739, 676.006, 13.548)
    2026-09-29T05:03:34.728Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.366, 638.741, 15.178) to=(-81.322, 640.678, 15.161)
    2026-09-29T05:03:34.925Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.218, 639.104, 15.188) to=(-81.408, 640.595, 15.161)
    2026-09-29T05:03:35.078Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.212, 639.109, 15.198) to=(-67.647, 667.129, 14.597)
    2026-09-29T05:03:35.220Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.228, 639.116, 15.195) to=(-67.647, 667.596, 14.629)
    2026-09-29T05:03:35.221Z [INFO] [autopilot] event VehicleRemoved handle=5122
    2026-09-29T05:03:35.273Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.277, 646.776, 15.08) to=(-62.548, 720.555, 14.499)
    2026-09-29T05:03:35.378Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.949, 644.216, 15.187) to=(-31.57, 731.626, 16.421)
    2026-09-29T05:03:35.379Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.228, 639.128, 15.199) to=(-73.919, 655.039, 14.765)
    2026-09-29T05:03:35.433Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.132, 646.661, 15.113) to=(-45.974, 899.515, 13.751)
    2026-09-29T05:03:35.552Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.969, 644.201, 15.195) to=(310.793, 1341.936, 17.332)
    2026-09-29T05:03:35.613Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.136, 646.645, 15.116) to=(-59.487, 809.95, 13.637)
    2026-09-29T05:03:35.724Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.967, 644.194, 15.186) to=(-67.549, 669.408, 14.331)
    2026-09-29T05:03:35.783Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.133, 646.652, 15.129) to=(-17.274, 1445.471, -11.645)
    2026-09-29T05:03:35.784Z [INFO] [autopilot] event VehicleAppeared handle=5123
    2026-09-29T05:03:35.945Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.145, 646.656, 15.124) to=(-7.688, 1445.204, 15.853)
    2026-09-29T05:03:36.098Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.149, 646.658, 15.128) to=(-44.057, 902.924, 13.762)
    2026-09-29T05:03:36.480Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.294, 654.198, 15.079) to=(-3.926, 1452.159, -7.99)
    2026-09-29T05:03:36.672Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.15, 654.061, 15.126) to=(-59.955, 714.302, 13.553)
    2026-09-29T05:03:36.723Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.943, 644.205, 15.185) to=(309.622, 1342.316, -6.54)
    2026-09-29T05:03:36.817Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.153, 654.06, 15.118) to=(15.611, 1450.629, 12.355)
    2026-09-29T05:03:36.864Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.974, 644.198, 15.198) to=(318.136, 1337.741, 19.453)
    2026-09-29T05:03:36.865Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.065, 641.201, 15.136) to=(291.057, 1349.382, -1.913)
    2026-09-29T05:03:36.953Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.152, 654.07, 15.129) to=(-9.411, 1452.536, -2.185)
    2026-09-29T05:03:37.046Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.972, 644.192, 15.189) to=(321.843, 1335.524, 13.507)
    2026-09-29T05:03:37.047Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-81.923, 641.985, 15.101) to=(-75.171, 655.406, 14.834)
    2026-09-29T05:03:37.139Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.163, 654.072, 15.124) to=(8.692, 1451.29, 16.443)
    2026-09-29T05:03:37.186Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.967, 644.196, 15.201) to=(-67.549, 668.407, 14.562)
    2026-09-29T05:03:37.187Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.027, 642.353, 15.103) to=(318.581, 1335.707, 5.651)
    2026-09-29T05:03:37.237Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-49.735, 653.698, 92.021) to=(-62.434, 674.758, 13.739)
    2026-09-29T05:03:37.238Z [INFO] [autopilot] event PedAppeared handle=2310
    2026-09-29T05:03:37.336Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.277, 642.449, 15.134) to=(-75.171, 655.445, 14.817)
    2026-09-29T05:03:37.336Z [INFO] [autopilot] event PedRemoved handle=8450
    2026-09-29T05:03:37.436Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-49.016, 652.673, 91.899) to=(-63.729, 674.039, 13.66)
    2026-09-29T05:03:37.492Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-82.583, 642.179, 15.105) to=(311.192, 1339.302, 19.146)
    2026-09-29T05:03:37.609Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-48.384, 651.87, 91.893) to=(-65.545, 675.533, 13.558)
    2026-09-29T05:03:37.610Z [INFO] [autopilot] event PedRemoved handle=2310
    2026-09-29T05:03:37.831Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-47.387, 650.747, 92.137) to=(-63.975, 677.504, 13.602)
    2026-09-29T05:03:38.006Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:03:38.705Z [INFO] [autopilot] event PedAppeared handle=5379
    2026-09-29T05:03:39.120Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.611, 645.27, 15.108) to=(-75.097, 654.892, 15.139)
    2026-09-29T05:03:39.217Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.94, 644.203, 15.184) to=(327.572, 1332.342, 19.162)
    2026-09-29T05:03:39.369Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.972, 644.2, 15.195) to=(-67.549, 669.889, 14.524)
    2026-09-29T05:03:39.370Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-39.52, 643.243, 100.19) to=(-65.419, 673.178, 13.558)
    2026-09-29T05:03:39.521Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.97, 644.193, 15.188) to=(-67.402, 670.266, 15.026)
    2026-09-29T05:03:39.522Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.287, 646.765, 15.078) to=(-62.524, 695.131, 13.667)
    2026-09-29T05:03:39.577Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-38.628, 642.503, 101.869) to=(-66.037, 675.562, 13.574)
    2026-09-29T05:03:39.693Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.969, 644.197, 15.202) to=(317.697, 1337.964, 16.935)
    2026-09-29T05:03:39.694Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.158, 646.665, 15.105) to=(-15.558, 1445.403, -11.547)
    2026-09-29T05:03:39.752Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-73.498, 661.905, 104.711) to=(-63.17, 673.605, 13.733)
    2026-09-29T05:03:39.752Z [INFO] [autopilot] event BulletFired shooter=6146 weapon=15 by_player=False from=(-37.966, 641.987, 103.341) to=(-65.296, 675.613, 13.558)
    2026-09-29T05:03:39.872Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.157, 646.648, 15.113) to=(-58.875, 738.615, 13.653)
    2026-09-29T05:03:39.933Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-74.225, 661.104, 104.55) to=(-65.371, 675.44, 13.558)
    2026-09-29T05:03:40.057Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.158, 646.649, 15.131) to=(-56.864, 832.152, 13.732)
    2026-09-29T05:03:40.129Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-74.953, 660.262, 104.403) to=(-65.321, 673.336, 13.558)
    2026-09-29T05:03:40.130Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.216, 646.631, 15.186) to=(-75.186, 655.337, 15.036)
    2026-09-29T05:03:40.194Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.169, 646.65, 15.123) to=(-16.133, 1445.731, 16.282)
    2026-09-29T05:03:40.313Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-75.708, 659.33, 104.268) to=(-64.893, 674.94, 13.558)
    2026-09-29T05:03:40.380Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.173, 646.654, 15.131) to=(-4.315, 1444.988, 16.137)
    2026-09-29T05:03:40.588Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.153, 654.113, 15.1) to=(11.532, 1451.182, 23.496)
    2026-09-29T05:03:40.694Z [INFO] [autopilot] event PedRemoved handle=5379
    2026-09-29T05:03:40.749Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.941, 644.205, 15.182) to=(-67.549, 669.737, 14.552)
    2026-09-29T05:03:40.750Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-66.087, 654.154, 15.075) to=(-59.468, 722.49, 13.588)
    2026-09-29T05:03:40.906Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.971, 644.199, 15.195) to=(323.97, 1334.373, 18.215)
    2026-09-29T05:03:40.907Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.796, 654.183, 15.1) to=(-22.929, 1453.604, 3.312)
    2026-09-29T05:03:41.063Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.438, 654.22, 15.025) to=(-11.584, 1453.157, 16.637)
    2026-09-29T05:03:41.111Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-81.972, 644.192, 15.188) to=(308.639, 1342.991, 4.324)
    2026-09-29T05:03:41.162Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.196, 646.599, 15.182) to=(305.126, 1348.415, 12.668)
    2026-09-29T05:03:41.162Z [INFO] [autopilot] event PedRemoved handle=8194
    2026-09-29T05:03:41.256Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.184, 654.643, 15.032) to=(-25.013, 1454.27, 11.347)
    2026-09-29T05:03:41.352Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.231, 646.601, 15.196) to=(-63.138, 678.479, 15.045)
    2026-09-29T05:03:41.496Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.231, 646.597, 15.188) to=(307.484, 1346.793, -7.804)
    2026-09-29T05:03:41.545Z [INFO] [autopilot] event PedRemoved handle=6402
    2026-09-29T05:03:41.546Z [INFO] [autopilot] event PedRemoved handle=5890
    2026-09-29T05:03:41.546Z [INFO] [autopilot] event PedRemoved handle=6146
    2026-09-29T05:03:41.645Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.228, 646.599, 15.2) to=(-63.026, 678.285, 15.297)
    2026-09-29T05:03:42.123Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-80.863, 650.969, 103.718) to=(-65.67, 675.164, 13.558)
    2026-09-29T05:03:42.288Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-81.262, 650.393, 103.675) to=(-66.04, 675.614, 13.575)
    2026-09-29T05:03:42.288Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.27, 685.163, 14.927) to=(-51.85, 684.021, 14.894)
    2026-09-29T05:03:42.289Z [INFO] [autopilot] event VehicleDamaged handle=1027 1000->995 engine 1000->1000
    2026-09-29T05:03:42.391Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.259, 685.166, 14.92) to=(-208.574, 561.935, 14.98)
    2026-09-29T05:03:42.495Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-81.807, 649.659, 103.612) to=(-64.337, 676.529, 13.571)
    2026-09-29T05:03:42.496Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.899, 646.929, 15.141) to=(274.294, 1365.697, 16.336)
    2026-09-29T05:03:42.497Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.267, 685.17, 14.932) to=(-51.769, 684.021, 14.898)
    2026-09-29T05:03:42.497Z [INFO] [autopilot] event VehicleDamaged handle=1027 995->991 engine 1000->1000
    2026-09-29T05:03:42.551Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.261, 646.766, 15.059) to=(-64.487, 674.658, 14.417)
    2026-09-29T05:03:42.552Z [INFO] [autopilot] event PedDamaged handle=2 30->15 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-64.487, 674.658, 14.417) dir=(0.063, 0.998, -0.023)
    2026-09-29T05:03:42.553Z [INFO] [autopilot] event PlayerDamaged 30->15
    2026-09-29T05:03:42.614Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.269, 685.15, 14.916) to=(-215.208, 570.965, 13.741)
    2026-09-29T05:03:42.670Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.6, 647.118, 15.162) to=(-74.553, 654.769, 14.998)
    2026-09-29T05:03:42.671Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.279, 685.148, 14.908) to=(-51.807, 684.021, 14.831)
    2026-09-29T05:03:42.672Z [INFO] [autopilot] event VehicleDamaged handle=1027 991->986 engine 1000->1000
    2026-09-29T05:03:42.726Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.154, 646.678, 15.1) to=(-24.828, 1446.133, 1.497)
    2026-09-29T05:03:42.781Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.337, 684.93, 14.981) to=(-67.077, 671.044, 14.757)
    2026-09-29T05:03:42.782Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.271, 685.132, 14.891) to=(-51.753, 684.021, 14.838)
    2026-09-29T05:03:42.783Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.201, 646.597, 15.184) to=(-75.122, 655.318, 15.076)
    2026-09-29T05:03:42.784Z [INFO] [autopilot] event VehicleDamaged handle=1027 986->982 engine 1000->1000
    2026-09-29T05:03:42.898Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.27, 685.116, 14.922) to=(-211.551, 565.96, 10.385)
    2026-09-29T05:03:42.898Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.149, 646.669, 15.108) to=(-15.853, 1445.818, 24.606)
    2026-09-29T05:03:42.956Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.321, 684.934, 15.078) to=(-591.118, 90.965, 0.27)
    2026-09-29T05:03:42.957Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.273, 685.108, 14.961) to=(-211.209, 565.415, 18.153)
    2026-09-29T05:03:42.958Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.237, 646.59, 15.199) to=(304.974, 1348.287, -0.69)
    2026-09-29T05:03:43.015Z [INFO] [autopilot] event PedAppeared handle=3330
    2026-09-29T05:03:43.015Z [INFO] [autopilot] event PedAppeared handle=3586
    2026-09-29T05:03:43.016Z [INFO] [autopilot] event PedAppeared handle=3842
    2026-09-29T05:03:43.017Z [INFO] [autopilot] event PedAppeared handle=4098
    2026-09-29T05:03:43.119Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.327, 684.929, 15.151) to=(-67.077, 671.34, 14.56)
    2026-09-29T05:03:43.120Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.255, 685.135, 15.049) to=(-210.267, 564.296, 9.943)
    2026-09-29T05:03:43.121Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.146, 646.677, 15.127) to=(-63.796, 691.779, 13.606)
    2026-09-29T05:03:43.180Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.271, 685.12, 15.074) to=(-51.815, 684.021, 15.032)
    2026-09-29T05:03:43.181Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-80.231, 646.586, 15.188) to=(-62.687, 678.299, 15.219)
    2026-09-29T05:03:43.181Z [INFO] [autopilot] event VehicleDamaged handle=1027 982->977 engine 1000->1000
    2026-09-29T05:03:43.227Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.158, 646.679, 15.12) to=(-62.849, 711.494, 13.636)
    2026-09-29T05:03:43.284Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.323, 684.933, 15.103) to=(-103.526, 633.553, 14.975)
    2026-09-29T05:03:43.285Z [INFO] [autopilot] event BulletFired shooter=4354 weapon=13 by_player=False from=(-50.261, 685.14, 15.052) to=(-51.742, 684.022, 15.036)
    2026-09-29T05:03:43.286Z [INFO] [autopilot] event VehicleDamaged handle=1027 977->973 engine 1000->1000
    2026-09-29T05:03:43.458Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.346, 684.939, 15.018) to=(-597.507, 96.701, 7.629)
    2026-09-29T05:03:43.607Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.344, 684.95, 15.028) to=(-64.458, 674.586, 14.726)
    2026-09-29T05:03:43.608Z [INFO] [autopilot] event PedDamaged handle=2 15->0 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=14.9 armour_lost=0.0 attacker=4610 vehicle=0 killed=True hit=True at=(-64.458, 674.586, 14.726) dir=(-0.698, -0.716, -0.021)
    2026-09-29T05:03:43.610Z [INFO] [autopilot] event PedDied handle=2 bone=0x4C1 by_player=False exact=True type=Bullet killer=4610 weapon=15
    2026-09-29T05:03:43.611Z [INFO] [autopilot] event PlayerDamaged 15->0
    2026-09-29T05:03:43.883Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.782, 689.888, 15.126) to=(-51.058, 689.507, 15.136)
    2026-09-29T05:03:43.884Z [INFO] [autopilot] event VehicleDamaged handle=1027 973->967 engine 1000->1000
    2026-09-29T05:03:43.939Z [INFO] [autopilot] event VehicleRemoved handle=1540
    2026-09-29T05:03:44.052Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.694, 689.57, 15.142) to=(-576.692, 86.141, -5.46)
    2026-09-29T05:03:44.338Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-78.674, 648.357, 15.116) to=(290.581, 1358.945, 22.411)
    2026-09-29T05:03:44.591Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.346, 647.249, 15.184) to=(-74.503, 654.79, 15.033)
    2026-09-29T05:03:44.991Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.84, 658.765, 15.062) to=(-5.094, 1456.449, -20.626)
    2026-09-29T05:03:45.187Z [INFO] [autopilot] event BulletFired shooter=1795 weapon=15 by_player=False from=(-65.998, 658.817, 15.119) to=(-64.313, 674.758, 14.733)
    2026-09-29T05:03:45.187Z [INFO] [autopilot] event PedDamaged handle=2 1->-100 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=19.8 health_lost=101.0 armour_lost=0.0 attacker=1795 vehicle=0 killed=False hit=True at=(-64.313, 674.758, 14.733) dir=(0.105, 0.994, -0.024)
    2026-09-29T05:03:45.188Z [INFO] [autopilot] event PlayerWeaponChanged 7->0
    2026-09-29T05:03:45.189Z [INFO] [autopilot] event PlayerDied
    2026-09-29T05:03:45.190Z [INFO] [autopilot] event PlayerDamaged 0->-100
    2026-09-29T05:03:45.250Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.397, 684.943, 15.017) to=(-127.538, 609.667, 15.707)
    2026-09-29T05:03:45.316Z [INFO] holsters_removed reason=wasted
    2026-09-29T05:03:45.318Z [ERROR] arsenal_loss_no_safehouse owned weapons cannot be stored
    2026-09-29T05:03:45.318Z [INFO] arsenal_loss reason=wasted id=3 owned=False
    2026-09-29T05:03:45.319Z [INFO] arsenal_loss reason=wasted id=7 owned=False
    2026-09-29T05:03:45.320Z [INFO] arsenal_loss reason=wasted id=12 owned=False
    2026-09-29T05:03:45.320Z [INFO] arsenal_loss reason=wasted id=16 owned=False
    2026-09-29T05:03:45.321Z [INFO] arsenal_loss reason=wasted id=18 owned=False
    2026-09-29T05:03:45.322Z [INFO] arsenal_loss reason=wasted id=5 owned=False
    2026-09-29T05:03:45.592Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.371, 684.936, 15.017) to=(-614.335, 112.803, 5.599)
    2026-09-29T05:03:45.702Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.384, 684.934, 14.998) to=(-615.346, 113.836, -0.276)
    2026-09-29T05:03:45.824Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.72, 688.238, 14.972) to=(-632.909, 139.191, -9.425)
    2026-09-29T05:03:45.873Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.389, 684.931, 15.01) to=(-638.22, 137.083, -0.183)
    2026-09-29T05:03:45.999Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.891, 687.943, 15.016) to=(-651.387, 158.46, -3.127)
    2026-09-29T05:03:46.060Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.384, 684.933, 14.998) to=(-64.966, 675.128, 13.991)
    2026-09-29T05:03:46.060Z [INFO] [autopilot] event BulletFired shooter=0 weapon=-1 by_player=False from=(-64.232, 675.043, 13.615) to=(-64.392, 675.179, 13.565)
    2026-09-29T05:03:46.061Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A3 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=0.0 armour_lost=0.0 attacker=4610 vehicle=0 killed=False hit=True at=(-64.966, 675.128, 13.991) dir=(-0.732, -0.678, -0.07)
    2026-09-29T05:03:46.207Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.906, 687.832, 15.012) to=(-50.83, 687.891, 15.037)
    2026-09-29T05:03:46.208Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.404, 684.938, 14.992) to=(-67.077, 674.369, 14.137)
    2026-09-29T05:03:46.208Z [INFO] [autopilot] event VehicleDamaged handle=1027 967->961 engine 1000->1000
    2026-09-29T05:03:46.276Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x36A1 by_player=False weapon=54 exact=True type=Fall amount=1.2 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:03:46.353Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.951, 687.795, 15.007) to=(-50.831, 687.898, 15.036)
    2026-09-29T05:03:46.353Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x1A2 by_player=False weapon=54 exact=True type=Fall amount=0.1 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:03:46.354Z [INFO] [autopilot] event VehicleDamaged handle=1027 961->955 engine 1000->1000
    2026-09-29T05:03:46.431Z [INFO] [autopilot] event VehicleRemoved handle=6914
    2026-09-29T05:03:46.498Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.595, 646.865, 15.214) to=(-13.266, 1444.599, -30.035)
    2026-09-29T05:03:46.499Z [INFO] [autopilot] event PedAppeared handle=7172
    2026-09-29T05:03:46.500Z [INFO] [autopilot] event PedAppeared handle=7427
    2026-09-29T05:03:46.579Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.204, 649.736, 15.174) to=(255.835, 1376.777, -23.161)
    2026-09-29T05:03:46.642Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.628, 646.892, 15.195) to=(-66.156, 658.352, 14.723)
    2026-09-29T05:03:46.727Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.357, 647.267, 15.177) to=(-77.51, 649.262, 15.056)
    2026-09-29T05:03:46.812Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.242, 649.727, 15.187) to=(-74.915, 654.789, 15.02)
    2026-09-29T05:03:46.814Z [INFO] density frame_ms=73.7 peds=0.55 cars=0.60
    2026-09-29T05:03:46.873Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.631, 646.892, 15.207) to=(-64.92, 672.685, 13.558)
    2026-09-29T05:03:47.005Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.388, 647.259, 15.187) to=(250.619, 1376.418, -16.983)
    2026-09-29T05:03:47.006Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.223, 649.733, 15.168) to=(-74.95, 654.791, 14.858)
    2026-09-29T05:03:47.083Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.644, 646.892, 15.201) to=(-40.288, 1446.416, -18.324)
    2026-09-29T05:03:47.084Z [INFO] [autopilot] event PedRemoved handle=7427
    2026-09-29T05:03:47.145Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.374, 647.26, 15.172) to=(253.073, 1374.901, -25.525)
    2026-09-29T05:03:47.146Z [INFO] [autopilot] event PedAppeared handle=8195
    2026-09-29T05:03:47.204Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.228, 649.721, 15.185) to=(-75.095, 654.759, 14.976)
    2026-09-29T05:03:47.285Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.641, 646.897, 15.204) to=(-66.135, 658.334, 14.574)
    2026-09-29T05:03:47.286Z [INFO] [autopilot] event PedAppeared handle=8451
    2026-09-29T05:03:47.286Z [INFO] [autopilot] event PedAppeared handle=8706
    2026-09-29T05:03:47.362Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.383, 647.247, 15.188) to=(-77.5, 649.231, 15.089)
    2026-09-29T05:03:47.362Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.238, 649.718, 15.173) to=(-74.879, 654.806, 14.809)
    2026-09-29T05:03:47.363Z [INFO] [autopilot] event PedAppeared handle=8962
    2026-09-29T05:03:47.427Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.634, 646.901, 15.195) to=(-65.011, 676.435, 13.67)
    2026-09-29T05:03:47.428Z [INFO] [autopilot] event PedDamaged handle=2 -100->-100 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=14.9 health_lost=0.0 armour_lost=0.0 attacker=2818 vehicle=0 killed=False hit=True at=(-65.011, 676.435, 13.67) dir=(0.055, 0.997, -0.051)
    2026-09-29T05:03:47.429Z [INFO] [autopilot] event PedAppeared handle=9218
    2026-09-29T05:03:47.508Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.391, 647.245, 15.176) to=(244.676, 1378.991, -18.4)
    2026-09-29T05:03:47.509Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.983, 687.772, 14.978) to=(-50.827, 687.83, 14.995)
    2026-09-29T05:03:47.510Z [INFO] [autopilot] event PedAppeared handle=9474
    2026-09-29T05:03:47.510Z [INFO] [autopilot] event VehicleDamaged handle=1027 955->949 engine 1000->1000
    2026-09-29T05:03:47.513Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1659 core=on peds=23 vehicles=18 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV frame_ms=70.75 p95_ms=64.17 pressure=0.99 private_mb=1857 working_set_mb=1218 address_free_mb=1493 largest_free_block_mb=1446 managed_mb=14 physical_load=94% core_us=66.4
    2026-09-29T05:03:47.583Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.442, 684.988, 14.974) to=(-62.587, 678.197, 13.944)
    2026-09-29T05:03:47.712Z [INFO] performance samples=497 frame_p50_ms=51 frame_p95_ms=73 frame_p99_ms=200 frames_over_33ms=496 frames_over_50ms=268 gunplay_avg_ms=1.206 gunplay_max_ms=10.870 phase_samples=462 phase_setup_avg_ms=0.837 phase_setup_max_ms=10.406 phase_camera_avg_ms=0.036 phase_camera_max_ms=0.802 phase_bullets_avg_ms=0.021 phase_bullets_max_ms=0.158 phase_weapon_avg_ms=0.019 phase_weapon_max_ms=0.125 phase_hud_avg_ms=0.381 phase_hud_max_ms=0.589
    2026-09-29T05:03:47.713Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=3.915/100.5/497@7 total=1946 module.gunplay=1.257/24.0/497@7 total=625 tick.gunplay=1.255/24.0/497@7 total=624 module.arsenal=0.764/20.4/497@7 total=380 tick.arsenal=0.763/20.4/497@7 total=379 gp.freeaim=0.718/10.3/462@7 total=332 engine.world=0.630/4.8/497@7 total=313 ar.storage=0.486/6.1/465@7 total=226 module.holsters=0.508/56.7/303@7 total=154 tick.holsters=0.508/56.7/303@7 total=154 engine.scheduler=0.286/98.0/497@7 total=142 ar.safehouse_flags=0.225/4.7/465@7 total=105 module.atmosphere=0.187/9.3/497@7 total=93 tick.atmosphere=0.186/9.3/497@7 total=92 module.combat=0.178/3.1/497@7 total=88 tick.combat=0.177/3.1/497@7 total=88 combat.sample=1.176/2.6/60@7 total=71 ho.show=0.225/56.7/269@7 total=60 gp.index_pad=0.048/0.1/462@7 total=22 cam.handle=0.030/0.8/462@7 total=14 module.devtools=0.023/0.0/497@7 total=11 tick.devtools=0.023/0.0/497@7 total=11 gp.shoulder=0.016/0.1/462@7 total=7 gp.player=0.014/0.1/462@7 total=7 module.world=0.120/0.5/54@7 total=6 gp.weapon_id=0.012/0.1/462@7 total=5 module.probe=1.225/1.3/3@7 total=4 gp.cycle=0.007/0.1/462@7 total=3 gp.state=0.006/0.0/462@7 total=3 ar.reconcile=0.006/0.1/465@7 total=3 ar.discover=0.004/0.7/497@7 total=2 ar.lvs=0.003/0.3/497@7 total=2 ar.vehicle=0.003/0.0/497@7 total=1 cam.aim_key=0.003/0.0/462@7 total=1 cam.find_active=0.002/0.0/462@7 total=1 gp.spread=0.002/0.0/462@7 total=1 gp.shots=0.002/0.0/462@7 total=1 ho.carried=0.002/0.0/269@7 total=1 combat.dismember=0.001/0.0/497@7 total=0 combat.blood=0.000/0.0/497@7 total=0 module.autopilot=0.000/0.0/497@7 total=0 cam.fov=0.001/0.0/90@7 total=0 module.weapon-probe=0.000/0.0/497@7 total=0 gp.feel=0.000/0.0/462@7 total=0 combat.pending=0.000/0.0/497@7 total=0 gp.recoil=0.000/0.0/462@7 total=0
    2026-09-29T05:03:47.714Z [INFO] engine_thread_probe ticks=497 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=496 ticks_after_skipped_frames=1
    2026-09-29T05:03:47.716Z [INFO] direct_native get_char_health direct=0 shdn=-100 match=False direct_us=0.15 shdn_us=98.0
    2026-09-29T05:03:47.797Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.406, 684.981, 14.99) to=(-62.576, 678.249, 13.981)
    2026-09-29T05:03:47.949Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.416, 684.984, 14.975) to=(-674.472, 184.191, -61.133)
    2026-09-29T05:03:48.007Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:03:48.072Z [INFO] [autopilot] event PedRemoved handle=9474
    2026-09-29T05:03:48.142Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.405, 684.988, 14.993) to=(-62.644, 678.806, 14.126)
    2026-09-29T05:03:48.348Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.395, 684.985, 14.985) to=(-677.471, 188.747, -65.965)
    2026-09-29T05:03:48.410Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.617, 646.918, 15.191) to=(-66.277, 658.411, 14.545)
    2026-09-29T05:03:48.473Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.964, 687.744, 15.122) to=(-682.584, 198.633, -38.41)
    2026-09-29T05:03:48.474Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.397, 684.985, 14.99) to=(-62.365, 678.542, 13.948)
    2026-09-29T05:03:48.626Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.637, 646.887, 15.204) to=(-26.524, 1445.135, -31.414)
    2026-09-29T05:03:48.691Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.952, 687.741, 15.033) to=(-50.827, 687.81, 15.058)
    2026-09-29T05:03:48.692Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.203, 649.736, 15.17) to=(258.082, 1375.523, -28.219)
    2026-09-29T05:03:48.692Z [INFO] [autopilot] event VehicleDamaged handle=1027 949->943 engine 1000->1000
    2026-09-29T05:03:48.770Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.355, 647.269, 15.171) to=(-77.508, 649.268, 15.029)
    2026-09-29T05:03:48.771Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.629, 646.89, 15.194) to=(-66.434, 654.253, 14.843)
    2026-09-29T05:03:48.837Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.964, 687.751, 14.982) to=(-50.827, 687.818, 14.994)
    2026-09-29T05:03:48.838Z [INFO] [autopilot] event VehicleDamaged handle=1027 943->937 engine 1000->1000
    2026-09-29T05:03:48.902Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.237, 649.722, 15.186) to=(-74.979, 654.79, 15.031)
    2026-09-29T05:03:48.980Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.387, 647.254, 15.188) to=(-77.484, 649.256, 15.074)
    2026-09-29T05:03:48.981Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.63, 646.896, 15.208) to=(-30.826, 1445.88, -20.699)
    2026-09-29T05:03:49.089Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.959, 687.751, 15.028) to=(-50.827, 687.82, 15.057)
    2026-09-29T05:03:49.090Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.224, 649.725, 15.171) to=(256.07, 1376.766, -21.407)
    2026-09-29T05:03:49.091Z [INFO] [autopilot] event VehicleDamaged handle=1027 937->931 engine 1000->1000
    2026-09-29T05:03:49.181Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.644, 646.899, 15.2) to=(-16.761, 1444.58, -33.907)
    2026-09-29T05:03:49.276Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.383, 647.249, 15.178) to=(-77.554, 649.263, 15.08)
    2026-09-29T05:03:49.277Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.946, 687.739, 15.025) to=(-50.827, 687.8, 15.048)
    2026-09-29T05:03:49.278Z [INFO] [autopilot] event PedRemoved handle=7172
    2026-09-29T05:03:49.279Z [INFO] [autopilot] event VehicleDamaged handle=1027 931->925 engine 1000->1000
    2026-09-29T05:03:49.353Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.229, 649.72, 15.189) to=(-75.003, 654.774, 15.004)
    2026-09-29T05:03:49.425Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.38, 647.261, 15.185) to=(249.656, 1375.878, -36.285)
    2026-09-29T05:03:49.426Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.644, 646.903, 15.207) to=(-66.463, 654.253, 14.83)
    2026-09-29T05:03:49.504Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.237, 649.722, 15.176) to=(254.567, 1377.969, -11.441)
    2026-09-29T05:03:49.641Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.396, 647.249, 15.182) to=(-77.515, 649.248, 15.076)
    2026-09-29T05:03:50.113Z [INFO] [autopilot] event PedRemoved handle=8962
    2026-09-29T05:03:50.169Z [INFO] [autopilot] event PedRemoved handle=8451
    2026-09-29T05:03:50.489Z [INFO] [autopilot] event PedAppeared handle=7428
    2026-09-29T05:03:50.490Z [INFO] [autopilot] event PedRemoved handle=8706
    2026-09-29T05:03:50.743Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.357, 647.266, 15.168) to=(247.556, 1378.057, -11.969)
    2026-09-29T05:03:50.744Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.405, 685.007, 14.959) to=(-686.57, 199.141, -57.882)
    2026-09-29T05:03:50.745Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.207, 649.732, 15.168) to=(262.342, 1373.423, -31.842)
    2026-09-29T05:03:50.808Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.61, 646.911, 15.194) to=(-66.295, 658.437, 14.435)
    2026-09-29T05:03:50.882Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-51.012, 687.747, 14.987) to=(-50.827, 687.815, 15.011)
    2026-09-29T05:03:50.883Z [INFO] [autopilot] event PedAppeared handle=8452
    2026-09-29T05:03:50.884Z [INFO] [autopilot] event VehicleDamaged handle=1027 925->919 engine 1000->1000
    2026-09-29T05:03:50.946Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.39, 647.256, 15.185) to=(245.206, 1378.164, -30.254)
    2026-09-29T05:03:50.947Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.375, 684.976, 14.953) to=(-62.543, 678.445, 14.225)
    2026-09-29T05:03:50.948Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.24, 649.724, 15.184) to=(-75.002, 654.79, 14.903)
    2026-09-29T05:03:51.010Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.639, 646.888, 15.207) to=(-66.039, 658.369, 14.557)
    2026-09-29T05:03:51.110Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.377, 647.263, 15.174) to=(-77.552, 649.265, 15.086)
    2026-09-29T05:03:51.111Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.987, 687.73, 14.987) to=(-50.827, 687.801, 15.009)
    2026-09-29T05:03:51.112Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.369, 684.974, 15.003) to=(-62.441, 678.425, 13.978)
    2026-09-29T05:03:51.112Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.23, 649.728, 15.174) to=(-75.098, 654.79, 14.891)
    2026-09-29T05:03:51.113Z [INFO] [autopilot] event PedAppeared handle=8707
    2026-09-29T05:03:51.114Z [INFO] [autopilot] event VehicleDamaged handle=1027 919->913 engine 1000->1000
    2026-09-29T05:03:51.189Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.631, 646.891, 15.195) to=(-32.809, 1445.246, -34.974)
    2026-09-29T05:03:51.254Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.987, 687.734, 14.982) to=(-681.305, 198.184, -48.889)
    2026-09-29T05:03:51.331Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.38, 647.258, 15.19) to=(249.112, 1376.241, -33.793)
    2026-09-29T05:03:51.331Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.363, 684.983, 15.133) to=(-62.585, 678.725, 14.24)
    2026-09-29T05:03:51.332Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.23, 649.726, 15.188) to=(-75.007, 654.79, 15.043)
    2026-09-29T05:03:51.397Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.995, 687.726, 14.991) to=(-50.827, 687.799, 15.015)
    2026-09-29T05:03:51.398Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.631, 646.889, 15.21) to=(-64.817, 674.425, 13.558)
    2026-09-29T05:03:51.399Z [INFO] [autopilot] event VehicleDamaged handle=1027 913->907 engine 1000->1000
    2026-09-29T05:03:51.463Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.397, 647.26, 15.183) to=(-77.456, 649.276, 15.048)
    2026-09-29T05:03:51.465Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.364, 684.975, 15.084) to=(-62.505, 678.733, 14.055)
    2026-09-29T05:03:51.465Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.248, 649.727, 15.183) to=(-74.921, 654.79, 14.874)
    2026-09-29T05:03:51.544Z [INFO] [autopilot] event BulletFired shooter=2818 weapon=15 by_player=False from=(-66.641, 646.891, 15.198) to=(-20.703, 1445.518, -18.711)
    2026-09-29T05:03:51.625Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.381, 647.269, 15.179) to=(-77.504, 649.229, 15.118)
    2026-09-29T05:03:51.625Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.983, 687.727, 14.989) to=(-50.827, 687.792, 15.013)
    2026-09-29T05:03:51.626Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.235, 649.732, 15.181) to=(-75.001, 654.806, 14.839)
    2026-09-29T05:03:51.627Z [INFO] [autopilot] event VehicleDamaged handle=1027 907->901 engine 1000->1000
    2026-09-29T05:03:51.692Z [INFO] [autopilot] event PedRemoved handle=8195
    2026-09-29T05:03:52.027Z [INFO] [autopilot] event VehicleDamaged handle=7682 1000->999 engine 1000->1000
    2026-09-29T05:03:52.726Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.358, 647.272, 15.169) to=(-77.515, 649.263, 15.121)
    2026-09-29T05:03:52.804Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.419, 684.988, 14.979) to=(-678.131, 190.312, -70.997)
    2026-09-29T05:03:52.944Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.39, 647.264, 15.185) to=(239.22, 1380.458, -38.029)
    2026-09-29T05:03:52.945Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-51.017, 687.754, 14.981) to=(-50.827, 687.819, 15.001)
    2026-09-29T05:03:52.946Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.206, 649.739, 15.172) to=(-74.986, 654.806, 14.843)
    2026-09-29T05:03:52.947Z [INFO] [autopilot] event VehicleDamaged handle=1027 901->895 engine 1000->1000
    2026-09-29T05:03:52.948Z [INFO] [autopilot] event VehicleDamaged handle=7682 999->992 engine 1000->1000
    2026-09-29T05:03:53.022Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.387, 684.981, 14.999) to=(-62.528, 678.762, 14.027)
    2026-09-29T05:03:53.099Z [INFO] [autopilot] event BulletFired shooter=6659 weapon=15 by_player=False from=(-78.379, 647.263, 15.173) to=(-77.459, 649.273, 15.06)
    2026-09-29T05:03:53.184Z [INFO] [autopilot] event BulletFired shooter=4866 weapon=15 by_player=False from=(-50.981, 687.745, 14.995) to=(-50.827, 687.813, 15.019)
    2026-09-29T05:03:53.185Z [INFO] [autopilot] event BulletFired shooter=4610 weapon=15 by_player=False from=(-54.388, 684.99, 14.991) to=(-62.569, 678.331, 14.097)
    2026-09-29T05:03:53.186Z [INFO] [autopilot] event BulletFired shooter=2562 weapon=15 by_player=False from=(-77.239, 649.729, 15.185) to=(-75.068, 654.79, 15.043)
    2026-09-29T05:03:53.187Z [INFO] [autopilot] event VehicleDamaged handle=1027 895->889 engine 1000->1000
    2026-09-29T05:03:55.053Z [INFO] [autopilot] event PedRemoved handle=8452
    2026-09-29T05:03:55.053Z [INFO] [autopilot] event PedRemoved handle=7428
    2026-09-29T05:03:55.054Z [INFO] [autopilot] event PedRemoved handle=3076
    2026-09-29T05:03:55.055Z [INFO] [autopilot] event PedRemoved handle=8707
    2026-09-29T05:03:55.056Z [INFO] [autopilot] event PedRemoved handle=6659
    2026-09-29T05:03:55.057Z [INFO] [autopilot] event PedRemoved handle=3842
    2026-09-29T05:03:55.058Z [INFO] [autopilot] event PedRemoved handle=3330
    2026-09-29T05:03:55.058Z [INFO] [autopilot] event PedRemoved handle=4866
    2026-09-29T05:03:55.060Z [INFO] [autopilot] event PedRemoved handle=4354
    2026-09-29T05:03:55.061Z [INFO] [autopilot] event PedRemoved handle=7938
    2026-09-29T05:03:55.061Z [INFO] [autopilot] event PedRemoved handle=2818
    2026-09-29T05:03:55.062Z [INFO] [autopilot] event PedRemoved handle=1795
    2026-09-29T05:03:55.063Z [INFO] [autopilot] event PedRemoved handle=9218
    2026-09-29T05:03:55.064Z [INFO] [autopilot] event PedRemoved handle=4098
    2026-09-29T05:03:55.064Z [INFO] [autopilot] event PedRemoved handle=3586
    2026-09-29T05:03:55.065Z [INFO] [autopilot] event PedRemoved handle=5122
    2026-09-29T05:03:55.066Z [INFO] [autopilot] event PedRemoved handle=4610
    2026-09-29T05:03:55.066Z [INFO] [autopilot] event PedRemoved handle=7682
    2026-09-29T05:03:55.067Z [INFO] [autopilot] event PedRemoved handle=2562
    2026-09-29T05:03:55.068Z [INFO] [autopilot] event VehicleAppeared handle=517
    2026-09-29T05:03:55.069Z [INFO] [autopilot] event VehicleAppeared handle=773
    2026-09-29T05:03:55.069Z [INFO] [autopilot] event VehicleAppeared handle=1028
    2026-09-29T05:03:55.070Z [INFO] [autopilot] event VehicleAppeared handle=1284
    2026-09-29T05:03:55.071Z [INFO] [autopilot] event VehicleAppeared handle=1541
    2026-09-29T05:03:55.072Z [INFO] [autopilot] event VehicleAppeared handle=1798
    2026-09-29T05:03:55.073Z [INFO] [autopilot] event VehicleAppeared handle=2052
    2026-09-29T05:03:55.075Z [INFO] [autopilot] event VehicleAppeared handle=2308
    2026-09-29T05:03:55.076Z [INFO] [autopilot] event VehicleAppeared handle=2564
    2026-09-29T05:03:55.077Z [INFO] [autopilot] event VehicleAppeared handle=2819
    2026-09-29T05:03:55.078Z [INFO] [autopilot] event VehicleAppeared handle=3075
    2026-09-29T05:03:55.079Z [INFO] [autopilot] event VehicleAppeared handle=3331
    2026-09-29T05:03:55.079Z [INFO] [autopilot] event VehicleAppeared handle=3587
    2026-09-29T05:03:55.080Z [INFO] [autopilot] event VehicleRemoved handle=772
    2026-09-29T05:03:55.081Z [INFO] [autopilot] event VehicleRemoved handle=516
    2026-09-29T05:03:55.082Z [INFO] [autopilot] event VehicleRemoved handle=7682
    2026-09-29T05:03:55.083Z [INFO] [autopilot] event VehicleRemoved handle=6402
    2026-09-29T05:03:55.083Z [INFO] [autopilot] event VehicleRemoved handle=6146
    2026-09-29T05:03:55.084Z [INFO] [autopilot] event VehicleRemoved handle=5890
    2026-09-29T05:03:55.085Z [INFO] [autopilot] event VehicleRemoved handle=4610
    2026-09-29T05:03:55.086Z [INFO] [autopilot] event VehicleRemoved handle=4354
    2026-09-29T05:03:55.086Z [INFO] [autopilot] event VehicleRemoved handle=3842
    2026-09-29T05:03:55.087Z [INFO] [autopilot] event VehicleRemoved handle=3586
    2026-09-29T05:03:55.088Z [INFO] [autopilot] event VehicleRemoved handle=3330
    2026-09-29T05:03:55.088Z [INFO] [autopilot] event VehicleRemoved handle=3074
    2026-09-29T05:03:55.089Z [INFO] [autopilot] event VehicleRemoved handle=2818
    2026-09-29T05:03:55.090Z [INFO] [autopilot] event VehicleRemoved handle=5123
    2026-09-29T05:03:55.090Z [INFO] [autopilot] event VehicleRemoved handle=1027
    2026-09-29T05:03:55.091Z [INFO] [autopilot] event VehicleRemoved handle=259
    2026-09-29T05:03:55.092Z [INFO] [autopilot] event VehicleRemoved handle=2563
    2026-09-29T05:03:55.092Z [INFO] [autopilot] event VehicleRemoved handle=2307
    2026-09-29T05:03:55.097Z [INFO] weapon_changed from=7 to=0 profile=vanilla
    2026-09-29T05:03:55.104Z [INFO] [world] world_object removed name=test_wall reason=out of range
    2026-09-29T05:03:56.608Z [INFO] [autopilot] event VehicleAppeared handle=3843
    2026-09-29T05:03:56.619Z [INFO] [autopilot] event VehicleAppeared handle=4099
    2026-09-29T05:03:56.634Z [INFO] [autopilot] event VehicleAppeared handle=4355
    2026-09-29T05:03:56.634Z [INFO] [autopilot] event VehicleAppeared handle=4611
    2026-09-29T05:03:56.650Z [INFO] [autopilot] event PedAppeared handle=2053
    2026-09-29T05:03:56.651Z [INFO] [autopilot] event VehicleAppeared handle=4867
    2026-09-29T05:03:56.670Z [INFO] [autopilot] event VehicleAppeared handle=5124
    2026-09-29T05:03:56.686Z [INFO] [autopilot] event VehicleAppeared handle=5379
    2026-09-29T05:03:56.883Z [INFO] [autopilot] event VehicleAppeared handle=5635
    2026-09-29T05:03:56.942Z [INFO] [autopilot] event VehicleAppeared handle=5891
    2026-09-29T05:03:56.999Z [INFO] [autopilot] event PedAppeared handle=2311
    2026-09-29T05:03:57.000Z [INFO] [autopilot] event PedAppeared handle=2563
    2026-09-29T05:03:57.000Z [INFO] [autopilot] event VehicleAppeared handle=6147
    2026-09-29T05:03:57.077Z [INFO] [autopilot] event PedAppeared handle=2819
    2026-09-29T05:03:57.078Z [INFO] [autopilot] event PedAppeared handle=3077
    2026-09-29T05:03:57.079Z [INFO] [autopilot] event PedAppeared handle=3331
    2026-09-29T05:03:57.080Z [INFO] [autopilot] event PedAppeared handle=3587
    2026-09-29T05:03:57.081Z [INFO] [autopilot] event PedAppeared handle=3843
    2026-09-29T05:03:57.082Z [INFO] [autopilot] event PedAppeared handle=4099
    2026-09-29T05:03:57.110Z [INFO] [autopilot] event PedAppeared handle=4355
    2026-09-29T05:03:57.111Z [INFO] [autopilot] event PedAppeared handle=4611
    2026-09-29T05:03:57.111Z [INFO] [autopilot] event PedAppeared handle=4867
    2026-09-29T05:03:57.113Z [INFO] [autopilot] event PedAppeared handle=5123
    2026-09-29T05:03:57.170Z [INFO] [autopilot] event PedAppeared handle=5380
    2026-09-29T05:03:57.171Z [INFO] [autopilot] event PedAppeared handle=5636
    2026-09-29T05:03:57.445Z [INFO] [autopilot] event PedAppeared handle=5891
    2026-09-29T05:03:58.024Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:04:00.798Z [INFO] [autopilot] event PedAppeared handle=6147
    2026-09-29T05:04:03.198Z [INFO] [autopilot] event PedAppeared handle=6403
    2026-09-29T05:04:03.199Z [INFO] [autopilot] event PedAppeared handle=6660
    2026-09-29T05:04:03.401Z [INFO] [autopilot] event PedAppeared handle=6918
    2026-09-29T05:04:03.402Z [INFO] [autopilot] event PedAppeared handle=7173
    2026-09-29T05:04:04.817Z [INFO] [autopilot] event PedAppeared handle=7429
    2026-09-29T05:04:04.846Z [INFO] [autopilot] event VehicleAppeared handle=7427
    2026-09-29T05:04:05.406Z [INFO] [autopilot] event VehicleRemoved handle=4099
    2026-09-29T05:04:05.611Z [INFO] [autopilot] event PedAppeared handle=7683
    2026-09-29T05:04:05.909Z [INFO] [autopilot] event PedAppeared handle=7939
    2026-09-29T05:04:06.349Z [INFO] [autopilot] event VehicleAppeared handle=6915
    2026-09-29T05:04:06.386Z [INFO] [autopilot] event PedAppeared handle=8196
    2026-09-29T05:04:06.627Z [INFO] [autopilot] event PedAppeared handle=8453
    2026-09-29T05:04:07.254Z [INFO] [autopilot] event PedAppeared handle=8708
    2026-09-29T05:04:07.687Z [INFO] [autopilot] event VehicleAppeared handle=8962
    2026-09-29T05:04:07.742Z [INFO] [autopilot] event PedAppeared handle=8963
    2026-09-29T05:04:08.025Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:04:08.201Z [INFO] [autopilot] event PedAppeared handle=9219
    2026-09-29T05:04:08.243Z [INFO] [autopilot] event VehicleAppeared handle=9218
    2026-09-29T05:04:08.862Z [INFO] [autopilot] event PedAppeared handle=9475
    2026-09-29T05:04:09.012Z [INFO] [autopilot] event PedAppeared handle=9730
    2026-09-29T05:04:09.673Z [INFO] [autopilot] event PedAppeared handle=9986
    2026-09-29T05:04:09.712Z [INFO] [autopilot] event VehicleAppeared handle=7171
    2026-09-29T05:04:09.943Z [INFO] [autopilot] event PedAppeared handle=10242
    2026-09-29T05:04:10.272Z [INFO] [autopilot] event VehicleAppeared handle=8194
    2026-09-29T05:04:11.876Z [INFO] [autopilot] event VehicleRemoved handle=6915
    2026-09-29T05:04:11.973Z [INFO] [autopilot] event VehicleAppeared handle=7938
    2026-09-29T05:04:12.036Z [INFO] [autopilot] event VehicleRemoved handle=7171
    2026-09-29T05:04:12.811Z [INFO] [autopilot] event PedRemoved handle=5636
    2026-09-29T05:04:13.181Z [INFO] [autopilot] event VehicleRemoved handle=7427
    2026-09-29T05:04:13.325Z [INFO] [autopilot] event PedAppeared handle=10498
    2026-09-29T05:04:13.445Z [INFO] [autopilot] event PedRemoved handle=9730
    2026-09-29T05:04:13.496Z [INFO] [autopilot] event PedRemoved handle=4099
    2026-09-29T05:04:13.515Z [INFO] [autopilot] event VehicleAppeared handle=9474
    2026-09-29T05:04:13.698Z [INFO] [autopilot] event PedRemoved handle=10242
    2026-09-29T05:04:13.838Z [INFO] [autopilot] event PedAppeared handle=5637
    2026-09-29T05:04:13.839Z [INFO] [autopilot] event PedAppeared handle=9731
    2026-09-29T05:04:13.878Z [INFO] [autopilot] event PedAppeared handle=10243
    2026-09-29T05:04:13.878Z [INFO] [autopilot] event PedRemoved handle=6660
    2026-09-29T05:04:13.879Z [INFO] [autopilot] event PedRemoved handle=6403
    2026-09-29T05:04:13.992Z [INFO] [autopilot] event PedAppeared handle=6661
    2026-09-29T05:04:14.125Z [INFO] [autopilot] event PedAppeared handle=10754
    2026-09-29T05:04:14.205Z [INFO] [autopilot] event VehicleAppeared handle=7683
    2026-09-29T05:04:15.588Z [INFO] [autopilot] event VehicleAppeared handle=9730
    2026-09-29T05:04:16.511Z [INFO] [autopilot] event PedRemoved handle=6918
    2026-09-29T05:04:16.512Z [INFO] [autopilot] event PedRemoved handle=7173
    2026-09-29T05:04:16.512Z [INFO] [autopilot] event VehicleRemoved handle=7683
    2026-09-29T05:04:16.830Z [INFO] density frame_ms=23.5 peds=1.00 cars=1.00
    2026-09-29T05:04:16.951Z [INFO] [autopilot] event PedAppeared handle=7174
    2026-09-29T05:04:16.952Z [INFO] [autopilot] event PedRemoved handle=9986
    2026-09-29T05:04:17.152Z [INFO] [autopilot] event VehicleAppeared handle=8450
    2026-09-29T05:04:17.185Z [INFO] [autopilot] event PedAppeared handle=9987
    2026-09-29T05:04:17.186Z [INFO] [autopilot] event VehicleAppeared handle=10242
    2026-09-29T05:04:17.272Z [INFO] [autopilot] event PedAppeared handle=11010
    2026-09-29T05:04:17.328Z [INFO] [autopilot] event PedRemoved handle=7174
    2026-09-29T05:04:17.433Z [INFO] [autopilot] event VehicleRemoved handle=9218
    2026-09-29T05:04:17.509Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=2680 core=on peds=33 vehicles=29 modules=10/10 coroutines=0 resources=2 raycast=on episode=GTAIV frame_ms=28.28 p95_ms=26.96 pressure=0.00 private_mb=2138 working_set_mb=1312 address_free_mb=1251 largest_free_block_mb=1204 managed_mb=14 physical_load=96% core_us=97.1
    2026-09-29T05:04:17.706Z [INFO] performance samples=1026 frame_p50_ms=20 frame_p95_ms=70 frame_p99_ms=97 frames_over_33ms=173 frames_over_50ms=100 gunplay_avg_ms=1.063 gunplay_max_ms=5.655 phase_samples=948 phase_setup_avg_ms=0.721 phase_setup_max_ms=4.718 phase_camera_avg_ms=0.025 phase_camera_max_ms=1.935 phase_bullets_avg_ms=0.002 phase_bullets_max_ms=0.139 phase_weapon_avg_ms=0.017 phase_weapon_max_ms=0.124 phase_hud_avg_ms=0.380 phase_hud_max_ms=0.586
    2026-09-29T05:04:17.707Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.074/53.7/1026@7 total=2128 module.gunplay=1.069/5.7/1026@7 total=1097 tick.gunplay=1.068/5.7/1026@7 total=1096 gp.freeaim=0.609/3.9/948@7 total=577 module.arsenal=0.493/3.8/652@7 total=321 tick.arsenal=0.492/3.8/652@7 total=321 engine.world=0.306/41.3/1026@7 total=314 module.atmosphere=0.242/4.4/1026@7 total=249 tick.atmosphere=0.241/4.3/1026@7 total=247 ar.storage=0.321/3.8/573@7 total=184 ar.safehouse_flags=0.165/2.6/573@7 total=94 gp.index_pad=0.053/0.2/948@7 total=50 module.combat=0.023/0.5/1026@7 total=23 tick.combat=0.022/0.5/1026@7 total=23 cam.handle=0.019/1.9/948@7 total=18 module.holsters=0.043/0.6/381@7 total=17 tick.holsters=0.043/0.6/381@7 total=16 module.devtools=0.024/0.1/652@7 total=15 tick.devtools=0.023/0.1/652@7 total=15 gp.shoulder=0.014/0.1/948@7 total=13 gp.weapon_id=0.012/0.8/948@7 total=11 gp.player=0.011/0.1/948@7 total=10 gp.cycle=0.008/0.1/948@7 total=7 gp.state=0.006/0.0/948@7 total=6 module.probe=1.386/1.5/3@7 total=4 gp.shots=0.004/0.2/948@7 total=4 module.world=0.054/1.6/55@7 total=3 cam.aim_key=0.003/0.3/948@7 total=3 cam.find_active=0.002/0.0/948@7 total=2 ar.vehicle=0.003/0.0/652@7 total=2 ar.lvs=0.003/0.3/652@7 total=2 gp.spread=0.002/0.0/948@7 total=2 ar.discover=0.002/0.5/652@7 total=2 ar.reconcile=0.002/0.0/573@7 total=1 engine.scheduler=0.001/0.0/1026@7 total=1 combat.dismember=0.001/0.1/1026@7 total=1 ho.carried=0.002/0.0/202@7 total=0 ho.show=0.002/0.0/202@7 total=0 combat.blood=0.000/0.0/1026@7 total=0 module.weapon-probe=0.000/0.0/1026@7 total=0 module.autopilot=0.000/0.0/1026@7 total=0 combat.pending=0.000/0.0/1026@7 total=0 gp.feel=0.000/0.0/948@7 total=0 cam.fov=0.001/0.0/85@7 total=0 gp.recoil=0.000/0.0/948@7 total=0
    2026-09-29T05:04:17.708Z [INFO] engine_thread_probe ticks=1026 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1025 ticks_after_skipped_frames=1
    2026-09-29T05:04:17.718Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.22 shdn_us=475.4
    2026-09-29T05:04:17.888Z [INFO] [autopilot] event PedRemoved handle=6147
    2026-09-29T05:04:18.022Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:04:18.347Z [INFO] [autopilot] event PedAppeared handle=6148
    2026-09-29T05:04:18.348Z [INFO] [autopilot] event PedRemoved handle=5123
    2026-09-29T05:04:18.831Z [INFO] [autopilot] event PedAppeared handle=6404
    2026-09-29T05:04:18.860Z [INFO] [autopilot] event PedRemoved handle=6148
    2026-09-29T05:04:18.884Z [INFO] [autopilot] event PedAppeared handle=6919
    2026-09-29T05:04:20.173Z [INFO] [autopilot] event PedAppeared handle=7175
    2026-09-29T05:04:20.174Z [INFO] [autopilot] event PedAppeared handle=11266
    2026-09-29T05:04:20.589Z [INFO] [autopilot] event VehicleAppeared handle=10498
    2026-09-29T05:04:21.016Z [INFO] [autopilot] event VehicleAppeared handle=9986
    2026-09-29T05:04:21.308Z [INFO] [autopilot] event PedRemoved handle=6919
    2026-09-29T05:04:21.308Z [INFO] [autopilot] event VehicleRemoved handle=4611
    2026-09-29T05:04:22.993Z [INFO] [autopilot] event PedAppeared handle=6149
    2026-09-29T05:04:22.994Z [INFO] [autopilot] event PedRemoved handle=5891
    2026-09-29T05:04:23.121Z [INFO] [autopilot] event VehicleAppeared handle=10754
    2026-09-29T05:04:23.228Z [INFO] [autopilot] event PedRemoved handle=3331
    2026-09-29T05:04:23.407Z [INFO] [autopilot] event PedRemoved handle=6149
    2026-09-29T05:04:23.569Z [INFO] [autopilot] event PedAppeared handle=4100
    2026-09-29T05:04:23.663Z [INFO] [autopilot] event PedAppeared handle=5124
    2026-09-29T05:04:23.704Z [INFO] [autopilot] event PedAppeared handle=5892
    2026-09-29T05:04:27.082Z [INFO] [autopilot] event VehicleRemoved handle=9986
    2026-09-29T05:04:27.544Z [INFO] [autopilot] event PedRemoved handle=7175
    2026-09-29T05:04:27.545Z [INFO] [autopilot] event PedRemoved handle=11266
    2026-09-29T05:04:28.036Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:04:29.477Z [INFO] [autopilot] event PedAppeared handle=6150
    2026-09-29T05:04:29.478Z [INFO] [autopilot] event PedRemoved handle=9731
    2026-09-29T05:04:29.479Z [INFO] [autopilot] event PedRemoved handle=10754
    2026-09-29T05:04:30.956Z [INFO] [autopilot] event VehicleRemoved handle=6147
    2026-09-29T05:04:31.512Z [INFO] [autopilot] event PedRemoved handle=7939
    2026-09-29T05:04:31.558Z [INFO] [autopilot] event VehicleAppeared handle=7428
    2026-09-29T05:04:32.218Z [INFO] [autopilot] event PedRemoved handle=5124
    2026-09-29T05:04:32.255Z [INFO] [autopilot] event VehicleAppeared handle=8195
    2026-09-29T05:04:32.256Z [INFO] [autopilot] event VehicleRemoved handle=8194
    2026-09-29T05:04:32.499Z [INFO] [autopilot] event PedAppeared handle=6920
    2026-09-29T05:04:33.159Z [INFO] [autopilot] event PedRemoved handle=7683
    2026-09-29T05:04:33.499Z [INFO] [autopilot] event PedAppeared handle=7176
    2026-09-29T05:04:34.388Z [INFO] [autopilot] event VehicleRemoved handle=7428
    2026-09-29T05:04:34.758Z [INFO] [autopilot] event VehicleAppeared handle=9219
    2026-09-29T05:04:35.596Z [INFO] [autopilot] event PedRemoved handle=6150
    2026-09-29T05:04:35.676Z [INFO] [autopilot] event VehicleRemoved handle=5635
    2026-09-29T05:04:36.624Z [INFO] [autopilot] event PedRemoved handle=4355
    2026-09-29T05:04:37.012Z [INFO] [autopilot] event PedAppeared handle=5125
    2026-09-29T05:04:37.273Z [INFO] [autopilot] event VehicleAppeared handle=11010
    2026-09-29T05:04:37.478Z [INFO] [autopilot] event VehicleRemoved handle=9219
    2026-09-29T05:04:37.657Z [INFO] [autopilot] event VehicleRemoved handle=5124
    2026-09-29T05:04:37.918Z [INFO] [autopilot] event PedRemoved handle=9987
    2026-09-29T05:04:38.049Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:04:38.461Z [INFO] [autopilot] event PedAppeared handle=6151
    2026-09-29T05:04:39.898Z [INFO] [autopilot] event VehicleAppeared handle=6917
    2026-09-29T05:04:41.635Z [INFO] [autopilot] event VehicleAppeared handle=6148
    2026-09-29T05:04:41.636Z [INFO] [autopilot] event VehicleRemoved handle=4355
    2026-09-29T05:04:41.739Z [INFO] [autopilot] event VehicleAppeared handle=8707
    2026-09-29T05:04:41.926Z [INFO] [autopilot] event VehicleRemoved handle=8450
    2026-09-29T05:04:41.984Z [INFO] [autopilot] event VehicleAppeared handle=6403
    2026-09-29T05:04:42.507Z [INFO] command source=file:cmd_20260929050442383.cmd line="perf" reply="frame_ms=19.85 p95_ms=22.33 pressure=0.00 private_mb=2138 working_set_mb=1343 address_free_mb=1260 largest_free_block_mb=1204 managed_mb=14 physical_load=96% core_us=102.3"
    2026-09-29T05:04:42.711Z [INFO] [autopilot] event VehicleRemoved handle=5891
    2026-09-29T05:04:43.002Z [INFO] command source=file:cmd_20260929050442777.cmd line="pools" reply="peds=30/120 vehicles=39/140 objects=230/1300"
    2026-09-29T05:04:43.140Z [INFO] [autopilot] event VehicleAppeared handle=6660
    2026-09-29T05:04:43.203Z [INFO] [autopilot] event VehicleRemoved handle=10754
    2026-09-29T05:04:43.299Z [INFO] command source=file:cmd_20260929050443158.cmd line="owned autopilot" reply="autopilot: invincible=1 weather=1"
    2026-09-29T05:04:43.332Z [INFO] [autopilot] event PedRemoved handle=7429

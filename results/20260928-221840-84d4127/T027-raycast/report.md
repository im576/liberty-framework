# Scenario raycast

- Result: FAIL
- Steps: 40, failed: 1
- Game alive at end: True
- Log errors during run: 0

## Failed steps
- expect ray dir=forward mask=World\|Vehicles\|Objects status=(Clear|Hit kind=World) .*passed=[1-9]: no new log line in 5 s

## Steps
- launch: engine booted on attempt 3
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 6000 ms
- time 13 0 => time 13:00
- raystats => raycast available=True enabled=True installed=1 queries=0 tests=0 hits=0 clears=0 passes=0 inconclusive=0 faults=0
- expect raycast available=True: OK 2026-09-29T05:40:14.243Z [INFO] command source=file:cmd_20260929054014015.cmd line="raystats" reply="raycast available=True enabled=True installed=1 queries=0 tests=0 hits=0 clears=0 passes=0 inconclusive=0 faults=0"
- ray down 10 => ray dir=down mask=All status=Hit kind=World handle=0 distance=1.505 pos=(-64.421,674.834,13.562) normal=(-0.054,-0.002,0.999) tests=2 passed=1 from=(-64.421,674.834,15.068) to=(-64.421,674.834,5.068)
- expect ray dir=down mask=All status=Hit kind=World: OK 2026-09-29T05:40:14.495Z [INFO] ray dir=down mask=All status=Hit kind=World handle=0 distance=1.505 pos=(-64.421,674.834,13.562) normal=(-0.054,-0.002,0.999) tests=2 passed=1 from=(-64.421,674.834,15.068) to=(-64.421,674.834,5.068)
- ray up 3 1.0 => ray dir=up mask=All status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,15.568) to=(-64.421,674.834,18.568)
- expect ray dir=up mask=All status=Clear: OK 2026-09-29T05:40:15.013Z [INFO] ray dir=up mask=All status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,15.568) to=(-64.421,674.834,18.568)
- spawncar admiral 6 => spawning admiral
- expect autopilot_spawncar handle=: OK 2026-09-29T05:40:15.374Z [INFO] [autopilot] autopilot_spawncar handle=7682 model=admiral
- wait 2000 ms
- shot vehicle -> vehicle.jpg
- ray forward 12 -0.3 => ray dir=forward mask=All status=Hit kind=Vehicle handle=7682 distance=4.995 pos=(-64.421,679.829,14.268) normal=(0.017,-0.955,-0.296) tests=1 passed=0 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)
- expect ray dir=forward mask=All status=Hit kind=Vehicle: OK 2026-09-29T05:40:19.875Z [INFO] ray dir=forward mask=All status=Hit kind=Vehicle handle=7682 distance=4.995 pos=(-64.421,679.829,14.268) normal=(0.017,-0.955,-0.296) tests=1 passed=0 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)
- ray forward 12 -0.3 world,peds => ray dir=forward mask=World|Peds status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=2 passed=1 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)
- expect ray dir=forward mask=World\|Peds status=(Clear|Hit kind=World) .*passed=[1-9]: OK 2026-09-29T05:40:20.132Z [INFO] ray dir=forward mask=World|Peds status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=2 passed=1 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)
- rayto car => autopilot_rayto target=car handle=7682 mask=All status=Hit kind=Vehicle hit_handle=7682 distance=5 of=5.996 normal=(-0.023, -0.912, 0.41) tests=1 passed=0 match=True
- expect autopilot_rayto target=car .*match=True: OK 2026-09-29T05:40:20.693Z [INFO] [autopilot] autopilot_rayto target=car handle=7682 mask=All status=Hit kind=Vehicle hit_handle=7682 distance=5 of=5.996 normal=(-0.023, -0.912, 0.41) tests=1 passed=0 match=True
- raybits forward 12 1 -0.3 => raybits dir=forward m=12.0 mode=1 start=(-64.421,674.834,14.268) hits: b3=Vehicle/4.995m
- clear => cleared 1
- wait 1000 ms
- spawn 1 5 0 => spawning 1 at 5 m
- expect autopilot_spawned count=1: OK 2026-09-29T05:40:22.771Z [INFO] [autopilot] autopilot_spawned count=1
- wait 2500 ms
- shot ped -> ped.jpg
- ray forward 10 0 => ray dir=forward mask=All status=Hit kind=Ped handle=1284 distance=5.177 pos=(-64.421,680.011,14.568) normal=(0.663,-0.730,-0.165) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)
- expect ray dir=forward mask=All status=Hit kind=Ped: OK 2026-09-29T05:40:27.425Z [INFO] ray dir=forward mask=All status=Hit kind=Ped handle=1284 distance=5.177 pos=(-64.421,680.011,14.568) normal=(0.663,-0.730,-0.165) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)
- ray forward 10 0 world,vehicles,objects => ray dir=forward mask=World|Vehicles|Objects status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)
- FAILED: expect ray dir=forward mask=World\|Vehicles\|Objects status=(Clear|Hit kind=World) .*passed=[1-9]: no new log line in 5 s
- rayto 0 => autopilot_rayto target=0 handle=1284 mask=All status=Hit kind=Ped hit_handle=1284 distance=7.423 of=7.569 normal=(0.018, -0.989, 0.146) tests=1 passed=0 match=True
- expect autopilot_rayto target=0 .*match=True: OK 2026-09-29T05:40:33.346Z [INFO] [autopilot] autopilot_rayto target=0 handle=1284 mask=All status=Hit kind=Ped hit_handle=1284 distance=7.423 of=7.569 normal=(0.018, -0.989, 0.146) tests=1 passed=0 match=True
- los 0 => autopilot_los subject=0 handle=1284 available=True sight=True back=True spotted=True
- expect autopilot_los subject=0 .*available=True sight=True back=True: OK 2026-09-29T05:40:33.592Z [INFO] [autopilot] autopilot_los subject=0 handle=1284 available=True sight=True back=True spotted=True
- raystats => raycast available=True enabled=True installed=1 queries=42 tests=46 hits=6 clears=36 passes=4 inconclusive=0 faults=0
- expect raycast available=True .*faults=0: OK 2026-09-29T05:40:34.128Z [INFO] command source=file:cmd_20260929054033917.cmd line="raystats" reply="raycast available=True enabled=True installed=1 queries=42 tests=46 hits=6 clears=36 passes=4 inconclusive=0 faults=0"
- clear => cleared 1

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:40:02.818Z [INFO] command source=file:cmd_20260929054002574.cmd line="events on" reply="event log on"
    2026-09-29T05:40:03.070Z [INFO] command source=file:cmd_20260929054002985.cmd line="god on" reply="invincible True"
    2026-09-29T05:40:03.567Z [INFO] command source=file:cmd_20260929054003374.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:40:07.468Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:40:07.469Z [INFO] command source=file:cmd_20260929054003789.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:40:07.569Z [INFO] [autopilot] event PedRemoved handle=2818
    2026-09-29T05:40:07.571Z [INFO] [autopilot] event PedRemoved handle=2306
    2026-09-29T05:40:07.571Z [INFO] [autopilot] event PedRemoved handle=1794
    2026-09-29T05:40:07.572Z [INFO] [autopilot] event PedRemoved handle=258
    2026-09-29T05:40:07.573Z [INFO] [autopilot] event PedRemoved handle=2562
    2026-09-29T05:40:07.573Z [INFO] [autopilot] event PedRemoved handle=2050
    2026-09-29T05:40:07.574Z [INFO] [autopilot] event PedRemoved handle=514
    2026-09-29T05:40:07.575Z [INFO] [autopilot] event VehicleRemoved handle=515
    2026-09-29T05:40:07.576Z [INFO] [autopilot] event VehicleRemoved handle=2050
    2026-09-29T05:40:07.576Z [INFO] [autopilot] event VehicleRemoved handle=1794
    2026-09-29T05:40:07.577Z [INFO] [autopilot] event VehicleRemoved handle=1538
    2026-09-29T05:40:07.578Z [INFO] [autopilot] event VehicleRemoved handle=1282
    2026-09-29T05:40:07.578Z [INFO] [autopilot] event VehicleRemoved handle=1026
    2026-09-29T05:40:07.579Z [INFO] [autopilot] event VehicleRemoved handle=2
    2026-09-29T05:40:09.836Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:40:09.837Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:40:10.490Z [INFO] [autopilot] event VehicleAppeared handle=2306
    2026-09-29T05:40:10.491Z [INFO] [autopilot] event VehicleAppeared handle=2562
    2026-09-29T05:40:10.492Z [INFO] [autopilot] event VehicleAppeared handle=2818
    2026-09-29T05:40:10.493Z [INFO] [autopilot] event VehicleAppeared handle=3074
    2026-09-29T05:40:10.494Z [INFO] [autopilot] event VehicleAppeared handle=3330
    2026-09-29T05:40:10.495Z [INFO] [autopilot] event VehicleAppeared handle=3586
    2026-09-29T05:40:10.496Z [INFO] [autopilot] event VehicleAppeared handle=3842
    2026-09-29T05:40:10.496Z [INFO] [autopilot] event VehicleAppeared handle=4354
    2026-09-29T05:40:10.497Z [INFO] [autopilot] event VehicleAppeared handle=4610
    2026-09-29T05:40:10.499Z [INFO] [autopilot] event VehicleAppeared handle=5122
    2026-09-29T05:40:10.499Z [INFO] [autopilot] event VehicleAppeared handle=5890
    2026-09-29T05:40:10.500Z [INFO] [autopilot] event VehicleAppeared handle=6146
    2026-09-29T05:40:10.501Z [INFO] [autopilot] event VehicleAppeared handle=6402
    2026-09-29T05:40:10.502Z [INFO] [autopilot] event VehicleAppeared handle=6658
    2026-09-29T05:40:10.503Z [INFO] [autopilot] event VehicleAppeared handle=6914
    2026-09-29T05:40:10.913Z [INFO] [autopilot] event PedAppeared handle=515
    2026-09-29T05:40:10.929Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=1028 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T05:40:11.075Z [INFO] [autopilot] event PedAppeared handle=1795
    2026-09-29T05:40:11.202Z [INFO] [autopilot] event PedAppeared handle=2051
    2026-09-29T05:40:11.235Z [INFO] [autopilot] event PedAppeared handle=2307
    2026-09-29T05:40:11.404Z [INFO] [autopilot] event PedAppeared handle=2563
    2026-09-29T05:40:11.665Z [INFO] [autopilot] event PedAppeared handle=2819
    2026-09-29T05:40:11.704Z [INFO] [autopilot] event PedAppeared handle=3074
    2026-09-29T05:40:11.740Z [INFO] [autopilot] event VehicleAppeared handle=259
    2026-09-29T05:40:11.774Z [INFO] [autopilot] event PedRemoved handle=1795
    2026-09-29T05:40:11.775Z [INFO] [autopilot] event VehicleAppeared handle=1027
    2026-09-29T05:40:12.397Z [INFO] [autopilot] event PedAppeared handle=3330
    2026-09-29T05:40:13.003Z [INFO] [autopilot] event PedAppeared handle=5634
    2026-09-29T05:40:13.037Z [INFO] [autopilot] event PedAppeared handle=5890
    2026-09-29T05:40:13.038Z [INFO] [autopilot] event PedAppeared handle=6146
    2026-09-29T05:40:13.075Z [INFO] [autopilot] event PedAppeared handle=6402
    2026-09-29T05:40:13.139Z [INFO] [autopilot] event PedAppeared handle=6658
    2026-09-29T05:40:13.140Z [INFO] [autopilot] event PedAppeared handle=6914
    2026-09-29T05:40:13.497Z [INFO] [autopilot] event PedRemoved handle=515
    2026-09-29T05:40:13.722Z [INFO] command source=file:cmd_20260929054013637.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:40:13.953Z [INFO] [autopilot] event PedAppeared handle=8450
    2026-09-29T05:40:14.063Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-69.702, 654.261, 15.172) to=(-52.516, 718.412, 14.735)
    2026-09-29T05:40:14.066Z [INFO] [autopilot] event VehicleDamaged handle=1027 1000->988 engine 1000->1000
    2026-09-29T05:40:14.209Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-69.761, 654.315, 15.17) to=(-67.549, 662.728, 14.812)
    2026-09-29T05:40:14.210Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-69.566, 649.088, 15.194) to=(-67.521, 658.387, 14.946)
    2026-09-29T05:40:14.243Z [INFO] command source=file:cmd_20260929054014015.cmd line="raystats" reply="raycast available=True enabled=True installed=1 queries=0 tests=0 hits=0 clears=0 passes=0 inconclusive=0 faults=0"
    2026-09-29T05:40:14.362Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-69.669, 654.341, 15.149) to=(146.854, 1425.319, 16.636)
    2026-09-29T05:40:14.391Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-69.565, 649.086, 15.186) to=(97.222, 1432.211, 14.899)
    2026-09-29T05:40:14.392Z [INFO] [autopilot] event PedRemoved handle=2307
    2026-09-29T05:40:14.495Z [INFO] ray dir=down mask=All status=Hit kind=World handle=0 distance=1.505 pos=(-64.421,674.834,13.562) normal=(-0.054,-0.002,0.999) tests=2 passed=1 from=(-64.421,674.834,15.068) to=(-64.421,674.834,5.068)
    2026-09-29T05:40:14.496Z [INFO] command source=file:cmd_20260929054014439.cmd line="ray down 10" reply="ray dir=down mask=All status=Hit kind=World handle=0 distance=1.505 pos=(-64.421,674.834,13.562) normal=(-0.054,-0.002,0.999) tests=2 passed=1 from=(-64.421,674.834,15.068) to=(-64.421,674.834,5.068)"
    2026-09-29T05:40:14.536Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-69.55, 654.353, 15.173) to=(-67.543, 661.921, 14.931)
    2026-09-29T05:40:14.537Z [INFO] [autopilot] event PedAppeared handle=8707
    2026-09-29T05:40:14.567Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-69.566, 649.095, 15.2) to=(-67.548, 659.151, 14.928)
    2026-09-29T05:40:14.713Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-69.326, 654.331, 15.122) to=(-67.363, 662.443, 15.052)
    2026-09-29T05:40:14.759Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-69.583, 649.1, 15.19) to=(-67.559, 659.969, 14.92)
    2026-09-29T05:40:14.854Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-69.176, 654.327, 15.135) to=(113.453, 1433.553, -6.092)
    2026-09-29T05:40:15.013Z [INFO] ray dir=up mask=All status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,15.568) to=(-64.421,674.834,18.568)
    2026-09-29T05:40:15.014Z [INFO] command source=file:cmd_20260929054014826.cmd line="ray up 3 1.0" reply="ray dir=up mask=All status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,15.568) to=(-64.421,674.834,18.568)"
    2026-09-29T05:40:15.263Z [INFO] [autopilot] event PedRemoved handle=5634
    2026-09-29T05:40:15.268Z [INFO] command source=file:cmd_20260929054015200.cmd line="spawncar admiral 6" reply="spawning admiral"
    2026-09-29T05:40:15.374Z [INFO] [autopilot] autopilot_spawncar handle=7682 model=admiral
    2026-09-29T05:40:15.398Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.693, 637.634, 15.188) to=(333.979, 1318.852, 17.809)
    2026-09-29T05:40:15.399Z [INFO] [autopilot] event VehicleAppeared handle=7682
    2026-09-29T05:40:15.548Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.642, 637.709, 15.186) to=(-31.37, 727.855, 15.454)
    2026-09-29T05:40:15.609Z [INFO] [autopilot] event PedRemoved handle=6914
    2026-09-29T05:40:15.656Z [INFO] [autopilot] event PedRemoved handle=6146
    2026-09-29T05:40:15.725Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.637, 637.719, 15.203) to=(-76.014, 654.977, 14.698)
    2026-09-29T05:40:15.763Z [INFO] [autopilot] event PedRemoved handle=6658
    2026-09-29T05:40:15.922Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.653, 637.745, 15.194) to=(-67.549, 669.087, 14.502)
    2026-09-29T05:40:15.965Z [INFO] [autopilot] event PedRemoved handle=5890
    2026-09-29T05:40:15.995Z [INFO] [autopilot] event PedAppeared handle=1027
    2026-09-29T05:40:16.055Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.651, 637.753, 15.194) to=(-30.36, 725.91, 15.925)
    2026-09-29T05:40:16.056Z [INFO] [autopilot] event PedRemoved handle=6402
    2026-09-29T05:40:16.111Z [INFO] [autopilot] event PedAppeared handle=1283
    2026-09-29T05:40:16.190Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.381, 635.98, 15.184) to=(266.888, 1356.471, 19.143)
    2026-09-29T05:40:16.308Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-67.743, 650.166, 15.14) to=(43.135, 1442.794, -7.864)
    2026-09-29T05:40:16.357Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.381, 635.991, 15.191) to=(253.715, 1362.466, -4.706)
    2026-09-29T05:40:16.473Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-67.277, 650.062, 15.14) to=(-27.278, 945.796, 16.704)
    2026-09-29T05:40:16.519Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.385, 636.004, 15.2) to=(-77.592, 646.238, 15.027)
    2026-09-29T05:40:16.634Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.86, 649.959, 15.088) to=(-64.719, 680.214, 14.936)
    2026-09-29T05:40:16.634Z [INFO] [autopilot] event VehicleDamaged handle=7682 1000->988 engine 1000->1000
    2026-09-29T05:40:16.686Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.401, 636.02, 15.193) to=(-62.956, 678.259, 15.242)
    2026-09-29T05:40:16.806Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.556, 649.833, 15.115) to=(8.829, 1447.018, 19.127)
    2026-09-29T05:40:16.858Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.387, 636.026, 15.197) to=(-77.602, 646.248, 15.037)
    2026-09-29T05:40:17.239Z [INFO] [autopilot] event PedAppeared handle=1539
    2026-09-29T05:40:17.840Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.347, 636.002, 15.184) to=(-77.755, 646.524, 15.128)
    2026-09-29T05:40:18.031Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.612, 637.729, 15.183) to=(318.714, 1328.192, 15.951)
    2026-09-29T05:40:18.191Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.647, 637.722, 15.198) to=(-62.888, 678.24, 15.457)
    2026-09-29T05:40:18.307Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.387, 649.604, 15.129) to=(-0.908, 1447.425, 3.206)
    2026-09-29T05:40:18.371Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.644, 637.718, 15.188) to=(335.753, 1317.908, 9.743)
    2026-09-29T05:40:18.373Z [INFO] density frame_ms=29.9 peds=0.85 cars=0.89
    2026-09-29T05:40:18.441Z [INFO] [autopilot] event PedRemoved handle=8707
    2026-09-29T05:40:18.499Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.403, 649.567, 15.14) to=(-63.475, 679.985, 14.775)
    2026-09-29T05:40:18.500Z [INFO] [autopilot] event VehicleDamaged handle=7682 988->976 engine 1000->1000
    2026-09-29T05:40:18.537Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-86.639, 637.721, 15.201) to=(-67.549, 668.573, 14.506)
    2026-09-29T05:40:18.538Z [INFO] [autopilot] event PedRemoved handle=1283
    2026-09-29T05:40:18.675Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.397, 649.561, 15.134) to=(-64.404, 674.69, 15.062)
    2026-09-29T05:40:18.678Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.404, 674.69, 15.062) dir=(0.079, 0.997, -0.003)
    2026-09-29T05:40:18.785Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.353, 636.001, 15.183) to=(-77.484, 646.255, 15.157)
    2026-09-29T05:40:18.955Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.385, 635.989, 15.196) to=(267.112, 1356.295, -0.013)
    2026-09-29T05:40:18.959Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=1631 core=on peds=9 vehicles=18 modules=10/10 coroutines=0 resources=3 raycast=on episode=GTAIV frame_ms=32.06 p95_ms=45.58 pressure=0.04 private_mb=1868 working_set_mb=1812 address_free_mb=1543 largest_free_block_mb=1511 managed_mb=15 physical_load=95% core_us=59.4
    2026-09-29T05:40:19.010Z [INFO] [autopilot] event PedRemoved handle=2819
    2026-09-29T05:40:19.101Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.379, 635.987, 15.187) to=(-81.77, 637.214, 15.161)
    2026-09-29T05:40:19.102Z [INFO] [autopilot] event PedRemoved handle=1539
    2026-09-29T05:40:19.153Z [INFO] performance samples=1634 frame_p50_ms=9 frame_p95_ms=35 frame_p99_ms=62 frames_over_33ms=99 frames_over_50ms=28 gunplay_avg_ms=1.162 gunplay_max_ms=3.323 phase_samples=1635 phase_setup_avg_ms=0.830 phase_setup_max_ms=138.030 phase_camera_avg_ms=0.024 phase_camera_max_ms=13.753 phase_bullets_avg_ms=0.007 phase_bullets_max_ms=5.393 phase_weapon_avg_ms=0.020 phase_weapon_max_ms=3.279 phase_hud_avg_ms=0.382 phase_hud_max_ms=3.942
    2026-09-29T05:40:19.169Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=4.629/3656.5/1634@7 total=7564 module.gunplay=1.267/169.1/1634@7 total=2070 tick.gunplay=1.265/168.9/1634@7 total=2067 gp.freeaim=0.659/42.0/1635@7 total=1078 module.arsenal=0.771/105.2/620@7 total=478 tick.arsenal=0.770/105.2/620@7 total=477 module.atmosphere=0.154/7.5/1634@7 total=251 tick.atmosphere=0.152/6.9/1634@7 total=249 engine.world=0.140/28.3/1635@7 total=229 module.combat=0.133/150.3/1634@7 total=218 tick.combat=0.133/150.2/1634@7 total=217 ar.storage=0.297/3.2/620@7 total=184 module.holsters=0.494/69.7/327@7 total=161 tick.holsters=0.493/69.7/327@7 total=161 ho.show=0.417/69.6/326@7 total=136 ar.reconcile=0.216/85.2/620@7 total=134 gp.index_pad=0.067/0.6/1635@7 total=109 gp.player=0.061/85.2/1635@7 total=100 ar.safehouse_flags=0.141/2.7/620@7 total=88 combat.sample=0.721/2.0/55@7 total=40 engine.scheduler=0.018/16.2/1635@7 total=29 gp.shoulder=0.016/1.5/1635@7 total=26 module.devtools=0.031/3.8/620@7 total=19 tick.devtools=0.030/3.8/620@7 total=19 gp.weapon_id=0.011/1.7/1635@7 total=18 cam.handle=0.010/0.9/1635@7 total=17 cam.find_active=0.010/13.0/1635@7 total=17 gp.cycle=0.008/1.3/1635@7 total=13 gp.state=0.006/1.0/1635@7 total=9 gp.shots=0.004/4.0/1635@7 total=7 module.world=0.135/3.0/47@7 total=6 module.probe=1.485/1.6/3@7 total=4 cam.aim_key=0.002/0.1/1635@7 total=4 gp.spread=0.002/0.8/1635@7 total=3 ar.discover=0.005/2.2/620@7 total=3 combat.dismember=0.002/2.0/1634@7 total=3 ar.vehicle=0.005/1.2/620@7 total=3 ar.lvs=0.003/0.5/620@7 total=2 engine.raycast=0.589/1.1/2@7 total=1 combat.blood=0.001/0.7/1634@7 total=1 gp.feel=0.001/0.7/1635@7 total=1 ho.carried=0.002/0.1/326@7 total=1 combat.pending=0.000/0.3/1634@7 total=1 gp.recoil=0.000/0.4/1635@7 total=1 module.autopilot=0.000/0.0/1634@7 total=0 cam.fov=0.004/0.3/89@7 total=0 module.weapon-probe=0.000/0.0/1634@7 total=0
    2026-09-29T05:40:19.170Z [INFO] engine_thread_probe ticks=1633 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1631 ticks_after_skipped_frames=1
    2026-09-29T05:40:19.174Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=13.73 shdn_us=103.9
    2026-09-29T05:40:19.255Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.378, 635.988, 15.199) to=(-77.793, 646.533, 15.101)
    2026-09-29T05:40:19.429Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-85.81, 638.592, 15.133) to=(-30.79, 725.91, 16.593)
    2026-09-29T05:40:19.429Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.388, 635.992, 15.193) to=(259.488, 1359.951, 13.237)
    2026-09-29T05:40:19.624Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-85.494, 639.007, 15.123) to=(-67.485, 668.843, 14.97)
    2026-09-29T05:40:19.625Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.387, 635.989, 15.198) to=(-77.651, 646.261, 14.994)
    2026-09-29T05:40:19.750Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.553, 648.767, 15.038) to=(-14.816, 1448.016, 21.403)
    2026-09-29T05:40:19.776Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-85.021, 639.564, 15.133) to=(-75.961, 655.052, 14.775)
    2026-09-29T05:40:19.840Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:40:19.875Z [INFO] ray dir=forward mask=All status=Hit kind=Vehicle handle=7682 distance=4.995 pos=(-64.421,679.829,14.268) normal=(0.017,-0.955,-0.296) tests=1 passed=0 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)
    2026-09-29T05:40:19.876Z [INFO] command source=file:cmd_20260929054019654.cmd line="ray forward 12 -0.3" reply="ray dir=forward mask=All status=Hit kind=Vehicle handle=7682 distance=4.995 pos=(-64.421,679.829,14.268) normal=(0.017,-0.955,-0.296) tests=1 passed=0 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)"
    2026-09-29T05:40:19.905Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.381, 648.363, 14.992) to=(-32.601, 1448.32, 16.506)
    2026-09-29T05:40:19.933Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.646, 639.995, 15.116) to=(332.913, 1323.37, 18.635)
    2026-09-29T05:40:20.091Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.388, 647.982, 15.028) to=(-62.42, 720.669, 15.607)
    2026-09-29T05:40:20.132Z [INFO] ray dir=forward mask=World|Peds status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=2 passed=1 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)
    2026-09-29T05:40:20.132Z [INFO] command source=file:cmd_20260929054020048.cmd line="ray forward 12 -0.3 world,peds" reply="ray dir=forward mask=World|Peds status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=2 passed=1 from=(-64.421,674.834,14.268) to=(-64.421,686.834,14.268)"
    2026-09-29T05:40:20.243Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.487, 647.557, 14.997) to=(-64.603, 675.009, 14.419)
    2026-09-29T05:40:20.244Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.603, 675.009, 14.419) dir=(0.032, 0.999, -0.021)
    2026-09-29T05:40:20.587Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-65.545, 647.176, 15.039) to=(-50.689, 1447.698, 1.123)
    2026-09-29T05:40:20.693Z [INFO] [autopilot] autopilot_rayto target=car handle=7682 mask=All status=Hit kind=Vehicle hit_handle=7682 distance=5 of=5.996 normal=(-0.023, -0.912, 0.41) tests=1 passed=0 match=True
    2026-09-29T05:40:20.694Z [INFO] command source=file:cmd_20260929054020446.cmd line="rayto car" reply="autopilot_rayto target=car handle=7682 mask=All status=Hit kind=Vehicle hit_handle=7682 distance=5 of=5.996 normal=(-0.023, -0.912, 0.41) tests=1 passed=0 match=True"
    2026-09-29T05:40:20.766Z [INFO] [autopilot] event PedAppeared handle=1540
    2026-09-29T05:40:20.808Z [INFO] [autopilot] event PedRemoved handle=1027
    2026-09-29T05:40:20.942Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.103, 640.803, 15.185) to=(324.058, 1329.613, 23.744)
    2026-09-29T05:40:20.948Z [INFO] raybits dir=forward m=12.0 mode=1 start=(-64.421,674.834,14.268) hits: b3=Vehicle/4.995m
    2026-09-29T05:40:20.949Z [INFO] command source=file:cmd_20260929054020826.cmd line="raybits forward 12 1 -0.3" reply="raybits dir=forward m=12.0 mode=1 start=(-64.421,674.834,14.268) hits: b3=Vehicle/4.995m"
    2026-09-29T05:40:21.044Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.877, 658.793, 15.075) to=(-64.369, 679.82, 14.4)
    2026-09-29T05:40:21.045Z [INFO] [autopilot] event VehicleDamaged handle=7682 976->970 engine 1000->1000
    2026-09-29T05:40:21.119Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.121, 640.795, 15.197) to=(-35.447, 725.544, 15.975)
    2026-09-29T05:40:21.245Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.034, 658.839, 15.122) to=(-1.363, 1456.899, 6.498)
    2026-09-29T05:40:21.272Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.117, 640.785, 15.189) to=(311.736, 1336.725, 16.241)
    2026-09-29T05:40:21.379Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.033, 658.839, 15.111) to=(-64.386, 679.931, 14.645)
    2026-09-29T05:40:21.380Z [INFO] [autopilot] event VehicleDamaged handle=7682 970->964 engine 1000->1000
    2026-09-29T05:40:21.426Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.113, 640.769, 15.198) to=(326.074, 1328.266, 1.852)
    2026-09-29T05:40:21.454Z [INFO] command source=file:cmd_20260929054021206.cmd line="clear" reply="cleared 1"
    2026-09-29T05:40:21.475Z [INFO] [autopilot] event VehicleRemoved handle=7682
    2026-09-29T05:40:21.527Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.031, 658.846, 15.124) to=(-62.725, 697.497, 13.701)
    2026-09-29T05:40:21.574Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.126, 640.76, 15.192) to=(-67.549, 669.045, 14.714)
    2026-09-29T05:40:21.650Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.357, 636.003, 15.182) to=(-31.705, 744.743, 15.583)
    2026-09-29T05:40:21.675Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.047, 658.847, 15.118) to=(-58.937, 738.026, 13.648)
    2026-09-29T05:40:21.747Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.12, 640.752, 15.195) to=(324.415, 1329.163, -1.656)
    2026-09-29T05:40:22.093Z [INFO] [autopilot] event VehicleRemoved handle=1027
    2026-09-29T05:40:22.227Z [INFO] [autopilot] event VehicleRemoved handle=6914
    2026-09-29T05:40:22.573Z [INFO] [autopilot] event VehicleAppeared handle=1795
    2026-09-29T05:40:22.637Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.089, 640.763, 15.183) to=(-75.731, 655.047, 14.771)
    2026-09-29T05:40:22.718Z [INFO] command source=file:cmd_20260929054022600.cmd line="spawn 1 5 0" reply="spawning 1 at 5 m"
    2026-09-29T05:40:22.769Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.123, 640.754, 15.198) to=(-67.549, 670.222, 14.687)
    2026-09-29T05:40:22.770Z [INFO] [autopilot] event PedAppeared handle=1284
    2026-09-29T05:40:22.771Z [INFO] [autopilot] autopilot_spawned count=1
    2026-09-29T05:40:22.843Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.358, 636.001, 15.186) to=(253.586, 1362.652, 5.535)
    2026-09-29T05:40:22.937Z [INFO] [autopilot] event VehicleRemoved handle=2562
    2026-09-29T05:40:23.012Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.389, 635.989, 15.198) to=(-77.563, 646.859, 15.161)
    2026-09-29T05:40:23.013Z [INFO] [autopilot] event PedAppeared handle=2820
    2026-09-29T05:40:23.195Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.386, 635.985, 15.189) to=(-31.69, 749.419, 15.765)
    2026-09-29T05:40:23.219Z [INFO] [autopilot] event PedRemoved handle=1540
    2026-09-29T05:40:23.358Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.379, 635.988, 15.198) to=(-77.492, 646.104, 14.996)
    2026-09-29T05:40:23.403Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.215, 646.708, 15.072) to=(-13.614, 1445.636, 20.501)
    2026-09-29T05:40:23.532Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.388, 635.992, 15.192) to=(-77.633, 646.402, 15.132)
    2026-09-29T05:40:23.574Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.119, 646.632, 15.116) to=(-64.156, 674.629, 14.538)
    2026-09-29T05:40:23.575Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C9 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.156, 674.629, 14.538) dir=(0.07, 0.997, -0.021)
    2026-09-29T05:40:23.604Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.892, 658.812, 15.083) to=(-64.729, 674.925, 14.567)
    2026-09-29T05:40:23.606Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.729, 674.925, 14.567) dir=(0.072, 0.997, -0.032)
    2026-09-29T05:40:23.656Z [INFO] [autopilot] event PedAppeared handle=7170
    2026-09-29T05:40:23.657Z [INFO] [autopilot] event PedAppeared handle=7426
    2026-09-29T05:40:23.658Z [INFO] [autopilot] event PedAppeared handle=7682
    2026-09-29T05:40:23.712Z [INFO] [autopilot] event VehicleRemoved handle=3330
    2026-09-29T05:40:23.736Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.115, 646.626, 15.113) to=(-22.678, 1446.134, 20.117)
    2026-09-29T05:40:23.765Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.01, 658.82, 15.121) to=(21.454, 1454.673, 6.423)
    2026-09-29T05:40:23.889Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.11, 646.634, 15.125) to=(-59.987, 724.233, 13.569)
    2026-09-29T05:40:23.940Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.008, 658.822, 15.11) to=(-64.491, 674.717, 14.461)
    2026-09-29T05:40:23.941Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.491, 674.717, 14.461) dir=(0.095, 0.995, -0.041)
    2026-09-29T05:40:23.996Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.088, 640.76, 15.184) to=(316.533, 1333.852, 4.821)
    2026-09-29T05:40:24.053Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.128, 646.638, 15.122) to=(-19.246, 1445.856, 11.633)
    2026-09-29T05:40:24.115Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.007, 658.829, 15.122) to=(12.398, 1455.173, -11.886)
    2026-09-29T05:40:24.177Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-84.096, 640.752, 15.169) to=(322.484, 1330.423, 9.332)
    2026-09-29T05:40:24.268Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.025, 658.835, 15.116) to=(-5.838, 1457.117, -4.488)
    2026-09-29T05:40:24.357Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-83.939, 640.881, 15.141) to=(-62.972, 678.257, 15.095)
    2026-09-29T05:40:24.423Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.358, 636, 15.185) to=(-63.101, 678.546, 15.43)
    2026-09-29T05:40:24.522Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-83.631, 641.224, 15.142) to=(321.471, 1331.623, -6.526)
    2026-09-29T05:40:24.595Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.391, 635.988, 15.2) to=(244.792, 1366.642, 4.3)
    2026-09-29T05:40:24.690Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-83.303, 641.641, 15.133) to=(-67.549, 670.432, 14.658)
    2026-09-29T05:40:24.761Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.386, 635.984, 15.19) to=(-77.735, 646.351, 14.999)
    2026-09-29T05:40:24.856Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-82.899, 642.131, 15.127) to=(305.303, 1342.18, -1.763)
    2026-09-29T05:40:24.915Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-57.085, 762.06, 45.41) to=(-65.639, 671.527, 13.558)
    2026-09-29T05:40:25.101Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-56.365, 758.137, 45.419) to=(-66.024, 673.154, 13.574)
    2026-09-29T05:40:25.304Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-55.704, 754.234, 45.417) to=(-63.593, 674.81, 13.681)
    2026-09-29T05:40:25.508Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-55.135, 750.551, 45.401) to=(-66.974, 671.236, 13.709)
    2026-09-29T05:40:25.608Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.358, 636.001, 15.184) to=(-81.846, 637.122, 15.161)
    2026-09-29T05:40:25.918Z [INFO] [autopilot] event VehicleAppeared handle=2051
    2026-09-29T05:40:26.079Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.69, 643.686, 15.182) to=(315.22, 1338.844, -3.943)
    2026-09-29T05:40:26.080Z [INFO] [autopilot] event PedRemoved handle=2820
    2026-09-29T05:40:26.208Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.881, 658.806, 15.073) to=(8.353, 1455.719, -6.464)
    2026-09-29T05:40:26.269Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.717, 643.687, 15.197) to=(-67.549, 669.457, 14.575)
    2026-09-29T05:40:26.404Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.012, 658.825, 15.122) to=(-64.637, 674.994, 14.482)
    2026-09-29T05:40:26.405Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C3 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.637, 674.994, 14.482) dir=(0.085, 0.996, -0.039)
    2026-09-29T05:40:26.448Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.712, 643.675, 15.188) to=(312.617, 1340.362, 4.541)
    2026-09-29T05:40:26.494Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-54.129, 734.542, 47.766) to=(-62.966, 676.966, 18.17)
    2026-09-29T05:40:26.631Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.711, 643.672, 15.195) to=(-75.115, 655.06, 15.103)
    2026-09-29T05:40:26.632Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.014, 658.827, 15.11) to=(-64.566, 674.805, 14.826)
    2026-09-29T05:40:26.632Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A0 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.566, 674.805, 14.826) dir=(0.09, 0.996, -0.018)
    2026-09-29T05:40:26.685Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-54.174, 732.148, 48.614) to=(-64.964, 675.618, 13.558)
    2026-09-29T05:40:26.727Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.014, 658.839, 15.121) to=(-64.687, 674.867, 14.928)
    2026-09-29T05:40:26.728Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.687, 674.867, 14.928) dir=(0.082, 0.997, -0.012)
    2026-09-29T05:40:26.779Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.355, 636, 15.183) to=(244.116, 1367.021, 4.92)
    2026-09-29T05:40:26.876Z [INFO] [autopilot] event BulletFired shooter=7682 weapon=15 by_player=False from=(-54.286, 728.686, 50.01) to=(-63.861, 674.652, 13.597)
    2026-09-29T05:40:26.915Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.026, 658.838, 15.116) to=(-2.07, 1457.035, 9.639)
    2026-09-29T05:40:26.915Z [INFO] [autopilot] event PedAppeared handle=6403
    2026-09-29T05:40:26.951Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.386, 635.989, 15.195) to=(249.292, 1364.459, -6.248)
    2026-09-29T05:40:27.058Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.03, 658.845, 15.119) to=(-64.283, 679.626, 14.94)
    2026-09-29T05:40:27.059Z [INFO] [autopilot] event PedDamaged handle=1284 100->4 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=96.0 health_lost=96.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.283, 679.626, 14.94) dir=(0.084, 0.996, -0.009)
    2026-09-29T05:40:27.088Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.385, 635.985, 15.188) to=(-77.727, 646.343, 14.999)
    2026-09-29T05:40:27.251Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.382, 635.988, 15.202) to=(-67.565, 666.872, 15.195)
    2026-09-29T05:40:27.286Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.284, 646.734, 15.068) to=(-26.821, 1446.441, 13.695)
    2026-09-29T05:40:27.385Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.395, 635.99, 15.195) to=(258.738, 1360.323, 16.279)
    2026-09-29T05:40:27.386Z [INFO] [autopilot] event PedRemoved handle=6403
    2026-09-29T05:40:27.425Z [INFO] ray dir=forward mask=All status=Hit kind=Ped handle=1284 distance=5.177 pos=(-64.421,680.011,14.568) normal=(0.663,-0.730,-0.165) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)
    2026-09-29T05:40:27.426Z [INFO] command source=file:cmd_20260929054027211.cmd line="ray forward 10 0" reply="ray dir=forward mask=All status=Hit kind=Ped handle=1284 distance=5.177 pos=(-64.421,680.011,14.568) normal=(0.663,-0.730,-0.165) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)"
    2026-09-29T05:40:27.460Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.186, 646.641, 15.113) to=(-64.413, 674.683, 14.416)
    2026-09-29T05:40:27.461Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.413, 674.683, 14.416) dir=(0.063, 0.998, -0.025)
    2026-09-29T05:40:27.638Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.187, 646.64, 15.114) to=(-64.562, 674.871, 14.328)
    2026-09-29T05:40:27.639Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x1A2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.562, 674.871, 14.328) dir=(0.057, 0.998, -0.028)
    2026-09-29T05:40:27.677Z [INFO] ray dir=forward mask=World|Vehicles|Objects status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)
    2026-09-29T05:40:27.678Z [INFO] command source=file:cmd_20260929054027595.cmd line="ray forward 10 0 world,vehicles,objects" reply="ray dir=forward mask=World|Vehicles|Objects status=Clear kind=World handle=0 distance=0.000 pos=(0.000,0.000,0.000) normal=(0.000,0.000,0.000) tests=1 passed=0 from=(-64.421,674.834,14.568) to=(-64.421,684.834,14.568)"
    2026-09-29T05:40:27.773Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.185, 646.647, 15.132) to=(-64.661, 680.23, 14.818)
    2026-09-29T05:40:27.774Z [INFO] [autopilot] event PedDamaged handle=1284 4->-68 bone=0x4C8 by_player=False weapon=15 exact=True type=Bullet amount=72.0 health_lost=72.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=True hit=True at=(-64.661, 680.23, 14.818) dir=(0.045, 0.999, -0.009)
    2026-09-29T05:40:27.775Z [INFO] [autopilot] event PedDied handle=1284 bone=0x4C8 by_player=False exact=True type=Bullet killer=2051 weapon=15
    2026-09-29T05:40:27.837Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-53.706, 710.913, 58.279) to=(-63.874, 673.677, 13.586)
    2026-09-29T05:40:27.953Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.201, 646.652, 15.127) to=(-64.719, 674.907, 14.584)
    2026-09-29T05:40:27.954Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4C2 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2051 vehicle=0 killed=False hit=True at=(-64.719, 674.907, 14.584) dir=(0.052, 0.998, -0.019)
    2026-09-29T05:40:28.021Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-53.994, 707.976, 60.213) to=(-64.601, 672.633, 13.556)
    2026-09-29T05:40:28.211Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-54.348, 704.94, 62.416) to=(-63.002, 674.2, 13.739)
    2026-09-29T05:40:28.431Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-54.823, 701.663, 65.018) to=(-65.054, 675.824, 13.558)
    2026-09-29T05:40:28.497Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.684, 643.656, 15.184) to=(-67.549, 670.218, 14.349)
    2026-09-29T05:40:28.498Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.358, 636.003, 15.182) to=(254.72, 1362.211, 16.734)
    2026-09-29T05:40:28.630Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-55.304, 698.988, 67.29) to=(-63.552, 674.74, 13.685)
    2026-09-29T05:40:28.670Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.718, 643.649, 15.196) to=(-75.501, 655.06, 14.781)
    2026-09-29T05:40:28.671Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.387, 635.989, 15.196) to=(-67.417, 667.061, 15.326)
    2026-09-29T05:40:28.781Z [INFO] [autopilot] event PedDamaged handle=1284 -68->-72 bone=0xFFFFFFFF by_player=False weapon=54 exact=True type=Fall amount=3.9 health_lost=3.9 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:28.782Z [INFO] [autopilot] event PedAppeared handle=2821
    2026-09-29T05:40:28.827Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.385, 635.984, 15.189) to=(243.181, 1367.369, 2.83)
    2026-09-29T05:40:28.903Z [INFO] [autopilot] event PedDamaged handle=1284 -72->-73 bone=0x4C1 by_player=False weapon=54 exact=True type=Fall amount=0.7 health_lost=0.7 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:28.904Z [INFO] [autopilot] event PedAppeared handle=1796
    2026-09-29T05:40:28.905Z [INFO] [autopilot] event PedAppeared handle=7938
    2026-09-29T05:40:28.905Z [INFO] [autopilot] event PedAppeared handle=8194
    2026-09-29T05:40:28.977Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.383, 635.989, 15.203) to=(258.083, 1360.588, 11.825)
    2026-09-29T05:40:29.140Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.391, 635.992, 15.194) to=(-77.802, 646.415, 14.997)
    2026-09-29T05:40:29.837Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:40:29.971Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.875, 658.778, 15.077) to=(-56.535, 780.528, 13.687)
    2026-09-29T05:40:29.995Z [INFO] [autopilot] event PedDamaged handle=1284 -73->-74 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:30.068Z [INFO] [autopilot] event PedRemoved handle=2821
    2026-09-29T05:40:30.137Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.021, 658.821, 15.122) to=(9.577, 1455.26, -20.135)
    2026-09-29T05:40:30.185Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.689, 643.66, 15.184) to=(-67.549, 670.506, 14.609)
    2026-09-29T05:40:30.186Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.356, 636.002, 15.182) to=(-62.934, 678.258, 15.521)
    2026-09-29T05:40:30.309Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.035, 658.828, 15.111) to=(-64.323, 674.832, 15.129)
    2026-09-29T05:40:30.310Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.323, 674.832, 15.129) dir=(0.106, 0.994, 0.001)
    2026-09-29T05:40:30.338Z [INFO] [autopilot] event PedDamaged handle=1284 -74->-75 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:30.365Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.72, 643.649, 15.197) to=(298.547, 1348.076, 0.721)
    2026-09-29T05:40:30.366Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.387, 635.989, 15.196) to=(-67.59, 667.497, 15.289)
    2026-09-29T05:40:30.478Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.03, 658.834, 15.121) to=(2.526, 1456.364, -0.749)
    2026-09-29T05:40:30.534Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.717, 643.644, 15.189) to=(-67.549, 670.131, 14.381)
    2026-09-29T05:40:30.535Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.385, 635.984, 15.189) to=(249.051, 1364.794, 16.404)
    2026-09-29T05:40:30.629Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-59.943, 683.005, 82.819) to=(-64.214, 676.439, 13.577)
    2026-09-29T05:40:30.664Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-66.049, 658.84, 15.117) to=(-57.479, 730.582, 13.688)
    2026-09-29T05:40:30.665Z [INFO] [autopilot] event PedDamaged handle=1284 -75->-76 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:30.704Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.713, 643.648, 15.202) to=(-64.479, 674.79, 15.24)
    2026-09-29T05:40:30.705Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.383, 635.989, 15.203) to=(-77.272, 646.764, 15.161)
    2026-09-29T05:40:30.706Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x4B5 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=3330 vehicle=0 killed=False hit=True at=(-64.479, 674.79, 15.24) dir=(0.484, 0.875, 0.001)
    2026-09-29T05:40:30.810Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-60.063, 682.262, 83.526) to=(-62.883, 673.244, 13.739)
    2026-09-29T05:40:30.864Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.398, 635.988, 15.196) to=(243.75, 1367.243, 15.697)
    2026-09-29T05:40:30.906Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.723, 643.651, 15.193) to=(313.188, 1340.155, 15.641)
    2026-09-29T05:40:30.907Z [INFO] [autopilot] event PedRemoved handle=8450
    2026-09-29T05:40:31.037Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-60.147, 681.466, 84.337) to=(-64.522, 673.016, 13.553)
    2026-09-29T05:40:31.038Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.39, 635.989, 15.199) to=(-77.614, 646.252, 15.014)
    2026-09-29T05:40:31.039Z [INFO] [autopilot] event PedDamaged handle=1284 -76->-77 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:31.185Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.276, 646.741, 15.077) to=(-1.796, 1444.823, 9.938)
    2026-09-29T05:40:31.336Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.171, 646.653, 15.11) to=(-22.608, 1446.044, 9.879)
    2026-09-29T05:40:31.336Z [INFO] [autopilot] event PedDamaged handle=1284 -77->-78 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:31.372Z [INFO] [autopilot] event PedAppeared handle=5636
    2026-09-29T05:40:31.498Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.173, 646.64, 15.115) to=(-24.588, 1446.197, 13.658)
    2026-09-29T05:40:31.680Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.17, 646.647, 15.13) to=(-62.149, 736.15, 13.741)
    2026-09-29T05:40:31.681Z [INFO] [autopilot] event PedDamaged handle=1284 -78->-79 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:31.828Z [INFO] [autopilot] event BulletFired shooter=2051 weapon=15 by_player=False from=(-66.188, 646.646, 15.127) to=(-33.255, 1446.62, 2.892)
    2026-09-29T05:40:32.002Z [INFO] [autopilot] event PedDamaged handle=1284 -79->-80 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:32.132Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-59.389, 678.597, 88.199) to=(-63.812, 674.032, 13.65)
    2026-09-29T05:40:32.212Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.689, 643.657, 15.186) to=(299.063, 1347.949, 11.629)
    2026-09-29T05:40:32.307Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-59.099, 678.283, 88.714) to=(-65.046, 676.514, 13.558)
    2026-09-29T05:40:32.330Z [INFO] [autopilot] event PedDamaged handle=1284 -80->-81 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:32.355Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.355, 635.998, 15.185) to=(-77.564, 646.211, 15.034)
    2026-09-29T05:40:32.389Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.721, 643.649, 15.198) to=(-75.203, 654.915, 15.003)
    2026-09-29T05:40:32.517Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-58.731, 677.948, 89.25) to=(-65.837, 675.455, 13.564)
    2026-09-29T05:40:32.518Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-82.389, 635.989, 15.198) to=(254.497, 1362.238, 11.752)
    2026-09-29T05:40:32.543Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.718, 643.645, 15.188) to=(-67.549, 670.491, 14.597)
    2026-09-29T05:40:32.544Z [INFO] [autopilot] event VehicleRemoved handle=5122
    2026-09-29T05:40:32.679Z [INFO] [autopilot] event PedDamaged handle=1284 -81->-82 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:32.709Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.713, 643.648, 15.199) to=(318.43, 1337.103, 3.226)
    2026-09-29T05:40:32.710Z [INFO] [autopilot] event VehicleAppeared handle=2563
    2026-09-29T05:40:32.739Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-58.304, 677.605, 89.76) to=(-66.532, 675.623, 13.601)
    2026-09-29T05:40:32.884Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.724, 643.651, 15.193) to=(-63.079, 678.334, 15.177)
    2026-09-29T05:40:32.912Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-57.946, 677.336, 90.121) to=(-65.199, 673.682, 13.558)
    2026-09-29T05:40:33.010Z [INFO] [autopilot] event PedDamaged handle=1284 -82->-83 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:33.041Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.722, 643.648, 15.199) to=(303.966, 1344.98, -6.301)
    2026-09-29T05:40:33.338Z [INFO] [autopilot] event PedDamaged handle=1284 -83->-84 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:33.346Z [INFO] [autopilot] autopilot_rayto target=0 handle=1284 mask=All status=Hit kind=Ped hit_handle=1284 distance=7.423 of=7.569 normal=(0.018, -0.989, 0.146) tests=1 passed=0 match=True
    2026-09-29T05:40:33.347Z [INFO] command source=file:cmd_20260929054033146.cmd line="rayto 0" reply="autopilot_rayto target=0 handle=1284 mask=All status=Hit kind=Ped hit_handle=1284 distance=7.423 of=7.569 normal=(0.018, -0.989, 0.146) tests=1 passed=0 match=True"
    2026-09-29T05:40:33.513Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-81.87, 639.116, 15.163) to=(-67.549, 669.064, 14.68)
    2026-09-29T05:40:33.592Z [INFO] [autopilot] autopilot_los subject=0 handle=1284 available=True sight=True back=True spotted=True
    2026-09-29T05:40:33.592Z [INFO] command source=file:cmd_20260929054033531.cmd line="los 0" reply="autopilot_los subject=0 handle=1284 available=True sight=True back=True spotted=True"
    2026-09-29T05:40:33.676Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-81.797, 639.705, 15.116) to=(-81.386, 640.49, 15.144)
    2026-09-29T05:40:33.677Z [INFO] [autopilot] event PedDamaged handle=1284 -84->-85 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:33.843Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-81.808, 640.004, 15.163) to=(280.26, 1354.152, 21.254)
    2026-09-29T05:40:33.959Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.354, 659.261, 15.003) to=(-64.537, 674.833, 15.086)
    2026-09-29T05:40:33.960Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.537, 674.833, 15.086) dir=(0.052, 0.999, 0.005)
    2026-09-29T05:40:33.961Z [INFO] [autopilot] event PedDamaged handle=1284 -85->-86 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:33.990Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-81.815, 640.048, 15.203) to=(286.374, 1351.015, 9.13)
    2026-09-29T05:40:34.055Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.696, 643.67, 15.186) to=(301.747, 1346.438, 7.196)
    2026-09-29T05:40:34.123Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-56.041, 676.042, 91.868) to=(-66.069, 675.601, 13.576)
    2026-09-29T05:40:34.124Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.211, 659.867, 15.014) to=(-56.143, 783.02, 13.669)
    2026-09-29T05:40:34.128Z [INFO] command source=file:cmd_20260929054033917.cmd line="raystats" reply="raycast available=True enabled=True installed=1 queries=42 tests=46 hits=6 clears=36 passes=4 inconclusive=0 faults=0"
    2026-09-29T05:40:34.159Z [INFO] [autopilot] event BulletFired shooter=3074 weapon=15 by_player=False from=(-81.83, 640.058, 15.195) to=(-74.588, 654.879, 15.122)
    2026-09-29T05:40:34.216Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.722, 643.649, 15.198) to=(-62.92, 678.246, 15.178)
    2026-09-29T05:40:34.270Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.209, 660.46, 15.017) to=(-64.445, 674.685, 14.464)
    2026-09-29T05:40:34.270Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.445, 674.685, 14.464) dir=(0.054, 0.998, -0.039)
    2026-09-29T05:40:34.271Z [INFO] [autopilot] event PedDamaged handle=1284 -86->-87 bone=0x4C8 by_player=False weapon=-1 exact=False type=Unknown amount=0.0 health_lost=0.0 armour_lost=0.0 attacker=0 vehicle=0 killed=False hit=False
    2026-09-29T05:40:34.301Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-55.841, 676.103, 92.115) to=(-65.004, 673.743, 13.558)
    2026-09-29T05:40:34.399Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.718, 643.645, 15.187) to=(303.248, 1345.355, -8.771)
    2026-09-29T05:40:34.406Z [INFO] command source=file:cmd_20260929054034305.cmd line="clear" reply="cleared 1"
    2026-09-29T05:40:34.435Z [INFO] [autopilot] event PedRemoved handle=1284
    2026-09-29T05:40:34.466Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.142, 661.073, 15.042) to=(-64.47, 674.697, 14.912)
    2026-09-29T05:40:34.467Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.47, 674.697, 14.912) dir=(0.049, 0.999, -0.01)
    2026-09-29T05:40:34.499Z [INFO] [autopilot] event BulletFired shooter=7426 weapon=15 by_player=False from=(-55.628, 676.277, 92.363) to=(-66.352, 675.511, 13.591)
    2026-09-29T05:40:34.564Z [INFO] [autopilot] event BulletFired shooter=3330 weapon=15 by_player=False from=(-81.713, 643.648, 15.199) to=(-31.57, 732.785, 16.149)
    2026-09-29T05:40:34.596Z [INFO] [autopilot] event BulletFired shooter=2563 weapon=15 by_player=False from=(-65.119, 661.522, 15.012) to=(-64.504, 674.736, 14.514)
    2026-09-29T05:40:34.597Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0xFFFFFFFF by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=2563 vehicle=0 killed=False hit=True at=(-64.504, 674.736, 14.514) dir=(0.046, 0.998, -0.038)

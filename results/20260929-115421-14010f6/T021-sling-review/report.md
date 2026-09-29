# Scenario sling-review

- Result: PASS
- Steps: 31, failed: 0
- Game alive at end: True
- Log errors during run: 0

## Failed steps

## Steps
- events on => event log on
- god on => invincible True
- wanted 0 => wanted 0
- goto east_park => Teleporting to Algonquin East Park Street
- wait 4000 ms
- time 13 0 => time 13:00
- weather 1 => weather 1
- mark log line 352
- give 15 200 => gave 15
- give 14 200 => gave 14
- give 7 100 => gave 7
- select 7 => selected 7
- expectmarked holster_sling_attached: OK 2026-09-29T19:06:43.040Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
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

## Log
    ﻿2026-09-29T05:54:53.784Z [INFO] [autopilot] event PedDamaged handle=2 100->100 bone=0x36A1 by_player=False weapon=15 exact=True type=Bullet amount=60.0 health_lost=0.0 armour_lost=0.0 attacker=6916 vehicle=0 killed=False hit=True at=(-64.256, 674.821, 14.998) dir=(-0.675, -0.738, -0.014)
    2026-09-29T19:06:34.868Z [INFO] command source=file:cmd_20260929190634847.cmd line="events on" reply="event log on"
    2026-09-29T19:06:35.393Z [INFO] command source=file:cmd_20260929190635269.cmd line="god on" reply="invincible True"
    2026-09-29T19:06:35.446Z [INFO] [autopilot] event PedAppeared handle=9221
    2026-09-29T19:06:35.447Z [INFO] [autopilot] event PedRemoved handle=9220
    2026-09-29T19:06:35.509Z [INFO] [autopilot] event PedRemoved handle=7173
    2026-09-29T19:06:35.687Z [INFO] [autopilot] event VehicleAppeared handle=11011
    2026-09-29T19:06:35.887Z [INFO] command source=file:cmd_20260929190635651.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T19:06:36.838Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T19:06:36.839Z [INFO] command source=file:cmd_20260929190636042.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T19:06:36.884Z [INFO] [autopilot] event PedRemoved handle=7431
    2026-09-29T19:06:36.885Z [INFO] [autopilot] event PedRemoved handle=9221
    2026-09-29T19:06:36.886Z [INFO] [autopilot] event PedRemoved handle=7685
    2026-09-29T19:06:36.886Z [INFO] [autopilot] event PedRemoved handle=2565
    2026-09-29T19:06:36.887Z [INFO] [autopilot] event PedRemoved handle=4614
    2026-09-29T19:06:36.888Z [INFO] [autopilot] event PedRemoved handle=2054
    2026-09-29T19:06:36.889Z [INFO] [autopilot] event PedRemoved handle=6662
    2026-09-29T19:06:36.889Z [INFO] [autopilot] event PedRemoved handle=3334
    2026-09-29T19:06:36.890Z [INFO] [autopilot] event PedRemoved handle=2310
    2026-09-29T19:06:36.891Z [INFO] [autopilot] event PedRemoved handle=4870
    2026-09-29T19:06:36.891Z [INFO] [autopilot] event PedRemoved handle=3846
    2026-09-29T19:06:36.892Z [INFO] [autopilot] event PedRemoved handle=12546
    2026-09-29T19:06:36.893Z [INFO] [autopilot] event PedRemoved handle=11010
    2026-09-29T19:06:36.893Z [INFO] [autopilot] event PedRemoved handle=8450
    2026-09-29T19:06:36.894Z [INFO] [autopilot] event PedRemoved handle=10499
    2026-09-29T19:06:36.895Z [INFO] [autopilot] event PedRemoved handle=8963
    2026-09-29T19:06:36.895Z [INFO] [autopilot] event PedRemoved handle=6915
    2026-09-29T19:06:36.896Z [INFO] [autopilot] event PedRemoved handle=5379
    2026-09-29T19:06:36.897Z [INFO] [autopilot] event PedRemoved handle=8195
    2026-09-29T19:06:36.897Z [INFO] [autopilot] event PedRemoved handle=10755
    2026-09-29T19:06:36.898Z [INFO] [autopilot] event PedRemoved handle=5123
    2026-09-29T19:06:36.899Z [INFO] [autopilot] event PedRemoved handle=4099
    2026-09-29T19:06:36.899Z [INFO] [autopilot] event PedRemoved handle=3587
    2026-09-29T19:06:36.900Z [INFO] [autopilot] event PedRemoved handle=3075
    2026-09-29T19:06:36.901Z [INFO] [autopilot] event PedRemoved handle=6404
    2026-09-29T19:06:36.902Z [INFO] [autopilot] event PedRemoved handle=7940
    2026-09-29T19:06:36.902Z [INFO] [autopilot] event PedRemoved handle=5892
    2026-09-29T19:06:36.903Z [INFO] [autopilot] event PedRemoved handle=2820
    2026-09-29T19:06:36.904Z [INFO] [autopilot] event PedRemoved handle=4357
    2026-09-29T19:06:36.904Z [INFO] [autopilot] event PedRemoved handle=8708
    2026-09-29T19:06:36.905Z [INFO] [autopilot] event PedRemoved handle=1540
    2026-09-29T19:06:36.906Z [INFO] [autopilot] event PedRemoved handle=10242
    2026-09-29T19:06:36.907Z [INFO] [autopilot] event VehicleRemoved handle=7941
    2026-09-29T19:06:36.907Z [INFO] [autopilot] event VehicleRemoved handle=13826
    2026-09-29T19:06:36.908Z [INFO] [autopilot] event VehicleRemoved handle=14082
    2026-09-29T19:06:36.909Z [INFO] [autopilot] event VehicleRemoved handle=12546
    2026-09-29T19:06:36.909Z [INFO] [autopilot] event VehicleRemoved handle=9218
    2026-09-29T19:06:36.910Z [INFO] [autopilot] event VehicleRemoved handle=12804
    2026-09-29T19:06:36.911Z [INFO] [autopilot] event VehicleRemoved handle=12036
    2026-09-29T19:06:36.911Z [INFO] [autopilot] event VehicleRemoved handle=5892
    2026-09-29T19:06:36.912Z [INFO] [autopilot] event VehicleRemoved handle=9988
    2026-09-29T19:06:36.913Z [INFO] [autopilot] event VehicleRemoved handle=8452
    2026-09-29T19:06:36.914Z [INFO] [autopilot] event VehicleRemoved handle=6660
    2026-09-29T19:06:36.914Z [INFO] [autopilot] event VehicleRemoved handle=8196
    2026-09-29T19:06:36.915Z [INFO] [autopilot] event VehicleRemoved handle=5380
    2026-09-29T19:06:36.916Z [INFO] [autopilot] event VehicleRemoved handle=2308
    2026-09-29T19:06:36.917Z [INFO] [autopilot] event VehicleRemoved handle=2052
    2026-09-29T19:06:36.917Z [INFO] [autopilot] event VehicleRemoved handle=1796
    2026-09-29T19:06:36.918Z [INFO] [autopilot] event VehicleRemoved handle=1540
    2026-09-29T19:06:36.919Z [INFO] [autopilot] event VehicleRemoved handle=1284
    2026-09-29T19:06:36.919Z [INFO] [autopilot] event VehicleRemoved handle=1028
    2026-09-29T19:06:36.920Z [INFO] [autopilot] event VehicleRemoved handle=772
    2026-09-29T19:06:36.921Z [INFO] [autopilot] event VehicleRemoved handle=516
    2026-09-29T19:06:36.921Z [INFO] [autopilot] event VehicleRemoved handle=11011
    2026-09-29T19:06:36.922Z [INFO] [autopilot] event VehicleRemoved handle=11267
    2026-09-29T19:06:36.923Z [INFO] [autopilot] event VehicleRemoved handle=11523
    2026-09-29T19:06:36.923Z [INFO] [autopilot] event VehicleRemoved handle=13315
    2026-09-29T19:06:36.924Z [INFO] [autopilot] event VehicleRemoved handle=10243
    2026-09-29T19:06:36.925Z [INFO] [autopilot] event VehicleRemoved handle=8963
    2026-09-29T19:06:36.925Z [INFO] [autopilot] event VehicleRemoved handle=8707
    2026-09-29T19:06:36.926Z [INFO] [autopilot] event VehicleRemoved handle=6915
    2026-09-29T19:06:36.927Z [INFO] [autopilot] event VehicleRemoved handle=4355
    2026-09-29T19:06:36.928Z [INFO] [autopilot] event VehicleRemoved handle=4099
    2026-09-29T19:06:36.928Z [INFO] [autopilot] event VehicleRemoved handle=3587
    2026-09-29T19:06:36.929Z [INFO] [autopilot] event VehicleRemoved handle=3331
    2026-09-29T19:06:36.930Z [INFO] [autopilot] event VehicleRemoved handle=3075
    2026-09-29T19:06:36.930Z [INFO] [autopilot] event VehicleRemoved handle=2819
    2026-09-29T19:06:36.931Z [INFO] [autopilot] event VehicleRemoved handle=2563
    2026-09-29T19:06:37.099Z [INFO] [world] world_object spawned name=test_wall model=lf_world_wall handle=33285 at=(-64.8, 671.4, 13.558) heading=90
    2026-09-29T19:06:37.191Z [INFO] [autopilot] event VehicleAppeared handle=5124
    2026-09-29T19:06:37.192Z [INFO] [autopilot] event VehicleAppeared handle=5636
    2026-09-29T19:06:37.193Z [INFO] [autopilot] event VehicleAppeared handle=6149
    2026-09-29T19:06:37.759Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T19:06:37.781Z [INFO] [autopilot] event VehicleRemoved handle=5124
    2026-09-29T19:06:37.800Z [INFO] [autopilot] event PedAppeared handle=4615
    2026-09-29T19:06:37.844Z [INFO] [autopilot] event PedAppeared handle=5893
    2026-09-29T19:06:37.845Z [INFO] [autopilot] event VehicleAppeared handle=7173
    2026-09-29T19:06:37.869Z [INFO] [autopilot] event PedAppeared handle=6149
    2026-09-29T19:06:37.870Z [INFO] [autopilot] event PedAppeared handle=6405
    2026-09-29T19:06:37.944Z [INFO] [autopilot] event PedAppeared handle=7432
    2026-09-29T19:06:37.970Z [INFO] [autopilot] event PedAppeared handle=7686
    2026-09-29T19:06:38.014Z [INFO] [autopilot] event PedAppeared handle=8451
    2026-09-29T19:06:38.580Z [INFO] [autopilot] event PedAppeared handle=8709
    2026-09-29T19:06:38.682Z [INFO] [autopilot] event PedAppeared handle=8964
    2026-09-29T19:06:40.287Z [INFO] [autopilot] event PedRemoved handle=7686
    2026-09-29T19:06:40.307Z [INFO] [autopilot] event PedRemoved handle=8451
    2026-09-29T19:06:40.519Z [INFO] [autopilot] event PedRemoved handle=5893
    2026-09-29T19:06:40.598Z [INFO] [autopilot] event PedRemoved handle=4615
    2026-09-29T19:06:40.680Z [INFO] [autopilot] event PedAppeared handle=5894
    2026-09-29T19:06:40.797Z [INFO] [autopilot] event PedRemoved handle=8964
    2026-09-29T19:06:40.816Z [INFO] [autopilot] event PedAppeared handle=7687
    2026-09-29T19:06:40.928Z [INFO] [autopilot] event PedRemoved handle=6405
    2026-09-29T19:06:40.949Z [INFO] [autopilot] event PedRemoved handle=7432
    2026-09-29T19:06:40.984Z [INFO] [autopilot] event PedRemoved handle=6149
    2026-09-29T19:06:41.006Z [INFO] [autopilot] event PedAppeared handle=6406
    2026-09-29T19:06:41.264Z [INFO] [autopilot] event PedAppeared handle=7433
    2026-09-29T19:06:41.357Z [INFO] [autopilot] event PedAppeared handle=8452
    2026-09-29T19:06:41.375Z [INFO] [autopilot] event PedAppeared handle=8965
    2026-09-29T19:06:41.427Z [INFO] [autopilot] event PedAppeared handle=9222
    2026-09-29T19:06:41.486Z [INFO] command source=file:cmd_20260929190641238.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T19:06:41.513Z [INFO] [autopilot] event WeatherChanged 3->4
    2026-09-29T19:06:41.514Z [INFO] [autopilot] event PedRemoved handle=8709
    2026-09-29T19:06:41.725Z [INFO] command source=file:cmd_20260929190641628.cmd line="weather 1" reply="weather 1"
    2026-09-29T19:06:41.740Z [INFO] [autopilot] event WeatherChanged 4->1
    2026-09-29T19:06:41.918Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:06:42.036Z [INFO] [autopilot] event PedAppeared handle=8196
    2026-09-29T19:06:42.161Z [INFO] [autopilot] event PedAppeared handle=8710
    2026-09-29T19:06:42.239Z [INFO] command source=file:cmd_20260929190642009.cmd line="give 15 200" reply="gave 15"
    2026-09-29T19:06:42.260Z [INFO] [autopilot] event PlayerWeaponChanged 0->15
    2026-09-29T19:06:42.268Z [INFO] weapon_changed from=0 to=15 profile=vanilla
    2026-09-29T19:06:42.270Z [INFO] arsenal_gain id=15 owned=False mission=False
    2026-09-29T19:06:42.484Z [INFO] command source=file:cmd_20260929190642399.cmd line="give 14 200" reply="gave 14"
    2026-09-29T19:06:42.509Z [INFO] [autopilot] event PlayerWeaponChanged 15->14
    2026-09-29T19:06:42.510Z [INFO] weapon_changed from=15 to=14 profile=vanilla
    2026-09-29T19:06:42.512Z [INFO] arsenal_gain id=14 owned=False mission=False
    2026-09-29T19:06:42.629Z [INFO] [autopilot] event PedAppeared handle=9478
    2026-09-29T19:06:42.969Z [INFO] [autopilot] event PedRemoved handle=7687
    2026-09-29T19:06:42.976Z [INFO] command source=file:cmd_20260929190642788.cmd line="give 7 100" reply="gave 7"
    2026-09-29T19:06:43.006Z [INFO] [autopilot] event PlayerWeaponChanged 14->7
    2026-09-29T19:06:43.008Z [INFO] weapon_changed from=14 to=7 profile=vanilla
    2026-09-29T19:06:43.010Z [INFO] arsenal_gain id=7 owned=False mission=False
    2026-09-29T19:06:43.040Z [INFO] holster_sling_attached slot=LongGun2 model=lf_sling_b bone=Spine2
    2026-09-29T19:06:43.067Z [INFO] [autopilot] event PedRemoved handle=5894
    2026-09-29T19:06:43.230Z [INFO] command source=file:cmd_20260929190643181.cmd line="select 7" reply="selected 7"
    2026-09-29T19:06:43.586Z [INFO] [autopilot] event PedAppeared handle=6150
    2026-09-29T19:06:43.746Z [INFO] command source=file:cmd_20260929190643605.cmd line="hud off" reply="hud off"
    2026-09-29T19:06:43.777Z [INFO] [autopilot] event PedRemoved handle=8965
    2026-09-29T19:06:44.009Z [INFO] command source=file:cmd_20260929190643989.cmd line="cam 0 2.2 0.4" reply="camera at 0 deg, 2.2 m"
    2026-09-29T19:06:44.051Z [INFO] [autopilot] event PedRemoved handle=6406
    2026-09-29T19:06:44.207Z [INFO] [autopilot] event PedRemoved handle=9222
    2026-09-29T19:06:44.208Z [INFO] [autopilot] event PedRemoved handle=7433
    2026-09-29T19:06:44.387Z [INFO] [autopilot] event PedAppeared handle=6407
    2026-09-29T19:06:44.504Z [INFO] [autopilot] event PedRemoved handle=8452
    2026-09-29T19:06:44.726Z [INFO] [autopilot] event PedAppeared handle=7434
    2026-09-29T19:06:44.931Z [INFO] [autopilot] event PedRemoved handle=8196
    2026-09-29T19:06:45.273Z [INFO] [autopilot] event PedAppeared handle=7688
    2026-09-29T19:06:45.833Z [INFO] [autopilot] event PedAppeared handle=7941
    2026-09-29T19:06:46.326Z [INFO] [autopilot] event VehicleRemoved handle=7173
    2026-09-29T19:06:46.568Z [INFO] [autopilot] event PedRemoved handle=6150
    2026-09-29T19:06:46.598Z [INFO] [autopilot] event PedAppeared handle=8197
    2026-09-29T19:06:46.990Z [INFO] [autopilot] event PedRemoved handle=6407
    2026-09-29T19:06:47.036Z [INFO] [autopilot] event PedAppeared handle=8453
    2026-09-29T19:06:47.323Z [INFO] [autopilot] event PedAppeared handle=8966
    2026-09-29T19:06:47.730Z [INFO] [autopilot] event PedRemoved handle=7688
    2026-09-29T19:06:47.796Z [INFO] [autopilot] event PedRemoved handle=7434
    2026-09-29T19:06:47.925Z [INFO] [autopilot] event PedAppeared handle=7689
    2026-09-29T19:06:47.930Z [INFO] camera_already_gone handle=4099 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:06:47.932Z [INFO] command source=file:cmd_20260929190647875.cmd line="cam 180 2.2 0.4" reply="camera at 180 deg, 2.2 m"
    2026-09-29T19:06:47.967Z [INFO] [autopilot] event PedRemoved handle=8710
    2026-09-29T19:06:48.207Z [INFO] [autopilot] event PedAppeared handle=8711
    2026-09-29T19:06:48.226Z [INFO] [autopilot] event PedAppeared handle=9223
    2026-09-29T19:06:49.091Z [INFO] [autopilot] event PedRemoved handle=8197
    2026-09-29T19:06:49.092Z [INFO] [autopilot] event VehicleAppeared handle=3844
    2026-09-29T19:06:49.151Z [INFO] [autopilot] event PedAppeared handle=9733
    2026-09-29T19:06:49.515Z [INFO] [autopilot] event PedAppeared handle=9987
    2026-09-29T19:06:49.782Z [INFO] [autopilot] event PedRemoved handle=8966
    2026-09-29T19:06:50.377Z [INFO] [autopilot] event PedRemoved handle=7689
    2026-09-29T19:06:50.591Z [INFO] [autopilot] event PedAppeared handle=8198
    2026-09-29T19:06:51.029Z [INFO] [autopilot] event PedRemoved handle=8711
    2026-09-29T19:06:51.242Z [INFO] [autopilot] event PedRemoved handle=9223
    2026-09-29T19:06:51.281Z [INFO] [autopilot] event PedAppeared handle=9224
    2026-09-29T19:06:51.393Z [INFO] [autopilot] event PedAppeared handle=11525
    2026-09-29T19:06:51.738Z [INFO] [autopilot] event PedAppeared handle=3336
    2026-09-29T19:06:51.810Z [INFO] camera_already_gone handle=4354 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:06:51.813Z [INFO] command source=file:cmd_20260929190651573.cmd line="cam 90 2.0 0.4" reply="camera at 90 deg, 2 m"
    2026-09-29T19:06:51.887Z [INFO] performance samples=1375 frame_p50_ms=18 frame_p95_ms=39 frame_p99_ms=63 frames_over_33ms=97 frames_over_50ms=29 gunplay_avg_ms=1.125 gunplay_max_ms=13.748 phase_samples=1375 phase_setup_avg_ms=0.717 phase_setup_max_ms=13.354 phase_camera_avg_ms=0.018 phase_camera_max_ms=0.630 phase_bullets_avg_ms=0.002 phase_bullets_max_ms=0.030 phase_weapon_avg_ms=0.017 phase_weapon_max_ms=0.164 phase_hud_avg_ms=0.371 phase_hud_max_ms=0.776
    2026-09-29T19:06:51.888Z [INFO] performance_scripts costs_ms(avg/max/count@thread) engine.frame=2.652/696.0/1375@7 total=3647 module.gunplay=1.131/13.8/1375@7 total=1555 tick.gunplay=1.129/13.7/1375@7 total=1553 gp.freeaim=0.598/13.2/1375@7 total=822 module.arsenal=0.687/17.2/807@7 total=555 tick.arsenal=0.687/17.2/807@7 total=554 ar.storage=0.453/4.0/807@7 total=366 engine.world=0.210/48.4/1375@7 total=288 module.atmosphere=0.191/4.1/1375@7 total=263 tick.atmosphere=0.190/4.1/1375@7 total=261 ar.safehouse_flags=0.155/2.3/807@7 total=125 gp.index_pad=0.063/0.2/1375@7 total=86 module.combat=0.045/2.2/1375@7 total=61 tick.combat=0.044/2.2/1375@7 total=60 ar.reconcile=0.052/14.0/807@7 total=42 module.holsters=0.083/14.6/431@7 total=36 tick.holsters=0.083/14.6/431@7 total=36 combat.sample=1.250/2.2/24@7 total=30 module.devtools=0.028/3.6/807@7 total=22 tick.devtools=0.027/3.6/807@7 total=22 gp.shoulder=0.014/0.2/1375@7 total=19 gp.weapon_id=0.013/1.0/1375@7 total=18 cam.handle=0.012/0.6/1375@7 total=17 ho.show=0.038/14.6/431@7 total=16 gp.player=0.011/0.1/1375@7 total=15 gp.cycle=0.008/0.0/1375@7 total=11 gp.state=0.006/0.0/1375@7 total=8 gp.shots=0.003/0.0/1375@7 total=4 module.probe=1.371/1.6/3@7 total=4 module.world=0.069/0.3/59@7 total=4 cam.aim_key=0.003/0.1/1375@7 total=4 cam.find_active=0.003/0.1/1375@7 total=4 gp.spread=0.002/0.0/1375@7 total=2 engine.scheduler=0.002/1.4/1375@7 total=2 ar.vehicle=0.003/0.0/807@7 total=2 ar.lvs=0.002/0.3/807@7 total=2 ar.discover=0.002/0.4/807@7 total=2 ho.carried=0.002/0.0/431@7 total=1 combat.dismember=0.001/0.0/1375@7 total=1 module.autopilot=0.000/0.0/1375@7 total=1 combat.blood=0.000/0.0/1375@7 total=0 gp.feel=0.000/0.0/1375@7 total=0 module.weapon-probe=0.000/0.0/1375@7 total=0 combat.pending=0.000/0.0/1375@7 total=0 gp.recoil=0.000/0.0/1375@7 total=0 cam.fov=0.001/0.0/113@7 total=0
    2026-09-29T19:06:51.888Z [INFO] engine_thread_probe ticks=1375 frame_advanced_during_tick=0 ticks_same_frame=0 ticks_next_frame=1374 ticks_after_skipped_frames=1
    2026-09-29T19:06:51.891Z [INFO] direct_native get_char_health direct=200 shdn=100 match=False direct_us=0.18 shdn_us=83.0
    2026-09-29T19:06:51.893Z [INFO] engine_status engine 1.1.0 sdk 1.1.0 frame=7582 core=on peds=10 vehicles=3 modules=10/10 coroutines=0 resources=5 raycast=on episode=GTAIV frame_ms=28.80 p95_ms=49.32 pressure=0.05 private_mb=2211 working_set_mb=1437 address_free_mb=1235 largest_free_block_mb=1199 managed_mb=14 physical_load=96% core_us=47.5
    2026-09-29T19:06:51.920Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:06:51.942Z [INFO] [autopilot] event PedRemoved handle=9987
    2026-09-29T19:06:51.969Z [INFO] density frame_ms=31.9 peds=0.87 cars=0.88
    2026-09-29T19:06:52.074Z [INFO] [autopilot] event PedAppeared handle=4616
    2026-09-29T19:06:52.366Z [INFO] [autopilot] event PedAppeared handle=4872
    2026-09-29T19:06:53.017Z [INFO] [autopilot] event PedRemoved handle=8198
    2026-09-29T19:06:53.388Z [INFO] [autopilot] event VehicleAppeared handle=11780
    2026-09-29T19:06:53.507Z [INFO] [autopilot] event PedAppeared handle=5125
    2026-09-29T19:06:53.736Z [INFO] [autopilot] event PedAppeared handle=3588
    2026-09-29T19:06:53.738Z [INFO] [autopilot] event PedAppeared handle=3847
    2026-09-29T19:06:53.739Z [INFO] [autopilot] event PedAppeared handle=4100
    2026-09-29T19:06:53.740Z [INFO] [autopilot] event PedAppeared handle=4358
    2026-09-29T19:06:54.257Z [INFO] [autopilot] event PedAppeared handle=5381
    2026-09-29T19:06:54.290Z [INFO] [autopilot] event PedAppeared handle=5637
    2026-09-29T19:06:54.660Z [INFO] [autopilot] event VehicleAppeared handle=3332
    2026-09-29T19:06:54.983Z [INFO] [autopilot] event PedAppeared handle=5895
    2026-09-29T19:06:54.984Z [INFO] [autopilot] event PedAppeared handle=6151
    2026-09-29T19:06:55.128Z [INFO] [autopilot] event PedAppeared handle=6663
    2026-09-29T19:06:55.129Z [INFO] [autopilot] event PedAppeared handle=6916
    2026-09-29T19:06:55.130Z [INFO] [autopilot] event PedAppeared handle=7174
    2026-09-29T19:06:55.181Z [INFO] [autopilot] event PedRemoved handle=4872
    2026-09-29T19:06:55.340Z [INFO] camera_already_gone handle=4610 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:06:55.346Z [INFO] command source=file:cmd_20260929190655209.cmd line="cam 270 2.0 0.4" reply="camera at 270 deg, 2 m"
    2026-09-29T19:06:55.362Z [INFO] [autopilot] event PedRemoved handle=4616
    2026-09-29T19:06:55.363Z [INFO] [autopilot] event PedRemoved handle=9224
    2026-09-29T19:06:55.576Z [INFO] [autopilot] event PedAppeared handle=4873
    2026-09-29T19:06:55.943Z [INFO] [autopilot] event PedAppeared handle=6408
    2026-09-29T19:06:56.830Z [INFO] [autopilot] event PedRemoved handle=5637
    2026-09-29T19:06:57.075Z [INFO] [autopilot] event PedRemoved handle=5381
    2026-09-29T19:06:57.337Z [INFO] [autopilot] event PedRemoved handle=5895
    2026-09-29T19:06:57.452Z [INFO] [autopilot] event PedAppeared handle=5638
    2026-09-29T19:06:57.674Z [INFO] [autopilot] event PedAppeared handle=5896
    2026-09-29T19:06:57.769Z [INFO] [autopilot] event PedAppeared handle=7435
    2026-09-29T19:06:57.796Z [INFO] [autopilot] event PedAppeared handle=1798
    2026-09-29T19:06:57.797Z [INFO] [autopilot] event PedAppeared handle=2055
    2026-09-29T19:06:57.798Z [INFO] [autopilot] event PedAppeared handle=2311
    2026-09-29T19:06:57.836Z [INFO] [autopilot] event PedAppeared handle=7690
    2026-09-29T19:06:57.994Z [INFO] [autopilot] event PedRemoved handle=6151
    2026-09-29T19:06:58.061Z [INFO] [autopilot] event PedRemoved handle=4873
    2026-09-29T19:06:58.289Z [INFO] [autopilot] event PedRemoved handle=7690
    2026-09-29T19:06:58.369Z [INFO] [autopilot] event PedRemoved handle=6408
    2026-09-29T19:06:58.458Z [INFO] [autopilot] event PedAppeared handle=5382
    2026-09-29T19:06:58.540Z [INFO] [autopilot] event PedAppeared handle=6152
    2026-09-29T19:06:58.603Z [INFO] [autopilot] event PedAppeared handle=6409
    2026-09-29T19:06:58.763Z [INFO] [autopilot] event PedAppeared handle=7691
    2026-09-29T19:06:59.030Z [INFO] camera_already_gone handle=4866 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:06:59.032Z [INFO] command source=file:cmd_20260929190658841.cmd line="cam 150 1.4 0.6" reply="camera at 150 deg, 1.4 m"
    2026-09-29T19:06:59.094Z [INFO] [autopilot] event PedRemoved handle=6409
    2026-09-29T19:06:59.904Z [INFO] [autopilot] event PedRemoved handle=5638
    2026-09-29T19:07:00.198Z [INFO] [autopilot] event PedRemoved handle=5896
    2026-09-29T19:07:00.406Z [INFO] [autopilot] event PedAppeared handle=5897
    2026-09-29T19:07:00.714Z [INFO] [autopilot] event PedRemoved handle=6152
    2026-09-29T19:07:00.808Z [INFO] [autopilot] event PedAppeared handle=6153
    2026-09-29T19:07:00.957Z [INFO] [autopilot] event PedRemoved handle=5382
    2026-09-29T19:07:01.225Z [INFO] [autopilot] event PedAppeared handle=5639
    2026-09-29T19:07:01.299Z [INFO] [autopilot] event PedAppeared handle=6410
    2026-09-29T19:07:01.348Z [INFO] [autopilot] event PedRemoved handle=7691
    2026-09-29T19:07:01.927Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T19:07:01.998Z [INFO] [autopilot] event PedRemoved handle=2055
    2026-09-29T19:07:01.999Z [INFO] [autopilot] event PedRemoved handle=2311
    2026-09-29T19:07:02.000Z [INFO] [autopilot] event PedRemoved handle=1798
    2026-09-29T19:07:02.620Z [INFO] camera_already_gone handle=5122 Invalid call to an object that doesn't exist anymore!
    2026-09-29T19:07:02.621Z [INFO] command source=file:cmd_20260929190702480.cmd line="cam off" reply="camera off"
    2026-09-29T19:07:02.782Z [INFO] [autopilot] event PedRemoved handle=5897
    2026-09-29T19:07:03.061Z [INFO] [autopilot] event PedAppeared handle=7692
    2026-09-29T19:07:03.120Z [INFO] command source=file:cmd_20260929190702874.cmd line="hud on" reply="hud on"
    2026-09-29T19:07:03.233Z [INFO] [autopilot] event PedAppeared handle=9225

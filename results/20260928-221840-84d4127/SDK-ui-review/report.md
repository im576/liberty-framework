# Scenario ui-review

- Result: PASS
- Steps: 21, failed: 0
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
- hud off => hud off
- menu-test => menu open
- wait 1500 ms
- shot sdk_list_menu -> sdk_list_menu.jpg
- key Down 80 ms
- wait 300 ms
- key Down 80 ms
- wait 300 ms
- shot sdk_list_menu_moved -> sdk_list_menu_moved.jpg
- menu-close => closed 1
- radial-test => radial open
- wait 1500 ms
- shot sdk_radial_menu -> sdk_radial_menu.jpg
- menu-close => closed 1
- hud on => hud on

## Errors

## Log
    ﻿2026-09-26T07:33:48.959Z [INFO] atmosphere_started
    2026-09-29T05:48:56.263Z [INFO] command source=file:cmd_20260929054856160.cmd line="god on" reply="invincible True"
    2026-09-29T05:48:56.776Z [INFO] command source=file:cmd_20260929054856577.cmd line="wanted 0" reply="wanted 0"
    2026-09-29T05:48:57.306Z [INFO] teleport_start id=east_park target=-64.8,663.4,15 snap=pavement
    2026-09-29T05:48:57.307Z [INFO] command source=file:cmd_20260929054856972.cmd line="goto east_park" reply="Teleporting to Algonquin East Park Street"
    2026-09-29T05:48:58.244Z [INFO] teleport_done id=east_park final=-64.4,674.8,15.5
    2026-09-29T05:48:58.462Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:02.670Z [INFO] command source=file:cmd_20260929054902651.cmd line="time 13 0" reply="time 13:00"
    2026-09-29T05:49:03.022Z [INFO] weather_block from=CLOUDY to=DRIZZLE hours=5 at=13h
    2026-09-29T05:49:03.174Z [INFO] command source=file:cmd_20260929054903043.cmd line="weather 1" reply="weather 1"
    2026-09-29T05:49:03.701Z [INFO] command source=file:cmd_20260929054903438.cmd line="hud off" reply="hud off"
    2026-09-29T05:49:03.960Z [INFO] command source=file:cmd_20260929054903828.cmd line="menu-test" reply="menu open"
    2026-09-29T05:49:08.702Z [INFO] T-001 heartbeat probe_label=default
    2026-09-29T05:49:11.935Z [INFO] command source=file:cmd_20260929054911837.cmd line="menu-close" reply="closed 1"
    2026-09-29T05:49:12.442Z [INFO] command source=file:cmd_20260929054912227.cmd line="radial-test" reply="radial open"
    2026-09-29T05:49:16.539Z [INFO] command source=file:cmd_20260929054916364.cmd line="menu-close" reply="closed 1"
    2026-09-29T05:49:16.793Z [INFO] command source=file:cmd_20260929054916751.cmd line="hud on" reply="hud on"

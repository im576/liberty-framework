---
name: liberty-persistence
description: Specify or implement durable weapon, trunk and vehicle state for Liberty mods, including interrupted operations and reconciliation with actual GTA saves.
---

# Preserve ownership and transfers

Read the mod's current product rules and existing state interfaces. Keep its
ownership/economy policies in the mod; reusable serialization/recovery primitives
belong in Liberty Framework. This workflow does not assert that any current mod
already has transactional persistence or durable vehicle identity.

Distinguish runtime handles from durable identities. Define identity generation,
save/profile binding, object recreation, duplicate reconciliation and tombstones.
Inventory, wheel, holsters and trunks consume the same weapon/ammunition records;
garage, registration, insurance and trunks consume the same vehicle identity.

Model transfers and purchases as operations with durable IDs, explicit states and
retry/recovery outcomes. Record where money, spawned objects and external state
change. Atomic JSON replacement cannot atomically transact those game changes.
Specify what happens when interrupted before and after each mutation, including
refund/recovery and idempotent replay, without granting duplicate items or charges.

Use focused fault injection for interruption boundaries and malformed/versioned
state. Then, when a game run is authorized, test actual GTA save/reload, older-save
reconciliation, destruction/death and unload. External state serialization alone
is not save compatibility. Use separate experimental saves and reversible receipts.

Report exact source/save identities, recovered operations, remaining limitations
and measured restoration work. Restore nearby entities within a bounded budget;
do not eagerly respawn every owned object on every tick. Retain evidence of failed
reconciliation instead of silently replacing it with a successful new save.

// LibertyCore: per-frame world snapshot and change events (ADR-0006). Runs inside the engine host's tick, while the
// game thread is parked; lc_frame measures that with the engine frame counter and reports violations.
#include <windows.h>
#include <algorithm>
#include <cmath>
#include <cstring>
#include <unordered_map>
#include "liberty_core.h"
#include "natives.h"
#include "rage_pool.h"
#include "safe_call.h"
#include "damage_hook.h"
#include "hooks.h"
#include "ray_walk.h"

namespace
{
    struct PedMemory { int32_t health; uint32_t flags; uint32_t seen_frame; };
    struct VehicleMemory { int32_t health; float engine; uint32_t seen_frame; bool destroyed; };

    struct State
    {
        bool initialised = false;
        lc::Natives natives;
        lc_address_book book{};
        lc::RagePool* peds_pool = nullptr;
        lc::RagePool* vehicles_pool = nullptr;
        lc::RagePool* objects_pool = nullptr;
        lc_snapshot snapshot{};
        std::unordered_map<int32_t, PedMemory> peds;
        std::unordered_map<int32_t, VehicleMemory> vehicles;
        bool have_previous_player = false;
        lc_player previous_player{};
        bool have_previous_world = false;
        lc_world previous_world{};
        LARGE_INTEGER frequency{};
        uint32_t pool_stats_frame = 0;
        uint32_t bullet_keys[2][64] = {};
        int bullet_key_count[2] = {};
        int bullet_side = 0;
    };

    State g;

    void push(int32_t type, int32_t a = 0, int32_t b = 0, int32_t c = 0, int32_t d = 0, int32_t e = 0)
    {
        lc_snapshot& s = g.snapshot;
        if (s.event_count >= LC_MAX_EVENTS) { s.events_dropped++; return; }
        s.events[s.event_count++] = lc_event{ type, a, b, c, d, e };
    }

    float as_float(int32_t bits) { float value; std::memcpy(&value, &bits, 4); return value; }

    bool ready(int id) { return g.natives.ready(id); }

    uint32_t engine_frame()
    {
        return g.book.frame_counter != 0 ? *reinterpret_cast<volatile uint32_t*>(g.book.frame_counter) : 0;
    }

    bool read_world(lc_world& world)
    {
        if (!ready(LC_N_IS_PAUSE_MENU_ACTIVE) || !ready(LC_N_IS_SCREEN_FADED_OUT)) { return false; }
        world = lc_world{};
        if (g.natives.call0(LC_N_IS_PAUSE_MENU_ACTIVE) != 0) { world.flags |= LC_WORLD_PAUSED; }
        if (g.natives.call0(LC_N_IS_SCREEN_FADED_OUT) != 0) { world.flags |= LC_WORLD_FADED_OUT; }
        int32_t out = 0;
        if (ready(LC_N_GET_GAME_TIMER)) { g.natives.call(LC_N_GET_GAME_TIMER, nullptr, &out); world.game_timer_ms = static_cast<uint32_t>(out); }
        if (ready(LC_N_GET_HOURS_OF_DAY)) { world.hours = g.natives.call0(LC_N_GET_HOURS_OF_DAY); }
        if (ready(LC_N_GET_MINUTES_OF_DAY)) { world.minutes = g.natives.call0(LC_N_GET_MINUTES_OF_DAY); }
        world.weather = -1;
        if (ready(LC_N_GET_CURRENT_WEATHER)) { g.natives.call(LC_N_GET_CURRENT_WEATHER, nullptr, &out); world.weather = out; }
        return true;
    }

    bool ped_exists(int32_t handle) { return handle > 0 && g.natives.call1(LC_N_DOES_CHAR_EXIST, handle) != 0; }

    void read_ped(int32_t handle, lc_ped& ped)
    {
        ped = lc_ped{};
        ped.handle = handle;
        int32_t outs[3] = {};
        int32_t args[1] = { handle };
        g.natives.call(LC_N_GET_CHAR_COORDINATES, args, outs);
        ped.x = as_float(outs[0]); ped.y = as_float(outs[1]); ped.z = as_float(outs[2]);
        int32_t value = 0;
        g.natives.out1(LC_N_GET_CHAR_HEADING, handle, value); ped.heading = as_float(value);
        g.natives.out1(LC_N_GET_CHAR_HEALTH, handle, value); ped.health = value;
        if (ready(LC_N_GET_CHAR_ARMOUR)) { g.natives.out1(LC_N_GET_CHAR_ARMOUR, handle, value); ped.armour = value; }
        g.natives.out1(LC_N_GET_CHAR_MODEL, handle, value); ped.model = static_cast<uint32_t>(value);
        if (g.natives.call1(LC_N_IS_CHAR_DEAD, handle) != 0) { ped.flags |= LC_PED_DEAD; }
        if (g.natives.call1(LC_N_IS_CHAR_IN_ANY_CAR, handle) != 0)
        {
            ped.flags |= LC_PED_IN_VEHICLE;
            if (ready(LC_N_GET_CAR_CHAR_IS_USING)) { g.natives.out1(LC_N_GET_CAR_CHAR_IS_USING, handle, value); ped.vehicle = value; }
        }
    }

    bool ped_natives_ready()
    {
        const int needed[] = { LC_N_DOES_CHAR_EXIST, LC_N_IS_CHAR_DEAD, LC_N_GET_CHAR_HEALTH, LC_N_GET_CHAR_COORDINATES,
            LC_N_GET_CHAR_HEADING, LC_N_GET_CHAR_MODEL, LC_N_IS_CHAR_IN_ANY_CAR };
        for (int id : needed) { if (!ready(id)) { return false; } }
        return true;
    }

    bool vehicle_natives_ready()
    {
        const int needed[] = { LC_N_DOES_VEHICLE_EXIST, LC_N_GET_CAR_COORDINATES, LC_N_GET_CAR_HEADING, LC_N_GET_CAR_SPEED,
            LC_N_GET_CAR_HEALTH, LC_N_GET_ENGINE_HEALTH, LC_N_GET_CAR_MODEL };
        for (int id : needed) { if (!ready(id)) { return false; } }
        return true;
    }

    bool read_player(int32_t ped_handle, lc_player& player)
    {
        if (!ped_natives_ready() || !ready(LC_N_GET_PLAYER_ID) || !ped_exists(ped_handle)) { return false; }
        lc_ped ped;
        read_ped(ped_handle, ped);
        player = lc_player{};
        player.index = g.natives.call0(LC_N_GET_PLAYER_ID);
        player.ped = ped_handle;
        player.x = ped.x; player.y = ped.y; player.z = ped.z; player.heading = ped.heading;
        player.health = ped.health; player.armour = ped.armour; player.vehicle = ped.vehicle;
        if (ped.flags & LC_PED_DEAD) { player.flags |= LC_PLAYER_DEAD; }
        if (ped.flags & LC_PED_IN_VEHICLE) { player.flags |= LC_PLAYER_IN_VEHICLE; }
        if (ready(LC_N_IS_PLAYER_PLAYING) && g.natives.call1(LC_N_IS_PLAYER_PLAYING, player.index) != 0) { player.flags |= LC_PLAYER_PLAYING; }
        if (ready(LC_N_IS_PLAYER_CONTROL_ON) && g.natives.call1(LC_N_IS_PLAYER_CONTROL_ON, player.index) != 0) { player.flags |= LC_PLAYER_CONTROL; }
        player.weapon = -1;
        player.ammo_in_clip = -1;
        if (ready(LC_N_GET_CURRENT_CHAR_WEAPON))
        {
            int32_t weapon = 0;
            g.natives.out1(LC_N_GET_CURRENT_CHAR_WEAPON, ped_handle, weapon);
            player.weapon = weapon;
            if (ready(LC_N_GET_AMMO_IN_CLIP))
            {
                int32_t args[2] = { ped_handle, weapon };
                int32_t clip = 0;
                g.natives.call(LC_N_GET_AMMO_IN_CLIP, args, &clip);
                player.ammo_in_clip = clip;
            }
        }
        return true;
    }

    // Reloading is inferred: the clip emptying with the same weapon starts a reload (the game reloads automatically),
    // and the clip refilling ends it. A manual reload with rounds left reports only its end.
    void player_events(lc_player& now)
    {
        if (!g.have_previous_player || g.previous_player.ped != now.ped) { g.previous_player = now; g.have_previous_player = true; return; }
        const lc_player& was = g.previous_player;
        bool reloading = (was.flags & LC_PLAYER_RELOADING) != 0 && now.weapon == was.weapon;
        if (now.weapon != was.weapon) { push(LC_EV_PLAYER_WEAPON, was.weapon, now.weapon); reloading = false; }
        else if (now.ammo_in_clip >= 0 && was.ammo_in_clip >= 0)
        {
            if (now.ammo_in_clip < was.ammo_in_clip)
            {
                push(LC_EV_PLAYER_SHOT, now.weapon, was.ammo_in_clip, now.ammo_in_clip);
                if (now.ammo_in_clip == 0 && !reloading) { push(LC_EV_PLAYER_RELOAD_STARTED, now.weapon, was.ammo_in_clip); reloading = true; }
            }
            else if (now.ammo_in_clip > was.ammo_in_clip)
            {
                push(LC_EV_PLAYER_RELOADED, now.weapon, was.ammo_in_clip, now.ammo_in_clip);
                reloading = false;
            }
        }
        if (reloading) { now.flags |= LC_PLAYER_RELOADING; }
        bool in_now = (now.flags & LC_PLAYER_IN_VEHICLE) != 0, in_was = (was.flags & LC_PLAYER_IN_VEHICLE) != 0;
        if (in_now && !in_was) { push(LC_EV_PLAYER_ENTERED_VEHICLE, now.vehicle); }
        if (!in_now && in_was) { push(LC_EV_PLAYER_EXITED_VEHICLE, was.vehicle); }
        if ((now.flags & LC_PLAYER_DEAD) && !(was.flags & LC_PLAYER_DEAD)) { push(LC_EV_PLAYER_DIED); }
        if (now.health < was.health || now.armour < was.armour) { push(LC_EV_PLAYER_DAMAGED, 0, was.health, now.health, was.armour, now.armour); }
        g.previous_player = now;
    }

    void world_events(const lc_world& now)
    {
        if (!g.have_previous_world) { g.previous_world = now; g.have_previous_world = true; return; }
        const lc_world& was = g.previous_world;
        if (now.weather != was.weather) { push(LC_EV_WEATHER_CHANGED, was.weather, now.weather); }
        if ((now.flags ^ was.flags) & LC_WORLD_PAUSED) { push(LC_EV_PAUSE_CHANGED, (now.flags & LC_WORLD_PAUSED) ? 1 : 0); }
        if ((now.flags ^ was.flags) & LC_WORLD_FADED_OUT) { push(LC_EV_FADE_CHANGED, (now.flags & LC_WORLD_FADED_OUT) ? 1 : 0); }
        g.previous_world = now;
    }

    struct Candidate { int32_t handle; float distance; uint32_t entity; };
    Candidate g_candidates[4096];

    // CEntity matrix pointer (+0x20, from the GET_CAR_COORDINATES / GET_CAR_HEADING workers). Some pooled vehicles have none;
    // GET_CAR_HEADING falls back for them but GET_CAR_COORDINATES dereferences it unchecked (2026-09-25 crash), so a vehicle is
    // read only when its matrix exists.
    const uint32_t kEntityMatrixOffset = 0x20;

    // Raw game-memory reads go through safe_invoke too; a fault is counted and the read reports failure.
    struct Read32 { uint32_t address; uint32_t value; };
    void read32_thunk(void* p) { Read32* r = static_cast<Read32*>(p); r->value = *reinterpret_cast<volatile uint32_t*>(r->address); }
    uint32_t g_raw_fault_count = 0;

    bool safe_read32(uint32_t address, uint32_t& value)
    {
        Read32 request{ address, 0 };
        if (!lc::safe_invoke(reinterpret_cast<uint32_t>(&read32_thunk), &request)) { g_raw_fault_count = lc::last_fault().count; g.natives.note_raw_fault(g_raw_fault_count); return false; }
        value = request.value;
        return true;
    }

    bool has_matrix(lc::RagePool* pool, int index)
    {
        uint32_t entity = pool->object_at(index), matrix = 0;
        return entity != 0 && safe_read32(entity + kEntityMatrixOffset, matrix) && matrix != 0;
    }

    // Live pool entries within the radius, nearest first when over capacity. need_matrix skips entities without one.
    int collect(lc::RagePool* pool, int native_exists, int native_coords, float radius, int32_t always, bool have_player, int capacity, bool need_matrix)
    {
        const lc_snapshot& s = g.snapshot;
        int count = 0;
        const float radius2 = radius * radius;
        const int size = pool->size();
        for (int index = 0; index < size && count < 4096; index++)
        {
            int32_t handle = pool->handle_at(index);
            if (handle == 0 || g.natives.call1(native_exists, handle) == 0) { continue; }
            if (need_matrix && !has_matrix(pool, index)) { continue; }
            int32_t outs[3] = {};
            int32_t args[1] = { handle };
            g.natives.call(native_coords, args, outs);
            float dx = as_float(outs[0]) - s.player.x, dy = as_float(outs[1]) - s.player.y, dz = as_float(outs[2]) - s.player.z;
            float d2 = dx * dx + dy * dy + dz * dz;
            if (have_player && d2 > radius2 && handle != always) { continue; }
            g_candidates[count].handle = handle;
            g_candidates[count].entity = pool->object_at(index);
            g_candidates[count].distance = std::sqrt(d2);
            count++;
        }
        if (count > capacity)
        {
            std::nth_element(g_candidates, g_candidates + capacity, g_candidates + count,
                [](const Candidate& a, const Candidate& b) { return a.distance < b.distance; });
            count = capacity;
        }
        return count;
    }

    void read_peds(const lc_frame_input& input, bool have_player)
    {
        lc_snapshot& s = g.snapshot;
        s.ped_count = 0;
        if (g.peds_pool == nullptr || !g.peds_pool->valid() || !ped_natives_ready()) { return; }
        int count = collect(g.peds_pool, LC_N_DOES_CHAR_EXIST, LC_N_GET_CHAR_COORDINATES, input.ped_radius, input.player_ped, have_player, LC_MAX_PEDS, false);
        const uint32_t frame = s.frame;
        for (int i = 0; i < count; i++)
        {
            float distance = g_candidates[i].distance;
            lc_ped& ped = s.peds[s.ped_count++];
            read_ped(g_candidates[i].handle, ped);
            ped.distance = distance;
            if (ped.handle == input.player_ped) { ped.flags |= LC_PED_PLAYER; }
            auto found = g.peds.find(ped.handle);
            if (found == g.peds.end())
            {
                ped.flags |= LC_PED_NEW;
                push(LC_EV_PED_APPEARED, ped.handle);
                g.peds.emplace(ped.handle, PedMemory{ ped.health, ped.flags, frame });
                continue;
            }
            PedMemory& was = found->second;
            if (ped.health < was.health && !(ped.flags & LC_PED_PLAYER))
            {
                int32_t bone = -1;
                if (ready(LC_N_GET_CHAR_LAST_DAMAGE_BONE)) { g.natives.out1(LC_N_GET_CHAR_LAST_DAMAGE_BONE, ped.handle, bone); }
                int32_t by_player = 0;
                if (have_player && ready(LC_N_HAS_CHAR_BEEN_DAMAGED_BY_CHAR))
                {
                    int32_t args[3] = { ped.handle, input.player_ped, 0 };
                    by_player = g.natives.call(LC_N_HAS_CHAR_BEEN_DAMAGED_BY_CHAR, args, nullptr) != 0 ? 1 : 0;
                }
                push(LC_EV_PED_DAMAGED, ped.handle, was.health, ped.health, bone, by_player);
                if ((ped.flags & LC_PED_DEAD) && !(was.flags & LC_PED_DEAD)) { push(LC_EV_PED_DIED, ped.handle, 0, 0, bone, by_player); }
            }
            else if ((ped.flags & LC_PED_DEAD) && !(was.flags & LC_PED_DEAD) && !(ped.flags & LC_PED_PLAYER))
            {
                push(LC_EV_PED_DIED, ped.handle);
            }
            was.health = ped.health;
            was.flags = ped.flags;
            was.seen_frame = frame;
        }
        for (auto it = g.peds.begin(); it != g.peds.end();)
        {
            if (it->second.seen_frame != frame) { push(LC_EV_PED_REMOVED, it->first); it = g.peds.erase(it); }
            else { ++it; }
        }
    }

    // CVehicle driver pointer (+0xF50, from the GET_DRIVER_OF_CAR worker), mapped to a handle through the ped pool by
    // plain reads. The native itself is not called per frame: on some pooled vehicles it faulted half-way through turning
    // the pointer into a script handle, and the game crashed a second later in its ped update (2026-09-25).
    const uint32_t kVehicleDriverOffset = 0xF50;

    int32_t driver_of(uint32_t entity)
    {
        uint32_t driver = 0;
        if (entity == 0 || g.peds_pool == nullptr || !safe_read32(entity + kVehicleDriverOffset, driver) || driver == 0) { return 0; }
        return g.peds_pool->handle_of(driver);
    }

    void read_vehicle(int32_t handle, uint32_t entity, lc_vehicle& v)
    {
        v = lc_vehicle{};
        v.handle = handle;
        int32_t outs[3] = {};
        int32_t args[1] = { handle };
        g.natives.call(LC_N_GET_CAR_COORDINATES, args, outs);
        v.x = as_float(outs[0]); v.y = as_float(outs[1]); v.z = as_float(outs[2]);
        int32_t value = 0;
        g.natives.out1(LC_N_GET_CAR_HEADING, handle, value); v.heading = as_float(value);
        g.natives.out1(LC_N_GET_CAR_SPEED, handle, value); v.speed = as_float(value);
        g.natives.out1(LC_N_GET_CAR_HEALTH, handle, value); v.health = value;
        v.engine_health = as_float(g.natives.call1(LC_N_GET_ENGINE_HEALTH, handle));
        g.natives.out1(LC_N_GET_CAR_MODEL, handle, value); v.model = static_cast<uint32_t>(value);
        v.driver = driver_of(entity);
    }

    void read_vehicles(const lc_frame_input& input, bool have_player)
    {
        lc_snapshot& s = g.snapshot;
        s.vehicle_count = 0;
        if (g.vehicles_pool == nullptr || !g.vehicles_pool->valid() || !vehicle_natives_ready()) { return; }
        int32_t player_vehicle = have_player ? s.player.vehicle : 0;
        int count = collect(g.vehicles_pool, LC_N_DOES_VEHICLE_EXIST, LC_N_GET_CAR_COORDINATES, input.vehicle_radius, player_vehicle, have_player, LC_MAX_VEHICLES, true);
        const uint32_t frame = s.frame;
        for (int i = 0; i < count; i++)
        {
            float distance = g_candidates[i].distance;
            lc_vehicle& v = s.vehicles[s.vehicle_count++];
            read_vehicle(g_candidates[i].handle, g_candidates[i].entity, v);
            v.distance = distance;
            if (v.handle == player_vehicle && player_vehicle != 0) { v.flags |= LC_VEH_PLAYER; }
            auto found = g.vehicles.find(v.handle);
            if (found == g.vehicles.end())
            {
                v.flags |= LC_VEH_NEW;
                push(LC_EV_VEHICLE_APPEARED, v.handle);
                g.vehicles.emplace(v.handle, VehicleMemory{ v.health, v.engine_health, frame, v.engine_health < 0 });
                continue;
            }
            VehicleMemory& was = found->second;
            if (v.health < was.health || v.engine_health < was.engine - 0.5f)
            {
                push(LC_EV_VEHICLE_DAMAGED, v.handle, was.health, v.health, static_cast<int32_t>(was.engine), static_cast<int32_t>(v.engine_health));
            }
            if (!was.destroyed && v.engine_health < 0) { push(LC_EV_VEHICLE_DESTROYED, v.handle); was.destroyed = true; }
            was.health = v.health;
            was.engine = v.engine_health;
            was.seen_frame = frame;
        }
        for (auto it = g.vehicles.begin(); it != g.vehicles.end();)
        {
            if (it->second.seen_frame != frame) { push(LC_EV_VEHICLE_REMOVED, it->first); it = g.vehicles.erase(it); }
            else { ++it; }
        }
    }

    uint32_t fnv(const uint8_t* data, int length, uint32_t seed)
    {
        uint32_t hash = 2166136261u ^ seed;
        for (int i = 0; i < length; i++) { hash = (hash ^ data[i]) * 16777619u; }
        return hash;
    }

    struct BulletCopy { uint32_t count_global, array_global; int32_t stride, maximum; int32_t count; uint8_t data[64 * 0x40]; };
    BulletCopy g_bullets;

    // Copies the game's bullet list (count and entries) into g_bullets; runs under safe_invoke.
    void copy_bullets_thunk(void* p)
    {
        BulletCopy* c = static_cast<BulletCopy*>(p);
        c->count = 0;
        int32_t count = *reinterpret_cast<volatile int32_t*>(c->count_global);
        if (count <= 0 || count > c->maximum || count > 64 || c->stride > 0x40) { return; }
        uint32_t array = *reinterpret_cast<volatile uint32_t*>(c->array_global);
        if (array == 0) { return; }
        std::memcpy(c->data, reinterpret_cast<const void*>(array), static_cast<size_t>(count * c->stride));
        c->count = count;
    }

    // Bullet traces added since the previous frame. An entry can stay in the list for more than one frame, so each
    // frame's entries are remembered by content and only new ones are reported (same rule as GameApi/BulletLog).
    void read_bullets()
    {
        lc_snapshot& s = g.snapshot;
        s.bullet_count = 0;
        const lc_address_book& b = g.book;
        if (b.bullet_count_global == 0 || b.bullet_array_global == 0 || b.bullet_stride < 0x20 || b.bullet_stride > 0x40 || b.bullet_maximum <= 0 ||
            b.bullet_owner_offset < 0 || b.bullet_owner_offset + 4 > b.bullet_stride || g.peds_pool == nullptr) { return; }
        int next = 1 - g.bullet_side;
        g.bullet_key_count[next] = 0;
        g_bullets.count_global = b.bullet_count_global; g_bullets.array_global = b.bullet_array_global;
        g_bullets.stride = b.bullet_stride; g_bullets.maximum = b.bullet_maximum;
        if (!lc::safe_invoke(reinterpret_cast<uint32_t>(&copy_bullets_thunk), &g_bullets)) { g_raw_fault_count = lc::last_fault().count; g.natives.note_raw_fault(g_raw_fault_count); g_bullets.count = 0; }
        int count = g_bullets.count;
        if (count > 0)
        {
            {
                for (int i = 0; i < count; i++)
                {
                    const uint8_t* entry = g_bullets.data + i * b.bullet_stride;
                    uint32_t owner;
                    std::memcpy(&owner, entry + b.bullet_owner_offset, 4);
                    uint32_t key = fnv(entry, 0x1C, owner);
                    g.bullet_keys[next][g.bullet_key_count[next]++] = key;
                    bool seen = false;
                    for (int k = 0; k < g.bullet_key_count[g.bullet_side] && !seen; k++) { seen = g.bullet_keys[g.bullet_side][k] == key; }
                    if (seen || s.bullet_count >= LC_MAX_BULLETS) { continue; }
                    lc_bullet& bullet = s.bullets[s.bullet_count++];
                    float v[7];
                    std::memcpy(&v[0], entry, 12);
                    std::memcpy(&v[3], entry + 0x10, 12);
                    bullet.from_x = v[0]; bullet.from_y = v[1]; bullet.from_z = v[2];
                    bullet.to_x = v[3]; bullet.to_y = v[4]; bullet.to_z = v[5];
                    bullet.shooter = g.peds_pool->handle_of(owner);
                    bullet.weapon = -1;
                    if (bullet.shooter != 0 && ready(LC_N_GET_CURRENT_CHAR_WEAPON) && ready(LC_N_DOES_CHAR_EXIST) && ped_exists(bullet.shooter))
                    {
                        int32_t weapon = -1;
                        g.natives.out1(LC_N_GET_CURRENT_CHAR_WEAPON, bullet.shooter, weapon);
                        bullet.weapon = weapon;
                    }
                }
            }
        }
        g.bullet_side = next;
    }
    // Exact damages recorded by the damage-response observer since the last frame (ADR-0007): pointers become handles
    // through the pools; kills are marked with IS_CHAR_DEAD. Records beyond capacity wait for the next frame.
    lc::damage::Record g_damage_records[LC_MAX_DAMAGES];

    void read_damages()
    {
        lc_snapshot& s = g.snapshot;
        s.damage_count = 0;
        uint32_t dropped = 0;
        int n = lc::damage::drain(g_damage_records, LC_MAX_DAMAGES, dropped);
        for (int i = 0; i < n; i++)
        {
            const lc::damage::Record& r = g_damage_records[i];
            lc_damage& d = s.damages[s.damage_count++];
            d = lc_damage{};
            d.victim = g.peds_pool != nullptr ? g.peds_pool->handle_of(r.victim) : 0;
            if (r.damager != 0 && g.peds_pool != nullptr) { d.attacker = g.peds_pool->handle_of(r.damager); if (d.attacker != 0) { d.attacker_kind = 1; } }
            if (d.attacker == 0 && r.damager != 0 && g.vehicles_pool != nullptr) { d.attacker = g.vehicles_pool->handle_of(r.damager); if (d.attacker != 0) { d.attacker_kind = 2; } }
            d.weapon = r.weapon;
            d.component = r.component;
            d.bone = r.bone;
            d.amount = r.amount;
            d.health_lost = r.health_lost;
            d.armour_lost = r.armour_lost;
            if (d.victim != 0 && ready(LC_N_DOES_CHAR_EXIST) && ready(LC_N_IS_CHAR_DEAD) && ped_exists(d.victim) &&
                g.natives.call1(LC_N_IS_CHAR_DEAD, d.victim) != 0)
            {
                d.flags |= LC_DAMAGE_KILLED;
            }
        }
    }
    // Pool occupancy is cheap but not free (a flag byte per slot); refreshed every 30 frames.
    void read_pools()
    {
        lc_pools& p = g.snapshot.pools;
        if (g.pool_stats_frame != 0 && g.snapshot.frame - g.pool_stats_frame < 30) { return; }
        g.pool_stats_frame = g.snapshot.frame;
        p.peds_size = g.peds_pool ? g.peds_pool->size() : 0; p.peds_used = g.peds_pool ? g.peds_pool->used() : 0;
        p.vehicles_size = g.vehicles_pool ? g.vehicles_pool->size() : 0; p.vehicles_used = g.vehicles_pool ? g.vehicles_pool->used() : 0;
        p.objects_size = g.objects_pool ? g.objects_pool->size() : 0; p.objects_used = g.objects_pool ? g.objects_pool->used() : 0;
    }
}

LC_API uint32_t lc_abi_version(void) { return LC_ABI_VERSION; }

LC_API uint32_t lc_native_hash(int32_t id) { return id >= 0 && id < LC_N_COUNT ? lc::native_info(id).hash : 0; }

LC_API int32_t lc_init(const lc_address_book* book, const uint32_t* handlers, char* error, int32_t error_size)
{
    auto fail = [&](const char* text) { if (error != nullptr && error_size > 0) { strncpy(error, text, error_size - 1); error[error_size - 1] = 0; } return 0; };
    if (book == nullptr || book->size != sizeof(lc_address_book)) { return fail("address book size mismatch"); }
    if (handlers == nullptr) { return fail("no native handlers"); }
    g.book = *book;
    lc::install_safe_calls();
    for (int id = 0; id < LC_N_COUNT; id++) { g.natives.set_handler(id, handlers[id]); }
    delete g.peds_pool; delete g.vehicles_pool; delete g.objects_pool;
    g.peds_pool = book->ped_pool_global != 0 ? new lc::RagePool(book->ped_pool_global, 0x4000) : nullptr;
    g.vehicles_pool = book->vehicle_pool_global != 0 ? new lc::RagePool(book->vehicle_pool_global, 0x8000) : nullptr;
    g.objects_pool = book->object_pool_global != 0 ? new lc::RagePool(book->object_pool_global, 0x4000) : nullptr;
    g.peds.clear(); g.peds.reserve(512);
    g.vehicles.clear(); g.vehicles.reserve(256);
    g.have_previous_player = false;
    g.have_previous_world = false;
    g.pool_stats_frame = 0;
    g.bullet_key_count[0] = g.bullet_key_count[1] = 0;
    std::memset(&g.snapshot, 0, sizeof(g.snapshot));
    g.snapshot.size = sizeof(lc_snapshot);
    g.snapshot.abi_version = LC_ABI_VERSION;
    QueryPerformanceFrequency(&g.frequency);
    g.initialised = true;
    return 1;
}

LC_API void lc_set_native_verified(int32_t id, int32_t verified)
{
    if (id >= 0 && id < LC_N_COUNT) { g.natives.set_verified(id, verified != 0); }
}

LC_API int32_t lc_call_native(int32_t id, int32_t argc, const int32_t* args, int32_t* outs)
{
    if (!g.initialised || !g.natives.available(id) || argc != lc::native_info(id).in_args) { return INT32_MIN; }
    return g.natives.call(id, args, outs);
}

LC_API const lc_snapshot* lc_frame(const lc_frame_input* input)
{
    if (!g.initialised || input == nullptr || input->size != sizeof(lc_frame_input)) { return nullptr; }
    LARGE_INTEGER start;
    QueryPerformanceCounter(&start);
    lc_snapshot& s = g.snapshot;
    s.frame++;
    s.event_count = 0;
    s.events_dropped = 0;
    s.valid = 0;
    s.engine_frame = engine_frame();

    if ((input->enabled & LC_VALID_WORLD) && read_world(s.world)) { s.valid |= LC_VALID_WORLD; world_events(s.world); }
    bool have_player = false;
    if ((input->enabled & LC_VALID_PLAYER) && read_player(input->player_ped, s.player))
    {
        s.valid |= LC_VALID_PLAYER;
        have_player = true;
        player_events(s.player);
    }
    if (input->enabled & LC_VALID_PEDS)
    {
        read_peds(*input, have_player);
        if (g.peds_pool != nullptr && g.peds_pool->valid() && ped_natives_ready()) { s.valid |= LC_VALID_PEDS; }
    }
    if (input->enabled & LC_VALID_VEHICLES)
    {
        read_vehicles(*input, have_player);
        if (g.vehicles_pool != nullptr && g.vehicles_pool->valid() && vehicle_natives_ready()) { s.valid |= LC_VALID_VEHICLES; }
    }
    if ((input->enabled & LC_VALID_BULLETS) && g.peds_pool != nullptr && g.peds_pool->valid() && g.book.bullet_array_global != 0)
    {
        read_bullets();
        s.valid |= LC_VALID_BULLETS;
    }
    else { s.bullet_count = 0; }
    if ((input->enabled & LC_VALID_DAMAGE) && lc::damage::active()) { read_damages(); s.valid |= LC_VALID_DAMAGE; }
    else { s.damage_count = 0; }
    read_pools();

    if (g.book.frame_counter != 0 && engine_frame() != s.engine_frame) { s.parked_violations++; }
    LARGE_INTEGER end;
    QueryPerformanceCounter(&end);
    s.core_microseconds = static_cast<float>((end.QuadPart - start.QuadPart) * 1000000.0 / g.frequency.QuadPart);
    return &s;
}

LC_API void lc_shutdown(void)
{
    g.initialised = false;
    // Hooks never outlive the engine that asked for them (script unload, or the core switched off by a safety check).
    lc::hooks::remove_all();
    delete g.peds_pool; delete g.vehicles_pool; delete g.objects_pool;
    g.peds_pool = g.vehicles_pool = g.objects_pool = nullptr;
    g.peds.clear();
    g.vehicles.clear();
}

// number: 0 = the latest fault, n = fault number n (1-based, while among the last 16).
LC_API void lc_faults(lc_fault* out, uint32_t number)
{
    if (out == nullptr) { return; }
    uint32_t total = lc::last_fault().count;
    if (number == 0) { number = total; }
    const lc::FaultRecord& f = number == 0 ? lc::last_fault() : lc::fault_at(number);
    out->count = total;
    out->native = number == 0 ? -1 : g.natives.fault_native(number);
    out->argument = number == total && out->native >= 0 ? g.natives.last_fault_argument() : 0;
    out->address = f.address;
    out->data_address = f.data_address;
    out->code = f.code;
}

LC_API int32_t lc_damage_hook_install(uint32_t function, uint32_t component_to_bone)
{
    return lc::damage::install(function, component_to_bone) ? 1 : 0;
}

LC_API int32_t lc_hooks_report(char* buffer, int32_t size) { return lc::hooks::report(buffer, size); }

// ---- raycast (ADR-0008, docs/research/Raycast.md) ----
namespace
{
    static_assert(sizeof(lc_ray) == 92, "lc_ray layout (CoreAbi.LcRay)");
    static_assert(sizeof(lc_ray_hit) == 144, "lc_ray_hit layout (CoreAbi.LcRayHit)");

    uint32_t g_line_test = 0;
    lc_ray_stats g_ray_stats{};

    // The line test's result block (0x60 bytes): +0x00 the hit physics instance, +0x10 position, +0x20 normal. Every
    // caller in the game zeroes it and sets +0x4C to 0xFFFF first.
    const int kRayResultWords = 24;
    const uint32_t kRayResultMarkerOffset = 0x4C;
    const int kRayPositionWord = 4, kRayNormalWord = 8;
    // The hit entity is [instance + 0x0C] (raycast-spike 2026-09-26: a ray into a ped found that ped there).
    const uint32_t kInstanceEntityOffset = 0x0C;

    struct LineTestCall
    {
        uint32_t function;
        const float* start;
        const float* end;
        uint32_t ignore;
        void* result;
        uint32_t flags;
        int32_t mode;
        bool returned;
    };

    // cdecl bool TestLine(Vec3* start, Vec3* end, CEntity* ignore, Result* out, uint includeFlags, int mode, int):
    // the function sets only al, so the return is read as one byte (never the whole eax) and any non-zero value is a hit.
    typedef uint8_t(__cdecl* LineTest)(const float*, const float*, uint32_t, void*, uint32_t, int32_t, int32_t);

    void line_test_thunk(void* p)
    {
        LineTestCall* c = static_cast<LineTestCall*>(p);
        // Every caller in the game passes a trailing 4 (unused by the function itself); pass it the same way.
        c->returned = reinterpret_cast<LineTest>(c->function)(c->start, c->end, c->ignore, c->result, c->flags, c->mode, 4) != 0;
    }

    lc::RagePool* pool_of(int32_t kind)
    {
        return kind == LC_ENTITY_PED ? g.peds_pool : kind == LC_ENTITY_VEHICLE ? g.vehicles_pool : kind == LC_ENTITY_OBJECT ? g.objects_pool : nullptr;
    }

    // Entity address of a live script handle (0 when stale or unknown). Pure pool arithmetic, no game call.
    uint32_t entity_of(int32_t kind, int32_t handle)
    {
        lc::RagePool* pool = pool_of(kind);
        if (pool == nullptr || !pool->valid() || handle <= 0) { return 0; }
        int index = handle >> 8;
        if (index < 0 || index >= pool->size() || pool->handle_at(index) != handle) { return 0; }
        return pool->object_at(index);
    }

    bool classify(uint32_t address, int32_t& kind, int32_t& handle)
    {
        if (address < 0x10000) { return false; }
        for (int32_t k = LC_ENTITY_PED; k <= LC_ENTITY_OBJECT; k++)
        {
            lc::RagePool* pool = pool_of(k);
            if (pool == nullptr || !pool->valid()) { continue; }
            int32_t h = pool->handle_of(address);
            if (h != 0) { kind = k; handle = h; return true; }
        }
        return false;
    }

    // Research only (LC_RAY_RESEARCH): look for the entity anywhere in the result, then behind its first word.
    int32_t research_link(const uint32_t* result, int32_t& kind, int32_t& handle, uint32_t& entity)
    {
        for (int i = 0; i < kRayResultWords; i++)
        {
            if (classify(result[i], kind, handle)) { entity = result[i]; return -100 - i; }
        }
        for (uint32_t offset = 0; offset < 0x100 && result[0] > 0x10000; offset += 4)
        {
            uint32_t value = 0;
            if (safe_read32(result[0] + offset, value) && classify(value, kind, handle)) { entity = value; return static_cast<int32_t>(offset); }
        }
        return -2;
    }

    struct RayContext
    {
        const lc_ray* ray;
        uint32_t result[kRayResultWords];
        float normal[3];
        int32_t link;
        bool faulted;
    };

    lc::ray::Probe run_line_test(RayContext& c, const lc::ray::Vec& from, const lc::ray::Vec& to, uint32_t ignore)
    {
        lc::ray::Probe p;
        alignas(16) float start[4] = { from.x, from.y, from.z, 0.0f };
        alignas(16) float end[4] = { to.x, to.y, to.z, 0.0f };
        alignas(16) uint32_t result[kRayResultWords] = {};
        result[kRayResultMarkerOffset / 4] = 0xFFFF;
        LineTestCall call{ g_line_test, start, end, ignore, result, c.ray->include_flags, c.ray->mode, false };
        g_ray_stats.tests++;
        if (!lc::safe_invoke(reinterpret_cast<uint32_t>(&line_test_thunk), &call))
        {
            c.faulted = true;
            p.status = lc::ray::kFailed;
            return p;
        }
        std::memcpy(c.result, result, sizeof(result));
        if (!call.returned) { p.status = lc::ray::kClear; return p; }
        p.status = lc::ray::kHit;
        std::memcpy(&p.position, &result[kRayPositionWord], 12);
        std::memcpy(c.normal, &result[kRayNormalWord], 12);
        c.link = -2;
        uint32_t entity = 0;
        int32_t kind = LC_ENTITY_NONE, handle = 0;
        if (result[0] > 0x10000 && safe_read32(result[0] + kInstanceEntityOffset, entity) && classify(entity, kind, handle))
        {
            c.link = static_cast<int32_t>(kInstanceEntityOffset);
        }
        else
        {
            entity = 0; kind = LC_ENTITY_NONE; handle = 0;
            if (c.ray->flags & LC_RAY_RESEARCH) { c.link = research_link(result, kind, handle, entity); }
        }
        p.kind = kind;
        p.handle = handle;
        // World geometry is never handed back as the ignored entity: only pool entities are known to be CEntity.
        p.entity = kind == LC_ENTITY_NONE ? 0 : entity;
        return p;
    }
}

LC_API int32_t lc_raycast_install(uint32_t line_test)
{
    g_line_test = line_test;
    g_ray_stats = lc_ray_stats{};
    g_ray_stats.installed = line_test != 0 ? 1 : 0;
    return g_ray_stats.installed;
}

LC_API int32_t lc_raycast(const lc_ray* ray, lc_ray_hit* hit)
{
    if (ray == nullptr || hit == nullptr || ray->size != sizeof(lc_ray) || ray->hit_size != sizeof(lc_ray_hit)) { return LC_RAY_UNAVAILABLE; }
    std::memset(hit, 0, sizeof(lc_ray_hit));
    hit->link = -2;
    if (!g.initialised || g_line_test == 0 || ray->ignore_count < 0 || ray->ignore_count > LC_RAY_MAX_IGNORE) { return LC_RAY_UNAVAILABLE; }
    g_ray_stats.queries++;
    lc::ray::Query q;
    q.start = lc::ray::Vec{ ray->start[0], ray->start[1], ray->start[2] };
    q.end = lc::ray::Vec{ ray->end[0], ray->end[1], ray->end[2] };
    q.accept = ray->accept;
    q.max_passes = ray->max_passes < 0 ? 0 : ray->max_passes;
    q.step = ray->pass_step;
    q.ignore_count = ray->ignore_count;
    for (int i = 0; i < ray->ignore_count; i++) { q.ignore_kind[i] = ray->ignore_kind[i]; q.ignore_handle[i] = ray->ignore_handle[i]; }
    q.first_ignore = ray->ignore_count > 0 ? entity_of(ray->ignore_kind[0], ray->ignore_handle[0]) : 0;

    RayContext context{};
    context.ray = ray;
    context.link = -2;
    lc::ray::Outcome out = lc::ray::walk(q, [&](const lc::ray::Vec& from, const lc::ray::Vec& to, uint32_t ignore) {
        return run_line_test(context, from, to, ignore);
    });
    hit->tests = out.tests;
    hit->passes = out.passes;
    g_ray_stats.passes += static_cast<uint32_t>(out.passes);
    std::memcpy(hit->raw, context.result, sizeof(context.result));
    if (context.faulted)
    {
        // A fault in the game's own physics query means an assumption about it is wrong: off for the session.
        uint32_t number = lc::last_fault().count;
        g.natives.note_raycast_fault(number);
        g_ray_stats.faults++;
        g_ray_stats.fault_number = number;
        g_ray_stats.installed = 0;
        g_line_test = 0;
        return LC_RAY_UNAVAILABLE;
    }
    if (out.result == lc::ray::kFailed) { return LC_RAY_UNAVAILABLE; }
    if (out.result == lc::ray::kClear) { g_ray_stats.clears++; return LC_RAY_CLEAR; }
    hit->position[0] = out.last.position.x; hit->position[1] = out.last.position.y; hit->position[2] = out.last.position.z;
    std::memcpy(hit->normal, context.normal, sizeof(context.normal));
    hit->distance = out.distance;
    hit->entity_kind = out.last.kind;
    hit->entity_handle = out.last.handle;
    hit->link = context.link;
    if (out.result == lc::ray::kInconclusive) { g_ray_stats.inconclusive++; return LC_RAY_INCONCLUSIVE; }
    g_ray_stats.hits++;
    return LC_RAY_HIT;
}

LC_API void lc_raycast_stats(lc_ray_stats* out)
{
    if (out != nullptr) { *out = g_ray_stats; }
}

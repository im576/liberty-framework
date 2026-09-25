// LibertyCore: per-frame world snapshot and change events (ADR-0006). Runs inside the engine host's tick, while the
// game thread is parked; lc_frame measures that with the engine frame counter and reports violations.
#include <windows.h>
#include <algorithm>
#include <cmath>
#include <cstring>
#include <unordered_map>
#include "liberty_core.h"
#include "natives.h"
#include "ped_pool.h"

namespace
{
    struct PedMemory
    {
        int32_t health;
        uint32_t flags;
        uint32_t seen_frame;
    };

    struct State
    {
        bool initialised = false;
        lc::Natives natives;
        lc_address_book book{};
        lc::PedPool* pool = nullptr;
        lc_snapshot snapshot{};
        std::unordered_map<int32_t, PedMemory> peds;
        bool have_previous_player = false;
        lc_player previous_player{};
        bool have_previous_world = false;
        lc_world previous_world{};
        LARGE_INTEGER frequency{};
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

    bool ped_exists(int32_t handle)
    {
        return handle > 0 && g.natives.call1(LC_N_DOES_CHAR_EXIST, handle) != 0;
    }

    // Position, heading, health, armour, model, vehicle and dead flag of an existing ped.
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

    void player_events(const lc_player& now)
    {
        if (!g.have_previous_player || g.previous_player.ped != now.ped) { g.previous_player = now; g.have_previous_player = true; return; }
        const lc_player& was = g.previous_player;
        if (now.weapon != was.weapon) { push(LC_EV_PLAYER_WEAPON, was.weapon, now.weapon); }
        else if (now.ammo_in_clip >= 0 && was.ammo_in_clip >= 0)
        {
            if (now.ammo_in_clip < was.ammo_in_clip) { push(LC_EV_PLAYER_SHOT, now.weapon, was.ammo_in_clip, now.ammo_in_clip); }
            else if (now.ammo_in_clip > was.ammo_in_clip) { push(LC_EV_PLAYER_RELOADED, now.weapon, was.ammo_in_clip, now.ammo_in_clip); }
        }
        bool inNow = (now.flags & LC_PLAYER_IN_VEHICLE) != 0, inWas = (was.flags & LC_PLAYER_IN_VEHICLE) != 0;
        if (inNow && !inWas) { push(LC_EV_PLAYER_ENTERED_VEHICLE, now.vehicle); }
        if (!inNow && inWas) { push(LC_EV_PLAYER_EXITED_VEHICLE, was.vehicle); }
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

    // Lists live peds within the radius (nearest first when over capacity) and diffs them against the last frame.
    void read_peds(const lc_frame_input& input, bool have_player)
    {
        lc_snapshot& s = g.snapshot;
        s.ped_count = 0;
        if (g.pool == nullptr || !g.pool->valid() || !ped_natives_ready()) { return; }
        static lc_ped candidates[1024];
        int count = 0;
        const float radius2 = input.ped_radius * input.ped_radius;
        const int size = g.pool->size();
        for (int index = 0; index < size && count < 1024; index++)
        {
            int32_t handle = g.pool->handle_at(index);
            if (handle == 0 || !ped_exists(handle)) { continue; }
            int32_t outs[3] = {};
            int32_t args[1] = { handle };
            g.natives.call(LC_N_GET_CHAR_COORDINATES, args, outs);
            float dx = as_float(outs[0]) - s.player.x, dy = as_float(outs[1]) - s.player.y, dz = as_float(outs[2]) - s.player.z;
            float d2 = dx * dx + dy * dy + dz * dz;
            if (have_player && d2 > radius2 && handle != input.player_ped) { continue; }
            candidates[count].handle = handle;
            candidates[count].distance = std::sqrt(d2);
            count++;
        }
        if (count > LC_MAX_PEDS)
        {
            std::nth_element(candidates, candidates + LC_MAX_PEDS, candidates + count,
                [](const lc_ped& a, const lc_ped& b) { return a.distance < b.distance; });
            count = LC_MAX_PEDS;
        }
        const uint32_t frame = s.frame;
        for (int i = 0; i < count; i++)
        {
            float distance = candidates[i].distance;
            lc_ped& ped = s.peds[s.ped_count++];
            read_ped(candidates[i].handle, ped);
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
                int32_t byPlayer = 0;
                if (have_player && ready(LC_N_HAS_CHAR_BEEN_DAMAGED_BY_CHAR))
                {
                    int32_t args[3] = { ped.handle, input.player_ped, 0 };
                    byPlayer = g.natives.call(LC_N_HAS_CHAR_BEEN_DAMAGED_BY_CHAR, args, nullptr) != 0 ? 1 : 0;
                }
                push(LC_EV_PED_DAMAGED, ped.handle, was.health, ped.health, bone, byPlayer);
                if ((ped.flags & LC_PED_DEAD) && !(was.flags & LC_PED_DEAD)) { push(LC_EV_PED_DIED, ped.handle, 0, 0, bone, byPlayer); }
            }
            else if ((ped.flags & LC_PED_DEAD) && !(was.flags & LC_PED_DEAD) && !(ped.flags & LC_PED_PLAYER))
            {
                push(LC_EV_PED_DIED, ped.handle);
            }
            was.health = ped.health;
            was.flags = ped.flags;
            was.seen_frame = frame;
        }
        // Peds not listed this frame (despawned or out of range) are forgotten.
        for (auto it = g.peds.begin(); it != g.peds.end();)
        {
            if (it->second.seen_frame != frame) { push(LC_EV_PED_REMOVED, it->first); it = g.peds.erase(it); }
            else { ++it; }
        }
    }
}

LC_API uint32_t lc_abi_version(void) { return LC_ABI_VERSION; }

LC_API uint32_t lc_native_hash(int32_t id)
{
    return id >= 0 && id < LC_N_COUNT ? lc::native_info(id).hash : 0;
}

LC_API int32_t lc_init(const lc_address_book* book, const uint32_t* handlers, char* error, int32_t error_size)
{
    auto fail = [&](const char* text) { if (error != nullptr && error_size > 0) { strncpy(error, text, error_size - 1); error[error_size - 1] = 0; } return 0; };
    if (book == nullptr || book->size != sizeof(lc_address_book)) { return fail("address book size mismatch"); }
    if (handlers == nullptr) { return fail("no native handlers"); }
    g.book = *book;
    for (int id = 0; id < LC_N_COUNT; id++) { g.natives.set_handler(id, handlers[id]); }
    delete g.pool;
    g.pool = book->ped_pool_global != 0 ? new lc::PedPool(book->ped_pool_global) : nullptr;
    g.peds.clear();
    g.peds.reserve(512);
    g.have_previous_player = false;
    g.have_previous_world = false;
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
        if (g.pool != nullptr && g.pool->valid() && ped_natives_ready()) { s.valid |= LC_VALID_PEDS; }
    }

    if (g.book.frame_counter != 0 && engine_frame() != s.engine_frame) { s.parked_violations++; }
    LARGE_INTEGER end;
    QueryPerformanceCounter(&end);
    s.core_microseconds = static_cast<float>((end.QuadPart - start.QuadPart) * 1000000.0 / g.frequency.QuadPart);
    return &s;
}

LC_API void lc_shutdown(void)
{
    g.initialised = false;
    delete g.pool;
    g.pool = nullptr;
    g.peds.clear();
}

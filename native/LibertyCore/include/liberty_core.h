// LibertyCore C ABI (ADR-0006). Mirrored field-for-field by src/LibertyFramework/Engine/Core/CoreAbi.cs.
// Rules: only 4-byte fields (int32_t, uint32_t, float), no padding; bump LC_ABI_VERSION on any change.
#pragma once
#include <stdint.h>

#ifdef __cplusplus
#define LC_EXTERN extern "C"
#else
#define LC_EXTERN
#endif
#define LC_API LC_EXTERN __declspec(dllexport)

#define LC_ABI_VERSION 5
#define LC_MAX_PEDS 128
#define LC_MAX_VEHICLES 64
#define LC_MAX_EVENTS 256
#define LC_MAX_BULLETS 32
#define LC_MAX_DAMAGES 32

// Script natives the core calls directly. The host resolves each hash to its handler (lc_native_hash) and passes
// the handlers to lc_init; a 0 handler or a failed verification leaves the dependent snapshot fields invalid.
enum lc_native_id
{
    LC_N_DOES_CHAR_EXIST,
    LC_N_IS_CHAR_DEAD,
    LC_N_GET_CHAR_HEALTH,
    LC_N_GET_CHAR_ARMOUR,
    LC_N_GET_CHAR_COORDINATES,
    LC_N_GET_CHAR_HEADING,
    LC_N_GET_CHAR_MODEL,
    LC_N_IS_CHAR_IN_ANY_CAR,
    LC_N_GET_CAR_CHAR_IS_USING,
    LC_N_GET_CURRENT_CHAR_WEAPON,
    LC_N_GET_AMMO_IN_CLIP,
    LC_N_GET_CHAR_LAST_DAMAGE_BONE,
    LC_N_HAS_CHAR_BEEN_DAMAGED_BY_CHAR,
    LC_N_GET_PLAYER_ID,
    LC_N_IS_PLAYER_PLAYING,
    LC_N_IS_PLAYER_CONTROL_ON,
    LC_N_IS_PAUSE_MENU_ACTIVE,
    LC_N_IS_SCREEN_FADED_OUT,
    LC_N_GET_GAME_TIMER,
    LC_N_GET_HOURS_OF_DAY,
    LC_N_GET_MINUTES_OF_DAY,
    LC_N_GET_CURRENT_WEATHER,
    // ABI 2: vehicles
    LC_N_DOES_VEHICLE_EXIST,
    LC_N_GET_CAR_COORDINATES,
    LC_N_GET_CAR_HEADING,
    LC_N_GET_CAR_SPEED,
    LC_N_GET_CAR_HEALTH,
    LC_N_GET_ENGINE_HEALTH,
    LC_N_GET_CAR_MODEL,
    LC_N_GET_DRIVER_OF_CAR,
    LC_N_COUNT
};

// Addresses the host resolved with its verified scanners (GameAddresses). 0 = unavailable.
typedef struct lc_address_book
{
    uint32_t size;                // sizeof(lc_address_book)
    uint32_t ped_pool_global;     // global holding the ped rage::fwPool*
    uint32_t frame_counter;       // engine frame counter (GET_FRAME_COUNT)
    uint32_t vehicle_pool_global; // ABI 2
    uint32_t object_pool_global;  // ABI 2
    // ABI 2: the per-frame bullet trace list IS_BULLET_IN_AREA scans (GameAddresses.ResolveBullets)
    uint32_t bullet_count_global; // int32 count
    uint32_t bullet_array_global; // pointer to the entry array
    int32_t bullet_stride;
    int32_t bullet_owner_offset;  // CPed* of the shooter
    int32_t bullet_maximum;
} lc_address_book;

typedef struct lc_ped
{
    int32_t handle;
    uint32_t model;
    float x, y, z;
    float heading;
    int32_t health;       // raw native value (gameplay health = raw - 100)
    int32_t armour;
    int32_t vehicle;      // vehicle handle or 0
    uint32_t flags;       // LC_PED_*
    float distance;       // metres from the player
} lc_ped;

#define LC_PED_DEAD 0x1u
#define LC_PED_IN_VEHICLE 0x2u
#define LC_PED_PLAYER 0x4u
#define LC_PED_NEW 0x8u

typedef struct lc_vehicle
{
    int32_t handle;
    uint32_t model;
    float x, y, z;
    float heading;
    float speed;          // m/s
    int32_t health;       // body health 0-1000
    float engine_health;  // -4000..1000; below 0 burning
    int32_t driver;       // ped handle or 0
    uint32_t flags;       // LC_VEH_*
    float distance;
} lc_vehicle;

#define LC_VEH_NEW 0x1u
#define LC_VEH_PLAYER 0x2u

// A bullet trace the game added since the previous frame (start and end after accuracy/impact).
typedef struct lc_bullet
{
    int32_t shooter;      // ped handle or 0
    int32_t weapon;       // shooter's current weapon or -1
    float from_x, from_y, from_z;
    float to_x, to_y, to_z;
} lc_bullet;

// ABI 3: a damage the game applied to a ped this frame, observed in its damage-response routine (ADR-0007).
typedef struct lc_damage
{
    int32_t victim;       // ped handle
    int32_t attacker;     // ped or vehicle handle, 0 if none/other
    int32_t attacker_kind;// 0 none/other, 1 ped, 2 vehicle
    int32_t weapon;       // weapon type (0-20 weapons, 21-44 episodic, 49+ special: car, explosion, fall...)
    int32_t component;    // body component
    int32_t bone;         // bone tag or -1
    float amount;         // damage the game computed
    float health_lost;
    float armour_lost;
    uint32_t flags;       // LC_DAMAGE_*
} lc_damage;

#define LC_DAMAGE_KILLED 0x1u

typedef struct lc_player
{
    int32_t index;
    int32_t ped;          // player ped handle (supplied by the host)
    float x, y, z;
    float heading;
    int32_t health;       // raw
    int32_t armour;
    int32_t weapon;
    int32_t ammo_in_clip;
    int32_t vehicle;
    uint32_t flags;       // LC_PLAYER_*
} lc_player;

#define LC_PLAYER_PLAYING 0x1u
#define LC_PLAYER_CONTROL 0x2u
#define LC_PLAYER_DEAD 0x4u
#define LC_PLAYER_IN_VEHICLE 0x8u
#define LC_PLAYER_RELOADING 0x10u

typedef struct lc_world
{
    uint32_t game_timer_ms;
    int32_t hours;
    int32_t minutes;
    int32_t weather;
    uint32_t flags;       // LC_WORLD_*
} lc_world;

#define LC_WORLD_PAUSED 0x1u
#define LC_WORLD_FADED_OUT 0x2u

// Pool occupancy (for the performance governor and the inspector).
typedef struct lc_pools
{
    int32_t peds_used, peds_size;
    int32_t vehicles_used, vehicles_size;
    int32_t objects_used, objects_size;
} lc_pools;

enum lc_event_type
{
    LC_EV_NONE,
    LC_EV_PED_APPEARED,       // a = ped
    LC_EV_PED_REMOVED,        // a = ped
    LC_EV_PED_DAMAGED,        // a = ped, b = health before, c = health after, d = bone, e = 1 if the player damaged it
    LC_EV_PED_DIED,           // a = ped, d = bone, e = 1 if the player damaged it
    LC_EV_PLAYER_SHOT,        // a = weapon, b = clip before, c = clip after
    LC_EV_PLAYER_RELOADED,    // a = weapon, b = clip before, c = clip after
    LC_EV_PLAYER_WEAPON,      // a = previous weapon, b = new weapon
    LC_EV_PLAYER_ENTERED_VEHICLE, // a = vehicle
    LC_EV_PLAYER_EXITED_VEHICLE,  // a = vehicle
    LC_EV_PLAYER_DIED,
    LC_EV_PLAYER_DAMAGED,     // b = health before, c = health after, d = armour before, e = armour after
    LC_EV_WEATHER_CHANGED,    // a = previous, b = new
    LC_EV_PAUSE_CHANGED,      // a = 1 paused
    LC_EV_FADE_CHANGED,       // a = 1 faded out
    // ABI 2
    LC_EV_VEHICLE_APPEARED,   // a = vehicle
    LC_EV_VEHICLE_REMOVED,    // a = vehicle
    LC_EV_VEHICLE_DAMAGED,    // a = vehicle, b = health before, c = after, d = engine before (int), e = engine after (int)
    LC_EV_VEHICLE_DESTROYED,  // a = vehicle
    LC_EV_PLAYER_RELOAD_STARTED // a = weapon, b = clip before
};

typedef struct lc_event
{
    int32_t type;
    int32_t a, b, c, d, e;
} lc_event;

typedef struct lc_snapshot
{
    uint32_t size;              // sizeof(lc_snapshot)
    uint32_t abi_version;
    uint32_t frame;
    uint32_t engine_frame;      // engine frame counter at the start of the frame (0 if unavailable)
    uint32_t parked_violations; // frames where the engine frame counter moved during the core's work
    uint32_t valid;             // LC_VALID_*
    float core_microseconds;
    lc_world world;
    lc_player player;
    lc_pools pools;
    int32_t ped_count;
    lc_ped peds[LC_MAX_PEDS];
    int32_t vehicle_count;
    lc_vehicle vehicles[LC_MAX_VEHICLES];
    int32_t bullet_count;
    lc_bullet bullets[LC_MAX_BULLETS];
    int32_t damage_count;
    lc_damage damages[LC_MAX_DAMAGES];
    int32_t event_count;
    int32_t events_dropped;
    lc_event events[LC_MAX_EVENTS];
} lc_snapshot;

#define LC_VALID_WORLD 0x1u
#define LC_VALID_PLAYER 0x2u
#define LC_VALID_PEDS 0x4u
#define LC_VALID_VEHICLES 0x8u
#define LC_VALID_BULLETS 0x10u
#define LC_VALID_DAMAGE 0x20u

typedef struct lc_frame_input
{
    uint32_t size;              // sizeof(lc_frame_input)
    int32_t player_ped;         // from the host (GET_PLAYER_CHAR is not called directly)
    float ped_radius;           // metres
    uint32_t enabled;           // LC_VALID_* parts to produce this frame
    float vehicle_radius;       // ABI 2
} lc_frame_input;

LC_API uint32_t lc_abi_version(void);
LC_API uint32_t lc_native_hash(int32_t id);
// handlers: LC_N_COUNT entries in lc_native_id order. Returns 1 on success; error text is written on failure.
LC_API int32_t lc_init(const lc_address_book* book, const uint32_t* handlers, char* error, int32_t error_size);
LC_API void lc_set_native_verified(int32_t id, int32_t verified);
// Calls a core native directly for the host's verification: value arguments in args, out slots returned via outs.
LC_API int32_t lc_call_native(int32_t id, int32_t argc, const int32_t* args, int32_t* outs);
LC_API const lc_snapshot* lc_frame(const lc_frame_input* input);
LC_API void lc_shutdown(void);

// ABI 2: crash capture. An unhandled exception writes <dir>\crash-<time>.dmp and .txt (exception, address, module,
// and the engine phase registered below) before the previous handler runs. dir is UTF-16.
LC_API int32_t lc_install_crash_handler(const wchar_t* dir);
// Names engine phases once (index -> name), then marks the current one each time it changes (no allocation).
LC_API void lc_register_phase(int32_t index, const char* name);
LC_API void lc_set_phase(int32_t index);
// Current phase index (-1 none); read by the host's watchdog thread.
LC_API int32_t lc_get_phase(void);
// Writes <dir>\stall-<time>.dmp/.txt without an exception (watchdog: an engine frame is taking too long). 1 on success.
LC_API int32_t lc_write_dump(const char* reason);

// ABI 2: faults the core contained (a native or game-memory read that faulted and was recovered from; the native is
// then disabled for the session). native is an lc_native_id or -1 for a raw read; address is the faulting instruction.
typedef struct lc_fault
{
    uint32_t count;
    int32_t native;          // lc_native_id; -1 raw game-memory read; -2 the raycast line test (ABI 5)
    int32_t argument;
    uint32_t address;
    uint32_t data_address;
    uint32_t code;
} lc_fault;

// number 0 = latest; n = fault number n (1-based) while it is among the last 16.
LC_API void lc_faults(lc_fault* out, uint32_t number);

// ABI 3: hooks (ADR-0007). Installs the exact-damage observer on the damage-response routine; both addresses come from
// the host's signature scan. 1 = installed (or already installed), 0 = refused (bytes differ / already hooked).
LC_API int32_t lc_damage_hook_install(uint32_t function, uint32_t component_to_bone);
// Writes one line per hook (name, target, state); returns the number of hooks.
LC_API int32_t lc_hooks_report(char* buffer, int32_t size);

// ---- Raycast / line of sight (ABI 5; docs/research/Raycast.md, ADR-0008) ----
// The game's general line test (GameAddresses.LineTestFunction), called directly. Engine tick only (the game thread is
// parked in the script); every call runs under the core's SEH guard and a fault switches raycasts off for the session.
//
// One query may run several line tests. The game reports the first thing along the segment; when that thing is not a
// kind the caller accepts (or is on the caller's ignore list), the walk passes through it: the next test starts
// pass_step metres beyond the hit, with the hit entity handed to the game as its one ignored entity. Hit kinds come
// from the entity pools (the verified link: entity = [first result word + 0x0C]), not from include bits, so the
// filtering is correct even for bits nobody has mapped yet.

#define LC_ENTITY_NONE 0     // world geometry, or anything that is not in the ped, vehicle or object pool
#define LC_ENTITY_PED 1
#define LC_ENTITY_VEHICLE 2
#define LC_ENTITY_OBJECT 3

// lc_ray.accept: bit (1 << LC_ENTITY_*) for each kind that stops the ray.
#define LC_RAY_ACCEPT_WORLD 0x1
#define LC_RAY_ACCEPT_PEDS 0x2
#define LC_RAY_ACCEPT_VEHICLES 0x4
#define LC_RAY_ACCEPT_OBJECTS 0x8
#define LC_RAY_ACCEPT_ALL 0xF

// Game include bits seen in game (raybits, 2026-09-26): 0x2 hit the ground, 0x20 and 0x40 hit a ped. The rest are
// unmapped; LC_RAY_INCLUDE_ALL hit both and is what the engine passes.
#define LC_RAY_INCLUDE_ALL 0xFFFFFFFFu

#define LC_RAY_MAX_IGNORE 4
#define LC_RAY_RESEARCH 0x1  // lc_ray.flags: also search the whole result for the hit entity (slow; raydebug)

// Return values of lc_raycast.
#define LC_RAY_CLEAR 0
#define LC_RAY_HIT 1
#define LC_RAY_UNAVAILABLE (-1)   // not installed, switched off after a fault, bad sizes, or this call faulted
#define LC_RAY_INCONCLUSIVE (-2)  // max_passes excluded hits and the segment was not finished

typedef struct lc_ray
{
    uint32_t size;           // sizeof(lc_ray)
    uint32_t hit_size;       // sizeof(lc_ray_hit)
    float start[3];
    float end[3];
    uint32_t include_flags;  // game include bits (LC_RAY_INCLUDE_ALL)
    int32_t mode;            // the line test's mode argument (callers in the game pass 1, -1, 0x40, 8; 1 verified)
    uint32_t accept;         // LC_RAY_ACCEPT_*
    int32_t max_passes;      // excluded hits a query may pass through (0 = report the raw first hit's verdict)
    float pass_step;         // metres beyond an excluded hit where the next test starts (> 0)
    int32_t ignore_count;    // 0..LC_RAY_MAX_IGNORE; entities that never stop the ray (the first goes to the game)
    int32_t ignore_kind[LC_RAY_MAX_IGNORE];    // LC_ENTITY_PED / VEHICLE / OBJECT
    int32_t ignore_handle[LC_RAY_MAX_IGNORE];
    uint32_t flags;          // LC_RAY_RESEARCH
} lc_ray;

typedef struct lc_ray_hit
{
    float position[3];       // LC_RAY_HIT: the accepted hit; LC_RAY_INCONCLUSIVE: the last excluded hit
    float normal[3];
    float distance;          // metres from lc_ray.start to position
    int32_t entity_kind;     // LC_ENTITY_*
    int32_t entity_handle;   // script handle, 0 for world geometry
    int32_t link;            // where the entity was found: 0x0C (the verified link), -100 - i (raw word i, research),
                             // a research scan offset, or -2 (not found: world)
    int32_t tests;           // line tests this query ran
    int32_t passes;          // hits it passed through
    uint32_t raw[24];        // the last line test's 0x60-byte result, for research and diagnostics
} lc_ray_hit;

typedef struct lc_ray_stats
{
    uint32_t queries;
    uint32_t tests;
    uint32_t hits;
    uint32_t clears;
    uint32_t passes;
    uint32_t inconclusive;
    uint32_t faults;
    uint32_t fault_number;   // the lc_faults number of the fault that switched raycasts off, 0 if none
    int32_t installed;       // 1 while raycasts are available
} lc_ray_stats;

LC_API int32_t lc_raycast_install(uint32_t line_test);
// LC_RAY_HIT, LC_RAY_CLEAR, LC_RAY_INCONCLUSIVE or LC_RAY_UNAVAILABLE. hit is always written (zeroed first).
LC_API int32_t lc_raycast(const lc_ray* ray, lc_ray_hit* hit);
LC_API void lc_raycast_stats(lc_ray_stats* out);

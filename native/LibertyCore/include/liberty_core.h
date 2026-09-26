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

#define LC_ABI_VERSION 2
#define LC_MAX_PEDS 128
#define LC_MAX_VEHICLES 64
#define LC_MAX_EVENTS 256
#define LC_MAX_BULLETS 32

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
    int32_t event_count;
    int32_t events_dropped;
    lc_event events[LC_MAX_EVENTS];
} lc_snapshot;

#define LC_VALID_WORLD 0x1u
#define LC_VALID_PLAYER 0x2u
#define LC_VALID_PEDS 0x4u
#define LC_VALID_VEHICLES 0x8u
#define LC_VALID_BULLETS 0x10u

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
    int32_t native;
    int32_t argument;
    uint32_t address;
    uint32_t data_address;
    uint32_t code;
} lc_fault;

// number 0 = latest; n = fault number n (1-based) while it is among the last 16.
LC_API void lc_faults(lc_fault* out, uint32_t number);

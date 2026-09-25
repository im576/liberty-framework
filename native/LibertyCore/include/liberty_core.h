// LibertyCore C ABI (ADR-0006). Mirrored field-for-field by src/LibertyFramework/Engine/Core/CoreAbi.cs.
// Rules: only 4-byte fields (int32_t, uint32_t, float), no padding, append-only; bump LC_ABI_VERSION on any change.
#pragma once
#include <stdint.h>

#ifdef __cplusplus
#define LC_EXTERN extern "C"
#else
#define LC_EXTERN
#endif
#define LC_API LC_EXTERN __declspec(dllexport)

#define LC_ABI_VERSION 1
#define LC_MAX_PEDS 128
#define LC_MAX_EVENTS 256

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
    LC_N_COUNT
};

// Addresses the host resolved with its verified scanners (GameAddresses). 0 = unavailable.
typedef struct lc_address_book
{
    uint32_t size;            // sizeof(lc_address_book)
    uint32_t ped_pool_global; // address of the global holding the ped rage::fwPool*
    uint32_t frame_counter;   // address of the engine frame counter (GET_FRAME_COUNT)
} lc_address_book;

typedef struct lc_ped
{
    int32_t handle;
    uint32_t model;
    float x, y, z;
    float heading;
    int32_t health;       // raw native value (SHDN Ped.Health = raw - 100)
    int32_t armour;
    int32_t vehicle;      // vehicle handle or 0
    uint32_t flags;       // LC_PED_*
    float distance;       // metres from the player
} lc_ped;

#define LC_PED_DEAD 0x1u
#define LC_PED_IN_VEHICLE 0x2u
#define LC_PED_PLAYER 0x4u
#define LC_PED_NEW 0x8u

typedef struct lc_player
{
    int32_t index;        // player id
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

enum lc_event_type
{
    LC_EV_NONE,
    LC_EV_PED_APPEARED,       // a = ped
    LC_EV_PED_REMOVED,        // a = ped
    LC_EV_PED_DAMAGED,        // a = ped, b = health before, c = health after, d = bone, e = 1 if the player damaged it
    LC_EV_PED_DIED,           // a = ped, e = 1 if the player damaged it
    LC_EV_PLAYER_SHOT,        // a = weapon, b = clip before, c = clip after
    LC_EV_PLAYER_RELOADED,    // a = weapon, b = clip before, c = clip after
    LC_EV_PLAYER_WEAPON,      // a = previous weapon, b = new weapon
    LC_EV_PLAYER_ENTERED_VEHICLE, // a = vehicle
    LC_EV_PLAYER_EXITED_VEHICLE,  // a = vehicle
    LC_EV_PLAYER_DIED,
    LC_EV_PLAYER_DAMAGED,     // b = health before, c = health after, d = armour before, e = armour after
    LC_EV_WEATHER_CHANGED,    // a = previous, b = new
    LC_EV_PAUSE_CHANGED,      // a = 1 paused
    LC_EV_FADE_CHANGED        // a = 1 faded out
};

typedef struct lc_event
{
    int32_t type;
    int32_t a, b, c, d, e;
} lc_event;

typedef struct lc_snapshot
{
    uint32_t size;            // sizeof(lc_snapshot)
    uint32_t abi_version;
    uint32_t frame;           // core frame index
    uint32_t engine_frame;    // engine frame counter at the start of the frame (0 if unavailable)
    uint32_t parked_violations; // frames where the engine frame counter moved during the core's work
    uint32_t valid;           // LC_VALID_* bits: which parts were produced by verified natives
    float core_microseconds;  // time lc_frame spent
    lc_world world;
    lc_player player;
    int32_t ped_count;
    lc_ped peds[LC_MAX_PEDS];
    int32_t event_count;
    int32_t events_dropped;
    lc_event events[LC_MAX_EVENTS];
} lc_snapshot;

#define LC_VALID_WORLD 0x1u
#define LC_VALID_PLAYER 0x2u
#define LC_VALID_PEDS 0x4u

typedef struct lc_frame_input
{
    uint32_t size;            // sizeof(lc_frame_input)
    int32_t player_ped;       // from the host (GET_PLAYER_CHAR is not called directly)
    float ped_radius;         // metres; peds farther than this are not listed
    uint32_t enabled;         // LC_VALID_* parts to produce this frame
} lc_frame_input;

LC_API uint32_t lc_abi_version(void);
LC_API uint32_t lc_native_hash(int32_t id);
// handlers: LC_N_COUNT entries in lc_native_id order. Returns 1 on success; error text is written on failure.
LC_API int32_t lc_init(const lc_address_book* book, const uint32_t* handlers, char* error, int32_t error_size);
// Enables or disables one native after the host compared its direct result with SHDN.
LC_API void lc_set_native_verified(int32_t id, int32_t verified);
// Calls a core native directly for the host's verification: up to 4 int-sized arguments, out slots returned via outs.
LC_API int32_t lc_call_native(int32_t id, int32_t argc, const int32_t* args, int32_t* outs);
LC_API const lc_snapshot* lc_frame(const lc_frame_input* input);
LC_API void lc_shutdown(void);

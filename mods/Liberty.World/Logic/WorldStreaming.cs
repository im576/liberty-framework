using System;

namespace Liberty.World
{
    // Whether a placed object should be spawned, removed or left as it is, from the player's horizontal distance. The gap
    // between the two radii (hysteresis) keeps an object at the edge from being created and deleted every check.
    public static class WorldStreaming
    {
        public enum Step { Keep, Spawn, Remove }

        // present: spawned or being spawned. Distances in metres.
        public static Step Decide(bool present, float distanceMeters, float streamInMeters, float streamOutMeters)
        {
            if (!present && distanceMeters <= streamInMeters) { return Step.Spawn; }
            if (present && distanceMeters > streamOutMeters) { return Step.Remove; }
            return Step.Keep;
        }

        // Horizontal distance (metres): objects stream by map position, whatever the player's height.
        public static float HorizontalDistance(float ax, float ay, float bx, float by)
        {
            float dx = ax - bx, dy = ay - by;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}

namespace Liberty.Sdk
{
    // Outcome of a raycast.
    public enum RayStatus
    {
        // Nothing in the mask lies between the two points.
        Clear = 0,
        // Something in the mask was hit; RayHit says what and where.
        Hit = 1,
        // More things outside the mask were in the way than one query passes through (engine.json raycastMaxPasses);
        // the answer is unknown. RayHit holds the last thing passed through. Treat as blocked when in doubt.
        Inconclusive = 2,
        // No raycast was made: the engine core is off, the game's line test was not found or was switched off after a
        // fault, raycasts are disabled in engine.json, or the call came from outside the engine tick (e.g. OnDraw).
        Unavailable = 3,
    }
}

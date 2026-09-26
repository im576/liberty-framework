using System;

namespace Liberty.Sdk
{
    // Peds. Created peds belong to the creating module and are deleted when it stops unless Release'd to the game.
    public interface IPeds
    {
        // Streams the model and creates the ped when ready (usually within a few frames); onReady gets None on failure.
        void Spawn(LibertyModule owner, ModelRef model, Vec3 position, float heading, Action<PedRef> onReady);
        // Creates immediately if the model is already loaded (and requests it otherwise); None when not ready yet.
        PedRef TryCreate(LibertyModule owner, ModelRef model, Vec3 position, float heading);
        // Random ambient ped model for the area.
        void SpawnRandom(LibertyModule owner, Vec3 position, float heading, Action<PedRef> onReady);
        void Delete(PedRef ped);
        // Hands an owned ped back to the game (it will not be deleted when the module stops).
        void Release(LibertyModule owner, PedRef ped);

        bool Exists(PedRef ped);
        Vec3 GetPosition(PedRef ped);
        void SetPosition(PedRef ped, Vec3 position);
        float GetHeading(PedRef ped);
        void SetHeading(PedRef ped, float heading);
        int GetHealth(PedRef ped);
        void SetHealth(PedRef ped, int health);
        int GetArmour(PedRef ped);
        void SetArmour(PedRef ped, int armour);
        bool IsDead(PedRef ped);
        void Kill(PedRef ped);
        ModelRef GetModel(PedRef ped);
        VehicleRef GetVehicle(PedRef ped);
        Vec3 GetBonePosition(PedRef ped, Bone bone);
        void SetInvincible(PedRef ped, bool invincible);
        void SetFrozen(PedRef ped, bool frozen);
        // Ignore ambient events (gunfire, threats) so the ped follows only scripted tasks.
        void SetBlockEvents(PedRef ped, bool block);
        // 0-100.
        void SetAccuracy(PedRef ped, int accuracy);
        void Ragdoll(PedRef ped, int milliseconds);
    }
}
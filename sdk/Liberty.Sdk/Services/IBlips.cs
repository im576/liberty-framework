namespace Liberty.Sdk
{
    // Radar/map blips. Owned; removed when the module stops.
    public interface IBlips
    {
        BlipRef AddForPosition(LibertyModule owner, Vec3 position);
        BlipRef AddForPed(LibertyModule owner, PedRef ped);
        BlipRef AddForVehicle(LibertyModule owner, VehicleRef vehicle);
        void SetSprite(BlipRef blip, int sprite);
        void SetColour(BlipRef blip, int colour);
        void SetName(BlipRef blip, string name);
        void SetRoute(BlipRef blip, bool on);
        void Remove(BlipRef blip);
    }
}
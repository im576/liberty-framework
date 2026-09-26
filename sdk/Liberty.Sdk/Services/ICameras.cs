namespace Liberty.Sdk
{
    // Scripted cameras (owned; destroyed and the game camera restored when the module stops) and the game camera.
    public interface ICameras
    {
        CameraRef Create(LibertyModule owner);
        void Destroy(CameraRef camera);
        void SetPosition(CameraRef camera, Vec3 position);
        // Rotation in degrees (pitch, roll, heading).
        void SetRotation(CameraRef camera, Vec3 degrees);
        void PointAt(CameraRef camera, Vec3 target);
        void PointAt(CameraRef camera, PedRef ped);
        void SetFov(CameraRef camera, float degrees);
        // Makes this camera the rendering camera; Deactivate returns to the game camera.
        void Activate(CameraRef camera);
        void Deactivate(CameraRef camera);
        Vec3 GameCameraPosition { get; }
        Vec3 GameCameraRotation { get; }
        float GameCameraFov { get; }
    }
}
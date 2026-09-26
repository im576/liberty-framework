using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK ICameras. Scripted cameras are ScriptHookDotNet Camera objects (the autopilot's review camera proved the
    // create/activate/delete path in game), kept by handle. When the owner stops, its cameras are deactivated and
    // destroyed, which hands rendering back to the game camera.
    public sealed class CameraService : ICameras
    {
        private readonly LibertyEngine engine;
        private readonly Dictionary<int, Camera> cameras = new Dictionary<int, Camera>();

        internal CameraService(LibertyEngine engine) { this.engine = engine; }

        public CameraRef Create(LibertyModule owner)
        {
            Camera camera = new Camera();
            int handle = camera.GetHashCode();
            if (handle == 0) { RuntimeLog.Error("[" + owner.Id + "] camera_create_failed"); return CameraRef.None; }
            cameras[handle] = camera;
            engine.Ledger.Add(owner, "camera", handle, () => DestroyNow(handle));
            return new CameraRef(handle);
        }

        public void Destroy(CameraRef camera)
        {
            if (engine.Ledger.Has("camera", camera.Handle)) { engine.Ledger.Release("camera", camera.Handle); }
        }

        private void DestroyNow(int handle)
        {
            Camera camera;
            if (!cameras.TryGetValue(handle, out camera)) { return; }
            cameras.Remove(handle);
            // The game may already have destroyed it (cutscene, death camera); that is not an error.
            try { camera.Deactivate(); camera.Delete(); }
            catch (Exception error) { RuntimeLog.Info("camera_already_gone handle=" + handle + " " + error.Message); }
        }

        private Camera Get(CameraRef camera)
        {
            Camera value;
            return cameras.TryGetValue(camera.Handle, out value) ? value : null;
        }

        public void SetPosition(CameraRef camera, Vec3 position) { Camera c = Get(camera); if (c != null) { c.Position = Handles.V(position); } }

        public void SetRotation(CameraRef camera, Vec3 degrees) { Camera c = Get(camera); if (c != null) { c.Rotation = Handles.V(degrees); } }

        public void PointAt(CameraRef camera, Vec3 target) { Camera c = Get(camera); if (c != null) { c.LookAt(Handles.V(target)); } }

        public void PointAt(CameraRef camera, PedRef ped)
        {
            Camera c = Get(camera);
            Ped target = Handles.Ped(ped);
            if (c != null && target != null) { c.LookAt(target); }
        }

        public void SetFov(CameraRef camera, float degrees) { Camera c = Get(camera); if (c != null) { c.FOV = degrees; } }

        public void Activate(CameraRef camera) { Camera c = Get(camera); if (c != null) { c.Activate(); } }

        public void Deactivate(CameraRef camera) { Camera c = Get(camera); if (c != null) { c.Deactivate(); } }

        public Vec3 GameCameraPosition { get { return NativeCall.OutVector("GET_CAM_POS", GameCam()); } }

        public Vec3 GameCameraRotation { get { return NativeCall.OutVector("GET_CAM_ROT", GameCam()); } }

        public float GameCameraFov { get { return NativeCall.OutFloat("GET_CAM_FOV", GameCam()); } }

        private static int GameCam()
        {
            Pointer camera = typeof(int);
            Function.Call("GET_GAME_CAM", camera);
            return (int)camera;
        }
    }
}

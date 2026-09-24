using System;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Math3;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // The third-person aim camera (CCamAimWeapon, camera type 9). CCamAimWeapon::AimFree adds
    // stick/mouse input to its pitch and heading fields every frame, so adding a kick to the
    // same fields behaves exactly like extra player input: it persists and the player can
    // counter it. The fields are found through the same FindChild call SET_GAME_CAM_PITCH uses.
    // Before any write, the fields are validated against GET_CAM_ROT of the final game camera.
    internal sealed class AimCamera
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr FindChildFunction(IntPtr camera, int type, int index);

        private const double RadiansToDegrees = 180.0 / Math.PI;
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;
        private readonly FindChildFunction findChild;
        private int goodSamples;
        private int badSamples;
        private int pitchSign = 1;

        internal AimCamera(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
            findChild = (FindChildFunction)Marshal.GetDelegateForFunctionPointer(
                new IntPtr((int)addresses.FindChildCamFunction), typeof(FindChildFunction));
        }

        internal bool Validated { get; private set; }
        internal bool Rejected { get; private set; }
        internal string ValidationDetail { get; private set; }

        internal void ResetValidation()
        {
            goodSamples = 0;
            badSamples = 0;
            Validated = false;
            Rejected = false;
            ValidationDetail = "pending";
        }

        // Returns the active aim camera object or 0 when the player is not in the aim camera.
        internal uint FindActive(int gameCameraHandle)
        {
            uint gameCamera = CameraFromHandle(gameCameraHandle);
            if (gameCamera == 0) { return 0; }
            uint child = (uint)findChild(new IntPtr((int)gameCamera), addresses.AimCamType, 0).ToInt32();
            if (child == 0) { return 0; }
            int end = Math.Max(addresses.AimCamPitchOffset, addresses.AimCamHeadingOffset) + 4;
            return memory.IsReadable(child, end) && memory.IsWritable(child + (uint)addresses.AimCamPitchOffset, 8) ? child : 0;
        }

        internal uint CameraFromHandle(int handle)
        {
            if (handle == 0) { return 0; }
            uint pool = memory.TryReadPointer(addresses.CamPoolGlobal);
            if (pool == 0 || !memory.IsReadable(pool, 16)) { return 0; }
            uint objects = memory.ReadUInt32(pool);
            uint flags = memory.ReadUInt32(pool + 4);
            int size = memory.ReadInt32(pool + 8);
            int itemSize = memory.ReadInt32(pool + 12);
            int index = handle >> 8;
            if (index < 0 || index >= size || itemSize <= 0 || !memory.IsReadable(flags + (uint)index, 1)) { return 0; }
            byte flag = memory.ReadByte(flags + (uint)index);
            if ((flag & 0x80) != 0 || flag != (handle & 0xFF)) { return 0; }
            uint camera = objects + (uint)(index * itemSize);
            return memory.IsReadable(camera, 4) ? camera : 0;
        }

        internal double PitchDegrees(uint camera)
        {
            return memory.ReadSingle(camera + (uint)addresses.AimCamPitchOffset) * RadiansToDegrees * pitchSign;
        }

        internal double HeadingDegrees(uint camera)
        {
            return memory.ReadSingle(camera + (uint)addresses.AimCamHeadingOffset) * RadiansToDegrees;
        }

        // Compares the aim fields with the rendered camera rotation (degrees) for this frame.
        internal void Sample(uint camera, Vec3 renderedRotation, double toleranceDegrees, int requiredSamples)
        {
            if (Validated || Rejected) { return; }
            double rawPitch = memory.ReadSingle(camera + (uint)addresses.AimCamPitchOffset) * RadiansToDegrees;
            double heading = HeadingDegrees(camera);
            double headingError = Math.Abs(WrapDegrees(heading - renderedRotation.Z));
            double pitchError = Math.Abs(rawPitch - renderedRotation.X);
            double invertedPitchError = Math.Abs(-rawPitch - renderedRotation.X);
            bool headingOk = headingError <= toleranceDegrees;
            if (headingOk && pitchError <= toleranceDegrees) { goodSamples++; pitchSign = 1; }
            else if (headingOk && invertedPitchError <= toleranceDegrees && Math.Abs(rawPitch) > toleranceDegrees) { goodSamples++; pitchSign = -1; }
            else { badSamples++; }
            ValidationDetail = "field_pitch=" + rawPitch.ToString("0.00") + " field_heading=" + heading.ToString("0.00") +
                " cam_rot_x=" + renderedRotation.X.ToString("0.00") + " cam_rot_z=" + renderedRotation.Z.ToString("0.00") +
                " good=" + goodSamples + " bad=" + badSamples + " pitch_sign=" + pitchSign;
            if (goodSamples >= requiredSamples)
            {
                Validated = true;
                RuntimeLog.Info("aimcam_validated " + ValidationDetail);
            }
            else if (badSamples >= requiredSamples * 6)
            {
                Rejected = true;
                RuntimeLog.Error("aimcam_validation_failed camera kick disabled " + ValidationDetail);
            }
        }

        internal void AddDegrees(uint camera, double pitchDegrees, double headingDegrees)
        {
            if (!Validated) { return; }
            uint pitchAddress = camera + (uint)addresses.AimCamPitchOffset;
            uint headingAddress = camera + (uint)addresses.AimCamHeadingOffset;
            float pitch = memory.ReadSingle(pitchAddress);
            float heading = memory.ReadSingle(headingAddress);
            memory.WriteSingle(pitchAddress, (float)(pitch + pitchSign * pitchDegrees / RadiansToDegrees));
            // Not wrapped: the game normalises the heading itself, and wrapping here could make its
            // camera smoothing interpolate the long way round.
            memory.WriteSingle(headingAddress, (float)(heading + headingDegrees / RadiansToDegrees));
        }

        internal static double WrapDegrees(double value)
        {
            value %= 360.0;
            if (value > 180.0) { value -= 360.0; }
            if (value < -180.0) { value += 360.0; }
            return value;
        }
    }
}

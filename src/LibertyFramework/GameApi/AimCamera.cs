using System;
using System.Collections.Generic;
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
        private readonly int cameraType;
        private readonly int pitchOffset;
        private readonly int headingOffset;
        private readonly string name;
        private int badSamples;
        private int pitchSign = 1;
        private int headingSign = 1;
        private readonly List<double[]> samples = new List<double[]>();

        internal AimCamera(LiveMemory memory, GameAddresses addresses, int cameraType, int pitchOffset, int headingOffset, string name)
        {
            this.memory = memory;
            this.addresses = addresses;
            this.cameraType = cameraType;
            this.pitchOffset = pitchOffset;
            this.headingOffset = headingOffset;
            this.name = name;
            findChild = (FindChildFunction)Marshal.GetDelegateForFunctionPointer(
                new IntPtr((int)addresses.FindChildCamFunction), typeof(FindChildFunction));
        }

        internal string Name { get { return name; } }
        internal bool Validated { get; private set; }
        internal bool Rejected { get; private set; }
        internal string ValidationDetail { get; private set; }

        internal void ResetValidation()
        {
            samples.Clear();
            pitchSign = 1;
            headingSign = 1;
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
            uint child = (uint)findChild(new IntPtr((int)gameCamera), cameraType, 0).ToInt32();
            if (child == 0) { return 0; }
            int end = Math.Max(pitchOffset, headingOffset) + 4;
            return memory.IsReadable(child, end) && memory.IsWritable(child + (uint)pitchOffset, 8) ? child : 0;
        }

        // T-026: the rage pool (header, object array, flag array) is allocated once and never freed; after one full
        // check its ranges skip VirtualQuery (see LiveMemory.Trust).
        private uint trustedPool;

        private void TrustPool(uint pool, uint objects, uint flags, int size, int itemSize)
        {
            if (pool == trustedPool || size <= 0 || itemSize <= 0 || size > 0x10000 || itemSize > 0x10000) { return; }
            if (memory.Trust(pool, 16, false) && memory.Trust(objects, size * itemSize, true) && memory.Trust(flags, size, false)) { trustedPool = pool; }
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
            TrustPool(pool, objects, flags, size, itemSize);
            int index = handle >> 8;
            if (index < 0 || index >= size || itemSize <= 0 || !memory.IsReadable(flags + (uint)index, 1)) { return 0; }
            byte flag = memory.ReadByte(flags + (uint)index);
            if ((flag & 0x80) != 0 || flag != (handle & 0xFF)) { return 0; }
            uint camera = objects + (uint)(index * itemSize);
            return memory.IsReadable(camera, 4) ? camera : 0;
        }

        internal double PitchDegrees(uint camera)
        {
            return memory.ReadSingle(camera + (uint)pitchOffset) * RadiansToDegrees * pitchSign;
        }

        internal double HeadingDegrees(uint camera)
        {
            return memory.ReadSingle(camera + (uint)headingOffset) * RadiansToDegrees;
        }

        // Compares the aim fields with the rendered camera rotation (degrees). Accepts a sign flip on
        // either axis and, once the player has turned at least 20 degrees, a constant heading offset:
        // the kick only needs the fields to move with the camera, not a particular convention.
        internal void Sample(uint camera, Vec3 renderedRotation, double toleranceDegrees, int requiredSamples)
        {
            if (Validated || Rejected) { return; }
            double[] sample = {
                memory.ReadSingle(camera + (uint)pitchOffset) * RadiansToDegrees,
                memory.ReadSingle(camera + (uint)headingOffset) * RadiansToDegrees,
                renderedRotation.X, renderedRotation.Z };
            samples.Add(sample);
            if (samples.Count > requiredSamples * 8) { samples.RemoveAt(0); }
            ValidationDetail = "field_pitch=" + sample[0].ToString("0.00") + " field_heading=" + sample[1].ToString("0.00") +
                " cam_rot_x=" + sample[2].ToString("0.00") + " cam_rot_z=" + sample[3].ToString("0.00") + " samples=" + samples.Count;
            if (samples.Count < requiredSamples) { return; }

            List<double[]> recent = samples.GetRange(samples.Count - requiredSamples, requiredSamples);
            double headingRange = 0;
            foreach (double[] a in samples) { foreach (double[] b in samples) { headingRange = Math.Max(headingRange, Math.Abs(WrapDegrees(a[3] - b[3]))); } }
            foreach (int candidateHeadingSign in new[] { 1, -1 })
            {
                foreach (int candidatePitchSign in new[] { 1, -1 })
                {
                    double maxPitchError = 0;
                    double minDelta = double.MaxValue;
                    double maxDelta = double.MinValue;
                    double reference = WrapDegrees(candidateHeadingSign * recent[0][1] - recent[0][3]);
                    foreach (double[] s in (headingRange >= 20 ? samples : recent))
                    {
                        maxPitchError = Math.Max(maxPitchError, Math.Abs(candidatePitchSign * s[0] - s[2]));
                        double delta = WrapDegrees(WrapDegrees(candidateHeadingSign * s[1] - s[3]) - reference);
                        minDelta = Math.Min(minDelta, delta);
                        maxDelta = Math.Max(maxDelta, delta);
                    }
                    bool pitchOk = maxPitchError <= toleranceDegrees;
                    bool headingConsistent = maxDelta - minDelta <= toleranceDegrees;
                    bool headingDirect = Math.Abs(reference) <= toleranceDegrees;
                    // A pitch sign can only be told apart when the player is not looking level.
                    bool pitchInformative = candidatePitchSign == 1 || Math.Abs(recent[0][0]) > toleranceDegrees;
                    if (pitchOk && pitchInformative && headingConsistent && (headingDirect || headingRange >= 20))
                    {
                        pitchSign = candidatePitchSign;
                        headingSign = candidateHeadingSign;
                        Validated = true;
                        RuntimeLog.Info("aimcam_validated camera=" + name + " " + ValidationDetail + " pitch_sign=" + pitchSign + " heading_sign=" + headingSign +
                            " heading_offset=" + reference.ToString("0.00") + " heading_range=" + headingRange.ToString("0.0"));
                        return;
                    }
                }
            }
            badSamples++;
            if (badSamples >= requiredSamples * 12)
            {
                Rejected = true;
                RuntimeLog.Error("aimcam_validation_failed camera=" + name + " kick disabled for this camera " + ValidationDetail + " heading_range=" + headingRange.ToString("0.0"));
            }
        }
        internal void AddDegrees(uint camera, double pitchDegrees, double headingDegrees)
        {
            if (!Validated) { return; }
            uint pitchAddress = camera + (uint)pitchOffset;
            uint headingAddress = camera + (uint)headingOffset;
            float pitch = memory.ReadSingle(pitchAddress);
            float heading = memory.ReadSingle(headingAddress);
            memory.WriteSingle(pitchAddress, (float)(pitch + pitchSign * pitchDegrees / RadiansToDegrees));
            // Not wrapped: the game normalises the heading itself, and wrapping here could make its
            // camera smoothing interpolate the long way round.
            // GTA headings grow counter-clockwise (to the left); callers pass positive = turn right.
            memory.WriteSingle(headingAddress, (float)(heading - headingSign * headingDegrees / RadiansToDegrees));
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace LibertyFramework.Content
{
    // glTF 2.0 reader (.gltf + .bin/data URIs, or .glb): the JSON tree plus typed accessor reads. Only what the compiler
    // needs: scenes/nodes/meshes/accessors/bufferViews/buffers/materials/textures/images and extras.
    internal sealed class GltfDocument
    {
        internal readonly Dictionary<string, object> Root;
        private readonly List<byte[]> buffers = new List<byte[]>();
        private readonly string directory;

        private GltfDocument(Dictionary<string, object> root, byte[] glbBinary, string directory)
        {
            Root = root;
            this.directory = directory;
            foreach (Dictionary<string, object> buffer in Array("buffers"))
            {
                string uri = Str(buffer, "uri");
                if (uri == null) { buffers.Add(glbBinary ?? throw new InvalidDataException("buffer without uri outside a .glb")); continue; }
                buffers.Add(ReadUri(uri));
            }
        }

        internal static GltfDocument Load(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            string directory = Path.GetDirectoryName(Path.GetFullPath(path));
            JavaScriptSerializer json = new JavaScriptSerializer();
            json.MaxJsonLength = int.MaxValue;
            if (bytes.Length >= 12 && bytes[0] == (byte)'g' && bytes[1] == (byte)'l' && bytes[2] == (byte)'T' && bytes[3] == (byte)'F')
            {
                if (BitConverter.ToUInt32(bytes, 4) != 2) { throw new InvalidDataException("GLB version is not 2"); }
                int at = 12;
                string text = null;
                byte[] binary = null;
                while (at + 8 <= bytes.Length)
                {
                    int length = BitConverter.ToInt32(bytes, at);
                    uint type = BitConverter.ToUInt32(bytes, at + 4);
                    if (type == 0x4E4F534A) { text = Encoding.UTF8.GetString(bytes, at + 8, length); }
                    else if (type == 0x004E4942) { binary = new byte[length]; Buffer.BlockCopy(bytes, at + 8, binary, 0, length); }
                    at += 8 + length;
                }
                if (text == null) { throw new InvalidDataException("GLB has no JSON chunk"); }
                return new GltfDocument((Dictionary<string, object>)json.DeserializeObject(text), binary, directory);
            }
            return new GltfDocument((Dictionary<string, object>)json.DeserializeObject(Encoding.UTF8.GetString(bytes)), null, directory);
        }

        internal byte[] ReadUri(string uri)
        {
            if (uri.StartsWith("data:", StringComparison.Ordinal))
            {
                int comma = uri.IndexOf(',');
                if (comma < 0 || !uri.Substring(0, comma).EndsWith(";base64", StringComparison.Ordinal)) { throw new InvalidDataException("unsupported data URI"); }
                return Convert.FromBase64String(uri.Substring(comma + 1));
            }
            string path = Path.Combine(directory, Uri.UnescapeDataString(uri));
            if (!File.Exists(path)) { throw new FileNotFoundException("glTF resource missing: " + uri, path); }
            return File.ReadAllBytes(path);
        }

        // ---- JSON helpers ----

        internal IList<Dictionary<string, object>> Array(string name) { return Array(Root, name); }

        internal static IList<Dictionary<string, object>> Array(Dictionary<string, object> node, string name)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            object value;
            if (node == null || !node.TryGetValue(name, out value) || !(value is IList)) { return result; }
            foreach (object item in (IList)value) { result.Add(item as Dictionary<string, object>); }
            return result;
        }

        internal static Dictionary<string, object> Obj(Dictionary<string, object> node, string name)
        {
            object value;
            return node != null && node.TryGetValue(name, out value) ? value as Dictionary<string, object> : null;
        }

        internal static string Str(Dictionary<string, object> node, string name)
        {
            object value;
            return node != null && node.TryGetValue(name, out value) ? value as string : null;
        }

        internal static int Int(Dictionary<string, object> node, string name, int fallback)
        {
            object value;
            return node != null && node.TryGetValue(name, out value) && value != null ? Convert.ToInt32(value) : fallback;
        }

        internal static double[] Numbers(Dictionary<string, object> node, string name)
        {
            object value;
            if (node == null || !node.TryGetValue(name, out value) || !(value is IList)) { return null; }
            IList list = (IList)value;
            double[] result = new double[list.Count];
            for (int i = 0; i < list.Count; i++) { result[i] = Convert.ToDouble(list[i]); }
            return result;
        }

        // ---- accessors ----

        internal static int ComponentCount(string type)
        {
            switch (type)
            {
                case "SCALAR": return 1;
                case "VEC2": return 2;
                case "VEC3": return 3;
                case "VEC4": return 4;
                case "MAT4": return 16;
                default: throw new InvalidDataException("unsupported accessor type " + type);
            }
        }

        // Reads accessor 'index' as floats (count x components), applying the normalized rule for integer types.
        internal float[] ReadFloats(int index, out int components)
        {
            Dictionary<string, object> accessor = Array("accessors")[index];
            int count = Int(accessor, "count", 0), componentType = Int(accessor, "componentType", 5126);
            components = ComponentCount(Str(accessor, "type"));
            bool normalized = accessor.ContainsKey("normalized") && (bool)accessor["normalized"];
            float[] values = new float[count * components];
            if (!accessor.ContainsKey("bufferView")) { return values; } // sparse-only / zero-filled accessor
            byte[] data; int offset, stride;
            Locate(accessor, ComponentSize(componentType) * components, out data, out offset, out stride);
            for (int i = 0; i < count; i++)
            {
                for (int c = 0; c < components; c++)
                {
                    int at = offset + i * stride + c * ComponentSize(componentType);
                    values[i * components + c] = Component(data, at, componentType, normalized);
                }
            }
            return values;
        }

        internal int[] ReadIndices(int index)
        {
            Dictionary<string, object> accessor = Array("accessors")[index];
            int count = Int(accessor, "count", 0), componentType = Int(accessor, "componentType", 5125);
            byte[] data; int offset, stride;
            Locate(accessor, ComponentSize(componentType), out data, out offset, out stride);
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                int at = offset + i * stride;
                switch (componentType)
                {
                    case 5121: result[i] = data[at]; break;
                    case 5123: result[i] = BitConverter.ToUInt16(data, at); break;
                    case 5125: result[i] = (int)BitConverter.ToUInt32(data, at); break;
                    default: throw new InvalidDataException("unsupported index component type " + componentType);
                }
            }
            return result;
        }

        internal byte[] ReadBufferView(int index)
        {
            Dictionary<string, object> view = Array("bufferViews")[index];
            byte[] buffer = buffers[Int(view, "buffer", 0)];
            int offset = Int(view, "byteOffset", 0), length = Int(view, "byteLength", 0);
            byte[] result = new byte[length];
            Buffer.BlockCopy(buffer, offset, result, 0, length);
            return result;
        }

        private void Locate(Dictionary<string, object> accessor, int elementSize, out byte[] data, out int offset, out int stride)
        {
            Dictionary<string, object> view = Array("bufferViews")[Int(accessor, "bufferView", 0)];
            data = buffers[Int(view, "buffer", 0)];
            offset = Int(view, "byteOffset", 0) + Int(accessor, "byteOffset", 0);
            stride = Int(view, "byteStride", 0);
            if (stride == 0) { stride = elementSize; }
        }

        private static int ComponentSize(int componentType)
        {
            switch (componentType)
            {
                case 5120: case 5121: return 1;
                case 5122: case 5123: return 2;
                case 5125: case 5126: return 4;
                default: throw new InvalidDataException("unsupported component type " + componentType);
            }
        }

        private static float Component(byte[] data, int at, int componentType, bool normalized)
        {
            switch (componentType)
            {
                case 5126: return BitConverter.ToSingle(data, at);
                case 5121: return normalized ? data[at] / 255f : data[at];
                case 5120: return normalized ? Math.Max((sbyte)data[at] / 127f, -1f) : (sbyte)data[at];
                case 5123: return normalized ? BitConverter.ToUInt16(data, at) / 65535f : BitConverter.ToUInt16(data, at);
                case 5122: return normalized ? Math.Max(BitConverter.ToInt16(data, at) / 32767f, -1f) : BitConverter.ToInt16(data, at);
                case 5125: return BitConverter.ToUInt32(data, at);
                default: throw new InvalidDataException("unsupported component type " + componentType);
            }
        }
    }
}

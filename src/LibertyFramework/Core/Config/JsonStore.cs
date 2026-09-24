using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace LibertyFramework.Core.Config
{
    // Small DataContract JSON helper: bounded reads, content hashing for change detection,
    // and atomic indented writes that keep one .bak of the replaced file.
    internal static class JsonStore
    {
        internal const int MaximumBytes = 262144;

        internal static byte[] ReadBytes(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (stream.Length > MaximumBytes) { throw new InvalidDataException(Path.GetFileName(path) + " exceeds " + MaximumBytes + " bytes."); }
                byte[] data = new byte[(int)stream.Length];
                int position = 0;
                while (position < data.Length)
                {
                    int count = stream.Read(data, position, data.Length - position);
                    if (count == 0) { throw new EndOfStreamException("File changed during read."); }
                    position += count;
                }
                return data;
            }
        }

        internal static string Hash(byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "");
            }
        }

        internal static T Parse<T>(byte[] data) where T : class
        {
            // Strip a UTF-8 byte order mark; DataContractJsonSerializer rejects it.
            int offset = data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF ? 3 : 0;
            using (MemoryStream stream = new MemoryStream(data, offset, data.Length - offset, false))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                T result;
                try
                {
                    result = serializer.ReadObject(stream) as T;
                }
                catch (SerializationException error)
                {
                    throw new InvalidDataException("JSON error: " + error.Message, error);
                }
                if (result == null) { throw new InvalidDataException("JSON root is not a " + typeof(T).Name + "."); }
                return result;
            }
        }

        internal static T Load<T>(string path) where T : class
        {
            return Parse<T>(ReadBytes(path));
        }

        internal static void Save<T>(string path, T value)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) { Directory.CreateDirectory(directory); }
            string temporary = path + ".tmp";
            using (FileStream stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
            using (XmlDictionaryWriterHolder holder = new XmlDictionaryWriterHolder(stream))
            {
                new DataContractJsonSerializer(typeof(T)).WriteObject(holder.Writer, value);
                holder.Writer.Flush();
            }
            if (File.Exists(path))
            {
                string backup = path + ".bak";
                if (File.Exists(backup)) { File.Delete(backup); }
                File.Replace(temporary, path, backup);
            }
            else
            {
                File.Move(temporary, path);
            }
        }

        private sealed class XmlDictionaryWriterHolder : IDisposable
        {
            internal readonly System.Xml.XmlDictionaryWriter Writer;

            internal XmlDictionaryWriterHolder(Stream stream)
            {
                Writer = JsonReaderWriterFactory.CreateJsonWriter(stream, new UTF8Encoding(false), false, true, "  ");
            }

            public void Dispose()
            {
                Writer.Close();
            }
        }
    }
}

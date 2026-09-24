using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Core.Config
{
    internal sealed class ProbeConfigLoader
    {
        private const int MaximumConfigBytes = 65536;
        private readonly string configPath;
        private string lastAttemptedHash;
        private ProbeConfig activeConfig;

        internal ProbeConfigLoader()
        {
            string gameDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            configPath = Path.Combine(gameDirectory, "scripts", "LibertyFramework", "config", "probe.json");
        }

        internal void Poll()
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    if (lastAttemptedHash != "missing")
                    {
                        lastAttemptedHash = "missing";
                        RuntimeLog.Error("config_missing path=" + configPath + " retained_label=" + ActiveLabel);
                    }
                    return;
                }

                byte[] data;
                using (FileStream stream = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (stream.Length > MaximumConfigBytes)
                    {
                        throw new InvalidDataException("Config exceeds 65536 bytes.");
                    }
                    data = new byte[(int)stream.Length];
                    int position = 0;
                    while (position < data.Length)
                    {
                        int count = stream.Read(data, position, data.Length - position);
                        if (count == 0) { throw new EndOfStreamException("Config changed during read."); }
                        position += count;
                    }
                }

                string hash;
                using (SHA256 sha = SHA256.Create())
                {
                    hash = BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "");
                }
                if (hash == lastAttemptedHash) { return; }
                lastAttemptedHash = hash;

                ProbeConfig candidate;
                using (MemoryStream stream = new MemoryStream(data, false))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ProbeConfig));
                    candidate = serializer.ReadObject(stream) as ProbeConfig;
                    if (stream.Position != stream.Length)
                    {
                        throw new InvalidDataException("Unexpected data after JSON object.");
                    }
                }
                if (candidate == null || candidate.SchemaVersion != 1)
                {
                    throw new InvalidDataException("schemaVersion must be 1.");
                }
                if (candidate.ProbeLabel == null ||
                    !Regex.IsMatch(candidate.ProbeLabel, "^[A-Za-z0-9_-]{1,64}$"))
                {
                    throw new InvalidDataException("probeLabel must be 1-64 letters, digits, underscores, or hyphens.");
                }

                activeConfig = candidate;
                RuntimeLog.Info("config_loaded path=" + configPath + " probe_label=" + ActiveLabel);
            }
            catch (Exception error)
            {
                RuntimeLog.Error("config_reload_failed path=" + configPath + " retained_label=" + ActiveLabel + " error=" + error);
            }
        }

        internal string ActiveLabel
        {
            get { return activeConfig == null ? "none" : activeConfig.ProbeLabel; }
        }
    }
}

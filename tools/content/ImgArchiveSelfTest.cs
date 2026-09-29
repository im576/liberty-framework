using System;
using System.IO;
using System.Security.Cryptography;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    internal static class ImgArchiveSelfTest
    {
        internal static void Run(SelfTest.Runner t, string output)
        {
            byte[] key = new byte[32];
            for (int i = 0; i < key.Length; i++) { key[i] = (byte)i; }
            t.Check(ImgArchive.Decrypt(new byte[0], key).Length == 0, "empty encrypted table requires no AES blocks");

            // The first 16 bytes of a v3 header are encrypted; count and table size are both zero.
            // This is synthetic data under a test key, not bytes from the owner's archives.
            byte[] header = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(0xA94E2A52u), 0, header, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(3), 0, header, 4, 4);
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Mode = CipherMode.ECB; aes.Padding = PaddingMode.None; aes.KeySize = 256; aes.Key = key;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    for (int round = 0; round < 16; round++) { encryptor.TransformBlock(header, 0, header.Length, header, 0); }
                }
            }
            byte[] archive = new byte[2048];
            Buffer.BlockCopy(header, 0, archive, 0, header.Length);
            archive[16] = 16; archive[18] = 0xE9;
            string path = Path.Combine(Path.GetTempPath(), "liberty-empty-img-" + Guid.NewGuid().ToString("N") + ".img");
            File.WriteAllBytes(path, archive);
            try { t.Check(ImgArchive.Open(path, key).Entries.Count == 0, "encrypted zero-entry IMG opens with no table"); }
            finally { File.Delete(path); }
        }
    }
}

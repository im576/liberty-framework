using System;

namespace Liberty.Sdk
{
    // A model by name or hash. GTA IV model hashes are Jenkins one-at-a-time over the lower-case name.
    public struct ModelRef : IEquatable<ModelRef>
    {
        public readonly int Hash;
        public readonly string Name;

        private ModelRef(int hash, string name) { Hash = hash; Name = name; }

        public static ModelRef FromName(string name) { return new ModelRef(HashOf(name), name); }
        public static ModelRef FromHash(int hash) { return new ModelRef(hash, null); }
        public static implicit operator ModelRef(string name) { return FromName(name); }

        public bool IsNone { get { return Hash == 0; } }

        public static int HashOf(string text)
        {
            if (string.IsNullOrEmpty(text)) { return 0; }
            uint hash = 0;
            foreach (char c in text.ToLowerInvariant())
            {
                hash += (byte)c;
                hash += hash << 10;
                hash ^= hash >> 6;
            }
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return unchecked((int)hash);
        }

        public bool Equals(ModelRef other) { return Hash == other.Hash; }
        public override bool Equals(object obj) { return obj is ModelRef && Equals((ModelRef)obj); }
        public override int GetHashCode() { return Hash; }
        public override string ToString() { return Name ?? ("0x" + Hash.ToString("X8")); }
    }
}
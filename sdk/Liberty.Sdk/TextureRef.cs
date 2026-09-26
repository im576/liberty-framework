using System;

namespace Liberty.Sdk
{
    // A texture loaded for UI drawing (cached by the engine). Value type; compare with == or IsNone.
    public struct TextureRef : IEquatable<TextureRef>
    {
        public readonly int Handle;
        public TextureRef(int handle) { Handle = handle; }
        public static readonly TextureRef None = new TextureRef(0);
        public bool IsNone { get { return Handle == 0; } }
        public static bool operator ==(TextureRef a, TextureRef b) { return a.Handle == b.Handle; }
        public static bool operator !=(TextureRef a, TextureRef b) { return a.Handle != b.Handle; }
        public bool Equals(TextureRef other) { return Handle == other.Handle; }
        public override bool Equals(object obj) { return obj is TextureRef && Equals((TextureRef)obj); }
        public override int GetHashCode() { return Handle; }
        public override string ToString() { return "TextureRef(" + Handle + ")"; }
    }
}
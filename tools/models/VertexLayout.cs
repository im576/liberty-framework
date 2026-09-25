using System;
using System.Collections.Generic;

namespace LibertyFramework.Models
{
    // grcVertexDeclaration: usage mask (bit per semantic), stride, element count, and a 64-bit field holding a 4-bit
    // type code per semantic. Elements are packed in semantic order. Semantics:
    //   0 position, 1 blend weight, 2 blend indices, 3 normal, 4 colour, 5 specular, 6-13 texcoord 0-7, 14 tangent, 15 binormal.
    // Type sizes are only those confirmed by stride sums across the game's drawables (see docs/research/ModelFormat.md);
    // any other code is rejected rather than guessed.
    internal sealed class VertexLayout
    {
        internal enum Semantic { Position = 0, BlendWeight = 1, BlendIndices = 2, Normal = 3, Colour = 4, Specular = 5, TexCoord0 = 6, Tangent = 14, Binormal = 15 }

        private static readonly Dictionary<int, int> TypeSizes = new Dictionary<int, int>
        {
            { 5, 8 },   // float2
            { 6, 12 },  // float3
            { 7, 16 },  // float4
            { 9, 4 },   // 4 x ubyte (D3DCOLOR / packed indices / packed weights)
        };

        internal readonly uint Mask;
        internal readonly int Stride;
        internal readonly int ElementCount;
        internal readonly ulong Types;
        private readonly int[] offsets = new int[16];
        private readonly int[] sizes = new int[16];

        internal VertexLayout(uint mask, int stride, int elementCount, ulong types)
        {
            Mask = mask; Stride = stride; ElementCount = elementCount; Types = types;
            int offset = 0, count = 0;
            for (int semantic = 0; semantic < 16; semantic++)
            {
                offsets[semantic] = -1;
                if ((mask & (1u << semantic)) == 0) { continue; }
                int code = TypeCode(semantic);
                int size;
                if (!TypeSizes.TryGetValue(code, out size)) { throw ResourceView.Bad("unknown vertex type code " + code + " for semantic " + semantic); }
                offsets[semantic] = offset; sizes[semantic] = size;
                offset += size; count++;
            }
            if (offset != stride) { throw ResourceView.Bad("vertex elements sum to " + offset + " but stride is " + stride); }
            if (count != elementCount) { throw ResourceView.Bad("mask has " + count + " elements but declaration says " + elementCount); }
        }

        internal int TypeCode(int semantic) { return (int)((Types >> (semantic * 4)) & 0xF); }
        internal bool Has(Semantic semantic) { return offsets[(int)semantic] >= 0; }
        internal bool Has(int semantic) { return offsets[semantic] >= 0; }
        internal int OffsetOf(int semantic) { return offsets[semantic]; }
        internal int SizeOf(int semantic) { return sizes[semantic]; }

        internal string Describe()
        {
            List<string> parts = new List<string>();
            for (int semantic = 0; semantic < 16; semantic++)
            {
                if (offsets[semantic] >= 0) { parts.Add(semantic + ":" + TypeCode(semantic)); }
            }
            return "mask=0x" + Mask.ToString("X") + " stride=" + Stride + " [" + string.Join(" ", parts.ToArray()) + "]";
        }
    }
}

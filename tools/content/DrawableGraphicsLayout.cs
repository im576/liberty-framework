using System;
using System.Collections.Generic;
using System.Linq;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Reports modeled buffer ranges and the ranges between them, without exporting graphics bytes.
    // An unmodeled range can contain padding or another allocation; its byte counts do not establish its meaning.
    internal static class DrawableGraphicsLayout
    {
        private sealed class BufferRange
        {
            internal int Geometry, Offset, Length, RebuiltOffset;
            internal string Kind;
        }

        internal static Dictionary<string, object> Describe(RscResource source, DrawableStructureBuilder.Output rebuilt)
        {
            List<BufferRange> buffers = new List<BufferRange>();
            int index = 0;
            foreach (DrawableStructureBuilder.Placement p in rebuilt.Placements)
            {
                DrawableGeometry g = p.Geometry;
                buffers.Add(new BufferRange { Geometry = index, Kind = "vertices", Offset = (int)(g.VertexData - ResourceView.GraphicsBase),
                    Length = g.VertexCount * g.Layout.Stride, RebuiltOffset = p.VertexOffset });
                buffers.Add(new BufferRange { Geometry = index, Kind = "indices", Offset = (int)(g.IndexData - ResourceView.GraphicsBase),
                    Length = g.IndexCount * 2, RebuiltOffset = p.IndexOffset });
                index++;
            }
            List<object> ranges = new List<object>(), gaps = new List<object>();
            int coveredEnd = 0;
            foreach (BufferRange buffer in buffers.OrderBy(b => b.Offset))
            {
                if (buffer.Offset < 0 || buffer.Length < 0 || (long)buffer.Offset + buffer.Length > source.GraphicsSize)
                { throw new System.IO.InvalidDataException("graphics layout range outside source segment"); }
                if (buffer.Offset > coveredEnd) { gaps.Add(Gap(source, coveredEnd, buffer.Offset - coveredEnd)); }
                ranges.Add(new Dictionary<string, object> {
                    { "geometry", buffer.Geometry }, { "kind", buffer.Kind }, { "sourceOffset", buffer.Offset },
                    { "rebuiltOffset", buffer.RebuiltOffset }, { "bytes", buffer.Length },
                    { "overlapWithEarlierBytes", Math.Max(0, Math.Min(coveredEnd - buffer.Offset, buffer.Length)) }
                });
                coveredEnd = Math.Max(coveredEnd, buffer.Offset + buffer.Length);
            }
            if (coveredEnd < source.GraphicsSize) { gaps.Add(Gap(source, coveredEnd, source.GraphicsSize - coveredEnd)); }
            return new Dictionary<string, object> {
                { "source", Header(source) }, { "rebuilt", Header(rebuilt.Resource) },
                { "buffers", ranges }, { "unmodeledSourceRanges", gaps },
                { "rule", "ranges and aggregate byte classes only; unmodeled ranges are not assumed to be padding" }
            };
        }

        private static Dictionary<string, object> Header(RscResource resource)
        {
            int shift = (int)((resource.Flags >> 26) & 0xF);
            return new Dictionary<string, object> {
                { "flags", "0x" + resource.Flags.ToString("X8") }, { "graphicsBytes", resource.GraphicsSize },
                { "graphicsPageShift", shift }, { "graphicsPageBytes", 256 << shift },
                { "graphicsPageCount", (resource.Flags >> 15) & 0x7FF }
            };
        }

        private static Dictionary<string, object> Gap(RscResource source, int offset, int length)
        {
            int zero = 0, cd = 0;
            for (int i = 0; i < length; i++)
            {
                byte value = source.Body[source.SystemSize + offset + i];
                if (value == 0) { zero++; } else if (value == 0xCD) { cd++; }
            }
            return new Dictionary<string, object> {
                { "offset", offset }, { "bytes", length }, { "zeroBytes", zero },
                { "cdBytes", cd }, { "otherBytes", length - zero - cd }
            };
        }
    }
}

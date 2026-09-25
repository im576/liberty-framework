using System.Collections.Generic;

namespace LibertyFramework.Models
{
    // One grmModel (a set of geometries drawn with one bone/matrix) at a given LOD (0 = high).
    internal sealed class DrawableModel
    {
        internal uint Address;
        internal int Lod;
        internal uint Bounds;
        internal readonly List<DrawableGeometry> Geometries = new List<DrawableGeometry>();
    }
}

using System.Collections.Generic;

namespace LibertyFramework.Content
{
    // A collision shape authored with the asset: never drawn, format-neutral, in GTA IV space (metres, Z up). How the game
    // stores collision is not established yet (docs/research/Collision.md); a collision writer turns these into bounds.
    // Primitives are fitted to the source object: its local bounding box, placed by its world transform. Axes are the
    // object's local X, Y and Z as Blender shows them (unit vectors); a capsule runs along the local Z axis.
    internal sealed class ContentCollision
    {
        internal const string ShapeMesh = "mesh", ShapeBox = "box", ShapeSphere = "sphere", ShapeCapsule = "capsule";
        internal static readonly string[] Shapes = { ShapeMesh, ShapeBox, ShapeSphere, ShapeCapsule };

        internal string Name;
        internal string Shape;
        // Optional authored surface name (liberty_surface). What the game does with surfaces is not established; the
        // collision writer decides how it maps.
        internal string Surface;
        // Set when the node also carried a LOD tag, which collision ignores.
        internal bool HadLodTag;

        // Mesh: triangles in GTA space.
        internal readonly List<ContentVertex> Vertices = new List<ContentVertex>();
        internal readonly List<int> Indices = new List<int>();

        // Primitives.
        internal readonly float[] Centre = new float[3];
        internal readonly float[][] Axes = { new float[] { 1, 0, 0 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } };
        internal readonly float[] HalfExtents = new float[3];  // box: along Axes
        internal float Radius;                                  // sphere, capsule
        internal float Length;                                  // capsule: between the two hemisphere centres, along Axes[2]
        // Sphere and capsule: the fitted extents differed (radius is the largest), as a ratio of largest to smallest.
        internal float NonUniformRatio = 1;
        // Why the shape could not be fitted (a sheared transform, a capsule shorter than its diameter), else null.
        internal string Problem;

        internal bool IsPrimitive { get { return Shape != ShapeMesh; } }
    }
}

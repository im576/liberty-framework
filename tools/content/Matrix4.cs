using System;

namespace LibertyFramework.Content
{
    // 4x4 affine transform, row-major storage with column vectors (p' = M p), as glTF defines node transforms.
    internal struct Matrix4
    {
        private double[] m; // m[row * 4 + column]

        internal static Matrix4 Identity
        {
            get { Matrix4 r = new Matrix4(); r.m = new double[16]; r.m[0] = r.m[5] = r.m[10] = r.m[15] = 1; return r; }
        }

        // glTF "matrix" is column-major.
        internal static Matrix4 FromColumnMajor(double[] values)
        {
            Matrix4 r = new Matrix4(); r.m = new double[16];
            for (int column = 0; column < 4; column++) { for (int row = 0; row < 4; row++) { r.m[row * 4 + column] = values[column * 4 + row]; } }
            return r;
        }

        // T * R * S with rotation quaternion (x, y, z, w).
        internal static Matrix4 Compose(double[] t, double[] q, double[] s)
        {
            double x = q[0], y = q[1], z = q[2], w = q[3];
            Matrix4 r = new Matrix4(); r.m = new double[16];
            r.m[0] = (1 - 2 * (y * y + z * z)) * s[0]; r.m[1] = (2 * (x * y - z * w)) * s[1]; r.m[2] = (2 * (x * z + y * w)) * s[2]; r.m[3] = t[0];
            r.m[4] = (2 * (x * y + z * w)) * s[0]; r.m[5] = (1 - 2 * (x * x + z * z)) * s[1]; r.m[6] = (2 * (y * z - x * w)) * s[2]; r.m[7] = t[1];
            r.m[8] = (2 * (x * z - y * w)) * s[0]; r.m[9] = (2 * (y * z + x * w)) * s[1]; r.m[10] = (1 - 2 * (x * x + y * y)) * s[2]; r.m[11] = t[2];
            r.m[15] = 1;
            return r;
        }

        public static Matrix4 operator *(Matrix4 a, Matrix4 b)
        {
            Matrix4 r = new Matrix4(); r.m = new double[16];
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    double sum = 0;
                    for (int k = 0; k < 4; k++) { sum += a.m[row * 4 + k] * b.m[k * 4 + column]; }
                    r.m[row * 4 + column] = sum;
                }
            }
            return r;
        }

        internal double[] TransformPoint(double x, double y, double z)
        {
            return new[] { m[0] * x + m[1] * y + m[2] * z + m[3], m[4] * x + m[5] * y + m[6] * z + m[7], m[8] * x + m[9] * y + m[10] * z + m[11] };
        }

        internal double[] TransformDirection(double x, double y, double z)
        {
            return new[] { m[0] * x + m[1] * y + m[2] * z, m[4] * x + m[5] * y + m[6] * z, m[8] * x + m[9] * y + m[10] * z };
        }

        internal double Determinant3()
        {
            return m[0] * (m[5] * m[10] - m[6] * m[9]) - m[1] * (m[4] * m[10] - m[6] * m[8]) + m[2] * (m[4] * m[9] - m[5] * m[8]);
        }

        // Inverse transpose of the upper 3x3 (for normals under non-uniform scale).
        internal Matrix4 InverseTransposeLinear()
        {
            double det = Determinant3();
            Matrix4 r = Identity;
            if (Math.Abs(det) < 1e-12) { return r; }
            double inv = 1.0 / det;
            // Cofactor matrix divided by the determinant is the inverse transpose.
            r.m[0] = (m[5] * m[10] - m[6] * m[9]) * inv; r.m[1] = -(m[4] * m[10] - m[6] * m[8]) * inv; r.m[2] = (m[4] * m[9] - m[5] * m[8]) * inv;
            r.m[4] = -(m[1] * m[10] - m[2] * m[9]) * inv; r.m[5] = (m[0] * m[10] - m[2] * m[8]) * inv; r.m[6] = -(m[0] * m[9] - m[1] * m[8]) * inv;
            r.m[8] = (m[1] * m[6] - m[2] * m[5]) * inv; r.m[9] = -(m[0] * m[6] - m[2] * m[4]) * inv; r.m[10] = (m[0] * m[5] - m[1] * m[4]) * inv;
            return r;
        }
    }
}

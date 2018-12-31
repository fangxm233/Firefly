using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ShaderLib
{
    public class ShaderMath
    {
        public const float Pi = 3.1415926535897932384626433832795f;
        public const float TwoPi = 6.283185307179586476925286766559f;

        public static Vector3 GetReflection(Vector3 In, Vector3 Normal) => 2 * Vector3.Dot(In, Normal) * Normal - In;

        public static Vector4 ColorMul(Vector4 c1, Vector4 c2) => new Vector4(c1.X * c2.X, c1.Y * c2.Y, c1.Z * c2.Z, c1.W * c2.W);

        #region Range
        public static float Range(float v, float min, float max) => v <= min ? min :
            v >= max ? max : v;
        public static int Range(int v, int min, int max) => v <= min ? min :
            v >= max ? max : v;

        #endregion

        public static float Min(float d1, float d2) => d1 > d2 ? d2 : d1;

        public static float Max(float d1, float d2) => d1 > d2 ? d1 : d2;

        #region VecterMulMatrix
        public static Vector4 Mul(Vector4 v, Matrix4x4 m) => new Vector4(
        v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + v.W * m.M14,
        v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + v.W * m.M24,
        v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + v.W * m.M34,
        v.X * m.M41 + v.Y * m.M42 + v.Z * m.M43 + v.W * m.M44);

        public static Vector4 Mul(Matrix4x4 m, Vector4 v) => new Vector4(
                v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + v.W * m.M14,
                v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + v.W * m.M24,
                v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + v.W * m.M34,
                v.X * m.M41 + v.Y * m.M42 + v.Z * m.M43 + v.W * m.M44);

        public static Vector3 Mul(Vector3 v, Matrix4x4 m) => new Vector3(
                v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13,
                v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23,
                v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33);

        public static Vector3 Mul(Matrix4x4 m, Vector3 v) => new Vector3(
                v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13,
                v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23,
                v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33);

        #endregion
    }
}

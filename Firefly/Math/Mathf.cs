using System.Numerics;
using Firefly.Render;
using Firefly.Render.Structure;

namespace Firefly.Math
{
    class Mathf
    {
        public static float Range(float v, float min, float max) => v <= min ? min :
            v >= max ? max : v;

        public static float Min(float d1, float d2) => d1 > d2 ? d2 : d1;

        public static float Max(float d1, float d2) => d1 > d2 ? d1 : d2;

        public const float Pi = 3.1415926535897932384626433832795f;
        public const float TwoPi = 6.283185307179586476925286766559f;

        public static Matrix4x4 GetRotationMatrix(Vector3 v) => 
            Matrix4x4.CreateRotationY(v.Y) * Matrix4x4.CreateRotationX(v.X) * Matrix4x4.CreateRotationZ(v.Z);

        public static Vector3 MulVector3AndMatrix4x4(Vector3 v, Matrix4x4 matrix) => new Vector3(
                v.X * matrix.M11 + v.Y * matrix.M12 + v.Z * matrix.M13 + matrix.M14,
                v.X * matrix.M21 + v.Y * matrix.M22 + v.Z * matrix.M23 + matrix.M24,
                v.X * matrix.M31 + v.Y * matrix.M32 + v.Z * matrix.M33 + matrix.M34);

        #region Lerp

        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            if (t <= 0)
                return a;
            if (t >= 1)
                return b;
            return new Color32(a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) =>
            new Vector3(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t), Lerp(a.Z, b.Z, t));

        public static Vector2Int Lerp(Vector2Int a, Vector2Int b, float t) =>
            new Vector2Int(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        public static float Lerp(float a, float b, float t)
        {
            if (t <= 0)
                return a;
            if (t >= 1)
                return b;
            return b * t + (1 - t) * a;
        }
        public static int Lerp(int a, int b, float t)
        {
            if (t <= 0)
                return a;
            if (t >= 1)
                return b;
            return (int) ((b * t + (1 - t) * a) + 0.5);
        }
        public static Vertex Lerp(Vertex a, Vertex b, float t)
        {
            return new Vertex
            {
                Color = Lerp(a.Color, b.Color, t),
                Point = Lerp(a.Point, b.Point, t)
            };
        }
        public static VertexInt Lerp(VertexInt a, VertexInt b, float t)
        {
            return new VertexInt
            {
                Point = Lerp(a.Point, b.Point, t),
                Color = Lerp(a.Color, b.Color, t)
            };
        }

        #endregion
    }
}

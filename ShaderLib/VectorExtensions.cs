using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ShaderLib
{
    public static class VectorExtensions
    {
        public static Vector3 XYZ(this Vector4 v) => new Vector3(v.X, v.Y, v.Z);
        public static Vector4 XYZ1(this Vector3 v) => new Vector4(v.X, v.Y, v.Z, 1);
        public static Vector4 XYZ0(this Vector3 v) => new Vector4(v.X, v.Y, v.Z, 0);
        public static Vector2 XYZ(this Vector3 v) => new Vector2(v.X, v.Y);
    }
}

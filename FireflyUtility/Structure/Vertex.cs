using System.Drawing;
using System.Numerics;
using FireflyUtility.Math;
using ShaderLib;

namespace FireflyUtility.Structure
{
    public struct Vertex
    {
        public Vector2 UV;
        public Vector3 Point, Normal, Tangent, Bitangent;
        public Vector4 Color;

        public Vertex4 ToVertex4() => new Vertex4(UV, Point.XYZ1(), Color, Normal);
    }

    public struct Vertex4
    {
        public Vector2 UV;
        public Vector3 Normal;
        public Vector4 Color, Point;

        public Vertex4(Vector2 uV, Vector4 point, Vector4 color, Vector3 normal)
        {
            UV = uV;
            Point = point;
            Normal = normal;
            Color = color;
        }
    }

    public struct VertexInt
    {
        public Vector2Int Point;
        public Vector4 Color;

        public VertexInt(Vector2Int point, Vector4 color)
        {
            Point = point;
            Color = color;
        }

        public VertexInt(Vector4 point, Vector4 color)
        {
            Point = new Vector2Int((int)point.X, (int)point.Y);
            Color = color;
        }
    }
}
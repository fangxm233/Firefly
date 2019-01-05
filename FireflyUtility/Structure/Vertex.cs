using System.Drawing;
using System.Numerics;
using FireflyUtility.Math;
using ShaderLib;

namespace FireflyUtility.Structure
{
    public struct Vertex
    {
        public Vector2 UV;
        public Vector3 Point, Normal;
        public Vector4 Color;

        public Vertex(Vector3 point, Vector4 color, Vector3 normal)
        {
            Point = point;
            Color = color;
            Normal = normal;
            UV = new Vector2();
        }

        public Vertex(Vector3 point, Vector4 color)
        {
            Point = point;
            Color = color;
            Normal = new Vector3();
            UV = new Vector2();
        }

        public Vertex(Vector3 point, Vector2 uV)
        {
            Point = point;
            Color = new Vector4();
            Normal = new Vector3();
            UV = uV;
        }

        public Vertex(Vector3 point, Color color, Vector3 normal)
        {
            Point = point;
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
            Normal = normal;
            UV = new Vector2();
        }

        public Vertex(Vector3 point, Color color)
        {
            Point = point;
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
            Normal = new Vector3();
            UV = new Vector2();
        }

        public Vertex(Vector2 uV, Vector3 point, Vector4 color, Vector3 normal)
        {
            UV = uV;
            Point = point;
            Normal = normal;
            Color = color;
        }

        public Vertex(Vector2 uV, Vector3 point, Color color, Vector3 normal)
        {
            UV = uV;
            Point = point;
            Normal = normal;
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
        }

        public Vertex(Vector2 uV, Vector3 point, Vector4 color)
        {
            UV = uV;
            Point = point;
            Normal = new Vector3();
            Color = color;
        }

        public Vertex(Vector2 uV, Vector3 point, Color color)
        {
            UV = uV;
            Point = point;
            Normal = new Vector3();
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
        }

        public Vertex4 ToVertex4() => new Vertex4(UV, Point.XYZ1(), Color, Normal);
    }

    public struct Vertex4
    {
        public Vector2 UV;
        public Vector3 Normal;
        public Vector4 Color, Point;

        public Vertex4(Vector4 point, Vector4 color, Vector3 normal)
        {
            Point = point;
            Color = color;
            Normal = normal;
            UV = new Vector2();
        }

        public Vertex4(Vector4 point, Vector4 color)
        {
            Point = point;
            Color = color;
            Normal = new Vector3();
            UV = new Vector2();
        }

        public Vertex4(Vector4 point, Vector2 uV)
        {
            Point = point;
            Color = new Vector4();
            Normal = new Vector3();
            UV = uV;
        }

        public Vertex4(Vector4 point, Color color, Vector3 normal)
        {
            Point = point;
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
            Normal = normal;
            UV = new Vector2();
        }

        public Vertex4(Vector4 point, Color color)
        {
            Point = point;
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
            Normal = new Vector3();
            UV = new Vector2();
        }

        public Vertex4(Vector2 uV, Vector4 point, Vector4 color, Vector3 normal)
        {
            UV = uV;
            Point = point;
            Normal = normal;
            Color = color;
        }

        public Vertex4(Vector2 uV, Vector4 point, Color color, Vector3 normal)
        {
            UV = uV;
            Point = point;
            Normal = normal;
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
        }

        public Vertex4(Vector2 uV, Vector4 point, Vector4 color)
        {
            UV = uV;
            Point = point;
            Normal = new Vector3();
            Color = color;
        }

        public Vertex4(Vector2 uV, Vector4 point, Color color)
        {
            UV = uV;
            Point = point;
            Normal = new Vector3();
            Color32 c = new Color32(color);
            Color = new Vector4(c.R, c.G, c.B, c.A);
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
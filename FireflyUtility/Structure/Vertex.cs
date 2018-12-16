using System.Drawing;
using System.Numerics;
using FireflyUtility.Math;

namespace FireflyUtility.Structure
{
    public struct Vertex
    {
        public Vector3 Point;
        public Vector4 Color;

        public Vertex(Vector3 point, Vector4 color)
        {
            Point = point;
            Color = color;
        }

        public Vertex(Vector3 point, Color color)
        {
            Point = point;
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
using System.Drawing;
using System.Numerics;
using Firefly.Math;

namespace Firefly.Render.Structure
{
    public struct Vertex
    {
        public Vector3 Point;
        public Color32 Color;

        public Vertex(Vector3 point, Color32 color)
        {
            Point = point;
            Color = color;
        }

        public Vertex(Vector3 point, Color color)
        {
            Point = point;
            Color = new Color32(color);
        }
    }

    public struct VertexInt
    {
        public Vector2Int Point;
        public Color32 Color;

        public VertexInt(Vector2Int point, Color32 color)
        {
            Point = point;
            Color = color;
        }
    }
}
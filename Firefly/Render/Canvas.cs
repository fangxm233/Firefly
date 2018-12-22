using System.Numerics;
using FireflyUtility.Math;
using FireflyUtility.Structure;
using Veldrid;

namespace Firefly.Render
{
    public class Canvas
    {
        public const uint Width = FireflyApplication.Width, Height = FireflyApplication.Height;

        public static void DrawTrangle(VertexInt v1, VertexInt v2, VertexInt v3)
        {
            //Console.WriteLine(v1.Point);
            //Console.WriteLine(v2.Point);
            //Console.WriteLine(v3.Point);
            //Console.WriteLine();

            Sort(ref v1, ref v2, ref v3);
            if (v2.Point.Y == v3.Point.Y)
                FillFlatTriangle(v1, v2, v3, false);
            else if (v1.Point.Y == v2.Point.Y)
                FillFlatTriangle(v3, v1, v2, true);
            else
            {
                float t = (float) (v2.Point.Y - v1.Point.Y) / (v3.Point.Y - v1.Point.Y);
                VertexInt v4 = Mathf.Lerp(v1, v3, t);
                FillFlatTriangle(v1, v2, v4, false);
                FillFlatTriangle(v3, v2, v4, true);
            }
        }

        private static void FillFlatTriangle(VertexInt v1, VertexInt v2, VertexInt v3, bool isFlatBottom)
        {
            if (isFlatBottom)
                for (int scanlineY = v2.Point.Y; scanlineY <= v1.Point.Y; scanlineY++)
                {
                    float t = (float)(scanlineY - v2.Point.Y) / (v1.Point.Y - v2.Point.Y);
                    DrawFlatLine(Mathf.Lerp(v1, v2, t), Mathf.Lerp(v1, v3, t));
                }
            else
                for (int scanlineY = v1.Point.Y; scanlineY <= v2.Point.Y; scanlineY++)
                {
                    float t = (float)(scanlineY - v1.Point.Y) / (v2.Point.Y - v1.Point.Y);
                    DrawFlatLine(Mathf.Lerp(v1, v2, t), Mathf.Lerp(v1, v3, t));
                }

        }

        private static void DrawFlatLine(VertexInt v1, VertexInt v2)
        {
            int x0 = v1.Point.X;
            int x1 = v2.Point.X;
            if (x0 > x1)
            {
                int t = x0;
                x0 = x1;
                x1 = t;
                Vector4 c = v1.Color;
                v1.Color = v2.Color;
                v2.Color = c;
            }
            float dx = x1 - x0 + float.Epsilon;

            for (int i = x0; i <= x1; i++)
                SetPixel(i, v1.Point.Y, Vector4.Lerp(v1.Color, v2.Color, (i - x0) / dx));
        }

        public static void DrawLine(Vector2 v1, Vector2 v2)
        {
            int x0 = (int)v1.X, y0 = (int)v1.Y, x1 = (int)v2.X, y1 = (int)v2.Y;
            int dx = System.Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = System.Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;

            for (; x0 != x1 || y0 != y1;)
            {
                int e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
                SetPixel(x0, y0);
            }
        }

        public static void DrawLine(Vector2 v1, Vector2 v2, Color32 color)
        {
            int x0 = (int)v1.X, y0 = (int)v1.Y, x1 = (int)v2.X, y1 = (int)v2.Y;
            int dx = System.Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = System.Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;

            for (; x0 != x1 || y0 != y1;)
            {
                int e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
                SetPixel(x0, y0, color);
            }
        }

        private static void SetPixel(int x, int y)
        {
            if (x < 0 || y < 0) return;
            if (x >= Width || y >= Height) return;
            Renderer.Buff[y * Width + x] = Renderer.Color.ToRgbaFloat();
        }

        private static void SetPixel(int x, int y, Color32 color)
        {
            if (x < 0 || y < 0) return;
            if (x >= Width || y >= Height) return;
            Renderer.Buff[y * Width + x] = color.ToRgbaFloat();
        }

        public static void SetPixel(int x, int y, Vector4 color)
        {
            if (x < 0 || y < 0) return;
            if (x >= Width || y >= Height) return;
            Renderer.Buff[y * Width + x] = ToRgbaFloat(color);
        }

        /// 小到大排序
        private static void Sort(ref VertexInt v1, ref VertexInt v2, ref VertexInt v3)
        {
            if (v1.Point.Y > v2.Point.Y) Swap(ref v1, ref v2);
            if (v2.Point.Y > v3.Point.Y) Swap(ref v2, ref v3);
            if (v1.Point.Y > v2.Point.Y) Swap(ref v1, ref v2);

            void Swap(ref VertexInt a, ref VertexInt b)
            {
                VertexInt c = a;
                a = b;
                b = c;
            }
        }

        public static bool BackFaceCulling(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p3 - p2;
            Vector3 normal = Vector3.Cross(v1, v2);
            Vector3 view_dir = p1 - Renderer.CurrentScene.Camera.Position;
            return Vector3.Dot(normal, view_dir) > 0;
        }

        public static Vector2Int ToScreen(Vector3 pos) => 
            new Vector2Int((int)(pos.X * (1 / pos.Z) * Width + Width / 2), (int)(pos.Y * (1 / pos.Z) * Height + Height / 2));

        public static Vector2Int ToScreen(Vector4 pos) =>
                new Vector2Int((int)(pos.X * Width / (2 * pos.W) + Width / 2), (int)(pos.Y * Height / (2 * pos.W) + Height / 2));
        //new Vector2Int((int)(pos.X* (1 / pos.Z) * Width + Width / 2), (int) (pos.Y* (1 / pos.Z) * Height + Height / 2));

        public static Vector2Int ToScreenO(Vector4 pos) =>
            new Vector2Int((int)(pos.X * Width / 2 + Width / 2), (int)(pos.Y * Height / 2 + Height / 2));

        public static VertexInt ToScreen(Vertex p) => new VertexInt() { Point = ToScreen(p.Point), Color = p.Color };

        public static RgbaFloat ToRgbaFloat(Vector4 color) => new RgbaFloat(color.X, color.Y, color.Z, color.W);
    }
}
using System.Numerics;
using FireflyUtility.Math;
using FireflyUtility.Structure;
using Veldrid;

namespace Firefly.Render
{
    public class Canvas
    {
        public const uint Width = FireflyApplication.Width, Height = FireflyApplication.Height;
        public static ShaderLib.Texture Tex;
        public static int TriCount;

        public static void DrawTrangle(Vertex4 v1, Vertex4 v2, Vertex4 v3)
        {
            if (Clip(v1) == true && Clip(v2) == true && Clip(v3) == true)
                return;
            TriCount++;

            v1.Point = ToScreen(v1.Point);
            v2.Point = ToScreen(v2.Point);
            v3.Point = ToScreen(v3.Point);

            v1 = MulOnePerZ(v1);
            v2 = MulOnePerZ(v2);
            v3 = MulOnePerZ(v3);

            Sort(ref v1, ref v2, ref v3);
            if (v2.Point.Y == v3.Point.Y)
                FillFlatTriangle(v1, v2, v3, false);
            else if (v1.Point.Y == v2.Point.Y)
                FillFlatTriangle(v3, v1, v2, true);
            else
            {
                float t = (v2.Point.Y - v1.Point.Y) / (v3.Point.Y - v1.Point.Y);
                Vertex4 v4 = Mathf.Lerp(v1, v3, t);
                FillFlatTriangle(v1, v2, v4, false);
                FillFlatTriangle(v3, v2, v4, true);
            }
        }

        private static void FillFlatTriangle(Vertex4 v1, Vertex4 v2, Vertex4 v3, bool isFlatBottom)
        {
            if (isFlatBottom)
                for (float scanlineY = v2.Point.Y; scanlineY <= v1.Point.Y; scanlineY++)
                {
                    float t = (scanlineY - v2.Point.Y) / (v1.Point.Y - v2.Point.Y);
                    DrawFlatLine(Mathf.Lerp(v1, v2, t), Mathf.Lerp(v1, v3, t));
                }
            else
                for (float scanlineY = v1.Point.Y; scanlineY <= v2.Point.Y; scanlineY++)
                {
                    float t = (scanlineY - v1.Point.Y) / (v2.Point.Y - v1.Point.Y);
                    DrawFlatLine(Mathf.Lerp(v1, v2, t), Mathf.Lerp(v1, v3, t));
                }
        }

        private static void DrawFlatLine(Vertex4 v1, Vertex4 v2)
        {
            if (v1.Point.Y > Height || v1.Point.Y < 0) return;
            if (v1.Point.X > v2.Point.X) Swap(ref v1, ref v2);
            if (v1.Point.X > Width || v2.Point.X < 0) return;
            float x0 = v1.Point.X;
            float x1 = v2.Point.X > Width ? Width : v2.Point.X;
            float dx = v2.Point.X - x0 + 0.01f;

            for (int i = (int)(x0 < 0 ? 0 : x0); i <= x1; i++)
            {
                float t = (i - x0) / dx;
                Vertex4 v = Mathf.Lerp(v1, v2, t);
                v = MulOnePerZ(v);
                SetPixel(i, (int)(v1.Point.Y + 0.5), Tex.Value(v.UV), v.Point.Z);
            }
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

        private static bool Clip(Vertex4 v)
        {
            //cvv为 x-1,1  y-1,1  z0,1
            if (v.Point.X >= -v.Point.W && v.Point.X <= v.Point.W &&
                v.Point.Y >= -v.Point.W && v.Point.Y <= v.Point.W &&
                v.Point.Z >= 0f && v.Point.Z <= v.Point.W)
                return true;
            return false;
        }

        private static Vertex4 MulOnePerZ(Vertex4 v)
        {
            v.Point.W = 1 / v.Point.W;
            v.Color *= v.Point.W;
            v.Normal *= v.Point.W;
            v.UV *= v.Point.W;
            return v;
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

        public static void SetPixel(int x, int y, Vector4 color, float z)
        {
            if (x < 0 || y < 0) return;
            if (x >= Width || y >= Height) return;
            if (Renderer.DepthBuff[y * Width + x] > z)
            {
                Renderer.DepthBuff[y * Width + x] = z;
                Renderer.Buff[y * Width + x] = ToRgbaFloat(color);
            }
        }

        /// 小到大排序
        private static void Sort(ref Vertex4 v1, ref Vertex4 v2, ref Vertex4 v3)
        {
            if (v1.Point.Y > v2.Point.Y) Swap(ref v1, ref v2);
            if (v2.Point.Y > v3.Point.Y) Swap(ref v2, ref v3);
            if (v1.Point.Y > v2.Point.Y) Swap(ref v1, ref v2);
        }

        static void Swap(ref Vertex4 a, ref Vertex4 b)
        {
            Vertex4 c = a;
            a = b;
            b = c;
        }

        public static bool BackFaceCulling(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p3 - p2;
            Vector3 normal = Vector3.Cross(v1, v2);
            Vector3 view_dir = p1 - Renderer.CurrentScene.Camera.Position;
            return Vector3.Dot(normal, view_dir) > 0;
        }

        public static Vector4 ToScreen(Vector4 pos) =>
            new Vector4((int)(pos.X * Renderer.Width / (2 * pos.W) + Renderer.Width / 2),
                (int)(pos.Y * Renderer.Height / (2 * pos.W) + Renderer.Height / 2), pos.Z, pos.W);
        //new Vector3((pos.X * (1 / pos.Z) * Renderer.Width + Renderer.Width / 2), (pos.Y * (1 / pos.Z) * Renderer.Height + Renderer.Height / 2), pos.Z);


        //public static Vector2Int ToScreen(Vector3 pos) => 
        //    new Vector2Int((int)(pos.X * (1 / pos.Z) * Width + Width / 2), (int)(pos.Y * (1 / pos.Z) * Height + Height / 2));

        //public static Vector2Int ToScreen(Vector4 pos) =>
        //        new Vector2Int((int)(pos.X * Width / (2 * pos.W) + Width / 2), (int)(pos.Y * Height / (2 * pos.W) + Height / 2));
        //new Vector2Int((int)(pos.X* (1 / pos.Z) * Width + Width / 2), (int) (pos.Y* (1 / pos.Z) * Height + Height / 2));

        public static Vector2Int ToScreenO(Vector4 pos) =>
            new Vector2Int((int)(pos.X * Width / 2 + Width / 2), (int)(pos.Y * Height / 2 + Height / 2));

        //public static VertexInt ToScreen(Vertex p) => new VertexInt() { Point = ToScreen(p.Point), Color = p.Color };

        public static RgbaFloat ToRgbaFloat(Vector4 color) => new RgbaFloat(color.X, color.Y, color.Z, color.W);
    }
}
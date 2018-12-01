using System.Drawing;
using Firefly.Math;
using Veldrid;

namespace Firefly.Render
{
    public struct Color32
    {
        public float R, B, G, A;

        public Color32(float r, float g, float b, float a = 1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color32(int r, int g, int b, int a = 255)
        {
            R = Mathf.Range((float)r / 255, 0, 1);
            G = Mathf.Range((float)g / 255, 0, 1);
            B = Mathf.Range((float)b / 255, 0, 1);
            A = Mathf.Range((float)a / 255, 0, 1);
        }

        public Color32(Color c)
        {
            R = Mathf.Range((float)c.R / 255, 0, 1);
            G = Mathf.Range((float)c.G / 255, 0, 1);
            B = Mathf.Range((float)c.B / 255, 0, 1);
            A = Mathf.Range((float)c.A / 255, 0, 1);
        }

        public override string ToString() => "<" + R + "," + G + "," + B + ">";

        public Color ToSystemColor()
        {
            if (float.IsNaN(R) || float.IsNaN(G) || float.IsNaN(B) || float.IsNaN(A)) return Color.DeepPink;
            return Color.FromArgb((int)(A * 255 + 0.5), (int)(R * 255 + 0.5), (int)(G * 255 + 0.5), (int)(B * 255 + 0.5));
        }
        public RgbaFloat ToRgbaFloat() => new RgbaFloat(R, G, B, A);

        public void Reset()
        {
            A = 0;
            R = 0;
            B = 0;
            G = 0;
        }

        public static Color32 operator +(Color32 a, Color32 b) =>
            new Color32(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);

        public static Color32 operator -(Color32 a, Color32 b) =>
            new Color32(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);

        public static Color32 operator *(Color32 a, Color32 b) =>
            new Color32(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);

        public static Color32 operator *(Color32 a, float b) => new Color32(a.R * b, a.G * b, a.B * b, a.A * b);

        public static Color32 operator *(float b, Color32 a) => new Color32(a.R * b, a.G * b, a.B * b, a.A * b);

        public static Color32 operator /(Color32 a, float b) => new Color32(a.R / b, a.G / b, a.B / b, a.A / b);

        public static Color32 Black = new Color32(0, 0, 0), White = new Color32(1, 1, 1);
    }
}

using System.Drawing;
using System.Numerics;

namespace ShaderLib
{
    public class Texture
    {
        private readonly byte[] _data;
        public readonly int Width, Height;
        private readonly float _scale = 1;

        public Texture(string file)
        {
            Bitmap bitmap = new Bitmap(Image.FromFile($"Texture/{file}"));
            Width = bitmap.Width;
            Height = bitmap.Height;
            _data = new byte[Width * Height * 3];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Color c = bitmap.GetPixel(j, i);
                    _data[j * 3 + Width * i * 3] = c.R;
                    _data[j * 3 + Width * i * 3 + 1] = c.G;
                    _data[j * 3 + Width * i * 3 + 2] = c.B;
                }
        }

        public Vector4 Value(Vector2 uv)
        {
            int i = ShaderMath.Range((int)(uv.X * Width), 0, Width - 1);
            int j = ShaderMath.Range((int)(uv.Y * Height), 0, Height - 1);

            return new Vector4(
                _data[j * 3 + Width * i * 3] / 255f,
                _data[j * 3 + Width * i * 3 + 1] / 255f,
                _data[j * 3 + Width * i * 3 + 2] / 255f, 1);
        }
    }
}

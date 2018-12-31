using System.Numerics;

namespace ShaderLib
{
    public class PointLight
    {
        public string Name;
        public Vector3 Position, Rotation;
        public Vector4 Color;
        public float Intensity, Range;

        public PointLight(string name, Vector3 position, Vector3 rotation, Vector4 color, float intensity, float range)
        {
            Name = name;
            Position = position;
            Rotation = rotation;
            Color = color * intensity;
            Color.W = 1;
            Intensity = intensity;
            Range = range;
        }
    }

    public class DirectionalLight
    {
        public string Name;
        public Vector3 Position, Rotation;
        public Vector4 Color;
        public float Intensity;
    }
}

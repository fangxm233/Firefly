using ShaderLib;
using ShaderLib.Attributes;
using System.Numerics;

[Shader]
public class TexShader
{
    public Texture Tex;

    [VertexShader]
    public V2F VS(A2V a)
    {
        return new V2F()
        {
            Position = ShaderMath.Mul(Matrixs.MVP, new Vector4(a.Position, 1)),
            UV = a.UV
        };
    }

    [FragmentShader]
    public F FS(V2F v)
    {
        return new F() { Color = Tex.Value(v.UV) };
    }
}

[VertexInput]
public struct A2V
{
    public Vector3 Position;
    public Vector2 UV;
}

[VertexOutput]
public struct V2F
{
    public Vector4 Position;
    public Vector2 UV;
}

[FragmentOutput]
public struct F
{
    public Vector4 Color;
}
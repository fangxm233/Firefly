using ShaderLib;
using ShaderLib.Attributes;
using System.Numerics;

[Shader]
public class TestShader
{
    public Texture Tex;

    [VertexShader]
    public VSOutput VS(VSInput input)
    {
        return new VSOutput()
        {
            Position = ShaderMath.Mul(Matrixs.MVP, new Vector4(input.Position, 1)),
            Color = input.Color
        };
    }

    [FragmentShader]
    public FSOutput FS(VSOutput input)
    {
        return new FSOutput() { Color = input.Color };
    }
}

[VertexInput]
public struct VSInput
{
    public Vector3 Position;
    public Vector4 Color;
}

[VertexOutput]
public struct VSOutput
{
    public Vector4 Position;
    public Vector4 Color;
}

[FragmentOutput]
public struct FSOutput
{
    public Vector4 Color;
}
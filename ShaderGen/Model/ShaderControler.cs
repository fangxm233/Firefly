using FireflyUtility.Renderable;
using FireflyUtility.Structure;
using Firefly;
using Firefly.Render;
using ShaderLib;
using System.Numerics;
using System;

public class ShaderControler
{
    public static __ShaderName__ Shader;

    public static object GetNewShader() => new __ShaderName__();

    public static void SetShader(object shader) => Shader = (__ShaderName__)shader;

    public static void Draw(Entity entity)
    {
        entity.CalculateMatrix();
        Mesh mesh = entity.Mesh;
        //Console.WriteLine("Rotation {0}", entity.Rotation);
        for (int j = 0; j + 2 < mesh.Triangles.Length; j += 3)
        {
            if (!Canvas.BackFaceCulling(entity.ToWorld(mesh.GetPoint(j)).Point,
                entity.ToWorld(mesh.GetPoint(j + 1)).Point,
                entity.ToWorld(mesh.GetPoint(j + 2)).Point))
                DrawTriangle(mesh.GetPoint(j), mesh.GetPoint(j + 1), mesh.GetPoint(j + 2));
        }
    }

    private static void DrawTriangle(Vertex v1, Vertex v2, Vertex v3)
    {
        //调用顶点着色shader
        __CreateVSInputStruct1__
        __VSOutputType__ p1 = Shader.__VertexShaderName__(vi1);
        __CreateVSInputStruct2__
        __VSOutputType__ p2 = Shader.__VertexShaderName__(vi2);
        __CreateVSInputStruct3__
        __VSOutputType__ p3 = Shader.__VertexShaderName__(vi3);

        //转换到屏幕坐标
        p1 = ToScreen(p1);
        p2 = ToScreen(p2);
        p3 = ToScreen(p3);

        //消除畸变
        p1 = MulOnePerZ(p1);
        p2 = MulOnePerZ(p2);
        p3 = MulOnePerZ(p3);

        //Console.WriteLine($"{p1.Position} {p2.Position} {p3.Position}");
        //Console.WriteLine((p1.Position.Y - p2.Position.Y));
        //排序，并调用填充函数
        Sort(ref p1, ref p2, ref p3);
        if (p2.Position.Y == p3.Position.Y)
            FillFlatTriangle(p1, p2, p3, false);
        else if (p1.Position.Y == p2.Position.Y)
            FillFlatTriangle(p3, p1, p2, true);
        else
        {
            float t = (p2.Position.Y - p1.Position.Y) / (p3.Position.Y - p1.Position.Y);
            __VSOutputType__ p4 = Lerp(p1, p3, t);
            FillFlatTriangle(p1, p2, p4, false);
            FillFlatTriangle(p3, p2, p4, true);
        }
    }

    private static void FillFlatTriangle(__VSOutputType__ v1, __VSOutputType__ v2, __VSOutputType__ v3, bool isFlatBottom)
    {
        if (isFlatBottom)
            for (float scanlineY = v2.Position.Y; scanlineY <= v1.Position.Y; scanlineY++)
            {
                float t = (scanlineY - v2.Position.Y) / (v1.Position.Y - v2.Position.Y);
                DrawFlatLine(Lerp(v1, v2, t), Lerp(v1, v3, t));
            }
        else
            for (float scanlineY = v1.Position.Y; scanlineY <= v2.Position.Y; scanlineY++)
            {
                float t = (scanlineY - v1.Position.Y) / (v2.Position.Y - v1.Position.Y);
                DrawFlatLine(Lerp(v1, v2, t), Lerp(v1, v3, t));
            }
    }

    private static void DrawFlatLine(__VSOutputType__ v1, __VSOutputType__ v2)
    {
        if (v1.Position.Y > Canvas.Height || v1.Position.Y < 0) return;
        if (v1.Position.X > v2.Position.X) Swap(ref v1, ref v2);
        if (v1.Position.X > Canvas.Width || v2.Position.X < 0) return;
        float x0 = v1.Position.X;
        float x1 = v2.Position.X > Canvas.Width ? Canvas.Width : v2.Position.X;
        float dx = v2.Position.X - x0 + 0.01f;

        //Console.WriteLine($"dx: {dx}");
        for (int i = (int)x0; i <= x1; i++)
        {
            float t = (i - x0) / dx;
            __VSOutputType__ v = Lerp(v1, v2, t);
            v = MulOnePerZ(v);
            __FSOutputType__ o = Shader.__FragmentShaderName__(v);
            Canvas.SetPixel(i, (int)(v1.Position.Y + 0.5), o.Color, v1.Position.Z);
        }
    }

    private static __VSOutputType__ MulOnePerZ(__VSOutputType__ v)
    {
        v.Position.W = 1 / v.Position.W;
        __MulOnePerZCode__
        return v;
    }

    private static __VSOutputType__ ToScreen(__VSOutputType__ pos)
    {
        __ToScreenCode__
    }

    private static Vector4 ToScreen(Vector4 pos) =>
                        new Vector4((int)(pos.X * Renderer.Width / (2 * pos.W) + Renderer.Width / 2), 
                            (int)(pos.Y * Renderer.Height / (2 * pos.W) + Renderer.Height / 2), pos.Z, pos.W);
    //new Vector4((pos.X * (1 / pos.Z) * Renderer.Width + Renderer.Width / 2), (pos.Y * (1 / pos.Z) * Renderer.Height + Renderer.Height / 2), pos.Z, pos.W);

    private static __VSOutputType__ Lerp(__VSOutputType__ a, __VSOutputType__ b, float t)
    {
        __LerpCode__
    }

    private static void Swap(ref __VSOutputType__ a, ref __VSOutputType__ b)
    {
        __VSOutputType__ c = a;
        a = b;
        b = c;
    }

    private static float Lerp(float a, float b, float t)
    {
        if (t <= 0)
            return a;
        if (t >= 1)
            return b;
        return b * t + (1 - t) * a;
    }

    private static void Sort(ref __VSOutputType__ v1, ref __VSOutputType__ v2, ref __VSOutputType__ v3)
    {
        if (v1.Position.Y > v2.Position.Y) Swap(ref v1, ref v2);
        if (v2.Position.Y > v3.Position.Y) Swap(ref v2, ref v3);
        if (v1.Position.Y > v2.Position.Y) Swap(ref v1, ref v2);
    }

    public static void SetField(string name, object value)
    {
        switch (name)
        {
            __CaseCode__
        }
    }
}
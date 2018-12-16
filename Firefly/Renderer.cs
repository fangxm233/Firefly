using System;
using Firefly.Render;
using FireflyUtility.Renderable;
using FireflyUtility.Structure;
using Veldrid;
using ShaderGen;
using ShaderLib;
using FireflyUtility.Math;
using System.Numerics;

namespace Firefly
{
    public enum RenderType
    {
        GouraudShading,
        NoShading
    }

    public static class Renderer
    {
        public static RgbaFloat[] Buff;
        public static Color32 Color;
        public const uint Width = FireflyApplication.Width, Height = FireflyApplication.Height;
        public static RenderType RenderType;

        public static Camera Camera;
        public static Light[] Lights;
        public static Entity[] Entities;

        private static readonly Random _random = new Random();

        public static void StartRender(Color32 color, RgbaFloat[] buff, RenderType renderType)
        {
            Buff = buff;
            Color = color;
            RenderType = renderType;
        }

        public static void Draw()
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entity entity = Entities[i];
                Matrixs.CameraPosition = Camera.Position;
                Matrixs.CameraRotation = new Vector3(0, 0, 1);
                Matrixs.EntityPosition = entity.Position;
                Matrixs.EntityRotation = entity.Rotation;
                Matrixs.Far = 500;
                Matrixs.Near = 0.5f;
                Matrixs.FOV = MathF.PI;
                Matrixs.Aspect = Width / Height;
                Matrixs.Width = Width;
                Matrixs.Height = Height;
                Matrixs.Size = 1;
                Matrixs.CalculateMatrixs();
                //ShaderGenerator.DrawDelegates[0].Invoke(entity);
                Mesh mesh = Entities[i].Mesh;
                entity.CalculateMatrix();
                for (int j = 0; j + 2 < mesh.Triangles.Length; j += 3)
                {
                    if (!Canvas.BackFaceCulling(entity.ToWorld(mesh.GetPoint(j)).Point,
                        entity.ToWorld(mesh.GetPoint(j + 1)).Point,
                        entity.ToWorld(mesh.GetPoint(j + 2)).Point))
                    {
                        Canvas.DrawTrangle(
                            new VertexInt(Canvas.ToScreen(ShaderMath.Mul(Matrixs.MVP, new Vector4(mesh.GetPoint(j).Point, 1))), mesh.GetPoint(j).Color),
                            new VertexInt(Canvas.ToScreen(ShaderMath.Mul(Matrixs.MVP, new Vector4(mesh.GetPoint(j + 1).Point, 1))), mesh.GetPoint(j + 1).Color),
                            new VertexInt(Canvas.ToScreen(ShaderMath.Mul(Matrixs.MVP, new Vector4(mesh.GetPoint(j + 2).Point, 1))), mesh.GetPoint(j + 2).Color)
                            );
                    }
                }
            }

            //Canvas.DrawTrangle(
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(255, 0, 0)),
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(0, 255, 0)),
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(0, 0, 255)));

            //Canvas.DrawTrangle(new VertexInt(new Vector2Int(256, 384), new Color32(255, 0, 0)),
            //    new VertexInt(new Vector2Int(128, 128), new Color32(0, 255, 0)),
            //    new VertexInt(new Vector2Int(384, 128), new Color32(0, 0, 255)));

            //Canvas.DrawTrangle(new VertexInt(new Vector2Int(256, 128), new Color32(0, 127, 127)),
            //    new VertexInt(new Vector2Int(128, 384), new Color32(0, 127, 127)),
            //    new VertexInt(new Vector2Int(0, 512), new Color32(0, 127, 127)));
        }


    }
}
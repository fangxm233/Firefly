using System;
using System.Drawing;
using System.Numerics;
using Firefly.Math;
using Firefly.Render;
using Firefly.Render.Renderable;
using Firefly.Render.Structure;
using Veldrid;

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
        public static uint Width, Height;
        public static RenderType RenderType;

        public static Camera Camera;
        public static Light[] Lights;
        public static Entity[] Entities;

        private static Random random = new Random();

        public static void StartRender(uint width, uint height, Color32 color, RgbaFloat[] buff, RenderType renderType)
        {
            Width = width;
            Height = height;
            Buff = buff;
            Color = color;
            RenderType = renderType;

            Canvas.Height = height;
            Canvas.Width = width;
        }

        public static void Draw()
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entity entity = Entities[i];
                Mesh mesh = Entities[i].Mesh;
                entity.CalculateMatrix();
                for (int j = 0; j + 2 < mesh.Triangles.Length; j += 3)
                {
                    if (!Canvas.BackFaceCulling(entity.ToWorld(mesh.GetPoint(j)).Point,
                        entity.ToWorld(mesh.GetPoint(j + 1)).Point,
                        entity.ToWorld(mesh.GetPoint(j + 2)).Point))
                    {
                        Canvas.DrawTrangle(
                            Canvas.ToScreen(entity.ToWorld(mesh.GetPoint(j))),
                            Canvas.ToScreen(entity.ToWorld(mesh.GetPoint(j + 1))),
                            Canvas.ToScreen(entity.ToWorld(mesh.GetPoint(j + 2))));
                    }
                }
            }

            //Canvas.DrawTrangle(
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(255, 0, 0)),
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(0, 255, 0)),
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(0, 0, 255)));

            //Canvas.DrawTrangle(new VertexInt(new Vector2Int(256, 128), new Color32(255, 0, 0)),
            //    new VertexInt(new Vector2Int(128, 384), new Color32(0, 255, 0)),
            //    new VertexInt(new Vector2Int(384, 384), new Color32(0, 0, 255)));

            //Canvas.DrawTrangle(new VertexInt(new Vector2Int(256, 128), new Color32(0, 127, 127)),
            //    new VertexInt(new Vector2Int(128, 384), new Color32(0, 127, 127)),
            //    new VertexInt(new Vector2Int(0, 512), new Color32(0, 127, 127)));
        }


    }
}
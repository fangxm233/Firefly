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
        public static int Width, Height;
        public static RenderType RenderType;

        public static Camera Camera;
        public static Light[] Lights;
        public static Entity[] Entities;

        private static Random random = new Random();

        public static void StartRender(int width, int height, Color32 color, RgbaFloat[] buff, RenderType renderType)
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
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(1, 0, 0)),
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(0, 1, 0)),
            //    new VertexInt(new Vector2Int(random.Next(0, 513), random.Next(0, 513)), new Color32(0, 0, 1)));

            //Canvas.DrawTrangle(new VertexInt(new Vector2Int(476, 326), new Color32(1, 0, 0)),
            //    new VertexInt(new Vector2Int(325, 117), new Color32(0, 1, 0)),
            //    new VertexInt(new Vector2Int(109, 459), new Color32(0, 0, 1)));
        }


    }
}
using System;
using Firefly.Render;
using FireflyUtility.Renderable;
using FireflyUtility.Structure;
using Veldrid;
using ShaderGen;
using ShaderLib;
using FireflyUtility.Math;
using System.Numerics;
using System.Collections.Generic;

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

        public static Dictionary<string, Material> Materials;
        public static Dictionary<string, DelegateCollection> DelegateCollections;
        public static Dictionary<string, ShaderInformation> ShaderInformation;
        public static Scene CurrentScene;

        private static List<(string MaterialName, string FieldName, object Value)> _commandQueue =
            new List<(string MaterialName, string FieldName, object Value)>();
        private static readonly Random _random = new Random();

        public static void InitRender(Color32 color, RgbaFloat[] buff, RenderType renderType)
        {
            Buff = buff;
            Color = color;
            RenderType = renderType;
        }

        public static void InitMaterials()
        {
            foreach (KeyValuePair<string, Material> item in Materials)
                item.Value.Shader = DelegateCollections[item.Value.ShaderName].GetShader.Invoke();
            foreach (var (MaterialName, prop) in ResourceLoader.MaterialInputs)
            {
                (string MaterialName, string FieldName, object Value) command;
                command.MaterialName = MaterialName;
                command.FieldName = prop.Name;
                command.Value = null;
                ShaderInformation info = ShaderInformation[Materials[MaterialName].ShaderName];
                switch (info.ShaderFields[prop.Name])
                {
                    case "Byte":
                        command.Value = prop.Value.ToObject<byte>();
                        break;
                    case "Int32":
                        command.Value = prop.Value.ToObject<int>();
                        break;
                    case "Single":
                        command.Value = prop.Value.ToObject<float>();
                        break;
                    case "Vector2":
                        command.Value = ResourceLoader.GetVector2(prop.Value);
                        break;
                    case "Vector3":
                        command.Value = ResourceLoader.GetVector3(prop.Value);
                        break;
                    case "Vector4":
                        command.Value = ResourceLoader.GetVector4(prop.Value);
                        break;
                    case "Texture":
                        command.Value = new ShaderLib.Texture(prop.Value.ToString());
                        break;
                    default:
                        break;
                }
                _commandQueue.Add(command);
            }
        }

        public static void LoadScene(string name)
        {
            CurrentScene = ResourceLoader.LoadScene(name);
            Materials = CurrentScene.GetNeedMaterials();
        }

        public static void Draw()
        {
            foreach (var (MaterialName, FieldName, Value) in _commandQueue)
            {
                DelegateCollection collection = DelegateCollections[Materials[MaterialName].ShaderName];
                collection.SetShader.Invoke(Materials[MaterialName].Shader);
                collection.SetField.Invoke(FieldName, Value);
                _commandQueue = new List<(string MaterialName, string FieldName, object Value)>();
            }
            foreach (KeyValuePair<string, Entity> item in CurrentScene.Entities)
            {
                Entity entity = item.Value;
                Matrixs.CameraPosition = CurrentScene.Camera.Position;
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
                DelegateCollection collection = DelegateCollections[entity.Material.ShaderName];
                collection.SetShader.Invoke(entity.Material.Shader);
                collection.Draw.Invoke(entity);
                //Mesh mesh = item.Value.Mesh;
                //entity.CalculateMatrix();
                //for (int j = 0; j + 2 < mesh.Triangles.Length; j += 3)
                //{
                //    if (!Canvas.BackFaceCulling(entity.ToWorld(mesh.GetPoint(j)).Point,
                //        entity.ToWorld(mesh.GetPoint(j + 1)).Point,
                //        entity.ToWorld(mesh.GetPoint(j + 2)).Point))
                //    {
                //        Canvas.DrawTrangle(
                //            new VertexInt(Canvas.ToScreen(ShaderMath.Mul(Matrixs.MVP, new Vector4(mesh.GetPoint(j).Point, 1))), mesh.GetPoint(j).Color),
                //            new VertexInt(Canvas.ToScreen(ShaderMath.Mul(Matrixs.MVP, new Vector4(mesh.GetPoint(j + 1).Point, 1))), mesh.GetPoint(j + 1).Color),
                //            new VertexInt(Canvas.ToScreen(ShaderMath.Mul(Matrixs.MVP, new Vector4(mesh.GetPoint(j + 2).Point, 1))), mesh.GetPoint(j + 2).Color)
                //            );
                //    }
                //}
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
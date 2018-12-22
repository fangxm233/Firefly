using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using FireflyUtility.Renderable;
using FireflyUtility.Structure;
using Newtonsoft.Json.Linq;
using ShaderGen;

namespace Firefly
{
    public class ResourceLoader
    {
        public static List<(string MaterialName, JProperty prop)> MaterialInputs = new List<(string MaterialName, JProperty prop)>();
        private static Dictionary<string, Material> _materials = new Dictionary<string, Material>();

        public static Scene LoadScene(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Scenes/{name}.json"));
            Scene scene = new Scene 
            {
                Entities = new Dictionary<string, Entity>(),
                Name = json["Name"].ToString(),
                Camera = GetCamera(json["Camera"])
            };
            JArray list = (JArray)json["Entities"];
            foreach (JToken item in list)
            {
                Entity entity = LoadEntity(item.ToString());
                scene.Entities.Add(entity.Name, entity);
            }
            return scene;
        }

        public static Entity LoadEntity(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Entities/{name}.json"));
            string mName = json["Material"].ToString();
            return new Entity 
            {
                Name = json["Name"].ToString(),
                Mesh = LoadMesh(json["Mesh"].ToString()),
                Material = _materials.ContainsKey(mName) ? _materials[mName] : LoadMaterial(mName),
                Position = GetVector3(json["Position"]),
                Rotation = GetVector3(json["Rotation"])
            };
        }

        private static Camera GetCamera(JToken token) => new Camera {
            Position = GetVector3(token["Position"])
        };

        public static Material LoadMaterial(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Materials/{name}.json"));
            foreach (JProperty item in json.Properties())
                if (item.Name != "Name" && item.Name != "Shader")
                    MaterialInputs.Add((name, item));
            Material m = new Material 
            {
                Name = json["Name"].ToString(),
                ShaderName = json["Shader"].ToString()
            };
            _materials.Add(m.Name, m);
            return m;
        }

        public static Mesh LoadMesh(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Meshs/{name}.json"));
            Mesh mesh = new Mesh 
            {
                Name = json["Name"].ToString(),
                Vertices = GetVertices((JArray)json["Vertexs"]),
                Triangles = GetIntArray((JArray)json["Triangles"])
            };
            return mesh;
        }

        //private static object GetValue()
        //{

        //}

        private static int[] GetIntArray(JArray list)
        {
            List<int> vs = new List<int>();
            foreach (JToken item in list)
                vs.Add(item.ToObject<int>());
            return vs.ToArray();
        }

        private static Vertex[] GetVertices(JArray list)
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach (JToken item in list)
                vertices.Add(GetVertex(item));
            return vertices.ToArray();
        }

        public static Vertex GetVertex(JToken token) => new Vertex(GetVector3(token["Position"]), GetColor(token["Color"]));

        public static Vector2 GetVector2(JToken token) =>
            new Vector2(token["X"].ToObject<float>(), token["Y"].ToObject<float>());

        public static Vector3 GetVector3(JToken token) => 
            new Vector3(token["X"].ToObject<float>(), token["Y"].ToObject<float>(), token["Z"].ToObject<float>());

        public static Vector4 GetVector4(JToken token) =>
            new Vector4(token["X"].ToObject<float>(), token["Y"].ToObject<float>(), 
                token["Z"].ToObject<float>(), token["W"].ToObject<float>());

        private static Color GetColor(JToken token) => 
            Color.FromArgb(token["R"].ToObject<int>(), token["G"].ToObject<int>(), token["B"].ToObject<int>());
    }
}

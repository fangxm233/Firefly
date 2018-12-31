using FireflyUtility.Renderable;
using ShaderLib;
using System.Collections.Generic;
using System.Numerics;

namespace FireflyUtility.Structure
{
    public class Scene
    {
        public string Name;
        public Camera Camera;
        public Vector4 AmbientColor;
        public DirectionalLight DirectionalLight;
        public Dictionary<string, PointLight> PointLights;
        public Dictionary<string, Entity> Entities;

        public Scene(string name, 
            Camera camera, 
            Vector4 ambientColor, 
            DirectionalLight directionalLight, 
            Dictionary<string, PointLight> pointLights, 
            Dictionary<string, Entity> entities)
        {
            Name = name;
            Camera = camera;
            AmbientColor = ambientColor;
            DirectionalLight = directionalLight;
            PointLights = pointLights;
            Entities = entities;
        }

        public Dictionary<string, Material> GetNeedMaterials()
        {
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            foreach (KeyValuePair<string, Entity> item in Entities)
                if (!materials.ContainsKey(item.Value.Material.Name))
                    materials.Add(item.Value.Material.Name, item.Value.Material);
            return materials;
        }
    }
}

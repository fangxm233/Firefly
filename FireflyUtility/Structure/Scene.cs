using FireflyUtility.Renderable;
using System.Collections.Generic;

namespace FireflyUtility.Structure
{
    public class Scene
    {
        public string Name;
        public Camera Camera;
        public Dictionary<string, Light> Lights;
        public Dictionary<string, Entity> Entities;

        public Dictionary<string, Material> GetNeedMaterials()
        {
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            foreach (KeyValuePair<string, Entity> item in Entities)
                materials.Add(item.Value.Material.Name, item.Value.Material);
            return materials;
        }
    }
}

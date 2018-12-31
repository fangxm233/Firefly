namespace FireflyUtility.Structure
{
    public class Material
    {
        public string Name;
        public string ShaderName;
        public object Shader;

        public Material(string name, string shaderName)
        {
            Name = name;
            ShaderName = shaderName;
        }
    }
}

using ShaderLib.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ShaderGen
{
    public class ShaderInformation
    {
        public string ShaderName, VSInputType, VSOutputType, FSOutputType, VertexShaderName, FragmentShaderName;
        public (string Name, string Type)[] VSInputFields, VSOutputFields, FSOutputFields;
        public Dictionary<string, string> ShaderFields = new Dictionary<string, string>();

        public ShaderInformation(string path)
        {
            Assembly asm = ShaderCompiler.Compile(new string[0], path.GetHashCode().ToString(), File.ReadAllText(path));
            Type[] types = asm.GetTypes();
            foreach (Type item in types)
            {
                IEnumerable<Attribute> attributes = item.GetCustomAttributes();
                foreach (Attribute item1 in attributes)
                {
                    FieldInfo[] fields;
                    switch (item1.ToString())
                    {
                        case "ShaderLib.Attributes." + nameof(ShaderAttribute):
                            ShaderName = item.Name;
                            fields = item.GetFields();
                            for (int i = 0; i < fields.Length; i++)
                                ShaderFields.Add(fields[i].Name, fields[i].FieldType.Name);
                            break;
                        case "ShaderLib.Attributes." + nameof(VertexInputAttribute):
                            VSInputType = item.Name;
                            fields = item.GetFields();
                            VSInputFields = new (string Name, string Type)[fields.Length];
                            for (int i = 0; i < VSInputFields.Length; i++)
                                VSInputFields[i] = (fields[i].Name, fields[i].FieldType.Name);
                            break;
                        case "ShaderLib.Attributes." + nameof(VertexOutputAttribute):
                            VSOutputType = item.Name;
                            fields = item.GetFields();
                            VSOutputFields = new (string Name, string Type)[fields.Length];
                            for (int i = 0; i < VSOutputFields.Length; i++)
                                VSOutputFields[i] = (fields[i].Name, fields[i].FieldType.Name);
                            break;
                        case "ShaderLib.Attributes." + nameof(FragmentOutputAttribute):
                            FSOutputType = item.Name;
                            fields = item.GetFields();
                            FSOutputFields = new (string Name, string Type)[fields.Length];
                            for (int i = 0; i < FSOutputFields.Length; i++)
                                FSOutputFields[i] = (fields[i].Name, fields[i].FieldType.Name);
                            break;
                        default:
                            break;
                    }
                }
            }
            Type type = asm.GetType(ShaderName);
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo item in methodInfos)
            {
                IEnumerable<Attribute> attributes = item.GetCustomAttributes();
                foreach (Attribute item1 in attributes)
                {
                    switch (item1.ToString())
                    {
                        case "ShaderLib.Attributes." + nameof(VertexShaderAttribute):
                            VertexShaderName = item.Name;
                            break;
                        case "ShaderLib.Attributes." + nameof(FragmentShaderAttribute):
                            FragmentShaderName = item.Name;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

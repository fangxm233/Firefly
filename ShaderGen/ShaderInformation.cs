using System;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Collections.Generic;
using ShaderLib.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.IO;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using System.Linq;

namespace ShaderGen
{
    class ShaderInformation
    {
        public string ShaderName, VSInputType, VSOutputType, FSOutputType, VertexShaderName, FragmentShaderName;
        public KeyValuePair<string, string>[] VSInputFields, VSOutputFields, FSOutputFields;

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
                            break;
                        case "ShaderLib.Attributes." + nameof(VertexInputAttribute):
                            VSInputType = item.Name;
                            fields = item.GetFields();
                            VSInputFields = new KeyValuePair<string, string>[fields.Length];
                            for (int i = 0; i < VSInputFields.Length; i++)
                                VSInputFields[i] = new KeyValuePair<string, string>(fields[i].Name, fields[i].FieldType.Name);
                            break;
                        case "ShaderLib.Attributes." + nameof(VertexOutputAttribute):
                            VSOutputType = item.Name;
                            fields = item.GetFields();
                            VSOutputFields = new KeyValuePair<string, string>[fields.Length];
                            for (int i = 0; i < VSOutputFields.Length; i++)
                                VSOutputFields[i] = new KeyValuePair<string, string>(fields[i].Name, fields[i].FieldType.Name);
                            break;
                        case "ShaderLib.Attributes." + nameof(FragmentOutputAttribute):
                            FSOutputType = item.Name;
                            fields = item.GetFields();
                            FSOutputFields = new KeyValuePair<string, string>[fields.Length];
                            for (int i = 0; i < FSOutputFields.Length; i++)
                                FSOutputFields[i] = new KeyValuePair<string, string>(fields[i].Name, fields[i].FieldType.Name);
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

using FireflyUtility.Renderable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ShaderGen
{
    enum InsertType
    {
        CreateVSInputStruct1,
        CreateVSInputStruct2,
        CreateVSInputStruct3,

        VSOutputType,
        FSOutputType,

        ShaderName,
        VertexShaderName,
        FragmentShaderName,

        CaseCode,
        LerpCode,
        ToScreenCode,
    }

    public class ShaderGenerator
    {
        private static readonly string[] _patterns;

        public static Dictionary<string, DelegateCollection> DelegateCollections = new Dictionary<string, DelegateCollection>();
        public static Dictionary<string, ShaderInformation> ShaderInformation = new Dictionary<string, ShaderInformation>();
        private static string[] _processedCode;
        private static Assembly[] _compiledShaders;

        static ShaderGenerator()
        {
            _patterns = new[]
            {
                "__CreateVSInputStruct1__",
                "__CreateVSInputStruct2__",
                "__CreateVSInputStruct3__",
                "__VSOutputType__",
                "__FSOutputType__",
                "__ShaderName__",
                "__VertexShaderName__",
                "__FragmentShaderName__",
                "__CaseCode__",
                "__LerpCode__",
                "__ToScreenCode__",
            };
        }

        public static bool CompleShader(List<string> path)
        {
            _processedCode = new string[path.Count];
            _compiledShaders = new Assembly[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                ShaderInformation information = new ShaderInformation(path[i]);
                ShaderInformation.Add(information.ShaderName, information);
                _processedCode[i] = InsertCodeToFile(@"Model\ShaderControler.cs", information);
                _compiledShaders[i] = ShaderCompiler.Compile(new[] { "Firefly.dll", "FireflyUtility.dll" },
                    information.ShaderName, _processedCode[i], File.ReadAllText(path[i]));
                //Console.WriteLine(_processedCode[i]);

                Type type = _compiledShaders[i].GetType("ShaderControler");
                DelegateCollection collection = new DelegateCollection
                {
                    Name = information.ShaderName,
                    Draw = (DrawDelegate)type.GetMethod("Draw").CreateDelegate(typeof(DrawDelegate)),
                    GetShader = (GetShaderDelegate)type.GetMethod("GetNewShader").CreateDelegate(typeof(GetShaderDelegate)),
                    SetShader = (SetShaderDelegate)type.GetMethod("SetShader").CreateDelegate(typeof(SetShaderDelegate)),
                    SetField = (SetFieldDelegate)type.GetMethod("SetField").CreateDelegate(typeof(SetFieldDelegate))
                };
                DelegateCollections.Add(collection.Name, collection);
            }
            return true;
        }

        private static string InsertCodeToFile(string name, ShaderInformation information)
        {
            string code = File.ReadAllText(name);
            for (int i = 0; i < _patterns.Length; i++)
                code = Regex.Replace(code, _patterns[i], GetCode((InsertType)i, information));
            return code;
        }

        private static string GetCode(InsertType type, ShaderInformation information)
        {
            string code = "";
            switch (type)
            {
                case InsertType.CreateVSInputStruct1:
                    code = $"{information.VSInputType} vi1 = new {information.VSInputType}(){{";
                    foreach (var (Name, Type) in information.VSInputFields)
                        if (Name == "Position") code += "Position = v1.Point,";
                        else if (Name == "Color") code += "Color = v1.Color,";
                        else code += $"{Name} = new {type}(),";
                    code += "};";
                    return code;
                case InsertType.CreateVSInputStruct2:
                    code = $"{information.VSInputType} vi2 = new {information.VSInputType}(){{";
                    foreach (var (Name, Type) in information.VSInputFields)
                        if (Name == "Position") code += "Position = v2.Point,";
                        else if (Name == "Color") code += "Color = v2.Color,";
                        else code += $"{Name} = new {type}(),";
                    code += "};";
                    return code;
                case InsertType.CreateVSInputStruct3:
                    code = $"{information.VSInputType} vi3 = new {information.VSInputType}(){{";
                    foreach (var (Name, Type) in information.VSInputFields)
                        if (Name == "Position") code += "Position = v3.Point,";
                        else if (Name == "Color") code += "Color = v3.Color,";
                        else code += $"{Name} = new {type}(),";
                    code += "};";
                    return code;
                case InsertType.VSOutputType:
                    return information.VSOutputType;
                case InsertType.FSOutputType:
                    return information.FSOutputType;
                case InsertType.ShaderName:
                    return information.ShaderName;
                case InsertType.VertexShaderName:
                    return information.VertexShaderName;
                case InsertType.FragmentShaderName:
                    return information.FragmentShaderName;
                case InsertType.CaseCode:
                    foreach (KeyValuePair<string, string> item in information.ShaderFields)
                        code += $"case \"{item.Key}\": Shader.{item.Key} = ({item.Value})value; break;";
                    return code;
                case InsertType.LerpCode:
                    code = $"return new {information.VSOutputType}(){{";
                    foreach (var (Name, Type) in information.VSOutputFields)
                        code += $"{Name} = {(Type == "float" ? "" : Type + ".")}Lerp(a.{Name}, b.{Name}, t),";
                    code += "};";
                    return code;
                case InsertType.ToScreenCode:
                    code = $"return new {information.VSOutputType}(){{";
                    foreach (var (Name, Type) in information.VSOutputFields)
                        code += $"{Name} = {(Name == "Position" ? "ToScreen(pos.Position)" : "pos." + Name)},";
                    code += "};";
                    return code;
                default:
                    break;
            }
            return null;
        }
    }
}

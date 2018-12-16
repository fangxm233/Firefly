using FireflyUtility.Renderable;
using System;
using System.Collections.Generic;
using System.IO;
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

        LerpCode,
        ToScreenCode,
    }

    public class ShaderGenerator
    {
        static ShaderInformation[] _shaderInformation;
        static string[] _processedCode;
        static Assembly[] _compiledShaders;
        public static DrawDelegate[] DrawDelegates;

        public delegate void DrawDelegate(Entity entity);

        public static bool CompleShader(List<string> path)
        {
            _processedCode = new string[path.Count];
            _shaderInformation = new ShaderInformation[path.Count];
            _compiledShaders = new Assembly[path.Count];
            DrawDelegates = new DrawDelegate[path.Count];
            for (int i = 0; i < _shaderInformation.Length; i++)
            {
                _shaderInformation[i] = new ShaderInformation(path[i]);
                _processedCode[i] = InsertCodeToFile(@"Model\ShaderControler.cs", _shaderInformation[i]);
                _compiledShaders[i] = ShaderCompiler.Compile(new[] {"Firefly.dll", "FireflyUtility.dll" }, 
                    _shaderInformation[i].ShaderName, _processedCode[i], File.ReadAllText(path[i]));
                //Console.WriteLine(_processedCode[i]);
                DrawDelegates[i] = 
                    (DrawDelegate)_compiledShaders[i].GetType("ShaderControler").GetMethod("Draw").CreateDelegate(typeof(DrawDelegate));
            }
            return true;
        }

        private static string InsertCodeToFile(string name, ShaderInformation information)
        {
            string code = File.ReadAllText(name);
            string pattern = "__CreateVSInputStruct1__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.CreateVSInputStruct1, information));
            pattern = "__CreateVSInputStruct2__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.CreateVSInputStruct2, information));
            pattern = "__CreateVSInputStruct3__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.CreateVSInputStruct3, information));
            pattern = "__VSOutputType__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.VSOutputType, information));
            pattern = "__FSOutputType__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.FSOutputType, information));
            pattern = "__VertexShaderName__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.VertexShaderName, information));
            pattern = "__FragmentShaderName__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.FragmentShaderName, information));
            pattern = "__ShaderName__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.ShaderName, information));
            pattern = "__LerpCode__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.LerpCode, information));
            pattern = "__ToScreenCode__";
            code = Regex.Replace(code, pattern, GetCode(InsertType.ToScreenCode, information));
            return code;
        }

        private static string GetCode(InsertType type, ShaderInformation information)
        {
            string code;
            switch (type)
            {
                case InsertType.CreateVSInputStruct1:
                    code = $"{information.VSInputType} vi1 = new {information.VSInputType}(){{";
                    foreach (KeyValuePair<string, string> item in information.VSInputFields)
                        if (item.Key == "Position") code += "Position = v1.Point,";
                        else if (item.Key == "Color") code += "Color = v1.Color,";
                        else code += $"{item.Key} = new {item.Value}(),";
                    code += "};";
                    return code;
                case InsertType.CreateVSInputStruct2:
                    code = $"{information.VSInputType} vi2 = new {information.VSInputType}(){{";
                    foreach (KeyValuePair<string, string> item in information.VSInputFields)
                        if (item.Key == "Position") code += "Position = v2.Point,";
                        else if (item.Key == "Color") code += "Color = v2.Color,";
                        else code += $"{item.Key} = new {item.Value}(),";
                    code += "};";
                    return code;
                case InsertType.CreateVSInputStruct3:
                    code = $"{information.VSInputType} vi3 = new {information.VSInputType}(){{";
                    foreach (KeyValuePair<string, string> item in information.VSInputFields)
                        if (item.Key == "Position") code += "Position = v3.Point,";
                        else if (item.Key == "Color") code += "Color = v3.Color,";
                        else code += $"{item.Key} = new {item.Value}(),";
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
                case InsertType.LerpCode:
                    code = $"return new {information.VSOutputType}(){{";
                    foreach (KeyValuePair<string, string> item in information.VSOutputFields)
                        code += $"{item.Key} = {(item.Value == "float" ? "" : item.Value + ".")}Lerp(a.{item.Key}, b.{item.Key}, t),";
                    code += "};";
                    return code;
                case InsertType.ToScreenCode:
                    code = $"return new {information.VSOutputType}(){{";
                    foreach (KeyValuePair<string, string> item in information.VSOutputFields)
                        code += $"{item.Key} = {(item.Key == "Position" ? "ToScreen(pos.Position)" : "pos." + item.Key)},";
                    code += "};";
                    return code;
                default:
                    break;
            }
            return null;
        }


    }
}

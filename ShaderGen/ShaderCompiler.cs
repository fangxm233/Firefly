using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace ShaderGen
{
    class ShaderCompiler
    {
        public static Assembly Compile(string[] references, string name, params string[] codes)
        {
            string systemAsm = typeof(object).GetTypeInfo().Assembly.Location;
            string attributeAsm = typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).GetTypeInfo().Assembly.Location;
            string vectorAsm = typeof(Vector3).GetTypeInfo().Assembly.Location;
            string consoleAsm = typeof(Console).GetTypeInfo().Assembly.Location;
            SyntaxTree[] trees = new SyntaxTree[codes.Length];
            for (int i = 0; i < codes.Length; i++)
                trees[i] = SyntaxFactory.ParseSyntaxTree(codes[i]);
            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
#if !DEBUG
            options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);
#endif
            CSharpCompilation compilation = CSharpCompilation.Create(name)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile("ShaderLib.dll"),
                MetadataReference.CreateFromFile(systemAsm),
                MetadataReference.CreateFromFile(attributeAsm),
                MetadataReference.CreateFromFile(vectorAsm),
                MetadataReference.CreateFromFile(consoleAsm))
                .AddSyntaxTrees(trees);
            foreach (string item in references)
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(item));
            string path = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), codes.GetHashCode().ToString());
            EmitResult result = compilation.Emit(path);
            if (!result.Success)
            {
                foreach (Diagnostic item in result.Diagnostics)
                {
                    if (item.Severity == DiagnosticSeverity.Error)
                        Console.WriteLine(item);
                }
                return null;
            }
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }
    }
}

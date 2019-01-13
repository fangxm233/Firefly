using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
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

            Encoding encoding = Encoding.UTF8;

            string assemblyName = Path.GetRandomFileName();
            string symbolsName = Path.ChangeExtension(assemblyName, "pdb");

            List<EmbeddedText> embeddedTexts = new List<EmbeddedText>();
            List<SyntaxTree> encoded = new List<SyntaxTree>();

            foreach (string code in codes)
            {
                string sourceCodePath = code.GetHashCode() + ".cs";

                byte[] buffer = encoding.GetBytes(code);
                SourceText sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
                    sourceText,
                    new CSharpParseOptions(),
                    path: sourceCodePath);

                encoded.Add(CSharpSyntaxTree.Create(syntaxTree.GetRoot() as CSharpSyntaxNode, null, sourceCodePath, encoding));
                embeddedTexts.Add(EmbeddedText.FromSource(sourceCodePath, sourceText));
            }

            OptimizationLevel optimizationLevel = OptimizationLevel.Debug;
#if !DEBUG
            optimizationLevel = OptimizationLevel.Release;
#endif

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: encoded,
                references: new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile("ShaderLib.dll"),
                    MetadataReference.CreateFromFile(systemAsm),
                    MetadataReference.CreateFromFile(attributeAsm),
                    MetadataReference.CreateFromFile(vectorAsm),
                    MetadataReference.CreateFromFile(consoleAsm)
                },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(optimizationLevel)
                    .WithPlatform(Platform.AnyCpu)
            );
            foreach (string item in references)
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(item));

            using (MemoryStream assemblyStream = new MemoryStream())
            using (MemoryStream symbolsStream = new MemoryStream())
            {
                EmitOptions emitOptions = new EmitOptions(
                        debugInformationFormat: DebugInformationFormat.PortablePdb,
                        pdbFilePath: symbolsName);

                EmitResult result = compilation.Emit(
                    peStream: assemblyStream,
                    pdbStream: symbolsStream,
                    embeddedTexts: embeddedTexts,
                    options: emitOptions);

                if (!result.Success)
                {
                    foreach (Diagnostic item in result.Diagnostics)
                        if (item.Severity == DiagnosticSeverity.Error)
                            Console.WriteLine(item);
                    return null;
                }

                assemblyStream.Seek(0, SeekOrigin.Begin);
                symbolsStream?.Seek(0, SeekOrigin.Begin);

                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream, symbolsStream);
                return assembly;
            }
        }
    }
}

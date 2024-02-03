using Katuusagi.SourceGeneratorCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Katuusagi.GenericEnhance.SourceGenerator
{
    [Generator]
    public class TypeDefGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            ContextUtils.InitLog<VariadicGenerator>(context);
            try
            {
                if (!context.IsReferencedAssembly("Katuusagi.GenericEnhance"))
                {
                    return;
                }

                Analyze(context);
            }
            catch (Exception e)
            {
                ContextUtils.LogException(e);
            }
        }

        private void Analyze(GeneratorExecutionContext context)
        {
            var typeGroups = context.Compilation.SyntaxTrees
                                    .OrderBy(v => v.FilePath)
                                    .SelectMany(v => v.GetCompilationUnitRoot().GetStructuredTypes())
                                    .GroupBy(v => v.GetFullName());
            foreach (var typeGroup in typeGroups)
            {
                foreach (var type in typeGroup)
                {
                    var typeDefNames = type.GetTypeNames("Katuusagi.GenericEnhance", "TypeDef").ToArray();
                    var voidNames = type.GetTypeNames("System", "Void").Append("void").ToImmutableHashSet();
                    var typeDef = type.GetAttribute(typeDefNames);
                    if (typeDef == null)
                    {
                        continue;
                    }

                    typeDef.TryGetArgumentValue("type", 0, string.Empty, out var typeDefName);
                    if (voidNames.Contains(typeDefName))
                    {
                        ContextUtils.LogError("GENERICENHANCE2001", "GenericEnhance failed", "\"System.Void\" cannot be specified in TypeDef.", typeDef);
                    }
                }
            }

            return;
        }
    }
}

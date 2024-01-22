using Katuusagi.CSharpScriptGenerator;
using Katuusagi.GenericEnhance.SourceGenerator.Utils;
using Katuusagi.SourceGeneratorCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace Katuusagi.GenericEnhance.SourceGenerator
{
    [Generator]
    public class VariadicGenerator : ISourceGenerator
    {
        private struct RootInfo
        {
            public List<TypeInfo> TypeInfos;
        }

        private struct TypeInfo
        {
            public AncestorInfo[] Ancestors;
            public string[] Generics;
            public string[] Usings;
            public ModifierType Modifier;
            public string NameSpace;
            public string Name;
            public List<MethodInfo> Methods;

            public string FileName
            {
                get
                {
                    var result = Name;
                    if (!string.IsNullOrEmpty(AncestorPath))
                    {
                        result = $"{AncestorPath}-{result}";
                    }

                    if (!string.IsNullOrEmpty(NameSpace))
                    {
                        result = $"{NameSpace}.{result}";
                    }

                    return $"{result}.GenericEnhance.Generated";
                }
            }

            public string AncestorPath => string.Join("-", Ancestors.Select(v => v.Name));
        }

        private struct AncestorInfo
        {
            public ModifierType Modifier;
            public string Name;
            public string[] Generics;
        }

        private struct MethodInfo
        {
            public string Id;
            public string[] Attributes;
            public ModifierType Modifier;
            public string Name;
            public string ReturnType;
            public byte TypeParameterMin;
            public byte TypeParameterMax;
            public bool HasVariadicParameter;
            public List<GenericParameterInfo> GenericParameters;
            public List<ParameterInfo> Parameters;
            public List<StatementInfo> Statements;
        }

        private class StatementInfo
        {
            public bool ForceHasChild { get; set; } = true;
            public bool IsExpand { get; set; } = true;
            public bool IsForEach { get; set; } = false;
            public string Statement { get; set; } = string.Empty;
            public List<StatementInfo> Children { get; } = new List<StatementInfo>();
        }

        private struct GenericParameterInfo
        {
            public string[] Attributes;
            public ModifierType Modifier;
            public string[] Wheres;
            public string Type;
        }

        private struct ParameterInfo
        {
            public string[] Attributes;
            public ModifierType Modifier;
            public string Type;
            public string Name;
            public string Default;
        }

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

                var rootInfo = Analyze(context);
                foreach (var typeInfo in rootInfo.TypeInfos)
                {
                    var root = new RootGenerator();
                    root.Generate(rg =>
                    {
                        foreach (var @using in typeInfo.Usings)
                        {
                            rg.Using.Generate(@using);
                        }

                        if (string.IsNullOrEmpty(typeInfo.NameSpace))
                        {
                            GenerateAncestor(rg.Type, typeInfo, typeInfo.Ancestors);
                            return;
                        }

                        rg.Namespace.Generate(typeInfo.NameSpace, ng =>
                        {
                            GenerateAncestor(ng.Type, typeInfo, typeInfo.Ancestors);
                        });
                    });
                    var builder = new CSharpScriptBuilder();
                    builder.BuildAndNewLine(root.Result);
                    context.AddSource($"{typeInfo.FileName}.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                ContextUtils.LogException(e);
            }
        }

        private void GenerateAncestor(TypeGenerator typeGenerator, TypeInfo typeInfo, IEnumerable<AncestorInfo> ancestors)
        {
            if (!ancestors.Any())
            {
                GenerateType(typeGenerator, typeInfo);
                return;
            }

            var ancestor = ancestors.First();
            typeGenerator.Generate(ancestor.Modifier, ancestor.Name, tg =>
            {
                foreach (var generic in ancestor.Generics)
                {
                    tg.GenericParam.Generate(generic);
                }
                GenerateAncestor(tg.Type, typeInfo, ancestors.Skip(1));
            });
        }

        private void GenerateType(TypeGenerator typeGenerator, TypeInfo typeInfo)
        {
            typeGenerator.Generate(typeInfo.Modifier, typeInfo.Name, tg =>
            {
                foreach (var generic in typeInfo.Generics)
                {
                    tg.GenericParam.Generate(generic);
                }

                foreach (var method in typeInfo.Methods)
                {
                    var variandicGenericParam = method.GenericParameters.LastOrDefault().Type;
                    var variandicGenericParamReplace = new Regex($"\\b{variandicGenericParam}\\b");
                    var variandicParam = string.Empty;
                    var variandicParamReplace = default(Regex);
                    if (method.HasVariadicParameter && method.Parameters.Any())
                    {
                        variandicParam = method.Parameters.LastOrDefault().Name;
                        variandicParamReplace = new Regex($"\\b{variandicParam}\\b");
                    }

                    for (int typeParameterCount = method.TypeParameterMin; typeParameterCount < method.TypeParameterMax + 1; ++typeParameterCount)
                    {
                        var modifier = method.Modifier;
                        if (typeParameterCount == 1)
                        {
                            continue;
                        }

                        var variadicGenericParamAll = string.Empty;
                        for (int j = 0; j < typeParameterCount; ++j)
                        {
                            variadicGenericParamAll = $"{variadicGenericParamAll}{variandicGenericParam}__{j}, ";
                        }

                        if (!string.IsNullOrEmpty(variadicGenericParamAll))
                        {
                            variadicGenericParamAll = variadicGenericParamAll.Remove(variadicGenericParamAll.Length - 2, 2);
                        }
                        else
                        {
                            variadicGenericParamAll = "global::Katuusagi.GenericEnhance.NoneType";
                        }

                        var returnType = variandicGenericParamReplace.Replace(method.ReturnType, variadicGenericParamAll);
                        tg.Method.Generate(modifier, returnType, method.Name, mg =>
                        {
                            mg.Attribute.Generate($"global::Katuusagi.GenericEnhance.VariadicGenerated({typeParameterCount})");
                            foreach (var attribute in method.Attributes)
                            {
                                mg.Attribute.Generate(attribute);
                            }

                            foreach (var genericParameter in method.GenericParameters.Take(method.GenericParameters.Count - 1))
                            {
                                mg.GenericParam.Generate(genericParameter.Type, gg =>
                                {
                                    foreach (var attribute in genericParameter.Attributes)
                                    {
                                        gg.Attribute.Generate(attribute);
                                    }

                                    foreach (var wh in genericParameter.Wheres)
                                    {
                                        gg.Where.Generate(wh);
                                    }
                                });
                            }

                            IEnumerable<ParameterInfo> parameters = method.Parameters;
                            ParameterInfo lastParameter = default;
                            if (method.HasVariadicParameter)
                            {
                                parameters = parameters.Take(method.Parameters.Count - 1);
                                lastParameter = method.Parameters.LastOrDefault();
                            }

                            foreach (var parameter in parameters)
                            {
                                var parameterType = variandicGenericParamReplace.Replace(parameter.Type, variadicGenericParamAll);
                                mg.Param.Generate(parameter.Modifier, parameterType, parameter.Name, pg =>
                                {
                                    foreach (var attribute in parameter.Attributes)
                                    {
                                        pg.Attribute.Generate(attribute);
                                    }

                                    if (!string.IsNullOrEmpty(parameter.Default))
                                    {
                                        pg.Default.Generate(parameter.Default);
                                    }
                                });
                            }

                            var lastGenericParameter = method.GenericParameters.Last();
                            for (int i = 0; i < typeParameterCount; ++i)
                            {
                                mg.GenericParam.Generate($"{lastGenericParameter.Type}__{i}", gg =>
                                {
                                    foreach (var attribute in lastGenericParameter.Attributes)
                                    {
                                        gg.Attribute.Generate(attribute);
                                    }

                                    foreach (var wh in lastGenericParameter.Wheres)
                                    {
                                        gg.Where.Generate(wh);
                                    }
                                });

                                if (method.HasVariadicParameter)
                                {
                                    mg.Param.Generate(lastParameter.Modifier, $"{lastParameter.Type}__{i}", $"{lastParameter.Name}__{i}", pg =>
                                    {
                                        foreach (var attribute in lastParameter.Attributes)
                                        {
                                            pg.Attribute.Generate(attribute);
                                        }

                                        if (!string.IsNullOrEmpty(lastParameter.Default))
                                        {
                                            pg.Default.Generate(lastParameter.Default);
                                        }
                                    });
                                }
                            }

                            foreach (var statement in method.Statements)
                            {
                                GenerateStatement(typeParameterCount, 0, variandicGenericParam, variandicParam, variandicGenericParamReplace, variandicParamReplace, mg.Statement, statement);
                            }
                        });
                    }
                }
            });
        }

        private void GenerateStatement(int count, int forEachIndex, string variadicGenericParam, string variadicParam, Regex variadicGenericParamReplace, Regex variadicParamReplace, StatementGenerator gen, StatementInfo statement)
        {
            var countMax = statement.IsForEach ? count : 1;
            for (int i = 0; i < countMax; ++i)
            {
                if (statement.IsForEach)
                {
                    forEachIndex = i;
                }

                var statementStr = statement.Statement;
                if (statement.IsExpand)
                {
                    var variadicGenericParamAll = string.Empty;
                    var variadicParamAll = string.Empty;
                    for (int j = 0; j < count; ++j)
                    {
                        variadicGenericParamAll = $"{variadicGenericParamAll}{variadicGenericParam}__{j}, ";
                        if (variadicParamReplace != null)
                        {
                            variadicParamAll = $"{variadicParamAll}{variadicParam}__{j}, ";
                        }
                    }

                    if (!string.IsNullOrEmpty(variadicGenericParamAll))
                    {
                        variadicGenericParamAll = variadicGenericParamAll.Remove(variadicGenericParamAll.Length - 2, 2);
                    }
                    else
                    {
                        variadicGenericParamAll = "global::Katuusagi.GenericEnhance.NoneType";
                    }

                    if (!string.IsNullOrEmpty(variadicParamAll))
                    {
                        variadicParamAll = variadicParamAll.Remove(variadicParamAll.Length - 2, 2);
                    }
                    else
                    {
                        variadicParamAll = "global::Katuusagi.GenericEnhance.NoneType.Default";
                    }

                    statementStr = variadicGenericParamReplace.Replace(statementStr, variadicGenericParamAll);
                    if (variadicParamReplace != null)
                    {
                        statementStr = variadicParamReplace.Replace(statementStr, variadicParamAll);
                    }
                }
                else
                {
                    statementStr = variadicGenericParamReplace.Replace(statementStr, $"{variadicGenericParam}__{forEachIndex}");
                    if (variadicParamReplace != null)
                    {
                        statementStr = variadicParamReplace.Replace(statementStr, $"{variadicParam}__{forEachIndex}");
                    }
                }

                if (statement.Children.Any() || statement.ForceHasChild)
                {
                    gen.Generate(statementStr, () =>
                    {
                        foreach (var child in statement.Children)
                        {
                            GenerateStatement(count, forEachIndex, variadicGenericParam, variadicParam, variadicGenericParamReplace, variadicParamReplace, gen, child);
                        }
                    });
                }
                else
                {
                    gen.Generate(statementStr);
                }

                if (statement.IsForEach)
                {
                    gen.Generate($"global::Katuusagi.GenericEnhance.VariadicUtils.ContinueTarget();");
                }
            }
        }

        private RootInfo Analyze(GeneratorExecutionContext context)
        {
            var result = new RootInfo()
            {
                TypeInfos = new List<TypeInfo>(),
            };

            var typeGroups = context.Compilation.SyntaxTrees
                                    .OrderBy(v => v.FilePath)
                                    .SelectMany(v => v.GetCompilationUnitRoot().GetStructuredTypes())
                                    .GroupBy(v => v.GetFullName());
            foreach (var typeGroup in typeGroups)
            {
                ModifierType typeModifier = ModifierType.None;
                foreach (var type in typeGroup)
                {
                    typeModifier |= ScriptGeneratorUtils.GetModifierType(type.Keyword.Text);
                    typeModifier |= ScriptGeneratorUtils.GetModifierType(type.Modifiers.ToString());
                }

                var firstType = typeGroup.First();
                var ancestors = firstType.GetAncestors<TypeDeclarationSyntax>().Reverse().Select(v =>
                {
                    var ancestorGenerics = Array.Empty<string>();
                    if (v.TypeParameterList != null)
                    {
                        ancestorGenerics = v.TypeParameterList.Parameters.Select(v2 => v2.ToString()).ToArray();
                    }
                    return new AncestorInfo()
                    {
                        Modifier = ScriptGeneratorUtils.GetModifierType(v.Keyword.Text) | ScriptGeneratorUtils.GetModifierType(v.Modifiers.ToString()),
                        Name = v.Identifier.ToString(),
                        Generics = ancestorGenerics,
                    };
                }).ToArray();

                var generics = Array.Empty<string>();
                if (firstType.TypeParameterList != null)
                {
                    generics = firstType.TypeParameterList.Parameters.Select(v => v.ToString()).ToArray();
                }

                var usings = typeGroup.SelectMany(v => v.GetAncestorUsings()).Select(v =>
                {
                    var usingName = v.Name.ToString();
                    if (!string.IsNullOrEmpty(v.StaticKeyword.Text))
                    {
                        usingName = $"{v.StaticKeyword.Text} {usingName}";
                    }

                    if (!string.IsNullOrEmpty(v.Alias?.Name?.ToString()))
                    {
                        usingName = $"{v.Alias.Name} = {usingName}";
                    }

                    return usingName;
                }).Distinct().OrderBy(v => v).ToArray();

                var typeInfo = new TypeInfo()
                {
                    Modifier = typeModifier,
                    Ancestors = ancestors,
                    NameSpace = firstType.GetNameSpace(),
                    Name = firstType.Identifier.ToString(),
                    Generics = generics,
                    Usings = usings,
                    Methods = new List<MethodInfo>(),
                };

                foreach (var type in typeGroup)
                {
                    var variadicGenericNames = type.GetTypeNames("Katuusagi.GenericEnhance", "VariadicGeneric").ToArray();
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        if (method.TypeParameterList == null)
                        {
                            continue;
                        }

                        var typeParameters = method.TypeParameterList.Parameters;
                        if (typeParameters == null || !typeParameters.Any())
                        {
                            continue;
                        }

                        var variadicType = method.GetAttribute(variadicGenericNames);
                        if (variadicType == null)
                        {
                            continue;
                        }

                        bool isTypePartial = type.IsPerfectPartial();
                        if (!isTypePartial)
                        {
                            ContextUtils.LogError("GENERICENHANCE4001", "GenericEnhance failed.", "Variadic type parameters can only be set for members of partial type.", type);
                            continue;
                        }

                        byte typeParameterMin = 0;
                        byte typeParameterMax = 0;
                        if (variadicType.ArgumentList.Arguments.Count == 2)
                        {
                            variadicType.TryGetArgumentValue("typeParameterMin", 0, 0, out typeParameterMin);
                            variadicType.TryGetArgumentValue("typeParameterMax", 1, 0, out typeParameterMax);
                        }
                        else
                        {
                            variadicType.TryGetArgumentValue("typeParameterMax", 0, 0, out typeParameterMax);
                        }

                        var lastTypeParameter = typeParameters.LastOrDefault();
                        bool hasVariadicParameter = false;
                        if (method.ParameterList != null)
                        {
                            var lastType = lastTypeParameter.Identifier.ToString();
                            var takedParameters = method.ParameterList.Parameters.Take(method.ParameterList.Parameters.Count - 1);
                            foreach (var parameter in takedParameters)
                            {
                                if (parameter.Type.ToString() == lastType)
                                {
                                    ContextUtils.LogError("GENERICENHANCE4002", "GenericEnhance failed.", $"\"{lastType}\" can only be set for tail of parameters.", parameter);
                                }
                            }

                            var lastParameter = method.ParameterList.Parameters.LastOrDefault();
                            if (lastParameter != null &&
                                lastParameter.AttributeLists != null)
                            {
                                hasVariadicParameter = lastParameter.Type.ToString() == lastType;
                            }
                        }

                        var methodName = method.Identifier.ToString();
                        var methodModifier = ScriptGeneratorUtils.GetModifierType(method.Modifiers.ToString());
                        var methodInfo = new MethodInfo()
                        {
                            Id = Guid.NewGuid().ToString().Replace("-", string.Empty),
                            Attributes = method.GetAttributes().Select(v2 => v2.ToString()).ToArray(),
                            Name = methodName,
                            ReturnType = method.ReturnType.ToString(),
                            Modifier = methodModifier,
                            HasVariadicParameter = hasVariadicParameter,
                            TypeParameterMin = typeParameterMin,
                            TypeParameterMax = typeParameterMax,
                            GenericParameters = new List<GenericParameterInfo>(),
                            Parameters = new List<ParameterInfo>(),
                            Statements = new List<StatementInfo>(),
                        };

                        if (method.Body != null)
                        {
                            foreach (var statement in method.Body.Statements)
                            {
                                GetStatement(method, statement, true, methodInfo.Statements);
                            }
                        }

                        if (method.TypeParameterList != null)
                        {
                            foreach (var parameter in method.TypeParameterList.Parameters)
                            {
                                var typeParameterName = parameter.Identifier.ToString();
                                var parameterInfo = new GenericParameterInfo()
                                {
                                    Attributes = parameter.GetAttributes().Select(v2 => v2.ToString()).ToArray(),
                                    Modifier = ScriptGeneratorUtils.GetModifierType(parameter.VarianceKeyword.ToString()),
                                    Wheres = method.ConstraintClauses.Where(v => v.Name.ToString() == typeParameterName).SelectMany(v => v.Constraints).Select(v => v.ToString()).ToArray(),
                                    Type = typeParameterName,
                                };

                                methodInfo.GenericParameters.Add(parameterInfo);
                            }
                        }

                        if (method.ParameterList != null)
                        {
                            foreach (var parameter in method.ParameterList.Parameters)
                            {
                                var parameterInfo = new ParameterInfo()
                                {
                                    Attributes = parameter.GetAttributes().Select(v2 => v2.ToString()).ToArray(),
                                    Modifier = ScriptGeneratorUtils.GetModifierType(parameter.Modifiers.ToString()),
                                    Type = parameter.Type.ToString(),
                                    Name = parameter.Identifier.ToString(),
                                    Default = parameter.Default?.Value?.ToString() ?? string.Empty,
                                };

                                methodInfo.Parameters.Add(parameterInfo);
                            }
                        }

                        typeInfo.Methods.Add(methodInfo);
                    }
                }

                if (typeInfo.Methods.Any())
                {
                    typeInfo.Methods = typeInfo.Methods.OrderBy(v => v.Name).ToList();
                    result.TypeInfos.Add(typeInfo);
                }
            }

            return result;
        }

        private void GetStatement(MethodDeclarationSyntax method, SyntaxNode statement, bool isExpand, List<StatementInfo> results)
        {
            if (statement is BlockSyntax block)
            {
                var result = new StatementInfo()
                {
                    Statement = "",
                    IsExpand = isExpand,
                };

                foreach (var child in block.ChildNodes())
                {
                    GetStatement(method, child, isExpand, result.Children);
                }

                results.Add(result);
                return;
            }

            if (statement is UsingStatementSyntax @using)
            {
                SyntaxNode use = @using.Declaration;
                if (use == null)
                {
                    use = @using.Expression;
                }

                var scopes = use.DescendantNodes().OfType<IdentifierNameSyntax>();
                if (scopes.Any(v => v.ToString() == "VariadicForEach"))
                {
                    string usingVar = string.Empty;
                    if (use is VariableDeclarationSyntax variadicForEach)
                    {
                        usingVar = variadicForEach.Variables.FirstOrDefault()?.Identifier.ToString();
                    }

                    StatementInfo result = new StatementInfo()
                    {
                        Statement = $"using ({use})",
                        IsExpand = false,
                    };

                    var children = result.Children;
                    var newBlock = new StatementInfo()
                    {
                        IsForEach = true,
                        IsExpand = false,
                        Statement = "",
                    };
                    children.Add(newBlock);
                    children = newBlock.Children;

                    GetChildStatement(method, @using.Statement, false, children);
                    results.Add(result);
                    return;
                }

                if (scopes.Any(v => v.ToString() == "ExpandVariadicParameters"))
                {
                    isExpand = true;
                }

                if (scopes.Any(v => v.ToString() == "PickVariadicParameter"))
                {
                    isExpand = false;
                }
            }

            {
                var identifiers = statement.DescendantNodes().OfType<IdentifierNameSyntax>().Where(v =>
                {
                    var name = v.ToString();
                    return name == "VariadicForEach" ||
                           name == "ExpandVariadicParameters" ||
                           name == "PickVariadicParameter";
                });
                foreach (var identifier in identifiers)
                {
                    ContextUtils.LogError("GENERICENHANCE4003", "GenericEnhance failed.", $"\"{identifier}\" can only be implemented with a using statement.", identifier);
                }
            }
            GetOtherStatement(method, statement, isExpand, results);
        }

        private void GetChildStatement(MethodDeclarationSyntax method, SyntaxNode child, bool isExpand, List<StatementInfo> results)
        {
            if (child is BlockSyntax childBlock)
            {
                foreach (var blockChild in childBlock.ChildNodes())
                {
                    GetStatement(method, blockChild, isExpand, results);
                }
            }
            else
            {
                GetStatement(method, child, isExpand, results);
            }
        }

        private void GetOtherStatement(MethodDeclarationSyntax method, SyntaxNode statement, bool isExpand, List<StatementInfo> results)
        {
            if (!statement.DescendantNodes().OfType<BlockSyntax>().Any())
            {
                var splited = statement.ToString().Replace("\r", string.Empty).Split('\n').Select(v => v.Trim());
                foreach (var str in splited)
                {
                    results.Add(new StatementInfo()
                    {
                        Statement = str,
                        IsExpand = isExpand,
                        ForceHasChild = false,
                    });
                }
                return;
            }

            var start = statement.SpanStart;
            GetOtherStatement(method, statement, isExpand, ref start, results);
            GetSpanStatement(method, start, statement.Span.End, isExpand, results);
        }

        private void GetOtherStatement(MethodDeclarationSyntax method, SyntaxNode statement, bool isExpand, ref int start, List<StatementInfo> results)
        {
            foreach (var child in statement.ChildNodes())
            {
                if (!(child is BlockSyntax block))
                {
                    GetOtherStatement(method, child, isExpand, ref start, results);
                    continue;
                }

                GetSpanStatement(method, start, block.SpanStart, isExpand, results);
                start = block.Span.End;
                GetStatement(method, block, isExpand, results);
            }
        }

        private static void GetSpanStatement(MethodDeclarationSyntax method, int start, int end, bool isExpand, List<StatementInfo> results)
        {
            var methodSource = method.ToString();
            var startIndex = start - method.SpanStart;
            var endIndex = end - method.SpanStart;

            var splited = methodSource.Substring(startIndex, endIndex - startIndex).Replace("\r", string.Empty).Split('\n').Select(v => v.Trim());
            foreach (var str in splited)
            {
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }

                results.Add(new StatementInfo()
                {
                    Statement = str,
                    IsExpand = isExpand,
                    ForceHasChild = false,
                });
            }
        }
    }
}

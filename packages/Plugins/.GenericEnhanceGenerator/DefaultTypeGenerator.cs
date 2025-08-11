using Katuusagi.CSharpScriptGenerator;
using Katuusagi.SourceGeneratorCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Katuusagi.GenericEnhance.SourceGenerator
{
    [Generator]
    public class DefaultTypeGenerator : ISourceGenerator
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

            public string AncestorPath
            {
                get
                {
                    var ancestorPath = string.Concat(Ancestors.Select(v => $"{v.Name}-"));
                    if (!string.IsNullOrEmpty(ancestorPath))
                    {
                        ancestorPath = ancestorPath.Remove(ancestorPath.Length - 1);
                    }

                    return ancestorPath;
                }
            }
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
            public List<GenericParameterInfo> GenericParameters;
            public List<ParameterInfo> Parameters;

            public bool HasReturn => !string.IsNullOrEmpty(ReturnType) && ReturnType != "void" && ReturnType != "Void" && ReturnType != "System.Void" && ReturnType != "global::System.Void";

            public string ArgumentString
            {
                get
                {
                    return Parameters.Select(v => v.Name).JoinParameters();
                }
            }

            public string GetCall(int count)
            {
                var genericArgumentString = GetGenericArgumentString(count);
                if (HasReturn)
                {
                    return $"return {Name}<{genericArgumentString}>({ArgumentString});";
                }
                else
                {
                    return $"{Name}<{genericArgumentString}>({ArgumentString});";
                }
            }

            public string GetGenericArgumentString(int count)
            {
                var before = GenericParameters.Take(count).Select(v => v.Type);
                var after = GenericParameters.Skip(count).Select(v => v.Default);
                return before.Concat(after).JoinParameters();
            }
        }

        private struct GenericParameterInfo
        {
            public string[] Attributes;
            public ModifierType Modifier;
            public string[] Wheres;
            public string Type;
            public string Default;

            public bool HasDefault => !string.IsNullOrEmpty(Default);
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
            ContextUtils.InitLog<DefaultTypeGenerator>(context);
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
                    var max = method.GenericParameters.Count;
                    var min = method.GenericParameters.Where(v => !v.HasDefault).Count();

                    for (int genericParameterCount = min; genericParameterCount < max; ++genericParameterCount)
                    {
                        var generics = method.GenericParameters.Take(genericParameterCount).ToArray();
                        var defaultGenerics = method.GenericParameters.Skip(genericParameterCount).ToArray();

                        var returnType = defaultGenerics.FirstOrDefault(v => v.Type == method.ReturnType).Default;
                        if (string.IsNullOrEmpty(returnType))
                        {
                            returnType = method.ReturnType;
                        }
                        tg.Method.Generate(method.Modifier, returnType, method.Name, mg =>
                        {
                            foreach (var attribute in method.Attributes)
                            {
                                mg.Attribute.Generate(attribute);
                            }

                            foreach (var defaultGeneric in defaultGenerics)
                            {
                                mg.Attribute.Generate(ag =>
                                {
                                    ag.Type.Generate("global::Katuusagi.GenericEnhance.SourceDefaultType");
                                    ag.Arg.Generate($"typeof({defaultGeneric.Default})");
                                });
                            }

                            foreach (var parameter in method.Parameters)
                            {
                                mg.Attribute.Generate(ag =>
                                {
                                    ag.Type.Generate("global::Katuusagi.GenericEnhance.SourceArgumentType");
                                    if (!method.GenericParameters.Any(v => v.Type == parameter.Type))
                                    {
                                        ag.Arg.Generate($"typeof({parameter.Type})");
                                    }
                                    else
                                    {
                                        ag.Arg.Generate($"\"{parameter.Type}\"");
                                    }
                                });
                            }

                            mg.Attribute.Generate(ag =>
                            {
                                ag.Type.Generate("global::System.Runtime.CompilerServices.MethodImpl");
                                ag.Arg.Generate("global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining");
                            });

                            foreach (var genericParameter in generics)
                            {
                                mg.GenericParam.Generate(genericParameter.Modifier, genericParameter.Type, gg =>
                                {
                                    foreach (var attribute in genericParameter.Attributes)
                                    {
                                        gg.Attribute.Generate(attribute);
                                    }

                                    foreach (var where in genericParameter.Wheres)
                                    {
                                        gg.Where.Generate(where);
                                    }
                                });
                            }

                            foreach (var parameter in method.Parameters)
                            {
                                var type = defaultGenerics.FirstOrDefault(v => v.Type == parameter.Type).Default;
                                if (string.IsNullOrEmpty(type))
                                {
                                    type = parameter.Type;
                                }
                                mg.Param.Generate(parameter.Modifier, type, parameter.Name, pg =>
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

                            mg.Statement.Generate(method.GetCall(genericParameterCount));
                        });
                    }
                }
            });
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
                    var defaultGenericNames = type.GetTypeNames("Katuusagi.GenericEnhance", "DefaultType").ToArray();
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        if (method.TypeParameterList == null ||
                            !method.TypeParameterList.Parameters.Any())
                        {
                            continue;
                        }

                        var typeParameters = method.TypeParameterList.Parameters;
                        if (!typeParameters.Any(v => v.GetAttribute(defaultGenericNames) != null))
                        {
                            continue;
                        }

                        bool isTypePartial = type.IsPerfectPartial();
                        if (!isTypePartial)
                        {
                            ContextUtils.LogError("GENERICENHANCE3001", "GenericEnhance failed.", "Default generic parameter can only be set for members of partial type.", type);
                            continue;
                        }

                        var defaultIndex = typeParameters.Select((v, i) => (v, i)).FirstOrDefault(v => v.v.GetAttribute(defaultGenericNames) != null).i;
                        if (!typeParameters.Skip(defaultIndex).All(v => v.GetAttribute(defaultGenericNames) != null))
                        {
                            ContextUtils.LogError("GENERICENHANCE3002", "GenericEnhance failed.", "Default generic parameters must appear after all required parameters.", typeParameters.ElementAt(defaultIndex).GetAttribute(defaultGenericNames));
                            continue;
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
                            GenericParameters = new List<GenericParameterInfo>(),
                            Parameters = new List<ParameterInfo>(),
                        };

                        foreach (var parameter in typeParameters)
                        {
                            var defaultGeneric = parameter.GetAttribute(defaultGenericNames);
                            string defaultType = null;
                            defaultGeneric?.TryGetArgumentValue("defaultType", 0, string.Empty, out defaultType);

                            var typeParameterName = parameter.Identifier.ToString();
                            var parameterInfo = new GenericParameterInfo()
                            {
                                Attributes = parameter.GetAttributes().Select(v2 => v2.ToString()).ToArray(),
                                Modifier = ScriptGeneratorUtils.GetModifierType(parameter.VarianceKeyword.Text),
                                Wheres = method.ConstraintClauses.Where(v => v.Name.ToString() == typeParameterName).SelectMany(v => v.Constraints).Select(v => v.ToString()).ToArray(),
                                Type = typeParameterName,
                                Default = defaultType,
                            };

                            methodInfo.GenericParameters.Add(parameterInfo);
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
    }
}

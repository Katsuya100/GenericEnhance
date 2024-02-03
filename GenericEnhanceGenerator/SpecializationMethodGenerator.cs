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
using System.Text.RegularExpressions;

namespace Katuusagi.GenericEnhance.SourceGenerator
{
    public enum SpecializeAlgorithm
    {
        VirtualStrategy,
        DelegateStrategy,
        TypeComparison,
        TypeIdComparison,
    }

    [Generator]
    public class SpecializationMethodGenerator : ISourceGenerator
    {
        private struct SpecializeInfo
        {
            public string SpecialMethod;
            public Dictionary<string, BindType> BindTypes;

            public bool HasTypeFormulaBinds => BindTypes.Values.Any(v => v.HasTypeFormulaBinds);

            public string GenericArgumentString
            {
                get
                {
                    return BindTypes.Values.Select(v => v.Type).JoinParameters();
                }
            }

            public string DefaultComparison
            {
                get
                {
                    var conditions = BindTypes.Select(pair =>
                    {
                        if (!pair.Value.HasTypeFormulaBinds)
                        {
                            return $"typeof({pair.Key}) == typeof({pair.Value.Type})";
                        }

                        var checkConditions = pair.Value.TypeFormulaBinds.Select(v => $"global::Katuusagi.GenericEnhance.TypeFormula.GetValue<{pair.Key}, {v}>() == global::Katuusagi.GenericEnhance.TypeFormula.GetValue<{pair.Value.Type}, {v}>()");
                        var check = string.Join(" || ", checkConditions);
                        return $"({check})";
                    });

                    return string.Join(" && ", conditions);
                }
            }

            public string Comparison
            {
                get
                {
                    var conditions = BindTypes.Select(pair =>
                    {
                        if (!pair.Value.HasTypeFormulaBinds)
                        {
                            return $"typeof({pair.Key}) == typeof({pair.Value.Type})";
                        }

                        var checkConditions = pair.Value.TypeFormulaBinds.Select(v => $"global::Katuusagi.GenericEnhance.TypeFormula.GetValue<{pair.Key}, {v}>() == global::Katuusagi.GenericEnhance.TypeFormula.GetValue<{pair.Value.Type}, {v}>()");
                        var check = string.Join(" || ", checkConditions);
                        return $"(typeof({pair.Key}) == typeof({pair.Value.Type}) || {check})";
                    });

                    return string.Join(" && ", conditions);
                }
            }

            public string GetBindType(string type)
            {
                if (BindTypes.TryGetValue(type, out var ret))
                {
                    return ret.Type;
                }

                return type;
            }

            public string Call(in MethodInfo method)
            {
                return $"{SpecialMethod}({GetArgs(method)});";
            }

            public string GetArgs(in MethodInfo method)
            {
                var self = this;
                return method.Parameters.Select(v => $"{v.Modifier.GetArgumentModifierLabel()}{self.GetCastArg(v.Type, v.Name)}").JoinParameters();
            }

            public string GetCastArg(string type, string arg)
            {
                if (!BindTypes.TryGetValue(type, out var baseType))
                {
                    return arg;
                }

                return $"global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.As<{type}, {baseType.Type}>(ref {arg})";
            }

            public string GetCastResult(string type)
            {
                if (!BindTypes.TryGetValue(type, out var baseType))
                {
                    return "__result__";
                }

                return $"global::Unity.Collections.LowLevel.Unsafe.UnsafeUtility.As<{baseType.Type}, {type}>(ref __result__)";
            }
        }

        private struct BindType
        {
            public string Type;
            public string[] TypeFormulaBinds;
            public bool HasTypeFormulaBinds => TypeFormulaBinds.Any();
        }

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
            public ModifierType Modifier;
            public string Name;
            public string ReturnType;
            public SpecializeAlgorithm Algorithm;
            public string DefaultMethod;
            public List<GenericParameterInfo> GenericParameters;
            public List<ParameterInfo> Parameters;
            public List<SpecializeInfo> SpecializeInfos;

            public bool IsStatic => Modifier.HasFlag(ModifierType.Static);

            public bool HasReturn => !string.IsNullOrEmpty(ReturnType) && ReturnType != "void" && ReturnType != "Void" && ReturnType != "System.Void" && ReturnType != "global::System.Void";

            public bool HasTypeFormulaBinds => SpecializeInfos.Any(v => v.HasTypeFormulaBinds);

            public string Call
            {
                get
                {
                    return $"_SpecializeObject_{Id}_<{GenericArgumentString}>.Object.Method<{GenericArgumentString}>({ArgumentString});";
                }
            }

            public string GenericArgumentString
            {
                get
                {
                    return GenericParameters.Select(v => v.Type).JoinParameters();
                }
            }

            public string ArgumentString
            {
                get
                {
                    var result = ArgumentBaseString;
                    if (IsStatic)
                    {
                        return result;
                    }

                    if (string.IsNullOrEmpty(result))
                    {
                        return $"ref __self__";
                    }

                    return $"ref __self__, {ArgumentBaseString}";
                }
            }

            public string ArgumentBaseString
            {
                get
                {
                    return Parameters.Select(v => $"{v.Modifier.GetArgumentModifierLabel()}{v.Name}").JoinParameters();
                }
            }
        }

        private struct GenericParameterInfo
        {
            public ModifierType Modifier;
            public string[] Wheres;
            public string Type;
        }

        private struct ParameterInfo
        {
            public ModifierType Modifier;
            public string Type;
            public string Name;
            public string Default;
        }

        private static readonly Regex TypeParameterMatch = new Regex("(?<=(\\<))(.*)(?=(\\>$))");

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            ContextUtils.InitLog<SpecializationMethodGenerator>(context);
            try
            {
                if (!context.IsAvailableAssembly("Katuusagi.GenericEnhance"))
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
                    var defaultMethod = method.DefaultMethod;
                    tg.Method.Generate(method.Modifier, method.ReturnType, method.Name, mg =>
                    {
                        foreach (var generic in method.GenericParameters)
                        {
                            mg.GenericParam.Generate(generic.Type, gg =>
                            {
                                foreach (var wh in generic.Wheres)
                                {
                                    gg.Where.Generate(wh);
                                }
                            });
                        }

                        foreach (var parameter in method.Parameters)
                        {
                            mg.Param.Generate(parameter.Modifier, parameter.Type, parameter.Name, parameter.Default);
                        }

                        switch (method.Algorithm)
                        {
                            case SpecializeAlgorithm.VirtualStrategy:
                                {
                                    if (!method.IsStatic)
                                    {
                                        mg.Statement.Generate($"var __self__ = this;");
                                    }

                                    if (method.HasReturn)
                                    {
                                        mg.Statement.Generate($"var __result__ = {method.Call}");
                                        mg.Statement.Generate($"return __result__;");
                                    }
                                    else
                                    {
                                        mg.Statement.Generate(method.Call);
                                    }
                                }
                                break;
                            case SpecializeAlgorithm.DelegateStrategy:
                                {
                                    if (!method.IsStatic)
                                    {
                                        mg.Statement.Generate($"var __self__ = this;");
                                    }

                                    mg.Statement.Generate($"if (_SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.Method != null)", () =>
                                    {
                                        if (method.HasReturn)
                                        {
                                            mg.Statement.Generate($"return _SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.Method({method.ArgumentString});");
                                        }
                                        else
                                        {
                                            mg.Statement.Generate($"_SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.Method({method.ArgumentString});");
                                        }
                                    });
                                    mg.Statement.Generate($"else", () =>
                                    {
                                        if (method.HasReturn)
                                        {
                                            mg.Statement.Generate($"return {method.DefaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                                        }
                                        else
                                        {
                                            mg.Statement.Generate($"{method.DefaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                                        }
                                    });
                                }
                                break;
                            case SpecializeAlgorithm.TypeComparison:
                                {
                                    var specializeInfos = method.SpecializeInfos;
                                    foreach (var parameter in method.Parameters)
                                    {
                                        if (!parameter.Modifier.HasFlag(ModifierType.Out))
                                        {
                                            continue;
                                        }
                                        mg.Statement.Generate($"{parameter.Name} = default;");
                                    }

                                    foreach (var specializeInfo in specializeInfos)
                                    {
                                        mg.Statement.Generate($"if ({specializeInfo.Comparison})", () =>
                                        {
                                            var call = specializeInfo.Call(method);
                                            if (method.HasReturn)
                                            {
                                                mg.Statement.Generate($"var __result__ = {call}");
                                                mg.Statement.Generate($"return {specializeInfo.GetCastResult(method.ReturnType)};");
                                            }
                                            else
                                            {
                                                mg.Statement.Generate(call);
                                            }
                                        });
                                    }

                                    if (method.HasReturn)
                                    {
                                        mg.Statement.Generate($"var __defaultResult__ = {defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                                        mg.Statement.Generate($"return __defaultResult__;");
                                    }
                                    else
                                    {
                                        mg.Statement.Generate($"{defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                                    }
                                }
                                break;
                            case SpecializeAlgorithm.TypeIdComparison:
                                {
                                    var specializeInfos = method.SpecializeInfos;
                                    foreach (var parameter in method.Parameters)
                                    {
                                        if (!parameter.Modifier.HasFlag(ModifierType.Out))
                                        {
                                            continue;
                                        }
                                        mg.Statement.Generate($"{parameter.Name} = default;");
                                    }

                                    mg.Statement.Generate($"switch (_SpecializeId_{method.Id}_<{method.GenericArgumentString}>.Id)", () =>
                                    {
                                        for (int i = 0; i < specializeInfos.Count; ++i)
                                        {
                                            var specializeInfo = specializeInfos[i];
                                            mg.Statement.Generate($"case {i + 1}:", () =>
                                            {
                                                var call = specializeInfo.Call(method);
                                                if (method.HasReturn)
                                                {
                                                    mg.Statement.Generate($"var __result__ = {call}");
                                                    mg.Statement.Generate($"return {specializeInfo.GetCastResult(method.ReturnType)};");
                                                }
                                                else
                                                {
                                                    mg.Statement.Generate(call);
                                                    mg.Statement.Generate("break;");
                                                }
                                            });
                                        }
                                    });

                                    if (method.HasReturn)
                                    {
                                        mg.Statement.Generate($"var __defaultResult__ = {defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                                        mg.Statement.Generate($"return __defaultResult__;");
                                    }
                                    else
                                    {
                                        mg.Statement.Generate($"{defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                                    }
                                }
                                break;
                        }
                    });

                    switch (method.Algorithm)
                    {
                        case SpecializeAlgorithm.VirtualStrategy:
                            GenerateVirtualStrategyTypes(tg, typeInfo, method);
                            break;
                        case SpecializeAlgorithm.DelegateStrategy:
                            GenerateDelegateStrategyTypes(tg, typeInfo, method);
                            break;
                        case SpecializeAlgorithm.TypeIdComparison:
                            GenerateTypeIdComparisonTypes(tg, method);
                            break;
                    }
                }
            });
        }

        private void GenerateVirtualStrategyTypes(TypeGenerator.Children tg, TypeInfo typeInfo, MethodInfo method)
        {
            var defaultMethod = method.DefaultMethod;
            tg.Type.Generate(ModifierType.Private | ModifierType.Interface, $"_ISpecializeWrapper_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                ttg.Method.Generate(ModifierType.None, method.ReturnType, "Method", mg =>
                {
                    foreach (var generic in method.GenericParameters)
                    {
                        mg.GenericParam.Generate(generic.Type, gg =>
                        {
                            foreach (var wh in generic.Wheres)
                            {
                                gg.Where.Generate(wh);
                            }
                        });
                    }

                    if (!method.IsStatic)
                    {
                        mg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        mg.Param.Generate(parameter.Modifier, parameter.Type, parameter.Name, parameter.Default);
                    }
                });
            });

            tg.Type.Generate(ModifierType.Private | ModifierType.Interface, $"_ISpecializeWrapper_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                foreach (var generic in method.GenericParameters)
                {
                    ttg.GenericParam.Generate(generic.Type, gg =>
                    {
                        foreach (var wh in generic.Wheres)
                        {
                            gg.Where.Generate(wh);
                        }
                    });
                }

                ttg.Method.Generate(ModifierType.None, method.ReturnType, "Method", mg =>
                {
                    if (!method.IsStatic)
                    {
                        mg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        mg.Param.Generate(parameter.Modifier, parameter.Type, parameter.Name, parameter.Default);
                    }
                });
            });

            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeDefaultWrapper_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                ttg.BaseType.Generate($"_ISpecializeWrapper_{method.Id}_");
                ttg.Field.Generate(ModifierType.Public | ModifierType.Static, $"_SpecializeDefaultWrapper_{method.Id}_", "Object", $"new _SpecializeDefaultWrapper_{method.Id}_()");
                ttg.Method.Generate(ModifierType.None, method.ReturnType, $"_ISpecializeWrapper_{method.Id}_.Method", mg =>
                {
                    foreach (var generic in method.GenericParameters)
                    {
                        mg.GenericParam.Generate(generic.Type);
                    }

                    if (!method.IsStatic)
                    {
                        mg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        mg.Param.Generate(parameter.Modifier, parameter.Type, parameter.Name, parameter.Default);
                    }

                    foreach (var specializeInfo in method.SpecializeInfos)
                    {
                        if (specializeInfo.BindTypes.All(v => !v.Value.TypeFormulaBinds.Any()))
                        {
                            continue;
                        }

                        mg.Statement.Generate($"if ({specializeInfo.DefaultComparison})", () =>
                        {
                            var call = specializeInfo.Call(method);
                            if (method.HasReturn)
                            {
                                mg.Statement.Generate($"var __result__ = {call}");
                                mg.Statement.Generate($"return {specializeInfo.GetCastResult(method.ReturnType)};");
                            }
                            else
                            {
                                mg.Statement.Generate(call);
                            }
                        });
                    }

                    if (method.IsStatic)
                    {
                        if (method.HasReturn)
                        {
                            mg.Statement.Generate($"return {defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                        }
                        else
                        {
                            mg.Statement.Generate($"{defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                        }
                    }
                    else
                    {
                        if (method.HasReturn)
                        {
                            mg.Statement.Generate($"return __self__.{defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                        }
                        else
                        {
                            mg.Statement.Generate($"__self__.{defaultMethod}<{method.GenericArgumentString}>({method.ArgumentBaseString});");
                        }
                    }
                });
            });

            var specializeInfos = method.SpecializeInfos;
            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeWrapper_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                ttg.BaseType.Generate($"_ISpecializeWrapper_{method.Id}_");
                ttg.Field.Generate(ModifierType.Public | ModifierType.Static, $"_SpecializeWrapper_{method.Id}_", "Object", $"new _SpecializeWrapper_{method.Id}_()");
                ttg.Method.Generate(ModifierType.None, method.ReturnType, $"_ISpecializeWrapper_{method.Id}_.Method", mg =>
                {
                    foreach (var generic in method.GenericParameters)
                    {
                        mg.GenericParam.Generate(generic.Type);
                    }

                    if (!method.IsStatic)
                    {
                        mg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        mg.Param.Generate(parameter.Modifier, parameter.Type, parameter.Name, parameter.Default);
                    }

                    if (method.HasReturn)
                    {
                        mg.Statement.Generate($"return (this as _ISpecializeWrapper_{method.Id}_<{method.GenericArgumentString}>).Method({method.ArgumentString});");
                    }
                    else
                    {
                        mg.Statement.Generate($"(this as _ISpecializeWrapper_{method.Id}_<{method.GenericArgumentString}>).Method({method.ArgumentString});");
                    }
                });

                foreach (var specializeInfo in specializeInfos)
                {
                    ttg.BaseType.Generate($"_ISpecializeWrapper_{method.Id}_<{specializeInfo.GenericArgumentString}>");

                    string returnType = specializeInfo.GetBindType(method.ReturnType);
                    ttg.Method.Generate(ModifierType.None, returnType, $"_ISpecializeWrapper_{method.Id}_<{specializeInfo.GenericArgumentString}>.Method", mg =>
                    {
                        if (!method.IsStatic)
                        {
                            mg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                        }

                        foreach (var parameter in method.Parameters)
                        {
                            string parameterType = specializeInfo.GetBindType(parameter.Type);
                            mg.Param.Generate(parameter.Modifier, parameterType, parameter.Name, parameter.Default);
                        }

                        if (method.IsStatic)
                        {
                            if (method.HasReturn)
                            {
                                mg.Statement.Generate($"return {specializeInfo.SpecialMethod}({method.ArgumentBaseString});");
                            }
                            else
                            {
                                mg.Statement.Generate($"{specializeInfo.SpecialMethod}({method.ArgumentBaseString});");
                            }
                        }
                        else
                        {
                            if (method.HasReturn)
                            {
                                mg.Statement.Generate($"return __self__.{specializeInfo.SpecialMethod}({method.ArgumentBaseString});");
                            }
                            else
                            {
                                mg.Statement.Generate($"__self__.{specializeInfo.SpecialMethod}({method.ArgumentBaseString});");
                            }
                        }
                    });
                }
            });

            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeObject_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                foreach (var generic in method.GenericParameters)
                {
                    ttg.GenericParam.Generate(generic.Type, gg =>
                    {
                        foreach (var wh in generic.Wheres)
                        {
                            gg.Where.Generate(wh);
                        }
                    });
                }

                ttg.Field.Generate(ModifierType.Public | ModifierType.Static, $"_ISpecializeWrapper_{method.Id}_", "Object");
                ttg.Method.Generate(ModifierType.Static, string.Empty, $"_SpecializeObject_{method.Id}_", mg =>
                {
                    mg.Statement.Generate($"if (_SpecializeWrapper_{method.Id}_.Object is _ISpecializeWrapper_{method.Id}_<{method.GenericArgumentString}>)", () =>
                    {
                        mg.Statement.Generate($"Object = _SpecializeWrapper_{method.Id}_.Object;");
                    });
                    mg.Statement.Generate($"else", () =>
                    {
                        mg.Statement.Generate($"Object = _SpecializeDefaultWrapper_{method.Id}_.Object;");
                    });
                });
            });
        }

        private void GenerateDelegateStrategyTypes(TypeGenerator.Children tg, TypeInfo typeInfo, MethodInfo method)
        {
            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeDelegate_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                foreach (var generic in method.GenericParameters)
                {
                    ttg.GenericParam.Generate(generic.Type, gg =>
                    {
                        foreach (var wh in generic.Wheres)
                        {
                            gg.Where.Generate(wh);
                        }
                    });
                }

                ttg.Delegate.Generate(ModifierType.Public, method.ReturnType, "MethodDelegate", dg =>
                {
                    if (!method.IsStatic)
                    {
                        dg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        dg.Param.Generate(parameter.Modifier, parameter.Type, parameter.Name, parameter.Default);
                    }
                });

                if (method.HasTypeFormulaBinds)
                {
                    ttg.Field.Generate(ModifierType.Public | ModifierType.Static, "MethodDelegate", "Method", $"_SpecializeDelegate_{method.Id}_.CreateMethod<{method.GenericArgumentString}>()");
                }
                else
                {
                    ttg.Field.Generate(ModifierType.Public | ModifierType.Static, "MethodDelegate", "Method", $"_SpecializeDelegate_{method.Id}_.GetMethod<{method.GenericArgumentString}>()");
                }
            });

            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeDelegate_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                ttg.Method.Generate(ModifierType.Static, $"_SpecializeDelegate_{method.Id}_", mg =>
                {
                    var specializeInfos = method.SpecializeInfos;
                    for (int i = 0; i < specializeInfos.Count; ++i)
                    {
                        var specializeInfo = specializeInfos[i];
                        mg.Statement.Generate($"_SpecializeDelegate_{method.Id}_<{specializeInfo.GenericArgumentString}>.Method = CreateMethod<{specializeInfo.GenericArgumentString}>();");
                    }
                });

                ttg.Method.Generate(ModifierType.Public | ModifierType.Static, $"_SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.MethodDelegate", "CreateMethod", mg =>
                {
                    foreach (var generic in method.GenericParameters)
                    {
                        mg.GenericParam.Generate(generic.Type, gg =>
                        {
                            foreach (var wh in generic.Wheres)
                            {
                                gg.Where.Generate(wh);
                            }
                        });
                    }

                    mg.Attribute.Generate(ag =>
                    {
                        ag.Type.Generate("global::System.Runtime.CompilerServices.MethodImpl");
                        ag.Arg.Generate("global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining");
                    });

                    var specializeInfos = method.SpecializeInfos;
                    for (int i = 0; i < specializeInfos.Count; ++i)
                    {
                        var specializeInfo = specializeInfos[i];
                        mg.Statement.Generate($"if ({specializeInfo.Comparison})", () =>
                        {
                            if (specializeInfo.HasTypeFormulaBinds)
                            {
                                mg.Statement.Generate($"return new _SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.MethodDelegate((_SpecializeDelegate_{method.Id}_<{specializeInfo.GenericArgumentString}>.MethodDelegate){specializeInfo.SpecialMethod});");
                            }
                            else
                            {
                                mg.Statement.Generate($"return (_SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.MethodDelegate)(_SpecializeDelegate_{method.Id}_<{specializeInfo.GenericArgumentString}>.MethodDelegate){specializeInfo.SpecialMethod};");
                            }
                        });
                    }

                    mg.Statement.Generate($"return null;");
                });

                if (!method.HasTypeFormulaBinds)
                {
                    ttg.Method.Generate(ModifierType.Public | ModifierType.Static, $"_SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.MethodDelegate", "GetMethod", mg =>
                    {
                        foreach (var generic in method.GenericParameters)
                        {
                            mg.GenericParam.Generate(generic.Type, gg =>
                            {
                                foreach (var wh in generic.Wheres)
                                {
                                    gg.Where.Generate(wh);
                                }
                            });
                        }

                        mg.Attribute.Generate(ag =>
                        {
                            ag.Type.Generate("global::System.Runtime.CompilerServices.MethodImpl");
                            ag.Arg.Generate("global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining");
                        });

                        mg.Statement.Generate($"return _SpecializeDelegate_{method.Id}_<{method.GenericArgumentString}>.Method;");
                    });
                }

                if (!method.IsStatic)
                {
                    var specializeInfos = method.SpecializeInfos;
                    for (int i = 0; i < specializeInfos.Count; ++i)
                    {
                        var specializeInfo = specializeInfos[i];
                        string returnType = specializeInfo.GetBindType(method.ReturnType);
                        ttg.Method.Generate(ModifierType.Public | ModifierType.Static, returnType, specializeInfo.SpecialMethod, mg =>
                        {
                            mg.Param.Generate(ModifierType.Ref, typeInfo.Name, "__self__");
                            foreach (var parameter in method.Parameters)
                            {
                                string parameterType = specializeInfo.GetBindType(parameter.Type);
                                mg.Param.Generate(parameter.Modifier, parameterType, parameter.Name, parameter.Default);
                            }

                            if (method.HasReturn)
                            {
                                mg.Statement.Generate($"var __result__ = __self__.{specializeInfo.SpecialMethod}({method.ArgumentBaseString});");
                                mg.Statement.Generate($"return __result__;");
                            }
                            else
                            {
                                mg.Statement.Generate($"__self__.{specializeInfo.SpecialMethod}({method.ArgumentBaseString});");
                            }
                        });
                    }
                }
            });
        }

        private void GenerateTypeIdComparisonTypes(TypeGenerator.Children tg, MethodInfo method)
        {
            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeId_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                foreach (var generic in method.GenericParameters)
                {
                    ttg.GenericParam.Generate(generic.Type, gg =>
                    {
                        foreach (var wh in generic.Wheres)
                        {
                            gg.Where.Generate(wh);
                        }
                    });
                }

                if (method.HasTypeFormulaBinds)
                {
                    ttg.Field.Generate(ModifierType.Public | ModifierType.Static, "int", "Id", $"_SpecializeId_{method.Id}_.CreateId<{method.GenericArgumentString}>()");
                }
                else
                {
                    ttg.Field.Generate(ModifierType.Public | ModifierType.Static, "int", "Id", $"_SpecializeId_{method.Id}_.GetId<{method.GenericArgumentString}>()");
                }
            });

            tg.Type.Generate(ModifierType.Private | ModifierType.Sealed | ModifierType.Class, $"_SpecializeId_{method.Id}_", ttg =>
            {
                ttg.Attribute.Generate(ag =>
                {
                    ag.Type.Generate("global::System.ComponentModel.EditorBrowsable");
                    ag.Arg.Generate("global::System.ComponentModel.EditorBrowsableState.Never");
                });

                ttg.Method.Generate(ModifierType.Static, $"_SpecializeId_{method.Id}_", mg =>
                {
                    var specializeInfos = method.SpecializeInfos;
                    for (int i = 0; i < specializeInfos.Count; ++i)
                    {
                        var specializeInfo = specializeInfos[i];
                        mg.Statement.Generate($"_SpecializeId_{method.Id}_<{specializeInfo.GenericArgumentString}>.Id = CreateId<{specializeInfo.GenericArgumentString}>();");
                    }
                });

                ttg.Method.Generate(ModifierType.Public | ModifierType.Static, "int", "CreateId", mg =>
                {
                    foreach (var generic in method.GenericParameters)
                    {
                        mg.GenericParam.Generate(generic.Type, gg =>
                        {
                            foreach (var wh in generic.Wheres)
                            {
                                gg.Where.Generate(wh);
                            }
                        });
                    }

                    mg.Attribute.Generate(ag =>
                    {
                        ag.Type.Generate("global::System.Runtime.CompilerServices.MethodImpl");
                        ag.Arg.Generate("global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining");
                    });

                    var specializeInfos = method.SpecializeInfos;
                    for (int i = 0; i < specializeInfos.Count; ++i)
                    {
                        var specializeInfo = specializeInfos[i];
                        mg.Statement.Generate($"if ({specializeInfo.Comparison})", () =>
                        {
                            mg.Statement.Generate($"return {i + 1};");
                        });
                    }

                    mg.Statement.Generate($"return 0;");
                });

                if (!method.HasTypeFormulaBinds)
                {
                    ttg.Method.Generate(ModifierType.Public | ModifierType.Static, "int", "GetId", mg =>
                    {
                        foreach (var generic in method.GenericParameters)
                        {
                            mg.GenericParam.Generate(generic.Type, gg =>
                            {
                                foreach (var wh in generic.Wheres)
                                {
                                    gg.Where.Generate(wh);
                                }
                            });
                        }

                        mg.Attribute.Generate(ag =>
                        {
                            ag.Type.Generate("global::System.Runtime.CompilerServices.MethodImpl");
                            ag.Arg.Generate("global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining");
                        });

                        mg.Statement.Generate($"return _SpecializeId_{method.Id}_<{method.GenericArgumentString}>.Id;");
                    });
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
                    var specializationNames = type.GetTypeNames("Katuusagi.GenericEnhance", "SpecializationMethod").ToArray();
                    var specializeMethodNames = type.GetTypeNames("Katuusagi.GenericEnhance", "SpecializedMethod").ToArray();
                    var typeFormulaNames = type.GetTypeNames("Katuusagi.GenericEnhance", "ITypeFormula<").ToArray();
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        var specialization = method.GetAttribute(specializationNames);
                        if (specialization == null)
                        {
                            continue;
                        }

                        bool isTypePartial = type.IsPerfectPartial();
                        if (!isTypePartial)
                        {
                            ContextUtils.LogError("GENERICENHANCE0001", "GenericEnhance failed.", "Specialization can only be set for members of partial type.", type);
                            continue;
                        }

                        bool isPartial = method.IsPartial();
                        if (!isPartial || method.Body != null || method.ExpressionBody != null)
                        {
                            ContextUtils.LogError("GENERICENHANCE0002", "GenericEnhance failed.", "Specialization can only be set for defining declaration of partial method.", method);
                            continue;
                        }

                        if (method.TypeParameterList == null ||
                            !method.TypeParameterList.Parameters.Any())
                        {
                            ContextUtils.LogError("GENERICENHANCE0003", "GenericEnhance failed.", "Specialization can only be set for generic method.", method);
                            continue;
                        }

                        specialization.TryGetArgumentValue("defaultMethodName", 0, string.Empty, out var defaultMethod);
                        specialization.TryGetArgumentValue("algorithm", 1, SpecializeAlgorithm.VirtualStrategy, out var algorithm);

                        var specializedMethods =  method.GetAttributes(specializeMethodNames).ToArray();
                        var specializeInfos = new List<SpecializeInfo>();

                        foreach (var specializedMethod in specializedMethods)
                        {
                            specializedMethod.TryGetArgumentValue("methodName", 0, string.Empty, out var specialMethod);
                            specializedMethod.TryGetArgumentValue("bindTypes", 1, null, out string[] bindTypeArray);
                            if (bindTypeArray == null)
                            {
                                var count = specializedMethod.ArgumentList?.Arguments.Count ?? 0;
                                bindTypeArray = new string[count - 1];
                                for (int i = 1; i < count; ++i)
                                {
                                    specializedMethod.TryGetArgumentValue(null, i, string.Empty, out bindTypeArray[i - 1]);
                                }
                            }

                            if (bindTypeArray.Length != method.TypeParameterList.Parameters.Count)
                            {
                                ContextUtils.LogError("GENERICENHANCE0004", "GenericEnhance failed.", "Count of type arguments is mismatch.", specializedMethod);
                                continue;
                            }

                            var bindTypes = new Dictionary<string, BindType>();
                            for (int i = 0; i < method.TypeParameterList.Parameters.Count; ++i)
                            {
                                var genericParameterName = method.TypeParameterList.Parameters[i].ToString();
                                var bindType = bindTypeArray[i];
                                var typeFormulaConstraints = method.ConstraintClauses
                                                                .Where(v => v.Name.ToString() == genericParameterName)
                                                                .SelectMany(v => v.Constraints)
                                                                .Select(v => v.ToString())
                                                                .Where(v => typeFormulaNames.Any(v2 => v.StartsWith(v2)))
                                                                .Select(v => TypeParameterMatch.Match(v).Value)
                                                                .ToArray();
                                bindTypes.Add(genericParameterName, new BindType()
                                {
                                    Type = bindType,
                                    TypeFormulaBinds = typeFormulaConstraints,
                                });
                            }

                            var specializeInfo = new SpecializeInfo();
                            specializeInfo.SpecialMethod = specialMethod;
                            specializeInfo.BindTypes = bindTypes;

                            var specializedReturnType = specializeInfo.GetBindType(method.ReturnType.ToString());
                            var specializedParameterTypes = method.ParameterList?.Parameters.Select(v => specializeInfo.GetBindType(v.Type.ToString())).ToArray() ?? Array.Empty<string>();
                            var isExistSpecializedMethod = IsExitMethod(typeGroups, 0, specialMethod, specializedReturnType, specializedParameterTypes);
                            if (!isExistSpecializedMethod)
                            {
                                string specializedParameters = specializedParameterTypes.JoinParameters();
                                ContextUtils.LogError("GENERICENHANCE0005", "GenericEnhance failed.", $"\"{specializedReturnType} {specialMethod}({specializedParameters})\" does not found.", specializedMethod);
                                continue;
                            }

                            specializeInfos.Add(specializeInfo);
                        }

                        var methodName = method.Identifier.ToString();
                        var methodModifier = ScriptGeneratorUtils.GetModifierType(method.Modifiers.ToString());
                        var methodInfo = new MethodInfo()
                        {
                            Id = Guid.NewGuid().ToString().Replace("-", string.Empty),
                            Name = methodName,
                            ReturnType = method.ReturnType.ToString(),
                            Modifier = methodModifier,
                            Algorithm = algorithm,
                            DefaultMethod = defaultMethod,
                            GenericParameters = new List<GenericParameterInfo>(),
                            Parameters = new List<ParameterInfo>(),
                            SpecializeInfos = specializeInfos,
                        };

                        foreach (var parameter in method.TypeParameterList.Parameters)
                        {
                            var typeParameterName = parameter.Identifier.ToString();
                            var parameterInfo = new GenericParameterInfo()
                            {
                                Modifier = ScriptGeneratorUtils.GetModifierType(parameter.VarianceKeyword.ToString()),
                                Wheres = method.ConstraintClauses.Where(v => v.Name.ToString() == typeParameterName).SelectMany(v => v.Constraints).Select(v => v.ToString()).ToArray(),
                                Type = typeParameterName,
                            };

                            methodInfo.GenericParameters.Add(parameterInfo);
                        }

                        if (method.ParameterList != null)
                        {
                            foreach (var parameter in method.ParameterList.Parameters)
                            {
                                var parameterInfo = new ParameterInfo()
                                {
                                    Modifier = ScriptGeneratorUtils.GetModifierType(parameter.Modifiers.ToString()),
                                    Type = parameter.Type.ToString(),
                                    Name = parameter.Identifier.ToString(),
                                    Default = parameter.Default?.Value?.ToString() ?? string.Empty,
                                };

                                methodInfo.Parameters.Add(parameterInfo);
                            }
                        }

                        var methodParameterTypes = method.ParameterList?.Parameters.Select(v => v.Type.ToString()).ToArray() ?? Array.Empty<string>();
                        var genericCount = method.TypeParameterList.Parameters.Count;
                        var isExistDefaultMethod = IsExitMethod(typeGroups, genericCount, defaultMethod, method.ReturnType.ToString(), methodParameterTypes);
                        if (!isExistDefaultMethod)
                        {
                            string methodParameters = methodParameterTypes.JoinParameters();
                            ContextUtils.LogError("GENERICENHANCE0005", "GenericEnhance failed.", $"\"{method.ReturnType} {defaultMethod}<{genericCount}>({methodParameters})\" does not found.", specialization);
                            continue;
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

        private static bool IsExitMethod(IEnumerable<IEnumerable<TypeDeclarationSyntax>> typeGroups, int genericCount, string method, string returnType, IEnumerable<string> methodParameterTypes)
        {
            var result = typeGroups.SelectMany(v => v).SelectMany(v => v.GetMethods()).Any(v =>
            {
                var cmpGenericCount = v.TypeParameterList?.Parameters.Count ?? 0;
                if (cmpGenericCount != genericCount)
                {
                    return false;
                }

                if (v.Identifier.ToString() != method)
                {
                    return false;
                }

                if (v.ReturnType.ToString() != returnType)
                {
                    return false;
                }

                var parameterTypes = v.ParameterList?.Parameters.Select(v2 => v2.Type.ToString()) ?? Array.Empty<string>();
                if (!parameterTypes.SequenceEqual(methodParameterTypes))
                {
                    return false;
                }

                return true;
            });

            return result;
        }
    }
}

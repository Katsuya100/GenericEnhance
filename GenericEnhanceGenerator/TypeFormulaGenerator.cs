using Katuusagi.CSharpScriptGenerator;
using Katuusagi.SourceGeneratorCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Katuusagi.GenericEnhance.SourceGenerator
{
    [Generator]
    public class TypeFormulaGenerator : ISourceGenerator
    {
        private struct LiteralTypeMatch
        {
            public LiteralTypeMatch(Regex match)
            {
                Match = match;
            }

            public Regex Match;
        }

        private struct LiteralType
        {
            public LiteralType(string name, string valueString)
            {
                Name = name;
                ValueString = valueString;
            }
            public string Name;
            public string ValueString;
        }

        private HashSet<LiteralType> _literalTypes = new HashSet<LiteralType>();

        private static readonly LiteralTypeMatch[] LiteralTypeMatches = new LiteralTypeMatch[]
        {
            new LiteralTypeMatch(new Regex("(?<=(^_))n?\\d+(?=($))")),
            new LiteralTypeMatch(new Regex("(?<=(^_))n?(\\d+_\\d+)(?=($))")),
        };

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            ContextUtils.InitLog<TypeFormulaGenerator>(context);
            try
            {
                if (!context.IsReferencedAssembly("Katuusagi.GenericEnhance"))
                {
                    return;
                }

                Analyze(context);
                if (!_literalTypes.Any())
                {
                    return;
                }

                var root = new RootGenerator();
                root.Generate(rg =>
                {
                    rg.Using.Generate("Katuusagi.GenericEnhance");
                    foreach (var value in _literalTypes)
                    {
                        rg.Type.Generate(ModifierType.Internal | ModifierType.Struct, value.Name, tg =>
                        {
                            tg.BaseType.Generate($"ITypeFormula<sbyte>");
                            tg.BaseType.Generate($"ITypeFormula<byte>");
                            tg.BaseType.Generate($"ITypeFormula<short>");
                            tg.BaseType.Generate($"ITypeFormula<ushort>");
                            tg.BaseType.Generate($"ITypeFormula<int>");
                            tg.BaseType.Generate($"ITypeFormula<uint>");
                            tg.BaseType.Generate($"ITypeFormula<long>");
                            tg.BaseType.Generate($"ITypeFormula<ulong>");
                            tg.BaseType.Generate($"ITypeFormula<float>");
                            tg.BaseType.Generate($"ITypeFormula<double>");
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "sbyte", "Int8ResultValue", $"unchecked((sbyte){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "sbyte", "ITypeFormula<sbyte>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return Int8ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "byte", "UInt8ResultValue", $"unchecked((byte){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "byte", "ITypeFormula<byte>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return UInt8ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "short", "Int16ResultValue", $"unchecked((short){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "short", "ITypeFormula<short>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return Int16ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "ushort", "UInt16ResultValue", $"unchecked((ushort){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "ushort", "ITypeFormula<ushort>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return UInt16ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "int", "Int32ResultValue", $"unchecked((int){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "int", "ITypeFormula<int>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return Int32ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "uint", "UInt32ResultValue", $"unchecked((uint){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "uint", "ITypeFormula<uint>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return UInt32ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "long", "Int64ResultValue", $"unchecked((long){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "long", "ITypeFormula<long>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return Int64ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "ulong", "UInt64ResultValue", $"unchecked((ulong){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "ulong", "ITypeFormula<ulong>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return UInt64ResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "float", "SingleResultValue", $"unchecked((float){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "float", "ITypeFormula<float>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return SingleResultValue;");
                                });
                            });
                            tg.Field.Generate(ModifierType.Public | ModifierType.Const, "double", "DoubleResultValue", $"unchecked((double){value.ValueString})");
                            tg.Property.Generate(ModifierType.None, "double", "ITypeFormula<double>.Result", pg =>
                            {
                                pg.Get.Generate(ModifierType.None, pgg =>
                                {
                                    pgg.Statement.Generate("return DoubleResultValue;");
                                });
                            });
                        });
                    }
                });

                var builder = new CSharpScriptBuilder();
                builder.BuildAndNewLine(root.Result);
                context.AddSource($"Katuusagi.TypeFormula.Literal.Generated.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
            catch (Exception e)
            {
                ContextUtils.LogException(e);
            }
        }

        private void Analyze(GeneratorExecutionContext context)
        {
            var trees = context.Compilation.SyntaxTrees
                                    .OrderBy(v => v.FilePath);
            foreach (var tree in trees)
            {
                Analyze(tree.GetRoot());
            }
        }

        private void Analyze(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeSyntax type)
            {
                Analyze(type);
            }

            foreach (var node in syntaxNode.ChildNodes())
            {
                Analyze(node);
            }
        }

        private void Analyze(TypeSyntax type)
        {
            var name = type.ToString();
            foreach (var literalTypeMatch in LiteralTypeMatches)
            {
                var match = literalTypeMatch.Match.Match(name);
                if (!match.Success)
                {
                    continue;
                }

                var valueString = match.Value;
                valueString = valueString.Replace("_", ".").Replace("n", "-");
                _literalTypes.Add(new LiteralType(name, valueString));
            }
        }
    }
}

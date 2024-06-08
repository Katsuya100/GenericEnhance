using Katuusagi.ILPostProcessorCommon.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace Katuusagi.GenericEnhance.Editor.Utils
{
    public static class TypeFormulaUtils
    {
        [ThreadStatic]
        private static ModuleDefinition _module;
        [ThreadStatic]
        private static TypeReference _valueType;
        [ThreadStatic]
        private static TypeReference _itypeFormulaTrue;
        [ThreadStatic]
        private static TypeReference _itypeFormulaFalse;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaInt8Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaUInt8Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaInt16Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaUInt16Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaInt32Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaUInt32Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaInt64Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaUInt64Type;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaSingleType;
        [ThreadStatic]
        private static GenericInstanceType _itypeFormulaDoubleType;

        public static void Init(ModuleDefinition module)
        {
            _module = module;
            _valueType = _module.ImportReference(typeof(System.ValueType));
            var usingTypeFormula = _module.Types.SelectMany(v => v.Interfaces)
                                                    .Select(v => v.InterfaceType)
                                                    .FirstOrDefault(v => v.FullName.StartsWith("Katuusagi.GenericEnhance.ITypeFormula`1"));
            if (usingTypeFormula != null)
            {
                var itypeFormula = usingTypeFormula.GetElementType();
                _itypeFormulaTrue = new TypeReference(string.Empty, "_true", usingTypeFormula.Module, usingTypeFormula.Scope, true);
                _itypeFormulaFalse = new TypeReference(string.Empty, "_false", usingTypeFormula.Module, usingTypeFormula.Scope, true);

                _itypeFormulaInt8Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaUInt8Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaInt16Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaUInt16Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaInt32Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaUInt32Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaInt64Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaUInt64Type = new GenericInstanceType(itypeFormula);
                _itypeFormulaSingleType = new GenericInstanceType(itypeFormula);
                _itypeFormulaDoubleType = new GenericInstanceType(itypeFormula);

                _itypeFormulaInt8Type.GenericArguments.Add(_module.TypeSystem.SByte);
                _itypeFormulaUInt8Type.GenericArguments.Add(_module.TypeSystem.Byte);
                _itypeFormulaInt16Type.GenericArguments.Add(_module.TypeSystem.Int16);
                _itypeFormulaUInt16Type.GenericArguments.Add(_module.TypeSystem.UInt16);
                _itypeFormulaInt32Type.GenericArguments.Add(_module.TypeSystem.Int32);
                _itypeFormulaUInt32Type.GenericArguments.Add(_module.TypeSystem.UInt32);
                _itypeFormulaInt64Type.GenericArguments.Add(_module.TypeSystem.Int64);
                _itypeFormulaUInt64Type.GenericArguments.Add(_module.TypeSystem.UInt64);
                _itypeFormulaSingleType.GenericArguments.Add(_module.TypeSystem.Single);
                _itypeFormulaDoubleType.GenericArguments.Add(_module.TypeSystem.Double);
            }
        }

        public static bool TryEmulateLiteralType(MethodDefinition method, Instruction instruction, TypeReference typeRef, out TypeReferenceInfo result)
        {
            if (typeRef == null)
            {
                result = default;
                return false;
            }

            result = new TypeReferenceInfo(typeRef);
            switch (result.Style)
            {
                case TypeStyle.BooleanLiteral:
                case TypeStyle.IntegerLiteral:
                case TypeStyle.FloatLiteral:
                case TypeStyle.Boolean:
                case TypeStyle.Int8:
                case TypeStyle.UInt8:
                case TypeStyle.Int16:
                case TypeStyle.UInt16:
                case TypeStyle.Int32:
                case TypeStyle.UInt32:
                case TypeStyle.Int64:
                case TypeStyle.UInt64:
                case TypeStyle.Single:
                case TypeStyle.Double:
                    return true;
            }

            if (!(typeRef is GenericInstanceType genType))
            {
                return true;
            }

            var argumentInfos = new TypeReferenceInfo[genType.GenericArguments.Count];
            for (int i = 0; i < argumentInfos.Length; ++i)
            {
                var argument = genType.GenericArguments[i];
                if (!TryEmulateLiteralType(method, instruction, argument, out var argumentInfo))
                {
                    return false;
                }

                argumentInfos[i] = argumentInfo;
            }

            object value = null;
            try
            {
                switch (result.Style)
                {
                    case TypeStyle.Add:
                        value = ArithmeticUtils.Add(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Sub:
                        value = ArithmeticUtils.Sub(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Mul:
                        value = ArithmeticUtils.Mul(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Div:
                        value = ArithmeticUtils.Div(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Mod:
                        value = ArithmeticUtils.Mod(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Minus:
                        value = ArithmeticUtils.Minus(argumentInfos[0].Type, argumentInfos[1].Value);
                        break;
                    case TypeStyle.BitNot:
                        value = BitLogicalUtils.Not(argumentInfos[0].Type, argumentInfos[1].Value);
                        break;
                    case TypeStyle.BitAnd:
                        value = BitLogicalUtils.And(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.BitOr:
                        value = BitLogicalUtils.Or(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.BitXor:
                        value = BitLogicalUtils.Xor(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.LShift:
                        if (argumentInfos[2].Value == null)
                        {
                            value = null;
                            break;
                        }
                        value = BitLogicalUtils.LShift(argumentInfos[0].Type, argumentInfos[1].Value, CastUtils.CastNumeric<int>(argumentInfos[2].Value));
                        break;
                    case TypeStyle.RShift:
                        if (argumentInfos[2].Value == null)
                        {
                            value = null;
                            break;
                        }
                        value = BitLogicalUtils.RShift(argumentInfos[0].Type, argumentInfos[1].Value, CastUtils.CastNumeric<int>(argumentInfos[2].Value));
                        break;
                    case TypeStyle.CastNumeric:
                        value = CastUtils.CastNumeric(argumentInfos[0].Type, argumentInfos[1].Value);
                        break;
                    case TypeStyle.Not:
                        value = ConditionalLogicalUtils.Not((bool)argumentInfos[0].Value);
                        break;
                    case TypeStyle.And:
                        value = ConditionalLogicalUtils.And((bool)argumentInfos[0].Value, (bool)argumentInfos[1].Value);
                        break;
                    case TypeStyle.Or:
                        value = ConditionalLogicalUtils.Or((bool)argumentInfos[0].Value, (bool)argumentInfos[1].Value);
                        break;
                    case TypeStyle.Equal:
                        value = ConditionalLogicalUtils.Equal(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.NotEqual:
                        value = ConditionalLogicalUtils.NotEqual(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Greater:
                        value = ConditionalLogicalUtils.Greater(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.GreaterOrEqual:
                        value = ConditionalLogicalUtils.GreaterOrEqual(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.Less:
                        value = ConditionalLogicalUtils.Less(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                    case TypeStyle.LessOrEqual:
                        value = ConditionalLogicalUtils.LessOrEqual(argumentInfos[0].Type, argumentInfos[1].Value, argumentInfos[2].Value);
                        break;
                }
            }
            catch
            {
                ILPPUtils.LogError("GENERICENHANCE1501", "GenericEnhance failed.", $"TypeFormula precalculate failed.", method, instruction);
                throw;
            }

            if (value == null)
            {
                result.TypeRef = result.TypeRef.GetElementType().MakeGenericInstanceType(argumentInfos.Select(v => v.TypeRef).ToArray());
                return true;
            }

            typeRef = CreateLiteralType(value);
            result = new TypeReferenceInfo(typeRef);
            return true;
        }

        public static TypeReference CreateLiteralType(object value)
        {
            if (value is bool boolValue)
            {
                return boolValue ? _itypeFormulaTrue : _itypeFormulaFalse;
            }

            var name = value.ToString();
            if (value is float || value is double)
            {
                name = name.Replace(".", "_");
            }

            name = $"_{name.Replace("-", "n")}";

            var literalType = _module.Types.FirstOrDefault(v => v.FullName == name);
            if (literalType != null)
            {
                return _module.ImportReference(literalType);
            }

            var typeAttr = TypeAttributes.NotPublic | TypeAttributes.SequentialLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;
            literalType = new TypeDefinition(string.Empty, name, typeAttr, _valueType);
            literalType.PackingSize = 0;
            literalType.ClassSize = 1;

            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaInt8Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaUInt8Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaInt16Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaUInt16Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaInt32Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaUInt32Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaInt64Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaUInt64Type));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaSingleType));
            literalType.Interfaces.Add(new InterfaceImplementation(_itypeFormulaDoubleType));

            CreateMember(literalType, _itypeFormulaInt8Type, _module.TypeSystem.SByte, typeof(sbyte), "Int8", value);
            CreateMember(literalType, _itypeFormulaUInt8Type, _module.TypeSystem.Byte, typeof(byte), "UInt8", value);
            CreateMember(literalType, _itypeFormulaInt16Type, _module.TypeSystem.Int16, typeof(short), "In16", value);
            CreateMember(literalType, _itypeFormulaUInt16Type, _module.TypeSystem.UInt16, typeof(ushort), "UInt16", value);
            CreateMember(literalType, _itypeFormulaInt32Type, _module.TypeSystem.Int32, typeof(int), "Int32", value);
            CreateMember(literalType, _itypeFormulaUInt32Type, _module.TypeSystem.UInt32, typeof(uint), "UInt32", value);
            CreateMember(literalType, _itypeFormulaInt64Type, _module.TypeSystem.Int64, typeof(long), "Int64", value);
            CreateMember(literalType, _itypeFormulaUInt64Type, _module.TypeSystem.UInt64, typeof(ulong), "UInt64", value);
            CreateMember(literalType, _itypeFormulaSingleType, _module.TypeSystem.Single, typeof(float), "Single", value);
            CreateMember(literalType, _itypeFormulaDoubleType, _module.TypeSystem.Double, typeof(double), "Double", value);

            _module.Types.Add(literalType);

            return _module.ImportReference(literalType);
        }

        public static void CreateMember(TypeDefinition literalType, TypeReference baseTypeRef, TypeReference propertyTypeRef, Type propertyType, string propertyName, object value)
        {
            var methodAttr = MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            value = CastUtils.CastNumeric(propertyType, value);

            var field = new FieldDefinition($"{propertyName}ResultValue", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal, propertyTypeRef);
            field.Constant = value;
            literalType.Fields.Add(field);

            var getMethod = new MethodDefinition($"Katuusagi.GenericEnhance.ITypeFormula<{propertyTypeRef.FullName}>.get_Result", methodAttr, propertyTypeRef);
            MethodReference baseMethod = baseTypeRef.Resolve().Methods.FirstOrDefault(v => v.Name == "get_Result");
            baseMethod = _module.ImportReference(baseMethod);
            baseMethod.DeclaringType = baseTypeRef;
            getMethod.Overrides.Add(baseMethod);
            getMethod.Body.Instructions.Add(ILPPUtils.LoadLiteral(value));
            getMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            literalType.Methods.Add(getMethod);

            var property = new PropertyDefinition($"Katuusagi.GenericEnhance.ITypeFormula<{propertyTypeRef.FullName}>.Result", PropertyAttributes.None, propertyTypeRef);
            property.GetMethod = getMethod;
            literalType.Properties.Add(property);
        }
    }
}

using Katuusagi.GenericEnhance.Editor.Utils;
using Katuusagi.ILPostProcessorCommon.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using static UnityEngine.Networking.UnityWebRequest;

namespace Katuusagi.GenericEnhance.Editor
{
    internal class GenericEnhanceILPostProcessor : ILPostProcessor
    {

        private struct SpecializeMethodInfo
        {
            public bool Result;
            public MethodDefinition MethodDef;
            public string DefaultMethod;
            public SpecializeInfo[] SpecializeInfos;
        }

        private struct SpecializeInfo
        {
            public string SpecialMethod;
            public Dictionary<string, TypeReference> BindTypes;
        }

        private Dictionary<MethodReference, SpecializeMethodInfo> _specializationResult = new Dictionary<MethodReference, SpecializeMethodInfo>(MethodReferenceComparer.Default);
        private ModuleDefinition _module;

        private TypeReference _voidReference;
        private TypeReference _valueType;
        private TypeReference _itypeFormulaTrue;
        private TypeReference _itypeFormulaFalse;
        private GenericInstanceType _itypeFormulaInt8Type;
        private GenericInstanceType _itypeFormulaUInt8Type;
        private GenericInstanceType _itypeFormulaInt16Type;
        private GenericInstanceType _itypeFormulaUInt16Type;
        private GenericInstanceType _itypeFormulaInt32Type;
        private GenericInstanceType _itypeFormulaUInt32Type;
        private GenericInstanceType _itypeFormulaInt64Type;
        private GenericInstanceType _itypeFormulaUInt64Type;
        private GenericInstanceType _itypeFormulaSingleType;
        private GenericInstanceType _itypeFormulaDoubleType;

        public override ILPostProcessor GetInstance() => this;
        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            return compiledAssembly.References.Any(v => v.EndsWith("Katuusagi.GenericEnhance.dll"));
        }

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            if (!WillProcess(compiledAssembly))
            {
                return null;
            }

            try
            {
                ILPPUtils.InitLog<GenericEnhanceILPostProcessor>(compiledAssembly);
                using (var assembly = ILPPUtils.LoadAssemblyDefinition(compiledAssembly))
                {
                    _module = assembly.MainModule;

                    _voidReference = _module.TypeSystem.Void;
                    _valueType = _module.ImportReference(typeof(ValueType));
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

                    foreach (var type in assembly.Modules.SelectMany(v => v.Types).GetAllTypes().ToArray())
                    {
                        TypeDefProcess(type);
                        TypeFormulaProcess(type);

                        foreach (var field in type.Fields.ToArray())
                        {
                            TypeDefProcess(field);
                            TypeFormulaProcess(field);
                            NoneTypeProcess(field);
                        }

                        foreach (var property in type.Properties.ToArray())
                        {
                            TypeDefProcess(property);
                            TypeFormulaProcess(property);
                            NoneTypeProcess(property);
                        }

                        foreach (var method in type.Methods.ToArray())
                        {
                            TypeDefProcess(method);
                            TypeFormulaProcess(method);
                            if (NoneTypeProcess(type, method))
                            {
                                continue;
                            }

                            var body = method.Body;
                            if (body == null)
                            {
                                continue;
                            }

                            bool isChanged = false;
                            var instructions = body.Instructions;
                            for (var i = 0; i < instructions.Count; ++i)
                            {
                                var instruction = instructions[i];
                                int diff =0;
                                TypeDefProcess(method, instruction);
                                DefaultTypeProcess(method, instruction);
                                TypeFormulaProcess(method, instruction);
                                SpecializationProcess(instruction);
                                isChanged = VariadicGenericProcess(body, instruction) || isChanged;
                                NoneTypeProcess(method, instruction);
                                i += diff;
                            }

                            if (isChanged)
                            {
                                ILPPUtils.ResolveInstructionOpCode(instructions);
                            }

                            var variables = body.Variables;
                            for (var i = 0; i < variables.Count; ++i)
                            {
                                var variable = variables[i];
                                TypeDefProcess(variable, method);
                                TypeFormulaProcess(variable, method);
                                NoneTypeProcess(variable, method);
                            }
                        }

                        foreach (var @event in type.Events)
                        {
                            TypeDefProcess(@event);
                            TypeFormulaProcess(@event);
                            NoneTypeProcess(@event);
                        }
                    }

                    var pe  = new MemoryStream();
                    var pdb = new MemoryStream();
                    var writeParameter = new WriterParameters
                    {
                        SymbolWriterProvider = new PortablePdbWriterProvider(),
                        SymbolStream         = pdb,
                        WriteSymbols         = true
                    };

                    assembly.Write(pe, writeParameter);
                    return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), ILPPUtils.Logger.Messages);
                }
            }
            catch (Exception e)
            {
                ILPPUtils.LogException(e);
            }
            return new ILPostProcessResult(null, ILPPUtils.Logger.Messages);
        }

        private void TypeDefProcess(TypeDefinition type)
        {
            var typeDef = type.CustomAttributes.FirstOrDefault(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.TypeDef");
            if (typeDef != null)
            {
                var dstTypeObject = typeDef.ConstructorArguments[0].Value;
                bool isValueType = false;
                if (dstTypeObject is TypeReference dstType)
                {
                    if (!(dstType is TypeDefinition dstTypeDef))
                    {
                        dstTypeDef = dstType.Resolve();
                    }

                    if (dstTypeDef != null)
                    {
                        dstType = dstTypeDef;
                    }

                    isValueType = dstType.IsValueType;
                }
                else if (dstTypeObject is string dstTypeName)
                {
                    var genericParameter = type.GenericParameters.FirstOrDefault(v => v.Name == dstTypeName);
                    if (genericParameter == null)
                    {
                        ILPPUtils.LogError("GENERICENHANCE2501", "GenericEnhance failed.", $"\"{dstTypeName}\" is not exist generic parameter.", type);
                        return;
                    }
                    else
                    {
                        isValueType = genericParameter.Constraints.Any(v => v.FullName == "System.ValueType");
                        if (!isValueType && (genericParameter.Attributes & GenericParameterAttributes.ReferenceTypeConstraint) == 0)
                        {
                            // ã≠êßìIÇ…é∏îsÇ≥ÇπÇÈ
                            isValueType = !type.IsValueType;
                        }
                    }
                }

                if (isValueType != type.IsValueType)
                {
                    ILPPUtils.LogError("GENERICENHANCE2502", "GenericEnhance failed.", $"TypeDef is only possible between ReferenceTypes or between ValueTypes.", type);
                }
            }

            {
                var baseType = type.BaseType;
                if (TryReplaceType(ref baseType, type, null, null))
                {
                    type.BaseType = baseType;
                }
            }

            for (int i = 0; i < type.Interfaces.Count; ++i)
            {
                var @interface = type.Interfaces[i];
                var interfaceType = @interface.InterfaceType;
                if (TryReplaceType(ref interfaceType, type, null, null))
                {
                    @interface.InterfaceType = interfaceType;
                }
            }

            for (int i = 0; i < type.GenericParameters.Count; ++i)
            {
                var genericParameter = type.GenericParameters[i];
                for (int j = 0; j < genericParameter.Constraints.Count; ++j)
                {
                    var constraint = genericParameter.Constraints[j];
                    if (TryReplaceType(ref constraint, type, null, null))
                    {
                        genericParameter.Constraints[j] = constraint;
                    }
                }
            }
        }

        private void TypeDefProcess(FieldDefinition field)
        {
            var type = field.FieldType;
            if (TryReplaceType(ref type, field, null, null))
            {
                field.FieldType = type;
            }
        }

        private void TypeDefProcess(PropertyDefinition property)
        {
            var method = property.GetMethod ?? property.SetMethod;
            var type = property.PropertyType;
            if (TryReplaceType(ref type, property, method, null))
            {
                property.PropertyType = type;
            }
        }

        private void TypeDefProcess(MethodDefinition method)
        {
            TypeDefProcess(method, method, null);

            for (int i = 0; i < method.Overrides.Count; ++i)
            {
                var @override = method.Overrides[i];
                var declaringType = @override.DeclaringType;
                if (TryReplaceType(ref declaringType, method, method, null))
                {
                    @override.DeclaringType = declaringType;
                }

                TypeDefProcess(@override, method, null);
            }
        }

        private void TypeDefProcess(MethodReference method, MethodDefinition def, Instruction instruction)
        {
            {
                var returnType = method.ReturnType;
                if (TryReplaceType(ref returnType, method, def, instruction))
                {
                    method.ReturnType = returnType;
                }
            }

            for (int i = 0; i < method.Parameters.Count; ++i)
            {
                var parameterType = method.Parameters[i].ParameterType;
                if (TryReplaceType(ref parameterType, method, def, instruction))
                {
                    method.Parameters[i].ParameterType = parameterType;
                }
            }

            for (int i = 0; i < method.GenericParameters.Count; ++i)
            {
                var genericParameter = method.GenericParameters[i];
                for (int j = 0; j < genericParameter.Constraints.Count; ++j)
                {
                    var constraint = genericParameter.Constraints[j];
                    if (TryReplaceType(ref constraint, method, def, instruction))
                    {
                        genericParameter.Constraints[j] = constraint;
                    }
                }
            }
        }

        private void TypeDefProcess(MethodDefinition method, Instruction instruction)
        {
            if (instruction.Operand is TypeReference type)
            {
                if (TryReplaceType(ref type, method, method, instruction))
                {
                    instruction.Operand = type;
                }
            }

            if (instruction.Operand is MemberReference member)
            {
                bool isReplaced = false;
                {
                    var declaring = member.DeclaringType;
                    if (TryReplaceType(ref declaring, method, method, instruction))
                    {
                        isReplaced = true;
                        member.DeclaringType = declaring;
                    }
                }

                if (member is GenericInstanceMethod genericInstanceMethod)
                {
                    for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; ++i)
                    {
                        var genericArgument = genericInstanceMethod.GenericArguments[i];
                        if (TryReplaceType(ref genericArgument, method, method, instruction))
                        {
                            isReplaced = true;
                            genericInstanceMethod.GenericArguments[i] = genericArgument;
                        }
                    }
                }

                if (isReplaced &&
                    member.Resolve() == null)
                {
                    ILPPUtils.LogError("GENERICENHANCE2503", "GenericEnhance failed.", $"\"{member.DeclaringType}\" has not \"{member}\".", method, instruction);
                }
            }
        }

        private void TypeDefProcess(VariableDefinition variable, MethodDefinition def)
        {
            var type = variable.VariableType;
            if (TryReplaceType(ref type, def, def, null))
            {
                variable.VariableType = type;
            }
        }

        private void TypeDefProcess(EventDefinition @event)
        {
            var type = @event.EventType;
            if (TryReplaceType(ref type, @event, null, null))
            {
                @event.EventType = type;
            }
        }

        private bool TryReplaceType(ref TypeReference srcType, MemberReference member, MethodDefinition method, Instruction instruction)
        {
            return TryReplaceTypeInternal(ref srcType, member, method, instruction, new HashSet<TypeReference>(TypeReferenceComparer.Default));
        }

        private bool TryReplaceTypeInternal(ref TypeReference srcType, MemberReference member, MethodDefinition method, Instruction instruction, HashSet<TypeReference> expandedTypes)
        {
            if (srcType == null || member.DeclaringType.Is(srcType))
            {
                return false;
            }

            if (expandedTypes.Contains(srcType))
            {
                if (method == null)
                {
                    ILPPUtils.LogError("GENERICENHANCE2505", "GenericEnhance failed.", $"TypeDef is expanded infinitely.", member);
                }
                else
                {
                    ILPPUtils.LogError("GENERICENHANCE2505", "GenericEnhance failed.", $"TypeDef is expanded infinitely.", method, instruction);
                }

                return false;
            }

            expandedTypes.Add(srcType);

            bool result = false;
            if (srcType is GenericInstanceType genericInstanceType)
            {
                for (int i = 0; i < genericInstanceType.GenericArguments.Count; ++i)
                {
                    var element = genericInstanceType.GenericArguments[i];
                    if (TryReplaceTypeInternal(ref element, member, method, instruction, new HashSet<TypeReference>(expandedTypes, TypeReferenceComparer.Default)))
                    {
                        genericInstanceType.GenericArguments[i] = element;
                        result = true;
                    }
                }
            }
            else if (srcType is ArrayType arrayType)
            {
                var element = arrayType.ElementType;
                if (TryReplaceTypeInternal(ref element, member, method, instruction, new HashSet<TypeReference>(expandedTypes, TypeReferenceComparer.Default)))
                {
                    srcType = new ArrayType(element);
                    result = true;
                }
            }
            else if (srcType is PointerType pointerType)
            {
                var element = pointerType.ElementType;
                if (TryReplaceTypeInternal(ref element, member, method, instruction, new HashSet<TypeReference>(expandedTypes, TypeReferenceComparer.Default)))
                {
                    srcType = new PointerType(element);
                    result = true;
                }
            }
            else if (srcType is ByReferenceType byRefType)
            {
                var element = byRefType.ElementType;
                if (TryReplaceTypeInternal(ref element, member, method, instruction, new HashSet<TypeReference>(expandedTypes, TypeReferenceComparer.Default)))
                {
                    srcType = new ByReferenceType(element);
                    result = true;
                }
            }

            var srcTypeDef = srcType.Resolve();
            if (srcTypeDef == null)
            {
                return result;
            }

            var typeDef = srcTypeDef.CustomAttributes.FirstOrDefault(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.TypeDef");
            if (typeDef == null)
            {
                return result;
            }

            var beforeSrcType = srcType;
            var dstTypeObject = typeDef.ConstructorArguments[0].Value;
            if (dstTypeObject is TypeReference dstType)
            {
                srcType = dstType;
                result = true;
            }
            else if (dstTypeObject is string dstTypeName)
            {
                dstType = GetGenericArgument(srcType, dstTypeName);
                if (dstType == null)
                {
                    if (method == null)
                    {
                        ILPPUtils.LogError("GENERICENHANCE2504", "GenericEnhance failed.", $"\"{dstTypeName}\" is not exist generic parameter.", member);
                    }
                    else
                    {
                        ILPPUtils.LogError("GENERICENHANCE2504", "GenericEnhance failed.", $"\"{dstTypeName}\" is not exist generic parameter.", method, instruction);
                    }
                    return false;
                }

                srcType = dstType;
                result = true;
            }

            if (!result)
            {
                return false;
            }

            TryReplaceTypeInternal(ref srcType, member, method, instruction, expandedTypes);
            return result;
        }

        private TypeReference GetGenericArgument(TypeReference type, string dstTypeName)
        {
            if (!(type is GenericInstanceType genericInstance))
            {
                return null;
            }

            var typeDefinition = type.Resolve();
            if (typeDefinition == null)
            {
                return null;
            }

            var value = typeDefinition.GenericParameters.Select((v, i) => (v, i)).FirstOrDefault(v => v.v.Name == dstTypeName);
            if (value.v == null)
            {
                return null;
            }

            return genericInstance.GenericArguments[value.i];
        }

        private void DefaultTypeProcess(MethodDefinition method, Instruction instruction)
        {
            if (!(instruction.Operand is MethodReference calledMethod))
            {
                return;
            }

            var calledMethodDef = calledMethod.Resolve();
            if (calledMethodDef == null)
            {
                return;
            }

            var attrs =  calledMethodDef.CustomAttributes.ToArray();
            var sourceDefaultTypeArguments = attrs.Where(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.SourceDefaultType")
                                                     .Select(v => v.ConstructorArguments[0].Value as TypeReference);
            if (!sourceDefaultTypeArguments.Any())
            {
                return;
            }

            var parameterTypes = attrs.Where(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.SourceArgumentType")
                                      .Select(v =>
                                      {
                                          var a = v.ConstructorArguments[0].Value;
                                          if (a is TypeReference t)
                                          {
                                              return t.FullName;
                                          }
                                          
                                          if (a is string s)
                                          {
                                              return s;
                                          }

                                          return string.Empty;
                                      }).ToArray();

            IEnumerable<TypeReference> genericArguments = Array.Empty<TypeReference>();
            if (calledMethod is GenericInstanceMethod genericInstance)
            {
                genericArguments = genericInstance.GenericArguments;
            }

            if (sourceDefaultTypeArguments.Any())
            {
                genericArguments = genericArguments.Concat(sourceDefaultTypeArguments).ToArray();
            }

            var calledMethodName = calledMethodDef.Name;
            var calledGenericCount = genericArguments.Count();
            calledMethodDef = calledMethodDef.DeclaringType.GetMethods().FirstOrDefault(v => v.Name == calledMethodName &&
                                                                                             v.GenericParameters.Count == calledGenericCount &&
                                                                                             v.Parameters.Select(v => v.ParameterType.FullName).SequenceEqual(parameterTypes));
            if (calledMethodDef == null)
            {
                return;
            }

            var genericCalledMethod = new GenericInstanceMethod(calledMethodDef);
            foreach (var genericArgument in genericArguments)
            {
                TypeReference t = genericArgument;
                if (genericArgument.GetType() == typeof(TypeReference))
                {
                    var def = genericArgument.Resolve();
                    t = method.Module.ImportReference(def);
                }

                genericCalledMethod.GenericArguments.Add(t);
            }

            instruction.Operand = genericCalledMethod;
        }

        private void TypeFormulaProcess(TypeDefinition type)
        {
            {
                if (TryEmulateLiteralType(null, null, type.BaseType, out var result) &&
                    !type.BaseType.Is(result.TypeRef))
                {
                    type.BaseType = result.TypeRef;
                }
            }

            for (int i = 0; i < type.Interfaces.Count; ++i)
            {
                var @interface = type.Interfaces[i];
                if (TryEmulateLiteralType(null, null, @interface.InterfaceType, out var result) &&
                    !@interface.InterfaceType.Is(result.TypeRef))
                {
                    @interface.InterfaceType = result.TypeRef;
                }
            }

            for (int i = 0; i < type.GenericParameters.Count; ++i)
            {
                var genericParameter = type.GenericParameters[i];
                for (int j = 0; j < genericParameter.Constraints.Count; ++j)
                {
                    var constraint = genericParameter.Constraints[j];
                    if (TryEmulateLiteralType(null, null, constraint, out var result) &&
                        !constraint.Is(result.TypeRef))
                    {
                        genericParameter.Constraints[j] = result.TypeRef;
                    }
                }
            }
        }

        private void TypeFormulaProcess(FieldDefinition field)
        {
            if (TryEmulateLiteralType(null, null, field.FieldType, out var result) &&
                !field.FieldType.Is(result.TypeRef))
            {
                field.FieldType = result.TypeRef;
            }
        }

        private void TypeFormulaProcess(PropertyDefinition property)
        {
            var method = property.GetMethod ?? property.SetMethod;
            if (TryEmulateLiteralType(method, null, property.PropertyType, out var result) &&
                !property.PropertyType.Is(result.TypeRef))
            {
                property.PropertyType = result.TypeRef;
            }
        }

        private void TypeFormulaProcess(MethodDefinition method)
        {
            TypeFormulaProcess(method, method, null);

            for (int i = 0; i < method.Overrides.Count; ++i)
            {
                var @override = method.Overrides[i];
                if (TryEmulateLiteralType(method, null, @override.DeclaringType, out var result) &&
                    !@override.DeclaringType.Is(result.TypeRef))
                {
                    @override.DeclaringType = result.TypeRef;
                }

                TypeFormulaProcess(@override, method, null);
            }
        }

        private void TypeFormulaProcess(MethodReference method, MethodDefinition def, Instruction instruction)
        {
            {
                if (TryEmulateLiteralType(def, instruction, method.ReturnType, out var result) &&
                    !method.ReturnType.Is(result.TypeRef))
                {
                    method.ReturnType = result.TypeRef;
                }
            }

            for (int i = 0; i < method.Parameters.Count; ++i)
            {
                var parameter = method.Parameters[i];
                if (TryEmulateLiteralType(def, instruction, parameter.ParameterType, out var result) &&
                    !parameter.ParameterType.Is(result.TypeRef))
                {
                    parameter.ParameterType = result.TypeRef;
                }
            }

            for (int i = 0; i < method.GenericParameters.Count; ++i)
            {
                var genericParameter = method.GenericParameters[i];
                for (int j = 0; j < genericParameter.Constraints.Count; ++j)
                {
                    var constraint = genericParameter.Constraints[j];
                    if (TryEmulateLiteralType(def, instruction, constraint, out var result) &&
                        !constraint.Is(result.TypeRef))
                    {
                        genericParameter.Constraints[j] = result.TypeRef;
                    }
                }
            }
        }

        private void TypeFormulaProcess(MethodDefinition method, Instruction instruction)
        {
            if (instruction.Operand is GenericInstanceType genericInstanceType)
            {
                if (TryEmulateLiteralType(method, instruction, genericInstanceType, out var result) &&
                    !genericInstanceType.Is(result.TypeRef))
                {
                    instruction.Operand = result.TypeRef;
                }
            }

            if (instruction.Operand is MemberReference member)
            {
                {
                    if (TryEmulateLiteralType(method, instruction, member.DeclaringType, out var result) &&
                        !member.DeclaringType.Is(result.TypeRef))
                    {
                        member.DeclaringType = result.TypeRef;
                    }
                }

                if (member is GenericInstanceMethod genericInstanceMethod)
                {
                    for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; ++i)
                    {
                        var genericArgument = genericInstanceMethod.GenericArguments[i];
                        if (TryEmulateLiteralType(method, instruction, genericArgument, out var result) &&
                            !genericArgument.Is(result.TypeRef))
                        {
                            genericInstanceMethod.GenericArguments[i] = result.TypeRef;
                        }
                    }
                }
            }
        }

        private void TypeFormulaProcess(VariableDefinition variable, MethodDefinition def)
        {
            if (TryEmulateLiteralType(def, null, variable.VariableType, out var result) &&
                !variable.VariableType.Is(result.TypeRef))
            {
                variable.VariableType = result.TypeRef;
            }
        }

        private void TypeFormulaProcess(EventDefinition @event)
        {
            if (TryEmulateLiteralType(null, null, @event.EventType, out var result) &&
                !@event.EventType.Is(result.TypeRef))
            {
                @event.EventType = result.TypeRef;
            }
        }

        private bool TryEmulateLiteralType(MethodDefinition method, Instruction instruction, TypeReference typeRef, out TypeReferenceInfo result)
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

        private TypeReference CreateLiteralType(object value)
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
                return literalType;
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

            return literalType;
        }

        private void CreateMember(TypeDefinition literalType, TypeReference baseTypeRef, TypeReference propertyTypeRef, Type propertyType, string propertyName, object value)
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

        private void SpecializationProcess(Instruction instruction)
        {
            if (!TryGetSpecialization(instruction, out var specializationMethodRef, out var methodInfo))
            {
                return;
            }

            var arguments = specializationMethodRef.GenericArguments;
            if (arguments.Any(v => v.ContainsGenericParameter))
            {
                return;
            }

            var methodDef = methodInfo.MethodDef;
            var parameters = methodDef.GenericParameters;
            var argInfos = parameters
                                .Select((v, i) => (v, i))
                                .Join(arguments.Select((v, i) => (v, i)), v => v.i, v => v.i, (v1, v2) => (v1.v, v2.v))
                                .ToDictionary(v => v.Item1.Name, v => v.Item2);

            var returnType = methodDef.ReturnType;
            if (argInfos.TryGetValue(returnType.Name, out var returnTmp))
            {
                returnType = returnTmp;
            }

            var parameterTypes = methodDef.Parameters.Select(v =>
            {
                var parameterType = v.ParameterType;
                if (argInfos.TryGetValue(parameterType.Name, out var parameterTmp))
                {
                    parameterType = parameterTmp;
                }

                return parameterType;
            }).ToArray();

            var methods = methodDef.DeclaringType.Methods;
            MethodReference methodReference = null;
            foreach (var specializeInfo in methodInfo.SpecializeInfos)
            {
                if (!specializeInfo.BindTypes.All(v => argInfos[v.Key].Is(v.Value)))
                {
                    continue;
                }

                methodReference = methods.Where(v => v.Name == specializeInfo.SpecialMethod).FirstOrDefault(cmp =>
                {
                    if (cmp.GenericParameters.Count != 0 ||
                       !cmp.ReturnType.Is(returnType))
                    {
                        return false;
                    }

                    var index = 0;
                    for (int i = 0; i < parameterTypes.Length; ++i)
                    {
                        var cmpParameter = cmp.Parameters[index];
                        var parameterType = parameterTypes[index];
                        if (!cmpParameter.ParameterType.Is(parameterType))
                        {
                            ++index;
                            return false;
                        }
                        ++index;
                    }

                    return true;
                });

                if (methodReference != null)
                {
                    break;
                }
            }

            if (methodReference == null)
            {
                if (methodInfo.MethodDef.GenericParameters.Any(v => v.Constraints.Any(v => v.IsGenericInstance && v.Resolve().FullName == "Katuusagi.GenericEnhance.ITypeFormula`1")))
                {
                    return;
                }

                var defaultMethodDef = methods.Where(v => v.Name == methodInfo.DefaultMethod).FirstOrDefault(cmp =>
                {
                    if (cmp.GenericParameters.Count != methodDef.GenericParameters.Count)
                    {
                        return false;
                    }

                    if (!cmp.ReturnType.Is(methodDef.ReturnType) &&
                        !CompareGenericParameter(cmp.ReturnType, methodDef.ReturnType))
                    {
                        return false;
                    }

                    for (int i = 0; i < cmp.Parameters.Count; ++i)
                    {
                        var cmpParameter = cmp.Parameters[i];
                        var defParameter = methodDef.Parameters[i];
                        if (!cmpParameter.ParameterType.Is(defParameter.ParameterType) &&
                            !CompareGenericParameter(cmpParameter.ParameterType, defParameter.ParameterType))
                        {
                            return false;
                        }
                    }

                    return true;
                });

                methodReference = defaultMethodDef.MakeGenericInstanceMethod(specializationMethodRef.GenericArguments);
            }

            if (!methodReference.Resolve().IsPublic)
            {
                return;
            }

            instruction.Operand = methodReference;
            return;
        }

        private bool CompareGenericParameter(TypeReference x, TypeReference y)
        {
            return x is GenericParameter xp && y is GenericParameter yp && xp.Position == yp.Position;
        }

        private bool TryGetSpecialization(Instruction instruction, out GenericInstanceMethod result, out SpecializeMethodInfo methodInfo)
        {
            methodInfo = default;
            if (instruction.OpCode != OpCodes.Call &&
                instruction.OpCode != OpCodes.Callvirt &&
                instruction.OpCode != OpCodes.Ldftn)
            {
                result = null;
                return false;
            }

            if (!(instruction.Operand is GenericInstanceMethod methodRef))
            {
                result = null;
                return false;
            }

            var method = methodRef.Resolve();
            if (method == null)
            {
                result = null;
                return false;
            }

            if (_specializationResult.TryGetValue(method, out methodInfo))
            {
                result = methodRef;
                return methodInfo.Result;
            }

            var specializationMethodAttr = method.GetAttribute("Katuusagi.GenericEnhance.SpecializationMethod");
            if (specializationMethodAttr == null)
            {
                result = null;
                _specializationResult.Add(method, default);
                return false;
            }

            var defaultMethodName = specializationMethodAttr.ConstructorArguments.FirstOrDefault();

            var specializedMethods =  method.CustomAttributes.Where(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.SpecializedMethod").ToArray();
            var specializeInfos = new List<SpecializeInfo>();
            foreach (var specializedMethod in specializedMethods)
            {
                var specialMethod = specializedMethod.ConstructorArguments[0].Value as string;
                var vaargs = specializedMethod.ConstructorArguments[1].Value as CustomAttributeArgument[];

                var bindTypes = new Dictionary<string, TypeReference>();
                for (int i = 0; i < method.GenericParameters.Count; ++i)
                {
                    var genericParameterName = method.GenericParameters[i].Name;
                    var bindType = vaargs[i].Value as TypeReference;
                    bindTypes.Add(genericParameterName, bindType);
                }

                var specializeInfo = new SpecializeInfo();
                specializeInfo.SpecialMethod = specialMethod;
                specializeInfo.BindTypes = bindTypes;
                specializeInfos.Add(specializeInfo);
            }

            result = methodRef;
            methodInfo = new SpecializeMethodInfo()
            {
                Result = true,
                MethodDef = method,
                DefaultMethod = defaultMethodName.Value as string,
                SpecializeInfos = specializeInfos.ToArray()
            };
            _specializationResult.Add(method, methodInfo);
            return true;
        }

        private bool VariadicGenericProcess(MethodBody body, Instruction instruction)
        {
            if (body.Method.GetAttribute("Katuusagi.GenericEnhance.VariadicGeneric") == null)
            {
                return false;
            }

            if (instruction.OpCode == OpCodes.Call &&
                instruction.Operand is MethodReference method &&
                method.DeclaringType.FullName == "Katuusagi.GenericEnhance.VariadicUtils")
            {
                if (method.Name == "Break")
                {
                    var variadicForEachInit = SeekVariadicForEach(instruction);
                    if (variadicForEachInit == null)
                    {
                        return false;
                    }

                    var start = body.ExceptionHandlers.FirstOrDefault(v => v.TryStart == variadicForEachInit.Next);
                    var end = start.TryEnd.Previous;

                    instruction.OpCode = OpCodes.Br;
                    instruction.Operand = end;
                    return true;
                }

                if (method.Name == "Continue")
                {
                    var variadicForEachInit = SeekVariadicForEach(instruction);
                    if (variadicForEachInit == null)
                    {
                        return false;
                    }

                    var start = body.ExceptionHandlers.FirstOrDefault(v => v.TryStart == variadicForEachInit.Next);
                    var end = start.TryEnd.Previous;
                    var next = SeekNextContinueTarget(instruction, end);

                    instruction.OpCode = OpCodes.Br;
                    instruction.Operand = next;
                    return true;
                }
            }
            else if (instruction.OpCode == OpCodes.Ldsfld &&
                    instruction.Operand is FieldReference field &&
                    field.DeclaringType.FullName == "Katuusagi.GenericEnhance.VariadicUtils")
            {
                if (field.Name == "VariadicParameterCount")
                {
                    var generated = body.Method.GetAttribute("Katuusagi.GenericEnhance.VariadicGenerated");
                    if (generated == null)
                    {
                        return false;
                    }

                    var count = generated.ConstructorArguments[0].Value;
                    var load = ILPPUtils.LoadLiteral(count);
                    instruction.OpCode = load.OpCode;
                    instruction.Operand = load.Operand;
                    return true;
                }
            }

            return false;
        }

        private Instruction SeekVariadicForEach(Instruction instruction)
        {
            int count = 0;
            while (true)
            {
                instruction = instruction.Previous;
                if (instruction == null)
                {
                    break;
                }

                if (instruction.OpCode == OpCodes.Initobj &&
                    instruction.Operand is TypeReference type &&
                    type.FullName == "Katuusagi.GenericEnhance.VariadicForEach")
                {
                    if (count == 0)
                    {
                        return instruction;
                    }

                    ++count;
                    continue;
                }

                if (instruction.OpCode == OpCodes.Call &&
                    instruction.Operand is MethodReference method &&
                    method.DeclaringType.FullName == "Katuusagi.GenericEnhance.VariadicForEach" &&
                    method.Name == "Dispose")
                {
                    --count;
                    continue;
                }
            }

            return null;
        }

        private Instruction SeekNextContinueTarget(Instruction instruction, Instruction end)
        {
            int count = 0;
            while (instruction != end)
            {
                instruction = instruction.Next;
                if (instruction == null)
                {
                    break;
                }

                if (instruction.OpCode == OpCodes.Initobj &&
                    instruction.Operand is TypeReference type &&
                    type.FullName == "Katuusagi.GenericEnhance.VariadicForEach")
                {
                    ++count;
                    continue;
                }

                if (instruction.OpCode == OpCodes.Call &&
                    instruction.Operand is MethodReference method &&
                    method.DeclaringType.FullName == "Katuusagi.GenericEnhance.VariadicForEach" &&
                    method.Name == "Dispose")
                {
                    --count;
                    continue;
                }

                if (count > 0)
                {
                    continue;
                }

                if (instruction.OpCode == OpCodes.Call &&
                    instruction.Operand is MethodReference method2 &&
                    method2.DeclaringType.FullName == "Katuusagi.GenericEnhance.VariadicUtils" &&
                    method2.Name == "ContinueTarget")
                {
                    return instruction;
                }
            }

            return end;
        }

        private void NoneTypeProcess(FieldDefinition field)
        {
            if (TryReplaceNoneType(field.FieldType, out var fieldType, field, null, null))
            {
                field.FieldType = fieldType;
            }

            if (fieldType == _voidReference)
            {
                ILPPUtils.LogError("GENERICENHANCE5501", "GenericEnhance failed.", $"\"NoneType\" cannot be used in this context.", field);
            }
        }

        private void NoneTypeProcess(PropertyDefinition property)
        {
            if (TryReplaceNoneType(property.PropertyType, out var propertyType, property, null, null))
            {
                property.PropertyType = propertyType;
            }

            if (propertyType == _voidReference)
            {
                ILPPUtils.LogError("GENERICENHANCE5501", "GenericEnhance failed.", $"\"NoneType\" cannot be used in this context.", property);
            }
        }

        private bool NoneTypeProcess(TypeDefinition type, MethodDefinition method)
        {
            if (TryReplaceNoneType(method.ReturnType, out var returnType, null, method, null))
            {
                method.ReturnType = returnType;
            }

            for (int i = method.Parameters.Count - 1; i >= 0; --i)
            {
                var parameter = method.Parameters[i];
                if (TryReplaceNoneType(parameter.ParameterType, out var parameterType, null, method, null))
                {
                    parameter.ParameterType = parameterType;
                }

                if (parameterType == _voidReference)
                {
                    method.Parameters.RemoveAt(i);
                }
            }

            if (type.Methods.Count(v => v.Is(method)) >= 2)
            {
                type.Methods.Remove(method);
                return true;
            }

            return false;
        }

        private void NoneTypeProcess(MethodDefinition locationMethod, Instruction instruction)
        {
            if ((instruction.Operand is FieldReference field && (field.FieldType.FullName == "Katuusagi.GenericEnhance.NoneType" || field.FieldType == _voidReference)) ||
                (instruction.Operand is PropertyReference property &&  (property.PropertyType.FullName == "Katuusagi.GenericEnhance.NoneType" || property.PropertyType == _voidReference)) ||
                (instruction.Operand is EventReference @event && (@event.EventType.FullName == "Katuusagi.GenericEnhance.NoneType" || @event.EventType == _voidReference)))
            {
                instruction.OpCode = OpCodes.Nop;
                instruction.Operand = null;
                return;
            }

            if (instruction.Operand is TypeReference type)
            {
                if (TryReplaceNoneType(type, out type, null, locationMethod, instruction))
                {
                    instruction.Operand = type;
                }
                return;
            }

            if (instruction.Operand is MethodReference method)
            {
                if (TryReplaceNoneType(method, out method, null, locationMethod, instruction))
                {
                    instruction.Operand = method;
                }
                return;
            }
        }

        private void NoneTypeProcess(VariableDefinition variable, MethodDefinition method)
        {
            if (TryReplaceNoneType(variable.VariableType, out var variableType, null, method, null))
            {
                variable.VariableType = variableType;
            }

            if (variableType == _voidReference)
            {
                ILPPUtils.LogError("GENERICENHANCE5501", "GenericEnhance failed.", $"\"NoneType\" cannot be used in this context.", method);
            }
        }

        private void NoneTypeProcess(EventDefinition @event)
        {
            if (TryReplaceNoneType(@event.EventType, out var eventType, @event, null, null))
            {
                @event.EventType = eventType;
            }

            if (eventType == _voidReference)
            {
                ILPPUtils.LogError("GENERICENHANCE5501", "GenericEnhance failed.", $"\"NoneType\" cannot be used in this context.", @event);
            }
        }

        private bool TryReplaceNoneType(MethodReference method, out MethodReference result, MemberReference locationMember, MethodDefinition locationMethod, Instruction locationInstruction)
        {
            var methodDef = method.Resolve();
            if (methodDef == null)
            {
                if (TryReplaceNoneType(method.DeclaringType, out var methodDeclaringType, locationMember, locationMethod, locationInstruction))
                {
                    method.DeclaringType = methodDeclaringType;
                }

                if (TryReplaceNoneType(method.ReturnType, out var methodReturnType, locationMember, locationMethod, locationInstruction))
                {
                    method.ReturnType = methodReturnType;
                }

                for (int i = method.Parameters.Count - 1; i >= 0; --i)
                {
                    var parameter = method.Parameters[i];
                    if (TryReplaceNoneType(parameter.ParameterType, out var parameterType, locationMember, locationMethod, locationInstruction))
                    {
                        parameter.ParameterType = parameterType;
                    }

                    if (parameterType == _voidReference)
                    {
                        method.Parameters.RemoveAt(i);
                    }
                }

                methodDef = method.Resolve();
                if (methodDef == null)
                {
                    result = method;
                    return false;
                }
            }

            List<TypeReference> genArguments = null;
            var declaringType = method.DeclaringType;
            var returnType = methodDef.ReturnType;
            var parameters = methodDef.Parameters.ToList();
            var genParameters = methodDef.GenericParameters.ToList();

            bool isChanged = false;
            if (declaringType is GenericInstanceType genInstanceType)
            {
                var typeGenParameters = genInstanceType.Resolve().GenericParameters.ToArray();
                var typeGenArguments = genInstanceType.GenericArguments.ToArray();
                for (int i = typeGenArguments.Length - 1; i >= 0; --i)
                {
                    var genArg = typeGenArguments[i];
                    if (TryReplaceNoneType(genArg, out genArg, locationMember, locationMethod, locationInstruction))
                    {
                        typeGenArguments[i] = genArg;
                    }

                    if (genArg != _voidReference)
                    {
                        continue;
                    }

                    var genParameter = typeGenParameters[i];
                    if (returnType.Is(genParameter))
                    {
                        returnType = _voidReference;
                        isChanged = true;
                    }

                    for (int j = parameters.Count - 1; j >= 0; --j)
                    {
                        if (!parameters[j].ParameterType.Is(genParameter))
                        {
                            continue;
                        }

                        parameters.RemoveAt(j);
                        isChanged = true;
                    }
                }
            }

            if (TryReplaceNoneType(declaringType, out declaringType, locationMember, locationMethod, locationInstruction))
            {
                isChanged = true;
            }

            if (method is GenericInstanceMethod genInstanceMethod)
            {
                genArguments = genInstanceMethod.GenericArguments.ToList();
                for (int i = genArguments.Count - 1; i >= 0; --i)
                {
                    var genArg = genArguments[i];
                    if (TryReplaceNoneType(genArg, out genArg, locationMember, locationMethod, locationInstruction))
                    {
                        genArguments[i] = genArg;
                        isChanged = true;
                    }

                    if (genArg != _voidReference)
                    {
                        continue;
                    }

                    var genParameter = genParameters[i];

                    genArguments.RemoveAt(i);
                    genParameters.RemoveAt(i);
                    isChanged = true;

                    if (returnType.Is(genParameter))
                    {
                        returnType = _voidReference;
                    }

                    for (int j = parameters.Count - 1; j >= 0; --j)
                    {
                        if (!parameters[j].ParameterType.Is(genParameter))
                        {
                            continue;
                        }

                        parameters.RemoveAt(j);
                    }
                }
            }

            if (!isChanged)
            {
                result = method;
                return false;
            }

            var genArgCount = genArguments?.Count ?? 0;
            var declaringTypeDef = declaringType.Resolve();
            var genMethodDef = declaringTypeDef.Methods.FirstOrDefault(v => v.Name == methodDef.Name &&
                                                                            v.GenericParameters.Count == genArgCount &&
                                                                            v.Parameters.Select(v => v.ParameterType).SequenceEqual(parameters.Select(v2 => v2.ParameterType), TypeReferenceComparer.Default));
            if (genMethodDef == null)
            {
                if (locationMethod == null)
                {
                    ILPPUtils.LogError("GENERICENHANCE5502", "GenericEnhance failed.", $"No method matching \"{method}\" found.", locationMember);
                }
                else
                {
                    ILPPUtils.LogError("GENERICENHANCE5502", "GenericEnhance failed.", $"No method matching \"{method}\" found.", locationMethod, locationInstruction);
                }
                result = method;
                return false;
            }

            var genMethodRef = _module.ImportReference(genMethodDef);
            genMethodRef.DeclaringType = declaringType;

            if (genArgCount > 0)
            {
                result = genMethodRef.MakeGenericInstanceMethod(genArguments.ToArray());
                result = _module.ImportReference(result, genMethodRef);
            }
            else
            {
                result = _module.ImportReference(genMethodRef);
            }

            result = _module.ImportReference(result);
            return true;
        }

        private bool TryReplaceNoneType(TypeReference type, out TypeReference result, MemberReference locationMember, MethodDefinition locationMethod, Instruction locationInstruction)
        {
            if (!(type is GenericInstanceType genInstanceType))
            {
                if (type.FullName == "Katuusagi.GenericEnhance.NoneType")
                {
                    result = _voidReference;
                    return true;
                }

                result = type;
                return false;
            }

            bool isChanged = false;
            var genArguments = genInstanceType.GenericArguments.ToList();
            for (int i = genArguments.Count - 1; i >= 0; --i)
            {
                var genArg = genArguments[i];
                if (TryReplaceNoneType(genArg, out genArg, locationMember, locationMethod, locationInstruction))
                {
                    genArguments[i] = genArg;
                    isChanged = true;
                }

                if (genArg != _voidReference)
                {
                    continue;
                }

                genArguments.RemoveAt(i);
                isChanged = true;
            }

            if (!isChanged)
            {
                result = type;
                return false;
            }

            var genTypeDef = type.Resolve().Module.Types.GetAllTypes().FirstOrDefault(v => v.Namespace == genInstanceType.Namespace &&
                                                                                           v.Name.Split('`')[0] == genInstanceType.Name.Split('`')[0] &&
                                                                                           v.GenericParameters.Count == genArguments.Count);
            if (genTypeDef == null)
            {
                if (locationMethod == null)
                {
                    ILPPUtils.LogError("GENERICENHANCE5503", "GenericEnhance failed.", $"No type matching \"{type}\" found.", locationMember);
                }
                else
                {
                    ILPPUtils.LogError("GENERICENHANCE5503", "GenericEnhance failed.", $"No type matching \"{type}\" found.", locationMethod, locationInstruction);
                }
                result = type;
                return false;
            }

            var genTypeRef = _module.ImportReference(genTypeDef);
            if (genArguments.Any())
            {
                result = genTypeRef.MakeGenericInstanceType(genArguments.ToArray());
                result = _module.ImportReference(result, genTypeRef);
            }
            else
            {
                result = _module.ImportReference(genTypeRef);
            }
            return true;
        }
    }
}

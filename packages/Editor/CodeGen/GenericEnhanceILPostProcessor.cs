using Katuusagi.GenericEnhance.Editor.Utils;
using Katuusagi.ILPostProcessorCommon.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Katuusagi.GenericEnhance.Editor
{
    internal class GenericEnhanceILPostProcessor : ILPostProcessor
    {
        public class TypeReferenceHashSetPool : ThreadStaticCollectionPool<HashSet<TypeReference>, TypeReference, TypeReferenceHashSetPool>
        {
            static TypeReferenceHashSetPool()
            {
                _createInstance = () => new HashSet<TypeReference>(TypeReferenceComparer.Default);
            }

            public static Handle Get(out HashSet<TypeReference> result, IEnumerable<TypeReference> init)
            {
                var ret = Get(out result);
                result.UnionWith(init);
                return ret;
            }
        }

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
            public (string name, TypeReference type)[] BindTypes;
        }

        private Dictionary<MethodReference, SpecializeMethodInfo> _specializationResult = new Dictionary<MethodReference, SpecializeMethodInfo>(MethodReferenceComparer.Default);
        private ModuleDefinition _module;

        private TypeReference _voidReference;

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
                    TypeFormulaUtils.Init(_module);

                    using (ThreadStaticArrayPool.Get(out var types, assembly.Modules.SelectMany(v => v.Types).GetAllTypes()))
                    {
                        foreach (var type in types)
                        {
                            TypeDefProcess(type);
                            TypeFormulaProcess(type);

                            using (ThreadStaticArrayPool.Get(out var fields, type.Fields))
                            {
                                foreach (var field in fields)
                                {
                                    TypeDefProcess(field);
                                    TypeFormulaProcess(field);
                                    NoneTypeProcess(field);
                                }
                            }

                            using (ThreadStaticArrayPool.Get(out var properties, type.Properties))
                            {
                                foreach (var property in properties)
                                {
                                    TypeDefProcess(property);
                                    TypeFormulaProcess(property);
                                    NoneTypeProcess(property);
                                }
                            }

                            using (ThreadStaticArrayPool.Get(out var methods, type.Methods))
                            {
                                foreach (var method in methods)
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
                                        isChanged = TypeDefProcess(method, instruction) || isChanged;
                                        isChanged = DefaultTypeProcess(method, instruction) || isChanged;
                                        isChanged = TypeFormulaProcess(method, instruction) || isChanged;
                                        isChanged = SpecializationProcess(instruction) || isChanged;
                                        isChanged = VariadicGenericProcess(body, instruction) || isChanged;
                                        isChanged = NoneTypeProcess(method, instruction) || isChanged;
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
                        }
                    }

                    return compiledAssembly.GetResult(assembly);
                }
            }
            catch (Exception e)
            {
                ILPPUtils.LogException(e);
            }
            return compiledAssembly.GetNullResult();
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
#if UNITY_2022_1_OR_NEWER
                        isValueType = genericParameter.Constraints.Any(v => v.ConstraintType.FullName == "System.ValueType");
#else
                        isValueType = genericParameter.Constraints.Any(v => v.FullName == "System.ValueType");
#endif
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
#if UNITY_2022_1_OR_NEWER
                    var constraintType = constraint.ConstraintType;
                    if (TryReplaceType(ref constraintType, type, null, null))
#else
                    if (TryReplaceType(ref constraint, type, null, null))
#endif
                    {
#if UNITY_2022_1_OR_NEWER
                        var constraintTmp = new GenericParameterConstraint(constraintType);
                        constraintTmp.MetadataToken = constraint.MetadataToken;
                        constraint = constraintTmp;
#endif
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
#if UNITY_2022_1_OR_NEWER
                    var constraintType = constraint.ConstraintType;
                    if (TryReplaceType(ref constraintType, method, def, instruction))
#else
                    if (TryReplaceType(ref constraint, method, def, instruction))
#endif
                    {
#if UNITY_2022_1_OR_NEWER
                        var constraintTmp = new GenericParameterConstraint(constraintType);
                        constraintTmp.MetadataToken = constraint.MetadataToken;
                        constraint = constraintTmp;
#endif
                        genericParameter.Constraints[j] = constraint;
                    }
                }
            }
        }

        private bool TypeDefProcess(MethodDefinition method, Instruction instruction)
        {
            bool isChanged = false;
            if (instruction.Operand is TypeReference type)
            {
                if (TryReplaceType(ref type, method, method, instruction))
                {
                    isChanged = true;
                    instruction.Operand = type;
                }
            }

            if (instruction.Operand is MemberReference member)
            {
                {
                    var declaring = member.DeclaringType;
                    if (TryReplaceType(ref declaring, method, method, instruction))
                    {
                        isChanged = true;
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
                            isChanged = true;
                            genericInstanceMethod.GenericArguments[i] = genericArgument;
                        }
                    }
                }

                if (isChanged &&
                    member.Resolve() == null)
                {
                    ILPPUtils.LogError("GENERICENHANCE2503", "GenericEnhance failed.", $"\"{member.DeclaringType}\" has not \"{member}\".", method, instruction);
                }
            }

            return isChanged;
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
            using (TypeReferenceHashSetPool.Get(out var expandedTypes))
            {
                return TryReplaceTypeInternal(ref srcType, member, method, instruction, expandedTypes);
            }
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
                    using (TypeReferenceHashSetPool.Get(out var subExpandedTypes, expandedTypes))
                    {
                        if (TryReplaceTypeInternal(ref element, member, method, instruction, subExpandedTypes))
                        {
                            genericInstanceType.GenericArguments[i] = element;
                            result = true;
                        }
                    }
                }
            }
            else if (srcType is ArrayType arrayType)
            {
                var element = arrayType.ElementType;
                using (TypeReferenceHashSetPool.Get(out var subExpandedTypes, expandedTypes))
                {
                    if (TryReplaceTypeInternal(ref element, member, method, instruction, subExpandedTypes))
                    {
                        srcType = new ArrayType(element);
                        result = true;
                    }
                }
            }
            else if (srcType is PointerType pointerType)
            {
                var element = pointerType.ElementType;
                using (TypeReferenceHashSetPool.Get(out var subExpandedTypes, expandedTypes))
                {
                    if (TryReplaceTypeInternal(ref element, member, method, instruction, subExpandedTypes))
                    {
                        srcType = new PointerType(element);
                        result = true;
                    }
                }
            }
            else if (srcType is ByReferenceType byRefType)
            {
                var element = byRefType.ElementType;
                using (TypeReferenceHashSetPool.Get(out var subExpandedTypes, expandedTypes))
                {
                    if (TryReplaceTypeInternal(ref element, member, method, instruction, subExpandedTypes))
                    {
                        srcType = new ByReferenceType(element);
                        result = true;
                    }
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

        private bool DefaultTypeProcess(MethodDefinition method, Instruction instruction)
        {
            if (!(instruction.Operand is MethodReference calledMethod))
            {
                return false;
            }

            var calledMethodDef = calledMethod.Resolve();
            if (calledMethodDef == null)
            {
                return false;
            }

            using (ThreadStaticArrayPool.Get(out var attrs, calledMethodDef.CustomAttributes))
            {
                var sourceDefaultTypeArguments = attrs.Where(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.SourceDefaultType")
                                                     .Select(v => v.ConstructorArguments[0].Value as TypeReference);
                if (!sourceDefaultTypeArguments.Any())
                {
                    return false;
                }

                var parameterTypesQuery = attrs.Where(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.SourceArgumentType")
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
                                      });
                using (ThreadStaticArrayPool.Get(out var parameterTypes, parameterTypesQuery))
                using (ThreadStaticListPool.Get<TypeReference>(out var genericArguments))
                {
                    if (calledMethod is GenericInstanceMethod genericInstance)
                    {
                        genericArguments.AddRange(genericInstance.GenericArguments);
                    }

                    if (sourceDefaultTypeArguments.Any())
                    {
                        genericArguments.AddRange(sourceDefaultTypeArguments);
                    }

                    var calledMethodName = calledMethodDef.Name;
                    var calledGenericCount = genericArguments.Count();
                    calledMethodDef = calledMethodDef.DeclaringType.GetMethods().FirstOrDefault(v => v.Name == calledMethodName &&
                                                                                                     v.GenericParameters.Count == calledGenericCount &&
                                                                                                     v.Parameters.Select(v => v.ParameterType.FullName).SequenceEqual(parameterTypes));
                    if (calledMethodDef == null)
                    {
                        return false;
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
                    return true;
                }
            }
        }

        private void TypeFormulaProcess(TypeDefinition type)
        {
            {
                if (TypeFormulaUtils.TryEmulateLiteralType(null, null, type.BaseType, out var result) &&
                    !type.BaseType.Is(result.TypeRef))
                {
                    type.BaseType = result.TypeRef;
                }
            }

            for (int i = 0; i < type.Interfaces.Count; ++i)
            {
                var @interface = type.Interfaces[i];
                if (TypeFormulaUtils.TryEmulateLiteralType(null, null, @interface.InterfaceType, out var result) &&
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
#if UNITY_2022_1_OR_NEWER
                    var constraint = genericParameter.Constraints[j];
                    var constraintType = constraint.ConstraintType;
#else
                    var constraintType = genericParameter.Constraints[j];
#endif
                    if (TypeFormulaUtils.TryEmulateLiteralType(null, null, constraintType, out var result) &&
                        !constraintType.Is(result.TypeRef))
                    {
#if UNITY_2022_1_OR_NEWER
                        var constraintTmp = new GenericParameterConstraint(result.TypeRef);
                        constraintTmp.MetadataToken = constraint.MetadataToken;
                        genericParameter.Constraints[j] = constraintTmp;
#else
                        genericParameter.Constraints[j] = result.TypeRef;
#endif
                    }
                }
            }
        }

        private void TypeFormulaProcess(FieldDefinition field)
        {
            if (TypeFormulaUtils.TryEmulateLiteralType(null, null, field.FieldType, out var result) &&
                !field.FieldType.Is(result.TypeRef))
            {
                field.FieldType = result.TypeRef;
            }
        }

        private void TypeFormulaProcess(PropertyDefinition property)
        {
            var method = property.GetMethod ?? property.SetMethod;
            if (TypeFormulaUtils.TryEmulateLiteralType(method, null, property.PropertyType, out var result) &&
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
                if (TypeFormulaUtils.TryEmulateLiteralType(method, null, @override.DeclaringType, out var result) &&
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
                if (TypeFormulaUtils.TryEmulateLiteralType(def, instruction, method.ReturnType, out var result) &&
                    !method.ReturnType.Is(result.TypeRef))
                {
                    method.ReturnType = result.TypeRef;
                }
            }

            for (int i = 0; i < method.Parameters.Count; ++i)
            {
                var parameter = method.Parameters[i];
                if (TypeFormulaUtils.TryEmulateLiteralType(def, instruction, parameter.ParameterType, out var result) &&
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
#if UNITY_2022_1_OR_NEWER
                    var constraint = genericParameter.Constraints[j];
                    var constraintType = constraint.ConstraintType;
#else
                    var constraintType = genericParameter.Constraints[j];
#endif
                    if (TypeFormulaUtils.TryEmulateLiteralType(def, instruction, constraintType, out var result) &&
                        !constraintType.Is(result.TypeRef))
                    {
#if UNITY_2022_1_OR_NEWER
                        var constraintTmp = new GenericParameterConstraint(result.TypeRef);
                        constraintTmp.MetadataToken = constraint.MetadataToken;
                        genericParameter.Constraints[j] = constraintTmp;
#else
                        genericParameter.Constraints[j] = result.TypeRef;
#endif
                    }
                }
            }
        }

        private bool TypeFormulaProcess(MethodDefinition method, Instruction instruction)
        {
            bool isChanged = false;
            if (instruction.Operand is GenericInstanceType genericInstanceType)
            {
                if (TypeFormulaUtils.TryEmulateLiteralType(method, instruction, genericInstanceType, out var result) &&
                    !genericInstanceType.Is(result.TypeRef))
                {
                    isChanged = true;
                    instruction.Operand = result.TypeRef;
                }
            }

            if (instruction.Operand is MemberReference member)
            {
                {
                    if (TypeFormulaUtils.TryEmulateLiteralType(method, instruction, member.DeclaringType, out var result) &&
                        !member.DeclaringType.Is(result.TypeRef))
                    {
                        isChanged = true;
                        member.DeclaringType = result.TypeRef;
                    }
                }

                if (member is GenericInstanceMethod genericInstanceMethod)
                {
                    for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; ++i)
                    {
                        var genericArgument = genericInstanceMethod.GenericArguments[i];
                        if (TypeFormulaUtils.TryEmulateLiteralType(method, instruction, genericArgument, out var result) &&
                            !genericArgument.Is(result.TypeRef))
                        {
                            isChanged = true;
                            genericInstanceMethod.GenericArguments[i] = result.TypeRef;
                        }
                    }
                }
            }

            return isChanged;
        }

        private void TypeFormulaProcess(VariableDefinition variable, MethodDefinition def)
        {
            if (TypeFormulaUtils.TryEmulateLiteralType(def, null, variable.VariableType, out var result) &&
                !variable.VariableType.Is(result.TypeRef))
            {
                variable.VariableType = result.TypeRef;
            }
        }

        private void TypeFormulaProcess(EventDefinition @event)
        {
            if (TypeFormulaUtils.TryEmulateLiteralType(null, null, @event.EventType, out var result) &&
                !@event.EventType.Is(result.TypeRef))
            {
                @event.EventType = result.TypeRef;
            }
        }

        private bool SpecializationProcess(Instruction instruction)
        {
            if (!TryGetSpecialization(instruction, out var specializationMethodRef, out var methodInfo))
            {
                return false;
            }

            var arguments = specializationMethodRef.GenericArguments;
            if (arguments.Any(v => v.ContainsGenericParameter))
            {
                return false;
            }

            var methodDef = methodInfo.MethodDef;
            var parameters = methodDef.GenericParameters;

            using (ThreadStaticDictionaryPool.Get<string, TypeReference>(out var argInfos))
            {
                var argInfoPairs = parameters
                                .Select((v, i) => (v, i))
                                .Join(arguments.Select((v, i) => (v, i)), v => v.i, v => v.i, (v1, v2) => (v1.v, v2.v));
                foreach (var pair in argInfoPairs)
                {
                    argInfos.Add(pair.Item1.Name, pair.Item2);
                }

                var returnType = methodDef.ReturnType;
                if (argInfos.TryGetValue(returnType.Name, out var returnTmp))
                {
                    returnType = returnTmp;
                }

                var parameterTypesQuery = methodDef.Parameters.Select(v =>
                {
                    var parameterType = v.ParameterType;
                    if (argInfos.TryGetValue(parameterType.Name, out var parameterTmp))
                    {
                        parameterType = parameterTmp;
                    }

                    return parameterType;
                });

                using (ThreadStaticArrayPool.Get(out var parameterTypes, parameterTypesQuery))
                {
                    var methods = methodDef.DeclaringType.Methods;
                    MethodReference methodReference = null;
                    foreach (var specializeInfo in methodInfo.SpecializeInfos)
                    {
                        if (!specializeInfo.BindTypes.All(v => argInfos[v.name].Is(v.type)))
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
#if UNITY_2022_1_OR_NEWER
                        if (methodInfo.MethodDef.GenericParameters.Any(v => v.Constraints.Any(v => v.ConstraintType.IsGenericInstance && v.ConstraintType.Resolve().FullName == "Katuusagi.GenericEnhance.ITypeFormula`1")))
#else
                        if (methodInfo.MethodDef.GenericParameters.Any(v => v.Constraints.Any(v => v.IsGenericInstance && v.Resolve().FullName == "Katuusagi.GenericEnhance.ITypeFormula`1")))
#endif
                        {
                            return false;
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
                        return false;
                    }

                    instruction.Operand = methodReference;
                    return true;
                }
            }
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

            using (ThreadStaticArrayPool.Get(out var specializedMethods, method.CustomAttributes.Where(v => v.AttributeType.FullName == "Katuusagi.GenericEnhance.SpecializedMethod")))
            using (ThreadStaticListPool.Get<SpecializeInfo>(out var specializeInfos))
            {
                foreach (var specializedMethod in specializedMethods)
                {
                    var specialMethod = specializedMethod.ConstructorArguments[0].Value as string;
                    var vaargs = specializedMethod.ConstructorArguments[1].Value as CustomAttributeArgument[];

                    var bindTypes = new (string name, TypeReference type)[method.GenericParameters.Count];
                    for (int i = 0; i < method.GenericParameters.Count; ++i)
                    {
                        var genericParameterName = method.GenericParameters[i].Name;
                        var bindType = vaargs[i].Value as TypeReference;
                        bindTypes[i] = (genericParameterName, bindType);
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
            }
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

        private bool NoneTypeProcess(MethodDefinition locationMethod, Instruction instruction)
        {
            if ((instruction.Operand is FieldReference field && (field.FieldType.FullName == "Katuusagi.GenericEnhance.NoneType" || field.FieldType == _voidReference)) ||
                (instruction.Operand is PropertyReference property &&  (property.PropertyType.FullName == "Katuusagi.GenericEnhance.NoneType" || property.PropertyType == _voidReference)) ||
                (instruction.Operand is EventReference @event && (@event.EventType.FullName == "Katuusagi.GenericEnhance.NoneType" || @event.EventType == _voidReference)))
            {
                instruction.OpCode = OpCodes.Nop;
                instruction.Operand = null;
                return true;
            }

            if (instruction.Operand is TypeReference type)
            {
                if (TryReplaceNoneType(type, out type, null, locationMethod, instruction))
                {
                    instruction.Operand = type;
                    return true;
                }
                return false;
            }

            if (instruction.Operand is MethodReference method)
            {
                if (TryReplaceNoneType(method, out method, null, locationMethod, instruction))
                {
                    instruction.Operand = method;
                    return true;
                }
                return false;
            }

            return false;
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

            var genArgCount = 0;
            var declaringType = method.DeclaringType;
            var returnType = methodDef.ReturnType;
            bool isChanged = false;
            using (ThreadStaticListPool.Get(out var parameters, methodDef.Parameters))
            using (ThreadStaticListPool.Get(out var genParameters, methodDef.GenericParameters))
            using (ThreadStaticListPool.Get<TypeReference>(out var genArguments))
            {
                if (declaringType is GenericInstanceType genInstanceType)
                {
                    var typeGenParameters = genInstanceType.Resolve().GenericParameters;
                    var typeGenArguments = genInstanceType.GenericArguments;
                    for (int i = typeGenArguments.Count - 1; i >= 0; --i)
                    {
                        var genArg = typeGenArguments[i];
                        TryReplaceNoneType(genArg, out genArg, locationMember, locationMethod, locationInstruction);

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
                    genArguments.AddRange(genInstanceMethod.GenericArguments);
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

                    genArgCount = genArguments.Count;
                }

                if (!isChanged)
                {
                    result = method;
                    return false;
                }

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
                    result = genMethodRef.MakeGenericInstanceMethod(genArguments);
                    result = _module.ImportReference(result, genMethodRef);
                }
                else
                {
                    result = _module.ImportReference(genMethodRef);
                }
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
            using (ThreadStaticListPool.Get(out var genArguments, genInstanceType.GenericArguments))
            {
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
                    result = genTypeRef.MakeGenericInstanceType(genArguments);
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
}

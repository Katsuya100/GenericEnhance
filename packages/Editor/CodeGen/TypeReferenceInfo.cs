using Mono.Cecil;
using System;
using System.Text.RegularExpressions;

namespace Katuusagi.GenericEnhance.Editor
{
    public struct TypeReferenceInfo
    {
        private static readonly Regex Integer = new Regex("(?<=(^_))n?\\d+(?=($))");
        private static readonly Regex Float = new Regex("(?<=(^_))n?(\\d+_\\d+)(?=($))");

        public TypeReference TypeRef;
        public TypeStyle Style;
        public object Value;
        public Type Type;

        public TypeReferenceInfo(TypeReference typeRef)
        {
            TypeRef = typeRef;
            Style = GetTypeStyle(typeRef.Resolve());
            Value = GetValue(TypeRef, Style);
            Type = GetStyleType(Style);
        }

        private static object GetValue(TypeReference typeRef, TypeStyle style)
        {
            var name = typeRef.FullName;
            switch (style)
            {
                case TypeStyle.BooleanLiteral:
                    {
                        switch (name)
                        {
                            case "_true":
                                return true;
                            case "_false":
                                return false;
                        }
                        return false;
                    }
                case TypeStyle.IntegerLiteral:
                    {
                        var match = Integer.Match(name);
                        if (match.Success)
                        {
                            var valueStr = match.Value;
                            valueStr = valueStr.Replace("n", "-");
                            if (valueStr[0] == '-')
                            {
                                return long.Parse(valueStr);
                            }
                            else
                            {
                                return ulong.Parse(valueStr);
                            }
                        }
                        return default(long);
                    }

                case TypeStyle.FloatLiteral:
                    {
                        var match = Float.Match(name);
                        if (match.Success)
                        {
                            var valueStr = match.Value;
                            valueStr = valueStr.Replace("_", ".").Replace("n", "-");
                            return double.Parse(valueStr);
                        }
                        return default(double);
                    }
            }

            return null;
        }

        private static Type GetStyleType(TypeStyle style)
        {
            switch (style)
            {
                case TypeStyle.Boolean:
                    return typeof(bool);
                case TypeStyle.Int8:
                    return typeof(sbyte);
                case TypeStyle.UInt8:
                    return typeof(byte);
                case TypeStyle.Int16:
                    return typeof(short);
                case TypeStyle.UInt16:
                    return typeof(ushort);
                case TypeStyle.Int32:
                    return typeof(int);
                case TypeStyle.UInt32:
                    return typeof(uint);
                case TypeStyle.Int64:
                    return typeof(long);
                case TypeStyle.UInt64:
                    return typeof(ulong);
                case TypeStyle.Single:
                    return typeof(float);
                case TypeStyle.Double:
                    return typeof(double);
            }

            return null;
        }

        private static TypeStyle GetTypeStyle(TypeDefinition type)
        {
            if (type == null)
            {
                return TypeStyle.None;
            }

            var typeName = type.FullName;
            switch (typeName)
            {
                case "System.Boolean":
                    return TypeStyle.Boolean;
                case "System.Int8":
                    return TypeStyle.Int8;
                case "System.UInt8":
                    return TypeStyle.UInt8;
                case "System.Int16":
                    return TypeStyle.Int16;
                case "System.UInt16":
                    return TypeStyle.UInt16;
                case "System.Int32":
                    return TypeStyle.Int32;
                case "System.UInt32":
                    return TypeStyle.UInt32;
                case "System.Int64":
                    return TypeStyle.Int64;
                case "System.UInt64":
                    return TypeStyle.UInt64;
                case "System.Single":
                    return TypeStyle.Single;
                case "System.Double":
                    return TypeStyle.Double;
                case "Katuusagi.GenericEnhance._true":
                case "Katuusagi.GenericEnhance._false":
                    return TypeStyle.Boolean;
                case "Katuusagi.GenericEnhance.Add`3":
                    return TypeStyle.Add;
                case "Katuusagi.GenericEnhance.Sub`3":
                    return TypeStyle.Sub;
                case "Katuusagi.GenericEnhance.Mul`3":
                    return TypeStyle.Mul;
                case "Katuusagi.GenericEnhance.Div`3":
                    return TypeStyle.Div;
                case "Katuusagi.GenericEnhance.Mod`3":
                    return TypeStyle.Mod;
                case "Katuusagi.GenericEnhance.Minus`2":
                    return TypeStyle.Minus;
                case "Katuusagi.GenericEnhance.BitAnd`3":
                    return TypeStyle.BitAnd;
                case "Katuusagi.GenericEnhance.BitOr`3":
                    return TypeStyle.BitOr;
                case "Katuusagi.GenericEnhance.BitXor`3":
                    return TypeStyle.BitXor;
                case "Katuusagi.GenericEnhance.BitNot`2":
                    return TypeStyle.BitNot;
                case "Katuusagi.GenericEnhance.LShift`3":
                    return TypeStyle.LShift;
                case "Katuusagi.GenericEnhance.RShift`3":
                    return TypeStyle.RShift;
                case "Katuusagi.GenericEnhance.CastNumeric`2":
                    return TypeStyle.CastNumeric;
                case "Katuusagi.GenericEnhance.Not`1":
                    return TypeStyle.Not;
                case "Katuusagi.GenericEnhance.And`2":
                    return TypeStyle.And;
                case "Katuusagi.GenericEnhance.Or`2":
                    return TypeStyle.Or;
                case "Katuusagi.GenericEnhance.Equal`3":
                    return TypeStyle.Equal;
                case "Katuusagi.GenericEnhance.NotEqual`3":
                    return TypeStyle.NotEqual;
                case "Katuusagi.GenericEnhance.Greater`3":
                    return TypeStyle.Greater;
                case "Katuusagi.GenericEnhance.GreaterOrEqual`3":
                    return TypeStyle.GreaterOrEqual;
                case "Katuusagi.GenericEnhance.Less`3":
                    return TypeStyle.Less;
                case "Katuusagi.GenericEnhance.LessOrEqual`3":
                    return TypeStyle.LessOrEqual;
            }

            if (typeName == "_true" ||
                typeName == "_false")
            {
                return TypeStyle.BooleanLiteral;
            }

            if (Integer.IsMatch(typeName))
            {
                return TypeStyle.IntegerLiteral;
            }

            if (Float.IsMatch(typeName))
            {
                return TypeStyle.FloatLiteral;
            }

            return TypeStyle.None;
        }
    }
}

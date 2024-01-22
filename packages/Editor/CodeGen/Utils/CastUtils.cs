using System;

namespace Katuusagi.GenericEnhance.Editor.Utils
{
    public static class CastUtils
    {
        public static T CastNumeric<T>(object src)
        {
            return (T)CastNumeric(typeof(T), src);
        }

        public static object CastNumeric(Type dstType, object src)
        {
            if (src == null)
            {
                return null;
            }

            var srcType = src.GetType();
            if (srcType == dstType)
            {
                return src;
            }

            if (src is sbyte sbyteValue)
            {
                if (dstType == typeof(byte))
                {
                    var result = (byte)sbyteValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)sbyteValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)sbyteValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)sbyteValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)sbyteValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)sbyteValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)sbyteValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)sbyteValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)sbyteValue;
                    return result;
                }
            }
            if (src is byte byteValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)byteValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)byteValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)byteValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)byteValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)byteValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)byteValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)byteValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)byteValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)byteValue;
                    return result;
                }
            }
            if (src is short shortValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)shortValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)shortValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)shortValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)shortValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)shortValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)shortValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)shortValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)shortValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)shortValue;
                    return result;
                }
            }
            if (src is ushort ushortValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)ushortValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)ushortValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)ushortValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)ushortValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)ushortValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)ushortValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)ushortValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)ushortValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)ushortValue;
                    return result;
                }
            }
            if (src is int intValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)intValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)intValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)intValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)intValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)intValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)intValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)intValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)intValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)intValue;
                    return result;
                }
            }
            if (src is uint uintValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)uintValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)uintValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)uintValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)uintValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)uintValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)uintValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)uintValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)uintValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)uintValue;
                    return result;
                }
            }
            if (src is long longValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)longValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)longValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)longValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)longValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)longValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)longValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)longValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)longValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)longValue;
                    return result;
                }
            }
            if (src is ulong ulongValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)ulongValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)ulongValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)ulongValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)ulongValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)ulongValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)ulongValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)ulongValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)ulongValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)ulongValue;
                    return result;
                }
            }
            if (src is float floatValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)floatValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)floatValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)floatValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)floatValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)floatValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)floatValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)floatValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)floatValue;
                    return result;
                }
                if (dstType == typeof(double))
                {
                    var result = (double)floatValue;
                    return result;
                }
            }
            if (src is double doubleValue)
            {
                if (dstType == typeof(sbyte))
                {
                    var result = (sbyte)doubleValue;
                    return result;
                }
                if (dstType == typeof(byte))
                {
                    var result = (byte)doubleValue;
                    return result;
                }
                if (dstType == typeof(short))
                {
                    var result = (short)doubleValue;
                    return result;
                }
                if (dstType == typeof(ushort))
                {
                    var result = (ushort)doubleValue;
                    return result;
                }
                if (dstType == typeof(int))
                {
                    var result = (int)doubleValue;
                    return result;
                }
                if (dstType == typeof(uint))
                {
                    var result = (uint)doubleValue;
                    return result;
                }
                if (dstType == typeof(long))
                {
                    var result = (long)doubleValue;
                    return result;
                }
                if (dstType == typeof(ulong))
                {
                    var result = (ulong)doubleValue;
                    return result;
                }
                if (dstType == typeof(float))
                {
                    var result = (float)doubleValue;
                    return result;
                }
            }
            return null;
        }
    }
}

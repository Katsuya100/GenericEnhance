using System;

namespace Katuusagi.GenericEnhance.Editor.Utils
{
    public static class BitLogicalUtils
    {
        public static object Not(Type type, object x)
        {
            if (x == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return ~CastUtils.CastNumeric<sbyte>(x);
            }
            if (type == typeof(byte))
            {
                return ~CastUtils.CastNumeric<byte>(x);
            }
            if (type == typeof(short))
            {
                return ~CastUtils.CastNumeric<short>(x);
            }
            if (type == typeof(ushort))
            {
                return ~CastUtils.CastNumeric<ushort>(x);
            }
            if (type == typeof(int))
            {
                return ~CastUtils.CastNumeric<int>(x);
            }
            if (type == typeof(uint))
            {
                return ~CastUtils.CastNumeric<uint>(x);
            }
            if (type == typeof(long))
            {
                return ~CastUtils.CastNumeric<long>(x);
            }
            if (type == typeof(ulong))
            {
                return ~CastUtils.CastNumeric<ulong>(x);
            }
            return null;
        }

        public static object And(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) & CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) & CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) & CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) & CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) & CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) & CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) & CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) & CastUtils.CastNumeric<ulong>(y);
            }
            return null;
        }

        public static object Or(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) | CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) | CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) | CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) | CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) | CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) | CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) | CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) | CastUtils.CastNumeric<ulong>(y);
            }
            return null;
        }

        public static object Xor(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) ^ CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) ^ CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) ^ CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) ^ CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) ^ CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) ^ CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) ^ CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) ^ CastUtils.CastNumeric<ulong>(y);
            }
            return null;
        }

        public static object LShift(Type type, object x, int y)
        {
            if (x == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) << y;
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) << y;
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) << y;
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) << y;
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) << y;
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) << y;
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) << y;
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) << y;
            }
            return null;
        }

        public static object RShift(Type type, object x, int y)
        {
            if (x == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) >> y;
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) >> y;
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) >> y;
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) >> y;
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) >> y;
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) >> y;
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) >> y;
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) >> y;
            }
            return null;
        }
    }
}

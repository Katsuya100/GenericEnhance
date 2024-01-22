using System;

namespace Katuusagi.GenericEnhance.Editor.Utils
{
    public static class ConditionalLogicalUtils
    {
        public static bool Not(bool x)
        {
            return !x;
        }

        public static bool And(bool x, bool y)
        {
            return x && y;
        }

        public static bool Or(bool x, bool y)
        {
            return x || y;
        }

        public static object Equal(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(bool))
            {
                return (bool)x == (bool)y;
            }
            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) == CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) == CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) == CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) == CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) == CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) == CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) == CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) == CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) == CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) == CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object NotEqual(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(bool))
            {
                return (bool)x != (bool)y;
            }
            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) != CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) != CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) != CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) != CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) != CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) != CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) != CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) != CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) != CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) != CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Greater(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) > CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) > CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) > CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) > CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) > CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) > CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) > CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) > CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) > CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) > CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object GreaterOrEqual(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) >= CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) >= CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) >= CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) >= CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) >= CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) >= CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) >= CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) >= CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) >= CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) >= CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Less(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) < CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) < CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) < CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) < CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) < CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) < CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) < CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) < CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) < CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) < CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object LessOrEqual(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) <= CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) <= CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) <= CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) <= CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) <= CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) <= CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) <= CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) <= CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) <= CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) <= CastUtils.CastNumeric<double>(y);
            }
            return null;
        }
    }
}

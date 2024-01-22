using System;

namespace Katuusagi.GenericEnhance.Editor.Utils
{
    public static class ArithmeticUtils
    {
        public static object Add(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) + CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) + CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) + CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) + CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) + CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) + CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) + CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) + CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) + CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) + CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Sub(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) - CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) - CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) - CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) - CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) - CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) - CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) - CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) - CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) - CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) - CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Mul(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) * CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) * CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) * CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) * CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) * CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) * CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) * CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) * CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) * CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) * CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Div(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) / CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) / CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) / CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) / CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) / CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) / CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) / CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) / CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) / CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) / CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Mod(Type type, object x, object y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return CastUtils.CastNumeric<sbyte>(x) % CastUtils.CastNumeric<sbyte>(y);
            }
            if (type == typeof(byte))
            {
                return CastUtils.CastNumeric<byte>(x) % CastUtils.CastNumeric<byte>(y);
            }
            if (type == typeof(short))
            {
                return CastUtils.CastNumeric<short>(x) % CastUtils.CastNumeric<short>(y);
            }
            if (type == typeof(ushort))
            {
                return CastUtils.CastNumeric<ushort>(x) % CastUtils.CastNumeric<ushort>(y);
            }
            if (type == typeof(int))
            {
                return CastUtils.CastNumeric<int>(x) % CastUtils.CastNumeric<int>(y);
            }
            if (type == typeof(uint))
            {
                return CastUtils.CastNumeric<uint>(x) % CastUtils.CastNumeric<uint>(y);
            }
            if (type == typeof(long))
            {
                return CastUtils.CastNumeric<long>(x) % CastUtils.CastNumeric<long>(y);
            }
            if (type == typeof(ulong))
            {
                return CastUtils.CastNumeric<ulong>(x) % CastUtils.CastNumeric<ulong>(y);
            }
            if (type == typeof(float))
            {
                return CastUtils.CastNumeric<float>(x) % CastUtils.CastNumeric<float>(y);
            }
            if (type == typeof(double))
            {
                return CastUtils.CastNumeric<double>(x) % CastUtils.CastNumeric<double>(y);
            }
            return null;
        }

        public static object Minus(Type type, object x)
        {
            if (x == null)
            {
                return null;
            }

            if (type == typeof(sbyte))
            {
                return -CastUtils.CastNumeric<sbyte>(x);
            }
            if (type == typeof(short))
            {
                return -CastUtils.CastNumeric<short>(x);
            }
            if (type == typeof(int))
            {
                return -CastUtils.CastNumeric<int>(x);
            }
            if (type == typeof(long))
            {
                return -CastUtils.CastNumeric<long>(x);
            }
            if (type == typeof(float))
            {
                return -CastUtils.CastNumeric<float>(x);
            }
            if (type == typeof(double))
            {
                return -CastUtils.CastNumeric<double>(x);
            }
            return null;
        }
    }
}

using Katuusagi.GenericEnhance.Utils;

namespace Katuusagi.GenericEnhance
{
    public struct Mod<T, TX, TY> :
        ITypeFormula<sbyte>,
        ITypeFormula<byte>,
        ITypeFormula<short>,
        ITypeFormula<ushort>,
        ITypeFormula<int>,
        ITypeFormula<uint>,
        ITypeFormula<long>,
        ITypeFormula<ulong>,
        ITypeFormula<float>,
        ITypeFormula<double>
        where TX : struct, ITypeFormula<T>
        where TY : struct, ITypeFormula<T>
    {
        public static readonly T ResultValue = ArithmeticUtils.Mod(TypeFormula.GetValue<TX, T>(), TypeFormula.GetValue<TY, T>());

        public static readonly sbyte Int8ResultValue = CastUtils.Cast<T, sbyte>(ResultValue);
        public static readonly byte UInt8ResultValue = CastUtils.Cast<T, byte>(ResultValue);
        public static readonly short Int16ResultValue = CastUtils.Cast<T, short>(ResultValue);
        public static readonly ushort UInt16ResultValue = CastUtils.Cast<T, ushort>(ResultValue);
        public static readonly int Int32ResultValue = CastUtils.Cast<T, int>(ResultValue);
        public static readonly uint UInt32ResultValue = CastUtils.Cast<T, uint>(ResultValue);
        public static readonly long Int64ResultValue = CastUtils.Cast<T, long>(ResultValue);
        public static readonly ulong UInt64ResultValue = CastUtils.Cast<T, ulong>(ResultValue);
        public static readonly float SingleResultValue = CastUtils.Cast<T, float>(ResultValue);
        public static readonly double DoubleResultValue = CastUtils.Cast<T, double>(ResultValue);

        sbyte ITypeFormula<sbyte>.Result => Int8ResultValue;
        byte ITypeFormula<byte>.Result => UInt8ResultValue;
        short ITypeFormula<short>.Result => Int16ResultValue;
        ushort ITypeFormula<ushort>.Result => UInt16ResultValue;
        int ITypeFormula<int>.Result => Int32ResultValue;
        uint ITypeFormula<uint>.Result => UInt32ResultValue;
        long ITypeFormula<long>.Result => Int64ResultValue;
        ulong ITypeFormula<ulong>.Result => UInt64ResultValue;
        float ITypeFormula<float>.Result => SingleResultValue;
        double ITypeFormula<double>.Result => DoubleResultValue;
    }
}

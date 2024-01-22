using Katuusagi.GenericEnhance.Utils;

namespace Katuusagi.GenericEnhance
{
    public struct Minus<T, TX> :
        ITypeFormula<sbyte>,
        ITypeFormula<short>,
        ITypeFormula<int>,
        ITypeFormula<long>,
        ITypeFormula<float>,
        ITypeFormula<double>
        where TX : struct, ITypeFormula<T>
    {
        public static readonly T ResultValue = ArithmeticUtils.Minus(TypeFormula.GetValue<TX, T>());

        public static readonly sbyte Int8ResultValue = CastUtils.Cast<T, sbyte>(ResultValue);
        public static readonly short Int16ResultValue = CastUtils.Cast<T, short>(ResultValue);
        public static readonly int Int32ResultValue = CastUtils.Cast<T, int>(ResultValue);
        public static readonly long Int64ResultValue = CastUtils.Cast<T, long>(ResultValue);
        public static readonly float SingleResultValue = CastUtils.Cast<T, float>(ResultValue);
        public static readonly double DoubleResultValue = CastUtils.Cast<T, double>(ResultValue);

        sbyte ITypeFormula<sbyte>.Result => Int8ResultValue;
        short ITypeFormula<short>.Result => Int16ResultValue;
        int ITypeFormula<int>.Result => Int32ResultValue;
        long ITypeFormula<long>.Result => Int64ResultValue;
        float ITypeFormula<float>.Result => SingleResultValue;
        double ITypeFormula<double>.Result => DoubleResultValue;
    }
}

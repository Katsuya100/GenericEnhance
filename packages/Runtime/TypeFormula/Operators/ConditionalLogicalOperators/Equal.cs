using Katuusagi.GenericEnhance.Utils;

namespace Katuusagi.GenericEnhance
{
    public struct Equal<T, TX, TY> :
        ITypeFormula<bool>
        where TX : struct, ITypeFormula<T>
        where TY : struct, ITypeFormula<T>
    {
        public static readonly bool ResultValue = ConditionalLogicalUtils.Equal(TypeFormula.GetValue<TX, T>(), TypeFormula.GetValue<TY, T>());
        bool ITypeFormula<bool>.Result => ResultValue;
    }
}

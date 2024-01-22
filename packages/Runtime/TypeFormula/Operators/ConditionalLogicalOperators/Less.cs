using Katuusagi.GenericEnhance.Utils;

namespace Katuusagi.GenericEnhance
{
    public struct Less<T, TX, TY> :
        ITypeFormula<bool>
        where TX : struct, ITypeFormula<T>
        where TY : struct, ITypeFormula<T>
    {
        public static readonly bool ResultValue = ConditionalLogicalUtils.Less(TypeFormula.GetValue<TX, T>(), TypeFormula.GetValue<TY, T>());
        bool ITypeFormula<bool>.Result => ResultValue;
    }
}

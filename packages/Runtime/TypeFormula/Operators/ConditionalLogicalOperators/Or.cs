using Katuusagi.GenericEnhance.Utils;

namespace Katuusagi.GenericEnhance
{
    public struct Or<TX, TY> :
        ITypeFormula<bool>
        where TX : struct, ITypeFormula<bool>
        where TY : struct, ITypeFormula<bool>
    {
        public static readonly bool ResultValue = ConditionalLogicalUtils.Or(TypeFormula.GetValue<TX, bool>(), TypeFormula.GetValue<TY, bool>());
        bool ITypeFormula<bool>.Result => ResultValue;
    }
}

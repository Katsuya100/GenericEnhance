using Katuusagi.GenericEnhance.Utils;

namespace Katuusagi.GenericEnhance
{
    public struct Not<TX> :
        ITypeFormula<bool>
        where TX : struct, ITypeFormula<bool>
    {
        public static readonly bool ResultValue = ConditionalLogicalUtils.Not(TypeFormula.GetValue<TX, bool>());
        bool ITypeFormula<bool>.Result => ResultValue;
    }
}

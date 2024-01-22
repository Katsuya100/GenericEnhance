using Katuusagi.GenericEnhance;

public struct _true : ITypeFormula<bool>
{
    public const bool ResultValue = true;
    bool ITypeFormula<bool>.Result => ResultValue;
}

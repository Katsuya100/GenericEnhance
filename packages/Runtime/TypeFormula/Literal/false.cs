using Katuusagi.GenericEnhance;

public struct _false : ITypeFormula<bool>
{
    public const bool ResultValue = false;
    bool ITypeFormula<bool>.Result => ResultValue;
}

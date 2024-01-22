using Katuusagi.MemoizationForUnity;
using System.Runtime.CompilerServices;

namespace Katuusagi.GenericEnhance
{
    public static partial class TypeFormula
    {
        [Memoization(Modifier = "public static")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetValueRaw<TFormula, T>()
            where TFormula : struct, ITypeFormula<T>
        {
            return default(TFormula).Result;
        }
    }
}

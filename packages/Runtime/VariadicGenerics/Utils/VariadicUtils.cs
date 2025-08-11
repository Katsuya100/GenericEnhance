using System.Runtime.CompilerServices;

namespace Katuusagi.GenericEnhance
{
    public static class VariadicUtils
    {
#if !DISABLE_GENERATE_IL
        public static readonly int VariadicParameterCount = 1;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ContinueTarget()
        {
        }

#if !DISABLE_GENERATE_IL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Break()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Continue()
        {
        }
#endif
    }
}

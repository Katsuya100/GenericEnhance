using Katuusagi.MemoizationForUnity;
using Unity.Collections.LowLevel.Unsafe;

namespace Katuusagi.GenericEnhance.Utils
{
    public static partial class CastUtils
    {
        public static ref T SafeAs<U, T>(ref U value, ref T defaultValue)
        {
            if (!IsEqual<U, T>())
            {
                return ref defaultValue;
            }

            return ref UnsafeUtility.As<U, T>(ref value);
        }

        public static bool TryAs<U, T>(ref U value, out T result)
        {
            if (!IsEqual<U, T>())
            {
                result = default;
                return false;
            }

            result = UnsafeUtility.As<U, T>(ref value);
            return true;
        }

        [Memoization(Modifier = "public static")]
        private static bool IsEqualRaw<TL, TR>()
        {
            return typeof(TL) == typeof(TR);
        }
    }
}

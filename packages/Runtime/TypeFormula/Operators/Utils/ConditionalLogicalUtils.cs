using System;
using System.Runtime.CompilerServices;
namespace Katuusagi.GenericEnhance.Utils
{
    public static partial class ConditionalLogicalUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(EqualInternal))]
        [SpecializedMethod(nameof(Equal), typeof(bool))]
        [SpecializedMethod(nameof(Equal), typeof(sbyte))]
        [SpecializedMethod(nameof(Equal), typeof(byte))]
        [SpecializedMethod(nameof(Equal), typeof(short))]
        [SpecializedMethod(nameof(Equal), typeof(ushort))]
        [SpecializedMethod(nameof(Equal), typeof(int))]
        [SpecializedMethod(nameof(Equal), typeof(uint))]
        [SpecializedMethod(nameof(Equal), typeof(long))]
        [SpecializedMethod(nameof(Equal), typeof(ulong))]
        [SpecializedMethod(nameof(Equal), typeof(float))]
        [SpecializedMethod(nameof(Equal), typeof(double))]
        public static partial bool Equal<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool EqualInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Equal<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(bool x, bool y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(sbyte x, sbyte y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(byte x, byte y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(short x, short y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(ushort x, ushort y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(int x, int y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(uint x, uint y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(long x, long y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(ulong x, ulong y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(float x, float y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(double x, double y)
        {
            return x == y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(NotEqualInternal))]
        [SpecializedMethod(nameof(NotEqual), typeof(bool))]
        [SpecializedMethod(nameof(NotEqual), typeof(sbyte))]
        [SpecializedMethod(nameof(NotEqual), typeof(byte))]
        [SpecializedMethod(nameof(NotEqual), typeof(short))]
        [SpecializedMethod(nameof(NotEqual), typeof(ushort))]
        [SpecializedMethod(nameof(NotEqual), typeof(int))]
        [SpecializedMethod(nameof(NotEqual), typeof(uint))]
        [SpecializedMethod(nameof(NotEqual), typeof(long))]
        [SpecializedMethod(nameof(NotEqual), typeof(ulong))]
        [SpecializedMethod(nameof(NotEqual), typeof(float))]
        [SpecializedMethod(nameof(NotEqual), typeof(double))]
        public static partial bool NotEqual<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool NotEqualInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"NotEqual<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(bool x, bool y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(sbyte x, sbyte y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(byte x, byte y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(short x, short y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(ushort x, ushort y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(int x, int y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(uint x, uint y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(long x, long y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(ulong x, ulong y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(float x, float y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(double x, double y)
        {
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(GreaterInternal))]
        [SpecializedMethod(nameof(Greater), typeof(sbyte))]
        [SpecializedMethod(nameof(Greater), typeof(byte))]
        [SpecializedMethod(nameof(Greater), typeof(short))]
        [SpecializedMethod(nameof(Greater), typeof(ushort))]
        [SpecializedMethod(nameof(Greater), typeof(int))]
        [SpecializedMethod(nameof(Greater), typeof(uint))]
        [SpecializedMethod(nameof(Greater), typeof(long))]
        [SpecializedMethod(nameof(Greater), typeof(ulong))]
        [SpecializedMethod(nameof(Greater), typeof(float))]
        [SpecializedMethod(nameof(Greater), typeof(double))]
        public static partial bool Greater<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GreaterInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Greater<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(sbyte x, sbyte y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(byte x, byte y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(short x, short y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(ushort x, ushort y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(int x, int y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(uint x, uint y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(long x, long y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(ulong x, ulong y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(float x, float y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater(double x, double y)
        {
            return x > y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(LessInternal))]
        [SpecializedMethod(nameof(Less), typeof(sbyte))]
        [SpecializedMethod(nameof(Less), typeof(byte))]
        [SpecializedMethod(nameof(Less), typeof(short))]
        [SpecializedMethod(nameof(Less), typeof(ushort))]
        [SpecializedMethod(nameof(Less), typeof(int))]
        [SpecializedMethod(nameof(Less), typeof(uint))]
        [SpecializedMethod(nameof(Less), typeof(long))]
        [SpecializedMethod(nameof(Less), typeof(ulong))]
        [SpecializedMethod(nameof(Less), typeof(float))]
        [SpecializedMethod(nameof(Less), typeof(double))]
        public static partial bool Less<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LessInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Less<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(sbyte x, sbyte y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(byte x, byte y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(short x, short y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(ushort x, ushort y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(int x, int y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(uint x, uint y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(long x, long y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(ulong x, ulong y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(float x, float y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less(double x, double y)
        {
            return x < y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(GreaterOrEqualInternal))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(sbyte))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(byte))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(short))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(ushort))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(int))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(uint))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(long))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(ulong))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(float))]
        [SpecializedMethod(nameof(GreaterOrEqual), typeof(double))]
        public static partial bool GreaterOrEqual<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GreaterOrEqualInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"GreaterOrEqual<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(sbyte x, sbyte y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(byte x, byte y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(short x, short y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(ushort x, ushort y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(int x, int y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(uint x, uint y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(long x, long y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(ulong x, ulong y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(float x, float y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEqual(double x, double y)
        {
            return x >= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(LessOrEqualInternal))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(sbyte))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(byte))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(short))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(ushort))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(int))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(uint))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(long))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(ulong))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(float))]
        [SpecializedMethod(nameof(LessOrEqual), typeof(double))]
        public static partial bool LessOrEqual<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LessOrEqualInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"LessOrEqual<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(sbyte x, sbyte y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(byte x, byte y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(short x, short y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(ushort x, ushort y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(int x, int y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(uint x, uint y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(long x, long y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(ulong x, ulong y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(float x, float y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessOrEqual(double x, double y)
        {
            return x <= y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool And(bool x, bool y)
        {
            return x && y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Or(bool x, bool y)
        {
            return x || y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Not(bool x)
        {
            return !x;
        }
    }
}

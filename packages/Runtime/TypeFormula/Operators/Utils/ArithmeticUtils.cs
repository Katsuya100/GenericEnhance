using System;
using System.Runtime.CompilerServices;
namespace Katuusagi.GenericEnhance.Utils
{
    public static partial class ArithmeticUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(AddInternal))]
        [SpecializedMethod(nameof(Add), typeof(sbyte))]
        [SpecializedMethod(nameof(Add), typeof(byte))]
        [SpecializedMethod(nameof(Add), typeof(short))]
        [SpecializedMethod(nameof(Add), typeof(ushort))]
        [SpecializedMethod(nameof(Add), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(uint))]
        [SpecializedMethod(nameof(Add), typeof(long))]
        [SpecializedMethod(nameof(Add), typeof(ulong))]
        [SpecializedMethod(nameof(Add), typeof(float))]
        [SpecializedMethod(nameof(Add), typeof(double))]
        public static partial T Add<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T AddInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Add<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Add(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Add(byte x, byte y)
        {
            return unchecked((byte)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Add(short x, short y)
        {
            return unchecked((short)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Add(ushort x, ushort y)
        {
            return unchecked((ushort)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Add(int x, int y)
        {
            return unchecked((int)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Add(uint x, uint y)
        {
            return unchecked((uint)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Add(long x, long y)
        {
            return unchecked((long)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Add(ulong x, ulong y)
        {
            return unchecked((ulong)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Add(float x, float y)
        {
            return unchecked((float)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Add(double x, double y)
        {
            return unchecked((double)(x + y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(SubInternal))]
        [SpecializedMethod(nameof(Sub), typeof(sbyte))]
        [SpecializedMethod(nameof(Sub), typeof(byte))]
        [SpecializedMethod(nameof(Sub), typeof(short))]
        [SpecializedMethod(nameof(Sub), typeof(ushort))]
        [SpecializedMethod(nameof(Sub), typeof(int))]
        [SpecializedMethod(nameof(Sub), typeof(uint))]
        [SpecializedMethod(nameof(Sub), typeof(long))]
        [SpecializedMethod(nameof(Sub), typeof(ulong))]
        [SpecializedMethod(nameof(Sub), typeof(float))]
        [SpecializedMethod(nameof(Sub), typeof(double))]
        public static partial T Sub<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T SubInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Sub<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Sub(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Sub(byte x, byte y)
        {
            return unchecked((byte)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Sub(short x, short y)
        {
            return unchecked((short)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Sub(ushort x, ushort y)
        {
            return unchecked((ushort)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sub(int x, int y)
        {
            return unchecked((int)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Sub(uint x, uint y)
        {
            return unchecked((uint)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sub(long x, long y)
        {
            return unchecked((long)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Sub(ulong x, ulong y)
        {
            return unchecked((ulong)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sub(float x, float y)
        {
            return unchecked((float)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sub(double x, double y)
        {
            return unchecked((double)(x - y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(MulInternal))]
        [SpecializedMethod(nameof(Mul), typeof(sbyte))]
        [SpecializedMethod(nameof(Mul), typeof(byte))]
        [SpecializedMethod(nameof(Mul), typeof(short))]
        [SpecializedMethod(nameof(Mul), typeof(ushort))]
        [SpecializedMethod(nameof(Mul), typeof(int))]
        [SpecializedMethod(nameof(Mul), typeof(uint))]
        [SpecializedMethod(nameof(Mul), typeof(long))]
        [SpecializedMethod(nameof(Mul), typeof(ulong))]
        [SpecializedMethod(nameof(Mul), typeof(float))]
        [SpecializedMethod(nameof(Mul), typeof(double))]
        public static partial T Mul<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T MulInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Mul<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Mul(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Mul(byte x, byte y)
        {
            return unchecked((byte)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Mul(short x, short y)
        {
            return unchecked((short)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Mul(ushort x, ushort y)
        {
            return unchecked((ushort)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mul(int x, int y)
        {
            return unchecked((int)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mul(uint x, uint y)
        {
            return unchecked((uint)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Mul(long x, long y)
        {
            return unchecked((long)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Mul(ulong x, ulong y)
        {
            return unchecked((ulong)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Mul(float x, float y)
        {
            return unchecked((float)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Mul(double x, double y)
        {
            return unchecked((double)(x * y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(DivInternal))]
        [SpecializedMethod(nameof(Div), typeof(sbyte))]
        [SpecializedMethod(nameof(Div), typeof(byte))]
        [SpecializedMethod(nameof(Div), typeof(short))]
        [SpecializedMethod(nameof(Div), typeof(ushort))]
        [SpecializedMethod(nameof(Div), typeof(int))]
        [SpecializedMethod(nameof(Div), typeof(uint))]
        [SpecializedMethod(nameof(Div), typeof(long))]
        [SpecializedMethod(nameof(Div), typeof(ulong))]
        [SpecializedMethod(nameof(Div), typeof(float))]
        [SpecializedMethod(nameof(Div), typeof(double))]
        public static partial T Div<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T DivInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Div<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Div(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Div(byte x, byte y)
        {
            return unchecked((byte)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Div(short x, short y)
        {
            return unchecked((short)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Div(ushort x, ushort y)
        {
            return unchecked((ushort)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Div(int x, int y)
        {
            return unchecked((int)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Div(uint x, uint y)
        {
            return unchecked((uint)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Div(long x, long y)
        {
            return unchecked((long)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Div(ulong x, ulong y)
        {
            return unchecked((ulong)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Div(float x, float y)
        {
            return unchecked((float)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Div(double x, double y)
        {
            return unchecked((double)(x / y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(ModInternal))]
        [SpecializedMethod(nameof(Mod), typeof(sbyte))]
        [SpecializedMethod(nameof(Mod), typeof(byte))]
        [SpecializedMethod(nameof(Mod), typeof(short))]
        [SpecializedMethod(nameof(Mod), typeof(ushort))]
        [SpecializedMethod(nameof(Mod), typeof(int))]
        [SpecializedMethod(nameof(Mod), typeof(uint))]
        [SpecializedMethod(nameof(Mod), typeof(long))]
        [SpecializedMethod(nameof(Mod), typeof(ulong))]
        [SpecializedMethod(nameof(Mod), typeof(float))]
        [SpecializedMethod(nameof(Mod), typeof(double))]
        public static partial T Mod<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T ModInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Mod<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Mod(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Mod(byte x, byte y)
        {
            return unchecked((byte)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Mod(short x, short y)
        {
            return unchecked((short)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Mod(ushort x, ushort y)
        {
            return unchecked((ushort)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(int x, int y)
        {
            return unchecked((int)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mod(uint x, uint y)
        {
            return unchecked((uint)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Mod(long x, long y)
        {
            return unchecked((long)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Mod(ulong x, ulong y)
        {
            return unchecked((ulong)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Mod(float x, float y)
        {
            return unchecked((float)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Mod(double x, double y)
        {
            return unchecked((double)(x % y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(MinusInternal))]
        [SpecializedMethod(nameof(Minus), typeof(sbyte))]
        [SpecializedMethod(nameof(Minus), typeof(short))]
        [SpecializedMethod(nameof(Minus), typeof(int))]
        [SpecializedMethod(nameof(Minus), typeof(long))]
        [SpecializedMethod(nameof(Minus), typeof(float))]
        [SpecializedMethod(nameof(Minus), typeof(double))]
        public static partial T Minus<T>(T x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T MinusInternal<T>(T x)
        {
            throw new NotImplementedException($"\"Minus<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Minus(sbyte x)
        {
            return unchecked((sbyte)-x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Minus(short x)
        {
            return unchecked((short)-x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Minus(int x)
        {
            return unchecked((int)-x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Minus(long x)
        {
            return unchecked((long)-x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Minus(float x)
        {
            return unchecked((float)-x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Minus(double x)
        {
            return unchecked((double)-x);
        }
    }
}

using System;
using System.Runtime.CompilerServices;
namespace Katuusagi.GenericEnhance.Utils
{
    public static partial class BitLogicalUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(XorInternal))]
        [SpecializedMethod(nameof(Xor), typeof(sbyte))]
        [SpecializedMethod(nameof(Xor), typeof(byte))]
        [SpecializedMethod(nameof(Xor), typeof(short))]
        [SpecializedMethod(nameof(Xor), typeof(ushort))]
        [SpecializedMethod(nameof(Xor), typeof(int))]
        [SpecializedMethod(nameof(Xor), typeof(uint))]
        [SpecializedMethod(nameof(Xor), typeof(long))]
        [SpecializedMethod(nameof(Xor), typeof(ulong))]
        public static partial T Xor<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T XorInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Xor<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Xor(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Xor(byte x, byte y)
        {
            return unchecked((byte)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Xor(short x, short y)
        {
            return unchecked((short)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Xor(ushort x, ushort y)
        {
            return unchecked((ushort)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Xor(int x, int y)
        {
            return unchecked((int)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Xor(uint x, uint y)
        {
            return unchecked((uint)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Xor(long x, long y)
        {
            return unchecked((long)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Xor(ulong x, ulong y)
        {
            return unchecked((ulong)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(OrInternal))]
        [SpecializedMethod(nameof(Or), typeof(sbyte))]
        [SpecializedMethod(nameof(Or), typeof(byte))]
        [SpecializedMethod(nameof(Or), typeof(short))]
        [SpecializedMethod(nameof(Or), typeof(ushort))]
        [SpecializedMethod(nameof(Or), typeof(int))]
        [SpecializedMethod(nameof(Or), typeof(uint))]
        [SpecializedMethod(nameof(Or), typeof(long))]
        [SpecializedMethod(nameof(Or), typeof(ulong))]
        public static partial T Or<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T OrInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"Or<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Or(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Or(byte x, byte y)
        {
            return unchecked((byte)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Or(short x, short y)
        {
            return unchecked((short)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Or(ushort x, ushort y)
        {
            return unchecked((ushort)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Or(int x, int y)
        {
            return unchecked((int)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Or(uint x, uint y)
        {
            return unchecked((uint)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Or(long x, long y)
        {
            return unchecked((long)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Or(ulong x, ulong y)
        {
            return unchecked((ulong)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(AndInternal))]
        [SpecializedMethod(nameof(And), typeof(sbyte))]
        [SpecializedMethod(nameof(And), typeof(byte))]
        [SpecializedMethod(nameof(And), typeof(short))]
        [SpecializedMethod(nameof(And), typeof(ushort))]
        [SpecializedMethod(nameof(And), typeof(int))]
        [SpecializedMethod(nameof(And), typeof(uint))]
        [SpecializedMethod(nameof(And), typeof(long))]
        [SpecializedMethod(nameof(And), typeof(ulong))]
        public static partial T And<T>(T x, T y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T AndInternal<T>(T x, T y)
        {
            throw new NotImplementedException($"\"And<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte And(sbyte x, sbyte y)
        {
            return unchecked((sbyte)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte And(byte x, byte y)
        {
            return unchecked((byte)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short And(short x, short y)
        {
            return unchecked((short)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort And(ushort x, ushort y)
        {
            return unchecked((ushort)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int And(int x, int y)
        {
            return unchecked((int)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint And(uint x, uint y)
        {
            return unchecked((uint)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long And(long x, long y)
        {
            return unchecked((long)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong And(ulong x, ulong y)
        {
            return unchecked((ulong)(x ^ y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(NotInternal))]
        [SpecializedMethod(nameof(Not), typeof(sbyte))]
        [SpecializedMethod(nameof(Not), typeof(byte))]
        [SpecializedMethod(nameof(Not), typeof(short))]
        [SpecializedMethod(nameof(Not), typeof(ushort))]
        [SpecializedMethod(nameof(Not), typeof(int))]
        [SpecializedMethod(nameof(Not), typeof(uint))]
        [SpecializedMethod(nameof(Not), typeof(long))]
        [SpecializedMethod(nameof(Not), typeof(ulong))]
        public static partial T Not<T>(T x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T NotInternal<T>(T x)
        {
            throw new NotImplementedException($"\"Not<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Not(sbyte x)
        {
            return unchecked((sbyte)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Not(byte x)
        {
            return unchecked((byte)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Not(short x)
        {
            return unchecked((short)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Not(ushort x)
        {
            return unchecked((ushort)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Not(int x)
        {
            return unchecked((int)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Not(uint x)
        {
            return unchecked((uint)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Not(long x)
        {
            return unchecked((long)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Not(ulong x)
        {
            return unchecked((ulong)(~x));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(LShiftInternal))]
        [SpecializedMethod(nameof(LShift), typeof(sbyte))]
        [SpecializedMethod(nameof(LShift), typeof(byte))]
        [SpecializedMethod(nameof(LShift), typeof(short))]
        [SpecializedMethod(nameof(LShift), typeof(ushort))]
        [SpecializedMethod(nameof(LShift), typeof(int))]
        [SpecializedMethod(nameof(LShift), typeof(uint))]
        [SpecializedMethod(nameof(LShift), typeof(long))]
        [SpecializedMethod(nameof(LShift), typeof(ulong))]
        public static partial T LShift<T>(T x, int y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T LShiftInternal<T>(T x, int y)
        {
            throw new NotImplementedException($"\"LShift<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte LShift(sbyte x, int y)
        {
            return unchecked((sbyte)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LShift(byte x, int y)
        {
            return unchecked((byte)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short LShift(short x, int y)
        {
            return unchecked((short)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LShift(ushort x, int y)
        {
            return unchecked((ushort)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LShift(int x, int y)
        {
            return unchecked((int)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LShift(uint x, int y)
        {
            return unchecked((uint)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LShift(long x, int y)
        {
            return unchecked((long)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LShift(ulong x, int y)
        {
            return unchecked((ulong)(x << y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(RShiftInternal))]
        [SpecializedMethod(nameof(RShift), typeof(sbyte))]
        [SpecializedMethod(nameof(RShift), typeof(byte))]
        [SpecializedMethod(nameof(RShift), typeof(short))]
        [SpecializedMethod(nameof(RShift), typeof(ushort))]
        [SpecializedMethod(nameof(RShift), typeof(int))]
        [SpecializedMethod(nameof(RShift), typeof(uint))]
        [SpecializedMethod(nameof(RShift), typeof(long))]
        [SpecializedMethod(nameof(RShift), typeof(ulong))]
        public static partial T RShift<T>(T x, int y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T RShiftInternal<T>(T x, int y)
        {
            throw new NotImplementedException($"\"RShift<{typeof(T)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte RShift(sbyte x, int y)
        {
            return unchecked((sbyte)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte RShift(byte x, int y)
        {
            return unchecked((byte)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short RShift(short x, int y)
        {
            return unchecked((short)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RShift(ushort x, int y)
        {
            return unchecked((ushort)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RShift(int x, int y)
        {
            return unchecked((int)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RShift(uint x, int y)
        {
            return unchecked((uint)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RShift(long x, int y)
        {
            return unchecked((long)(x >> y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RShift(ulong x, int y)
        {
            return unchecked((ulong)(x >> y));
        }
    }
}

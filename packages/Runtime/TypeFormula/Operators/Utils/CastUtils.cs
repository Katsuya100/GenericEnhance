using System;
using System.Runtime.CompilerServices;
namespace Katuusagi.GenericEnhance.Utils
{
    public static partial class CastUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SpecializationMethod(nameof(CastInternal))]
        [SpecializedMethod(nameof(sbyte2sbyte), typeof(sbyte), typeof(sbyte))]
        [SpecializedMethod(nameof(sbyte2byte), typeof(sbyte), typeof(byte))]
        [SpecializedMethod(nameof(sbyte2short), typeof(sbyte), typeof(short))]
        [SpecializedMethod(nameof(sbyte2ushort), typeof(sbyte), typeof(ushort))]
        [SpecializedMethod(nameof(sbyte2int), typeof(sbyte), typeof(int))]
        [SpecializedMethod(nameof(sbyte2uint), typeof(sbyte), typeof(uint))]
        [SpecializedMethod(nameof(sbyte2long), typeof(sbyte), typeof(long))]
        [SpecializedMethod(nameof(sbyte2ulong), typeof(sbyte), typeof(ulong))]
        [SpecializedMethod(nameof(sbyte2float), typeof(sbyte), typeof(float))]
        [SpecializedMethod(nameof(sbyte2double), typeof(sbyte), typeof(double))]
        [SpecializedMethod(nameof(byte2sbyte), typeof(byte), typeof(sbyte))]
        [SpecializedMethod(nameof(byte2byte), typeof(byte), typeof(byte))]
        [SpecializedMethod(nameof(byte2short), typeof(byte), typeof(short))]
        [SpecializedMethod(nameof(byte2ushort), typeof(byte), typeof(ushort))]
        [SpecializedMethod(nameof(byte2int), typeof(byte), typeof(int))]
        [SpecializedMethod(nameof(byte2uint), typeof(byte), typeof(uint))]
        [SpecializedMethod(nameof(byte2long), typeof(byte), typeof(long))]
        [SpecializedMethod(nameof(byte2ulong), typeof(byte), typeof(ulong))]
        [SpecializedMethod(nameof(byte2float), typeof(byte), typeof(float))]
        [SpecializedMethod(nameof(byte2double), typeof(byte), typeof(double))]
        [SpecializedMethod(nameof(short2sbyte), typeof(short), typeof(sbyte))]
        [SpecializedMethod(nameof(short2byte), typeof(short), typeof(byte))]
        [SpecializedMethod(nameof(short2short), typeof(short), typeof(short))]
        [SpecializedMethod(nameof(short2ushort), typeof(short), typeof(ushort))]
        [SpecializedMethod(nameof(short2int), typeof(short), typeof(int))]
        [SpecializedMethod(nameof(short2uint), typeof(short), typeof(uint))]
        [SpecializedMethod(nameof(short2long), typeof(short), typeof(long))]
        [SpecializedMethod(nameof(short2ulong), typeof(short), typeof(ulong))]
        [SpecializedMethod(nameof(short2float), typeof(short), typeof(float))]
        [SpecializedMethod(nameof(short2double), typeof(short), typeof(double))]
        [SpecializedMethod(nameof(ushort2sbyte), typeof(ushort), typeof(sbyte))]
        [SpecializedMethod(nameof(ushort2byte), typeof(ushort), typeof(byte))]
        [SpecializedMethod(nameof(ushort2short), typeof(ushort), typeof(short))]
        [SpecializedMethod(nameof(ushort2ushort), typeof(ushort), typeof(ushort))]
        [SpecializedMethod(nameof(ushort2int), typeof(ushort), typeof(int))]
        [SpecializedMethod(nameof(ushort2uint), typeof(ushort), typeof(uint))]
        [SpecializedMethod(nameof(ushort2long), typeof(ushort), typeof(long))]
        [SpecializedMethod(nameof(ushort2ulong), typeof(ushort), typeof(ulong))]
        [SpecializedMethod(nameof(ushort2float), typeof(ushort), typeof(float))]
        [SpecializedMethod(nameof(ushort2double), typeof(ushort), typeof(double))]
        [SpecializedMethod(nameof(int2sbyte), typeof(int), typeof(sbyte))]
        [SpecializedMethod(nameof(int2byte), typeof(int), typeof(byte))]
        [SpecializedMethod(nameof(int2short), typeof(int), typeof(short))]
        [SpecializedMethod(nameof(int2ushort), typeof(int), typeof(ushort))]
        [SpecializedMethod(nameof(int2int), typeof(int), typeof(int))]
        [SpecializedMethod(nameof(int2uint), typeof(int), typeof(uint))]
        [SpecializedMethod(nameof(int2long), typeof(int), typeof(long))]
        [SpecializedMethod(nameof(int2ulong), typeof(int), typeof(ulong))]
        [SpecializedMethod(nameof(int2float), typeof(int), typeof(float))]
        [SpecializedMethod(nameof(int2double), typeof(int), typeof(double))]
        [SpecializedMethod(nameof(uint2sbyte), typeof(uint), typeof(sbyte))]
        [SpecializedMethod(nameof(uint2byte), typeof(uint), typeof(byte))]
        [SpecializedMethod(nameof(uint2short), typeof(uint), typeof(short))]
        [SpecializedMethod(nameof(uint2ushort), typeof(uint), typeof(ushort))]
        [SpecializedMethod(nameof(uint2int), typeof(uint), typeof(int))]
        [SpecializedMethod(nameof(uint2uint), typeof(uint), typeof(uint))]
        [SpecializedMethod(nameof(uint2long), typeof(uint), typeof(long))]
        [SpecializedMethod(nameof(uint2ulong), typeof(uint), typeof(ulong))]
        [SpecializedMethod(nameof(uint2float), typeof(uint), typeof(float))]
        [SpecializedMethod(nameof(uint2double), typeof(uint), typeof(double))]
        [SpecializedMethod(nameof(long2sbyte), typeof(long), typeof(sbyte))]
        [SpecializedMethod(nameof(long2byte), typeof(long), typeof(byte))]
        [SpecializedMethod(nameof(long2short), typeof(long), typeof(short))]
        [SpecializedMethod(nameof(long2ushort), typeof(long), typeof(ushort))]
        [SpecializedMethod(nameof(long2int), typeof(long), typeof(int))]
        [SpecializedMethod(nameof(long2uint), typeof(long), typeof(uint))]
        [SpecializedMethod(nameof(long2long), typeof(long), typeof(long))]
        [SpecializedMethod(nameof(long2ulong), typeof(long), typeof(ulong))]
        [SpecializedMethod(nameof(long2float), typeof(long), typeof(float))]
        [SpecializedMethod(nameof(long2double), typeof(long), typeof(double))]
        [SpecializedMethod(nameof(ulong2sbyte), typeof(ulong), typeof(sbyte))]
        [SpecializedMethod(nameof(ulong2byte), typeof(ulong), typeof(byte))]
        [SpecializedMethod(nameof(ulong2short), typeof(ulong), typeof(short))]
        [SpecializedMethod(nameof(ulong2ushort), typeof(ulong), typeof(ushort))]
        [SpecializedMethod(nameof(ulong2int), typeof(ulong), typeof(int))]
        [SpecializedMethod(nameof(ulong2uint), typeof(ulong), typeof(uint))]
        [SpecializedMethod(nameof(ulong2long), typeof(ulong), typeof(long))]
        [SpecializedMethod(nameof(ulong2ulong), typeof(ulong), typeof(ulong))]
        [SpecializedMethod(nameof(ulong2float), typeof(ulong), typeof(float))]
        [SpecializedMethod(nameof(ulong2double), typeof(ulong), typeof(double))]
        [SpecializedMethod(nameof(float2sbyte), typeof(float), typeof(sbyte))]
        [SpecializedMethod(nameof(float2byte), typeof(float), typeof(byte))]
        [SpecializedMethod(nameof(float2short), typeof(float), typeof(short))]
        [SpecializedMethod(nameof(float2ushort), typeof(float), typeof(ushort))]
        [SpecializedMethod(nameof(float2int), typeof(float), typeof(int))]
        [SpecializedMethod(nameof(float2uint), typeof(float), typeof(uint))]
        [SpecializedMethod(nameof(float2long), typeof(float), typeof(long))]
        [SpecializedMethod(nameof(float2ulong), typeof(float), typeof(ulong))]
        [SpecializedMethod(nameof(float2float), typeof(float), typeof(float))]
        [SpecializedMethod(nameof(float2double), typeof(float), typeof(double))]
        [SpecializedMethod(nameof(double2sbyte), typeof(double), typeof(sbyte))]
        [SpecializedMethod(nameof(double2byte), typeof(double), typeof(byte))]
        [SpecializedMethod(nameof(double2short), typeof(double), typeof(short))]
        [SpecializedMethod(nameof(double2ushort), typeof(double), typeof(ushort))]
        [SpecializedMethod(nameof(double2int), typeof(double), typeof(int))]
        [SpecializedMethod(nameof(double2uint), typeof(double), typeof(uint))]
        [SpecializedMethod(nameof(double2long), typeof(double), typeof(long))]
        [SpecializedMethod(nameof(double2ulong), typeof(double), typeof(ulong))]
        [SpecializedMethod(nameof(double2float), typeof(double), typeof(float))]
        [SpecializedMethod(nameof(double2double), typeof(double), typeof(double))]
        public static partial TDst Cast<TSrc, TDst>(TSrc src);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TDst CastInternal<TSrc, TDst>(TSrc src)
        {
            throw new NotImplementedException($"\"Cast<{typeof(TSrc)}, {typeof(TDst)}>\" is not implemented generic parameter.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool bool2bool(bool src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte sbyte2sbyte(sbyte src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte sbyte2byte(sbyte src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short sbyte2short(sbyte src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort sbyte2ushort(sbyte src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int sbyte2int(sbyte src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint sbyte2uint(sbyte src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long sbyte2long(sbyte src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong sbyte2ulong(sbyte src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float sbyte2float(sbyte src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double sbyte2double(sbyte src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte byte2sbyte(byte src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte byte2byte(byte src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short byte2short(byte src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort byte2ushort(byte src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int byte2int(byte src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint byte2uint(byte src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long byte2long(byte src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong byte2ulong(byte src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float byte2float(byte src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double byte2double(byte src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte short2sbyte(short src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte short2byte(short src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short short2short(short src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort short2ushort(short src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int short2int(short src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint short2uint(short src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long short2long(short src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong short2ulong(short src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float short2float(short src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double short2double(short src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte ushort2sbyte(ushort src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ushort2byte(ushort src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short ushort2short(ushort src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort ushort2ushort(ushort src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ushort2int(ushort src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ushort2uint(ushort src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ushort2long(ushort src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ushort2ulong(ushort src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ushort2float(ushort src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ushort2double(ushort src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte int2sbyte(int src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte int2byte(int src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short int2short(int src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort int2ushort(int src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int int2int(int src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint int2uint(int src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long int2long(int src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong int2ulong(int src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float int2float(int src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double int2double(int src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte uint2sbyte(uint src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte uint2byte(uint src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short uint2short(uint src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort uint2ushort(uint src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int uint2int(uint src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint uint2uint(uint src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long uint2long(uint src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong uint2ulong(uint src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float uint2float(uint src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double uint2double(uint src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte long2sbyte(long src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte long2byte(long src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short long2short(long src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort long2ushort(long src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int long2int(long src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint long2uint(long src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long long2long(long src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong long2ulong(long src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float long2float(long src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double long2double(long src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte ulong2sbyte(ulong src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ulong2byte(ulong src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short ulong2short(ulong src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort ulong2ushort(ulong src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ulong2int(ulong src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ulong2uint(ulong src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ulong2long(ulong src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ulong2ulong(ulong src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ulong2float(ulong src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ulong2double(ulong src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte float2sbyte(float src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte float2byte(float src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short float2short(float src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort float2ushort(float src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int float2int(float src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint float2uint(float src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long float2long(float src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong float2ulong(float src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float float2float(float src)
        {
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double float2double(float src)
        {
            return unchecked((double)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte double2sbyte(double src)
        {
            return unchecked((sbyte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte double2byte(double src)
        {
            return unchecked((byte)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short double2short(double src)
        {
            return unchecked((short)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort double2ushort(double src)
        {
            return unchecked((ushort)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int double2int(double src)
        {
            return unchecked((int)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint double2uint(double src)
        {
            return unchecked((uint)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long double2long(double src)
        {
            return unchecked((long)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong double2ulong(double src)
        {
            return unchecked((ulong)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float double2float(double src)
        {
            return unchecked((float)src);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double double2double(double src)
        {
            return src;
        }
    }
}

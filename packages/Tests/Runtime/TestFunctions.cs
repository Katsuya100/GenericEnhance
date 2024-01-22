using System;
using UnityEngine;

namespace Katuusagi.GenericEnhance.Tests
{
    public partial class TestFunctions
    {
        [TypeDef(typeof(int))]
        public struct DefInt
        {
        }

        [TypeDef(nameof(T))]
        public struct AnyStructDef<T>
            where T : struct
        {
            public float magnitude { get; set; }
        }

        [TypeDef(nameof(T))]
        public class AnyClassDef<T>
            where T : class
        {
            public Transform transform { get; set; }
        }

        public static DefInt DefValue;

        public void WrappedGetValue_VirtualStrategy<T>(out T value)
        {
            GetValue_VirtualStrategy(out value);
        }

        public void WrappedGetValue_DelegateStrategy<T>(out T value)
        {
            GetValue_DelegateStrategy(out value);
        }

        public void WrappedGetValue_TypeComparison<T>(out T value)
        {
            GetValue_TypeComparison(out value);
        }

        public void WrappedGetValue_TypeIdComparison<T>(out T value)
        {
            GetValue_TypeIdComparison(out value);
        }

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.VirtualStrategy)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial void GetValue_VirtualStrategy<T>(out T value);

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.DelegateStrategy)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial void GetValue_DelegateStrategy<T>(out T value);

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.TypeComparison)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial void GetValue_TypeComparison<T>(out T value);

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.TypeIdComparison)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial void GetValue_TypeIdComparison<T>(out T value);

        public void GetInteger(out int value)
        {
            value = 100;
        }

        public void GetInternal<T>(out T value)
        {
            value = default;
        }

        public T WrappedGetValue_VirtualStrategy<T>()
        {
            return GetValue_VirtualStrategy<T>();
        }

        public T WrappedGetValue_DelegateStrategy<T>()
        {
            return GetValue_DelegateStrategy<T>();
        }

        public T WrappedGetValue_TypeComparison<T>()
        {
            return GetValue_TypeComparison<T>();
        }

        public T WrappedGetValue_TypeIdComparison<T>()
        {
            return GetValue_TypeIdComparison<T>();
        }

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.VirtualStrategy)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial T GetValue_VirtualStrategy<T>();

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.DelegateStrategy)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial T GetValue_DelegateStrategy<T>();

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.TypeComparison)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial T GetValue_TypeComparison<T>();

        [SpecializationMethod(nameof(GetInternal), SpecializeAlgorithm.TypeIdComparison)]
        [SpecializedMethod(nameof(GetInteger), typeof(int))]
        public partial T GetValue_TypeIdComparison<T>();

        public int GetInteger()
        {
            return 100;
        }

        public T GetInternal<T>()
        {
            return default;
        }

        public static TResult WrappedAdd_VirtualStrategy<TL, TR, TResult>(TL l, TR r)
        {
            return Add_VirtualStrategy<TL, TR, TResult>(l, r);
        }

        public static TResult WrappedAdd_DelegateStrategy<TL, TR, TResult>(TL l, TR r)
        {
            return Add_DelegateStrategy<TL, TR, TResult>(l, r);
        }

        public static TResult WrappedAdd_TypeComparison<TL, TR, TResult>(TL l, TR r)
        {
            return Add_TypeComparison<TL, TR, TResult>(l, r);
        }

        public static TResult WrappedAdd_TypeIdComparison<TL, TR, TResult>(TL l, TR r)
        {
            return Add_TypeIdComparison<TL, TR, TResult>(l, r);
        }

        [SpecializationMethod(nameof(AddInternal), SpecializeAlgorithm.VirtualStrategy)]
        [SpecializedMethod(nameof(Add), typeof(byte), typeof(byte), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(short), typeof(short), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(int), typeof(int), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(long), typeof(long), typeof(long))]
        [SpecializedMethod(nameof(Add), typeof(float), typeof(float), typeof(float))]
        [SpecializedMethod(nameof(Add), typeof(double), typeof(double), typeof(double))]
        public static partial TResult Add_VirtualStrategy<TL, TR, TResult>(TL l, TR r);

        [SpecializationMethod(nameof(AddInternal), SpecializeAlgorithm.DelegateStrategy)]
        [SpecializedMethod(nameof(Add), typeof(byte), typeof(byte), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(short), typeof(short), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(int), typeof(int), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(long), typeof(long), typeof(long))]
        [SpecializedMethod(nameof(Add), typeof(float), typeof(float), typeof(float))]
        [SpecializedMethod(nameof(Add), typeof(double), typeof(double), typeof(double))]
        public static partial TResult Add_DelegateStrategy<TL, TR, TResult>(TL l, TR r);

        [SpecializationMethod(nameof(AddInternal), SpecializeAlgorithm.TypeComparison)]
        [SpecializedMethod(nameof(Add), typeof(byte), typeof(byte), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(short), typeof(short), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(int), typeof(int), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(long), typeof(long), typeof(long))]
        [SpecializedMethod(nameof(Add), typeof(float), typeof(float), typeof(float))]
        [SpecializedMethod(nameof(Add), typeof(double), typeof(double), typeof(double))]
        public static partial TResult Add_TypeComparison<TL, TR, TResult>(TL l, TR r);

        [SpecializationMethod(nameof(AddInternal), SpecializeAlgorithm.TypeIdComparison)]
        [SpecializedMethod(nameof(Add), typeof(byte), typeof(byte), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(short), typeof(short), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(int), typeof(int), typeof(int))]
        [SpecializedMethod(nameof(Add), typeof(long), typeof(long), typeof(long))]
        [SpecializedMethod(nameof(Add), typeof(float), typeof(float), typeof(float))]
        [SpecializedMethod(nameof(Add), typeof(double), typeof(double), typeof(double))]
        public static partial TResult Add_TypeIdComparison<TL, TR, TResult>(TL l, TR r);

        public static TResult AddInternal<TL, TR, TResult>(TL l, TR r)
        {
            return default;
        }

        public static int Add(byte l, byte r)
        {
            return l + r;
        }

        public static int Add(short l, short r)
        {
            return l + r;
        }

        public static int Add(int l, int r)
        {
            return l + r;
        }

        public static long Add(long l, long r)
        {
            return l + r;
        }

        public static float Add(float l, float r)
        {
            return l + r;
        }

        public static double Add(double l, double r)
        {
            return l + r;
        }

        public static string WrappedGetBoolean_VirtualStrategy<T>()
            where T : struct, ITypeFormula<bool>
        {
            return GetBoolean_VirtualStrategy<T>();
        }

        public static string WrappedGetBoolean_DelegateStrategy<T>()
            where T : struct, ITypeFormula<bool>
        {
            return GetBoolean_DelegateStrategy<T>();
        }

        public static string WrappedGetBoolean_TypeComparison<T>()
            where T : struct, ITypeFormula<bool>
{
            return GetBoolean_TypeComparison<T>();
        }

        public static string WrappedGetBoolean_TypeIdComparison<T>()
            where T : struct, ITypeFormula<bool>
{
            return GetBoolean_TypeIdComparison<T>();
        }

        [SpecializationMethod(nameof(GetBooleanInternal), SpecializeAlgorithm.VirtualStrategy)]
        [SpecializedMethod(nameof(GetBooleanTrue), typeof(_true))]
        [SpecializedMethod(nameof(GetBooleanFalse), typeof(_false))]
        public static partial string GetBoolean_VirtualStrategy<T>()
            where T : struct, ITypeFormula<bool>;

        [SpecializationMethod(nameof(GetBooleanInternal), SpecializeAlgorithm.DelegateStrategy)]
        [SpecializedMethod(nameof(GetBooleanTrue), typeof(_true))]
        [SpecializedMethod(nameof(GetBooleanFalse), typeof(_false))]
        public static partial string GetBoolean_DelegateStrategy<T>()
            where T : struct, ITypeFormula<bool>;

        [SpecializationMethod(nameof(GetBooleanInternal), SpecializeAlgorithm.TypeComparison)]
        [SpecializedMethod(nameof(GetBooleanTrue), typeof(_true))]
        [SpecializedMethod(nameof(GetBooleanFalse), typeof(_false))]
        public static partial string GetBoolean_TypeComparison<T>()
            where T : struct, ITypeFormula<bool>;

        [SpecializationMethod(nameof(GetBooleanInternal), SpecializeAlgorithm.TypeIdComparison)]
        [SpecializedMethod(nameof(GetBooleanTrue), typeof(_true))]
        [SpecializedMethod(nameof(GetBooleanFalse), typeof(_false))]
        public static partial string GetBoolean_TypeIdComparison<T>()
            where T : struct, ITypeFormula<bool>;

        public static string GetBooleanTrue()
        {
            return "true";
        }

        public static string GetBooleanFalse()
        {
            return "false";
        }

        public static string GetBooleanInternal<T>()
        {
            return "none";
        }

        public static bool WrappedIs100_VirtualStrategy<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>
        {
            return Is100_VirtualStrategy<T>();
        }

        public static bool WrappedIs100_DelegateStrategy<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>
        {
            return Is100_DelegateStrategy<T>();
        }

        public static bool WrappedIs100_TypeComparison<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>
        {
            return Is100_TypeComparison<T>();
        }

        public static bool WrappedIs100_TypeIdComparison<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>
        {
            return Is100_TypeIdComparison<T>();
        }

        [SpecializationMethod(nameof(IsNot100), SpecializeAlgorithm.VirtualStrategy)]
        [SpecializedMethod(nameof(Is100), typeof(_100))]
        public static partial bool Is100_VirtualStrategy<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>;

        [SpecializationMethod(nameof(IsNot100), SpecializeAlgorithm.DelegateStrategy)]
        [SpecializedMethod(nameof(Is100), typeof(_100))]
        public static partial bool Is100_DelegateStrategy<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>;

        [SpecializationMethod(nameof(IsNot100), SpecializeAlgorithm.TypeComparison)]
        [SpecializedMethod(nameof(Is100), typeof(_100))]
        public static partial bool Is100_TypeComparison<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>;

        [SpecializationMethod(nameof(IsNot100), SpecializeAlgorithm.TypeIdComparison)]
        [SpecializedMethod(nameof(Is100), typeof(_100))]
        public static partial bool Is100_TypeIdComparison<T>()
            where T : struct, ITypeFormula<int>, ITypeFormula<float>;

        public static bool Is100()
        {
            return true;
        }

        public static bool IsNot100<T>()
        {
            return false;
        }

        public static int AddValue<T1, [DefaultType(typeof(_10))] T2>()
            where T1 : struct, ITypeFormula<int>
            where T2 : struct, ITypeFormula<int>
        {
            return TypeFormula.GetValue<T1, int>() + TypeFormula.GetValue<T2, int>();
        }

        public static string ToString<T1, [DefaultType(typeof(int))] T2, [DefaultType(typeof(float))] T3>(T1 a, T2 b = default, T3 c = default)
        {
            return $"{a}, {b}, {c}";
        }

        public static T GetDefault<[DefaultType(typeof(System.Guid))] T>()
        {
            return default;
        }

        [VariadicGeneric(1, 16)]
        public static string Take<T>(int count, T args)
        {
            int i = 0;
            string cat = string.Empty;
            using (new VariadicForEach())
            {
                if (i >= count)
                {
                    VariadicUtils.Break();
                }

                cat += args.ToString();
                ++i;
            }

            return cat;
        }

        [VariadicGeneric(1, 16)]
        public static string Skip<T>(int count, T args)
        {
            int i = 0;
            string cat = string.Empty;
            using (new VariadicForEach())
            {
                if (i < count)
                {
                    ++i;
                    VariadicUtils.Continue();
                }

                cat += args.ToString();
                ++i;
            }

            return cat;
        }

        [VariadicGeneric(1, 16)]
        public static string TakeAndSkip<T>(int takeCount, int skipCount, T args)
        {
            int i = 0;
            string cat = string.Empty;
            using (new VariadicForEach())
            {
                if (i < skipCount)
                {
                    ++i;
                    VariadicUtils.Continue();
                }

                if (i >= takeCount)
                {
                    VariadicUtils.Break();
                }

                cat += args.ToString();
                ++i;
            }

            return cat;
        }


        [VariadicGeneric(1, 16)]
        public static object[] MakeArray<T>(T args)
        {
            int i = 0;
            var values = new object[VariadicUtils.VariadicParameterCount];
            using (new VariadicForEach())
            {
                values[i] = args;
                ++i;
            }

            return values;
        }

        [VariadicGeneric(1, 16)]
        public static string ForEachJoin<T>(string separator, T args)
        {
            var str = string.Empty;
            using (new VariadicForEach())
            {
                str = $"{str}{(T)args}{separator}";
            }

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return str.Remove(str.Length - separator.Length, separator.Length);
        }

        public static string RecursiveJoin<TFirst>(string separator, TFirst arg)
        {
            return arg.ToString();
        }

        [VariadicGeneric(1, 16)]
        public static string RecursiveJoin<TFirst, TVar>(string separator, TFirst arg, TVar argVars)
        {
            var str = RecursiveJoin<TVar>(separator, argVars);
            return $"{arg}{separator}{str}";
        }

        public static string GetTypeName()
        {
            return string.Empty;
        }

        public static string GetTypeName<T>(T value)
        {
            return value.GetType().Name;
        }

        [VariadicGeneric(0, 16)]
        public static void InvokeAction<TVar>(Action<TVar> action, TVar args)
        {
            action?.Invoke(args);
        }

        [VariadicGeneric(0, 16)]
        public static TResult InvokeFunc<TResult, TVar>(Func<TVar, TResult> func, TVar args)
        {
            if (func == null)
            {
                return default;
            }

            return func(args);
        }
    }
}

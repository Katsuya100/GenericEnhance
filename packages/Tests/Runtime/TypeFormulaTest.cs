using NUnit.Framework;
using System;

namespace Katuusagi.GenericEnhance.Tests
{
    [TypeDef(typeof(Equal<bool, Not<NotEqual<int, _5, _10>>, _true>))]
    public struct BoolValue : ITypeFormula<bool>
    {
        bool ITypeFormula<bool>.Result => default;
    }

    [TypeDef(typeof(Add<float, _500_0222, _499_9998>))]
    public struct FloatValue : ITypeFormula<float>
    {
        float ITypeFormula<float>.Result => default;
    }

    [TypeDef(typeof(Add<int, _100, _200>))]
    public struct IntValue : ITypeFormula<int>, ITypeFormula<float>
    {
        int ITypeFormula<int>.Result => default;
        float ITypeFormula<float>.Result => default;
    }

    [TypeDef(typeof(Sub<float, FloatValue, IntValue>))]
    public struct SubValue : ITypeFormula<float>
    {
        float ITypeFormula<float>.Result => default;
    }

    public class TypeFormulaTest : IComparable<IntValue>
    {
        [Test]
        public void GetValue()
        {
            var p = TypeFormula.GetValue<_100, int>();
            Assert.AreEqual(p, 100);
            var n = TypeFormula.GetValue<_n100, int>();
            Assert.AreEqual(n, -100);
            var fp = TypeFormula.GetValue<_100_5, float>();
            Assert.AreEqual(fp, 100.5f);
            var fn = TypeFormula.GetValue<_n100_5, float>();
            Assert.AreEqual(fn, -100.5f);
        }

        [Test]
        public void Calc()
        {
            var add = TypeFormula.GetValue<Add<int, _100, _200>, int>();
            Assert.AreEqual(add, 100 + 200);
            var sub = TypeFormula.GetValue<Sub<int, _100, _200>, int>();
            Assert.AreEqual(sub, 100 - 200);
        }

        [Test]
        public void Variable()
        {
            var iv = TypeFormula.GetValue<IntValue, int>();
            Assert.AreEqual(iv, 100 + 200);
            var fv = TypeFormula.GetValue<FloatValue, float>();
            Assert.AreEqual(fv, 500.0222f + 499.9998f);
            var sv = TypeFormula.GetValue<SubValue, float>();
            Assert.AreEqual(sv, (500.0222f + 499.9998f) - (100 + 200));
            var result = TypeFormula.GetValue<BoolValue, bool>();
            Assert.AreEqual(result, false);
        }

        [Test]
        public void OptimizedFormula()
        {
            var v = ((IComparable<Add<int, _200, _100>>)this).CompareTo(default);
            Assert.AreEqual(v, 100);
            Assert.AreEqual(typeof(Add<int, _30, _90>), typeof(Add<int, _60, _60>));
            Assert.AreEqual(typeof(_100), typeof(Add<int, _50, _50>));
            Assert.AreNotEqual(typeof(_100), GetAddType<int, _50, _50>());
        }

        private Type GetAddType<T, TX, TY>()
            where TX : struct, ITypeFormula<T>
            where TY : struct, ITypeFormula<T>
        {
            return typeof(Add<T, TX, TY>);
        }

        int IComparable<IntValue>.CompareTo(IntValue other)
        {
            return 100;
        }
    }
}

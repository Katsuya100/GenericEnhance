using NUnit.Framework;
using System;

namespace Katuusagi.GenericEnhance.Tests
{
    public class SpecializationTest
    {
        public struct ThrewIntFloat<T>
            : ITypeFormula<int>, ITypeFormula<float>
            where T : struct, ITypeFormula<int>, ITypeFormula<float>
        {
            int ITypeFormula<int>.Result => TypeFormula.GetValue<T, int>();

            float ITypeFormula<float>.Result => TypeFormula.GetValue<T, float>();
        }

        [Test]
        public void CallDefaultInstanceMethod()
        {
            var t = new TestFunctions();
            {
                var i = t.GetValue_VirtualStrategy<long>();
                Assert.AreEqual(i, default(long));
            }
            {
                var i = t.GetValue_DelegateStrategy<long>();
                Assert.AreEqual(i, default(long));
            }
            {
                var i = t.GetValue_TypeComparison<long>();
                Assert.AreEqual(i, default(long));
            }
            {
                var i = t.GetValue_TypeIdComparison<long>();
                Assert.AreEqual(i, default(long));
            }
        }

        [Test]
        public void CallDynamicDefaultInstanceMethod()
        {
            var t = new TestFunctions();
            {
                var i = t.WrappedGetValue_VirtualStrategy<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.WrappedGetValue_DelegateStrategy<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.WrappedGetValue_TypeComparison<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.WrappedGetValue_TypeIdComparison<int>();
                Assert.AreEqual(i, 100);
            }
        }

        [Test]
        public void CallSpecializeInstanceMethod()
        {
            var t = new TestFunctions();
            {
                var i = t.GetValue_VirtualStrategy<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.GetValue_DelegateStrategy<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.GetValue_TypeComparison<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.GetValue_TypeIdComparison<int>();
                Assert.AreEqual(i, 100);
            }
        }

        [Test]
        public void CallDynamicSpecializeInstanceMethod()
        {
            var t = new TestFunctions();
            {
                var i = t.WrappedGetValue_VirtualStrategy<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.WrappedGetValue_DelegateStrategy<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.WrappedGetValue_TypeComparison<int>();
                Assert.AreEqual(i, 100);
            }
            {
                var i = t.WrappedGetValue_TypeIdComparison<int>();
                Assert.AreEqual(i, 100);
            }
        }

        [Test]
        public void CallDefaultStaticMethod()
        {
            {
                var c = TestFunctions.Add_VirtualStrategy<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
            {
                var c = TestFunctions.Add_TypeComparison<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
            {
                var c = TestFunctions.Add_TypeIdComparison<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
            {
                var c = TestFunctions.Add_DelegateStrategy<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
        }

        [Test]
        public void CallDynamicDefaultStaticMethod()
        {
            {
                var c = TestFunctions.WrappedAdd_VirtualStrategy<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
            {
                var c = TestFunctions.WrappedAdd_TypeComparison<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
            {
                var c = TestFunctions.WrappedAdd_TypeIdComparison<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
            {
                var c = TestFunctions.WrappedAdd_DelegateStrategy<char, char, char>('a', 'b');
                Assert.AreEqual(c, default(char));
            }
        }

        [Test]
        public void CallSpecializedStaticMethod()
        {
            {
                var f = TestFunctions.Add_VirtualStrategy<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.Add_VirtualStrategy<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);

                Func<int, int, int> a = TestFunctions.Add_VirtualStrategy<int, int, int>;
                var i2 = a(10, 20);
                Assert.AreEqual(i2, 30);
            }

            {
                var f = TestFunctions.Add_TypeComparison<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.Add_TypeComparison<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);

                Func<int, int, int> a = TestFunctions.Add_TypeComparison<int, int, int>;
                var i2 = a(10, 20);
                Assert.AreEqual(i2, 30);
            }

            {
                var f = TestFunctions.Add_TypeIdComparison<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.Add_TypeIdComparison<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);

                Func<int, int, int> a = TestFunctions.Add_TypeIdComparison<int, int, int>;
                var i2 = a(10, 20);
                Assert.AreEqual(i2, 30);
            }

            {
                var f = TestFunctions.Add_DelegateStrategy<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.Add_DelegateStrategy<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);

                Func<int, int, int> a = TestFunctions.Add_DelegateStrategy<int, int, int>;
                var i2 = a(10, 20);
                Assert.AreEqual(i2, 30);
            }
        }

        [Test]
        public void CallDynamicSpecializedStaticMethod()
        {
            {
                var f = TestFunctions.WrappedAdd_VirtualStrategy<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.WrappedAdd_VirtualStrategy<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);
            }
            {
                var f = TestFunctions.WrappedAdd_TypeComparison<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.WrappedAdd_TypeComparison<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);
            }
            {
                var f = TestFunctions.WrappedAdd_TypeIdComparison<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.WrappedAdd_TypeIdComparison<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);
            }
            {
                var f = TestFunctions.WrappedAdd_DelegateStrategy<float, float, float>(30, 40);
                Assert.AreEqual(f, 70);

                var i = TestFunctions.WrappedAdd_DelegateStrategy<int, int, int>(10, 20);
                Assert.AreEqual(i, 30);
            }
        }

        [Test]
        public void CallTypeFormulaSpecializedStaticMethod()
        {
            {
                var a = TestFunctions.GetBoolean_VirtualStrategy<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.GetBoolean_VirtualStrategy<Equal<int, _100, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.GetBoolean_VirtualStrategy<Equal<int, _100, _101>>();
                Assert.AreEqual(c, "false");
            }
            {
                var a = TestFunctions.GetBoolean_DelegateStrategy<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.GetBoolean_DelegateStrategy<Equal<int, _100, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.GetBoolean_DelegateStrategy<Equal<int, _100, _101>>();
                Assert.AreEqual(c, "false");
            }
            {
                var a = TestFunctions.GetBoolean_TypeComparison<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.GetBoolean_TypeComparison<Equal<int, _100, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.GetBoolean_TypeComparison<Equal<int, _100, _101>>();
                Assert.AreEqual(c, "false");
            }
            {
                var a = TestFunctions.GetBoolean_TypeIdComparison<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.GetBoolean_TypeIdComparison<Equal<int, _100, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.GetBoolean_TypeIdComparison<Equal<int, _100, _101>>();
                Assert.AreEqual(c, "false");
            }

            {
                var d = TestFunctions.Is100_VirtualStrategy<_100>();
                Assert.True(d);

                var e = TestFunctions.Is100_VirtualStrategy<_101>();
                Assert.False(e);

                var f = TestFunctions.Is100_VirtualStrategy<Add<int, _70, _30>>();
                Assert.True(f);
            }

            {
                var d = TestFunctions.Is100_DelegateStrategy<_100>();
                Assert.True(d);

                var e = TestFunctions.Is100_DelegateStrategy<_101>();
                Assert.False(e);

                var f = TestFunctions.Is100_DelegateStrategy<Add<int, _70, _30>>();
                Assert.True(f);
            }

            {
                var d = TestFunctions.Is100_TypeComparison<_100>();
                Assert.True(d);

                var e = TestFunctions.Is100_TypeComparison<_101>();
                Assert.False(e);

                var f = TestFunctions.Is100_TypeComparison<Add<int, _70, _30>>();
                Assert.True(f);
            }

            {
                var d = TestFunctions.Is100_TypeIdComparison<_100>();
                Assert.True(d);

                var e = TestFunctions.Is100_TypeIdComparison<_101>();
                Assert.False(e);

                var f = TestFunctions.Is100_TypeIdComparison<Add<int, _70, _30>>();
                Assert.True(f);
            }
        }

        [Test]
        public void CallTypeFormulaDynamicSpecializedStaticMethod()
        {
            {
                var a = TestFunctions.WrappedGetBoolean_VirtualStrategy<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.WrappedGetBoolean_VirtualStrategy<Equal<int, ThrewIntFloat<_100>, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.WrappedGetBoolean_VirtualStrategy<Equal<int, ThrewIntFloat<_100>, _101>>();
                Assert.AreEqual(c, "false");
            }
            {
                var a = TestFunctions.WrappedGetBoolean_DelegateStrategy<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.WrappedGetBoolean_DelegateStrategy<Equal<int, ThrewIntFloat<_100>, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.WrappedGetBoolean_DelegateStrategy<Equal<int, ThrewIntFloat<_100>, _101>>();
                Assert.AreEqual(c, "false");
            }
            {
                var a = TestFunctions.WrappedGetBoolean_TypeComparison<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.WrappedGetBoolean_TypeComparison<Equal<int, ThrewIntFloat<_100>, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.WrappedGetBoolean_TypeComparison<Equal<int, ThrewIntFloat<_100>, _101>>();
                Assert.AreEqual(c, "false");
            }
            {
                var a = TestFunctions.WrappedGetBoolean_TypeIdComparison<_true>();
                Assert.AreEqual(a, "true");

                var b = TestFunctions.WrappedGetBoolean_TypeIdComparison<Equal<int, ThrewIntFloat<_100>, _100>>();
                Assert.AreEqual(b, "true");

                var c = TestFunctions.WrappedGetBoolean_TypeIdComparison<Equal<int, ThrewIntFloat<_100>, _101>>();
                Assert.AreEqual(c, "false");
            }

            {
                var d = TestFunctions.WrappedIs100_VirtualStrategy<_100>();
                Assert.True(d);

                var e = TestFunctions.WrappedIs100_VirtualStrategy<_101>();
                Assert.False(e);

                var f = TestFunctions.WrappedIs100_VirtualStrategy<Add<int, ThrewIntFloat<_70>, _30>>();
                Assert.True(f);
            }

            {
                var d = TestFunctions.WrappedIs100_DelegateStrategy<_100>();
                Assert.True(d);

                var e = TestFunctions.WrappedIs100_DelegateStrategy<_101>();
                Assert.False(e);

                var f = TestFunctions.WrappedIs100_DelegateStrategy<Add<int, ThrewIntFloat<_70>, _30>>();
                Assert.True(f);
            }

            {
                var d = TestFunctions.WrappedIs100_TypeComparison<_100>();
                Assert.True(d);

                var e = TestFunctions.WrappedIs100_TypeComparison<_101>();
                Assert.False(e);

                var f = TestFunctions.WrappedIs100_TypeComparison<Add<int, ThrewIntFloat<_70>, _30>>();
                Assert.True(f);
            }

            {
                var d = TestFunctions.WrappedIs100_TypeIdComparison<_100>();
                Assert.True(d);

                var e = TestFunctions.WrappedIs100_TypeIdComparison<_101>();
                Assert.False(e);

                var f = TestFunctions.WrappedIs100_TypeIdComparison<Add<int, ThrewIntFloat<_70>, _30>>();
                Assert.True(f);
            }
        }
    }
}

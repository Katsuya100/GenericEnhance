using NUnit.Framework;

namespace Katuusagi.GenericEnhance.Tests
{
    public class NoneTypeTest
    {
        [Test]
        public void NoneTypeArg()
        {
            {
                var typeName = TestFunctions.GetTypeName();
                Assert.AreEqual(typeName, string.Empty);
            }

            {
                var typeName = TestFunctions.GetTypeName(0);
                Assert.AreEqual(typeName, typeof(int).Name);
            }

            {
                var typeName = TestFunctions.GetTypeName(NoneType.Default);
                Assert.AreEqual(typeName, string.Empty);
            }
        }

        [Test]
        public void Variadic()
        {
            {
                int result = 0;
                TestFunctions.InvokeAction(_ =>
                {
                    result = 1;
                });
                Assert.AreEqual(result, 1);
            }
            {
                int result = TestFunctions.InvokeFunc(_ =>
                {
                    return 2;
                });
                Assert.AreEqual(result, 2);
            }
        }
    }
}

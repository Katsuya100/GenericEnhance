using NUnit.Framework;

namespace Katuusagi.GenericEnhance.Tests
{
    public class DefaultTypeTest
    {
        [Test]
        public void DefaultTypeArgument()
        {
            var v = TestFunctions.AddValue<_10>();
            Assert.AreEqual(v, 10 + 10);
        }
        [Test]
        public void MultipleDefaultTypeArguments()
        {
            var v = TestFunctions.ToString(0);
            Assert.AreEqual(v, $"{0}, {default(int)}, {default(float)}");
        }
        [Test]
        public void ReturnGenericArgument()
        {
            var v = TestFunctions.GetDefault();
            Assert.AreEqual(v, default(System.Guid));
        }
    }
}

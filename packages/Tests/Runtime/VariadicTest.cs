using NUnit.Framework;

namespace Katuusagi.GenericEnhance.Tests
{
    public class VariadicTest
    {
        [Test]
        public void Recursive()
        {
            var concat = TestFunctions.RecursiveJoin(", ", 1, 2.3f, 4.56789, "hoge");
            Assert.AreEqual(concat, $"{1}, {2.3f}, {4.56789}, hoge");
        }

        [Test]
        public void ForEach()
        {
            var concat = TestFunctions.ForEachJoin(", ", 1, 2.3f, 4.56789, "hoge");
            Assert.AreEqual(concat, $"{1}, {2.3f}, {4.56789}, hoge");
        }

        [Test]
        public void Break()
        {
            var concat = TestFunctions.Take(2, 1, 2, 3, 4);
            Assert.AreEqual(concat, "12");
        }

        [Test]
        public void Continue()
        {
            var concat = TestFunctions.Skip(2, 1, 2, 3, 4);
            Assert.AreEqual(concat, "34");
        }

        [Test]
        public void ContinueAndBreak()
        {
            var concat = TestFunctions.TakeAndSkip(4, 2, 1, 2, 3, 4, 5, 6);
            Assert.AreEqual(concat, "34");
        }

        [Test]
        public void Count()
        {
            var array = TestFunctions.MakeArray(1, 2, 3, 4, 5, 6);
            Assert.AreEqual(array[0], 1);
            Assert.AreEqual(array[1], 2);
            Assert.AreEqual(array[2], 3);
            Assert.AreEqual(array[3], 4);
            Assert.AreEqual(array[4], 5);
            Assert.AreEqual(array[5], 6);
        }
    }
}

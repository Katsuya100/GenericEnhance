using Katuusagi.GenericEnhance.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Katuusagi.GenericEnhance.Tests
{
    public class TypeDefTest
    {
        [Test]
        public void TypeOf()
        {
            Assert.AreEqual(typeof(TestFunctions.DefInt), typeof(int));
            Assert.AreEqual(typeof(List<TestFunctions.AnyStructDef<int>>), typeof(List<int>));

            {
                TestFunctions.DefInt v = default;
                Assert.AreEqual(v.GetType(), typeof(int));
            }

            {
                List<TestFunctions.AnyStructDef<int>> v = new List<TestFunctions.AnyStructDef<int>>();
                Assert.AreEqual(v.GetType(), typeof(List<int>));
            }

            {
                Assert.AreEqual(TestFunctions.DefValue.GetType(), typeof(int));
            }

            {
                var obj = new GameObject();
                try
                {
                    CastUtils.TryAs(ref obj, out TestFunctions.AnyClassDef<GameObject> v);
                    Assert.AreEqual(v.GetType(), typeof(GameObject));
                }
                finally
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        [Test]
        public void MemberAccess()
        {
            {
                var vec = Vector3.one;
                CastUtils.TryAs(ref vec, out TestFunctions.AnyStructDef<Vector3> v);
                Assert.AreEqual(v.magnitude, vec.magnitude);
            }

            {
                var obj = new GameObject();
                try
                {
                    CastUtils.TryAs(ref obj, out TestFunctions.AnyClassDef<GameObject> v);
                    Assert.AreEqual(v.transform, obj.transform);
                }
                finally
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }
    }
}

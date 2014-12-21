using System;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    [TestFixture]
    public class NUnitTestsBase
    {
        #region Setup/Teardown

        [TearDown]
        public virtual void BaseTestCleanup()
        {
            EquivalenceComparer.ClearGlobalFilter();
        }

        #endregion


        #region Delegates

        public delegate void Proc();

        #endregion


        public NUnitTestsBase()
        {
            Check.FireDotNetAsserts = false;
        }


        #region Test helpers

        public static void AssertEqualsContract<T>(T x, T y, T z)
        {
            //symetrically equal
            Assert.That(x, Is.EqualTo(y), "x is not equal to y");
            Assert.That(y, Is.EqualTo(x), "y is not equal to x");
            Assert.That(x.GetHashCode(), Is.EqualTo(y.GetHashCode()), "x hash code not equal to y");

            //equal by transitive closure
            Assert.That(y, Is.EqualTo(z), "y is not equal to z");
            Assert.That(x, Is.EqualTo(z), "x is not equal to z");
            Assert.That(x.GetHashCode(), Is.EqualTo(z.GetHashCode()), "x hash code not equal to z");
        }


        public static void AssertNotEqualsContract<T>(T x, T y, T z)
        {
            //symetrically not equal
            Assert.That(x, Is.Not.EqualTo(y), "x is equal to y");
            Assert.That(y, Is.Not.EqualTo(x), "y is equal to x");

            //not equal by transitive closure
            Assert.That(y, Is.Not.EqualTo(z), "y is equal to z");
            Assert.That(x, Is.Not.EqualTo(z), "x is equal to z");
        }


        /// <summary>
        /// TODO: Replace with Assert.ThrowsInstanceOfType{T} when added to NUnit
        /// </summary>
        public static T ExecuteAndExpect<T>(Proc proc) where T : Exception
        {
            try
            {
                proc();
            }
            catch (Exception e)
            {
                Assert.That(e, Is.AssignableTo<T>());
                return (T) e;
            }

            Assert.Fail(string.Format("Exepected an exception of type: {0}", typeof (T).Name));
            return null;
        }


        public static void ExecuteAndIgnore<T>(Proc proc)
        {
            try
            {
                proc();
            }
            catch (Exception e)
            {
                if (!typeof (T).IsAssignableFrom(e.GetType()))
                    throw;
            }
        }

        #endregion
    }
}
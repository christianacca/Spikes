using System;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class Nullables
    {
        [Test]
        public void NullableEnumConversion()
        {
            object boxedInt = 1;

            //explicit conversion from a boxed int to an enum works fine
            TestEnum enumValue = (TestEnum)boxedInt;
            Assert.That(enumValue, Is.EqualTo(TestEnum.Apple), "Explit conversion works fine");

            //explicit conversion from a boxed int to a nullable enum is invalid
            Assert.Throws<InvalidCastException>(delegate {
                TestEnum? nullableEnum = (TestEnum?) boxedInt;
            });
        }

        [Test]
        public void HowToSafelyAssignNullableToEquivalentNonNullableType()
        {
            int i = 0;
            int? ni = null;

            //safe way to coherse a nullable to a non-nullable
            i = ni ?? 1;

            //bad way (notice it throws an excpetion because ni == null)
            Assert.Throws<InvalidOperationException>(delegate {
                i = (int) ni;
            });

            //another bad way
            Assert.Throws<InvalidOperationException>(delegate{
                i = ni.Value;
            });
        }

        [Test]
        public void TestingForNull()
        {
            int? ni = null;
            Assert.That(ni == null, Is.True, "shorthand");
            Assert.That(ni.HasValue, Is.False, "long form");
        }


        enum TestEnum
        {
            Pears = 0,
            Apple = 1,
            Orange = 2
        }
    }
}
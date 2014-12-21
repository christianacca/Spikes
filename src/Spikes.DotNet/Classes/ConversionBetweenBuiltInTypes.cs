using System;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ConversionBetweenBuiltInTypes
    {
        [Test]
        public void WideningConversionsAreSupportedImplicitly()
        {
            double d;
            int i = 100;

            d = i;
            Assert.That(d, Is.EqualTo(100));
        }


        [Test]
        public void NarrowingConversionsAreSupportedExplicitly()
        {
            double d = 100;
            int i;

            i = (int) d;
            Assert.That(i, Is.EqualTo(100));
        }


        [Test]
        public void FractionsAreTruncatedWhenConvertingToAnInt()
        {
            Single f = 1.99f;
            Double d = 1.99;
            Decimal dec = 1.99m;

            Assert.That((int)f, Is.EqualTo(1), "single has been truncated");
            Assert.That((int)d, Is.EqualTo(1), "double has been truncated");
            Assert.That((int)dec, Is.EqualTo(1), "decimal has been truncated");
        }


        [Test]
        public void OverflowExceptionThrownWhenAssigningNumbersBiggerThan_Int32_MaxValue()
        {
            decimal bigNumber = Int32.MaxValue;
            bigNumber += 1;
            Assert.Throws<OverflowException>(delegate {
                int i = (int) bigNumber;
            });
        }
    }
}
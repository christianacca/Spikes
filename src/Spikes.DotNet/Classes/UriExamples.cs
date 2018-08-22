using System;
using NUnit.Framework;

namespace Eca.Spikes.DotNet.Classes
{
    [TestFixture]
    public class UriExamples_Path
    {
        [Test]
        public void Will_automatically_add_empty_path()
        {
            var value = new Uri("http://somedomain");
            Assert.That(value.ToString(), Is.EqualTo("http://somedomain/"));
        }

        [Test]
        public void Will_not_add_trailing_slash_to_path()
        {
            var value = new Uri("http://somedomain/Spa");
            Assert.That(value.ToString(), Is.EqualTo("http://somedomain/Spa"));
        }
    }

    [TestFixture]
    public class UriExamples_equals
    {
        public void Uses_value_based_equality()
        {
            var left = new Uri("http://somedomain");
            var right = new Uri("http://somedomain");
            var different = new Uri("http://otherdomain");

            Assert.That(left == right, Is.True);
            Assert.That(left == different, Is.False);
        }

        [Test]
        public void Default_port_number_ignored()
        {
            var left = new Uri("http://somedomain:80");
            var right = new Uri("http://somedomain");

            Assert.That(left == right, Is.True);
        }

        [Test]
        public void Tailing_backslash_ignored()
        {
            var left = new Uri("http://somedomain/");
            var right = new Uri("http://somedomain");

            Assert.That(left == right, Is.True);
        }

        [Test]
        public void Case_ignored()
        {
            var left = new Uri("http://SomeDomain/");
            var right = new Uri("http://somedomain");

            Assert.That(left == right, Is.True);
        }
    }
}
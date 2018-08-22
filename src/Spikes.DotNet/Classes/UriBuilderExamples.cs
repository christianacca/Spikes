using System;
using NUnit.Framework;

namespace Eca.Spikes.DotNet.Classes
{
    [TestFixture]
    public class UriBuilderExamples_Path
    {
        [Test]
        public void Leading_slash_not_required()
        {
            var existingTrailingSlash = new Uri("http://somedomain/");
            var value = new UriBuilder(existingTrailingSlash)
            {
                Path = "Spa"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa"));
        }

        [Test]
        public void Leading_slash_ignored()
        {
            var existingTrailingSlash = new Uri("http://somedomain/");
            var value = new UriBuilder(existingTrailingSlash)
            {
                Path = "/Spa"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa"));
        }

        [Test]
        public void Trailing_slash_preserved()
        {
            var existingTrailingSlash = new Uri("http://somedomain/");
            var value = new UriBuilder(existingTrailingSlash)
            {
                Path = "Spa/"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa/"));
        }

        [Test]
        public void Trailing_slash_not_added_to_path_by_default()
        {
            var existingTrailingSlash = new Uri("http://somedomain/");
            var value = new UriBuilder(existingTrailingSlash)
            {
                Path = "Spa"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa"));
        }

        [Test]
        public void Trailing_slash_will_not_be_normalized()
        {
            var existingTrailingSlash = new Uri("http://somedomain/");
            var value = new UriBuilder(existingTrailingSlash)
            {
                Path = "Spa//"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa//"));
        }
    }

    [TestFixture]
    public class UriBuilderExamples_Fragment
    {
        [Test]
        public void Hash_character_not_supplied()
        {
            var value = new UriBuilder("http://somedomain/Spa")
            {
                Fragment = "assets"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa#assets"));
        }

        [Test]
        public void Url_trailing_slash_will_be_preserved()
        {
            var value = new UriBuilder("http://somedomain/Spa/")
            {
                Fragment = "assets/1"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa/#assets/1"));
        }

        [Test]
        public void Fragment_leading_slash_will_be_preserved()
        {
            var value = new UriBuilder("http://somedomain/Spa/")
            {
                Fragment = "/assets/1"
            };

            Assert.That(value.Uri.ToString(), Is.EqualTo("http://somedomain/Spa/#/assets/1"));
        }
    }
}
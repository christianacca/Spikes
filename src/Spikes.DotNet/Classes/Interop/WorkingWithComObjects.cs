using NUnit.Framework;
using SpikesInteropExample;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class WorkingWithComObjects
    {
        [Test]
        public void CanCreateComProxy()
        {
            PersonClass person = new PersonClass();
            person.Firstname = "Christian";
            Assert.That(person.Firstname, Is.EqualTo("Christian"));
        }
    }
}
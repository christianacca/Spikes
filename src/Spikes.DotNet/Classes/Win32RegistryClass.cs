using Microsoft.Win32;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class Win32RegistryClass
    {
        [Test]
        public void CanReadFromRegistry()
        {
            object value = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion").GetValue("ProgramFilesDir");
            Assert.That(value, Is.Not.Null);
        }
    }
}
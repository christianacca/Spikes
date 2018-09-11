using Microsoft.Win32;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class Win32RegistryClass
    {
        [Test]
        public void Can_read_from_LocalMachine_Hive()
        {
            object value = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion").GetValue("ProgramFilesDir");
            Assert.That(value, Is.Not.Null);
        }


        [Test]
        public void Can_read_value_from_Users_Hive()
        {
            object value = Registry.Users.OpenSubKey(@"S-1-5-21-1777940914-404423538-310601177-4787\Environment").GetValue("TEMP");
            Assert.That(value, Is.Not.Null);
        }

        [Test]
        public void Can_write_to_Users_Hive()
        {
            RegistryKey environKey = Registry.Users.OpenSubKey(@"S-1-5-21-1777940914-404423538-310601177-4787\Environment", true);
            environKey.SetValue("OK_TO_DELETE_ME2", "some_value");
            object value = Registry.Users.OpenSubKey(@"S-1-5-21-1777940914-404423538-310601177-4787\Environment").GetValue("OK_TO_DELETE_ME2");
            Assert.That(value, Is.EqualTo("some_value"));
        }

        [Test]
        public void Can_read_value_from_CurrentUser_Hive()
        {
            object value = Registry.CurrentUser.OpenSubKey(@"Environment").GetValue("TEMP");
            Assert.That(value, Is.Not.Null);
        }

        [Test]
        public void Can_write_to_CurrentUser_Hive()
        {
            RegistryKey environKey = Registry.CurrentUser.OpenSubKey(@"Environment", true);
            environKey.SetValue("OK_TO_DELETE_ME", "mmmm");
            object value = Registry.CurrentUser.OpenSubKey(@"Environment").GetValue("OK_TO_DELETE_ME");
            Assert.That(value, Is.EqualTo("mmmm"));
        }
    }
}
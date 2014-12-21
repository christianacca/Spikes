using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class TestReplacementUsingRegEx
    {
        [Test]
        public void ChangeDateFormatFrom_MMDDYYYY_ToYYYYMMDD()
        {
            string pattern = @"\b(?<month>\d{1,2})-(?<day>\d{1,2})-(?<year>\d{4})";
            string replacement = @"${year}-${month}-${day}";

            Regex regex = new Regex(pattern);
            Assert.That(regex.Replace("10-25-2007", replacement), Is.EqualTo("2007-10-25"));
            Assert.That(regex.Replace("1-6-2007", replacement), Is.EqualTo("2007-1-6"));
        }
    }
}
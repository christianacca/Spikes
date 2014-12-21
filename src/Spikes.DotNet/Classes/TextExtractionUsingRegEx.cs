using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class TextExtractionUsingRegEx
    {
        [Test]
        public void SimpleExtractExample()
        {
            //(.*$) defines a capture group that is implicitly associated with the group name 1
            //the value of this group is then returned as we see below
            string pattern = @"turnover:\s*(.*$)";
            Assert.That(Regex.Match("Company turnover: £100.00", pattern).Groups["1"].Value, Is.EqualTo("£100.00"));
        }


        [Test]
        public void ExtractAllOccurrancesOf_href_links()
        {

            string href1 = "href='mylink.com'";
            string href2 = "href = 'mylink.com'";
            string href3 = "href=''";

            StringBuilder search = new StringBuilder();
            search.AppendFormat("some text {0} ", href1);
            search.AppendFormat("some more text {0}" , href2);
            search.AppendFormat("whatabout {0}?", href3);

            string pattern = @"(href\s*=\s*'.*?')";
            Regex regex = new Regex(pattern);

            List<string> stringMatches = new List<string>();
            foreach (Match match in regex.Matches(search.ToString()))
            {
                stringMatches.Add(match.Groups[1].Value);
            }
            Assert.That(stringMatches.ToArray(), Is.EqualTo(ListContaining(href1, href2, href3)));
        }


        [Test]
        public void ExtractNiNumber()
        {
            string containingNiNumber = "sdfsdfNP68HG127     \t \n";
            string pattern = @"(\w{9})\s*\n?$";
            Assert.That(Regex.Match(containingNiNumber, pattern).Groups[1].Value, Is.EqualTo("NP68HG127"));
        }


        private string[] ListContaining(params string[] items)
        {
            return items;
        }
    }
}
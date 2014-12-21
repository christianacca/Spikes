using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class WorkingWithStrings
    {
        [Test]
        public void JoiningAnEmptyArrayReturnsEmptyString()
        {
            string[] empty = new string[0];
            Assert.That(string.Join(",", empty), Is.EqualTo(string.Empty));
        }


        [Test]
        public void PerformatStringConcatenating()
        {
            string one = "one";
            string two = "two";
            string three = "three";
            string four = "four";
            string expected = one + two + three + four; // inefficient concatenation as it creates additional objects
            Assert.That(string.Format("{0}{1}{2}{3}", one, two, three, four), Is.EqualTo(expected));
            Assert.That(string.Concat(one, two, three, four), Is.EqualTo(expected));
        }



        [TestFixture]
        public class StringComparisonExamples
        {
            [Test]
            public void IgnoringCase()
            {
                //these two comparisons are identical - they both use the current culture, and both ignore case
                int resultsUsingStringClass = string.Compare("Hello WORLD", "hello worlD", true);
                int resultsUsingCompareInfoClass =
                    Thread.CurrentThread.CurrentCulture.CompareInfo.Compare("HELLO", "hello", CompareOptions.IgnoreCase);

                Assert.That(resultsUsingCompareInfoClass, Is.EqualTo(resultsUsingStringClass));
            }


            [Test]
            public void CompareByOridinalPositionShouldNotBeUsedForSortingStrings()
            {
                bool A_fallsAfter_a;
                A_fallsAfter_a = (string.Compare("A", "a") > 0);
                Assert.That(A_fallsAfter_a, Is.True);

                A_fallsAfter_a = (string.CompareOrdinal("A", "a") > 0);
                Assert.That(A_fallsAfter_a, Is.False, "Using ordinal comparison, a falls after A");
            }


            [Test]
            public void CanIgnoreWhiteSpaceAndPunctuationInComparisons()
            {
                string s1 = "Some   silly,sentance;";
                string s2 = "Some silly sentance";

                CompareInfo comparer = Thread.CurrentThread.CurrentCulture.CompareInfo;
                Assert.That(comparer.Compare(s1, s2, CompareOptions.IgnoreSymbols), Is.EqualTo(0));
            }


            [Test]
            public void Gotcha_ComparisonsMayNotGiveTheResultsYouExpect()
            {
                //simulate running on a German culture
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

                string s1 = "Straﬂ";
                string s2 = "Strass";
                Assert.That(string.Compare(s1, s2), Is.EqualTo(0), "different strings compare the same");

                Assert.That(string.Equals(s1, s2),
                            Is.False,
                            "a comparison using ordinal position of characters indicate strings are different");
            }
        }
    }
}
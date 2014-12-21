using System;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class TextMatchesUsingRegEx
    {
        private void AssertNoLinesMatch(string pattern, StringBuilder lines)
        {
            Assert.That(Regex.IsMatch(lines.ToString(), pattern, RegexOptions.Multiline), Is.False);
        }


        private void AssertAtLeastOneLineMatches(string pattern, StringBuilder lines) 
        {
            Assert.That(Regex.IsMatch(lines.ToString(), pattern, RegexOptions.Multiline), Is.True);
        }


        private StringBuilder Lines(params string[] lines) {
            StringBuilder builder = new StringBuilder();
            foreach (string line in lines)
                builder.AppendLine(line);
            return builder;
        }


        [Test]
        public void SimpleSingleDigitMatch()
        {
            string pattern = @"\d";
            Assert.That(Regex.IsMatch("1", pattern), Is.True);
            Assert.That(Regex.IsMatch("", pattern), Is.False);
            Assert.That(Regex.IsMatch("a", pattern), Is.False);
            Assert.That(Regex.IsMatch("%", pattern), Is.False);
        }


        [Test]
        public void SimpleSingleAlphaNumbericMatch()
        {
            string pattern = @"\w";
            Assert.That(Regex.IsMatch("1", pattern), Is.True);
            Assert.That(Regex.IsMatch("a", pattern), Is.True);
            Assert.That(Regex.IsMatch("A", pattern), Is.True);
            Assert.That(Regex.IsMatch("", pattern), Is.False);
            Assert.That(Regex.IsMatch(" ", pattern), Is.False);
            Assert.That(Regex.IsMatch("!", pattern), Is.False);
        }


        [Test]
        public void SimpleSingleAlphaMatch()
        {
            string pattern = @"[a-zA-Z]";
            Assert.That(Regex.IsMatch("1", pattern), Is.False);
            Assert.That(Regex.IsMatch("a", pattern), Is.True);
            Assert.That(Regex.IsMatch("A", pattern), Is.True);
            Assert.That(Regex.IsMatch("", pattern), Is.False);
            Assert.That(Regex.IsMatch(" ", pattern), Is.False);
            Assert.That(Regex.IsMatch("!", pattern), Is.False);
        }


        [Test]
        public void SimpleSingleCharacterMatch()
        {
            string pattern = @".";
            Assert.That(Regex.IsMatch("1", pattern), Is.True);
            Assert.That(Regex.IsMatch("a", pattern), Is.True);
            Assert.That(Regex.IsMatch("", pattern), Is.False);
            Assert.That(Regex.IsMatch(" ", pattern), Is.True);
            Assert.That(Regex.IsMatch("!", pattern), Is.True);
            Assert.That(Regex.IsMatch("@", pattern), Is.True);

            string carrigeReturnCharacter = "\r";
            Assert.That(Regex.IsMatch(carrigeReturnCharacter, pattern), Is.True);

            string lineFeedCharacter = "\n";
            Assert.That(Regex.IsMatch(lineFeedCharacter, pattern), Is.False);
        }


        [Test]
        public void SimpleSingleWhitespaceSpaceMatch()
        {
            string pattern = @"\s";
            Assert.That(Regex.IsMatch(" ", pattern), Is.True);
            Assert.That(Regex.IsMatch("\n", pattern), Is.True);
            Assert.That(Regex.IsMatch("\t", pattern), Is.True);
            Assert.That(Regex.IsMatch("\r", pattern), Is.True);
            Assert.That(Regex.IsMatch("1", pattern), Is.False);
            Assert.That(Regex.IsMatch("a", pattern), Is.False);
            Assert.That(Regex.IsMatch("", pattern), Is.False);
            Assert.That(Regex.IsMatch("@", pattern), Is.False);
        }

        [Test]
        public void SimpleExactMatchAgainstSetOfIntegers()
        {
            //exactly 5 numeric digits (the \A and \Z characters determine the exact match criteria)
            string pattern = @"\A\d{5}\Z";

            Assert.That(Regex.IsMatch("12345", pattern ), Is.True);
            Assert.That(Regex.IsMatch("12345\n", pattern ), Is.True);
            Assert.That(Regex.IsMatch("1234", pattern), Is.False);
        }


        [Test]
        public void SimpleExactMatchAgainstAnyLineOfIntegers()
        {
            //a line with exactly 5 numeric digits (the ^ and $ characters determines the exact match criteria for a line)
            //gotcha: you have to include explicit carriage return ie \r for this to work
            string pattern = @"^\d{5}\r$";

            AssertAtLeastOneLineMatches(pattern, Lines("aaaaaa", "12345", "123"));
            AssertAtLeastOneLineMatches(pattern, Lines("aaaaaa", "123", "12345"));
            AssertAtLeastOneLineMatches(pattern, Lines("12345", "123", "aaaaaa"));
            AssertAtLeastOneLineMatches(pattern, Lines("12345", "12345", "aaaaaa"));
            AssertNoLinesMatch(pattern, Lines("aaaaaa", "123"));
        }


        [Test]
        public void SimpleStartsWithMatchAgainstSetOfIntegers()
        {
            //starts with 5 numeric digits (the \A character determines the 'starts with' match criteria)
            string pattern = @"\A\d{5}";

            Assert.That(Regex.IsMatch("12345aa", pattern ), Is.True);
            Assert.That(Regex.IsMatch("aa12345", pattern), Is.False);
        }


        [Test]
        public void SimpleStartsWithMatchAgainstSetOfIntegers_MatchMustAppearOnFirstLine()
        {
            //starts with 5 numeric digits
            string pattern = @"\A\d{5}";

            string stringToSearch = Lines("12345", "34", "5aa").ToString();
            Assert.That(Regex.IsMatch(stringToSearch, pattern, RegexOptions.Multiline), Is.True);

            stringToSearch = Lines("12", "12345", "5aa").ToString();
            Assert.That(Regex.IsMatch(stringToSearch, pattern, RegexOptions.Multiline), Is.False);
        }


        [Test]
        public void SimpleStartsWithMatchAgainstAnyLineOfIntegers()
        {
            //a line starts with 5 numeric digits (the ^ character determines the 'start of line' match criteria)
            string pattern = @"^\d{5}";

            AssertAtLeastOneLineMatches(pattern, Lines("aaaaaa", "12345aa", "123"));
            AssertNoLinesMatch(pattern, Lines("aaaaaa", "aa12345"));
        }


        [Test]
        public void Gotcha_MatchingAgainstAnyLineRequiresMultilineOption()
        {
            string pattern = @"^a";

            string lines = Lines("  ", "a", "!").ToString();
            Assert.That(Regex.IsMatch(lines, pattern, RegexOptions.Multiline), Is.True);
            Assert.That(Regex.IsMatch(lines, pattern), Is.False);
        }


        [Test]
        public void SimpleEndsWithMatchAgainstSetOfIntegers()
        {
            //ends with 5 numeric digits (the \Z character determines the 'ends with' match criteria)
            string pattern = @"\d{5}\Z";

            Assert.That(Regex.IsMatch("aa12345", pattern), Is.True);
            Assert.That(Regex.IsMatch("12345aa", pattern ), Is.False);
        }


        [Test]
        public void SimpleEndsWithMatchAgainstSetOfIntegers_MatchMustAppearOnLastLine()
        {
            //ends with 5 numeric digits
            string pattern = @"\d{5}\r\Z";

            string stringToSearch = Lines("aaaaaa", "aaa", "aa12345").ToString();
            Assert.That(Regex.IsMatch(stringToSearch, pattern), Is.True);

            stringToSearch = Lines("aaaaaa", "a12345", "345").ToString();
            Assert.That(Regex.IsMatch(stringToSearch, pattern), Is.False);
        }


        [Test]
        public void SimpleEndsWithMatchAgainstAnyLineOfIntegers()
        {
            //a line ends with 5 numeric digits (the $ character determines the 'end of line' match criteria)
            string pattern = @"\d{5}\r$";

            AssertAtLeastOneLineMatches(pattern, Lines("aaaaaa", "a", "aa12345"));
            AssertNoLinesMatch(pattern, Lines("1234", "12345aa", "123"));
        }


        [Test]
        public void SimpleWordStartingWithMatch()
        {
            string pattern = @"\bcar";

            Assert.That(Regex.IsMatch("enter car", pattern), Is.True, "whole words match pattern");
            Assert.That(Regex.IsMatch("enter carbonate again", pattern), Is.True, "matches words starting with pattern");
            Assert.That(Regex.IsMatch("enter tocar", pattern), Is.False, "words not starting with pattern do not match");
        }


        [Test]
        public void SimpleWordEndingWithMatch()
        {
            string pattern = @"car\b";

            Assert.That(Regex.IsMatch("enter car", pattern), Is.True, "whole words match pattern");
            Assert.That(Regex.IsMatch("enter carbonate a", pattern), Is.False, "words not ending with pattern do not match");
            Assert.That(Regex.IsMatch("enter tocar", pattern), Is.True, "matches words ending with pattern");
        }


        [Test]
        public void SimpleWholeWordSearch()
        {
            string pattern = @"\bcar\b";

            Assert.That(Regex.IsMatch("This car is red", pattern), Is.True, "word found in sentance");
            Assert.That(Regex.IsMatch(" car ", pattern), Is.True, "word surrounded by spaces found");
            Assert.That(Regex.IsMatch(" car", pattern), Is.True, "word preceeded by space is found");
            Assert.That(Regex.IsMatch("car ", pattern), Is.True, "word followed by space is found");
            Assert.That(Regex.IsMatch("car", pattern), Is.True, "word with no spaces is found");
            Assert.That(Regex.IsMatch("This carbonated drink", pattern), Is.False, "start of word will not match");
        }


        [Test]
        public void TwoWordsInSentanceSearch()
        {
            string pattern = @"\belvis\b.*\balive\b";

            Assert.That(Regex.IsMatch("elvis is not alive", pattern), Is.True);
            Assert.That(Regex.IsMatch("elvis alive", pattern), Is.True);
            Assert.That(Regex.IsMatch("elvis is not here", pattern), Is.False);
            Assert.That(Regex.IsMatch("Is elvis alive or not", pattern), Is.True);
            Assert.That(Regex.IsMatch("alive - elvis", pattern), Is.False);
        }


        [Test]
        public void UsingBackreferencesToFindRepeatingWords()
        {
            string pattern = @"(?<word>\b\w+\b)\s+\k<word>";

            Assert.That(Regex.IsMatch("This car car is doubled up", pattern), Is.True);
            Assert.That(Regex.IsMatch("This car is not doubled up - car", pattern), Is.False);
            Assert.That(Regex.IsMatch("This    This  word is doubled up", pattern), Is.True);
            Assert.That(Regex.IsMatch("  This This", pattern), Is.True);
        }


        [Test]
        public void UsingBackreferencesToFindRepeatingWords_ShorthandNotation()
        {
            string pattern = @"(\b\w+\b)\s+\1";

            Assert.That(Regex.IsMatch("This car car is doubled up", pattern), Is.True);
            Assert.That(Regex.IsMatch("This car is not doubled up - car", pattern), Is.False);
            Assert.That(Regex.IsMatch("This    This  word is doubled up", pattern), Is.True);
            Assert.That(Regex.IsMatch("  This This", pattern), Is.True);
        }


        [Test]
        public void UsingBackreferencesToEnsureProperUseOfDelimiters()
        {
            string pattern = @"(?<delimiter>[""' ]).+\k<delimiter>";

            Assert.That(Regex.IsMatch(@"'Correct'", pattern), Is.True);
            Assert.That(Regex.IsMatch(@"""Correct""", pattern), Is.True);
            Assert.That(Regex.IsMatch(@" Correct ", pattern), Is.True);
            Assert.That(Regex.IsMatch(@"""Wrong'", pattern), Is.False);
            Assert.That(Regex.IsMatch(@"'Wrong""", pattern), Is.False);
            Assert.That(Regex.IsMatch(@" Wrong""", pattern), Is.False);
            Assert.That(Regex.IsMatch(@" Wrong'", pattern), Is.False);
        }


        [Test]
        public void ExampleUsTelephoneValidation()
        {

        }
    }
}
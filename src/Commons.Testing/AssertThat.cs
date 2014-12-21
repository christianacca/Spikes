using System;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    public class AssertThat
    {
        #region Class Members

        public static void AreEqual(Exception actual, Exception expected)
        {
            Assert.That(actual,
                        Is.AssignableTo(expected.GetType()),
                        "Actual exception must be the same type as Expected or derived from it");
            Assert.That(actual.Message,
                        Is.EqualTo(expected.Message),
                        "Exception messages must be the same");
            Assert.That(actual.Data, Is.EquivalentTo(expected.Data), "Exception data must be equivalent");
//            Assert.That(actual.Data.Keys, Is.EquivalentTo(expected.Data.Keys), "actual.Data.Keys must be equivalent to expected.Data.Keys");
//            Assert.That(actual.Data.Values, Is.EquivalentTo(expected.Data.Values), "actual.Data.Values must be equivalent to expected.Data.Values");
//            foreach (DictionaryEntry entry in actual.Data)
//            {
//                Assert.That(entry.Value, Is.EqualTo(expected.Data[entry.Key]), "actual.Data key/value pair must be equal");
//            }
        }


        public static void AreEquivalent<T>(T actual, T expected)
        {
            EquivalenceComparer comparer = EquivalenceComparer.Default;
            Assert.That(comparer.PropertiesNotEqual(actual, expected), Is.Empty, "properties not equal");
        }

        #endregion
    }
}
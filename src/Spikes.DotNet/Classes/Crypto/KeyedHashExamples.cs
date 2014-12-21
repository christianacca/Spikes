using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class KeyedHashExamples
    {
        /// <summary>
        /// <para>
        /// The idea here is that you want to protect the integrity of data being transmitted so
        /// that any tampering can be detected, but the data itself is not something that you need
        /// to keep secret.
        /// </para>
        /// <para>
        /// To do this you protect the hash value itself with a secret key. The hash value is now
        /// dependent on two pieces of information - the data being hashed AND the secret key. An
        /// attacker can no longer change the data and derive a new hash value and expect this to go
        /// undetected. The hash value that the attacker has derived will not be same as the hash
        /// value we derive even though the data being hashed is the same. This is because the
        /// attacker does not have access to the same secret key.
        /// </para>
        /// <para>
        /// Any difference between the hash value that we derive and the one appended to the data
        /// by an attacker will be due either to:
        /// 1. the data being tampered with
        /// 2. the secret key used to generate the hash being different
        /// </para>
        /// </summary>
        [Test]
        public void CanProtectDataFromTamperingUsingKeyedHashAlgorithm()
        {
            Rfc2898DeriveBytes ourKeyGenerator = NewKeyGenerator("My secret salt phrase", "My secret pass phrase");
            HMAC ourHashFunction = HMACSHA256.Create();
            ourHashFunction.Key = ourKeyGenerator.GetBytes(ourHashFunction.HashSize/8);

            Rfc2898DeriveBytes attackersKeyGenerator = NewKeyGenerator("Bad salt phrase", "Bad pass phrase");
            HMAC attackersHashFunction = HMACSHA256.Create();
            attackersHashFunction.Key = attackersKeyGenerator.GetBytes(ourHashFunction.HashSize/8);

            byte[] dataToHash = Encoding.UTF8.GetBytes("Data to hash");
            byte[] expectedHashValue = ourHashFunction.ComputeHash(dataToHash);

            Assert.That(attackersHashFunction.ComputeHash(dataToHash),
                        Is.Not.EqualTo(expectedHashValue),
                        "different because attacker used a differnt secret key");

            byte[] tamperedData = Encoding.UTF8.GetBytes("Tampered Data to hash");
            Assert.That(ourHashFunction.ComputeHash(tamperedData),
                        Is.Not.EqualTo(expectedHashValue),
                        "different because attacker has tampered with data");

        }


        private Rfc2898DeriveBytes NewKeyGenerator(string saltPhrase, string passPhrase)
        {
            byte[] salt = Encoding.UTF8.GetBytes(saltPhrase);
            return new Rfc2898DeriveBytes(passPhrase, salt);
        }

    }
}
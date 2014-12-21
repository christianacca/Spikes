using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class HashExamples
    {
        [Test]
        public void CanComputeHashFromAnArbitraryString()
        {
            string stringToHash = "Some string that needs hashing to protect against tampering";
            SHA256 hashFunction = SHA256.Create();
            byte[] hashValue = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
            Assert.That(hashValue, Is.Not.Empty, "Hash computed");
        }

        [Test]
        public void CanComputeHashFromStream()
        {
            MemoryStream stream = StreamContaining("Some data");
            SHA256 hashFunction = SHA256.Create();
            byte[] hashValue = hashFunction.ComputeHash(stream);
            Assert.That(hashValue, Is.Not.Empty, "Hash computed");
        }


        [Test]
        public void CanProduceHashWhileReadingFromStream()
        {
            SHA256 hashFunction = SHA256.Create();
            MemoryStream dataToHash = StreamContaining("Some data");

            using (CryptoStream cryptoStream = new CryptoStream(dataToHash, hashFunction, CryptoStreamMode.Read))
            {
                while (cryptoStream.ReadByte() != -1)
                {
                    //imagine doing something useful with the data being read
                    //notice that the data being read will not be hashed
                }

                byte[] hashBuiltWhenReadingFromStream = hashFunction.Hash;

                dataToHash.Position = 0;
                byte[] computedHash = SHA256.Create().ComputeHash(dataToHash);

                Assert.That(hashBuiltWhenReadingFromStream, Is.EqualTo(computedHash));
            }
        }


        [Test]
        public void TheSameInputToHashFunctionWillAlwaysProduceSameHashValue()
        {
            string stringToHash = "Some string that needs hashing to protect against tampering";
            byte[] firstHashValue = ComputeHashFor(stringToHash);

            for (int i = 0; i < 1000; i++)
            {
                Assert.That(ComputeHashFor(stringToHash), Is.EqualTo(firstHashValue));
            }
        }


        [Test]
        public void TwoStringsThatDifferOnlyByCaseWillProduceDifferentHashValues()
        {
            Assert.That(ComputeHashFor("Some Data"), Is.Not.EqualTo(ComputeHashFor("Some DAta")));
        }


        [Test]
        public void HashIsAlwaysTheSameSize()
        {
            SHA256 hashFunction = SHA256.Create();
            byte[] firstHashValue = hashFunction.ComputeHash(Encoding.UTF8.GetBytes("Some data"));
            byte[] secondHashValue = hashFunction.ComputeHash(Encoding.UTF8.GetBytes("Some more data"));

            Assert.That(firstHashValue.Length, Is.EqualTo(secondHashValue.Length));
        }


        [Test]
        public void HashAlgorithmDeterminesSizeOfHashValue()
        {
            Assert.That(SHA256.Create().HashSize, Is.EqualTo(256));
            Assert.That(SHA512.Create().HashSize, Is.EqualTo(512));
        }


        [Test]
        public void HashAlgorithmInstanceWillRememberTheLastHashValueComputed()
        {
            SHA256 hashFunction = SHA256.Create();
            byte[] computedHashValue = hashFunction.ComputeHash(Encoding.UTF8.GetBytes("Some data"));
            Assert.That(hashFunction.Hash, Is.EqualTo(computedHashValue));
        }


        private byte[] ComputeHashFor(string toHash)
        {
            SHA256 hashFunction = SHA256.Create();
            return hashFunction.ComputeHash(Encoding.UTF8.GetBytes(toHash));
        }


        private MemoryStream StreamContaining(string value) {
            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }
    }
}
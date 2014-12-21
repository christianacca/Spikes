using System;
using System.Security.Cryptography;
using System.Text;

namespace Eca.Commons.Security
{
    public class RandomSaltedSHA256HashValue : IHasher
    {
        #region Member Variables

        private const int HashSizeInBytes = 256/8;

        #endregion


        #region IHasher Members

        public string GenerateHash(string plainText)
        {
            return GenerateHash(plainText, GetRandomSalt());
        }


        public bool VerifyHash(string plainText, string hashValue)
        {
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            if (hashWithSaltBytes.Length < HashSizeInBytes)
                return false;

            var saltBytes = new byte[hashWithSaltBytes.Length - HashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[HashSizeInBytes + i];

            return (hashValue == GenerateHash(plainText, saltBytes));
        }

        #endregion


        private byte[] CombineSaltBytesWithOtherBytes(byte[] saltBytes, byte[] otherBytes)
        {
            var otherWithSaltBytes = new byte[otherBytes.Length + saltBytes.Length];

            for (int i = 0; i < otherBytes.Length; i++)
                otherWithSaltBytes[i] = otherBytes[i];

            for (int i = 0; i < saltBytes.Length; i++)
                otherWithSaltBytes[otherBytes.Length + i] = saltBytes[i];
            return otherWithSaltBytes;
        }


        private string GenerateHash(string plainText, byte[] saltBytes)
        {
            // Create combined Array with both plainText and salt
            var plainTextWithSaltBytes = CombineSaltBytesWithOtherBytes(saltBytes ?? GetRandomSalt(),
                                                                        Encoding.ASCII.GetBytes(plainText));
            // Hash the combined array
            HashAlgorithm hash = new SHA256Managed();
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            //Create combined Hash and append salt
            byte[] hashWithSaltBytes = CombineSaltBytesWithOtherBytes(saltBytes, hashBytes);

            // Return the combined array as a string
            return Convert.ToBase64String(hashWithSaltBytes);
        }


        private byte[] GetRandomSalt()
        {
            const int minSaltSize = 4;
            const int maxSaltSize = 8;
            var random = new Random();
            var saltSize = random.Next(minSaltSize, maxSaltSize);
            var saltBytes = new byte[saltSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);
            return saltBytes;
        }


        #region Class Members

        public static RandomSaltedSHA256HashValue Instance = new RandomSaltedSHA256HashValue();

        #endregion
    }
}
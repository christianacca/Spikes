using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class SymetricKeyCryptoExamples
    {
        private const string PassPhrase = "This is my password; it could be anything - maybe the user password?";
        private const string SaltPhrase = "This is my salt; it could be anything - maybe the user name?";


        #region Test helpers

        private byte[] CryptoTransform(byte[] input, ICryptoTransform transform)
        {
            MemoryStream backingStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(backingStream, transform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(input, 0, input.Length);
                cryptoStream.FlushFinalBlock();
                return backingStream.ToArray();
            }
        }


        private byte[] Decrypt(byte[] encryptedData, string saltPhrase, string passPhrase)
        {
            Rijndael algorithm = NewSymmetricKeyAlgorithumFrom(saltPhrase, passPhrase);
            return CryptoTransform(encryptedData, algorithm.CreateDecryptor());
        }


        private string Decrypt(string encryptedText, string passPhrase, string saltPhrase)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedText);
            byte[] result = Decrypt(encryptedData, saltPhrase, passPhrase);
            return Encoding.UTF8.GetString(result);
        }


        private string Encrypt(string clearText, string passPhrase, string saltPhrase)
        {
            byte[] clearData = Encoding.UTF8.GetBytes(clearText);
            byte[] result = Encrypt(clearData, saltPhrase, passPhrase);
            return Convert.ToBase64String(result);
        }


        private byte[] Encrypt(byte[] clearData, string saltPhrase, string passPhrase)
        {
            Rijndael algorithm = NewSymmetricKeyAlgorithumFrom(saltPhrase, passPhrase);
            return CryptoTransform(clearData, algorithm.CreateEncryptor());
        }


        private Rfc2898DeriveBytes NewKeyGenerator(string saltPhrase, string passPhrase)
        {
            byte[] salt = Encoding.UTF8.GetBytes(saltPhrase);
            return new Rfc2898DeriveBytes(passPhrase, salt);
        }


        private Rijndael NewSymmetricKeyAlgorithumFrom(string saltPhrase, string passPhrase)
        {
            Rfc2898DeriveBytes keyGenerator = NewKeyGenerator(saltPhrase, passPhrase);
            Rijndael result = Rijndael.Create();
            result.Key = keyGenerator.GetBytes(result.KeySize/8);
            result.IV = keyGenerator.GetBytes(result.BlockSize/8);
            return result;
        }

        #endregion


        [Test]
        public void CanConsistentlyDeriveKeyFromKnownPasswordAndSaltPhrase()
        {
            byte[] salt = Encoding.ASCII.GetBytes(SaltPhrase);

            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(PassPhrase, salt);
            Rfc2898DeriveBytes byteGenerator2 = new Rfc2898DeriveBytes(PassPhrase, salt);

            //notice that the same bytes are returned by two Rfc2898DeriveBytes instances
            //this is because they were constructed with the same salt and password
            Assert.That(keyGenerator.GetBytes(128), Is.EqualTo(byteGenerator2.GetBytes(128)));
            Assert.That(keyGenerator.GetBytes(128), Is.EqualTo(byteGenerator2.GetBytes(128)));
        }


        [Test]
        public void CanUseSaltAndPasswordToConstructSymmetricAlgorithm()
        {
            Rfc2898DeriveBytes keyGenerator = NewKeyGenerator(SaltPhrase, PassPhrase);

            Rijndael algorithm = Rijndael.Create();
            //this is the simplest way of deciding on the size of key and IV
            //- use the default byte sizes from the algorithm class that you are going to use
            algorithm.Key = keyGenerator.GetBytes(algorithm.KeySize/8);
            algorithm.IV = keyGenerator.GetBytes(algorithm.BlockSize/8);
        }


        [Test]
        public void CanEncryptTextWhileWritingToStream()
        {
            Rijndael algorithm = NewSymmetricKeyAlgorithumFrom(SaltPhrase, PassPhrase);

            ICryptoTransform encryptor = algorithm.CreateEncryptor();
            MemoryStream backingStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(backingStream, encryptor, CryptoStreamMode.Write))
            {
                string clearText = "This is the text that is going to be encrypted";
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(clearText);
                cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                cryptoStream.FlushFinalBlock();

                Assert.That(backingStream.ToArray(), Is.Not.EqualTo(dataToEncrypt));
            }
        }


        [Test]
        public void CanEncryptTextWhileReadingFromStream()
        {
            Rijndael algorithm = NewSymmetricKeyAlgorithumFrom(SaltPhrase, PassPhrase);
            ICryptoTransform encryptor = algorithm.CreateEncryptor();
            string clearText = "This is the text that is going to be encrypted";
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(clearText);
            MemoryStream backingStream = new MemoryStream(dataToEncrypt);
            using (CryptoStream cryptoStream = new CryptoStream(backingStream, encryptor, CryptoStreamMode.Read))
            {
                MemoryStream encryptedData = new MemoryStream();
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int numOfBytesRead = cryptoStream.Read(buffer, 0, buffer.Length);
                    if (numOfBytesRead == 0) break;
                    encryptedData.Write(buffer, 0, numOfBytesRead);
                }

                Assert.That(encryptedData.ToArray(), Is.Not.EqualTo(dataToEncrypt));
            }
        }


        [Test]
        public void CanDecryptTextWhenPassphraseAndSaltUsedOnEncyptionIsKnown()
        {
            //setup
            string clearText = "This is the text that is going to be encrypted";
            string encryptedText = Encrypt(clearText, PassPhrase, SaltPhrase);

            //run test...
            string decryptedText = Decrypt(encryptedText, PassPhrase, SaltPhrase);
            Assert.That(decryptedText, Is.EqualTo(clearText));
        }


        [Test]
        public void Gotcha_MustNotConvertEncryptedDataToNormalString()
        {
            //this problem is explained here: - http://blogs.msdn.com/shawnfa/archive/2005/11/10/491431.aspx

            string clearText = "%$^This is the text that is going to be encrypted";
            byte[] encryptedData = Encrypt(Encoding.Unicode.GetBytes(clearText), SaltPhrase, PassPhrase);

            //upto this point encryptedData is fine. What happens next is the problem - ie trying
            //to round-trip the encryptedData to and from a normal unicode string
            string badString = Encoding.Unicode.GetString(encryptedData);
            byte[] badData = Encoding.Unicode.GetBytes(badString);

            //you only get to find out there is a problem when you try and decrypt the badData
            Assert.Throws<CryptographicException>(delegate {
                Decrypt(badData, SaltPhrase, PassPhrase);
            });
        }
    }
}
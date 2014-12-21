using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class AsymmetricKeyCryptoExamples
    {
        private const bool OptimalPadding = true;
        private const string PersistentContainerName = "PersistentContainer";
        private const bool ExportPublicKeyOnly = false;


        #region Test helpers

        private CspParameters DSAKeyPairFromContainer
        {
            get { return new CspParameters(13, null, PersistentContainerName); }
        }

        private string HashFunctionObjectId
        {
            get { return CryptoConfig.MapNameToOID("SHA1"); }
        }


        private CspParameters RSAKeyPairFromContainer
        {
            get
            {
                CspParameters result = new CspParameters(1, null, PersistentContainerName);
                result.KeyNumber = (int) KeyNumber.Exchange;
                return result;
            }
        }


        private void AssertAgorithmsConstructedWithSameKeyPair(RSA algorithm1, RSA algorithm2)
        {
            AssertKeyPairsAreEqual(algorithm1.ExportParameters(true), algorithm2.ExportParameters(true));
        }


        private void AssertKeyPairsAreEqual(RSAParameters keyPairRef1, RSAParameters keyPairRef2)
        {
            Assert.That(keyPairRef1.D, Is.EqualTo(keyPairRef2.D), "Private keys are the same");
            Assert.That(keyPairRef1.Modulus, Is.EqualTo(keyPairRef2.Modulus), "Public keys are the same");
        }


        private RSACryptoServiceProvider NewAlgorithmFrom(string publicKeyOnly)
        {
            RSACryptoServiceProvider result = new RSACryptoServiceProvider();
            result.FromXmlString(publicKeyOnly);
            return result;
        }


        private string NewPublicKey()
        {
            return new RSACryptoServiceProvider().ToXmlString(false);
        }

        #endregion


        #region Constructing Algorithm

        [Test]
        public void ByDefaultNewKeyPairWillBeGeneratedForEachAlgorithmInstance()
        {
            RSACryptoServiceProvider algorithm1 = new RSACryptoServiceProvider();
            RSACryptoServiceProvider algorithm2 = new RSACryptoServiceProvider();

            RSAParameters keyPair1 = algorithm1.ExportParameters(true);
            RSAParameters keyPair2 = algorithm2.ExportParameters(true);

            Assert.That(keyPair1.D, Is.Not.EqualTo(keyPair2.D), "Private keys are different");
            Assert.That(keyPair1.Modulus, Is.Not.EqualTo(keyPair2.Modulus), "Public keys are different");
        }


        [Test]
        public void CanConstructAlgorithmWithKeyPairStoredInCrytoContainer()
        {
            RSACryptoServiceProvider algorithm1 =
                new RSACryptoServiceProvider(new CspParameters(1, null, PersistentContainerName));
            RSACryptoServiceProvider algorithm2 =
                new RSACryptoServiceProvider(new CspParameters(1, null, PersistentContainerName));

            AssertAgorithmsConstructedWithSameKeyPair(algorithm1, algorithm2);
        }


        [Test]
        public void CanConstructAlgorithmWithKeyPairStoredAsXml()
        {
            RSACryptoServiceProvider algorithm1 = new RSACryptoServiceProvider();
            string keyPair = algorithm1.ToXmlString(true);

            RSACryptoServiceProvider algorithm2 = new RSACryptoServiceProvider();
            algorithm2.FromXmlString(keyPair);

            AssertAgorithmsConstructedWithSameKeyPair(algorithm1, algorithm2);
        }


        [Test]
        public void CanContructAlgorithmFromPublicKeyOnly()
        {
            RSACryptoServiceProvider algorithm1 = new RSACryptoServiceProvider(RSAKeyPairFromContainer);
            string publicKey = algorithm1.ToXmlString(false);

            RSACryptoServiceProvider algorithm2 = new RSACryptoServiceProvider();
            algorithm2.FromXmlString(publicKey);
        }

        #endregion


        [Test]
        public void CanEncryptAndDecryptData()
        {
            RSACryptoServiceProvider algorithm = new RSACryptoServiceProvider(RSAKeyPairFromContainer);
            byte[] data = Encoding.UTF8.GetBytes("some data");
            byte[] encryptedData = algorithm.Encrypt(data, OptimalPadding);
            Assert.That(algorithm.Decrypt(encryptedData, OptimalPadding), Is.EqualTo(data));
        }


        [Test]
        public void CanUsePublicKeyToEncryptButNotDecrypt()
        {
            string publicKeyOnly = NewPublicKey();
            RSACryptoServiceProvider algorithm1 = NewAlgorithmFrom(publicKeyOnly);
            RSACryptoServiceProvider algorithm2 = NewAlgorithmFrom(publicKeyOnly);

            byte[] data = Encoding.UTF8.GetBytes("some data");

            byte[] encryptedData = algorithm1.Encrypt(data, OptimalPadding);

            Assert.Throws<CryptographicException>(delegate {
                algorithm2.Decrypt(encryptedData, OptimalPadding);
            });
        }


        [Test]
        public void CanCreateDigitalSignatureForDataAndUseThisToAuthenticateAndVerifyData()
        {
            byte[] data = Encoding.UTF8.GetBytes("Some data that needs a digital signature");

            DSACryptoServiceProvider algorithm = new DSACryptoServiceProvider(DSAKeyPairFromContainer);
            byte[] signature = algorithm.SignData(data);
            Assert.That(algorithm.VerifyData(data, signature), Is.True);
        }


        [Test]
        public void CanCreateDigitalSignatureForHashAndUseThisToAuthenticateAndVerifyHash()
        {
            byte[] data = Encoding.UTF8.GetBytes("Some data that needs a digital signature");
            byte[] hashValue = SHA1.Create().ComputeHash(data);

            DSACryptoServiceProvider algorithm = new DSACryptoServiceProvider(DSAKeyPairFromContainer);
            byte[] signature = algorithm.SignHash(hashValue, HashFunctionObjectId);
            Assert.That(algorithm.VerifyHash(hashValue, HashFunctionObjectId, signature), Is.True);
        }


        [Test]
        public void CannotUsePublicKeyToCreateSignature()
        {
            string publicKeyOnly = NewPublicKey();
            RSACryptoServiceProvider algorithm1 = NewAlgorithmFrom(publicKeyOnly);

            byte[] data = Encoding.UTF8.GetBytes("Some data that needs a digital signature");
            Assert.Throws<CryptographicException>(delegate {
                algorithm1.SignData(data, HashFunctionObjectId);
            });
        }


        [Test]
        public void CanUsePublicKeyToVerifySignature()
        {
            RSACryptoServiceProvider signingAlgorithm = new RSACryptoServiceProvider(RSAKeyPairFromContainer);

            byte[] data = Encoding.UTF8.GetBytes("Some data that needs a digital signature");
            byte[] signature = signingAlgorithm.SignData(data, HashFunctionObjectId);

            RSACryptoServiceProvider verifier = new RSACryptoServiceProvider();
            verifier.FromXmlString(signingAlgorithm.ToXmlString(ExportPublicKeyOnly));

            Assert.That(verifier.VerifyData(data, HashFunctionObjectId, signature), Is.True);
        }
    }
}
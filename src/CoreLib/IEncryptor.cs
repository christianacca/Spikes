using System.Security.Cryptography;

namespace Eca.Commons
{
    /// <summary>
    /// Defines the contract for encrypting/decrypting a string
    /// </summary>
    public interface IEncryptor
    {
        /// <exception cref="CryptographicException">Thrown when there is a problem that relates to encryption</exception>
        string Encrypt(string clearText);


        /// <exception cref="CryptographicException">Thrown when there is a problem that relates to decryption</exception>
        string Decrypt(string encryptedText);
    }
}
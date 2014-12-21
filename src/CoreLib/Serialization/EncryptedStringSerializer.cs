using System;

namespace Eca.Commons.Serialization
{
    /// <summary>
    /// Uses a <see cref="IEncryptor"/> and <see cref="IStringSerializer"/> to implement the
    /// <see cref="IEncryptedStringSerializer"/> service.
    /// </summary>
    public class EncryptedStringSerializer : IEncryptedStringSerializer
    {
        #region Constructors

        public EncryptedStringSerializer(IEncryptor encryptor, IStringSerializer serializer)
        {
            Encryptor = encryptor;
            Serializer = serializer;
        }

        #endregion


        #region Properties

        private IEncryptor Encryptor { get; set; }
        private IStringSerializer Serializer { get; set; }

        #endregion


        #region IEncryptedStringSerializer Members

        public virtual T Decrypt<T>(string encryptedText)
        {
            if (String.IsNullOrEmpty(encryptedText)) return default(T);

            string dycryptedText = Encryptor.Decrypt(encryptedText);
            return Serializer.Deserialize<T>(dycryptedText);
        }


        public virtual string Encrypt<T>(T o)
        {
            if (ReferenceEquals(o, null)) return String.Empty;

            string clearText = Serializer.Serialize(o);
            return Encryptor.Encrypt(clearText);
        }

        #endregion


        #region Class Members

        public static EncryptedStringSerializer CreateJsonStringEncryptedSerializer(IEncryptor encryptor)
        {
            return new EncryptedStringSerializer(encryptor, new DataContractJsonStringSerializer());
        }

        #endregion
    }
}
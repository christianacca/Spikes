namespace Eca.Commons.Serialization
{
    /// <summary>
    /// Defines the contract to serialize/deserialize to/from an encrypted string
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation will decide the serialization format and encryption algorithm. 
    /// </para>
    /// <para>
    /// Its important to remember that whatever implementation is selected to perform the serialization
    /// and encryption, that the same implementation is used to perform the decryption and deserialization.
    /// </para>
    /// </remarks>
    public interface IEncryptedStringSerializer
    {
        /// <summary>
        /// Serializes <paramref name="o"/> into a string that is then encrypted
        /// </summary>
        /// <remarks>
        /// An implementation should return an empty string where <paramref name="o"/> is null.
        /// </remarks>
        string Encrypt<T>(T o);


        /// <summary>
        /// Decrypts <paramref name="encryptedText"/> and deserializes this into an instance of 
        /// <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// An implementation should return the default for type <typeparamref name="T"/> where
        /// <paramref name="encryptedText"/> is null or empty.
        /// </remarks>        
        T Decrypt<T>(string encryptedText);
    }
}
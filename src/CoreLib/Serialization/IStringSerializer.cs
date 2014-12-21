using System.Runtime.Serialization;

namespace Eca.Commons.Serialization
{
    /// <summary>
    /// Defines the contract to serialize/deserialize an object graph to/from a string
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation will decide the serialization format. 
    /// </para>
    /// <para>
    /// Its important to remember that whatever implementation is selected to perform the serialization, 
    /// that the same implementation is used to perform the deserialization.
    /// </para>
    /// </remarks>
    public interface IStringSerializer
    {
        /// <summary>
        /// Serializes <paramref name="o"/> into a string
        /// </summary>
        /// <remarks>
        /// An implementation should return an empty string where <paramref name="o"/> is null.
        /// </remarks>
        /// <exception cref="SerializationException">Thrown when there is a problem that relates to serialization</exception>
        string Serialize<T>(T o);


        /// <summary>
        /// Deserializes the <paramref name="encryptedText"/> into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// An implementation should return the default for type <typeparamref name="T"/> where
        /// <paramref name="encryptedText"/> is null or empty.
        /// </remarks>  
        /// <exception cref="SerializationException">Thrown when there is a problem that relates to deserialization</exception>
        T Deserialize<T>(string encryptedText);
    }
}
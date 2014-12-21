namespace Eca.Commons.Serialization
{
    /// <summary>
    /// Convenience base class that subclasses can inherit from so as to adapt
    /// their typed serialize/deserialize methods to the <see cref="IStringSerializer"/> interface
    /// </summary>
    /// <typeparam name="TSubject"></typeparam>
    public abstract class StringSerialiserAdaptor<TSubject> : IStringSerializer
    {
        #region IStringSerializer Members

        T IStringSerializer.Deserialize<T>(string encryptedText)
        {
            return EnhancedConvertor.ChangeType<T>(Deserialize(encryptedText));
        }


        string IStringSerializer.Serialize<T>(T o)
        {
            return Serialize(EnhancedConvertor.ChangeType<TSubject>(o));
        }

        #endregion


        public abstract TSubject Deserialize(string decrypted);
        public abstract string Serialize(TSubject credentials);
    }
}
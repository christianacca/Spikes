using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Eca.Commons.Extensions;

namespace Eca.Commons.Serialization
{
    /// <summary>
    /// Uses the <see cref="DataContractJsonSerializer"/> to perform the serialization
    /// </summary>
    public class DataContractJsonStringSerializer : IStringSerializer
    {
        #region IStringSerializer Members

        public T Deserialize<T>(string encryptedText)
        {
            if (String.IsNullOrEmpty(encryptedText)) return default(T);

            try
            {
                return encryptedText.FromJson<T>();
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Problem deserializing string to type {0}", typeof (T)),
                                                 ex);
            }
        }


        public string Serialize<T>(T o)
        {
            if (ReferenceEquals(o, null)) return string.Empty;

            try
            {
                return o.ToJson();
            }
            catch (Exception ex)
            {
                throw new SerializationException(
                    string.Format("Problem serializing to string, an instance of type {0}", typeof (T)), ex);
            }
        }

        #endregion
    }
}
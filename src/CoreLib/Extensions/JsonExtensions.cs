using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace Eca.Commons.Extensions
{
    public static class JsonExtensions
    {
        #region Class Members

        /// <summary>
        /// Deserialize <paramref name="json"/> into an instance of type <typeparamref name="T"/>, using
        /// the <see cref="DataContractJsonSerializer"/>
        /// </summary>
        public static T FromJson<T>(this string json)
        {
            return FromJson<T>(json, true);
        }


        /// <summary>
        /// Deserialize <paramref name="json"/> into an instance of type <typeparamref name="T"/>, using
        /// either the <see cref="DataContractJsonSerializer"/> or <see cref="JavaScriptSerializer"/>
        /// </summary>
        public static T FromJson<T>(this string json, bool useDataContract)
        {
            if (useDataContract)
            {
                using (var ms = new MemoryStream(Encoding.Default.GetBytes(json.ToCharArray())))
                {
                    var ser = new DataContractJsonSerializer(typeof (T));
                    return (T) ser.ReadObject(ms);
                }
            }
            else
            {
                return new JavaScriptSerializer().Deserialize<T>(json);
            }
        }


        /// <summary>
        /// Serializes <paramref name="obj"/> into a json string, using
        /// the <see cref="DataContractJsonSerializer"/>
        /// </summary>
        public static string ToJson<T>(this T obj)
        {
            return ToJson(obj, true);
        }


        /// <summary>
        /// Serializes <paramref name="obj"/> into a json string, using
        /// either the <see cref="DataContractJsonSerializer"/> or <see cref="JavaScriptSerializer"/>
        /// </summary>
        public static string ToJson<T>(this T obj, bool useDataContract)
        {
            if (useDataContract)
            {
                using (var ms = new MemoryStream())
                {
                    var ser = new DataContractJsonSerializer(obj.GetType());
                    ser.WriteObject(ms, obj);
                    return Encoding.Default.GetString(ms.ToArray());
                }
            }
            else
            {
                return new JavaScriptSerializer().Serialize(obj);
            }
        }


        /// <summary>
        /// Deserialize <paramref name="json"/> into an instance of type <typeparamref name="T"/>, using
        /// the <see cref="DataContractJsonSerializer"/>
        /// </summary>
        public static T TryFromJson<T>(this string json) where T : class, new()
        {
            return TryFromJson<T>(json, true);
        }


        /// <summary>
        /// Deserialize <paramref name="json"/> into an instance of type <typeparamref name="T"/>, using
        /// either the <see cref="DataContractJsonSerializer"/> or <see cref="JavaScriptSerializer"/>
        /// </summary>
        public static T TryFromJson<T>(this string json, bool useDataContract) where T : class, new()
        {
            try
            {
                return FromJson<T>(json, useDataContract);
            }
            catch (SerializationException) {}
            catch (InvalidDataContractException) {}
            catch (ArgumentNullException) {}
            return default(T);
        }

        #endregion
    }
}
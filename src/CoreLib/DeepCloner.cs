using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NValidate.Framework;

namespace Eca.Commons
{
    /// <summary>
    /// Deep clones objects
    /// </summary>
    /// <remarks>
    /// Currently the implementation is very simple and places a constraint that the object to be cloned
    /// is decorated with the <see cref="SerializableAttribute"/>.
    /// <para>
    /// This code is explained here: http://dotnetjunkies.com/WebLog/anoras/archive/2005/11/28/134032.aspx
    /// </para>
    /// <para>
    /// Consider optimising this implementation as explained here: http://andersnoras.com/blogs/anoras/archive/2007/06/07/icloneable-revisited.aspx
    /// and here: http://andersnoras.com/blogs/anoras/archive/2007/06/13/blazing-fast-reflection.aspx
    /// </para>
    /// </remarks> 
    public static class DeepCloner
    {
        #region Class Members

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="PreconditionException">The type must be serializable.</exception>
        public static T Clone<T>(T source)
        {
            Check.Require(
                () => Demand.The.Param(typeof (T).IsSerializable, "source").IsTrue("The type must be serializable."));

            if (ReferenceEquals(source, null))
            {
                return default(T);
            }
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }

        #endregion
    }
}
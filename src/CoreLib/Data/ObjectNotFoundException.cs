using System;

namespace Eca.Commons.Data
{
    /// <summary>
    /// Exception thrown when the requested object was not found in the datastore
    /// </summary>
    /// <remarks>
    /// This exception does not have any custom properties, 
    /// thus it does not implement ISerializable.
    /// </remarks>
    [Serializable]
    public sealed class ObjectNotFoundException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
        /// </summary>
        public ObjectNotFoundException() {}


        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ObjectNotFoundException(string message) : base(message) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException) {}

        #endregion
    }
}
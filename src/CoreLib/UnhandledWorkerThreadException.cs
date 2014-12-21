using System;
using System.Runtime.Serialization;

namespace Eca.Commons
{
    /// <summary>
    /// Exception that wraps an exception thrown on a worker thread
    /// </summary>
    public class UnhandledWorkerThreadException : Exception
    {
        #region Constructors

        public UnhandledWorkerThreadException(Exception innerException) : base(null, innerException) {}
        public UnhandledWorkerThreadException(string message, Exception innerException) : base(message, innerException) {}
        protected UnhandledWorkerThreadException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        #endregion
    }
}
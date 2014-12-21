using System;
using System.Runtime.Serialization;

namespace Eca.Commons.Testing
{
    public class InvalidMethodChainGrammerException : InvalidOperationException
    {
        #region Constructors

        public InvalidMethodChainGrammerException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}


        public InvalidMethodChainGrammerException(string message, Exception innerException)
            : base(message, innerException) {}


        public InvalidMethodChainGrammerException(string message) : base(message) {}
        public InvalidMethodChainGrammerException() : this("Method chaining grammer problem") {}

        #endregion
    }
}
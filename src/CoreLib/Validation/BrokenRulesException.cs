using System;
using System.Runtime.Serialization;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Exception thrown when there are broken rules
    /// </summary>
    /// <remarks>
    /// TODO: ideally BrokenRules will move into CoreLib and this with it
    /// </remarks>
    [Serializable]
    public sealed class BrokenRulesException : Exception, ISerializable
    {
        #region Member Variables

        private readonly BrokenRules _brokenRules;

        #endregion


        #region Constructors

        public BrokenRulesException() {}


        public BrokenRulesException(string message) : base(message) {}


        public BrokenRulesException(string message, Exception innerException) : base(message, innerException) {}


        public BrokenRulesException(string message, BrokenRules brokenRules) : base(message)
        {
            _brokenRules = brokenRules;
        }


        public BrokenRulesException(string message, BrokenRules brokenRules, Exception innerException)
            : base(message, innerException)
        {
            _brokenRules = brokenRules;
        }


        private BrokenRulesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Because this class is sealed, this constructor is private. 
            // if this class is not sealed, this constructor should be protected.
            _brokenRules = info.GetValue("BrokenRules", typeof (BrokenRules)) as BrokenRules;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the BrokenRules.
        /// </summary>
        public BrokenRules BrokenRules
        {
            get { return _brokenRules; }
        }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (BrokenRules != null)
                    message += Environment.NewLine + "BrokenRules: " + _brokenRules;
                return message;
            }
        }

        #endregion


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BrokenRules", _brokenRules);
            GetObjectData(info, context);
        }

        #endregion
    }
}
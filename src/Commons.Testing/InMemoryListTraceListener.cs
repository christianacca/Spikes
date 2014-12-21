using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eca.Commons.Testing
{
    public class InMemoryListTraceListener : TraceListener
    {
        #region Member Variables

        private readonly IList<string> _messages = new List<string>();

        #endregion


        #region Properties

        public IList<string> Messages
        {
            get { return _messages; }
        }

        #endregion


        public override void Write(string message)
        {
            _messages.Add(message);
        }


        public override void WriteLine(string message)
        {
            _messages.Add(string.Concat(message, Environment.NewLine));
        }
    }
}
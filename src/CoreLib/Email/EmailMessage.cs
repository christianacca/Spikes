using System;
using System.Collections.Generic;
using System.IO;

namespace Eca.Commons.Email
{
    public class EmailMessage
    {
        #region Member Variables

        private readonly IDictionary<string, Stream> _attachmentStreams = new Dictionary<string, Stream>();
        private readonly EmailAddresses _emailAddresses;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a message with reasonable defaults
        /// </summary>
        /// <remarks>
        /// By default the message will be plain text
        /// </remarks>
        public EmailMessage(EmailAddresses emailAddresses)
        {
            _emailAddresses = emailAddresses;
            Subject = string.Empty;
            Body = string.Empty;
            AttachmentPath = string.Empty;
            IsHtmlBody = false;
        }


        public EmailMessage() : this(new EmailAddresses()) {}

        public EmailMessage(string from) : this(from, String.Empty) {}
        public EmailMessage(string from, string to) : this(new EmailAddresses(from, to)) {}


        public EmailMessage(string from, string to, string cc)
            : this(new EmailAddresses(from, to) {CcList = EmailAddresses.SplitEmailAddresses(cc)}) {}

        #endregion


        #region Properties

        public string AttachmentPath { get; set; }

        public IDictionary<string, Stream> AttachmentStreams
        {
            get { return _attachmentStreams; }
        }

        public string Body { get; set; }


        public EmailAddresses EmailAddresses
        {
            get { return _emailAddresses; }
        }

        public bool IsHtmlBody { get; set; }
        public string Subject { get; set; }

        #endregion


        public void AddAttachment(Stream stream, string name)
        {
            _attachmentStreams.Add(name, stream);
        }
    }
}
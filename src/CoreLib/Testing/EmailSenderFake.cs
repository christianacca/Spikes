using System.Collections.Generic;
using Eca.Commons.Email;

namespace Eca.Commons.Testing
{
    public class EmailSenderFake : IEmailSender
    {
        #region Constructors

        public EmailSenderFake()
        {
            ActualMessages = new List<EmailMessage>();
        }

        #endregion


        #region Properties

        public IList<EmailMessage> ActualMessages { get; set; }

        #endregion


        #region IEmailSender Members

        public string Host { get; set; }


        public bool SendEmail(string from, string to, string subject, string body)
        {
            var actualMessage = new EmailMessage(from, to) {Subject = subject, Body = body};
            ActualMessages.Add(actualMessage);
            return true;
        }


        public bool SendEmail(EmailAddresses addresses, string subject, string body)
        {
            var actualMessage = new EmailMessage(addresses) {Subject = subject, Body = body};
            ActualMessages.Add(actualMessage);
            return true;
        }


        public bool SendEmail(EmailMessage emailMessage)
        {
            ActualMessages.Add(emailMessage);
            return true;
        }

        #endregion
    }
}
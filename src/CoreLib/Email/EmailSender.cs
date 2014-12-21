using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using NValidate.Framework;

namespace Eca.Commons.Email
{
    /// <summary>
    /// Web Email Helper Class.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        #region Constructors

        public EmailSender(string hostName)
        {
            Host = hostName;
        }

        #endregion


        #region IEmailSender Members

        public string Host { get; set; }


        public bool SendEmail(string from, string to, string subject, string body)
        {
            return SendEmail(new EmailMessage(from, to) {Subject = subject, Body = body});
        }


        public bool SendEmail(EmailAddresses addresses, string subject, string body)
        {
            return SendEmail(new EmailMessage(addresses) {Subject = subject, Body = body});
        }


        public bool SendEmail(EmailMessage emailMessage)
        {
            string sTo = emailMessage.EmailAddresses.RecipientList.Count > 0
                             ? emailMessage.EmailAddresses.RecipientList.ElementAt(0)
                             : String.Empty;

            // The modified using statement below adds the flexibility of controlling the disposal of the
            // MailMessage object. This is required to ensure that we can delay disposal for tests to test
            // certain properties while the production code is disposed as normal.
            using (
                DisposableAction<MailMessage> disposableMail =
                    CreateDisposableMail(emailMessage.EmailAddresses.From, sTo))
            {
                MailMessage mailMessage = disposableMail.Value;

                AddTos(emailMessage.EmailAddresses, mailMessage);
                AddCcs(emailMessage.EmailAddresses, mailMessage);
                AddBccs(emailMessage.EmailAddresses, mailMessage);
                AddReplyToIfDefined(emailMessage.EmailAddresses, mailMessage);
                AddAttachments(mailMessage, emailMessage.AttachmentPath);
                foreach (var attachmentStreamEntry in emailMessage.AttachmentStreams)
                {
                    AddAttachments(mailMessage, attachmentStreamEntry.Value, attachmentStreamEntry.Key);
                }

                mailMessage.Subject = String.IsNullOrEmpty(emailMessage.Subject)
                                          ? Application.ProductName
                                          : emailMessage.Subject;
                mailMessage.Body = emailMessage.Body;
                mailMessage.IsBodyHtml = emailMessage.IsHtmlBody;

                DoSendMail(Host, mailMessage);
            }
            return true;
        }

        #endregion


        private void AddAttachments(MailMessage mailMessage, string attachmentPath)
        {
            if (String.IsNullOrEmpty(attachmentPath)) return;

            foreach (string attachment in attachmentPath.Split(';'))
                mailMessage.Attachments.Add(new Attachment(attachment));
        }


        private void AddAttachments(MailMessage mailMessage, Stream attachmentStream, string name)
        {
            if (attachmentStream == null) return;

            mailMessage.Attachments.Add(new Attachment(attachmentStream, name));
        }


        private void AddBccs(EmailAddresses emailAddresses, MailMessage mailMessage)
        {
            foreach (string bcc in emailAddresses.BccList)
            {
                if (bcc.Trim().Length > 0)
                {
                    mailMessage.Bcc.Add(bcc);
                }
            }
        }


        private void AddCcs(EmailAddresses emailAddresses, MailMessage mailMessage)
        {
            foreach (string cc in emailAddresses.CcList)
            {
                if (cc.Trim().Length > 0)
                {
                    mailMessage.CC.Add(cc);
                }
            }
        }


        private void AddReplyToIfDefined(EmailAddresses emailAddresses, MailMessage mailMessage)
        {
            if (!string.IsNullOrEmpty(emailAddresses.ReplyTo))
                mailMessage.ReplyTo = new MailAddress(emailAddresses.ReplyTo);
        }


        private void AddTos(EmailAddresses emailAddresses, MailMessage mailMessage)
        {
            if (emailAddresses.RecipientList.Count < 2) return;

            for (int i = 1; i < emailAddresses.RecipientList.Count; i++)
                mailMessage.To.Add(emailAddresses.RecipientList.ElementAt(i));
        }


        protected virtual DisposableAction<MailMessage> CreateDisposableMail(string emailFrom, string emailTo)
        {
            return new DisposableAction<MailMessage>(o => o.Dispose(), CreateMessage(emailFrom, emailTo));
        }


        protected MailMessage CreateMessage(string emailFrom, string emailTo)
        {
            //note: do not change the parameter names below as this is being maintained for backward compatibility
            Check.Require(() => Demand.The.Param(emailTo, "to").IsNotNullOrEmpty());
            Check.Require(() => Demand.The.Param(emailFrom, "from").IsNotNullOrEmpty());

            return new MailMessage(emailFrom, emailTo);
        }


        protected virtual void DoSendMail(string exchangeServer, MailMessage message)
        {
            new SmtpClient(exchangeServer).Send(message);
        }
    }
}
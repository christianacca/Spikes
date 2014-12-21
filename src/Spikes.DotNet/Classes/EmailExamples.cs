using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class EmailExamples : FileSystemTestsBase
    {
        private const string EmailHostIPAddress = "some ip address";
        private const string MultipleReceipientAddresses = "Sevit@eca.co.uk;christian.crowhurst@eca.co.uk";
        private const string ReceipientAddress = "SevIt@eca.co.uk";
        private const string SecondReceipientAddress = "christian.crowhurst@eca.co.uk";
        private const string SecondReceipientDisplayName = "Christian Crowhurst";
        private const string SenderAddress = "christian.crowhurst@eca.co.uk";
        private const string ThirdReceipientAddress = "Kurt.Malmstrom@eca.co.uk";
        private List<IDisposable> _objectsToDisposeAtEndOfTests = new List<IDisposable>();


        public override void ReleaseFileLocksIfAnyHeld()
        {
            foreach (IDisposable obj in _objectsToDisposeAtEndOfTests)
            {
                obj.Dispose();
            }
            _objectsToDisposeAtEndOfTests.Clear();
        }


        #region Test helpers

        private MailMessage NewMailMessage()
        {
            MailMessage result = new MailMessage(SenderAddress, ReceipientAddress, "Test subject", "Test body");
            _objectsToDisposeAtEndOfTests.Add(result);
            return result;
        }

        #endregion


        [Test, Ignore("Don't want to send actual emails")]
        public void SimplestMethodOfSendingSimpleMail()
        {
            SmtpClient smtpClient = new SmtpClient(EmailHostIPAddress);
            smtpClient.Send(SenderAddress, MultipleReceipientAddresses, "Test email", "Test email");
        }


        [Test]
        public void HowToDynamicallyConstructEmailMessage()
        {
            using (
                MailMessage message1 =
                    new MailMessage(SenderAddress, ReceipientAddress, "Test subject text", "Test body text"))
            {
                message1.CC.Add(ThirdReceipientAddress);
                message1.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            }

            //Alternatively when you need more control building the message using dynamic code...
            using (MailMessage message2 = new MailMessage())
            {
                message2.From = new MailAddress(SenderAddress);
                message2.To.Add(new MailAddress(ReceipientAddress));
                message2.To.Add(new MailAddress(SecondReceipientAddress, SecondReceipientDisplayName));
                message2.Body = "Test email";
                message2.Subject = "Test email";
                message2.Priority = MailPriority.High;
            }
        }


        [Test]
        public void HowToAttachFiles()
        {
            FileInfo tempFile = TempFile.Create();
            MailMessage message = NewMailMessage();
            message.Attachments.Add(new Attachment(tempFile.FullName));
            Assert.That(message.Attachments[0].Name, Is.EqualTo(tempFile.Name));
        }


        [Test]
        public void HowToSpecifyMIMETypeForAttachments()
        {
            FileInfo tempFile = TempFile.Create();
            MailMessage message = NewMailMessage();
            message.Attachments.Add(new Attachment(tempFile.FullName, MediaTypeNames.Application.Octet));
        }


        [Test]
        public void HowToCreateHtmlEmails()
        {
            string html = "<html><body><h1>MyMessage</ht1><br>Test html message</br></body></html>";
            MailMessage message = NewMailMessage();
            message.Body = html;
            message.IsBodyHtml = true;
        }


        [Test]
        public void HowToEmbedImagesIntoMessage()
        {
            string imageId = "MyPic";
            string html =
                string.Format("<html><body><h1>MyMessage</ht1><br><img src='cid:{0}'/></br></body></html>", imageId);
            AlternateView richHtmlView =
                AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
            LinkedResource imageResource = new LinkedResource("SampleImage.jpg", MediaTypeNames.Image.Jpeg);
            imageResource.ContentId = imageId;
            richHtmlView.LinkedResources.Add(imageResource);

            string plainText = "You must use an e-mail client that supports HTML messages";
            AlternateView plainView =
                AlternateView.CreateAlternateViewFromString(plainText, null, MediaTypeNames.Text.Plain);

            MailMessage message = NewMailMessage();
            message.AlternateViews.Add(richHtmlView);
            message.AlternateViews.Add(plainView);
        }


        [Test]
        public void HowToConfigureSmtpToSendEmailOverSsl_UsingSpecifiedNetworkCredentials()
        {
            SmtpClient client = new SmtpClient(EmailHostIPAddress);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("SomeUserName", "SomePassword");
            //alternatively...
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;
        }


        [Test, Ignore("Don't want to send actual emails")]
        public void HowToSendEmailAsynchronously()
        {
            SmtpClient client = new SmtpClient(EmailHostIPAddress);
            client.SendCompleted += delegate(object sender, AsyncCompletedEventArgs e) {
                if (e.Error != null)
                    Assert.Fail(string.Format("Problem sending email; Details = {0}", e.Error));
                else if (e.Cancelled)
                    Console.Out.WriteLine("Email send operation cancelled by user");
                else
                    Console.Out.WriteLine("Email sent successfully");
            };

            client.SendAsync(NewMailMessage(), null);

            //cancel the operation
            client.SendAsyncCancel();
        }
    }
}
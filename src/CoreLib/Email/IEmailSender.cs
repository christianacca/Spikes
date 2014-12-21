namespace Eca.Commons.Email
{
    public interface IEmailSender
    {
        string Host { get; set; }
        bool SendEmail(string from, string to, string subject, string body);
        bool SendEmail(EmailAddresses addresses, string subject, string body);
        bool SendEmail(EmailMessage emailMessage);
    }
}
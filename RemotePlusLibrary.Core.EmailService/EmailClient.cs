using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace RemotePlusLibrary.Core.EmailService
{
    public class EmailClient
    {
        EmailSettings Settings { get; set; }
        public EmailClient(EmailSettings settings)
        {
            Settings = settings;
        }
        public bool SendEmail(string Subject, string Message, out Exception error)
        {
            MailMessage mm = new MailMessage(Settings.FromAddress, Settings.DefaultTo, Subject, Message);
            SmtpClient client = new SmtpClient();
            client.Host = Settings.SMTPHost;
            client.Port = Settings.Port;
            client.Timeout = Settings.Timeout;
            client.Credentials = new System.Net.NetworkCredential(Settings.Username, Settings.Password);
            client.EnableSsl = Settings.EnableSSL;
            try
            {
                client.Send(mm);
                error = null;
                return true;

            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }
        public bool SendEmail(string To, string Subject, string Message, out Exception error)
        {
            MailMessage mm = new MailMessage(Settings.FromAddress, To, Subject, Message);
            SmtpClient client = new SmtpClient();
            client.Host = Settings.SMTPHost;
            client.Port = Settings.Port;
            client.Timeout = Settings.Timeout;
            client.Credentials = new System.Net.NetworkCredential(Settings.Username, Settings.Password);
            client.EnableSsl = Settings.EnableSSL;
            try
            {
                client.Send(mm);
                error = null;
                return true;

            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }
    }
}
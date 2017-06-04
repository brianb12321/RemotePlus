using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using SecuritySystem.Email.EmailClient;

namespace SecuritySystem.Email.EmailClient
{ 
    public class Client
    {

        public static bool Send(string to, string subject, string message)
        {
            try
            {
                MailMessage newmail = new MailMessage();
                newmail.From = new MailAddress(Mail.Default.Username);
                newmail.Subject = subject;
                if (EmailClient.EmailAdvanced.EmailAdvancedSettings.Default.TimeStamp)
                {
                    newmail.Body = message + Environment.NewLine + "Sent at: " + DateTime.Now;
                }
                else
                {
                    newmail.Body = message;
                }
                newmail.To.Add(Mail.Default.To);
                SmtpClient newsmpt = new SmtpClient();
                newsmpt.Credentials = new NetworkCredential(Mail.Default.Username, Mail.Default.Password);
                newsmpt.Host = Mail.Default.smtp;
                newsmpt.EnableSsl = Mail.Default.SSL;
                newsmpt.Port = Mail.Default.port;
                newsmpt.Timeout = Mail.Default.timeout;
                newsmpt.Send(newmail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

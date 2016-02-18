using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SpcBowling.Models
{
    public class EmailNotifier
    {
        private EmailSettings emailSettings;

        public EmailNotifier() { emailSettings = new EmailSettings(); }

        public void SendNotification(string controllerName, string userName)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSSL;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                StringBuilder body = new StringBuilder()
                    .Append("User ")
                    .Append(userName)
                    .Append(" Has Accessed ")
                    .Append(controllerName)
                    .Append(" Controller")
                    .Append(" at ")
                    .Append(DateTime.Now.ToShortTimeString()).Append(" on ")
                    .Append(DateTime.Now.ToShortDateString());

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress,
                    emailSettings.MailToAddress,
                    "User Access Notification!",
                    body.ToString());

                smtpClient.Send(mailMessage);
            }
        }
    }

    public class EmailSettings
    {
        public string MailToAddress = "allotisam@hotmail.com";
        public string MailFromAddress = "allotisam@yahoo.com";
        public bool UseSSL = true;
        public string Username = "allotisam";
        public string Password = "";        // Enter my yahoo mail password
        public string ServerName = "smtp.mail.yahoo.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
    }
}
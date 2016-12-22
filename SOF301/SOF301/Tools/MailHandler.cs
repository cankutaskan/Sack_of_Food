using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace SOF301.Tools
{
    public class MailHandler
    { 
        public static void SendMail(string to, string subject, string msg)
        {
            try
            {
                new SmtpClient
                {
                    Host = "Smtp.Gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("sackoffood@Gmail.com", "sofadmin")
                }.Send(new MailMessage {
                    From = new MailAddress("sackoffood@Gmail.com", "Sack of Food Admin"),
                    To = { to },
                    Subject = subject,
                    Body = msg,
                    BodyEncoding = Encoding.UTF8 });
            }
            catch (Exception ex)
            {

            }

        }

        public static void SendForgottenPassword(SOF301.Models.Users u)
        {
            string to = u.Email;
            string subject = "Sack of Food - Forgotten Password";
            string msg = "Hello " + u.UserName + ",\n\nYour password is: " + CustomDecrypt.Decrypt(u.Password) + "\n\nBest Regards\nSack of Food Admin";
            SendMail(to, subject, msg);
        }
    }
}
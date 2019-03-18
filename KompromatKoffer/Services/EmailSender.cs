using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net; 
using System.Net.Mail;

namespace KompromatKoffer.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string To_Email_Address, string subject, string message)

        {
            if (To_Email_Address == null) return; 

            try
            {

                string sFrom_Email_Address = Config.Parameter.Mail_From_Email_Address;

                string sFrom_Email_DisplayName = Config.Parameter.Mail_From_Email_DisplayName;


                MailMessage email = new MailMessage();

                email.To.Add(new MailAddress(To_Email_Address));

                email.From = new MailAddress(sFrom_Email_Address, sFrom_Email_DisplayName);

                email.Subject = subject;

                email.Body = message;

                email.IsBodyHtml = true;


                SmtpClient smtp = new SmtpClient();

                //smtp.Host = sHost;

                //smtp.Port = Convert.ToInt32(intPort);

                //smtp.Credentials = new NetworkCredential(sEmail_Login, sEmail_Passwort);

                smtp.EnableSsl = false;


                await smtp.SendMailAsync(email);
            }
            catch (Exception ex)

            {
                Console.WriteLine("Error EmailSender.cs error:" + ex.Message);
            }
        }
    }
}

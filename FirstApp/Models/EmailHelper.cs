using System.Net.Mail;

namespace FirstApp.Models
{
    public class EmailHelper
    {
        private const string adminMailAddress = "jason75951@gmail.com";
        private const string adminMailPassword = "gmailpwd001";

        private static readonly SmtpClient client = new()
        {
            Credentials = new System.Net.NetworkCredential(adminMailAddress, adminMailPassword),
            Host = "smtpout.secureserver.net",
            Port = 80
        };

        public EmailHelper()
        {
            //client = new SmtpClient
            //{
            //    Credentials = new System.Net.NetworkCredential(adminMailAddress, adminMailPassword),
            //    Host = "smtpout.secureserver.net",
            //    Port = 80
            //};
        }

        public static bool SendEmailTwoFactorCode(string userEmail, string code)
        {
            MailMessage mailMessage = NewMethod(userEmail, "Two Factor Code", code);

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }



        public static bool SendEmail(string userEmail, string confirmationLink)
        {
            MailMessage mailMessage = NewMethod(userEmail, "Confirm your email", confirmationLink);

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public static bool SendEmailPasswordReset(string userEmail, string link)
        {
            MailMessage mailMessage = NewMethod(userEmail, "Password Reset", link);


            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // log exception
            }
            return false;
        }

        private static MailMessage NewMethod(string mailTo, string subject, string mailBody)
        {
            MailMessage mailMessage = new()
            {
                From = new MailAddress(adminMailAddress),
                Subject = subject,
                IsBodyHtml = true,
                Body = mailBody
            };
            mailMessage.To.Add(new MailAddress(mailTo));
            return mailMessage;
        }
    }
}

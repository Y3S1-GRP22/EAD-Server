// Services/CsrNotificationService.cs
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Services
{
    public class CsrNotificationService : ICsrNotificationService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "nonamenecessary0612@gmail.com";
        private readonly string _smtpPass = "ekbgdpcvlpdiciws";

        public async Task NotifyCsrsAboutNewCustomerAsync(string customerEmail, List<string> csrEmails)
        {
            foreach (var csrEmail in csrEmails)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
                emailMessage.To.Add(new MailboxAddress("CSR Team", csrEmail)); // Notify each CSR

                emailMessage.Subject = "New Customer Registered";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <p>Dear CSR,</p>
                    <p>A new customer has registered with the email: {customerEmail}.</p>
                    <p>Please take the necessary actions.</p>
                    <p>Best regards,<br>iCorner Team</p>"
                };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    try
                    {
                        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(_smtpUser, _smtpPass);
                        await client.SendAsync(emailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email to {csrEmail}: {ex.Message}");
                    }
                    finally
                    {
                        await client.DisconnectAsync(true);
                    }
                }
            }
        }
    }
}

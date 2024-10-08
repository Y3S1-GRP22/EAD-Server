using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using EAD.Services;

namespace EAD.Services
{
    public class CustomerNotificationService : ICustomerNotificationService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "nonamenecessary0612@gmail.com";
        private readonly string _smtpPass = "ekbgdpcvlpdiciws";

        public async Task NotifyCustomerActivationAsync(string customerEmail)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", customerEmail));
            emailMessage.Subject = "Account Activation Successful";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <p>Dear Customer,</p>
                <p>We are pleased to inform you that your account has been successfully activated.</p>
                <p>You can now log in and enjoy our services.</p>
                <p>Thank you for choosing iCorner.</p>
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
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        public async Task NotifyCustomerDeactivationAsync(string customerEmail)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", customerEmail));
            emailMessage.Subject = "Account Deactivation Successful";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <p>Dear Customer,</p>
                <p>We are pleased to inform you that your account has been successfully deactivated.</p>
                <p>Please continue using our services by requesting reactivation.</p>
                <p>Thank you for choosing iCorner.</p>
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
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}

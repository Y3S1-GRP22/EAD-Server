using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace EAD.Services
{
    public class VendorNotificationService : IVendorNotificationService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "nonamenecessary0612@gmail.com";
        private readonly string _smtpPass = "ekbgdpcvlpdiciws";

        public async Task NotifyVendorAsync(string vendorId, string productId, int stockQuantity)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("EAD", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", vendorId));
            emailMessage.Subject = "Low Stock Alert";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <p>Dear Vendor,</p>
                <p>We are writing to inform you that the stock for product <strong>{productId}</strong> is running low.</p>
                <p>Current stock quantity: <strong>{stockQuantity}</strong></p>
                <p>Please take the necessary actions to restock.</p>
                <p>Thank you.</p>
                <p>Best regards,<br>EAD</p>"
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

namespace EAD.Services
{
    public class VendorNotificationService : IVendorNotificationService
    {
        public Task NotifyVendorAsync(string vendorId, string productId, int stockQuantity)
        {
            // Implement your notification logic here (e.g., sending email, SMS, etc.)
            Console.WriteLine($"Notifying vendor {vendorId} about low stock for product {productId}. Current stock: {stockQuantity}");

            return Task.CompletedTask;
        }
    }
}
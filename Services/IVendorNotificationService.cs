namespace EAD.Services
{
    public interface IVendorNotificationService
    {
        Task NotifyVendorAsync(string vendorId, string productId, int stockQuantity);
    }
}
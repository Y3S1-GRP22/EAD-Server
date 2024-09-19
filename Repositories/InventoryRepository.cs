namespace EAD.Repositories
{
    using EAD.Models;
    using EAD.Services;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IVendorNotificationService _notificationService;

        public InventoryRepository(IMongoDatabase database, IVendorNotificationService notificationService)
        {
            _products = database.GetCollection<Product>("Products");
            _notificationService = notificationService;
        }

        public async Task UpdateStockAsync(string productId, int quantity)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null)
            {
                product.StockQuantity += quantity;
                await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
            }
        }

        public async Task RemoveStockAsync(string productId, int quantity)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();

            if (product != null && product.StockQuantity >= quantity)
            {
                bool hasPendingOrders = await CheckPendingOrdersAsync(productId);
                if (hasPendingOrders)
                {
                    throw new InvalidOperationException("Cannot remove stock for products with pending orders.");
                }

                product.StockQuantity -= quantity;
                await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
            }
        }

        private async Task<bool> CheckPendingOrdersAsync(string productId)
        {
            // Implement logic to check if there are pending orders for the given product
            // This could involve checking an Orders collection or similar.
            return false; // Placeholder
        }

        public async Task<int> GetStockQuantityAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            return product?.StockQuantity ?? 0;
        }

        public async Task NotifyVendorIfLowStockAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null && product.StockQuantity < 10) 
            {
                await _notificationService.NotifyVendorAsync("Vendor Id", productId, product.StockQuantity);
            }
        }
    }
}

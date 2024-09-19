namespace EAD.Repositories
{
    public interface IInventoryRepository
    {
        Task UpdateStockAsync(string productId, int quantity);
        Task RemoveStockAsync(string productId, int quantity);
        Task<int> GetStockQuantityAsync(string productId);
    }
}
namespace EAD.Repositories;
using EAD.Models;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(string id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(string id);
    Task DeactivateProductAsync(string id);
    Task ActivateProductAsync(string id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string id);
}

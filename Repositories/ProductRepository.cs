namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Category> _categories;

        public ProductRepository(IMongoDatabase database)
        {
            _products = database.GetCollection<Product>("Products");
            _categories = database.GetCollection<Category>("Categories");
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _products.Find(p => p.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = ObjectId.GenerateNewId().ToString(); // Assign new ObjectId
            }

            await _products.InsertOneAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var objectId = ObjectId.Parse(product.Id); // Ensure the ID is treated as ObjectId
            await _products.ReplaceOneAsync(p => p.Id == objectId.ToString(), product);
        }

        public async Task DeleteProductAsync(string id)
        {
            var objectId = ObjectId.Parse(id); // Convert string ID to ObjectId

            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product != null && product.StockQuantity < 10)
            {
                throw new InvalidOperationException("Stock is not empty.");
            }

            bool hasPendingOrders = await CheckPendingOrdersAsync(id);
            if (hasPendingOrders)
            {
                throw new InvalidOperationException("Cannot remove stock for products with pending orders.");
            }

            await _products.DeleteOneAsync(p => p.Id == objectId.ToString());
        }

        private async Task<bool> CheckPendingOrdersAsync(string productId)
        {
            // Implement logic to check if there are pending orders for the given product
            // This could involve checking an Orders collection or similar.
            return false; // Placeholder
        }

        public async Task DeactivateProductAsync(string id)
        {
            var objectId = ObjectId.Parse(id); // Convert string ID to ObjectId
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            await _products.UpdateOneAsync(p => p.Id == objectId.ToString(), update);
        }

        public async Task ActivateProductAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            await _products.UpdateOneAsync(p => p.Id == objectId.ToString(), update);
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithCategoriesAsync()
        {
            var products = await _products.Find(_ => true).ToListAsync();
            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();

            var categories = await _categories.Find(c => categoryIds.Contains(c.Id)).ToListAsync();
            var categoryDict = categories.ToDictionary(c => c.Id);

            foreach (var product in products)
            {
                if (categoryDict.TryGetValue(product.CategoryId, out var category))
                {
                    product.CategoryName = category.Name; 
                }
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryId)
        {
            var objectId = ObjectId.Parse(categoryId);
            var categoryExists = await _categories.Find(c => c.Id == objectId.ToString()).AnyAsync();

            if (!categoryExists)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return await _products.Find(p => p.CategoryId == objectId.ToString()).ToListAsync();
        }

    }
}

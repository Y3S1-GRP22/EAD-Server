namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryRepository(IMongoDatabase database)
        {
            _categories = database.GetCollection<Category>("Categories");
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categories.Find(_ => true).ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _categories.Find(c => c.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            if (string.IsNullOrEmpty(category.Id))
            {
                category.Id = ObjectId.GenerateNewId().ToString();
            }


            await _categories.InsertOneAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var objectId = ObjectId.Parse(category.Id);
            await _categories.ReplaceOneAsync(c => c.Id == objectId.ToString(), category);
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
        {
            var filter = Builders<Category>.Filter.Eq(c => c.IsActive, true);
            return await _categories.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllInactiveCategoriesAsync()
        {
            var filter = Builders<Category>.Filter.Eq(c => c.IsActive, false);
            return await _categories.Find(filter).ToListAsync();
        }

        public async Task DeactivateCategoryAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<Category>.Update.Set(c => c.IsActive, false);
            await _categories.UpdateOneAsync(c => c.Id == objectId.ToString(), update);
        }

        public async Task ActivateCategoryAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var update = Builders<Category>.Update.Set(c => c.IsActive, true);
            await _categories.UpdateOneAsync(c => c.Id == objectId.ToString(), update);
        }


        public async Task DeleteCategoryAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _categories.DeleteOneAsync(c => c.Id == objectId.ToString());
        }
    }
}

namespace EAD.Repositories
{
    using EAD.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(string id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(string id);
        Task DeactivateCategoryAsync(string id);
        Task ActivateCategoryAsync(string id);
        Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();
        Task<IEnumerable<Category>> GetAllInactiveCategoriesAsync();
    }
}

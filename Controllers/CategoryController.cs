using Microsoft.AspNetCore.Mvc;
using EAD.Models;
using EAD.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null.");
            }

            await _categoryRepository.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] Category category)
        {
            if (category.Name == null || id == null)
            {
                return BadRequest("Category data is invalid.");
            }

            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
            return Ok(existingCategory);
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateCategory(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeactivateCategoryAsync(id);
            return Ok(new { Id = id });
        }

        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateCategory(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.ActivateCategoryAsync(id);
            return Ok(new { Id = id });
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveCategories()
        {
            var categories = await _categoryRepository.GetAllActiveCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("deactive")]
        public async Task<IActionResult> GetAllInactiveCategories()
        {
            var categories = await _categoryRepository.GetAllInactiveCategoriesAsync();
            return Ok(categories);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            return Ok(new { Id = id });
        }
    }
}

using EAD.Models;
using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(product.CategoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            product.CategoryName = category.Name;
            
            await _productRepository.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            if (id != product.Id) return BadRequest();

            var category = await _categoryRepository.GetCategoryByIdAsync(product.CategoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            product.CategoryName = category.Name;

            await _productRepository.UpdateProductAsync(product);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productRepository.DeleteProductAsync(id);
            return Ok(new { Id = id });
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeactivateProductAsync(id);
            return Ok(new { Id = id });
        }

        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateProduct(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.ActivateProductAsync(id);
            return Ok(new { Id = id });
        }
    }
}


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
        public async Task<IActionResult> AddProduct([FromBody] Product product, [FromForm] IFormFile? imageFile)
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

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] string productId, [FromForm] IFormFile imageFile)
        {
            if (string.IsNullOrEmpty(productId))
                return BadRequest("Product ID is required.");

            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded.");

            // Save image to local folder
            var imagePath = await SaveImageLocallyAsync(imageFile);

            // Update the product with the new image path
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound("Product not found.");

            product.ImagePath = imagePath;
            await _productRepository.UpdateProductAsync(product);

            return Ok(new { imagePath });
        }


        private async Task<string> SaveImageLocallyAsync(IFormFile imageFile)
        {
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
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


using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public class UpdateStockRequest
        {
            public int Stock { get; set; }
        }


        [HttpPut("update-stock/{productId}")]
        public async Task<IActionResult> UpdateStock(string productId, [FromBody] UpdateStockRequest request)
        {
            try
            {
                await _inventoryRepository.UpdateStockAsync(productId, request.Stock);
                return Ok(new { Message = "Stock updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error updating stock: {ex.Message}" });
            }
        }

        [HttpPut("remove-stock/{productId}")]
        public async Task<IActionResult> RemoveStock(string productId, [FromBody] UpdateStockRequest request)
        {
            try
            {
                await _inventoryRepository.RemoveStockAsync(productId, request.Stock);
                return Ok(new { Message = "Stock removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error removing stock: {ex.Message}" });
            }
        }

        [HttpGet("get-stock/{productId}")]
        public async Task<IActionResult> GetStockQuantity(string productId)
        {
            try
            {
                var quantity = await _inventoryRepository.GetStockQuantityAsync(productId);
                return Ok(new { ProductId = productId, StockQuantity = quantity });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error fetching stock quantity: {ex.Message}" });
            }
        }
    }
}

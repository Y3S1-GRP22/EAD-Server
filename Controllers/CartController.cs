
    using EAD.Models;
    using EAD.Repositories;
    using global::EAD.Models;
    using global::EAD.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    namespace EAD.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class CartController : ControllerBase
        {
            private readonly ICartRepository _cartRepository;

            public CartController(ICartRepository cartRepository)
            {
                _cartRepository = cartRepository;
            }

        // Get Cart for a User
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return NotFound(new { message = $"No cart found for the user with ID: {userId}" });
            }

            return Ok(cart);
        }


        // Add Item to Cart
        [HttpPost("{userId}/items")]
            public async Task<IActionResult> AddItemToCart(string userId, [FromBody] CartItem cartItem)
            {
                if (cartItem == null)
                    return BadRequest("Invalid item data.");

                await _cartRepository.AddItemToCartAsync(userId, cartItem);
                return Ok(cartItem);
            }

            // Update Item Quantity in Cart
            [HttpPut("{userId}/items/{itemId}")]
            public async Task<IActionResult> UpdateCartItemQuantity(string userId, string itemId, [FromBody] int quantity)
            {
                if (quantity <= 0)
                    return BadRequest("Quantity must be greater than 0.");

                await _cartRepository.UpdateCartItemQuantityAsync(userId, itemId, quantity);
                return Ok(new { message = "Item quantity updated successfully." });
            }

            // Remove Item from Cart
            [HttpDelete("{userId}/items/{itemId}")]
            public async Task<IActionResult> RemoveItemFromCart(string userId, string itemId)
            {
                await _cartRepository.RemoveItemFromCartAsync(userId, itemId);
                return Ok(new { message = "Item removed from cart successfully." });
            }

            // Clear Cart for a User
            [HttpDelete("{userId}/clear")]
            public async Task<IActionResult> ClearCart(string userId)
            {
                await _cartRepository.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared successfully." });
            }

            // Create Cart for a User
            [HttpPost("{userId}")]
            public async Task<IActionResult> CreateCart(string userId)
            {
                var cart = await _cartRepository.CreateCartAsync(userId);
                return Ok(cart);
            }

            // Delete Cart
            [HttpDelete("{userId}")]
            public async Task<IActionResult> DeleteCart(string userId)
            {
                await _cartRepository.DeleteCartAsync(userId);
                return Ok(new { message = "Cart deleted successfully." });
            }
        }
    }



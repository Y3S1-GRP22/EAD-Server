namespace EAD.Repositories

{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> _carts;

        public CartRepository(IMongoDatabase database)
        {
            _carts = database.GetCollection<Cart>("Carts");
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _carts.Find(cart => cart.UserId == userId && cart.Status == true)
                               .FirstOrDefaultAsync();
        }


        public async Task AddItemToCartAsync(string userId, CartItem newItem)
        {
            // Find all carts for the given user
            var carts = await _carts.Find(c => c.UserId == userId).ToListAsync();

            // Check if any cart is active (status is true)
            var activeCart = carts.FirstOrDefault(c => c.Status);

            // If no active cart exists, create a new one
            if (activeCart == null)
            {
                activeCart = new Cart { UserId = userId, Items = new List<CartItem>(), Status = true };
                await _carts.InsertOneAsync(activeCart);
            }

            // Update or add item to the active cart
            if (activeCart.Items.Any(i => i.ProductId == newItem.ProductId))
            {
                // Item already exists, update quantity
                var existingItem = activeCart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                // New item, ensure it has an Id
                if (string.IsNullOrEmpty(newItem.Id))
                {
                    newItem.Id = ObjectId.GenerateNewId().ToString();
                }

                // Add the item to the cart
                activeCart.Items.Add(newItem);
            }

            // Save the updated cart to the database
            await _carts.ReplaceOneAsync(c => c.UserId == userId && c.Status, activeCart);

            // Ensure the cart is not empty after adding the item
            if (activeCart.Items.Count == 0)
            {
                throw new InvalidOperationException("The cart cannot be empty after adding an item.");
            }
        }


        public async Task RemoveItemFromCartAsync(string userId, string itemId)
        {
            // Find the cart for the user where Status is true
            var cart = await _carts.Find(c => c.UserId == userId && c.Status == true).FirstOrDefaultAsync();

            if (cart == null) return; // If no active cart found, exit the method

            // Remove the item from the cart's item list
            cart.Items.RemoveAll(item => item.Id == itemId);

            // Update the cart in the database
            await _carts.ReplaceOneAsync(c => c.UserId == userId && c.Status == true, cart);
        }

        public async Task ClearCartAsync(string cartId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null) return;

            cart.Items.Clear();
            await _carts.ReplaceOneAsync(c => c.Id == cartId, cart);
        }

        public async Task UpdateCartItemQuantityAsync(string userId, string itemId, int quantity)
        {
            // Find the cart where the user's cart is active (Status == true)
            var cart = await _carts.Find(c => c.UserId == userId && c.Status == true).FirstOrDefaultAsync();

            if (cart == null) return; // If no active cart is found, exit the method

            // Find the specific item within the cart
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                // Update the quantity of the item
                item.Quantity = quantity;

                // Replace the updated cart in the database
                await _carts.ReplaceOneAsync(c => c.UserId == userId && c.Status == true, cart);
            }
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            await _carts.InsertOneAsync(cart);
            return cart;
        }

        public async Task DeleteCartAsync(string userId)
        {
            await _carts.DeleteOneAsync(cart => cart.UserId == userId);
        }

        // Update Cart (including Status)
        public async Task UpdateCartAsync(Cart updatedCart)
        {
            // Ensure the cart has the original _id before updating
            var existingCart = await _carts.Find(c => c.Id == updatedCart.Id).FirstOrDefaultAsync();
            if (existingCart != null)
            {
                // Update only the status or any other fields as needed
                existingCart.Status = updatedCart.Status; // Update the status field

                // Replace the entire cart document, preserving the original _id
                await _carts.ReplaceOneAsync(c => c.Id == existingCart.Id, existingCart);
            }
            else
            {
                throw new InvalidOperationException("Cart not found for update.");
            }
        }

        public async Task<Cart> GetCartByIdAsync(string cartId)
        {
            return await _carts.Find(cart => cart.Id == cartId).FirstOrDefaultAsync();
        }

        public async Task UpdateCartItemQuantityByCartIdAsync(string cartId, string itemId, int quantity)
        {
            // Find the cart by cartId
            var cart = await _carts.Find(c => c.Id == cartId).FirstOrDefaultAsync();

            if (cart == null) return; // If no cart is found, exit the method

            // Find the specific item within the cart
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                // Update the quantity of the item
                item.Quantity = quantity;

                // Replace the updated cart in the database
                await _carts.ReplaceOneAsync(c => c.Id == cartId, cart);
            }
        }

        public async Task RemoveItemFromCartByCartIdAsync(string cartId, string itemId)
        {
            // Find the cart by cartId
            var cart = await _carts.Find(c => c.Id == cartId).FirstOrDefaultAsync();

            if (cart == null) return; // If no cart is found, exit the method

            // Remove the item with the matching itemId from the cart
            cart.Items.RemoveAll(item => item.Id == itemId);

            // Replace the updated cart in the database
            await _carts.ReplaceOneAsync(c => c.Id == cartId, cart);
        }



    }
}
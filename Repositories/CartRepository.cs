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
                return await _carts.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();
            }

            public async Task AddItemToCartAsync(string userId, CartItem item)
            {
                var cart = await GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                    await _carts.InsertOneAsync(cart);
                }

                cart.Items.Add(item);
                await _carts.ReplaceOneAsync(c => c.UserId == userId, cart);
            }

            public async Task RemoveItemFromCartAsync(string userId, string itemId)
            {
                var cart = await GetCartByUserIdAsync(userId);
                if (cart == null) return;

                cart.Items.RemoveAll(item => item.Id == itemId);
                await _carts.ReplaceOneAsync(c => c.UserId == userId, cart);
            }

            public async Task ClearCartAsync(string userId)
            {
                var cart = await GetCartByUserIdAsync(userId);
                if (cart == null) return;

                cart.Items.Clear();
                await _carts.ReplaceOneAsync(c => c.UserId == userId, cart);
            }

            public async Task UpdateCartItemQuantityAsync(string userId, string itemId, int quantity)
            {
                var cart = await GetCartByUserIdAsync(userId);
                if (cart == null) return;

                var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    await _carts.ReplaceOneAsync(c => c.UserId == userId, cart);
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
        }
    }



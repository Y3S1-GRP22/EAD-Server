﻿namespace EAD.Repositories

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

        public async Task AddItemToCartAsync(string userId, CartItem newItem)
        {
            // Find the cart for the given user
            var cart = await GetCartByUserIdAsync(userId);

            // If the cart doesn't exist, create a new cart for the user
            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                await _carts.InsertOneAsync(cart);
            }

            // Check if the item already exists in the cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);

            if (existingItem != null)
            {
                // If the item already exists, update the quantity
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                // If the item doesn't exist, ensure that it has an Id
                if (string.IsNullOrEmpty(newItem.Id))
                {
                    newItem.Id = ObjectId.GenerateNewId().ToString(); // Generate a new ObjectId
                }

                // Add the item to the cart
                cart.Items.Add(newItem);
            }

            // Save the updated cart to the database
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


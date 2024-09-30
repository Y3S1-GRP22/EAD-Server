﻿﻿namespace EAD.Repositories;
using EAD.Models;
using global::EAD.Models;

public interface ICartRepository
{
    Task<Cart> GetCartByUserIdAsync(string userId);
    Task AddItemToCartAsync(string userId, CartItem item);
    Task RemoveItemFromCartAsync(string userId, string itemId);
    Task ClearCartAsync(string userId);
    Task UpdateCartItemQuantityAsync(string userId, string itemId, int quantity);
    Task<Cart> CreateCartAsync(string userId);
    Task DeleteCartAsync(string userId);
}
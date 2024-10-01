namespace EAD.Repositories
{
    using EAD.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderByIdAsync(string id);

        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);

        Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId);

        Task CreateOrderAsync(Order order);

        Task<Order> UpdateOrderAsync(string id, Order order);

        Task DeleteOrderAsync(string id);
    }
}

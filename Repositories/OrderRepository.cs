namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>("Orders");
        }

        // Get all orders
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }

        // Get order by ID
        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        // Get orders by Customer ID
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            return await _orders.Find(o => o.CustomerId == customerId).ToListAsync();
        }

        // Get orders by Vendor ID
        public async Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId)
        {
            return await _orders.Find(o => o.VendorId == vendorId).ToListAsync();
        }

        // Create a new order
        public async Task CreateOrderAsync(Order order)
        {
            Console.WriteLine("Order ID >|: " + order.Id);
            if (string.IsNullOrEmpty(order.Id))
            {
                order.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await _orders.InsertOneAsync(order);
            } else
            {
                Console.WriteLine("Order ID Sucess :  " + order.Id);
                await UpdateOrderAsync(order.Id,order);
            }


            
        }

        // Update an existing order
        public async Task<Order> UpdateOrderAsync(string id, Order order)
        {
            var result = await _orders.ReplaceOneAsync(o => o.Id == id, order);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Order not found for update.");
            }

            return order;
        }

        // Delete an order by ID
        public async Task DeleteOrderAsync(string id)
        {
            var result = await _orders.DeleteOneAsync(o => o.Id == id);

            if (result.DeletedCount == 0)
            {
                throw new Exception("Order not found for deletion.");
            }
        }

       

    }
}
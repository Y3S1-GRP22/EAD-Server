using EAD.Models;
using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EAD.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Get all orders
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Get order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }
            return Ok(order);
        }

        // Get orders by Customer ID
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(string customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        // Get orders by Vendor ID
        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetOrdersByVendorId(string vendorId)
        {
            var orders = await _orderRepository.GetOrdersByVendorIdAsync(vendorId);
            return Ok(orders);
        }

        // Create a new order
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null || string.IsNullOrEmpty(order.CustomerId)  || string.IsNullOrEmpty(order.Cart))
            {
                return BadRequest(new { message = "Customer ID, and Cart ID are required." });
            }

            try
            {
                await _orderRepository.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order." });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] Order order)
        {
            if (order == null || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(order.Cart))
            {
                return BadRequest(new { message = "Order ID and Cart ID are required." });
            }

            try
            {
                var updatedOrder = await _orderRepository.UpdateOrderAsync(id, order);
                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order." });
            }
        }

        // Delete an order
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            try
            {
                await _orderRepository.DeleteOrderAsync(id);
                return Ok(new { message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order." });
            }
        }

        // Update the status of an order
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrderStatus(string id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null || order.Id != id)
            {
                return NotFound(new { message = $"No order found  with ID: {id} " });
            }

            // Update the cart status
            order.Status = "Cancelled";
            await _orderRepository.UpdateOrderAsync(id,order);
            return Ok(new { message = "Order status updated successfully." });
        }

    }
}
﻿using EAD.Models;
using EAD.Repositories;
using EAD.Services;
using Microsoft.AspNetCore.Mvc;
using static EAD.Repositories.CustomerRepository;

namespace EAD.Controller;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICsrNotificationService _csrNotificationService;
    private readonly UserRepository _userRepository;

    public CustomerController(ICustomerRepository customerRepository, ICsrNotificationService csrNotificationService, UserRepository userRepository)
    {
        _customerRepository = customerRepository;
        _csrNotificationService = csrNotificationService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _customerRepository.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] Customer customer)
    {
        if (customer == null || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
        if (existingCustomer != null)
        {
            return Conflict(new { message = "Customer with this email already exists." });
        }

        try
        {
            await _customerRepository.RegisterCustomerAsync(customer);
            // Get all registered CSRs
            List<User> csrs = await _userRepository.GetAllCsrsAsync();
            List<string> csrEmails = csrs.Select(csr => csr.Email).ToList();

            // Notify all CSRs about the new customer registration
            await _csrNotificationService.NotifyCsrsAboutNewCustomerAsync(customer.Email, csrEmails);
            return CreatedAtAction(nameof(GetCustomerByEmail), new { email = customer.Email }, customer);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An unexpected error occurred during registration." });
        }
    }


    [HttpGet("{email}")]
    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, [FromBody] Customer customer)
    {
        if (customer == null || string.IsNullOrEmpty(id))
        {
            return BadRequest("Customer ID is required.");
        }

        // Ensure the ID is included in the customer object
        customer.Id = id;

        var updatedCustomer = await _customerRepository.UpdateCustomerAsync(id, customer);
        return Ok(updatedCustomer);
    }



    [HttpPut("activate/{email}")]
    public async Task<IActionResult> ActivateCustomer(string email)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound();
        }
        if (customer.IsActive)
        {
            return BadRequest(new { Email = email, Status = "Already Activated" }); // Account is already activated
        }

        await _customerRepository.ActivateCustomerAsync(email);
        return Ok(new { Email = email, Status = "Activated" });
    }

    [HttpPut("deactivate/{email}")]
    public async Task<IActionResult> DeactivateCustomer(string email)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound();
        }
        if (customer.IsActive == false)
        {
            return BadRequest(new { Email = email, Status = "Already Deactivated" }); // Account is already activated
        }

        await _customerRepository.DeactivateCustomerAsync(email);
        return Ok(new { Email = email, Status = "Deactivated" });
    }

    [HttpDelete("delete/{email}")]
    public async Task<IActionResult> DeleteCustomer(string email)
    {
        // Check if the customer exists
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound(new { Status = "Error", Message = "Customer not found" });
        }

        // Proceed to delete the customer
        await _customerRepository.DeleteCustomerAsync(email);
        return Ok(new { Status = "Success", Message = "Customer deleted", Email = email });
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginCustomer([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var loginResult = await _customerRepository.LoginCustomerAsync(loginRequest.Email, loginRequest.Password);

        switch (loginResult)
        {
            case LoginResult.Success:
                // Retrieve the customer details after successful login
                var customer = await _customerRepository.GetCustomerByEmailAsync(loginRequest.Email);
                return Ok(new
                {
                    Message = "Login successful.",
                    Customer = customer
                });
            case LoginResult.InvalidEmail:
                return NotFound(new { Message = "Invalid email address." });
            case LoginResult.IncorrectPassword:
                return Unauthorized(new { Message = "Incorrect password." });
            case LoginResult.AccountDeactivated:
                return Unauthorized(new { Message = "Account is deactivated." });
            default:
                return StatusCode(500, "An unexpected error occurred.");
        }
    }

}

using EAD.Models;
using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;
using static EAD.Repositories.CustomerRepository;

namespace EAD.Controller;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] Customer customer)
    {
        if (customer == null || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
        if (existingCustomer != null)
        {
            return Conflict("Customer with this email already exists.");
        }

        await _customerRepository.RegisterCustomerAsync(customer);
        return CreatedAtAction(nameof(GetCustomerByEmail), new { email = customer.Email }, customer);
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

        await _customerRepository.DeactivateCustomerAsync(email);
        return Ok(new { Email = email, Status = "Deactivated" });
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
                return Ok(new { Message = "Login successful." });
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

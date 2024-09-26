using System;
using System.Security.Claims;
using EAD.Models;
using EAD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            if (newUser == null ||
            string.IsNullOrEmpty(newUser.Username) ||
            string.IsNullOrEmpty(newUser.Email) ||
            string.IsNullOrEmpty(newUser.Password) ||
            string.IsNullOrEmpty(newUser.MobileNumber) ||
            string.IsNullOrEmpty(newUser.Address) ||
            string.IsNullOrEmpty(newUser.Role)
            )
            {
                return BadRequest("Invalid user data.");
            }

            if (newUser.Role != "Admin" && newUser.Role != "Vendor" && newUser.Role != "CSR")
            {
                return BadRequest("Invalid role. Must be Admin, Vendor, or CSR.");
            }

            try
            {
                await _userService.RegisterAsync(newUser);
                return Ok(new { Message = "Registration successful. Please log in." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userService.LoginAsync(model.Email, model.Password);
                return Ok(new { Token = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private string GetLoggedInUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");

            return Ok(user);
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var userEmail = GetLoggedInUserEmail();
            if (userEmail == null) return Unauthorized("You must be logged in to update your profile.");

            // Check if the user exists by ID
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");

            // Update user properties
            user.Username = updatedUser.Username;
            user.MobileNumber = updatedUser.MobileNumber;
            user.Address = updatedUser.Address;

            // Hash password only if it's provided and not empty
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                user.Password = PasswordHasher.HashPassword(updatedUser.Password);
            }

            // Attempt to update the user
            var result = await _userService.UpdateUserAsync(user);
            if (!result) return BadRequest("Failed to update user");

            return Ok("User updated successfully");
        }


        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userEmail = GetLoggedInUserEmail();
            if (userEmail == null) return Unauthorized("You must be logged in to delete your profile.");
            // Check if the provided ID corresponds to a valid user
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");

            // Perform the deletion
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return BadRequest("Failed to delete user");

            return Ok("User deleted successfully");
        }


    }
}
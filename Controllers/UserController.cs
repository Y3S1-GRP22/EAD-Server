using System;
using EAD.Models;
using EAD.Services;
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
            // if (newUser == null || string.IsNullOrEmpty(newUser.Email) || string.IsNullOrEmpty(newUser.Password))
            // {
            //     return BadRequest("Invalid user data.");
            // }

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

    }
}
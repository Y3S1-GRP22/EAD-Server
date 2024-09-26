using System;
using EAD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _userService.GetAllAdminsAsync();
            return Ok(admins);
        }

        [HttpGet("vendors")]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _userService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        [HttpGet("csrs")]
        public async Task<IActionResult> GetAllCsrs()
        {
            var csrs = await _userService.GetAllCsrsAsync();
            return Ok(csrs);
        }

        // Endpoint to activate a user
        [HttpPatch("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var result = await _userService.ActivateUserAsync(userId);
            if (result)
            {
                return Ok(new { message = "User account activated successfully" });
            }
            return NotFound(new { message = "User not found" });
        }

        // Endpoint to deactivate a user
        [HttpPatch("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var result = await _userService.DeactivateUserAsync(userId);
            if (result)
            {
                return Ok(new { message = "User account deactivated successfully" });
            }
            return NotFound(new { message = "User not found" });
        }


    }
}

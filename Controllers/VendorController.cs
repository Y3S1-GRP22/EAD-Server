using System.Threading.Tasks;
using EAD.Models;
using EAD.Services;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    [ApiController]
    [Route("api/vendors")]
    public class VendorController : ControllerBase
    {
        private readonly VendorService _vendorService;

        public VendorController(VendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // Add ranking for a vendor
        [HttpPost("{vendorId}/rank")]
        public async Task<IActionResult> AddRanking(string vendorId, [FromBody] AddRankingModel model)
        {
            var result = await _vendorService.AddRankingAsync(model.CustomerId, vendorId, model.Score);
            if (!result)
            {
                return BadRequest(new { message = "Ranking cannot be modified once submitted." });
            }
            return Ok(new { message = "Ranking added successfully." });
        }

        // Add or update comment for a vendor
        [HttpPost("{vendorId}/comment")]
        public async Task<IActionResult> AddOrUpdateComment(string vendorId, [FromBody] AddCommentModel model)
        {
            await _vendorService.AddOrUpdateCommentAsync(model.CustomerId, vendorId, model.Comment);
            return Ok(new { message = "Comment added or updated successfully." });
        }

        // Get average ranking for a vendor
        [HttpGet("{vendorId}/average-ranking")]
        public async Task<IActionResult> GetAverageRanking(string vendorId)
        {
            var averageRanking = await _vendorService.GetAverageRankingForVendorAsync(vendorId);
            return Ok(new { averageRanking });
        }

        [HttpGet("{vendorId}/comments/{customerId}")]
        public async Task<IActionResult> GetCommentsByCustomer(string vendorId, string customerId)
        {
            var comment = await _vendorService.GetCommentAsync(customerId, vendorId);
            if (comment == null)
            {
                return NotFound(new { message = "Comment not found." });
            }
            return Ok(comment);
        }
    }
}

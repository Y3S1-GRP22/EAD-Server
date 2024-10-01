using EAD.Models;
using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet("product/{productId}/rating")]
        public async Task<ActionResult<double?>> GetAverageRatingByProductId(string productId)
        {
            var averageRating = await _commentRepository.GetAverageRatingByProductId(productId);

            if (averageRating == null)
            {
                return NotFound("No ratings available for this product.");
            }

            return Ok(averageRating);
        }

        // GET: api/comment/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByProductId(string productId)
        {
            var comments = await _commentRepository.GetCommentsByProductId(productId);
            return Ok(comments);
        }

        // GET: api/comment/vendor/{vendorId}
        [HttpGet("vendor/{vendorId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByVendorId(string vendorId)
        {
            var comments = await _commentRepository.GetCommentsByVendorId(vendorId);
            return Ok(comments);
        }

        // POST: api/comment
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment([FromBody] Comment comment)
        {
            var newComment = await _commentRepository.AddComment(comment);
            return CreatedAtAction(nameof(GetCommentsByProductId), new { productId = newComment.ProductId }, newComment);
        }

        // DELETE: api/comment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var deleted = await _commentRepository.DeleteComment(id);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }



        // GET: api/comment/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByUserId(string userId)
        {
            var comments = await _commentRepository.GetCommentsByUserId(userId);
            return Ok(comments);
        }

        

    }
}

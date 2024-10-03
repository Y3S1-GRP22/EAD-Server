using MongoDB.Driver;
using EAD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace EAD.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentRepository(IMongoDatabase database)
        {
            _comments = database.GetCollection<Comment>("Comments");
        }

        // Fetch comments by Product ID
        public async Task<IEnumerable<Comment>> GetCommentsByProductId(string productId)
        {
            return await _comments.Find(comment => comment.ProductId == productId).ToListAsync();
        }

        // Add a new comment
        public async Task<Comment> AddComment(Comment comment)
        {
            await _comments.InsertOneAsync(comment);
            return comment;
        }

        // Delete a comment by ID
        public async Task<bool> DeleteComment(string commentId)
        {
            var result = await _comments.DeleteOneAsync(comment => comment.Id == commentId);
            return result.DeletedCount > 0;
        }

        // Fetch comments by Vendor ID
        public async Task<IEnumerable<Comment>> GetCommentsByVendorId(string vendorId)
        {
            return await _comments.Find(comment => comment.VendorId == vendorId).ToListAsync();
        }


        // Add the new method in CommentRepository to retrieve comments by UserId
        public async Task<IEnumerable<Comment>> GetCommentsByUserId(string userId)
        {
            return await _comments.Find(comment => comment.UserId == userId).ToListAsync();
        }

        public async Task<double?> GetAverageRatingByProductId(string productId)
        {
            var comments = await _comments.Find(comment => comment.ProductId == productId).ToListAsync();

            // Check if there are any comments for the product
            if (comments.Count == 0)
            {
                return null; // No ratings available
            }

            // Calculate the average rating
            double averageRating = (double)comments.Average(comment => comment.Rating);

            return averageRating;
        }
    }

}


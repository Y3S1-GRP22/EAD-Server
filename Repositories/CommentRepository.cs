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
    }
}

using EAD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByProductId(string productId);
        Task<Comment> AddComment(Comment comment);
        Task<bool> DeleteComment(string commentId);
        Task<IEnumerable<Comment>> GetCommentsByVendorId(string vendorId);

        Task<IEnumerable<Comment>> GetCommentsByUserId(string userId);

        Task<double?> GetAverageRatingByProductId(string productId);

    }
}

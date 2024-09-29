using System.Threading.Tasks;
using EAD.Models;
using MongoDB.Driver;

namespace EAD.Repositories
{
    public class VendorRepository
    {
        private readonly IMongoCollection<VendorRanking> _ranking;
        private readonly IMongoCollection<VendorComment> _comment;

        public VendorRepository(IMongoDatabase database)
        {
            _ranking = database.GetCollection<VendorRanking>("Rankings");
            _comment = database.GetCollection<VendorComment>("Comments");
        }

        public async Task AddRankingAsync(VendorRanking ranking)
        {
            await _ranking.InsertOneAsync(ranking);
        }

        public async Task<VendorRanking?> GetRankingAsync(string customerId, string vendorId)
        {
            var ranking = await _ranking.Find(r => r.CustomerId == customerId && r.VendorId == vendorId).FirstOrDefaultAsync();
            if (ranking == null)
            {
                return null;
            }
            return ranking;
        }

        public async Task<double> GetVendorScoreAsync(string vendorId)
        {
            var ranking = await _ranking.Find(r => r.VendorId == vendorId).ToListAsync();

            if (ranking.Count == 0) return 0;

            return ranking.Average(r => r.Score);
        }

        public async Task AddOrUpdateCommentAsync(VendorComment comment)
        {
            var existingComment = await _comment.Find(c => c.CustomerId == comment.CustomerId && c.VendorId == comment.VendorId).FirstOrDefaultAsync();

            if (existingComment == null)
            {
                await _comment.InsertOneAsync(comment);
            }
            else
            {
                existingComment.Comment = comment.Comment;
                existingComment.UpdatedAt = DateTime.Now;
                await _comment.ReplaceOneAsync(c => c.Id == existingComment.Id, existingComment);

            }
        }
        public async Task<VendorComment?> GetCommentAsync(string customerId, string vendorId)
        {
            var comment = await _comment.Find(r => r.CustomerId == customerId && r.VendorId == vendorId).FirstOrDefaultAsync();
            if (comment == null)
            {
                return null;
            }
            return comment;
        }


    }
}
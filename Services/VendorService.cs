using System.Threading.Tasks;
using EAD.Models;
using EAD.Repositories;

namespace EAD.Services
{
    public class VendorService
    {
        private readonly VendorRepository _repository;

        public VendorService(VendorRepository repository)
        {
            _repository = repository;
        }

        // Add ranking for a vendor
        public async Task<bool> AddRankingAsync(string customerId, string vendorId, int score)
        {
            var existingRanking = await _repository.GetRankingAsync(customerId, vendorId);
            if (existingRanking != null)
            {
                return false; // Ranking cannot be modified once created
            }

            var ranking = new VendorRanking
            {
                CustomerId = customerId,
                VendorId = vendorId,
                Score = score
            };

            await _repository.AddRankingAsync(ranking);
            return true;
        }

        // Add or update a comment for a vendor
        public async Task<bool> AddOrUpdateCommentAsync(string customerId, string vendorId, string comment)
        {
            var CustomerComment = new VendorComment
            {
                CustomerId = customerId,
                VendorId = vendorId,
                Comment = comment
            };

            await _repository.AddOrUpdateCommentAsync(CustomerComment);
            return true;
        }

        // Get average ranking for a vendor
        public async Task<double> GetAverageRankingForVendorAsync(string vendorId)
        {
            return await _repository.GetVendorScoreAsync(vendorId);
        }

        // Get comment for a specific customer and vendor
        public async Task<VendorComment?> GetCommentAsync(string customerId, string vendorId)
        {
            return await _repository.GetCommentAsync(customerId, vendorId);
        }
    }
}

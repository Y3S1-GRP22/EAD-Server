using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD.Models
{
    public class VendorRanking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string VendorId { get; set; }
        public string CustomerId { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
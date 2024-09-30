using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD.Models
{

    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string VendorId { get; set; }

        public int? Rating { get; set; }
        public string? Comments { get; set; }
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD.Models
{
    public class VendorComment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string VendorId { get; set; }
        public string CustomerId { get; set; }
        public string Comment { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp when the comment is created

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdatedAt { get; set; }
    }
}
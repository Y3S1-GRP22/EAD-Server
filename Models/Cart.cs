using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD.Models
{

    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; }
    }
}

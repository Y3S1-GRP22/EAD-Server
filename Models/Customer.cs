namespace EAD.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Email { get; set; } 

    public string Password { get; set; } 

    public string FullName { get; set; }

    public int MobileNumber { get; set; } 

    public string Address { get; set; } 

    public bool IsActive { get; set; } 

}

using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }

	[Required]
	public string Username { get; set; }

	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	public string Password { get; set; }

	[Required]
	public string MobileNumber { get; set; }

	[Required]
	public string Address { get; set; }

	[Required]
	public string Role { get; set; }

	public bool IsActive { get; set; } = true;
}

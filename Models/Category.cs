﻿namespace EAD.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}


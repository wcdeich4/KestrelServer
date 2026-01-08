using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// C# and F# support records which are compared by value and entirely immutable by default.
/// VB.NET does not support records without a NuGet package to convert a struct to a record, 
/// therefore this record typd is here.
/// </summary>
/// <param name="Name">The primary way to make a record constructor is with "Positional 
/// Record Syntax" where you only give the name and type and the properties are automaticaly
/// created as init-only properties.
/// </param>
public record FileRecord(string Name)
{
    /// <summary>
    /// ObjectId used by MongoDB
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    /// <summary>
    /// However, if you need mutable properties, you can define them like this.
    /// </summary>
    public string? Contents { get; set; } = null;
}
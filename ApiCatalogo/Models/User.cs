using Api.Enumerator;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
  public class User
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Position { get; set; }
    public string Name { get; set; }
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; }
    public bool IsConnected { get; set; } = false;
    public bool IsInBlackList { get; set; } = false;
    public DateTime? BirthDate { get; set; } = null;
    public DateTime LastModified { get; set; } = DateTime.Now;
    public DateTime RegistrerDate { get; set; } = DateTime.Now;
    public ESexo Genre { get; set; } = ESexo.Homem;
    public BsonArray Pictures { get; set; } = new BsonArray(new List<string>());

    public User()
    {
    }
  }

  public class UserDTO
  {
    public string Id { get; set; }

    [Required]    [MaxLength(100)]
    public string Name { get; set; }

    [Required]    [MinLength(8)]
    public string Password { get; set; }

    public string BirthDate { get; set; }

    public ESexo Genre { get; set; }

    [Required]
    public string Email { get; set; }

    public bool IsConnected { get; set; }

    public PositionClass Position { get; set; }

    public class PositionClass
    {
      public double X { get; set; }

      public double Y { get; set; }
    }
  }
}

using Api.Enumerator;
using Api.Models;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Repositórios
{
  public interface IRepositoryUser
  {
    Task<User> CreateOrUpdateAsync(User source, EType type);

    Task<User> GetByIdAsync(string id);

    Task<bool> DeleteAsync(string id);

    Task<List<User>> GetAllAsync();

    Task<Tuple<User, EStatusLogin>> Autorize(string email, string password);

    Task<List<User>> GetAllByDistance(GeoJsonPoint<GeoJson2DGeographicCoordinates> Position, long meters);

    EStatusRegistrer CheckRegistrer(UserDTO user);

    Task<bool> Disconnect(string id);
  }
}

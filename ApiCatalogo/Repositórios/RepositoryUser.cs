using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Enumerator;
using Api.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Api.Repositórios
{
  public class RepositoryUser : IRepositoryUser
  {

    private readonly DBContext _context;

    public RepositoryUser(DBContext context)
    {
      _context = context;
    }

    public async Task<Tuple<User,EStatusLogin>> Autorize(string email, string password)
    {
      var filter = Builders<User>.Filter
          .And(
          Builders<User>.Filter.Eq(x => x.Email, email));//Builders<User>.Filter.Eq(x => x.Password, password)

      var existUser = await _context.GetByFilter(filter);

      if(existUser.Count > 0 )
      {
        filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.Email, email), Builders<User>.Filter.Eq(x => x.Password, password));

        var exist = await _context.GetByFilter(filter);

        if(exist.Count > 0)
        {
          if(exist.ElementAtOrDefault(0).IsConnected)
            return new Tuple<User, EStatusLogin>(exist.ElementAtOrDefault(0), EStatusLogin.ALREADY_CONNECTED);
          return new Tuple<User, EStatusLogin>(exist.ElementAtOrDefault(0), EStatusLogin.OK);
        }
        return new Tuple<User, EStatusLogin>(null, EStatusLogin.INVALID_PASSWORD);

      }
      return new Tuple<User, EStatusLogin>(null, EStatusLogin.INVALID_USER);
    }

    public async Task<User> CreateOrUpdateAsync(User source, EType type)
    {
      try
      {
        if (type == EType.Save)
        {
          var post = await _context.PostAsync<User>(source);
          return post;
        }
        else
        {
          var put = await _context.PutAsync<User>(source, source.id);
          return put;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<bool> DeleteAsync(string id)
    {
      var exclude = await _context.DeleteAsync<User>(id);
      return exclude;
    }

    public async Task<List<User>> GetAllAsync()
    {
      var lst = await _context.GetAll<User>();

      return lst;
    }

    public async Task<List<User>> GetAllByDistance(GeoJsonPoint<GeoJson2DGeographicCoordinates> Position, long meters)
    {
      var filter = Builders<User>.Filter
          .NearSphere(
          x => x.Position,
          Position,
          meters);

      var lst = await _context.GetByFilter(filter);

      return lst;
    }

    public async Task<User> GetByIdAsync(string id)
    {
      var item = await _context.GetByIdAsync<User>(id);
      return item;
    }

    public async Task<bool> Disconnect(string id)
    {
      try
      {
        var user = await this.GetByIdAsync(id);
        user.IsConnected = false;
        await CreateOrUpdateAsync(user, EType.Update);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public EStatusRegistrer CheckRegistrer(UserDTO user)
    {
      var filter = Builders<User>.Filter
          .Eq(x => x.Email, user.Email);

      var emailOk = _context.GetByFilter(filter).Result.Count == 0;

      if (!emailOk)
        return EStatusRegistrer.EMAIL_EXIST;

      return EStatusRegistrer.OK;
    }
  }
}

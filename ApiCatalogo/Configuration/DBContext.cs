using System.Linq;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Api
{
  public class DBContext
  {
    private IConfiguration _configuration;
    private IMongoDatabase db;

    public DBContext(IConfiguration config)
    {
      _configuration = config;
      var mongoClient = new MongoClient(_configuration.GetConnectionString("ConexaoCatalogo"));
      db = mongoClient.GetDatabase("mybe");

      bool isAlive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

      if (!isAlive)
        throw new Exception();
    }

    public async Task<T> GetByIdAsync<T>(string id) where T : class
    {
      if (!id.Length.Equals(24))
        return default(T);

      var filter = Builders<T>.Filter.Eq("id", id);

      var x = await db.GetCollection<T>(typeof(T).Name)
          .Find(filter).FirstOrDefaultAsync();
      return x;
    }

    public async Task<List<T>> GetAll<T>() where T : class
    {
      var lst = await db.GetCollection<T>(typeof(T).Name).FindAsync("{}");
      return lst.ToList();
    }

    public async Task<List<T>> GetByFilter<T>(FilterDefinition<T> filter) where T : class
    {
      var lst = await db.GetCollection<T>(typeof(T).Name).FindAsync(filter);
      return lst.ToList();
    }

    public async Task<T> PostAsync<T>(T source) where T : class
    {
      var collection = db.GetCollection<T>(typeof(T).Name);

      try
      {
        await collection.InsertOneAsync(source);

        return source;
      }
      catch (Exception e)
      {
        throw e;
      }

    }

    public async Task<T> PutAsync<T>(T source, string id) where T : class
    {
      var filter = Builders<T>.Filter.Eq("id", id);

      await db.GetCollection<T>(typeof(T).Name).ReplaceOneAsync(filter, source);
      return source;
    }

    public async Task<bool> DeleteAsync<T>(string id) where T : class
    {
      try
      {
        var filter = Builders<T>.Filter.Eq("id", id);
        await db.GetCollection<T>(typeof(T).Name).DeleteOneAsync(filter);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
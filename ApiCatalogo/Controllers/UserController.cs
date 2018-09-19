using Api.Enumerator;
using Api.Repositórios;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Threading.Tasks;
using System;
using Api.Utils;
using System.Collections.Generic;

namespace Api.Controllers
{
  [Route("[controller]")]
  //[Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Authorize(Policy = "Over18")]
  public class UserController : Controller
  {
    private readonly IRepositoryUser _repositoryUser;
    public UserController(IRepositoryUser repositoryUser)
    {
      _repositoryUser = repositoryUser;
    }

    [HttpGet("")]
    public async Task<ObjectResult> Get()
    {
      var lst = await _repositoryUser.GetAllAsync();
      
      return new ObjectResult(lst.ToDTO());
    }

    [HttpPost("disconnect/{id}")]
    public async Task<IActionResult> DisconnectUser(string id)
    {
      var setDisconnect = await _repositoryUser.Disconnect(id);
      if (setDisconnect)
        return Ok();
      return BadRequest();
    }

    [HttpGet("getone/{id}")]
    public async Task<IActionResult> GetOne(string id)
    {
      var user = await _repositoryUser.GetByIdAsync(id);
      if (user != null)
        return new ObjectResult(user.ToDTO());
      return NotFound();

    }

    [HttpGet("getbyradius/{id}")]
    public async Task<IActionResult> GetProximos(string id, long distance = 50)
    {
      var item = await _repositoryUser.GetByIdAsync(id);
      if (item != null && item.Position != null)
      {
        var lst = await _repositoryUser.GetAllByDistance(item.Position, distance);
        return new ObjectResult(lst.ToDTO());
      }
      return NotFound();

    }
    //[HttpPut]
    //public async Task<IActionResult> Put([FromBody]UserDTO user,string id)
    //{
    //  if (user.Name != string.Empty && user.Password != string.Empty)
    //  {
    //    User userPut = new User()
    //    {
    //      Name = user.Name,
    //      Password = user.Password,
    //      _id = id,
    //      Position = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(user.X, user.Y))
    //    };

    //    var newUser = await _repositoryUser.CreateOrUpdateAsync(userPut, EType.Update);

    //    return new ObjectResult(newUser);
    //  }
    //  return BadRequest(new { msg = "Invalid Object, please check the null values"});
    //}

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      var delete = await _repositoryUser.DeleteAsync(id);
      if (delete)
        return Ok();
      else
        return BadRequest();
    }
  }
}
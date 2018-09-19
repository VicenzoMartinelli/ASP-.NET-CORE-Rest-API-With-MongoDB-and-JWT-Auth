using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Api.Repositórios;
using MongoDB.Driver.GeoJsonObjectModel;
using Api.Enumerator;
using Api.Configuration;
using AutoMapper;
using System.Collections.Generic;
using Api.Utils;

namespace Api.Controllers
{
  [Route("[controller]")]
  public class LoginController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly IRepositoryUser _repositoryUser;
    private readonly IMapper _mapper;

    public LoginController(IConfiguration configuration, IRepositoryUser repositoryUser, IMapper mapper)
    {
      _configuration = configuration;
      _repositoryUser = repositoryUser;
      _mapper = mapper;
    }

    private JwtSecurityToken GetToken(string name)
    {
      var claims = new[]
              {
            new Claim(ClaimTypes.Name, name)
        };

      //recebe uma instancia da classe SymmetricSecurityKey 
      //armazenando a chave de criptografia usada na criação do token
      var key = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));

      //recebe um objeto do tipo SigninCredentials contendo a chave de 
      //criptografia e o algoritmo de segurança empregados na geração 
      // de assinaturas digitais para tokens
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
           issuer: "MybeApi",
           audience: "MybeApi",
           claims: claims,
           expires: DateTime.Now.AddYears(2),
           signingCredentials: creds);

      return token;
    }

    [AllowAnonymous]
    [HttpPost()]
    public async Task<IActionResult> Login([FromBody] UserDTO request)
    {
      Tuple<User,EStatusLogin> userRet = await _repositoryUser.Autorize(request.Email, request.Password);

      if ( userRet.Item1 != null)
      {
        var token = GetToken(userRet.Item1.Name);

        return Ok(new
        {
          token = new JwtSecurityTokenHandler().WriteToken(token),
          User = userRet.Item1.ToDTO()
        });
      }

      return BadRequest(
        new { Error = userRet.Item2.ToInt()}
        );
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody]UserDTO user)
    {
      if (ModelState.IsValid)
      {
        User userP          = _mapper.Map<UserDTO, User>(user);
        userP.LastModified  = DateTime.Now;
        userP.IsConnected    = true;
        userP.RegistrerDate = DateTime.Now;
        userP.Position      = user.Position != null ? new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(user.Position.X,user.Position.Y)) : null;
        DateTime birth      = DateTime.MinValue;
        userP.BirthDate     = DateTime.TryParse(user.BirthDate,out birth) ? (DateTime?)birth : null;

        var check          =  _repositoryUser.CheckRegistrer(user);

        if (check != EStatusRegistrer.OK)
          return BadRequest( 
            new { Error = check.ToInt()
            });

        var newUser        = await _repositoryUser.CreateOrUpdateAsync(userP, EType.Save);
       
        var token = GetToken(userP.Name);

        return new ObjectResult(new {
          User = userP.ToDTO(),
          Token = new JwtSecurityTokenHandler().WriteToken(token)
        });
      }

      return BadRequest(new
      {
        CountErrors = ModelState.ErrorCount,
        Errors = ModelState.GetAllErrors(),
      });
    }
  }
}

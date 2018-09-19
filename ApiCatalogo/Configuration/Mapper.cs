using Api.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Configuration
{
  public class Mapper : Profile
  {
    public Mapper()
    {
      // Add as many of these lines as you need to map your objects
      CreateMap<User, UserDTO>();
      CreateMap<UserDTO, User>();
    }
  }
}

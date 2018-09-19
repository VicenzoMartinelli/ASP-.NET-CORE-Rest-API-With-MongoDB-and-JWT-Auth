using Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Utils
{
  public static class MyPrototypes
  {
    public static IEnumerable<Error> GetAllErrors(this ModelStateDictionary modelState)
    {
      var result = new List<Error>();
      var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any())
                                      .Select(x => new { x.Key, x.Value.Errors });

      foreach (var erroneousField in erroneousFields)
      {
        var fieldKey = erroneousField.Key;
        var fieldErrors = erroneousField.Errors
                           .Select(error => new Error(fieldKey, error.ErrorMessage));
        result.AddRange(fieldErrors);
      }

      return result;
    }

    public static int ToInt(this Enum status)
    {
      return Convert.ToInt32(status);
    }

    public static UserDTO ToDTO(this User user)
    {
      return AutoMapper.Mapper.Map<User, UserDTO>(user);
    }

    public static List<UserDTO> ToDTO(this List<User> user)
    {
      var lst = new List<UserDTO>();
      user.ForEach(x => lst.Add(x.ToDTO()));

      return lst;
    }

  }
}

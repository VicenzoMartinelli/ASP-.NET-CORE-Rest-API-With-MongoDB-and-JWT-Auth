using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enumerator
{
  public enum EStatusLogin
  {
    OK                = 200,
    INVALID_USER      = 201,
    INVALID_PASSWORD  = 202,
    ALREADY_CONNECTED = 203
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IAuthenticationConfiguration
    {
        string SystemUserPassword { get; }
        string SystemUserName { get; }
        string UserClass { get; }
        bool Caching { get; set; }
        Type GetUserType();

    }
}

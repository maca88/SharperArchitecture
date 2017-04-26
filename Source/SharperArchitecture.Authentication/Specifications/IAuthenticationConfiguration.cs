using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Authentication.Specifications
{
    public interface IAuthenticationConfiguration
    {
        string SystemUserPassword { get; }
        string SystemUserName { get; }
        bool Caching { get; set; }
    }
}

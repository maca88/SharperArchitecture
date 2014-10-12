using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace PowerArhitecture.Common.Specifications
{
    public interface IRole : IRole<long>
    {
        string Description { get; set; }
    }
}

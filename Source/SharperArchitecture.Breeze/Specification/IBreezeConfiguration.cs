using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Breeze.Specification
{
    public interface IBreezeConfiguration
    {
        string DataServiceNamesBaseUri { get; }
    }
}

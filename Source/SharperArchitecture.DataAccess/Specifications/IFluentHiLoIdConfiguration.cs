using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IFluentHiLoIdConfiguration
    {
        IFluentHiLoIdConfiguration Enabled(bool value = true);
        IFluentHiLoIdConfiguration TableName(string value);
        IFluentHiLoIdConfiguration MaxLo(int value);
    }
}

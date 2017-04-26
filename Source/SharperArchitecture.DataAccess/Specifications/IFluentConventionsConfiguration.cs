using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IFluentConventionsConfiguration
    {
        IFluentConventionsConfiguration IdDescending(bool value = true);
        IFluentConventionsConfiguration UniqueWithMultipleNulls(bool value = true);
        IFluentConventionsConfiguration RequiredLastModifiedProperty(bool value = true);
        IFluentConventionsConfiguration HiLoId(Action<IFluentHiLoIdConfiguration> action);
        IFluentConventionsConfiguration DateTimeZone(DateTimeKind kind);
    }
}

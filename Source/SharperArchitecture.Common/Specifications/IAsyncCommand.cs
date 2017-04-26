using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Specifications
{
    public interface IAsyncCommand
    {
    }

    public interface IAsyncCommand<out TResult>
    {
    }
}

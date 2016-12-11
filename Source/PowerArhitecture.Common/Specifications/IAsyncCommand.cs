using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Specifications
{
    public interface IAsyncCommand
    {
    }

    public interface IAsyncCommand<out TResult>
    {
    }
}

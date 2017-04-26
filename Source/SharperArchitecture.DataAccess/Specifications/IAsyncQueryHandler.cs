using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IAsyncQueryHandler<in TQuery, TResult> where TQuery : IAsyncQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}

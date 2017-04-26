using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IQueryProcessor
    {
        TResult Process<TResult>(IQuery<TResult> query);

        Task<TResult> ProcessAsync<TResult>(IAsyncQuery<TResult> query);
    }
}

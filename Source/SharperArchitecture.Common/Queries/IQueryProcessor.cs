using System.Threading;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Queries
{
    public interface IQueryProcessor
    {
        TResult Process<TResult>(IQuery<TResult> query);

        Task<TResult> ProcessAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace PowerArhitecture.Common.Specifications
{
    public interface ICommand : IRequest, ICancellableAsyncRequest
    {
    }

    public interface ICommand<out TResult> : IRequest<TResult>, ICancellableAsyncRequest<TResult>
    {
    }
}

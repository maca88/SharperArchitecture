using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace PowerArhitecture.Common.Specifications
{
    public interface ICommandHandler<in TCommand> :
        IRequestHandler<TCommand, Unit>,
        ICancellableAsyncRequestHandler<TCommand, Unit>
        where TCommand : ICommand
    {

    }

    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, TResult>,
        ICancellableAsyncRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {

    }
}

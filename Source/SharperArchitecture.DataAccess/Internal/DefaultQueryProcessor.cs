using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.DataAccess.Specifications;
using SimpleInjector;

namespace SharperArchitecture.DataAccess.Internal
{
    internal class DefaultQueryProcessor : IQueryProcessor
    {
        private readonly Container _container;

        public DefaultQueryProcessor(Container container)
        {
            _container = container;
        }

        public TResult Process<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _container.GetInstance(handlerType);
            return handler.Handle((dynamic)query);
        }

        public Task<TResult> ProcessAsync<TResult>(IAsyncQuery<TResult> query)
        {
            var handlerType = typeof(IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _container.GetInstance(handlerType);
            return handler.HandleAsync((dynamic)query);
        }
    }
}

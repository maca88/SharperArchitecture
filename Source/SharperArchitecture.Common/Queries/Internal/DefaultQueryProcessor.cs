using System.Threading.Tasks;
using SimpleInjector;

namespace SharperArchitecture.Common.Queries.Internal
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

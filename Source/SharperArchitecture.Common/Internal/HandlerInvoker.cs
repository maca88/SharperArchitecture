using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Internal
{
    internal class HandlerInvoker
    {
        protected readonly ConcurrentDictionary<Type, HandlerInfo> CachedHandlers = new ConcurrentDictionary<Type, HandlerInfo>();
        protected readonly ConcurrentDictionary<Type, HandlerInfo> CachedAsyncHandlers = new ConcurrentDictionary<Type, HandlerInfo>();

        internal class HandlerInfo
        {
            public HandlerInfo(Type type, Func<Type, object> createHandleInvoker)
            {
                Type = type;
                HandleInvoker = createHandleInvoker(type);
            }

            public Type Type { get; }

            public dynamic HandleInvoker { get; }
        }

        protected static TResult CreateHandlerInvoker<TResult>(Type eventType, Type handlerType, string methodName, params ParameterExpression[] parameters)
        {
            var param1 = Expression.Parameter(typeof(object), "handler");
            var param2 = Expression.Parameter(typeof(object), "obj");
            var convertHandler = Expression.Convert(param1, handlerType);
            var convertCommand = Expression.Convert(param2, eventType);
            var callParams = new List<Expression>
            {
                convertCommand
            }.Concat(parameters);
            var lambdaParams = new List<ParameterExpression>
            {
                param1,
                param2
            }.Concat(parameters);
            var callMethod = Expression.Call(convertHandler, handlerType.GetMethod(methodName), callParams);
            return Expression.Lambda<TResult>(callMethod, lambdaParams).Compile();
        }
    }
}

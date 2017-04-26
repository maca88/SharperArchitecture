using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Intercept;
using NHibernate.Proxy;
using NHibernate.Proxy.DynamicProxy;
using SharperArchitecture.Domain;

namespace SharperArchitecture.DataAccess.Extensions
{
    public static class EntityExtensions
    {
        public static Type GetTypeUnproxied(this IEntity entity)
        {
            var nhProxy = entity as IProxy;
            if (nhProxy == null) return entity.GetType();

            var lazyInitializer = nhProxy.Interceptor as ILazyInitializer;
            if (lazyInitializer != null)
                return lazyInitializer.PersistentClass;

            var fieldInterceptorAccessor = nhProxy.Interceptor as IFieldInterceptorAccessor;
            if (fieldInterceptorAccessor != null)
            {
                return fieldInterceptorAccessor.FieldInterceptor == null
                    ? entity.GetType().BaseType
                    : fieldInterceptorAccessor.FieldInterceptor.MappedClass;
            }
            return entity.GetType();
        }
    }
}

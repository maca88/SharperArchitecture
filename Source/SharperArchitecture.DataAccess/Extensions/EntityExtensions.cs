using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
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
            if (!(entity is IProxy proxy))
            {
                return NHibernateUtil.GetClass(entity); // INHibernateProxy
            }

            switch (proxy.Interceptor)
            {
                case ILazyInitializer lazyInitializer:
                    return lazyInitializer.PersistentClass;
                case IFieldInterceptorAccessor fieldInterceptorAccessor:
                    return fieldInterceptorAccessor.FieldInterceptor == null
                        ? entity.GetType().BaseType
                        : fieldInterceptorAccessor.FieldInterceptor.MappedClass;
            }

            return entity.GetType();
        }
    }
}

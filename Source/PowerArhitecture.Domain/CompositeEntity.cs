using System;
using NHibernate.Intercept;
using NHibernate.Proxy;
using NHibernate.Proxy.DynamicProxy;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.Domain
{
    public interface ICompositeEntity : IEntity
    {
        ICompositeKey GetCompositeKey();
    }

    [Ignore]
    public abstract partial class CompositeEntity : ICompositeEntity
    {
        private volatile ICompositeKey _cachedKey;
        private readonly object _lock = new object();

        public virtual bool IsTransient()
        {
            return GetCompositeKey().IsTransient();
        }

        public virtual object GetId()
        {
            return GetCompositeKey();
        }

        public virtual Type GetIdType()
        {
            return GetCompositeKey().GetType();
        }

        public virtual Type GetTypeUnproxied()
        {
            return GetProxyRealType(this);
        }

        public virtual ICompositeKey GetCompositeKey()
        {
            if (_cachedKey != null) return _cachedKey;
            lock (_lock)
            {
                if (_cachedKey != null) return _cachedKey;
                _cachedKey = CreateCompositeKeyInternal();
            }
            return _cachedKey;
        }

        protected abstract  ICompositeKey CreateCompositeKeyInternal();

        private static Type GetProxyRealType(object proxy)
        {
            var nhProxy = proxy as IProxy;
            if (nhProxy == null) return proxy.GetType();

            var lazyInitializer = nhProxy.Interceptor as ILazyInitializer;
            if (lazyInitializer != null)
                return lazyInitializer.PersistentClass;

            var fieldInterceptorAccessor = nhProxy.Interceptor as IFieldInterceptorAccessor;
            if (fieldInterceptorAccessor != null)
            {
                return fieldInterceptorAccessor.FieldInterceptor == null
                    ? proxy.GetType().BaseType
                    : fieldInterceptorAccessor.FieldInterceptor.MappedClass;
            }
            return proxy.GetType();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = (CompositeEntity)obj;
            return Equals(item.GetCompositeKey(), GetCompositeKey());
        }

        public override int GetHashCode()
        {
            return GetCompositeKey().GetHashCode();
        }
    }

    [Ignore]
    public abstract partial class CompositeEntity<TType, TCol1, TCol2> : CompositeEntity
    {
        protected abstract CompositeKey<TType, TCol1, TCol2> CreateCompositeKey();

        protected override ICompositeKey CreateCompositeKeyInternal()
        {
            return CreateCompositeKey();
        }
    }

    [Ignore]
    public abstract partial class CompositeEntity<TType, TCol1, TCol2, TCol3> : CompositeEntity
    {
        protected abstract CompositeKey<TType, TCol1, TCol2, TCol3> CreateCompositeKey();

        protected override ICompositeKey CreateCompositeKeyInternal()
        {
            return CreateCompositeKey();
        }
    }

    [Ignore]
    public abstract partial class CompositeEntity<TType, TCol1, TCol2, TCol3, TCol4> : CompositeEntity
    {
        protected abstract CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> CreateCompositeKey();

        protected override ICompositeKey CreateCompositeKeyInternal()
        {
            return CreateCompositeKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.SimpleInjector;

namespace SimpleInjector.Extensions
{
    public static class ContainerExtensions
    {
        // This extension method is equivalent to the following registration, for each and every T:
        // container.Register<Lazy<T>>(() => new Lazy<T>(() => container.GetInstance<T>()));
        // This is useful for consumers that have a dependency on a service that is expensive to create, but
        // not always needed.
        // This mimics the behavior of Autofac and Ninject 3. In Autofac this behavior is default.
        internal static void AllowResolvingLazyFactories(this Container container)
        {
            container.ResolveUnregisteredType += (sender, e) =>
            {
                if (!e.UnregisteredServiceType.IsGenericType ||
                    e.UnregisteredServiceType.GetGenericTypeDefinition() != typeof(Lazy<>))
                {
                    return;
                }
                var serviceType = e.UnregisteredServiceType.GetGenericArguments()[0];

                var registration = container.GetRegistration(serviceType, true);

                var funcType = typeof(Func<>).MakeGenericType(serviceType);
                var lazyType = typeof(Lazy<>).MakeGenericType(serviceType);

                var factoryDelegate =
                    Expression.Lambda(funcType, registration.BuildExpression()).Compile();

                var lazyConstructor = (
                    from ctor in lazyType.GetConstructors()
                    where ctor.GetParameters().Length == 1
                    where ctor.GetParameters()[0].ParameterType == funcType
                    select ctor)
                    .Single();

                e.Register(Expression.New(lazyConstructor, Expression.Constant(factoryDelegate)));
            };
        }

        // This extension method is equivalent to the following registration, for each and every T:
        // container.RegisterSingleton<Func<T>>(() => container.GetInstance<T>());
        // This is useful for consumers that need to create multiple instances of a dependency.
        // This mimics the behavior of Autofac. In Autofac this behavior is default.
        internal static void AllowResolvingFuncFactories(this Container container)
        {
            container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType.IsGenericType &&
                    e.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(Func<>))
                {
                    Type serviceType = e.UnregisteredServiceType.GetGenericArguments()[0];

                    InstanceProducer registration = container.GetRegistration(serviceType, true);

                    Type funcType = typeof(Func<>).MakeGenericType(serviceType);

                    var factoryDelegate =
                        Expression.Lambda(funcType, registration.BuildExpression()).Compile();

                    e.Register(Expression.Constant(factoryDelegate));
                }
            };
        }

        public static void Register<TService>(this Container container, Func<TService> instanceCreator, string key, Lifestyle lifestyle)
            where TService : class
        {
            var namedInjection = container.GetKeyedDependencyInjectionBehavior();
            namedInjection.KeyedRegistration.Register(typeof(TService), instanceCreator, key, lifestyle);
        }

        public static void RegisterSingleton<TService>(this Container container, TService instance, string key) where TService : class
        {
            var namedInjection = container.GetKeyedDependencyInjectionBehavior();
            namedInjection.KeyedRegistration.Register(typeof(TService), () => instance, key, Lifestyle.Singleton);
        }

        public static TService GetInstance<TService>(this Container container, string key) where TService : class
        {
            var namedInjection = container.GetKeyedDependencyInjectionBehavior();
            return (TService)namedInjection.GetInstance(typeof(TService), key);
        }

        public static object GetInstance(this Container container, Type serviceType, string key)
        {
            var namedInjection = container.GetKeyedDependencyInjectionBehavior();
            return namedInjection.GetInstance(serviceType, key);
        }

        public static IEnumerable<object> TryGetAllInstances(this Container container, Type serviceType)
        {
            var collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            IServiceProvider provider = container;
            return (IEnumerable<object>)(provider.GetService(collectionType) ??
                     Activator.CreateInstance(typeof(List<>).MakeGenericType(serviceType)));
        }

        public static object TryGetInstance(this Container container, Type serviceType)
        {
            IServiceProvider provider = container;
            return provider.GetService(serviceType);
        }

        private static KeyedDependencyInjectionBehavior GetKeyedDependencyInjectionBehavior(this Container container)
        {
            var namedInjection = container.Options.DependencyInjectionBehavior as KeyedDependencyInjectionBehavior;
            if (namedInjection == null)
            {
                throw new PowerArhitectureException(
                    "Cannot get or create a keyed registration because the current DependencyInjectionBehavior does not support it. " +
                    "Hint: Set the default dependency injection behaviour to: new KeyedDependencyInjectionBehavior(container)");
            }
            return namedInjection;
        }
    }
}

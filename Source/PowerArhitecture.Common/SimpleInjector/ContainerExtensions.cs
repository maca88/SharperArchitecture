using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.Common.Specifications;

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

        public static void RunStartupTasks(this Container container)
        {
            foreach (var task in container.GetAllInstances<IStartupTask>().OrderByDescending(o =>
            {
                var attr = o.GetType().GetCustomAttribute<PriorityAttribute>();
                return attr?.Priority ?? PriorityAttribute.Default;
            }))
            {
                task.Run();
            }
        }
    }
}

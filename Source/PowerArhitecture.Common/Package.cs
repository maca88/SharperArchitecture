using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Util;
using MediatR;
using PowerArhitecture.Common.Adapters;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Commands;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Cryptographics;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.Common.JsonNet;
using PowerArhitecture.Common.Publishers;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.Common.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions;
using SimpleInjector.Packaging;

namespace PowerArhitecture.Common
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IInstanceProvider, InstanceProvider>();
            container.RegisterSingleton<UnhandledExceptionPublisher>();
            container.RegisterSingleton<ICryptography, Sha1Cryptography>();

            container.AllowResolvingLazyFactories();
            container.AllowResolvingFuncFactories();

            container.RegisterSingleton<IMultipleContractResolver>(new MultipleContractResolver());
            container.RegisterSingleton<ICommonConfiguration>(new CommonConfiguration());

            container.RegisterConditional(typeof(ILogger),
                c => typeof(Log4NetAdapter<>).MakeGenericType(c.Consumer.ImplementationType),
                Lifestyle.Singleton,
                c => true);
            LogLog.LogReceived += (s, e) =>
            {
                if (e.LogLog.Prefix.Contains("ERROR"))
                {
                    throw new ConfigurationErrorsException(e.LogLog.Message,
                        e.LogLog.Exception);
                }
            };

            // MediatR
            AppConfiguration.GetDomainAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    !t.IsGenericType &&
                    (
                        t.IsAssignableToGenericType(typeof(IRequestHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncRequestHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(ICancellableAsyncRequestHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(INotificationHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncNotificationHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(ICancellableAsyncNotificationHandler<>))
                    )
                )
                .Select(t => new { Implementation = t, Services = t.GetInterfaces() })
                .ForEach(o =>
                {
                    Registration registration;
                    var injectAsCollection = false;
                    if (o.Implementation.IsAssignableToGenericType(typeof(IRequestHandler<,>)) ||
                        o.Implementation.IsAssignableToGenericType(typeof(IAsyncRequestHandler<,>)) ||
                        o.Implementation.IsAssignableToGenericType(typeof(ICancellableAsyncRequestHandler<,>)))
                    {
                        registration = Lifestyle.Transient.CreateRegistration(o.Implementation, container);
                        container.AddRegistration(o.Implementation, registration);
                    }
                    else
                    {
                        registration = Lifestyle.Singleton.CreateRegistration(o.Implementation, container);
                        container.AddRegistration(o.Implementation, registration);
                        injectAsCollection = true;
                    }

                    foreach (var serviceType in o.Services)
                    {
                        if (injectAsCollection)
                        {
                            container.AppendToCollection(serviceType, registration);
                        }
                        else
                        {
                            container.AddRegistration(serviceType, registration);
                        }
                    }
                });
            container.Register<IMediator, Mediator>();
            container.RegisterSingleton(new SingleInstanceFactory(container.GetInstance));
            container.RegisterSingleton(
                new MultiInstanceFactory(t =>
                {
                    var instances = container.TryGetAllInstances(t).OrderByDescending(o =>
                    {
                        return o.GetType().GetCustomAttribute<PriorityAttribute>()?.Priority ??
                               PriorityAttribute.Default;
                    });
                    return instances;
                }));

            container.RegisterSingleton<IEventPublisher, EventPublisher>();
            container.Register<ICommandDispatcher, CommandDispatcher>();
        }
    }
}

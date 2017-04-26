using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Util;
using SharperArchitecture.Common.Adapters;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Commands;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.Common.Cryptographics;
using SharperArchitecture.Common.Enums;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.Common.Extensions;
using SharperArchitecture.Common.JsonNet;
using SharperArchitecture.Common.Publishers;
using SharperArchitecture.Common.SimpleInjector;
using SharperArchitecture.Common.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions;
using SimpleInjector.Packaging;

namespace SharperArchitecture.Common
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
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

            container.RegisterCollection<IStartupTask>(typeof(IStartupTask).Assembly.GetDependentAssemblies());

            Registration registration;
            var registeredTypes = container
                .GetCurrentRegistrations()
                .Select(o => o.Registration.ImplementationType)
                .ToHashSet();

            // MediatR
            Assembly.GetExecutingAssembly()
                .GetDependentAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    !t.IsGenericType &&
                    (
                        t.IsAssignableToGenericType(typeof(ICommandHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(ICommandHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncCommandHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncCommandHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(IEventHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncEventHandler<>))
                    ) &&
                    !registeredTypes.Contains(t)
                )
                .Select(t => new { Implementation = t, Services = t.GetInterfaces() })
                .ForEach(o =>
                {
                    
                    var injectAsCollection = false;
                    if (o.Implementation.IsAssignableToGenericType(typeof(ICommandHandler<>)) ||
                        o.Implementation.IsAssignableToGenericType(typeof(ICommandHandler<,>)) ||
                        o.Implementation.IsAssignableToGenericType(typeof(IAsyncCommandHandler<>)) ||
                        o.Implementation.IsAssignableToGenericType(typeof(IAsyncCommandHandler<,>))
                        )
                    {
                        registration = Lifestyle.Transient.CreateRegistration(o.Implementation, container);
                        container.AddRegistration(o.Implementation, registration);
                    }
                    else
                    {
                        var attr = o.Implementation.GetCustomAttribute<LifetimeAttribute>()
                                   ?? new LifetimeAttribute(Lifetime.Singleton);
                        switch (attr.Lifetime)
                        {
                            case Lifetime.Singleton:
                                registration = Lifestyle.Singleton.CreateRegistration(o.Implementation, container);
                                break;
                            case Lifetime.Scoped:
                                registration = Lifestyle.Scoped.CreateRegistration(o.Implementation, container);
                                break;
                            case Lifetime.Transient:
                                registration = Lifestyle.Transient.CreateRegistration(o.Implementation, container);
                                break;
                            default:
                                throw new SharperArchitectureException($"Invalid {nameof(Lifetime)} value: {attr.Lifetime}");
                        }
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

            registration = Lifestyle.Singleton.CreateRegistration<EventAggregator>(container);
            container.AddRegistration(typeof(EventAggregator), registration);
            container.AddRegistration(typeof(IEventPublisher), registration);
            container.AddRegistration(typeof(IEventSubscriber), registration);

            container.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();
        }
    }
}

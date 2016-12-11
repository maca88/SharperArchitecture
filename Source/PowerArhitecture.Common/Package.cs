using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Util;
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

            var registeredTypes = container
                .GetCurrentRegistrations()
                .Select(o => o.Registration.ImplementationType)
                .ToHashSet();

            // MediatR
            AppConfiguration.GetDomainAssemblies()
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
                    Registration registration;
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
            container.RegisterSingleton<IEventPublisher, EventPublisher>();
            container.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();
        }
    }
}

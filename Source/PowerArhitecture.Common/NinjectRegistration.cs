using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using MediatR;
using PowerArhitecture.Common.Cryptographics;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.JsonNet;
using PowerArhitecture.Common.Publishers;
using PowerArhitecture.Common.Specifications;
using Newtonsoft.Json;
using Ninject.Extensions.Conventions;
using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Commands;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Enums;

namespace PowerArhitecture.Common
{
    public class NinjectRegistration : NinjectModule
    {
        private static readonly MethodInfo NinjectGetAllMethodInfo;

        static NinjectRegistration()
        {
            NinjectGetAllMethodInfo = typeof (ResolutionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(o => o.Name == "GetAll" && o.IsGenericMethod && o.GetParameters().Length == 2);
        }

        public override void Load()
        {
            Bind<UnhandledExceptionPublisher>().ToSelf().InSingletonScope();
            Bind<ICryptography>().To<Sha1Cryptography>().InSingletonScope();

            //Support for Lazy injections
            Bind(typeof(Lazy<>)).ToMethod(ctx =>
                GetType()
                    .GetMethod("GetLazyProvider", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(ctx.GenericArguments[0])
                    .Invoke(this, new object[] { ctx.GetContextPreservingResolutionRoot() }));

            //Convenction for tasks
            Kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof(ITask).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .SelectAllClasses()
                .InheritedFrom<ITask>()
                .WhichAreNotGeneric()
                .BindDefaultInterfaces());

            //Json.Net
            var contractResolver = new MultipleContractResolver();
            Bind<IMultipleContractResolver>().ToConstant(contractResolver);

            Bind<ICommonConfiguration>().To<CommonConfiguration>().InSingletonScope();

            // MediatR
            Kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes()
                        .Any(t =>
                            t.IsAssignableToGenericType(typeof(IRequestHandler<,>)) ||
                            t.IsAssignableToGenericType(typeof(IAsyncRequestHandler<,>)) ||
                            t.IsAssignableToGenericType(typeof(ICancellableAsyncRequestHandler<,>)) ||
                            t.IsAssignableToGenericType(typeof(INotificationHandler<>)) ||
                            t.IsAssignableToGenericType(typeof(IAsyncNotificationHandler<>)) ||
                            t.IsAssignableToGenericType(typeof(ICancellableAsyncNotificationHandler<>))
                        )))
                .IncludingNonePublicTypes()
                .Select(t => 
                    !t.IsInterface &&
                    !t.IsAbstract &&
                    (
                        t.IsAssignableToGenericType(typeof(IRequestHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncRequestHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(ICancellableAsyncRequestHandler<,>)) ||
                        t.IsAssignableToGenericType(typeof(INotificationHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(IAsyncNotificationHandler<>)) ||
                        t.IsAssignableToGenericType(typeof(ICancellableAsyncNotificationHandler<>))
                    ) && 
                    !Kernel.GetBindings(t).Any())
                .BindSelection((type, types) => new List<Type> { type }.Union(types))
                .Configure((syntax, type) =>
                {
                    if (type.IsAssignableToGenericType(typeof(IRequestHandler<,>)) ||
                        type.IsAssignableToGenericType(typeof(IAsyncRequestHandler<,>)) ||
                        type.IsAssignableToGenericType(typeof(ICancellableAsyncRequestHandler<,>)))
                    {
                        syntax.InTransientScope();
                    }
                    else
                    {
                        syntax.InSingletonScope();
                    }
                }));

            Bind<SingleInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.Get(t));
            Bind<MultiInstanceFactory>().ToMethod(ctx => t => ctx.Kernel.GetAll(t).OrderByDescending(o =>
            {
                var attr = o.GetType().GetCustomAttribute<PriorityAttribute>();
                return attr?.Priority ?? 0;
            }));
            Bind<IMediator>().To<Mediator>();
            Bind<IEventPublisher>().To<EventPublisher>().InSingletonScope();
            Bind<ICommandDispatcher>().To<CommandDispatcher>();
        }

        protected Lazy<T> GetLazyProvider<T>(IResolutionRoot resolutionRoot)
        {
            return new Lazy<T>(() =>
            {
                var type = typeof(T);
                if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
                {
                    return (T) NinjectGetAllMethodInfo.MakeGenericMethod(type.GetGenericArguments()[0])
                        .Invoke(null, new object[] {resolutionRoot, new IParameter[0]});
                }
                return resolutionRoot.Get<T>();
            });
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using PowerArhitecture.Common.Cryptographics;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.JsonNet;
using PowerArhitecture.Common.Providers;
using PowerArhitecture.Common.Publishers;
using PowerArhitecture.Common.Specifications;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject.Extensions.Conventions;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Syntax;

namespace PowerArhitecture.Common
{
    public class NinjectRegistration : NinjectModule
    {
        private static readonly MethodInfo NinjectGetAllMethodInfo;

        static NinjectRegistration()
        {
            NinjectGetAllMethodInfo = typeof (ResolutionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(o => o.Name == "GetAll" && o.IsGenericMethod && o.GetParameters().Count() == 2);
        }

        public override void Load()
        {
            Bind<IEventAggregator>()
                .ToProvider<EventAggregatorProvider>().InSingletonScope()
                .OnActivation(EventAggregatorProvider.OnActivation);

            Bind<UnhandledExceptionPublisher>().ToSelf().InSingletonScope();
            Bind<ICryptography>().To<Sha1Cryptography>().InSingletonScope();
            Bind<IPasswordHasher>().To<PasswordHasher>().InSingletonScope();

            //Support for Lazy injections
            Bind(typeof(Lazy<>)).ToMethod(ctx =>
                GetType()
                    .GetMethod("GetLazyProvider", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(ctx.GenericArguments[0])
                    .Invoke(this, new object[] { ctx.Kernel }));

            //Convenction for tasks
            Kernel.Bind(o => o
                .From(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof(ITask).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .SelectAllClasses()
                .InheritedFrom<ITask>()
                .WhichAreNotGeneric()
                .BindDefaultInterfaces());

            //Convenction for listeners
            Kernel.Bind(o => o
                .From(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetTypes().Any(t => t.IsAssignableToGenericType(typeof(IListener<>)))))
                .IncludingNonePublicTypes()
                .Select(t => !t.IsInterface && !t.IsAbstract && t.IsAssignableToGenericType(typeof(IListener<>)) && 
                    t != typeof(DelegateListener<>) && !Kernel.GetBindings(t).Any())
                .BindSelection((type, types) => new List<Type>{type}.Union(types))
                .Configure(syntax => syntax.InSingletonScope()));

            Bind<IPrincipal>().ToMethod(o => Thread.CurrentPrincipal);

            //Bind(typeof(Lazy<IUserCache>)).ToMethod(ctx => new Lazy<IUserCache>(() => Kernel.Get<IUserCache>())); //DONE

            //Json.Net
            var contractResolver = new MultipleContractResolver();
            contractResolver.SetDefaultResolver(ContractResolvers.CamelCasePropertyNamesResolver);
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };
            var jsonNetSerializer = JsonSerializer.Create(serializerSettings);

            Bind<IMultipleContractResolver>().ToConstant(contractResolver);
            Bind<JsonSerializerSettings>().ToConstant(serializerSettings);
            Bind<JsonSerializer>().ToConstant(jsonNetSerializer);

        }

        protected Lazy<T> GetLazyProvider<T>(IKernel kernel)
        {
            return new Lazy<T>(() =>
            {
                var type = typeof (T);
                if (typeof (IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                    return (T)NinjectGetAllMethodInfo.MakeGenericMethod(type).Invoke(null, new object[] { kernel, new IParameter[0] });
                }
                return kernel.Get<T>();
            });
        }
    }
}

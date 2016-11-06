using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Web.Common;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Parameters;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.Extensions
{
    public static class NinjectModuleExtensions
    {
        private static IBinding _defaultSessionFactoryBinding;
        private static IBinding _defaultNhConfigurationBinding;
        private static IBinding _defaultDbSettingsBinding;
        private static readonly IList<IBinding> DefaultSessionBindings = new List<IBinding>();

        public const string DatabaseConfigurationMedatadataKey = "DatabaseConfigurationName";

        public static void RegisterDatabaseConfiguration(this IKernel kernel, DatabaseConfiguration dbConfiguration)
        {
            var name = dbConfiguration.Name;
            if (name == DatabaseConfiguration.DefaultName && _defaultDbSettingsBinding != null)
                kernel.RemoveBinding(_defaultDbSettingsBinding);

            var bindingBuilder = (BindingBuilder<DatabaseConfiguration>)kernel.Bind<DatabaseConfiguration>();
            var syntaxBinding = bindingBuilder.ToConstant(dbConfiguration)
                .When(CheckSessionFactoryName(name))
                .InSingletonScope();

            if (name == DatabaseConfiguration.DefaultName)
                _defaultDbSettingsBinding = bindingBuilder.Binding;

            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);
            kernel.RegisterNhConfiguration(dbConfiguration.NHibernateConfiguration, name);
            Database.RegisteredDatabaseConfigurations.AddOrUpdate(name, dbConfiguration, (k, v) => dbConfiguration);
        }

        public static void RegisterDatabaseConfiguration(this NinjectModule module, DatabaseConfiguration configuration)
        {
            module.Kernel.RegisterDatabaseConfiguration(configuration);
        }

        private static void RegisterNhConfiguration(this IKernel kernel, Configuration cfg, string name)
        {
            if (name == DatabaseConfiguration.DefaultName && _defaultNhConfigurationBinding != null)
                kernel.RemoveBinding(_defaultNhConfigurationBinding);

            var bindingBuilder = (BindingBuilder<Configuration>)kernel.Bind<Configuration>();
            var syntaxBinding = bindingBuilder.ToConstant(cfg)
                .When(CheckSessionFactoryName(name))
                .InSingletonScope();

            if (name == DatabaseConfiguration.DefaultName)
                _defaultNhConfigurationBinding = bindingBuilder.Binding;
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);
            kernel.RegisterSessionFactory(name);
        }

        private static void RegisterSession(this IKernel kernel, string name)
        {
            if (name == DatabaseConfiguration.DefaultName && DefaultSessionBindings.Any())
            {
                foreach (var binding in DefaultSessionBindings)
                {
                    kernel.RemoveBinding(binding);
                }
                DefaultSessionBindings.Clear();
            }

            var bindingBuilder = (BindingBuilder<ISession>)kernel.Bind<ISession>();
            var bindingBuilder2 = (BindingBuilder<ISession>)kernel.Bind<ISession>();
            var bindingBuilder3 = (BindingBuilder<ISession>)kernel.Bind<ISession>();

            var syntaxBinding = bindingBuilder
                .ToProvider<SessionProvider>()
                .WhenRequestScopeExistsAndNoAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InRequestScope();
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);

            syntaxBinding = bindingBuilder2
                .ToProvider<SessionProvider>()
                .WhenRequestScopeNotExistsAndNoAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InCallScope();
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);

            syntaxBinding = bindingBuilder3
                .ToProvider<SessionProvider>()
                .WhenAnyAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InNamedScope(ResolutionScopes.UnitOfWork);
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);

            if (name == DatabaseConfiguration.DefaultName)
            {
                DefaultSessionBindings.Add(bindingBuilder.Binding);
                DefaultSessionBindings.Add(bindingBuilder2.Binding);
                DefaultSessionBindings.Add(bindingBuilder3.Binding);
            }
            kernel.RegisterRepositores(name);
        }

        private static void RegisterRepositores(this IKernel kernel, string name)
        {
            kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof(IRepository).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .Select(CanBindCustomRepository)
                .BindSelection((type, types) => new List<Type> { type }.Union(types))
                .Configure(c =>
                {
                    c.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);
                    c.WhenAnyAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork,
                        CheckSessionFactoryName(name))
                        .InNamedScope(ResolutionScopes.UnitOfWork);
                }));
            kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof(IRepository).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .Select(CanBindCustomRepository)
                .BindSelection((type, types) => new List<Type> { type }.Union(types))
                .Configure(c =>
                {
                    c.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);
                    c.WhenRequestScopeExistsAndNoAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork,
                        CheckSessionFactoryName(name))
                        .InRequestScope();
                }));

            var syntaxBinding = kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>))
                .WhenAnyAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InNamedScope(ResolutionScopes.UnitOfWork);
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);

            syntaxBinding = kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>))
                .WhenRequestScopeExistsAndNoAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InRequestScope();
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);

            syntaxBinding = kernel.Bind(typeof(IRepository<,>)).To(typeof(Repository<,>))
                .WhenAnyAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InNamedScope(ResolutionScopes.UnitOfWork);
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);

            syntaxBinding = kernel.Bind(typeof(IRepository<,>)).To(typeof(Repository<,>))
                .WhenRequestScopeExistsAndNoAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InRequestScope();
            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);
        }

        private static void RegisterSessionFactory(this IKernel kernel, string name)
        {
            if (name == DatabaseConfiguration.DefaultName && _defaultSessionFactoryBinding != null)
                kernel.RemoveBinding(_defaultSessionFactoryBinding);

            var bindingBuilder = (BindingBuilder<ISessionFactory>)kernel.Bind<ISessionFactory>();
            var syntaxBinding = bindingBuilder.ToProvider<SessionFactoryProvider>()
                .When(CheckSessionFactoryName(name))
                .InSingletonScope()
                .OnActivation(SessionFactoryProvider.PopulateData);

            if (name == DatabaseConfiguration.DefaultName)
                _defaultSessionFactoryBinding = bindingBuilder.Binding;

            syntaxBinding.BindingConfiguration.Metadata.Set(DatabaseConfigurationMedatadataKey, name);
            kernel.RegisterSession(name);
        }

        private static bool CanBindCustomRepository(Type t)
        {
            if (!t.IsClass || t.IsAbstract || t.IsGenericType || !typeof(IRepository).IsAssignableFrom(t))
            {
                return false;
            }
            var repoAttr = t.GetCustomAttribute<RepositoryAttribute>();
            return repoAttr == null || repoAttr.AutoBind;
        }

        internal static Func<IRequest, bool> CheckSessionFactoryName(string name)
        {
            return r =>
            {
                if (r.Target != null) //null can be when injecting like: kernel.Get<ISession>()
                {
                    var attr = (DatabaseAttribute)r.Target.GetCustomAttributes(typeof(DatabaseAttribute), true).FirstOrDefault();
                    if (attr != null)
                        return attr.ConfigurationName == name;
                }
                var param = r.Parameters.OfType<DatabaseConfigurationParameter>().FirstOrDefault();
                if (param != null)
                    return param.Name == name;
                return name == DatabaseConfiguration.DefaultName;
            };
        } 
    }
}

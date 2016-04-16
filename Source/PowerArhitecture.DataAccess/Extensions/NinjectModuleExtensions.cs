using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Web.Common;
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

        public static void RegisterDatabaseConfiguration(this IKernel kernel, DatabaseConfiguration dbConfiguration, string name = null)
        {
            if (name == null && _defaultDbSettingsBinding != null)
                kernel.RemoveBinding(_defaultDbSettingsBinding);

            var bindingBuilder = (BindingBuilder<DatabaseConfiguration>)kernel.Bind<DatabaseConfiguration>();
            var syntaxBinding = bindingBuilder.ToConstant(dbConfiguration)
                .When(CheckSessionFactoryName(name))
                .InSingletonScope();

            if (name == null)
                _defaultDbSettingsBinding = bindingBuilder.Binding;

            syntaxBinding.BindingConfiguration.Metadata.Set("SessionFactoryName", name);
            kernel.RegisterNhConfiguration(dbConfiguration.NHibernateConfiguration, name);
        }

        public static void RegisterDatabaseConfiguration(this NinjectModule module, DatabaseConfiguration configuration, string name = null)
        {
            module.Kernel.RegisterDatabaseConfiguration(configuration, name);
        }

        private static void RegisterNhConfiguration(this IKernel kernel, Configuration cfg, string name = null)
        {
            if (name == null && _defaultNhConfigurationBinding != null)
                kernel.RemoveBinding(_defaultNhConfigurationBinding);

            var bindingBuilder = (BindingBuilder<Configuration>)kernel.Bind<Configuration>();
            var syntaxBinding = bindingBuilder.ToConstant(cfg)
                .When(CheckSessionFactoryName(name))
                .InSingletonScope();

            if (name == null)
                _defaultNhConfigurationBinding = bindingBuilder.Binding;
            syntaxBinding.BindingConfiguration.Metadata.Set("SessionFactoryName", name);
            kernel.RegisterSessionFactory(name);
        }

        private static void RegisterSession(this IKernel kernel, string name = null)
        {
            if (name == null && DefaultSessionBindings.Any())
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
                .InRequestScope()
                .OnDeactivation(SessionDeactivated);

            var syntaxBinding2 = bindingBuilder2
                .ToProvider<SessionProvider>()
                .WhenRequestScopeNotExistsAndNoAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryName(name))
                .InCallScope()
                .OnDeactivation(SessionDeactivated);

            var syntaxBinding3 = bindingBuilder3
                .ToProvider<SessionProvider>()
                .WhenAnyAncestorOrCurrentNamedAnd(ResolutionScopes.UnitOfWork, CheckSessionFactoryNameForContext(name))
                .InNamedScope(ResolutionScopes.UnitOfWork)
                .OnDeactivation(SessionDeactivated);

            if (name == null)
            {
                DefaultSessionBindings.Add(bindingBuilder.Binding);
                DefaultSessionBindings.Add(bindingBuilder2.Binding);
                DefaultSessionBindings.Add(bindingBuilder3.Binding);
            }

            syntaxBinding.BindingConfiguration.Metadata.Set("SessionFactoryName", name);
            syntaxBinding2.BindingConfiguration.Metadata.Set("SessionFactoryName", name);
            syntaxBinding3.BindingConfiguration.Metadata.Set("SessionFactoryName", name);
        }

        private static void SessionDeactivated(IContext context, ISession session)
        {
            context.Kernel.Get<ISessionManager>().Remove(session);
        }

        private static void RegisterSessionFactory(this IKernel kernel, string name = null)
        {
            if (name == null && _defaultSessionFactoryBinding != null)
                kernel.RemoveBinding(_defaultSessionFactoryBinding);

            var bindingBuilder = (BindingBuilder<ISessionFactory>)kernel.Bind<ISessionFactory>();
            var syntaxBinding = bindingBuilder.ToProvider<SessionFactoryProvider>()
                .When(CheckSessionFactoryName(name))
                .InSingletonScope()
                .OnActivation(SessionFactoryProvider.PopulateData);

            if (name == null)
                _defaultSessionFactoryBinding = bindingBuilder.Binding;

            syntaxBinding.BindingConfiguration.Metadata.Set("SessionFactoryName", name);
            kernel.RegisterSession(name);
        }

        

        private static Func<IContext, bool> CheckSessionFactoryNameForContext(string name)
        {
            return ctx => CheckSessionFactoryName(name)(ctx.Request);
        }

        private static Func<IRequest, bool> CheckSessionFactoryName(string name)
        {
            return r =>
            {
                if (r.Target != null) //null can be when injecting like: kernel.Get<ISession>()
                {
                    var attr = (NamedSessionFactoryAttribute)r.Target.GetCustomAttributes(typeof(NamedSessionFactoryAttribute), true).FirstOrDefault();
                    if (attr != null)
                        return attr.Name == name;
                }
                var param = r.Parameters.OfType<NamedSessionFactoryParameter>().FirstOrDefault();
                if (param != null)
                    return param.Name == name;
                return name == null;
            };
        } 
    }
}

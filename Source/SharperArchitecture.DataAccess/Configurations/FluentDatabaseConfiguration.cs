using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using NHibernate.Cfg;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;

namespace SharperArchitecture.DataAccess.Configurations
{
    public class FluentDatabaseConfiguration : IFluentDatabaseConfiguration
    {
        private readonly DatabaseConfiguration _configuration;

        public string Name => _configuration.Name;

        public static IFluentDatabaseConfiguration Create(Configuration configuration)
        {
            return new FluentDatabaseConfiguration(configuration);
        }

        public static IFluentDatabaseConfiguration Create(Configuration configuration, string name, bool fillFromConfig = true)
        {
            return new FluentDatabaseConfiguration(configuration, name, fillFromConfig);
        }

        internal FluentDatabaseConfiguration(Configuration configuration, string name = null, bool fillFromConfig = true)
        {
            _configuration = new DatabaseConfiguration(configuration, name, fillFromConfig);
        }

        public IFluentDatabaseConfiguration ValidateSchema(bool value = true)
        {
            _configuration.ValidateSchema = value;
            return this;
        }

        public IFluentDatabaseConfiguration RecreateAtStartup(bool value = true)
        {
            _configuration.RecreateAtStartup = value;
            return this;
        }

        public IFluentDatabaseConfiguration UpdateSchemaAtStartup(bool value = true)
        {
            _configuration.UpdateSchemaAtStartup = value;
            return this;
        }

        public IFluentDatabaseConfiguration AllowOneToOneWithoutLazyLoading(bool value = true)
        {
            _configuration.AllowOneToOneWithoutLazyLoading = value;
            return this;
        }

        public IFluentDatabaseConfiguration AutomappingConfiguration(Action<IFluentAutoMappingConfiguration> action)
        {
            _configuration.AutoMappingConfigurationAction = action;
            return this;
        }

        public IFluentDatabaseConfiguration AddEntityAssembly(Assembly assembly)
        {
            _configuration.EntityAssemblies.Add(assembly);
            return this;
        }

        public IFluentDatabaseConfiguration AddEntityAssemblies(IEnumerable<Assembly> assemblies)
        {
            _configuration.EntityAssemblies.AddRange(assemblies);
            return this;
        }

        public IFluentDatabaseConfiguration AddConventionAssembly(Assembly assembly)
        {
            _configuration.ConventionAssemblies.Add(assembly);
            return this;
        }

        public IFluentDatabaseConfiguration AddConventionAssemblies(IEnumerable<Assembly> assemblies)
        {
            _configuration.ConventionAssemblies.AddRange(assemblies);
            return this;
        }

        public IFluentDatabaseConfiguration HbmMappingsPath(string path)
        {
            _configuration.HbmMappingsPath = path;
            return this;
        }

        public IFluentDatabaseConfiguration Conventions(Action<IFluentConventionsConfiguration> action)
        {
            action(new FluentConventionsConfiguration(_configuration.Conventions));
            return this;
        }

        public IFluentDatabaseConfiguration FluentNHibernate(Action<FluentConfiguration> action)
        {
            _configuration.FluentConfigurationAction = action;
            return this;
        }

        public IFluentDatabaseConfiguration ConfigurationCompletedAction(Action<Configuration> action)
        {
            _configuration.ConfigurationCompletedAction = action;
            return this;
        }

        public DatabaseConfiguration Build()
        {
            _configuration.AutoMappingConfigurationAction?.Invoke(_configuration.AutoMappingConfiguration);
            if (!_configuration.EntityAssemblies.Any())
            {
                _configuration.EntityAssemblies.AddRange(AppConfiguration.GetDomainAssemblies()
                    .Where(assembly => assembly.GetTypes().Any(o => typeof(IEntity).IsAssignableFrom(o))));
            }
            if (!_configuration.ConventionAssemblies.Any())
            {
                _configuration.ConventionAssemblies.AddRange(AppConfiguration.GetDomainAssemblies()
                    .Where(assembly => assembly != Assembly.GetAssembly(typeof(IAutomappingConfiguration)))
                    .Where(assembly => assembly.GetTypes().Any(o => typeof(IConvention).IsAssignableFrom(o))));
            }
            return _configuration;
        }
    }
}

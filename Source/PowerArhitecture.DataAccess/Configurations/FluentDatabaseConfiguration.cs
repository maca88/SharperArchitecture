using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.Configurations
{
    public class FluentDatabaseConfiguration : IFluentDatabaseConfiguration
    {
        private readonly DatabaseConfiguration _configuration;

        public static IFluentDatabaseConfiguration Create(Configuration configuration)
        {
            return new FluentDatabaseConfiguration(configuration);
        }

        internal FluentDatabaseConfiguration(Configuration configuration)
        {
            _configuration = new DatabaseConfiguration(configuration);
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

        public IFluentDatabaseConfiguration AutomappingConfiguration(IAutomappingConfiguration value)
        {
            _configuration.AutomappingConfiguration = value;
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
            return _configuration;
        }
    }
}

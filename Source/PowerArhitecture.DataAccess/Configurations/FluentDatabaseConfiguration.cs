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

        public string Name => _configuration.Name;

        public static IFluentDatabaseConfiguration Create(Configuration configuration, string name = null)
        {
            return new FluentDatabaseConfiguration(configuration, name);
        }

        internal FluentDatabaseConfiguration(Configuration configuration, string name = null)
        {
            _configuration = new DatabaseConfiguration(configuration, name);
        }

        public IFluentDatabaseConfiguration ValidateSchema(bool value = true)
        {
            _configuration.ValidateSchema = value;
            return this;
        }

        public IFluentDatabaseConfiguration RegisterRepository(Type interfaceType, Type concreteType)
        {
            if (Name == DatabaseConfiguration.DefaultName)
            {
                throw new NotSupportedException("Registering a custom repository for the default database configuration is not supported.");
            }
            if (!interfaceType.IsInterface || !interfaceType.IsGenericType || !interfaceType.IsAssignableToGenericType(typeof(IRepository<,>)))
            {
                throw new ArgumentException("Type must be an open generic interface that derives from IRepository<,>", nameof(interfaceType));
            }
            if (!concreteType.IsClass || !concreteType.IsGenericType || !concreteType.IsAssignableToGenericType(typeof(Repository<,>)))
            {
                throw new ArgumentException("Type must be an open generic class that derives from Repository<,>", nameof(concreteType));
            }
            if (!concreteType.IsAssignableToGenericType(interfaceType))
            {
                throw new ArgumentException($"{nameof(interfaceType)} must be assignable from {nameof(concreteType)}");
            }
            foreach (var key in _configuration.RepositoryTypes.Keys)
            {
                if (!key.IsAssignableToGenericType(interfaceType) &&
                    !interfaceType.IsAssignableToGenericType(key))
                {
                    throw new ArgumentException("Type must be assignable to previous registered repositories", nameof(interfaceType));
                }
            }
            _configuration.RepositoryTypes[interfaceType] = concreteType;
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
            return _configuration;
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using SimpleInjector;

namespace PowerArhitecture.DataAccess.Internal
{
    internal class DbStore : IDbStore
    {
        private readonly Container _container;

        public DbStore(Container container)
        {
            _container = container;
        }

        public IQueryable<T> Query<T>() where T : IEntity
        {
            return GetSession(typeof(T).GetDatabaseConfigurationNameForModel()).Query<T>();
        }

        public T Load<T>(object id) where T : IEntity
        {
            return GetSession(typeof(T).GetDatabaseConfigurationNameForModel()).Load<T>(id);
        }

        public Task<T> LoadAsync<T>(object id) where T : IEntity
        {
            return GetSession(typeof(T).GetDatabaseConfigurationNameForModel()).LoadAsync<T>(id);
        }

        public T Get<T>(object id) where T : IEntity
        {
            return GetSession(typeof(T).GetDatabaseConfigurationNameForModel()).Get<T>(id);
        }

        public Task<T> GetAsync<T>(object id) where T : IEntity
        {
            return GetSession(typeof(T).GetDatabaseConfigurationNameForModel()).GetAsync<T>(id);
        }

        public void Save(IEntity model)
        {
            GetSession(model.GetTypeUnproxied().GetDatabaseConfigurationNameForModel()).Save(model);
        }

        public Task SaveAsync(IEntity model)
        {
            return GetSession(model.GetTypeUnproxied().GetDatabaseConfigurationNameForModel()).SaveAsync(model);
        }

        public void Delete(IEntity model)
        {
            GetSession(model.GetTypeUnproxied().GetDatabaseConfigurationNameForModel()).Delete(model);
        }

        public async Task DeleteAsync(IEntity model)
        {
            await GetSession(model.GetTypeUnproxied().GetDatabaseConfigurationNameForModel()).DeleteAsync(model);
        }

        public void Refresh(IEntity model)
        {
            GetSession(model.GetTypeUnproxied().GetDatabaseConfigurationNameForModel()).Refresh(model);
        }

        public async Task RefreshAsync(IEntity model)
        {
            await GetSession(model.GetTypeUnproxied().GetDatabaseConfigurationNameForModel()).RefreshAsync(model);
        }

        private ISession GetSession(string dbConfigName)
        {
            if (!Database.ContainsDatabaseConfiguration(dbConfigName))
            {
                throw new PowerArhitectureException(
                    $"There isn't any DatabaseConfiguration registered with the name '{dbConfigName}'");
            }
            dbConfigName = dbConfigName ?? DatabaseConfiguration.DefaultName;
            return _container.GetDatabaseService<ISession>(dbConfigName);
        }
    }
}

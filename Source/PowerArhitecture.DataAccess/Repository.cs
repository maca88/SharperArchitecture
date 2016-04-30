using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation.Specifications;
using FluentValidation.Results;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using LockMode = PowerArhitecture.DataAccess.Enums.LockMode;
using Property = NHibernate.Mapping.Property;

namespace PowerArhitecture.DataAccess
{
    public class Repository<TModel> : Repository<TModel, long>, IRepository<TModel> where TModel : class, IEntity<long>, new()
    {
        public Repository(ISession session, ILogger logger, ISessionEventProvider sessionEventProvider) //TODO: Session Lazy - need to test perserving context!
            : base(session, logger, sessionEventProvider)
        {
        }
    }

    /// <summary>
    /// Dispose only if is used outside HttpContext or UnitOfWork
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class Repository<TModel, TId> : IRepository<TModel, TId> where TModel : class, IEntity<TId>, new()
    {
        public Repository(ISession session, ILogger logger, ISessionEventProvider sessionEventProvider)
        {
            Logger = logger;
            Session = session;
            SessionEventProvider = sessionEventProvider;
        }

        protected ILogger Logger { get; private set; }

        public ISession Session { get; private set; }

        public virtual IPrincipal User
        {
            get
            {
                var context = Session?.GetSessionImplementation()?.CustomContext as SessionContext;
                return context?.CurrentPrincipal;
            }
        }

        protected ISessionEventProvider SessionEventProvider { get; private set; }

        public virtual Task<TUser> GetCurrentUserAsync<TUser>(Expression<Func<TUser, string>> nameProperty, bool cachable) where TUser : IEntity
        {
            return GetCurrentUserAsync<TUser>(nameProperty.GetFullPropertyName(), cachable);
        }

        public virtual async Task<TUser> GetCurrentUserAsync<TUser>(string nameProperty, bool cachable) where TUser : IEntity
        {
            if (string.IsNullOrEmpty(User?.Identity?.Name))
            {
                return default(TUser);
            }
            return (TUser)await Session.CreateCriteria(typeof(TUser))
                .Add(Restrictions.Eq(nameProperty, User.Identity.Name))
                .SetCacheable(cachable)
                .UniqueResultAsync().ConfigureAwait(false);
        }

        public virtual IQueryable<TModel> Query()
        {
            return Session.Query<TModel>();
        }

        public virtual TModel Get(TId id)
        {
            return Session.Get<TModel>(id);
        }

        public virtual Task<TModel> GetAsync(TId id)
        {
            return Session.GetAsync<TModel>(id);
        }

        public virtual TModel Load(TId id)
        {
            return Session.Load<TModel>(id); //select query will not be executed until a property of the model is accessed
        }

        public virtual Task<TModel> LoadAsync(TId id)
        {
            return Session.LoadAsync<TModel>(id);
        }

        public virtual T Load<T>(object id)
        {
            return Session.Load<T>(id);
        }

        public virtual Task<T> LoadAsync<T>(object id)
        {
            return Session.LoadAsync<T>(id);
        }

        public virtual T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        public virtual Task<T> GetAsync<T>(object id)
        {
            return Session.GetAsync<T>(id);
        }

        public virtual void Save(object model)
        {
            Session.Save(model);
        }

        public virtual Task SaveAsync(object model)
        {
            return Session.SaveAsync(model);
        }

        public virtual void AddAListener(Func<Task> action, SessionListenerType listenerType)
        {
            SessionEventProvider.AddAListener(listenerType, Session, action);
        }

        public virtual void AddAListener(Func<IRepository<TModel, TId>, Task> action, SessionListenerType listenerType)
        {
            SessionEventProvider.AddAListener(listenerType, Session, () => action(this));
        }

        public virtual IQueryable<T> Query<T>() where T : IEntity
        {
            return Session.Query<T>();
        }

        public virtual void Save(TModel model)
        {
            Session.SaveOrUpdate(model); 
        }

        public virtual Task SaveAsync(TModel model)
        {
            return Session.SaveAsync(model);
        }

        public virtual void Update(TModel model)
        {
            Session.Update(model);
        }

        public virtual Task UpdateAsync(TModel model)
        {
            return Session.UpdateAsync(model);
        }

        public virtual void Delete(TModel model)
        {
            Session.Delete(model);
        }

        public Task DeleteAsync(TModel model)
        {
            return Session.DeleteAsync(model);
        }

        public virtual void Delete(TId id)
        {
            Delete(Load(id));
        }

        public virtual async Task DeleteAsync(TId id)
        {
            await DeleteAsync(await LoadAsync(id).ConfigureAwait(false));
        }

        public virtual IEnumerable<PropertyInfo> GetMappedProperties()
        {
            var typeProps = typeof(TModel).GetProperties().ToDictionary(o => o.Name);
            return Database.GetSessionFactoryInfo(Session.SessionFactory)
                .Configuration.ClassMappings
                .Where(o => o.MappedClass == typeof(TModel))
                .SelectMany(o => o.PropertyIterator.Union(new List<Property>{ o.IdentifierProperty }))
                .Where(o => typeProps.ContainsKey(o.Name))
                .Select(o => typeProps[o.Name]);
        }

    }
}

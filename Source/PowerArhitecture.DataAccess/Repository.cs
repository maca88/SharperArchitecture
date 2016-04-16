using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Managers;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation.Specifications;
using FluentValidation.Results;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Mapping;
using Ninject.Extensions.Logging;
using LockMode = PowerArhitecture.DataAccess.Enums.LockMode;

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

        protected ISessionEventProvider SessionEventProvider { get; private set; }

        public IQueryable<TModel> Query()
        {
            return Session.Query<TModel>();
        }

        public virtual TModel Get(TId id)
        {
            return Session.Get<TModel>(id);
        }

        public virtual TModel Load(TId id)
        {
            return Session.Load<TModel>(id); //select query will not be executed until a property of the model is accessed
        }

        public virtual T Load<T>(object id)
        {
            return Session.Load<T>(id);
        }

        public T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        public void Save(object model)
        {
            Session.Save(model);
        }

        public void AddAListener(Action action, SessionListenerType listenerType)
        {
            SessionEventProvider.AddAListener(listenerType, Session, action);
        }

        public void AddAListener(Action<IRepository<TModel, TId>> action, SessionListenerType listenerType)
        {
            SessionEventProvider.AddAListener(listenerType, Session, () => action(this));
        }

        //public T DeepCopy<T>(T model, DeepCopyOptions opts = null) where T : IEntity
        //{
        //    return Session.DeepCopy(model, opts);
        //}

        public IQueryable<T> Query<T>() where T : IEntity
        {
            return Session.Query<T>();
        }

        public virtual void Save(TModel model)
        {
            Session.SaveOrUpdate(model); 
        }

        public virtual void Update(TModel model)
        {
            Session.Update(model);
        }

        public virtual void Delete(TModel model)
        {
            Session.Delete(model);
        }

        public virtual void Delete(TId id)
        {
            Delete(Load(id));
        }

        //public TModel DeepCopy(TModel model, DeepCopyOptions opts = null)
        //{
        //    return Session.DeepCopy(model, opts);
        //}

        public IEnumerable<PropertyInfo> GetMappedProperties()
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

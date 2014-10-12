﻿using System;
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
        public Repository(ISession session, ILogger logger, ISessionEventListener sessionEventListener) //TODO: Session Lazy - need to test perserving context!
            : base(session, logger, sessionEventListener)
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
        public Repository(ISession session, ILogger logger, ISessionEventListener sessionEventListener)
        {
            Logger = logger;
            Session = session;
            SessionEventListener = sessionEventListener;
        }

        protected ILogger Logger { get; private set; }

        protected ISession Session { get; private set; }

        protected IValidatorEngine Validator { get; private set; }

        protected ISessionEventListener SessionEventListener { get; private set; }

        public IQueryable<TModel> GetLinqQuery()
        {
            return Session.Query<TModel>();
        }

        public TModel Get(TId id)
        {
            return Session.Get<TModel>(id);
        }

        public TModel Load(TId id)
        {
            return Session.Load<TModel>(id); //select query will not be executed until a property of the model is accessed
        }

        public void AddAListener(Action action, SessionListenerType listenerType)
        {
            SessionEventListener.AddAListener(listenerType, Session, action);
        }

        public void AddAListener(Action<IRepository<TModel, TId>> action, SessionListenerType listenerType)
        {
            SessionEventListener.AddAListener(listenerType, Session, () => action(this));
        }

        public TModel DeepCopy(TModel model)
        {
            return Session.DeepCopy(model);
        }

        public void Save(TModel model)
        {
            Session.SaveOrUpdate(model); 
        }

        public void Update(TModel model)
        {
            Session.Update(model);
        }

        public void Delete(TModel model)
        {
            Session.Delete(model);
        }

        public void Delete(TId id)
        {
            Delete(Load(id));
        }

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

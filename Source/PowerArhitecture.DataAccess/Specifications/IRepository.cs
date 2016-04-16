using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.Domain;

namespace PowerArhitecture.DataAccess.Specifications
{
    //only needed for generic constraint
    public interface IRepository //: IDisposable
    {
        T Load<T>(object id);

        T Get<T>(object id);

        void Save(object model);

        //T DeepCopy<T>(T model, DeepCopyOptions opts = null) where T : IEntity;

        IQueryable<T> Query<T>() where T : IEntity;
    } 

    public interface IRepository<TModel> : IRepository<TModel, long> where TModel : class, IEntity<long>, new() {}

    public interface IRepository<TModel, TId> : IRepository where TModel : class, IEntity<TId>, new()
    {
        IQueryable<TModel> Query();

        TModel Get(TId id);

        TModel Load(TId id);

        void AddAListener(Action action, SessionListenerType listenerType);

        void AddAListener(Action<IRepository<TModel, TId>> action, SessionListenerType listenerType);

        //ValidationResult Validate(TModel model);

        void Save(TModel model);

        void Update(TModel model);

        void Delete(TModel model);

        void Delete(TId id);

        //TModel DeepCopy(TModel model, DeepCopyOptions opts = null);

        IEnumerable<PropertyInfo> GetMappedProperties();
    }
}

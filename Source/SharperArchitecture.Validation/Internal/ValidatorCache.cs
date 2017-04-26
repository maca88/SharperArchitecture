using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SimpleInjector;

namespace SharperArchitecture.Validation.Internal
{
    internal class ValidatorCache
    {
        private readonly ConcurrentDictionary<Type, ConcurrentSet<InstanceProducer>> _childProducers =
            new ConcurrentDictionary<Type, ConcurrentSet<InstanceProducer>>();

        private readonly ConcurrentDictionary<Type, ConcurrentSet<InstanceProducer>> _rootProducers =
            new ConcurrentDictionary<Type, ConcurrentSet<InstanceProducer>>();

        private readonly HashSet<Type> _genericBusinessRules = new HashSet<Type>();

        public void AddChildProducers<T>(IEnumerable<InstanceProducer> producers)
        {
            var set = _childProducers.GetOrAdd(typeof(T), new ConcurrentSet<InstanceProducer>());
            producers.ForEach(o => set.Add(o));
        }

        public void AddChildProducers(Type type, params InstanceProducer[] producers)
        {
            var set = _childProducers.GetOrAdd(type, new ConcurrentSet<InstanceProducer>());
            producers.ForEach(o => set.Add(o));
        }

        public IEnumerable<InstanceProducer> GetChildProducers<T>()
        {
            return _childProducers.GetOrAdd(typeof(T), new ConcurrentSet<InstanceProducer>());
        }

        public void AddRootProducers<T>(params InstanceProducer[] producers)
        {
            var set = _rootProducers.GetOrAdd(typeof(T), new ConcurrentSet<InstanceProducer>());
            producers.ForEach(o => set.Add(o));
        }

        public void AddRootProducers<T>(IEnumerable<InstanceProducer> producers)
        {
            var set = _rootProducers.GetOrAdd(typeof(T), new ConcurrentSet<InstanceProducer>());
            producers.ForEach(o => set.Add(o));
        }

        public IEnumerable<InstanceProducer> GetRootProducers<T>()
        {
            return _rootProducers.GetOrAdd(typeof(T), new ConcurrentSet<InstanceProducer>());
        }

        public void AddGenericBusinessRule(Type type)
        {
            _genericBusinessRules.Add(type);
        }

        public IEnumerable<Type> GetGenericBusinessRules()
        {
            return _genericBusinessRules;
        }
    }
}

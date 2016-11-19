using System;
using System.Collections.Concurrent;
using SimpleInjector;

namespace PowerArhitecture.Common.SimpleInjector
{
    public class KeyedRegistration
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceProducer>> _serviceRegistrations =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceProducer>>();
        private readonly Container _container;

        public KeyedRegistration(Container container)
        {
            _container = container;
        }

        public bool ContainsRegistration(Type type, string key)
        {
            ConcurrentDictionary<string, InstanceProducer> dict;
            return _serviceRegistrations.TryGetValue(type, out dict) && dict.ContainsKey(key);
        }

        public InstanceProducer GetRegistration(Type type, string key)
        {
            ConcurrentDictionary<string, InstanceProducer> dict;
            InstanceProducer producer;
            if (!_serviceRegistrations.TryGetValue(type, out dict) || !dict.TryGetValue(key, out producer))
            {
                return null;
            }
            return producer;
        }

        public void Register<TService, TImplementation>(string key)
        {
            Register(typeof(TService), typeof(TImplementation), key);
        }

        public void Register(Type serviceType, Type implementationType, string key)
        {
            Register(serviceType, implementationType, key, GetDefaultLifestyle(serviceType, implementationType));
        }

        public void Register(Type serviceType, Type implementationType, string key, Lifestyle lifestyle)
        {
            Register(serviceType, lifestyle.CreateRegistration(serviceType, implementationType, _container), key);
        }

        public void Register(Type serviceType, Func<object> instanceCreator, string key)
        {
            Register(serviceType, instanceCreator, key, GetDefaultLifestyle(serviceType, serviceType));
        }

        public void Register(Type serviceType, Func<object> instanceCreator, string key, Lifestyle lifestyle)
        {
            Register(serviceType, lifestyle.CreateRegistration(serviceType, instanceCreator, _container), key);
        }

        private void Register(Type serviceType, Registration registration, string key)
        {
            var producer = new InstanceProducer(serviceType, registration);
            var serviceProducers = _serviceRegistrations.GetOrAdd(serviceType,
                t => new ConcurrentDictionary<string, InstanceProducer>());

            serviceProducers.AddOrUpdate(key, s => producer, (k, v) => producer);
        }

        private Lifestyle GetDefaultLifestyle(Type serviceType, Type implementationType)
        {
            return _container.Options.LifestyleSelectionBehavior
                .SelectLifestyle(serviceType, implementationType);
        }
    }
}

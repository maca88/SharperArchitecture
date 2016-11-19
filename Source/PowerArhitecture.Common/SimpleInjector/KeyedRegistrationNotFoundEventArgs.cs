using System;
using SimpleInjector;

namespace PowerArhitecture.Common.SimpleInjector
{
    public class KeyedRegistrationNotFoundEventArgs
    {
        private readonly KeyedRegistration _keyedRegistration;

        public KeyedRegistrationNotFoundEventArgs(KeyedRegistration keyedRegistration, Type serviceType, string key, InjectionConsumerInfo consumer)
        {
            _keyedRegistration = keyedRegistration;
            ServiceType = serviceType;
            Key = key;
            Consumer = consumer;
        }

        public InjectionConsumerInfo Consumer { get; }

        public Type ServiceType { get; }

        public string Key { get; }

        public void Register(Type serviceType, Func<object> instanceCreator, string key, Lifestyle lifestyle)
        {
            _keyedRegistration.Register(serviceType, instanceCreator, key, lifestyle);
        }

        public void Register(Type serviceType, Type implementationType, string key, Lifestyle lifestyle)
        {
            _keyedRegistration.Register(serviceType, implementationType, key, lifestyle);
        }
    }
}

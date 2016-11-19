using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerArhitecture.Common.Attributes;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace PowerArhitecture.Common.SimpleInjector
{
    public class KeyedDependencyInjectionBehavior : IDependencyInjectionBehavior
    {
        private readonly IDependencyInjectionBehavior _defaultBehavior;

        public KeyedDependencyInjectionBehavior(Container container)
        {
            _defaultBehavior = container.Options.DependencyInjectionBehavior;
            KeyedRegistration = new KeyedRegistration(container);
        }

        public KeyedRegistration KeyedRegistration { get; }

        private EventHandler<KeyedRegistrationNotFoundEventArgs> _onKeyedRegistrationNotFound;

        public event EventHandler<KeyedRegistrationNotFoundEventArgs> OnKeyedRegistrationNotFound
        {
            add { _onKeyedRegistrationNotFound += value; }
            remove { _onKeyedRegistrationNotFound -= value; }
        }

        public object GetInstance(Type serviceType, string key)
        {
            return GetKeyedRegistration(serviceType, key).GetInstance();
        }

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            var attribute = consumer.Target.GetCustomAttribute<NamedAttribute>();
            if (attribute != null)
            {
                return GetKeyedRegistration(consumer.Target.TargetType, attribute.Name, consumer)
                    .BuildExpression();
            }
            return _defaultBehavior.BuildExpression(consumer);
        }

        private InstanceProducer GetKeyedRegistration(Type serviceType, string key, InjectionConsumerInfo consumer = null)
        {
            var instanceProducer = KeyedRegistration.GetRegistration(serviceType, key);
            if (instanceProducer != null)
            {
                return instanceProducer;
            }
            var eventArgs = new KeyedRegistrationNotFoundEventArgs(KeyedRegistration, serviceType, key, consumer);
            _onKeyedRegistrationNotFound(this, eventArgs);
            instanceProducer = KeyedRegistration.GetRegistration(serviceType, key);
            if (instanceProducer == null)
            {
                throw new ActivationException($"Keyed registration for type {serviceType} and key {key} was not found.");
            }
            return instanceProducer;
        }

        public void Verify(InjectionConsumerInfo consumer)
        {
            _defaultBehavior.Verify(consumer);
        }
    }
}

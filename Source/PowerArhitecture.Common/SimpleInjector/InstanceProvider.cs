using System;
using System.Collections.Generic;
using PowerArhitecture.Common.Specifications;
using SimpleInjector;

namespace PowerArhitecture.Common.SimpleInjector
{
    public class InstanceProvider : IInstanceProvider
    {
        private readonly Container _container;

        public InstanceProvider(Container container)
        {
            _container = container;
        }

        public object Get(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }

        public TService Get<TService>() where TService : class
        {
            return _container.GetInstance<TService>();
        }

        public IEnumerable<TService> GetAll<TService>() where TService : class
        {
            return _container.GetAllInstances<TService>();
        }

        public IEnumerable<object> GetAll(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }
    }
}

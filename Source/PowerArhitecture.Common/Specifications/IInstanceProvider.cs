using System;
using System.Collections.Generic;

namespace PowerArhitecture.Common.Specifications
{
    public interface IInstanceProvider
    {
        object Get(Type serviceType);

        TService Get<TService>() where TService : class;

        IEnumerable<TService> GetAll<TService>() where TService : class;

        IEnumerable<object> GetAll(Type serviceType);
    }
}

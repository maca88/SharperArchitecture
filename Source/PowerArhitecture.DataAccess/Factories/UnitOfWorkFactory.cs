using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;

namespace PowerArhitecture.DataAccess.Factories
{
    internal class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly Container _container;
        private readonly IEventSubscriber _eventSubscriber;

        public UnitOfWorkFactory(Container container, IEventSubscriber eventSubscriber)
        {
            _container = container;
            _eventSubscriber = eventSubscriber;
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var unitOfWork = new UnitOfWork(_container, _eventSubscriber, isolationLevel);
            return unitOfWork;
        }
    }
}

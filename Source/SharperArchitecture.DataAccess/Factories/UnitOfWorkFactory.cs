using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Specifications;
using SimpleInjector;

namespace SharperArchitecture.DataAccess.Factories
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace PowerArhitecture.DataAccess.Factories
{
    internal class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        readonly Container _container;

        public UnitOfWorkFactory(Container container)
        {
            _container = container;
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var unitOfWork = _container.GetInstance<UnitOfWork>();
            unitOfWork.IsolationLevel = isolationLevel;
            return unitOfWork;
        }
    }
}

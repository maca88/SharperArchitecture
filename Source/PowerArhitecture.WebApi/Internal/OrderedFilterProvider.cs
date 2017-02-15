using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.WebApi.Specifications;
using SimpleInjector;

namespace PowerArhitecture.WebApi.Internal
{
    internal class OrderedFilterProvider : IFilterProvider
    {
        private readonly Container _container;

        public OrderedFilterProvider(Container container)
        {
            _container = container;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var globalFilters = new List<IFilter>();
            var controllerFilters = new List<IFilter>(actionDescriptor.ControllerDescriptor.GetFilters());
            var actionFilters = new List<IFilter>(actionDescriptor.GetFilters());

            var registeredFilters = configuration.DependencyResolver.GetServices(typeof(IFilter));
            foreach (var registeredFilter in registeredFilters.OfType<IFilter>())
            {
                var info = _container.GetHttpFilterInfo(registeredFilter);
                var canExecute = info.Predicate?.Invoke(actionDescriptor);
                if (canExecute.HasValue && canExecute.Value == false)
                {
                    continue;
                }
                switch (info.FilterScope)
                {
                    case FilterScope.Global:
                        globalFilters.Add(registeredFilter);
                        break;
                    case FilterScope.Controller:
                        controllerFilters.Add(registeredFilter);
                        break;
                    case FilterScope.Action:
                        actionFilters.Add(registeredFilter);
                        break;
                }

            }

            return OrderFilters(globalFilters, FilterScope.Global)
                .Concat(OrderFilters(controllerFilters, FilterScope.Controller))
                .Concat(OrderFilters(actionFilters, FilterScope.Action));
        }

        private IEnumerable<FilterInfo> OrderFilters(ICollection<IFilter> filters, FilterScope scope)
        {
            return filters
                .Select(o => new
                {
                    FilterInfo = new FilterInfo(o, scope),
                    Priority = o.GetType().GetPriority()
                })
                .OrderByDescending(o => o.Priority)
                .Select(o => o.FilterInfo);
        }
    }
}

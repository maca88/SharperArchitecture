using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.OData.Query;
using Breeze.ContextProvider.NH;
using Breeze.WebApi2;
using Newtonsoft.Json;

namespace PowerArhitecture.Breeze
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PABreezeNHControllerAttribute : BreezeControllerAttribute
    {
        public PABreezeNHControllerAttribute()
        {
            this._queryableFilter = new PABreezeNHQueryableAttribute();
        }

        public override void Initialize(HttpControllerSettings settings, HttpControllerDescriptor descriptor)
        {
            base.Initialize(settings, descriptor);
            var jsonFormatter = settings.Formatters.JsonFormatter;
            if (jsonFormatter == null) return;
            var serializeSettings = jsonFormatter.SerializerSettings ?? new JsonSerializerSettings();
            serializeSettings.Converters.Add(new NHibernateProxyJsonConverter());
            serializeSettings.ContractResolver = NHibernateContractResolver.Instance;
        }

        protected override IFilterProvider GetQueryableFilterProvider(BreezeQueryableAttribute defaultFilter)
        {
            return new PABreezeNHQueryableFilterProvider(defaultFilter);
        }

    }

    internal class PABreezeNHQueryableFilterProvider : IFilterProvider
    {
        public PABreezeNHQueryableFilterProvider(BreezeQueryableAttribute filter)
        {
            _filter = filter;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null ||
              actionDescriptor.GetCustomAttributes<QueryableAttribute>().Any() || // if method already has a QueryableAttribute (or subclass) then skip it.
              actionDescriptor.GetParameters().Any(parameter => typeof(ODataQueryOptions).IsAssignableFrom(parameter.ParameterType))
            )
            {
                return Enumerable.Empty<FilterInfo>();
            }

            return new[] { new FilterInfo(_filter, FilterScope.Global) };
        }

        private readonly BreezeQueryableAttribute _filter;
    }
}

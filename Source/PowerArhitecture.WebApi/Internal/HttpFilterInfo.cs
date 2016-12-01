using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PowerArhitecture.WebApi.Internal
{
    internal class HttpFilterInfo
    {
        public HttpFilterInfo(FilterScope scope, Predicate<HttpActionDescriptor> predicate = null)
        {
            FilterScope = scope;
            Predicate = predicate;
        }

        public FilterScope FilterScope { get; }

        public Predicate<HttpActionDescriptor> Predicate { get; }
    }
}

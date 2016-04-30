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
using NHibernate;

namespace PowerArhitecture.Breeze
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PABreezeNHControllerAttribute : BreezeNHControllerAttribute
    {
        public PABreezeNHControllerAttribute()
        {
            this._queryableFilter = new PABreezeNHQueryableAttribute();
        }
    }
}

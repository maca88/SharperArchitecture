using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NHibernate;
using NHibernate.Linq;

namespace PowerArhitecture.Breeze
{
    public class PABreezeController : ApiController
    {
        protected internal static readonly MethodInfo SessionQueryMethod = typeof(LinqExtensionMethods).GetMethods()
            .First(o => o.Name == "Query" && o.GetParameters().First().ParameterType == typeof(ISession));

        public PABreezeController(ISession session)
        {
            Session = session;
        }

        protected internal ISession Session { get; set; }
    }
}

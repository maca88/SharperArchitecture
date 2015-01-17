using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;
using Breeze.ContextProvider.NH;
using Breeze.WebApi2;

namespace PowerArhitecture.Breeze
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class PABreezeNHQueryableAttribute : BreezeQueryableAttribute
    {
        /// <summary>
        /// Sets HandleNullPropagation = false on the base class.  Otherwise it's true for non-EF, and that
        /// complicates the query expressions and breaks NH's query parser.
        /// </summary>
        public PABreezeNHQueryableAttribute()
        {
          HandleNullPropagation = HandleNullPropagationOption.False;
        }

        protected override QueryHelper NewQueryHelper()
        {
            return new PANHQueryHelper(GetODataQuerySettings());
        }
    }
}

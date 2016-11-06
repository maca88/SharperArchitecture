using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.OData.Query;
using Breeze.ContextProvider.NH;
using Breeze.WebApi2;
using NHibernate.Linq;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Breeze
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CustomBreezeNHQueryableAttribute : BreezeNHQueryableAttribute
    {
        private static readonly MethodInfo DistinctMethodInfo;

        static CustomBreezeNHQueryableAttribute()
        {
            DistinctMethodInfo =
                typeof (Queryable).GetMethods().First(o => o.Name == "Distinct" && o.GetParameters().Length == 1);
        }

    /// <summary>
        /// Sets HandleNullPropagation = false on the base class.  Otherwise it's true for non-EF, and that
        /// complicates the query expressions and breaks NH's query parser.
        /// </summary>
        public CustomBreezeNHQueryableAttribute()
        {
            HandleNullPropagation = HandleNullPropagationOption.False;
        }

        protected override QueryHelper NewQueryHelper()
        {
            return new CustomNHQueryHelper(GetODataQuerySettings());
        }

        //TODO: TEST AND REMOVE
        //public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        //{
        //    base.OnActionExecuted(actionExecutedContext);

        //    var request = actionExecutedContext.Request;
        //    var returnType = actionExecutedContext.ActionContext.ActionDescriptor.ReturnType;
        //    if(typeof(IQueryable).IsAssignableFrom(returnType))
        //        return;
        //    // FIX this: session factory may not be set in the nh resolver
        //    var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as PABreezeController;
        //    if (!typeof (IEntity).IsAssignableFrom(returnType) || controller == null) return;
        //    var queryHelper = GetQueryHelper(request);
        //    var queryResult = (IQueryable)PABreezeController.SessionQueryMethod.MakeGenericMethod(returnType)
        //        .Invoke(null, new object[] { controller.Session });
        //    queryHelper.ConfigureFormatter(actionExecutedContext.Request, queryResult);
        //    queryHelper.Close(queryResult);
        //}

        //public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        //{
        //    await  base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

        //    var request = actionExecutedContext.Request;
        //    var returnType = actionExecutedContext.ActionContext.ActionDescriptor.ReturnType;
        //    if (typeof(IQueryable).IsAssignableFrom(returnType))
        //        return;

        //    var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as PABreezeController;
        //    if (!typeof(IEntity).IsAssignableFrom(returnType) || controller == null) return;
        //    var queryHelper = (NHQueryHelper)GetQueryHelper(request);
        //    var queryResult = (IQueryable)PABreezeController.SessionQueryMethod.MakeGenericMethod(returnType)
        //        .Invoke(null, new object[] { controller.Session });
        //    await queryHelper.ConfigureFormatterAsync(actionExecutedContext.Request, queryResult);
        //    queryHelper.Close(queryResult);

        //}

        /// <summary>
        /// All standard OData web api support is handled here (except select and expand).
        /// This method also handles nested orderby statements the the current ASP.NET web api does not yet support.
        /// This method is called by base.OnActionExecuted
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            var queryHelper = GetQueryHelper(queryOptions.Request);
            queryable = queryHelper.BeforeApplyQuery(queryable, queryOptions);
            queryable = queryHelper.ApplyQuery(queryable, queryOptions);
            var paramValues = queryOptions.Request.GetQueryNameValuePairs().ToDictionary(o => o.Key, o => o.Value);
            var qType = queryable.GetType();
            if (paramValues.ContainsKey("_distinct") && qType.IsGenericType)
            {
                queryable = (IQueryable)DistinctMethodInfo
                    .MakeGenericMethod(qType.GenericTypeArguments.First())
                    .Invoke(null, new[] { queryable });
            }
            return queryable;
        }
    }
}

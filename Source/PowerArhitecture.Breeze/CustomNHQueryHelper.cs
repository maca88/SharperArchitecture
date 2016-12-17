﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;
using Breeze.ContextProvider.NH;
using NHibernate.Linq;

namespace PowerArhitecture.Breeze
{
    public class CustomNHQueryHelper : NHQueryHelper
    {
        public CustomNHQueryHelper(ODataQuerySettings querySettings)
            : base(querySettings)
        {
        }

        public CustomNHQueryHelper()
        {
        }

        public override IQueryable BeforeApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            queryable = base.BeforeApplyQuery(queryable, queryOptions);
            if (expandPaths == null) return queryable;

            foreach (var path in expandPaths)
            {
                queryable.Include(path.Replace("/", "."));
            }

            expandPaths = null;

            return queryable;
        }
    }
}
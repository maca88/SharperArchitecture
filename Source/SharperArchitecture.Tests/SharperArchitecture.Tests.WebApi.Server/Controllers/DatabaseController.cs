using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NHibernate.Stat;
using SharperArchitecture.DataAccess.Specifications;

namespace SharperArchitecture.Tests.WebApi.Server.Controllers
{
    public class DatabaseController : ApiController
    {
        private readonly ISessionFactoryProvider _sessionFactoryProvider;

        public DatabaseController(ISessionFactoryProvider sessionFactoryProvider)
        {
            _sessionFactoryProvider = sessionFactoryProvider;
        }

        public IStatistics GetStatistics(string dbConfigName = null)
        {
            var sf = _sessionFactoryProvider.Get(dbConfigName);
            return sf.Statistics;
        }

        public void ClearStatistics(string dbConfigName = null)
        {
            var sf = _sessionFactoryProvider.Get(dbConfigName);
            sf.Statistics.Clear();
        }
    }
}
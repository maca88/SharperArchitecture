using System.Data;
using FluentNHibernate.Conventions;
using NHibernate.Cfg;
using NHibernate.Dialect;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISchemaConvention : IConvention
    {
        bool CanApply(Dialect dialect);

        void Setup(Configuration configuration);

        void ApplyBeforeExecutingQuery(NHibernate.Cfg.Configuration config, IDbConnection connection, IDbCommand dbCommand);

        void ApplyAfterExecutingQuery(NHibernate.Cfg.Configuration config, IDbConnection connection, IDbCommand dbCommand);
    }
}

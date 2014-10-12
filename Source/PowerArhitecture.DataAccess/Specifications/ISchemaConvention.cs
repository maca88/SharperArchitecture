using System.Data;
using FluentNHibernate.Conventions;
using NHibernate.Dialect;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISchemaConvention : IConvention
    {
        bool CanApply(Dialect dialect);

        void ApplyBeforeExecutingQuery(NHibernate.Cfg.Configuration config, IDbConnection connection, IDbCommand dbCommand);

        void ApplyAfterExecutingQuery(NHibernate.Cfg.Configuration config, IDbConnection connection, IDbCommand dbCommand);
    }
}

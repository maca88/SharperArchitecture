using System.Data;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ICreateSchemaConvention : ISchemaConvention
    {
        void ApplyBeforeSchemaCreate(NHibernate.Cfg.Configuration config, IDbConnection connection);

        void ApplyAfterSchemaCreate(NHibernate.Cfg.Configuration config, IDbConnection connection);
    }
}

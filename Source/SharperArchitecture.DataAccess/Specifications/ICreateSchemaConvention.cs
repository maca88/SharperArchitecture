using System.Data;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface ICreateSchemaConvention : ISchemaConvention
    {
        void ApplyBeforeSchemaCreate(NHibernate.Cfg.Configuration config, IDbConnection connection);

        void ApplyAfterSchemaCreate(NHibernate.Cfg.Configuration config, IDbConnection connection);
    }
}

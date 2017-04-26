using System.Data;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IUpdateSchemaConvention : ISchemaConvention
    {
        void ApplyBeforeSchemaUpdate(NHibernate.Cfg.Configuration config, IDbConnection connection);

        void ApplyAfterSchemaUpdate(NHibernate.Cfg.Configuration config, IDbConnection connection);
    }
}

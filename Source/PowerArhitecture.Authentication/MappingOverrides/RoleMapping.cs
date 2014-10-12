using PowerArhitecture.Authentication.Entities;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace PowerArhitecture.Authentication.MappingOverrides
{
    public class RoleMapping : IAutoMappingOverride<Role>
    {
        public void Override(AutoMapping<Role> mapping)
        {
        }
    }
}

using PowerArhitecture.Authentication.Entities;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace PowerArhitecture.Authentication.MappingOverrides
{
    public class RightMapping : IAutoMappingOverride<Permission>
    {
        public void Override(AutoMapping<Permission> mapping)
        {
        }
    }
}

using System;
using PowerArhitecture.Authentication.Entities;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace PowerArhitecture.Authentication.MappingOverrides
{
    public class UserMapping : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
        }
    }
}

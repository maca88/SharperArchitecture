using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Generated;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.Authentication.Alternations
{
    public class EntityAlternation : IAutoMappingAlteration
    {
        public void Alter(AutoPersistenceModel model)
        {
            var organizationClass = AppConfiguration.GetSetting<string>(AuthenticationSettingKeys.OrganizationClass);
            var organizationType = typeof (Organization);
            if (!string.IsNullOrEmpty(AppConfiguration.GetSetting<string>(AuthenticationSettingKeys.UserClass)))
            {
                model.IgnoreBase(typeof(User));
            }

            if (!string.IsNullOrEmpty(organizationClass))
            {
                model.IgnoreBase(typeof(Organization));
                organizationType = Type.GetType(organizationClass, true);
            }
            model.IgnoreBase(typeof(User<>).MakeGenericType(organizationType));
        }
    }
}

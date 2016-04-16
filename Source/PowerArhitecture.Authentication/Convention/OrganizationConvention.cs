using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using PowerArhitecture.Authentication.Configurations;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.Authentication.Convention
{
    //public class OrganizationConvention : IReferenceConvention
    //{
    //    private readonly Type _organizationType;

    //    public OrganizationConvention()
    //    {
    //        var organizationClass = AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.OrganizationClass);
    //        _organizationType = Type.GetType(organizationClass, true);
    //    }

    //    public void Apply(IManyToOneInstance instance)
    //    {
    //        if (!typeof(IOrganization).IsAssignableFrom(instance.Property.PropertyType)) return;

    //        instance.OverrideInferredClass(_organizationType);
    //    }
    //}
}

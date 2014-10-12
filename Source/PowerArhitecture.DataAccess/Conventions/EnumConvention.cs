using PowerArhitecture.Common.Attributes;
using Castle.Core.Internal;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Type;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class EnumConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        #region IPropertyConvention Members

        public void Apply(IPropertyInstance instance)
        {
            var attr = instance.Property.MemberInfo.GetAttribute<EnumStringAttribute>();
            if (attr == null)
                instance.CustomType(instance.Property.PropertyType);
            else
            {
                var customType = typeof (EnumStringType<>).MakeGenericType(instance.Property.PropertyType);
                instance.CustomType(customType);
            }
        }

        #endregion

        #region IPropertyConventionAcceptance Members

        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum);
        }

        #endregion

    }
}

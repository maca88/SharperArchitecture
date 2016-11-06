using System.Reflection;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Validation.Attributes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class NotEmptyAttributeConvention : AttributePropertyConvention<NotEmptyAttribute>, IReferenceConvention
    {
        protected override void Apply(NotEmptyAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }

        public void Apply(IManyToOneInstance instance)
        {
            var attribute = instance.Property.MemberInfo.GetCustomAttribute<NotEmptyAttribute>();
            if (attribute == null) return;
            instance.Not.Nullable();
        }
    }
}

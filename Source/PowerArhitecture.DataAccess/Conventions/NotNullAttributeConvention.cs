using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Validation.Attributes;
using Castle.Core.Internal;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class NotNullAttributeConvention : AttributePropertyConvention<NotNullAttribute>, IReferenceConvention
    {
        protected override void Apply(NotNullAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }

        public void Apply(IManyToOneInstance instance)
        {
            var attribute = instance.Property.MemberInfo.GetAttribute<NotNullAttribute>();
            if (attribute == null) return;
            instance.Not.Nullable();
        }
    }
}

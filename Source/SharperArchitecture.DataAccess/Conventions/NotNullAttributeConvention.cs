using System.Reflection;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Validation.Attributes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class NotNullAttributeConvention : AttributePropertyConvention<NotNullAttribute>, IReferenceConvention
    {
        protected override void Apply(NotNullAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }

        public void Apply(IManyToOneInstance instance)
        {
            var attribute = instance.Property.MemberInfo.GetCustomAttribute<NotNullAttribute>();
            if (attribute == null) return;
            instance.Not.Nullable();
        }
    }
}

using PowerArhitecture.Domain.Attributes;
using Castle.Core.Internal;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class UniqueAttributeConvention : AttributePropertyConvention<UniqueAttribute>, IReferenceConvention
    {
        protected override void Apply(UniqueAttribute attribute, IPropertyInstance instance)
        {
            if(attribute.IsKeySet)
                instance.UniqueKey(string.Format("UQ_{0}", attribute.KeyName));
            else
                instance.Unique();
        }

        public void Apply(IManyToOneInstance instance)
        {
            var attribute = instance.Property.MemberInfo.GetAttribute<UniqueAttribute>();
            if (attribute == null) return;
            if (attribute.IsKeySet)
                instance.UniqueKey(string.Format("UQ_{0}", attribute.KeyName));
            else
                instance.Unique();

        }
    }
}

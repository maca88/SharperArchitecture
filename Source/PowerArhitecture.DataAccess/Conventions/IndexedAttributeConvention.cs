using PowerArhitecture.Domain.Attributes;
using Castle.Core.Internal;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class IndexedAttributeConvention : AttributePropertyConvention<IndexAttribute>, IReferenceConvention
    {
        protected override void Apply(IndexAttribute attribute, IPropertyInstance instance)
        {
            instance.Index(attribute.IsKeySet 
                ? GetIndexName(instance.EntityType.Name, attribute.KeyName) 
                : GetIndexName(instance.EntityType.Name, instance.Name));
        }

        public void Apply(IManyToOneInstance instance)
        {
            var attribute = instance.Property.MemberInfo.GetAttribute<IndexAttribute>();
            if (attribute == null) return;
            instance.Index(attribute.IsKeySet
                ? GetIndexName(instance.EntityType.Name, attribute.KeyName)
                : GetIndexName(instance.EntityType.Name, instance.Name));
        }

        private static string GetIndexName(string tableName, string name)
        {
            return string.Format("IX_{0}_{1}", tableName, name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class ForeignKeyColumnNameConvention : IReferenceConvention, IHasManyToManyConvention, IJoinedSubclassConvention, IJoinConvention, ICollectionConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            var keyName = GetKeyName(instance.Property, instance.Class.GetUnderlyingSystemType());
            instance.Column(keyName);
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            var keyName1 = GetKeyName(null, instance.EntityType);
            var keyName2 = GetKeyName(null, instance.ChildType);
            instance.Key.Column(keyName1);
            instance.Relationship.Column(keyName2);
        }

        public void Apply(IJoinedSubclassInstance instance)
        {
            var keyName = GetKeyName(null, instance.Type.BaseType);
            instance.Key.Column(keyName);
        }

        public void Apply(IJoinInstance instance)
        {
            var keyName = GetKeyName(null, instance.EntityType);
            instance.Key.Column(keyName);
        }

        public void Apply(ICollectionInstance instance)
        {
            var type = instance.Member.DeclaringType == instance.EntityType
                ? instance.EntityType
                : instance.Member.DeclaringType;

            var keyName = GetKeyName(null, type);
            instance.Key.Column(keyName);
        }

        protected string GetKeyName(Member property, Type type)
        {
            return (property != (Member)null ? property.Name : GetClearTypeName(type)) + "Id";
        }

        /// <summary>
        /// Clear the type name if is a generic one
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetClearTypeName(Type type)
        {
            return type.IsGenericType ? type.Name.Remove(type.Name.IndexOf('`')) : type.Name;
        }
    }
}

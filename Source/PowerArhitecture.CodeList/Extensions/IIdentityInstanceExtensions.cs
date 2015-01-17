using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.MappingModel;

namespace PowerArhitecture.CodeList.Extensions
{
    internal static class FluentNHibernateExtensions
    {
        public static IEnumerable<ColumnMapping> GetColumns(this IKeyInspector instance)
        {
            return instance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns");
        }

        public static IEnumerable<ColumnMapping> GetColumns(this IManyToOneInspector instance)
        {
            return instance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns");
        }

        public static IEnumerable<ColumnMapping> GetColumns(this IPropertyInspector instance)
        {
            return instance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns");
        }

        public static IEnumerable<ColumnMapping> GetColumns(this IIdentityInstance identityInstance)
        {
            return identityInstance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns");
        }

        public static IEnumerable<ColumnMapping> GetColumns(this IKeyInstance instance)
        {
            return instance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns");
        }

        public static IEnumerable<ColumnMapping> GetColumns(this IManyToOneInstance instance)
        {
            return instance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns");
        }
    }
}

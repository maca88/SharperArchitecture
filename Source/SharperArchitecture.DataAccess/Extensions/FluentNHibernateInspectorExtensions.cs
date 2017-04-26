using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Identity;
using Remotion.Linq.Utilities;

namespace SharperArchitecture.DataAccess.Extensions
{
    public static class FluentNHibernateInspectorExtensions
    {

        public static ColumnMapping GetMapping(this IColumnInspector inspector)
        {
            var type = inspector.GetType();
            var field = type.GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingMemberException(string.Format("field 'mapping' was not found in type {0}", type));
            return field.GetValue(inspector) as ColumnMapping;
        }

        public static PropertyMapping GetMapping(this IPropertyInspector inspector)
        {
            var type = inspector.GetType();
            var field = type.GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingMemberException(string.Format("field 'mapping' was not found in type {0}", type));
            return field.GetValue(inspector) as PropertyMapping;
        }

        public static VersionMapping GetMapping(this IVersionInspector inspector)
        {
            var type = inspector.GetType();
            var field = type.GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingMemberException(string.Format("field 'mapping' was not found in type {0}", type));
            return field.GetValue(inspector) as VersionMapping;
        }

        public static IdMapping GetMapping(this IIdentityInspectorBase inspector)
        {
            var type = inspector.GetType();
            if (typeof (CompositeIdentityInspector) == type)
                throw new ArgumentException(
                    "inspector is of type CompositeIdentityInspector, call GetCompositeMapping instead");

            var field = type.GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingMemberException(string.Format("field 'mapping' was not found in type {0}", type));
            return field.GetValue(inspector) as IdMapping;
        }

        public static CompositeIdMapping GetCompositeMapping(this IIdentityInspectorBase inspector)
        {
            var type = inspector.GetType();
            if (typeof(IdentityInspector) == type)
                throw new ArgumentException(
                    "inspector is of type IdentityInspector, call GetMapping instead");

            var field = type.GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingMemberException(string.Format("field 'mapping' was not found in type {0}", type));
            return field.GetValue(inspector) as CompositeIdMapping;
        }
    }
}

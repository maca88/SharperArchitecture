using FluentNHibernate;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.MappingModel.Identity;
using PowerArhitecture.CodeList.Attributes;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PowerArhitecture.CodeList.EventListeners
{
    public class HibernateMappingsBuiltEventListener : IListener<HibernateMappingsBuiltEvent>
    {
        public void Apply(ClassMappingBase classMapBase)
        {
            Type type = classMapBase.Type;


            foreach (var reference in classMapBase.References.Where(o => typeof(ICodeList).IsAssignableFrom(o.Member.PropertyType)))
            {
                var refColumn = Apply(reference);
                var synteticColumn = classMapBase.Properties.SelectMany(o => o.Columns).FirstOrDefault(o => o.Name == refColumn.Name);
                if (synteticColumn != null)
                {
                    synteticColumn.Set(o => o.Length, Layer.UserSupplied, refColumn.Length);
                    synteticColumn.Set(o => o.NotNull, Layer.UserSupplied, refColumn.NotNull);
                }
            }

            foreach (var collection in classMapBase.Collections.Where(o => typeof(ICodeList).IsAssignableFrom(o.ContainingEntityType)))
            {
                Apply(collection);
            }

            foreach (var subClass in classMapBase.Subclasses)
            {
                Apply(subClass);
            }

            foreach (var component in classMapBase.Components)
            {
                Apply(component);
            }

            if (!typeof(ICodeList).IsAssignableFrom(type)) return;

            var codeListAttr = type.GetCustomAttribute<GenerateCodeListAttribute>(false);
            var length = (codeListAttr != null) ? codeListAttr.CodeLength : 20;

            var classMap = classMapBase as ClassMapping;
            if (classMap == null) return;
            var tableName = classMap.TableName.Trim(new char[] { '`' });
            if (tableName.EndsWith("CodeList"))
            {
                tableName = tableName.Substring(0, tableName.IndexOf("CodeList", StringComparison.InvariantCulture));
                tableName = "CodeList" + tableName;
            }
            else if (!tableName.StartsWith("CodeList"))
            {
                tableName = "CodeList" + tableName;
            }
            var id = classMap.Id as IdMapping;
            if (id != null)
            {
                var idCol = id.Columns.First();
                idCol.Set(o => o.Name, Layer.UserSupplied, "Code");
                idCol.Set(o => o.Length, Layer.UserSupplied, length);
            }

            if (codeListAttr != null && !string.IsNullOrEmpty(codeListAttr.ViewName))
            {
                tableName = codeListAttr.ViewName;
                classMap.Set(o => o.Mutable, Layer.UserSupplied, false);
            }

            classMap.Set(o => o.TableName, Layer.UserSupplied, tableName);
        }

        public void Apply(IComponentMapping componentMap)
        {
            foreach (var reference in componentMap.References.Where(o => typeof(ICodeList).IsAssignableFrom(o.Member.PropertyType)))
            {
                var refColumn = Apply(reference);
                var synteticColumn = componentMap.Properties.SelectMany(o => o.Columns).FirstOrDefault(o => o.Name == refColumn.Name);
                if (synteticColumn == null) continue;
                synteticColumn.Set(o => o.Length, Layer.UserSupplied, refColumn.Length);
                synteticColumn.Set(o => o.NotNull, Layer.UserSupplied, refColumn.NotNull);
            }

            foreach (var collection in componentMap.Collections.Where(o => typeof(ICodeList).IsAssignableFrom(o.ContainingEntityType)))
            {
                Apply(collection);
            }

            foreach (var component in componentMap.Components)
            {
                Apply(component);
            }
        }

        public ColumnMapping Apply(CollectionMapping colectionMap)
        {
            var keyName = GetKeyName(null, colectionMap.ContainingEntityType);
            var codeListAttr = colectionMap.ContainingEntityType.GetCustomAttribute<GenerateCodeListAttribute>(false);
            var length = (codeListAttr != null) ? codeListAttr.CodeLength : 20;
            var col = colectionMap.Key.Columns.First();
            col.Set(o => o.Name, Layer.UserSupplied, keyName);
            col.Set(o => o.Length, Layer.UserSupplied, length);
            return col;
        }

        public ColumnMapping Apply(ManyToOneMapping manyToOneMap)
        {
            var codeListAttr = manyToOneMap.Member.PropertyType.GetCustomAttribute<GenerateCodeListAttribute>(false);
            var length = (codeListAttr != null) ? codeListAttr.CodeLength : 20;
            var keyName = GetKeyName(manyToOneMap.Member, manyToOneMap.Class.GetUnderlyingSystemType());
            var col = manyToOneMap.Columns.First();
            col.Set(o => o.Name, Layer.UserSupplied, keyName);
            col.Set(o => o.Length, Layer.UserSupplied, length);
            return col;
        }

        protected string GetKeyName(Member property, Type type)
        {
            return (property != null ? property.Name : type.Name) + "Code";
        }

        public void Handle(HibernateMappingsBuiltEvent e)
        {
            IEnumerable<HibernateMapping> message = e.Message;

            foreach (var classMap in message.SelectMany(o => o.Classes))
            {
                Apply(classMap);
            }
        }
    }
}


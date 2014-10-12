using System;
using System.Data;
using System.Linq;
using System.Reflection;
using PowerArhitecture.CodeList.Attributes;
using PowerArhitecture.CodeList.Extensions;
using PowerArhitecture.CodeList.Specifications;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.MappingModel;

namespace PowerArhitecture.CodeList.Conventions
{
    public class CodeListConvention : IClassConvention, IIdConvention
    {
        public void Apply(IClassInstance instance)
        {
            if(!typeof(ICodeList).IsAssignableFrom(instance.EntityType)) 
                return;

            var tableName = instance.EntityType.Name;
            if (tableName.EndsWith("CodeList"))
            {
                tableName = tableName.Substring(0, tableName.IndexOf("CodeList", StringComparison.InvariantCulture));
                instance.Table("CodeList" + tableName);
            }

            var attribute = instance.EntityType.GetCustomAttribute<GenerateCodeListAttribute>(false);
            if(attribute == null) 
                return;

            if (!string.IsNullOrEmpty(attribute.ViewName))
            {
                instance.Table(attribute.ViewName);
                instance.ReadOnly();
            }
        }

        public void Apply(IIdentityInstance instance)
        {
            if (!typeof(ICodeList).IsAssignableFrom(instance.EntityType))
                return;
            var attribute = instance.EntityType.GetCustomAttribute<GenerateCodeListAttribute>(false);
            if (attribute == null)
                return;
            instance.GetColumns().First().Set(o => o.Name, Layer.UserSupplied, "Code");
            instance.Length(attribute.CodeLength);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Mapping;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class TableAttributeConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            var tableAttr = instance.EntityType.GetCustomAttribute<TableAttribute>();
            if(tableAttr == null) return;

            if(!string.IsNullOrEmpty(tableAttr.Name))
                instance.Table(tableAttr.Name);

            if (tableAttr.View)
            {
                instance.ReadOnly();
                instance.SchemaAction.None();
            }
                
        }
    }
}

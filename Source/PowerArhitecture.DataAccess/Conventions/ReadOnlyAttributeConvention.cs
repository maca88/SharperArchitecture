using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class ReadOnlyAttributeConvention : AttributePropertyConvention<ReadOnlyAttribute>
    {
        protected override void Apply(ReadOnlyAttribute attribute, IPropertyInstance instance)
        {
            if (attribute.IsReadOnly)
                instance.ReadOnly();
            else
                instance.Not.ReadOnly();
        }
    }
}

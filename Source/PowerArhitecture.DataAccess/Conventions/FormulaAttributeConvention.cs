using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.DataAccess.Conventions
{
    class FormulaAttributeConvention : AttributePropertyConvention<FormulaAttribute>
    {
        protected override void Apply(FormulaAttribute attribute, IPropertyInstance instance)
        {
            instance.Formula(attribute.SqlFormula);
        }

    }
}

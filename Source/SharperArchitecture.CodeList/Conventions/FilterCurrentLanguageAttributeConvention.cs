using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using SharperArchitecture.CodeList.Attributes;
using SharperArchitecture.DataAccess.Extensions;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class FilterCurrentLanguageAttributeConvention : AttributePropertyConvention<FilterCurrentLanguageAttribute>
    {
        protected override void Apply(FilterCurrentLanguageAttribute attribute, IPropertyInstance instance)
        {
            // The real formula will be set in the NHMappings event listener. 
            // We need to set the formula here in order to prevent fluent nh to create a column mapping.
            instance.Formula("Id"); 
        }

    }
}

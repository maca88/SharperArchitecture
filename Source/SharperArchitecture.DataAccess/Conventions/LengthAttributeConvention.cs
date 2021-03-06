﻿using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Validation.Attributes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace SharperArchitecture.DataAccess.Conventions
{
    class LengthAttributeConvention : AttributePropertyConvention<LengthAttribute>
    {
        protected override void Apply(LengthAttribute attribute, IPropertyInstance instance)
        {
            //http://stackoverflow.com/questions/2343105/override-for-fluent-nhibernate-for-long-text-strings-nvarcharmax-not-nvarchar
            instance.Length(attribute.Max == int.MaxValue ? 10000 : attribute.Max);
        }
    }
}

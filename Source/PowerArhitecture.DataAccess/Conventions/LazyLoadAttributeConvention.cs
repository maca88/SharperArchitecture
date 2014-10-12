using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class LazyLoadAttributeConvention : AttributePropertyConvention<LazyLoadAttribute>
    {
        protected override void Apply(LazyLoadAttribute attribute, IPropertyInstance instance)
        {
            instance.LazyLoad();
        }
    }
}

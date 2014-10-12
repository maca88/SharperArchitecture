using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class LazyLoadingConvention : IReferenceConvention, IHasManyConvention, IHasOneConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.LazyLoad();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.ExtraLazyLoad();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.ExtraLazyLoad();
        }

        public void Apply(IOneToOneInstance instance)
        {
            instance.LazyLoad();
        }
    }
}

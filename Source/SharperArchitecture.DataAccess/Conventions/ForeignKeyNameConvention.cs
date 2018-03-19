using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class ForeignKeyNameConvention : IReferenceConvention, IHasOneConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.ForeignKey($"FK_{instance.EntityType.Name}_{instance.Name}");
        }

        public void Apply(IOneToOneInstance instance)
        {
            instance.ForeignKey($"FK_{instance.EntityType.Name}_{instance.Name}");
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Relationship.ForeignKey($"FK_{instance.EntityType.Name}_{((ICollectionInspector) instance).Name}");
        }
    }
}

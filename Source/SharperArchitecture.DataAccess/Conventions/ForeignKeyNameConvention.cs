using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class ForeignKeyNameConvention : IReferenceConvention, IHasOneConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.ForeignKey(string.Format("FK_{0}To{1}_{2}", instance.EntityType.Name, instance.Class.Name, instance.Name));
        }

        public void Apply(IOneToOneInstance instance)
        {
            var oneToOne = instance as IOneToOneInspector;
            instance.ForeignKey(string.Format("FK_{0}To{1}_{2}", instance.EntityType.Name, oneToOne.Class.Name, instance.Name));
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Relationship.ForeignKey(string.Format("FK_{0}{1}_{2}", instance.EntityType.Name, instance.OtherSide.EntityType.Name,
                ((ICollectionInspector)instance).Name));
        }
    }
}

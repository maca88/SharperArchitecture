using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.Tests.Common.Conventions
{
    public class ForeignKeyNameConvenction : IReferenceConvention, IHasOneConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.ForeignKey(string.Format("FK_Test{0}ToTest{1}_{2}", instance.EntityType.Name, instance.Class.Name, instance.Name));
        }

        public void Apply(IOneToOneInstance instance)
        {
            var oneToOne = instance as IOneToOneInspector;
            instance.ForeignKey(string.Format("FK_Test{0}ToTest{1}_{2}", instance.EntityType.Name, oneToOne.Class.Name, instance.Name));
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Relationship.ForeignKey(string.Format("FK_Test{0}{1}_{2}", instance.EntityType.Name, instance.OtherSide.EntityType.Name,
                ((ICollectionInspector)instance).Name));
        }
    }
}

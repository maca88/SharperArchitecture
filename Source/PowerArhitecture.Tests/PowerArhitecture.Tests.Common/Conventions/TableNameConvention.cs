using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.Tests.Common.Conventions
{
    public class TableNameConvention : ManyToManyTableNameConvention, IClassConvention 
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(String.Format("Test{0}", instance.EntityType.Name));
        }

        protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection, IManyToManyCollectionInspector otherSide)
        {
            return String.Format("Test{0}{1}",collection.EntityType.Name, otherSide.EntityType.Name);
        }

        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            return String.Format("Test{0}{1}", collection.EntityType.Name, collection.ChildType.Name);
        }
    }
}

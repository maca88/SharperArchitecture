using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasOneConvention, IHasManyToManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            var type = instance.Member.GetUnderlyingType();
            if(!typeof(IDictionary).IsAssignableFrom(type) && !type.IsAssignableToGenericType(typeof(IDictionary<,>))) //Map must have inverse set to false
                instance.Inverse();
            instance.Cascade.AllDeleteOrphan();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }

        public void Apply(IOneToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;

namespace SharperArchitecture.DataAccess.Visitors
{
    public abstract class NHibernateMappingVisitor
    {
        protected virtual void Visit(IEnumerable<HibernateMapping> mappings)
        {
            foreach (var classMap in mappings.SelectMany(o => o.Classes))
            {
                VisitClassBase(classMap);
            }
        }

        protected virtual void VisitClass(ClassMapping classMapping)
        {
            VisitClassBase(classMapping);
        }

        protected virtual void VisitClassBase(ClassMappingBase classMapping)
        {
            foreach (var reference in classMapping.References)
            {
                VisitReference(reference);
            }

            foreach (var collection in classMapping.Collections)
            {
                VisitCollection(collection);
            }

            foreach (var subClass in classMapping.Subclasses)
            {
                VisitSubclass(subClass);
            }

            foreach (var component in classMapping.Components)
            {
                VisitComponent(component);
            }

            foreach (var any in classMapping.Anys)
            {
                VisitAny(any);
            }

            foreach (var join in classMapping.Joins)
            {
                VisitJoin(join);
            }

            foreach (var one in classMapping.OneToOnes)
            {
                VisitOneToOne(one);
            }
        }

        protected virtual void VisitOneToOne(OneToOneMapping one)
        {
        }

        protected virtual void VisitJoin(JoinMapping @join)
        {
        }

        protected virtual void VisitAny(AnyMapping anyMapping)
        {
        }

        protected virtual void VisitComponent(IComponentMapping componentMapping)
        {
            foreach (var reference in componentMapping.References)
            {
                VisitReference(reference);
            }

            foreach (var collection in componentMapping.Collections)
            {
                VisitCollection(collection);
            }

            foreach (var component in componentMapping.Components)
            {
                VisitComponent(component);
            }
        }

        protected virtual void VisitSubclass(SubclassMapping subClass)
        {
            VisitClassBase(subClass);
        }

        protected virtual void VisitCollection(CollectionMapping collection)
        {
        }

        protected virtual void VisitReference(ManyToOneMapping reference)
        {
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;

namespace SharperArchitecture.DataAccess.MappingSteps
{
    /// <summary>
    /// Copied from nh with a minor modification
    /// </summary>
    internal class CustomCollectionStep : IAutomappingStep
    {
        private readonly IAutomappingConfiguration _cfg;
        private readonly AutoKeyMapper _keys;

        public CustomCollectionStep(IAutomappingConfiguration cfg)
        {
            _cfg = cfg;
            _keys = new AutoKeyMapper();
        }

        public bool ShouldMap(Member member)
        {
            if (
                FluentNHibernate.Utils.Extensions.In(member.PropertyType.Namespace, "System.Collections.Generic",
                    "Iesi.Collections.Generic") &&
                !FluentNHibernate.Utils.Extensions.HasInterface(member.PropertyType, typeof (IDictionary)) &&
                !FluentNHibernate.Utils.Extensions.ClosesInterface(member.PropertyType, typeof (IDictionary<,>)))
                return !FluentNHibernate.Utils.Extensions.Closes(member.PropertyType, typeof (IDictionary<,>));
            else
                return false;
        }

        public void Map(ClassMappingBase classMap, Member member)
        {
            //Added additional contraint ShouldMap so now we are able to map a collection in a ignored base class within a derivered one
            if (member.DeclaringType != classMap.Type && _cfg.ShouldMap(member.DeclaringType))
                return;
            var collectionMapping = CollectionMapping.For(CollectionTypeResolver.Resolve(member));
            collectionMapping.ContainingEntityType = classMap.Type;
            collectionMapping.Member = member;
            collectionMapping.Set(x => x.Name, 0, member.Name);
            collectionMapping.Set(x => x.ChildType, 0,
                member.PropertyType.GetGenericArguments()[0]);
            SetDefaultAccess(member, collectionMapping);
            SetRelationship(member, classMap, collectionMapping);
            _keys.SetKey(member, classMap, collectionMapping); //This is overridden by the foreignkey convention
            classMap.AddCollection(collectionMapping);
        }

        private void SetDefaultAccess(Member member, CollectionMapping mapping)
        {
            var access = MemberAccessResolver.Resolve(member);
            if (access != Access.Property && access != Access.Unset)
                mapping.Set(x => x.Access, 0, access.ToString());
            if (!member.IsProperty || member.CanWrite)
                return;
            mapping.Set(x => x.Access, 0,
                _cfg.GetAccessStrategyForReadOnlyProperty(member).ToString());
        }

        private static void SetRelationship(Member property, ClassMappingBase classMap, CollectionMapping mapping)
        {
            var oneToManyMapping = new OneToManyMapping
            {
                ContainingEntityType = classMap.Type
            };
            oneToManyMapping.Set(x => x.Class, 0,
                new TypeReference(property.PropertyType.GetGenericArguments()[0]));
            mapping.Set(
                x => x.Relationship, 0,
                oneToManyMapping);
        }
    }
}

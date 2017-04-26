using System.Reflection;
using SharperArchitecture.Domain.Attributes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using SharperArchitecture.Domain.Enums;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class FetchModeAttributeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention
    {

        public void Apply(IManyToOneInstance instance)
        {
            var attr = instance.Property.MemberInfo.GetCustomAttribute<FetchModeAttribute>();
            if(attr == null) return;

            switch (attr.Mode)
            {
                case FetchMode.Select:
                    instance.Fetch.Select();
                    break;
                case FetchMode.Join:
                    instance.Fetch.Join();
                    break;
                case FetchMode.SubSelect:
                    instance.Fetch.Subselect();
                    break;
            }
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            var attr = instance.Member.GetCustomAttribute<FetchModeAttribute>();
            if (attr == null) return;
            switch (attr.Mode)
            {
                case FetchMode.Select:
                    instance.Fetch.Select();
                    break;
                case FetchMode.Join:
                    instance.Fetch.Join();
                    break;
                case FetchMode.SubSelect:
                    instance.Fetch.Subselect();
                    break;
            }
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            var attr = instance.Member.GetCustomAttribute<FetchModeAttribute>();
            if (attr == null) return;
            switch (attr.Mode)
            {
                case FetchMode.Select:
                    instance.Fetch.Select();
                    break;
                case FetchMode.Join:
                    instance.Fetch.Join();
                    break;
                case FetchMode.SubSelect:
                    instance.Fetch.Subselect();
                    break;
            }
        }
    }
}

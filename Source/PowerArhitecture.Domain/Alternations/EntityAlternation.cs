using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace PowerArhitecture.Domain.Alternations
{
    public class EntityAlternation : IAutoMappingAlteration
    {
        public void Alter(AutoPersistenceModel model)
        {
            model.IgnoreBase(typeof (Entity<>));
            model.IgnoreBase(typeof (Entity));
            model.IgnoreBase(typeof (VersionedEntity<,>));
            model.IgnoreBase(typeof (VersionedEntity));
        }
    }
}

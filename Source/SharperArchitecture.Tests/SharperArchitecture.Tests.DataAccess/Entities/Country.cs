using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.DataAccess.Entities
{
    public class Country : Entity
    {
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }
    }
}

using PowerArhitecture.Domain;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public class Country : Entity
    {
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }
    }
}

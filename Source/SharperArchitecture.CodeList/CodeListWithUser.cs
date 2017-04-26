using System;
using SharperArchitecture.CodeList.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class CodeListWithUser<TUser> : VersionedEntityWithUser<TUser, string>, INonLocalizableCodeList
    {
        [Formula("Id")]
        public virtual string Code { get { return Id; } set { Id = value; } }

        public virtual bool Active { get; set; } = true;

        public virtual string Name { get; set; }

        // Id can be changed via Code so we have to check CreatedDate
        public override bool IsTransient()
        {
            return CreatedDate == DateTime.MinValue;
        }
    }
}

using System;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class CodeListWithUser<TUser> : VersionedEntityWithUser<TUser, string>, INonLocalizableCodeList
    {
        [Formula("Id")]
        public virtual string Code { get { return Id; } set { Id = value; } }

        public virtual bool Active { get; set; } = true;

        public virtual string Name { get; set; }

        //Id can be changed via Code so we have to check CreatedBy and LastModifiedBy
        public override bool IsTransient()
        {
            return CreatedBy == null || LastModifiedBy == null;
        }
    }
}

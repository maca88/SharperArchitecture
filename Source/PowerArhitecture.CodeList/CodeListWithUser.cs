using System;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class CodeListWithUser<TUser> : VersionedEntityWithUser<TUser, string>, ICodeListNoLoc
    {
        [Ignore]
        public virtual string Code { get { return Id; } set { Id = value; } }

        private bool _active = true;

        public virtual bool Active { get { return _active; } set { _active = value; } }

        public virtual string Name { get; set; }

        //Id can be changed via Code so we have to check CreatedBy and LastModifiedBy
        public override bool IsTransient()
        {
            return CreatedBy == null || LastModifiedBy == null;
        }
    }
}

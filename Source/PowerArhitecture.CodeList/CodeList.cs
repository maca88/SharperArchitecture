using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class CodeList : VersionedEntity<string, IUser>, ICodeList, ICodeListNoLoc
    {
        [Ignore]
        public virtual string Code { get { return Id; } set { Id = value; } }

        public virtual bool Active { get; set; }

        public virtual string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class CodeList : VersionedEntity<string>, INonLocalizableCodeList
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

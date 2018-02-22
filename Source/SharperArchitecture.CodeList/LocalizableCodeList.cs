using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using SharperArchitecture.CodeList.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class LocalizableCodeList<TCodeList, TCodeListNames> : VersionedEntity<string>, ILocalizableCodeList<TCodeList, TCodeListNames>
        where TCodeListNames : class, ILocalizableCodeListLanguage<TCodeList, TCodeListNames>
        where TCodeList : LocalizableCodeList<TCodeList, TCodeListNames>
    {
        private ISet<TCodeListNames> _names;

        public virtual ISet<TCodeListNames> Names
        {
            get { return _names ?? (_names = new HashSet<TCodeListNames>()); }
            protected set { _names = value; }
        }

        [Formula("`Id`")]
        public virtual string Code { get { return Id; } set { Id = value; } }

        public virtual bool Active { get; set; } = true;

        public virtual void AddName(TCodeListNames name)
        {
            this.AddOneToMany(o => o.Names, name, o => o.CodeList, o => RemoveName);
        }

        public virtual void RemoveName(TCodeListNames name)
        {
            this.RemoveOneToMany(o => o.Names, name, o => o.CodeList);
        }

        // Id can be changed via Code so we have to check CreatedDate
        public override bool IsTransient()
        {
            return CreatedDate == DateTime.MinValue;
        }
    }
}

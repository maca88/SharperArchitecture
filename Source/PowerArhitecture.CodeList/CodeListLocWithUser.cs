﻿using System;
using System.Collections.Generic;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract class CodeListLocWithUser<TUser, TCodeList, TCodeListNames> : VersionedEntityWithUser<TUser, string>, ICodeListLoc<TCodeList, TCodeListNames>
        where TCodeListNames : class, ICodeListLocalization<TCodeList, TCodeListNames>
        where TCodeList : CodeListLocWithUser<TUser, TCodeList, TCodeListNames>
    {
        private ISet<TCodeListNames> _names;

        public virtual ISet<TCodeListNames> Names
        {
            get { return _names ?? (_names = new HashSet<TCodeListNames>()); }
            protected set { _names = value; }
        }

        [Ignore]
        public virtual string Code { get { return Id; } set { Id = value; } }

        public virtual bool Active { get; set; }

        public virtual void AddName(TCodeListNames name)
        {
            this.AddOneToMany(o => o.Names, name, o => o.CodeList, o => RemoveName);
        }

        public virtual void RemoveName(TCodeListNames name)
        {
            this.RemoveOneToMany(o => o.Names, name, o => o.CodeList);
        }
    }
}
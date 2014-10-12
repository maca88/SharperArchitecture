﻿using System;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.CodeList
{
    [Ignore]
    [Serializable]
    public abstract partial class CodeListLocalization<TCodeList, TCodeListNames> : Entity, ICodeListLocalization<TCodeList, TCodeListNames>
        where TCodeList : class, ICodeListLoc<TCodeList, TCodeListNames>
        where TCodeListNames : class, ICodeListLocalization<TCodeList, TCodeListNames>
    {
        public virtual TCodeList CodeList { get; set; }

        public virtual string LanguageCode { get; set; }

        public virtual string Name { get; set; }

        public object GetCodeList()
        {
            return CodeList;
        }

        public virtual void SetCodeList(TCodeList codeList)
        {
            var me = this as TCodeListNames;
            me.SetManyToOne(o => o.CodeList, codeList, o => o.RemoveName, o => o.Names);
        }

        public virtual void UnsetCodeList()
        {
            var me = this as TCodeListNames;
            me.UnsetManyToOne(o => o.CodeList, o => o.Names);
        }
    }
}

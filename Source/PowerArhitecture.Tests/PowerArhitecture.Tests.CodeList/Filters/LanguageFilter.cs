using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace PowerArhitecture.Tests.CodeList.Filters
{
    public class LanguageFilter : FilterDefinition
    {
        public LanguageFilter()
        {
            WithName("Language")
                .AddParameter("Current", NHibernate.NHibernateUtil.String)
                .AddParameter("Fallback", NHibernate.NHibernateUtil.String);
        }
    }
}

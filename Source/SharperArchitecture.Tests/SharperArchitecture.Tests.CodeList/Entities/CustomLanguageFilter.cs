using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.CodeList;
using SharperArchitecture.CodeList.Attributes;
using SharperArchitecture.Tests.CodeList.Attributes;

namespace SharperArchitecture.Tests.CodeList.Entities
{
    [CodeListConfiguration]
    public partial class CustomLanguageFilter : LocalizableCodeListWithUser<string, CustomLanguageFilter, CustomLanguageFilterLanguage>
    {
        [CurrentLanguage]
        public virtual string Name { get; set; }

        [CurrentLanguage]
        public virtual string Custom { get; set; }

        [CurrentLanguage("Custom2")]
        public virtual string CurrentCustom2 { get; set; }
    }

    public partial class CustomLanguageFilterLanguage : LocalizableCodeListLanguage<CustomLanguageFilter, CustomLanguageFilterLanguage>
    {

        public virtual string Custom { get; set; }

        public virtual string Custom2 { get; set; }
    }
}

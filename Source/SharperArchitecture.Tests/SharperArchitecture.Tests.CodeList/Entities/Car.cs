
using System;
using SharperArchitecture.CodeList;
using SharperArchitecture.CodeList.Attributes;
using FluentNHibernate.Automapping;
using SharperArchitecture.Domain.Attributes;
using SharperArchitecture.Tests.CodeList.Attributes;

namespace SharperArchitecture.Tests.CodeList.Entities
{
    [CodeListConfiguration]
    public partial class Car : LocalizableCodeListWithUser<string, Car, CarLanguage>
    {
        [FilterCurrentLanguage("Language", "Current", "Fallback")]
        public virtual string Name { get; set; }

    }

    public partial class CarLanguage : LocalizableCodeListLanguage<Car, CarLanguage>
    {
    }
}

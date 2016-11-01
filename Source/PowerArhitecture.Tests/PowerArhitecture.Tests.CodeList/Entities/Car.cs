
using System;
using PowerArhitecture.CodeList;
using PowerArhitecture.CodeList.Attributes;
using FluentNHibernate.Automapping;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Tests.CodeList.Attributes;

namespace PowerArhitecture.Tests.CodeList.Entities
{
    [Configuration]
    public partial class Car : LocalizableCodeListWithUser<string, Car, CarLanguage>
    {
        [FilterCurrentLanguage("Language", "Current", "Fallback")]
        public virtual string Name { get; set; }

    }

    public partial class CarLanguage : LocalizableCodeListLanguage<Car, CarLanguage>
    {
    }
}

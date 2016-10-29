using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class LengthModel
    {
        [Length(10)]
        public string Name { get; set; }

        [Length(10, IncludePropertyName = true)]
        public string Name2 { get; set; }

        [Length(5, 10)]
        public string Nickname { get; set; }

        [Length(5, 10, IncludePropertyName = true)]
        public string Nickname2 { get; set; }
    }
}

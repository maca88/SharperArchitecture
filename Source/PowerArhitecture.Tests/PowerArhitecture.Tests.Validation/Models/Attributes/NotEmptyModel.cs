using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class NotEmptyModel
    {
        [NotEmpty]
        public string Name { get; set; }

        [NotEmpty(IncludePropertyName = true)]
        public string Name2 { get; set; }

        [NotEmpty("Default", IncludePropertyName = true)]
        public string Name3 { get; set; }
    }
}

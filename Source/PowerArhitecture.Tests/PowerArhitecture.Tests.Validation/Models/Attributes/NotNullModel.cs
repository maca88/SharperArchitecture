using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class NotNullModel
    {
        [NotNull]
        public string Name { get; set; }

        [NotNull(IncludePropertyName = true)]
        public string Name2 { get; set; }
    }
}

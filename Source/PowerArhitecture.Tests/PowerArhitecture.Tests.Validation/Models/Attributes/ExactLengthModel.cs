using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class ExactLengthModel
    {
        [ExactLength(5)]
        public string Name { get; set; }

        [ExactLength(5, IncludePropertyName = true)]
        public string Name2 { get; set; }
    }
}

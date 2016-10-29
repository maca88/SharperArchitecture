using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class EmailModel
    {
        [Email]
        public string Email { get; set; }

        [Email(IncludePropertyName = true)]
        public string Email2 { get; set; }
    }
}

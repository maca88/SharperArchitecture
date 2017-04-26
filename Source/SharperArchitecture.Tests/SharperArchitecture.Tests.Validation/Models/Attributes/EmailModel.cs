using SharperArchitecture.Validation.Attributes;

namespace SharperArchitecture.Tests.Validation.Models.Attributes
{
    public class EmailModel
    {
        [Email]
        public string Email { get; set; }

        [Email(IncludePropertyName = true)]
        public string Email2 { get; set; }
    }
}

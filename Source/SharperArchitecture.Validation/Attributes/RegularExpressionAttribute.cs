using System;
using System.Text.RegularExpressions;

namespace SharperArchitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RegularExpressionAttribute : ValidationAttribute
    {
        public RegularExpressionAttribute(string expression, RegexOptions options = RegexOptions.None)
        {
            Expression = expression;
            RegexOptions = options;
        }

        public string Expression { get; private set; }

        public RegexOptions RegexOptions { get; private set; }
    }
}

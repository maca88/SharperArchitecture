using System;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotEmptyAttribute : ValidationAttribute
    {
        public NotEmptyAttribute(object defaultValue = null)
        {
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; }
    }
}

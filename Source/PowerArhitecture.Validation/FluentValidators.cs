using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PowerArhitecture.Validation.Extensions;
using FluentValidation;
using FluentValidation.Validators;

namespace PowerArhitecture.Validation
{
    public class FluentValidators
    {
        private static readonly Dictionary<Type, string> ValNames = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, Func<IPropertyValidator, IDictionary<string, object>>> ValParametersFuncs =
            new Dictionary<Type, Func<IPropertyValidator, IDictionary<string, object>>>();

        static FluentValidators()
        {
            ValNames.Add(typeof(NotNullValidator), "fvNotNull");
            ValNames.Add(typeof(EmailValidator), "fvEmail");
            ValNames.Add(typeof(NotEmptyValidator), "fvNotEmpty");
            ValNames.Add(typeof(EqualValidator), "fvEqual");
            ValNames.Add(typeof(ExclusiveBetweenValidator), "fvExclusiveBetween");
            ValNames.Add(typeof(InclusiveBetweenValidator), "fvInclusiveBetween");
            ValNames.Add(typeof(LengthValidator), "fvLength");
            ValNames.Add(typeof(NotEqualValidator), "fvNotEqual");
            ValNames.Add(typeof(RegularExpressionValidator), "fvRegularExpression");
            ValNames.Add(typeof(CreditCardValidator), "fvCreditCard");
            ValNames.Add(typeof(ExactLengthValidator), "fvExactLength");
            ValNames.Add(typeof(GreaterThanValidator), "fvGreaterThan");
            ValNames.Add(typeof(GreaterThanOrEqualValidator), "fvGreaterThanOrEqual");
            ValNames.Add(typeof(LessThanOrEqualValidator), "fvLessThanOrEqual");
            ValNames.Add(typeof(LessThanValidator), "fvLessThan");
        }

        public static Dictionary<Type, string> GetAllValidators()
        {
            return ValNames.ToDictionary(o => o.Key, o => o.Value);
        }

        public static string GetName(IPropertyValidator validator)
        {
            return GetName(validator.GetType());
        }

        private static string ToClientFormat(string format)
        {
            return Regex.Replace(format, @"{(.)(\w*)}", match => //TODO: Config
                "{{" + match.Groups[1].Value.ToLowerInvariant() + match.Groups[2].Value + "}}", 
                RegexOptions.IgnoreCase);
        }

        public static IDictionary<string, object> GetParamaters(IPropertyValidator validator)
        {
            if (ValParametersFuncs.ContainsKey(validator.GetType()))
                return ValParametersFuncs[validator.GetType()](validator);

            var result = new Dictionary<string, object>();
            result["errorMessageId"] = ToClientFormat(validator.GetMessageId());
            var equalVal = validator as IComparisonValidator;
            if (equalVal != null)
            {
                //result["comparison"] = equalVal.Comparison.ToString();
                result["valueToCompare"] = equalVal.ValueToCompare;
                if (equalVal.MemberToCompare != null)
                    result["memberToCompare"] = equalVal.MemberToCompare.Name;
            }
            var btwVal = validator as IBetweenValidator;
            if (btwVal != null)
            {
                result["from"] = btwVal.From;
                result["to"] = btwVal.To;
            }
            var lenghtVal = validator as ILengthValidator;
            if (lenghtVal != null)
            {
                result["min"] = lenghtVal.Min;
                result["max"] = lenghtVal.Max;
            }
            var regexVal = validator as IRegularExpressionValidator;
            if (regexVal != null)
            {
                result["expression"] = regexVal.Expression;
            }
            return result;
        }

        public static string GetName(Type validatorType)
        {
            return ValNames.ContainsKey(validatorType)
                ? ValNames[validatorType]
                : null;
        }

        public static void RegisterValidator(Type validatorType, string name)
        {
            ValNames[validatorType] = name;
        }

        public static void RegisterValidator<T>(string name, Func<T, IDictionary<string, object>> parametersFunc = null) 
            where T : IPropertyValidator
        {
            ValNames[typeof(T)] = name;
            if (parametersFunc != null)
                ValParametersFuncs[typeof (T)] = validator => parametersFunc((T) validator);
        }

    }
}

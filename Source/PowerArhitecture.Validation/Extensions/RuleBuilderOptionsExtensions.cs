using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Internationalization;
using FluentValidation;
using FluentValidation.Resources;

namespace PowerArhitecture.Validation.Extensions
{
    public static class RuleBuilderOptionsExtensions
    {
        /// <summary>
        /// Use this extension when using a FluentValidation built-in validator 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="rule"></param>
        /// <param name="includePropName"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> WithL10NMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, bool includePropName = false)
        {
            return rule.Configure(config => config.SetL10NMessage(includePropName));
        }

        public static IRuleBuilderOptions<T, TProperty> WithL10NMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string messageId, 
            params Func<T, TProperty, object>[] funcs)
        {
            return rule.Configure(config =>
            {
                config.CurrentValidator.ErrorMessageSource = new StaticStringSource(I18N.Translate(messageId));
                funcs
                    .Select(func => new Func<object, object, object>((instance, value) => func((T)instance, (TProperty)value)))
                    .ForEach(config.CurrentValidator.CustomMessageFormatArguments.Add);
            });
        } 
    }
}

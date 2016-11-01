using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;

namespace PowerArhitecture.Domain.Extensions
{
    public static class ValidationContextExtensions
    {
        internal static string RootAutoValidationKey = "_rootAutoValidation";
        internal static string AutoValidationKey = "_autoValidation";

        /// <summary>
        /// Check if the validation was triggered automatically
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>True whether the validation was triggered automatically, False otherwise</returns>
        public static bool IsAutoValidating(this ValidationContext context)
        {
            return context.RootContextData.ContainsKey(AutoValidationKey);
        }

        /// <summary>
        /// Check if the validation was triggered automatically
        /// </summary>
        /// <param name="context">Property validation context</param>
        /// <returns>True whether the validation was triggered automatically, False otherwise</returns>
        public static bool IsAutoValidating(this PropertyValidatorContext context)
        {
            return context.ParentContext.IsAutoValidating();
        }

        /// <summary>
        /// Check whether the root entity is being auto validated or not.
        /// There are some scenarios when also a child entity will be validated separatly:
        /// 1. When a child entity is being removed for the database
        /// 2. When a child entity is being added to a parent that is already persisted
        /// 3. When a persistent child entity is being switched to a transient parent
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Whether the root entity is being validated</returns>
        public static bool IsRootAutoValidating(this ValidationContext context)
        {
            return context.RootContextData.ContainsKey(RootAutoValidationKey) &&
                   context.RootContextData[RootAutoValidationKey] as bool? == true;
        }

        /// <summary>
        /// Check whether the root entity is being auto validated or not.
        /// There are some scenarios when also a child entity will be validated separatly:
        /// 1. When a child entity is being removed for the database
        /// 2. When a child entity is being added to a parent that is already persisted
        /// 3. When a persistent child entity is being switched to a transient parent
        /// </summary>
        /// <param name="context">Property validation context</param>
        /// <returns>Whether the root entity is being validated</returns>
        public static bool IsRootAutoValidating(this PropertyValidatorContext context)
        {
            return context.ParentContext.IsRootAutoValidating();
        }
    }
}

using PowerArhitecture.Common.Internationalization;
using FluentValidation.Internal;
using FluentValidation.Resources;
using FluentValidation.Validators;

namespace PowerArhitecture.Validation.Extensions
{
    public static class PropertyRuleExtensions
    {
        public static string GetMessageId(this PropertyRule propRule, bool includePropName = false)
        {
            return propRule.CurrentValidator.GetMessageId(includePropName);
        }

        public static void SetL10NMessage(this PropertyRule propRule, bool includePropName = false)
        {
            var propVal = propRule.CurrentValidator;
            var delegVal = propVal as DelegatingValidator;
            if (delegVal != null)
                propVal = delegVal.InnerValidator; //Get the actual validator
            var propValType = propVal.GetType();
            var messageId = propRule.GetMessageId(includePropName);
            var propName = propRule.DisplayName.GetString() ?? propRule.PropertyName;
            string errorMsg = null;

            #region NotNullValidator
            if (propValType == typeof(NotNullValidator))
            {
                errorMsg = includePropName
                    ? I18N.Translate(messageId, new { PropertyName = new { PropertyName = I18N.Translate(propName) } })
                    : I18N.Translate(messageId);
            }
            #endregion
            #region EmailValidator
            else if (propValType == typeof(EmailValidator))
            {
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName) })
                            : I18N.Translate(messageId);
            }
            #endregion
            #region NotEmptyValidator
            else if (propValType == typeof(NotEmptyValidator))
            {
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName) })
                            : I18N.Translate(messageId);
            }
            #endregion
            #region EqualValidator
            else if (propValType == typeof(EqualValidator))
            {
                var val = (EqualValidator)propVal;
                errorMsg = val.ValueToCompare != null
                        ? (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = val.ValueToCompare })
                            : I18N.Translate(messageId, new { ComparisonValue = val.ValueToCompare }))
                        : (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) })
                            : I18N.Translate(messageId, new { ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) }));
            }
            #endregion
            #region ExclusiveBetweenValidator
            else if (propValType == typeof(ExclusiveBetweenValidator))
            {
                var val = (ExclusiveBetweenValidator)propVal;
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), val.From, val.To })
                            : I18N.Translate(messageId, new { val.From, val.To });
            }
            #endregion
            #region InclusiveBetweenValidator
            else if (propValType == typeof(InclusiveBetweenValidator))
            {
                var val = (InclusiveBetweenValidator)propVal;
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), val.From, val.To })
                            : I18N.Translate(messageId, new { val.From, val.To });
            }
            #endregion
            #region LengthValidator
            else if (propValType == typeof(LengthValidator))
            {
                var val = (LengthValidator)propVal;
                errorMsg = (val.Min > 0)
                            ? (includePropName
                                ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), val.Min, val.Max })
                                : I18N.Translate(messageId, new { val.Min, val.Max }))
                            : (includePropName
                                ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), val.Max })
                                : I18N.Translate(messageId, new { val.Max }));
            }
            #endregion
            #region NotEqualValidator
            else if (propValType == typeof(NotEqualValidator))
            {
                var val = (NotEqualValidator)propVal;
                errorMsg = val.ValueToCompare != null
                        ? (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = val.ValueToCompare })
                            : I18N.Translate(messageId, new { ComparisonValue = val.ValueToCompare }))
                        : (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) })
                            : I18N.Translate(messageId, new { ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) }));
            }
            #endregion
            #region RegularExpressionValidator
            else if (propValType == typeof(RegularExpressionValidator))
            {
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName) })
                            : I18N.Translate(messageId);
            }
            #endregion
            #region CreditCardValidator
            else if (propValType == typeof(CreditCardValidator))
            {
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName) })
                            : I18N.Translate(messageId);
            }
            #endregion
            #region ExactLengthValidator
            else if (propValType == typeof(ExactLengthValidator))
            {
                var val = (ExactLengthValidator)propVal;
                errorMsg = includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), val.Max })
                            : I18N.Translate(messageId, new { val.Max });
            }
            #endregion
            #region GreaterThanValidator
            else if (propValType == typeof(GreaterThanValidator))
            {
                var val = (GreaterThanValidator)propVal;
                errorMsg = val.ValueToCompare != null
                        ? (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = val.ValueToCompare })
                            : I18N.Translate(messageId, new { ComparisonValue = val.ValueToCompare }))
                        : (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) })
                            : I18N.Translate(messageId, new { ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) }));
            }
            #endregion
            #region GreaterThanOrEqualValidator
            else if (propValType == typeof(GreaterThanOrEqualValidator))
            {
                var val = (GreaterThanOrEqualValidator)propVal;
                errorMsg = val.ValueToCompare != null
                        ? (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = val.ValueToCompare })
                            : I18N.Translate(messageId, new { ComparisonValue = val.ValueToCompare }))
                        : (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) })
                            : I18N.Translate(messageId, new { ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) }));
            }
            #endregion
            #region LessThanOrEqualValidator
            else if (propValType == typeof(LessThanOrEqualValidator))
            {
                var val = (LessThanOrEqualValidator)propVal;
                errorMsg = val.ValueToCompare != null
                        ? (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = val.ValueToCompare })
                            : I18N.Translate(messageId, new { ComparisonValue = val.ValueToCompare }))
                        : (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) })
                            : I18N.Translate(messageId, new { ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) }));
            }
            #endregion
            #region LessThanValidator
            else if (propValType == typeof(LessThanValidator))
            {
                var val = (LessThanValidator)propVal;
                errorMsg = val.ValueToCompare != null
                        ? (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = val.ValueToCompare })
                            : I18N.Translate(messageId, new { ComparisonValue = val.ValueToCompare }))
                        : (includePropName
                            ? I18N.Translate(messageId, new { PropertyName = I18N.Translate(propName), ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) })
                            : I18N.Translate(messageId, new { ComparisonValue = (object)I18N.Translate(val.MemberToCompare.Name) }));
            }
            #endregion
            propVal.ErrorMessageSource = new StaticStringSource(errorMsg);
        }
    }
}

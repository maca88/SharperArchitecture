using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;

namespace SharperArchitecture.Validation
{
    /// <summary>
    /// Selects validators that belong to the specified rulesets.
    /// </summary>
    public class CustomRulesetValidatorSelector : IValidatorSelector
    {
        readonly HashSet<string> _rulesetsToExecute;

        /// <summary>
        /// Creates a new instance of the RulesetValidatorSelector.
        /// </summary>
        public CustomRulesetValidatorSelector(IEnumerable<string> rulesetsToExecute)
        {
            _rulesetsToExecute = new HashSet<string>(rulesetsToExecute);
        }

        public CustomRulesetValidatorSelector()
        {
            _rulesetsToExecute = new HashSet<string>();
        }

        /// <summary>
        /// Determines whether or not a rule should execute.
        /// </summary>
        /// <param name="rule">The rule</param>
        /// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
        /// <param name="context">Contextual information</param>
        /// <returns>Whether or not the validator can execute.</returns>
        public bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context)
        {
            if (HasNoRuleSets(rule) && _rulesetsToExecute.Count == 0)
            {
                return true;
            }
            if (HasNoRuleSets(rule) && _rulesetsToExecute.Count > 0 &&
                _rulesetsToExecute.Contains("default", StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
            if (!HasNoRuleSets(rule) && _rulesetsToExecute.Count > 0 &&
                _rulesetsToExecute.Any(r => rule.RuleSets.Contains(r)))
            {
                return true;
            }
            if (_rulesetsToExecute.Contains("*"))
            {
                return true;
            }
            return false;
        }

        private bool HasNoRuleSets(IValidationRule rule)
        {
            return rule.RuleSets.Length == 0;
        }
    }
}

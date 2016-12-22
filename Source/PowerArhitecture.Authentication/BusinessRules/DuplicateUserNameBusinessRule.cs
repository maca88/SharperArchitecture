﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Authentication.BusinessRules
{
    public class DuplicateUserNameBusinessRule<TUser> : AbstractBusinessRule<TUser>
        where TUser : class, IUser, IEntity<long>, new()
    {
        private readonly IDbStore _dbStore;

        public DuplicateUserNameBusinessRule(IDbStore dbStore)
        {
            _dbStore = dbStore;
        }

        public override ValidationFailure Validate(TUser user, ValidationContext context)
        {
            return _dbStore.Query<TUser>().Any(o => o.UserName == user.UserName)
                ? Failure(o => o.UserName, I18N.Translate("'{0}' '{1}' already exists.", I18N.Translate("UserName"), user.UserName), context)
                : Success;
        }

        public override bool CanValidate(TUser user, ValidationContext context)
        {
            return true;
        }

        public override string[] RuleSets => new[] { ValidationRuleSet.Insert };
    }
}

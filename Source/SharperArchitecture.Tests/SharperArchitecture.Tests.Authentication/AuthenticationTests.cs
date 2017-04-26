using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.DataAccess;
using SharperArchitecture.Domain;
using SharperArchitecture.Tests.Authentication.Entities;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Validation;
using SharperArchitecture.Validation.Specifications;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using PasswordHasher = SharperArchitecture.Authentication.PasswordHasher;

namespace SharperArchitecture.Tests.Authentication
{
    [TestFixture]
    public class AuthenticationTests : DatabaseBaseTest
    {
        public AuthenticationTests()
        {
            EntityAssemblies.Add(typeof(AuthenticationTests).Assembly);
            EntityAssemblies.Add(typeof(PasswordHasher).Assembly);
            ConventionAssemblies.Add(typeof(PasswordHasher).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(AuthenticationTests).Assembly);
            TestAssemblies.Add(typeof(PasswordHasher).Assembly);
        }

        [Test]
        public void DeleteValidationUserRulesShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var validator = Container.GetInstance<IValidator<User>>();
                var systemUser = new User
                {
                    UserName = "admin"
                };
                var result = validator.Validate(systemUser, ruleSet: ValidationRuleSet.Delete);
                Assert.IsFalse(result.IsValid);
                Assert.AreEqual(1, result.Errors.Count);
            }
        }

        [Test]
        public void DuplicateUserValidationRuleShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var validator = Container.GetInstance<IValidator<User>>();
                var systemUser = new User
                {
                    UserName = "admin"
                };
                var result = validator.Validate(systemUser, ruleSet: ValidationRuleSet.Insert);
                Assert.IsFalse(result.IsValid);
                Assert.AreEqual(1, result.Errors.Count);
            }
        }

        [Test]
        public void UserFormatValidationRuleShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var validator = Container.GetInstance<IValidator<User>>();
                var systemUser = new User
                {
                    UserName = "admin@notValid"
                };
                var result = validator.Validate(systemUser, ruleSet: ValidationRuleSet.Insert);
                Assert.IsFalse(result.IsValid);
                Assert.AreEqual(1, result.Errors.Count);

                result = validator.Validate(systemUser, ruleSet: ValidationRuleSet.Update);
                Assert.IsFalse(result.IsValid);
                Assert.AreEqual(1, result.Errors.Count);
            }
        }

        [Test]
        public void IdentityStoreInterfacesShouldBeScoped()
        {
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IUserLoginStore<User, long>>(); });
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IUserClaimStore<User, long>>(); });
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IUserRoleStore<User, long>>(); });
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IUserPasswordStore<User, long>>(); });
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IUserSecurityStampStore<User, long>>(); });
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IRoleStore<Role, long>>(); });
            Assert.Throws<ActivationException>(() => { Container.GetInstance<IQueryableRoleStore<Role, long>>(); });

            using (Container.BeginExecutionContextScope())
            {
                var loginStore = Container.GetInstance<IUserLoginStore<User, long>>();
                Assert.AreEqual(loginStore, Container.GetInstance<IUserLoginStore<User, long>>());

                var claimStore = Container.GetInstance<IUserClaimStore<User, long>>();
                Assert.AreEqual(claimStore, Container.GetInstance<IUserClaimStore<User, long>>());

                var userRoleStore = Container.GetInstance<IUserRoleStore<User, long>>();
                Assert.AreEqual(userRoleStore, Container.GetInstance<IUserRoleStore<User, long>>());

                var userPasswordStore = Container.GetInstance<IUserPasswordStore<User, long>>();
                Assert.AreEqual(userPasswordStore, Container.GetInstance<IUserPasswordStore<User, long>>());

                var userSecurityStampStore = Container.GetInstance<IUserSecurityStampStore<User, long>>();
                Assert.AreEqual(userSecurityStampStore, Container.GetInstance<IUserSecurityStampStore<User, long>>());

                var roleStore = Container.GetInstance<IRoleStore<Role, long>>();
                Assert.AreEqual(roleStore, Container.GetInstance<IRoleStore<Role, long>>());

                var queryRoleStore = Container.GetInstance<IQueryableRoleStore<Role, long>>();
                Assert.AreEqual(queryRoleStore, Container.GetInstance<IQueryableRoleStore<Role, long>>());
            }
        }
    }
}

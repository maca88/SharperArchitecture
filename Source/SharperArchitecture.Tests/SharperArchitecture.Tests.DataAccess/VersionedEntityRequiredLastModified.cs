using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using NHibernate;
using NUnit.Framework;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Providers;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.DataAccess.Entities;
using SharperArchitecture.Tests.DataAccess.Entities.Versioning;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Tests.DataAccess
{
    [TestFixture]
    public class VersionedEntityRequiredLastModified : DatabaseBaseTest
    {
        public VersionedEntityRequiredLastModified()
        {
            EntityAssemblies.Add(typeof(LifecycleTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
        }

        protected override IFluentDatabaseConfiguration CreateDatabaseConfiguration(string dbName = "foo", string name = null)
        {
            return base.CreateDatabaseConfiguration(dbName, name)
                .Conventions(c => c
                    .HiLoId(o => o.Enabled(false))
                    .RequiredLastModifiedProperty()
                );
        }

        #region VersionEntity

        [Test]
        public void ValidatorForVersionEntityMustHaveRequiredLastModified()
        {
            var validator = /*(Validator<VersionCar>)*/Container.GetInstance<IValidator<VersionCar>>() as IEnumerable;

            var lastModifiedDateRules = validator.OfType<PropertyRule>()
                .Where(o => o.PropertyName == "LastModifiedDate")
                .ToList();

            Assert.AreEqual(1, lastModifiedDateRules.Count);
            Assert.AreEqual(ValidationRuleSet.Attribute, lastModifiedDateRules[0].RuleSet);
            Assert.AreEqual(1, lastModifiedDateRules[0].Validators.Count());
            Assert.AreEqual(typeof(NotNullValidator), lastModifiedDateRules[0].Validators.First().GetType());
        }

        [Test]
        public void SaveVersionEntity()
        {
            var car = new VersionCar { Model = "BMW" };
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                try
                {
                    unitOfWork.Save(car); //A flush will happen as we set the id generator to indentity
                    Assert.AreEqual(1, car.Id);
                    Assert.AreEqual(1, car.Version);
                    Assert.AreEqual(car.LastModifiedDate, car.CreatedDate);
                    car.Model = "Audi";
                    unitOfWork.Save(car);
                    unitOfWork.Flush();
                    Assert.AreEqual(2, car.Version);
                    Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                unitOfWork.Commit();
            }
            Assert.AreEqual(2, car.Version);
        }

        [Test]
        public void SaveVersionEntityNested()
        {
            var bmw = new VersionCar { Model = "BMW" };
            var bmwWheel1 = new VersionWheel { Dimension = 17 };
            var bmwWheel2 = new VersionWheel { Dimension = 17 };
            bmw.AddWheel(bmwWheel1);
            bmw.AddWheel(bmwWheel2);

            var audi = new VersionCar { Model = "AUDI" };
            var audiWheel1 = new VersionWheel { Dimension = 18 };
            var audiWheel2 = new VersionWheel { Dimension = 18 };
            audi.AddWheel(audiWheel1);
            audi.AddWheel(audiWheel2);

            bmw.Child = audi;

            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                unitOfWork.DefaultSession.FlushMode = FlushMode.Never;
                //Save only the root entity (because the default conventions the children will be saved too)
                unitOfWork.Save(bmw); //A flush will happen as we set the id generator to indentity

                Assert.AreEqual(false, bmw.IsTransient());
                Assert.AreEqual(1, bmw.Version);
                Assert.AreEqual(bmw.CreatedDate, bmw.LastModifiedDate);

                Assert.AreEqual(false, bmwWheel1.IsTransient());
                Assert.AreEqual(1, bmwWheel1.Version);
                Assert.AreEqual(bmwWheel1.CreatedDate, bmwWheel1.LastModifiedDate);

                Assert.AreEqual(false, bmwWheel2.IsTransient());
                Assert.AreEqual(1, bmwWheel2.Version);
                Assert.AreEqual(bmwWheel2.CreatedDate, bmwWheel2.LastModifiedDate);

                Assert.AreEqual(false, audi.IsTransient());
                Assert.AreEqual(1, audi.Version);
                Assert.AreEqual(audi.CreatedDate, audi.LastModifiedDate);

                Assert.AreEqual(false, audiWheel1.IsTransient());
                Assert.AreEqual(1, audiWheel1.Version);
                Assert.AreEqual(audiWheel1.CreatedDate, audiWheel1.LastModifiedDate);

                Assert.AreEqual(false, audiWheel2.IsTransient());
                Assert.AreEqual(1, audiWheel2.Version);
                Assert.AreEqual(audiWheel2.CreatedDate, audiWheel2.LastModifiedDate);

                Thread.Sleep(2000);

                //Update
                audiWheel1.Dimension = 19;
                unitOfWork.Flush();
            }

            Assert.AreEqual(2, audiWheel1.Version);
            Assert.AreNotEqual(audiWheel1.LastModifiedDate, audiWheel1.CreatedDate);
            Assert.AreEqual(1, bmw.Version);
            Assert.AreEqual(1, bmwWheel1.Version);
            Assert.AreEqual(1, bmwWheel2.Version);
            Assert.AreEqual(1, audi.Version);
            Assert.AreEqual(1, audiWheel2.Version);
        }

        #endregion

        #region VersionEntityWithStringUser

        [Test]
        public void ValidatorForVersionEntityWithStringUserMustHaveRequiredLastModified()
        {
            var validator = Container.GetInstance<IValidator<VersionCarWithStringUser>>() as IEnumerable;

            var lastModifiedDateRules = validator.OfType<PropertyRule>()
                .Where(o => o.PropertyName == "LastModifiedDate")
                .ToList();
            var lastModifiedByRules = validator.OfType<PropertyRule>()
                .Where(o => o.PropertyName == "LastModifiedBy")
                .ToList();

            Assert.AreEqual(1, lastModifiedDateRules.Count);
            Assert.AreEqual(ValidationRuleSet.Attribute, lastModifiedDateRules[0].RuleSet);
            Assert.AreEqual(1, lastModifiedDateRules[0].Validators.Count());
            Assert.AreEqual(typeof(NotNullValidator), lastModifiedDateRules[0].Validators.First().GetType());

            Assert.AreEqual(1, lastModifiedByRules.Count);
            Assert.AreEqual(ValidationRuleSet.Attribute, lastModifiedByRules[0].RuleSet);
            Assert.AreEqual(1, lastModifiedByRules[0].Validators.Count());
            Assert.AreEqual(typeof(NotNullValidator), lastModifiedByRules[0].Validators.First().GetType());
        }

        [Test]
        public void SaveVersionEntityWithStringUser()
        {
            var currentUser = Thread.CurrentPrincipal.Identity.Name;
            var car = new VersionCarWithStringUser { Model = "BMW" };
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                unitOfWork.Save(car); //A flush will happen as we set the id generator to indentity
                Assert.AreEqual(1, car.Id);
                Assert.AreEqual(1, car.Version);
                Assert.AreEqual(car.LastModifiedDate, car.CreatedDate);
                Assert.AreEqual(currentUser, car.LastModifiedBy);
                Assert.AreEqual(currentUser, car.CreatedBy);

                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Test"), new[] {"Role1"});
                var newCurrentUser = Thread.CurrentPrincipal.Identity.Name;
                Thread.Sleep(2000);

                //Update
                car.Model = "Audi";
                unitOfWork.Save(car);
                unitOfWork.Flush();
                Assert.AreEqual(2, car.Version);
                Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
                Assert.AreEqual(newCurrentUser, car.LastModifiedBy);
                Assert.AreEqual(currentUser, car.CreatedBy);
            }
            Assert.AreEqual(2, car.Version);
        }

        [Test]
        public void SaveVersionEntityWithStringUserNested()
        {
            var currentUser = Thread.CurrentPrincipal.Identity.Name;
            string newCurrentUser;
            var bmw = new VersionCarWithStringUser { Model = "BMW" };
            var bmwWheel1 = new VersionWheelWithStringUser { Dimension = 17 };
            var bmwWheel2 = new VersionWheelWithStringUser { Dimension = 17 };
            var audi = new VersionCarWithStringUser { Model = "AUDI" };
            var audiWheel1 = new VersionWheelWithStringUser { Dimension = 18 };
            var audiWheel2 = new VersionWheelWithStringUser { Dimension = 18 };

            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                unitOfWork.DefaultSession.FlushMode = FlushMode.Never;
                
                bmw.AddWheel(bmwWheel1);
                bmw.AddWheel(bmwWheel2);

                audi.AddWheel(audiWheel1);
                audi.AddWheel(audiWheel2);

                bmw.Child = audi;

                //Save only the root entity (because the default conventions the children will be saved too)
                unitOfWork.Save(bmw); //A flush will happen as we set the id generator to indentity

                Assert.AreEqual(false, bmw.IsTransient());
                Assert.AreEqual(1, bmw.Version);
                Assert.AreEqual(currentUser, bmw.CreatedBy);
                Assert.AreEqual(currentUser, bmw.LastModifiedBy);
                Assert.AreEqual(bmw.CreatedDate, bmw.LastModifiedDate);

                Assert.AreEqual(false, bmwWheel1.IsTransient());
                Assert.AreEqual(1, bmwWheel1.Version);
                Assert.AreEqual(currentUser, bmwWheel1.CreatedBy);
                Assert.AreEqual(currentUser, bmwWheel1.LastModifiedBy);
                Assert.AreEqual(bmwWheel1.CreatedDate, bmwWheel1.LastModifiedDate);

                Assert.AreEqual(false, bmwWheel2.IsTransient());
                Assert.AreEqual(1, bmwWheel2.Version);
                Assert.AreEqual(currentUser, bmwWheel2.CreatedBy);
                Assert.AreEqual(currentUser, bmwWheel2.LastModifiedBy);
                Assert.AreEqual(bmwWheel2.CreatedDate, bmwWheel2.LastModifiedDate);

                Assert.AreEqual(false, audi.IsTransient());
                Assert.AreEqual(1, audi.Version);
                Assert.AreEqual(currentUser, audi.CreatedBy);
                Assert.AreEqual(currentUser, audi.LastModifiedBy);
                Assert.AreEqual(audi.CreatedDate, audi.LastModifiedDate);

                Assert.AreEqual(false, audiWheel1.IsTransient());
                Assert.AreEqual(1, audiWheel1.Version);
                Assert.AreEqual(currentUser, audiWheel1.CreatedBy);
                Assert.AreEqual(currentUser, audiWheel1.LastModifiedBy);
                Assert.AreEqual(audiWheel1.CreatedDate, audiWheel1.LastModifiedDate);

                Assert.AreEqual(false, audiWheel2.IsTransient());
                Assert.AreEqual(1, audiWheel2.Version);
                Assert.AreEqual(currentUser, audiWheel2.CreatedBy);
                Assert.AreEqual(currentUser, audiWheel2.LastModifiedBy);
                Assert.AreEqual(audiWheel2.CreatedDate, audiWheel2.LastModifiedDate);

                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Test"), new[] { "Role1" });

                newCurrentUser = Thread.CurrentPrincipal.Identity.Name;

                Thread.Sleep(2000);

                //Update
                audiWheel1.Dimension = 19;

                unitOfWork.Flush();
            }

            Assert.AreEqual(2, audiWheel1.Version);
            Assert.AreNotEqual(audiWheel1.LastModifiedDate, audiWheel1.CreatedDate);
            Assert.AreEqual(currentUser, audiWheel1.CreatedBy);
            Assert.AreEqual(newCurrentUser, audiWheel1.LastModifiedBy);
            Assert.AreEqual(1, bmw.Version);
            Assert.AreEqual(1, bmwWheel1.Version);
            Assert.AreEqual(1, bmwWheel2.Version);
            Assert.AreEqual(1, audi.Version);
            Assert.AreEqual(1, audiWheel2.Version);

        }

        #endregion

        #region VersionEntityWithEntityUser

        [Test]
        public void ValidatorForVersionEntityWithEntityUserMustHaveRequiredLastModified()
        {
            var validator = Container.GetInstance<IValidator<VersionCarWithEntityUser>>() as IEnumerable;

            var lastModifiedDateRules = validator.OfType<PropertyRule>()
                .Where(o => o.PropertyName == "LastModifiedDate")
                .ToList();
            var lastModifiedByRules = validator.OfType<PropertyRule>()
                .Where(o => o.PropertyName == "LastModifiedBy")
                .ToList();

            Assert.AreEqual(1, lastModifiedDateRules.Count);
            Assert.AreEqual(ValidationRuleSet.Attribute, lastModifiedDateRules[0].RuleSet);
            Assert.AreEqual(1, lastModifiedDateRules[0].Validators.Count());
            Assert.AreEqual(typeof(NotNullValidator), lastModifiedDateRules[0].Validators.First().GetType());

            Assert.AreEqual(1, lastModifiedByRules.Count);
            Assert.AreEqual(ValidationRuleSet.Attribute, lastModifiedByRules[0].RuleSet);
            Assert.AreEqual(1, lastModifiedByRules[0].Validators.Count());
            Assert.AreEqual(typeof(NotNullValidator), lastModifiedByRules[0].Validators.First().GetType());
        }

        [Test]
        public void SaveVersionEntityWithEntityUser()
        {
            var currentUser = new GenericPrincipal(new GenericIdentity("Current"), new[] { "Role1" });
            var currentUserName = currentUser.Identity.Name;
            var newUser = new GenericPrincipal(new GenericIdentity("Test"), new[] { "Role1" });
            var newUserName = newUser.Identity.Name;
            var car = new VersionCarWithEntityUser { Model = "BMW" };
            var user = new User { UserName = currentUserName };
            var user2 = new User { UserName = newUserName };

            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                //Create two users
                unitOfWork.Save(user);
                unitOfWork.Save(user2);

                Thread.CurrentPrincipal = currentUser;

                unitOfWork.Save(car); //A flush will happen as we set the id generator to indentity
                Assert.AreEqual(1, car.Id);
                Assert.AreEqual(1, car.Version);
                Assert.AreEqual(car.LastModifiedDate, car.CreatedDate);
                Assert.AreEqual(currentUserName, car.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, car.LastModifiedBy.UserName);

                Thread.CurrentPrincipal = newUser;

                Thread.Sleep(2000);

                //UpdateContainer.GetInstance<IUnitOfWorkFactory>().Create()
                car.Model = "Audi";
                unitOfWork.Save(car);
                unitOfWork.Flush();
                Assert.AreEqual(2, car.Version);
                Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
                Assert.AreEqual(currentUserName, car.CreatedBy.UserName);
                Assert.AreEqual(newUserName, car.LastModifiedBy.UserName);
            }
            Assert.AreEqual(2, car.Version);
        }

        [Test]
        public void SaveVersionEntityWithEntityUserNested()
        {
            var currentUser = new GenericIdentity("Current");
            var currentUserName = currentUser.Name;
            const string newUserName = "Test";
            var bmw = new VersionCarWithEntityUser { Model = "BMW" };
            var bmwWheel1 = new VersionWheelWithEntityUser { Dimension = 17 };
            var bmwWheel2 = new VersionWheelWithEntityUser { Dimension = 17 };
            var audi = new VersionCarWithEntityUser { Model = "AUDI" };
            var audiWheel1 = new VersionWheelWithEntityUser { Dimension = 18 };
            var audiWheel2 = new VersionWheelWithEntityUser { Dimension = 18 };

            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                AuditUserProvider.CacheUserIds = true; //Will reduce the number of total queries from 15 to 10
                
                //Create two users
                var user = new User { UserName = currentUserName };
                unitOfWork.Save(user);
                currentUser.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)));

                Thread.CurrentPrincipal = new GenericPrincipal(currentUser, new[] { "Role1" });

                var user2 = new User { UserName = newUserName };
                unitOfWork.Save(user2);

                var genIndentity = new GenericIdentity(newUserName);
                genIndentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user2.Id.ToString(CultureInfo.InvariantCulture)));
                var newUser = new GenericPrincipal(genIndentity, new[] { "Role1" });

                unitOfWork.DefaultSession.FlushMode = FlushMode.Never;
                
                bmw.AddWheel(bmwWheel1);
                bmw.AddWheel(bmwWheel2);

                audi.AddWheel(audiWheel1);
                audi.AddWheel(audiWheel2);

                bmw.Child = audi;

                //Save only the root entity (because the default conventions the children will be saved too)
                unitOfWork.Save(bmw); //A flush will happen as we set the id generator to indentity

                Assert.AreEqual(false, bmw.IsTransient());
                Assert.AreEqual(1, bmw.Version);
                Assert.AreEqual(currentUserName, bmw.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, bmw.LastModifiedBy.UserName);
                Assert.AreEqual(bmw.CreatedDate, bmw.LastModifiedDate);

                Assert.AreEqual(false, bmwWheel1.IsTransient());
                Assert.AreEqual(1, bmwWheel1.Version);
                Assert.AreEqual(currentUserName, bmwWheel1.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, bmwWheel1.LastModifiedBy.UserName);
                Assert.AreEqual(bmwWheel1.CreatedDate, bmwWheel1.LastModifiedDate);

                Assert.AreEqual(false, bmwWheel2.IsTransient());
                Assert.AreEqual(1, bmwWheel2.Version);
                Assert.AreEqual(currentUserName, bmwWheel2.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, bmwWheel2.LastModifiedBy.UserName);
                Assert.AreEqual(bmwWheel2.CreatedDate, bmwWheel2.LastModifiedDate);

                Assert.AreEqual(false, audi.IsTransient());
                Assert.AreEqual(1, audi.Version);
                Assert.AreEqual(currentUserName, audi.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, audi.LastModifiedBy.UserName);
                Assert.AreEqual(audi.CreatedDate, audi.LastModifiedDate);

                Assert.AreEqual(false, audiWheel1.IsTransient());
                Assert.AreEqual(1, audiWheel1.Version);
                Assert.AreEqual(currentUserName, audiWheel1.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, audiWheel1.LastModifiedBy.UserName);
                Assert.AreEqual(audiWheel1.CreatedDate, audiWheel1.LastModifiedDate);

                Assert.AreEqual(false, audiWheel2.IsTransient());
                Assert.AreEqual(1, audiWheel2.Version);
                Assert.AreEqual(currentUserName, audiWheel2.CreatedBy.UserName);
                Assert.AreEqual(currentUserName, audiWheel2.LastModifiedBy.UserName);
                Assert.AreEqual(audiWheel2.CreatedDate, audiWheel2.LastModifiedDate);

                Thread.CurrentPrincipal = newUser;

                Thread.Sleep(2000);

                //Update
                audiWheel1.Dimension = 19;

                unitOfWork.Flush();
            }

            Assert.AreEqual(2, audiWheel1.Version);
            Assert.AreNotEqual(audiWheel1.LastModifiedDate, audiWheel1.CreatedDate);
            Assert.AreEqual(currentUserName, audiWheel1.CreatedBy.UserName);
            Assert.AreEqual(newUserName, audiWheel1.LastModifiedBy.UserName);
            Assert.AreEqual(1, bmw.Version);
            Assert.AreEqual(1, bmwWheel1.Version);
            Assert.AreEqual(1, bmwWheel2.Version);
            Assert.AreEqual(1, audi.Version);
            Assert.AreEqual(1, audiWheel2.Version);

        }

        #endregion
    }
}

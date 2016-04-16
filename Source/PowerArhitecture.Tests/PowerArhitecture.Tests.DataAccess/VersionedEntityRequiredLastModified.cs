using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Ninject;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Entities.Versioning;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.DataAccess
{
    [TestClass]
    public class VersionedEntityRequiredLastModified : BaseTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            EntityAssemblies.Add(typeof(UnitOfWorkSingleDbTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(IValidatorEngine).Assembly);
            BaseClassInitialize(testContext,
                CreateDatabaseConfiguration()
                .Conventions(c => c
                    .HiLoId(o => o.Enabled(false)
                    )
                    .RequiredLastModifiedProperty()
                )
            );
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            BaseClassCleanup();
        }

        #region VersionEntity

        [TestMethod]
        public void save_version_entity()
        {
            var car = new VersionCar { Model = "BMW" };
            using (var session = Kernel.Get<ISession>())
            using (var tx = session.BeginTransaction())
            {
                session.Save(car); //A flush will happen as we set the id generator to indentity
                Assert.AreEqual(1, car.Id);
                Assert.AreEqual(1, car.Version);
                Assert.AreEqual(car.LastModifiedDate, car.CreatedDate);
                car.Model = "Audi";
                session.Save(car);
                session.Flush();
                Assert.AreEqual(2, car.Version);
                Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
                tx.Commit();
            }
            Assert.AreEqual(2, car.Version);
        }

        [TestMethod]
        public void save_version_entity_nested()
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

            using (var session = Kernel.Get<ISession>())
            using (var tx = session.BeginTransaction())
            {
                session.FlushMode = FlushMode.Never;
                //Save only the root entity (because the default conventions the children will be saved too)
                session.Save(bmw); //A flush will happen as we set the id generator to indentity

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
                session.Flush();
                tx.Commit();
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

        [TestMethod]
        public void save_version_entity_with_string_user()
        {
            var currentUser = Thread.CurrentPrincipal.Identity.Name;

            var session = Kernel.Get<ISession>();
            var car = new VersionCarWithStringUser { Model = "BMW" };
            session.Save(car); //A flush will happen as we set the id generator to indentity
            Assert.AreEqual(1, car.Id);
            Assert.AreEqual(1, car.Version);
            Assert.AreEqual(car.LastModifiedDate, car.CreatedDate);
            Assert.AreEqual(currentUser, car.LastModifiedBy);
            Assert.AreEqual(currentUser, car.CreatedBy);

            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Test"), new[] { "Role1" });
            var newCurrentUser = Thread.CurrentPrincipal.Identity.Name;
            Thread.Sleep(2000);

            //Update
            car.Model = "Audi";
            session.Save(car);
            session.Flush();
            Assert.AreEqual(2, car.Version);
            Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
            Assert.AreEqual(newCurrentUser, car.LastModifiedBy);
            Assert.AreEqual(currentUser, car.CreatedBy);
            session.Dispose();
            Assert.AreEqual(2, car.Version);
        }

        [TestMethod]
        public void save_version_entity_with_string_user_nested()
        {
            var currentUser = Thread.CurrentPrincipal.Identity.Name;

            var session = Kernel.Get<ISession>();
            session.FlushMode = FlushMode.Never;
            var bmw = new VersionCarWithStringUser { Model = "BMW" };
            var bmwWheel1 = new VersionWheelWithStringUser { Dimension = 17 };
            var bmwWheel2 = new VersionWheelWithStringUser { Dimension = 17 };
            bmw.AddWheel(bmwWheel1);
            bmw.AddWheel(bmwWheel2);

            var audi = new VersionCarWithStringUser { Model = "AUDI" };
            var audiWheel1 = new VersionWheelWithStringUser { Dimension = 18 };
            var audiWheel2 = new VersionWheelWithStringUser { Dimension = 18 };
            audi.AddWheel(audiWheel1);
            audi.AddWheel(audiWheel2);

            bmw.Child = audi;

            //Save only the root entity (because the default conventions the children will be saved too)
            session.Save(bmw); //A flush will happen as we set the id generator to indentity

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

            var newCurrentUser = Thread.CurrentPrincipal.Identity.Name;

            Thread.Sleep(2000);

            //Update
            audiWheel1.Dimension = 19;

            session.Flush();
            session.Dispose();

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

        [TestMethod]
        public void save_version_entity_with_entity_user()
        {
            var session = Kernel.Get<ISession>();
            var currentUser = new GenericPrincipal(new GenericIdentity("Current"), new[] { "Role1" });
            var currentUserName = currentUser.Identity.Name;
            var newUser = new GenericPrincipal(new GenericIdentity("Test"), new[] { "Role1" });
            var newUserName = newUser.Identity.Name;
            //Create two users
            var user = new User { UserName = currentUserName };
            session.Save(user);
            var user2 = new User { UserName = newUserName };
            session.Save(user2);

            Thread.CurrentPrincipal = currentUser;

            var car = new VersionCarWithEntityUser { Model = "BMW" };
            session.Save(car); //A flush will happen as we set the id generator to indentity
            Assert.AreEqual(1, car.Id);
            Assert.AreEqual(1, car.Version);
            Assert.AreEqual(car.LastModifiedDate, car.CreatedDate);
            Assert.AreEqual(currentUserName, car.CreatedBy.UserName);
            Assert.AreEqual(currentUserName, car.LastModifiedBy.UserName);

            Thread.CurrentPrincipal = newUser;

            Thread.Sleep(2000);

            //Update
            car.Model = "Audi";
            session.Save(car);
            session.Flush();
            Assert.AreEqual(2, car.Version);
            Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
            Assert.AreEqual(currentUserName, car.CreatedBy.UserName);
            Assert.AreEqual(newUserName, car.LastModifiedBy.UserName);



            session.Dispose();
            Assert.AreEqual(2, car.Version);
        }

        [TestMethod]
        public void save_version_entity_with_entity_user_nested()
        {
            var session = Kernel.Get<ISession>();
            AuditUserProvider.CacheUserIds = true; //Will reduce the number of total queries from 15 to 10
            var currentUser = new GenericIdentity("Current");
            var currentUserName = currentUser.Name;
            const string newUserName = "Test";
            //Create two users
            var user = new User { UserName = currentUserName };
            session.Save(user);
            currentUser.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)));

            Thread.CurrentPrincipal = new GenericPrincipal(currentUser, new[] { "Role1" });

            var user2 = new User { UserName = newUserName };
            session.Save(user2);

            var genIndentity = new GenericIdentity(newUserName);
            genIndentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user2.Id.ToString(CultureInfo.InvariantCulture)));
            var newUser = new GenericPrincipal(genIndentity, new[] { "Role1" });

            session.FlushMode = FlushMode.Never;
            var bmw = new VersionCarWithEntityUser { Model = "BMW" };
            var bmwWheel1 = new VersionWheelWithEntityUser { Dimension = 17 };
            var bmwWheel2 = new VersionWheelWithEntityUser { Dimension = 17 };
            bmw.AddWheel(bmwWheel1);
            bmw.AddWheel(bmwWheel2);

            var audi = new VersionCarWithEntityUser { Model = "AUDI" };
            var audiWheel1 = new VersionWheelWithEntityUser { Dimension = 18 };
            var audiWheel2 = new VersionWheelWithEntityUser { Dimension = 18 };
            audi.AddWheel(audiWheel1);
            audi.AddWheel(audiWheel2);

            bmw.Child = audi;

            //Save only the root entity (because the default conventions the children will be saved too)
            session.Save(bmw); //A flush will happen as we set the id generator to indentity

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

            session.Flush();
            session.Dispose();

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

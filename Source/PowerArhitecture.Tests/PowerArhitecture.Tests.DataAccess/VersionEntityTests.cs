using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Event;
using NHibernate.Impl;
using Ninject;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Entities.Versioning;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.DataAccess
{
    [TestClass]
    public class VersionEntityTests : BaseTest
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
                Assert.IsNull(car.LastModifiedDate);
                Assert.AreEqual(DateTime.UtcNow.Date, car.CreatedDate.Date);
                car.Model = "Audi";
                session.Save(car);
                session.Flush();
                Assert.AreEqual(2, car.Version);
                Assert.IsNotNull(car.LastModifiedDate);
                Assert.AreNotEqual(car.LastModifiedDate, car.CreatedDate);
                tx.Commit();
            }
            Assert.AreEqual(2, car.Version);
        }

        #endregion

        
    }
}

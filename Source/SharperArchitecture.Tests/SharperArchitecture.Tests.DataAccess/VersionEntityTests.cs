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
using NHibernate;
using NHibernate.Event;
using NHibernate.Impl;
using NUnit.Framework;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Providers;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.DataAccess.Entities;
using SharperArchitecture.Tests.DataAccess.Entities.Versioning;
using SharperArchitecture.Validation;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace SharperArchitecture.Tests.DataAccess
{
    [TestFixture]
    public class VersionEntityTests : DatabaseBaseTest
    {
        public VersionEntityTests()
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
                );
        }


        #region VersionEntity

        [Test]
        public void save_version_entity()
        {
            var car = new VersionCar { Model = "BMW" };
            using (Container.BeginExecutionContextScope())
            using (var session = Container.GetInstance<ISession>())
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

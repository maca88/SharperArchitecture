using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Transactions;
using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NUnit.Framework;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.DataAccess.Entities;
using SharperArchitecture.Tests.DataAccess.MultiDatabase;
using SharperArchitecture.Tests.DataAccess.MultiDatabase.BazModels;
using SharperArchitecture.Validation;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace SharperArchitecture.Tests.DataAccess
{
    [TestFixture]
    public class MultiDatabasesTests : DatabaseBaseTest
    {
        public MultiDatabasesTests()
        {
            EntityAssemblies.Add(typeof(LifecycleTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(MultiDatabasesTests).Assembly);
        }

        protected override IEnumerable<IFluentDatabaseConfiguration> GetDatabaseConfigurations()
        {
            yield return CreateDatabaseConfiguration()
                .AutomappingConfiguration(o => o.ShouldMapType(t => !t.Namespace.Contains("BazModel")));
            yield return CreateDatabaseConfiguration("bar", "bar")
                .AutomappingConfiguration(o => o.ShouldMapType(t => !t.Namespace.Contains("BazModel")));
            yield return CreateDatabaseConfiguration("baz", "baz")
                .AutomappingConfiguration(o => o.ShouldMapType(t => t.Namespace.Contains("BazModel")));
        }

        [Test]
        public void EachDatabaseMustHaveItsOwnSessionFactory()
        {
            var sfProvider = Container.GetInstance<ISessionFactoryProvider>();
            var fooSf = sfProvider.Get();
            var barSf = sfProvider.Get("bar");

            Assert.NotNull(fooSf);
            Assert.NotNull(barSf);
            Assert.AreNotEqual(fooSf, barSf);
        }

        [Test]
        public void BazModelShouldHaveOnlyOneDatabaseConfiguration()
        {
            var configs = Database.GetDatabaseConfigurationsForModel<BazModel>();
            Assert.AreEqual(1, configs.Count);
            Assert.AreEqual("baz", configs.First().Name);
        }

        [Test]
        public void EachDatabaseMustHaveItsOwnSession()
        {
            var sProvider = Container.GetInstance<ISessionProvider>();
            var sfProvider = Container.GetInstance<ISessionFactoryProvider>();
            var sf = sfProvider.Get();
            var sfBar = sfProvider.Get("bar");

            sf.Statistics.Clear();
            sfBar.Statistics.Clear();

            using (Container.BeginExecutionContextScope())
            {
                var session = sProvider.Get();
                var barSession = sProvider.Get("bar");

                Assert.AreEqual(0, sf.Statistics.SessionOpenCount);
                Assert.AreEqual(0, sfBar.Statistics.SessionOpenCount);
                Assert.NotNull(session);
                Assert.NotNull(barSession);
                Assert.AreNotEqual(barSession, session);
                Assert.AreEqual(sf, session.SessionFactory);
                Assert.AreEqual(sfBar, barSession.SessionFactory);
                Assert.AreEqual(1, sf.Statistics.SessionOpenCount);
                Assert.AreEqual(1, sfBar.Statistics.SessionOpenCount);
                Assert.AreNotEqual(session.SessionFactory, barSession.SessionFactory);
                Assert.AreEqual(0, sf.Statistics.SessionCloseCount);
                Assert.AreEqual(0, sfBar.Statistics.SessionCloseCount);
            }
            Assert.AreEqual(1, sf.Statistics.SessionCloseCount);
            Assert.AreEqual(1, sfBar.Statistics.SessionCloseCount);
        }

        [Test]
        public void UnitOfWorkShouldFallbackToDefaultDatabaseWhenRetrievingEntityThatIsContainedInMultipleDatabases()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create())
            {
                unitOfWork.Query<AttrLazyLoad>().ToList();
            }
        }

        [Test]
        public void UnitOfWorkShouldRevertAllDataIfAnySessionFails()
        {
            Assert.Throws<GenericADOException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        
                        var session = Container.GetInstance<ISessionProvider>().Get();
                        var barSession = Container.GetInstance<ISessionProvider>().Get("bar");

                        var model1 = new AttrIndexAttribute { Index1 = "Test", SharedIndex1 = DateTime.Now };
                        var model2 = new AttrIndexAttribute { Index1 = "Test2Loong" };
                        session.Save(model1);
                        barSession.Save(model2);

                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }

            });

            using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
            {
                try
                {
                    var session = Container.GetInstance<ISessionProvider>().Get();
                    var barSession = Container.GetInstance<ISessionProvider>().Get("bar");

                    Assert.AreEqual(0, session.Query<AttrIndexAttribute>().Count());
                    Assert.AreEqual(0, barSession.Query<AttrIndexAttribute>().Count());
                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }

        }
    }
}

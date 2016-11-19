using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel.Collections;
using NHibernate;
using NHibernate.Impl;
using NUnit.Framework;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Extensions;
using PowerArhitecture.Tests.DataAccess.MultiDatabase;
using PowerArhitecture.Tests.DataAccess.MultiDatabase.BazModels;
using PowerArhitecture.Validation;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Extensions;

namespace PowerArhitecture.Tests.DataAccess
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

        protected override void ConfigureContainer(Container container)
        {
            container.Options.DependencyInjectionBehavior = new KeyedDependencyInjectionBehavior(container);
            base.ConfigureContainer(container);
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
            var multiSf = Container.GetInstance<MultiSessionFactories>();

            Assert.NotNull(multiSf.DefaultSessionFactory);
            Assert.NotNull(multiSf.BarSessionFactory);
            Assert.AreNotEqual(multiSf.DefaultSessionFactory, multiSf.BarSessionFactory);
        }

        [Test]
        public void BazModelShouldHaveOnlyOneDatabaseConfiguration()
        {
            var configs = Database.GetDatabaseConfigurationsForModel<BazModel>();
            Assert.AreEqual(1, configs.Count);
            Assert.AreEqual("baz", configs.First().Name);
        }

        [Test]
        public void ShouldThrowIfIncorrectDatabaseConfigurationNameIsPovidedForRepository()
        {
            Assert.Throws<ActivationException>(() =>
            {
                using (Container.BeginExecutionContextScope())
                {
                    Container.GetInstance<IRepository<BazModel>>("foo");
                }
            });
        }

        [Test]
        public void BazModelRepositoryShouldHaveCorrectSession()
        {
            using (Container.BeginExecutionContextScope())
            {
                var bazRepo = Container.GetInstance<IRepository<BazModel>>();
                var bazRepo2 = Container.GetInstance<IRepository<BazModel>>("baz");
                var sf = bazRepo.GetSession().SessionFactory;

                Assert.AreEqual(sf, bazRepo2.GetSession().SessionFactory);
                Assert.AreEqual(bazRepo.GetSession(), bazRepo2.GetSession());
                Assert.IsTrue(bazRepo.GetSession().Connection.ConnectionString.Contains("baz"));
            }
        }

        [Test]
        public void EachDatabaseMustHaveItsOwnSession()
        {
            var multiSf = Container.GetInstance<MultiSessionFactories>();
            var sf = multiSf.DefaultSessionFactory;
            var sfBar = multiSf.BarSessionFactory;
            sf.Statistics.Clear();
            sfBar.Statistics.Clear();

            using (Container.BeginExecutionContextScope())
            {
                var multiS = Container.GetInstance<MultiSessions>();
                var session = multiS.DefaultSession;
                var barSession = multiS.BarSession;

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
        public void EachDatabaseMustHaveItsOwnGenericRepository()
        {
            using (Container.BeginExecutionContextScope())
            {
                var sf = Container.GetInstance<ISessionFactory>();
                sf.Statistics.Clear();

                var multiSf = Container.GetInstance<MultiGenericRepositories>();
                var defSession = multiSf.Repository.GetMemberValue("Session") as ISession;
                var barSession = multiSf.BarRepository.GetMemberValue("Session") as ISession;

                Assert.NotNull(defSession);
                Assert.NotNull(barSession);
                Assert.AreEqual(0, sf.Statistics.SessionOpenCount);
                Assert.AreEqual(sf, defSession.SessionFactory);
                Assert.AreEqual(1, sf.Statistics.SessionOpenCount);
                Assert.AreNotEqual(defSession, barSession);
                Assert.AreNotEqual(defSession.SessionFactory, barSession.SessionFactory);
            }
        }

        [Test]
        public void UnitOfWorkShouldFallbackToDefaultDatabaseWhenRetrievingEntityThatIsContainedInMultipleDatabases()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWork>())
            {
                unitOfWork.GetRepository<AttrLazyLoad>();
            }
        }

        [Test]
        public void UnitOfWorkShouldThrowWhenAnInvalidDatabaseConfigurationNameIsProvided()
        {
            Assert.Throws<ActivationException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWork>())
                {
                    unitOfWork.GetRepository<AttrLazyLoad>("baz");
                }
            });
        }

        [Test]
        public void UnitOfWorkShouldWorkWithMultipleDatabasesAndCustomRepositories()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    Assert.AreEqual(0, unitOfWork.GetActiveSessions().Count());

                    var repo = unitOfWork.GetRepository<AttrLazyLoad>();
                    var repo2 = unitOfWork.GetRepository<AttrLazyLoad>("bar");
                    var repo3 = unitOfWork.GetCustomRepository<IAttrLazyLoadRepository>("bar");

                    Assert.AreNotEqual(repo, repo2);
                    Assert.AreNotEqual(repo.GetSession(), repo2.GetSession());
                    Assert.AreEqual(repo2.GetSession(), repo3.GetSession());
                    Assert.AreEqual(2, unitOfWork.GetActiveSessions().Count());
                    Assert.IsTrue(unitOfWork.GetActiveSessions().All(o => o.Transaction.IsActive));

                    var model1 = new AttrLazyLoad {Name = "Test"};
                    var model2 = new AttrLazyLoad { Name = "Test2" };
                    repo.Save(model1);
                    repo3.Save(model2);

                    Assert.AreNotEqual(0, model1.Id);
                    Assert.AreEqual(model1.Id, model2.Id);
                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void UnitOfWorkShouldWorkWithMultipleDatabasesAndGenericRepositories()
        {
            using (var unitOfWork = Container.GetInstance<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    Assert.AreEqual(0, unitOfWork.GetActiveSessions().Count());
                    Assert.IsTrue(unitOfWork.GetActiveSessions().All(o => o.Transaction.IsActive));

                    var repo = unitOfWork.GetRepository<AttrIndexAttribute>();
                    var repo2 = unitOfWork.GetRepository<AttrIndexAttribute>("bar");
                    var repo3 = unitOfWork.GetRepository<AttrIndexAttribute>("bar");

                    Assert.AreEqual(2, unitOfWork.GetActiveSessions().Count());
                    Assert.AreNotEqual(repo.GetSession(), repo2.GetSession());
                    Assert.AreEqual(repo2.GetSession(), repo3.GetSession());
                    Assert.IsTrue(unitOfWork.GetActiveSessions().All(o => o.Transaction.IsActive));

                    var model1 = new AttrIndexAttribute { Index1 = "Test", SharedIndex1 = DateTime.Now };
                    var model2 = new AttrIndexAttribute { Index1 = "Test2", SharedIndex1 = DateTime.Now };
                    repo.Save(model1);
                    repo3.Save(model2);

                    Assert.AreNotEqual(0, model1.Id);
                    Assert.AreEqual(model1.Id, model2.Id);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        [Test]
        public void UnitOfWorkShouldRevertAllDataIfAnySessionFails()
        {
            Assert.Throws<SqlTypeException>(() =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    try
                    {
                        var repo = unitOfWork.GetRepository<AttrIndexAttribute>();
                        var repo2 = unitOfWork.GetRepository<AttrIndexAttribute>("bar");

                        var model1 = new AttrIndexAttribute { Index1 = "Test", SharedIndex1 = DateTime.Now };
                        var model2 = new AttrIndexAttribute { Index1 = "Test2" };
                        repo.Save(model1);
                        repo2.Save(model2);

                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }

            });

            using (var unitOfWork = Container.GetInstance<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    var repo = unitOfWork.GetRepository<AttrIndexAttribute>();
                    var repo2 = unitOfWork.GetRepository<AttrIndexAttribute>("bar");

                    Assert.AreEqual(0, repo.Query().Count());
                    Assert.AreEqual(0, repo2.Query().Count());
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

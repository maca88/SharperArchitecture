using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel.Collections;
using Ninject;
using NUnit.Framework;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Extensions;
using PowerArhitecture.Tests.DataAccess.MultiDatabase;
using PowerArhitecture.Validation.Specifications;

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
            TestAssemblies.Add(typeof(IValidatorEngine).Assembly);
        }

        protected override IEnumerable<IFluentDatabaseConfiguration> GetDatabaseConfigurations()
        {
            foreach (var config in base.GetDatabaseConfigurations())
            {
                yield return config;
            }
            yield return CreateDatabaseConfiguration("bar", "bar");
        }

        [Test]
        public void EachDatabaseMustHaveItsOwnSessionFacotry()
        {
            var multiSf = Kernel.Get<MultiSessionFactories>();

            Assert.NotNull(multiSf.DefaultSessionFactory);
            Assert.NotNull(multiSf.BarSessionFactory);
            Assert.AreNotEqual(multiSf.DefaultSessionFactory, multiSf.BarSessionFactory);
        }

        [Test]
        public void EachDatabaseMustHaveItsOwnSession()
        {
            var multiSf = Kernel.Get<MultiSessions>();

            Assert.NotNull(multiSf.DefaultSession);
            Assert.NotNull(multiSf.BarSession);
            Assert.AreNotEqual(multiSf.DefaultSession, multiSf.BarSession);
            Assert.AreNotEqual(multiSf.DefaultSession.SessionFactory, multiSf.BarSession.SessionFactory);
        }

        [Test]
        public void UnitOfWorkShouldFallbackToDefaultDatabaseWhenRetrievingEntityThatIsContainedInMultipleDatabases()
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>())
            {
                var repo = unitOfWork.GetRepository<AttrLazyLoad>();
            }
        }

        [Test]
        public void UnitOfWorkShouldThrowWhenAnInvalidDatabaseConfigurationNameIsProvided()
        {
            Assert.Throws<PowerArhitectureException>(() =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>())
                {
                    var repo = unitOfWork.GetRepository<AttrLazyLoad>("baz");
                }
            });
        }

        [Test]
        public void UnitOfWorkShouldWorkWithMultipleDatabasesAndCustomRepositories()
        {
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    Assert.AreEqual(0, unitOfWork.GetActiveSessions().Count());

                    var repo = unitOfWork.GetRepository<AttrLazyLoad>();
                    var repo2 = unitOfWork.GetRepository<AttrLazyLoad>("bar");
                    var repo3 = unitOfWork.GetCustomRepository<IAttrLazyLoadRepository>("bar");

                    Assert.AreNotEqual(repo, repo2);
                    Assert.AreEqual(repo2, repo3);
                    Assert.AreEqual(2, unitOfWork.GetActiveSessions().Count());
                    Assert.AreNotEqual(repo.GetSession(), repo2.GetSession());
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
            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
            {
                try
                {
                    Assert.AreEqual(0, unitOfWork.GetActiveSessions().Count());
                    Assert.IsTrue(unitOfWork.GetActiveSessions().All(o => o.Transaction.IsActive));

                    var repo = unitOfWork.GetRepository<AttrIndexAttribute>();
                    var repo2 = unitOfWork.GetRepository<AttrIndexAttribute>("bar");
                    var repo3 = unitOfWork.GetRepository<AttrIndexAttribute>("bar");

                    Assert.AreNotEqual(repo, repo2);
                    Assert.AreEqual(repo2, repo3);
                    Assert.AreEqual(2, unitOfWork.GetActiveSessions().Count());
                    Assert.AreNotEqual(repo.GetSession(), repo2.GetSession());
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
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
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

            using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
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

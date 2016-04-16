using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Transform;
using Ninject;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.DataAccess
{
    [TestClass]
    public class UnitOfWorkSingleDbTests : BaseTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            EntityAssemblies.Add(typeof(UnitOfWorkSingleDbTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(IValidatorEngine).Assembly);
            BaseClassInitialize(testContext, CreateDatabaseConfiguration());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            BaseClassCleanup();
        }

        [TestMethod]
        public void session_within_a_unit_of_work()
        {
            var unitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            using (var unitOfWork = (UnitOfWork)unitOfWorkFactory.GetNew())
            {  
                var repo1 = (Repository<AttrIndexAttribute, long>) unitOfWork.GetRepository<AttrIndexAttribute>();
                var repo2 = (Repository<AttrLazyLoad, long>)unitOfWork.GetRepository<AttrLazyLoad>();
                var session = unitOfWork.ResolutionRoot.Get<ISession>();

                Assert.AreEqual(unitOfWork.Session, repo1.Session);
                Assert.AreEqual(unitOfWork.Session, repo2.Session);
                Assert.AreEqual(unitOfWork.Session, session);
            }
        }

        [TestMethod]
        public void session_within_a_request()
        {
            HttpContextSetup();
            var session = Kernel.Get<ISession>();
            var repo1 = (Repository<AttrIndexAttribute, long>)Kernel.Get<IRepository<AttrIndexAttribute, long>>();
            var repo2 = (Repository<AttrLazyLoad, long>)Kernel.Get<IRepository<AttrLazyLoad, long>>();
            Assert.AreEqual(session, repo1.Session);
            Assert.AreEqual(session, repo2.Session);
        }

        [TestMethod]
        public void session_within_multiple_requests()
        {
            HttpContextSetup();
            var session = Kernel.Get<ISession>();
            var repo1 = (Repository<AttrIndexAttribute, long>)Kernel.Get<IRepository<AttrIndexAttribute, long>>();
            Assert.AreEqual(session, repo1.Session);
            
            HttpContextSetup();
            var session2 = Kernel.Get<ISession>();
            var repo2 = (Repository<AttrLazyLoad, long>)Kernel.Get<IRepository<AttrLazyLoad, long>>();
            Assert.AreEqual(session2, repo2.Session);

            Assert.AreNotEqual(session, session2);
        }

        [TestMethod]
        public void session_non_managed()
        {
            var session = Kernel.Get<ISession>();
            var session2 = Kernel.Get<ISession>();
            Assert.AreNotEqual(session, session2);
        }


        /*
        [TestMethod]
        [ExpectedException(typeof(TransactionException))]
        public async Task IsolationLevelRepeatableReadDeadLock()
        {
            FillData();
            var server1 = Task.Factory.StartNew(() =>
                {
                    using (var unitOfWork = UnitOfWorkFactory.GetNew(IsolationLevel.RepeatableRead))
                    {
                        var person = unitOfWork
                            .GetEntityQuery<EQBPerson>(1)
                            .SingleOrDefault();
                        person.Age = 35;
                        unitOfWork.Save(person);
                        Thread.Sleep(4000);
                    } //kaaaa booom
                });

            Thread.Sleep(1000);
            var server2 = Task.Factory.StartNew(() =>
                {
                    using (var unitOfWork = UnitOfWorkFactory.GetNew(IsolationLevel.RepeatableRead))
                    {
                        var person = unitOfWork
                            .GetEntityQuery<EQBPerson>(1)
                            .SingleOrDefault();
                        person.Age = 36;
                        unitOfWork.Save(person);
                    } //first commits, this transaction will be commited after the upper one will be rollebacked (after 4sek)
                });

            await server1;
            await server2;
        }

        [TestMethod]
        [ExpectedException(typeof(TransactionException))]
        public async Task IsolationLevelSnapshotConflict()
        {
            FillData();
            var server1 = Task.Factory.StartNew(() =>
            {
                using (var unitOfWork = UnitOfWorkFactory.GetNew(IsolationLevel.Snapshot))
                {
                    var person = unitOfWork
                        .GetEntityQuery<EQBPerson>(2)
                        .SingleOrDefault();
                    person.Age = 35;
                    unitOfWork.Save(person);
                    Thread.Sleep(1000);
                } //this transaction will be immediately committed
            });

            var server2 = Task.Factory.StartNew(() =>
            {
                using (var unitOfWork = UnitOfWorkFactory.GetNew(IsolationLevel.Snapshot))
                {
                    var person = unitOfWork
                        .GetEntityQuery<EQBPerson>(1)
                        .SingleOrDefault();
                    Thread.Sleep(2000);
                    var oldAge = person.Age;
                    unitOfWork.Refresh(person);
                    Assert.AreEqual(oldAge, person.Age);
                    person.Age = 36;
                    unitOfWork.Save(person);
                } //kaaa buum - even if we are updating different person
            });

            await server1;
            await server2;
        }

        [TestMethod]
        public async Task Locking()
        {
            FillData();

            var server1 = Task.Factory.StartNew(() =>
                {
                    using (var unitOfWork = UnitOfWorkFactory.GetNew(IsolationLevel.RepeatableRead))
                    {
                        var person = unitOfWork
                            .GetEntityQuery<EQBPerson>(1, LockMode.Write)
                            .Include(o => o.IdentityCard)
                            .SingleOrDefault();
                        person.Age = 35;
                        unitOfWork.Save(person);
                        Thread.Sleep(10000);
                    }
                });
            Thread.Sleep(1000);
            var server2 = Task.Factory.StartNew(() =>
            {
                using (var unitOfWork = UnitOfWorkFactory.GetNew(IsolationLevel.RepeatableRead))
                {
                    var person = unitOfWork
                        .GetEntityQuery<EQBPerson>(1, LockMode.Write)
                        .Timeout(2) //Not working
                        .Include(o => o.IdentityCard)
                        .SingleOrDefault();
                    person.Age = 36;
                    unitOfWork.Save(person);
                }
            });
           
            await server1;
            await server2;

            using (var unitOfWork = UnitOfWorkFactory.GetNew())
            {
                Assert.AreEqual(unitOfWork.Get<EQBPerson>(1).Age, 36);
            }
        }*/
    }



}

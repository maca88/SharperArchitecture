using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NHibernate;
using NHibernate.Transform;
using Ninject;
using Nito.AsyncEx;
using NUnit.Framework;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Extensions;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.DataAccess
{
    [TestFixture]
    public class LifecycleTests : DatabaseBaseTest
    {
        public LifecycleTests()
        {
            EntityAssemblies.Add(typeof(LifecycleTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(IValidatorEngine).Assembly);
        }

        [Test]
        public void SessionWithinUnitOfWork()
        {
            var unitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            using (var unitOfWork = unitOfWorkFactory.GetNew().GetUnitOfWorkImplementation())
            {  
                var repo1 = (Repository<AttrIndexAttribute, long>) unitOfWork.GetRepository<AttrIndexAttribute>();
                var repo2 = (Repository<AttrLazyLoad, long>)unitOfWork.GetRepository<AttrLazyLoad>();
                var session = unitOfWork.ResolutionRoot.Get<ISession>();

                Assert.AreEqual(unitOfWork.Session, repo1.GetSession());
                Assert.AreEqual(unitOfWork.Session, repo2.GetSession());
                Assert.AreEqual(unitOfWork.Session, session);
            }
        }

        [Test]
        public void SessionWithinUnitOfWorkAsyncFlow()
        {
            AsyncContext.Run(async () =>
            {
                using (var unitOfWork = Kernel.Get<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    await Task.Delay(100);

                    var repo1 = (Repository<AttrIndexAttribute, long>) unitOfWork.GetRepository<AttrIndexAttribute>();
                    var repo2 = (Repository<AttrLazyLoad, long>) unitOfWork.GetRepository<AttrLazyLoad>();
                    var session = unitOfWork.ResolutionRoot.Get<ISession>();

                    Assert.AreEqual(unitOfWork.Session, repo1.GetSession());
                    Assert.AreEqual(unitOfWork.Session, repo2.GetSession());
                    Assert.AreEqual(unitOfWork.Session, session);

                    var id = Thread.CurrentThread.ManagedThreadId;
                    await Task.Delay(100).ConfigureAwait(false);
                    Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);

                    repo1 = (Repository<AttrIndexAttribute, long>) unitOfWork.GetRepository<AttrIndexAttribute>();
                    repo2 = (Repository<AttrLazyLoad, long>) unitOfWork.GetRepository<AttrLazyLoad>();
                    session = unitOfWork.ResolutionRoot.Get<ISession>();

                    Assert.AreEqual(unitOfWork.Session, repo1.GetSession());
                    Assert.AreEqual(unitOfWork.Session, repo2.GetSession());
                    Assert.AreEqual(unitOfWork.Session, session);
                }
            });
        }

        [Test]
        public void SessionWithinUnitOfWorkAndRequest()
        {
            HttpContextSetup();
            var reqSession = Kernel.Get<ISession>();

            var unitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            using (var unitOfWork = unitOfWorkFactory.GetNew().GetUnitOfWorkImplementation())
            {
                var repo1 = (Repository<AttrIndexAttribute, long>)unitOfWork.GetRepository<AttrIndexAttribute>();
                var repo2 = (Repository<AttrLazyLoad, long>)unitOfWork.GetRepository<AttrLazyLoad>();
                var session = unitOfWork.ResolutionRoot.Get<ISession>();

                Assert.AreEqual(unitOfWork.Session, repo1.GetSession());
                Assert.AreEqual(unitOfWork.Session, repo2.GetSession());
                Assert.AreEqual(unitOfWork.Session, session);
                Assert.AreNotEqual(reqSession, session);
            }
        }

        [Test]
        public void SessionWithinRequest()
        {
            HttpContextSetup();
            var session = Kernel.Get<ISession>();
            var repo1 = (Repository<AttrIndexAttribute, long>)Kernel.Get<IRepository<AttrIndexAttribute, long>>();
            var repo2 = (Repository<AttrLazyLoad, long>)Kernel.Get<IRepository<AttrLazyLoad, long>>();
            Assert.AreEqual(session, repo1.GetSession());
            Assert.AreEqual(session, repo2.GetSession());
        }

        [Test]
        public void SessionWithinMultipleRequests()
        {
            HttpContextSetup();
            var session = Kernel.Get<ISession>();
            var repo1 = (Repository<AttrIndexAttribute, long>)Kernel.Get<IRepository<AttrIndexAttribute, long>>();
            Assert.AreEqual(session, repo1.GetSession());
            
            HttpContextSetup();
            var session2 = Kernel.Get<ISession>();
            var repo2 = (Repository<AttrLazyLoad, long>)Kernel.Get<IRepository<AttrLazyLoad, long>>();
            Assert.AreEqual(session2, repo2.GetSession());

            Assert.AreNotEqual(session, session2);
        }

        [Test]
        public void SessionNonManaged()
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

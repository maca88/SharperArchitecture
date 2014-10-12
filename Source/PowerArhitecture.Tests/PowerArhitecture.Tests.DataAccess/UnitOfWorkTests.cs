using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Transform;
using LockMode = PowerArhitecture.DataAccess.Enums.LockMode;

namespace PowerArhitecture.Tests.DataAccess
{
    /*
    [TestClass]
    public class UnitOfWorkTests : EntityQueryBuilderTests
    {
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
        }
    }*/
}

using System;
using System.CodeDom;
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
using Nito.AsyncEx;
using NUnit.Framework;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Attributes;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Extensions;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Specifications;
using SimpleInjector.Extensions;
using SimpleInjector.Extensions.ExecutionContextScoping;
using ActivationException = SimpleInjector.ActivationException;

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
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(LifecycleTests).Assembly);
        }

        [Test]
        public void SessionWithinUnitOfWork()
        {
            var unitOfWorkFactory = Container.GetInstance<IUnitOfWorkFactory>();
            using (var unitOfWork = unitOfWorkFactory.Create().GetUnitOfWorkImplementation())
            {  
                var repo1 = unitOfWork.GetRepository<AttrIndexAttribute>();
                var repo2 = unitOfWork.GetRepository<AttrLazyLoad>();
                var repo3 = unitOfWork.GetCustomRepository<IAttrLazyLoadRepository>();
                var repo4 = unitOfWork.GetCustomRepository<IAttrLazyLoadRepository>();

                Assert.AreEqual(typeof(Repository<AttrIndexAttribute>), repo1.GetType());
                Assert.AreEqual(typeof(AttrLazyLoadRepository), repo2.GetType());
                Assert.AreEqual(typeof(AttrLazyLoadRepository), repo3.GetType());
                Assert.AreEqual(typeof(AttrLazyLoadRepository), repo4.GetType());
                var session = Container.GetInstance<ISession>();

                Assert.AreEqual(unitOfWork.DefaultSession, repo1.GetSession());
                Assert.AreEqual(unitOfWork.DefaultSession, repo2.GetSession());
                Assert.AreEqual(unitOfWork.DefaultSession, repo3.GetSession());
                Assert.AreEqual(unitOfWork.DefaultSession, repo4.GetSession());
                Assert.AreEqual(unitOfWork.DefaultSession, session);
            }
        }

        [Test]
        public void UnmanagedSessionShouldThrow()
        {
            Assert.Throws<ActivationException>(() =>
            {
                Container.GetInstance<ISession>();
            });
        }

        [Test]
        public void SessionShouldBeLazy()
        {
            var sf = Container.GetInstance<ISessionFactory>();
            sf.Statistics.Clear();

            using (Container.BeginExecutionContextScope())
            {
                var session = Container.GetInstance<ISession>();
                Assert.AreEqual(0, sf.Statistics.SessionOpenCount);
                Assert.AreEqual(sf, session.SessionFactory);
                Assert.AreEqual(1, sf.Statistics.SessionOpenCount);
            }
            Assert.AreEqual(1, sf.Statistics.SessionCloseCount);
        }

        [Test]
        public void LazySessionShouldThrowIfInitializedOutsideScope()
        {
            var sf = Container.GetInstance<ISessionFactory>();
            sf.Statistics.Clear();

            ISession session;
            using (Container.BeginExecutionContextScope())
            {
                session = Container.GetInstance<ISession>();
                Assert.AreEqual(0, sf.Statistics.SessionOpenCount);
            }

            Assert.Throws<ActivationException>(() =>
            {
                var sf2 = session.SessionFactory;
            });
        }

        [Test]
        public void SessionWithinUnitOfWorkAsyncFlow()
        {
            AsyncContext.Run(async () =>
            {
                using (var unitOfWork = Container.GetInstance<IUnitOfWork>().GetUnitOfWorkImplementation())
                {
                    await Task.Delay(100);

                    var repo1 = (Repository<AttrIndexAttribute, long>) unitOfWork.GetRepository<AttrIndexAttribute>();
                    var repo2 = (Repository<AttrLazyLoad, long>) unitOfWork.GetRepository<AttrLazyLoad>();
                    var session = Container.GetInstance<ISession>();

                    Assert.AreEqual(unitOfWork.DefaultSession, repo1.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, repo2.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, session);

                    var id = Thread.CurrentThread.ManagedThreadId;
                    await Task.Delay(100).ConfigureAwait(false);
                    Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);

                    repo1 = (Repository<AttrIndexAttribute, long>) unitOfWork.GetRepository<AttrIndexAttribute>();
                    repo2 = (Repository<AttrLazyLoad, long>) unitOfWork.GetRepository<AttrLazyLoad>();
                    session = Container.GetInstance<ISession>();

                    Assert.AreEqual(unitOfWork.DefaultSession, repo1.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, repo2.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, session);
                }
            });
        }

        [Test]
        public void SessionWithinUnitOfWorkAndGlobalScopeMustBeDifferent()
        {
            using (Container.BeginExecutionContextScope())
            {
                var outerSession = Container.GetInstance<ISession>();
                var outerRepo1 = Container.GetInstance<IRepository<AttrIndexAttribute>>();
                var outerRepo2 = Container.GetInstance<IRepository<AttrLazyLoad>>();
                var outerRepo3 = Container.GetInstance<IAttrLazyLoadRepository>();

                Assert.AreEqual(typeof(Repository<AttrIndexAttribute>), outerRepo1.GetType());
                Assert.AreEqual(typeof(AttrLazyLoadRepository), outerRepo2.GetType());
                Assert.AreEqual(typeof(AttrLazyLoadRepository), outerRepo3.GetType());

                Assert.AreEqual(outerSession, outerRepo1.GetSession());
                Assert.AreEqual(outerSession, outerRepo2.GetSession());
                Assert.AreEqual(outerSession, outerRepo3.GetSession());

                var unitOfWorkFactory = Container.GetInstance<IUnitOfWorkFactory>();
                using (var unitOfWork = unitOfWorkFactory.Create().GetUnitOfWorkImplementation())
                {
                    var repo1 = unitOfWork.GetRepository<AttrIndexAttribute>();
                    var repo2 = unitOfWork.GetRepository<AttrLazyLoad>();
                    var repo3 = unitOfWork.GetCustomRepository<IAttrLazyLoadRepository>();

                    Assert.AreEqual(typeof(Repository<AttrIndexAttribute>), repo1.GetType());
                    Assert.AreEqual(typeof(AttrLazyLoadRepository), repo2.GetType());
                    Assert.AreEqual(typeof(AttrLazyLoadRepository), repo3.GetType());
                    var session = Container.GetInstance<ISession>();

                    Assert.AreEqual(unitOfWork.DefaultSession, repo1.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, repo2.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, repo3.GetSession());
                    Assert.AreEqual(unitOfWork.DefaultSession, session);
                    Assert.AreNotEqual(outerSession, session);
                }
            }
            
        }

        [Test]
        public void RepositoriesWithinSameScopeMustBeEqual()
        {
            using (Container.BeginExecutionContextScope())
            {
                var session = Container.GetInstance<ISession>();
                var repo1 = (Repository<AttrIndexAttribute, long>)Container.GetInstance<IRepository<AttrIndexAttribute, long>>();
                var repo2 = (Repository<AttrLazyLoad, long>)Container.GetInstance<IRepository<AttrLazyLoad, long>>();
                var repo3 = Container.GetInstance<IAttrLazyLoadRepository>();
                var repo4 = Container.GetInstance<IAttrLazyLoadRepository>();
                var repo5 = Container.GetInstance<IRepository<AttrLazyLoad, long>>();

                Assert.AreNotEqual(repo1, repo2);
                Assert.AreEqual(repo2, repo3);
                Assert.AreEqual(repo3, repo4);
                Assert.AreEqual(repo2, repo5);
                Assert.AreEqual(session, repo1.GetSession());
                Assert.AreEqual(session, repo2.GetSession());
                Assert.AreEqual(session, repo3.GetSession());
                Assert.AreEqual(session, repo4.GetSession());
                Assert.AreEqual(session, repo5.GetSession());
            }
        }

        [Test]
        public void SessionWithinMultipleScopes()
        {
            ISession session1, session2;
            using (Container.BeginExecutionContextScope())
            {
                session1 = Container.GetInstance<ISession>();
                var repo1 = (Repository<AttrIndexAttribute, long>)Container.GetInstance<IRepository<AttrIndexAttribute, long>>();
                Assert.AreEqual(session1.SessionFactory, repo1.GetSession().SessionFactory);
                Assert.AreEqual(session1, repo1.GetSession());
            }
            using (Container.BeginExecutionContextScope())
            {
                session2 = Container.GetInstance<ISession>();
                var repo2 = (Repository<AttrLazyLoad, long>)Container.GetInstance<IRepository<AttrLazyLoad, long>>();
                Assert.AreEqual(session2.SessionFactory, repo2.GetSession().SessionFactory);
                Assert.AreEqual(session2, repo2.GetSession());
            }

            Assert.AreNotEqual(session1, session2);
        }

        [Test]
        public void SessionMustBeEqualInTheSameScope()
        {
            using (Container.BeginExecutionContextScope())
            {
                var session = Container.GetInstance<ISession>();
                var session2 = Container.GetInstance<ISession>();

                Assert.AreEqual(session, session2);
            }
        }

        [Test]
        public void RetieveSessionOutsideAScopeShouldThrow()
        {
            Assert.Throws<ActivationException>(() =>
            {
                var sf = Container.GetInstance<ISession>().SessionFactory;
            });
        }

        [Test]
        public void SessionsMustBeDifferentInNestedScopes()
        {
            using (Container.BeginExecutionContextScope())
            {
                var session = Container.GetInstance<ISession>();
                var filler = Container.GetAllInstances<IBusinessRule<ValidableEntity>>().First() as ValidableEntityBusinessRule;
                Assert.NotNull(filler);
                Assert.AreEqual(session, filler.Session);

                using (Container.BeginExecutionContextScope())
                {
                    var filler2 = Container.GetAllInstances(typeof(IBusinessRule<ValidableEntity>)).First() as ValidableEntityBusinessRule;
                    var session2 = Container.GetInstance<ISession>();
                    Assert.NotNull(filler2);
                    Assert.AreNotEqual(session, filler2.Session);
                    Assert.AreNotEqual(session, session2);
                }
            }
        }

        [Test]
        public void IsolationAttributeShouldWorkForUnitOfWork()
        {
            var test = Container.GetInstance<ReadCommited>();
            Assert.AreEqual(IsolationLevel.ReadCommitted, test.UnitOfWork.GetMemberValue("IsolationLevel"));
            var test2 = Container.GetInstance<Chaos>();
            Assert.AreEqual(IsolationLevel.Chaos, test2.UnitOfWork.GetMemberValue("IsolationLevel"));
        }
    }
}

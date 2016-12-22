﻿using System;
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
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Queries;
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
                var session = Container.GetInstance<ISession>();
                Assert.AreEqual(unitOfWork.DefaultSession, session);
            }
        }

        [Test]
        public void QueryHandlerMustBeScoped()
        {
            Assert.Throws<ActivationException>(() => Container.GetInstance<IQueryHandler<GetAllUsersQuery, IEnumerable<User>>>());

            using (Container.BeginExecutionContextScope())
            {
                var handler = Container.GetInstance<IQueryHandler<GetAllUsersQuery, IEnumerable<User>>>();
                Assert.AreEqual(handler, Container.GetInstance<IQueryHandler<GetAllUsersQuery, IEnumerable<User>>>());
            }
        }

        [Test]
        public void AsyncQueryHandlerMustBeScoped()
        {
            Assert.Throws<ActivationException>(() => Container.GetInstance<IAsyncQueryHandler<GetAllUsersAsyncQuery, List<User>>>());

            using (Container.BeginExecutionContextScope())
            {
                var handler = Container.GetInstance<IAsyncQueryHandler<GetAllUsersAsyncQuery, List<User>>>();
                Assert.AreEqual(handler, Container.GetInstance<IAsyncQueryHandler<GetAllUsersAsyncQuery, List<User>>>());
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
                using (var unitOfWork = Container.GetInstance<IUnitOfWorkFactory>().Create().GetUnitOfWorkImplementation())
                {
                    await Task.Delay(100);
                    
                    var session = Container.GetInstance<ISession>();
                    Assert.AreEqual(unitOfWork.DefaultSession, session);

                    var id = Thread.CurrentThread.ManagedThreadId;
                    await Task.Delay(100).ConfigureAwait(false);
                    Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);

                    session = Container.GetInstance<ISession>();
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

                var unitOfWorkFactory = Container.GetInstance<IUnitOfWorkFactory>();
                using (var unitOfWork = unitOfWorkFactory.Create().GetUnitOfWorkImplementation())
                {
                    var session = Container.GetInstance<ISession>();

                    Assert.AreEqual(unitOfWork.DefaultSession, session);
                    Assert.AreNotEqual(outerSession, session);
                }
            }
            
        }

        [Test]
        public void SessionWithinMultipleScopes()
        {
            ISession session1, session2;
            using (Container.BeginExecutionContextScope())
            {
                session1 = Container.GetInstance<ISession>();
                var sf = session1.SessionFactory;
            }
            using (Container.BeginExecutionContextScope())
            {
                session2 = Container.GetInstance<ISession>();
                var sf = session1.SessionFactory;
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
        public void UnitOfWorkShouldNotBeInjected()
        {
            Assert.Throws<ActivationException>(() =>
            {
                var wo = Container.GetInstance<IUnitOfWork>();
            });

            Assert.Throws<ActivationException>(() =>
            {
                var wo = Container.GetInstance<UnitOfWork>();
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.DataAccess.Entities;
using PowerArhitecture.Tests.DataAccess.Queries;
using PowerArhitecture.Validation;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace PowerArhitecture.Tests.DataAccess
{
    [TestFixture]
    public class QueryHandlerTests : DatabaseBaseTest
    {
        public QueryHandlerTests()
        {
            EntityAssemblies.Add(typeof(LifecycleTests).Assembly);
            TestAssemblies.Add(typeof(Entity).Assembly);
            TestAssemblies.Add(typeof(Database).Assembly);
            TestAssemblies.Add(typeof(ValidationRuleSet).Assembly);
            TestAssemblies.Add(typeof(LifecycleTests).Assembly);
        }

        [Test]
        public void GetAllUserQueryShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var handler = Container.GetInstance<IQueryHandler<GetAllUsersQuery, IEnumerable<User>>>();
                var list = handler.Handle(new GetAllUsersQuery());
                Assert.AreEqual(2, list.Count());
            }
        }

        [Test]
        public async Task GetAllUserAsyncQueryShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var handler = Container.GetInstance<IAsyncQueryHandler<GetAllUsersAsyncQuery, List<User>>>();
                var list = await handler.HandleAsync(new GetAllUsersAsyncQuery());
                Assert.AreEqual(2, list.Count);
            }
        }

        [Test]
        public void GetAllUserQueryByQueryProcessorShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var procesor = Container.GetInstance<IQueryProcessor>();
                var list = procesor.Process(new GetAllUsersQuery());
                Assert.AreEqual(2, list.Count());
            }
        }

        [Test]
        public async Task GetAllUserAsyncQueryByQueryProcessorShouldWork()
        {
            using (Container.BeginExecutionContextScope())
            {
                var procesor = Container.GetInstance<IQueryProcessor>();
                var list2 = await procesor.ProcessAsync(new GetAllUsersAsyncQuery());
                Assert.AreEqual(2, list2.Count);
            }
        }

        protected override void FillData(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            using(var trans = session.BeginTransaction())
            {
                session.Save(new User
                {
                    UserName = "user1"
                });
                session.Save(new User
                {
                    UserName = "user2"
                });
                trans.Commit();
            }
        }
    }
}

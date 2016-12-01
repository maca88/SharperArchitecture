using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Tests.WebApi.Server.Controllers;
using System.Web;
using PowerArhitecture.Tests.WebApi.Server;

namespace PowerArhitecture.Tests.WebApi
{
    [TestFixture]
    public class SingleDatabaseTests : WebApiTest<SingleDatabaseStartup>
    {
        [Test]
        public async Task QueryTest()
        {
            await ClearDatabaseStatistics();
            var result = await CallMethod<TestController, int>(o => o.GetQueryCount());
            Assert.AreEqual(0, result);
            var stats = await GetDatabaseStatistics();

            Assert.AreEqual(1, stats.SessionOpenCount);
            Assert.AreEqual(1, stats.SessionCloseCount);
            Assert.AreEqual(1, stats.TransactionCount);
            Assert.AreEqual(1, stats.SuccessfulTransactionCount);
        }

        [Test]
        public async Task BrokenQueryTest()
        {
            await ClearDatabaseStatistics();

            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await CallMethod<TestController, int>(o => o.GetBrokenQueryCount());
            });
            var stats = await GetDatabaseStatistics();

            Assert.AreEqual(1, stats.SessionOpenCount);
            Assert.AreEqual(1, stats.SessionCloseCount);
            Assert.AreEqual(1, stats.TransactionCount);
            Assert.AreEqual(0, stats.SuccessfulTransactionCount);
        }
    }
}

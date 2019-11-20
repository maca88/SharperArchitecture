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
using SharperArchitecture.Common.Helpers;
using SharperArchitecture.Tests.WebApi.Server.Controllers;
using System.Web;
using SharperArchitecture.Tests.WebApi.Server;

namespace SharperArchitecture.Tests.WebApi
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

            var response = await CallMethod<TestController>(o => o.GetBrokenQueryCount());
            Assert.IsFalse(response.IsSuccessStatusCode);

            var responseObject = await response.Content.ReadAsAsync<HttpResponseError>();
            Assert.AreEqual("Fake exception", responseObject.ExceptionMessage);
            Assert.AreEqual(typeof(InvalidOperationException), responseObject.ExceptionType);

            var stats = await GetDatabaseStatistics();

            Assert.AreEqual(1, stats.SessionOpenCount);
            Assert.AreEqual(1, stats.SessionCloseCount);
            Assert.AreEqual(1, stats.TransactionCount);
            Assert.AreEqual(0, stats.SuccessfulTransactionCount);
        }

        [Test]
        public async Task StaleExceptionRetryTest()
        {
            await ClearDatabaseStatistics();

            var response = await CallMethod<TestController>(o => o.SaveData());
            Assert.IsTrue(response.IsSuccessStatusCode);

            var stats = await GetDatabaseStatistics();

            Assert.AreEqual(2, stats.SessionOpenCount);
            Assert.AreEqual(2, stats.SessionCloseCount);
            Assert.AreEqual(2, stats.TransactionCount);
            Assert.AreEqual(1, stats.SuccessfulTransactionCount);
        }
    }
}
